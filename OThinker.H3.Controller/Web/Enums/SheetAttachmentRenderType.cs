using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 控件的显示方式
    /// </summary>
    public enum SheetAttachmentRenderType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,
        /// <summary>
        /// 如果是图片且控件处于不可编辑的状态显示图片
        /// </summary>
        ShowImageWhenDisable
    }
}
