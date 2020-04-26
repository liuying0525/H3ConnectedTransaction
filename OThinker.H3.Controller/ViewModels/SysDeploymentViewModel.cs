using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 一键部署模型
    /// </summary>
    public class SysDeploymentViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置服务器IP地址
        /// </summary>
        public string IPAddr { get; set; }

        /// <summary>
        /// 获取或设置端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 获取或设置实例名
        /// </summary>
        public string EngineName { get; set; }

        /// <summary>
        /// 获取或设置登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password { get; set; }

    }
}
