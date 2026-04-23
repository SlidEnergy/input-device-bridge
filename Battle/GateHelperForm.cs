using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tser.Battle.Maps;
using static System.Net.Mime.MediaTypeNames;

namespace tser
{
    internal partial class GateHelperForm : Form
    {
        private System.Windows.Forms.Timer _timer;
        private Point _anchor;
        private const int Threshold = 5;

        public GateHelperForm()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            textBox1.Text = text;
        }

        private string BuildAvaRoadSummary(AvaRoadEntry map)
        {
            if (map == null)
                return null;

            var chests = map.Data.Components
                .Where(c => c.Type.Equals("chest", StringComparison.OrdinalIgnoreCase));

            //var grouped = chests
            //    .GroupBy(c => new { c.Size, c.BgColor })
            //    .Select(g => new
            //    {
            //        Count = g.Count(),
            //        Size = g.Key.Size,
            //        Color = g.Key.BgColor
            //    })
            //    .OrderByDescending(x => x.Size)   // Big → Small
            //    .ThenBy(x => x.Color)
            //    .ToList();

            var sb = new StringBuilder();

            sb.AppendLine(map.Name);
            sb.AppendLine($"Road type: {map.Data.Type}");

            //foreach (var g in grouped)
            //{
            //    sb.AppendLine($"{g.Count} {g.Size} {g.Color}");
            //}

            var order = new[]
            {
                ("Big", "Gold"),
                ("Small", "Gold"),
                ("Big", "Blue"),
                ("Small", "Green")
            };

            var groupedDict = chests
                .GroupBy(c => (c.Size, c.BgColor, c.Tier))
                .ToDictionary(g => (g.Key.Size, g.Key.BgColor), g => (g.Key.Tier, g.Count()));

            foreach (var (size, color) in order)
            {
                if (groupedDict.TryGetValue((size, color), out var g))
                {
                    sb.AppendLine($"{g.Item2} {size} {color} {g.Tier}");
                }
            }

            return sb.ToString();
        }

        private string BuildMapSummary(Map map)
        {
            if (map == null)
                return null;

            var sb = new StringBuilder();

            var tierQuality = map.Quality != null
                ? $"{map.Tier}.{map.Quality}"
                : $"{map.Tier}";

            var center = ExtractCenterLevel(map.Type);

            var smuggler = map.IsSmugglersNetworkMarket ? "Yes" : "No";

            sb.AppendLine(map.DisplayName);
            sb.AppendLine(tierQuality);
            sb.AppendLine(map.PvpCategory);
            sb.AppendLine($"Center: {center}");
            sb.AppendLine($"Smuggler: {smuggler}");

            return sb.ToString();
        }

        private static int? ExtractCenterLevel(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;

            var parts = type.Split('_');
            if (int.TryParse(parts.Last(), out var value))
                return value;

            return null;
        }

        public void SetRoad(AvaRoadEntry road)
        {
            if (road == null)
                return;

            var text = BuildAvaRoadSummary(road);

            textBox1.Text = text;
        }

        public void SetMap(Map map)
        {
            if (map == null)
                return;

            var text = BuildMapSummary(map);

            textBox1.Text = text;
        }

        public void SetLocation(Point location)
        {
            this.Location = location;
        }

        public void InitAutoClose(Point anchor)
        {
            _anchor = anchor;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 50;
            _timer.Tick += CheckCursor;
            _timer.Start();
        }

        private void CheckCursor(object? sender, EventArgs e)
        {
            var cursor = Cursor.Position;

            // 1. курсор внутри формы
            if (this.Bounds.Contains(cursor))
                return;

            // 2. курсор рядом с anchor
            if (Math.Abs(cursor.X - _anchor.X) <= Threshold &&
                Math.Abs(cursor.Y - _anchor.Y) <= Threshold)
                return;

            // иначе закрываем
            CloseSafe();
        }

        private void CloseSafe()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            this.Close();
        }
    }
}
