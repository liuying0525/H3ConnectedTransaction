using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class AgentsViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置任务委托ID
        /// </summary>
        public string AgencyID { get; set; }
        /// <summary>
        /// 获取或设置流程名称
        /// </summary>
        public string WorkflowName { get; set; }

        /// <summary>
        /// 获取或设置流程编码
        /// </summary>
        public string WorkflowCode { get; set; }

        /// <summary>
        /// 获取或设置委托类型
        /// </summary>
        public string AgencyType { get; set; }

        /// <summary>
        /// 获取或设置代理人ID
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        /// 获取或设置代理人名字
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 获取或设置开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 获取或设置结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 获取或设置委托人ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 获取或设置委托人名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置默认委托
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// 发起人组织结构范围
        /// </summary>
        public string Originator { get; set; }
    }
}
