using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using tser.Battle.Maps;
using tser.Screen.Screenshots;
using static System.Net.Mime.MediaTypeNames;

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
        private List<int> _verticalOffsetsBetweenHp = new List<int>();

        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private readonly GroupPanelScreen screen;
        private readonly IServiceProvider serviceProvider;
        private int LowHpIndex = -1;
        private CancellationTokenSource _cts;
        private HandlerContext context;
        private Point cornerPosition;
        private DateTime _lastShowDateTime;
        private double _lowLimit = 0.05;
        private double _alarmLimit = 0.4;
        private bool _isCalibrated = false;
        private int _verticalOffsetFromCornerPosition = 0;
        private int _horisontalOffsetFromCornerPosition = 0;

        public LowHpPlayerHandler(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            sim = serviceProvider.GetRequiredService<InputSimulator>();
            screen = serviceProvider.GetRequiredService<GroupPanelScreen>();
            settings = serviceProvider.GetRequiredService<AppSettings>();
            _analyzer = serviceProvider.GetRequiredService<ScreenAnalyzer>();

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
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_isCalibrated)
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    var groupPanelRect = new Rectangle(
                        cornerPosition.X,
                        cornerPosition.Y,
                        _width,
                        (_heights.Sum() + _verticalOffsetsBetweenHp.Sum()));

                    var mat = _analyzer.CaptureRegion(groupPanelRect);

                    double[] hpArray = new double[_count];
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

                        accumulatedHeight += _heights[rowIndex] + _verticalOffsetsBetweenHp[rowIndex];

                        using var cropped = new OpenCvSharp.Mat(mat, new OpenCvSharp.Rect(hpBarRect.X, hpBarRect.Y, hpBarRect.Width, hpBarRect.Height));

                        //OpenCvSharp.Cv2.ImWrite("hp_debug" + rowIndex + ".png", cropped);

                        var hp = GetHp(mat, hpBarRect);

                        hpArray[rowIndex] = hp;

                        if (hp < _alarmLimit && hp >= _lowLimit && DateTime.Now - _lastShowDateTime > TimeSpan.FromMilliseconds(2000))
                        {
                            _lastShowDateTime = DateTime.Now;
                            ShowAsterics();
                        }
                        //if (IsLowHp(mat, hpBarRect))
                        //{
                        //    LowHpIndex = rowIndex;
                        //    ShowAsterics();
                        //    await Task.Delay(2000);
                        //    break;
                        //}
                    }

                    var minIndex = GetMinHpIndex(hpArray);

                    if (minIndex >= 0)
                    {
                        LowHpIndex = minIndex;
                    }

                    await Task.Delay(500);
                }
            }
            catch (TaskCanceledException) { }
        }

        private int GetMinHpIndex(double[] hp)
        {
            var min = 1.0d;
            var minIndex = -1;

            for (int i = 0; i < _count; i++)
            {
                var multiplier = settings.BattleSettings.HightPriorityForFirst ? 1.5 : 1;

                if (hp[i] * multiplier < min && hp[i] >= _lowLimit)
                {
                    min = hp[i] * multiplier;
                    minIndex = i;
                }
            }

            return minIndex;
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

                if (!IsRedMiddle(r, g, b))
                    break;

                startPosition.Y = cursor.Y;
            }

            cursor.Y++;

            while (true)
            {
                cursor.X--;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                if (!IsRedMiddle(r, g, b))
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
            _verticalOffsetsBetweenHp.Clear();
            _heights.Clear();
            bool calcOffset = false;

            while (true)
            {
                cursor.Y++;

                var (r, g, b) = GetPixel(mat, cursor.X, cursor.Y); // из bitmap/mat

                var isRed = IsRedBottom(r, g, b);
                if (!calcOffset && !isRed)
                {
                    if (height < 7)
                    {
                        height = _heights[_heights.Count() - 1] + height;
                        _heights.RemoveAt(_heights.Count() - 1);
                        calcOffset = true;
                    }
                    else
                    {
                        _heights.Add(height);
                        height = 0;
                        calcOffset = true;
                        index++;
                    }
                }
                else if (calcOffset && isRed)
                {
                    _verticalOffsetsBetweenHp.Add(height);
                    height = 0;
                    calcOffset = false;
                }

                height++;

                if (height > 20)
                {
                    _verticalOffsetsBetweenHp.Add(3);
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

                if (!IsRedTop(r, g, b))
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

            // add points lefter the corner

            if (_heights[0] >= 15)
            {
                _verticalOffsetFromCornerPosition = 2;
                _horisontalOffsetFromCornerPosition = 2;
            }
            else
            {
                _verticalOffsetFromCornerPosition = 1;
                _horisontalOffsetFromCornerPosition = 1;
            }

            _width += _horisontalOffsetFromCornerPosition;
        }

        public Task Calibrate(HandlerContext context)
        {
            var mat = _analyzer.CaptureRegion(new Rectangle(0, 0, 1980, 1080));
            var cursor = Cursor.Position;
            cornerPosition = FindStartPosition(mat, cursor);

            RecalcHeightsAndOffsets(mat, cornerPosition);

            CalcWidth(mat, cornerPosition, _heights[0]);

            cornerPosition.X -= _horisontalOffsetFromCornerPosition;

            var groupPanelRect = new Rectangle(
                cornerPosition.X,
                cornerPosition.Y,
                _width,
                _heights.Sum() + _verticalOffsetsBetweenHp.Sum());

            var mat2 = _analyzer.CaptureRegion(groupPanelRect);

            ShowTooltip(context, new Point(cursor.X + 20, cursor.Y + 20));

            _isCalibrated = true;

            return Task.CompletedTask;
        }

        private void ShowTooltip(HandlerContext context, Point position)
        {
            context.SynchronizationContext.Post(_ =>
            {
                var form = serviceProvider.GetRequiredService<GateHelperForm>();
                form.SetText("width = " + _width + "\r\ncount = " + _count);

                form.SetLocation(position);
                form.Show();

                form.InitAutoClose(Cursor.Position);
            }, null);
        }

        public async Task Run(HandlerContext context)
        {
            if (LowHpIndex >= 0)
            {
                if (LowHpIndex > 0 && LowHpIndex < 9)
                    sim.KeyPress((Keys)((int)Keys.D1 + LowHpIndex));
                else if(LowHpIndex == 9 )
                    sim.KeyPress((Keys)((int)Keys.D0));
                else if(LowHpIndex > 9 && LowHpIndex < 20)
                    sim.KeyPress((Keys)((int)Keys.F1 + LowHpIndex - 10));
            }
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
                for (int x = 0; x < hpBarRect.Width; x++)
                {
                    int y = hpBarRect.Top + dy;

                    if (x < _horisontalOffsetFromCornerPosition)
                        y += _verticalOffsetFromCornerPosition;

                    var (r, g, b) = GetPixel(mat, x, y); // из bitmap/mat

                    if (IsRedTop(r, g, b))
                        votes[x]++;
                }
            }

            for (int dy = 0; dy < 2; dy++)
            {
                for (int x = 0; x < hpBarRect.Width; x++)
                {
                    int y = hpBarRect.Top + hpBarRect.Height - dy - 1;

                    if (x < _horisontalOffsetFromCornerPosition)
                        y -= _verticalOffsetFromCornerPosition;

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

                    if (filledWidth > 0)
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

            if (percent < _alarmLimit && percent >= _lowLimit)
                return true;

            return false;
        }

        private double GetHp(OpenCvSharp.Mat mat, Rectangle hpBarRect)
        {
            var votes = CalculateVotes(mat, hpBarRect);

            bool IsFilledColumn(int x) => votes[x] >= 3;

            var filledWidth = CalculateFilledWidth(votes);

            double percent = (double)filledWidth / _width;

            return percent;
        }

        bool IsRedMiddle(byte r, byte g, byte b)
        {
            return r > 150 &&              // отсечь темный фон
                   r > g * 1.5 &&
                   r > b * 1.5 &&
                   g < 100 &&          // ограничиваем желтизну
                   b < 80;
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
