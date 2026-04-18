using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tser.Screen;

namespace tser
{
    internal partial class ScreenshotCanvas : UserControl
    {
        public Image Image { get; set; }
        public Dictionary<string , Region> Regions { get; set; }

        //public RegionManager RegionManager { get; set; }

        public event EventHandler<Region> NewRegion;

        public ScreenshotCanvas()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Image == null) return;

            // центрирование
            int offsetX = Math.Max(0, (ClientSize.Width - Image.Width) / 2);
            int offsetY = Math.Max(0, (ClientSize.Height - Image.Height) / 2);

            var imgRect = new Rectangle(offsetX, offsetY, Image.Width, Image.Height);

            e.Graphics.DrawImage(Image, imgRect);

            using var pen = new Pen(Color.Black, 2);
            var brush = new SolidBrush(Color.Black);
            var font = new Font("Segoe UI", 10);
            var bg = new SolidBrush(Color.FromArgb(150, Color.White));

            if (Regions != null)
            {
                foreach (var v in Regions.Values)
                {
                    var r = v.Rect;
                    var drawRect = new Rectangle(
                        r.X + offsetX,
                        r.Y + offsetY,
                        r.Width,
                        r.Height);

                    e.Graphics.DrawRectangle(pen, drawRect);
                    var textPoint = new PointF(
                        r.X + offsetX,
                        r.Y + offsetY - 16); // чуть выше прямоугольника

                    var size = e.Graphics.MeasureString(v.Name, font);

                    var rect = new RectangleF(
                        r.X + offsetX,
                        r.Y + offsetY - size.Height,
                        size.Width,
                        size.Height);

                    e.Graphics.FillRectangle(bg, rect);
                    e.Graphics.DrawString(v.Name, font, brush, textPoint);
                }
            }

            // текущий (в процессе рисования)
            if (_drawing)
            {
                var drawRect = new Rectangle(
                    _current.X + offsetX,
                    _current.Y + offsetY,
                    _current.Width,
                    _current.Height);

                e.Graphics.DrawRectangle(pen, drawRect);
            }
        }

        private Point _start;
        private Rectangle _current;
        private bool _drawing;
        private Point _dragStart;
        private Point _dragStartScreen;
        private bool _dragging;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _drawing = true;
                _start = e.Location;
            }
            if (e.Button == MouseButtons.Middle)
            {
                _dragging = true;

                _dragStartScreen = Cursor.Position; // экранные координаты
                _dragStart = this.Location;     // начальная позиция контрола
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_drawing)
            {
                _current = new Rectangle(
                    Math.Min(_start.X, e.X),
                    Math.Min(_start.Y, e.Y),
                    Math.Abs(_start.X - e.X),
                    Math.Abs(_start.Y - e.Y));

                Invalidate();
            }


            if (_dragging)
            {
                var currentScreen = Cursor.Position;

                var dx = currentScreen.X - _dragStartScreen.X;
                var dy = currentScreen.Y - _dragStartScreen.Y;

                this.Location = new Point(
                    _dragStart.X + dx,
                    _dragStart.Y + dy);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _drawing = false;
            _dragging = false;

            if (_current.Width < 0 || _current.Height < 0)
            {
                Invalidate();
                return;
            }

            using var form = new AskNameForm();
            form.ShowDialog();

            var region = new Region(form.Name, _current);
            Regions.Add(form.Name, region);
            NewRegion?.Invoke(this, region);

            Invalidate();
        }
    }
}
