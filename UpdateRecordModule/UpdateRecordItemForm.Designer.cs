namespace UpdateRecordModule_IBSH
{
    partial class UpdateRecordItemForm
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
            this.cbxSel = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.UpdateRecordEditorPanle = new DevComponents.DotNetBar.PanelEx();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnConfirm = new DevComponents.DotNetBar.ButtonX();
            this.intSchoolYear = new DevComponents.Editors.IntegerInput();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.intSemester = new DevComponents.Editors.IntegerInput();
            this.integerInput2 = new DevComponents.Editors.IntegerInput();
            this.lablex11 = new DevComponents.DotNetBar.LabelX();
            this.cbxGradeYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.integerInput2)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxSel
            // 
            this.cbxSel.DisplayMember = "Text";
            this.cbxSel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSel.FormattingEnabled = true;
            this.cbxSel.ItemHeight = 17;
            this.cbxSel.Location = new System.Drawing.Point(11, 12);
            this.cbxSel.Name = "cbxSel";
            this.cbxSel.Size = new System.Drawing.Size(121, 23);
            this.cbxSel.TabIndex = 0;
            this.cbxSel.SelectedIndexChanged += new System.EventHandler(this.cbxSel_SelectedIndexChanged);
            // 
            // UpdateRecordEditorPanle
            // 
            this.UpdateRecordEditorPanle.CanvasColor = System.Drawing.SystemColors.Control;
            this.UpdateRecordEditorPanle.Location = new System.Drawing.Point(9, 45);
            this.UpdateRecordEditorPanle.Name = "UpdateRecordEditorPanle";
            this.UpdateRecordEditorPanle.Size = new System.Drawing.Size(515, 562);
            this.UpdateRecordEditorPanle.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.UpdateRecordEditorPanle.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.UpdateRecordEditorPanle.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.UpdateRecordEditorPanle.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.UpdateRecordEditorPanle.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.UpdateRecordEditorPanle.Style.GradientAngle = 90;
            this.UpdateRecordEditorPanle.TabIndex = 1;
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(454, 616);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "離開";
            // 
            // btnConfirm
            // 
            this.btnConfirm.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(384, 616);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(64, 23);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "確定";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // intSchoolYear
            // 
            this.intSchoolYear.AutoOverwrite = true;
            this.intSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSchoolYear.Location = new System.Drawing.Point(235, 12);
            this.intSchoolYear.MaxValue = 9999;
            this.intSchoolYear.MinValue = 1;
            this.intSchoolYear.Name = "intSchoolYear";
            this.intSchoolYear.Size = new System.Drawing.Size(57, 23);
            this.intSchoolYear.TabIndex = 5;
            this.intSchoolYear.Value = 1;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(186, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(46, 23);
            this.labelX1.TabIndex = 6;
            this.labelX1.Text = "學年度";
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(309, 12);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(29, 23);
            this.labelX2.TabIndex = 8;
            this.labelX2.Text = "學期";
            // 
            // intSemester
            // 
            this.intSemester.AutoOverwrite = true;
            this.intSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSemester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSemester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSemester.Location = new System.Drawing.Point(342, 12);
            this.intSemester.MaxValue = 2;
            this.intSemester.MinValue = 1;
            this.intSemester.Name = "intSemester";
            this.intSemester.Size = new System.Drawing.Size(57, 23);
            this.intSemester.TabIndex = 7;
            this.intSemester.Value = 1;
            // 
            // integerInput2
            // 
            this.integerInput2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.integerInput2.BackgroundStyle.Class = "DateTimeInputBackground";
            this.integerInput2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.integerInput2.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.integerInput2.Location = new System.Drawing.Point(364, 11);
            this.integerInput2.Name = "integerInput2";
            this.integerInput2.ShowUpDown = true;
            this.integerInput2.Size = new System.Drawing.Size(67, 22);
            this.integerInput2.TabIndex = 7;
            // 
            // lablex11
            // 
            this.lablex11.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lablex11.BackgroundStyle.Class = "";
            this.lablex11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lablex11.Location = new System.Drawing.Point(408, 12);
            this.lablex11.Name = "lablex11";
            this.lablex11.Size = new System.Drawing.Size(37, 23);
            this.lablex11.TabIndex = 10;
            this.lablex11.Text = "年級";
            // 
            // cbxGradeYear
            // 
            this.cbxGradeYear.DisplayMember = "Text";
            this.cbxGradeYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxGradeYear.FormattingEnabled = true;
            this.cbxGradeYear.ItemHeight = 17;
            this.cbxGradeYear.Location = new System.Drawing.Point(445, 12);
            this.cbxGradeYear.Name = "cbxGradeYear";
            this.cbxGradeYear.Size = new System.Drawing.Size(57, 23);
            this.cbxGradeYear.TabIndex = 11;
            // 
            // UpdateRecordItemForm
            // 
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(532, 648);
            this.Controls.Add(this.cbxGradeYear);
            this.Controls.Add(this.lablex11);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.intSemester);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.intSchoolYear);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.UpdateRecordEditorPanle);
            this.Controls.Add(this.cbxSel);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "UpdateRecordItemForm";
            this.Text = "管理學生異動資料";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UpdateRecordItemForm_FormClosing);
            this.Load += new System.EventHandler(this.UpdateRecordItemForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSemester)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.integerInput2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSel;
        private DevComponents.DotNetBar.PanelEx UpdateRecordEditorPanle;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnConfirm;
        private DevComponents.Editors.IntegerInput intSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.Editors.IntegerInput intSemester;
        private DevComponents.Editors.IntegerInput integerInput2;
        private DevComponents.DotNetBar.LabelX lablex11;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxGradeYear;
    }
}