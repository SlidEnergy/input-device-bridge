using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace tser
{
    internal class Mover : IMover
    {
        private readonly InputSimulator _sim;
        private readonly Random _rnd = new Random();

        public Mover(InputSimulator sim)
        {
            _sim = sim;
        }

        /// <summary>
        /// Плавное перемещение мыши к координатам (toX, toY)
        /// </summary>
        //public void MoveSmooth(int toX, int toY)
        //{
        //    var from = Cursor.Position;

        //    double distance = Math.Sqrt(Math.Pow(toX - from.X, 2) + Math.Pow(toY - from.Y, 2));

        //    double baseSpeed = 5;
        //    double duration = Math.Max(20, distance / baseSpeed);
        //    int steps = (int)Math.Clamp(distance / 3, 20, 200);

        //    double stepTime = duration / steps;

        //    double prevX = from.X;
        //    double prevY = from.Y;

        //    for (int i = 1; i <= steps; i++)
        //    {
        //        double jitterX = (_rnd.NextDouble() * 2 - 1) * 0.5;
        //        double jitterY = (_rnd.NextDouble() * 2 - 1) * 0.5;

        //        double t = (double)i / steps;
        //        t = 0.5 - Math.Cos(t * Math.PI) / 2;

        //        double curX = from.X + (toX - from.X) * t + jitterX;
        //        double curY = from.Y + (toY - from.Y) * t + jitterY;

        //        int dx = (int)Math.Round(curX - prevX);
        //        int dy = (int)Math.Round(curY - prevY);

        //        prevX = curX;
        //        prevY = curY;

        //        SendMoveClamped(dx, dy);

        //        //Thread.Sleep((int)stepTime);
        //    }

        //    // добиваем остаток
        //    int finalDx = toX - (int)Math.Round(prevX);
        //    int finalDy = toY - (int)Math.Round(prevY);

        //    SendMoveClamped(finalDx, finalDy);
        //}

        //private double _vx;
        //private double _vy;

        //public void Init(int startX, int startY)
        //{
        //    _vx = startX;
        //    _vy = startY;
        //}

        //public void MoveSmooth(int toX, int toY)
        //{
        //    double fromX = _vx;
        //    double fromY = _vy;

        //    double dxTotal = toX - fromX;
        //    double dyTotal = toY - fromY;

        //    double distance = Math.Sqrt(dxTotal * dxTotal + dyTotal * dyTotal);

        //    int steps = (int)Math.Clamp(distance / 8.0, 15, 60);

        //    double prevX = fromX;
        //    double prevY = fromY;

        //    double accX = 0;
        //    double accY = 0;

        //    for (int i = 1; i <= steps; i++)
        //    {
        //        double t = (double)i / steps;
        //        t = 0.5 - Math.Cos(t * Math.PI) / 2; // smooth easing

        //        double curX = fromX + dxTotal * t;
        //        double curY = fromY + dyTotal * t;

        //        double dx = curX - prevX;
        //        double dy = curY - prevY;

        //        prevX = curX;
        //        prevY = curY;

        //        accX += dx;
        //        accY += dy;

        //        int ix = (int)Math.Round(accX);
        //        int iy = (int)Math.Round(accY);

        //        if (ix != 0 || iy != 0)
        //        {
        //            SendMove(ix, iy);

        //            accX -= ix;
        //            accY -= iy;

        //            _vx += ix;
        //            _vy += iy;
        //        }
        //    }

        //    // финальная синхронизация
        //    _vx = toX;
        //    _vy = toY;
        //}

        //public void MoveSmooth(int toX, int toY)
        //{
        //    SetCalibration(Form1.ScaleX, Form1.ScaleY);

        //    GetCursorPos(out var p);

        //    int dx = (int)((toX - p.X) / _scaleX);
        //    int dy = (int)((toY - p.Y) / _scaleY);

        //    int steps = 20;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        int stepX = dx / steps;
        //        int stepY = dy / steps;

        //        SendMove(stepX, stepY);

        //        Thread.Sleep(1);
        //    }
        //}

        //public void MoveSmooth(int toX, int toY)
        //{
        //    GetCursorPos(out var p);

        //    int dx = toX - p.X;
        //    int dy = toY - p.Y;

        //    int steps = Math.Clamp(Math.Abs(dx) / 10 + 10, 5, 30);

        //    double stepX = dx / (double)steps;
        //    double stepY = dy / (double)steps;

        //    double curX = 0;
        //    double curY = 0;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        int moveX = (int)Math.Round(stepX - curX);
        //        int moveY = (int)Math.Round(stepY - curY);

        //        curX += moveX - stepX;
        //        curY += moveY - stepY;

        //        // clamp HID range safety
        //        //moveX = Math.Clamp(moveX, -127, 127);
        //        //moveY = Math.Clamp(moveY, -127, 127);

        //        _sim.MouseMove(moveX, moveY);

        //        Thread.Sleep(1);
        //    }
        //}

        //public void MoveSmooth(int toX, int toY)
        //{
        //    GetCursorPos(out var p);

        //    double dx = toX - p.X;
        //    double dy = toY - p.Y;

        //    int steps = Math.Clamp(Math.Abs((int)dx) / 8 + 5, 5, 30);

        //    double stepX = dx / steps;
        //    double stepY = dy / steps;

        //    double accX = 0;
        //    double accY = 0;

        //    for (int i = 0; i < steps; i++)
        //    {
        //        accX += stepX;
        //        accY += stepY;

        //        int moveX = (int)Math.Round(accX);
        //        int moveY = (int)Math.Round(accY);

        //        accX -= moveX;
        //        accY -= moveY;

        //        _sim.MouseMove(moveX, moveY);

        //        Thread.Sleep(1);
        //    }
        //}

        public void MoveSmooth(int toX, int toY)
        {
            GetCursorPos(out var p);

            double dx = toX - p.X;
            double dy = toY - p.Y;

            double distance = Math.Sqrt(dx * dx + dy * dy);

            int duration = Math.Clamp((int)(distance * 1.2), 12, 90);
            int steps = Math.Clamp(duration / 5, 4, 25);

            double accX = 0;
            double accY = 0;

            Random rnd = new Random();

            for (int i = 0; i < steps; i++)
            {
                double t = (double)i / (steps - 1);

                // smooth interpolation
                double ease = t * t * (3 - 2 * t);

                double targetX = dx * ease;
                double targetY = dy * ease;

                double stepX = targetX - accX;
                double stepY = targetY - accY;

                accX += stepX;
                accY += stepY;

                // human imperfections
                stepX += (rnd.NextDouble() - 0.5) * 0.6;
                stepY += (rnd.NextDouble() - 0.5) * 0.6;

                _sim.MouseMove((int)Math.Round(stepX), (int)Math.Round(stepY));

                Thread.Sleep(duration / steps);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        private struct POINT
        {
            public int X;
            public int Y;
        }

        ///// <summary>
        ///// Плавное перемещение мыши к координатам (toX, toY)
        ///// </summary>
        //public void MoveSmooth(int toX, int toY)
        //{
        //    var from = Cursor.Position;

        //    double distance = Math.Sqrt(Math.Pow(toX - from.X, 2) + Math.Pow(toY - from.Y, 2));

        //    // Настройка параметров движения в зависимости от расстояния
        //    double baseSpeed = 5; // пикселей за миллисекунду (чем меньше — тем медленнее)
        //    double duration = Math.Max(20, distance / baseSpeed); // минимальная длительность 50 мс
        //    int steps = (int)Math.Clamp(distance / 3, 20, 200);   // шаги: 20–200, в зависимости от длины

        //    double stepTime = duration / steps;

        //    for (int i = 1; i <= steps; i++)
        //    {
        //        // Добавляем небольшую "человеческую" погрешность
        //        double jitterX = (_rnd.NextDouble() * 2 - 1) * 0.5;
        //        double jitterY = (_rnd.NextDouble() * 2 - 1) * 0.5;

        //        double t = (double)i / steps;
        //        // Плавная интерполяция (ускорение в начале и замедление в конце)
        //        t = 0.5 - Math.Cos(t * Math.PI) / 2;

        //        double x = from.X + (toX - from.X) * t + jitterX;
        //        double y = from.Y + (toY - from.Y) * t + jitterY;

        //        _sim.MoveMouseToPositionOnVirtualDesktop(
        //            x * 65535 / Screen.PrimaryScreen.Bounds.Width,
        //            y * 65535 / Screen.PrimaryScreen.Bounds.Height);

        //        Thread.Sleep((int)stepTime);
        //    }

        //    // Гарантируем точное попадание в целевую позицию
        //    _sim.MoveMouseToPositionOnVirtualDesktop(
        //        toX * 65535 / Screen.PrimaryScreen.Bounds.Width,
        //        toY * 65535 / Screen.PrimaryScreen.Bounds.Height);
        //}

        /// <summary>
        /// Плавно переместить мышь и кликнуть левой кнопкой
        /// </summary>
        public async Task MoveAndClick(int toX, int toY)
        {
            MoveSmooth(toX, toY);
            //await Task.Delay(100);
            _sim.LeftButtonClick();
            await Task.Delay(100);
        }

        public async Task MoveAndClick(int x1, int y1, int x2, int y2)
        {
            // Определяем границы прямоугольника
            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            // Выбираем случайную точку внутри прямоугольника
            int randomX = _rnd.Next(minX, maxX + 1);
            int randomY = _rnd.Next(minY, maxY + 1);

            // Вызываем первый метод
            await MoveAndClick(randomX, randomY);
        }

        public async Task MoveAndClick(Rectangle rect)
        {
            int x = _rnd.Next(rect.X, rect.X + rect.Width);
            int y = _rnd.Next(rect.Y, rect.Y + rect.Height);

            await MoveAndClick(x, y);
        }

        public void MoveSmooth(int x1, int y1, int x2, int y2)
        {
            // Определяем границы прямоугольника
            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            // Выбираем случайную точку внутри прямоугольника
            int randomX = _rnd.Next(minX, maxX + 1);
            int randomY = _rnd.Next(minY, maxY + 1);

            // Вызываем первый метод
            MoveSmooth(randomX, randomY);
        }

        public void MoveSmooth(Rectangle rect)
        {
            int x = _rnd.Next(rect.X, rect.X + rect.Width);
            int y = _rnd.Next(rect.Y, rect.Y + rect.Height);

            MoveSmooth(x, y);
        }

        private void SendMoveClamped(int dx, int dy)
        {
            while (dx != 0 || dy != 0)
            {
                int stepX = Math.Clamp(dx, -127, 127);
                int stepY = Math.Clamp(dy, -127, 127);

                //var sw = Stopwatch.StartNew();

                _sim.MouseMove(dx, dy);

                //sw.Stop();
                //Debug.WriteLine(sw.ElapsedMilliseconds);

                dx -= stepX;
                dy -= stepY;
            }
        }

        private void SendMove(int dx, int dy)
        {
            _sim.MouseMove(dx, dy);
        }
    }
}
