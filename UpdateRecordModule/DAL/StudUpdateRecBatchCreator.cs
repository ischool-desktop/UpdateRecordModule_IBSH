using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHSchool.Data;
using System.Xml.Linq;

namespace UpdateRecordModule_IBSH.DAL
{
    /// <summary>
    /// 建立名冊者
    /// </summary>
    public class StudUpdateRecBatchCreator
    {
        public enum UpdateRecBatchType { 新生名冊, 畢業名冊, 轉入學生名冊, 學籍異動名冊, 延修生名冊, 延修生畢業名冊, 延修生學籍異動名冊, 新生保留錄取資格名冊, 借讀學生名冊 }

        private UpdateRecBatchType _UpdateRecBatchType;
        private List<BL.StudUpdateRecDoc> _StudUpdateRecDocList;        
        
        public StudUpdateRecBatchCreator()
        {                      
            _StudUpdateRecDocList = new List<UpdateRecordModule_IBSH.BL.StudUpdateRecDoc>();        
        }

        /// <summary>
        /// 取得異動資料並且轉換成名冊用異動
        /// </summary>
        public void UpdateRecConvertUpdateRecDoc(UpdateRecBatchType urbt)
        {
            _UpdateRecBatchType = urbt;

            // 學生異動資料
            List<SHUpdateRecordRecord> updatRecList= new List<SHUpdateRecordRecord> ();

            // 延修生異動代號
            List<string> ExtendCodeList = new List<string>();
            ExtendCodeList.Add("235");
            ExtendCodeList.Add("236");

            // 新生保留代碼
            List<string> chkCodeList1 = new List<string>();
            chkCodeList1.Add("601");
            chkCodeList1.Add("602");
            chkCodeList1.Add("603");
            chkCodeList1.Add("604");

            // 借讀代碼
            List<string> chkCodeList2 = new List<string>();
            chkCodeList2.Add("701");
            chkCodeList2.Add("702");
            chkCodeList2.Add("703");
            // 取得異動代碼資料
            XElement elms = DAL.DALTransfer.GetUpdateCodeList();
            List<string> CodeList= new List<string> ();

            // 取得非刪除學生
            List<string> StudentIDList = utility.GetStudentIDListNot256();

            // 透過異動代碼取沒有核准文號異動記錄，學生狀態非刪除。
            switch (_UpdateRecBatchType)
            {

                case UpdateRecBatchType.新生名冊:
                    // 取得新生異動代碼
                    CodeList=(from elm in elms.Elements("異動") where elm.Element("分類").Value=="新生異動" select elm.Element("代號").Value).ToList ();                    
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;
            
                case UpdateRecBatchType.畢業名冊:
                    // 取得畢業異動代碼
                    CodeList = (from elm in elms.Elements("異動") where elm.Element("分類").Value == "畢業異動" select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();                   
                    break;

                case UpdateRecBatchType.學籍異動名冊:
                    // 取得學籍異動代碼
                    CodeList = (from elm in elms.Elements("異動") where elm.Element("分類").Value == "學籍異動" select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;

                case UpdateRecBatchType.轉入學生名冊:
                    // 取得轉入異動代碼
                    CodeList = (from elm in elms.Elements("異動") where elm.Element("分類").Value == "轉入異動" select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;

                case UpdateRecBatchType.延修生名冊:
                    // 規則：只取得異動代碼為延修且年級是延修生。
                    CodeList = (from elm in elms.Elements("異動") where ExtendCodeList.Contains(elm.Element("代號").Value) select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear == "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;

                case UpdateRecBatchType.延修生畢業名冊:
                    // 規則：畢業異動代碼且年級是延修生。
                    CodeList = (from elm in elms.Elements("異動") where elm.Element("分類").Value == "畢業異動" select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear == "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;

                case UpdateRecBatchType.延修生學籍異動名冊:
                    // 規則：學籍異動代碼(不包含延修生異動代碼)且年級是延修生。
                    foreach (string code in (from elm in elms.Elements("異動") where elm.Element("分類").Value == "學籍異動" select elm.Element("代號").Value).ToList())
                        if (!ExtendCodeList.Contains(code))
                            CodeList.Add(code);
                    //CodeList = (from elm in elms.Elements("異動") where elm.Element("分類").Value == "學籍異動" select elm.Element("代號").Value).ToList();
                    updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear == "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;            

                case UpdateRecBatchType.新生保留錄取資格名冊:
CodeList = (from elm in elms.Elements("異動") where chkCodeList1.Contains(elm.Element("代號").Value) select elm.Element("代號").Value).ToList();
updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();
break;


                case UpdateRecBatchType.借讀學生名冊:
CodeList = (from elm in elms.Elements("異動") where chkCodeList2.Contains(elm.Element("代號").Value) select elm.Element("代號").Value).ToList();
updatRecList = (from data in SHUpdateRecord.SelectByUpdateCodes(CodeList) where StudentIDList.Contains(data.StudentID) && data.GradeYear != "延修生" && data.ADNumber.Trim()=="" select data).ToList();
                    break;
            }


            // 轉換成符合名冊異動格式
            _StudUpdateRecDocList = DAL.DALTransfer.ConvertSHUpdateRecToStudUpdateRec(updatRecList);
        }

        /// <summary>
        /// 建立名冊
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        /// <param name="DocName"></param>
        /// <param name="dataList"></param>
        public void CreateUpdateRecBatchDoc(string SchoolYear, string Semester, string DocName, List<BL.StudUpdateRecDoc> dataList)
        {
            int sy, ss;
            BL.StudUpdateRecBatchRec StudUpdateRecBRec = new UpdateRecordModule_IBSH.BL.StudUpdateRecBatchRec();

            if(int.TryParse(SchoolYear,out sy))
                StudUpdateRecBRec.SchoolYear=sy;

            if (int.TryParse(Semester, out ss))
                StudUpdateRecBRec.Semester = ss;

            StudUpdateRecBRec.UpdateType= _UpdateRecBatchType.ToString ();
            StudUpdateRecBRec.Name = DocName;
            StudUpdateRecBRec.StudUpdateRecDocList = dataList;
            DAL.DALTransfer.SetStudUpdateRecBatchRec(StudUpdateRecBRec,true);
        
        }


        /// <summary>
        /// 取得名冊異動資料 List
        /// </summary>
        /// <returns></returns>
        public List<BL.StudUpdateRecDoc> GetSHUpdateRecordRecordList()
        {
            return _StudUpdateRecDocList;        
        }

        /// <summary>
        /// 取得異動名冊類別
        /// </summary>
        /// <returns></returns>
        public UpdateRecBatchType GetUpdateRecBatchType()
        {
            return _UpdateRecBatchType;
        }
    }
}
