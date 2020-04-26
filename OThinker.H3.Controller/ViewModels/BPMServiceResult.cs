using OThinker.H3.Instance;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 流程服务返回消息类
    /// </summary>
    [Serializable]
    public class BPMServiceResult
    {
        /// <summary>
        /// 消息类构造函数
        /// </summary>
        /// <param name="success"></param>
        /// <param name="instanceId"></param>
        /// <param name="workItemId"></param>
        /// <param name="message"></param>
        public BPMServiceResult(bool success, string instanceId, string workItemId,
            string message, string WorkItemUrl)
        {
            this.Success = success;
            this.InstanceID = instanceId;
            this.Message = message;
            this.WorkItemID = workItemId;
            this.WorkItemUrl = WorkItemUrl;
        }

        /// <summary>
        /// 消息类构造函数
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public BPMServiceResult(bool success, string message)
            : this(success, string.Empty, string.Empty, message, string.Empty)
        {

        }

        public BPMServiceResult() { }

        private bool success = false;
        /// <summary>
        /// 获取或设置流程启动是否成功
        /// </summary>
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
        private string instanceId = string.Empty;
        /// <summary>
        /// 获取或设置启动的流程实例ID
        /// </summary>
        public string InstanceID
        {
            get { return instanceId; }
            set { this.instanceId = value; }
        }
        private string message = string.Empty;
        /// <summary>
        /// 获取或设置系统返回消息
        /// </summary>
        public string Message
        {
            get { return message; }
            set { this.message = value; }
        }
        private string workItemId = string.Empty;
        /// <summary>
        /// 获取或设置第一个节点的ItemID
        /// </summary>
        public string WorkItemID
        {
            get { return workItemId; }
            set { this.workItemId = value; }
        }
        private string workItemUrl = string.Empty;
        /// <summary>
        /// 获取或设置第一个节点的url
        /// </summary>
        public string WorkItemUrl
        {
            get { return workItemUrl; }
            set { this.workItemUrl = value; }
        }
    }

    /// <summary>
    /// 数据项参数
    /// </summary>
    [Serializable]
    public class DataItemParam
    {
        private string itemName = string.Empty;
        /// <summary>
        /// 获取或设置数据项名称
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { this.itemName = value; }
        }

        private object itemValue = string.Empty;
        /// <summary>
        /// 获取或设置数据项的值
        /// </summary>
        public object ItemValue
        {
            get { return itemValue; }
            set { this.itemValue = value; }
        }
    }

    [Serializable]
    public class CommentParam
    {
        /// <summary>
        /// 获取或设置意见文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 获取或设置签章ID
        /// </summary>
        public string SignatureId { get; set; }
    }

    [Serializable]
    public class AttachmentParam
    {
        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 获取或设置文件内容
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// 获取或设置文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 获取或设置附件的上传地址
        /// </summary>
        public string DownloadUrl { get; set; }
    }

    /// <summary>
    /// 获取流程实例信息
    /// </summary>
    [Serializable]
    public class InstanceInfo
    {
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public string InstanceId { get; set; }
        public string SequenceNo { get; set; }
        /// <summary>
        /// 获取或设置活动ID
        /// </summary>
        public int TokenId { get; set; }
        /// <summary>
        /// 获取或设置流程状态
        /// </summary>
        public InstanceState InstanceState { get; set; }
        /// <summary>
        /// 获取或设置任务状态
        /// </summary>
        public WorkItemState WorkItemState { get; set; }
        /// <summary>
        /// 获取或设置发起人
        /// </summary>
        public string Originator { get; set; }
        /// <summary>
        /// 获取或设置允许的操作
        /// </summary>
        public OThinker.H3.WorkflowTemplate.PermittedActions PermittedActions { get; set; }
        /// <summary>
        /// 获取或设置数据项权限
        /// </summary>
        public DataItemPermission[] DataPermissions { get; set; }
        /// <summary>
        /// 获取或设置当前用户的签章
        /// </summary>
        public List<SignatureParam> MySignatures { get; set; }
        /// <summary>
        /// 获取或设置常用意见
        /// </summary>
        public List<string> FrequentlyComment { get; set; }
        /// <summary>
        /// 获取或设置允许驳回的节点
        /// </summary>
        public List<ActivityParam> RejectActivies { get; set; }
    }

    public class SignatureParam
    {
        /// <summary>
        /// 签章ID
        /// </summary>
        public string SignautreId { get; set; }
        /// <summary>
        /// 签章名称
        /// </summary>
        public string SignatureName { get; set; }

        /// <summary>
        /// 获取或设置是否默认签章
        /// </summary>
        public bool IsDefault { get; set; }
        public int SortKey { get; set; }
    }

    /// <summary>
    /// 流程节点
    /// </summary>
    public class ActivityParam
    {
        public string ActivityCode { get; set; }
        public string DisplayName { get; set; }
    }
}
