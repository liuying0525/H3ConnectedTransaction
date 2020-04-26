using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{

    /// <summary>
    /// 数据字典显示模型
    /// </summary>
    public class MasterDataViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 获取或设置编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 获取或设置显示文本
        /// </summary>
        public string DisplayText { get; set; }
        /// <summary>
        /// 获取或设置排序码
        /// </summary>
        public int SortKey { get; set; }
        /// <summary>
        /// 获取或设置 默认
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// 导入错误
    /// </summary>
    public class ImportError {
        /// <summary>
        /// 获取或设置行数
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
}
