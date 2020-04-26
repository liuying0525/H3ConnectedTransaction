using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 绑定模式
    /// </summary>
    public enum BindType
    {
        /// <summary>
        /// 数据和可视性都绑定
        /// </summary>
        All,
        /// <summary>
        /// 仅绑定数据
        /// </summary>
        OnlyData,
        /// <summary>
        /// 仅绑定可视性
        /// </summary>
        OnlyVisibility, 
        /// <summary>
        /// 仅绑定可见性和可编辑性，不绑定数据
        /// </summary>
        OnlyState
    }
}
