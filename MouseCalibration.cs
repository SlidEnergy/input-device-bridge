using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace tser
{
    internal class MouseCalibration
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private readonly InputSimulator sim;

        public MouseCalibration(InputSimulator inputSimulator)
        {
            sim = inputSimulator;
        }

        public (double scaleX, double scaleY) Calibrate(int testHid = 1000)
        {
            GetCursorPos(out var start);

            // движение вправо-вниз
            sim.MouseMove(testHid, testHid);

            Thread.Sleep(5000); // дать ОС догнать курсор

            GetCursorPos(out var end);

            double movedX = end.X - start.X;
            double movedY = end.Y - start.Y;

            double scaleX = movedX / (double)testHid;
            double scaleY = movedY / (double)testHid;

            return (scaleX, scaleY);
        }
    }
}