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
