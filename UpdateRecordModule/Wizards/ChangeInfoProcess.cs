using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using SHSchool.Data;
//using SmartSchool.ApplicationLog;
using SmartSchool.Common;
using SmartSchool;
using System.Collections.Generic;

namespace UpdateRecordModule_IBSH.Wizards
{
    public partial class ChangeInfoProcess : BaseForm
    {
        private string mStudentID;

        private Dictionary<string, string> _StudNumDictCheck;
        private Dictionary<string, string> _StudIDNumDictCheck;

        /// <summary>
        /// 此版本會爆炸,請改以NewChangeInfoProcess呼叫
        /// </summary>
        /// <param name="vStudentID"></param>
        public ChangeInfoProcess(string vStudentID)
        {
            InitializeComponent();
            // 取得一般狀態學生學號
            _StudNumDictCheck = utility.GetStudentNumberStatus1();
            _StudIDNumDictCheck = utility.GetStudentIDNumberStatus1();

            mStudentID = vStudentID;
            this.updateRecordInfo1.SetDefaultValue(mStudentID);
            this.updateRecordInfo1.Style = UpdateRecordType.學籍異動;
        }

        private void wizardPage1_AfterPageDisplayed(object sender, DevComponents.DotNetBar.WizardPageChangeEventArgs e)
        {
            this.office2007StyleRadioButton1.Checked = office2007StyleRadioButton2.Checked = office2007StyleRadioButton3.Checked = office2007StyleRadioButton4.Checked = office2007StyleRadioButton5.Checked = false;
        }

        private void CanMoveNext(object sender, EventArgs e)
        {
            wizardPage1.NextButtonEnabled = (this.office2007StyleRadioButton1.Checked || office2007StyleRadioButton2.Checked || office2007StyleRadioButton3.Checked || office2007StyleRadioButton4.Checked || office2007StyleRadioButton5.Checked) ? DevComponents.DotNetBar.eWizardButtonState.True : DevComponents.DotNetBar.eWizardButtonState.False;
        }

        private void wizardPage2_AfterPageDisplayed(object sender, DevComponents.DotNetBar.WizardPageChangeEventArgs e)
        {
            labelX1.Text = "新" + (office2007StyleRadioButton1.Checked ? office2007StyleRadioButton1.Text :
                office2007StyleRadioButton2.Checked ? office2007StyleRadioButton2.Text :
                office2007StyleRadioButton3.Checked ? office2007StyleRadioButton3.Text :
                office2007StyleRadioButton4.Checked ? office2007StyleRadioButton4.Text :
                office2007StyleRadioButton5.Checked ? office2007StyleRadioButton5.Text : "啥") + "：";
            wizardPage2.PageDescription = "輸入更正後的" + (office2007StyleRadioButton1.Checked ? office2007StyleRadioButton1.Text :
                office2007StyleRadioButton2.Checked ? office2007StyleRadioButton2.Text :
                office2007StyleRadioButton3.Checked ? office2007StyleRadioButton3.Text :
                office2007StyleRadioButton4.Checked ? office2007StyleRadioButton4.Text :
                office2007StyleRadioButton5.Checked ? office2007StyleRadioButton5.Text : "啥");
            textBoxX1_TextChanged(null, null);
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBoxX1, "");
            bool pass = true;
            if (office2007StyleRadioButton4.Checked)
            {
                DateTime d = DateTime.Now;
                if (!DateTime.TryParse(textBoxX1.Text, out d))
                {
                    errorProvider1.SetError(textBoxX1, "請依照 西元年/月/日 格式輸入");
                    pass = false;
                }
            }
            if (office2007StyleRadioButton5.Checked)
            {
                if (textBoxX1.Text != "男" && textBoxX1.Text != "女")
                {
                    errorProvider1.SetError(textBoxX1, "請輸入\"男\"或\"女\"");
                    pass = false;
                }
            }

            // 檢查身分證號
            if (office2007StyleRadioButton2.Checked)
            {
                if (!string.IsNullOrEmpty(textBoxX1.Text))
                    if (_StudIDNumDictCheck.ContainsKey(textBoxX1.Text))
                    {
                        errorProvider1.SetError(textBoxX1, "系統內已有相同身分證號無法新增");
                        pass = false;
                    }
            }

            // 檢查學號
            if (office2007StyleRadioButton3.Checked)
            {
                if (!string.IsNullOrEmpty(textBoxX1.Text))
                    if (_StudNumDictCheck.ContainsKey(textBoxX1.Text))
                    {
                        errorProvider1.SetError(textBoxX1, "系統內已有相同學號無法新增");
                        pass = false;
                    }
            }

            if (pass)
                wizardPage2.NextButtonEnabled = (textBoxX1.Text == "" ? DevComponents.DotNetBar.eWizardButtonState.False : DevComponents.DotNetBar.eWizardButtonState.True);
            else
                wizardPage2.NextButtonEnabled = DevComponents.DotNetBar.eWizardButtonState.False;
        }

        private void wizardPage3_AfterPageDisplayed(object sender, DevComponents.DotNetBar.WizardPageChangeEventArgs e)
        {
            this.updateRecordInfo1.NewStudentNumberVisible = false;
            this.updateRecordInfo1.NewStudentNumber = "";
            this.updateRecordInfo1.NewData = "";

            if (office2007StyleRadioButton1.Checked)
            {
                this.updateRecordInfo1.UpdateCode = "402";
                this.updateRecordInfo1.UpdateDescription = "更正姓名";
                this.updateRecordInfo1.NewData = textBoxX1.Text;
            }
            if (office2007StyleRadioButton2.Checked)
            {
                this.updateRecordInfo1.UpdateCode = "407";
                this.updateRecordInfo1.UpdateDescription = "更正身分證號碼";
                this.updateRecordInfo1.NewData = textBoxX1.Text;
            }
            if (office2007StyleRadioButton3.Checked)
            {
                this.updateRecordInfo1.UpdateCode = "401";
                this.updateRecordInfo1.UpdateDescription = "更正學號";
                this.updateRecordInfo1.NewStudentNumberVisible = true;
                this.updateRecordInfo1.NewData = textBoxX1.Text;
            }
            if (office2007StyleRadioButton4.Checked)
            {
                this.updateRecordInfo1.UpdateCode = "405";
                DateTime d = DateTime.Parse(textBoxX1.Text);
                this.updateRecordInfo1.UpdateDescription = "更正生日";
                this.updateRecordInfo1.NewData = (d.Year - 1911) + "/" + d.Month + "/" + d.Day;
            }
            if (office2007StyleRadioButton5.Checked)
            {
                this.updateRecordInfo1.UpdateCode = "403";
                this.updateRecordInfo1.UpdateDescription = "更正性別";
                this.updateRecordInfo1.NewData = textBoxX1.Text;
            }

        }

        private void wizardPage2_NextButtonClick(object sender, CancelEventArgs e)
        {

            this.Size = new Size(525, 600);
            this.Location = new Point(this.Location.X - 43, this.Location.Y - 124);
        }

        private void wizardPage3_BackButtonClick(object sender, CancelEventArgs e)
        {
            this.Size = new Size(393, 294);
            this.Location = new Point(this.Location.X + 43, this.Location.Y + 124);
        }

        private void wizardPage3_FinishButtonClick(object sender, CancelEventArgs e)
        {
            StringBuilder desc = new StringBuilder("");
            SHStudentRecord stu = SHStudent.SelectByID(mStudentID);

            desc.AppendLine("學生姓名：" + stu.Name + " ");

            if (office2007StyleRadioButton1.Checked)
            {
                desc.AppendLine("姓名由「" + stu.Name + "」變更為「" + textBoxX1.Text + "」");
                stu.Name = textBoxX1.Text;
            }
            if (office2007StyleRadioButton2.Checked)
            {
                desc.AppendLine("身分證號由「" + stu.IDNumber + "」變更為「" + textBoxX1.Text + "」");
                stu.IDNumber = textBoxX1.Text;
            }
            if (office2007StyleRadioButton3.Checked)
            {
                desc.AppendLine("學號由「" + stu.StudentNumber + "」變更為「" + textBoxX1.Text + "」");
                stu.StudentNumber = textBoxX1.Text;
            }
            if (office2007StyleRadioButton4.Checked)
            {
                desc.AppendLine("生日由「" + stu.Birthday + "」變更為「" + textBoxX1.Text + "」");
                stu.Birthday = K12.Data.DateTimeHelper.Parse(textBoxX1.Text);
            }
            if (office2007StyleRadioButton5.Checked)
            {
                desc.AppendLine("性別由「" + stu.Gender + "」變更為「" + textBoxX1.Text + "」");
                stu.Gender = textBoxX1.Text;
            }

            this.Close();
            //            CurrentUser.Instance.AppLog.Write(EntityType.Student, "學籍更正", mStudentID, desc.ToString(), "", "");
            SHStudent.Update(stu);

            this.updateRecordInfo1.Save();
            //          SmartSchool.Broadcaster.Events.Items["學生/資料變更"].Invoke(mStudentID);
        }
    }
}