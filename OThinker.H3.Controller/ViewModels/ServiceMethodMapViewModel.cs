using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务服务与业务方法映射模型
    /// </summary>
    public class ServiceMethodMapViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置业务服务Code
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// 获取或设置业务规则名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 获取或设置方法类型
        /// </summary>
        public string MethodType { get; set; }

        /// <summary>
        /// 获取或设置列号
        /// </summary>
        public string MapIndex { get; set; }

        /// <summary>
        /// 获取或设置业务方法名称
        /// </summary>
        public string Method { get; set; } 
        
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }

        /// <summary>
        /// 获取或设置是否已存在业务映射
        /// </summary>
        public bool IsSelectMap { get; set; }

        /// <summary>
        /// 获取或设置执行参数
        /// </summary>
        public string ExeCondition { get; set; }
    }

}
