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
            ((System.ComponentModel.ISupportInitialize)allowedBestPriceOrderPositionNumericUpDown).BeginInit();
            SuspendLayout();
            // 
            // bootsellButton
            // 
            bootsellButton.Location = new Point(174, 296);
            bootsellButton.Name = "bootsellButton";
            bootsellButton.Size = new Size(75, 23);
            bootsellButton.TabIndex = 1;
            bootsellButton.Text = "bootsell";
            bootsellButton.UseVisualStyleBackColor = true;
            bootsellButton.Click += bootsellButton_Click;
            // 
            // testButton
            // 
            testButton.Location = new Point(93, 296);
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
            calibrateButton.Location = new Point(12, 296);
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
            allLootStrategyRadioButton.Location = new Point(30, 244);
            allLootStrategyRadioButton.Name = "allLootStrategyRadioButton";
            allLootStrategyRadioButton.Size = new Size(39, 19);
            allLootStrategyRadioButton.TabIndex = 15;
            allLootStrategyRadioButton.TabStop = true;
            allLootStrategyRadioButton.Text = "All";
            allLootStrategyRadioButton.UseVisualStyleBackColor = true;
            // 
            // bestLootStrategyRadioButton
            // 
            bestLootStrategyRadioButton.AutoSize = true;
            bestLootStrategyRadioButton.Location = new Point(30, 269);
            bestLootStrategyRadioButton.Name = "bestLootStrategyRadioButton";
            bestLootStrategyRadioButton.Size = new Size(47, 19);
            bestLootStrategyRadioButton.TabIndex = 16;
            bestLootStrategyRadioButton.TabStop = true;
            bestLootStrategyRadioButton.Text = "Best";
            bestLootStrategyRadioButton.UseVisualStyleBackColor = true;
            // 
            // regionManagerButton
            // 
            regionManagerButton.Location = new Point(255, 296);
            regionManagerButton.Name = "regionManagerButton";
            regionManagerButton.Size = new Size(123, 23);
            regionManagerButton.TabIndex = 17;
            regionManagerButton.Text = "Region Manager";
            regionManagerButton.UseVisualStyleBackColor = true;
            regionManagerButton.Click += regionManagerButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 333);
            Controls.Add(regionManagerButton);
            Controls.Add(bestLootStrategyRadioButton);
            Controls.Add(allLootStrategyRadioButton);
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
    }
}
