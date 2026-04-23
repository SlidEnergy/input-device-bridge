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
        private PriceTyper priceTyper;

        public NewBuyOrderHandler(InputSimulator inputSimulator, BuySellItemFullScreen screen, BuySellItemQualityScreen qualityScreen, ScreenAnalyzer screenAnalyzer) 
        {
            sim = inputSimulator;
            this.screen = screen;
            this.qualityScreen = qualityScreen;
            mover = new Mover(sim);
            _analyzer = screenAnalyzer;
            _analyzer.ClearTemplates();
            _analyzer.AddTemplate("Normal", "assets/templates/ru/wide/buy_sell_item_quality_normal.png");
            _analyzer.AddTemplate("Good", "assets/templates/ru/wide/buy_sell_item_quality_good.png");
            _analyzer.AddTemplate("Outstanding", "assets/templates/ru/wide/buy_sell_item_quality_outstanding.png");
            _analyzer.AddTemplate("Excelent", "assets/templates/ru/wide/buy_sell_item_quality_excelent.png");
            _analyzer.AddTemplate("Masterpiece", "assets/templates/ru/wide/buy_sell_item_quality_masterpiece.png");
            priceTyper = new PriceTyper(sim);
        }

        public async Task Run(HandlerContext context)
        {
            var from = Cursor.Position;

            await mover.MoveAndClick(screen.OrderBuyRegion);

            await Task.Delay(500);

            var result = _analyzer.DetectCurrentWindow(qualityScreen.SelectRegion);

            var selectedQuality = Enum.Parse<ItemQuality>(result);

            await CheckQuality(selectedQuality, new CheckQualityContext() { MaxQuality = selectedQuality });

        }

        public async Task SwitchQuality(ItemQuality quality)
        {
            switch (quality)
            {
                case ItemQuality.Normal:
                    await mover.MoveAndClick(qualityScreen.SelectRegion);
                    await mover.MoveAndClick(qualityScreen.NormalRegion);

                    return;

                case ItemQuality.Good:
                    await mover.MoveAndClick(qualityScreen.SelectRegion);
                    await mover.MoveAndClick(qualityScreen.GoodRegion);

                    return;

                case ItemQuality.Outstanding:
                    await mover.MoveAndClick(qualityScreen.SelectRegion);
                    await mover.MoveAndClick(qualityScreen.OutstandingRegion);
                    return;

                case ItemQuality.Excelent:
                    await mover.MoveAndClick(qualityScreen.SelectRegion);
                    await mover.MoveAndClick(qualityScreen.ExcelentRegion);
                    return;

                case ItemQuality.Masterpiece:
                    await mover.MoveAndClick(qualityScreen.SelectRegion);
                    await mover.MoveAndClick(qualityScreen.MasterpieceRegion);

                    return;
            }
        }

        public ItemQuality IncreaseQuality(ItemQuality quality)
        {
            var next = (ItemQuality)((int)quality << 1);

            return Enum.IsDefined(typeof(ItemQuality), next)
                ? next
                : quality;
        }

        public ItemQuality DecreaseQuality(ItemQuality quality)
        {
            var prev = (ItemQuality)((int)quality >> 1);

            return Enum.IsDefined(typeof(ItemQuality), prev)
                ? prev
                : quality;
        }

        public async Task BuyAndClose(int compositeBestBuyPrice, int bestBuyPrice)
        {
            var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            // close
            if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice))
            {
                // close
                if (compositeBestBuyPrice == bestBuyPrice || !ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
                {
                    mover.MoveSmooth(screen.CloseRect);
                    return;
                }

                compositeBestBuyPrice = bestBuyPrice;
            }

            // buy
            await mover.MoveAndClick(screen.PriceRect);

            await priceTyper.TypePrice(compositeBestBuyPrice + 500);

            mover.MoveSmooth(screen.OkRect);
            return;
        }

        internal class CheckQualityContext
        {
            public int CompositeBestBuyPrice;
            public int BestBuyPrice;
            public ItemQuality MaxQuality = ItemQuality.Excelent;
        }

        public async Task<bool> CheckQuality(ItemQuality quality, CheckQualityContext price)
        {
            var index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            // close
            if (index == 1)
            {
                if (quality == price.MaxQuality)
                {
                    mover.MoveSmooth(screen.CloseRect);
                    return true;
                }

                return false;
            }

            price.BestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            price.CompositeBestBuyPrice = price.BestBuyPrice * 1.15 > price.CompositeBestBuyPrice ? price.CompositeBestBuyPrice : price.BestBuyPrice;

            if (quality == ItemQuality.Normal)
            {
                await BuyAndClose(price.CompositeBestBuyPrice, price.BestBuyPrice);
                return true;
            }

            var lowerQuality = DecreaseQuality(quality);
            await SwitchQuality(lowerQuality);
            await Task.Delay(500);

            var result = await CheckQuality(lowerQuality, price);

            if (!result)
            {
                await SwitchQuality(quality);
                await Task.Delay(500);

                await BuyAndClose(price.CompositeBestBuyPrice, price.BestBuyPrice);
                return true;
            }

            return true;
            //index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            //if (index == 1)
            //{
            //    await SwitchQuality(IncreaseQuality(quality));
            //    await mover.MoveAndClick(qualityScreen.SelectRegion);
            //    await mover.MoveAndClick(qualityScreen.ExcelentRegion);

            //    await Task.Delay(500);

            //    await BuyAndClose(compositeBestBuyPrice, bestBuyPrice);
            //    return;
            //}

            //bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            //compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            //await mover.MoveAndClick(qualityScreen.SelectRegion);
            //await mover.MoveAndClick(qualityScreen.GoodRegion);

            //await Task.Delay(500);

            //index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            //if (index == 1)
            //{
            //    await mover.MoveAndClick(qualityScreen.SelectRegion);
            //    await mover.MoveAndClick(qualityScreen.OutstandingRegion);

            //    await Task.Delay(500);

            //    var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            //    // close
            //    if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice))
            //    {
            //        // close
            //        if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
            //        {
            //            mover.MoveSmooth(screen.CloseRect);
            //            return;
            //        }

            //        compositeBestBuyPrice = bestBuyPrice;
            //    }

            //    // buy
            //    await mover.MoveAndClick(screen.PriceRect);

            //    await priceTyper.TypePrice(compositeBestBuyPrice + 1);

            //    mover.MoveSmooth(screen.OkRect);
            //    return;
            //}

            //bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            //compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            //await mover.MoveAndClick(qualityScreen.SelectRegion);
            //await mover.MoveAndClick(qualityScreen.NormalRegion);

            //await Task.Delay(500);

            //index = _analyzer.GetSelectedIndexOfSelectList(screen.BuyListRegion, true);

            //if (index == 1)
            //{
            //    await mover.MoveAndClick(qualityScreen.SelectRegion);
            //    await mover.MoveAndClick(qualityScreen.GoodRegion);

            //    await Task.Delay(500);

            //    var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            //    // close
            //    if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice))
            //    {
            //        // close
            //        if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice))
            //        {
            //            mover.MoveSmooth(screen.CloseRect);
            //            return;
            //        }

            //        compositeBestBuyPrice = bestBuyPrice;
            //    }

            //    // buy
            //    await mover.MoveAndClick(screen.PriceRect);

            //    await priceTyper.TypePrice(compositeBestBuyPrice + 1);

            //    mover.MoveSmooth(screen.OkRect);
            //    return;
            //}

            //bestBuyPrice = _analyzer.GetPrice(screen.BestBuyPriceRegion);
            //compositeBestBuyPrice = bestBuyPrice * 1.15 > compositeBestBuyPrice ? compositeBestBuyPrice : bestBuyPrice;

            //var bestSellPrice2 = _analyzer.GetPrice(screen.BestSellPriceRegion);

            //// close
            //if (!ProfitCalculator.IsProfitable(compositeBestBuyPrice, bestSellPrice2))
            //{
            //    // close
            //    if (!ProfitCalculator.IsProfitable(bestBuyPrice, bestSellPrice2))
            //    {
            //        mover.MoveSmooth(screen.CloseRect);
            //        return;
            //    }

            //    compositeBestBuyPrice = bestBuyPrice;
            //}

            //// buy
            //await mover.MoveAndClick(screen.PriceRect);

            //await priceTyper.TypePrice(compositeBestBuyPrice + 1);

            //mover.MoveSmooth(screen.OkRect);
            //return;
        }
    }
}
