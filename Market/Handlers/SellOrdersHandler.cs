using tser.Screen.Screenshots;

namespace tser
{
    internal class SellOrdersHandler
    {
        private InputSimulator sim;
        private readonly AppSettings _settings;
        private readonly BuySellItemFullScreen screen;
        private Mover mover;
        private ScreenAnalyzer _analyzer;

        public SellOrdersHandler(InputSimulator inputSimulator, AppSettings appSettings, BuySellItemFullScreen screen, ScreenAnalyzer screenAnalyzer)
        {
            sim = inputSimulator;
            this._settings = appSettings;
            this.screen = screen;
            mover = new Mover(sim);
            _analyzer = screenAnalyzer;
        }

        public async Task Run()
        {
            var priceTyper = new PriceTyper(sim);

            sim.LeftButtonClick();
            await Task.Delay(500);

            var index = _analyzer.GetSelectedIndexOfSelectList(screen.SellListRegion, false);

            if (index == _settings.TradingSettings.AllowedBestPriceOrderPosition)
            {
                mover.MoveSmooth(screen.CloseRect);
                return;
            }

            var bestPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            await mover.MoveAndClick(screen.PriceRect);

            await priceTyper.TypePrice(bestPrice - 1);

            mover.MoveSmooth(screen.OkRect);

        }
    }
}
