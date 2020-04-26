using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 功能权限控制ViewModel
    /// </summary>
    public class FunctionAclViewModel:ViewModelBase
    {
        /// <summary>
        /// 功能编码
        /// </summary>
        public string FunctionCode { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 功能描述
        /// </summary>
        public string FunctionDesc { get; set; }

        /// <summary>
        /// 继承权限
        /// </summary>
        public bool InheritPower { get; set; }

        /// <summary>
        /// 自有权限
        /// </summary>
        public bool OwnPower { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 是否具有运行的权限
        /// </summary>
        public bool Run { get; set; }
        
    }
}
