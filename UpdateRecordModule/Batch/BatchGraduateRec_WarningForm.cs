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
    public partial class BatchGraduateRec_WarningForm : FISCA.Presentation.Controls.BaseForm
    {
        Dictionary<string, SHClassRecord> _classRecDict;
        Dictionary<string, SHStudentRecord> _studentDict;
        List<SHUpdateRecordRecord> _GUpdateRecList;
        public BatchGraduateRec_WarningForm(Dictionary<string,SHClassRecord> classRec,Dictionary<string,SHStudentRecord> studRec,List<SHUpdateRecordRecord> updateRec)
        {
            InitializeComponent();
            _classRecDict = classRec;
            _studentDict = studRec;
            _GUpdateRecList = updateRec;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (_GUpdateRecList.Count == 0)
                return;
            int rowIdx = 1;
            Workbook wb = new Workbook();
            wb.Worksheets[0].Name = "已有畢業異動";
            wb.Worksheets[0].Cells[0, 0].PutValue("學號");
            wb.Worksheets[0].Cells[0, 1].PutValue("班級");
            wb.Worksheets[0].Cells[0, 2].PutValue("座號");
            wb.Worksheets[0].Cells[0, 3].PutValue("姓名");
            wb.Worksheets[0].Cells[0, 4].PutValue("異動日期");

            foreach (SHUpdateRecordRecord rec in _GUpdateRecList)
            {
                if (_studentDict.ContainsKey(rec.StudentID))
                {
                    wb.Worksheets[0].Cells[rowIdx, 0].PutValue(_studentDict[rec.StudentID].StudentNumber);
                    
                    if(_classRecDict.ContainsKey(_studentDict[rec.StudentID].RefClassID))
                        wb.Worksheets[0].Cells[rowIdx, 1].PutValue(_classRecDict[_studentDict[rec.StudentID].RefClassID].Name);

                    if((_studentDict[rec.StudentID].SeatNo.HasValue))
                        wb.Worksheets[0].Cells[rowIdx, 2].PutValue(_studentDict[rec.StudentID].SeatNo.Value.ToString());

                    wb.Worksheets[0].Cells[rowIdx, 3].PutValue(_studentDict[rec.StudentID].Name);
                    wb.Worksheets[0].Cells[rowIdx, 4].PutValue(rec.UpdateDate);
                    rowIdx++;
                }
            }
            utility.ExportXls("已有重複畢業異動(高中)", wb);
            
        }

        private void BatchGraduateRec_WarningForm_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "有" + _GUpdateRecList.Count + "名學生已有畢業異動。是否覆蓋?";
        }
    }
}
