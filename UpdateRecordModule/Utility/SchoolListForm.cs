using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.Utility
{
    public partial class SchoolListForm : FISCA.Presentation.Controls.BaseForm
    {
        private string _SchoolName;
        private string _SchoolTypeCode;
        private string _SchoolLocationCode;
        private string _SchoolCode;
        private string _strCounty;
        private string _strSchoolName;

        public enum LoadSchoolType {高中,國中 }

        LoadSchoolType _LoadSchoolType;

        private XElement _Elms;

        public SchoolListForm(LoadSchoolType schoolType)
        {
            InitializeComponent();
            this.MaximumSize = this.MinimumSize = this.Size;
            _LoadSchoolType = schoolType;
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SchoolListForm_Load(object sender, EventArgs e)
        {
            // 取得國中列表
            if (_LoadSchoolType == LoadSchoolType.高中)
            {
                _Elms = BL.Get.SHSchoolList();
                this.Text = "高中";
            }
            else
            {
                _Elms = BL.Get.JHSchoolList();
                this.Text = "國中";
            }
            // 填入資料
            List<string> countys = (from elm in _Elms.Elements("學校") orderby elm.Attribute("所在地").Value select elm.Attribute("所在地").Value).Distinct().ToList();
            foreach (string str in countys)
                lvCounty.Items.Add(str);
        }

        /// <summary>
        /// 取得學校名稱
        /// </summary>
        /// <returns></returns>
        public string GetSchoolName()
        {
            return _SchoolName;        
        }

        /// <summary>
        /// 取得學校類別代碼
        /// </summary>
        /// <returns></returns>
        public string GetSchoolTypeCode()
        {
            return _SchoolTypeCode;
        }

        /// <summary>
        /// 取得學校代碼
        /// </summary>
        /// <returns></returns>
        public string GetSchoolCode()
        {
            return _SchoolCode;
        }

        /// <summary>
        /// 取得學校所在地代碼
        /// </summary>
        /// <returns></returns>
        public string GetSchoolLocationCode()
        {
            return _SchoolLocationCode;        
        }

        private void lvCounty_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvSchoolName.Items.Clear();
            if (lvCounty.SelectedItems.Count > 0)
            {
                _strCounty = lvCounty.SelectedItems[0].Text;
                List<string> strList = (from elm in _Elms.Elements("學校") where elm.Attribute("所在地").Value == _strCounty orderby elm.Attribute("名稱").Value select elm.Attribute("名稱").Value).ToList();
                foreach (string str in strList)
                    lvSchoolName.Items.Add(str);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lvSchoolName.SelectedItems.Count > 0)
            {
                _strSchoolName =lvSchoolName.SelectedItems[0].Text;
                XElement xm = (from elm in _Elms.Elements("學校") where elm.Attribute("所在地").Value == _strCounty && elm.Attribute("名稱").Value == lvSchoolName.SelectedItems[0].Text select elm).First();
                if (xm != null)
                {
                    _SchoolCode = xm.Attribute("代碼").Value;
                    _SchoolName = xm.Attribute("名稱").Value;
                    _SchoolLocationCode = xm.Attribute("所在地代碼").Value;
                    if (_SchoolCode.Length > 3)
                        _SchoolTypeCode = _SchoolCode.Substring(2, 1);                
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
