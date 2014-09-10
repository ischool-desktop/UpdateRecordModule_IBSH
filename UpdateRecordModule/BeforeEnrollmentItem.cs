using System;
using System.ComponentModel;
using System.Windows.Forms;
using Framework;
using SHSchool.Data;
using FCode = FISCA.Permission.FeatureCodeAttribute;
using FISCA.Permission;

namespace UpdateRecordModule_IBSH
{
    [FCode("SHSchool.Student.BeforeEnrollmentItem", "前級畢業資訊")]
    public partial class BeforeEnrollmentItem : FISCA.Presentation.DetailContent
    {
        PermRecLogProcess prlp;
        SHBeforeEnrollmentRecord _BeforeEnrollmentRecord;
        private BackgroundWorker _worker = new BackgroundWorker();
        private ChangeListener listener = new ChangeListener();
        private bool PaddingWorking = false;
        private ErrorProvider epSeatNo = new ErrorProvider();
        public static string FeatureCode = "";
        private FeatureAce _permission;

        public BeforeEnrollmentItem()            
        {
            InitializeComponent();

            prlp = new PermRecLogProcess();
            Group = "前級畢業資訊";

            #region 權限判斷程式碼。
            //取得此 Class 定議的 FeatureCode。
            FeatureCode = FeatureCodeAttribute.GetCode(this.GetType());
            _permission = FISCA.Permission.UserAcl.Current[FeatureCode];

            txtClass.Enabled = _permission.Editable;
            txtMemo.Enabled = _permission.Editable;
            txtSchool.Enabled  = _permission.Editable;
            txtSchoolLocation.Enabled = _permission.Editable;
            txtSeatNo.Enabled = _permission.Editable;
            
            #endregion
        }

        void BeforeEnrollmentItem_Disposed(object sender, EventArgs e)
        {
            SHBeforeEnrollment.AfterUpdate -= new EventHandler<K12.Data.DataChangedEventArgs>(SHBeforeEnrollment_AfterUpdate);
        }

        void SHBeforeEnrollment_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(SHBeforeEnrollment_AfterUpdate), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_worker.IsBusy)
                        _worker.RunWorkerAsync();
                }
            }
        }

        private void listener_StatusChanged(object sender, ChangeEventArgs e)
        {
            if(FISCA.Permission.UserAcl.Current[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;

            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetData();
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PaddingWorking)
            {
                PaddingWorking = false;
                _worker.RunWorkerAsync();
            }
            else
            {
                FillData();
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;

            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
            else
                PaddingWorking = true;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            SaveButtonVisible = false;
            CancelButtonVisible = false;
            GetData();
            FillData();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            _BeforeEnrollmentRecord.School = txtSchool.Text;
            _BeforeEnrollmentRecord.SchoolLocation = txtSchoolLocation.Text;
            _BeforeEnrollmentRecord.ClassName = txtClass.Text;
            int intSeatNo;
            if (string.IsNullOrEmpty(txtSeatNo.Text))
            {
                _BeforeEnrollmentRecord.SeatNo = null;
            }
            else
            {
                if (int.TryParse(txtSeatNo.Text, out intSeatNo))
                    _BeforeEnrollmentRecord.SeatNo = intSeatNo;
                else
                {
                    epSeatNo.SetError(txtSeatNo, "請填入數字.");
                    return;
                }
            }


            _BeforeEnrollmentRecord.Memo = txtMemo.Text;

            int GSchoolYear;
            if (string.IsNullOrEmpty(txtGraduateSchoolYear.Text))
            {
                _BeforeEnrollmentRecord.GraduateSchoolYear = null;
            }
            else
            {
                if (int.TryParse(txtGraduateSchoolYear.Text, out GSchoolYear))
                    _BeforeEnrollmentRecord.GraduateSchoolYear = GSchoolYear.ToString ();
                else
                {
                    epSeatNo.SetError(txtGraduateSchoolYear, "請填入數字.");
                    return;
                }
            }

            SHBeforeEnrollment.Update(_BeforeEnrollmentRecord);
            listener.Reset();
            SaveButtonVisible = false;
            CancelButtonVisible = SaveButtonVisible;

            prlp.SetAfterSaveText("學校名稱", txtSchool.Text);
            prlp.SetAfterSaveText("所在地", txtSchoolLocation.Text);
            prlp.SetAfterSaveText("班級", txtClass.Text);
            prlp.SetAfterSaveText("座號", txtSeatNo.Text);
            prlp.SetAfterSaveText("備註", txtMemo.Text);
            prlp.SetAfterSaveText("國中畢業學年度", txtGraduateSchoolYear.Text);
            prlp.SetActionBy("學籍", "學生前級畢業資訊");
            prlp.SetAction("修改學生前級畢業資訊");
            SHStudentRecord studRec = SHStudent.SelectByID(PrimaryKey);
            prlp.SetDescTitle("學生姓名:" + studRec.Name + ",學號:" + studRec.StudentNumber + ",");
            prlp.SaveLog("", "", "student", PrimaryKey);
        }

        private void GetData()
        {
            _BeforeEnrollmentRecord = SHBeforeEnrollment.SelectByStudentID(PrimaryKey);
        }

        private void FillData()
        {
            listener.SuspendListen();

            txtSchool.Text = _BeforeEnrollmentRecord.School;
            txtSchoolLocation.Text = _BeforeEnrollmentRecord.SchoolLocation;
            txtClass.Text = _BeforeEnrollmentRecord.ClassName;
            if (_BeforeEnrollmentRecord.SeatNo.HasValue)
                txtSeatNo.Text = _BeforeEnrollmentRecord.SeatNo.Value + "";
            else
                txtSeatNo.Text = "";
            txtMemo.Text = _BeforeEnrollmentRecord.Memo;
            txtGraduateSchoolYear.Text = _BeforeEnrollmentRecord.GraduateSchoolYear;
            epSeatNo.Clear();
            listener.ResumeListen();
            listener.Reset();

            prlp.SetBeforeSaveText("學校名稱", txtSchool.Text);
            prlp.SetBeforeSaveText("所在地", txtSchoolLocation.Text);
            prlp.SetBeforeSaveText("班級", txtClass.Text);
            prlp.SetBeforeSaveText("座號", txtSeatNo.Text);
            prlp.SetBeforeSaveText("備註", txtMemo.Text);
            prlp.SetBeforeSaveText("國中畢業學年度", txtGraduateSchoolYear.Text);

        }

        private void txtSeatNo_TextChanged(object sender, EventArgs e)
        {
            int tempNo;
            epSeatNo.Clear();
            if (!string.IsNullOrEmpty(txtSeatNo.Text))
            {
                if (int.TryParse(txtSeatNo.Text, out tempNo))
                {
                    SaveButtonVisible = true;
                    CancelButtonVisible = true;
                    if (_BeforeEnrollmentRecord.SeatNo.HasValue)
                        if (_BeforeEnrollmentRecord.SeatNo.Value == tempNo)
                        {
                            SaveButtonVisible = false;
                            CancelButtonVisible = false;
                        }
                }
                else
                {
                    epSeatNo.SetError(txtSeatNo, "請輸入整數");
                }
            }
        }

        private void BeforeEnrollmentItem_Load(object sender, EventArgs e)
        {
            _BeforeEnrollmentRecord = SHBeforeEnrollment.SelectByStudentID(PrimaryKey);
            listener.Add(new TextBoxSource(txtSchool));
            listener.Add(new TextBoxSource(txtSchoolLocation));
            listener.Add(new TextBoxSource(txtClass));
            listener.Add(new TextBoxSource(txtSeatNo));
            listener.Add(new TextBoxSource(txtMemo));
            listener.Add(new TextBoxSource(txtGraduateSchoolYear));
            listener.Reset();
            listener.StatusChanged += new EventHandler<ChangeEventArgs>(listener_StatusChanged);

            SHBeforeEnrollment.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(SHBeforeEnrollment_AfterUpdate);
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);

            Disposed += new EventHandler(BeforeEnrollmentItem_Disposed);
        }
    }
}
