using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class MarketActionHandler
    {
        private InputSimulator sim;
        private readonly AppSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private ScreenAnalyzer analyzer;
        private Rectangle _mainTextRegion;
        private BuyHandler _buyHandler;
        private NewBuyOrderHandler _newBuyOrderHandler;
        private SellHandler _sellHandler;
        private SellOrdersHandler _sellOrdersHandler;
        private BuyOrdersHandler _buyOrdersHandler;
        private RegionManager _regionManager;

        public MarketActionHandler(InputSimulator inputSimulator, AppSettings appSettings, IServiceProvider provider)
        {
            sim = inputSimulator;
            _settings = appSettings;
            _serviceProvider = provider;
            _buyHandler = _serviceProvider.GetRequiredService<BuyHandler>();
            _newBuyOrderHandler = _serviceProvider.GetRequiredService<NewBuyOrderHandler>();
            _sellHandler = _serviceProvider.GetRequiredService<SellHandler>();
            _sellOrdersHandler = _serviceProvider.GetRequiredService<SellOrdersHandler>();
            _buyOrdersHandler = _serviceProvider.GetRequiredService<BuyOrdersHandler>();
            analyzer = _serviceProvider.GetRequiredService<ScreenAnalyzer>();

            _regionManager = _serviceProvider.GetRequiredService<RegionManager>();
            _mainTextRegion = _regionManager.GetRect("buy", "title");
        }

        public async Task Run(HandlerContext context)
        { 
            var detected = analyzer.DetectCurrentWindow(_mainTextRegion, nameof(MarketActionHandler));

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
                    if(_settings.TradingSettings.FastBuy)
                        await _buyHandler.Run(context);
                    else
                        await _newBuyOrderHandler.Run(context);
                    break;
            }
        }
    }
}
