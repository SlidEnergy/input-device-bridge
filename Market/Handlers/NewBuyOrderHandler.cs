using Microsoft.Extensions.DependencyInjection;
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
        private TemplateManager templateManager;

        public NewBuyOrderHandler(IServiceProvider provider) 
        {
            sim = provider.GetRequiredService<InputSimulator>();
            this.screen = provider.GetRequiredService<BuySellItemFullScreen>();
            this.qualityScreen = provider.GetRequiredService<BuySellItemQualityScreen>();
            mover = new Mover(sim);
            _analyzer = provider.GetRequiredService<ScreenAnalyzer>();
            templateManager = provider.GetRequiredService<TemplateManager>();

            priceTyper = new PriceTyper(sim);
        }

        public async Task Run(HandlerContext context)
        {
            var from = Cursor.Position;

            await mover.MoveAndClick(screen.OrderBuyRegion);

            await Task.Delay(500);

            var result = _analyzer.DetectCurrentTemplate(qualityScreen.SelectRegion, nameof(NewBuyOrderHandler));

            if (result == null)
                return;

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

        public async Task BuyAndClose(CheckQualityContext context)
        {
            var bestSellPrice = _analyzer.GetPrice(screen.BestSellPriceRegion);

            // close
            if (!ProfitCalculator.IsProfitable(context.CompositeBestBuyPrice, bestSellPrice))
            {
                // close
                if (context.CompositeBestBuyPrice == context.BestBuyPrice || !ProfitCalculator.IsProfitable(context.BestBuyPrice, bestSellPrice))
                {
                    mover.MoveSmooth(screen.CloseRect);
                    return;
                }

                context.CompositeBestBuyPrice = context.BestBuyPrice;
            }

            // buy
            await mover.MoveAndClick(screen.PriceRect);

            await priceTyper.TypePrice(context.CompositeBestBuyPrice + 511);

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
            
            if (price.CompositeBestBuyPrice == 0)
                price.CompositeBestBuyPrice = price.BestBuyPrice;
            else
                price.CompositeBestBuyPrice = price.BestBuyPrice * 1.15 > price.CompositeBestBuyPrice ? price.CompositeBestBuyPrice : price.BestBuyPrice;

            if (quality == ItemQuality.Normal)
            {
                await BuyAndClose(price);
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

                await BuyAndClose(price);
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
