namespace UpdateRecordModule_IBSH.Batch
{
    partial class BatchNewStudUpdateRecForm
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.btnRun = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.dtUpdateDate = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.cbxUpdateCode = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cbxClassType = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.dtUpdateDate)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            this.labelX1.Location = new System.Drawing.Point(-6, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(75, 23);
            this.labelX1.TabIndex = 0;
            this.labelX1.Text = "異動日期";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            this.labelX2.Location = new System.Drawing.Point(-5, 46);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(75, 23);
            this.labelX2.TabIndex = 1;
            this.labelX2.Text = "資格代碼";
            this.labelX2.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            this.labelX3.Location = new System.Drawing.Point(-5, 82);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(75, 23);
            this.labelX3.TabIndex = 2;
            this.labelX3.Text = "班別";
            this.labelX3.TextAlignment = System.Drawing.StringAlignment.Far;
            // 
            // btnRun
            // 
            this.btnRun.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRun.BackColor = System.Drawing.Color.Transparent;
            this.btnRun.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRun.Location = new System.Drawing.Point(77, 116);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(61, 23);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "產生";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Location = new System.Drawing.Point(144, 116);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dtUpdateDate
            // 
            this.dtUpdateDate.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.dtUpdateDate.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtUpdateDate.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtUpdateDate.ButtonDropDown.Visible = true;
            this.dtUpdateDate.Location = new System.Drawing.Point(86, 12);
            // 
            // 
            // 
            this.dtUpdateDate.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtUpdateDate.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtUpdateDate.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtUpdateDate.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtUpdateDate.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtUpdateDate.MonthCalendar.DisplayMonth = new System.DateTime(2011, 5, 1, 0, 0, 0, 0);
            this.dtUpdateDate.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtUpdateDate.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtUpdateDate.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtUpdateDate.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtUpdateDate.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtUpdateDate.MonthCalendar.TodayButtonVisible = true;
            this.dtUpdateDate.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtUpdateDate.Name = "dtUpdateDate";
            this.dtUpdateDate.Size = new System.Drawing.Size(121, 25);
            this.dtUpdateDate.TabIndex = 5;
            // 
            // cbxUpdateCode
            // 
            this.cbxUpdateCode.DisplayMember = "Text";
            this.cbxUpdateCode.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxUpdateCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxUpdateCode.DropDownWidth = 300;
            this.cbxUpdateCode.FormattingEnabled = true;
            this.cbxUpdateCode.ItemHeight = 19;
            this.cbxUpdateCode.Location = new System.Drawing.Point(86, 46);
            this.cbxUpdateCode.Name = "cbxUpdateCode";
            this.cbxUpdateCode.Size = new System.Drawing.Size(121, 25);
            this.cbxUpdateCode.TabIndex = 6;
            // 
            // cbxClassType
            // 
            this.cbxClassType.DisplayMember = "Text";
            this.cbxClassType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxClassType.DropDownHeight = 103;
            this.cbxClassType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxClassType.DropDownWidth = 160;
            this.cbxClassType.FormattingEnabled = true;
            this.cbxClassType.IntegralHeight = false;
            this.cbxClassType.ItemHeight = 19;
            this.cbxClassType.Location = new System.Drawing.Point(86, 82);
            this.cbxClassType.Name = "cbxClassType";
            this.cbxClassType.Size = new System.Drawing.Size(121, 25);
            this.cbxClassType.TabIndex = 7;
            // 
            // BatchNewStudUpdateRecForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 146);
            this.Controls.Add(this.cbxClassType);
            this.Controls.Add(this.cbxUpdateCode);
            this.Controls.Add(this.dtUpdateDate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Name = "BatchNewStudUpdateRecForm";
            this.Text = "產生批次新生異動";
            ((System.ComponentModel.ISupportInitialize)(this.dtUpdateDate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.ButtonX btnRun;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtUpdateDate;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxUpdateCode;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxClassType;
    }
}