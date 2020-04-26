using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class SAPConnectionConfigViewModel:ViewModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登陆用户
        /// </summary>
        public string LoginUser { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }

        public int MaxPoolSize { get; set; }

        public string Client { get; set; }

        public string AppserverHost { get; set; }

        public string SystemNumber { get; set; }

        public string IsDefault { get; set; }
    }
}
