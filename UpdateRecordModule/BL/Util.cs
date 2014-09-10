using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.BL
{
    /// <summary>
    /// 名冊用轉換工具
    /// </summary>
    public class Util
    {
        /// <summary>
        /// 將西元日期傳換成民國000000格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertDate1(string str)
        {
            string retVal = "";
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dt;
                if (DateTime.TryParse(str, out dt))
                {
                    // 判斷日期內容是民國或西元
                    if(str.Trim().IndexOf("/")<4)
                        retVal = (dt.Year-1900) + string.Format("{0:00}", dt.Month)+string.Format("{0:00}", dt.Day); // 民國
                    else
                        retVal = (dt.Year - 1911) + string.Format("{0:00}", dt.Month) + string.Format("{0:00}", dt.Day); // 西元
                }
            }
            return retVal;
        }

        /// <summary>
        /// 取得文字號的文字
        /// </summary>
        /// <param name="DocNo"></param>
        /// <returns></returns>
        public static string GetDocNo_Doc(string DocNo)
        {
            string retVal = DocNo;
            if (!string.IsNullOrEmpty(DocNo))
            {
                int len = DocNo.IndexOf("字");
                if (len > 0)
                    retVal = DocNo.Substring(0, len);
            }
            return retVal;
        }

        /// <summary>
        /// 取得文字號的文號
        /// </summary>
        /// <param name="DocNo"></param>
        /// <returns></returns>
        public static string GetDocNo_No(string DocNo)
        {
            string retVal = DocNo;
            if (!string.IsNullOrEmpty(DocNo))
            {
                int bs = DocNo.IndexOf("第") + 1; // 開始
                int be = DocNo.IndexOf("號"); // 結束
                if (bs > 0 && be > bs)
                    retVal = DocNo.Substring(bs, (be - bs));
            }
            return retVal;
        }

        /// <summary>
        /// 將日期年轉成民國年 2010/1/1->99/1/1
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static string ConvertDateStr2(string strDate)
        {
            string retVal = string.Empty;
            DateTime dt;
            if (DateTime.TryParse(strDate, out dt))
            {
                if (dt.Year > 1000)
                    retVal = (dt.Year - 1911) + "/" + dt.Month + "/" + dt.Day;
            }
            return retVal;
        }

        /// <summary>
        /// 取得國中所在地代碼與名稱對照
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetJHSchooLocationNameDict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            XElement elmRoot = XElement.Parse(Properties.Resources.JHSchoolList);
            foreach (XElement elm in elmRoot.Elements("學校"))
                if (!retVal.ContainsKey(elm.Attribute("所在地代碼").Value))
                    retVal.Add(elm.Attribute("所在地代碼").Value, elm.Attribute("所在地").Value);

            return retVal;
        }
    }
}
