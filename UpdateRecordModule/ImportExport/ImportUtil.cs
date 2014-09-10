using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;

namespace UpdateRecordModule_IBSH.ImportExport
{
    class ImportUtil
    {
        /// <summary>
        /// 取得欄位驗證字串
        /// </summary>
        /// <returns></returns>
        public static string GetChekcDataStr(int idx, Worksheet wst, Dictionary<string, int> ColIndexDic)
        {
            string chkStr = string.Empty;
            if (ColIndexDic.ContainsKey("學號"))
                chkStr += wst.Cells[idx, ColIndexDic["學號"]].StringValue;

            if (ColIndexDic.ContainsKey("姓名"))
                chkStr += wst.Cells[idx, ColIndexDic["姓名"]].StringValue;

            //if (ColIndexDic.ContainsKey("學年度"))
            //    chkStr += wst.Cells[idx, ColIndexDic["學年度"]].StringValue;

            //if (ColIndexDic.ContainsKey("學期"))
            //    chkStr += wst.Cells[idx, ColIndexDic["學期"]].StringValue;

            if (ColIndexDic.ContainsKey("異動日期"))
                chkStr += wst.Cells[idx, ColIndexDic["異動日期"]].StringValue;

            if (ColIndexDic.ContainsKey("異動代碼"))
                chkStr += wst.Cells[idx, ColIndexDic["異動代碼"]].StringValue;

            if (ColIndexDic.ContainsKey("原因及事項"))
                chkStr += wst.Cells[idx, ColIndexDic["原因及事項"]].StringValue;
            return chkStr;
        }
    }
}
