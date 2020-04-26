using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 选择用户的方式
    /// </summary>
    public enum UserListType
    {
        /// <summary>
        /// 列出Unit的子成员
        /// </summary>
        Children = 1,

        /// <summary>
        /// 列出Unit的所有父对象
        /// </summary>
        Parents = 2,

        /// <summary>
        /// 列出从UnitID开始查找的角色的直接子成员
        /// </summary>
        Role = 4, 

        /// <summary>
        /// 列出名称以某个字符串打头的组织对象
        /// </summary>
        NameStartWith = 5
    }
}
