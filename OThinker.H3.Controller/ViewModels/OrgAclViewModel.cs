using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class OrgAclViewModel:ViewModelBase
    {
        public OrgAclViewModel(){
            this.Edit = false;
            this.View = false;
        }
        /// <summary>
        ///是否组织编辑权限
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// 是否有组织查看权限
        /// </summary>
        public bool View { get; set; }
    }
}
