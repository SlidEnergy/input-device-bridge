using Microsoft.UI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using WinRT;
//using System;
//using System.Runtime.InteropServices;
//using Windows.Graphics.Capture;
//using Windows.Graphics.DirectX.Direct3D11;
//using WinRT;
//using SharpDX.Direct3D11;
//using SharpDX.DXGI;
using Windows.Graphics.Capture;
using WinRT.Interop;
using SharpDX.DXGI;
using SharpDX;

namespace tser.Wgc
{
    public static class WgcBootstrap
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [ComImport]
        [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        interface IGraphicsCaptureItemInterop
        {
            IntPtr CreateForWindow(
                [In] IntPtr window,
                in Guid iid);

            IntPtr CreateForMonitor(
                [In] IntPtr monitor,
                in Guid iid);
        }
        static readonly Guid GraphicsCaptureItemGuid = new Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760");

        public static GraphicsCaptureItem CreateItemForWindow(IntPtr hwnd)
        {
            var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
            var itemPointer = interop.CreateForWindow(hwnd, GraphicsCaptureItemGuid);
            var item = GraphicsCaptureItem.FromAbi(itemPointer);
            Marshal.Release(itemPointer);

            return item;
        }

        public static GraphicsCaptureItem CreateFromHwnd1(IntPtr hwnd)
        {
            var activationFactory = Windows.Graphics.Capture.GraphicsCaptureItem.As<IActivationFactory>();
            var interop = (IGraphicsCaptureItemInterop)activationFactory;

            Guid iid = typeof(GraphicsCaptureItem).GUID;

            IntPtr ptr = interop.CreateForWindow(hwnd, ref iid);

            return System.Runtime.InteropServices.Marshal
                .GetObjectForIUnknown(ptr) as GraphicsCaptureItem;
        }

        [DllImport("combase.dll", PreserveSig = true)]
        private static extern int RoGetActivationFactory(
            IntPtr activatableClassId,
            ref Guid iid,
            out IntPtr factory);

        public static GraphicsCaptureItem CreateFromHwnd2(IntPtr hwnd)
        {
            Guid iid = typeof(IGraphicsCaptureItemInterop).GUID;

            IntPtr factoryPtr;

            var hstring = WinRTInterop.StringToHString(
                "Windows.Graphics.Capture.GraphicsCaptureItem"
            );

            RoGetActivationFactory(hstring, ref iid, out factoryPtr);

            var factory = (IGraphicsCaptureItemInterop)
                Marshal.GetObjectForIUnknown(factoryPtr);

            var itemIid = typeof(GraphicsCaptureItem).GUID;

            var itemPtr = factory.CreateForWindow(hwnd, ref itemIid);

            return Marshal.GetObjectForIUnknown(itemPtr) as GraphicsCaptureItem;
        }

        public static IDirect3DDevice CreateDevice()
        {
            var device = new SharpDX.Direct3D11.Device(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport);

            using var dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device1>();

            return CreateDirect3DDeviceFromDXGIDevice(dxgiDevice.NativePointer);
        }

        [DllImport("Windows.Graphics.Capture.Interop.dll", EntryPoint = "CreateDirect3D11DeviceFromDXGIDevice")]
        static extern uint CreateDirect3D11DeviceFromDXGIDevice(
            IntPtr dxgiDevice,
            out IntPtr graphicsDevice);

        static IDirect3DDevice CreateDirect3DDeviceFromDXGIDevice(IntPtr dxgiDevice)
        {
            IDirect3DDevice device = null;

            var hr = CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice, out var pUnknown);

            if (hr == 0)
            {
                device = MarshalInterface<IDirect3DDevice>.FromAbi(pUnknown);
                Marshal.Release(pUnknown);
            }

            return device;
            //return Marshal.GetObjectForIUnknown(ptr) as IDirect3DDevice;
        }
    }



internal static class WinRTInterop
    {
        [DllImport("combase.dll")]
        internal static extern int WindowsCreateString(
            [MarshalAs(UnmanagedType.LPWStr)] string sourceString,
            int length,
            out IntPtr hstring);

        public static IntPtr StringToHString(string s)
        {
            WindowsCreateString(s, s.Length, out var h);
            return h;
        }
    }
}
