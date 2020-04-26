using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置流程名称
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// 获取或设置流程的发布日期
        /// </summary>
        public string PublishedDate { get; set; }
        /// <summary>
        /// 获取或设置流程的版本号
        /// </summary>
        public string Verson { get; set; }
        /// <summary>
        /// 获取或设置流程的存为常用
        /// </summary>
        public string Usual { get; set; }
        /// <summary>
        /// 获取或设置流程的发起链接
        /// </summary>
        public string Initiate { get; set; }
    }
}
