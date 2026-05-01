using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Text;
using Tesseract;

namespace tser
{
    internal class ScreenAnalyzer: IDisposable
    {
        private readonly double _threshold;
        private const int SELECT_LIST_TOTAL_LINES_ON_SCREEN = 6;
        private TesseractEngine numericOcr;
        private TesseractEngine textOcr;
        private TemplateManager _templateManager;

        public ScreenAnalyzer(TemplateManager templateManager)
        {
            double threshold = 0.2;
            _threshold = threshold;
            _templateManager = templateManager;
        }

        public void Init()
        {
            // OCR
            numericOcr = new TesseractEngine(@"./assets/tessdata", "eng", EngineMode.Default);
            numericOcr.SetVariable("tessedit_char_whitelist", "0123456789,");
            numericOcr.SetVariable("classify_bln_numeric_mode", "1"); // Tesseract “numeric mode”

            textOcr = new TesseractEngine(@"./assets/tessdata", "eng", EngineMode.Default);
            textOcr.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789- ");
            //textOcr.SetVariable("tessedit_char_whitelist", "");
            textOcr.SetVariable("classify_bln_numeric_mode", "0");
            textOcr.DefaultPageSegMode = PageSegMode.SingleLine;
        }

        public string? DetectCurrentWindow(Rectangle region, string source)
        {
            var templates = _templateManager.GetTemplates(source);

            if (templates == null)
                return null;

            return DetectCurrentWindow(region, templates);
        }

        /// <summary>
        /// Делает снимок заданной области экрана и определяет, какой шаблон найден.
        /// </summary>
        public string? DetectCurrentWindow(Rectangle region, Dictionary<string, Mat> templates)
        {
            using var screenMat = CaptureRegion(region);

            string? bestMatch = null;
            double bestScore = 0;

            foreach (var kvp in templates)
            {
                using var result = new Mat();
                Cv2.MatchTemplate(screenMat, kvp.Value, result, TemplateMatchModes.SqDiffNormed);
                Cv2.MinMaxLoc(result, out _, out double minVal, out _, out _);
                //Cv2.ImWrite("debug_region.png", screenMat);
                //Cv2.ImWrite("debug_template.png", kvp.Value);

                if (minVal < bestScore || bestMatch == null)
                {
                    bestScore = minVal;
                    bestMatch = kvp.Key;
                }

                //if (maxVal > bestScore)
                //{
                //    bestScore = maxVal;
                //    bestMatch = kvp.Key;
                //}
            }

            return bestScore <= 0.2 ? bestMatch : null;
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

        public Mat CaptureRegion(Rectangle region)
        {
            // Создаем bitmap нужного размера
            using var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format24bppRgb);

            // Захватываем область экрана
            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size, CopyPixelOperation.SourceCopy);

            // Конвертируем в Mat
            var mat = BitmapConverter.ToMat(bmp);

            // Переводим в градации серого (BGR -> GRAY)
            //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

            // Приводим к 8-битному типу
            if (mat.Type() != MatType.CV_8U)
                mat.ConvertTo(mat, MatType.CV_8U);

            Cv2.ImWrite("debug_region.png", mat);

            // Возвращаем готовый Mat (копия, не зависящая от using)
            return mat;
        }

        public int GetSelectedIndexOfSelectList(Rectangle region, bool isGraySelection)
        {
            // 1. Захватываем регион и конвертируем в серый
            using var listboxMat = CaptureRegion(region); // твоя CaptureRegion уже возвращает GRAY 8U

            using var gray = new Mat();
            Cv2.CvtColor(listboxMat, gray, ColorConversionCodes.BGR2GRAY);

            // 2. Маска для яркого зеленого фона (диапазон подбирается под твой цвет)
            using var mask = new Mat();
            // Так как CaptureRegion уже в GRAY, используем диапазон яркости
            if(isGraySelection)
                Cv2.InRange(gray, new Scalar(50), new Scalar(160), mask);
            else
                Cv2.InRange(gray, new Scalar(90), new Scalar(140), mask);

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
            var text = GetNumericText(region);

            string digitsOnly = new string(text.Where(c => char.IsDigit(c)).ToArray());
            Debug.WriteLine(digitsOnly); // Например, "12345"

            var number = Int32.Parse(digitsOnly);

            return number;
        }

        public string GetNumericText(Rectangle region)
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

            using var pix = MatToPix(row); // твой конвертер Mat -> Pix
            using var page = numericOcr.Process(pix);
            string text = page.GetText();

            return text;
        }

        public string GetText(Rectangle region)
        {
            var row = CaptureRegion(region);

            using var gray = new Mat();
            Cv2.CvtColor(row, gray, ColorConversionCodes.BGR2GRAY);

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

            using var pix = MatToPix(gray); // твой конвертер Mat -> Pix
            using var page = textOcr.Process(pix);
            string text = page.GetText();

            return text;
        }

        public string GetTextWithBackground(Rectangle region)
        {
            var row = CaptureRegion(region);

            using var gray = new Mat();
            Cv2.CvtColor(row, gray, ColorConversionCodes.BGR2GRAY);

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

            // 1. blur
            //Cv2.GaussianBlur(row, processed, new OpenCvSharp.Size(3, 3), 0);
            //Cv2.ImWrite("processed1.png", processed);

            // 2. выделение светлого текста
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
            Cv2.MorphologyEx(gray, processed, MorphTypes.TopHat, kernel);
            Cv2.ImWrite("processed2.png", processed);

            // мягкий порог (ключевой момент — НЕ высокий)
            //Cv2.Threshold(processed, processed, 120, 255, ThresholdTypes.Binary);
            //Cv2.ImWrite("processed3.png", processed);

            Cv2.Normalize(processed, processed, 0, 255, NormTypes.MinMax);

            //var k = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 1));
            //Cv2.Erode(processed, processed, k);

            // 3. бинаризация
            //Cv2.Threshold(processed, processed, 180, 255, ThresholdTypes.Binary);
            //Cv2.ImWrite("processed3.png", processed);
            // 4. усиление
            //var kernel2 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            //Cv2.MorphologyEx(processed, processed, MorphTypes.Close, kernel2);
            //Cv2.ImWrite("processed4.png", processed);
            // 5. upscale
            //Cv2.Resize(processed, processed, new OpenCvSharp.Size(), 2, 2, InterpolationFlags.Nearest);


            // debug
            Cv2.ImWrite("processed.png", processed);

            using var pix = MatToPix(processed); // твой конвертер Mat -> Pix
            using var page = textOcr.Process(pix);
            string text = page.GetText();

            return text;
        }

        private static Pix MatToPix(Mat mat)
        {
            byte[] bytes = mat.ImEncode(".png");
            return Pix.LoadFromMemory(bytes);
        }

        public void Dispose()
        {
            numericOcr?.Dispose();
        }
    }
}
