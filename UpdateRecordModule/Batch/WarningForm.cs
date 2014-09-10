using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHSchool.Data;
using Aspose.Cells;

namespace UpdateRecordModule_IBSH.Batch
{
    public partial class WarningForm : FISCA.Presentation.Controls.BaseForm    
    {
        int _StudentCount = 0;

        List<SHUpdateRecordRecord> _updateRecList;
        List<SHStudentRecord> _StudRecList;

        public WarningForm()
        {
            InitializeComponent();
            this.MaximumSize = this.MinimumSize = this.Size;
            _updateRecList = new List<SHUpdateRecordRecord>();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public void SetStudRec(List<SHStudentRecord> StudRecList)
        {
            _StudRecList = StudRecList;        
        }


        /// <summary>
        /// 設定人數
        /// </summary>
        /// <param name="count"></param>
        public void SetStudentCount(int count)
        {
            _StudentCount = count;
        }

        public void SetUpdateRecList(List<SHUpdateRecordRecord> upRecList)
        {
            _updateRecList = upRecList;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (_updateRecList.Count == 0)
                return;

            Workbook wb = new Workbook();
            wb.Worksheets[0].Name = "已有新生異動";
            wb.Worksheets[0].Cells[0, 0].PutValue("學號");
            wb.Worksheets[0].Cells[0, 1].PutValue("班級");
            wb.Worksheets[0].Cells[0, 2].PutValue("座號");
            wb.Worksheets[0].Cells[0, 3].PutValue("姓名");
            wb.Worksheets[0].Cells[0, 4].PutValue("異動日期");

            int row = 1;

            foreach (SHUpdateRecordRecord rec in _updateRecList)
            {
                foreach (SHStudentRecord srec in _StudRecList.Where(x => x.ID == rec.StudentID))
                {
                    wb.Worksheets[0].Cells[row, 0].PutValue(srec.StudentNumber);
                    if (srec.Class != null)
                        wb.Worksheets[0].Cells[row, 1].PutValue(srec.Class.Name);
                    if (srec.SeatNo.HasValue)
                        wb.Worksheets[0].Cells[row, 2].PutValue(srec.SeatNo.Value);

                    wb.Worksheets[0].Cells[row, 3].PutValue(srec.Name);
                }
                wb.Worksheets[0].Cells[row, 4].PutValue(rec.UpdateDate);
                row++;
            }

            //string filePath = Application.StartupPath + "\\已有重複新生異動.xls";
            //try
            //{
            //    wb.Save(filePath, FileFormatType.Excel2003);
            //    System.Diagnostics.Process.Start(filePath);
            //}
            //catch
            //{
            //    FISCA.Presentation.Controls.MsgBox.Show("檔案開啟發生錯誤.");
            //}

            utility.ExportXls("已有重複新生異動(高中)", wb);
        }

        private void WarningForm_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "有"+_StudentCount+"名學生已有新生異動。是否覆蓋?";
        }
    }
}
