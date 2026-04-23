namespace tser
{
    internal class SellHandler
    {
        private InputSimulator sim;
        private readonly RegionManager regionManager;
        private Mover mover;
        private Rectangle decreasePriceRect;
        private Rectangle orderSellRect;
        private Rectangle okRect;

        public SellHandler(InputSimulator inputSimulator, RegionManager regionManager) 
        {
            sim = inputSimulator;
            this.regionManager = regionManager;
            mover = new Mover(sim);

            decreasePriceRect = regionManager.GetRect("buy_sell_item_full", "decrease_price");
            orderSellRect = regionManager.GetRect("buy_sell_item_full", "order_sell");
            okRect = regionManager.GetRect("buy_sell_item_full", "ok");
        }

        public async Task Run(HandlerContext context)
        {
            var from = Cursor.Position;

            sim.LeftButtonClick();
            await Task.Delay(50);

            await mover.MoveAndClick(decreasePriceRect);
            await mover.MoveAndClick(orderSellRect);

            await mover.MoveAndClick(okRect);

            //await mover.MoveAndClick(609, 510, 618, 519);

            //await mover.MoveAndClick(844, 722, 926, 740);

            mover.MoveSmooth(from.X, from.Y);
        }
    }
}
