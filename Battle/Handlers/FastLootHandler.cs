using OpenCvSharp;
using tser.Screen.Screenshots;

namespace tser
{
    internal class FastLootHandler
    {
        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private readonly LootPersonScreen screen;

        public FastLootHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, LootPersonScreen screen)
        {
            _analyzer = screenAnalyzer;

            this.screen = screen;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
        }

        public async Task Run(HandlerContext context)
        {
            if (settings.BattleSettings.LootStrategy == LootStrategy.All)
            {
                var priceTyper = new PriceTyper(sim);
                var from = Cursor.Position;

                sim.LeftButtonClick();

                await Task.Delay(500);

                //await mover.MoveAndClick(1042, 745, 1092, 755);
                await mover.MoveAndClick(screen.TakeAllRegion);
                return;
            }

            if (settings.BattleSettings.LootStrategy == LootStrategy.Best)
            {
                var from = Cursor.Position;

                //if (!_analyzer.DetectCurrentWindow(screen.LootTitle, lootTitle))
                //{
                //    sim.LeftButtonClick();

                //    await Task.Delay(300);

                //    var timeout = TimeSpan.FromSeconds(1);
                //    var start = DateTime.UtcNow;

                //    while (_analyzer.DetectCurrentWindow(screen.LootTitle, lootTitle))
                //    {
                //        if (DateTime.UtcNow - start > timeout)
                //            break;

                //        await Task.Delay(100);
                //    }
                //}

                if (settings.BattleSettings.OpenLootWindow)
                {
                    sim.LeftButtonClick();
                    await Task.Delay(400);
                }

                sim.ShiftDown(Keys.LShiftKey);

                try
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int row = i / 4;
                        int column = i % 4;

                        var result = _analyzer.DetectCurrentWindow(new Rectangle(screen.CellPoint.X + screen.CellOffset * column, screen.CellPoint.Y + screen.CellOffset * row,
                            screen.CellTemplateSize, screen.CellTemplateSize), nameof(FastLootHandler));

                        if (result == "Empty2")
                            break;

                        if (result != null)
                            continue;

                        await mover.MoveAndClick(new Rectangle(screen.ClickCellPoint.X + screen.CellOffset * column, screen.ClickCellPoint.Y + screen.CellOffset * row,
                            screen.CellClickSize, screen.CellClickSize));

                        if (_analyzer.DetectCurrentWindow(screen.LootTitle, nameof(FastLootHandler) + "_Title") == null)
                            break;
                    }
                }
                finally
                {
                    sim.ShiftUp(Keys.LShiftKey);
                }

                return;
            }
        }
    }
}
