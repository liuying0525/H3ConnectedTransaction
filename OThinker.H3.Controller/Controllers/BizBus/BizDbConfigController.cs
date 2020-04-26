using OThinker.Data.Database;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    public class BizDbConfigController:ControllerBase
    {
        public BizDbConfigController() { }

        public override string FunctionCode
        {
            get
            {
                return Acl.FunctionNode.BizBus_EditBizDbConfig_Code;
            }
        }

        public const string EditBizDbConfig_Default = "默认";


        public JsonResult GetDbConfig(string code)
        {
            return ExecuteFunctionRun(() => {
                try
                {
                    Settings.BizDbConnectionConfig BizDbConnectionConfigEntity = this.Engine.SettingManager.GetBizDbConnectionConfig(code);
                    if (BizDbConnectionConfigEntity == null)
                        return null;

                    BizDbConfigViewModel model = new BizDbConfigViewModel();
                    model.Code = BizDbConnectionConfigEntity.DbCode;
                    model.DisplayName = BizDbConnectionConfigEntity.DisplayName;

                    model.DbType = BizDbConnectionConfigEntity.DbType;

                    model.Description = BizDbConnectionConfigEntity.Description;

                    if (!string.IsNullOrEmpty(BizDbConnectionConfigEntity.DbConnectionString))
                    {
                        string[] array = BizDbConnectionConfigEntity.DbConnectionString.Split(';');

                        model.ServerName = GetValue(BizDbConnectionConfigEntity.DbConnectionString, "(server|Source|Host)");
                        model.DBName = GetValue(BizDbConnectionConfigEntity.DbConnectionString, "(database|Catalog)");
                        model.UserName = GetValue(BizDbConnectionConfigEntity.DbConnectionString, "(uid|id)");
                        string password = GetValue(BizDbConnectionConfigEntity.DbConnectionString, "(password|pwd)");
                        model.Password = password;
                        //this.txtPwd.Attributes.Add("value", password);
                    }
                    model.ObjectID = BizDbConnectionConfigEntity.ObjectID;

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return null;
                }
                
            });
        }

        /// <summary>
        /// 保存数据库连接
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveDbConfig(BizDbConfigViewModel model)
        {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult(true, "");

                //校验连接
                //try
                //{
                //    if (!CheckConnection(model)) { result.Success = false; result.Message = "BizDbConnectionConfig.Msg0"; return Json(result); }
                //}
                //catch (Exception ex) {

                //    result.Success = false; result.Message = "BizDbConnectionConfig.Msg0"; result.Extend = ex.Message; return Json(result); 
                //}

                OThinker.H3.Settings.BizDbConnectionConfig config;

                if (!string.IsNullOrEmpty(model.ObjectID))
                {
                    //如果不是新增
                    config = this.Engine.SettingManager.GetBizDbConnectionConfig(model.Code);
                }
                else
                {
                    config = new H3.Settings.BizDbConnectionConfig();
                }

                if (!string.IsNullOrEmpty(model.Code))
                {
                    if (config == null)
                    { result.Success = false; result.Message = "BizDbConnectionConfig.CodeNotNull"; return Json(result); }
                }

                config.DbCode = model.Code;
                config.DbType = model.DbType;
                config.DbConnectionString = GetConnectionStr(model);
                config.Description = model.Description;
                config.DisplayName = model.DisplayName;

                try
                {
                    //如果是新增
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        //如果编码重复，则提示已经存在相同的编码，请修改
                        //This code has already exists
                        if (this.Engine.SettingManager.GetBizDbConnectionConfig(model.Code) != null)
                        {
                            result.Success = false;
                            result.Message = "BizDbConnectionConfig.Msg2";//TODO
                        }
                        else
                        {
                            if (!this.Engine.SettingManager.AddBizDbConnectionConfig(config))
                            {
                                result.Success = false;
                                result.Message = "BizDbConnectionConfig.Msg0";//TODO
                            }
                        }
                       

                       
                    }
                    else
                    {
                        if (!this.Engine.SettingManager.UpdateBizDbConnectionConfig(config))
                        {
                            result.Success = false;
                            result.Message = "BizDbConnectionConfig.Msg0";
                        }
                    }
                    //前台处理
                   // CloseCurrentDialog();
                }
                catch (Exception ex)
                {
                    string InnerEx=string.Empty;
                    string DeeperInnerEx=string.Empty;
                    if (ex.InnerException != null)
                    {
                        InnerEx = ex.InnerException.ToString();
                        if (ex.InnerException.InnerException != null)
                        {
                            DeeperInnerEx = ex.InnerException.InnerException.ToString();
                        }
                    }
                     
                    result.Message = "BizDbConnectionConfig.Failed";
                    result.Extend = ex.Message + " Detail: " + InnerEx + " InnerDetail: " + DeeperInnerEx;
                    result.Success = false;
                }
                return Json(result);
            });
           
        }

        /// <summary>
        /// 加载数据库连接列表
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDbConfigList()
        {
            return ExecuteFunctionRun(() =>
            {
                List<Dictionary<string, object>> lists = new List<Dictionary<string, object>>();
                OThinker.H3.Settings.BizDbConnectionConfig[] configs = this.Engine.SettingManager.GetBizDbConnectionConfigList();
                if (configs != null)
                {
                    foreach (OThinker.H3.Settings.BizDbConnectionConfig config in configs)
                    {
                        Dictionary<string, object> result = new Dictionary<string, object>();
                        result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_DbCode, config.DbCode);
                        result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_DisplayName, config.DisplayName);
                        //判断数据库类型
                        if (config.DbType != OThinker.Data.Database.DatabaseType.Unspecified)
                        {
                            result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_DbType, config.DbType);
                        }
                        else
                        {
                            result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_DbType, EditBizDbConfig_Default);
                        }

                        result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_DbConnectionString, config.DbConnectionString);
                        result.Add(OThinker.H3.Settings.BizDbConnectionConfig.PropertyName_Description, config.Description);
                        lists.Add(result);
                    }
                }
                var griddata = CreateLigerUIGridData(lists.ToArray());
                return Json(griddata);
            });
        }


        /// <summary>
        /// 删除数据库连接，如果正在由事务引用到，则无法删除
        /// </summary>
        /// <param name="ids">数据库编码字符串</param>
        /// <returns></returns>
        public JsonResult DeleteDbConfig(string ids)
        {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult(true, "");
                try
                {
                    if (string.IsNullOrWhiteSpace(ids))
                    {
                        return null;
                    }
                    ids = ids.TrimEnd(';');
                    string[] idList = ids.Split(';');
                    foreach (string code in idList)
                    {
                        this.Engine.SettingManager.RemoveBizDbConnectionConfig(code);
                    }
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                    result.Success = false;
                }

                return Json(result);
            });
        }

        private string GetValue(string str, string key)
        {
            string conStr = str;
            if (!conStr.EndsWith(";")) conStr += ";";
            var reg = new Regex(@key + @"\s*=\s*(?<key>.*?);", RegexOptions.IgnoreCase);
            Match mc = reg.Match(conStr);
            return mc.Groups["key"].Value;
        }

        #region 校验连接
        private bool CheckConnection(BizDbConfigViewModel model)
        {
            bool result;
            OThinker.Data.Database.CommandFactory factory = null;
            OThinker.Data.Database.DatabaseType dbType = model.DbType;

            factory = new CommandFactory(dbType, this.GetConnectionStr(model));
            result = factory.CheckConnection();

            if (!result)
            {
                //this.ShowWarningMessage(this.PortalResource.GetString("EditBizDbConfigDetail_Msg0"));
                return false;
            }
            return true;
        }
        #endregion

        #region 获取连接串
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private string GetConnectionStr(BizDbConfigViewModel model)
        {
            return OThinker.Data.Database.Database.GetConnectionString(model.DbType,
                 model.ServerName,
                 model.DBName,
                 model.UserName,
                 model.Password);
        }
        #endregion
    }
}
