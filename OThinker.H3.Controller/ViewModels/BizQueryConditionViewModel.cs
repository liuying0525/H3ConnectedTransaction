using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 查询列表查询条件模型
    /// </summary>
    public class BizQueryConditionViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string ConditionColumn { get; set; }
        /// <summary>
        /// 获取或设置匹配类型
        /// </summary>
        public int FilterType { get; set; }
        /// <summary>
        /// 获取或设置默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 获取或设置是否显示
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 获取或设置显示类型
        /// </summary>
        public int DisplayType { get; set; }
        /// <summary>
        /// 获取或设置值
        /// </summary>
        public string ConditionValue { get; set; }

        /// <summary>
        /// 获取或设置显示类型列表
        /// </summary>
        public List<Item> DisplayTypes { get; set; }

    }
}
