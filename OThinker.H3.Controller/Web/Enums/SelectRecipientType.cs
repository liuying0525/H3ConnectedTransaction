using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 用户进行征询意见、协办、传阅、转发、调整活动处理人等步骤的时候都需要选择相应的处理人，表单中通过方法来设置这个处理人的范围。
    /// </summary>
    public enum SelectRecipientType
    {
        /// <summary>
        /// 工作项级别的征询意见
        /// </summary>
        Consult = OThinker.H3.WorkItem.WorkItemType.WorkItemConsult,
        /// <summary>
        /// 传阅，用户可以选择读或者不读，不会阻塞流程执行
        /// </summary>
        Circulate = OThinker.H3.WorkItem.WorkItemType.Circulate,
        /// <summary>
        /// 协助填写表单
        /// </summary>
        Assist = OThinker.H3.WorkItem.WorkItemType.WorkItemAssist, 
        /// <summary>
        /// 转发给其他人
        /// </summary>
        Forward, 
        /// <summary>
        /// 调整活动的处理人
        /// </summary>
        AdjustActivity
    }
}
