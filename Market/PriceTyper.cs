using System;

namespace tser
{

    internal class PriceTyper
    {
        private Random _rnd = new Random();
        private readonly InputSimulator _sim;

        const int VK_0 = 0x30;
        const int VK_1 = 0x31;
        const int VK_2 = 0x32;
        const int VK_3 = 0x33;
        const int VK_4 = 0x34;
        const int VK_5 = 0x35;
        const int VK_6 = 0x36;
        const int VK_7 = 0x37;
        const int VK_8 = 0x38;
        const int VK_9 = 0x39;

        public PriceTyper(InputSimulator inputSimulator)
        {
            _sim = inputSimulator;
        }

        /// <summary>
        /// Печатает цену на клавиатуре, принимая int без разделителей.
        /// </summary>
        public async Task TypePrice(int price)
        {
            string priceStr = price.ToString(); // "12345"

            foreach (char c in priceStr)
            {
                if (char.IsDigit(c))
                {
                    int key = DigitToKeyCode(c);
                   _sim.KeyPress(key);
                    await Task.Delay(45 + _rnd.Next(11)); // 40..50
                    //System.Threading.Thread.Sleep(40);
                }
            }
        }

        private int DigitToKeyCode(char digit)
        {
            return digit switch
            {
                '0' => VK_0,
                '1' => VK_1,
                '2' => VK_2,
                '3' => VK_3,
                '4' => VK_4,
                '5' => VK_5,
                '6' => VK_6,
                '7' => VK_7,
                '8' => VK_8,
                '9' => VK_9,
                _ => throw new ArgumentException($"Invalid digit: {digit}")
            };
        }
    }
}
