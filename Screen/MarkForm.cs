using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tser
{
    public partial class MarkForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer;

        public MarkForm()
        {
            InitializeComponent();

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ShowInTaskbar = false;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;

            Width = 48;
            Height = 48;

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Location = new Point(
                screen.Width / 2 - Width / 2,
                screen.Height / 2 - Height / 2
            );

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += (_, __) =>
            {
                _timer.Stop();
                Close();
            };
        }

        public void SetIcon(string name)
        {
            var original = (Image)Properties.Resources.ResourceManager.GetObject(name);

            var icon = new Bitmap(original, new Size(48, 48));

            var pb = new PictureBox
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            Controls.Add(pb);
        }

        public void SetPosition(Point position)
        {
            Location = position;
        }

        public void SetTimeout(int timeout)
        {
            _timer.Interval = timeout;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }
    }
}
