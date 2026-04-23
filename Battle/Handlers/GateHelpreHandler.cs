using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using tser.Battle.Maps;
using tser.Screen.Screenshots;

namespace tser
{
    internal class GateHelperHandler
    {
        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private Rectangle _lootRegion = new Rectangle(815, 447, 265, 240);
        private int cellSize = 60;
        private MapScreen screen;
        private GateHelperForm form;
        private MapsManager mapsManager;

        public GateHelperHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, IServiceProvider serviceProvider)
        {
            _analyzer = screenAnalyzer;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
            screen = serviceProvider.GetRequiredService<MapScreen>();
            form = serviceProvider.GetRequiredService<GateHelperForm>();
            mapsManager = serviceProvider.GetRequiredService<MapsManager>();
        }

        public async Task Run(HandlerContext context)
        {
            var cursor = Cursor.Position;

            //GetCursorPos(out var cursor);

            var rectRight = new Rectangle(cursor.X + screen.GateTitleOffsetRight.X, cursor.Y + screen.GateTitleOffsetRight.Y, screen.GateTitleSizeRight.Width, screen.GateTitleSizeRight.Height);
            var rectLeft = new Rectangle(cursor.X + screen.GateTitleOffsetLeft.X, cursor.Y + screen.GateTitleOffsetLeft.Y, screen.GateTitleSizeLeft.Width, screen.GateTitleSizeLeft.Height);

            var t1 = _analyzer.GetTextWithBackground(rectRight);
            var t2 = _analyzer.GetTextWithBackground(rectLeft);

            t1 = CleanTitle(t1);
            t2 = CleanTitle(t2);

            var text = t1.Length > t2.Length ? t1 : t2;

            context.SynchronizationContext.Post(_ =>
            {
                Clipboard.SetText(text);
            },null);
            
            if (text.Contains('-'))
            {
                var road = mapsManager.SearchRoad(text);
                if (road != null)
                    form.SetRoad(road);
                else
                    form.SetText(text);
            }
            else
            {
                var map = mapsManager.SearchMap(text);
                if (map != null)
                    form.SetMap(map);
                else
                    form.SetText(text);
            }

            //var titleRect = GetTitleRectDirect(cursor);

            //string text = _analyzer.GetText(titleRect);


            ShowTooltip(context, new Point(cursor.X + 20, cursor.Y + 20));

            return;
        }

        private string CleanTitle(string text)
        {
            var cleaned = new string(
                text.Where(c =>
                    char.IsLetter(c) ||
                    c == '-' ||
                    c == ' ')
                .ToArray()).Trim();

            // схлопнуть пробелы
            //cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

            return cleaned;
        }

        //private Rectangle GetTitleRectDirect(Point cursor)
        //{
        //    bool right = cursor.X < Screen.PrimaryScreen.Bounds.Width / 2;

        //    var offset = right ? screen.GateTitleOffsetRight : screen.GateTitleOffsetLeft;

        //    return new Rectangle(
        //        cursor.X + offset.X,
        //        cursor.Y + offset.Y,
        //        screen.GateTitleSize.Width,
        //        screen.GateTitleSize.Height);
        //}

        private ToolTip _tooltip = new ToolTip();

        private void ShowTooltip(HandlerContext context, Point position)
        {
            context.SynchronizationContext.Post(_ => 
            { 
                form.SetLocation(position);
                form.Show();

                form.InitAutoClose(Cursor.Position);
            }, null);
        }
    }
}
