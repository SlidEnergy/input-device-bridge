namespace tser
{
    internal class BuyHandler
    {
        private ScreenAnalyzer _analyzer = new ScreenAnalyzer(threshold: 0.85);
        private Rectangle _bestBuyPriceRegion = new Rectangle(1260, 360, 80, 25);
        private Rectangle _bestSellPriceRegion = new Rectangle(1017, 360, 80, 25);
        private Mover mover;
        private InputSimulator sim;

        public BuyHandler(InputSimulator inputSimulator)
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
            var bestBuyPrice = _analyzer.GetPrice(_bestBuyPriceRegion);
            var bestSellPrice = _analyzer.GetPrice(_bestSellPriceRegion);

            if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
            {
                mover.MoveSmooth(932, 303, 944, 315);
                return;
            }

            await mover.MoveAndClick(559, 532, 709, 542);

            await mover.MoveAndClick(852, 626, 869, 639);

            //await mover.MoveAndClick(862, 589, 869, 598);

            await mover.MoveAndClick(842, 721, 926, 740);

            mover.MoveSmooth(814, 545, 903, 565);

        }
    }
}
