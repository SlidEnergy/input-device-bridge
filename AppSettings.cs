using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class AppSettings
    {
        public TradingSettings TradingSettings { get; set; } = new TradingSettings();

        public BattleSettings BattleSettings { get; set; } = new BattleSettings();
    }
}
