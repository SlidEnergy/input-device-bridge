using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    [Serializable]
    internal class Region
    {
        public string Name { get; set; }

        public Rectangle Rect { get; set; }

        public Region(string name, Rectangle rect) 
        {
            Name = name;
            Rect = rect;
        }
    }
}
