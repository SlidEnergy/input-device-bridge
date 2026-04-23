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
        private Mat lootTitle;

        public FastLootHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, LootPersonScreen screen)
        {
            _analyzer = screenAnalyzer;
            _analyzer.ClearTemplates();
            lootTitle = _analyzer.CreateTemplate("LootTitle", "assets/templates/ru/wide/loot_title.png");
            _analyzer.AddTemplate("Empty1", "assets/templates/ru/wide/loot_empty1.png");
            _analyzer.AddTemplate("Empty2", "assets/templates/ru/wide/loot_empty2.png");
            _analyzer.AddTemplate("Broken1", "assets/templates/ru/wide/loot_broken1.png");
            _analyzer.AddTemplate("Broken2", "assets/templates/ru/wide/loot_broken2.png");
            _analyzer.AddTemplate("Broken3", "assets/templates/ru/wide/loot_broken3.png");
            _analyzer.AddTemplate("Broken4", "assets/templates/ru/wide/loot_broken4.png");
            _analyzer.AddTemplate("Broken5", "assets/templates/ru/wide/loot_broken5.png");
            _analyzer.AddTemplate("Broken6", "assets/templates/ru/wide/loot_broken6.png");
            _analyzer.AddTemplate("Broken7", "assets/templates/ru/wide/loot_broken7.png");
            _analyzer.AddTemplate("Broken8", "assets/templates/ru/wide/loot_broken8.png");

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
                            screen.CellTemplateSize, screen.CellTemplateSize));

                        if (result == "Empty2")
                            break;

                        if (result != null)
                            continue;

                        await mover.MoveAndClick(new Rectangle(screen.ClickCellPoint.X + screen.CellOffset * column, screen.ClickCellPoint.Y + screen.CellOffset * row,
                            screen.CellClickSize, screen.CellClickSize));

                        if (!_analyzer.DetectCurrentWindow(screen.LootTitle, lootTitle))
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
