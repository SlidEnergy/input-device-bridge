namespace tser
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            bootsellButton = new Button();
            testButton = new Button();
            buyAndSellRadioButton = new RadioButton();
            spamQRadioButton = new RadioButton();
            noneRadioButton = new RadioButton();
            calibrateButton = new Button();
            numPadCheckBox = new CheckBox();
            spamERadioButton = new RadioButton();
            allowedBestPriceOrderPositionNumericUpDown = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            fastLootRadioButton = new RadioButton();
            newBuyOrderRadioButton = new RadioButton();
            allLootStrategyRadioButton = new RadioButton();
            bestLootStrategyRadioButton = new RadioButton();
            regionManagerButton = new Button();
            gateHelperRadioButton = new RadioButton();
            panel1 = new Panel();
            openLootWindowCheckBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)allowedBestPriceOrderPositionNumericUpDown).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bootsellButton
            // 
            bootsellButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bootsellButton.Location = new Point(174, 390);
            bootsellButton.Name = "bootsellButton";
            bootsellButton.Size = new Size(75, 23);
            bootsellButton.TabIndex = 1;
            bootsellButton.Text = "bootsell";
            bootsellButton.UseVisualStyleBackColor = true;
            bootsellButton.Click += bootsellButton_Click;
            // 
            // testButton
            // 
            testButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            testButton.Location = new Point(93, 390);
            testButton.Name = "testButton";
            testButton.Size = new Size(75, 23);
            testButton.TabIndex = 2;
            testButton.Text = "test";
            testButton.UseVisualStyleBackColor = true;
            testButton.Click += testButton_Click;
            // 
            // buyAndSellRadioButton
            // 
            buyAndSellRadioButton.AutoSize = true;
            buyAndSellRadioButton.Checked = true;
            buyAndSellRadioButton.Location = new Point(12, 37);
            buyAndSellRadioButton.Name = "buyAndSellRadioButton";
            buyAndSellRadioButton.Size = new Size(79, 19);
            buyAndSellRadioButton.TabIndex = 3;
            buyAndSellRadioButton.TabStop = true;
            buyAndSellRadioButton.Text = "Buy && Sell";
            buyAndSellRadioButton.UseVisualStyleBackColor = true;
            // 
            // spamQRadioButton
            // 
            spamQRadioButton.AutoSize = true;
            spamQRadioButton.Location = new Point(12, 169);
            spamQRadioButton.Name = "spamQRadioButton";
            spamQRadioButton.Size = new Size(67, 19);
            spamQRadioButton.TabIndex = 4;
            spamQRadioButton.Text = "Spam Q";
            spamQRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            noneRadioButton.AutoSize = true;
            noneRadioButton.Location = new Point(12, 12);
            noneRadioButton.Name = "noneRadioButton";
            noneRadioButton.Size = new Size(54, 19);
            noneRadioButton.TabIndex = 5;
            noneRadioButton.Text = "None";
            noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // calibrateButton
            // 
            calibrateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            calibrateButton.Location = new Point(12, 390);
            calibrateButton.Name = "calibrateButton";
            calibrateButton.Size = new Size(75, 23);
            calibrateButton.TabIndex = 6;
            calibrateButton.Text = "Calibrate";
            calibrateButton.UseVisualStyleBackColor = true;
            calibrateButton.Click += calibrateButton_Click;
            // 
            // numPadCheckBox
            // 
            numPadCheckBox.AutoSize = true;
            numPadCheckBox.Location = new Point(30, 62);
            numPadCheckBox.Name = "numPadCheckBox";
            numPadCheckBox.Size = new Size(116, 19);
            numPadCheckBox.TabIndex = 8;
            numPadCheckBox.Text = "Process NumPad";
            numPadCheckBox.UseVisualStyleBackColor = true;
            // 
            // spamERadioButton
            // 
            spamERadioButton.AutoSize = true;
            spamERadioButton.Location = new Point(12, 194);
            spamERadioButton.Name = "spamERadioButton";
            spamERadioButton.Size = new Size(64, 19);
            spamERadioButton.TabIndex = 9;
            spamERadioButton.TabStop = true;
            spamERadioButton.Text = "Spam E";
            spamERadioButton.UseVisualStyleBackColor = true;
            // 
            // allowedBestPriceOrderPositionNumericUpDown
            // 
            allowedBestPriceOrderPositionNumericUpDown.Location = new Point(220, 87);
            allowedBestPriceOrderPositionNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            allowedBestPriceOrderPositionNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            allowedBestPriceOrderPositionNumericUpDown.Name = "allowedBestPriceOrderPositionNumericUpDown";
            allowedBestPriceOrderPositionNumericUpDown.Size = new Size(44, 23);
            allowedBestPriceOrderPositionNumericUpDown.TabIndex = 10;
            allowedBestPriceOrderPositionNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 89);
            label1.Name = "label1";
            label1.Size = new Size(184, 15);
            label1.TabIndex = 11;
            label1.Text = "Allowed best price order position:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 151);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 12;
            label2.Text = "Battle mode";
            // 
            // fastLootRadioButton
            // 
            fastLootRadioButton.AutoSize = true;
            fastLootRadioButton.Location = new Point(12, 219);
            fastLootRadioButton.Name = "fastLootRadioButton";
            fastLootRadioButton.Size = new Size(70, 19);
            fastLootRadioButton.TabIndex = 13;
            fastLootRadioButton.TabStop = true;
            fastLootRadioButton.Text = "Fast loot";
            fastLootRadioButton.UseVisualStyleBackColor = true;
            // 
            // newBuyOrderRadioButton
            // 
            newBuyOrderRadioButton.AutoSize = true;
            newBuyOrderRadioButton.Location = new Point(12, 107);
            newBuyOrderRadioButton.Name = "newBuyOrderRadioButton";
            newBuyOrderRadioButton.Size = new Size(103, 19);
            newBuyOrderRadioButton.TabIndex = 14;
            newBuyOrderRadioButton.TabStop = true;
            newBuyOrderRadioButton.Text = "New buy order";
            newBuyOrderRadioButton.UseVisualStyleBackColor = true;
            // 
            // allLootStrategyRadioButton
            // 
            allLootStrategyRadioButton.AutoSize = true;
            allLootStrategyRadioButton.Location = new Point(3, 3);
            allLootStrategyRadioButton.Name = "allLootStrategyRadioButton";
            allLootStrategyRadioButton.Size = new Size(39, 19);
            allLootStrategyRadioButton.TabIndex = 15;
            allLootStrategyRadioButton.Text = "All";
            allLootStrategyRadioButton.UseVisualStyleBackColor = true;
            // 
            // bestLootStrategyRadioButton
            // 
            bestLootStrategyRadioButton.AutoSize = true;
            bestLootStrategyRadioButton.Checked = true;
            bestLootStrategyRadioButton.Location = new Point(3, 28);
            bestLootStrategyRadioButton.Name = "bestLootStrategyRadioButton";
            bestLootStrategyRadioButton.Size = new Size(47, 19);
            bestLootStrategyRadioButton.TabIndex = 16;
            bestLootStrategyRadioButton.TabStop = true;
            bestLootStrategyRadioButton.Text = "Best";
            bestLootStrategyRadioButton.UseVisualStyleBackColor = true;
            // 
            // regionManagerButton
            // 
            regionManagerButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            regionManagerButton.Location = new Point(255, 390);
            regionManagerButton.Name = "regionManagerButton";
            regionManagerButton.Size = new Size(123, 23);
            regionManagerButton.TabIndex = 17;
            regionManagerButton.Text = "Region Manager";
            regionManagerButton.UseVisualStyleBackColor = true;
            regionManagerButton.Click += regionManagerButton_Click;
            // 
            // gateHelperRadioButton
            // 
            gateHelperRadioButton.AutoSize = true;
            gateHelperRadioButton.Location = new Point(12, 326);
            gateHelperRadioButton.Name = "gateHelperRadioButton";
            gateHelperRadioButton.Size = new Size(85, 19);
            gateHelperRadioButton.TabIndex = 18;
            gateHelperRadioButton.TabStop = true;
            gateHelperRadioButton.Text = "Gate helper";
            gateHelperRadioButton.UseVisualStyleBackColor = true;
            gateHelperRadioButton.CheckedChanged += gateHelperRadioButton_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(openLootWindowCheckBox);
            panel1.Controls.Add(allLootStrategyRadioButton);
            panel1.Controls.Add(bestLootStrategyRadioButton);
            panel1.Location = new Point(30, 244);
            panel1.Name = "panel1";
            panel1.Size = new Size(234, 76);
            panel1.TabIndex = 19;
            // 
            // openLootWindowCheckBox
            // 
            openLootWindowCheckBox.AutoSize = true;
            openLootWindowCheckBox.Location = new Point(3, 53);
            openLootWindowCheckBox.Name = "openLootWindowCheckBox";
            openLootWindowCheckBox.Size = new Size(129, 19);
            openLootWindowCheckBox.TabIndex = 20;
            openLootWindowCheckBox.Text = "Open Loot Window";
            openLootWindowCheckBox.UseVisualStyleBackColor = true;
            openLootWindowCheckBox.CheckedChanged += openLootWindowCheckBox_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 427);
            Controls.Add(panel1);
            Controls.Add(gateHelperRadioButton);
            Controls.Add(regionManagerButton);
            Controls.Add(newBuyOrderRadioButton);
            Controls.Add(fastLootRadioButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(allowedBestPriceOrderPositionNumericUpDown);
            Controls.Add(spamERadioButton);
            Controls.Add(numPadCheckBox);
            Controls.Add(calibrateButton);
            Controls.Add(noneRadioButton);
            Controls.Add(spamQRadioButton);
            Controls.Add(buyAndSellRadioButton);
            Controls.Add(testButton);
            Controls.Add(bootsellButton);
            Name = "MainForm";
            Text = "tser";
            FormClosing += Form1_FormClosing;
            Load += MainForm_Load;
            Shown += Form1_Shown;
            ((System.ComponentModel.ISupportInitialize)allowedBestPriceOrderPositionNumericUpDown).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button bootsellButton;
        private Button testButton;
        private RadioButton buyAndSellRadioButton;
        private RadioButton spamQRadioButton;
        private RadioButton noneRadioButton;
        private Button calibrateButton;
        private CheckBox numPadCheckBox;
        private RadioButton spamERadioButton;
        private NumericUpDown allowedBestPriceOrderPositionNumericUpDown;
        private Label label1;
        private Label label2;
        private RadioButton fastLootRadioButton;
        private RadioButton newBuyOrderRadioButton;
        private RadioButton allLootStrategyRadioButton;
        private RadioButton bestLootStrategyRadioButton;
        private Button regionManagerButton;
        private RadioButton gateHelperRadioButton;
        private Panel panel1;
        private CheckBox openLootWindowCheckBox;
    }
}
