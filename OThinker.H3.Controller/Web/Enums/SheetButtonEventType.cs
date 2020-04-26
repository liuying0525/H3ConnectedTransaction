using System;

namespace OThinker.H3.Controllers
{
	/// <summary>
	/// ButtonType 的摘要说明。
	/// </summary>
    public enum SheetButtonEventType
    {
        /// <summary>
        /// 不触发任何事件
        /// </summary>
        None = 0,
        /// <summary>
        /// 更新按钮
        /// </summary>
        UpdateButton = 1,
        /// <summary>
        /// 完成按钮
        /// </summary>
        FinishButton = WorkItem.ActionEventType.Forward, 
        /// <summary>
        /// 触发流程回退的事件
        /// </summary>
        ReturnButton = WorkItem.ActionEventType.Backward
    }
}
