using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;

namespace tser
{
    public class ScreenAnalyzer
    {
        private readonly Dictionary<string, Mat> _templates = new();
        private readonly double _threshold;
        private const int SELECT_LIST_TOTAL_LINES_ON_SCREEN = 6;

        public ScreenAnalyzer(double threshold = 0.8)
        {
            _threshold = threshold;
        }

        /// <summary>
        /// Добавляет шаблон для поиска.
        /// </summary>
        //public void AddTemplate(string name, string filePath)
        //{
        //    var mat = Cv2.ImRead(filePath, ImreadModes.Color);
        //    _templates[name] = mat;
        //}

        public void AddTemplate(string name, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine($"[WARN] Template file not found: {filePath}");
                return;
            }

            var mat = Cv2.ImRead(filePath, ImreadModes.Unchanged);

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
                Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

                // Приводим к 8-битному типу, если нужно
                if (mat.Type() != MatType.CV_8U)
                    mat.ConvertTo(mat, MatType.CV_8U);

                _templates[name] = mat;
                Console.WriteLine($"[OK] Template '{name}' loaded ({filePath}), size={mat.Width}x{mat.Height}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Error processing template '{name}': {ex.Message}");
            }
        }

        /// <summary>
        /// Делает снимок заданной области экрана и определяет, какой шаблон найден.
        /// </summary>
        public string? DetectCurrentWindow(Rectangle region)
        {
            using var screenMat = CaptureRegion(region);

            string? bestMatch = null;
            double bestScore = 0;

            foreach (var kvp in _templates)
            {
                using var result = new Mat();
                Cv2.MatchTemplate(screenMat, kvp.Value, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);
                //Cv2.ImWrite("debug_region.png", screenMat);
                //Cv2.ImWrite("debug_template.png", kvp.Value);

                if (maxVal > bestScore)
                {
                    bestScore = maxVal;
                    bestMatch = kvp.Key;
                }
            }

            return bestScore >= _threshold ? bestMatch : null;
        }

        /// <summary>
        /// Снимает изображение с экрана в виде Mat только в заданном прямоугольнике.
        /// </summary>
        //private static Mat CaptureRegion(Rectangle region)
        //{
        //    using var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format24bppRgb);
        //    using (var g = Graphics.FromImage(bmp))
        //        g.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size, CopyPixelOperation.SourceCopy);
        //    return BitmapConverter.ToMat(bmp);
        //}

        private static Mat CaptureRegion(Rectangle region)
        {
            // Создаем bitmap нужного размера
            using var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format24bppRgb);

            // Захватываем область экрана
            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size, CopyPixelOperation.SourceCopy);

            // Конвертируем в Mat
            var mat = BitmapConverter.ToMat(bmp);

            // Переводим в градации серого (BGR -> GRAY)
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

            // Приводим к 8-битному типу
            if (mat.Type() != MatType.CV_8U)
                mat.ConvertTo(mat, MatType.CV_8U);

            //Cv2.ImWrite("debug_region.png", mat);

            // Возвращаем готовый Mat (копия, не зависящая от using)
            return mat;
        }

        public int GetSelectedIndexOfSelectList(Rectangle region, bool isGraySelection)
        {
            // 1. Захватываем регион и конвертируем в серый
            using var listboxMat = CaptureRegion(region); // твоя CaptureRegion уже возвращает GRAY 8U

            // 2. Маска для яркого зеленого фона (диапазон подбирается под твой цвет)
            using var mask = new Mat();
            // Так как CaptureRegion уже в GRAY, используем диапазон яркости
            if(isGraySelection)
                Cv2.InRange(listboxMat, new Scalar(50), new Scalar(160), mask);
            else
                Cv2.InRange(listboxMat, new Scalar(90), new Scalar(140), mask);

            //Cv2.ImWrite(@"mask_debug.png", mask);

            // 3. Немного сглаживаем, чтобы убрать шум
            //Cv2.GaussianBlur(mask, mask, new OpenCvSharp.Size(3, 3), 0);

            // 4. Находим контуры выделения
            Cv2.FindContours(mask, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            var validContours = contours.Where(c => Cv2.ContourArea(c) > 50).ToArray();
            if (validContours.Length == 0)
                return -1; // нет выделенной строки

            //if (contours.Length == 0)
            //{
            //    Console.WriteLine("Нет выделенной строки");
            //    return 0;
            //}

            // 5. Берем верхнюю координату выделенной области
            int selectedY = validContours.Select(c => Cv2.BoundingRect(c).Y).Min();

            // 6. Вычисляем высоту строки и индекс
            int lineHeight = region.Height / SELECT_LIST_TOTAL_LINES_ON_SCREEN;
            int selectedIndex = (selectedY / lineHeight) + 1; // нумерация с 1

            return selectedIndex;
        }

        public int GetPrice(Rectangle region)
        {
            var text = GetText(region);

            string digitsOnly = new string(text.Where(c => char.IsDigit(c)).ToArray());
            Console.WriteLine(digitsOnly); // Например, "12345"

            var number = Int32.Parse(digitsOnly);

            return number;
        }

        public string GetText(Rectangle region)
        {
            var row = CaptureRegion(region);
  
            // Увеличиваем изображение для OCR
            //Mat resized = new Mat();
            //Cv2.Resize(row, resized, new OpenCvSharp.Size(row.Width * 2, row.Height * 2), 0, 0, InterpolationFlags.Linear);
            //Cv2.ImWrite("resized.png", resized);

            Mat processed = new Mat();
            //Cv2.BitwiseNot(resized, processed); // если текст темный, фон светлый
            //Cv2.GaussianBlur(processed, processed, new OpenCvSharp.Size(3, 3), 0); // убираем шум
            //Cv2.ImWrite("processed1.png", processed);

            // Адаптивная бинаризация (темный текст на светлом фоне)
            //Cv2.AdaptiveThreshold(row, processed, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, 15, 5);
            //Cv2.ImWrite("processed2.png", processed);

            // Морфология для удаления шумов
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            //Cv2.MorphologyEx(processed, processed, MorphTypes.Close, kernel);
            //Cv2.ImWrite("processed3.png", processed);

            // OCR
            using var ocr = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            ocr.SetVariable("tessedit_char_whitelist", "0123456789,");
            ocr.SetVariable("classify_bln_numeric_mode", "1"); // Tesseract “numeric mode”

            using var pix = MatToPix(row); // твой конвертер Mat -> Pix
            using var page = ocr.Process(pix);
            string text = page.GetText();

            return text;
        }

        private static Pix MatToPix(Mat mat)
        {
            byte[] bytes = mat.ImEncode(".png");
            return Pix.LoadFromMemory(bytes);
        }
    }
}
