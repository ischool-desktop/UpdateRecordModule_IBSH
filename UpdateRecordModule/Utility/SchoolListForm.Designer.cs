namespace UpdateRecordModule_IBSH.Utility
{
    partial class SchoolListForm
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
            this.lvCounty = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSchoolName = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOk = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // lvCounty
            // 
            // 
            // 
            // 
            this.lvCounty.Border.Class = "ListViewBorder";
            this.lvCounty.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvCounty.Location = new System.Drawing.Point(12, 12);
            this.lvCounty.Name = "lvCounty";
            this.lvCounty.Size = new System.Drawing.Size(126, 161);
            this.lvCounty.TabIndex = 0;
            this.lvCounty.UseCompatibleStateImageBehavior = false;
            this.lvCounty.View = System.Windows.Forms.View.Details;
            this.lvCounty.SelectedIndexChanged += new System.EventHandler(this.lvCounty_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "縣市名稱";
            this.columnHeader1.Width = 120;
            // 
            // lvSchoolName
            // 
            // 
            // 
            // 
            this.lvSchoolName.Border.Class = "ListViewBorder";
            this.lvSchoolName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvSchoolName.Location = new System.Drawing.Point(137, 12);
            this.lvSchoolName.Name = "lvSchoolName";
            this.lvSchoolName.Size = new System.Drawing.Size(207, 161);
            this.lvSchoolName.TabIndex = 1;
            this.lvSchoolName.UseCompatibleStateImageBehavior = false;
            this.lvSchoolName.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "學校名稱";
            this.columnHeader2.Width = 200;
            // 
            // btnOk
            // 
            this.btnOk.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOk.Location = new System.Drawing.Point(177, 189);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "確定";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(267, 189);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // SchoolListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 219);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lvSchoolName);
            this.Controls.Add(this.lvCounty);
            this.Name = "SchoolListForm";
            this.Text = "國中";
            this.Load += new System.EventHandler(this.SchoolListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ListViewEx lvCounty;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSchoolName;
        private DevComponents.DotNetBar.ButtonX btnOk;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}