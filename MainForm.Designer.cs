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
            numPadCheckBox = new CheckBox();
            spamERadioButton = new RadioButton();
            allowedBestPriceOrderPositionNumericUpDown = new NumericUpDown();
            label1 = new Label();
            fastLootRadioButton = new RadioButton();
            allLootStrategyRadioButton = new RadioButton();
            bestLootStrategyRadioButton = new RadioButton();
            regionManagerButton = new Button();
            gateHelperRadioButton = new RadioButton();
            panel1 = new Panel();
            openLootWindowCheckBox = new CheckBox();
            fastBuyCheckBox = new CheckBox();
            groupBox1 = new GroupBox();
            lowHpHelperRadioButton = new RadioButton();
            setGroupPanelPositionButton = new Button();
            groupBox2 = new GroupBox();
            nameTextBox = new TextBox();
            comPortsComboBox = new ComboBox();
            connectButton = new Button();
            markerHelperRadioButton = new RadioButton();
            groupBox3 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)allowedBestPriceOrderPositionNumericUpDown).BeginInit();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // bootsellButton
            // 
            bootsellButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            bootsellButton.Location = new Point(93, 420);
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
            testButton.Location = new Point(12, 420);
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
            buyAndSellRadioButton.Location = new Point(6, 47);
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
            spamQRadioButton.Location = new Point(6, 137);
            spamQRadioButton.Name = "spamQRadioButton";
            spamQRadioButton.Size = new Size(67, 19);
            spamQRadioButton.TabIndex = 4;
            spamQRadioButton.Text = "Spam Q";
            spamQRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            noneRadioButton.AutoSize = true;
            noneRadioButton.Location = new Point(6, 22);
            noneRadioButton.Name = "noneRadioButton";
            noneRadioButton.Size = new Size(54, 19);
            noneRadioButton.TabIndex = 5;
            noneRadioButton.Text = "None";
            noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // numPadCheckBox
            // 
            numPadCheckBox.AutoSize = true;
            numPadCheckBox.Location = new Point(24, 97);
            numPadCheckBox.Name = "numPadCheckBox";
            numPadCheckBox.Size = new Size(116, 19);
            numPadCheckBox.TabIndex = 8;
            numPadCheckBox.Text = "Process NumPad";
            numPadCheckBox.UseVisualStyleBackColor = true;
            // 
            // spamERadioButton
            // 
            spamERadioButton.AutoSize = true;
            spamERadioButton.Location = new Point(6, 162);
            spamERadioButton.Name = "spamERadioButton";
            spamERadioButton.Size = new Size(64, 19);
            spamERadioButton.TabIndex = 9;
            spamERadioButton.Text = "Spam E";
            spamERadioButton.UseVisualStyleBackColor = true;
            // 
            // allowedBestPriceOrderPositionNumericUpDown
            // 
            allowedBestPriceOrderPositionNumericUpDown.Location = new Point(217, 117);
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
            label1.Location = new Point(27, 119);
            label1.Name = "label1";
            label1.Size = new Size(184, 15);
            label1.TabIndex = 11;
            label1.Text = "Allowed best price order position:";
            // 
            // fastLootRadioButton
            // 
            fastLootRadioButton.AutoSize = true;
            fastLootRadioButton.Checked = true;
            fastLootRadioButton.Location = new Point(6, 22);
            fastLootRadioButton.Name = "fastLootRadioButton";
            fastLootRadioButton.Size = new Size(70, 19);
            fastLootRadioButton.TabIndex = 13;
            fastLootRadioButton.TabStop = true;
            fastLootRadioButton.Text = "Fast loot";
            fastLootRadioButton.UseVisualStyleBackColor = true;
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
            regionManagerButton.Location = new Point(174, 420);
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
            gateHelperRadioButton.Checked = true;
            gateHelperRadioButton.Location = new Point(6, 22);
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
            panel1.Location = new Point(24, 47);
            panel1.Name = "panel1";
            panel1.Size = new Size(220, 76);
            panel1.TabIndex = 19;
            // 
            // openLootWindowCheckBox
            // 
            openLootWindowCheckBox.AutoSize = true;
            openLootWindowCheckBox.Location = new Point(20, 53);
            openLootWindowCheckBox.Name = "openLootWindowCheckBox";
            openLootWindowCheckBox.Size = new Size(129, 19);
            openLootWindowCheckBox.TabIndex = 20;
            openLootWindowCheckBox.Text = "Open Loot Window";
            openLootWindowCheckBox.UseVisualStyleBackColor = true;
            openLootWindowCheckBox.CheckedChanged += openLootWindowCheckBox_CheckedChanged;
            // 
            // fastBuyCheckBox
            // 
            fastBuyCheckBox.AutoSize = true;
            fastBuyCheckBox.Location = new Point(24, 72);
            fastBuyCheckBox.Name = "fastBuyCheckBox";
            fastBuyCheckBox.Size = new Size(170, 19);
            fastBuyCheckBox.TabIndex = 20;
            fastBuyCheckBox.Text = "Купить текущего качества";
            fastBuyCheckBox.UseVisualStyleBackColor = true;
            fastBuyCheckBox.CheckedChanged += fastBuyCheckBox_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(noneRadioButton);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(allowedBestPriceOrderPositionNumericUpDown);
            groupBox1.Controls.Add(fastBuyCheckBox);
            groupBox1.Controls.Add(lowHpHelperRadioButton);
            groupBox1.Controls.Add(numPadCheckBox);
            groupBox1.Controls.Add(buyAndSellRadioButton);
            groupBox1.Controls.Add(setGroupPanelPositionButton);
            groupBox1.Controls.Add(spamERadioButton);
            groupBox1.Controls.Add(spamQRadioButton);
            groupBox1.Location = new Point(12, 73);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(272, 312);
            groupBox1.TabIndex = 21;
            groupBox1.TabStop = false;
            groupBox1.Text = "Back mouse key";
            // 
            // lowHpHelperRadioButton
            // 
            lowHpHelperRadioButton.AutoSize = true;
            lowHpHelperRadioButton.Location = new Point(6, 187);
            lowHpHelperRadioButton.Name = "lowHpHelperRadioButton";
            lowHpHelperRadioButton.Size = new Size(100, 19);
            lowHpHelperRadioButton.TabIndex = 27;
            lowHpHelperRadioButton.TabStop = true;
            lowHpHelperRadioButton.Text = "Low hp helper";
            lowHpHelperRadioButton.UseVisualStyleBackColor = true;
            lowHpHelperRadioButton.CheckedChanged += lowHpHelperRadioButton_CheckedChanged;
            // 
            // setGroupPanelPositionButton
            // 
            setGroupPanelPositionButton.Location = new Point(24, 213);
            setGroupPanelPositionButton.Name = "setGroupPanelPositionButton";
            setGroupPanelPositionButton.Size = new Size(149, 23);
            setGroupPanelPositionButton.TabIndex = 1;
            setGroupPanelPositionButton.Text = "Set Group Panel Position";
            setGroupPanelPositionButton.UseVisualStyleBackColor = true;
            setGroupPanelPositionButton.Click += setGroupPanelPositionButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(fastLootRadioButton);
            groupBox2.Controls.Add(panel1);
            groupBox2.Location = new Point(294, 73);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(250, 134);
            groupBox2.TabIndex = 22;
            groupBox2.TabStop = false;
            groupBox2.Text = "Forward mouse key";
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(398, 289);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(100, 23);
            nameTextBox.TabIndex = 1;
            nameTextBox.TextChanged += nameTextBox_TextChanged;
            // 
            // comPortsComboBox
            // 
            comPortsComboBox.FormattingEnabled = true;
            comPortsComboBox.Location = new Point(12, 12);
            comPortsComboBox.Name = "comPortsComboBox";
            comPortsComboBox.Size = new Size(121, 23);
            comPortsComboBox.TabIndex = 23;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(139, 12);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(75, 23);
            connectButton.TabIndex = 24;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // markerHelperRadioButton
            // 
            markerHelperRadioButton.AutoSize = true;
            markerHelperRadioButton.Location = new Point(294, 290);
            markerHelperRadioButton.Name = "markerHelperRadioButton";
            markerHelperRadioButton.Size = new Size(98, 19);
            markerHelperRadioButton.TabIndex = 25;
            markerHelperRadioButton.TabStop = true;
            markerHelperRadioButton.Text = "Marker helper";
            markerHelperRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(gateHelperRadioButton);
            groupBox3.Location = new Point(294, 213);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(250, 66);
            groupBox3.TabIndex = 26;
            groupBox3.TabStop = false;
            groupBox3.Text = "Ctrl + Forward mouse key";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 455);
            Controls.Add(groupBox3);
            Controls.Add(markerHelperRadioButton);
            Controls.Add(nameTextBox);
            Controls.Add(connectButton);
            Controls.Add(comPortsComboBox);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(regionManagerButton);
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
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button bootsellButton;
        private Button testButton;
        private RadioButton buyAndSellRadioButton;
        private RadioButton spamQRadioButton;
        private RadioButton noneRadioButton;
        private CheckBox numPadCheckBox;
        private RadioButton spamERadioButton;
        private NumericUpDown allowedBestPriceOrderPositionNumericUpDown;
        private Label label1;
        private RadioButton fastLootRadioButton;
        private RadioButton allLootStrategyRadioButton;
        private RadioButton bestLootStrategyRadioButton;
        private Button regionManagerButton;
        private RadioButton gateHelperRadioButton;
        private Panel panel1;
        private CheckBox openLootWindowCheckBox;
        private CheckBox fastBuyCheckBox;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private ComboBox comPortsComboBox;
        private Button connectButton;
        private Button setGroupPanelPositionButton;
        private TextBox nameTextBox;
        private RadioButton lowHpHelperRadioButton;
        private RadioButton markerHelperRadioButton;
        private GroupBox groupBox3;
    }
}
