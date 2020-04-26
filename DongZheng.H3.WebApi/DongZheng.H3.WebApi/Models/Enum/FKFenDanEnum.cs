using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.Enum
{
    /// <summary>
    /// 风控分单开关枚举
    /// </summary>
    public enum E_FkFenDanStaus
    {
        /// <summary>
        /// 全部东正风控
        /// </summary>
        [Description("全部东正风控")]
        AllNew = 0,

        /// <summary>
        /// 
        /// </summary>
        [Description("全部数融风控")]
        AllOld = 1,

        /// <summary>
        /// 部分东正，部分数融
        /// </summary>
        [Description("部分东正，部分数融")]
        PartIn = 2
    }
}