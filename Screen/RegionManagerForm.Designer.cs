namespace tser
{
    partial class RegionManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            screenshotCanvas1 = new ScreenshotCanvas();
            panel1 = new Panel();
            createTemplateButton = new Button();
            regionListBox = new ListBox();
            screenshotListBox = new ListBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // screenshotCanvas1
            // 
            screenshotCanvas1.Anchor = AnchorStyles.None;
            screenshotCanvas1.Image = null;
            screenshotCanvas1.Location = new Point(0, 113);
            screenshotCanvas1.Name = "screenshotCanvas1";
            screenshotCanvas1.Regions = null;
            screenshotCanvas1.Size = new Size(1347, 534);
            screenshotCanvas1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(createTemplateButton);
            panel1.Controls.Add(regionListBox);
            panel1.Controls.Add(screenshotListBox);
            panel1.Controls.Add(screenshotCanvas1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1347, 761);
            panel1.TabIndex = 1;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;
            panel1.Resize += panel1_Resize;
            // 
            // createTemplateButton
            // 
            createTemplateButton.Location = new Point(1231, 100);
            createTemplateButton.Name = "createTemplateButton";
            createTemplateButton.Size = new Size(113, 23);
            createTemplateButton.TabIndex = 3;
            createTemplateButton.Text = "Create template";
            createTemplateButton.UseVisualStyleBackColor = true;
            createTemplateButton.Click += createTemplateButton_Click;
            // 
            // regionListBox
            // 
            regionListBox.FormattingEnabled = true;
            regionListBox.ItemHeight = 15;
            regionListBox.Location = new Point(1227, 0);
            regionListBox.Name = "regionListBox";
            regionListBox.Size = new Size(120, 94);
            regionListBox.TabIndex = 2;
            regionListBox.KeyDown += regionListBox_KeyDown;
            // 
            // screenshotListBox
            // 
            screenshotListBox.FormattingEnabled = true;
            screenshotListBox.ItemHeight = 15;
            screenshotListBox.Location = new Point(0, 0);
            screenshotListBox.Name = "screenshotListBox";
            screenshotListBox.Size = new Size(120, 94);
            screenshotListBox.TabIndex = 1;
            screenshotListBox.SelectedIndexChanged += screenshotListBox_SelectedIndexChanged;
            // 
            // RegionManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1347, 761);
            Controls.Add(panel1);
            Name = "RegionManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "RegionManagerForm";
            Load += RegionManagerForm_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ScreenshotCanvas screenshotCanvas1;
        private Panel panel1;
        private ListBox regionListBox;
        private ListBox screenshotListBox;
        private Button createTemplateButton;
    }
}