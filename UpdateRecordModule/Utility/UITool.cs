using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.Utility
{
    /// <summary>
    /// 異動內部工具
    /// </summary>
    class UITool
    {
        /// <summary>
        /// 取得異動字串內代碼
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStrUpdateCode(string str)
        {
            int idx = str.IndexOf('-');
            if ( idx > 0)
            {
                return str.Substring(0, idx );
            }
            else
                return string.Empty;
        }

        public static string GetStrDesc(string str)
        {
            int idx = str.IndexOf('-');
            if (idx > 0)
            {
                idx++;
                return str.Substring(idx, str.Length - idx);
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// 將異動代碼與原因及事由，由XML轉成Dict
        /// </summary>
        /// <param name="UpdateCodeElms"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertUpdateCodeDescDict(List<XElement> UpdateCodeElms)        
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            foreach (XElement elm in UpdateCodeElms)
            {
                if (!retVal.ContainsKey(elm.Element("代號").Value))
                    retVal.Add(elm.Element("代號").Value, elm.Element("原因及事項").Value);
            }
            return retVal;
        }

        /// <summary>
        /// 透異動分類名稱取得代碼
        /// </summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        public static List<string> GetUpdateCodeListByUpdateType(string uType)
        {
            List<string> retVal = new List<string>();
            XElement _UpdateCode = DAL.DALTransfer.GetUpdateCodeList();
            retVal = (from elm in _UpdateCode.Elements("異動") where elm.Element("分類").Value == uType select elm.Element("代號").Value).ToList();

            return retVal;
        }
    }
}
