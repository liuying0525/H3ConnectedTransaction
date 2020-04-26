using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务方法运行方法显示模型
    /// </summary>
    public class TestObjectViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get;set;}

        /// <summary>
        /// 获取或设置方法名称
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 获取或设置字表的关联对象
        /// </summary>
        public string AssociationName { get; set; }
    }
}
