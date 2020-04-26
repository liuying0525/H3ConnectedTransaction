using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 应用中心显示模型
    /// </summary>
    public class AppsViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置编码
        /// </summary>
        public string AppCode { get; set; }

        /// <summary>
        /// 获取或设置名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取或设置微信ID
        /// </summary>
        public string WeChatID { get; set; }

        /// <summary>
        /// 获取或设置排序
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 获取或设置门户是否可见
        /// </summary>
        public bool VisibleOnPortal { get; set; }

         /// <summary>
        /// 获取或设置移动是否可见
        /// </summary>
        public bool VisibleOnMobile { get; set; }

        /// <summary>
        /// 获取或设置是否固定到首页
        /// </summary>
        public bool DockOnHomePage { get; set; }

        /// <summary>
        /// 获取或设置图标
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 钉钉应用ID
        /// </summary>
        public string DingTalkID { get; set; }
    }
}
