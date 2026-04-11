namespace tser
{
    partial class Form1
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
            SuspendLayout();
            // 
            // bootsellButton
            // 
            bootsellButton.Location = new Point(16, 123);
            bootsellButton.Name = "bootsellButton";
            bootsellButton.Size = new Size(75, 23);
            bootsellButton.TabIndex = 1;
            bootsellButton.Text = "bootsell";
            bootsellButton.UseVisualStyleBackColor = true;
            bootsellButton.Click += bootsellButton_Click;
            // 
            // testButton
            // 
            testButton.Location = new Point(16, 152);
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
            buyAndSellRadioButton.Location = new Point(12, 37);
            buyAndSellRadioButton.Name = "buyAndSellRadioButton";
            buyAndSellRadioButton.Size = new Size(79, 19);
            buyAndSellRadioButton.TabIndex = 3;
            buyAndSellRadioButton.Text = "Buy && Sell";
            buyAndSellRadioButton.UseVisualStyleBackColor = true;
            // 
            // spamQRadioButton
            // 
            spamQRadioButton.AutoSize = true;
            spamQRadioButton.Location = new Point(12, 62);
            spamQRadioButton.Name = "spamQRadioButton";
            spamQRadioButton.Size = new Size(67, 19);
            spamQRadioButton.TabIndex = 4;
            spamQRadioButton.Text = "Spam Q";
            spamQRadioButton.UseVisualStyleBackColor = true;
            // 
            // noneRadioButton
            // 
            noneRadioButton.AutoSize = true;
            noneRadioButton.Checked = true;
            noneRadioButton.Location = new Point(12, 12);
            noneRadioButton.Name = "noneRadioButton";
            noneRadioButton.Size = new Size(54, 19);
            noneRadioButton.TabIndex = 5;
            noneRadioButton.TabStop = true;
            noneRadioButton.Text = "None";
            noneRadioButton.UseVisualStyleBackColor = true;
            // 
            // calibrateButton
            // 
            calibrateButton.Location = new Point(16, 181);
            calibrateButton.Name = "calibrateButton";
            calibrateButton.Size = new Size(75, 23);
            calibrateButton.TabIndex = 6;
            calibrateButton.Text = "Calibrate";
            calibrateButton.UseVisualStyleBackColor = true;
            calibrateButton.Click += calibrateButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(120, 221);
            Controls.Add(calibrateButton);
            Controls.Add(noneRadioButton);
            Controls.Add(spamQRadioButton);
            Controls.Add(buyAndSellRadioButton);
            Controls.Add(testButton);
            Controls.Add(bootsellButton);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            Text = "Form1";
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
    }
}
