using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务服务与业务方法映射详情模型
    /// </summary>
    public class ServiceMethodMapDetailViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置参数名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 获取或设置映射方式
        /// </summary>
        public string MapType { get; set; }

        /// <summary>
        /// 获取或设置映射对象
        /// </summary>
        public string MapTo { get; set; }
    }

}
