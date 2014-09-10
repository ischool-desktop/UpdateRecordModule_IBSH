using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateRecordModule_IBSH
{
    /// <summary>
    /// 放全域變數(用於異動內有全域變數共同存取)
    /// </summary>
    public class Global
    {
      
        public static event EventHandler UpdateDocs;

        public static void OnUpdateDocsChange()
        {
            if (UpdateDocs != null)
                UpdateDocs(null, EventArgs.Empty);
        }    

        /// <summary>
        /// 名冊用學年度
        /// </summary>
        public static string _GSchoolYear { get; set; }
        /// <summary>
        /// 名冊用學期
        /// </summary>
        public static string _GSemester { get; set; }
        /// <summary>
        /// 名冊用名冊名稱
        /// </summary>
        public static string _GDocName { get; set; }
        /// <summary>
        /// 名冊用異動類別
        /// </summary>
        public static string _GUpdateBatchType { get; set; }
        /// <summary>
        /// 名冊用學校代碼
        /// </summary>
        public static string _GSchoolCode { get; set; }
        /// <summary>
        /// 名冊用學校名稱
        /// </summary>
        public static string _GSchoolName { get; set; }

        /// <summary>
        /// 所選的異動名冊ID
        /// </summary>
        public static string _SelectUpdateBatchID { get; set; }

        /// <summary>
        /// 學生學號與狀態暫存
        /// </summary>
        public static Dictionary<string, int> _AllStudentNumberStatusIDTemp = new Dictionary<string, int>();

        /// <summary>
        /// 學生異動暫存
        /// </summary>
        public static Dictionary<string, string> _StudentUpdateRecordTemp = new Dictionary<string, string>();
    }
}
 