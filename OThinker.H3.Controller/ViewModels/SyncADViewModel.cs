using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{

    /// <summary>
    /// 组织同步ViewModel
    /// </summary>
    public class SyncADViewModel:ViewModelBase
    {
        public SyncADViewModel() { }

        /// <summary>
        /// 路径
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 上次同步时间
        /// </summary>
        public string LastSyncTime { get; set; }

        /// <summary>
        /// 自动同步时间
        /// </summary>
        public string AutoSynchTime { get; set; }

        /// <summary>
        /// 自动加载时间
        /// </summary>
        public string AutoReloadTime { get; set; }

        /// <summary>
        /// 启用AD验证
        /// </summary>
        public bool ADValidator { get; set; }
      
    }
}
