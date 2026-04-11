using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class MainActionHandler
    {
        private InputSimulator sim;
        private ScreenAnalyzer analyzer = new ScreenAnalyzer(threshold: 0.85);
        private Rectangle _mainTextRegion = new Rectangle(570, 320, 370, 25);
        private BuyHandler _buyHandler;
        private SellHandler _sellHandler;
        private SellOrdersHandler _sellOrdersHandler;
        private BuyOrdersHandler _buyOrdersHandler;

        public MainActionHandler(InputSimulator inputSimulator)
        {
            sim = inputSimulator;
            _buyHandler = new BuyHandler(inputSimulator);
            _sellHandler = new SellHandler(inputSimulator);
            _sellOrdersHandler = new SellOrdersHandler(inputSimulator);
            _buyOrdersHandler = new BuyOrdersHandler(inputSimulator);

            // Добавляем шаблоны
            analyzer.AddTemplate("main_buy", "templates/main_buy.png");
            analyzer.AddTemplate("main_sell", "templates/main_sell.png");
            analyzer.AddTemplate("main_create_buy_order", "templates/main_create_buy_order.png");
            analyzer.AddTemplate("main_orders", "templates/main_orders.png");
            analyzer.AddTemplate("main_suits", "templates/main_suits.png");
        }

        public async Task Run()
        { 
            var detected = analyzer.DetectCurrentWindow(_mainTextRegion);

            switch (detected)
            {
                case "main_buy":
                    //await _buyHandler.Run();
                    break;

                case "main_sell":
                    await _sellHandler.Run();
                    break;

                case "main_orders":
                    {
                        var from = Cursor.Position;
                        if(from.Y >= 615)
                            await _sellOrdersHandler.Run();
                        else
                            await _buyOrdersHandler.Run();
                        break;
                    }

                default:
                    await _buyHandler.Run();
                    break;
            }
        }
    }
}
