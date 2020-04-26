using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizDbConfigViewModel:ViewModelBase
    {
        public BizDbConfigViewModel() { }

        /// <summary>
        /// DB连接编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// DB连接显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public OThinker.Data.Database.DatabaseType DbType { get; set; }

        /// <summary>
        ///数据服务器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 数据库连接用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 数据库连接用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库连接说明
        /// </summary>
        public string Description { get; set; }
    }
}
