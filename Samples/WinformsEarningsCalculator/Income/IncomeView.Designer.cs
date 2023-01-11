namespace WinformsEarningsCalculator.Income
{
    partial class IncomeView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.numIncome = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dtStart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.propertyBinding1 = new Clarity.Winforms.PropertyBinding(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboFrequency = new System.Windows.Forms.ComboBox();
            this.chkInvalid = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonBinding1 = new Clarity.Winforms.ButtonBinding(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numIncome)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertyBinding1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonBinding1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label1, null);
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.propertyBinding1.SetPropertyName(this.label1, null);
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Income";
            // 
            // numIncome
            // 
            this.numIncome.DecimalPlaces = 2;
            this.propertyBinding1.SetDisplayFormat(this.numIncome, null);
            this.numIncome.Location = new System.Drawing.Point(72, 4);
            this.numIncome.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numIncome.Name = "numIncome";
            this.propertyBinding1.SetPropertyName(this.numIncome, "Income.IncomeAmount");
            this.numIncome.Size = new System.Drawing.Size(120, 20);
            this.numIncome.TabIndex = 1;
            this.numIncome.ThousandsSeparator = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label2, "c");
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(67, 92);
            this.label2.Name = "label2";
            this.propertyBinding1.SetPropertyName(this.label2, "EarningsToday");
            this.label2.Size = new System.Drawing.Size(19, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "-";
            // 
            // button1
            // 
            this.buttonBinding1.SetCommandName(this.button1, "CloseCommand");
            this.button1.Location = new System.Drawing.Point(295, 145);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // dtStart
            // 
            this.propertyBinding1.SetDisplayFormat(this.dtStart, null);
            this.dtStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtStart.Location = new System.Drawing.Point(72, 57);
            this.dtStart.Name = "dtStart";
            this.propertyBinding1.SetPropertyName(this.dtStart, "StartTime");
            this.dtStart.ShowUpDown = true;
            this.dtStart.Size = new System.Drawing.Size(120, 20);
            this.dtStart.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label3, null);
            this.label3.Location = new System.Drawing.Point(4, 63);
            this.label3.Name = "label3";
            this.propertyBinding1.SetPropertyName(this.label3, null);
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Start Time";
            // 
            // btnTest
            // 
            this.buttonBinding1.SetCommandName(this.btnTest, "Test");
            this.btnTest.Location = new System.Drawing.Point(7, 145);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(114, 23);
            this.btnTest.TabIndex = 6;
            this.btnTest.Text = "Get Answer Test";
            this.btnTest.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label4, null);
            this.label4.Location = new System.Drawing.Point(142, 150);
            this.label4.Name = "label4";
            this.propertyBinding1.SetPropertyName(this.label4, "Answer");
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label5, null);
            this.label5.Location = new System.Drawing.Point(6, 101);
            this.label5.Name = "label5";
            this.propertyBinding1.SetPropertyName(this.label5, null);
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Earnings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.label6, null);
            this.label6.Location = new System.Drawing.Point(8, 34);
            this.label6.Name = "label6";
            this.propertyBinding1.SetPropertyName(this.label6, null);
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Frequency";
            // 
            // cboFrequency
            // 
            this.propertyBinding1.SetDisplayFormat(this.cboFrequency, null);
            this.cboFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrequency.FormattingEnabled = true;
            this.cboFrequency.Location = new System.Drawing.Point(72, 30);
            this.cboFrequency.Name = "cboFrequency";
            this.propertyBinding1.SetPropertyName(this.cboFrequency, null);
            this.cboFrequency.Size = new System.Drawing.Size(120, 21);
            this.cboFrequency.TabIndex = 10;
            // 
            // chkInvalid
            // 
            this.chkInvalid.AutoSize = true;
            this.propertyBinding1.SetDisplayFormat(this.chkInvalid, null);
            this.chkInvalid.Location = new System.Drawing.Point(9, 175);
            this.chkInvalid.Name = "chkInvalid";
            this.propertyBinding1.SetPropertyName(this.chkInvalid, "MakeInvalid");
            this.chkInvalid.Size = new System.Drawing.Size(206, 17);
            this.chkInvalid.TabIndex = 12;
            this.chkInvalid.Text = "Make Form Invalid if Frequency is Day";
            this.chkInvalid.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.buttonBinding1.SetCommandName(this.button2, "ResetFrequency");
            this.button2.Location = new System.Drawing.Point(265, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(105, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Reset Frequency";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // IncomeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkInvalid);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cboFrequency);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtStart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numIncome);
            this.Controls.Add(this.label1);
            this.propertyBinding1.SetDisplayFormat(this, null);
            this.Name = "IncomeView";
            this.propertyBinding1.SetPropertyName(this, null);
            this.Size = new System.Drawing.Size(386, 199);
            ((System.ComponentModel.ISupportInitialize)(this.numIncome)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertyBinding1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonBinding1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numIncome;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dtStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label4;
        private Clarity.Winforms.PropertyBinding propertyBinding1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboFrequency;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkInvalid;
        private Clarity.Winforms.ButtonBinding buttonBinding1;
    }
}
