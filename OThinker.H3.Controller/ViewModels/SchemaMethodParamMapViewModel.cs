using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class SchemaMethodParamMapViewModel : ViewModelBase
    {

        /// <summary>
        /// 获取活设置
        /// </summary>
        public string MapType { get; set; }

        /// <summary>
        /// 获取活设置
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 获取活设置
        /// </summary>
        public string ConstValue { get; set; }

        /// <summary>
        /// 获取或设置参数名称
        /// </summary>
        public string ParamName { get; set; }
        
        /// <summary>
        /// 获取或设置是否是参数
        /// </summary>
        public bool IsParam { get; set; }

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


    }
}
