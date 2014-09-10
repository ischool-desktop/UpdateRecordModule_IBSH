using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Aspose.Cells;
using System.Linq;
using UpdateRecordModule_IBSH.BL;

namespace UpdateRecordModule_IBSH.GovernmentalDocument.Reports.List
{
    public class StudentUpdateRecordList : ReportBuilder
    {
        List<string> UpdateDataCodes = new List<string>() {"401","402","403","404","405","407","408","409","499"};
        //List<string> NewStudentNumberCodes = new List<string>() { "211", "221", "222", "223", "224", "231", "232", "233", "234", "401" };
        List<string> NewStudentNumberCodes = new List<string>() { "211", "212", "221", "222", "223", "224", "225", "226", "231", "232", "233", "234", "237", "238", "239", "240", "241" };
        protected override void Build(System.Xml.XmlElement source, string location)
        {
            #region 建立 Excel

            //從 Resources 將學籍異動名冊template讀出來
            Workbook template = new Workbook();
            template.Open(new MemoryStream(Properties.Resources.StudentUpdateRecordListTemplate), FileFormatType.Excel2003);

            //產生 excel
            Workbook wb = new Aspose.Cells.Workbook();
            wb.Open(new MemoryStream(Properties.Resources.StudentUpdateRecordListTemplate), FileFormatType.Excel2003);

            #endregion

            #region 複製樣式-預設樣式、欄寬

            //設定預設樣式
            wb.DefaultStyle = template.DefaultStyle;

            //複製樣版中前18個 Column(欄寬)
            for (int m = 0; m < 18; m++)
            {
                /*
                 * 複製 template的第一個 Sheet的第 m個 Column
                 * 到 wb的第一個 Sheet的第 m個 Column
                 */
                wb.Worksheets[0].Cells.CopyColumn(template.Worksheets[0].Cells, m, m);
            }

            #endregion

            #region 初始變數

            /****************************** 
                * rowi 填入學校資料用
                * rowj 填入學生資料用
                * num 計算清單份數
                * numcount 計算每份清單頁數
                * j 計算所產生清單頁數
                * x 判斷個數是否為20被數用
                ******************************/
            int rowi = 0, rowj = 1, num = source.SelectNodes("清單").Count, numcount = 1, j = 0;
            bool x = false;

            int recCount = 0;
            int totalRec = source.SelectNodes("清單/異動紀錄").Count;

            #endregion

            foreach (XmlNode list in source.SelectNodes("清單"))
            {
                int i = 0;

                #region 找出資料總數及判斷

                //找出資料總數方便評估進度
                int count = list.SelectNodes("異動紀錄").Count;

                //判斷個數是否為20被數
                if (count % 20 == 0)
                {
                    x = true;
                }

                #endregion

                #region 異動紀錄

                //將xml資料填入至excel
                foreach (XmlElement st in list.SelectNodes("異動紀錄"))
                {
                    recCount++;
                    if (i % 20 == 0)
                    {
                        #region 複製樣式-欄高、範圍

                        //複製樣版中前287個 Row(欄高)
                        //for (int m = 0; m < 28; m++)
                        //{
                        //    /*
                        //     * 複製 template的第一個 Sheet的第m個 Row
                        //     * 到 wb的第一個 Sheet的第(j * 28) + m個 Row
                        //     */
                        //    wb.Worksheets[0].Cells.CopyRow(template.Worksheets[0].Cells, m, (j * 28) + m);
                        //}

                        /*
                         * 複製Style(包含儲存格合併的資訊)
                         * 先用CreateRange()選取要複製的Range("A1", "R28")
                         * 再用CopyStyle複製另一個Range中的格式
                         */
                        Range range = template.Worksheets[0].Cells.CreateRange(0, 28, false);
                        int t = j * 28;
                        wb.Worksheets[0].Cells.CreateRange(t, 28, false).Copy(range);

                        #endregion

                        #region 填入學校資料

                        //將學校資料填入適當的位置內
                        wb.Worksheets[0].Cells[rowi, 13].PutValue(source.SelectSingleNode("@學校代號").InnerText);
                        wb.Worksheets[0].Cells[rowi, 16].PutValue(list.SelectSingleNode("@科別代號").InnerText);
                        wb.Worksheets[0].Cells[rowi + 2, 2].PutValue(source.SelectSingleNode("@學校名稱").InnerText);
                        wb.Worksheets[0].Cells[rowi + 2, 7].PutValue(Convert.ToInt32(source.SelectSingleNode("@學年度").InnerText) + " 學年度 第 " + Convert.ToInt32(source.SelectSingleNode("@學期").InnerText) + " 學期");
                        wb.Worksheets[0].Cells[rowi + 2, 12].PutValue(list.SelectSingleNode("@科別").InnerText);
                        wb.Worksheets[0].Cells[rowi + 2, 14].PutValue(list.SelectSingleNode("@年級").InnerText);

                        #endregion

                        if (j > 0)
                        {
                            //插入分頁(在 j * 28 跟 (j * 28) +1 中間，R跟S中間)
                            wb.Worksheets[0].HPageBreaks.Add(j * 28, 18);
                            rowj += 8;
                        }
                        else
                        {
                            rowj = 6;
                        }

                        rowi += 28;
                        j++;

                        #region 顯示頁數

                        //顯示頁數
                        if (x != true)
                        {
                            wb.Worksheets[0].Cells[(28 * (j - 1)) + 27, 13].PutValue("第" + numcount + "頁，共" + Math.Ceiling((double)count / 20) + "頁");
                        }
                        else
                        {
                            wb.Worksheets[0].Cells[(28 * (j - 1)) + 27, 13].PutValue("第" + numcount + "頁，共" + (Math.Ceiling((double)count / 20) + 1) + "頁");
                        }
                        numcount++;

                        #endregion
                    }

                    #region 填入學生資料

                    string updatecode = st.SelectSingleNode("@異動代號").InnerText;

                    ////將學生資料填入適當的位置內
                    //if (NewStudentNumberCodes.Contains(updatecode))
                    //{

                    //    string strNum = "";
                    //    if (!string.IsNullOrEmpty(st.SelectSingleNode("@新學號").InnerText))
                    //        strNum = st.SelectSingleNode("@新學號").InnerText;
                    //    else
                    //    {
                    //        string sid = "";
                    //        if (!string.IsNullOrEmpty(st.SelectSingleNode("@學生編號").InnerText))
                    //            sid = st.SelectSingleNode("@學生編號").InnerText;
                    //        List<string> ids = new List<string>();
                    //        ids.Add(sid);
                    //        SHSchool.Data.SHStudent.RemoveByIDs(ids);

                    //        SHSchool.Data.SHStudentRecord stud = SHSchool.Data.SHStudent.SelectByID(sid);
                    //        strNum = stud.StudentNumber;
                    //    }
                    //    wb.Worksheets[0].Cells[rowj, 1].PutValue(strNum);
                    //}
                    //else
                        wb.Worksheets[0].Cells[rowj, 1].PutValue(st.SelectSingleNode("@學號").InnerText);

                    wb.Worksheets[0].Cells[rowj, 3].PutValue(st.SelectSingleNode("@姓名").InnerText);
                    wb.Worksheets[0].Cells[rowj, 4].PutValue(st.SelectSingleNode("@身分證號").InnerText);
                    wb.Worksheets[0].Cells[rowj, 8].PutValue(Util.ConvertDateStr2(st.SelectSingleNode("@備查日期").InnerText) + "\n" + st.SelectSingleNode("@備查文號").InnerText);
                    wb.Worksheets[0].Cells[rowj, 11].PutValue(st.SelectSingleNode("@異動代號").InnerText);

                    //wb.Worksheets[0].Cells[rowj, 12].PutValue(st.SelectSingleNode("@原因及事項").InnerText + (string.IsNullOrEmpty(st.GetAttribute("更正後資料")) ? "" : "\n" + st.GetAttribute("更正後資料")));

                    string UpdateData = "";
                    if (st.SelectSingleNode("@新資料")!=null)
                    {
                        // 更正學號填到另一格
                        if (updatecode != "401")
                            UpdateData = st.SelectSingleNode("@新資料").InnerText;
                    }

                    wb.Worksheets[0].Cells[rowj, 12].PutValue(st.SelectSingleNode("@原因及事項").InnerText+"\n"+UpdateData);

                    string strUpdateDate = Util.ConvertDateStr2(st.SelectSingleNode("@異動日期").InnerText);


                        //假設有異動學生學號的類別才出現新學號字樣
                        if (st.SelectSingleNode("@新學號")!=null)
                            if (!string.IsNullOrEmpty(st.SelectSingleNode("@新學號").InnerText))
                            {
                                int newNo;
                                if (int.TryParse(st.SelectSingleNode("@新學號").InnerText, out newNo))
                                    strUpdateDate = newNo + "\n" + strUpdateDate;
                            }
                            else
                            {
                                // 更正學號
                                if (updatecode == "401")
                                {
                                    if (st.SelectSingleNode("@新資料") != null)
                                        if (!string.IsNullOrEmpty(st.SelectSingleNode("@新資料").InnerText))
                                            strUpdateDate = st.SelectSingleNode("@新資料").InnerText + "\n" + strUpdateDate;
                                }
                            }
                    
                    wb.Worksheets[0].Cells[rowj, 13].PutValue(strUpdateDate);
                    if(st.SelectSingleNode("@特殊身份代碼") !=null )
                        wb.Worksheets[0].Cells[rowj, 16].PutValue(st.SelectSingleNode("@特殊身份代碼").InnerText);
                    //wb.Worksheets[0].Cells[rowj, 16].PutValue(st.SelectSingleNode("@備註").InnerText);

                    #endregion

                    i++;
                    rowj++;

                    //回報進度
                    ReportProgress((int)(((double)recCount * 100.0) / ((double)totalRec)));
                }

                #endregion

                #region 若個數為20倍數，處理單一頁面

                if (x == true)
                {

                    #region 複製樣式-欄高、範圍

                    //複製樣版前28個 Row(欄高)
                    //for (int m = 0; m < 28; m++)
                    //{
                    //    /*
                    //     * 複製 template的第一個 Sheet的第m個 Row
                    //     * 到 wb的第一個 Sheet的第(j * 28) + m個 Row
                    //     */
                    //    wb.Worksheets[0].Cells.CopyRow(template.Worksheets[0].Cells, m, (j * 28) + m);
                    //}

                    /*
                     * 複製Style(包含儲存格合併的資訊)
                     * 先用CreateRange()選取要複製的Range("A1", "R28")
                     * 再用CopyStyle複製另一個Range中的格式
                     */
                    Range range = template.Worksheets[0].Cells.CreateRange(0, 28, false);
                    int t = j * 28;
                    wb.Worksheets[0].Cells.CreateRange(t, 28, false).Copy(range);

                    #endregion

                    #region 填入學校資料

                    //將學校資料填入適當的位置內
                    wb.Worksheets[0].Cells[rowi, 13].PutValue(source.SelectSingleNode("@學校代號").InnerText);
                    wb.Worksheets[0].Cells[rowi, 16].PutValue(list.SelectSingleNode("@科別代號").InnerText);
                    wb.Worksheets[0].Cells[rowi + 2, 2].PutValue(source.SelectSingleNode("@學校名稱").InnerText);
                    wb.Worksheets[0].Cells[rowi + 2, 6].PutValue(Convert.ToInt32(source.SelectSingleNode("@學年度").InnerText));
                    wb.Worksheets[0].Cells[rowi + 2, 9].PutValue(Convert.ToInt32(source.SelectSingleNode("@學期").InnerText));
                    wb.Worksheets[0].Cells[rowi + 2, 12].PutValue(list.SelectSingleNode("@科別").InnerText);

                    #endregion

                    if (j > 0)
                    {
                        //插入分頁(在i跟i+1中間，O跟P中間)
                        wb.Worksheets[0].HPageBreaks.Add(j * 28, 18);
                        rowj += 8;
                    }

                    rowi += 28;
                    j++;

                    #region 顯示頁數

                    //顯示頁數
                    wb.Worksheets[0].Cells[(28 * (j - 1)) + 27, 13].PutValue("第" + numcount + "頁，共" + (Math.Ceiling((double)count / 20) + 1) + "頁");
                    numcount++;

                    #endregion
                }

                #endregion

                #region 統計人數

                //填入統計人數
                wb.Worksheets[0].Cells.CreateRange(rowj, 1, 1, 2).UnMerge();
                wb.Worksheets[0].Cells.Merge(rowj, 1, 1, 3);
                wb.Worksheets[0].Cells[rowj, 1].PutValue("合  計 " + count.ToString() + " 名");

                #endregion

                wb.Worksheets[0].HPageBreaks.Add(j * 28, 18);

                #region 設定變數

                //調整新清單所使用變數
                numcount = 1;
                rowj = (28 * j) - 2;
                rowi = (28 * j);
                x = false;

                #endregion
            }

            // 因2010年格式不同小修改

            #region 學籍異動電子格式
            //範本
            Worksheet TemplateWb = wb.Worksheets["電子格式範本"];
            //實做頁面
            Worksheet DyWb = wb.Worksheets[wb.Worksheets.Add()];
            //名稱
            DyWb.Name = "電子格式";
            //範圍
            Range range_H = TemplateWb.Cells.CreateRange(0, 1, false);
            Range range_R = TemplateWb.Cells.CreateRange(1, 1, false);
            //拷貝range_H
            DyWb.Cells.CreateRange(0, 1, false).Copy(range_H);

            int DyWb_index = 0;
            // 遇到特殊異動代碼要處理
            List<string> spcCode = new List<string>();
            spcCode.Add("211");

            DAL.DALTransfer DALTranser = new DAL.DALTransfer();

            // 格式轉換
            List<GovernmentalDocument.Reports.List.rpt_UpdateRecord> _data = DALTranser.ConvertRptUpdateRecord(source);

            // 排序 (依 班別、年級、科別代碼、異動代碼)
            _data = (from data in _data orderby data.ClassType, GYear(data.GradeYear), data.DeptCode, data.UpdateCode select data).ToList();
            
            foreach(GovernmentalDocument.Reports.List.rpt_UpdateRecord rec in _data )
            {
                DyWb_index++;
                //每增加一行,複製一次
                DyWb.Cells.CreateRange(DyWb_index, 1, false).Copy(range_R);

                //班別
                DyWb.Cells[DyWb_index, 0].PutValue(rec.ClassType);
                //科別代碼
                DyWb.Cells[DyWb_index, 1].PutValue(rec.DeptCode);

                // 2 放上傳類別，請使用者自填 

                //學號
                DyWb.Cells[DyWb_index, 3].PutValue(rec.StudentNumber);
                //姓名
                DyWb.Cells[DyWb_index, 4].PutValue(rec.Name);
                //身分證字號
                DyWb.Cells[DyWb_index, 5].PutValue(rec.IDNumber);

                //註1
                DyWb.Cells[DyWb_index, 6].PutValue(rec.Comment1);

                //性別代碼
                DyWb.Cells[DyWb_index, 7].PutValue(rec.GenderCode);
                //出生日期
                DyWb.Cells[DyWb_index, 8].PutValue(rec.Birthday);

                //特殊身份代碼
                DyWb.Cells[DyWb_index, 9].PutValue(rec.SpecialStatusCode);
                //年級
                DyWb.Cells[DyWb_index, 10].PutValue(rec.GradeYear);
                //異動原因代碼
                DyWb.Cells[DyWb_index, 11].PutValue(rec.UpdateCode);
                //異動日期
                DyWb.Cells[DyWb_index, 12].PutValue(rec.UpdateDate);

                // 異動順序
                DyWb.Cells[DyWb_index, 13].PutValue(rec.Order);

                //備查日期
                DyWb.Cells[DyWb_index, 14].PutValue(rec.LastADDate);
                //備查文字
                DyWb.Cells[DyWb_index, 15].PutValue(rec.LastADDoc);
                //備查文號
                DyWb.Cells[DyWb_index, 16].PutValue(rec.LastADNum);


                //更正後資料
                string strUpdateData = string.Empty;

                //若是更正後資料有值則填入更正後資料
                if (!string.IsNullOrEmpty(rec.NewData))
                    strUpdateData = rec.NewData;

                //若是新學號中有值則填入新學號
                //判斷strUpdateData是否已有值，若是已有值則加入斷行符號
                if (!string.IsNullOrEmpty(rec.NewStudNumber))
                    strUpdateData += string.IsNullOrEmpty(strUpdateData) ? rec.NewStudNumber : "\n" + rec.NewStudNumber;

                DyWb.Cells[DyWb_index, 17].PutValue(strUpdateData);

                // 註2
                DyWb.Cells[DyWb_index, 18].PutValue(rec.Comment2);

                //備註說明
                DyWb.Cells[DyWb_index, 19].PutValue(rec.Comment);

                // 2011 新承辦單位修正，轉科讀取新學號
                if (NewStudentNumberCodes.Contains(rec.UpdateCode))
                {
                    List<string> ids = new List<string>();
                    ids.Add(rec.StudentID);
                    SHSchool.Data.SHStudent.RemoveByIDs(ids);
                    SHSchool.Data.SHStudentRecord studRec = SHSchool.Data.SHStudent.SelectByID(rec.StudentID);
                    if(studRec !=null)
                        DyWb.Cells[DyWb_index, 3].PutValue(studRec.StudentNumber);
                    DyWb.Cells[DyWb_index, 17].PutValue("");
                }
            }


            //foreach (XmlElement Record in source.SelectNodes("清單/異動紀錄"))
            //{
            //    DyWb_index++;
            //    //每增加一行,複製一次
            //    DyWb.Cells.CreateRange(DyWb_index, 1, false).Copy(range_R);

            //    //班別
            //    DyWb.Cells[DyWb_index, 0].PutValue(Record.GetAttribute("班別"));
            //    //科別代碼
            //    DyWb.Cells[DyWb_index, 1].PutValue((Record.ParentNode as XmlElement).GetAttribute("科別代號"));

            //    // 2 放上傳類別，請使用者自填 

            //    //學號
            //    DyWb.Cells[DyWb_index, 3].PutValue(Record.GetAttribute("學號"));
            //    //姓名
            //    DyWb.Cells[DyWb_index, 4].PutValue(Record.GetAttribute("姓名"));
            //    //身分證字號
            //    DyWb.Cells[DyWb_index, 5].PutValue(Record.GetAttribute("身分證號"));

            //    //註1
            //    DyWb.Cells[DyWb_index, 6].PutValue(Record.GetAttribute("註1"));

            //    //性別代碼
            //    DyWb.Cells[DyWb_index, 7].PutValue(Record.GetAttribute("性別代號"));
            //    //出生日期
            //    DyWb.Cells[DyWb_index, 8].PutValue(GetBirthdateWithoutSlash(Record.GetAttribute("出生年月日")));

            //    //特殊身份代碼
            //    DyWb.Cells[DyWb_index, 9].PutValue(Record.GetAttribute("特殊身份代碼")); //原為抓取備註欄位
            //    //年級
            //    DyWb.Cells[DyWb_index, 10].PutValue((Record.ParentNode as XmlElement).GetAttribute("年級"));
            //    //異動原因代碼
            //    DyWb.Cells[DyWb_index, 11].PutValue(Record.GetAttribute("異動代號"));
            //    //異動日期
            //    DyWb.Cells[DyWb_index, 12].PutValue(GetBirthdateWithoutSlash(Record.GetAttribute("異動日期")));
            //    //原備查日期
            //    DyWb.Cells[DyWb_index, 13].PutValue(GetBirthdateWithoutSlash(Record.GetAttribute("備查日期")));
            //    //原備查文字
            //    DyWb.Cells[DyWb_index, 14].PutValue(GetNumAndSrt1(Record.GetAttribute("備查文號")));
            //    //原備查文號
            //    DyWb.Cells[DyWb_index, 15].PutValue(GetNumAndSrt2(Record.GetAttribute("備查文號")));

            //    // 捨棄
            //    ////舊班別
            //    //DyWb.Cells[DyWb_index, 15].PutValue(Record.GetAttribute("舊班別"));
            //    ////舊科別代碼
            //    //DyWb.Cells[DyWb_index, 16].PutValue(Record.GetAttribute("舊科別代碼"));
                
            //    //更正後資料
            //    string strUpdateData = string.Empty;

            //    //若是更正後資料有值則填入更正後資料
            //    if (!string.IsNullOrEmpty(Record.GetAttribute("更正後資料")))
            //        strUpdateData = Record.GetAttribute("更正後資料");

            //    //若是新學號中有值則填入新學號
            //    //判斷strUpdateData是否已有值，若是已有值則加入斷行符號
            //    if (!string.IsNullOrEmpty(Record.GetAttribute("新學號")))
            //        strUpdateData += string.IsNullOrEmpty(strUpdateData) ? Record.GetAttribute("新學號") : "\n" + Record.GetAttribute("新學號");

            //    DyWb.Cells[DyWb_index, 16].PutValue(strUpdateData);
            //    //備註說明
            //    DyWb.Cells[DyWb_index, 17].PutValue(Record.GetAttribute("備註"));

            //    // 2011 新承辦單位修正
            //    if(spcCode.Contains(Record.GetAttribute("異動代號")))
            //    {
            //        DyWb.Cells[DyWb_index, 3].PutValue(Record.GetAttribute("新學號"));
            //        DyWb.Cells[DyWb_index, 16].PutValue("");
            //    }
            //}


            DyWb.AutoFitColumns();

            wb.Worksheets.RemoveAt("電子格式範本");

            #endregion

            wb.Worksheets.ActiveSheetIndex = 0;
            //儲存 Excel
            wb.Save(location, FileFormatType.Excel2003);
        }

        private int GYear(string str)
        {
            int i;
            if (int.TryParse(str, out i))
                return i;
            else
                return 99;
        }

        private string GetBirthdateWithoutSlash(string orig)
        {
            if (string.IsNullOrEmpty(orig)) return orig;
            string[] array = orig.Split('/');
            int chang;
            if (array[0].Length == 4)
            {
                chang = int.Parse(array[0]) - 1911;
            }
            else
            {
                chang = int.Parse(array[0]);
            }
            return chang.ToString() + array[1].PadLeft(2, '0') + array[2].PadLeft(2, '0');
        }

        //切文字
        #region 切文字

        private string GetNumAndSrt1(string fuct)
        {
            if (fuct.Contains("字"))
            {
                return fuct.Remove(fuct.LastIndexOf("字"));
            }
            return fuct;
        }

        #endregion

        //切文號
        #region 切文號

        private string GetNumAndSrt2(string fuct)
        {

            if (fuct.Contains("第") && fuct.Contains("號"))
            {
                return fuct.Substring(fuct.LastIndexOf("第") + 1, fuct.LastIndexOf("號") - fuct.LastIndexOf("第") - 1);
            }
            return fuct;

        }

        #endregion

        public override string Copyright
        {
            get { return "IntelliSchool"; }
        }

        public override string Description
        {
            get { return "中部辦公室95年11月編印管理手冊規範格式"; }
        }

        public override string ReportName
        {
            get { return "學籍異動名冊"; }
        }

        public override string Version
        {
            get { return "1.0.0.0"; }
        }
    }
}
