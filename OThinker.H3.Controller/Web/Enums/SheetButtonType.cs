using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 按钮的类型
    /// </summary>
    public enum SheetButtonType
    {
        /// <summary>
        /// 打印按钮
        /// </summary>
        Print = 1,

        /// <summary>
        /// 保存按钮
        /// </summary>
        Save = 2,

        /// <summary>
        /// 查看状态
        /// </summary>
        ViewInstanceState = 3,

        /// <summary>
        /// 征询意见
        /// </summary>
        Consult = 4,

        /// <summary>
        /// 转发
        /// </summary>
        Forward = 5,

        /// <summary>
        /// 传阅
        /// </summary>
        Circulate = 6,

        /// <summary>
        /// 取回
        /// </summary>
        Retrieve = 7,

        /// <summary>
        /// 协办
        /// </summary>
        Assist = 8,

        /// <summary>
        /// 回滚
        /// </summary>
        Rollback = 9,

        /// <summary>
        /// 返回到上一步骤
        /// </summary>
        Return = 10,

        /// <summary>
        /// 提交
        /// </summary>
        Submit = 11,

        /// <summary>
        /// 跳转按钮
        /// </summary>
        Jump = 12,

        /// <summary>
        /// 取消
        /// </summary>
        CancelInstance = 13,

        /// <summary>
        /// 取消
        /// </summary>
        CancelWorkItem = 14,

        /// <summary>
        /// 取消
        /// </summary>
        LockInstance = 15,

        /// <summary>
        /// 取消
        /// </summary>
        UnlockInstance = 16,

        /// <summary>
        /// 调整活动的参与者
        /// </summary>
        AdjustParticipant = 17,

        /// <summary>
        /// 手工选择
        /// </summary>
        ManualChoose = 18,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 19,

        /// <summary>
        /// 结束掉流程
        /// </summary>
        FinishInstance = 20,

        /// <summary>
        /// 移除业务对象
        /// </summary>
        RemoveBizObject = 21,

        /// <summary>
        /// 发起流程
        /// </summary>
        StartInstance = 22
    }
}