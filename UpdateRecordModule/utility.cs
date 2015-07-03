using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;
using System.Data;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH
{
    public class utility
    {
        public static void ExportXls(string inputReportName, Workbook inputXls)
        {
            string reportName = inputReportName;

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, reportName + ".xls");

            Workbook wb = inputXls;

            if (File.Exists(path))
            {
                int i = 1;
                while (true)
                {
                    string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                    if (!File.Exists(newPath))
                    {
                        path = newPath;
                        break;
                    }
                }
            }

            try
            {
                wb.Save(path, Aspose.Cells.FileFormatType.Excel2003);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = reportName + ".xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.Save(sd.FileName, Aspose.Cells.FileFormatType.Excel2003);

                    }
                    catch
                    {
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 檢查一般學生學號是否重複
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool CheckStudentNumberSame1(string number)
        {
            bool retVal = false;
            QueryHelper qh = new QueryHelper();
            string strSQL = "select id from student where status=1 and student_number='"+number+"'";
            DataTable dt= qh.Select(strSQL);
            if (dt.Rows.Count > 0)
                retVal = true;
            return retVal;
        }

        /// <summary>
        /// 取的所有狀態一般1的學生學號
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetStudentNumberStatus1()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select student_number,id from student where status=1;";            
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string num = dr[0].ToString();

                if (!retVal.ContainsKey(num))
                    retVal.Add(num, dr[1].ToString());
            }
            return retVal;
        }

        /// <summary>
        /// 取的所有狀態一般1的學生身分證號
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetStudentIDNumberStatus1()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select id_number,id from student where status=1;";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string num = dr[0].ToString();

                if (!retVal.ContainsKey(num))
                    retVal.Add(num, dr[1].ToString());
            }
            return retVal;
        }

        /// <summary>
        /// 取讀非刪除學生ID
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStudentIDListNot256()
        {
            List<string> retVal = new List<string>();
            QueryHelper qh = new QueryHelper();
            string strSQL = "select id from student where status<256;";
            DataTable dt = qh.Select(strSQL);
            foreach (DataRow dr in dt.Rows)
                retVal.Add(dr[0].ToString());

            return retVal;
        }

        /// <summary>
        /// 學籍身分對照表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetPermCodeMappingDict()
        {
            Dictionary<string, List<string>> retValue = new Dictionary<string, List<string>>();
            QueryHelper qh = new QueryHelper();
            string strSQL = @"select content from list where name='SchoolConfig'";
            DataTable dt = qh.Select(strSQL);
            XElement elmRoot = null;

            try
            {
                if (dt.Rows.Count == 1)
                {
                    elmRoot = XElement.Parse(dt.Rows[0][0].ToString());
                    if (elmRoot.Element("學籍身分對照表") != null)
                    {
                        foreach (XElement elm1 in elmRoot.Element("學籍身分對照表").Elements("Identity"))
                        {
                            string Code = elm1.Attribute("Code").Value;
                            foreach (XElement elm2 in elm1.Elements("Tag"))
                            {
                                string FullName = elm2.Attribute("FullName").Value;
                                if (!retValue.ContainsKey(FullName))
                                    retValue.Add(FullName, new List<string>());

                                if (!retValue[FullName].Contains(Code))
                                    retValue[FullName].Add(Code);
                            }
                        }
                    }
                }
            }
            catch (Exception ex
                ) { }
            return retValue;
        }

        /// <summary>
        /// 透過學生ID取得學生類別 FullName
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetStudentFullNameDictByStudentIDs(List<string> StudentIDList)
        {
            Dictionary<string, List<string>> retVal = new Dictionary<string, List<string>>();
            if (StudentIDList.Count > 0)
            {
                QueryHelper qh = new QueryHelper();
                string strSQL = @"select ref_student_id as sid,(case when prefix is null then name else prefix||':'||name end) as full_name from tag inner join tag_student on tag.id=tag_student.ref_tag_id where ref_student_id in(" + string.Join(",", StudentIDList.ToArray()) + ") order by ref_student_id";
                DataTable dt = qh.Select(strSQL);
                foreach (DataRow dr in dt.Rows)
                {
                    string sid = dr["sid"].ToString();
                    if (!retVal.ContainsKey(sid))
                        retVal.Add(sid, new List<string>());
                    
                    retVal[sid].Add(dr["full_name"].ToString());
                }
            }
            return retVal;
        }

        /// <summary>
        /// 透過異動代碼區間來判斷異動類別
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetUpdateTypeByCode(string code)
        {
            string retVal = "";

            int intcode;
            if (int.TryParse(code, out intcode))
            {
                if (intcode > 0 && intcode < 100)
                    retVal = "新生異動";

                if (intcode >= 100 && intcode < 200)
                    retVal = "轉入異動";

                if (intcode >= 200 && intcode != 501)
                    retVal = "學籍異動";

                if (intcode == 501)
                    retVal = "畢業異動";
            }
            return retVal;
        }
    }
}
