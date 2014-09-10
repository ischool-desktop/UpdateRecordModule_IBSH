using System;
using System.Collections.Generic;
using System.Text;
using SHSchool.Data;
using System.Threading;
using SmartSchool.API.PlugIn;
using Framework;
using System.Linq;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.ImportExport
{
    class ImportUpdateRecCode100:SmartSchool.API.PlugIn.Import.Importer
    {
        List<string> _UpdateCodeList;
        public ImportUpdateRecCode100()
        {
            this.Image = null;
            this.Text = "匯入學籍異動";
            // 取得學籍異動的異動代碼
            _UpdateCodeList = (from elm in DAL.DALTransfer.GetUpdateCodeList().Elements("異動") where elm.Element("分類").Value == "學籍異動" select elm.Element("代號").Value).ToList();
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            // 取得學生資料
            Dictionary<string, SHStudentRecord> Students = new Dictionary<string, SHStudentRecord>();
            
            

            // 取得異動資料
            Dictionary<string, List<SHUpdateRecordRecord>> UpdateRecs = new Dictionary<string, List<SHUpdateRecordRecord>>();
            wizard.PackageLimit = 3000;
            wizard.ImportableFields.AddRange("班別", "特殊身份代碼", "異動科別", "年級", "異動學號", "異動姓名", "身分證號", "註1", "異動代碼", "異動日期", "原因及事項", "新學號", "更正後資料", "舊班別", "舊科別代碼", "備查日期", "備查文號", "核准日期", "核准文號", "備註");
            wizard.RequiredFields.AddRange("異動代碼", "異動日期");
            wizard.ValidateStart += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateStartEventArgs e)
            {
                Students.Clear();
                UpdateRecs.Clear();

                // 取得學生資料
                foreach (SHStudentRecord studRec in SHStudent.SelectByIDs(e.List))
                    if (!Students.ContainsKey(studRec.ID))
                        Students.Add(studRec.ID, studRec);
                foreach (string str in Students.Keys)
                {
                    List<SHUpdateRecordRecord> UpdRecList = new List<SHUpdateRecordRecord>();
                    UpdateRecs.Add(str, UpdRecList);
                }

                // 取得異動
                MultiThreadWorker<string> loader1 = new MultiThreadWorker<string>();
                loader1.MaxThreads = 3;
                loader1.PackageSize = 250;
                loader1.PackageWorker += delegate(object sender1, PackageWorkEventArgs<string> e1)
                {
                    foreach (SHUpdateRecordRecord UpdRec in SHUpdateRecord.SelectByStudentIDs(e.List))
                    {                      

                        // 過濾非符合標準的異動(目前是學籍)
                        if(!_UpdateCodeList.Contains(UpdRec.UpdateCode))
                            continue;

                            if (UpdateRecs.ContainsKey(UpdRec.StudentID))
                                UpdateRecs[UpdRec.StudentID].Add(UpdRec);
                    }
                };
                loader1.Run(e.List);
            };

            wizard.ValidateRow += delegate(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
            {
                int i = 0;
                DateTime dt;
                // 檢查學生是否存在
                SHStudentRecord studRec = null;
                if (Students.ContainsKey(e.Data.ID))
                    studRec = Students[e.Data.ID];
                else
                {
                    e.ErrorMessage = "沒有這位學生" + e.Data.ID;
                    return;
                }

                // 驗證格式資料
                bool InputFormatPass = true;
                foreach (string field in e.SelectFields)
                {
                    string value = e.Data[field].Trim();
                    //// 驗證$無法匯入
                    //if (value.IndexOf('$') > -1)
                    //{
                    //    e.ErrorFields.Add(field, "儲存格有$無法匯入.");
                    //    break;
                    //}
                    switch (field)
                    {
                        default:
                            break;

                        //// 班別
                        //case "班別": break;
                        //// 特殊身份代碼
                        //case "特殊身份代碼": break;
                        //// 異動科別
                        //case "異動科別": break;
                        //// 年級
                        //case "年級": break;
                        //// 異動學號
                        //case "異動學號": break;
                        //// 異動姓名
                        //case "異動姓名": break;
                        //// 身分證號
                        //case "身分證號": break;
                        //// 註1
                        //case "註1": break;
                        //// 異動種類
                        //case "異動種類": break;
                        // 異動代碼
                        case "異動代碼":
                            if (!_UpdateCodeList.Contains(value))
                            {
                                InputFormatPass &= false;
                                e.ErrorFields.Add(field, "非學籍異動代碼!");
                            }                            
                            break;
                        // 異動日期(必填)
                        case "異動日期":
                            DateTime dtC1;
                            if (DateTime.TryParse(value, out dtC1))
                            { }
                            else
                            {
                                InputFormatPass &= false;
                                e.ErrorFields.Add(field, "日期錯誤!");                                
                            }
                            
                            break;

                        case "備查日期":
                        case "核准日期":
                            DateTime dtC2;
                            if (value.Trim() != "")
                            {
                                if (DateTime.TryParse(value, out dtC2))
                                { }
                                else
                                {
                                    InputFormatPass &= false;
                                    e.ErrorFields.Add(field, "日期錯誤!");
                                }
                            }
                            break;                            
                        //// 原因及事項
                        //case "原因及事項": break;
                        //// 新學號
                        //case "新學號": break;
                        //// 更正後資料
                        //case "更正後資料": break;
                        //// 舊班別
                        //case "舊班別": break;
                        //// 舊科別代碼
                        //case "舊科別代碼": break;
                        //// 備查日期
                        //case "備查日期":                             
                        //    break;
                        //// 備查文號
                        //case "備查文號": break;
                        //// 核准日期
                        //case "核准日期": break;
                        //// 核准文號
                        //case "核准文號": break;
                        //// 備註
                        //case "備註": break;
                    }
                }

            };


            wizard.ImportPackage += delegate(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
            {
                Dictionary<string, List<RowData>> id_Rows = new Dictionary<string, List<RowData>>();
                foreach (RowData data in e.Items)
                {
                    if (!id_Rows.ContainsKey(data.ID))
                        id_Rows.Add(data.ID, new List<RowData>());
                    id_Rows[data.ID].Add(data);
                }

                List<SHUpdateRecordRecord> InsertList = new List<SHUpdateRecordRecord>();
                List<SHUpdateRecordRecord> UpdateList = new List<SHUpdateRecordRecord>();

                // 檢查新增或更新方式：
                // 每筆 Key 為：異動日期+異動代碼+原因及事項，如果三者內容相同更新，如果不同就新增。
                foreach (string id in id_Rows.Keys)
                {
                    DateTime dt;

                    // 讀取工作表內資料
                    foreach (RowData data in id_Rows[id])
                    {
                        // 當異動記錄內沒有工作表讀取轉換後學生ID，就跳過。
                        if (!UpdateRecs.ContainsKey(id))
                            continue;

                        DateTime.TryParse(data["異動日期"], out dt);

                        // 異動代碼
                        string UpdateCode = string.Empty;
                        if (data.ContainsKey("異動代碼"))
                            UpdateCode = data["異動代碼"];

                        // 取得原因及事項
                        string UpdateDesc = string.Empty;
                        if (data.ContainsKey("原因及事項"))
                            UpdateDesc = data["原因及事項"];


                        SHUpdateRecordRecord updateRec = null;
                        // 異動日期+異動代碼 (如果相同有當更新，不同就新增)
                        foreach (SHUpdateRecordRecord urr in UpdateRecs[id])
                        {
                            if (UpdateCode == urr.UpdateCode)
                            {
                                DateTime dt1;
                                DateTime.TryParse(urr.UpdateDate, out dt1);
                                if (dt == dt1)
                                {
                                    // 使用原因及事項當作Key
                                    if(UpdateDesc == urr.UpdateDescription )
                                        updateRec = urr;
                                }
                            }
                        }
                        bool isInsert = true;

                        if (updateRec == null)
                        {
                            updateRec = new SHUpdateRecordRecord();
                            updateRec.StudentID = id;
                        }
                        else
                            isInsert = false;

                        // 這段在做資料填入異動紀錄
                        foreach (string field in e.ImportFields)
                        {
                            string value = data[field].Trim();
                            switch (field)
                            {
                                // 班別
                                case "班別":
                                    updateRec.ClassType = value;
                                    break;
                                // 特殊身份代碼
                                case "特殊身份代碼":
                                    updateRec.SpecialStatus = value;
                                    break;
                                // 異動科別
                                case "異動科別":
                                    updateRec.Department = value;
                                    break;
                                // 年級
                                case "年級":
                                    updateRec.GradeYear = value;
                                    break;
                                // 異動學號
                                case "異動學號":
                                    updateRec.StudentNumber = value;
                                    break;
                                // 異動姓名
                                case "異動姓名":
                                    updateRec.StudentName = value;
                                    break;
                                // 身分證號
                                case "身分證號":
                                    updateRec.IDNumber = value;
                                    break;
                                // 註1
                                case "註1":
                                    //updateRec.IDNumberComment = value;
                                    break;
                                //// 異動種類
                                //case "異動種類":
                                //    break;
                                // 異動代碼
                                case "異動代碼":
                                    updateRec.UpdateCode = value;
                                    break;
                                // 異動日期
                                case "異動日期":
                                    DateTime dt1;
                                    if (DateTime.TryParse(value, out dt1))
                                        updateRec.UpdateDate = dt1.ToShortDateString();
                                    break;
                                // 原因及事項
                                case "原因及事項":
                                    updateRec.UpdateDescription = value;
                                    break;
                                // 新學號
                                case "新學號":
                                    updateRec.NewStudentNumber = value;
                                    break;
                                // 更正後資料
                                case "更正後資料":
                                    updateRec.NewData = value;
                                    break;
                                // 舊班別
                                case "舊班別":
                                    updateRec.OldClassType = value;
                                    break;
                                // 舊科別代碼
                                case "舊科別代碼":
                                    updateRec.OldDepartmentCode = value;
                                    break;
                                // 備查日期
                                case "備查日期":
                                    DateTime dt2;
                                    if (DateTime.TryParse(value,out dt2))
                                        updateRec.LastADDate = dt2.ToShortDateString ();
                                    break;
                                // 備查文號
                                case "備查文號":
                                    updateRec.LastADNumber = value;
                                    break;
                                // 核准日期
                                case "核准日期":
                                    DateTime dt3;
                                    if(DateTime.TryParse(value, out dt3))
                                        updateRec.ADDate = dt3.ToShortDateString();
                                    break;
                                // 核准文號
                                case "核准文號":
                                    updateRec.ADNumber = value;
                                    break;
                                // 備註
                                case "備註":
                                    updateRec.GraduateComment = value;
                                    break;
                            }
                        }


                        if (string.IsNullOrEmpty(updateRec.StudentID) || string.IsNullOrEmpty(updateRec.UpdateDate) || string.IsNullOrEmpty(updateRec.UpdateCode))
                            continue;
                        else
                        {
                            if (isInsert)
                                InsertList.Add(updateRec);
                            else
                                UpdateList.Add(updateRec);
                        }
                    }

                }

                try
                {
                    if (InsertList.Count > 0)
                        Insert(InsertList);

                    if (UpdateList.Count > 0)
                        Update(UpdateList);

                    PermRecLogProcess prlp = new PermRecLogProcess();
                    prlp.SaveLog("學生.匯入異動", "匯入學籍異動", "匯入學籍異動：共新增" + InsertList.Count + "筆資料,共更新:" + UpdateList.Count + "筆資料");
                    SmartSchool.StudentRelated.Student.Instance.SyncAllBackground();
                }
                catch (Exception ex) { }
            };
        }

        private void Update(object item)
        {
            try
            {
                List<SHUpdateRecordRecord> UpdatePackage = (List<SHUpdateRecordRecord>)item;

                SHUpdateRecord.Update(UpdatePackage);

            }
            catch (Exception ex) { FISCA.Presentation.Controls.MsgBox.Show("更新資料發生異常."); }

        }

        private void Insert(object item)
        {
            try
            {
                List<SHUpdateRecordRecord> InsertPackage = (List<SHUpdateRecordRecord>)item;
                SHUpdateRecord.Insert(InsertPackage);
            }
            catch (Exception ex) { FISCA.Presentation.Controls.MsgBox.Show("新增資料發生異常."); }
        }
    }
}
