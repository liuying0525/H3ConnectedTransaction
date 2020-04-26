using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels.Cluster
{
    public class VesselViewModel : ViewModelBase
    {

        /// <summary>
        /// 获取或设置编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 获取或设置服务器地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 获取或设置端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 获取或设置权重
        /// </summary>
        public int LoadWeight { get; set; }

        /// <summary>
        /// 获取或设置排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 获取或设置当前运行状态
        /// </summary>
        public string CurrentState { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string Description { get; set; }
    }
}
