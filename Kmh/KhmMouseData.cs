using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;         // координаты
        public uint mouseData;   // дополнительные данные (высшее слово — XButton)
        public uint flags;       // флаги, содержит LLMHF_INJECTED и т.д.
        public uint time;        // время
        public UIntPtr dwExtraInfo; // pointer-sized дополнительная инфа
    }
}
