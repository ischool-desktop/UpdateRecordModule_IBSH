using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using FISCA.DSAUtil;
using SHSchool.Data;
using SmartSchool.ApplicationLog;
using SmartSchool.Common;
using SmartSchool.Feature;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.Wizards
{
    public enum UpdateRecordType { 學籍異動, 新生異動, 轉入異動, 畢業異動 }
    public enum UpdateRecordAction { Insert, Update, None }
    public partial class UpdateRecordInfo : UserControl
    {
        private List<string> _DateFields = new List<string>(new string[] { "ADDate", "UpdateDate", "Birthdate", "LastADDate", "PreviousSchoolLastADDate" });

        private List<string> _NonNullFields = new List<string>(new string[] { "UpdateDate", "UpdateCode" });

        private Dictionary<string, List<Control>> _ControlDictionary = new Dictionary<string, List<Control>>();

        private List<Control> _DepartmentControls = new List<Control>();

        private List<Control> _UpdateCodeControls = new List<Control>();

        private ManualResetEvent _UpdateCodeLoadedEvent = new ManualResetEvent(true);

        private ManualResetEvent _UpdateCodeEditingEvent = new ManualResetEvent(true);

        private ManualResetEvent _SchoolListLoadedEvent = new ManualResetEvent(true);

        private ManualResetEvent _DepartmentCodeLoadedEvent = new ManualResetEvent(true);

        private BackgroundWorker _BWDepartmentCode, _BWUpdateCode,_BWSchoolList;

        private string _StudentID;

        private string _UpdateRecordID;

        private UpdateRecordType _Style;

        private UpdateRecordAction _Action;

        private Dictionary<Control, ErrorProvider> _errorProviderDictionary = new Dictionary<Control, ErrorProvider>();

        /// <summary>
        /// 設定某個元件的錯誤訊息
        /// </summary>
        /// <param name="control">控制項物件</param>
        /// <param name="p">錯誤訊息</param>
        /// <param name="isError">是否為錯誤或警告</param>
        private void SetErrorProvider(Control control, string p, bool isError)
        {
            if (_ErrorControls.Contains(control))
            {
                _ErrorControls.Remove(control);
            }
            if (!_errorProviderDictionary.ContainsKey(control))
            {
                ErrorProvider ep = new ErrorProvider();
                ep.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                ep.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                if (isError)
                {
                    ep.Icon = Properties.Resources.error;
                }
                else
                {
                    ep.Icon = Properties.Resources.warning;
                }
                ep.SetError(control, p);
                _errorProviderDictionary.Add(control, ep);
            }
            if (isError)
            {
                _ErrorControls.Add(control);
            }
        }

        private void ResetErrorProvider(Control control)
        {
            if (_errorProviderDictionary.ContainsKey(control))
            {
                _errorProviderDictionary[control].Clear();
                _errorProviderDictionary.Remove(control);
            }
            if (_ErrorControls.Contains(control))
            {
                _ErrorControls.Remove(control);
            }
        }

        //科別列表，定義在SHSchool.Data.SHDepartment
        private List<SHDepartmentRecord> _DepartmentList = new List<SHDepartmentRecord>();

        //學校列表，定義在K12.Data.School
        private List<K12.Data.SchoolRecord> _SchoolList = new List<K12.Data.SchoolRecord>();

        private string[] _AllowedUpdateCode;

        private Dictionary<string, Dictionary<string, string>> _UpdateCodeSynopsis = new Dictionary<string, Dictionary<string, string>>();

        private void Control_TextChanged(object sender, EventArgs e)
        {
            if (_StopEvent) return;
            Control control = (Control)sender;
            //把所有相同意函欄位設成相同值
            if (control.Tag != null)
            {
                SetValue(control.Tag.ToString(), control.Text, !control.Focused);
                if (DataChanged != null)
                {
                    DataChanged.Invoke(this, new EventArgs());
                }
            }
        }

        private bool CheckIsDate(string text)
        {
            if (text.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Length != 3)
                return false;
            DateTime d = DateTime.Now;
            return DateTime.TryParse(text, out  d);
        }

        private bool _StopEvent = false;

        private Dictionary<string, string> GetUpdateCodeSynopsis(string style)
        {
            _UpdateCodeEditingEvent.WaitOne();
            Dictionary<string, string> ucsdic = new Dictionary<string, string>();
            if (_UpdateCodeSynopsis.ContainsKey(style))
            {
                if (_AllowedUpdateCode == null || _AllowedUpdateCode.Length == 0)
                {
                    return _UpdateCodeSynopsis[style];
                }
                else
                {
                    Dictionary<string, string> items = _UpdateCodeSynopsis[style];
                    string value = "";
                    foreach (string key in _AllowedUpdateCode)
                    {
                        if (items.TryGetValue(key, out value))
                        {
                            ucsdic.Add(key, value);
                        }
                    }
                    return ucsdic;
                }
            }
            else
                return ucsdic;
        }

        private void ValueValidate(Control control, bool showError)
        {
            #region 如果是異動代號則檢查輸入代號是否在清單中
            if (control.Tag != null && control.Tag.ToString() == "UpdateCode")
            {
                if (GetUpdateCodeSynopsis(_Style.ToString()).Count != 0 && !GetUpdateCodeSynopsis(_Style.ToString()).ContainsKey(control.Text))
                {
                    if (showError)
                        SetErrorProvider(control, "輸入的代號不在指定的代號清單中。", true);
                    return;
                }
                else
                {
                    ResetErrorProvider(control);
                }
            }
            #endregion
            #region 如果是日期欄位檢查輸入值
            if (_DateFields.Contains(control.Tag.ToString()))
            {
                if (control.Text == "" && _NonNullFields.Contains(control.Tag.ToString()))
                {
                    if (showError)
                        SetErrorProvider(control, "此欄為必填欄位，請輸入西元年/月/日。", true); return;
                }
                else
                {
                    if (control.Text != "")
                    {
                        //檢查欄位值
                        if (!CheckIsDate(control.Text))
                        {
                            if (_NonNullFields.Contains(control.Tag.ToString()))
                            {
                                if (showError)
                                    SetErrorProvider(control, "此欄為必填欄位，\n請依照\"西元年/月/日\"格式輸入。", true);
                            }
                            else
                            {
                                if (showError)
                                    SetErrorProvider(control, "輸入格式錯誤，請輸入西元年/月/日。\n此筆錯誤資料將不會被儲存", false);
                            }
                            return;
                        }
                        else
                        {
                            ResetErrorProvider(control);
                        }
                    }
                }
            }
            #endregion
            #region 如果是必填欄位檢查非空值
            if (_NonNullFields.Contains(control.Tag.ToString()) && control.Text == "")
            {
                if (showError)
                    SetErrorProvider(control, "此欄位必須填寫，不允許空值", true);
                return;
            }
            else
            {
                ResetErrorProvider(control);
            }
            #endregion
            #region 如果是年級則檢查輸入資料
            if (control.Tag != null && control.Tag.ToString() == "GradeYear")
            {
                int i = 0;
                if (control.Text != "延修生" && (!int.TryParse(control.Text, out i) || i <= 0))
                {
                    if (showError)
                        SetErrorProvider(control, "年級欄必需依以下格式填寫：\n\t1.若為一般學生請填入學生年級。\n\t2.若為延修生請填入\"延修生\"", true);
                    return;
                }
                else
                {
                    ResetErrorProvider(control);
                }
            }
            #endregion
            #region 檢查畢業國中學年度是否為數字
            if (control.Tag != null && control.Tag.ToString() == "GraduateSchoolYear")
            {
                int SchoolYear;
                if (!string.IsNullOrEmpty(control.Text))
                    if (!int.TryParse(control.Text, out SchoolYear))
                        if (showError)
                        {
                            SetErrorProvider(control, "畢業國中學年度需為數字。", false);
                            return;
                        }
                        else
                            ResetErrorProvider(control);
            }
            #endregion
            #region 檢查班別
            if (control.Tag != null && control.Tag.ToString().EndsWith("ClassType"))
            {
                List<string> ClassTypeIDs = new List<string>() { "1", "3", "4", "7", "01", "02", "03", "04", "05", "06" };

                if (!ClassTypeIDs.Contains(control.Text))
                    if (showError)
                    {
                        SetErrorProvider(control, "輸入的代號不在指定的代號清單中。", false);
                        return;
                    }
                    else
                        ResetErrorProvider(control);

            }
            #endregion
            #region 檢查科別代碼
            if (control.Tag != null && control.Tag.ToString().Equals("OldDepartmentCode"))
            {
                _DepartmentCodeLoadedEvent.WaitOne();

                List<string> DepartmentCodes = new List<string>();

                foreach (SHDepartmentRecord var in _DepartmentList)
                    DepartmentCodes.Add(var.Code);

                if (!DepartmentCodes.Contains(control.Text))
                    if (showError)
                    {
                        SetErrorProvider(control, "輸入的代號不在指定的代號清單中。", false);
                        return;
                    }
                    else
                        ResetErrorProvider(control);
            }
            #endregion
            #region 檢查畢業國中是否在清單內
            if (control.Tag != null && control.Tag.ToString().Equals("GraduateSchool"))
            {
                // 先暫不檢查
                //bool IsContains = false;

                //_SchoolListLoadedEvent.WaitOne();

                //foreach (K12.Data.SchoolRecord SchoolRec in _SchoolList)
                //{
                //    if ((SchoolRec.County+SchoolRec.Name).Equals(GetValue("GraduateSchool")))
                //     IsContains = true;
                //}

                //if (!IsContains)
                //{
                //    if (showError)
                //    {
                //        if (string.IsNullOrEmpty(GetValue("GraduateComment")))
                //            SetValue("GraduateComment", "1");
                //        SetErrorProvider(control, "輸入的名稱不在指定的清單中。", false);
                //        return;
                //    }
                //    else
                //        ResetErrorProvider(control);
                //}
                //else
                //{
                //    if (string.IsNullOrEmpty(GetValue("GraduateComment")))
                //        SetValue("GraduateComment", "");
                //}
            }
            #endregion
            #region 檢查身分證字號
            if (control.Tag != null && control.Tag.ToString().Equals("IDNumber"))
            {
                string CheckResult = IDNumberCheck.Execute(GetValue("IDNumber"));

                string Value = GetValue("IDNumberComment");

                switch (CheckResult)
                {
                    case "0":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "");
                        ResetErrorProvider(control);
                        return;
                    case "1":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "1");
                            SetErrorProvider(control, "字數不等於10。", false);
                        return;
                    case "2":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "1");
                        SetErrorProvider(control, "第二碼非1,2。", false);
                        return;
                    case "3":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "1");
                        SetErrorProvider(control, "首碼有誤。", false);
                        return;
                    case "4":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "1");
                        SetErrorProvider(control, "檢查碼不對。", false);
                        return;
                    case "5":
                        if (string.IsNullOrEmpty(Value))
                            SetValue("IDNumberComment", "1");
                        SetErrorProvider(control, "格式不對。", false);
                        return;
                }
            }
            #endregion
        }

        private List<Control> _ErrorControls = new List<Control>();

        //NewData
        private ChangVisibleDataField _newDataField = null;
        public string NewData { get { return GetValue("NewData"); } set { SetValue("NewData", value); } }
        //public bool NewDataVisible { get { return _newDataField.Visible; } set { _newDataField.Visible = value; } }

        //記錄Log所需要用到的變數
        private Dictionary<string, string> beforeData = new Dictionary<string, string>();
        private Dictionary<string, string> afterData = new Dictionary<string, string>();

        //英漢字典，用來查詢每個欄位的中文名稱
        private Dictionary<string, string> _EnChDict = new Dictionary<string, string>();

        //初始化英漢字典
        private void InitialDict()
        {
            _EnChDict.Add("Department", "科別");
            _EnChDict.Add("ADDate", "核准日期");
            _EnChDict.Add("ADNumber", "核准文號");
            _EnChDict.Add("UpdateDate", "異動日期");
            _EnChDict.Add("UpdateCode", "異動代號");
            _EnChDict.Add("UpdateDescription", "原因及事項");
            _EnChDict.Add("Name", "姓名");
            _EnChDict.Add("StudentNumber", "學號");
            _EnChDict.Add("Gender", "性別");
            _EnChDict.Add("IDNumber", "身分證號");
            _EnChDict.Add("Birthdate", "生日");
            _EnChDict.Add("GradeYear", "年級");
            _EnChDict.Add("LastADDate", "備查日期");
            _EnChDict.Add("LastADNumber", "備查文號");
            _EnChDict.Add("Comment", "備註");
            _EnChDict.Add("LastUpdateCode", "最後異動代號");
            _EnChDict.Add("NewStudentNumber", "新學號");
            _EnChDict.Add("PreviousSchool", "轉入前學生資料-學校");
            _EnChDict.Add("PreviousStudentNumber", "轉入前學生資料-學號");
            _EnChDict.Add("PreviousDepartment", "轉入前學生資料-科別");
            _EnChDict.Add("PreviousSchoolLastADDate", "轉入前學生資料-備查日期");
            _EnChDict.Add("PreviousSchoolLastADNumber", "轉入前學生資料-備查文號");
            _EnChDict.Add("PreviousGradeYear", "轉入前學生資料-年級");
            _EnChDict.Add("GraduateSchoolLocationCode", "入學資格-畢業國中所在地代碼");
            _EnChDict.Add("GraduateSchool", "入學資格-畢業國中");
            _EnChDict.Add("GraduateCertificateNumber", "畢(結)業證書字號");
            _EnChDict.Add("NewData", "更正後資料");

            _EnChDict.Add("IDNumberComment", "身分證號註");
            _EnChDict.Add("ClassType", "班別");
            _EnChDict.Add("SpecialStatus","特殊身份代碼");
            _EnChDict.Add("OldClassType","舊班別");
            _EnChDict.Add("OldDepartmentCode","舊科別代碼");
            _EnChDict.Add("GraduateSchoolYear","國中畢業年度");
            _EnChDict.Add("GraduateComment","入學資格備註");
        }

        //用Control的Tag判定欄位
        public void SetValue(string tagName, string value) { SetValue(tagName, value, true); }
        public void SetValue(string tagName, string value, bool showError)
        {
            _StopEvent = true;
            //如果是異動代號欄位則查詢對照表更新UpdateDescription 
            if (tagName == "UpdateCode")
            {
                if (GetUpdateCodeSynopsis(_Style.ToString()).ContainsKey(value) && GetValue("UpdateDescription") != GetUpdateCodeSynopsis(_Style.ToString())[value])
                {
                    SetValue("UpdateDescription", GetUpdateCodeSynopsis(_Style.ToString())[value]);
                    _newDataField.ChangeNewDataText(value);
                }
            }
            //更新相同tagName控制項的值
            if (_ControlDictionary.ContainsKey(tagName))
            {
                value = value.Trim();

                foreach (Control var in _ControlDictionary[tagName])
                {
                    if (var is DevComponents.Editors.DateTimeAdv.DateTimeInput)
                    {
                        if (!string.IsNullOrEmpty(value.Trim()))
                            var.Text = value;
                    }else 
                        var.Text = value;
                    ValueValidate(var, showError);
                }
            }
            _StopEvent = false;
        }

        //用Control的Tag判定欄位
        public string GetValue(string tagName)
        {
            if (_ControlDictionary.ContainsKey(tagName))
            {
                string text = _ControlDictionary[tagName][0].Text;
                //如果是日期欄位且不符合日期格式則傳回空字串
                if (_DateFields.Contains(tagName) && !CheckIsDate(text))
                    return "";
                else
                    return text;
            }
            else
                return "";
        }

        public UpdateRecordInfo()
        {
            _BWUpdateCode = new BackgroundWorker();
            _BWUpdateCode.DoWork += new DoWorkEventHandler(_BWUpdateCode_DoWork);
            _BWUpdateCode.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BWUpdateCode_RunWorkerCompleted);

            _UpdateCodeLoadedEvent.Reset();
            _BWUpdateCode.RunWorkerAsync();

            _BWDepartmentCode = new BackgroundWorker();
            _BWDepartmentCode.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    _DepartmentList.AddRange(SHDepartment.SelectAll());
                }
                catch { }
                finally
                {
                    _DepartmentCodeLoadedEvent.Set();
                }
            };
            _DepartmentCodeLoadedEvent.Reset();
            _BWDepartmentCode.RunWorkerAsync();

            _BWSchoolList = new BackgroundWorker();
            _BWSchoolList.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    _SchoolList.AddRange(K12.Data.School.SelectJuniorSchools());
                    _SchoolList.AddRange(K12.Data.School.SelectElementarySchools());
                }
                catch {}
                finally
                {
                    _SchoolListLoadedEvent.Set(); 
                }
            };

            _SchoolListLoadedEvent.Reset();
            _BWSchoolList.RunWorkerAsync();

            InitializeComponent();
            SetupControl(this);

            InitialDict();
            _newDataField = new ChangVisibleDataField(lblNewData, txtNewData,lblNewStudentNumber,txtNewStudentNumber);
        }

        private void _BWUpdateCode_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (Control var in _UpdateCodeControls)
            {
                DropDownUpdateCode(var, ((ComboBoxEx)var).DroppedDown);
            }
        }

        private void _BWUpdateCode_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
        
                //DSResponse dsrsp = SmartSchool.Feature.Basic.Config.GetUpdateCodeSynopsis();
                //_UpdateCodeEditingEvent.Reset();
                XElement _UpdateCode = DAL.DALTransfer.GetUpdateCodeList();

                foreach(XElement elm in _UpdateCode.Elements("異動"))
                {
                    string UpdateCode, UpdateDescription, UpdateType;
                    UpdateCode = elm.Element("代號").Value;
                    UpdateDescription = elm.Element("原因及事項").Value;
                    UpdateType = elm.Element("分類").Value;

                    if (!_UpdateCodeSynopsis.ContainsKey(UpdateType))
                    {
                        _UpdateCodeSynopsis.Add(UpdateType, new Dictionary<string, string>());
                    }
                    _UpdateCodeSynopsis[UpdateType].Add(UpdateCode, UpdateDescription);
                }
                _UpdateCodeEditingEvent.Set();
            }
            catch { }
            finally
            {
                _UpdateCodeLoadedEvent.Set();
            }
        }

        public event EventHandler DataChanged;

        public string StudentID { get { return _StudentID; } set { _StudentID = value; } }

        public UpdateRecordType Style
        {
            get { return _Style; }
            set
            {
                if (value == _Style) return;
                this.Visible = false;
                this.畢業名冊.Visible = false;
                this.新生名冊.Visible = false;
                this.學籍異動.Visible = false;
                this.轉入名冊.Visible = false;
                _Style = value;
                switch (_Style)
                {
                    default:
                    case UpdateRecordType.學籍異動:
                        {
                            this.學籍異動.Visible = true;
                            break;
                        }
                    case UpdateRecordType.轉入異動:
                        {
                            this.轉入名冊.Visible = true;
                            break;
                        }
                    case UpdateRecordType.新生異動:
                        {
                            this.新生名冊.Visible = true;
                            break;
                        }
                    case UpdateRecordType.畢業異動:
                        {
                            this.畢業名冊.Visible = true;
                            break;
                        }
                }
                foreach (Control var in _UpdateCodeControls)
                {
                    DropDownUpdateCode(var, ((ComboBoxEx)var).DroppedDown);
                }
                SetValue("UpdateCode", "", true);
                this.Visible = true;
            }
        }

        public bool NewStudentNumberVisible
        {
            get
            {
                return txtNewStudentNumber.Visible;
            }
            set
            {
                txtNewStudentNumber.Visible = lblNewStudentNumber.Visible = value;
            }
        }

        #region 取得或設定各項填入值

        /// <summary>
        /// 科別 
        /// </summary>
        public string Department { get { return GetValue("Department"); } set { SetValue("Department", value); } }

        /// <summary>
        /// 核准日期(回填) 
        /// </summary>
        public string ADDate { get { return GetValue("ADDate"); } set { SetValue("ADDate", value); } }
        
        /// <summary>
        /// 核准文號(回填) 
        /// </summary>
        public string ADNumber { get { return GetValue("ADNumber"); } set { SetValue("ADNumber", value); } }
        
        /// <summary>
        /// 異動日期 
        /// </summary>
        public string UpdateDate { get { return GetValue("UpdateDate"); } set { SetValue("UpdateDate", value); } }
        
        /// <summary>
        /// 異動代碼 
        /// </summary>
        public string UpdateCode { get { return GetValue("UpdateCode"); } set { SetValue("UpdateCode", value); } }
        
        /// <summary>
        /// 原因及事項 
        /// </summary>
        public string UpdateDescription { get { return GetValue("UpdateDescription"); } set { SetValue("UpdateDescription", value); } }
        
        /// <summary>
        /// 姓名 
        /// </summary>
        public new string Name { get { return GetValue("Name"); } set { SetValue("Name", value); } }
        
        /// <summary>
        /// 學號 
        /// </summary>
        public string StudentNumber { get { return GetValue("StudentNumber"); } set { SetValue("StudentNumber", value); } }
        
        /// <summary>
        /// 性別 
        /// </summary>
        public string Gender { get { return GetValue("Gender"); } set { SetValue("Gender", value); } }
        
        /// <summary>
        /// 身分證號 
        /// </summary>
        public string IDNumber { get { return GetValue("IDNumber"); } set { SetValue("IDNumber", value); } }
        
        /// <summary>
        /// 生日 
        /// </summary>
        public string Birthdate { get { return GetValue("Birthdate"); } set { SetValue("Birthdate", value); } }
        
        /// <summary>
        /// 年級 
        /// </summary>
        public string GradeYear { get { return GetValue("GradeYear"); } set { SetValue("GradeYear", value); } }
        
        /// <summary>
        /// 最後核准日期 
        /// </summary>
        public string LastADDate { get { return GetValue("LastADDate"); } set { SetValue("LastADDate", value); } }

        /// <summary>
        /// 最後核准文號 
        /// </summary>
        public string LastADNumber { get { return GetValue("LastADNumber"); } set { SetValue("LastADNumber", value); } }
        
        /// <summary>
        /// 備註 
        /// </summary>
        public string Comment { get { return GetValue("Comment"); } set { SetValue("Comment", value); } }

        /// <summary>
        /// 最後核准異動代碼 
        /// </summary>
        public string LastUpdateCode { get { return GetValue("LastUpdateCode"); } set { SetValue("LastUpdateCode", value); } }
        
        /// <summary>
        /// 新學號 
        /// </summary>
        public string NewStudentNumber { get { return GetValue("NewStudentNumber"); } set { SetValue("NewStudentNumber", value); } }

        /// <summary>
        /// 轉入前學生資料-學校 
        /// </summary>
        public string PreviousSchool { get { return GetValue("PreviousSchool"); } set { SetValue("PreviousSchool", value); } }

        /// <summary>
        /// 轉入前學生資料-學號 
        /// </summary>
        public string PreviousStudentNumber { get { return GetValue("PreviousStudentNumber"); } set { SetValue("PreviousStudentNumber", value); } }
        
        /// <summary>
        /// 轉入前學生資料-科別 
        /// </summary>
        public string PreviousDepartment { get { return GetValue("PreviousDepartment"); } set { SetValue("PreviousDepartment", value); } }

        /// <summary>
        /// 轉入前學生資料-(最後核准日期) 
        /// </summary>
        public string PreviousSchoolLastADDate { get { return GetValue("PreviousSchoolLastADDate"); } set { SetValue("PreviousSchoolLastADDate", value); } }

        /// <summary>
        /// 轉入前學生資料-(最後核准文號) 
        /// </summary>
        public string PreviousSchoolLastADNumber { get { return GetValue("PreviousSchoolLastADNumber"); } set { SetValue("PreviousSchoolLastADNumber", value); } }

        /// <summary>
        /// 轉入前學生資料-年級 
        /// </summary>
        public string PreviousGradeYear { get { return GetValue("PreviousGradeYear"); } set { SetValue("PreviousGradeYear", value); } }
        
        /// <summary>
        /// 入學資格-畢業國中所在地代碼
        /// </summary>
        public string GraduateSchoolLocationCode { get { return GetValue("GraduateSchoolLocationCode"); } set { SetValue("GraduateSchoolLocationCode", value); } }
        
        /// <summary>
        /// 入學資格-畢業國中 
        /// </summary>
        public string GraduateSchool { get { return GetValue("GraduateSchool"); } set { SetValue("GraduateSchool", value); } }
        
        /// <summary>
        ///  畢(結)業證書字號
        /// </summary>
        public string GraduateCertificateNumber { get { return GetValue("GraduateCertificateNumber"); } set { SetValue("GraduateCertificateNumber", value); } }

        #region 2009年新制增加，並且符合夜校規格

        /// <summary>
        /// 身份證號註（註1）
        /// </summary>
        public string IDNumberComment { get{ return GetValue("IDNumberComment");} set{ SetValue("IDNumberComment",value);}}

        /// <summary>
        /// 班別
        /// </summary>
        public string ClassType { get{ return GetValue("ClassType");}  set{ SetValue("ClassType",value);} }

        /// <summary>
        /// 特殊身份代碼
        /// </summary>
        public string SpecialStatus { get{ return GetValue("SpecialStatus");}  set{ SetValue("SpecialStatus",value);} }

        /// <summary>
        /// 舊班別
        /// </summary>
        public string OldClassType { get{ return GetValue("OldClassType");}  set{ SetValue("OldClassType",value);} }
        
        /// <summary>
        /// 舊科別代碼
        /// </summary>
        public string OldDepartmentCode { get{ return GetValue("OldDepartmentCode");}  set{ SetValue("OldDepartmentCode",value);} }

        /// <summary>
        /// 國中畢業年度
        /// </summary>
        public string GraduateSchoolYear { get{ return GetValue("GraduateSchoolYear");}  set{ SetValue("GraduateSchoolYear",value);} }
        
        /// <summary>
        /// 入學資格備註（註2） 
        /// </summary>
        public string GraduateComment { get{ return GetValue("GraduateComment");}  set{ SetValue("GraduateComment",value);} }
        #endregion

        #endregion
        public XmlElement GetElement()
        {
            XmlDocument doc = new XmlDocument();
            //doc.LoadXml("<root></root>");
            //XmlElement rootElement = doc.DocumentElement;
            //XmlElement infoElement;

            XmlElement rootElement = doc.CreateElement("Root");
            doc.AppendChild(rootElement);
            XmlElement infoElement;
            //科別
            infoElement = doc.CreateElement("Department");
            infoElement.InnerText = GetValue("Department");
            rootElement.AppendChild(infoElement);
            //核准日期(回填)
            infoElement = doc.CreateElement("ADDate");
            infoElement.InnerText = GetValue("ADDate");
            rootElement.AppendChild(infoElement);
            //核准文號(回填)
            infoElement = doc.CreateElement("ADNumber");
            infoElement.InnerText = GetValue("ADNumber");
            rootElement.AppendChild(infoElement);
            //異動日期
            infoElement = doc.CreateElement("UpdateDate");
            infoElement.InnerText = GetValue("UpdateDate");
            rootElement.AppendChild(infoElement);
            //異動代碼
            infoElement = doc.CreateElement("UpdateCode");
            infoElement.InnerText = GetValue("UpdateCode");
            rootElement.AppendChild(infoElement);
            //原因及事項
            infoElement = doc.CreateElement("UpdateDescription");
            infoElement.InnerText = GetValue("UpdateDescription");
            rootElement.AppendChild(infoElement);
            //姓名
            infoElement = doc.CreateElement("Name");
            infoElement.InnerText = GetValue("Name");
            rootElement.AppendChild(infoElement);
            //學號
            infoElement = doc.CreateElement("StudentNumber");
            infoElement.InnerText = GetValue("StudentNumber");
            rootElement.AppendChild(infoElement);
            //性別
            infoElement = doc.CreateElement("Gender");
            infoElement.InnerText = GetValue("Gender");
            rootElement.AppendChild(infoElement);
            //身分證號
            infoElement = doc.CreateElement("IDNumber");
            infoElement.InnerText = GetValue("IDNumber");
            rootElement.AppendChild(infoElement);
            //生日
            infoElement = doc.CreateElement("Birthdate");
            infoElement.InnerText = GetValue("Birthdate");
            rootElement.AppendChild(infoElement);
            //年級
            infoElement = doc.CreateElement("GradeYear");
            infoElement.InnerText = GetValue("GradeYear");
            rootElement.AppendChild(infoElement);
            //最後核准日期
            infoElement = doc.CreateElement("LastADDate");
            infoElement.InnerText = GetValue("LastADDate");
            rootElement.AppendChild(infoElement);
            //最後核准文號
            infoElement = doc.CreateElement("LastADNumber");
            infoElement.InnerText = GetValue("LastADNumber");
            rootElement.AppendChild(infoElement);
            //備註
            infoElement = doc.CreateElement("Comment");
            infoElement.InnerText = GetValue("Comment");
            rootElement.AppendChild(infoElement);
            //最後核准異動代碼
            infoElement = doc.CreateElement("LastUpdateCode");
            infoElement.InnerText = GetValue("LastUpdateCode");
            rootElement.AppendChild(infoElement);
            //新學號
            infoElement = doc.CreateElement("NewStudentNumber");
            infoElement.InnerText = GetValue("NewStudentNumber");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-學校
            infoElement = doc.CreateElement("PreviousSchool");
            infoElement.InnerText = GetValue("PreviousSchool");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-學號
            infoElement = doc.CreateElement("PreviousStudentNumber");
            infoElement.InnerText = GetValue("PreviousStudentNumber");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-科別
            infoElement = doc.CreateElement("PreviousDepartment");
            infoElement.InnerText = GetValue("PreviousDepartment");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-(最後核准日期)
            infoElement = doc.CreateElement("PreviousSchoolLastADDate");
            infoElement.InnerText = GetValue("PreviousSchoolLastADDate");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-(最後核准文號)
            infoElement = doc.CreateElement("PreviousSchoolLastADNumber");
            infoElement.InnerText = GetValue("PreviousSchoolLastADNumber");
            rootElement.AppendChild(infoElement);
            //轉入前學生資料-年級
            infoElement = doc.CreateElement("PreviousGradeYear");
            infoElement.InnerText = GetValue("PreviousGradeYear");
            rootElement.AppendChild(infoElement);
            //入學資格-畢業國中所在地代碼
            infoElement = doc.CreateElement("GraduateSchoolLocationCode");
            infoElement.InnerText = GetValue("GraduateSchoolLocationCode");
            rootElement.AppendChild(infoElement);
            //入學資格-畢業國中
            infoElement = doc.CreateElement("GraduateSchool");
            infoElement.InnerText = GetValue("GraduateSchool");
            rootElement.AppendChild(infoElement);
            //畢(結)業證書字號
            infoElement = doc.CreateElement("GraduateCertificateNumber");
            infoElement.InnerText = GetValue("GraduateCertificateNumber");
            rootElement.AppendChild(infoElement);

            //NewData
            infoElement = doc.CreateElement("NewData");
            infoElement.InnerText = GetValue("NewData");
            rootElement.AppendChild(infoElement);

            #region 2009年新制新增

            //班別
            infoElement = doc.CreateElement("ClassType");
            infoElement.InnerText = GetValue("ClassType");
            rootElement.AppendChild(infoElement);

            //特殊身份代碼
            infoElement = doc.CreateElement("SpecialStatus");
            infoElement.InnerText = GetValue("SpecialStatus");
            rootElement.AppendChild(infoElement);

            //身份證號註
            infoElement = doc.CreateElement("IDNumberComment");
            infoElement.InnerText = GetValue("IDNumberComment");
            rootElement.AppendChild(infoElement);

            //舊班別
            infoElement = doc.CreateElement("OldClassType");
            infoElement.InnerText = GetValue("OldClassType");
            rootElement.AppendChild(infoElement);

            //舊科別代碼
            infoElement = doc.CreateElement("OldDepartmentCode");
            infoElement.InnerText = GetValue("OldDepartmentCode");
            rootElement.AppendChild(infoElement);

            //國中畢業年度
            infoElement = doc.CreateElement("GraduateSchoolYear");
            infoElement.InnerText = GetValue("GraduateSchoolYear");
            rootElement.AppendChild(infoElement);

             //入學資格備註（註2）
            infoElement = doc.CreateElement("GraduateComment");
            infoElement.InnerText = GetValue("GraduateComment");
            rootElement.AppendChild(infoElement);

            #endregion

            return rootElement;
        }

        public bool IsValid()
        {
            int ErrorCount = 0;
            ErrorCount = _ErrorControls.Count;

            // 過濾新生異動年級檢查
            if (_Style == UpdateRecordType.新生異動)
            {
                foreach (Control ctr in _ErrorControls)
                    if (ctr.Tag.ToString() == "GradeYear")
                        ErrorCount--;
            }
            return (ErrorCount == 0);
            //return (_ErrorControls.Count == 0);
        }

        #region 設定所有Control的事件處理
        private void SetupControl(Control control)
        {
            foreach (Control var in control.Controls)
            {
                string tag = "" + var.Tag;
                if (tag != "")
                {
                    if (!_ControlDictionary.ContainsKey(tag))
                        _ControlDictionary.Add(tag, new List<Control>());
                    _ControlDictionary[tag].Add(var);
                    ValueValidate(var, true);
                    var.TextChanged += new EventHandler(Control_TextChanged);
                    var.LostFocus += new EventHandler(Control_TextChanged);
                    //判斷如果是有下拉式選單則先設定下拉式選單資料
                    #region 判斷如果是有下拉式選單則先設定下拉式選單資料
                    switch (tag)
                    {
                        case "GradeYear":
                            DropDownGradeYear(var, null);
                            break;
                        case "Gender":
                            DropDownGender(var, null);
                            break;
                        //設定同時加入清單，當清單被改變時可以回來更新
                        case "Department":
                            _DepartmentControls.Add(var);
                            DropDownDepartment(var, null);
                            break;
                        case "OldDepartmentCode":
                            _DepartmentControls.Add(var);
                            DropDownDepartmentCode(var, ((ComboBoxEx)var).DroppedDown);
                            ((ComboBoxEx)var).DropDownChange += new ComboBoxEx.OnDropDownChangeEventHandler(this.DropDownDepartmentCode);
                            break;
                        //設定同時加入清單，當清單被改變時可以回來更新
                        case "UpdateCode":
                            _UpdateCodeControls.Add(var);
                            DropDownUpdateCode(var, ((ComboBoxEx)var).DroppedDown);
                            ((ComboBoxEx)var).DropDownChange += new ComboBoxEx.OnDropDownChangeEventHandler(this.DropDownUpdateCode);
                            break;
                        case "ClassType":
                            DropDownClassType(var, ((ComboBoxEx)var).DroppedDown);
                            ((ComboBoxEx)var).DropDownChange += new ComboBoxEx.OnDropDownChangeEventHandler(this.DropDownClassType);
                            break;
                        case "OldClassType":
                            DropDownClassType(var, ((ComboBoxEx)var).DroppedDown);
                            ((ComboBoxEx)var).DropDownChange += new ComboBoxEx.OnDropDownChangeEventHandler(this.DropDownClassType);
                            break;
                        //case "SpecialStatus":
                        //    DropDownSpecialStatus(var, ((ComboBoxEx)var).DroppedDown);
                        //    ((ComboBoxEx)var).DropDownChange += new ComboBoxEx.OnDropDownChangeEventHandler(this.DropDownSpecialStatus);
                        //    break;
                        default:
                            break;
                    }
                    #endregion
                }
                if (var.Controls.Count > 0)
                {
                    SetupControl(var);
                }
            }
        }

        private void DropDownGradeYear(object sender, EventArgs e)
        {
            ComboBox gradeYear = (ComboBox)sender;
            if (!gradeYear.ContainsFocus)
            {
                gradeYear.SelectedItem = null;
                gradeYear.Items.Clear();
                for (int i = 1; i <= 12;i++ )
                    gradeYear.Items.Add(i+"");
                //gradeYear.Items.Add("延修生");
            }
        }

        private void DropDownGender(object sender, EventArgs e)
        {
            ComboBox gander = (ComboBox)sender;
            gander.SelectedItem = null;
            gander.Items.Clear();
            gander.Items.Add("");
            gander.Items.Add("男");
            gander.Items.Add("女");
        }

        private void DropDownUpdateCode(object sender, bool Expand)
        {
            ComboBox updateCode = (ComboBox)sender;
            string s = "" + updateCode.SelectedItem;

            updateCode.DropDownHeight = 350;
            updateCode.DropDownWidth = 250;
            //updateCode.SelectedItem = null;
            updateCode.Items.Clear();
            foreach (string var in GetUpdateCodeSynopsis(_Style.ToString()).Keys)
            {
                updateCode.Items.Add(Expand ? var + "-" + GetUpdateCodeSynopsis(_Style.ToString())[var] : var);
            }
            updateCode.SelectedItem = s.Split("-".ToCharArray())[0];
            //if (Expand)
            //    updateCode.Text = "";
        }

        private void DropDownDepartmentCode(object sender, bool Expand)
        {
            ComboBox cmbDepartmentCode = (sender as ComboBox);

            string s = "" + cmbDepartmentCode.SelectedItem;
            cmbDepartmentCode.DropDownHeight = 350;
            cmbDepartmentCode.DropDownWidth = 250;
            cmbDepartmentCode.Items.Clear();

            foreach(SHDepartmentRecord var in _DepartmentList)
                cmbDepartmentCode.Items.Add(Expand ? var.Code +"-"+ var.FullName : var.Code);

            cmbDepartmentCode.SelectedItem = s.Split("-".ToCharArray())[0];
        }
        
        private void DropDownDepartment(object sender, EventArgs e)
        {
            ComboBox department = (ComboBox)sender;
            department.DropDownHeight = 350;
            department.DropDownWidth = 250;
            department.SelectedItem = null;
            department.Items.Clear();
            foreach (SHDepartmentRecord var in _DepartmentList)
            {
                department.Items.Add(var.FullName);
            }
        }

        private void DropDownClassType(object sender,bool Expand)
        {
            ComboBox cmbClassType = (sender as ComboBox);

            string s = "" + cmbClassType.SelectedItem;
            cmbClassType.DropDownHeight = 350;
            cmbClassType.DropDownWidth = 250;
            cmbClassType.Items.Clear();
            cmbClassType.Items.Add(Expand ? "1-日間部" : "1");
            cmbClassType.Items.Add(Expand ? "3-實用技能班" : "3");
            cmbClassType.Items.Add(Expand ? "4-建教班" : "4");
            cmbClassType.Items.Add(Expand ? "7-重點產業班與台德菁英班" : "7");         
            cmbClassType.SelectedItem = s.Split("-".ToCharArray())[0];
        }

        #endregion

        public void SetDefaultValue(string studentid)
        {
            _Action = UpdateRecordAction.Insert;
            _StudentID = studentid;

            SetValue("UpdateDate", DateTime.Now.ToShortDateString());
            SHStudentRecord studentRec = SHStudent.SelectByID(studentid);

            #region 查學籍身分

            List<SHPermrecStatusMappingInfo> MappingInfos = SHPermrecStatusMapping.SelectyByStudentID(studentid);

            if (MappingInfos.Count>0)
            {
                string comment = "";

                foreach (SHPermrecStatusMappingInfo MappingInfo in MappingInfos)
                {
                    if (!string.IsNullOrEmpty(comment)) comment += ",";
                    comment += MappingInfo.Code;
                }

                SetValue("SpecialStatus", comment);
                SetValue("Comment",comment);
            }

            #endregion

            SetValue("Name", studentRec.Name);
            SetValue("Birthdate", K12.Data.DateTimeHelper.ToDisplayString(studentRec.Birthday));
            SetValue("Gender", studentRec.Gender);
            SetValue("IDNumber", studentRec.IDNumber);
            SetValue("StudentNumber", studentRec.StudentNumber);
            SetValue("GradeYear", studentRec.Status.ToString() == "延修" ? "延修生" : string.IsNullOrEmpty(studentRec.RefClassID) ? "" : K12.Data.Int.GetString(studentRec.Class.GradeYear));
            if(studentRec.Department != null )
                SetValue("Department", studentRec.Department.FullName.Contains(":") ? studentRec.Department.FullName.Split(':')[0] : studentRec.Department.FullName);
            DateTime lastADDate = DateTime.MinValue;
            string lastADNumber = "";
            string lastUpdateCode = "";

            foreach (SHUpdateRecordRecord var in SHUpdateRecord.SelectByStudentID(studentRec.ID))
            {
                DateTime d1;
                if (DateTime.TryParse(var.ADDate, out d1) && d1 > lastADDate)
                {
                    lastADDate = d1;
                    lastADNumber = var.ADNumber;
                    lastUpdateCode = var.UpdateCode;
                }
            }
            SetValue("LastADDate", lastADNumber == "" ? "" : lastADDate.ToShortDateString());
            SetValue("LastADNumber", lastADNumber);
            SetValue("LastUpdateCode", lastUpdateCode);

            Style = UpdateRecordType.學籍異動;

            //Log，紀錄修改前的資料
            foreach (XmlNode node in GetElement().ChildNodes)
            {
                beforeData.Add(node.Name, node.InnerText);
            }
        }

        public void SetUpdateValue(string updateRecordId)
        {
            _Action = UpdateRecordAction.Update;
            _UpdateRecordID = updateRecordId;

            DSResponse dsrsp = SmartSchool.Feature.QueryStudent.GetUpdateRecord(updateRecordId);
            XmlElement element = dsrsp.GetContent().GetElement("UpdateRecord");

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name != "ContextInfo")
                {
                    if (node.Name == "UpdateCode")
                    {
                        _UpdateCodeLoadedEvent.WaitOne();
                        foreach (UpdateRecordType type in new UpdateRecordType[] { UpdateRecordType.新生異動, UpdateRecordType.轉入異動, UpdateRecordType.學籍異動, UpdateRecordType.畢業異動 })
                        {
                            if (_UpdateCodeSynopsis.ContainsKey(type.ToString()) && _UpdateCodeSynopsis[type.ToString()].ContainsKey(node.InnerText))
                            {
                                Style = type;
                                break;
                            }
                        }
                        _newDataField.ChangeNewDataText(node.InnerText);
                    }
                    this.SetValue(node.Name, node.InnerText);
                }
                else
                {
                    if (node.SelectSingleNode("ContextInfo") != null)
                    {
                        foreach (XmlNode contextInfo in node.SelectSingleNode("ContextInfo").ChildNodes)
                        {
                            this.SetValue(contextInfo.Name, contextInfo.InnerText);
                        }
                    }
                }
            }

            //Log，紀錄修改前的資料
            foreach (XmlNode node in GetElement().ChildNodes)
            {
                beforeData.Add(node.Name, node.InnerText);
            }
        }

        public string[] AllowedUpdateCode
        {
            get { return _AllowedUpdateCode; }
            set { _AllowedUpdateCode = value; }
        }

        public bool Save()
        {
            if (_Action == UpdateRecordAction.None)
                return false;

            bool _saved = false;

            DSXmlHelper helper = new DSXmlHelper("InsertRequest");
            helper.AddElement("UpdateRecord");
            helper.AddElement("UpdateRecord", "Field");
            if (_Action != UpdateRecordAction.Update)
                helper.AddElement("UpdateRecord/Field", "RefStudentID", _StudentID);
            helper.AddElement("UpdateRecord/Field", "ContextInfo");

            // 加入學年度學期
            helper.AddElement("UpdateRecord/Field", "SchoolYear",K12.Data.School.DefaultSchoolYear);
            helper.AddElement("UpdateRecord/Field", "Semester",K12.Data.School.DefaultSemester);

            XmlDocument contextInfo = new XmlDocument();
            XmlElement root = contextInfo.CreateElement("ContextInfo");
            contextInfo.AppendChild(root);

            XmlElement Elm = GetElement();

            foreach (XmlNode node in Elm.ChildNodes)
            {
                //Log，紀錄修改後的資料
                if (afterData.ContainsKey(node.Name))
                    afterData.Remove(node.Name);
                afterData.Add(node.Name, node.InnerText);

                // 如果是 Previous 開頭的全丟到 ContextInfo 中

                //2009年新制新增欄位
                //* 班別（ClassType）
                //* 特殊身份代碼（SpecialStatus）
                //* 身分證號註（註1）（IDNumberComment）
                //* 舊班別（OldClassType）
                //* 舊科別代碼（OldDepartmentCode）
                //* 國中畢業年度（GraduateSchoolYear）
                //* 入學資格備註（註2）（GraduateComment）

                if (node.Name.StartsWith("Previous") ||
                    node.Name.StartsWith("Graduate") ||
                    node.Name == "NewStudentNumber" ||
                    node.Name == "LastUpdateCode" ||
                    node.Name == "NewData" ||
                    node.Name == "ClassType" ||
                    node.Name == "SpecialStatus" ||
                    node.Name == "IDNumberComment" ||
                    node.Name == "OldClassType" ||
                    node.Name == "OldDepartmentCode" ||
                    node.Name == "GraduateSchoolYear" ||
                    node.Name == "GraduateComment"
                    )
                {
                    XmlNode importNode = node.Clone();
                    importNode = contextInfo.ImportNode(importNode, true);
                    root.AppendChild(importNode);
                }
                else
                {
                    helper.AddElement("UpdateRecord/Field", node.Name, node.InnerText);
                }
            }

            // 將 contextInfo 這個 document 的資料塞進 ContextInfo 的 CdataSection 裡
            helper.AddXmlString("UpdateRecord/Field/ContextInfo", root.OuterXml);

            if (_Action == UpdateRecordAction.Update)
            {
                // 補上條件
                helper.AddElement("UpdateRecord", "Condition");
                helper.AddElement("UpdateRecord/Condition", "ID", _UpdateRecordID);
            }

            try
            {
                if (IsValid())
                {
                    if (_Action == UpdateRecordAction.Update)
                        EditStudent.ModifyUpdateRecord(new DSRequest(helper));
                    else
                        EditStudent.InsertUpdateRecord(new DSRequest(helper));

                    _saved = true;
                }
            }
            catch (Exception ex)
            {
                if (_Action == UpdateRecordAction.Update)
                    MsgBox.Show("修改異動資料失敗：" + ex.Message);
                else
                    MsgBox.Show("新增異動資料失敗：" + ex.Message);
            }

            #region 處理 Log，寫入 Log

            StringBuilder desc = new StringBuilder("");

            SHStudentRecord StudentRec = SHStudent.SelectByID(_StudentID);

            desc.AppendLine("學生姓名：" + (StudentRec!=null ? StudentRec.Name + " " : "未知 "));

            if (_Action == UpdateRecordAction.Update)
                desc.AppendLine("修改異動紀錄： " + beforeData["UpdateDate"] + " " + beforeData["UpdateDescription"] + " ");
            else
                desc.AppendLine("新增異動紀錄： " + afterData["UpdateDate"] + " " + afterData["UpdateDescription"] + " ");

            foreach (string var in afterData.Keys)
            {
                if (beforeData[var] != afterData[var])
                {
                    if (_Action == UpdateRecordAction.Update)
                        desc.AppendLine("欄位「" + _EnChDict[var] + "」由「" + beforeData[var] + "」變更為「" + afterData[var] + "」");
                    else
                        desc.AppendLine("新增欄位「" + _EnChDict[var] + "」為「" + afterData[var] + "」");
                }
            }

            EntityAction entityAction = EntityAction.Insert;
            if (_Action == UpdateRecordAction.Update)
                entityAction = EntityAction.Update;

            //CurrentUser.Instance.AppLog.Write(EntityType.Student, entityAction, _StudentID, desc.ToString(), "異動資料", "");

            #endregion

            return _saved;
        }

        internal class ChangVisibleDataField
        {
            private Dictionary<string, string> _code_text = new Dictionary<string, string>();
            private List<string> _NewStudentNumberUpdateCodes = new List<string>() { "211", "221", "222", "223", "224", "231", "232", "233", "234", "401" };
            private LabelX _newDataLabel;
            private TextBoxX _newDataText;
            private LabelX _newStudentNumberLabel;
            private TextBoxX _newStudentNumberText;

            public ChangVisibleDataField(LabelX newDataLabel, TextBoxX newDataText,LabelX newStudentNumberLabel,TextBoxX newStudentNumberText)
            {
                _code_text.Add("401", "更正學號");
                _code_text.Add("402", "更正姓名");
                _code_text.Add("403", "更正性別");
                _code_text.Add("404", "更正籍貫");
                _code_text.Add("405", "更正生日");
                _code_text.Add("407", "更正身份證號碼");
                _code_text.Add("408", "更正入學資格學校名稱");
                _code_text.Add("409", "更正特殊身份");
                _code_text.Add("499", "更正其他學籍事項");


                _newDataLabel = newDataLabel;
                _newDataText = newDataText;
                _newStudentNumberLabel = newStudentNumberLabel;
                _newStudentNumberText = newStudentNumberText;
            }

            public void ChangeNewDataText(string code)
            {
                if (_code_text.ContainsKey(code))
                {
                    _newDataLabel.Text = _code_text[code];
                    NewDataVisible = true;
                    NewStudentNumberVisible = false;
                }
                else if (_NewStudentNumberUpdateCodes.Contains(code))
                {
                    NewDataVisible = false;
                    NewStudentNumberVisible = true;
                }
                else
                {
                    NewDataVisible = false;
                    NewStudentNumberVisible = false;
                }
            }

            public bool NewDataVisible
            {
                get { return _newDataText.Visible; }
                set 
                {
                    _newDataLabel.Visible = _newDataText.Visible = value;
                    _newDataText.Visible = value;
                }
            }

            public bool NewStudentNumberVisible
            {
                get { return _newStudentNumberText.Visible; }
                set
                {
                    _newStudentNumberLabel.Visible = value;
                    _newStudentNumberText.Visible = value;
                }
            }
        }

        private void btngetSchoolName_Click(object sender, EventArgs e)
        {
          ////  JHPermrec.UpdateRecord.UpdateRecordItemControls.GetJHSchoolNames gjn = new JHPermrec.UpdateRecord.UpdateRecordItemControls.GetJHSchoolNames();

          //  if (gjn.ShowDialog() == DialogResult.OK)
          //  {
          //      SetValue("GraduateSchool",gjn.County + gjn.SchoolName);
          //      gjn.Close();
          //  }
        }
    }
}