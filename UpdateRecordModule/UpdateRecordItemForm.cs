using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using UpdateRecordModule_IBSH.UpdateRecordItemControls;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using SHSchool.Data;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH
{
    [FCode(Program.UpdateRecordContentCode, "異動資料")]
    public partial class UpdateRecordItemForm : FISCA.Presentation.Controls.BaseForm
    {
        // 學生異動
        private SHUpdateRecordRecord _StudUpdateRec;
        private SHUpdateRecordRecord _DefStudUpdateRec;

        public enum actMode { 新增, 修改 };
        private actMode _actMode;
        private string _StudentID;
        bool _checkSave = false;

        // Log
        private UpdateRecordModule_IBSH.PermRecLogProcess _prlp;

        // 異動代碼
        XElement _UpdateCode;

        public void setCbxSelIndex(int Idx)
        {
            cbxSel.SelectedIndex = Idx;
        }
        public UpdateRecordItemForm(actMode mode, SHUpdateRecordRecord StudUdRecEnty, string StudentID)
        {
            InitializeComponent();

            _actMode = mode;
            _StudentID = StudentID;
            _checkSave = false;
            if (FISCA.Permission.UserAcl.Current[GetType()].Editable)
                btnConfirm.Enabled = true;
            else
                btnConfirm.Enabled = false;

            _StudUpdateRec = StudUdRecEnty;

            _DefStudUpdateRec = StudUdRecEnty;
            _prlp = new UpdateRecordModule_IBSH.PermRecLogProcess();

            // 取得異動代碼
            _UpdateCode = DAL.DALTransfer.GetUpdateCodeList();

            // 先將畫面學年度、學期、年級設空
            intSchoolYear.IsEmpty = true;
            intSemester.IsEmpty = true;
            cbxGradeYear.Text = "";

            for (int i = 1; i <= 12; i++)
                cbxGradeYear.Items.Add(i + "");

            //cbxGradeYear.Items.Add("延修生");

            if (mode == actMode.新增)
            {
                cbxSel.Enabled = true;

                cbxSel.Items.Add("學籍異動");
                cbxSel.Items.Add("他校轉入");
                cbxSel.Items.Add("新生異動");
                cbxSel.Items.Add("畢業異動");

                cbxSel.SelectedIndex = 0;
                SetDefaultSchoolYearSemester();

                // 加入 log
                _prlp.SetAction("新增");
            }

            if (mode == actMode.修改)
            {
                cbxSel.Enabled = false;
                UpdateRecordEditorPanle.Controls.Clear();

                UserControl ui = CreateByUpdateCode();
                UpdateRecordEditorPanle.Controls.Add(ui);
                UpdateRecordEditorPanle.Size = ui.Size;
                Size s1 = new System.Drawing.Size();
                s1 = this.Size;
                s1.Height = ui.Size.Height + 120;
                this.Size = s1;

                // 加入 log
                _prlp.SetAction("修改");
            }

            // 加入 log
            _prlp.SetBeforeSaveText("學年度", intSchoolYear.Text);
            _prlp.SetBeforeSaveText("學期", intSemester.Text);
            //if (_StudUpdateRec.Student.Status == K12.Data.StudentRecord.StudentStatus.延修)
            //    cbxGradeYear.Text = "延修生";
        }

        private void UpdateRecordItemForm_Load(object sender, EventArgs e)
        {

        }

        // 設定畫面上學年度學期
        private void SetLoadUpdateSchoolYearSemester(string SchoolYear, string Semester, string GradeYear)
        {
            if (string.IsNullOrEmpty(SchoolYear) && string.IsNullOrEmpty(Semester) && string.IsNullOrEmpty(GradeYear))
                SetDefaultSchoolYearSemester();
            else
            {
                int sy, sm, gr;

                if (int.TryParse(SchoolYear, out sy))
                    intSchoolYear.Value = sy;
                else
                    intSchoolYear.IsEmpty = true;

                if (int.TryParse(Semester, out sm))
                    intSemester.Value = sm;
                else
                    intSemester.IsEmpty = true;

                if (GradeYear == "延修生")
                    cbxGradeYear.Text = "延修生";
                else
                {
                    if (int.TryParse(GradeYear, out gr))
                        cbxGradeYear.Text = gr.ToString();
                    else
                        cbxGradeYear.Text = "";
                }
            }

        }

        public UserControl CreateByUpdateType(string FormUpdateType)
        {
            // 001-新生
            List<XElement> UpdateCodeElms = (from elm in _UpdateCode.Elements("異動") where elm.Element("分類").Value == FormUpdateType select elm).ToList();

            if (_StudUpdateRec == null)
                _StudUpdateRec = _DefStudUpdateRec;

            // 日校
            if (FormUpdateType == "學籍異動")
                return new UpdateRecordInfo01(_StudUpdateRec, _prlp, UpdateCodeElms);
            else if (FormUpdateType == "轉入異動")
                return new UpdateRecordInfo02(_StudUpdateRec, _prlp, UpdateCodeElms);
            else if (FormUpdateType == "新生異動")
                return new UpdateRecordInfo03(_StudUpdateRec, _prlp, UpdateCodeElms);
            else if (FormUpdateType == "畢業異動")
                return new UpdateRecordInfo04(_StudUpdateRec, _prlp, UpdateCodeElms);
            else
                return null;

        }

        public UserControl CreateByUpdateCode()
        {
            // 初始化資料
            string strSchoolYear = string.Empty, strSemester = string.Empty, strUpdateType = string.Empty;
            if (_StudUpdateRec.SchoolYear.HasValue)
                strSchoolYear = _StudUpdateRec.SchoolYear.Value.ToString();
            if (_StudUpdateRec.Semester.HasValue)
                strSemester = _StudUpdateRec.Semester.Value.ToString();



            // 設定畫面上學年度學期年級資料
            SetLoadUpdateSchoolYearSemester(strSchoolYear, strSemester, _StudUpdateRec.GradeYear);

            // 用異動代碼判斷是哪種異動
            string UpdateType = string.Empty;
            List<string> xx = (from elm in _UpdateCode.Elements("異動") where elm.Element("代號").Value == _StudUpdateRec.UpdateCode select elm.Element("分類").Value).ToList();
            if (xx.Count > 0)
                UpdateType = xx[0];

            // 001-新生
            List<XElement> UpdateCodeElms = (from elm in _UpdateCode.Elements("異動") where elm.Element("分類").Value == UpdateType select elm).ToList();
            if (_StudUpdateRec == null)
                _StudUpdateRec = _DefStudUpdateRec;


            if (UpdateType == "學籍異動")
            {
                cbxSel.Text = "學籍異動";
                return new UpdateRecordInfo01(_StudUpdateRec, _prlp, UpdateCodeElms);
            }
            else if (UpdateType == "轉入異動")
            {
                cbxSel.Text = "他校轉入";
                return new UpdateRecordInfo02(_StudUpdateRec, _prlp, UpdateCodeElms);
            }
            else if (UpdateType == "新生異動")
            {
                cbxSel.Text = "新生異動";
                return new UpdateRecordInfo03(_StudUpdateRec, _prlp, UpdateCodeElms);
            }
            else if (UpdateType == "畢業異動")
            {
                cbxSel.Text = "畢業異動";
                return new UpdateRecordInfo04(_StudUpdateRec, _prlp, UpdateCodeElms);
            }
            else
                return null;


        }
        private void cbxSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRecordEditorPanle.Controls.Clear();

            UserControl ui;

            if (cbxSel.Text == "他校轉入")
                ui = CreateByUpdateType("轉入異動");
            else
                ui = CreateByUpdateType(cbxSel.Text);

            UpdateRecordEditorPanle.Controls.Add(ui);
            UpdateRecordEditorPanle.Size = ui.Size;

            Size s1 = new Size();
            s1 = this.Size;
            s1.Height = UpdateRecordEditorPanle.Size.Height + 120;
            this.Size = s1;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {

            if (UpdateRecordEditorPanle.Controls.Count > 0)
            {

                IUpdateRecordInfo IU;
                IU = UpdateRecordEditorPanle.Controls[0] as IUpdateRecordInfo;
                _StudUpdateRec = IU.GetStudUpdateRecord();
                _prlp = IU.GetLogData();

                int codeInt;
                if (_StudUpdateRec != null)
                    if (int.TryParse(_StudUpdateRec.UpdateCode, out codeInt) && _actMode == actMode.新增)
                    {
                        // 檢查是否有新生異動
                        List<SHUpdateRecordRecord> UpRec01List = (from data in SHUpdateRecord.SelectByStudentID(_StudentID) where int.Parse(data.UpdateCode) < 100 select data).ToList();
                        if (UpRec01List.Count > 0 && codeInt < 100)
                            if (FISCA.Presentation.Controls.MsgBox.Show("已有" + UpRec01List.Count + "筆新生異動，是否覆蓋", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                                SHUpdateRecord.Delete(UpRec01List);
                            else
                                return;

                        // 檢查是否有畢業異動
                        List<SHUpdateRecordRecord> UpRec05List = (from data in SHUpdateRecord.SelectByStudentID(_StudentID) where int.Parse(data.UpdateCode) > 500 select data).ToList();
                        if (UpRec05List.Count > 0 && codeInt > 500)
                            if (FISCA.Presentation.Controls.MsgBox.Show("已有" + UpRec01List.Count + "筆畢業異動，是否覆蓋", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                                SHUpdateRecord.Delete(UpRec05List);
                            else
                                return;
                    }

                if (_StudUpdateRec != null)
                {
                    // 儲存學年度學期 年級
                    _StudUpdateRec.SchoolYear = intSchoolYear.Value;
                    _StudUpdateRec.Semester = intSemester.Value;

                    //// 過濾科別:
                    //int deptIdx = _StudUpdateRec.Department.IndexOf(":");
                    //if (deptIdx > 1)
                    //    _StudUpdateRec.Department = _StudUpdateRec.Department.Substring(0, deptIdx);


                    if (cbxGradeYear.Text == "延修生")
                        _StudUpdateRec.GradeYear = "延修生";
                    else
                        _StudUpdateRec.GradeYear = cbxGradeYear.Text;

                    // 儲存異動資料
                    string strItemName = "";
                    if (_actMode == actMode.新增)
                    {
                        SHUpdateRecord.Insert(_StudUpdateRec);
                        strItemName = "新增:";
                    }
                    else
                    {
                        SHUpdateRecord.Update(_StudUpdateRec);
                        strItemName = "修改:";
                    }

                    SHStudentRecord studRec = SHStudent.SelectByID(_StudentID);
                    // Log                
                    strItemName += "學生姓名:" + studRec.Name + _actMode.ToString();

                    _prlp.SetActionBy("學生", strItemName);
                    _prlp.SaveLog("", ":", "student", _StudentID);
                    _checkSave = true;

                    // 取得 UpdateCode
                    int intUpdateCode;
                    int.TryParse(_StudUpdateRec.UpdateCode, out intUpdateCode);



                    // 學生資料 Cache
                    List<SHStudentRecord> AllStudRec = null;


                    // 復學，學生狀態是非一般，輸入異動代碼是復學，問使用者是否轉成一般。
                    if (intUpdateCode >= 221 && intUpdateCode <= 226 && studRec.Status != K12.Data.StudentRecord.StudentStatus.一般)
                        if (FISCA.Presentation.Controls.MsgBox.Show("請問是否將學生狀態變更成一般？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (AllStudRec == null)
                                AllStudRec = SHStudent.SelectAll();

                            // 檢查將變更學生狀態
                            // 身分證
                            List<string> studIDNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.一般 && stud.IDNumber == studRec.IDNumber select stud.ID).ToList();
                            // 學號
                            List<string> studNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.一般 && stud.StudentNumber == studRec.StudentNumber select stud.ID).ToList();

                            if (studNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在一般狀態有相同的學號:" + studRec.StudentNumber + ",無法變更學生狀態");

                            if (studIDNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在一般狀態有相同的身分證號:" + studRec.IDNumber + ",無法變更學生狀態");

                            if (studIDNumber.Count == 0 && studNumber.Count == 0)
                            {
                                studRec.Status = K12.Data.StudentRecord.StudentStatus.一般;
                                SHStudent.Update(studRec);
                            }

                        }

                    // 轉出，學生狀態是一般，輸入異動代碼是轉出，問使用者是否轉成畢業或離校。
                    if (intUpdateCode >= 311 && intUpdateCode <= 316 && studRec.Status == K12.Data.StudentRecord.StudentStatus.一般)
                        if (FISCA.Presentation.Controls.MsgBox.Show("請問是否將學生狀態變更成畢業或離校？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (AllStudRec == null)
                                AllStudRec = SHStudent.SelectAll();

                            // 檢查將變更學生狀態
                            // 身分證
                            List<string> studIDNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.畢業或離校 && stud.IDNumber == studRec.IDNumber select stud.ID).ToList();
                            // 學號
                            List<string> studNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.畢業或離校 && stud.StudentNumber == studRec.StudentNumber select stud.ID).ToList();

                            if (studNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在畢業或離校狀態有相同的學號:" + studRec.StudentNumber + ",無法變更學生狀態");

                            if (studIDNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在畢業或離校狀態有相同的身分證號:" + studRec.IDNumber + ",無法變更學生狀態");

                            if (studIDNumber.Count == 0 && studNumber.Count == 0)
                            {
                                studRec.Status = K12.Data.StudentRecord.StudentStatus.畢業或離校;
                                SHStudent.Update(studRec);
                            }
                        }

                    // 休學，學生狀態是一般，輸入異動代碼是休學，問使用者是否轉成休學。
                    if (intUpdateCode >= 341 && intUpdateCode <= 349 && studRec.Status == K12.Data.StudentRecord.StudentStatus.一般)
                        if (FISCA.Presentation.Controls.MsgBox.Show("請問是否將學生狀態變更成休學？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (AllStudRec == null)
                                AllStudRec = SHStudent.SelectAll();

                            // 檢查將變更學生狀態
                            // 身分證
                            List<string> studIDNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.休學 && stud.IDNumber == studRec.IDNumber select stud.ID).ToList();
                            // 學號
                            List<string> studNumber = (from stud in AllStudRec where stud.Status == K12.Data.StudentRecord.StudentStatus.休學 && stud.StudentNumber == studRec.StudentNumber select stud.ID).ToList();

                            if (studNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在休學狀態有相同的學號:" + studRec.StudentNumber + ",無法變更學生狀態");

                            if (studIDNumber.Count > 0)
                                FISCA.Presentation.Controls.MsgBox.Show("在休學狀態有相同的身分證號:" + studRec.IDNumber + ",無法變更學生狀態");


                            if (studIDNumber.Count == 0 && studNumber.Count == 0)
                            {
                                studRec.Status = K12.Data.StudentRecord.StudentStatus.休學;
                                SHStudent.Update(studRec);
                            }

                        }
                    this.Close();
                    // 畫面同步
                    SmartSchool.StudentRelated.Student.Instance.SyncAllBackground();
                }

            }
        }

        // 載入系統預設學年度學期
        private void SetDefaultSchoolYearSemester()
        {

            int sy, sm;

            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out sy))
                intSchoolYear.Value = sy;
            else
                intSchoolYear.IsEmpty = true;

            if (int.TryParse(K12.Data.School.DefaultSemester, out sm))
                intSemester.Value = sm;
            else
                intSemester.IsEmpty = true;


            // 取得年級，如果學生狀態是延修生，年級顯示延修生，如果不是就依照原本年級顯示
            SHStudentRecord studRec = SHStudent.SelectByID(_StudentID);
            if (studRec.Class != null)
                if (studRec.Class.GradeYear.HasValue)
                    if (cbxGradeYear.Text == "")
                    {
                        if (studRec.Status == K12.Data.StudentRecord.StudentStatus.延修)
                            cbxGradeYear.Text = "延修生";
                        else
                        {
                            cbxGradeYear.Text = studRec.Class.GradeYear.Value.ToString();
                        }
                    }
        }

        private void UpdateRecordItemForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            IUpdateRecordInfo IU;

            if (UpdateRecordEditorPanle.Controls.Count == 0)
                e.Cancel = true;


            IU = UpdateRecordEditorPanle.Controls[0] as IUpdateRecordInfo;

            //IU.GetStudUpdateRecord();

            _StudUpdateRec = IU.GetStudUpdateRecord();
            // 當資料有改變            
            if (IU.GetLogData().GetDataHasChange() && _checkSave == false || string.IsNullOrEmpty(IU.GetStudUpdateRecord().UpdateCode))
                if (FISCA.Presentation.Controls.MsgBox.Show("這個動作將放棄目前編輯中的資料，是否確定離開？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;



            // 當異動代碼是否存在問題，不存在不儲存。
            if (IU.GetStudUpdateRecord() == null)
                return;
            else
            {
                List<string> xx = (from elm in _UpdateCode.Elements("異動") where elm.Element("代號").Value == IU.GetStudUpdateRecord().UpdateCode select elm.Element("代號").Value).ToList();
                if (xx.Count == 0)
                    return;
            }

        }

    }
}
