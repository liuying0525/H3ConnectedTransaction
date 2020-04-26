using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using OThinker.H3.Site;
using OThinker.Organization;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Tracking;
using OThinker.Data.Database;
using OThinker.Data;

namespace OThinker.H3.Controllers.Controllers.SysLog
{
    /// <summary>
    /// 登陆日志控制器
    /// </summary>
    [Authorize]
    public class LoginLogController : ControllerBase
    {
        /// <summary>
        /// 获取权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysLog_LoginLog_Code;
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public LoginLogController() { }

        /// <summary>
        /// 获取登录日志数据集
        /// </summary>
        /// <param name="pagerInfo">页码信息</param>
        /// <param name="startTime">开始事件</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户ID</param>
        /// <param name="siteType">登录类型</param>
        /// <param name="loginIP">登录IP</param>
        /// <returns>登录日志json对象</returns>
        [HttpPost]
        public JsonResult GetLoginLogList(PagerInfo pagerInfo, string startTime, string endTime, string userIds, string siteTypes, string loginIP)
        {
            return ExecuteFunctionRun(() =>
            {
                int total = 0; 
                DataTable dtLoginLog = QueryUserLoginLog(startTime,
                    endTime,
                    userIds,
                    siteTypes,
                     loginIP,
                    pagerInfo.PageIndex,
                    pagerInfo.PageSize,
                    out total);

                List<LoginLogViewModel> lists = new List<LoginLogViewModel>();

                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtLoginLog, new string[] { UserLog.PropertyName_UserID });
                string id = string.Empty;
                string siteTypeTemp = string.Empty;
                int siteTypeInt = 0;
                for (int i = 0; i < dtLoginLog.Rows.Count; i++)
                {
                    id = dtLoginLog.Rows[i][Tracking.UserLog.PropertyName_UserID] + string.Empty;
                    if (int.TryParse(dtLoginLog.Rows[i][UserLog.PropertyName_SiteType] + string.Empty, out siteTypeInt))
                    {
                        siteTypeTemp = Enum.GetName(typeof(PortalType), siteTypeInt);
                    }
                    lists.Add(new LoginLogViewModel()
                    {
                        ObjectID = dtLoginLog.Rows[i][UserLog.PropertyName_ObjectID] + string.Empty,
                        UserName = this.GetValueFromDictionary(unitNames, id),
                        UserID = dtLoginLog.Rows[i][UserLog.PropertyName_UserCode] + string.Empty,
                        SiteType = "LoginLog." + siteTypeTemp,
                        LoginTime = this.GetValueFromDate(dtLoginLog.Rows[i][UserLog.PropertyName_CreatedTime], "yyyy-MM-dd HH:mm:ss"),
                        Browser = dtLoginLog.Rows[i][UserLog.PropertyName_ClientBrowser] + string.Empty,
                        PlatForm = dtLoginLog.Rows[i][UserLog.PropertyName_ClientPlatform] + string.Empty,
                        IP = dtLoginLog.Rows[i][UserLog.PropertyName_ClientAddress] + string.Empty,
                        ClientType = dtLoginLog.Rows[i][UserLog.PropertyName_ClientType] + string.Empty,
                    });
                }
                var griddata = new { Rows = lists, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取客户端类型下拉框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetSiteType()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                string[] PortalTypes = Enum.GetNames(typeof(PortalType));
                var items = PortalTypes.Select(s => new
                {
                    id = (int)Enum.Parse(typeof(PortalType), s),
                    text = "LoginLog." + s
                }).ToList();
                result.Success = true;
                result.Extend = items;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 查询用户登录日志
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">登录用户ID</param>
        /// <param name="siteType">客户端登录类型</param>
        /// <param name="loginIP">登录IP</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="total">返回记录总数</param>
        /// <returns>返回用户登录日志表格</returns>
        private System.Data.DataTable QueryUserLoginLog(string startTime, string endTime,
            string userIds, string siteTypes, string loginIP, int pageIndex, int pageSize, out int total)
        {
            QueryConditonCollection conditions = new QueryConditonCollection();
            DateTime start;
            if (DateTime.TryParse(startTime, out start))
            {
                conditions.Add(ComparisonOperatorType.NotBelow,
                  UserLog.PropertyName_CreatedTime,
                  start,
                 this.Engine.EngineConfig.LogDBType);
            }
            DateTime end;
            if (DateTime.TryParse(endTime, out end))
            {
                end = end.AddDays(1);
                conditions.Add(ComparisonOperatorType.NotAbove,
                  UserLog.PropertyName_CreatedTime,
                  end,
                 this.Engine.EngineConfig.LogDBType);
            }
            if (!string.IsNullOrEmpty(userIds))
            {
                string[] userIdarray = userIds.Split(','); 
                conditions.Add(ComparisonOperatorType.In,
                UserLog.PropertyName_UserID,
                userIdarray, this.Engine.EngineConfig.LogDBType);  
            }
            if (!string.IsNullOrEmpty(siteTypes))
            {
                string[] siteTypearray = siteTypes.Split(',');
                conditions.Add(ComparisonOperatorType.In,
                 UserLog.PropertyName_SiteType,
                 siteTypearray, this.Engine.EngineConfig.LogDBType);
            }
            if (!string.IsNullOrEmpty(loginIP))
            {
                conditions.Add(ComparisonOperatorType.Equal,
                 UserLog.PropertyName_ClientAddress,
                 loginIP, this.Engine.EngineConfig.LogDBType);
            }
            conditions.Add(ComparisonOperatorType.Equal,
                UserLog.PropertyName_LogType, ((int)UserLogType.Login).ToString(), this.Engine.EngineConfig.LogDBType);
            return this.Engine.Query.QueryUserLoginLog(conditions.ToArray(), pageIndex, pageSize, out total);
        }
    }
}