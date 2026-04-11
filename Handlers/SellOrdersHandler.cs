namespace tser
{
    internal class SellOrdersHandler
    {
        private InputSimulator sim;
        private Mover mover;
        private ScreenAnalyzer _analyzer = new ScreenAnalyzer(threshold: 0.85);
        private Rectangle _sellListRegion = new Rectangle(1015, 360, 165, 160);
        private Rectangle _bestSellPriceRegion = new Rectangle(1017, 360, 80, 25);

        public SellOrdersHandler(InputSimulator inputSimulator)
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

            var index = _analyzer.GetSelectedIndexOfSelectList(_sellListRegion, false);

            if (index != -1)
            {
                mover.MoveSmooth(932, 301, 945, 314);
                return;
            }

            var bestPrice = _analyzer.GetPrice(_bestSellPriceRegion);

            await mover.MoveAndClick(607, 626, 702, 639);

            priceTyper.TypePrice(bestPrice - 1);

            mover.MoveSmooth(845, 722, 923, 738);

        }
    }
}
