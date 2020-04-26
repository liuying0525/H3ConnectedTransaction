using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 流程设计页面ViewModel
    /// </summary>
    public class ActivityTemplateConfigsViewModel : ViewModelBase
    {
        public string ClauseName { get; set; }

        public string WorkflowViewUrl { get; set; }

        public object Package { get; set; }

        /// <summary>
        /// 流程是否锁定 0：锁定，1：未锁定
        /// </summary>
        public int IsControlUsable { get; set; }

        /// <summary>
        /// 标签名称，前台设置
        /// </summary>
        public string TabName { get; set; }

        public string WorkflowFullName { get; set; }

        /// <summary>
        /// 可以选择的业务方法
        /// </summary>
        public object BizMethods { get; set; }

        /// <summary>
        /// 数据项
        /// </summary>
        public List<object> DataItems { get; set; }
        /// <summary>
        /// 前置条件
        /// </summary>
        public object EntryCondition { get; set; }
        /// <summary>
        /// 同步异步
        /// </summary>
        public object SyncOrASync { get; set; }

        /// <summary>
        /// 表单列表
        /// </summary>
        public object SheetCodes { get; set; }

        /// <summary>
        /// 发送条件
        /// </summary>
        public object NotifyCondition { get; set; }

        /// <summary>
        /// 审批结果
        /// </summary>
        public object ApprovalDataItem { get; set; }

        /// <summary>
        /// 意见数据
        /// </summary>
        public object CommentDataItem { get; set; }

        /// <summary>
        /// 参与者策略
        /// </summary>
        public object ParAbnormalPolicy { get; set; }

        /// <summary>
        /// 发起者参与策略
        /// </summary>
        public object OriginatorParAbnormalPolicy { get; set; }

        /// <summary>
        /// 参与者方式，多人单人
        /// </summary>
        public object ParticipantMode { get; set; }

        /// <summary>
        /// 可选参与方式:并行/串行
        /// </summary>
        public object ParticipateMethod { get; set; }

        /// <summary>
        /// 提交时是否检查
        /// </summary>
        public object SubmittingValidation { get; set; }

        /// <summary>
        /// 子流程数据模型映射
        /// </summary>
        public List<object> MapTypes { get; set; }

        /// <summary>
        /// 表单锁
        /// </summary>
        public object LockLevel { get; set; }

        /// <summary>
        /// 锁策略
        /// </summary>
        public object LockPolicy { get; set; }

        /// <summary>
        /// 超时策略
        /// </summary>
        public object OvertimePolicy { get; set; }

        /// <summary>
        /// 数据项映射类型
        /// </summary>
        public List<object> DataDisposalTypes { get; set; }
        /// <summary>
        /// 初始化活动的数据权限
        /// </summary>
        public Dictionary<string, string> WorkflowNames { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WorkflowDocument WorkflowTemplate { get; set; }

        public Instance.InstanceContext InstanceContext { get; set; }

        /// <summary>
        /// 数据项选择
        /// </summary>
        public List<FormulaTreeNode> DataItemsSelect { get; set; }

        /// <summary>
        /// 异常节点
        /// </summary>
        public List<string> ExceptionActivities { get; set; }

        public string workflowMode { get; set; }

    }
}
