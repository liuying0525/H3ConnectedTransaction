using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class OrgJobViewModel:ViewModelBase
    {
        public OrgJobViewModel()
        {
            RoleLevel = 0;
            Sortkey = 0;
        }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 上级角色ObjectID
        /// </summary>
        public string ParentPostObjectID { get; set; }

        /// <summary>
        /// 上级角色名称
        /// </summary>
        public string ParentPostName { get; set; }

        /// <summary>
        /// 角色层级
        /// </summary>
        public int RoleLevel { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sortkey { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 用户数
        /// </summary>
        public int UserCount { get; set; }
      
    }


    public class RoleUserViewModel : ViewModelBase
    {
        public string RoleID { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户帐号
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 管理范围
        /// </summary>
        public string ManagerScope { get; set; }

        /// <summary>
        /// 管理范围ID
        /// </summary>
        public string ManagerScopeIds { get; set; }

        public int Sortkey { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
