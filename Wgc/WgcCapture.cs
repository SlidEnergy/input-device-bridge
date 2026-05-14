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
using tser.Properties;

namespace tser.Wgc
{
    //public static class Direct3D11Helpers
    //{
    //    public static T As<T>(this object obj)
    //    {
    //        return (T)obj;
    //    }
    //}

    public class WgcCapture : IDisposable
    {
        private GraphicsCaptureSession _session;
        private Direct3D11CaptureFramePool _framePool;

        private readonly object _lock = new();
        private Mat _latest;

        private bool _running;

        public bool IsRunning => _running;

        SharpDX.Direct3D11.Device _sharpDXDevice;

        [ComImport]
        [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IDirect3DDxgiInterfaceAccess
        {
            IntPtr GetInterface([In] ref Guid iid);
        }

        public void Start(GraphicsCaptureItem item)
        {
            if (_running) return;

            _sharpDXDevice = CreateSharpDXDevice();

            if (_sharpDXDevice == null)
                throw new InvalidOperationException("Failed to create SharpDxDevice device");

            var device = Direct3D11Helper.CreateDirect3DDeviceFromSharpDXDevice(_sharpDXDevice);

            if (device == null)
                throw new InvalidOperationException("Failed to create D3D11 device");

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

        public static SharpDX.Direct3D11.Device CreateSharpDXDevice()
        {
            return new SharpDX.Direct3D11.Device(
                                SharpDX.Direct3D.DriverType.Hardware,
                            SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport);
        }

        private SharpDX.DXGI.Resource CreateResource(IDirect3DSurface surface)
        {
            IntPtr inspectablePtr = WinRT.MarshalInspectable<IDirect3DSurface>
                .FromManaged(surface);

            Guid guid = typeof(IDirect3DDxgiInterfaceAccess).GUID;

            Marshal.QueryInterface(
                inspectablePtr,
                ref guid,
                out IntPtr accessPtr);

            var access = Marshal.GetObjectForIUnknown(accessPtr)
                as IDirect3DDxgiInterfaceAccess;

            var texPtr = access.GetInterface(typeof(SharpDX.DXGI.Resource).GUID);

            return new SharpDX.DXGI.Resource(texPtr);
        }

        private void OnFrame(Direct3D11CaptureFramePool sender, object args)
        {
            using var frame = sender.TryGetNextFrame();

            if (frame == null)
                return;

            //var surface = frame.Surface;

            //IntPtr inspectablePtr = WinRT.MarshalInspectable<IDirect3DSurface>
            //    .FromManaged(surface);

            //Guid guid = typeof(IDirect3DDxgiInterfaceAccess).GUID;

            //Marshal.QueryInterface(
            //    inspectablePtr,
            //    ref guid,
            //    out IntPtr accessPtr);

            //var access = Marshal.GetObjectForIUnknown(accessPtr)
            //    as IDirect3DDxgiInterfaceAccess;

            //var texPtr = access.GetInterface(typeof(SharpDX.DXGI.Resource).GUID);

            //using var resource = new SharpDX.DXGI.Resource(texPtr);

            using var resource = CreateResource(frame.Surface);

            using var texture = CreateStagingTexture(resource);

            //using var texture = resource.QueryInterface<SharpDX.Direct3D11.Texture2D>();

            //var desc = texture.Description;

            //// staging texture для CPU чтения
            //using var staging = new SharpDX.Direct3D11.Texture2D(_sharpDXDevice,
            //    new SharpDX.Direct3D11.Texture2DDescription
            //    {
            //        Width = desc.Width,
            //        Height = desc.Height,
            //        MipLevels = 1,
            //        ArraySize = 1,
            //        Format = desc.Format,
            //        SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            //        Usage = SharpDX.Direct3D11.ResourceUsage.Staging,
            //        BindFlags = SharpDX.Direct3D11.BindFlags.None,
            //        CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read,
            //        OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None
            //    });

            //// Копируем GPU -> staging
            //_sharpDXDevice.ImmediateContext.CopyResource(texture, staging);

            using var mat = TextureToMat(texture);

            lock (_lock)
            {
                _latest?.Dispose();
                _latest = mat.Clone();
            }
        }

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


        private Mat TextureToMat(Texture2D texture)
        {
            var desc = texture.Description;

            // Map
            var db = _sharpDXDevice.ImmediateContext.MapSubresource(
                texture,
                0,
                SharpDX.Direct3D11.MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None);

            try
            {
                var bgra = Mat.FromPixelData(
                    desc.Height,
                    desc.Width,
                    MatType.CV_8UC4,
                    db.DataPointer,
                    db.RowPitch);

                return bgra;
            }
            finally
            {
                _sharpDXDevice.ImmediateContext.UnmapSubresource(texture, 0);
            }
        }

        private Texture2D CreateStagingTexture(SharpDX.DXGI.Resource resource)
        {
            using var texture = resource.QueryInterface<SharpDX.Direct3D11.Texture2D>();

            var desc = texture.Description;

            // staging texture для CPU чтения
            var staging = new SharpDX.Direct3D11.Texture2D(_sharpDXDevice,
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

            return staging;
        }
    }
}
