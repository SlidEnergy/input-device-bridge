using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using tser.Screen.Screenshots;

namespace tser
{
    internal class LowHpPlayerHandler
    {
        //private Size rowSize = new Size(78, 17);
        //private Size hpBarSize = new Size(61, 13);
        private int _width = 61;
        private int _count = 0;
        private List<int> _heights = new List<int>();
        //private int offsettY = 5;
        private List<int> _verticalOffsets = new List<int>();

        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private readonly GroupPanelScreen screen;

        private int LowHpIndex = 0;
        private CancellationTokenSource _cts;
        private HandlerContext context;
        private Point cornerPosition;

        public LowHpPlayerHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, GroupPanelScreen screen)
        {
            _analyzer = screenAnalyzer;

            this.screen = screen;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
        }

        public Task Activate(HandlerContext context)
        {
            this.context = context;

            if (_cts != null && !_cts.IsCancellationRequested)
                return Task.CompletedTask;

            _cts = new CancellationTokenSource();
            _ = Loop(_cts.Token);
            return Task.CompletedTask;
        }

        public Task Deactivate(HandlerContext context)
        {
            this.context = context;

            _cts?.Cancel();
            return Task.CompletedTask;
        }

        public async Task Loop(CancellationToken cancellationToken)
        {
            if (cornerPosition == null)
                return;

            var groupPanelRect = new Rectangle(
                cornerPosition.X,
                cornerPosition.Y,
                _width,
                (_heights.Sum() + _verticalOffsets.Sum()));

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var mat = _analyzer.CaptureRegion(groupPanelRect);

                    var accumulatedHeight = 0;

                    for (int rowIndex = 0; rowIndex < _count; rowIndex++)
                    {
                        //var hpBarRect = new Rectangle(
                        //    settings.BattleSettings.GroupPanelPosition.X,
                        //    settings.BattleSettings.GroupPanelPosition.Y + rowIndex * (hpBarSize.Height + offsettY),
                        //    hpBarSize.Width,
                        //    hpBarSize.Height
                        //);

                        var hpBarRect = new Rectangle(
                            0,
                            0 + accumulatedHeight,
                            _width,
                            _heights[rowIndex]
                        );

                        accumulatedHeight += _heights[rowIndex] + _verticalOffsets[rowIndex];

                        using var cropped = new OpenCvSharp.Mat(mat, new OpenCvSharp.Rect(hpBarRect.X, hpBarRect.Y, hpBarRect.Width, hpBarRect.Height));

                        OpenCvSharp.Cv2.ImWrite("hp_debug.png", cropped);

                        if (IsLowHp(mat, hpBarRect))
                        {
                            LowHpIndex = rowIndex;
                            ShowAsterics();
                            await Task.Delay(2000);
                            break;
                        }
                    }

                    await Task.Delay(500);
                }
            }
            catch (TaskCanceledException) { }
        }

        private void ShowAsterics()
        {
            context.SynchronizationContext.Post(_ =>
            {
                var form = new MarkForm();
                form.SetIcon("alert");
                form.Show();
            }, null);
        }

        public Point FindStartPosition(OpenCvSharp.Mat mat, Point firstRowRandomPoint)
        {
            var cursor = new Point(firstRowRandomPoint.X, firstRowRandomPoint.Y);
            var startPosition = new Point(cursor.X, cursor.Y);

            while (true)
            {
                cursor.Y--;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                if (!IsRedBottom(r, g, b))
                    break;

                startPosition.Y = cursor.Y;
            }

            cursor.Y++;

            while (true)
            {
                cursor.X--;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                if (!IsRedBottom(r, g, b))
                    break;

                startPosition.X = cursor.X;
            }

            cursor.X++;

            return startPosition;
        }

        public void RecalcHeightsAndOffsets(OpenCvSharp.Mat mat, Point startPosition)
        {
            var cursor = new Point(startPosition.X + 3, startPosition.Y);
            var height = 1;
            var index = 0;
            _verticalOffsets.Clear();
            _heights.Clear();
            bool calcOffset = false;

            while (true)
            {
                cursor.Y++;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                var isRed = IsRedBottom(r, g, b);
                if (!calcOffset && !isRed)
                {
                    _heights.Add(height);
                    height = 0;
                    calcOffset = true;
                    index++;
                }
                else if (calcOffset && isRed)
                {
                    _verticalOffsets.Add(height);
                    height = 0;
                    calcOffset = false;
                }

                height++;

                if (height > 20)
                {
                    _verticalOffsets.Add(3);
                    _count = index;
                    break;
                }
            }
        }

        public void CalcWidth(OpenCvSharp.Mat mat, Point startPosition, int height)
        {
            var cursor = new Point(startPosition.X, startPosition.Y);
            var width1 = 1;
            var width2 = 1;

            while (true)
            {
                cursor.X++;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                if (!IsRedBottom(r, g, b))
                    break;

                width1++;
            }

            cursor.X = startPosition.X;
            cursor.Y = startPosition.Y;

            cursor.Y += height - 1;

            while (true)
            {
                cursor.X++;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                if (!IsRedBottom(r, g, b))
                    break;

                width2++;
            }

            _width = Math.Max(width1, width2);
        }

        public Task Calibrate(HandlerContext context)
        {
            var mat = _analyzer.CaptureRegion(new Rectangle(0, 0, 1980, 1080));
            var cursor = Cursor.Position;
            cornerPosition = FindStartPosition(mat, cursor);

            RecalcHeightsAndOffsets(mat, cornerPosition);

            CalcWidth(mat, cornerPosition, _heights[0]);

            var groupPanelRect = new Rectangle(
                cornerPosition.X,
                cornerPosition.Y,
                _width,
                _heights.Sum() + _verticalOffsets.Sum());

            var mat2 = _analyzer.CaptureRegion(groupPanelRect);

            return Task.CompletedTask;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public async Task Run(HandlerContext context)
        {
            sim.KeyPress((Keys)((int)Keys.D0 + LowHpIndex + 1));
        }

        private unsafe (byte r, byte g, byte b) GetPixel(OpenCvSharp.Mat mat, int x, int y)
        {
            byte* p = (byte*)mat.DataPointer + y * mat.Step() + x * 3;

            return (p[2], p[1], p[0]); // BGR -> RGB
        }

        private int[] CalculateVotes(OpenCvSharp.Mat mat, Rectangle hpBarRect)
        {
            int[] votes = new int[hpBarRect.Width];

            for (int dy = 0; dy < 2; dy++)
            {
                int y = hpBarRect.Top + dy;

                for (int x = 0; x < hpBarRect.Width; x++)
                {
                    var (r, g, b) = GetPixel(mat, x, y); // из bitmap/mat

                    if (IsRedTop(r, g, b))
                        votes[x]++;
                }
            }

            for (int dy = 0; dy < 2; dy++)
            {
                int y = hpBarRect.Top + hpBarRect.Height - dy - 1;

                for (int x = 0; x < hpBarRect.Width; x++)
                {
                    var (r, g, b) = GetPixel(mat, x, y); // из bitmap/mat

                    if (IsRedBottom(r, g, b))
                        votes[x]++;
                }
            }

            return votes;
        }

        private int CalculateFilledWidth(int[] votes)
        {
            bool IsFilledColumn(int x) => votes[x] >= 2;

            int filledWidth = 0;
            int gap = 0;

            for (int x = 0; x < _width; x++)
            {
                if (IsFilledColumn(x))
                {
                    filledWidth++;
                    gap = 0;
                }
                else
                {
                    gap++;
                    if (gap > 2)
                        break;

                    filledWidth++;
                }
            }

            return filledWidth;
        }

        private bool IsLowHp(OpenCvSharp.Mat mat, Rectangle hpBarRect)
        {
            var votes = CalculateVotes(mat, hpBarRect);

            bool IsFilledColumn(int x) => votes[x] >= 3;

            var filledWidth = CalculateFilledWidth(votes);

            double percent = (double)filledWidth / _width;

            if (percent < 0.5 && percent > 0.05)
                return true;

            return false;
        }

        bool IsRed(byte r, byte g, byte b)
        {
            return r > 150 &&              // отсечь темный фон
                   r > g * 1.3 &&
                   r > b * 1.3;
        }

        bool IsRedTop(byte r, byte g, byte b)
        {
            //сверху 250 73 59
            return r > 200 &&          // яркий красный
                   r > g * 1.5 &&
                   r > b * 1.5 &&
                   g < 100 &&          // ограничиваем желтизну
                   b < 80;
        }

        bool IsRedBottom(byte r, byte g, byte b)
        {
            //снизу 113 39 41
            return r > 100 &&           // темнее, но всё ещё красный
                   r > g * 1.5 &&
                   r > b * 1.5 &&
                   g < 100 &&     // чтобы не ловить серый/фон
                   b < 80;
        }
    }
}
