using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 系统设置模型
    /// </summary>
    public class CommonParamViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置新用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置委托身份
        /// </summary>
        public string AgencyIdentity { get; set; }

        /// <summary>
        /// 获取或设置异常管理员
        /// </summary>
        public string ExceptionManager { get; set; }

        /// <summary>
        /// 获取或设置流水号的方式
        /// </summary>
        public string SequenceNoModal { get; set; }

        /// <summary>
        /// 获取或设置流水号的日期格式
        /// </summary>
        public string SequenceNoDateFormat { get; set; }

        /// <summary>
        /// 获取或设置流水号的长度
        /// </summary>
        public string SequenceNoOrder { get; set; }

        /// <summary>
        /// 获取或设置流水号的顺序
        /// </summary>
        public string SequenceNoLength { get; set; }

        /// <summary>
        /// 获取或设置流水号的重置策略
        /// </summary>
        public string SequenceNoResetType { get; set; }

        /// <summary>
        /// 获取或设置流程的顺序号
        /// </summary>
        public string NextInstanceID { get; set; }

        /// <summary>
        /// 获取或设置任务超时检查的时间间隔
        /// </summary>
        public string OvertimeCheckInterval { get; set; }

        /// <summary>
        /// 获取或设置内网集成服务器的地址
        /// </summary>
        public string RemoteHubAddress { get; set; }

        /// <summary>
        /// 获取或设置帮助站点Url
        /// </summary>
        public string WikiUrl { get; set; }

        #region 微信
        /// <summary>
        /// 获取或设置微信CropID
        /// </summary>
        public string CorpID { get; set; }

        /// <summary>
        /// 获取或设置微信通讯录Secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 获取或设置微信消息应用Secret
        /// </summary>
        public string MessageSecret { get; set; }

        /// <summary>
        /// 获取或设置微信消息推送应用
        /// </summary>
        public string WeChatProID { get; set; }

        /// <summary>
        /// 获取或设置微信打开消息url
        /// </summary>
        public string WeChatMsgUrl { get; set; }
        #endregion

        #region Jpush
        /// <summary>
        /// 获取或设置AppKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// app名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 获取或设置MasterSecret
        /// </summary>
        public string MasterSecret { get; set; }
        #endregion

        #region 钉钉
        /// <summary>
        /// 获取或设置钉钉CropID
        /// </summary>
        public string DDCorpID { get; set; }

        /// <summary>
        /// 获取或设置钉钉Secret
        /// </summary>
        public string DDSecret { get; set; }

        /// <summary>
        /// 获取或设置钉钉消息推送应用
        /// </summary>
        public string DDProID { get; set; }

        /// <summary>
        /// 获取或设置钉钉打开消息url
        /// </summary>
        public string DDMsgUrl { get; set; }


        /// <summary>
        /// 获取或设置钉钉Pc端打开消息url
        /// </summary>
        public string DDPcMsgUrl { get; set; }
        #endregion

        /// <summary>
        /// 获取或设置存储方式
        /// </summary>
        public string StorageMethod { get; set; }

        /// <summary>
        /// 获取或设置数据库连接池
        /// </summary>
        public string SQLPool { get; set; }

        /// <summary>
        /// 获取或设置ftp服务器
        /// </summary>
        public string FTPServer { get; set; }

        /// <summary>
        /// 获取或设置ftp端口
        /// </summary>
        public string FTPServerPort { get; set; }

        /// <summary>
        /// 获取或设置ftp账户
        /// </summary>
        public string FTPAcount { get; set; }

        /// <summary>
        /// 获取或设置ftp密码
        /// </summary>
        public string FTPPassWord { get; set; }

        /// <summary>
        /// 获取或设置客户端打开方式
        /// </summary>
        public string ClientMethod { get; set; }

        /// <summary>
        /// 获取或设置直接打开URL
        /// </summary>
        public string FTPUrl { get; set; }

        /// <summary>
        /// 获取或设置ftp目录
        /// </summary>
        public string BasePath { get; set; }


    }
}
