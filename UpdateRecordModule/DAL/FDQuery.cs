using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Data;
using System.Data;

namespace UpdateRecordModule_IBSH.DAL
{
    /// <summary>
    /// 使用 FISCA.Data Query
    /// </summary>
    class FDQuery
    {
        /// <summary>
        /// 用所有學生取得學生狀態,學生ID,學生狀態
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllStudentStatus1Dict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select student.id,student.status from student;";
            Dictionary<string, string> statusDict = new Dictionary<string, string>();
            statusDict.Add("1", "一般");
            statusDict.Add("2", "延修");
            statusDict.Add("4", "休學");
            statusDict.Add("8", "輟學");
            statusDict.Add("16", "畢業或離校");
            statusDict.Add("256", "刪除");
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string id = dr[0].ToString();
                string statusCode = dr[1].ToString();
                if (!retVal.ContainsKey(id))
                    if(statusDict.ContainsKey(statusCode))
                        retVal.Add(id, statusDict[statusCode]);
            }

            return retVal;
        }

        /// <summary>
        /// 取得所有學生學號_狀態對應 StudentNumber_Status,id
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetAllStudenNumberStatusDict()
        {
            Dictionary<string, int> retVal = new Dictionary<string, int>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select student.student_number,student.status,student.id from student;";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr[0].ToString() + "_" + dr[1].ToString();
                int id = int.Parse(dr[2].ToString());
                if (!retVal.ContainsKey(key))
                    retVal.Add(key, id);
            }

            return retVal;
        }

        /// <summary>
        /// 取得所有學生異動(學號_狀態代號,學生編號)
        /// </summary>
        /// <param name="開始異動代碼"></param>
        /// <param name="結束異動代碼"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHasStudentUpdateRecord01Dict(int beginCode,int endCode)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            string strSQL="select student.student_number,student.status,student.id from student inner join update_record on student.id=update_record.ref_student_id where to_number(update_code,'999') between "+beginCode+" and "+endCode+";";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string key = dr[0].ToString() + "_" + dr[1].ToString();                
                if (!retVal.ContainsKey(key))
                    retVal.Add(key, dr[2].ToString());
            }
            return retVal;
        }


        /// <summary>
        /// 取得學生狀態與資料庫數字對應
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetStudentStatusDBValDict()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("一般", "1");
            retVal.Add("延修", "2");
            retVal.Add("休學", "4");
            retVal.Add("輟學", "8");
            retVal.Add("畢業或離校", "16");
            retVal.Add("刪除", "256");
            retVal.Add("", "1");
            return retVal;
        }


        /// <summary>
        /// 依學號取得學生學號_狀態對應 StudentNumber_Status,id
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetStudenNumberStatusDictByStudentNumber(List<string> StudNumList)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            StringBuilder sbQry = new StringBuilder();
            sbQry.Append("'"); sbQry.Append(string.Join("','",StudNumList.ToArray())); sbQry.Append("'");
            string strSQL = "select student.student_number,student.status,student.id from student where student_number in("+sbQry.ToString ()+");";
            DataTable dt = qh.Select(strSQL);
            Dictionary<string, string> statusDict = new Dictionary<string, string>();
            statusDict.Add("1", "一般");
            statusDict.Add("2", "延修");
            statusDict.Add("4", "休學");
            statusDict.Add("8", "輟學");
            statusDict.Add("16", "畢業或離校");
            statusDict.Add("256", "刪除");
            foreach (DataRow dr in dt.Rows)
            {
                string key="";
                if(statusDict.ContainsKey(dr[1].ToString()))
                    key = dr[0].ToString() + "_" + statusDict[dr[1].ToString()];                
                if (!retVal.ContainsKey(key))
                    retVal.Add(key, dr[2].ToString());
            }

            return retVal;
        }

        /// <summary>
        /// 透過學生ID取得學生異動部分資料，主要用在刪除異動資料畫面時使用
        /// </summary>
        /// <returns></returns>
        public static DataTable GetUpdateRecordInfoDTByStudentIDList(List<string> StudentIDList)
        {
            DataTable dt = null;
            if (StudentIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                //-- 異動編號,學號、班級、座號、姓名、異動日期、異動代碼、原因及事項
                string query = "select update_record.id,student.student_number,class.class_name,student.seat_no,student.name,update_record.update_date,update_record.update_code,update_record.update_desc,student.id as sid from student inner join update_record on student.id=update_record.ref_student_id left join class on student.ref_class_id=class.id where student.id in("+string.Join(",",StudentIDList.ToArray())+") order by student.student_number,class.class_name,student.seat_no,student.name,update_record.update_date,update_record.id;";
                dt = qh.Select(query);            
            }
            return dt;
        }
    }
}
