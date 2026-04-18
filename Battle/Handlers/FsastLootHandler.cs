namespace tser
{
    internal class FastLootHandler
    {
        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private Rectangle _lootRegion = new Rectangle(815, 447, 265, 240);
        private int cellSize = 60;

        public FastLootHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer)
        {
            _analyzer = screenAnalyzer;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
        }

        public async Task Run()
        {
            if (settings.BattleSettings.LootStrategy == LootStrategy.All)
            {
                var priceTyper = new PriceTyper(sim);
                var from = Cursor.Position;

                sim.LeftButtonClick();

                await Task.Delay(500);

                await mover.MoveAndClick(1042, 745, 1092, 755);
                return;
            }
        }
    }
}
