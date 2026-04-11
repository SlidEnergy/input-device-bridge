using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class BuyOrdersHandler
    {
        private InputSimulator sim;
        private Mover mover;
        private ScreenAnalyzer _analyzer = new ScreenAnalyzer(threshold: 0.85);
        private Rectangle _buyListRegion = new Rectangle(1260, 360, 185, 157);
        private Rectangle _bestBuyPriceRegion = new Rectangle(1260, 360, 80, 25);
        private Rectangle _bestSellPriceRegion = new Rectangle(1017, 360, 80, 25);

        public BuyOrdersHandler(InputSimulator inputSimulator)
        {
            sim = inputSimulator;
            mover = new Mover(sim);
            // Добавляем шаблоны
            //analyzer.AddTemplate("main_buy", "templates/main_buy.png");
            //analyzer.AddTemplate("main_sell", "templates/main_sell.png");
            //analyzer.AddTemplate("main_create_buy_order", "templates/main_create_buy_order.png");
            //analyzer.AddTemplate("main_orders", "templates/main_orders.png");
            //analyzer.AddTemplate("main_suits", "templates/main_suits.png");
        }

        public async Task Run()
        {
            var priceTyper = new PriceTyper();

            sim.LeftButtonClick();
            await Task.Delay(500);

            var index = _analyzer.GetSelectedIndexOfSelectList(_buyListRegion, true);

            if (index == 1 || index == 2 || index == 3)
            {
                mover.MoveSmooth(932, 303, 944, 315);
                return;
            }

            var bestBuyPrice = _analyzer.GetPrice(_bestBuyPriceRegion);
            var bestSellPrice = _analyzer.GetPrice(_bestSellPriceRegion);

            if(!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
            {
                mover.MoveSmooth(932, 303, 944, 315);
                return;
            }

            await mover.MoveAndClick(607, 626, 702, 639);

            priceTyper.TypePrice(bestBuyPrice + 1);

            mover.MoveSmooth(845, 722, 923, 738);
        }
    }
}
