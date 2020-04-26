using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 移动应用节点模型
    /// </summary>
    public class AppFunctionNodeViewModel : ViewModelBase
    {
        /// <summary>
        /// 应用节点编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 图标样式地址
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 应用中打开方式
        /// </summary>
        public string AppOpenMethod { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int EdgeNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EdgeCode { get; set; }
    }


}
