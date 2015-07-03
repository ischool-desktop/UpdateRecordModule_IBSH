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
    public partial class UpdateRecordInfo02 : UserControl,IUpdateRecordInfo
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
        /// 他校轉入
        /// </summary>
        public UpdateRecordInfo02(SHUpdateRecordRecord UpdateRec, PermRecLogProcess prlp, List<XElement> UpdateCodeElms)
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
            
            // 建異動索引            
            _UCodeDict = Utility.UITool.ConvertUpdateCodeDescDict(_UpdateCodeElms);

            // 載入資料           
            cbxUpdateCode = _faldn.SetFormData(_UpdateRec.UpdateCode, cbxUpdateCode, "異動代碼");
            txtDesc = _faldn.SetFormData(_UpdateRec.UpdateDescription, txtDesc, "原因及事項");
            dtUpdateDate = _faldn.SetFormData(_UpdateRec.UpdateDate, dtUpdateDate, "異動日期");
            txtMemo = _faldn.SetFormData(_UpdateRec.Comment, txtMemo, "備註");
            cbxDept = _faldn.SetFormData(_UpdateRec.Department, cbxDept, "科別");
            cbxClass = _faldn.SetFormData(_UpdateRec.ClassType, cbxClass, "班別");
            txtSpecial = _faldn.SetFormData(_UpdateRec.SpecialStatus, txtSpecial, "特殊身分代碼");
            txtName = _faldn.SetFormData(_UpdateRec.StudentName, txtName, "姓名");
            txtStudentNumber = _faldn.SetFormData(_UpdateRec.StudentNumber, txtStudentNumber, "學號");
            txtIDNumber = _faldn.SetFormData(_UpdateRec.IDNumber, txtIDNumber, "身分證字號");
            dtBirthday = _faldn.SetFormData(_UpdateRec.Birthdate, dtBirthday, "生日");
            txtIDNumber1 = _faldn.SetFormData(_UpdateRec.IDNumberComment, txtIDNumber1, "註1");
            cbxGender = _faldn.SetFormData(_UpdateRec.Gender, cbxGender, "性別");
            txtSSchool = _faldn.SetFormData(_UpdateRec.PreviousSchool,txtSSchool, "原就讀學校");
            txtSStudentNumber = _faldn.SetFormData(_UpdateRec.PreviousStudentNumber, txtSStudentNumber, "原就讀學號");
            txtSDept = _faldn.SetFormData(_UpdateRec.PreviousDepartment, txtSDept, "原就讀科別");
            txtSGradYear = _faldn.SetFormData(_UpdateRec.PreviousGradeYear, txtSGradYear, "原就讀年級");
            dtSLastUpdateDate = _faldn.SetFormData(_UpdateRec.PreviousSchoolLastADDate, dtSLastUpdateDate, "原就讀備查日期");
            txtSLastDocNo = _faldn.SetFormData(_UpdateRec.PreviousSchoolLastADNumber, txtSLastDocNo, "原就讀備查文號");
            dtADDate = _faldn.SetFormData(_UpdateRec.ADDate, dtADDate, "核准日期");
            txtADDocNo = _faldn.SetFormData(_UpdateRec.ADNumber, txtADDocNo, "核准文號");
            txtSSemester = _faldn.SetFormData(_UpdateRec.PreviousSemester, txtSSemester, "原就讀學期");
            txtComment2 = _faldn.SetFormData(_UpdateRec.Comment2, txtComment2, "轉入身分別代碼");
            
        }



        #region IUpdateRecordInfo 成員

        /// <summary>
        /// 異動資料
        /// </summary>
        /// <returns></returns>
        SHSchool.Data.SHUpdateRecordRecord IUpdateRecordInfo.GetStudUpdateRecord()
        {
            // 檢查畫面
            if (string.IsNullOrEmpty(cbxUpdateCode.Text))
                _epUpdateCode.SetError(cbxUpdateCode, "請輸入異動代碼.");

            if (_epUpdateCode.GetError(cbxUpdateCode).Length > 0)
                return null;
            
            // 載入資料           
            _UpdateRec.UpdateCode = _faldn.GetFormData(cbxUpdateCode, "異動代碼");
            _UpdateRec.UpdateDescription = _faldn.GetFormData(txtDesc, "原因及事項");
            _UpdateRec.UpdateDate = _faldn.GetFormData(dtUpdateDate, "異動日期");
            _UpdateRec.Comment = _faldn.GetFormData( txtMemo, "備註");
            _UpdateRec.Department = _faldn.GetFormData( cbxDept, "科別");
            _UpdateRec.ClassType = _faldn.GetFormData( cbxClass, "班別");
            _UpdateRec.SpecialStatus = _faldn.GetFormData(txtSpecial, "特殊身分代碼");
            _UpdateRec.StudentName = _faldn.GetFormData( txtName, "姓名");
            _UpdateRec.StudentNumber = _faldn.GetFormData( txtStudentNumber, "學號");
            _UpdateRec.IDNumber = _faldn.GetFormData(txtIDNumber, "身分證字號");
            _UpdateRec.Birthdate = _faldn.GetFormData( dtBirthday, "生日");
            _UpdateRec.IDNumberComment = _faldn.GetFormData( txtIDNumber1, "註1");
            _UpdateRec.Gender = _faldn.GetFormData(cbxGender, "性別");
            _UpdateRec.PreviousSchool = _faldn.GetFormData( txtSSchool, "原就讀學校");
            _UpdateRec.PreviousStudentNumber = _faldn.GetFormData( txtSStudentNumber, "原就讀學號");
            _UpdateRec.PreviousDepartment = _faldn.GetFormData(txtSDept, "原就讀科別");
            _UpdateRec.PreviousGradeYear = _faldn.GetFormData(txtSGradYear, "原就讀年級");
            _UpdateRec.PreviousSchoolLastADDate = _faldn.GetFormData( dtSLastUpdateDate, "原就讀備查日期");
            _UpdateRec.PreviousSchoolLastADNumber = _faldn.GetFormData( txtSLastDocNo, "原就讀備查文號");
            _UpdateRec.PreviousSemester = _faldn.GetFormData(txtSSemester, "原就讀學期");
            _UpdateRec.Comment2 = _faldn.GetFormData(txtComment2, "轉入身分別代碼");
            _UpdateRec.ADDate = _faldn.GetFormData(dtADDate, "核准日期");
            _UpdateRec.ADNumber = _faldn.GetFormData(txtADDocNo, "核准文號");

            _prlp = _faldn.GetLogData();
            return _UpdateRec;
        }

        /// <summary>
        /// 取得Log資料
        /// </summary>
        /// <returns></returns>
        PermRecLogProcess IUpdateRecordInfo.GetLogData()
        {
            return _prlp;
        }

 
        #endregion

        private void cbxUpdateCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxUpdateCode.Text.Length >= 3) 
            {
                string code = cbxUpdateCode.Text.Substring(0, 3);
                cbxUpdateCode.Items.Add(code);
                cbxUpdateCode.Text = code;
            }            
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

        private void cbxUpdateCode_Leave(object sender, EventArgs e)
        {
            // 檢查異動代碼            
            if(!_UCodeDict.ContainsKey(cbxUpdateCode.Text ))
                _epUpdateCode.SetError(cbxUpdateCode, "異動代碼不在清單內");
        }

        private void txtStudentNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void dtBirthday_Click(object sender, EventArgs e)
        {

        }

        private void cbxGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupPanel3_Click(object sender, EventArgs e)
        {

        }

        private void btlSchoolList_Click(object sender, EventArgs e)
        {
            Utility.SchoolListForm sf = new UpdateRecordModule_IBSH.Utility.SchoolListForm(Utility.SchoolListForm.LoadSchoolType.高中);
            if (sf.ShowDialog() == DialogResult.OK)
            {
                txtSSchool.Text = sf.GetSchoolCode() + sf.GetSchoolName();                
            }
        }
    }
}
