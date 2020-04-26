using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 轮播图模型
    /// </summary>
    public class SlideShowViewModel:ViewModelBase
    {
        /// <summary>
        /// 轮播图编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 图像地址
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 父节点编码
        /// </summary>
        public string ParentCode { get; set; }
    }

}
