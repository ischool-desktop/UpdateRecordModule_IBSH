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
    public partial class UpdateRecordInfo03 : UserControl,IUpdateRecordInfo
    {
        // 異動
        private SHUpdateRecordRecord _UpdateRec;
        // Log
        private PermRecLogProcess _prlp;
        private Utility.FormAndLogDataManager _faldn;
        private List<XElement> _UpdateCodeElms;
        private List<string> _UpdateCoodeList;
        private List<SHDepartmentRecord> _DeptList;
        ErrorProvider _epUpdateCode;
        // 異動代碼索引，給畫面與檢查使用
        Dictionary<string, string> _UCodeDict;

        /// <summary>
        /// 新生異動
        /// </summary>
        public UpdateRecordInfo03(SHUpdateRecordRecord UpdateRec, PermRecLogProcess prlp, List<XElement> UpdateCodeElms)
        {
            InitializeComponent();
            cbxGender.Items.Add("男");
            cbxGender.Items.Add("女");

            _UpdateRec = UpdateRec;
            _prlp = prlp;
            _UpdateCodeElms = UpdateCodeElms;
            _UpdateCoodeList = (from x in _UpdateCodeElms select x.Element("代號").Value + " " + x.Element("原因及事項").Value).ToList();
            _DeptList = SHDepartment.SelectAll();
            _epUpdateCode = new ErrorProvider();

            _UCodeDict = Utility.UITool.ConvertUpdateCodeDescDict(_UpdateCodeElms);

            _faldn = new UpdateRecordModule_IBSH.Utility.FormAndLogDataManager(_prlp);
            
            // 載入資料
            // 載入預設前級畢業資訊
            Dictionary<string, DAL.SchoolData> schoolDataDict = new Dictionary<string, DAL.SchoolData>();

            foreach (XElement elm in BL.Get.JHSchoolList().Elements("學校"))
            {
                DAL.SchoolData sd = new DAL.SchoolData();
                sd.SchoolCode = elm.Attribute("代碼").Value;
                sd.SchoolLocation = elm.Attribute("所在地").Value;
                sd.SchoolName = elm.Attribute("名稱").Value;
                sd.SchoolLocationCode = elm.Attribute("所在地代碼").Value;
                if (sd.SchoolCode.Length > 3)
                    sd.SchoolType = sd.SchoolCode.Substring(2, 1);

                string s1 = elm.Attribute("所在地").Value + elm.Attribute("名稱").Value;

                if (!schoolDataDict.ContainsKey(s1))
                    schoolDataDict.Add(s1, sd);

                if (!schoolDataDict.ContainsKey(sd.SchoolName))
                    schoolDataDict.Add(sd.SchoolName, sd);

            }

            SHBeforeEnrollmentRecord brfRec = SHBeforeEnrollment.SelectByStudentID(_UpdateRec.StudentID);

            if (string.IsNullOrEmpty(_UpdateRec.GraduateSchool))
                _UpdateRec.GraduateSchool = brfRec.School;
            if (string.IsNullOrEmpty(_UpdateRec.GraduateSchoolYear))
                _UpdateRec.GraduateSchoolYear = brfRec.GraduateSchoolYear;

            // 用學校名稱解析
            if (!string.IsNullOrEmpty(brfRec.School))
            {
                string key = brfRec.SchoolLocation.Replace("台", "臺") + brfRec.School.Trim();
                if (schoolDataDict.ContainsKey(key))
                {
                    if (string.IsNullOrEmpty(_UpdateRec.GraduateSchoolCode))
                    {
                        _UpdateRec.GraduateSchoolCode = schoolDataDict[key].SchoolCode;
                        _UpdateRec.GraduateSchoolLocationCode = schoolDataDict[key].SchoolLocationCode;
                    }
                }
            }
            

            cbxUpdateCode = _faldn.SetFormData(_UpdateRec.UpdateCode, cbxUpdateCode, "資格代碼");
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
            txtISchool = _faldn.SetFormData(_UpdateRec.GraduateSchool, txtISchool, "入學資格(畢業國中)");
            txtISpaceCode = _faldn.SetFormData(_UpdateRec.GraduateSchoolLocationCode,txtISpaceCode,"所在地代碼");
            txtISchoolCode = _faldn.SetFormData(_UpdateRec.GraduateSchoolCode, txtISchoolCode, "畢業國中學校代號");
            txtIJHGradeYear = _faldn.SetFormData(_UpdateRec.GraduateSchoolYear, txtIJHGradeYear, "國中畢業年度");
            dtADDate = _faldn.SetFormData(_UpdateRec.ADDate, dtADDate, "核准日期");
            txtADDocNo = _faldn.SetFormData(_UpdateRec.ADNumber, txtADDocNo, "核准文號");
            txtGradeDoc = _faldn.SetFormData(_UpdateRec.GraduateDocument, txtGradeDoc, "入學資格證明文件");          
                        
        }


        #region IUpdateRecordInfo 成員

        SHUpdateRecordRecord IUpdateRecordInfo.GetStudUpdateRecord()
        {
            // 檢查畫面
            if (string.IsNullOrEmpty(cbxUpdateCode.Text))
                _epUpdateCode.SetError(cbxUpdateCode, "請輸入異動代碼.");

            if (_epUpdateCode.GetError(cbxUpdateCode).Length > 0)
                return null;

            // 畫面資料           
            _UpdateRec.UpdateCode = _faldn.GetFormData(cbxUpdateCode, "資格代碼");
            _UpdateRec.UpdateDescription = _faldn.GetFormData( txtDesc, "原因及事項");
            _UpdateRec.UpdateDate = _faldn.GetFormData( dtUpdateDate, "異動日期");
            _UpdateRec.Comment = _faldn.GetFormData(txtMemo, "備註");
            _UpdateRec.Department = _faldn.GetFormData(cbxDept, "科別");
            _UpdateRec.ClassType = _faldn.GetFormData(cbxClass, "班別");
            _UpdateRec.SpecialStatus = _faldn.GetFormData(txtSpecial, "特殊身分代碼");
            _UpdateRec.StudentName = _faldn.GetFormData(txtName, "姓名");
            _UpdateRec.StudentNumber = _faldn.GetFormData(txtStudentNumber, "學號");
            _UpdateRec.IDNumber = _faldn.GetFormData( txtIDNumber, "身分證字號");
            _UpdateRec.Birthdate = _faldn.GetFormData(dtBirthday, "生日");
            _UpdateRec.IDNumberComment = _faldn.GetFormData(txtIDNumber1, "註1");
            _UpdateRec.Gender = _faldn.GetFormData(cbxGender, "性別");
            _UpdateRec.GraduateSchool = _faldn.GetFormData(txtISchool, "入學資格(畢業國中)");
            _UpdateRec.GraduateSchoolLocationCode = _faldn.GetFormData(txtISpaceCode, "所在地代碼");
            _UpdateRec.GraduateSchoolYear= _faldn.GetFormData( txtIJHGradeYear, "國中畢業年度");            
            _UpdateRec.ADDate = _faldn.GetFormData(dtADDate, "核准日期");
            _UpdateRec.ADNumber =_faldn.GetFormData(txtADDocNo, "核准文號");
            _UpdateRec.GraduateDocument = _faldn.GetFormData(txtGradeDoc, "入學資格證明文件");
            _UpdateRec.GraduateSchoolCode = _faldn.GetFormData(txtISchoolCode, "畢業國中學校代號");

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
            if (cbxUpdateCode.Text.Length >= 3)
            {
                string code = cbxUpdateCode.Text.Substring(0, 3);
                cbxUpdateCode.Items.Add(code);
                cbxUpdateCode.Text = code;
                SettxtGradeComment();
            }            
        }

        private void SettxtGradeComment()
        {
           
        }

        private void cbxUpdateCode_TextChanged(object sender, EventArgs e)
        {
            if (cbxUpdateCode.Text.Length >= 3)
            {
                cbxUpdateCode.Text = cbxUpdateCode.Text.Substring(0, 3);

                if (_UCodeDict.ContainsKey(cbxUpdateCode.Text))
                    txtDesc.Text = _UCodeDict[cbxUpdateCode.Text];

                _epUpdateCode.SetError(cbxUpdateCode, "");
                SettxtGradeComment();
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

        private void btlSchoolList_Click(object sender, EventArgs e)
        {
            Utility.SchoolListForm sf = new UpdateRecordModule_IBSH.Utility.SchoolListForm(Utility.SchoolListForm.LoadSchoolType.國中);
            if (sf.ShowDialog() == DialogResult.OK)
            {
                txtISchool.Text = sf.GetSchoolName();
                txtISpaceCode.Text = sf.GetSchoolLocationCode();
                txtISchoolCode.Text = sf.GetSchoolCode();                
            }
        }

        //private void cbxInformation_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string code = cbxInformation.Text.Substring(0, 3);
        //    cbxInformation.Items.Add(code);
        //    cbxInformation.Text = code;
        //}

        //private void cbxInformation_DropDown(object sender, EventArgs e)
        //{
        //    cbxInformation.Items.Clear();
        //    cbxInformation.Items.AddRange(DAL.DALTransfer.GetNewStudCode().ToArray());
        //}

        private void cbxUpdateCode_Leave(object sender, EventArgs e)
        {
            // 檢查異動代碼
            if(!_UCodeDict.ContainsKey(cbxUpdateCode.Text ))                
                _epUpdateCode.SetError(cbxUpdateCode, "異動代碼不在清單內");
        }
    }
}
