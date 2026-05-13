using OpenCvSharp;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace tser.Wgc
{
    public static class Direct3D11Helpers
    {
        public static T As<T>(this object obj)
        {
            return (T)obj;
        }
    }

    public class WgcCapture : IDisposable
    {
        private GraphicsCaptureSession _session;
        private Direct3D11CaptureFramePool _framePool;

        private readonly object _lock = new();
        private Mat _latest;

        private bool _running;

        public bool IsRunning => _running;


        private IDirect3DDevice _device;
        private GraphicsCaptureItem _item;
        SharpDX.Direct3D11.Device _sharpDXDevice;

        public void Start(GraphicsCaptureItem item, IDirect3DDevice device, SharpDX.Direct3D11.Device d3dDevice)
        {
            if (_running) return;

            _device = device;
            _item = item;
            _sharpDXDevice = d3dDevice;

            _framePool = Direct3D11CaptureFramePool.Create(
                device,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2,
                item.Size);

            _session = _framePool.CreateCaptureSession(item);

            _framePool.FrameArrived += OnFrame;

            _session.StartCapture();

            _running = true;
        }

        [ComImport]
        [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IDirect3DDxgiInterfaceAccess
        {
            IntPtr GetInterface([In] ref Guid iid);
        }

        private void OnFrame(Direct3D11CaptureFramePool sender, object args)
        {
            using var frame = sender.TryGetNextFrame();

            if (frame == null)
                return;

            var surface = frame.Surface;

            IntPtr inspectablePtr = WinRT.MarshalInspectable<IDirect3DSurface>
                .FromManaged(surface);

                Guid guid = typeof(IDirect3DDxgiInterfaceAccess).GUID;

                Marshal.QueryInterface(
                    inspectablePtr,
                    ref guid,
                    out IntPtr accessPtr);

                    var access = Marshal.GetObjectForIUnknown(accessPtr)
                        as IDirect3DDxgiInterfaceAccess;


                    // Получаем DXGI Texture2D
                    //var access = surface.As<IDirect3DDxgiInterfaceAccess>();

                    var texPtr = access.GetInterface(
                typeof(SharpDX.DXGI.Resource).GUID);

            using var resource = new SharpDX.DXGI.Resource(texPtr);

            using var texture = resource.QueryInterface<SharpDX.Direct3D11.Texture2D>();

            var desc = texture.Description;

            // staging texture для CPU чтения
            using var staging = new SharpDX.Direct3D11.Texture2D(_sharpDXDevice,
                new SharpDX.Direct3D11.Texture2DDescription
                {
                    Width = desc.Width,
                    Height = desc.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = desc.Format,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                    Usage = SharpDX.Direct3D11.ResourceUsage.Staging,
                    BindFlags = SharpDX.Direct3D11.BindFlags.None,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None
                });

            // Копируем GPU -> staging
            _sharpDXDevice.ImmediateContext.CopyResource(texture, staging);

            // Map
            var db = _sharpDXDevice.ImmediateContext.MapSubresource(
                staging,
                0,
                SharpDX.Direct3D11.MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None);

            try
            {
                using var bgra = Mat.FromPixelData(
                    desc.Height,
                    desc.Width,
                    MatType.CV_8UC4,
                    db.DataPointer,
                    db.RowPitch);

                // Копия в managed Mat
                var copy = bgra.Clone();

                lock (_lock)
                {
                    _latest?.Dispose();
                    _latest = copy;
                }
            }
            finally
            {
                _sharpDXDevice.ImmediateContext.UnmapSubresource(staging, 0);
            }
        }

        //private void OnFrame(Direct3D11CaptureFramePool sender, object args)
        //{
        //    using var frame = sender.TryGetNextFrame();

        //    if (frame == null)
        //    {
        //        Debug.WriteLine("Frame is null — skipping");
        //        return;
        //    }

        //    var surface = frame.Surface;

        //    if (surface == null)
        //    {
        //        Debug.WriteLine("Surface is null — skipping");
        //        return;
        //    }

        //    // Проверяем размер поверхности
        //    var desc = surface.Description;
        //    Debug.WriteLine($"Frame size: {desc.Width}x{desc.Height}");

        //    var mat = DxgiToMat(surface); // слой 2 → слой 3

        //    lock (_lock)
        //    {
        //        _latest?.Dispose();
        //        _latest = mat;
        //    }
        //}

        public Mat GetFrame()
        {
            lock (_lock)
                return _latest?.Clone();
        }


        public async Task<Mat> WaitForFrame(WgcCapture capture, int timeoutMs = 5000)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                var frame = capture.GetFrame();
                if (frame != null && !frame.Empty())
                    return frame;
                await Task.Delay(50);
            }
            return null;
        }

        public void Stop()
        {
            if (!_running) return;

            _framePool.FrameArrived -= OnFrame;
            _session?.Dispose();//?.Close();
            _framePool?.Dispose();

            _session = null;
            _framePool = null;

            lock (_lock)
            {
                _latest?.Dispose();
                _latest = null;
            }

            _running = false;
        }

        public void Dispose() => Stop();


        private Mat DxgiToMat(IDirect3DSurface surface)
        {
            var desc = surface.Description;
            int width = (int)desc.Width;
            int height = (int)desc.Height;

            if (width <= 0 || height <= 0)
            {
                Console.WriteLine("Invalid surface dimensions");
                return null;
            }

            Console.WriteLine($"Creating staging texture: {width}x{height}");

            Texture2D stagingTexture = null;
            try
            {
                stagingTexture = CreateStagingTexture(width, height);

                CopySurfaceToTexture(surface, stagingTexture);

                var mappedResource = _sharpDXDevice.ImmediateContext.MapSubresource(
                    stagingTexture,
                    0,
                    MapMode.Read,
                    SharpDX.Direct3D11.MapFlags.None);

                try
                {
                    var firstPixelData = new byte[4];
                    Marshal.Copy(mappedResource.DataPointer, firstPixelData, 0, 4);
                    Console.WriteLine($"Staging texture first pixel: R={firstPixelData[2]}, G={firstPixelData[1]}, B={firstPixelData[0]}, A={firstPixelData[3]}");

                    if (firstPixelData[0] == 0 && firstPixelData[1] == 0 &&
                        firstPixelData[2] == 0 && firstPixelData[3] == 0)
                    {
                        Console.WriteLine("WARNING: Staging texture is empty — copy failed");
                        return null;
                    }

                    var mat = new Mat(height, width, MatType.CV_8UC4);
                    var dataPtr = mat.Data;
                    var rowPitch = mappedResource.RowPitch;
                    var sourcePtr = mappedResource.DataPointer;
                    int matRowSize = width * 4;

                    for (int y = 0; y < height; y++)
                    {
                        Utilities.CopyMemory(
                            dataPtr + y * matRowSize,
                    sourcePtr + y * rowPitch,
                    matRowSize
                );
                    }

                    return mat;
                }
                finally
                {
                    _sharpDXDevice.ImmediateContext.UnmapSubresource(stagingTexture, 0);
                }
            }
            catch (SharpDXException ex)
            {
                Console.WriteLine($"DxgiToMat failed: {ex.Message}");
                return null;
            }
            finally
            {
                stagingTexture?.Dispose();
            }
        }

        private Texture2D CreateStagingTexture(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException($"Invalid dimensions: {width}x{height}");

            var stagingDesc = new Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            };

            try
            {
                return new Texture2D(_sharpDXDevice, stagingDesc);
            }
            catch (SharpDXException ex) //when (ex.HResult == SharpDX.ResultCode.InvalidArgument.ResultCode)
            {
                Console.WriteLine($"Invalid staging texture parameters: {width}x{height}, Format: {stagingDesc.Format}");
                // Пробуем альтернативный формат
                stagingDesc.Format = Format.R8G8B8A8_UNorm;
                try
                {
                    return new Texture2D(_sharpDXDevice, stagingDesc);
                }
                catch
                {
                    throw new InvalidOperationException("Failed to create staging texture with any supported format");
                }
            }
        }



        private void CopySurfaceToTexture(IDirect3DSurface sourceSurface, Texture2D destinationTexture)
        {
            var nativeSurface = GetNativeSurface(sourceSurface);
            if (nativeSurface == null)
            {
                Console.WriteLine("Failed to get native surface");
                return;
            }

            try
            {
                // 1. Пытаемся получить Texture2D напрямую
                var sourceTexture = TryGetTextureFromSurface(nativeSurface);
                if (sourceTexture != null)
                {
                    try
                    {
                        _sharpDXDevice.ImmediateContext.CopyResource(sourceTexture, destinationTexture);
                        Console.WriteLine("Successfully copied via direct CopyResource");
                        return;
                    }
                    finally
                    {
                        sourceTexture.Dispose();
                    }
                }

                // 2. Если не получилось — используем staging texture
                CopySurfaceToTextureViaStaging(nativeSurface, destinationTexture);
            }
            finally
            {
                nativeSurface.Dispose();
            }
        }




        private SharpDX.Direct3D11.Texture2D TryGetTextureFromSurface(SharpDX.DXGI.Surface nativeSurface)
        {
            try
            {
                return nativeSurface.QueryInterfaceOrNull<SharpDX.Direct3D11.Texture2D>();
            }
            catch (SharpDXException ex) //when (ex.HResult == SharpDX.ResultCode.NoInterface.ResultCode)
            {
                Console.WriteLine("Surface does not support Texture2D interface");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error getting Texture2D: {ex.Message}");
                return null;
            }
        }



        private void CopySurfaceToTextureViaStaging(SharpDX.DXGI.Surface nativeSurface, Texture2D destinationTexture)
        {
            try
            {
                var desc = nativeSurface.Description;

                // Получаем Texture2D из Surface
                var sourceTexture = TryGetTextureFromSurface(nativeSurface);
                if (sourceTexture == null)
                {
                    throw new InvalidOperationException("Cannot convert Surface to Texture2D");
                }

                // Создаём staging texture для чтения данных
                var stagingDesc = new Texture2DDescription
                {
                    Width = (int)desc.Width,
                    Height = (int)desc.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Staging,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.Read,
                    OptionFlags = ResourceOptionFlags.None
                };

                using var stagingTexture = new Texture2D(_sharpDXDevice, stagingDesc);

                // Копируем из sourceTexture (уже Resource) в stagingTexture
                _sharpDXDevice.ImmediateContext.CopySubresourceRegion(
                    source: sourceTexture,
                    sourceSubresource: 0,
                    sourceRegion: null,
                    destination: stagingTexture,
                    destinationSubResource: 0,
                    dstX: 0, dstY: 0, dstZ: 0
                );

                // Теперь копируем из stagingTexture в destinationTexture
                _sharpDXDevice.ImmediateContext.CopyResource(stagingTexture, destinationTexture);

                Console.WriteLine($"Successfully copied via staging: {desc.Width}x{desc.Height}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CopySurfaceToTextureViaStaging failed: {ex.Message}");
                throw;
            }
        }





        //private void CopySurfaceToTextureFallback(IDirect3DSurface sourceSurface, Texture2D destinationTexture)
        //{
        //    var desc = sourceSurface.Description;
        //    int width = (int)desc.Width;
        //    int height = (int)desc.Height;

        //    // Создаём описание SwapChain
        //    var swapChainDesc = new SwapChainDescription1
        //    {
        //        Width = width,
        //        Height = height,
        //        Format = Format.B8G8R8A8_UNorm,
        //        Stereo = false,
        //        SampleDescription = new SampleDescription(1, 0),
        //       // BufferUsage = Usage.RenderTargetOutput,
        //        BufferCount = 2,
        //        Scaling = Scaling.Stretch,
        //        SwapEffect = SwapEffect.FlipSequential,
        //        AlphaMode = AlphaMode.Ignore,
        //        Flags = 0
        //    };

        //    // Создаём временный SwapChain
        //    using var factory2 = new SharpDX.DXGI.Factory2();
        //    using var swapChain1 = new SharpDX.DXGI.SwapChain1(factory2, _sharpDXDevice, ref swapChainDesc);

        //    // Копируем данные через SwapChain
        //    using (var backBuffer = swapChain1.GetBackBuffer<Texture2D>(0))
        //    {
        //        _sharpDXDevice.ImmediateContext.CopyResource(backBuffer, destinationTexture);
        //    }
        //}


        private SharpDX.DXGI.Surface GetNativeSurface(IDirect3DSurface surface)
        {
            try
            {
                var unknown = Marshal.GetIUnknownForObject(surface);
                try
                {
                    return new SharpDX.DXGI.Surface(unknown);
                }
                finally
                {
                    Marshal.Release(unknown);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetNativeSurface failed: {ex.Message}");
                return null;
            }
        }




    }
}
