using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
   public class QuerySystemOrgAclListParam
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string[] UserName { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string[] OUName { get; set; }
    }
}
