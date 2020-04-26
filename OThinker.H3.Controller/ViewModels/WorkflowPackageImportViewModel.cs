using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowPackageImportViewModel:ViewModelBase
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
        ///流程包名称
        /// </summary>
        public ItemDetail WorkflowPackage { get; set; }

     
        /// <summary>
        /// 数据模型名称
        /// </summary>
        public ItemDetail BizSchema { get; set; }

        /// <summary>
        /// 查询列表
        /// </summary>
        public List<ItemDetail> QueryList { get; set; }
       
        /// <summary>
        /// 表单
        /// </summary>
        public List<ItemDetail> BizSheets { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        public List<ItemDetail> WorkFlows { get; set; }
    }

    public class ItemDetail
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
        /// 姓名
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
