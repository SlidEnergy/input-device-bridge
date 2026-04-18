using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    [Serializable]
    internal class Screenshot
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public List<Region> Regions { get; set; } = new();
    }
}
