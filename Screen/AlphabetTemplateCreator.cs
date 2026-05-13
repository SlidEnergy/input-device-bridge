using OpenCvSharp;
using System.Xml.Linq;

namespace tser.Screen
{
    internal class AlphabetTemplateCreator
    {
        public AlphabetTemplateCreator() { }

        public static async Task Parse(Mat src, string chars)
        {
            // grayscale
            var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // upscale
            Cv2.Resize(gray, gray, new OpenCvSharp.Size(), 4, 4);

            // threshold
            var thresh = new Mat();

            //Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);

            Cv2.Threshold(
                gray,
                thresh,
                180,
                255,
                ThresholdTypes.Binary);

            // remove noise
            //var kernel = Cv2.GetStructuringElement(
            //    MorphShapes.Rect,
            //    new OpenCvSharp.Size(2, 2));

            //Cv2.MorphologyEx(
            //    thresh,
            //    thresh,
            //    MorphTypes.Close,
            //    kernel);

            Cv2.ImShow("parse", thresh);

            // contours
            Cv2.FindContours(
                thresh,
                out OpenCvSharp.Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            var rects = contours
                .Select(c => Cv2.BoundingRect(c))
                .OrderBy(r => r.X)
                .ToList();
            rects = rects
                .Where(r => r.Width > 3 && r.Height > 10)
                
                .ToList();

            Console.WriteLine($"Found: {rects.Count}");

            //Cv2.DrawContours(thresh, contours, -1, Scalar.Red, 2);

            //    Cv2.ImShow("debug", thresh);

            //var debug = new Mat();
            //Cv2.CvtColor(thresh, debug, ColorConversionCodes.GRAY2BGR);

            //Cv2.DrawContours(debug, contours, -1, Scalar.Red, 2);

            //Cv2.ImShow("debug", debug);

            var dir = Path.Combine("assets", "templates", "chars", "wide");
            Directory.CreateDirectory(dir);



            for (int i = 0; i < rects.Count && i < chars.Length; i++)
            {
                var rect = rects[i];

                // padding
                rect.X = Math.Max(0, rect.X - 0);
                rect.Y = Math.Max(0, rect.Y - 0);

                rect.Width = Math.Min(
                    thresh.Width - rect.X,
                    rect.Width + 0);

                rect.Height = Math.Min(
                    thresh.Height - rect.Y,
                    rect.Height + 0);

                var roi = new Mat(thresh, rect);

                var output = Normalize(roi);

                Cv2.ImShow("char", output);
                await Task.Delay(500);

                //string name = Char.IsLower(chars[i]) ? ("" + chars[i] + chars[i]) : "" + chars[i];
                //string filename = $"assets/templates/chars/{(int)chars[i]}.png";
                var chr = (char)chars[i];
                var path = Path.Combine(dir, (int)chars[i] + ".png");

                output.SaveImage(path);
            }
        }

        private static Mat Normalize(Mat roi)
        {
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
                throw new Exception("size incorrect");

            // обрезаем ROI если он больше canvas
            int copyW = Math.Min(w, target);
            int copyH = Math.Min(h, target);

            var roiCropped = new Mat(roi, new Rect(0, 0, copyW, copyH));

            // вставка
            roiCropped.CopyTo(new Mat(output, new Rect(x, y, copyW, copyH)));

            return output;
        }
    }
}
