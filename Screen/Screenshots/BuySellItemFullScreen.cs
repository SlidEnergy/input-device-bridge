using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tser;

namespace tser.Screen.Screenshots
{
    internal class BuySellItemFullScreen
    {
        public Rectangle BuyListRegion;// = new Rectangle(1260, 360, 185, 157);
        public Rectangle SellListRegion;// new Rectangle(1015, 360, 165, 160);
        public Rectangle BestBuyPriceRegion;// = new Rectangle(1260, 360, 80, 25);
        public Rectangle BestSellPriceRegion;// = new Rectangle(1017, 360, 80, 25);
        public Rectangle CloseRect; //932, 303, 944, 315
        public Rectangle OkRect; //845, 722, 923, 738
        public Rectangle PriceRect; //607, 626, 702, 639
        public Rectangle SellRegion; //607, 626, 702, 639
        public Rectangle BuyRegion; //607, 626, 702, 639
        public Rectangle OrderSellRegion;
        public Rectangle OrderBuyRegion;
        public Rectangle DecreaseCountRegion;
        public Rectangle IncreaseCountRegion;
        public Rectangle DecreasepriceRegion;
        public Rectangle IncreasepriceRegion;

        public BuySellItemFullScreen(RegionManager regionManager)
        {
            BuyListRegion = regionManager.GetRect("buy_sell_item_full", "buy_list");
            SellListRegion = regionManager.GetRect("buy_sell_item_full", "sell_list");
            BestBuyPriceRegion = regionManager.GetRect("buy_sell_item_full", "best_buy_price");
            BestSellPriceRegion = regionManager.GetRect("buy_sell_item_full", "best_sell_price");

            CloseRect = regionManager.GetRect("buy_sell_item_full", "close");
            OkRect = regionManager.GetRect("buy_sell_item_full", "ok");
            PriceRect = regionManager.GetRect("buy_sell_item_full", "price_input");

            SellRegion = regionManager.GetRect("buy_sell_item_full", "sell");
            BuyRegion = regionManager.GetRect("buy_sell_item_full", "buy");
            OrderSellRegion = regionManager.GetRect("buy_sell_item_full", "order_sell");
            OrderBuyRegion = regionManager.GetRect("buy_sell_item_full", "order_buy");

            DecreaseCountRegion = regionManager.GetRect("buy_sell_item_full", "decrease_count");
            IncreaseCountRegion = regionManager.GetRect("buy_sell_item_full", "increase_count");
            DecreasepriceRegion = regionManager.GetRect("buy_sell_item_full", "decrease_price");
            IncreasepriceRegion = regionManager.GetRect("buy_sell_item_full", "increase_price");
        }
    }
}
