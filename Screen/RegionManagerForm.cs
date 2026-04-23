using Microsoft.Extensions.DependencyInjection;
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
    internal partial class RegionManagerForm : Form
    {
        private readonly RegionManager _regionManager;
        private readonly IServiceProvider serviceProvider;

        private string SelectedScreenshot => (string)screenshotListBox.SelectedItem;
        private string SelectedRegion => (string)regionListBox.SelectedItem;

        public RegionManagerForm(RegionManager regionManager, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _regionManager = regionManager;
            this.serviceProvider = serviceProvider;
            //screenshotCanvas1.RegionManager = regionManager;
            screenshotCanvas1.NewRegion += ScreenshotCanvas1_NewRegion;
        }

        private void ScreenshotCanvas1_NewRegion(object? sender, Region e)
        {
            _regionManager.AddRegion((string)screenshotListBox.SelectedItem, e);
            regionListBox.Items.Add(e.Name);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                PasteImageFromClipboard();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void PasteImageFromClipboard()
        {
            if (!Clipboard.ContainsImage())
                return;

            var img = GetClipboardImageSafe();

            if (img == null)
                return;

            // важно: делаем копию (буфер может освободиться)
            var bmp = new Bitmap(img);

            SetImage(bmp);

            using var form = serviceProvider.GetRequiredService<AskNameForm>();

            form.ShowDialog();

            _regionManager.AddScreenshot(form.Name, bmp);

            screenshotListBox.Items.Add(form.Name);

            screenshotListBox.SelectedIndex = screenshotListBox.Items.Count - 1;
        }

        private Image GetClipboardImageSafe()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (Clipboard.ContainsImage())
                        return Clipboard.GetImage();
                }
                catch
                {
                    Thread.Sleep(50);
                }
            }
            return null;
        }

        private void SetImage(Image image)
        {
            screenshotCanvas1.Image = image;

            // подгоняем размер canvas под картинку + центрирование
            //screenshotCanvas1.Size = new Size(
            //    Math.Max(bmp.Width, panel1.ClientSize.Width),
            //    Math.Max(bmp.Height, panel1.ClientSize.Height));

            screenshotCanvas1.Size = image.Size;

            CenterCanvas();

            screenshotCanvas1.Invalidate();
        }

        private void CenterCanvas()
        {
            var panel = panel1;

            int x = (panel.ClientSize.Width - screenshotCanvas1.Width) / 2;
            int y = (panel.ClientSize.Height - screenshotCanvas1.Height) / 2;

            screenshotCanvas1.Location = new Point(x, y);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            //CenterCanvas();
        }



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void RegionManagerForm_Load(object sender, EventArgs e)
        {
            var screenshots = _regionManager.GetScreenshots();

            if (screenshots != null && screenshots.Length > 0)
            {
                screenshotListBox.Items.Clear();
                screenshotListBox.Items.AddRange(screenshots);

                screenshotListBox.SelectedIndex = 0;

                var image = _regionManager.GetScreenshot(SelectedScreenshot);
                SetImage(image);

                var regions = _regionManager.GetScreenshotRegions(SelectedScreenshot);
                screenshotCanvas1.Regions = regions;

                regionListBox.Items.Clear();
                regionListBox.Items.AddRange(regions.Keys.ToArray());

                if(regions.Count > 0)
                    regionListBox.SelectedIndex = 0;
            }
        }

        private void screenshotListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var image = _regionManager.GetScreenshot(SelectedScreenshot);
            SetImage(image);

            var regions = _regionManager.GetScreenshotRegions(SelectedScreenshot);
            screenshotCanvas1.Regions = regions;

            regionListBox.Items.Clear();
            regionListBox.Items.AddRange(regions.Keys.ToArray());

            if(regions.Count > 0)
                regionListBox.SelectedIndex = 0;
        }

        private void regionListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                _regionManager.DeleteRegion(SelectedScreenshot, SelectedRegion);
                regionListBox.Items.Remove(SelectedRegion);

                e.Handled = true;
            }
        }

        private void createTemplateButton_Click(object sender, EventArgs e)
        {
            if (SelectedScreenshot == null || SelectedRegion == null)
                return;

            var sourceImage = screenshotCanvas1.Image;

            var regions = screenshotCanvas1.Regions;

            if (!regions.ContainsKey(SelectedRegion))
                return;

            var rect = regions[SelectedRegion].Rect;

            // вырезаем
            using var bmp = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(sourceImage,
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    rect,
                    GraphicsUnit.Pixel);
            }

            // спрашиваем имя
            using var form = serviceProvider.GetRequiredService<AskNameForm>();

            form.ShowDialog();

            var name = form.Name.Trim();

            // путь
            var dir = Path.Combine("assets", "templates", "ru", "wide");
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, name + ".png");

            // сохраняем
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
