using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BPMServiceResult 的摘要说明
/// </summary>
public class BPMServiceResult
{
    /// <summary>
    /// 消息类构造函数
    /// </summary>
    /// <param name="success"></param>
    /// <param name="instanceId"></param>
    /// <param name="workItemId"></param>
    /// <param name="message"></param>
    public BPMServiceResult(bool success, string instanceId, string workItemId, string message, string WorkItemUrl)
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