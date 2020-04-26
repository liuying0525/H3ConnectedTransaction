using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class AdapterViewModel:ViewModelBase
    {
        public AdapterViewModel() { }

        /// <summary>
        /// 适配器名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 适配器描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 适配器文件路径
        /// </summary>
        public string Assembly { get; set; }
    }
}
