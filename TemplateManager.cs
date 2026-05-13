using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tser
{
    internal class TemplateManager
    {
        private Dictionary<string, Dictionary<string, Mat>> _templates = new();

        public TemplateManager()
        {
            AddTemplate(nameof(NewBuyOrderHandler), "Normal", "assets/templates/ru/wide/buy_sell_item_quality_normal.png");
            AddTemplate(nameof(NewBuyOrderHandler), "Good", "assets/templates/ru/wide/buy_sell_item_quality_good.png");
            AddTemplate(nameof(NewBuyOrderHandler), "Outstanding", "assets/templates/ru/wide/buy_sell_item_quality_outstanding.png");
            AddTemplate(nameof(NewBuyOrderHandler), "Excelent", "assets/templates/ru/wide/buy_sell_item_quality_excelent.png");
            AddTemplate(nameof(NewBuyOrderHandler), "Masterpiece", "assets/templates/ru/wide/buy_sell_item_quality_masterpiece.png");

            AddTemplate(nameof(MarketActionHandler), "main_buy", "assets/templates/ru/wide/main_buy.png");
            AddTemplate(nameof(MarketActionHandler), "main_sell", "assets/templates/ru/wide/main_sell.png");
            AddTemplate(nameof(MarketActionHandler), "main_create_buy_order", "assets/templates/ru/wide/main_create_buy_order.png");
            AddTemplate(nameof(MarketActionHandler), "main_orders", "assets/templates/ru/wide/main_orders.png");
            AddTemplate(nameof(MarketActionHandler), "main_suits", "assets/templates/ru/wide/main_suits.png");

            AddTemplate(nameof(FastLootHandler) + "_Title", "LootTitle", "assets/templates/ru/wide/loot_title.png");
            AddTemplate(nameof(FastLootHandler), "Empty1", "assets/templates/ru/wide/loot_empty1.png");
            AddTemplate(nameof(FastLootHandler), "Empty2", "assets/templates/ru/wide/loot_empty2.png");
            AddTemplate(nameof(FastLootHandler), "Broken1", "assets/templates/ru/wide/loot_broken1.png");
            AddTemplate(nameof(FastLootHandler), "Broken2", "assets/templates/ru/wide/loot_broken2.png");
            AddTemplate(nameof(FastLootHandler), "Broken3", "assets/templates/ru/wide/loot_broken3.png");
            AddTemplate(nameof(FastLootHandler), "Broken4", "assets/templates/ru/wide/loot_broken4.png");
            AddTemplate(nameof(FastLootHandler), "Broken5", "assets/templates/ru/wide/loot_broken5.png");
            AddTemplate(nameof(FastLootHandler), "Broken6", "assets/templates/ru/wide/loot_broken6.png");
            AddTemplate(nameof(FastLootHandler), "Broken7", "assets/templates/ru/wide/loot_broken7.png");
            AddTemplate(nameof(FastLootHandler), "Broken8", "assets/templates/ru/wide/loot_broken8.png");

            AddTemplate("chars", "0", "assets/templates/chars/wide/48.png");
            AddTemplate("chars", "1", "assets/templates/chars/wide/49.png");
            AddTemplate("chars", "2", "assets/templates/chars/wide/50.png");
            AddTemplate("chars", "3", "assets/templates/chars/wide/51.png");
            AddTemplate("chars", "4", "assets/templates/chars/wide/52.png");
            AddTemplate("chars", "5", "assets/templates/chars/wide/53.png");
            AddTemplate("chars", "6", "assets/templates/chars/wide/54.png");
            AddTemplate("chars", "7", "assets/templates/chars/wide/55.png");
            AddTemplate("chars", "8", "assets/templates/chars/wide/56.png");
            AddTemplate("chars", "9", "assets/templates/chars/wide/57.png");

            AddTemplate("chars", "A", "assets/templates/chars/wide/65.png");
            AddTemplate("chars", "B", "assets/templates/chars/wide/66.png");
            AddTemplate("chars", "C", "assets/templates/chars/wide/67.png");
            AddTemplate("chars", "D", "assets/templates/chars/wide/68.png");
            AddTemplate("chars", "E", "assets/templates/chars/wide/69.png");
            AddTemplate("chars", "F", "assets/templates/chars/wide/70.png");
            AddTemplate("chars", "G", "assets/templates/chars/wide/71.png");
            AddTemplate("chars", "H", "assets/templates/chars/wide/72.png");
            AddTemplate("chars", "I", "assets/templates/chars/wide/73.png");
            AddTemplate("chars", "J", "assets/templates/chars/wide/74.png");

            AddTemplate("chars", "K", "assets/templates/chars/wide/75.png");
            AddTemplate("chars", "L", "assets/templates/chars/wide/76.png");
            AddTemplate("chars", "M", "assets/templates/chars/wide/77.png");
            AddTemplate("chars", "N", "assets/templates/chars/wide/78.png");
            AddTemplate("chars", "O", "assets/templates/chars/wide/79.png");
            AddTemplate("chars", "P", "assets/templates/chars/wide/80.png");
            AddTemplate("chars", "Q", "assets/templates/chars/wide/81.png");
            AddTemplate("chars", "R", "assets/templates/chars/wide/82.png");
            AddTemplate("chars", "S", "assets/templates/chars/wide/83.png");
            AddTemplate("chars", "T", "assets/templates/chars/wide/84.png");
            AddTemplate("chars", "U", "assets/templates/chars/wide/85.png");
            AddTemplate("chars", "V", "assets/templates/chars/wide/86.png");
            AddTemplate("chars", "W", "assets/templates/chars/wide/87.png");
            AddTemplate("chars", "X", "assets/templates/chars/wide/88.png");
            AddTemplate("chars", "Y", "assets/templates/chars/wide/89.png");
            AddTemplate("chars", "Z", "assets/templates/chars/wide/90.png");

            AddTemplate("chars", "a", "assets/templates/chars/wide/97.png");
            AddTemplate("chars", "b", "assets/templates/chars/wide/98.png");
            AddTemplate("chars", "c", "assets/templates/chars/wide/99.png");
            AddTemplate("chars", "d", "assets/templates/chars/wide/100.png");
            AddTemplate("chars", "e", "assets/templates/chars/wide/101.png");
            AddTemplate("chars", "f", "assets/templates/chars/wide/102.png");
            AddTemplate("chars", "g", "assets/templates/chars/wide/103.png");
            AddTemplate("chars", "h", "assets/templates/chars/wide/104.png");
            AddTemplate("chars", "i", "assets/templates/chars/wide/105.png");
            AddTemplate("chars", "j", "assets/templates/chars/wide/106.png");
            AddTemplate("chars", "k", "assets/templates/chars/wide/107.png");
            AddTemplate("chars", "l", "assets/templates/chars/wide/108.png");
            AddTemplate("chars", "m", "assets/templates/chars/wide/109.png");
            AddTemplate("chars", "n", "assets/templates/chars/wide/110.png");
            AddTemplate("chars", "o", "assets/templates/chars/wide/111.png");
            AddTemplate("chars", "p", "assets/templates/chars/wide/112.png");
            AddTemplate("chars", "q", "assets/templates/chars/wide/113.png");
            AddTemplate("chars", "r", "assets/templates/chars/wide/114.png");
            AddTemplate("chars", "s", "assets/templates/chars/wide/115.png");
            AddTemplate("chars", "t", "assets/templates/chars/wide/116.png");
            AddTemplate("chars", "u", "assets/templates/chars/wide/117.png");
            AddTemplate("chars", "v", "assets/templates/chars/wide/118.png");
            AddTemplate("chars", "w", "assets/templates/chars/wide/119.png");
            AddTemplate("chars", "x", "assets/templates/chars/wide/120.png");
            AddTemplate("chars", "y", "assets/templates/chars/wide/121.png");
            AddTemplate("chars", "z", "assets/templates/chars/wide/122.png");

            //for (int i = 48; i <= 57; i++)
            //    AddTemplate("chars", ((char)i).ToString(), $"assets/templates/chars/wide/{i}.png");

            //for (int i = 65; i <= 90; i++)
            //    AddTemplate("chars", ((char)i).ToString(), $"assets/templates/chars/wide/{i}.png");

            //for (int i = 97; i <= 122; i++)
            //    AddTemplate("chars", ((char)i).ToString(), $"assets/templates/chars/wide/{i}.png");
        }

        public Dictionary<string, Mat> GetTemplates(string setName)
        {
            return _templates.TryGetValue(setName, out Dictionary<string, Mat>? value) ? value : null;
        }

        public void AddTemplate(string setName, string name, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine($"[WARN] Template file not found: {filePath}");
                return;
            }

            //var mat = Cv2.ImRead(filePath, ImreadModes.Unchanged);
            var mat = Cv2.ImRead(filePath, ImreadModes.Color);

            if (mat.Empty())
            {
                Console.WriteLine($"[ERROR] Failed to load template '{name}' from {filePath}");
                return;
            }

            try
            {
                // Если есть альфа-канал — убираем его
                if (mat.Channels() == 4)
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGRA2BGR);

                // Приводим к серому, чтобы ускорить поиск
                //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

                // Приводим к 8-битному типу, если нужно
                if (mat.Type() != MatType.CV_8U)
                    mat.ConvertTo(mat, MatType.CV_8U);

                if (!_templates.ContainsKey(setName))
                    _templates[setName] = new();

                _templates[setName][name] = mat;
                Console.WriteLine($"[OK] Template '{name}' loaded ({filePath}), size={mat.Width}x{mat.Height}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Error processing template '{name}': {ex.Message}");
            }
        }

        public Mat CreateTemplate(string name, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine($"[WARN] Template file not found: {filePath}");
                return null;
            }

            var mat = Cv2.ImRead(filePath, ImreadModes.Unchanged);

            if (mat.Empty())
            {
                Console.WriteLine($"[ERROR] Failed to load template '{name}' from {filePath}");
                return null;
            }

            try
            {
                // Если есть альфа-канал — убираем его
                if (mat.Channels() == 4)
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGRA2BGR);

                // Приводим к серому, чтобы ускорить поиск
                //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

                // Приводим к 8-битному типу, если нужно
                if (mat.Type() != MatType.CV_8U)
                    mat.ConvertTo(mat, MatType.CV_8U);

                return mat;
                Console.WriteLine($"[OK] Template '{name}' loaded ({filePath}), size={mat.Width}x{mat.Height}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Error processing template '{name}': {ex.Message}");
            }

            return null;
        }
    }
}
