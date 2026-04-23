using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser.Battle.Maps
{
    public class Map
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Tier { get; set; }
        public string Quality { get; set; }

        public string PvpCategory { get; set; }
        public string Type { get; set; }

        public bool IsSmugglersNetworkMarket { get; set; }
        public string Biome { get; set; }
        public string MapCategory { get; set; }
        public bool IsRoyal { get; set; }

        public string Name { get; set; }
    }
}
