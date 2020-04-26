using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 全局变量显示模型
    /// </summary>
    public class GlobalDataViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 获取或设置属性值
        /// </summary>
        public string ItemValue { get; set; }
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string ItemName { get; set; }
    }
}
