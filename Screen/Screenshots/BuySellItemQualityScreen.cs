using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tser;

namespace tser.Screen.Screenshots
{
    internal class BuySellItemQualityScreen
    {
        public Rectangle SelectRegion;
        public Rectangle NormalRegion;
        public Rectangle GoodRegion; 
        public Rectangle OutstandingRegion;
        public Rectangle ExcelentRegion;
        public Rectangle MasterpieceRegion;

        public BuySellItemQualityScreen(RegionManager regionManager)
        {
            SelectRegion = regionManager.GetRect("buy_sell_item_quality", "select");
            NormalRegion = regionManager.GetRect("buy_sell_item_quality", "Normal");
            GoodRegion = regionManager.GetRect("buy_sell_item_quality", "Good");
            OutstandingRegion = regionManager.GetRect("buy_sell_item_quality", "Outstanding");
            ExcelentRegion = regionManager.GetRect("buy_sell_item_quality", "Excelent");
            MasterpieceRegion = regionManager.GetRect("buy_sell_item_quality", "Masterpiece");
        }
    }
}
