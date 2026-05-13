using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using Tesseract;
using tser.Screen.Screenshots;
using System.Runtime;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Text.RegularExpressions;
using tser.Wgc;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX.Direct3D11;
using tser.Battle.Maps;
using System.Threading.Tasks;
using System.Threading;
using tser.Screen;

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
        private readonly IServiceProvider serviceProvider;
        private int LowHpIndex = 0;
        private CancellationTokenSource _cts;
        private HandlerContext context;
        private TemplateManager _templateManager;

        private System.Drawing.Point point;
        private TesseractEngine ocr;
        private ConcurrentDictionary<string, OpenCvSharp.Rect> enemies = new ConcurrentDictionary<string, OpenCvSharp.Rect>();
        private ConcurrentDictionary<string, DateTime> clickedEnemies = new ConcurrentDictionary<string, DateTime>();

        public MarkerHelperHandler(InputSimulator inputSimulator, AppSettings appSettings, ScreenAnalyzer screenAnalyzer, GroupPanelScreen screen, IServiceProvider serviceProvider)
        {
            _analyzer = screenAnalyzer;

            this.screen = screen;
            this.serviceProvider = serviceProvider;
            sim = inputSimulator;
            settings = appSettings;
            mover = new Mover(sim);
            _templateManager = serviceProvider.GetRequiredService<TemplateManager>();
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



        //public async Task WaitForCaptureToCompleteAsync(WgcCapture capture, GraphicsCaptureItem item, IDirect3DDevice device)
        //{
        //    var tcs = new TaskCompletionSource();

        //    context.SynchronizationContext.Post(_ =>
        //    {
        //        try
        //        {
        //            capture.Start(item, device);

        //            // Сигнализируем, что работа завершена
        //            tcs.TrySetResult();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Сигнализируем об ошибке
        //            tcs.TrySetException(ex);
        //        }
        //    }, null);

        //    // Ждём завершения
        //    await tcs.Task;
        //}

        WgcCapture _capture;

        public void SetCapture(WgcCapture capture)
        {
            _capture = capture;
        }

        public class Track
        {
            public int Id;

            public OpenCvSharp.Rect Box;
            public Point2f Center;

            public Point2f Velocity;

            public int MissedFrames;

            public float MaxDistance = 60f;

            public List<Point2f> History = new List<Point2f>();
        }

        Point2f GetCenter(OpenCvSharp.Rect r)
        {
            return new Point2f(
                r.X + r.Width / 2f,
                r.Y + r.Height / 2f
            );
        }

        public void Update(List<OpenCvSharp.Rect> detections)
        {
            var usedTracks = new HashSet<int>();
            var usedDetections = new HashSet<int>();

            // ----------------------------
            // 1. собираем ВСЕ пары track ↔ detection
            // ----------------------------
            var pairs = new List<(int trackIdx, int detIdx, double dist, Point2f detCenter)>();

            for (int ti = 0; ti < tracks.Count; ti++)
            {
                var t = tracks[ti];

                var predicted = new Point2f(
                    t.Center.X + t.Velocity.X,
                    t.Center.Y + t.Velocity.Y
                );

                for (int di = 0; di < detections.Count; di++)
                {
                    var c = GetCenter(detections[di]);

                    double dx = predicted.X - c.X;
                    double dy = predicted.Y - c.Y;

                    double dist = dx * dx + dy * dy;

                    pairs.Add((ti, di, dist, c));
                }
            }

            // ----------------------------
            // 2. сортируем по лучшему совпадению
            // ----------------------------
            var ordered = pairs
                .OrderBy(p => p.dist)
                .ToList();

            // ----------------------------
            // 3. greedy assignment (НО УЖЕ ГЛОБАЛЬНЫЙ)
            // ----------------------------
            foreach (var p in ordered)
            {
                if (usedTracks.Contains(p.trackIdx)) continue;
                if (usedDetections.Contains(p.detIdx)) continue;

                var t = tracks[p.trackIdx];
                var det = detections[p.detIdx];

                // порог (квадрат расстояния!)
                if (p.dist > t.MaxDistance * t.MaxDistance)
                    continue;

                var newCenter = p.detCenter;

                // velocity
                t.Velocity = newCenter - t.Center;

                // update state
                t.Center = newCenter;
                t.Box = det;
                t.MissedFrames = 0;

                // history
                t.History.Add(newCenter);
                if (t.History.Count > 10)
                    t.History.RemoveAt(0);

                usedTracks.Add(p.trackIdx);
                usedDetections.Add(p.detIdx);
            }

            // ----------------------------
            // 4. missed tracks
            // ----------------------------
            for (int i = 0; i < tracks.Count; i++)
            {
                if (!usedTracks.Contains(i))
                    tracks[i].MissedFrames++;
            }

            // ----------------------------
            // 5. create new tracks
            // ----------------------------
            for (int i = 0; i < detections.Count; i++)
            {
                if (usedDetections.Contains(i))
                    continue;

                var c = GetCenter(detections[i]);

                tracks.Add(new Track
                {
                    Id = nextId++,
                    Box = detections[i],
                    Center = c,
                    Velocity = new Point2f(0, 0),
                    MissedFrames = 0,
                    History = new List<Point2f> { c }
                });
            }

            // ----------------------------
            // 6. cleanup
            // ----------------------------
            tracks.RemoveAll(t => t.MissedFrames > 30);
        }

        private List<Track> tracks = new();
        private int nextId = 1;

        public async Task Loop(CancellationToken cancellationToken)
        {
            if (ocr != null)
            {
                ocr.Dispose();
            }

            // OCR
            ocr = new TesseractEngine("./assets/tessdata", "eng", EngineMode.Default);

            // Только латиница
            ocr.SetVariable("tessedit_char_whitelist",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            ocr.DefaultPageSegMode = PageSegMode.SingleLine;

            //var capture = new WgcCapture();



            try
            {
                // ===== LAYER 1 =====
                //var hwnd = WgcBootstrap.FindWindow(null, "Albion Online Client");
                //var hwnd = WgcBootstrap.FindWindow(null, "Albion Online Launcher");

                //var item = WgcBootstrap.CreateItemForWindow(hwnd);

                ////var device = WgcBootstrap.CreateDevice();

                //var device = Direct3D11Helper.CreateDevice();


                //await WaitForCaptureToCompleteAsync(capture, item, device);

                while (!cancellationToken.IsCancellationRequested)
                {
                    //if(string.IsNullOrEmpty(settings.BattleSettings.Name))
                    //{
                    //    await Task.Delay(2000);
                    //    continue;
                    //}

                    var frame = _capture.GetFrame();

                    var cropped = new Mat(frame, new OpenCvSharp.Rect(screenRegion.X, screenRegion.Y, screenRegion.Width, screenRegion.Height)).Clone();

                    var debug = cropped.Clone();

                    var squares = await FindSquares(cropped);

                    Update(squares);

                    foreach (var t in tracks)
                    {
                        Cv2.Rectangle(debug, t.Box, Scalar.Green, 2);
                        Cv2.PutText(debug, t.Id.ToString(), t.Box.TopLeft,
                            HersheyFonts.HersheySimplex, 0.5, Scalar.White);
                    }

                    await DetectAndShowMarker(cropped, squares, debug);

                    context.SynchronizationContext.Post(_ =>
                    {
                        Cv2.ImShow("debug", debug);
                    }, null);

                    //var screen = _analyzer.CaptureRegion(screenRegion);

                    //var squares = await FindSquares(screen);

                    //if (squares.Count() == 0)
                    //{
                    //    await Task.Delay(400);
                    //    continue;
                    //}

                    //squares = squares.Where(x => x.X != 905 && x.Y != 272).ToList();
                    //Cv2.ImShow("debug", screen);

                    

                    await Task.Delay(100);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {

            }
            finally
            {
                _capture.Stop();
            }
        }

        private async Task DetectAndShowMarker(Mat screen, List<OpenCvSharp.Rect> squares, Mat debug)
        {
            // Dictionary<string, System.Drawing.Point> names = new Dictionary<string, System.Drawing.Point>();
            enemies.Clear();

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
                using var letterRoi = new Mat(screen, new OpenCvSharp.Rect(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));

                // OCR preprocessing
                var processed = PreprocessForOCR(textRoi);
                using var processedletter = PreprocessForOCR(letterRoi);



                var rects = GetCharRects(processed);

                //var processedDebug = new Mat();
                //Cv2.CvtColor(processed, processedDebug, ColorConversionCodes.GRAY2BGR);

                //foreach (var r in rects)
                //    Cv2.Rectangle(processedDebug, r, Scalar.Red, 2);

                //context.SynchronizationContext.Post(_ =>
                //{
                //    Cv2.ImShow("text", processedDebug);

                //}, null);

                string text = await ReadByTemplates(processed);
                string letter = await ReadByTemplates(processedletter);

                // OCR
                //string text = ReadText(ocr, processed);
                //string letter = ReadText(ocr, processedletter);

                text = letter + text;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine(text);

                    // Overlay debug
                    Cv2.PutText(
                        debug,
                        text,
                        new OpenCvSharp.Point(rect.X, rect.Y - 10),
                        HersheyFonts.HersheySimplex,
                        0.6,
                        Scalar.Yellow,
                        2);
                }

                if (Normalize(text) == "caprainsiid")
                    continue;

                var point = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y - 48 + 70);
                //names.Add(Normalize(text), point);
                enemies.TryAdd(Normalize(text), rect);
            }

            //ShowTooltip(context, new System.Drawing.Point(Cursor.Position.X + 20, Cursor.Position.Y + 20), string.Join("\r\n", enemies.Keys.Select(text => Normalize(text))));

            //var find = FindClosest(settings.BattleSettings.Name, names);

            //if (find != null)
            //{
            //    point = find.Point;
            //    ShowAsterics();
            //}

            //context.SynchronizationContext.Post(_ =>
            //{
            //    Cv2.ImShow("debug", screen);
            //}, null);
        }

        async Task<string> ReadByTemplates(Mat img)
        {
            var bin = img.Clone();

            var chars = GetCharRects(bin);

            string result = "";

            foreach (var r in chars)
            {
                var roi = new Mat(bin, r);

                //var processedDebug = new Mat();
                //Cv2.CvtColor(processed, processedDebug, ColorConversionCodes.GRAY2BGR);

                //foreach (var r in rects)
                //    Cv2.Rectangle(processedDebug, r, Scalar.Red, 2);

                //context.SynchronizationContext.Post(_ =>
                //{
                //    using var show = roi.Clone();
                //    Cv2.ImShow(Guid.NewGuid().ToString(), show);

                //}, null);

                await Task.Delay(400);

                char c = MatchChar(roi);

                result += c;
            }

            return result;
        }

        Mat Normalize(Mat roi)
        {
            //var m = new Mat();

            //Cv2.CvtColor(src, m, ColorConversionCodes.BGR2GRAY);
            //Cv2.Resize(m, m, new OpenCvSharp.Size(24, 24));

            //return m;
            int target = 100;

            // canvas
            var output = new Mat(new OpenCvSharp.Size(target, target), MatType.CV_8UC1, Scalar.Black);

            // размеры ROI
            int w = roi.Width;
            int h = roi.Height;

            // позиция вставки (без масштабирования)
            int x = 0;
            int y = 0;

            // если хочешь центрировать без изменения размера
            x = Math.Max(0, (target - w) / 2);
            y = Math.Max(0, (target - h) / 2);

            if (w > target || h > target)
                return null;

            // обрезаем ROI если он больше canvas
            int copyW = Math.Min(w, target);
            int copyH = Math.Min(h, target);

            var roiCropped = new Mat(roi, new OpenCvSharp.Rect(0, 0, copyW, copyH));

            // вставка
            roiCropped.CopyTo(new Mat(output, new OpenCvSharp.Rect(x, y, copyW, copyH)));

            return output;
        }

        char MatchChar(Mat roi)
        {
            char bestChar = '?';

            var input = Normalize(roi);

            if (input == null)
                return bestChar;


            double bestScore = double.MaxValue;

            foreach (var tpl in _templateManager.GetTemplates("chars"))
            {
                Mat template = new();
                
                Cv2.CvtColor(tpl.Value, template, ColorConversionCodes.BGR2GRAY);
                double score = Cv2.Norm(input, template, NormTypes.L2);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestChar = tpl.Key[0];
                }
            }

            return bestChar;
        }

        List<OpenCvSharp.Rect> GetCharRects(Mat bin)
        {
            Cv2.FindContours(
                bin,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            return contours
                .Select(c => Cv2.BoundingRect(c))
                .Where(r => r.Width > 3 && r.Height > 10)
                .OrderBy(r => r.X)
                .ToList();
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
                4,
                4,
                InterpolationFlags.Linear);

            // Threshold
            Cv2.Threshold(
                resized,
                thresh,
                180,
                255,
                ThresholdTypes.Binary);

                        // remove noise
            //var kernel = Cv2.GetStructuringElement(
            //    MorphShapes.Rect,
            //    new OpenCvSharp.Size(3, 3));

            //Cv2.MorphologyEx(
            //    thresh,
            //    thresh,
            //    MorphTypes.Open,
            //    kernel);

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
            Mat mask = new Mat();
            Cv2.InRange(thresh,
                new Scalar(0, 0, 200),
                new Scalar(180, 40, 255),
                mask);

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

                //Cv2.Rectangle(src, rect, Scalar.Green, 2);
            }

            //Cv2.ImShow("thresh", thresh);
            //Cv2.ImShow("debug", src);

            //context.SynchronizationContext.Post(_ =>
            //{
            //    Cv2.ImShow("debug", src);
            //}, null);

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
            List<string> removeClickedEnemies = new List<string>();

            foreach (var key in enemies.Keys)
            {
                if (clickedEnemies.TryGetValue(key, out var dateStamp))
                {
                    if (DateTime.Now - dateStamp > TimeSpan.FromSeconds(20))
                    {
                        removeClickedEnemies.Add(key);
                        break;
                    }

                    continue;
                }


                if (enemies.TryGetValue(key, out var enemy))
                {
                    await SelectEnemy(enemy);
                    clickedEnemies.TryAdd(key, DateTime.Now);
                    break;
                }
            }

            foreach (var key in removeClickedEnemies)
            {
                clickedEnemies.TryRemove(key, out _);
            }
        }

        private async Task SelectEnemy(OpenCvSharp.Rect rect)
        {
            mover.MoveSmooth(rect.X + rect.Width + 25, rect.Y + rect.Height + 120);
        }

        private void ShowTooltip(HandlerContext context, System.Drawing.Point position, string text)
        {
            context.SynchronizationContext.Post(_ =>
            {
                var form = serviceProvider.GetRequiredService<GateHelperForm>();
                form.SetText(text);

                form.SetLocation(position);
                form.Show();

                form.InitAutoClose(Cursor.Position);
            }, null);
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
            return s.Split(' ')[0]
                .ToLowerInvariant()
                .Replace('l', 'i') // считаем l и i одинаковыми
                .Replace('t', 'r'); // считаем l и i одинаковыми
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
