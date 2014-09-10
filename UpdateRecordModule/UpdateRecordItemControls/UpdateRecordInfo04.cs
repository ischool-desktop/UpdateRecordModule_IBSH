using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHSchool.Data;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.UpdateRecordItemControls
{
    public partial class UpdateRecordInfo04 : UserControl,IUpdateRecordInfo 
    {
        // 異動
        private SHUpdateRecordRecord _UpdateRec;
        // Log
        private PermRecLogProcess _prlp;
        private Utility.FormAndLogDataManager _faldn;
        List<XElement> _UpdateCodeElms;
        private List<string> _UpdateCoodeList;
        private List<SHDepartmentRecord> _DeptList;
        ErrorProvider _epUpdateCode;
        // 異動代碼索引，給畫面與檢查使用
        Dictionary<string, string> _UCodeDict;


        /// <summary>
        /// 畢業異動
        /// </summary>
        public UpdateRecordInfo04(SHUpdateRecordRecord UpdateRec, PermRecLogProcess prlp, List<XElement> UpdateCodeElms)
        {
            InitializeComponent();
            cbxGender.Items.Add("男");
            cbxGender.Items.Add("女");

            _UpdateRec = UpdateRec;
            _UpdateCodeElms = UpdateCodeElms;
            _prlp = prlp;

            _UpdateCoodeList = (from x in _UpdateCodeElms select x.Element("代號").Value + " " + x.Element("原因及事項").Value).ToList();
            _DeptList = SHDepartment.SelectAll();
            _faldn = new UpdateRecordModule_IBSH.Utility.FormAndLogDataManager(_prlp);
            _epUpdateCode = new ErrorProvider();
            _UCodeDict = Utility.UITool.ConvertUpdateCodeDescDict(_UpdateCodeElms);

            //當畢業證書字號沒有值取得SHLeaveInfoRecord畢業證書字號
            if (string.IsNullOrEmpty(_UpdateRec.GraduateCertificateNumber))
            {
                SHLeaveInfoRecord lvr = SHLeaveInfo.SelectByStudentID(_UpdateRec.StudentID);
                _UpdateRec.GraduateCertificateNumber = lvr.DiplomaNumber;
            }
            
            if (string.IsNullOrEmpty(_UpdateRec.ExpectGraduateSchoolYear))
            {
                SHLeaveInfoRecord shl = SHLeaveInfo.SelectByStudentID(_UpdateRec.StudentID);
                if(shl.SchoolYear.HasValue)
                    _UpdateRec.ExpectGraduateSchoolYear = shl.SchoolYear.Value.ToString();
            }

            // 載入資料           
            cbxUpdateCode = _faldn.SetFormData(_UpdateRec.UpdateCode, cbxUpdateCode, "異動代碼");
            txtDesc = _faldn.SetFormData(_UpdateRec.UpdateDescription, txtDesc, "原因及事項");
            dtUpdateDate = _faldn.SetFormData(_UpdateRec.UpdateDate, dtUpdateDate, "異動日期");
            txtMemo = _faldn.SetFormData(_UpdateRec.Comment, txtMemo, "備註");
            cbxDept = _faldn.SetFormData(_UpdateRec.Department, cbxDept, "科別");
            cbxClass = _faldn.SetFormData(_UpdateRec.ClassType, cbxClass, "班別");            
            txtName = _faldn.SetFormData(_UpdateRec.StudentName, txtName, "姓名");
            txtStudentNumber = _faldn.SetFormData(_UpdateRec.StudentNumber, txtStudentNumber, "學號");
            txtIDNumber = _faldn.SetFormData(_UpdateRec.IDNumber, txtIDNumber, "身分證字號");
            dtBirthday = _faldn.SetFormData(_UpdateRec.Birthdate, dtBirthday, "生日");
            txtIDNumber1 = _faldn.SetFormData(_UpdateRec.IDNumberComment, txtIDNumber1, "註1");
            cbxGender = _faldn.SetFormData(_UpdateRec.Gender, cbxGender, "性別");
            dtLastUpdateDate = _faldn.SetFormData(_UpdateRec.LastADDate, dtLastUpdateDate, "備查日期");
            txtLastDocNo = _faldn.SetFormData(_UpdateRec.LastADNumber, txtLastDocNo, "備查文號");
            txtLastUpdateCode = _faldn.SetFormData(_UpdateRec.LastUpdateCode, txtLastUpdateCode, "最後異動代號");
            txtGradeDocNo = _faldn.SetFormData(_UpdateRec.GraduateCertificateNumber, txtGradeDocNo, "畢業證書字號");
            dtADDate = _faldn.SetFormData(_UpdateRec.ADDate, dtADDate, "核准日期");
            txtADDocNo = _faldn.SetFormData(_UpdateRec.ADNumber, txtADDocNo, "核准文號");
            txtSpecial = _faldn.SetFormData(_UpdateRec.SpecialStatus, txtSpecial, "特殊身分代碼");
            txtSHSchoolYear = _faldn.SetFormData(_UpdateRec.ExpectGraduateSchoolYear, txtSHSchoolYear, "應畢業學年度");
        }

        #region IUpdateRecordInfo 成員

        SHSchool.Data.SHUpdateRecordRecord IUpdateRecordInfo.GetStudUpdateRecord()
        {
            // 檢查畫面
            if (string.IsNullOrEmpty(cbxUpdateCode.Text))
                _epUpdateCode.SetError(cbxUpdateCode, "請輸入異動代碼.");

            if (_epUpdateCode.GetError(cbxUpdateCode).Length > 0)
                return null;

            // 畫面資料           
            _UpdateRec.UpdateCode = _faldn.GetFormData(cbxUpdateCode, "異動代碼");
            _UpdateRec.UpdateDescription = _faldn.GetFormData(txtDesc, "原因及事項");
            _UpdateRec.UpdateDate = _faldn.GetFormData(dtUpdateDate, "異動日期");
            _UpdateRec.Comment = _faldn.GetFormData(txtMemo, "備註");
            _UpdateRec.Department = _faldn.GetFormData( cbxDept, "科別");
            _UpdateRec.ClassType = _faldn.GetFormData( cbxClass, "班別");
            _UpdateRec.StudentName = _faldn.GetFormData( txtName, "姓名");
            _UpdateRec.StudentNumber = _faldn.GetFormData( txtStudentNumber, "學號");
            _UpdateRec.IDNumber = _faldn.GetFormData( txtIDNumber, "身分證字號");
            _UpdateRec.Birthdate = _faldn.GetFormData(dtBirthday, "生日");
            _UpdateRec.IDNumberComment = _faldn.GetFormData(txtIDNumber1, "註1");
            _UpdateRec.Gender = _faldn.GetFormData(cbxGender, "性別");
            _UpdateRec.LastADDate = _faldn.GetFormData(dtLastUpdateDate, "備查日期");
            _UpdateRec.LastADNumber = _faldn.GetFormData( txtLastDocNo, "備查文號");
            _UpdateRec.LastUpdateCode = _faldn.GetFormData( txtLastUpdateCode, "最後異動代號");
            _UpdateRec.GraduateCertificateNumber = _faldn.GetFormData( txtGradeDocNo, "畢業證書字號");
            _UpdateRec.ADDate = _faldn.GetFormData(dtADDate, "核准日期");
            _UpdateRec.ADNumber = _faldn.GetFormData(txtADDocNo, "核准文號");
            _UpdateRec.SpecialStatus = _faldn.GetFormData(txtSpecial, "特殊身分代碼");
            _UpdateRec.ExpectGraduateSchoolYear = _faldn.GetFormData(txtSHSchoolYear, "應畢業學年度");

            _prlp = _faldn.GetLogData();
            return _UpdateRec;
        }


        PermRecLogProcess IUpdateRecordInfo.GetLogData()
        {
            return _prlp;
        }

        #endregion


        private void cbxUpdateCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string code = cbxUpdateCode.Text.Substring(0, 3);
            cbxUpdateCode.Items.Add(code);
            cbxUpdateCode.Text = code;
        }

        private void cbxUpdateCode_TextChanged(object sender, EventArgs e)
        {
            if (cbxUpdateCode.Text.Length >= 3)
            {
                cbxUpdateCode.Text = cbxUpdateCode.Text.Substring(0, 3);

                if (_UCodeDict.ContainsKey(cbxUpdateCode.Text))
                    txtDesc.Text = _UCodeDict[cbxUpdateCode.Text];

                _epUpdateCode.SetError(cbxUpdateCode, "");
            }            
        }

        private void cbxUpdateCode_DropDown(object sender, EventArgs e)
        {
            cbxUpdateCode.Items.Clear();
            cbxUpdateCode.Items.AddRange(_UpdateCoodeList.ToArray());

        }

        private void cbxClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cbxClass.Text.IndexOf("-");
            if (idx > 0)
            {
                string code = cbxClass.Text.Substring(0, idx);
                cbxClass.Items.Add(code);
                cbxClass.Text = code;
            }
        }

        private void cbxClass_DropDown(object sender, EventArgs e)
        {
            cbxClass.Items.Clear();
            cbxClass.Items.AddRange(DAL.DALTransfer.GetClassTypeList().ToArray());
        }

        private void cbxDept_DropDown(object sender, EventArgs e)
        {
            cbxDept.Items.Clear();
            cbxDept.Items.AddRange((from dept in _DeptList orderby dept.Code select dept.FullName).ToArray());
        }



        #region IUpdateRecordInfo 成員


        #endregion

        private void cbxUpdateCode_Leave(object sender, EventArgs e)
        {
            // 檢查異動代碼
            if(!_UCodeDict.ContainsKey(cbxUpdateCode.Text ))
                _epUpdateCode.SetError(cbxUpdateCode, "異動代碼不在清單內");
        }
    }
}
