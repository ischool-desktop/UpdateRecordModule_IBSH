using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;

namespace UpdateRecordModule_IBSH.ValidationRule.RowValidator
{
    public class StudCheckUpdateRecordVal01 : IRowVaildator
    {
        #region IRowVaildator 成員
        Dictionary<string, string> _StudStatusDict;

        public StudCheckUpdateRecordVal01()
        {
            _StudStatusDict = DAL.FDQuery.GetStudentStatusDBValDict();
        }

        public string Correct(IRowStream Value)
        {
            return string.Empty;
        }

        public string ToString(string template)
        {
            return template;
        }

        public bool Validate(IRowStream Value)
        {
            // 檢查是否已有異動資料
            bool retVal = true;
            if (Value.Contains("學號") && Value.Contains("狀態"))
            {
                string status = Value.GetValue("狀態").Trim();
                string key = Value.GetValue("學號") + "_";

                if (_StudStatusDict.ContainsKey(status))
                    key += _StudStatusDict[status];
                else
                    key += "1";

                if (Global._StudentUpdateRecordTemp.ContainsKey(key))
                    retVal = false;

            }

            return retVal;
        }

        #endregion
    }
}