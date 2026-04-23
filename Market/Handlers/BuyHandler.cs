namespace tser
{
    internal class BuyHandler
    {
        private ScreenAnalyzer _analyzer;
        private Rectangle _bestBuyPriceRegion = new Rectangle(1260, 360, 80, 25);
        private Rectangle _bestSellPriceRegion = new Rectangle(1017, 360, 80, 25);
        private Mover mover;
        private InputSimulator sim;

        public BuyHandler(InputSimulator inputSimulator, ScreenAnalyzer screenAnalyzer)
        {
            _analyzer = screenAnalyzer;
            sim = inputSimulator;
            mover = new Mover(sim);
        }

        public async Task Run(HandlerContext context)
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
