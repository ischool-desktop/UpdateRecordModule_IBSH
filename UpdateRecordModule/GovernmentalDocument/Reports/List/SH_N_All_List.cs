using System;
using System.Collections.Generic;
using ReportHelper;
using System.Data;
using System.IO;
using Aspose.Cells;
using UpdateRecordModule_IBSH.BL;
using System.Windows.Forms;

namespace UpdateRecordModule_IBSH.GovernmentalDocument.Reports.List
{    
    class SH_N_All_List
    {
        UpdateRecordModule_IBSH.BL.StudUpdateRecBatchRec _SelectBRec;

        public SH_N_All_List(UpdateRecordModule_IBSH.BL.StudUpdateRecBatchRec SelectBRec)
        {

            _SelectBRec = SelectBRec;
            if (_SelectBRec == null)
                return;
            
            // 取得國中所在地代碼與名稱對照
            Dictionary<string, string> SchoolLocationCodeNameDict = Util.GetJHSchooLocationNameDict();

            // 讀取異動名冊內學生異動資料
            Dictionary<string, List<UpdateRecordModule_IBSH.BL.StudUpdateRecDoc>> UpdateBook = new Dictionary<string, List<UpdateRecordModule_IBSH.BL.StudUpdateRecDoc>>();
            foreach (UpdateRecordModule_IBSH.BL.StudUpdateRecDoc rec in _SelectBRec.StudUpdateRecDocList)
            {
                if (UpdateBook.ContainsKey(rec.DeptCode + "_" + rec.GradeYear))
                    UpdateBook[rec.DeptCode + "_" + rec.GradeYear].Add(rec);
                else
                {
                    UpdateBook.Add(rec.DeptCode + "_" + rec.GradeYear, new List<UpdateRecordModule_IBSH.BL.StudUpdateRecDoc>());
                    UpdateBook[rec.DeptCode + "_" + rec.GradeYear].Add(rec);
                }
            }

            System.Data.DataSet UpdateBookRecord = new DataSet("DataSection");
            Dictionary<string, List<DataSet>> UpdateBookSet = new Dictionary<string, List<DataSet>>();
            UpdateBookSet.Add("名冊", new List<DataSet>());
            foreach (string DeptCode_Year in UpdateBook.Keys)
            {
                int rowCount = 1;
                int TotalPeople = 0;
                DataTable dataTable = new DataTable();
                DataRow row = dataTable.NewRow();
                foreach (UpdateRecordModule_IBSH.BL.StudUpdateRecDoc rec in UpdateBook[DeptCode_Year])
                {
                    //名冊內容
                    //表頭
                    if (rowCount == 1)
                    {
                        // 校名
                        dataTable = new DataTable("校名");
                        dataTable.Columns.Add("校名");

                        row = dataTable.NewRow();
                        row["校名"] = _SelectBRec.SchoolName;
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                        //學年度          
                        dataTable = new DataTable("學年度");
                        dataTable.Columns.Add("學年度");

                        row = dataTable.NewRow();
                        row["學年度"] = _SelectBRec.SchoolYear;
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                        //學年期          
                        dataTable = new DataTable("學期");
                        dataTable.Columns.Add("學期");

                        row = dataTable.NewRow();
                        row["學期"] = _SelectBRec.Semester;
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                        //代碼          
                        dataTable = new DataTable("代碼");
                        dataTable.Columns.Add("代碼");

                        row = dataTable.NewRow();
                        row["代碼"] = _SelectBRec.SchoolCode + "-" + UpdateBook[DeptCode_Year][0].DeptCode;
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                        if (_SelectBRec.UpdateType == "學籍異動名冊" || _SelectBRec.UpdateType == "轉入學生名冊")
                        {
                            //年級         
                            dataTable = new DataTable("年級");
                            dataTable.Columns.Add("年級");

                            row = dataTable.NewRow();
                            row["年級"] = UpdateBook[DeptCode_Year][0].GradeYear;
                            dataTable.Rows.Add(row);
                            UpdateBookRecord.Tables.Add(dataTable);
                        }
                        //科別         
                        dataTable = new DataTable("科別");
                        dataTable.Columns.Add("科別");

                        row = dataTable.NewRow();
                        row["科別"] = UpdateBook[DeptCode_Year][0].Department;    //rec.Department; // 
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                        dataTable = new DataTable("名冊內容");
                        UpdateBookRecord.Tables.Add(dataTable);
                        switch (_SelectBRec.UpdateType)
                        {
                            case "學籍異動名冊":
                                dataTable.Columns.Add("原學號");
                                dataTable.Columns.Add("姓名");
                                dataTable.Columns.Add("身份證字號");
                                dataTable.Columns.Add("學籍異動年月文號");
                                dataTable.Columns.Add("代號");
                                dataTable.Columns.Add("原因或事項");
                                dataTable.Columns.Add("異動日期");
                                dataTable.Columns.Add("備註");
                                break;
                            case "畢業名冊":
                                dataTable.Columns.Add("學號姓名");
                                dataTable.Columns.Add("性別1");
                                dataTable.Columns.Add("性别2");
                                dataTable.Columns.Add("出生年月日身份證字號");
                                dataTable.Columns.Add("代號");
                                dataTable.Columns.Add("學籍異動年月文號");
                                dataTable.Columns.Add("畢業證書字號");
                                dataTable.Columns.Add("備註");
                                break;
                            case "新生名冊":
                                dataTable.Columns.Add("學號");
                                dataTable.Columns.Add("姓名");
                                dataTable.Columns.Add("身份證字號");
                                dataTable.Columns.Add("性別1");
                                dataTable.Columns.Add("性别2");
                                dataTable.Columns.Add("出年年月日");
                                dataTable.Columns.Add("入學資格代碼");
                                dataTable.Columns.Add("入學資格");
                                dataTable.Columns.Add("備註");
                                break;
                            case "轉入學生名冊":
                                dataTable.Columns.Add("新學號姓名");
                                dataTable.Columns.Add("身份證字號");
                                dataTable.Columns.Add("性別1");
                                dataTable.Columns.Add("性別2");
                                dataTable.Columns.Add("出年年月日");
                                dataTable.Columns.Add("學校");
                                dataTable.Columns.Add("科別代號");
                                dataTable.Columns.Add("學籍異動");
                                dataTable.Columns.Add("年級");
                                dataTable.Columns.Add("代號");
                                dataTable.Columns.Add("原因");
                                dataTable.Columns.Add("年月日");
                                break;

                        }
                    }
                    //DataTable dataTable = new DataTable();
                    switch (_SelectBRec.UpdateType)
                    {
                        case "學籍異動名冊":
                            row = dataTable.NewRow();
                            row["原學號"] = rec.StudentNumber;
                            row["姓名"] = rec.StudentName;
                            row["身份證字號"] = rec.IDNumber;
                            row["學籍異動年月文號"] = Util.ConvertDateStr2(rec.LastADDate) + "\n" + rec.LastADNumber;
                            row["代號"] = rec.UpdateCode;
                            row["原因或事項"] = rec.UpdateDescription;
                            row["異動日期"] = Util.ConvertDateStr2(rec.UpdateDate);
                            row["備註"] = rec.Comment;
                            UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                            break;
                        case "畢業名冊":
                            row = dataTable.NewRow();
                            row["學號姓名"] = rec.StudentNumber + "\n" + rec.StudentName;
                            row["性別1"] = rec.GenderCode;
                            row["性别2"] = rec.Gender;
                            row["出生年月日身份證字號"] = Util.ConvertDateStr2(rec.Birthday) + "\n" + rec.IDNumber;
                            row["代號"] = rec.LastUpdateCode;
                            row["學籍異動年月文號"] = Util.ConvertDateStr2(rec.LastADDate) + "\n" + rec.LastADNumber;
                            row["畢業證書字號"] = rec.GraduateCertificateNumber;
                            row["備註"] = rec.Comment;
                            UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                            break;
                        case "新生名冊":
                            row = dataTable.NewRow();
                            row["學號"] = rec.StudentNumber;
                            row["姓名"] = rec.StudentName;
                            row["身份證字號"] = rec.IDNumber;
                            row["性別1"] = rec.GenderCode;
                            row["性别2"] = rec.Gender;
                            row["出年年月日"] = Util.ConvertDateStr2(rec.Birthday);
                            row["入學資格代碼"] =rec.GraduateSchoolLocationCode+"\n"+rec.UpdateCode;

                            string strGradeType = " 畢";
                            if (rec.UpdateCode == "003")
                                strGradeType = " 修";

                            if (rec.UpdateCode == "004")
                                strGradeType = " 結";

                            if (string.IsNullOrEmpty(rec.GraduateSchool))
                                strGradeType = "";

                            if (SchoolLocationCodeNameDict.ContainsKey(rec.GraduateSchoolLocationCode))
                                row["入學資格"] = SchoolLocationCodeNameDict[rec.GraduateSchoolLocationCode] + rec.GraduateSchool + strGradeType + "\n";
                            else
                                row["入學資格"] = rec.GraduateSchool + strGradeType + "\n";
                            row["備註"] = rec.Comment; ;
                            UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                            break;
                        case "轉入學生名冊":
                            row = dataTable.NewRow();
                            row["新學號姓名"] = rec.StudentNumber + "\n" + rec.StudentName;
                            row["身份證字號"] = rec.IDNumber;
                            row["性別1"] = rec.GenderCode;
                            row["性別2"] = rec.Gender;
                            row["出年年月日"] = Util.ConvertDateStr2(rec.Birthday);
                            row["學校"] = rec.PreviousSchool;
                            row["科別代號"] = rec.OldDepartmentCode + "\n" + rec.PreviousDepartment;
                            row["學籍異動"] = Util.ConvertDateStr2(rec.LastADDate) + "\n" + rec.LastADNumber;
                            string strSem = "";
                            if (!string.IsNullOrEmpty(rec.PreviousSemester))
                            {
                                int i;
                                if (int.TryParse(rec.PreviousSemester, out i))
                                {
                                    if (i == 1)
                                        strSem = "上";
                                    else
                                        strSem = "下";
                                }
                                else
                                    strSem = rec.PreviousSemester;                            
                            }
                            row["年級"] = rec.PreviousGradeYear+"\n"+strSem;
                            row["代號"] = rec.UpdateCode;
                            row["原因"] = rec.UpdateDescription;
                            row["年月日"] = Util.ConvertDateStr2(rec.UpdateDate);
                            UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                            break;

                    }
                    TotalPeople++;
                    rowCount++;
                    if (rowCount > 16)
                    {
                        UpdateBookSet["名冊"].Add(UpdateBookRecord);
                        UpdateBookRecord = new DataSet("DataSection");
                        rowCount = 1;
                    }
                }
                ////名冊內容
                ////表頭
                if (rowCount == 1)
                {
                    // 校名
                    dataTable = new DataTable("校名");
                    dataTable.Columns.Add("校名");

                    row = dataTable.NewRow();
                    row["校名"] = _SelectBRec.SchoolName;
                    dataTable.Rows.Add(row);
                    UpdateBookRecord.Tables.Add(dataTable);
                    //學年度          
                    dataTable = new DataTable("學年度");
                    dataTable.Columns.Add("學年度");

                    row = dataTable.NewRow();
                    row["學年度"] = _SelectBRec.SchoolYear;
                    dataTable.Rows.Add(row);
                    UpdateBookRecord.Tables.Add(dataTable);
                    //學年期          
                    dataTable = new DataTable("學期");
                    dataTable.Columns.Add("學期");

                    row = dataTable.NewRow();
                    row["學期"] = _SelectBRec.Semester;
                    dataTable.Rows.Add(row);
                    UpdateBookRecord.Tables.Add(dataTable);
                    //代碼          
                    dataTable = new DataTable("代碼");
                    dataTable.Columns.Add("代碼");

                    row = dataTable.NewRow();
                    row["代碼"] = _SelectBRec.SchoolCode + "-" + UpdateBook[DeptCode_Year][0].DeptCode;
                    dataTable.Rows.Add(row);
                    UpdateBookRecord.Tables.Add(dataTable);
                    if (_SelectBRec.UpdateType == "學籍異動名冊" || _SelectBRec.UpdateType == "轉入學生名冊")
                    {
                        //年級         
                        dataTable = new DataTable("年級");
                        dataTable.Columns.Add("年級");

                        row = dataTable.NewRow();
                        row["年級"] = UpdateBook[DeptCode_Year][0].GradeYear;
                        dataTable.Rows.Add(row);
                        UpdateBookRecord.Tables.Add(dataTable);
                    }
                    //科別         
                    dataTable = new DataTable("科別");
                    dataTable.Columns.Add("科別");

                    row = dataTable.NewRow();
                    row["科別"] = UpdateBook[DeptCode_Year][0].Department;
                    dataTable.Rows.Add(row);
                    UpdateBookRecord.Tables.Add(dataTable);
                    dataTable = new DataTable("名冊內容");
                    UpdateBookRecord.Tables.Add(dataTable);
                    switch (_SelectBRec.UpdateType)
                    {
                        case "學籍異動名冊":
                            dataTable.Columns.Add("原學號");
                            dataTable.Columns.Add("姓名");
                            dataTable.Columns.Add("身份證字號");
                            dataTable.Columns.Add("學籍異動年月文號");
                            dataTable.Columns.Add("代號");
                            dataTable.Columns.Add("原因或事項");
                            dataTable.Columns.Add("異動日期");
                            dataTable.Columns.Add("備註");
                            break;
                        case "畢業名冊":
                            dataTable.Columns.Add("學號姓名");
                            dataTable.Columns.Add("性別1");
                            dataTable.Columns.Add("性别2");
                            dataTable.Columns.Add("出生年月日身份證字號");
                            dataTable.Columns.Add("學籍異動年月文號");
                            dataTable.Columns.Add("代號");
                            dataTable.Columns.Add("畢業證書字號");
                            dataTable.Columns.Add("備註");
                            break;
                        case "新生名冊":
                            dataTable.Columns.Add("學號");
                            dataTable.Columns.Add("姓名");
                            dataTable.Columns.Add("身份證字號");
                            dataTable.Columns.Add("性別1");
                            dataTable.Columns.Add("性别2");
                            dataTable.Columns.Add("出年年月日");
                            dataTable.Columns.Add("入學資格代碼");
                            dataTable.Columns.Add("入學資格");
                            dataTable.Columns.Add("備註");
                            break;
                        case "轉入學生名冊":
                            dataTable.Columns.Add("新學號姓名");
                            dataTable.Columns.Add("身份證字號");
                            dataTable.Columns.Add("性別1");
                            dataTable.Columns.Add("性別2");
                            dataTable.Columns.Add("學校");
                            dataTable.Columns.Add("科別代號");
                            dataTable.Columns.Add("學籍異動");
                            dataTable.Columns.Add("年級");
                            dataTable.Columns.Add("代號");
                            dataTable.Columns.Add("原因");
                            dataTable.Columns.Add("年月日");
                            break;

                    }
                }
                switch (_SelectBRec.UpdateType)
                {
                    case "學籍異動名冊":
                        row = dataTable.NewRow();
                        row["原學號"] = "合計";
                        row["姓名"] = TotalPeople + "名";
                        UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                        break;
                    case "畢業名冊":
                        row = dataTable.NewRow();
                        row["學號姓名"] = "合計";
                        row["性別1"] = TotalPeople + "名";
                        UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                        break;
                    case "新生名冊":
                        row = dataTable.NewRow();
                        row["學號"] = "合計";
                        row["姓名"] = TotalPeople + "名";
                        UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                        break;
                    case "轉入學生名冊":
                        row = dataTable.NewRow();
                        row["新學號姓名"] = "合計";
                        row["身份證字號"] = TotalPeople + "名";
                        UpdateBookRecord.Tables["名冊內容"].Rows.Add(row);
                        break;

                }
                UpdateBookSet["名冊"].Add(UpdateBookRecord);
                UpdateBookRecord = new DataSet("DataSection");
            }
            
        }


        /// <summary>
        /// 處理存檔
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="name"></param>
        private void ReportSaveExcel(Workbook wb, string name)
        {
            try
            {
                string filename = Application.StartupPath + "\\Reports\\" + name + ".xls";
                wb.Save(filename, FileFormatType.Excel2003);
                System.Diagnostics.Process.Start(filename);
            }
            catch(Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("名冊無法存檔," + ex.Message);            
            }
        }
    }
}
