using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreCore
{
    /// <summary>
    /// 匹配类型
    /// </summary>
    public enum MatchingType
    {
        /// <summary>
        /// 范围
        /// </summary>
        Range,
        /// <summary>
        /// 有值
        /// </summary>
        Exists,
        /// <summary>
        /// 无值
        /// </summary>
        NotExists,
        /// <summary>
        /// 等于，或者数组内有等于
        /// </summary>
        Equal
    }

    /// <summary>
    /// 经销商数据类型
    /// </summary>
    public enum DealerDataType
    {
        /// <summary>
        /// 单一值
        /// </summary>
        Single,
        /// <summary>
        /// 值数组
        /// </summary>
        Array
    }
}
