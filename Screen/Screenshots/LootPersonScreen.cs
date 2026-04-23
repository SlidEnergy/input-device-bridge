using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tser;

namespace tser.Screen.Screenshots
{
    internal class LootPersonScreen
    {
        public Point CellPoint = new Point(811, 397);
        public Point ClickCellPoint = new Point(815, 400);
        public int CellOffset = 70;
        public int CellTemplateSize = 62;
        public int CellClickSize = 50;
        public Rectangle TakeAllRegion;
        public Rectangle LootTitle;

        public LootPersonScreen(RegionManager regionManager)
        {
            TakeAllRegion = regionManager.GetRect("loot_person", "take_all");
            LootTitle = regionManager.GetRect("loot_person", "title");
        }
    }
}
