using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class ProfitCalculator
    {
        public static bool IsProfitable(int bestBuyPrice, int bestSellPrice)
        {
            var reorderCount = 3;
            var wat = 0.04f;
            var fee = 0.025f;
            var profit = bestSellPrice * (1 - wat) - bestBuyPrice * (1 + reorderCount * fee);
            return profit / bestBuyPrice > 0.2;
        }
    }
}
