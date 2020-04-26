using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 钉钉应用模型
    /// </summary>
    public class DingAppViewModel
    {
        /// <summary>
        /// 微应用ID
        /// </summary>
        public string AgentId { get; set; }
        /// <summary>
        /// 获取或设置应用头像ID(钉钉服务器)
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 获取或设置应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 获取或设置应用描述
        /// </summary>
        public string AppDesc { get; set; }

        /// <summary>
        /// 获取或设置移动应用打开的地址
        /// </summary>
        public string HomeUrl { get; set; }

        /// <summary>
        /// 获取或设置PC端打开的地址
        /// </summary>
        public string PcUrl { get; set; }

        /// <summary>
        /// 获取或设置应用管理页面地址
        /// </summary>
        public string  OmpLink{get;set;}

        /// <summary>
        /// 获取或设置图片地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
