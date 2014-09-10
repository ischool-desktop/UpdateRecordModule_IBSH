using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UpdateRecordModule_IBSH.BL
{
    /// <summary>
    /// 異動名冊類別(使用在存取異動名冊)
    /// </summary>
    public class StudUpdateRecBatchRec
    {
        /// <summary>
        /// 名冊編號
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        public int SchoolYear { get; set; }
        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }
        /// <summary>
        /// 名冊名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 核准日期
        /// </summary>
        public DateTime? ADDate { get; set; }
        /// <summary>
        /// 核准文號
        /// </summary>
        public string ADNumber { get; set; }
        /// <summary>
        /// 名冊內異動資料
        /// </summary>
        public List<StudUpdateRecDoc> StudUpdateRecDocList { get; set; }

        /// <summary>
        /// 學校代碼
        /// </summary>
        public string SchoolCode { get; set; }

        /// <summary>
        /// 學校名稱
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 名冊類別
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// 名冊原來XML
        /// </summary>
        public XmlElement Content { get; set; }

    }
}
