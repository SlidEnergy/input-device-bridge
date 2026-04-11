using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    /// <summary>
    /// keyboard Hook Process called hooked to and called by Windows.
    /// </summary>
    /// <param name="nCode">A code the hook procedure uses to determine 
    /// how to process the message.</param>
    /// <param name="wParam">The virtual-key code of the key that generated 
    /// the keystroke message.</param>
    /// <param name="lParam">The repeat count, scan code, extended-key flag, 
    /// context code, previous key-state flag,</param>                                                       
    /// <returns></returns>
    public delegate IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam);
    public delegate IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Keyboard Hook Event called by <typeparamref name="KeyboardHook"/>.
    /// </summary>
    /// <param name="wParam">The virtual-key code of the key that generated 
    /// the keystroke message.</param>
    /// <param name="lParam">The repeat count, scan code, extended-key flag, 
    /// context code, previous key-state flag,</param>     
    public delegate bool KeyboardHookEvent(int wParam, KBDLLHOOKSTRUCT lParam);
    public delegate bool MouseHookEvent(int wParam, MSLLHOOKSTRUCT lParam);

    /// <summary>
    /// Wrapper class for a Win32 Keyboard event hook.
    /// </summary>
    public class Kmh
    {
        //private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //#############################################################       
        #region [# Win32 Constants #]

        /// <summary>
        /// The WH_KEYBOARD_LL hook enables you to monitor keyboard 
        /// input events about to be posted in a thread input queue. 
        /// </summary>
        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644959%28v=vs.85%29.aspx#wh_keyboard_llhook"/>
        public static readonly int WH_KEYBOARD_LL = 13;
        /// <summary>
        ///  
        /// </summary>
        public static readonly int WM_KEYDOWN = 0x100;
        /// <summary>
        /// 
        /// </summary>
        public static readonly int WM_KEYUP = 0x101;
        /// <summary>
        /// 
        /// </summary>
        public static readonly int WM_SYSKEYDOWN = 0x104;
        /// <summary>
        /// 
        /// </summary>
        public static readonly int WM_SYSKEYUP = 0x105;
        /// <summary>
        /// 
        /// </summary>
        public static readonly int VK_PACKAGE = 0xE7;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int VK_BACK = 0x08;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int VK_ENTER = 0x0D;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int VK_LALT = 0xA4;

        private const int WH_MOUSE_LL = 14;
        private const int WM_XBUTTONDOWN = 0x020B;
        private const int WM_XBUTTONUP = 0x020C;
        private const int XBUTTON2 = 0x0002;

        //public static IntPtr hInstance = LoadLibrary("User32");

        #endregion
        //#############################################################
        #region [# Properties #]

        // Мышиный хук
        private static IntPtr _mouseHook = IntPtr.Zero;
        private MouseHookProc _mouseProc;

        // Клавиатурный хук
        private static IntPtr _keyboardHook = IntPtr.Zero;
        private KeyboardHookProc _keyboardProc;


        #endregion
        //#############################################################  
        #region [# Flags #]        

        //public bool Hooked { get { return keyHooked; } }
        private volatile bool keyHooked = false;
        private volatile bool mouseHooked = false;
        public bool LeftShiftHeld { get { return bLeftShiftHeld; } }
        private volatile bool bLeftShiftHeld = false;
        public bool RightShiftHeld { get { return bRightShiftHeld; } }
        private volatile bool bRightShiftHeld = false;
        public bool ShiftHeld { get { return bShiftHeld; } }
        private volatile bool bShiftHeld = false;
        public bool AltHeld { get { return bAltHeld; } }
        private volatile bool bAltHeld = false;
        public bool CtrlHeld { get { return bCtrlHeld; } }
        private volatile bool bCtrlHeld = false;

        #endregion
        //#############################################################
        #region [# Events #]

        /// <summary>
        /// KeyDown event for when a key is pressed down.
        /// </summary>
        public event KeyboardHookEvent KeyDown;
        /// <summary>
        /// KeyUp event for then the key is released.
        /// </summary>
        public event KeyboardHookEvent KeyUp;

        /// <summary>
        /// KeyDown event for when a key is pressed down.
        /// </summary>
        public event MouseHookEvent MouseDown;
        /// <summary>
        /// KeyUp event for then the key is released.
        /// </summary>
        public event MouseHookEvent MouseUp;

        #endregion
        //#############################################################
        #region [# Construction / Destruction #]

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="autoHook"></param>
        public Kmh(bool autoHook = false) 
        { 
            if (autoHook) 
                hook();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Kmh() { unhook(); }

        #endregion
        //#############################################################
        #region [# Hooks #]

        /// <summary>
        /// Hooks keyboard event process <paramref name="_hookProc"/> from Windows.
        /// </summary>
        public virtual void hook()
        {
            var moduleHandle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            // Мышиный хук
            _mouseProc = MouseHookCallback;
            _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);

            // Клавиатурный хук
            _keyboardProc = KeyboardHookCallback;
            _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, moduleHandle, 0);

            //Set bHooked to true if successful.
            keyHooked = (_keyboardHook != null);
            mouseHooked = (_mouseHook != null);
        }

        /// <summary>
        /// Unhooks the keyboard event process from Windows.
        /// </summary>
        public virtual void unhook()
        {
            //Call library unhook function
            UnhookWindowsHookEx(_mouseHook);
            UnhookWindowsHookEx(_keyboardHook);
            keyHooked = false;
            mouseHooked = false;
        }

        /// <summary>
        /// Overridable function called by the hooked procedure
        /// function <typeparamref name="_hookProc"/>.
        /// </summary>
        /// <param name="nCode">A code the hook procedure uses to determine 
        /// how to process the message.</param>
        /// <param name="wParam">The virtual-key code of the key that generated 
        /// the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, 
        /// context code, previous key-state flag,</param>
        /// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644984%28v=vs.85%29.aspx"/>
        /// <returns></returns>
        public virtual IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var handled = false;
                var data = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                {
                    handled = KeyDown((int)wParam, data);
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                {
                    handled = KeyUp((int)wParam, data);
                }

                if (handled && (wParam == WM_KEYDOWN || wParam == WM_KEYUP))
                    return (IntPtr)1;
            }

            return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        public virtual IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (int)wParam.ToInt64();
                var handled = false;
                var data = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                if (msg == WM_XBUTTONDOWN)
                {
                    handled = MouseDown((int)wParam, data);
                }
                if (msg == WM_XBUTTONUP)
                {
                    handled = MouseUp((int)wParam, data);
                }
                //else if (((int)wParam.ToInt64() == WM_XBUTTONUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                //{
                //    handled = KeyUp(wParam, lParam);
                //}

                if (handled && (wParam == WM_XBUTTONDOWN || wParam == WM_XBUTTONUP))
                    return (IntPtr)1;
            }

            return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        #endregion
        //#############################################################
        #region [# DLL Imports #]

        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        ///// <returns>A handle to the library</returns>
        //[DllImport("kernel32.dll")]
        //static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
