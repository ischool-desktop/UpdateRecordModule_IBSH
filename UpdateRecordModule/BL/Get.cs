using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

// 商業層主要給第3方使用
namespace UpdateRecordModule_IBSH.BL
{
    /// <summary>
    /// 取得資料(提供非異動核心容易取得資料)
    /// </summary>
    public class Get
    {
        /// <summary>
        /// 取得國中學校名稱與代碼列
        /// </summary>
        /// <returns></returns>
        public static XElement JHSchoolList()
        {
            return XElement.Parse(Properties.Resources.JHSchoolList);
        }

        /// <summary>
        /// 取得高中學校名稱與代碼列
        /// </summary>
        /// <returns></returns>
        public static XElement SHSchoolList()
        {
            return XElement.Parse(Properties.Resources.SHSchoolList);
        }


        /// <summary>
        /// 取得異動代碼List
        /// </summary>
        /// <returns></returns>
        public static XElement UpdateCodeList()
        {
            return DAL.DALTransfer.GetUpdateCodeList();
        }

        /// <summary>
        /// 取得畫面上選的異動名冊ID
        /// </summary>
        /// <returns></returns>
        public static string UpdateBatchSelectID()
        {
            return Global._SelectUpdateBatchID;
        }    

        /// <summary>
        /// 透過異動名冊ID取得異動名冊內學生異動資料List
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<StudUpdateRecDoc> StudUpdateRecDocListByID(string ID)
        {
            List<StudUpdateRecDoc> retVal = new List<StudUpdateRecDoc>();
            StudUpdateRecBatchRec surbr = DAL.DALTransfer.GetStudUpdateRecBatchRec(ID);
            retVal = surbr.StudUpdateRecDocList;
            return retVal;
        }

        /// <summary>
        /// 透動異動名冊ID取得整份名冊
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static StudUpdateRecBatchRec StudUpdateRecBatchRecByID(string ID)
        {
            return DAL.DALTransfer.GetStudUpdateRecBatchRec(ID);
        }
    }
}
