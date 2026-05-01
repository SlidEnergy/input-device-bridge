using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser.Battle.Maps
{
    internal class InterestedLocation
    {
        public int Depth { get; set; }
        public Map Map { get; set; }
        public string Description { get; set; }

        public override string ToString()
            => $"{Depth} локаций до {Description} ({Map.DisplayName})";
    }
}
