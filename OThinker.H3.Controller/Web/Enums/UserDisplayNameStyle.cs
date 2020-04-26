using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 用户显示名称的格式
    /// </summary>
    public enum UserDisplayNameStyle
    {
        /// <summary>
        /// 默认方式
        /// </summary>
        Default = 0,
        /// <summary>
        /// 全名称
        /// </summary>
        FullName = 1,
        /// <summary>
        /// 只显示用户名
        /// </summary>
        Name = 2,
        /// <summary>
        /// 只显示用户登录名
        /// </summary>
        LoginName = 3,
        /// <summary>
        /// 不显示
        /// </summary>
        None = 10
    }
}
