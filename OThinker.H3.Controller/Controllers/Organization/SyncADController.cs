using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    public class SyncADController:ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Settings_Code; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SyncADController() { }

        /// <summary>
        /// 获取组织同步页面设置信息
        /// </summary>
        /// <returns>返回组织同步设置信息</returns>
        public JsonResult GetSyncADInfo()
        {
            SyncADViewModel model = new SyncADViewModel();
            //活动目录路径:
            model.Route = this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_ADPathes);
            //密码
            model.Password = this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_ADPassword);
            //用户名
            model.UserName = this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_ADUser);
            //上一次加载时间
            model.LastSyncTime = H3.Settings.CustomSetting.GetLastSyncTime(this.Engine.SettingManager).ToString();
            if (DateTime.Parse(model.LastSyncTime) < new DateTime(1900, 1, 1))
            {
                model.LastSyncTime = "未同步";
            }
            //自动同步时间
            model.AutoSynchTime = this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_ADSyncTimes);
            //自动重新加载时间
            model.AutoReloadTime = this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_OrgReloadTimes);

            //启用AD认证
            model.ADValidator = OThinker.H3.Settings.CustomSetting.GetEnableADValidation(this.Engine.SettingManager);

            return Json(model,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存更新组织同步设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns>保存更新结果</returns>
        public JsonResult SaveSyncADInfo(SyncADViewModel model)
        {
            ActionResult result = new ActionResult(false,"");
            try
            {
                // 域
                this.Engine.SettingManager.SetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_ADPathes, model.Route);
                this.Engine.SettingManager.SetCustomSetting(H3.Settings.CustomSetting.Setting_ADUser, model.UserName);
                if (model.Password != null && model.Password != "")
                {
                    this.Engine.SettingManager.SetCustomSetting(H3.Settings.CustomSetting.Setting_ADPassword, model.Password);
                }

                OThinker.H3.Settings.CustomSetting.SetEnableADValidation(this.Engine.SettingManager, model.ADValidator);
                OThinker.H3.Settings.CustomSetting.SetADSyncTimes(this.Engine.SettingManager, model.AutoSynchTime);

                OThinker.H3.Settings.CustomSetting.SetOrgReloadTimes(this.Engine.SettingManager, model.AutoReloadTime);

                result.Success = true;
                result.Message = "msgGlobalString.SaveSucced";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "msgGlobalString.SaveFailed";
                result.Extend = ex.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 测试AD连接是否成功
        /// </summary>
        /// <param name="model"></param>
        /// <returns>连接成功返回成功对话框，连接失败返回失败错误信息</returns>
        public JsonResult TestADConnection(SyncADViewModel model)
        {
            ActionResult result = new ActionResult(false, "");

            if (string.IsNullOrEmpty(model.Route) ||
                string.IsNullOrEmpty(model.UserName) ||
                string.IsNullOrEmpty(model.Password))
            {
                result.Message = "SyncAD.ConnectFailed";
            }
            else
            { 
                string message = "";

                //OThinker.Organization.ADAdapter adapter = new OThinker.Organization.ADAdapter(this.Engine.Organization, this.Engine.LogWriter);
                //adapter.Sync(new string[] { "LDAP://OU=奥哲科技,DC=authine,DC=com" }, "OThinker\\zztb", "h3password");

                //测试连接
                if (!OThinker.Organization.ADAdapter.ADConnect(model.Route, model.UserName, model.Password, out message))
                {
                    result.Extend = message;

                    result.Success = false;
                    result.Message = "SyncAD.ConnectFailed";
                    return Json(result);
                }

                result.Success = true;
                result.Message = "SyncAD.ConnectSucess";
                
            }

            return Json(result);
        }

        /// <summary>
        /// 增量同步
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncAD()
        {
            //上一次同步时间
            DateTime UTCLastSyncTime = H3.Settings.CustomSetting.GetUTCLastSyncTime(this.Engine.SettingManager);

            JsonResult result = DoSyncAD(UTCLastSyncTime);
            return result;
        }

        /// <summary>
        /// 完整同步
        /// </summary>
        /// <returns></returns>
        public JsonResult SyncAllAD()
        {
            JsonResult result = DoSyncAD( DateTime.MinValue);
            return result;
        }

        /// <summary>
        /// 同步AD
        /// </summary>
        /// <param name="model">AD账户信息</param>
        /// <param name="UTCLastSyncTime">上一次同步时间</param>
        /// <returns>同步结果</returns>
        private JsonResult DoSyncAD( DateTime UTCLastSyncTime)
        {
            ActionResult result = new ActionResult(false, "");

            string[] paths = OThinker.H3.Settings.CustomSetting.GetADPathes(this.Engine.SettingManager);
            string AdUser =this.Engine.SettingManager.GetCustomSetting(H3.Settings.CustomSetting.Setting_ADUser);
            string AdPassword=this.Engine.SettingManager.GetCustomSetting(H3.Settings.CustomSetting.Setting_ADPassword);

            //首先判断设置是否已经存在
            if (paths!= null && paths.Length > 0)
            {
                this.Engine.LogWriter.Write("SyncAD: paths-" + string.Join(";", paths)+"->ADUSer:"+AdUser+"->Password:"+AdPassword);
                System.DateTime lastSyncTime = System.DateTime.Now;//同步时间
                OThinker.Organization.HandleResult resultHander = this.Engine.Organization.Sync(
                    paths,
                    AdUser,
                    AdPassword,
                    UTCLastSyncTime);

                if (resultHander == OThinker.Organization.HandleResult.SUCCESS)
                {
                    H3.Settings.CustomSetting.SetLastSyncTime(this.Engine.SettingManager, lastSyncTime);
                    result.Success = true;
                    result.Message= "SyncAD.Msg0";
                   
                }
                else
                {
                    result.Message = "Orgnization." + "Orgnization_"+resultHander; //返回处理结果字符串对应的多语言文字
                   
                }
            }
            else
            {
                result.Message= "SyncAD.PleaseSetPath";
              
            }

            return Json(result);
        }

        
    }
}
