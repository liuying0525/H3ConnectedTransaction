using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class CirculateItemViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置参与者
        /// </summary>
        public string Participant { get; set; }
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
        /// 获取或设置流程实例名称
        /// </summary>
        public string InstanceName { get; set; }
        /// <summary>
        /// 获取或设置活动名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 获取或设置工作任务接收时间
        /// </summary>
        public string ReceiveTime { get; set; }
        /// <summary>
        /// 获取或设置工作任务计划完成时间
        /// </summary>
        public string PlanFinishTime { get; set; }
        /// <summary>
        /// 获取或设置工作任务完成时间
        /// </summary>
        public string FinishTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例ID
        /// </summary>
        public string InstanceId { get; set; }
        /// <summary>
        /// 获取或设置流程实例WorkflowCode
        /// </summary>
        public string WorkflowCode { get; set; }
        /// <summary>
        /// 获取或设置流程实例WorkflowName
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// 获取或设置流程模板的流程数(待办任务分组模式)
        /// </summary>
        public string ItemCount { get; set; }
        /// <summary>
        /// 获取或设置表中表是否展示(待办任务分组模式)
        /// </summary>
        public bool DisplayWorkflowCode { get; set; }
        /// <summary>
        /// 获取或设置工作任务紧急程度
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// 获取或设置工作任务状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 获取或设置当前任务是否被催办
        /// </summary>
        public bool Urged { get; set; }
        /// <summary>
        /// 获取或设置当前任务是否有协办
        /// </summary>
        public bool Assisted { get; set; }
        /// <summary>
        /// 获取或设置当前任务是否协办已完成
        /// </summary>
        public bool AssistantFinished { get; set; }
        /// <summary>
        /// 获取或设置当前任务是否有征询意见
        /// </summary>
        public bool Consulted { get; set; }
        /// <summary>
        /// 获取或设置当前任务是否征询已完成
        /// </summary>
        public bool ConsultantFinished { get; set; }
        /// <summary>
        /// 获取或设置传阅人ID
        /// </summary>
        public string CirculateCreator { get; set; }
        /// <summary>
        /// 获取或设置传阅人Name
        /// </summary>
        public string CirculateCreatorName { get; set; }

        /// <summary>
        /// 获取或设置流程实例运行时长（滞留时间）
        /// </summary>
        public TimeSpan StayTime { get; set; }
    }
}
