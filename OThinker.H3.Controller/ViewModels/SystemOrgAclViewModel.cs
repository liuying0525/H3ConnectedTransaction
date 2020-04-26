using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
   public  class SystemOrgAclViewModel:ViewModelBase
    {
       /// <summary>
       /// 获取或设置
       /// </summary>
       public string UserName { get; set; }

       public string[] UserIDArr { get; set; }
       /// <summary>
       /// 获取或设置
       /// </summary>
       public string OrgScope { get; set; }

       public string[] OrgScopeArr { get; set; }
       /// <summary>
       /// 获取或设置
       /// </summary>
       public bool ViewOrgData { get; set; }

       /// <summary>
       /// 获取或设置
       /// </summary>
       public bool Administrator { get; set; }


    }
}
