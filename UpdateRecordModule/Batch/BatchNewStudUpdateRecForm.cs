using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateRecordModule_IBSH.Batch
{
    public partial class BatchNewStudUpdateRecForm : FISCA.Presentation.Controls.BaseForm
    {
        // 批次新生異動管理
        BatchNewStudUpdateRecManager bnsuer;        
        List<string> _StudentIDList;
        string _UpdateDate, _ClassType, _UpdateCode, _UpdateDesc;

        public BatchNewStudUpdateRecForm()
        {
            InitializeComponent();
            this.MaximumSize = this.MinimumSize = this.Size;
            bnsuer = new BatchNewStudUpdateRecManager();
            LoadDefaultData();
            _StudentIDList = new List<string>();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
           
            // 檢查學生是否選擇
            if (K12.Presentation.NLDPanels.Student.SelectedSource.Count < 1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請選學生!");
                return;
            }

            if (string.IsNullOrEmpty(cbxClassType.Text))
            {
                FISCA.Presentation.Controls.MsgBox.Show("請選班別!");
                return;
            }

            if (string.IsNullOrEmpty(cbxUpdateCode.Text))
            {
                FISCA.Presentation.Controls.MsgBox.Show("請選異動代碼!");
                return;            
            }

            btnRun.Enabled = false;
            _StudentIDList = K12.Presentation.NLDPanels.Student.SelectedSource;
            int n=cbxClassType.Text.IndexOf("-");
            if(n>0)
                _ClassType = cbxClassType.Text.Substring(0, n).Trim() ;
            _UpdateCode = cbxUpdateCode.Text.Substring(0, 3);
            _UpdateDesc = cbxUpdateCode.Text.Replace(_UpdateCode, "").Trim();
            _UpdateDate = dtUpdateDate.Value.ToShortDateString();

            if (bnsuer.Run(_StudentIDList, _UpdateDate, _UpdateCode, _UpdateDesc, _ClassType))
            {
                //FISCA.Presentation.Controls.MsgBox.Show("產生完成");
                //this.Close();
            }

            btnRun.Enabled = true;
            this.Close();
        }

        private void LoadDefaultData()
        { 
            // 異動日期
            dtUpdateDate.Value = DateTime.Now;
        
            
            // 異動代碼
            cbxUpdateCode.Items.AddRange(bnsuer.GetNewUpdateCodeList().ToArray());

            // 班別
            cbxClassType.Items.AddRange(bnsuer.GetClassTypeList().ToArray());

            
        }      
    }
}
