using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHSchool.Data;
using DevComponents.DotNetBar;
using UpdateRecordModule_IBSH.BL;
using System.IO;
using UpdateRecordModule_IBSH.GovernmentalDocument.Reports.List;
using System.ComponentModel;
using System.Xml;


namespace UpdateRecordModule_IBSH.GovernmentalDocument
{
    public partial class ListForm : FISCA.Presentation.Controls.BaseForm
    {

        public ListForm()
        {
            InitializeComponent();
            Initialize();
            Global.UpdateDocs += new EventHandler(Global_UpdateDocs);
        }

        void Global_UpdateDocs(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            btnEReport1.Visible = false;
            btnEReport2.Visible = false;
            lblADName1.Text = "";
            lblADName2.Text = "";
            lblADName3.Text = "";
            lblADInfo.Text = "";
            lblADCounter.Text = "";
            lblADName3.Text = "";
            // 還不能用的控制項 disabled
            btnDelete.Enabled = false;
            btnAD.Enabled = false;
            lblListContent.Text = "";
            lblAD.Text = "";
            listView.Clear();
            
            cboSchoolYear.SelectedItem = null;
            cboSchoolYear.Items.Clear();

            foreach (string year in DAL.DALTransfer.GetGovDocSchoolYearList())
            {
                string schoolYear = year + " 學年度";
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(year, schoolYear);
                cboSchoolYear.Items.Add(kvp);
            }
            cboSchoolYear.DisplayMember = "Value";
            cboSchoolYear.ValueMember = "Key";

            // 預選為當學年度
            int selectedIndex = 0;
            if (cboSchoolYear.Items.Count > 0)
            {
                for (int i = 0; i < cboSchoolYear.Items.Count; i++)
                {
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)cboSchoolYear.Items[i];
                    if (kvp.Key == K12.Data.School.DefaultSchoolYear)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                cboSchoolYear.SelectedIndex = selectedIndex;
            }
        }


        private void LoadBatchList()
        {
            itemPanelName.SuspendLayout();
            itemPanelName.Items.Clear();
            KeyValuePair<string, string> kvp;//= (KeyValuePair<string, string>)cboSchoolYear.SelectedItem;
            string schoolYear;
            if (cboSchoolYear.SelectedItem == null)
            {
                return;
            }
            else
            {
                kvp = (KeyValuePair<string, string>)cboSchoolYear.SelectedItem;
                schoolYear = kvp.Key;
            }

            List<SHUpdateRecordBatchRecord> SHURBRList = new List<SHUpdateRecordBatchRecord> ();
            int sy;
            if (int.TryParse(schoolYear, out sy))
            {
                SHURBRList.AddRange(SHUpdateRecordBatch.SelectBySchoolYearAndSemester(sy,1));
                SHURBRList.AddRange(SHUpdateRecordBatch.SelectBySchoolYearAndSemester(sy,2));
            }

            
           

            if(SHURBRList.Count>0)
            {
                // 排序
                var x1 = from x in SHURBRList orderby x.ADDate.HasValue ascending ,x.Name ascending select x;
                SHURBRList = x1.ToList();
            }
                

            foreach (SHUpdateRecordBatchRecord rec in SHURBRList)
            {
                int imageIndex = !string.IsNullOrEmpty(rec.ADNumber) ? 1 : 0;
                ButtonItem btnItem = new ButtonItem();
                btnItem.Tag = rec.ID;
                btnItem.Name = rec.ID + rec.Name;
                btnItem.Text = rec.Name;
                btnItem.OptionGroup = "itemPanelName";
                btnItem.ImageIndex = imageIndex;
                btnItem.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
                btnItem.Click += new EventHandler(btnItem_Click);
                itemPanelName.Items.Add(btnItem);
            }

            itemPanelName.ResumeLayout();
            itemPanelName.Refresh();

        }

        public void LoadData()
        {
            if (itemPanelName.SelectedItems.Count > 0)
            {
                btnDelete.Enabled = true;
                btnAD.Enabled = true;
                SetSelectUpdateBatchID();
                //ButtonItem item = sender as ButtonItem;
                string id = BL.Get.UpdateBatchSelectID();

                // 取得所選名冊
                BL.StudUpdateRecBatchRec rec = DAL.DALTransfer.GetStudUpdateRecBatchRec(id);

                // 取得名冊內科別數
                List<string> DeptNameList = (from dept in rec.StudUpdateRecDocList orderby dept.Department ascending select dept.Department).Distinct().ToList();

                // 人數統計儲存
                Dictionary<string, int> TotalCountDict = new Dictionary<string, int>();
                Dictionary<string, int> BoyCountDict = new Dictionary<string, int>();
                Dictionary<string, int> GirlCountDict = new Dictionary<string, int>();
                Dictionary<string, int> UnKnowCountDict = new Dictionary<string, int>();

                // 計算人數
                foreach (string str in DeptNameList)
                {
                    int total = 0, boy = 0, girl = 0, UnKnow = 0;

                    foreach (BL.StudUpdateRecDoc val in rec.StudUpdateRecDocList.Where(x => x.Department == str))
                    {
                        if (val.Gender == "男")
                            boy++;
                        else if (val.Gender == "女")
                            girl++;
                        else
                            UnKnow++;

                        total++;
                    }
                    if (string.IsNullOrEmpty(str))
                    {
                        TotalCountDict.Add("未分科別", total);
                        GirlCountDict.Add("未分科別", girl);
                        BoyCountDict.Add("未分科別", boy);
                        UnKnowCountDict.Add("未分科別", UnKnow);
                    }
                    else
                    {
                        TotalCountDict.Add(str, total);
                        GirlCountDict.Add(str, girl);
                        BoyCountDict.Add(str, boy);
                        UnKnowCountDict.Add(str, UnKnow);
                    }
                }

                if (DeptNameList.Count > 0)
                {
                    listView.SuspendLayout();

                    lblADCounter.Text = "【" + rec.Name + "】 各科人數統計";

                    //lblADInfo.Text = "【" + rec.ADNumber + "】 核准文號";
                    lblADInfo.Text = "【" + rec.Name + "】 核准文號";
                    btnEReport1.Visible = true;
                    btnEReport2.Visible = true;

                    //處理看板資訊
                    StringBuilder builder = new StringBuilder("");
                    foreach (string deptN in DeptNameList)
                    {
                        string deptName = "";
                        if (string.IsNullOrEmpty(deptN))
                            deptName = "未分科別";
                        else
                            deptName = deptN;

                        if (GirlCountDict.ContainsKey(deptName) && TotalCountDict.ContainsKey(deptName) && BoyCountDict.ContainsKey(deptName))
                        {

                            builder = builder.Append("◎").Append(deptName).Append("　")
                                .Append("男生 <font color='blue'>").Append(BoyCountDict[deptName]).Append("</font> 人　")
                                .Append("女生 <font color='blue'>").Append(GirlCountDict[deptName]).Append("</font> 人　")
                                .Append("(合計 <font color='blue'>").Append(TotalCountDict[deptName]).Append("</font> 人)");
                        }

                        if (UnKnowCountDict.ContainsKey(deptName))
                            if (UnKnowCountDict[deptName] > 0)
                            {
                                builder = builder.Append("<font color='red'>").Append(UnKnowCountDict[deptName])
                                .Append("</font>人未填性別");
                            }
                        builder = builder.Append("<br/>");
                    }
                    lblListContent.Text = builder.ToString();
                    lblADName1.Text = rec.Name;
                    lblADName2.Text = rec.Name;
                    lblADName3.Text = rec.Name;

                    // 處理核准日期與文號
                    string adString = "";
                    if (!string.IsNullOrEmpty(rec.ADNumber))
                    {
                        adString += "核准文號　<font color='red'>" + rec.ADNumber + "</font>　";
                        if (rec.ADDate.HasValue)
                            adString += "核准日期　<font color='red'>" + rec.ADDate.Value.ToShortDateString() + "</font>";
                    }
                    else
                        adString = "<font color='red'>未登錄</font>";
                    lblAD.Text = adString;

                    //處理ListView呈現資料
                    listView.Clear();

                    // 欄位名稱
                    Dictionary<string, int> _ColumnWidthDict = new Dictionary<string, int>();
                    _ColumnWidthDict.Add("學號", 100);
                    _ColumnWidthDict.Add("姓名", 100);
                    _ColumnWidthDict.Add("年級", 100);
                    _ColumnWidthDict.Add("科別", 100);
                    _ColumnWidthDict.Add("異動原因及事項", 100);
                    _ColumnWidthDict.Add("異動日期", 100);

                    foreach (string strDeptName in DeptNameList)
                    {
                        // 若無群組則先加上群組
                        // 因為沒有科別先註掉
                        if (listView.Groups[strDeptName] == null)
                            listView.Groups.Add(strDeptName, strDeptName);

                        // 若沒有欄名則先加上欄名
                        if (listView.Columns.Count == 0)
                        {
                            foreach (string column in _ColumnWidthDict.Keys)
                                listView.Columns.Add(column, _ColumnWidthDict[column]);
                        }

                        ListViewItem rowItem = null;
                        foreach (BL.StudUpdateRecDoc val in rec.StudUpdateRecDocList.Where(x => x.Department == strDeptName))
                        {
                            //string strNum = val.StudentNumber;
                            //// 當異動代碼211讀取目前學生學號
                            //if (val.UpdateCode == "211")
                            //{
                            //    List<string> ids= new List<string> ();
                            //    ids.Add(val.StudentID);
                            //    SHStudent.RemoveByIDs(ids);
                            //    SHStudentRecord stud = SHStudent.SelectByID(val.StudentID);
                            //    strNum = stud.StudentNumber;
                            //}

                            //rowItem = new ListViewItem(strNum);
                            rowItem = new ListViewItem(val.StudentNumber);
                            rowItem.SubItems.Add(val.StudentName);
                            rowItem.SubItems.Add(val.GradeYear);
                            rowItem.SubItems.Add(val.Department);
                            rowItem.SubItems.Add(val.UpdateDescription);
                            rowItem.SubItems.Add(val.UpdateDate);
                            rowItem.Group = listView.Groups[strDeptName];
                            listView.Items.Add(rowItem);
                        }
                    }
                }
                listView.ResumeLayout();
            }
            else
            {
                btnDelete.Enabled = false;
                btnAD.Enabled = false;
            }

            this.itemPanel1.RecalcLayout();

        
        }

        void btnItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }



        private void ListForm_Load(object sender, EventArgs e)
        {

        }

        private void cboSchoolYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBatchList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string id = "";
            id = BL.Get.UpdateBatchSelectID();
           
            if (id != "")
                if ( FISCA.Presentation.Controls.MsgBox.Show("您確定刪除該名冊及其內容?", "確認", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SHUpdateRecordBatch.Delete(id);
                    Initialize();
                    LoadBatchList();
                }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            BuildWizard bw = new BuildWizard();            
            //bw.ShowDialog();
            if (bw.ShowDialog() == DialogResult.OK)            
                Initialize();
        }

        private void btnEReport2_Click(object sender, EventArgs e)
        {
            btnEReport2.Enabled = false;
            //BL.Reporter rpt = new BL.Reporter();
            //rpt.Print();
            Print();
            btnEReport2.Enabled = true;
   
        }
        private UpdateRecordADN _adnForm;
        private void btnAD_Click(object sender, EventArgs e)
        {
            
            _adnForm = new UpdateRecordADN(BL.Get.UpdateBatchSelectID());
            _adnForm.ShowDialog();
        }

        private void itemPanelName_ItemClick(object sender, EventArgs e)
        {
            SetSelectUpdateBatchID();
        }

        private void SetSelectUpdateBatchID()
        {
            if (itemPanelName.SelectedItems.Count > 0)
            {
                Global._SelectUpdateBatchID = itemPanelName.SelectedItems[0].Tag.ToString();
            }
        }


        // 畫面上選擇的異動名冊
        UpdateRecordModule_IBSH.BL.StudUpdateRecBatchRec _SelectBRec;
        // 畫面上選擇的異動名冊ID
        string _SelectBRecID;
        string path = "";

        /// <summary>
        /// 產生報表
        /// </summary>
        public void Print()
        {
            // 取得異動名冊畫面上所選擇異動名冊ID
            _SelectBRecID = UpdateRecordModule_IBSH.BL.Get.UpdateBatchSelectID();
            // 透過異動名冊ID取得異動名冊
            _SelectBRec = UpdateRecordModule_IBSH.BL.Get.StudUpdateRecBatchRecByID(_SelectBRecID);

            try
            {             
                    ReportSelector rs = new ReportSelector(_SelectBRec);
                    IReportBuilder builder = rs.GetReport() as IReportBuilder;
                    
                    progressBarX1.Value = 0;
                    pnlReport.Visible = true;

                    builder.ProgressChanged += new ProgressChangedEventHandler(builder_ProgressChanged);
                    builder.Completed += new RunWorkerCompletedEventHandler(builder_Completed);
                    path = Path.Combine(Application.StartupPath, "Reports");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path = Path.Combine(path, _SelectBRec.UpdateType + ".xls");

                    if (File.Exists(path))
                    {
                        int i = 1;
                        while (true)
                        {
                            string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                            if (!File.Exists(newPath))
                            {
                                path = newPath;
                                break;
                            }
                        }
                    }
                    try
                    {
                        File.Create(path).Close();
                    }
                    catch
                    {
                        SaveFileDialog sd = new SaveFileDialog();
                        sd.Title = "另存新檔";
                        sd.FileName = Path.GetFileNameWithoutExtension(path) + ".xls";
                        sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                        if (sd.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                File.Create(sd.FileName);
                                path = sd.FileName;
                            }
                            catch
                            {
                                FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    builder.BuildReport((XmlElement)_SelectBRec.Content.FirstChild, path);

               
                // 產生異動名冊的過程，系統佔用之記憶體，回收之
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("產生電子報表失敗." + ex.Message);                
            }
        }

        void _bwN_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarX1.Value = e.ProgressPercentage;
        }

        void _bwN_DoWork(object sender, DoWorkEventArgs e)
        {
            ReportSelector rs = new ReportSelector(_SelectBRec);
            IReportBuilder builder = rs.GetReport() as IReportBuilder;
            
        }

        void _bwN_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBarX1.Value = 0;
            pnlReport.Visible = false ;            
        }


        void builder_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarX1.Value = e.ProgressPercentage;
        }

        void builder_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBarX1.Value = 0;
            pnlReport.Visible = false;

            try
            {
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception)
            {

            }
        } 
    }
}
