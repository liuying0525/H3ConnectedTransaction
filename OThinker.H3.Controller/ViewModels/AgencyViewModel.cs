using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class AgencyViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置委托ID
        /// </summary>
        public string AgencyID { get; set; }

        /// <summary>
        /// 获取或设置委托类型
        /// </summary>
        public string AgencyType { get; set; }
        /// <summary>
        /// 获取或设置委托人ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 获取或设置委托人姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获取或设置被委托人ID
        /// </summary>
        public string WasAgentID { get; set; }
        /// <summary>
        /// 获取或设置被委托人姓名
        /// </summary>
        public string WasAgentName { get; set; }
        /// <summary>
        /// 获取或设置流程模板类型
        /// </summary>
        public string WorkflowCode { get; set; }
        /// <summary>
        /// 获取或设置流程模板名称
        /// </summary>
        public string WorkflowName { get; set; }
        
        /// <summary>
        /// 获取或设置发起人组织结构范围
        /// </summary>
        public string OriginatorRange { get; set; }
        /// <summary>
        /// 获取或设置开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 获取或设置结束时间
        /// </summary>
        public string EndTime { get; set; }
    }
}
