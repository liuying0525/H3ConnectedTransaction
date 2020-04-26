using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 打开表单的模式
    /// </summary>
    public enum SheetMode
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unspecified = InteractiveType.Unspecified,
        /// <summary>
        /// 填写表单，完成用户任务
        /// </summary>
        Work = InteractiveType.Work,
        /// <summary>
        /// 查看某个流程实例的表单
        /// </summary>
        View = InteractiveType.View,
        /// <summary>
        /// 发起流程
        /// </summary>
        Originate = InteractiveType.Create,
        /// <summary>
        /// 打印模式
        /// </summary>
        Print = InteractiveType.Print,
        /// <summary>
        /// 传阅查看表单模式(区分View的原因是Item从不同接口获取)
        /// </summary>
        Circulate = InteractiveType.Circulate
    }

    /// <summary>
    /// 表单模式
    /// </summary>
    public enum SheetDataType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// 流程数据
        /// </summary>
        Workflow = 1,
        /// <summary>
        /// 业务对象
        /// </summary>
        BizObject = 2
    }

    /// <summary>
    /// 附件打开方式
    /// </summary>
    public enum AttachmentOpenMethod
    {
        /// <summary>
        /// 下载方式
        /// </summary>
        Download = 0,
        /// <summary>
        /// 浏览器直接打开方式
        /// </summary>
        Browse = 1,
        /// <summary>
        /// 使用 NTKO 的方式打开附件
        /// </summary>
        NTKO = 2
    }

}