using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 数据模型权限模型
    /// </summary>
    public class BizObjectAclViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置目录权限的目录的ID
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }

        /// <summary>
        /// 获取或设置用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 获取或设置数据模型名称
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// 获取或设置组织范围
        /// </summary>
        public string OrgScope { get; set; }

        /// <summary>
        /// 获取或设置组织类型
        /// </summary>
        public string OrgScopeType { get; set; }

        /// <summary>
        /// 获取或设置权限范围
        /// </summary>
        public string[] OrgScopeArr { get; set; }

        /// <summary>
        /// 获取或设置新增权限
        /// </summary>
        public bool IsCreate { get; set; }

        /// <summary>
        /// 获取或设置查看权限
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 获取或设置管理权限
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
