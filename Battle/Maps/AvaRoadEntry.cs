using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser.Battle.Maps
{
    internal class AvaRoadEntry
    {
        public string Name { get; set; }
        public AvaRoadData Data { get; set; }
    }

    internal class AvaRoadData
    {
        public string Type { get; set; }
        public string Tier { get; set; }
        public List<Component> Components { get; set; }
    }

    internal class Component
    {
        public string Type { get; set; }
        public string BgColor { get; set; }
        public string Size { get; set; }
        public string Tier { get; set; }
    }
}
