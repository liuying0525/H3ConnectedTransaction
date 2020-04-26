using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 数据模型显示模型
    /// </summary>
    public class BizObjectSchemaViewModel:ViewModelBase
    {
        ///<summary>
        ///获取或设置编码
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        ///获取或设置显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置排序码
        ///</summary>
        public string SortKey { get; set; }

        /// <summary>
        /// 获取或设置类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 获取或设置描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父节点编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 是否应用流程包
        /// </summary>
        public bool IsQuotePacket { get; set; }
    }
}
