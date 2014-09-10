using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHSchool.Data;

namespace UpdateRecordModule_IBSH.Batch
{
    public partial class BatchGraduateRecForm : FISCA.Presentation.Controls.BaseForm
    {
        BackgroundWorker _bgWorkerLoad;
        BackgroundWorker _bgWorkerRun;
        // 學生已有畢業異動
        Dictionary<string, SHUpdateRecordRecord> _StudHasGraduateRecDict = new Dictionary<string, SHUpdateRecordRecord>();
        // 最後一筆異動
        Dictionary<string, SHUpdateRecordRecord> _StudLastupdateRecDict = new Dictionary<string, SHUpdateRecordRecord>();
        // 畢業證書字號
        Dictionary<string, SHLeaveInfoRecord> _LeaveInfoRecordDict = new Dictionary<string, SHLeaveInfoRecord>();

        // 班級
        Dictionary<string, SHClassRecord> _ClassRecDict = new Dictionary<string, SHClassRecord>();

        // 科別
        Dictionary<string, SHDepartmentRecord> _DeptRecDict = new Dictionary<string, SHDepartmentRecord>();

        List<string> _StudentIDList;
        /// <summary>
        /// 異動日期
        /// </summary>
        DateTime _UpdateDate = DateTime.Now;
        Dictionary<string, SHStudentRecord> _StudentDict = new Dictionary<string, SHStudentRecord>();

        // 新增資料
        List<SHUpdateRecordRecord> _InsertDataList = new List<SHUpdateRecordRecord>();

        public BatchGraduateRecForm()
        {
            InitializeComponent();
            _StudentIDList = K12.Presentation.NLDPanels.Student.SelectedSource;
            _bgWorkerLoad = new BackgroundWorker();
            _bgWorkerLoad.DoWork += new DoWorkEventHandler(_bgWorkerLoad_DoWork);
            _bgWorkerLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorkerLoad_RunWorkerCompleted);
            _bgWorkerRun = new BackgroundWorker();
            _bgWorkerRun.DoWork += new DoWorkEventHandler(_bgWorkerRun_DoWork);
            _bgWorkerRun.WorkerReportsProgress = true;
            _bgWorkerRun.ProgressChanged += new ProgressChangedEventHandler(_bgWorkerRun_ProgressChanged);
            _bgWorkerRun.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorkerRun_RunWorkerCompleted);
         
        }

        void _bgWorkerRun_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage("批次畢業異動產生中", e.ProgressPercentage);
        }

        void _bgWorkerRun_DoWork(object sender, DoWorkEventArgs e)
        {
            // 處理資料
            try
            {
                _bgWorkerRun.ReportProgress(1);
                // 刪除舊資料
                List<SHUpdateRecordRecord> _oldDataList = _StudHasGraduateRecDict.Values.ToList();
                SHUpdateRecord.Delete(_oldDataList);

                int SchoolYear = int.Parse(K12.Data.School.DefaultSchoolYear);
                int Semester = int.Parse(K12.Data.School.DefaultSemester);


                // 學籍身分對照表
                Dictionary<string, List<string>> StudPermCodeMappingDict = utility.GetPermCodeMappingDict();

                // 取得學生類別對照
                Dictionary<string, List<string>> StudentTagDict = utility.GetStudentFullNameDictByStudentIDs(_StudentIDList);

                _bgWorkerRun.ReportProgress(10);
                foreach (string sid in _StudentIDList)
                {
                    if (!_StudentDict.ContainsKey(sid))
                        continue;

                    SHUpdateRecordRecord rec = new SHUpdateRecordRecord();
                    // 學年度
                    rec.SchoolYear = SchoolYear;
                    // 學期
                    rec.Semester = Semester;

                    rec.StudentID = sid;

                    string ClassName = "";
                    string DeptName = "";

                    // 年級
                    if (_ClassRecDict.ContainsKey(_StudentDict[sid].RefClassID))
                    {
                        if (_ClassRecDict[_StudentDict[sid].RefClassID].GradeYear.HasValue)
                        {
                            rec.GradeYear = _ClassRecDict[_StudentDict[sid].RefClassID].GradeYear.Value.ToString();

                            // 班級名稱
                            ClassName = _ClassRecDict[_StudentDict[sid].RefClassID].Name;
                        }
                        else
                            rec.GradeYear = "";
                    }
                        string depID = _StudentDict[sid].DepartmentID;
                        // 科別名稱
                        if (_DeptRecDict.ContainsKey(depID))
                            DeptName = _DeptRecDict[depID].FullName;

                        //// 科別名稱
                        //if (_DeptRecDict.ContainsKey(_ClassRecDict[_StudentDict[sid].RefClassID].RefDepartmentID))
                        //    DeptName = _DeptRecDict[_ClassRecDict[_StudentDict[sid].RefClassID].RefDepartmentID].FullName;

                    // 畢業代碼
                    rec.UpdateCode = "501";

                    // 原因及事項
                    rec.UpdateDescription = "畢業";

                    // 異動日期
                    rec.UpdateDate = _UpdateDate.ToShortDateString();
                                        
                    // 姓名
                    rec.StudentName = _StudentDict[sid].Name;
                    
                    // 學號
                    rec.StudentNumber = _StudentDict[sid].StudentNumber;

                    // 身分證字號
                    rec.IDNumber = _StudentDict[sid].IDNumber;

                    // 生日
                    if (_StudentDict[sid].Birthday.HasValue)
                        rec.Birthdate = _StudentDict[sid].Birthday.Value.ToShortDateString();

                    // 性別
                    rec.Gender = _StudentDict[sid].Gender;                    

                    if (_StudLastupdateRecDict.ContainsKey(sid))
                    {
                        // 備查日期
                        rec.LastADDate = _StudLastupdateRecDict[sid].ADDate;

                        // 備查文號
                        rec.LastADNumber = _StudLastupdateRecDict[sid].ADNumber;

                        // 最後異動代碼
                        rec.LastUpdateCode = _StudLastupdateRecDict[sid].UpdateCode;
                    }
                    // 畢業證書字號
                    if (_LeaveInfoRecordDict.ContainsKey(sid))
                    {
                        rec.GraduateCertificateNumber = _LeaveInfoRecordDict[sid].DiplomaNumber;
                        rec.Department = DeptName;
                        rec.ExpectGraduateSchoolYear = SchoolYear.ToString();
                        _LeaveInfoRecordDict[sid].SchoolYear = SchoolYear;
                        _LeaveInfoRecordDict[sid].ClassName = ClassName;
                        _LeaveInfoRecordDict[sid].DepartmentName = DeptName;
                    }

                    // 學生特殊身分代碼
                    if (StudentTagDict.ContainsKey(sid))
                    {
                        List<string> codeList = new List<string>();
                        foreach (string fullName in StudentTagDict[sid])
                        {
                            if (StudPermCodeMappingDict.ContainsKey(fullName))
                            {
                                foreach (string code in StudPermCodeMappingDict[fullName])
                                    if (!codeList.Contains(code))
                                        codeList.Add(code);
                            }
                        }

                        if (codeList.Count > 0)
                        {
                            codeList.Sort();
                            rec.SpecialStatus = string.Join(",", codeList.ToArray());
                        }
                    }

                    _InsertDataList.Add(rec);
                    
                }
                _bgWorkerRun.ReportProgress(70);
                // 新增資料
                if (_InsertDataList.Count > 0)
                    SHUpdateRecord.Insert(_InsertDataList);

                // 更新畢業離校資訊 離校學年度、離校科別、離校班級
                SHLeaveInfo.Update(_LeaveInfoRecordDict.Values.ToList());

                _bgWorkerRun.ReportProgress(99);
            }
            catch (Exception ex)
            {
                e.Result = ex;
                e.Cancel = true;
            }
        }

        void _bgWorkerRun_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Exception ex = e.Result as Exception;
                FISCA.Presentation.Controls.MsgBox.Show("產生過程發生錯誤:" + ex.Message);
            }
            else
            {
                SmartSchool.StudentRelated.Student.Instance.SyncDataBackground(_StudentIDList);
                FISCA.Presentation.Controls.MsgBox.Show("產生完成" + _InsertDataList.Count+"筆資料");                
            }
            FISCA.Presentation.MotherForm.SetStatusBarMessage("批次畢業異動產生完成.", 100);
            this.Close();
        }

        void _bgWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            // 取得就學生畢業異動資料
            _StudHasGraduateRecDict.Clear();
            List<SHUpdateRecordRecord> recList = SHUpdateRecord.SelectByStudentIDs(_StudentIDList);
            foreach (SHUpdateRecordRecord rec in recList)
            {
                // 不是畢業異動跳過
                if (rec.UpdateCode != "501")
                    continue;

                if (!_StudHasGraduateRecDict.ContainsKey(rec.StudentID))
                    _StudHasGraduateRecDict.Add(rec.StudentID, rec);
            }

            // 取得最後一筆異動
            _StudLastupdateRecDict.Clear();
            foreach (string sid in _StudentIDList)
            {
                List<SHUpdateRecordRecord> recL = (from data in recList where data.StudentID == sid && data.ADDate.Trim() != "" orderby DateTime.Parse(data.ADDate) descending, int.Parse(data.ID) descending select data).ToList();
                if (recL.Count > 0)
                {
                    SHUpdateRecordRecord r1 = recL[0];
                    if (!_StudLastupdateRecDict.ContainsKey(r1.StudentID))
                        _StudLastupdateRecDict.Add(r1.StudentID, r1);
                }
            }

            // 學生資料
            _StudentDict.Clear();
            foreach (SHStudentRecord rec in SHStudent.SelectByIDs(_StudentIDList))
                _StudentDict.Add(rec.ID, rec);

            // 畢業證書
            _LeaveInfoRecordDict.Clear();
            foreach (SHLeaveInfoRecord rec in SHLeaveInfo.SelectByStudentIDs(_StudentIDList))
            {
                if (!_LeaveInfoRecordDict.ContainsKey(rec.RefStudentID))
                    _LeaveInfoRecordDict.Add(rec.RefStudentID, rec);
            }

            // 班級
            _ClassRecDict.Clear();
            foreach (SHClassRecord rec in SHClass.SelectAll())
                if (!_ClassRecDict.ContainsKey(rec.ID))
                    _ClassRecDict.Add(rec.ID, rec);

            // 科別
            _DeptRecDict.Clear();
            foreach (SHDepartmentRecord rec in SHDepartment.SelectAll())
                if (!_DeptRecDict.ContainsKey(rec.ID))
                    _DeptRecDict.Add(rec.ID, rec);
        }

        void _bgWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRun.Enabled = true;
        }
    

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            // 檢查異動日期
            if (dtUpdate.IsEmpty)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請輸入異動日期!");
                return;
            }
            _UpdateDate = dtUpdate.Value;
            // 檢查是否已有資料
            if (_StudHasGraduateRecDict.Count > 0)
            {
                BatchGraduateRec_WarningForm war = new BatchGraduateRec_WarningForm(_ClassRecDict, _StudentDict, _StudHasGraduateRecDict.Values.ToList());
                if (war.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    _bgWorkerRun.RunWorkerAsync();
                }
                else
                    this.Close();
             
            }
            else
                _bgWorkerRun.RunWorkerAsync();
        }

        private void BatchGraduateRecForm_Load(object sender, EventArgs e)
        {
            this.btnRun.Enabled = false;
            dtUpdate.Value = DateTime.Now;
            _bgWorkerLoad.RunWorkerAsync();
        }
    }
}
