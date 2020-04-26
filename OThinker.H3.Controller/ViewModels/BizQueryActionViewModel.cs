using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 查询列表功能列表
    /// </summary>
    public class BizQueryActionViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public int ActionType { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ActionMethod { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ActionSheet { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool WithID { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool Confirm { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public int SaveCompleted { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public int SortKey { get; set; }
        /// <summary>
        /// 是否课件
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
