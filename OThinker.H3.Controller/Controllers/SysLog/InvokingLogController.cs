using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysLog
{
    /// <summary>
    /// 集成日志控制器
    /// </summary>
    [Authorize]
    public class InvokingLogController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysLog_IntegrationLog_Code;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public InvokingLogController() { }

        /// <summary>
        /// 获取集成日志显示数据
        /// </summary>
        /// <param name="pagerInfo">页码信息</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="serverCode">服务编码</param>
        /// <returns>集成日志数据</returns>
        [HttpPost]
        public JsonResult GetInvokingLogList(PagerInfo pagerInfo, string startTime, string endTime, string serverCode)
        {
            return ExecuteFunctionRun(() =>
           {
               int total = 0;
               //查询集成日志
               System.Data.DataTable dt = this.QueryInvokingLog(pagerInfo.PageIndex, pagerInfo.PageSize, startTime, endTime, serverCode, out total);
               List<InvokingLogViewModel> list = new List<InvokingLogViewModel>();
               int endIndex = dt.Rows.Count;
               Dictionary<string, string> fullNames = this.GetUnitFullNamesFromTable(dt, new string[] { InvokingLog.PropertyName_UserId });
               //构造返回前端数据
               for (int i = 0; i < endIndex; i++)
               {
                   Dictionary<string, object> result = new Dictionary<string, object>();
                   string id = dt.Rows[i][InvokingLog.PropertyName_UserId] + string.Empty;
                   list.Add(new InvokingLogViewModel()
                   {
                       ObjectID = dt.Rows[i][InvokingLog.PropertyName_ObjectID] + string.Empty,
                       BizServiceCode = dt.Rows[i][InvokingLog.PropertyName_BizServiceCode] + string.Empty,
                       MethodName = dt.Rows[i][InvokingLog.PropertyName_MethodName] + string.Empty,
                       StartTime = dt.Rows[i][InvokingLog.PropertyName_StartTime] + string.Empty,
                       EndTime = dt.Rows[i][InvokingLog.PropertyName_EndTime] + string.Empty,
                       UsedTime = GetUsedTimeFromStr(dt.Rows[i][InvokingLog.PropertyName_UsedTime] + string.Empty),
                       UserID = this.GetValueFromDictionary(fullNames, id)
                   });
               }
               var griddata = new { Rows = list, Total = total };
               return Json(griddata, JsonRequestBehavior.AllowGet);
           });
        }

        /// <summary>
        /// 获取集成日志详情
        /// </summary>
        /// <param name="id">集成日志ID</param>
        /// <returns>集成日志详情</returns>
        [HttpGet]
        public JsonResult GetInvokingLogDetail(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                OThinker.H3.BizBus.BizService.InvokingLog log = this.Engine.BizBus.GetInvokingLog(id);
                if (log == null)
                {
                    result.Message = "";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string paramXml = log.ParamXml;
                paramXml = paramXml == null ? null : paramXml.Replace("></", ">" + Environment.NewLine + "</");
                string returnXml = log.ReturnXml;
                returnXml = returnXml == null ? null : returnXml.Replace("></", ">" + Environment.NewLine + "</");
                InvokingLogDetailViewModel model = new InvokingLogDetailViewModel()
                {
                    User = this.Engine.Organization.GetFullName(log.UserId),
                    ServiceCode = log.BizServiceCode,
                    CreatedTime = log.CreatedTime.ToString(),
                    EndTime = log.EndTime.ToString(),
                    UsedTime = ((double)(log.UsedTime.Ticks / 1000) / 10000.0).ToString(),
                    SlaveCode = log.SlaveCode,
                    ExceptionMessage = log.ExceptionMessage,
                    ExceptionStack = log.ExceptionStack,
                    MethodName = log.MethodName,
                    ParamXml = paramXml,
                    ReturnXml = returnXml,
                    TranId = log.TransactionId

                };
                result.Success = true;
                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 将毫秒转化为秒
        /// </summary>
        /// <param name="sTicks">毫秒数</param>
        /// <returns>秒数</returns>
        private string GetUsedTimeFromStr(string sTicks)
        {
            long ticks = 0;
            if (long.TryParse(sTicks, out ticks))
            {
                return ((ticks / 1000)/10000.0).ToString();
            }
            else
            {
                return sTicks;
            }
        }
        /// <summary>
        /// 查询集成日志
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">大小</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="serverCode">服务编码</param>
        /// <param name="total">返回记录总数</param>
        /// <returns>返回集成日志数据表</returns>
        private DataTable QueryInvokingLog(int pageIndex, int pageSize, string startTime, string endTime, string serverCode, out int total)
        {
            OThinker.Data.Database.QueryConditonCollection conditions = new OThinker.Data.Database.QueryConditonCollection();
            DateTime start;
            //构造查询条件
            if (DateTime.TryParse(startTime, out start))
            {
                conditions.Add(ComparisonOperatorType.NotBelow, InvokingLog.PropertyName_CreatedTime, start, this.Engine.EngineConfig.LogDBType);
            }
            DateTime end;
            if (DateTime.TryParse(endTime, out end))
            {
                end = end.AddDays(1);
                conditions.Add(ComparisonOperatorType.NotAbove, InvokingLog.PropertyName_CreatedTime, end, Engine.EngineConfig.LogDBType);
            }
            if (!string.IsNullOrWhiteSpace(serverCode))
            {
                conditions.Add(ComparisonOperatorType.Contain, InvokingLog.PropertyName_BizServiceCode, serverCode, this.Engine.EngineConfig.LogDBType);
            }
            return this.Engine.Query.QueryInvokingLog(conditions.ToArray(), pageIndex, pageSize, out total);
        }
    }
}