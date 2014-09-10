using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.LogAgent;
using Aspose.Cells;

namespace UpdateRecordModule_IBSH
{
    public partial class DeleteUpdateRecordForm : FISCA.Presentation.Controls.BaseForm
    {
        // 使用  BGWorker
        BackgroundWorker _bgWorker;

        DataTable dt = null;
        List<string> _StudentIDList;
        //  暫存資料
        Dictionary<string, DataRow> _dataRowDict;
        Dictionary<string, StringBuilder> _logDict;
        public DeleteUpdateRecordForm(List<string> StudentIDList)
        {
            InitializeComponent();
            _StudentIDList = StudentIDList;
            _dataRowDict = new Dictionary<string, DataRow>();
            _logDict = new Dictionary<string, StringBuilder>();
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
            // 設定初始
            lblMsg.Text = "";
            btnDel.Enabled = false;


        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 資料載入畫面
            LoadDataToDG();
            btnDel.Enabled = true;
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _dataRowDict.Clear();
            _logDict.Clear();
            // 傳入畫面上選擇學生編號，取得學生異動資料
            dt = null;
            if(_StudentIDList.Count>0)
                dt = DAL.FDQuery.GetUpdateRecordInfoDTByStudentIDList(_StudentIDList);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string id = dr["id"].ToString();
                    _dataRowDict.Add(id, dr);
                }
            }
        }

        private void LoadDataToDG()
        {
            //  清空畫面填入值
            dgData.Rows.Clear();
            lblMsg.Text = "";
            int cot = 0;
            foreach (string id in _dataRowDict.Keys)
            {
                DataRow dr = _dataRowDict[id];

                //學號、班級、座號、姓名、異動日期、異動代碼、原因及事項
                int RowIdx = dgData.Rows.Add();
                //  存入異動編號               
                dgData.Rows[RowIdx].Tag = id;
                dgData.Rows[RowIdx].Cells[colStudentNumber.Index].Value = dr["student_number"].ToString();

                dgData.Rows[RowIdx].Cells[colClassName.Index].Value = dr["class_name"].ToString();
                dgData.Rows[RowIdx].Cells[colSeatNo.Index].Value = dr["seat_no"].ToString();
                dgData.Rows[RowIdx].Cells[colName.Index].Value = dr["name"].ToString();
                dgData.Rows[RowIdx].Cells[colUpdateDate.Index].Value = DateTime.Parse(dr["update_date"].ToString()).ToShortDateString();
                dgData.Rows[RowIdx].Cells[colUpdateCode.Index].Value = dr["update_code"].ToString();
                dgData.Rows[RowIdx].Cells[colUpdateDesc.Index].Value = dr["update_desc"].ToString();

                cot++;
            }
            lblMsg.Text = "共 " + cot + " 筆";
        }

        private void DeleteUpdateRecordForm_Load(object sender, EventArgs e)
        {
            // 執行查詢
            _bgWorker.RunWorkerAsync();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                _logDict.Clear();
                // 檢查是否選擇
                if (dgData.SelectedRows.Count > 0)
                {
                    // 取得異動編號
                    List<string> urId = new List<string>();
                    foreach (DataGridViewRow drv in dgData.SelectedRows)
                    {
                        string id = drv.Tag.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            urId.Add(id);
                            //  記錄 log
                            if (_dataRowDict.ContainsKey(id))
                            { 
                                // 取得學生ID
                                string sid = _dataRowDict[id]["sid"].ToString();
                                if (!_logDict.ContainsKey(sid))
                                    _logDict.Add(sid, new StringBuilder().AppendLine("刪除異動資料："));
                                
                                //學號、班級、座號、姓名、異動日期、異動代碼、原因及事項
                                string logdata = "學號：" + _dataRowDict[id]["student_number"].ToString() + ",班級：" + _dataRowDict[id]["class_name"].ToString() + ".座號：" + _dataRowDict[id]["seat_no"].ToString() + ",姓名：" + _dataRowDict[id]["name"].ToString() + ",異動日期：" + _dataRowDict[id]["update_date"].ToString() + ",異動代碼：" + _dataRowDict[id]["update_code"].ToString() + ",原因及事項：" + _dataRowDict[id]["update_desc"].ToString();
                                _logDict[sid].AppendLine(logdata);
                            }
                        }
                    }

                    // 取得異動資料
                    List<K12.Data.UpdateRecordRecord> urList = K12.Data.UpdateRecord.SelectByIDs(urId);

                    // 刪除資料
                    K12.Data.UpdateRecord.Delete(urList);

                    // Log
                    // 刪除異動 log
                    foreach (string sid in _logDict.Keys)
                    {
                        ApplicationLog.Log("異動資料", "刪除異動", "student", sid,_logDict[sid].ToString());
                    }
                    FISCA.Presentation.Controls.MsgBox.Show("共刪除 " + urList.Count + " 筆異動資料");
                    this.Close();
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("請選擇異動資料");
                }
            }
            catch (Exception ex)
            {
                SmartSchool.ErrorReporting.ErrorMessgae msg = new SmartSchool.ErrorReporting.ErrorMessgae(ex);
                FISCA.Presentation.Controls.MsgBox.Show("刪除異動資料發生錯誤,"+ex.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            if (dgData.Rows.Count > 0)
            {
                try
                {
                    Workbook wb = new Workbook();

                    int col=0;
                    // 欄位名稱
                    foreach (DataGridViewColumn dc in dgData.Columns)
                    {
                        wb.Worksheets[0].Cells[0, col].PutValue(dc.HeaderText);
                        col++;
                    }

                    // 填入資料
                    int rowIdx = 1;
                    foreach (DataGridViewRow drv in dgData.Rows)
                    {
                        if (drv.IsNewRow)
                            continue;
                        int colIdx=0;
                        foreach (DataGridViewCell cell in drv.Cells)
                        {
                            if(cell.Value !=null)
                                wb.Worksheets[0].Cells[rowIdx, colIdx].PutValue(cell.Value.ToString());
                            colIdx++;
                        }
                        rowIdx++;
                    }
                    wb.Worksheets[0].AutoFitColumns();

                    utility.ExportXls("異動資料(刪除)", wb);
                    btnExport.Enabled = true;
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("匯出失敗," + ex.Message);
                    btnExport.Enabled = true;
                }
            }
        }
    }
}
