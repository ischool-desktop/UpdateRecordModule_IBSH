using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateRecordModule_IBSH.GovernmentalDocument
{
    public partial class UpdateRecordADN : FISCA.Presentation.Controls.BaseForm
    {
        BL.StudUpdateRecBatchRec GovDoc;

        /// <summary>
        /// 登入文號
        /// </summary>
        public UpdateRecordADN(string id)
        {
            InitializeComponent();
            // 取得名冊
            GovDoc = DAL.DALTransfer.GetStudUpdateRecBatchRec(id);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            // 檢查資料
            if (string.IsNullOrEmpty(txtNumber.Text))
            {
                FISCA.Presentation.Controls.MsgBox.Show("請輸入核准文號。");
                return;
            }

            if (dtADDate.IsEmpty)
            {
                FISCA.Presentation.Controls.MsgBox.Show("請輸入核准日期。");
                return;           
            }
            

            // 修改名冊內異動
            GovDoc.ADDate = dtADDate.Value;
            GovDoc.ADNumber = txtNumber.Text;

            // 取得名冊內異動資料ID,修改學生異動資料使用
            List<string> updateRecIDList = new List<string>();
            

            foreach (BL.StudUpdateRecDoc rec in GovDoc.StudUpdateRecDocList)
            {
                rec.ADDate = dtADDate.Value.ToShortDateString();
                rec.ADNumber = txtNumber.Text;
                if (!updateRecIDList.Contains(rec.URID))
                    updateRecIDList.Add(rec.URID);
            }

            // 回存名冊
            DAL.DALTransfer.SetStudUpdateRecBatchRec(GovDoc, false);

            // 修改學生身上的相對異動記錄核准日期文號
            DAL.DALTransfer.SetStudsUpdateRecADdata(dtADDate.Value.ToShortDateString(), txtNumber.Text, updateRecIDList);
            Global.OnUpdateDocsChange();
            this.Close();
        }

        private void UpdateRecordADN_Load(object sender, EventArgs e)
        {
            // 載入初始
            if (GovDoc != null)
            {
                if (GovDoc.ADDate.HasValue)
                    dtADDate.Value = GovDoc.ADDate.Value;
                txtNumber.Text = GovDoc.ADNumber;
            }
            else
            {
                dtADDate.Text = "";
                txtNumber.Text = "";
            }

        }
    }
}
