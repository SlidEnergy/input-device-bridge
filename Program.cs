using Microsoft.Extensions.DependencyInjection;
using tser;
using tser.Battle.Maps;
using tser.Screen;
using tser.Screen.Screenshots;

namespace tser
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<AppSettings>();
            services.AddSingleton<RegionManager>();
            services.AddSingleton<InputSimulator>();
            services.AddSingleton<ScreenAnalyzer>();

            services.AddTransient<MainActionHandler>();

            services.AddTransient<SpamQHandler>();
            services.AddTransient<SpamEHandler>();
            services.AddTransient<FastLootHandler>();
            services.AddTransient<GateHelperHandler>();

            services.AddTransient<Select43Handler>();
            services.AddTransient<Select52Handler>();
            services.AddTransient<Select53Handler>();
            services.AddTransient<Select61Handler>();
            services.AddTransient<Select62Handler>();

            services.AddTransient<BuyOrdersHandler>();
            services.AddTransient<NewBuyOrderHandler>();
            services.AddTransient<SellHandler>();
            services.AddTransient<SellOrdersHandler>();
            services.AddTransient<BuyHandler>();

            services.AddTransient<BuySellItemFullScreen>();
            services.AddTransient<BuySellItemQualityScreen>();
            services.AddTransient<MapScreen>();
            services.AddTransient<LootPersonScreen>();

            services.AddTransient<MainForm>();
            services.AddTransient<RegionManagerForm>();
            services.AddTransient<AskNameForm>();

            services.AddTransient<GateHelperForm>();

            services.AddSingleton<MapsManager>();


            var provider = services.BuildServiceProvider();

            ApplicationConfiguration.Initialize();

            Application.Run(provider.GetRequiredService<MainForm>());
        }
    }
}