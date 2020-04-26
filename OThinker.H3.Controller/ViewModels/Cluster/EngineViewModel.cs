using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels.Cluster
{
    /// <summary>
    /// 引擎实例模型
    /// </summary>
    public class EngineViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置引擎编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 获取或设置数据库类型
        /// </summary>
        public string DBType { get; set; }

        /// <summary>
        /// 获取或设置服务地址
        /// </summary>
        public string DBServer { get; set; }

        /// <summary>
        /// 获取或设置数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>日志数据库类型
        /// 获取或设置
        /// </summary>
        public string LogDBType { get; set; }

        /// <summary>
        /// 获取或设置日志数据库地址
        /// </summary>
        public string LogDBServer { get; set; }

        /// <summary>
        /// 获取或设置日志数据库名称
        /// </summary>
        public string LogDBName { get; set; }

        /// <summary>
        /// 获取或设置引擎运行状态
        /// </summary>
        public string UnitState { get; set; }

        /// <summary>
        /// 获取或设置账户名
        /// </summary>
        public string DBUser { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string DBPassword { get; set; }

        /// <summary>
        /// 获取或设置账户名
        /// </summary>
        public string LogDBUser { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string LogDBPassword { get; set; }
    }
}
