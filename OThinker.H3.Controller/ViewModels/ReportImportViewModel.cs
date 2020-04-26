using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class ReportImportViewModel : ViewModelBase
    {
        /// <summary>
        /// 上级目录名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 上级目录编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 流程包XML
        /// </summary>
        public string XMLString { get; set; }


        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool IsCover { get; set; }
       
        /// <summary>
        ///报表模型名称
        /// </summary>
        public ReportsItemDetail ReportsPackage { get; set; }

    

    }

    public class ReportsItemDetail
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 更改之前的Code
        /// </summary>
        public string OldCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码类型（流程包，查询，流程，数据模型）
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
    }
}
