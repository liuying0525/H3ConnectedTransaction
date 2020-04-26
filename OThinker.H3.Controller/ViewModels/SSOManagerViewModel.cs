using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 单点登录显示模型
    /// </summary>
    public class SSOManagerViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool AllowGetToken { get; set; } 
        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string DefaultUrl { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string SubmitPasswordID { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string SubmitUrl { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string SubmitUserNameControlID { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string SystemCode { get; set; }
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string SystemName { get; set; }

    }
}
