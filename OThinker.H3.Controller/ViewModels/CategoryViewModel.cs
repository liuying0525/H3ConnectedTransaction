using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 组织类型
    /// </summary>
    public class CategoryViewModel:ViewModelBase
    {
        /// <summary>
        /// 组织类型编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 组织类型显示名称
        /// </summary>
        public string DisplayName { get; set; }

        //组织类型描述
        public string Description { get; set; }

    }


    /// <summary>
    /// 绑定组织类型的组织显示类
    /// </summary>
    public class CategoryOrgListViewModel : ViewModelBase
    {
      
        /// <summary>
        /// 组织全路径
        /// </summary>
        public string OrgFullPath { get; set; }

    }
}
