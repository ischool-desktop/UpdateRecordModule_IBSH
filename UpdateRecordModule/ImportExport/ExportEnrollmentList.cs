using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSchool.API.PlugIn;
using SHSchool.Data;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.ImportExport
{
    /// <summary>
    /// 匯出新生異動
    /// </summary>
    public class ExportEnrollmentList:SmartSchool.API.PlugIn.Export.Exporter 
    {
        /// <summary>
        /// 可匯出項目
        /// </summary>
        List<string> _ExportList;

        public ExportEnrollmentList()
        {
            this.Image = null;
            this.Text = "匯出新生異動";
            _ExportList = new List<string>();
            _ExportList.Add("學年度");
            _ExportList.Add("學期");
            _ExportList.Add("異動年級");
            _ExportList.Add("異動代碼");
            _ExportList.Add("原因及事項");
            _ExportList.Add("異動日期");
            _ExportList.Add("備註");
            _ExportList.Add("班別");
            _ExportList.Add("科別");
            _ExportList.Add("特殊身分代碼");            
            _ExportList.Add("異動姓名");
            _ExportList.Add("異動學號");
            _ExportList.Add("異動身分證字號");
            _ExportList.Add("異動生日");
            _ExportList.Add("異動身分證註記");
            _ExportList.Add("異動性別");
            _ExportList.Add("畢業國中");
            _ExportList.Add("畢業國中所在地代碼");
            _ExportList.Add("畢業國中學校代碼");
            _ExportList.Add("國中畢業學年度");
            _ExportList.Add("核准日期");
            _ExportList.Add("核准文號");            
            _ExportList.Add("入學資格證明文件");
            _ExportList.Add("狀態");
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange(_ExportList);            
            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {
                // 取得異動代碼
                XElement _UpdateCode=DAL.DALTransfer.GetUpdateCodeList();
                List<string> UpdateCodeList = (from elm in _UpdateCode.Elements("異動") where elm.Element("分類").Value == "新生異動" select elm.Element("代號").Value).ToList();
                // 取得學生新生異動
                List<SHUpdateRecordRecord> updateRecList = (from data in SHUpdateRecord.SelectByStudentIDs(e.List) where UpdateCodeList.Contains(data.UpdateCode) select data).ToList();
            Dictionary<string, string> StudentStatusDict = DAL.FDQuery.GetAllStudentStatus1Dict();

            foreach (SHUpdateRecordRecord rec in updateRecList)
            {
                RowData row = new RowData();
                row.ID = rec.StudentID;
                foreach (string field in e.ExportFields)
                {
                    if (wizard.ExportableFields.Contains(field))
                    {
                        switch(field)
                        {
                            case "學年度": 
                                if(rec.SchoolYear.HasValue )
                                    row.Add(field, rec.SchoolYear.Value.ToString ()); break;                               
                            case "學期":
                                if(rec.Semester.HasValue )
                                    row.Add(field, rec.Semester.Value.ToString ()); break;
                            case "年級": row.Add(field, rec.GradeYear); break;
                            case "異動代碼": row.Add(field, rec.UpdateCode); break;
                            case "原因及事項": row.Add(field, rec.UpdateDescription); break;
                            case "異動日期": row.Add(field, rec.UpdateDate); break;
                            case "備註": row.Add(field, rec.Comment); break;
                            case "班別": row.Add(field, rec.ClassType); break;
                            case "科別": row.Add(field, rec.Department); break;
                            case "特殊身分代碼": row.Add(field, rec.SpecialStatus); break;
                            case "入學資格證明文件": row.Add(field, rec.GraduateDocument); break;
                            case "異動姓名": row.Add(field, rec.StudentName); break;
                            case "異動學號": row.Add(field, rec.StudentNumber); break;
                            case "異動身分證字號": row.Add(field, rec.IDNumber); break;
                            case "異動生日": row.Add(field, rec.Birthdate); break;
                            case "異動身分證註記": row.Add(field, rec.IDNumberComment); break;
                            case "異動性別": row.Add(field, rec.Gender); break;
                            case "畢業國中": row.Add(field, rec.GraduateSchool); break;
                            case "畢業國中所在地代碼": row.Add(field, rec.GraduateSchoolLocationCode); break;
                            case "畢業國中學校代碼": row.Add(field, rec.GraduateSchoolCode); break;
                            case "畢業國中畢業學年度": row.Add(field, rec.GraduateSchoolYear); break;
                            case "畢業國中入學資格註記": row.Add(field, rec.GraduateComment); break;
                            case "核准日期": row.Add(field, rec.ADDate); break;
                            case "核准文號": row.Add(field, rec.ADNumber); break;
                            case "狀態": 
                                if(StudentStatusDict.ContainsKey(rec.StudentID))
                                    row.Add(field, StudentStatusDict[rec.StudentID]); break;
                        }                    
                    }                
                }
                e.Items.Add(row);
            }
            };
        }

    }


}
