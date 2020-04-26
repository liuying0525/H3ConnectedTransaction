using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class InstanceViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置流程实例ID
        /// </summary>
        public string InstanceID { get; set; }
        /// <summary>
        /// 获取或设置流程实例优先级
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// 获取或设置流程实例名称
        /// </summary>
        public string InstanceName { get; set; }
        /// <summary>
        /// 获取或设置流程实例模板Code
        /// </summary>
        public string WorkflowCode { get; set; }
        /// <summary>
        /// 获取或设置流程实例模板Name
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// 获取或设置流程发起人
        /// </summary>
        public string Originator { get; set; }
        /// <summary>
        /// 获取或设置发起人姓名
        /// </summary>
        public string OriginatorName { get; set; }
        /// <summary>
        /// 获取或设置发起人所在OU名称
        /// </summary>
        public string OriginatorOUName { get; set; }
        /// <summary>
        /// 获取或设置流程实例审批环节
        /// </summary>
        public string ApproverLink { get; set; }
        /// <summary>
        /// 获取或设置流程实例审批环节的审批人
        /// </summary>
        public string Approver { get; set; }
        /// <summary>
        /// 获取或设置流程实例审批环节的审批人所属组织
        /// </summary>
        public string ApproverOuDept { get; set; }
        /// <summary>
        /// 获取或设置流程实例状态
        /// </summary>
        public string InstanceState { get; set; }
        /// <summary>
        /// 获取或设置流程实例发起时间
        /// </summary>
        public string CreatedTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例计划完成时间
        /// </summary>
        public string PlanFinishTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例结束时间
        /// </summary>
        public string FinishedTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例运行时长（滞留时间）
        /// </summary>
        public TimeSpan stayTime { get; set; }
        /// <summary>
        /// 获取或设置流程是否异常
        /// </summary>
        public bool Exceptional { get; set; }
    }
}
