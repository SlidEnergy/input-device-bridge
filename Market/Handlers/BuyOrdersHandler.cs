using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tser.Screen.Screenshots;

namespace tser
{
    internal class BuyOrdersHandler
    {
        private InputSimulator sim;
        private readonly AppSettings _settings;
        private readonly BuySellItemFullScreen screen;
        private Mover mover;
        private ScreenAnalyzer _analyzer;

        public BuyOrdersHandler(InputSimulator inputSimulator, AppSettings settings, BuySellItemFullScreen screen, ScreenAnalyzer screenAnalyzer)
        {
            _analyzer = screenAnalyzer;
            sim = inputSimulator;
            this._settings = settings;
            this.screen = screen;
            mover = new Mover(sim);
        }

        public async Task Run()
        {
            var priceTyper = new PriceTyper(sim);

            sim.LeftButtonClick();
            await Task.Delay(500);

            var index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            //if (index == 1 || index == 2 || index == 3)
            if(index == _settings.TradingSettings.AllowedBestPriceOrderPosition)
            {
                mover.MoveSmooth(screen.CloseRect);
                return;
            }

            var bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            if(!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
            {
                mover.MoveSmooth(screen.CloseRect);
                return;
            }

            await mover.MoveAndClick(screen.PriceRect);

            await priceTyper.TypePrice(bestBuyPrice + 1);

            mover.MoveSmooth(screen.OkRect);
        }
    }
}
