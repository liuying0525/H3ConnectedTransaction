using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels.Cluster
{
    /// <summary>
    /// Cluster设置模型
    /// </summary>
    public class SettingViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置文件服务器
        /// </summary>
        public string StoragePath { get; set; }

        /// <summary>
        /// 获取或设置词库
        /// </summary>
        public string DictionaryPath { get; set; }

        /// <summary>
        /// 获取或设置Smtp
        /// </summary>
        public string Smtp { get; set; }

        /// <summary>
        /// 获取或设置是否启用SSL
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 获取或设置邮件端口号
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// 获取或设置邮件用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置邮件密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置发件人
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 获取或设置编码
        /// </summary>
        public string Encode { get; set; }

        /// <summary>
        /// 获取或设置数据库类型
        /// </summary>
        public string DBType { get; set; }

        /// <summary>
        /// 获取或设置数据库服务地址
        /// </summary>
        public string DBServer { get; set; }

        /// <summary>
        /// 获取或设置数据库名
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 获取或设置数据库用户名
        /// </summary>
        public string DBUser { get; set; }

        /// <summary>
        /// 获取或设置用户密码
        /// </summary>
        public string DBPassword { get; set; }

        /// <summary>
        /// 获取或设置发送短信的SQL
        /// </summary>
        public string SMSInsert { get; set; }

        /// <summary>
        /// 获取或设置读短信的SQL
        /// </summary>
        public string SMSRead { get; set; }



    }
}
