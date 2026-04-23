using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tser;

namespace tser.Screen.Screenshots
{
    internal class MapScreen
    {
        public readonly Size TooltipSize = new(248, 76);

        // смещение title внутри tooltip
        public readonly Point TitleOffset = new(63, 17);

        // итоговые смещения от курсора
        public readonly Point GateTitleOffsetRight = new(87, -59);   // 30 + 63
        public readonly Point GateTitleOffsetLeft = new(-187, -62); // -248+63, -76+17

        public readonly Size GateTitleSizeRight = new(180, 18);
        public readonly Size GateTitleSizeLeft = new(180, 18);

        public MapScreen(RegionManager regionManager)
        {

        }
    }
}
