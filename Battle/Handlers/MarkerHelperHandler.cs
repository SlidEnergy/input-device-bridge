using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using Tesseract;
using tser.Screen.Screenshots;
using System.Runtime;

namespace tser
{
    internal class MarkerHelperHandler
    {
        private Rectangle screenRegion = new Rectangle(0, 70, 1920, 940);
        private int offsettY = 5;
        private int rowsCount = 4;

        private InputSimulator sim;
        private readonly AppSettings settings;
        private Mover mover;
        private ScreenAnalyzer _analyzer;
        private readonly GroupPanelScreen screen;

        private int LowHpIndex = 0;
        private CancellationTokenSource _cts;
        private HandlerContext context;

        private System.Drawing.Point point;
        private TesseractEngine ocr;

        public MarkerHelperHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, GroupPanelScreen screen)
        {
            _analyzer = screenAnalyzer;

            this.screen = screen;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
        }

        public Task Activate(HandlerContext context)
        {
            this.context = context;

            if (_cts != null && !_cts.IsCancellationRequested)
                return Task.CompletedTask;

            _cts = new CancellationTokenSource();
            _ = Loop(_cts.Token);
            return Task.CompletedTask;
        }

        public Task Deactivate(HandlerContext context)
        {
            this.context = context;

            _cts?.Cancel();
            return Task.CompletedTask;
        }

        public async Task Loop(CancellationToken cancellationToken)
        {
            if(ocr != null)
            {
                ocr.Dispose();
            }

            // OCR
            ocr = new TesseractEngine("./assets/tessdata", "eng", EngineMode.Default);

            // Только латиница
            ocr.SetVariable("tessedit_char_whitelist",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            ocr.DefaultPageSegMode = PageSegMode.SingleLine;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if(string.IsNullOrEmpty(settings.BattleSettings.Name))
                    {
                        await Task.Delay(2000);
                        continue;
                    }

                    var screen = _analyzer.CaptureRegion(screenRegion);

                    var squares = await FindSquares(screen);

                    //DetectAndShowMarker(screen, squares);



                    await Task.Delay(400);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {

            }
        }

        private void DetectAndShowMarker(Mat screen, List<OpenCvSharp.Rect> squares)
        {
            Dictionary<string, System.Drawing.Point> names = new Dictionary<string, System.Drawing.Point>();

            foreach (var rect in squares)
            {
                // Рисуем найденный квадрат
                //Cv2.Rectangle(screen, rect, Scalar.Lime, 2);

                // ROI справа от квадрата
                OpenCvSharp.Rect textRect = new OpenCvSharp.Rect(
                    rect.X + rect.Width,
                    rect.Y,
                    300,
                    rect.Height);

                //Cv2.Rectangle(screen, textRect, Scalar.Lime, 2);

                // Проверка границ
                if (textRect.Right >= screen.Width)
                    textRect.Width -= textRect.Right - screen.Width;

                if (textRect.Bottom >= screen.Height)
                    textRect.Height -= textRect.Bottom - screen.Height;

                if (textRect.X < 0)
                    textRect.X = 0;

                if (textRect.Y < 0)
                    textRect.Y = 0;

                using var textRoi = new Mat(screen, textRect);
                using var letterRoi = new Mat(screen, new OpenCvSharp.Rect(rect.X + 1, rect.Y - 1, rect.Width - 2, rect.Height - 2));

                // OCR preprocessing
                using var processed = PreprocessForOCR(textRoi);
                using var processedletter = PreprocessForOCR(letterRoi);

                // OCR
                string text = ReadText(ocr, processed);
                string letter = ReadText(ocr, processedletter);

                text = letter + text;

                //if (!string.IsNullOrWhiteSpace(text))
                //{
                //    Console.WriteLine(text);

                //    // Overlay debug
                //    Cv2.PutText(
                //        screen,
                //        text,
                //        new OpenCvSharp.Point(rect.X, rect.Y - 10),
                //        HersheyFonts.HersheySimplex,
                //        0.6,
                //        Scalar.Yellow,
                //        2);
                //}

                var point = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y - 48 + 70);
                names.Add(Normalize(text), point);
            }

            var find = FindClosest(settings.BattleSettings.Name, names);

            if (find != null)
            {
                point = find.Point;
                ShowAsterics();
            }

            //context.SynchronizationContext.Post(_ =>
            //{
            //    Cv2.ImShow("debug", screen);
            //}, null);
        }

        static Mat PreprocessForOCR(Mat src)
        {
            var gray = new Mat();
            var resized = new Mat();
            var thresh = new Mat();

            // Gray
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Увеличение
            Cv2.Resize(
                gray,
                resized,
                new OpenCvSharp.Size(),
                2,
                2,
                InterpolationFlags.Linear);

            // Threshold
            Cv2.Threshold(
                resized,
                thresh,
                180,
                255,
                ThresholdTypes.Binary);

            gray.Dispose();
            resized.Dispose();

            return thresh;
        }

        static string ReadText(
            TesseractEngine engine,
            Mat mat)
        {
            using var bmp = BitmapConverter.ToBitmap(mat);
            using var pix = MatToPix(bmp);
            using var page = engine.Process(pix);

            return page.GetText()
                .Trim()
                .Replace("\n", "")
                .Replace("\r", "");
        }

        private static Pix MatToPix(Bitmap bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return Pix.LoadFromMemory(ms.ToArray());
        }

        private async Task<List<OpenCvSharp.Rect>> FindSquares(Mat src)
        {
            var result = new List<OpenCvSharp.Rect>();
            using var gray = new Mat();
            using var thresh = new Mat();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Бинаризация
            Cv2.Threshold(
                gray,
                thresh,
                180,
                255,
                ThresholdTypes.Binary);

            //// Адаптивная бинаризация
            //Cv2.AdaptiveThreshold(
            //    gray,
            //    thresh,
            //    255,
            //    AdaptiveThresholdTypes.GaussianC,
            //    ThresholdTypes.Binary,
            //    11,
            //    2);

            //// Морфологическая обработка
            //using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            //Cv2.MorphologyEx(thresh, thresh, MorphTypes.Close, kernel);

            Cv2.FindContours(
                thresh,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            //Cv2.DrawContours(src, contours, -1, Scalar.Red, 2);

            //context.SynchronizationContext.Post(_ =>
            //{
            //    Cv2.ImShow("debug", src);
            //}, null);

            //await Task.Delay(4000);

            foreach (var contour in contours)
            {
                //if (!Cv2.IsContourConvex(approx))
                //    continue;

                var rect = Cv2.BoundingRect(contour);

                if (rect.Width < 12 || rect.Height < 20 || rect.Height > 30 || rect.Width > 25)
                    continue;

                // Aspect ratio
                float ratio = (float)rect.Width / rect.Height;

                if (ratio < 0.5f || ratio > 1.5f)
                    continue;

                //// Площадь контура
                //double area = Cv2.ContourArea(contour);
                //if (area < 100)
                //    continue;

                // Площадь прямоугольника (не контура)
                double rectArea = rect.Width * rect.Height;

                if (rectArea < 300)
                    continue;

                //// Согласованность площади
                //double contourArea = Cv2.ContourArea(contour);
                //double fillRatio = contourArea / rectArea;

                //if (fillRatio < 0.7)  // Исключаем сильно искажённые контуры
                //{

                //    Cv2.DrawContours(src, new[] { contour }, -1, Scalar.Red, 2);

                //    context.SynchronizationContext.Post(_ =>
                //    {
                //        Cv2.ImShow("debug", src);
                //    }, null);

                //    await Task.Delay(1000);

                //    continue;
                //}

                // Углы (проверка на прямоугольники)
                OpenCvSharp.Point[] approx = Cv2.ApproxPolyDP(contour, 0.02 * Cv2.ArcLength(contour, true), true);

                if (approx.Length < 4)
                    continue;

                double maxCosine = 0;

                for (int i = 0; i < 4; i++)
                {
                    double cosine = Math.Abs(
                        Angle(
                            approx[(i + 1) % 4],
                            approx[(i + 3) % 4],
                            approx[i]));

                    maxCosine = Math.Max(maxCosine, cosine);
                }

                // 90° => cosine ~ 0
                if (maxCosine > 0.3)
                    continue;
                result.Add(rect);
            }

            //Cv2.ImShow("thresh", thresh);
            //Cv2.ImShow("debug", src);

            return result;
        }

        //private async Task<List<OpenCvSharp.Rect>> FindSquares(Mat src)
        //{
        //    var result = new List<OpenCvSharp.Rect>();

        //    using var gray = new Mat();
        //    using var blur = new Mat();
        //    using var edges = new Mat();

        //    // Gray
        //    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

        //    // Blur
        //    Cv2.GaussianBlur(gray, blur, new  OpenCvSharp.Size(3, 3), 0);

        //    // Edges
        //    Cv2.Canny(blur, edges, 30, 100);

        //    // Contours
        //    Cv2.FindContours(
        //        edges,
        //        out OpenCvSharp.Point[][] contours,
        //        out _,
        //        RetrievalModes.List,
        //        ContourApproximationModes.ApproxSimple);

        //    //Cv2.DrawContours(src, contours, -1, Scalar.Red, 2);

        //    //context.SynchronizationContext.Post(_ =>
        //    //{
        //    //    Cv2.ImShow("debug", src);
        //    //}, null);

        //    //await Task.Delay(4000);

        //    foreach (var contour in contours)
        //    {
        //        double perimeter = Cv2.ArcLength(contour, true);

        //        OpenCvSharp.Point[] approx = Cv2.ApproxPolyDP(
        //            contour,
        //            0.02 * perimeter,
        //            true);

        //        //Cv2.DrawContours(src, new[] { contour }, -1, Scalar.Red, 2);
        //        //foreach(var p in approx)
        //        //Cv2.Circle(src, p, 2, Scalar.Red, 1);

        //        //context.SynchronizationContext.Post(_ =>
        //        //{
        //        //    Cv2.ImShow("debug", src);
        //        //}, null);

        //        //await Task.Delay(1000);

        //        // 4 угла
        //        if (approx.Length != 4)
        //            continue;

        //        //if (!Cv2.IsContourConvex(approx))
        //        //    continue;

        //        //double maxCosine = 0;

        //        //for (int i = 0; i < 4; i++)
        //        //{
        //        //    double cosine = Math.Abs(
        //        //        Angle(
        //        //            approx[(i + 1) % 4],
        //        //            approx[(i + 3) % 4],
        //        //            approx[i]));

        //        //    maxCosine = Math.Max(maxCosine, cosine);
        //        //}

        //        //// 90° => cosine ~ 0
        //        //if (maxCosine > 0.3)
        //        //    continue;

        //        OpenCvSharp.Rect rect = Cv2.BoundingRect(approx);

        //        //double rectArea = rect.Width * rect.Height;
        //        //double contourArea = Cv2.ContourArea(approx);

        //        //double fill = contourArea / rectArea;

        //        //if (fill < 0.7)
        //        //    continue;

        //        //Cv2.Rectangle(src, rect, Scalar.Red, 2);

        //        //context.SynchronizationContext.Post(_ =>
        //        //{
        //        //    Cv2.ImShow("debug", src);
        //        //}, null);

        //        //await Task.Delay(1000);

        //        // Размер
        //        //if (rect.Width < 12 || rect.Width > 60)
        //        //    continue;

        //        //// Aspect ratio
        //        //float ratio = (float)rect.Width / rect.Height;

        //        //if (ratio < 0.5f || ratio > 1.5f)
        //        //    continue;

        //        //// Площадь
        //        //double area = Cv2.ContourArea(contour);

        //        //if (area < 300)
        //        //    continue;

        //        result.Add(rect);
        //    }

        //    return result;
        //}

        static double Angle(OpenCvSharp.Point pt1, OpenCvSharp.Point pt2, OpenCvSharp.Point pt0)
        {
            double dx1 = pt1.X - pt0.X;
            double dy1 = pt1.Y - pt0.Y;
            double dx2 = pt2.X - pt0.X;
            double dy2 = pt2.Y - pt0.Y;

            return (dx1 * dx2 + dy1 * dy2) /
                   Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2));
        }

        private void ShowAsterics()
        {
            context.SynchronizationContext.Post(_ =>
            {
                var form = new MarkForm();
                form.SetIcon("skull");
                form.SetPosition(point);
                form.SetTimeout(200);
                form.Show();
            }, null);
        }

        public async Task Run(HandlerContext context)
        {
            
        }

        public dynamic FindClosest(string input, Dictionary<string, System.Drawing.Point> dict)
        {
            input = Normalize(input);

            return dict
                .Select(k => new { Key = k.Key, Dist = Levenshtein(k.Key, input), Point = k.Value })
                .OrderBy(x => x.Dist)
                .FirstOrDefault(x => x.Dist <= 2); // порог подбираешь
        }

        private string Normalize(string s)
        {
            return s
                .ToLowerInvariant()
                .Replace('l', 'i'); // считаем l и i одинаковыми
        }

        public static int Levenshtein(string a, string b)
        {
            int n = a.Length;
            int m = b.Length;

            var dp = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                dp[i, 0] = i;

            for (int j = 0; j <= m; j++)
                dp[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1,     // удаление
                                 dp[i, j - 1] + 1),    // вставка
                        dp[i - 1, j - 1] + cost        // замена
                    );
                }
            }

            return dp[n, m];
        }
    }
}
