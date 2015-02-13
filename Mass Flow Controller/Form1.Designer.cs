namespace Mass_Flow_Controller
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.purgeButton = new System.Windows.Forms.Button();
            this.offButton = new System.Windows.Forms.Button();
            this.rampButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comBox = new System.Windows.Forms.ComboBox();
            this.comButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.flowTitleLabel = new System.Windows.Forms.Label();
            this.flowLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.setPointBox = new System.Windows.Forms.MaskedTextBox();
            this.timeBox = new System.Windows.Forms.TextBox();
            this.tempLabel = new System.Windows.Forms.Label();
            this.tempTitleLabel = new System.Windows.Forms.Label();
            this.pressureLabel = new System.Windows.Forms.Label();
            this.pressureTitleLabel = new System.Windows.Forms.Label();
            this.pressureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // purgeButton
            // 
            this.purgeButton.Location = new System.Drawing.Point(233, 11);
            this.purgeButton.Name = "purgeButton";
            this.purgeButton.Size = new System.Drawing.Size(75, 23);
            this.purgeButton.TabIndex = 0;
            this.purgeButton.Text = "Purge";
            this.purgeButton.UseVisualStyleBackColor = true;
            this.purgeButton.Click += new System.EventHandler(this.purgeButton_Click);
            // 
            // offButton
            // 
            this.offButton.Location = new System.Drawing.Point(233, 46);
            this.offButton.Name = "offButton";
            this.offButton.Size = new System.Drawing.Size(75, 23);
            this.offButton.TabIndex = 0;
            this.offButton.Text = "Turn Off";
            this.offButton.UseVisualStyleBackColor = true;
            this.offButton.Click += new System.EventHandler(this.offButton_Click);
            // 
            // rampButton
            // 
            this.rampButton.Location = new System.Drawing.Point(89, 92);
            this.rampButton.Name = "rampButton";
            this.rampButton.Size = new System.Drawing.Size(75, 23);
            this.rampButton.TabIndex = 0;
            this.rampButton.Text = "Set Flow";
            this.rampButton.UseVisualStyleBackColor = true;
            this.rampButton.Click += new System.EventHandler(this.rampButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(10, 186);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(37, 13);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Status";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Ramp Time:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Set Point:";
            // 
            // comBox
            // 
            this.comBox.FormattingEnabled = true;
            this.comBox.Location = new System.Drawing.Point(13, 13);
            this.comBox.Name = "comBox";
            this.comBox.Size = new System.Drawing.Size(132, 21);
            this.comBox.TabIndex = 8;
            // 
            // comButton
            // 
            this.comButton.Location = new System.Drawing.Point(151, 11);
            this.comButton.Name = "comButton";
            this.comButton.Size = new System.Drawing.Size(75, 23);
            this.comButton.TabIndex = 9;
            this.comButton.Text = "Connect";
            this.comButton.UseVisualStyleBackColor = true;
            this.comButton.Click += new System.EventHandler(this.comButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(179, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "seconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(179, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "mL/min";
            // 
            // flowTitleLabel
            // 
            this.flowTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowTitleLabel.Location = new System.Drawing.Point(11, 123);
            this.flowTitleLabel.Name = "flowTitleLabel";
            this.flowTitleLabel.Size = new System.Drawing.Size(90, 21);
            this.flowTitleLabel.TabIndex = 12;
            this.flowTitleLabel.Text = "Flow (mL/min):";
            this.flowTitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // flowLabel
            // 
            this.flowLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLabel.Location = new System.Drawing.Point(11, 144);
            this.flowLabel.Name = "flowLabel";
            this.flowLabel.Size = new System.Drawing.Size(90, 37);
            this.flowLabel.TabIndex = 13;
            this.flowLabel.Text = "---";
            this.flowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLabel
            // 
            this.timeLabel.Location = new System.Drawing.Point(265, 181);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(42, 18);
            this.timeLabel.TabIndex = 4;
            this.timeLabel.Text = "00:00";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // setPointBox
            // 
            this.setPointBox.Location = new System.Drawing.Point(76, 40);
            this.setPointBox.Name = "setPointBox";
            this.setPointBox.Size = new System.Drawing.Size(100, 20);
            this.setPointBox.TabIndex = 15;
            this.setPointBox.Leave += new System.EventHandler(this.setPointBox_Leave);
            // 
            // timeBox
            // 
            this.timeBox.Location = new System.Drawing.Point(76, 66);
            this.timeBox.Name = "timeBox";
            this.timeBox.Size = new System.Drawing.Size(100, 20);
            this.timeBox.TabIndex = 16;
            // 
            // tempLabel
            // 
            this.tempLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempLabel.Location = new System.Drawing.Point(117, 145);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(86, 37);
            this.tempLabel.TabIndex = 18;
            this.tempLabel.Text = "---";
            this.tempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tempTitleLabel
            // 
            this.tempTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tempTitleLabel.Location = new System.Drawing.Point(113, 123);
            this.tempTitleLabel.Name = "tempTitleLabel";
            this.tempTitleLabel.Size = new System.Drawing.Size(90, 22);
            this.tempTitleLabel.TabIndex = 17;
            this.tempTitleLabel.Text = "Temp (*C):";
            this.tempTitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pressureLabel
            // 
            this.pressureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pressureLabel.Location = new System.Drawing.Point(213, 147);
            this.pressureLabel.Name = "pressureLabel";
            this.pressureLabel.Size = new System.Drawing.Size(90, 37);
            this.pressureLabel.TabIndex = 20;
            this.pressureLabel.Text = "---";
            this.pressureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pressureTitleLabel
            // 
            this.pressureTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pressureTitleLabel.Location = new System.Drawing.Point(213, 123);
            this.pressureTitleLabel.Name = "pressureTitleLabel";
            this.pressureTitleLabel.Size = new System.Drawing.Size(90, 18);
            this.pressureTitleLabel.TabIndex = 19;
            this.pressureTitleLabel.Text = "Pressure (psi):";
            this.pressureTitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pressureButton
            // 
            this.pressureButton.Location = new System.Drawing.Point(233, 80);
            this.pressureButton.Name = "pressureButton";
            this.pressureButton.Size = new System.Drawing.Size(75, 35);
            this.pressureButton.TabIndex = 21;
            this.pressureButton.Text = "Calibrate Pressure";
            this.pressureButton.UseVisualStyleBackColor = true;
            this.pressureButton.Click += new System.EventHandler(this.pressure_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 205);
            this.Controls.Add(this.pressureButton);
            this.Controls.Add(this.pressureLabel);
            this.Controls.Add(this.pressureTitleLabel);
            this.Controls.Add(this.tempLabel);
            this.Controls.Add(this.tempTitleLabel);
            this.Controls.Add(this.timeBox);
            this.Controls.Add(this.setPointBox);
            this.Controls.Add(this.flowLabel);
            this.Controls.Add(this.flowTitleLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comButton);
            this.Controls.Add(this.comBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rampButton);
            this.Controls.Add(this.offButton);
            this.Controls.Add(this.purgeButton);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.statusLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Mass Flow Controller";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button purgeButton;
        private System.Windows.Forms.Button offButton;
        private System.Windows.Forms.Button rampButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comBox;
        private System.Windows.Forms.Button comButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label flowTitleLabel;
        private System.Windows.Forms.Label flowLabel;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.MaskedTextBox setPointBox;
        private System.Windows.Forms.TextBox timeBox;
        private System.Windows.Forms.Label tempLabel;
        private System.Windows.Forms.Label tempTitleLabel;
        private System.Windows.Forms.Label pressureLabel;
        private System.Windows.Forms.Label pressureTitleLabel;
        private System.Windows.Forms.Button pressureButton;

    }
}

