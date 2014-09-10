using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateRecordModule_IBSH.DAL
{
    public class SchoolData
    {
        /// <summary>
        /// 學校代碼
        /// </summary>
        public string SchoolCode { get;set; }

        /// <summary>
        /// 學校名稱
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 學校所在地
        /// </summary>
        public string SchoolLocation { get; set; }

        /// <summary>
        /// 學校所在地代碼
        /// </summary>
        public string SchoolLocationCode { get; set; }

        /// <summary>
        /// 學校類別
        /// </summary>
        public string SchoolType { get; set; }
    }
}
