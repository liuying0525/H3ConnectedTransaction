using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class HybridAppViewModel:ViewModelBase
    {
        /// <summary>
        /// 是否显示轮播图
        /// </summary>
        public bool SlideShowDisplay { get; set; }

        /// <summary>
        /// 轮播图列表
        /// </summary>
        public List<SlideShowViewModel> SlideShows { get; set; }

        /// <summary>
        /// 移动应用节点列表
        /// </summary>
        public List<AppFunctionNodeViewModel> AppFunctionNodes { get; set; }
    }
}
