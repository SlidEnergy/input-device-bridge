using tser.Screen.Screenshots;

namespace tser
{
    internal class NewBuyOrderHandler
    {
        private InputSimulator sim;
        private readonly BuySellItemFullScreen screen;
        private readonly BuySellItemQualityScreen qualityScreen;
        private Mover mover;
        private ScreenAnalyzer _analyzer;

        public NewBuyOrderHandler(InputSimulator inputSimulator, BuySellItemFullScreen screen, BuySellItemQualityScreen qualityScreen, ScreenAnalyzer screenAnalyzer) 
        {
            sim = inputSimulator;
            this.screen = screen;
            this.qualityScreen = qualityScreen;
            mover = new Mover(sim);
            _analyzer = screenAnalyzer;
            _analyzer.AddTemplate("Normal", "templates/ru/wide/buy_sell_item_quality_normal.png");
            _analyzer.AddTemplate("Good", "templates/ru/wide/buy_sell_item_quality_good.png");
            _analyzer.AddTemplate("Outstanding", "templates/ru/wide/buy_sell_item_quality_outstanding.png");
            _analyzer.AddTemplate("Excelent", "templates/ru/wide/buy_sell_item_quality_excelent.png");
            _analyzer.AddTemplate("Masterpiece", "templates/ru/wide/buy_sell_item_quality_masterpiece.png");
        }

        public async Task Run()
        {
            var priceTyper = new PriceTyper(sim);

            var from = Cursor.Position;

            await mover.MoveAndClick(screen.OrderBuyRegion);

            await Task.Delay(500);

        //    var window = _analyzer.DetectCurrentWindow(qualityScreen.SelectRegion);

        //    var selectedQuality = Enum.Parse<ItemQuality>(window);

        //}

        //public void CheckQuality(ItemQuality quality)
        //{
            var index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            // close
            if (index == 1)
            {
                mover.MoveSmooth(screen.CloseRect);
                return;
            }

            var bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            var compositeBestBuyPrice = bestBuyPrice;

            await mover.MoveAndClick(qualityScreen.SelectRegion);
            await mover.MoveAndClick(qualityScreen.OutstandingRegion);

            await Task.Delay(500);

            index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            if (index == 1)
            {
                await mover.MoveAndClick(qualityScreen.SelectRegion);
                await mover.MoveAndClick(qualityScreen.ExcelentRegion);

                await Task.Delay(500);

                var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

                // close
                if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
                {
                    mover.MoveSmooth(screen.CloseRect);
                    return;
                }

                // buy
                await mover.MoveAndClick(screen.PriceRect);

                await priceTyper.TypePrice(bestBuyPrice + 1);

                mover.MoveSmooth(screen.OkRect);
                return;
            }

            bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            await mover.MoveAndClick(qualityScreen.SelectRegion);
            await mover.MoveAndClick(qualityScreen.GoodRegion);

            await Task.Delay(500);

            index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            if (index == 1)
            {
                await mover.MoveAndClick(qualityScreen.SelectRegion);
                await mover.MoveAndClick(qualityScreen.OutstandingRegion);

                await Task.Delay(500);

                var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

                // close
                if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice))
                {
                    // close
                    if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
                    {
                        mover.MoveSmooth(screen.CloseRect);
                        return;
                    }

                    compositeBestBuyPrice = bestBuyPrice;
                }

                // buy
                await mover.MoveAndClick(screen.PriceRect);

                await priceTyper.TypePrice(compositeBestBuyPrice + 1);

                mover.MoveSmooth(screen.OkRect);
                return;
            }

            bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            await mover.MoveAndClick(qualityScreen.SelectRegion);
            await mover.MoveAndClick(qualityScreen.NormalRegion);

            await Task.Delay(500);

            index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            if (index == 1)
            {
                await mover.MoveAndClick(qualityScreen.SelectRegion);
                await mover.MoveAndClick(qualityScreen.GoodRegion);

                await Task.Delay(500);

                var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

                // close
                if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice))
                {
                    // close
                    if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
                    {
                        mover.MoveSmooth(screen.CloseRect);
                        return;
                    }

                    compositeBestBuyPrice = bestBuyPrice;
                }

                // buy
                await mover.MoveAndClick(screen.PriceRect);

                await priceTyper.TypePrice(compositeBestBuyPrice + 1);

                mover.MoveSmooth(screen.OkRect);
                return;
            }

            bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            var bestSellPrice2 = _analyzer.GetPrice(screen.BestSellPriceRegion);

            // close
            if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice2))
            {
                // close
                if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice2))
                {
                    mover.MoveSmooth(screen.CloseRect);
                    return;
                }

                compositeBestBuyPrice = bestBuyPrice;
            }

            // buy
            await mover.MoveAndClick(screen.PriceRect);

            await priceTyper.TypePrice(compositeBestBuyPrice + 1);

            mover.MoveSmooth(screen.OkRect);
            return;
        }
    }
}
