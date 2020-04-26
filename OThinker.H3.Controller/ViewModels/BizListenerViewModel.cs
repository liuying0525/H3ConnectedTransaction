using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务对象监听模型
    /// </summary>
    public class BizListenerViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置流程实例
        /// </summary>
        public string InstanceID { get; set; }

        /// <summary>
        /// 获取或设置节点编码
        /// </summary>
        public string ActivityCode { get; set; }

        /// <summary>
        /// 获取或设置查询条件
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        public string CreatedTime { get; set; }
    }
}
