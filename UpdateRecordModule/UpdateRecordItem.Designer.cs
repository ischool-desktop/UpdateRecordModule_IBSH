namespace UpdateRecordModule_IBSH
{
    partial class UpdateRecordItem
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRemove = new DevComponents.DotNetBar.ButtonX();
            this.bthUpdate = new DevComponents.DotNetBar.ButtonX();
            this.btnAdd = new DevComponents.DotNetBar.ButtonX();
            this.lstRecord = new SmartSchool.Common.ListViewEX();
            this.UpdateDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UpdateDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ADDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ADNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnView = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // btnRemove
            // 
            this.btnRemove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRemove.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRemove.Location = new System.Drawing.Point(178, 204);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 25);
            this.btnRemove.TabIndex = 11;
            this.btnRemove.Text = "刪除";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // bthUpdate
            // 
            this.bthUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.bthUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.bthUpdate.Location = new System.Drawing.Point(97, 204);
            this.bthUpdate.Name = "bthUpdate";
            this.bthUpdate.Size = new System.Drawing.Size(75, 25);
            this.bthUpdate.TabIndex = 10;
            this.bthUpdate.Text = "修改";
            this.bthUpdate.Click += new System.EventHandler(this.bthUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAdd.Location = new System.Drawing.Point(16, 204);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 25);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "新增";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstRecord
            // 
            // 
            // 
            // 
            this.lstRecord.Border.Class = "ListViewBorder";
            this.lstRecord.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lstRecord.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.UpdateDate,
            this.UpdateDescription,
            this.ADDate,
            this.ADNumber});
            this.lstRecord.FullRowSelect = true;
            this.lstRecord.HideSelection = false;
            this.lstRecord.Location = new System.Drawing.Point(15, 12);
            this.lstRecord.MultiSelect = false;
            this.lstRecord.Name = "lstRecord";
            this.lstRecord.Size = new System.Drawing.Size(520, 186);
            this.lstRecord.TabIndex = 13;
            this.lstRecord.UseCompatibleStateImageBehavior = false;
            this.lstRecord.View = System.Windows.Forms.View.Details;
            this.lstRecord.DoubleClick += new System.EventHandler(this.lstRecord_DoubleClick);
            // 
            // UpdateDate
            // 
            this.UpdateDate.Text = "異動日期";
            this.UpdateDate.Width = 100;
            // 
            // UpdateDescription
            // 
            this.UpdateDescription.Text = "異動原因";
            this.UpdateDescription.Width = 210;
            // 
            // ADDate
            // 
            this.ADDate.Text = "異動核准日期";
            this.ADDate.Width = 100;
            // 
            // ADNumber
            // 
            this.ADNumber.Text = "異動核准文號";
            this.ADNumber.Width = 100;
            // 
            // btnView
            // 
            this.btnView.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnView.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnView.Location = new System.Drawing.Point(15, 204);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 25);
            this.btnView.TabIndex = 14;
            this.btnView.Text = "檢視";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // UpdateRecordItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.lstRecord);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.bthUpdate);
            this.Controls.Add(this.btnAdd);
            this.Name = "UpdateRecordItem";
            this.Size = new System.Drawing.Size(550, 240);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnRemove;
        private DevComponents.DotNetBar.ButtonX bthUpdate;
        private DevComponents.DotNetBar.ButtonX btnAdd;

        private SmartSchool.Common.ListViewEX lstRecord;
       
        private DevComponents.DotNetBar.ButtonX btnView;
        private System.Windows.Forms.ColumnHeader UpdateDate;
        private System.Windows.Forms.ColumnHeader UpdateDescription;
        private System.Windows.Forms.ColumnHeader ADDate;
        private System.Windows.Forms.ColumnHeader ADNumber;

    }
}
