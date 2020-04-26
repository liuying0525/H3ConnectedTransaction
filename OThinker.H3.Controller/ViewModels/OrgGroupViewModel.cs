using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class OrgGroupViewModel:ViewModelBase
    {
        public OrgGroupViewModel() { 
         
        }
        /// <summary>
        /// 编辑权限
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// 查看权限
        /// </summary>
        public bool View { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 组成员
        /// </summary>
        public string Members { get; set; }

        /// <summary>
        /// 所属组织
        /// </summary>
        public string ParentUnit { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortKey { get; set; }

        /// <summary>
        /// 可见类型
        /// </summary>
        public OThinker.Organization.VisibleType VisibleType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
