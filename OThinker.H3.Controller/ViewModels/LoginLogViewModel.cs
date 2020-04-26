using OThinker.H3.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLogViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获取或设置用户
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 获取或设置登录类型
        /// </summary>
        public string SiteType { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public string LoginTime { get; set; }
        /// <summary>
        /// 浏览器
        /// </summary>
        public string Browser { get; set; }
        /// <summary>
        /// 客户端平台
        /// </summary>
        public string PlatForm { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 获取或设置客户端类型
        /// </summary>
        public string ClientType { get; set; }
    }
}
