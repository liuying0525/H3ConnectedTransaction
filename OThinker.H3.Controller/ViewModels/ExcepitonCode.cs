using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ExceptionCode
    {
        /// <summary>
        /// 未指定类型
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// 未正常通过认证
        /// </summary>
        NoAuthorize = 1
    }
}