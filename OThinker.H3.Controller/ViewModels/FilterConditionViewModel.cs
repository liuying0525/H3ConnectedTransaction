using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 过滤条件显示模型(定时任务)
    /// </summary>
    public class FilterConditionViewModel
    {
        /// <summary>
        /// 获取或设置属性
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 获取或设置运算符
        /// </summary>
        public string Operater { get; set; }

        /// <summary>
        /// 获取或设置排序方式
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public string Value { get; set; }
    }
}
