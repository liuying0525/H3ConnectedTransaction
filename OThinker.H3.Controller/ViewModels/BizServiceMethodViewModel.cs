using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizServiceMethodViewModel:ViewModelBase
    {
        public string ServiceCode { get; set; }

        /// <summary>
        /// 适配器编码,页面判断时使用
        /// </summary>
        public string BizAdapterCode { get; set; }
        public string MethodName { get; set; }
        public string DisplayName { get; set; }

        public string Description { get; set; }
        public string ReturnType { get; set; }
        public string MethodSetting { get; set; }

        /// <summary>
        /// 数据库参数前缀
        /// </summary>
        public string ParameterPrefix { get; set; }
    }
}
