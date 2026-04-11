using System.Runtime.InteropServices;

namespace tser
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;      // виртуальный код
        public uint scanCode;    // скан-код
        public uint flags;       // флаги (например LLKHF_INJECTED = 0x10)
        public uint time;        // время
        public UIntPtr dwExtraInfo; // pointer-sized
    }
}
