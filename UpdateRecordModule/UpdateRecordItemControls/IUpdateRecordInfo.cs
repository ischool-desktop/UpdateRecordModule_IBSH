using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpdateRecordModule_IBSH.UpdateRecordItemControls
{
    /// <summary>
    /// 異動畫面繼承這需實作
    /// </summary>
    interface IUpdateRecordInfo
    {
        /// <summary>
        /// 學生異動資料
        /// </summary>
        /// <returns></returns>
        SHSchool.Data.SHUpdateRecordRecord GetStudUpdateRecord();
                
        /// <summary>
        /// 取得畫面 Log 後結果
        /// </summary>
        /// <returns></returns>
        PermRecLogProcess GetLogData();
    }
}
