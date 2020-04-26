using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 渲染控件属性设置
    /// </summary>
    public class UserOptions
    {
        /// <summary>
        /// 获取或设置控件的水印文本
        /// </summary>
        public string PlaceHolder { set; get; }
        /// <summary>
        /// 获取或设置是否选择【组织群】
        /// </summary>
        public bool SegmentVisible { set; get; }
        /// <summary>
        /// 获取或设置是否选择【组织单元】
        /// </summary>
        public bool OrgUnitVisible { set; get; }
        /// <summary>
        /// 获取或设置是否选择【用户组】
        /// </summary>
        public bool GroupVisible { set; get; }
        /// <summary>
        /// 获取或设置是否选择【岗位】
        /// </summary>
        public bool PostVisible { set; get; }
        /// <summary>
        /// 获取或设置是否选择【用户】
        /// </summary>
        public bool UserVisible { set; get; }
        /// <summary>
        /// 获取或设置根节点显示的组织节点编码
        /// </summary>
        public string RootUnitID { set; get; }
        /// <summary>
        /// 获取需要显示的组织单元编码
        /// </summary>
        public string VisibleUnits { set; get; }
    }
}
