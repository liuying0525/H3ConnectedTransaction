using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class PortalPageViewModel
    {
        /// <summary>
        /// 部件实例ID
        /// </summary>
        public string ObjectID { get; set; }
        /// <summary>
        /// 页面模板id
        /// </summary>
        public string PageID { get; set; }
        /// <summary>
        /// 部件ID
        /// </summary>
        public string WebPartID { get; set; }
        /// <summary>
        /// 部件Name
        /// </summary>
        public string WebPartName { get; set; }
        /// <summary>
        /// 获取部件的展示区域
        /// </summary>
        public string WebPartPost { get; set; }

        /// <summary>
        /// 获取部件的HtmlContent
        /// </summary>
        public string HtmlContent { get; set; }


        /// <summary>
        /// 获取或设置编辑部件标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 获取或设置编辑部件权限编码
        /// </summary>
        public string FunctionCode { get; set; }
        /// <summary>
        /// 获取或设置编辑部件显示标题栏是否显示
        /// </summary>
        public int TitleVisible { get; set; }
        /// <summary>
        /// 获取或设置编辑部件标题图标
        /// </summary>
        public string TitleIcon { get; set; }
        /// <summary>
        /// 获取或设置编辑部件标题图标是否显示
        /// </summary>
        public int TitleIconVisible { get; set; }
        /// <summary>
        /// 获取或设置编辑部件css样式
        /// </summary>
        public string CssName { get; set; }
        /// <summary>
        /// 获取或设置编辑部件宽度
        /// </summary>
        public string Width { get; set; }
        /// </summary>
        /// <summary>
        /// 获取或设置编辑部件宽度单位
        /// </summary>
        public string WidthUnit { get; set; }
        /// <summary>
        /// 获取或设置编辑部件高度
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 获取或设置编辑部件高度单位
        /// </summary>
        public string HeightUnit { get; set; }



        /// <summary>
        /// 获取或设置编辑部件控件路径[基于本站]
        /// </summary>
        public string ControlPath { get; set; }
        /// <summary>
        /// 获取或设置编辑部件更多链接
        /// </summary>
        public string MoreLink { get; set; }
        /// <summary>
        /// 获取或设置编辑部件更多链接文字
        /// </summary>
        public string MoreText { get; set; }
        /// <summary>
        /// 获取或设置编辑部件更多链接位置
        /// </summary>
        public string MorePos { get; set; }
    }
}
