using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using FCode = FISCA.Permission.FeatureCodeAttribute;//Framework.Security.FeatureCodeAttribute;
using SHSchool.Data;
using System.Linq;
using FISCA.Permission;
using FISCA.Presentation.Controls;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH
{
    [FCode(Program.UpdateRecordContentCode, "異動資料")]
    public partial class UpdateRecordItem : FISCA.Presentation.DetailContent
    {
        
        private Dictionary<string, string> _headers;
        private bool _isInitialized = false;
        public static string FeatureCode = "";
        private FeatureAce _permission;
        private string _StudentID = string.Empty;
        private List<SHUpdateRecordRecord> _StudUpateRecList;
        private bool isBGBusy = false;
        private BackgroundWorker BGWorker;

        public UpdateRecordItem()
        {
            InitializeComponent();
            Group = "異動資料";

            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += new DoWorkEventHandler(BGWorker_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGWorker_RunWorkerCompleted);

            SHUpdateRecord.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(SHUpdateRecord_AfterChange);

            if (!string.IsNullOrEmpty(_StudentID))
                BGWorker.RunWorkerAsync();

            #region 權限判斷程式碼。
            //取得此 Class 定議的 FeatureCode。
            FeatureCode = FeatureCodeAttribute.GetCode(this.GetType());
            _permission = FISCA.Permission.UserAcl.Current[FeatureCode];

            btnAdd.Visible = _permission.Editable;
            btnRemove.Visible = _permission.Editable;
            bthUpdate.Visible = _permission.Editable;

            btnView.Visible = !_permission.Editable;
            #endregion

            Disposed += new EventHandler(UpdateRecordItem_Disposed);

        }

        void SHUpdateRecord_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            ReloadData();
        }

        void UpdateRecordItem_Disposed(object sender, EventArgs e)
        {
            SHUpdateRecord.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(SHUpdateRecord_AfterChange);
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            ReloadData();
        }

        void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isBGBusy)
            {
                isBGBusy = false;
                BGWorker.RunWorkerAsync();
                return;
            }
            else
            {

                BindData();
                this.Loading = false;
            }

        }


        void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _StudUpateRecList = SHUpdateRecord.SelectByStudentID(PrimaryKey);
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            _headers = new Dictionary<string, string>();
            _headers.Add("UpdateDate", "異動日期");
            _headers.Add("UpdateDescription", "異動原因及事項");
            _headers.Add("ADDate", "異動核准日期");
            _headers.Add("ADNumber", "異動核准文號");
            _isInitialized = true;

        }


        private void BindData()
        {
            Initialize();

            foreach (string key in _headers.Keys)
            {
                // Set Columns Width
                ColumnHeader ch = new ColumnHeader();
                ch.Text = _headers[key];
                ch.Tag = key;
                if (key == "UpdateDate")
                    ch.Width = 100;
                if (key == "ADDate")
                    ch.Width = 100;
                if (key == "UpdateDescription")
                    ch.Width = 180;
                if (key == "ADNumber")
                    ch.Width = 120;


                lstRecord.Columns.Add(ch);
            }
            // 依照異動日期排序(新->舊)
            var x=from record in _StudUpateRecList orderby DateTime.Parse(record.UpdateDate) descending select record;
            _StudUpateRecList = x.ToList();           


            foreach (SHUpdateRecordRecord node in _StudUpateRecList)
            {
                ListViewItem item = new ListViewItem();
                item.Tag = node;
                item.Text = node.UpdateDate;
                // Add Data
                item.SubItems.Add((node.UpdateDescription));
                item.SubItems.Add(node.ADDate);
                item.SubItems.Add(node.ADNumber);


                lstRecord.Items.Add(item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 初始化資料
            SHUpdateRecordRecord updateRec = new SHUpdateRecordRecord();
            // 取得學生ID
            // 先清DAL Cache
            List<string> rmIDs=new List<string> ();
            rmIDs.Add(PrimaryKey);
            SHStudent.RemoveByIDs(rmIDs);
            SHStudentRecord studRec = SHStudent.SelectByID(PrimaryKey);

            // 取得最後一筆異動資料，備查使用。
            List<SHUpdateRecordRecord> UpdList = (from rec in SHUpdateRecord.SelectByStudentID(PrimaryKey) where rec.ID !=updateRec.ID && rec .ADDate.Trim()!="" orderby DateTime.Parse(rec.ADDate) descending,int.Parse(rec.ID) descending select rec).ToList ();
            if (UpdList.Count > 0)
            {
                updateRec.LastADDate = UpdList[0].ADDate;
                updateRec.LastADNumber = UpdList[0].ADNumber;
                updateRec.LastUpdateCode =UpdList[0].UpdateCode;
            }

            updateRec.StudentID = studRec.ID;
            updateRec.StudentNumber = studRec.StudentNumber;
            updateRec.StudentName = studRec.Name;
            updateRec.IDNumber = studRec.IDNumber;
            if (studRec.Birthday.HasValue)
                updateRec.Birthdate = studRec.Birthday.Value.ToShortDateString();
            updateRec.Gender = studRec.Gender;
            if (studRec.Department != null)
                updateRec.Department = studRec.Department.FullName;

            // 取得學生學籍特殊身分代碼
            updateRec.SpecialStatus = DAL.DALTransfer.GetSpecialCode(studRec.ID);

            updateRec.UpdateDate = DateTime.Now.ToShortDateString();            
            if (updateRec == null)
                return;
            UpdateRecordItemForm form = new UpdateRecordItemForm(UpdateRecordItemForm.actMode.新增, updateRec, PrimaryKey);
            form.ShowDialog();            
        }

        // 重載資料
        private void ReloadData()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ReloadData));
            }
            else
            {
                if (BGWorker.IsBusy)
                    isBGBusy = true;
                else
                {
                    lstRecord.Clear();
                    this.Loading = true;
                    this.CancelButtonVisible = false;
                    this.SaveButtonVisible = false;

                    BGWorker.RunWorkerAsync();
                }
            }
        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                MsgBox.Show("您必須先選擇一筆資料");
            if (lstRecord.SelectedItems.Count == 1)
                EditStudentUpdateRecord();
        }

        // 修改異動紀錄
        private void EditStudentUpdateRecord()
        {
            SHUpdateRecordRecord objUpdate = lstRecord.SelectedItems[0].Tag as SHUpdateRecordRecord;
            UpdateRecordItemForm form = new UpdateRecordItemForm(UpdateRecordItemForm.actMode.修改, objUpdate, PrimaryKey);
            if (form._chkLoadForm)
                form.ShowDialog();
            
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            bthUpdate_Click(sender, e);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                MsgBox.Show("您必須先選擇一筆資料");
            if (lstRecord.SelectedItems.Count == 1)
            {
                SHUpdateRecordRecord record = lstRecord.SelectedItems[0].Tag as SHUpdateRecordRecord;
                if (MsgBox.Show("您確定將此筆異動資料永久刪除?", "確認", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        // 刪除異動記錄
                        SHUpdateRecord.Delete(record);                        
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("異動資料刪除失敗：" + ex.Message);
                    }
                }
            }
        }


        public UpdateRecordItem Clone()
        {
            return new UpdateRecordItem();
        }



        private void lstRecord_DoubleClick(object sender, EventArgs e)
        {
            if (lstRecord.SelectedItems.Count < 1)
                return;

            if (lstRecord.SelectedItems.Count == 1)
                EditStudentUpdateRecord();

        }
    }
}
