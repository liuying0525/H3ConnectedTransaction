using OThinker.Data.Database;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Controllers.ViewModels.Cluster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Cluster
{
    /// <summary>
    /// 设置控制器
    /// </summary>
    public class SettingController : ClusterController
    {
        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <returns>设置信息</returns>
        [HttpGet]
        public JsonResult GetSetting()
        {
            return ExecuteFunctionRun(() =>
            {

                List<Item> encodes = GetEncodes();
                string encode = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_MailEncoding) ?? encodes.FirstOrDefault().Value;
                bool enableSsl = false;
                bool.TryParse(this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpEnableSsl), out enableSsl);
                int port = 25;
                int.TryParse(this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpPort), out port);
                SettingViewModel model = new SettingViewModel()
                {
                    StoragePath = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_FileStoragePath),
                    DictionaryPath = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_DictionaryPath),
                    From = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpFrom),
                    Password = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpPassword),
                    Smtp = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpServer),
                    UserName = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpUserName),
                    Encode = encode,
                    SmtpPort = port,
                    EnableSsl = enableSsl,
                    DBServer = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbServer),
                    DBName = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbName),
                    DBUser = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbUser),
                    DBPassword = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbPassword),
                    DBType = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbConnType),
                    SMSRead = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSReadSql),
                    SMSInsert = this.Connection.GetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSInsertSql)
                };
                if (string.IsNullOrEmpty(model.DBType))
                {
                    model.DBType = DatabaseType.SqlServer.ToString();
                };
                ActionResult result = new ActionResult(false);
                result.Extend = new
                {
                    Setting = model,
                    Encodes = GetEncodes(),
                    DBTypes = GetDBTypes()
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存设置信息
        /// </summary>
        /// <param name="setting">设置信息</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveSetting(SettingViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                // 端口设置
                int port = model.SmtpPort == 0 ? 25 : model.SmtpPort;
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_FileStoragePath, model.StoragePath);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_DictionaryPath, model.DictionaryPath);

                // 邮件
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpFrom, model.From);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpPassword, model.Password);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpServer, model.Smtp);

                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpUserName, model.UserName);
                // 邮件编码
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_MailEncoding, model.Encode);

                // Smtp端口和Ssl
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpPort, port.ToString());
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SmtpEnableSsl, model.EnableSsl.ToString());

                // 短信审批模式
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbConnType, model.DBType);
                // 短信设置
                //this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbConnString, this.txtSMSDbConnection.Text);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbServer, model.DBServer);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbName, model.DBName);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbUser, model.DBUser);

                OThinker.Data.Database.DatabaseType dbType;
                string connString = string.Empty;
                if (Enum.TryParse<DatabaseType>(model.DBType, out dbType))
                {
                    connString = OThinker.Data.Database.Database.GetConnectionString(dbType, model.DBServer, model.DBName, model.DBUser, model.DBPassword);
                    this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbConnString, connString);
                }
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSDbPassword, model.DBPassword);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSInsertSql, model.SMSInsert);
                this.Connection.SetCustomSetting(OThinker.Clusterware.VesselCustomSetting.Setting_SMSReadSql, model.SMSRead);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取编码列表
        /// </summary>
        /// <returns>编码列表</returns>
        private List<Item> GetEncodes()
        {
            List<Item> list = new List<Item>();
            System.Text.EncodingInfo[] encodingInfos = System.Text.Encoding.GetEncodings();
            list = encodingInfos.Select(s => new Item()
            {
                Text = s.DisplayName,
                Value = s.Name
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <returns></returns>
        private List<Item> GetDBTypes()
        {
            string[] names = Enum.GetNames(typeof(OThinker.Data.Database.DatabaseType));
            List<Item> list = new List<Item>();
            foreach (string name in names)
            {
                if (name != DatabaseType.Unspecified.ToString())
                    list.Add(new Item()
                    {
                        Text = name,
                        Value = name
                    });
            }
            return list;
        }
    }
}
