using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 编辑组织ViewModel
    /// </summary>
    public class OrgUnitViewModel:ViewModelBase
    {
        /// <summary>
        /// 编辑权限
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// 查看权限
        /// </summary>
        public bool View { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 主管
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string ParentUnit { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 可见类型
        /// </summary>
        public string VisibleType { get; set; }

        /// <summary>
        /// 流程编码
        /// </summary>
        public string WorkflowCode { get; set; }

        /// <summary>
        /// 工作日历
        /// </summary>
        public string Calendar { get; set; }

        /// <summary>
        /// 组织类型
        /// </summary>
        public string OrgCategory { get; set; }

        /// <summary>
        /// 组织完整路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 组织树根ID 
        /// </summary>
        public string RootID { get; set; }
    }
}
