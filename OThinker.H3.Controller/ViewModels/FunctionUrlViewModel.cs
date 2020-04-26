using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// url参数模型
    /// </summary>
    public class FunctionUrlViewModel
    {
        /// <summary>
        /// 获取或设置页面
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// 获取或设置流程编码
        /// </summary>
        public string SchemaCode { get; set; }

        /// <summary>
        /// 获取或设置查询列表编码
        /// </summary>
        public string QueryCode { get; set; }

        /// <summary>
        /// 获取或设置流程状态
        /// </summary>
        public string WorkItemState { get; set; }

        /// <summary>
        /// 获取或设置作用域
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 获取或设置实例状态
        /// </summary>
        public string InstanceState { get; set; }
        /// <summary>
        /// 获取或设置表单编码
        /// </summary>
        public string SheetCode { get; set; }
        /// <summary>
        /// 获取或设置报表编码
        /// </summary>
        public string ReportCode { get; set; }

        /// <summary>
        /// 获取或设置功能编码
        /// </summary>
        public string FunctionCode { get; set; }

        /// <summary>
        /// 获取或设置模式
        /// </summary>
        public string Mode { get; set; }
    }
}
