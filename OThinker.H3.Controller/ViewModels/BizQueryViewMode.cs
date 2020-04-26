using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 查询列表显示模型
    /// </summary>
    public class BizQueryViewMode : ViewModelBase
    {
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }
        ///<summary>
        ///获取或设置查询列表编码
        ///</summary>
        public string QueryCode { get; set; }

        ///<summary>
        ///获取或设置显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置方法列表
        ///</summary>
        public string ListMethod { get; set; }

        ///<summary>
        ///获取或设置是否打开就查询
        ///</summary>
        public bool QueryWhenLoad { get; set; }

        /// <summary>
        /// 获取或设置查询列显示设置
        /// </summary>
        public BizQueryColumn[] Columns { get; set; }

        /// <summary>
        /// 获取或设置初始查询编码
        /// </summary>
        public string OrginQueryCode { get; set; }

    }
}
