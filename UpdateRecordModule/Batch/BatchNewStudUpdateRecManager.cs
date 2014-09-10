using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SHSchool.Data;

namespace UpdateRecordModule_IBSH.Batch
{
    public class BatchNewStudUpdateRecManager
    {
        // 異動原因及事項
        List<string> _UpdateCodeList;
        
        public BatchNewStudUpdateRecManager()
        {
            // 取得新生異動選項
            _UpdateCodeList = new List<string>();

            foreach (XElement elm in DAL.DALTransfer.GetUpdateCodeList().Elements("異動").Where(x => x.Element("分類").Value == "新生異動"))
            {
                // 代號
                // 原因及事項                
                _UpdateCodeList.Add(elm.Element("代號").Value+" "+ elm.Element("原因及事項").Value);
            }

        }

        

        /// <summary>
        /// 取得新生異動選項
        /// </summary>
        /// <returns></returns>
        public List<string> GetNewUpdateCodeList()
        {
            return _UpdateCodeList;
        }

        /// <summary>
        /// 取得班別選項
        /// </summary>
        /// <returns></returns>
        public List<string> GetClassTypeList()
        {
            return DAL.DALTransfer.GetClassTypeList();        
        }

        /// <summary>
        /// 產生新生異動(傳入學生編號、異動日期、異動代碼、班別)
        /// </summary>
        /// <param name="StudentIDList"></param>
        /// <param name="UpdateDate"></param>
        /// <param name="UpdateCode"></param>
        /// <param name="ClassType"></param>
        /// <returns></returns>
        public bool Run(List<string> StudentIDList, string UpdateDate, string UpdateCode,string UpdateDesc, string ClassType)
        {
            bool pass = true;
            List<SHBeforeEnrollmentRecord> befRecList = SHBeforeEnrollment.SelectByStudentIDs(StudentIDList);

            Dictionary<string,DAL.SchoolData> schoolDataDict =new Dictionary<string,DAL.SchoolData> ();

            foreach (XElement elm in BL.Get.JHSchoolList().Elements("學校"))
            { 
                DAL.SchoolData sd = new DAL.SchoolData();
                sd.SchoolCode=elm.Attribute("代碼").Value ;
                sd.SchoolLocation=elm.Attribute("所在地").Value ;
                sd.SchoolName=elm.Attribute("名稱").Value ;
                sd.SchoolLocationCode=elm.Attribute("所在地代碼").Value;
                 if (sd.SchoolCode.Length > 3)                        
                    sd.SchoolType=sd.SchoolCode.Substring(2,1);

                string s1=elm.Attribute("所在地").Value +elm.Attribute("名稱").Value;

                if (!schoolDataDict.ContainsKey(s1))
                    schoolDataDict.Add(s1, sd);

                if (!schoolDataDict.ContainsKey(sd.SchoolName))
                    schoolDataDict.Add(sd.SchoolName, sd);
            
            }

            int SchoolYear=0, Semester=0;
            int.TryParse(K12.Data.School.DefaultSchoolYear, out SchoolYear);
            int.TryParse(K12.Data.School.DefaultSemester, out Semester);

            List<string> _CheckCodeList = (from data in _UpdateCodeList select data.Substring(0, 3)).ToList();

            List<SHUpdateRecordRecord> insertData = new List<SHUpdateRecordRecord>();
            List<SHUpdateRecordRecord> WaitDeleteData = new List<SHUpdateRecordRecord>();

            // 取得學生資料
            List<SHStudentRecord> studRecList = SHStudent.SelectByIDs(StudentIDList);

            List<string> SIDList = new List<string>();

            // 取得已有新生異動
            foreach (SHUpdateRecordRecord rec in SHUpdateRecord.SelectByStudentIDs(StudentIDList))
            {
                if (_CheckCodeList.Contains(rec.UpdateCode))
                {
                    WaitDeleteData.Add(rec);
                    if (!SIDList.Contains(rec.StudentID))
                        SIDList.Add(rec.StudentID);
                }
            }            


            bool checkdelData=true;

            // 已有資料是否覆蓋
            if (SIDList.Count > 0)
            {
                
                WarningForm wf = new WarningForm();
                wf.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                wf.SetStudentCount(SIDList.Count);
                wf.SetStudRec(studRecList);
                wf.SetUpdateRecList(WaitDeleteData);
                if (wf.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    checkdelData = false;
                    pass = false;
                }
            }


            // 清掉舊資料
            if (checkdelData)
            {
                SHUpdateRecord.Delete(WaitDeleteData);
            }

            // 取得新生異動
            foreach (SHStudentRecord studRec in studRecList)
            {
                // 不刪除代表不需要新增，所以略過
                if(checkdelData== false)
                    if (SIDList.Contains(studRec.ID))
                        continue;

                SHUpdateRecordRecord NewUpdateRec = new SHUpdateRecordRecord();

                if (SchoolYear > 0)
                    NewUpdateRec.SchoolYear = SchoolYear;
                
                if (Semester > 0)
                    NewUpdateRec.Semester = Semester;

                if (studRec.Class != null)
                    if (studRec.Class.GradeYear.HasValue)
                        NewUpdateRec.GradeYear = studRec.Class.GradeYear.Value.ToString();

                NewUpdateRec.GraduateComment = "";
                NewUpdateRec.UpdateCode = UpdateCode;
                NewUpdateRec.UpdateDescription = UpdateDesc;
                NewUpdateRec.UpdateDate = UpdateDate;
                NewUpdateRec.ClassType = ClassType;

                NewUpdateRec.SpecialStatus = DAL.DALTransfer.GetSpecialCode(studRec.ID);

                if (studRec.Department != null)
                {
                    NewUpdateRec.Department = studRec.Department.FullName;
                }
                NewUpdateRec.StudentID = studRec.ID;
                NewUpdateRec.StudentName = studRec.Name;
                NewUpdateRec.StudentNumber = studRec.StudentNumber;
                NewUpdateRec.IDNumber = studRec.IDNumber;
                if (studRec.Birthday.HasValue)
                    NewUpdateRec.Birthdate = studRec.Birthday.Value.ToShortDateString();
                NewUpdateRec.Gender = studRec.Gender;

                if (int.Parse(UpdateCode) > 5)
                    NewUpdateRec.GraduateComment = "1";
                

                foreach (SHBeforeEnrollmentRecord brfRec in befRecList)
                {
                    if (brfRec.RefStudentID == NewUpdateRec.StudentID)
                    {
                        NewUpdateRec.GraduateSchool = brfRec.School;
                        NewUpdateRec.GraduateSchoolYear = brfRec.GraduateSchoolYear;

                        // 用學校名稱解析
                        if (!string.IsNullOrEmpty(brfRec.School))
                        {
                            string key = brfRec.SchoolLocation.Replace("台", "臺") + brfRec.School.Trim();
                            if (schoolDataDict.ContainsKey(key))
                            {
                                NewUpdateRec.GraduateSchoolCode = schoolDataDict[key].SchoolCode;
                                NewUpdateRec.GraduateSchoolLocationCode = schoolDataDict[key].SchoolLocationCode;
                            }
                        }
                    }
                
                }
                insertData.Add(NewUpdateRec);
            }

            // 新增異動
            SHUpdateRecord.Insert(insertData);

            return pass;
        }

        ///// <summary>
        ///// 取得新生入學資料代碼
        ///// </summary>
        ///// <returns></returns>
        //public List<string> GetEnrollCodeList()
        //{
        //    return DAL.DALTransfer.GetNewStudCode();
        //}

    }
}
