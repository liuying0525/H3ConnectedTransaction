using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.AppCode
{
    /// <summary>
    /// 编辑组织结构的模式
    /// </summary>
    public enum EditUserType
    {
        /// <summary>
        /// 未指定的模式
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// 添加模式
        /// </summary>
        Add = 0,
        /// <summary>
        /// 完全控制模式
        /// </summary>
        Admin = 1,
        /// <summary>
        /// 编辑自己的信息的模式
        /// </summary>
        Profile = 2,
        /// <summary>
        /// 查看模式
        /// </summary>
        View = 3
    }
}
