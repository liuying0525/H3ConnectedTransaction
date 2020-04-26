using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 页脚的加载方式
    /// </summary>
    public enum CountingType
    {
        /// <summary>
        /// 总和
        /// </summary>
        Sum = 0,
        /// <summary>
        /// 平均
        /// </summary>
        Avg = 1,
        /// <summary>
        /// 计数
        /// </summary>
        Count = 2,
        /// <summary>
        /// 不做统计
        /// </summary>
        None = 3,
        /// <summary>
        /// 用户自定义统计的方式，实际上，是由用户自行来进行统计值
        /// </summary>
        Custom = 4
    }
}
