using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateRecordModule_IBSH.BL
{
    /// <summary>
    /// 異動名冊內學生異動資料類別(使用在存取異動名冊內學生異動資料)
    /// </summary>
    public class StudUpdateRecDoc
    {
        /// <summary>
        /// 異動代碼
        /// </summary>         
        public string UpdateCode {get; set;}
        /// <summary>
        /// 原因及事項
        /// </summary>
        public string UpdateDescription { get; set; }

        /// <summary>
        /// 異動日期
        /// </summary>
        public string UpdateDate { get; set; }
        /// <summary>
        /// 備註
        /// </summary>    
        public string Comment{get;set;}
        /// <summary>
        /// 班別
        /// </summary>
        public string ClassType{get;set;}
        
        /// <summary>
        /// 科別
        /// </summary>
        public string Department{get;set;}

        /// <summary>
        /// 科別代碼
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 特殊身分代碼
        /// </summary>
        public string SpecialStatus{get;set;}
        /// <summary>
        /// 姓名
        /// </summary>
        public string StudentName{get;set;}
        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber{get;set;}
        
        /// <summary>
        /// 身分證字號
        /// </summary>
        public string IDNumber{get;set;}

        /// <summary>
        /// 生日
        /// </summary>    
        public string Birthday{get;set;}
        /// <summary>
        /// 註1(身分證註記)
        /// </summary>
        public string IDNumberComment{get;set;}

        /// <summary>
        /// 性別
        /// </summary>    
        public string Gender{get;set;}

        /// <summary>
        /// 性別代碼
        /// </summary>
        public string GenderCode { get; set; }

        /// <summary>
        /// 畢業國中
        /// </summary>    
        public string GraduateSchool{get;set;}
        /// <summary>
        /// 畢業國中所在地代碼
        /// </summary>    
        public string GraduateSchoolLocationCode{get;set;}        

        /// <summary>
        /// 註2(入學資格註記)
        /// </summary>
        public string GraduateComment{get;set;}
            
        /// <summary>
        /// 畢業國中代碼
        /// </summary>
        public string GraduateSchoolCode{get;set;}

        /// <summary>
        /// 國中畢業學年度
        /// </summary>
        public string GraduateSchoolYear{get;set;}
        /// <summary>
        /// 核准日期
        /// </summary>
        public string ADDate{get;set;}
        /// <summary>
        /// 核准文號
        /// </summary>
        public string ADNumber{get;set;}
        
        /// <summary>
        /// 舊科別代碼
        /// </summary>
        public string OldDepartmentCode{get;set;}
        
        /// <summary>
        /// 舊班別
        /// </summary>
        public string OldClassType{get;set;}
   
        /// <summary>
        /// 備查日期
        /// </summary>
        public string LastADDate {get;set;}
        /// <summary>
        /// 備查文號
        /// </summary>
        public string LastADNumber{get;set;}
        /// <summary>
        /// 原就讀學校
        /// </summary>
        public string PreviousSchool{get;set;}
        /// <summary>
        /// 原就讀學號
        /// </summary>
        public string PreviousStudentNumber{get;set;}
        /// <summary>
        /// 原就讀科別
        /// </summary>
        public string PreviousDepartment{get;set;}
    
        /// <summary>
        /// 原就讀年級
        /// </summary>
        public string PreviousGradeYear{get;set;}

        /// <summary>
        /// 原就讀學期
        /// </summary>
        public string PreviousSemester { get; set; }
        
        /// <summary>
        /// 原就讀備查日期
        /// </summary>
        public string PreviousSchoolLastADDate{get;set;}    
        /// <summary>
        /// 原就讀備查文號
        /// </summary>
        public string PreviousSchoolLastADNumber{get;set;}
        /// <summary>
        /// 最後異動代碼
        /// </summary>
        public string LastUpdateCode{get;set;}
        /// <summary>
        /// 畢業證書字號
        /// </summary>
        public string GraduateCertificateNumber{get;set;}    
        
        /// <summary>
        /// 年級
        /// </summary>
        public string GradeYear{get;set;}

        /// <summary>
        /// 新資料
        /// </summary>
        public string NewData { get;set;}

        /// <summary>
        /// 學生 ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 異動紀錄編號
        /// </summary>
        public string URID { get; set; }

        /// <summary>
        /// 學生狀態
        /// </summary>
        public string StudStatus { get; set; }

        /// <summary>
        /// 新學號
        /// </summary>
        public string NewStudentNumber { get; set; }
        /// <summary>
        /// 更正後身分證註記
        /// </summary>
        public string Comment2 { get; set; }

        /// <summary>
        /// 入學資格證明文件
        /// </summary>
        public string GraduateDocument { get; set; }

        /// <summary>
        /// 應畢業學年度
        /// </summary>
        public string ExpectGraduateSchoolYear { get; set; }
        /// <summary>
        /// 借讀學校代碼
        /// </summary>
        public string Code7SchoolCode { get; set; }

        /// <summary>
        /// 借讀科別代碼
        /// </summary>
        public string Code7DeptCode { get; set; }

        /// <summary>
        /// 申請開始日期
        /// </summary>
        public string Code71BeginDate { get; set; }

        /// <summary>
        /// 申請結束日期
        /// </summary>
        public string Code71EndDate { get; set; }

        /// <summary>
        /// 實際開始日期
        /// </summary>
        public string Code72BeginDate { get; set; }

        /// <summary>
        /// 實際結束日期
        /// </summary>
        public string Code72EndDate { get; set; }
    }
}
