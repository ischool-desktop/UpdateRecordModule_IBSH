using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using IntelliSchool.DSA.ClientFramework.ControlCommunication;

namespace UpdateRecordModule_IBSH.GovernmentalDocument
{
    public partial class BuildWizard : FISCA.Presentation.Controls.BaseForm 
    {
        BackgroundWorker _BackgroundWorker = new BackgroundWorker();
        DAL.StudUpdateRecBatchCreator _StudUpdateRecBatchCreator = new UpdateRecordModule_IBSH.DAL.StudUpdateRecBatchCreator ();
        DAL.StudUpdateRecBatchCreator.UpdateRecBatchType _UpdateType;

        private int _checkCount = 0,_totalCount=0;

        public BuildWizard()
        {
            InitializeComponent();

            // 初始值
            // 設定學年度學期
            cbxSemester.Items.Add("1");
            cbxSemester.Items.Add("2");

            int scYear;
            // 加入學年度初始值
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out scYear))
            {
                for (int i = (scYear - 3); i <= (scYear + 3); i++)
                    cbxSchoolYear.Items.Add(i);
            }

            cbxSchoolYear.Text = K12.Data.School.DefaultSchoolYear;
            cbxSemester.Text = K12.Data.School.DefaultSemester;

            // 加入可選擇異動名冊名稱
            List<DAL.StudUpdateRecBatchCreator.UpdateRecBatchType> NameList = new List<DAL.StudUpdateRecBatchCreator.UpdateRecBatchType>();
                     
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.新生名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.學籍異動名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.轉入學生名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.畢業名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.延修生名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.延修生學籍異動名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.延修生畢業名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.新生保留錄取資格名冊);
                NameList.Add(DAL.StudUpdateRecBatchCreator.UpdateRecBatchType.借讀學生名冊);
         

            foreach (DAL.StudUpdateRecBatchCreator.UpdateRecBatchType item in NameList)
            {
                ListViewItem itm = new ListViewItem(item.ToString(), 0);
                itm.Tag = item;
                listView1.Items.Add(itm);
            }
                       

            // 排序子
            lvSorter1.ColumnSorter.Add(columnHeader3, SortType.Int);
            lvSorter1.ColumnSorter.Add(columnHeader6, SortType.Int);
            lvSorter1.ColumnSorter.Add(columnHeader5, SortType.DateTime);
            lvSorter1.ColumnSorter.Add(columnHeader1, SortType.Int);
            lvSorter1.ColumnSorter.Add(columnHeader2, SortType.String);
            lvSorter1.ColumnSorter.Add(columnHeader4, SortType.String);
            lvSorter1.ColumnSorter.Add(columnHeader7, SortType.String);
            lvViewManager1.SetDynamicGrouping(columnHeader1);
            lvViewManager1.SetDynamicGrouping(columnHeader2);
            lvViewManager1.SetDynamicGrouping(columnHeader5);
            lvViewManager1.SetDynamicGrouping(columnHeader6);            
        }

        private void BuildWizard_Load(object sender, EventArgs e)
        {
            int xm, ym;
            xm = (this.Width - panelEx1.Width - 20) / 2;
            ym = (this.Height - panelEx1.Height - 40) / 2;
            this.Top += ym;
            this.Left += xm;
            this.SuspendLayout();
            panelEx1.SuspendLayout();
            panelEx2.SuspendLayout();
            panelEx3.SuspendLayout();
            this.Height = panelEx1.Height + 40;
            this.Width = panelEx1.Width + 20;
            panelEx1.Visible = true;
            panelEx2.Visible = false;
            panelEx3.Visible = false;
            panelEx1.Dock = DockStyle.Fill;
            panelEx2.Dock = DockStyle.None;
            panelEx3.Dock = DockStyle.None;
            panelEx1.ResumeLayout();
            panelEx2.ResumeLayout();
            panelEx3.ResumeLayout();
            this.ResumeLayout();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnWF1Next_Click(object sender, EventArgs e)
        {

            this.Visible = false;
            int xm, ym;
            xm = (this.Width - panelEx2.Width - 20) / 2;
            ym = (this.Height - panelEx2.Height - 40) / 2;
            this.Top += ym;
            this.Left += xm;
            this.SuspendLayout();
            panelEx1.SuspendLayout();
            panelEx2.SuspendLayout();
            panelEx3.SuspendLayout();
            this.Height = panelEx2.Height + 40;
            this.Width = panelEx2.Width + 20;
            panelEx1.Visible = false;
            panelEx2.Visible = true;
            panelEx3.Visible = false;
            panelEx1.Dock = DockStyle.None;
            panelEx2.Dock = DockStyle.Fill;
            panelEx3.Dock = DockStyle.None;
            this.Visible = true;
            panelEx1.ResumeLayout();
            panelEx2.ResumeLayout();
            panelEx3.ResumeLayout();
            this.ResumeLayout();
            _UpdateType = (DAL.StudUpdateRecBatchCreator.UpdateRecBatchType)listView1.SelectedItems[0].Tag;

            // 名冊預設名稱
            setDefaultNameListName();

            listView2.Items.Clear();

            _BackgroundWorker.DoWork += new DoWorkEventHandler(_BackgroundWorker_DoWork);
            _BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BackgroundWorker_RunWorkerCompleted);
            _BackgroundWorker.WorkerSupportsCancellation = true;
            _BackgroundWorker.RunWorkerAsync();
                        
        }

        /// <summary>
        /// 設定新增名冊時名冊預設名稱
        /// </summary>
        /// <returns></returns>
        private void setDefaultNameListName()
        {
            txtNameListName.Text = cbxSchoolYear.Text + "_" + cbxSemester.Text + "_" + _UpdateType.ToString();
        }

        void _BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)            {

                pictureBox1.Visible = false;
                listView2.SuspendLayout();
                foreach (BL.StudUpdateRecDoc var in _StudUpdateRecBatchCreator.GetSHUpdateRecordRecordList())
                {
                    ListViewItem item = new ListViewItem(new string[] { 
                            var.UpdateCode,
                            var.UpdateDate,
                            var.GradeYear,
                            var.Department,
                            var.StudentNumber,
                            var.StudentName,
                            var.UpdateDescription,
                            var.StudStatus 
                        });
                    item.Tag = var;
                    listView2.Items.Add(item);
                }
                AutoCheckByDate();
                listView2.ResumeLayout();
                _checkCount = listView2.CheckedItems.Count;
                _totalCount = listView2.Items.Count;
                lblMsg.Text = "已勾選" + _checkCount + "筆異動,共有" + _totalCount + "筆異動.";
            }
        }

        void _BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            // 產生異動需要資料
            _StudUpdateRecBatchCreator.UpdateRecConvertUpdateRecDoc(_UpdateType);
            if (_BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }

        }

        private void btnWF2Next_Click(object sender, EventArgs e)
        {
            
            int i = 0;
            dtBeginDate.IsEmpty = true; dtEndDate.IsEmpty = true;

            if (int.TryParse(cbxSchoolYear.Text, out i))
            {
                if (cbxSemester.Text == "1")
                {
                    dtBeginDate.Text = "" + (i + 1911) + "/8/1";
                    dtEndDate.Text = "" + (i + 1911 + 1) + "/1/31";
                }
                if (cbxSemester.Text == "2")
                {
                    dtBeginDate.Text = "" + (i + 1911 + 1) + "/2/1";
                    dtEndDate.Text = "" + (i + 1911 + 1) + "/7/31";
                }
                btnWF2Next.Enabled = true;
            }
            else
                btnWF2Next.Enabled = false;


            this.Visible = false;
            int xm, ym;
            xm = (this.Width - panelEx3.Width - 20) / 2;
            ym = (this.Height - panelEx3.Height - 40) / 2;
            this.Top += ym;
            this.Left += xm;
            this.SuspendLayout();
            panelEx1.SuspendLayout();
            panelEx2.SuspendLayout();
            panelEx3.SuspendLayout();
            this.Height = panelEx3.Height + 40;
            this.Width = panelEx3.Width + 20;
            panelEx1.Visible = false;
            panelEx2.Visible = false;
            panelEx3.Visible = true;
            panelEx1.Dock = DockStyle.None;
            panelEx2.Dock = DockStyle.None;
            panelEx3.Dock = DockStyle.Fill;
            this.Visible = true;
            panelEx1.ResumeLayout();
            panelEx2.ResumeLayout();
            panelEx3.ResumeLayout();
            
            this.ResumeLayout();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnWF1Next.Enabled = (listView1.SelectedItems.Count == 1);

        }

        private void BuildWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_BackgroundWorker.IsBusy)
            {
                _BackgroundWorker.CancelAsync();
            }
            if (this.DialogResult != DialogResult.OK)
                this.DialogResult = DialogResult.Cancel;
        }
        


        private void btnCreateDoc_Click(object sender, EventArgs e)
        {
            List<BL.StudUpdateRecDoc> list = new List<UpdateRecordModule_IBSH.BL.StudUpdateRecDoc>();
            foreach (ListViewItem var in listView2.Items)
            {
                if (var.Checked)
                    list.Add((BL.StudUpdateRecDoc)var.Tag);
            }

            try
            {
                _StudUpdateRecBatchCreator.CreateUpdateRecBatchDoc(cbxSchoolYear.Text, cbxSemester.Text,txtNameListName.Text, list);                
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("產生名冊XML發生錯誤" + ex.Message);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void AutoCheckByDate()
        {
            DateTime begin = DateTime.Now;
            DateTime end = DateTime.Now;
            DateTime updateDate = DateTime.Now;
            if (DateTime.TryParse(dtBeginDate.Text, out begin) && DateTime.TryParse(dtEndDate.Text, out end))
            {
                foreach (ListViewItem var in listView2.Items)
                {
                    BL.StudUpdateRecDoc rec = (BL.StudUpdateRecDoc)var.Tag;

                    if (DateTime.TryParse(rec.UpdateDate, out updateDate) && updateDate >= begin && updateDate <= end)
                    {
                        var.ForeColor = Color.Blue;
                        var.Checked = true;
                    }
                    else
                    {
                        var.ForeColor = listView2.ForeColor;
                        var.Checked = false;
                    }
                }
            }
        }

        private void dtBeginDate_TextChanged(object sender, EventArgs e)
        {
            AutoCheckByDate();
        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
                _checkCount++;
            else
                _checkCount--;
        

            lblMsg.Text = "已勾選" + _checkCount + "筆異動,共有" + _totalCount + "筆異動.";
        }

        private void cbxSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            setDefaultNameListName();
        }

        private void cbxSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            setDefaultNameListName();
        }
    }
}
