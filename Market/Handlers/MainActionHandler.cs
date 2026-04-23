using Microsoft.Extensions.DependencyInjection;
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
        private readonly AppSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private ScreenAnalyzer analyzer;
        private Rectangle _mainTextRegion;
        private BuyHandler _buyHandler;
        private SellHandler _sellHandler;
        private SellOrdersHandler _sellOrdersHandler;
        private BuyOrdersHandler _buyOrdersHandler;
        private RegionManager _regionManager;

        public MainActionHandler(InputSimulator inputSimulator, AppSettings appSettings, IServiceProvider provider)
        {
            sim = inputSimulator;
            _settings = appSettings;
            _serviceProvider = provider;
            _buyHandler = _serviceProvider.GetRequiredService<BuyHandler>();
            _sellHandler = _serviceProvider.GetRequiredService<SellHandler>();
            _sellOrdersHandler = _serviceProvider.GetRequiredService<SellOrdersHandler>();
            _buyOrdersHandler = _serviceProvider.GetRequiredService<BuyOrdersHandler>();
            analyzer = _serviceProvider.GetRequiredService<ScreenAnalyzer>();

            // Добавляем шаблоны
            analyzer.ClearTemplates();
            analyzer.AddTemplate("main_buy", "assets/templates/ru/wide/main_buy.png");
            analyzer.AddTemplate("main_sell", "assets/templates/ru/wide/main_sell.png");
            analyzer.AddTemplate("main_create_buy_order", "assets/templates/ru/wide/main_create_buy_order.png");
            analyzer.AddTemplate("main_orders", "assets/templates/ru/wide/main_orders.png");
            analyzer.AddTemplate("main_suits", "assets/templates/ru/wide/main_suits.png");

            _regionManager = _serviceProvider.GetRequiredService<RegionManager>();
            _mainTextRegion = _regionManager.GetRect("buy", "title");
        }

        public async Task Run(HandlerContext context)
        { 
            var detected = analyzer.DetectCurrentWindow(_mainTextRegion);

            switch (detected)
            {
                case "main_buy":
                    //await _buyHandler.Run();
                    break;

                case "main_sell":
                    await _sellHandler.Run(context);
                    break;

                case "main_orders":
                    {
                        var from = Cursor.Position;
                        if(from.Y >= 600)
                            await _sellOrdersHandler.Run(context);
                        else
                            await _buyOrdersHandler.Run(context);
                        break;
                    }

                default:
                    await _buyHandler.Run(context);
                    break;
            }
        }
    }
}
