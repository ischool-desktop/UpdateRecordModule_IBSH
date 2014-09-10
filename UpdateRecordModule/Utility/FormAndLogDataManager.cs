using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.Editors.DateTimeAdv;
using DevComponents.DotNetBar.Controls;


namespace UpdateRecordModule_IBSH.Utility
{
    /// <summary>
    /// 管理畫面與Log使用
    /// </summary>
    class FormAndLogDataManager
    {
        PermRecLogProcess _prlp;

        public FormAndLogDataManager(PermRecLogProcess LogData)
        {
            _prlp = LogData;
        }

        /// <summary>
        /// ComboBox 用
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="obj"></param>
        /// <param name="logObjName"></param>
        /// <returns></returns>
        public ComboBoxEx SetFormData(string strValue, ComboBoxEx obj, string logObjName)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                obj.Text = "";
                _prlp.SetBeforeSaveText(logObjName, "");
            }
            else
            {
                obj.Text = strValue;
                _prlp.SetBeforeSaveText(logObjName, strValue);
            }

            return obj;
        }

        /// <summary>
        /// TextBox 用
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="obj"></param>
        /// <param name="logObjName"></param>
        /// <returns></returns>
        public TextBoxX SetFormData(string strValue, TextBoxX obj, string logObjName)
        {
            if (string.IsNullOrEmpty(strValue))
            {
                obj.Text = "";
                _prlp.SetBeforeSaveText(logObjName, "");
            }
            else
            {
                obj.Text = strValue;
                _prlp.SetBeforeSaveText(logObjName, strValue);
            }

            return obj;
        }

        /// <summary>
        /// DateTimeInput 用
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="obj"></param>
        /// <param name="logObjName"></param>
        /// <returns></returns>
        public DateTimeInput SetFormData(string strValue, DateTimeInput obj, string logObjName)
        {
            DateTime dt;

            if (DateTime.TryParse(strValue, out dt))
            {
                obj.Value = dt;
                _prlp.SetBeforeSaveText(logObjName, strValue);
            }
            else
            {
                obj.IsEmpty = true;
                _prlp.SetBeforeSaveText(logObjName, "");
            }

            return obj;
        }

        public string GetFormData(TextBoxX obj, string LogObjName)
        {
            _prlp.SetAfterSaveText(LogObjName, obj.Text);
            return obj.Text;
        }

        public string GetFormData(ComboBoxEx obj, string LogObjName)
        {
            _prlp.SetAfterSaveText(LogObjName, obj.Text);
            return obj.Text;
        }

        public string GetFormData(DateTimeInput obj, string LogObjName)
        {
            if (string.IsNullOrEmpty(obj.Text))
                return "";
            else
            {
                string str = obj.Value.ToShortDateString();
                _prlp.SetAfterSaveText(LogObjName, str);
                return str;
            }
        }





        /// <summary>
        /// 取得 Log 資料
        /// </summary>
        /// <returns></returns>
        public PermRecLogProcess GetLogData()
        {
            return _prlp;
        }
    }
}
