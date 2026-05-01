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
    public partial class AlertForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer;

        public AlertForm()
        {
            InitializeComponent();

            var original = (Image)Properties.Resources.ResourceManager.GetObject("alert");

            var icon = new Bitmap(original, new Size(48, 48));

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ShowInTaskbar = false;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;

            Width = icon.Width;
            Height = icon.Height;

            var pb = new PictureBox
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.AutoSize
            };

            Controls.Add(pb);

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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }
    }
}
