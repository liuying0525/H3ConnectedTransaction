using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class FunctionViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置菜单图标
        /// </summary>
        public string IconCss { get; set; }

        /// <summary>
        /// 获取或设置父节点编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 获取或设置菜单编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 获取或设置菜单显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 一级节点编码
        /// </summary>
        public string TopAppCode { get; set; }

        /// <summary>
        /// 获取或设置菜单Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 获取或设置菜单的排序值
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 获取或设置子菜单
        /// </summary>
        public List<FunctionViewModel> Children { get; set; }

        /// <summary>
        /// 获取或设置是否打开新窗口
        /// </summary>
        public bool OpenNewWindow { get; set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置图标地址
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 获取或设置树节点类型
        /// </summary>
        public int NodeType { get; set; }

        /// <summary>
        /// 获取或设置图标的存储类型
        /// </summary>
        public int IconType { get; set; }

        /// <summary>
        /// 获取或设置父节点名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 是否固定到首页
        /// </summary>
        public bool DockOnHomePage { get; set; }

        /// <summary>
        /// 获取或设置页面地址
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// 获取或设置应用地址详情
        /// </summary>
        public FunctionUrlViewModel FunctionUrl { get; set; }
    }
}