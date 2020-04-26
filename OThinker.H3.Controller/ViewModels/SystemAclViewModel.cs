using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 系统管理员显示模型
    /// </summary>
    public class SystemAclViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获取或设置所属组织
        /// </summary>
        public string Organization { get; set; }
    }
}
