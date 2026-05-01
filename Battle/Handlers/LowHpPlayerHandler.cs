using System.Drawing;
using tser.Screen.Screenshots;

namespace tser
{
    internal class LowHpPlayerHandler
    {
        private Size rowSize = new Size(78, 17);
        private Size hpBarSize = new Size(61, 13);
        private int offsettY = 5;
        private int rowsCount = 4;

        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private readonly GroupPanelScreen screen;

        private int LowHpIndex = 0;
        private CancellationTokenSource _cts;
        private HandlerContext context;

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
            var groupPanelRect = new Rectangle(
                settings.BattleSettings.GroupPanelPosition.X,
                settings.BattleSettings.GroupPanelPosition.Y,
                hpBarSize.Width,
                rowsCount * (hpBarSize.Height + offsettY));

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var mat = _analyzer.CaptureRegion(groupPanelRect);

                    for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                    {
                        //var hpBarRect = new Rectangle(
                        //    settings.BattleSettings.GroupPanelPosition.X,
                        //    settings.BattleSettings.GroupPanelPosition.Y + rowIndex * (hpBarSize.Height + offsettY),
                        //    hpBarSize.Width,
                        //    hpBarSize.Height
                        //);

                        var hpBarRect = new Rectangle(
                            0,
                            0 + rowIndex * (hpBarSize.Height + offsettY),
                            hpBarSize.Width,
                            hpBarSize.Height
                        );

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
                var form = new AlertForm();
                form.Show();
            }, null);
        }

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
            int[] votes = new int[hpBarSize.Width];

            for (int dy = 0; dy < 2; dy++)
            {
                int y = hpBarRect.Top + dy;

                for (int x = 0; x < hpBarSize.Width; x++)
                {
                    var (r, g, b) = GetPixel(mat, x, y); // из bitmap/mat

                    if (IsRedTop(r, g, b))
                        votes[x]++;
                }
            }

            for (int dy = 0; dy < 2; dy++)
            {
                int y = hpBarRect.Top + hpBarSize.Height - dy - 1;

                for (int x = 0; x < hpBarSize.Width; x++)
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

            for (int x = 0; x < hpBarSize.Width; x++)
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

            double percent = (double)filledWidth / hpBarSize.Width;

            if (percent < 0.4 && percent > 0.05)
                return true;

            return false;
        }

        bool IsRed(byte r, byte g, byte b)
        {
            return r > 100 &&              // отсечь темный фон
                   r > g * 1.3 &&
                   r > b * 1.3;
        }

        bool IsRedTop(byte r, byte g, byte b)
        {
            //сверху 250 73 59
            return r > 200 &&          // яркий красный
                   r > g * 2 &&
                   r > b * 2 &&
                   g < 100 &&          // ограничиваем желтизну
                   b < 80;
        }

        bool IsRedBottom(byte r, byte g, byte b)
        {
            //снизу 113 39 41
            return r > 100 &&           // темнее, но всё ещё красный
                   r > g * 2 &&
                   r > b * 2 &&
                   (r - g) > 20 &&     // чтобы не ловить серый/фон
                   (r - b) > 15;
        }
    }
}
