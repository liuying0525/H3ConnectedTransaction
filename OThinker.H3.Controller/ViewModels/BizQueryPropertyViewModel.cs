using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 查询列表查询条件模型
    /// </summary>
    public class BizQueryPropertyViewModel
    {
        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string Name { get; set; }

        public string NameValue { get; set; }
        /// <summary>
        /// 获取或设置是否显示
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 获取或设置宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 获取或设置显示格式
        /// </summary>
        public string Foramt { get; set; }

        /// <summary>
        /// 是否启用排序
        /// </summary>
        public bool Sortable { get; set; }
        /// <summary>
        /// 排序顺序
        /// </summary>
        public int Zindex { get; set; }
    }
}
