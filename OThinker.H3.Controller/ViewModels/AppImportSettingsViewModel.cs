using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 导入应用设置
    /// </summary>
    public class AppImportSettingsViewModel
    {
        /// <summary>
        /// 获取或设置应用编码
        /// </summary>
        public string AppCode { get; set; }

        /// <summary>
        /// 获取或设置应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 获取或设置是否覆盖
        /// </summary>
        public bool IsCover { get; set; }

        /// <summary>
        /// 获取或设置菜单节点
        /// </summary>
        public List<FunctionNode> FunctionNodeList { get; set; }

    }
}
