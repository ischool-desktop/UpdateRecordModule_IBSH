using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateRecordModule_IBSH.GovernmentalDocument.Reports.List
{
    /// <summary>
    /// 報表用異動(讀取XML暫存與排序使用)
    /// </summary>
    public class rpt_UpdateRecord
    {  
        /// <summary>
        /// 學生系統編號
        /// </summary> 
        public string StudentID { get; set; }
        
        /// <summary>
        /// 異動編號
        /// </summary> 
        public string URID { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>  
        public string Birthday { get; set; }

        /// <summary>
        /// 年級
        /// </summary>  
        public string GradeYear { get; set; }

        /// <summary>
        /// 更正後資料
        /// </summary> 
        public string NewData { get; set; }

        /// <summary>
        /// 身份證字號
        /// </summary> 
        public string IDNumber { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// 性別代碼
        /// </summary>  
        public string GenderCode { get; set; }

        /// <summary>
        /// 科別代碼
        /// </summary> 
        public string DeptCode { get; set; }
        
        /// <summary>
        /// 原年級
        /// </summary>  
        public string PreviousGradeYear { get; set; }

        /// <summary>
        /// 原科別代碼
        /// </summary>  
        public string PreviousDeptCode { get; set; }

        /// <summary>
        /// 原備查文字
        /// </summary>  
        public string PreviousSchoolLastADDoc { get; set; }

        /// <summary>
        /// 原備查文號
        /// </summary> 
        public string PreviousSchoolLastADNum { get; set; }

        /// <summary>
        /// 原備查日期
        /// </summary>  
        public string PreviousSchoolLastADDate { get; set; }

        /// <summary>
        /// 備查日期
        /// </summary>
        public string LastADDate { get; set; }

        /// <summary>
        /// 備查文號
        /// </summary>
        public string LastADNum { get; set; }

        /// <summary>
        /// 備查文字
        /// </summary>
        public string LastADDoc { get; set; }

        /// <summary>
        /// 原學校代碼
        /// </summary>  
        public string PreviousSchoolCode { get; set; }

        /// <summary>
        /// 原學期
        /// </summary>  
        public string PreviousSemester { get; set; }

        /// <summary>
        /// 原學號
        /// </summary>  
        public string PreviousStudentNumber { get; set; }

        /// <summary>
        /// 特殊身份代碼
        /// </summary> 
        public string SpecialStatusCode { get; set; }
        
        /// <summary>
        /// 班別
        /// </summary> 
        public string ClassType { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 異動原因代碼
        /// </summary>
        public string UpdateCode { get; set; }

        /// <summary>
        /// 異動順序
        /// </summary>  
        public string Order { get; set; }

        /// <summary>
        /// 備註說明
        /// </summary>  
        public string Comment { get; set; }

        /// <summary>
        /// 註1
        /// </summary>  
        public string Comment1 { get; set; }
        
        /// <summary>
        /// 註2
        /// </summary>  
        public string Comment2 { get; set; }

        /// <summary>
        /// 學號
        /// </summary>  
        public string StudentNumber { get; set; }

        /// <summary>
        /// 轉入日期
        /// </summary> 
        public string TransferDate { get; set; }

        /// <summary>
        /// 轉入身份別
        /// </summary>
        public string TransferStatus { get; set; }

        /// <summary>
        /// 新學號
        /// </summary>
        public string NewStudNumber { get; set; }

        /// <summary>
        /// 畢業證書字號
        /// </summary>
        public string GraduateCertificateNumber { get; set; }

        /// <summary>
        /// 應畢業學年度
        /// </summary>
        public string ExpectGraduateSchoolYear { get; set; }
    }
}
