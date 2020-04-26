using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using OThinker.H3.Exceptions;
using System.Data;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.Data.Database;
using OThinker.H3.Instance;

namespace OThinker.H3.Controllers.Controllers.SysLog
{
    /// <summary>
    /// 异常日志控制器
    /// </summary>
    [Authorize]
    public class ExceptionLogController : ControllerBase
    {
        /// <summary>
        /// 获取菜单权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysLog_ExceptionLog_Code;
            }
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        public ExceptionLogController() { }

        /// <summary>
        /// 获取异常日志数据集合
        /// </summary>
        /// <param name="pagerInfo">页码信息</param>
        /// <param name="startTime">开始事件</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="instanceName">实例名称</param>
        /// <param name="sequenceNo">流水号</param>
        /// <param name="userId">用户ID</param>
        /// <returns>异常日志Json对象</returns>
        [HttpPost]
        public JsonResult GetExceptionLogList(PagerInfo pagerInfo, string startTime, string endTime, string instanceName, string sequenceNo, string userId)
        {
            return ExecuteFunctionRun(() =>
             {
                 int total = 0;
                 //查询结果集(数据库返回对象)
                 DataTable dtException = QueryExceptionTable(pagerInfo.PageIndex, pagerInfo.PageSize,
                     startTime, endTime, instanceName, sequenceNo, userId, out total);

                 int endIndex = dtException.Rows.Count;
                 List<ExceptionLogViewModel> gridData = new List<ExceptionLogViewModel>();
                 Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtException, new string[] { ExceptionLog.PropertyName_Originator });
                 for (int i = 0; i < endIndex; i++)
                 {
                     //构造前端显示对象(json对象)
                     gridData.Add(new ExceptionLogViewModel()
                     {
                         ObjectID = dtException.Rows[i][ExceptionLog.PropertyName_ObjectID] + string.Empty,
                         Workflow = dtException.Rows[i][ExceptionLog.PropertyName_WorkflowCode] + string.Empty,
                         SourceName = dtException.Rows[i][ExceptionLog.PropertyName_SourceName] + string.Empty,
                         Originator = this.GetValueFromDictionary(unitNames,
                                            dtException.Rows[i][ExceptionLog.PropertyName_Originator] + string.Empty),
                         SequenceNo = dtException.Rows[i][ExceptionLog.PropertyName_SequenceNo] + string.Empty,
                         ExceptionTime = dtException.Rows[i][ExceptionLog.PropertyName_ThrownTime] + string.Empty,
                         Block = int.Parse(dtException.Rows[i][ExceptionLog.PropertyName_Block] + string.Empty),
                         State = int.Parse(dtException.Rows[i][ExceptionLog.PropertyName_State] + string.Empty),
                     });
                 }
                 var result = new { Rows = gridData, Total = total };

                 return Json(result, JsonRequestBehavior.AllowGet);
             });
        }

        /// <summary>
        /// 获取异常详情
        /// </summary>
        /// <param name="id">异常ID</param>
        /// <returns>异常详情</returns>
        [HttpGet]
        public JsonResult GetExceptionLogDetail(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ExceptionDetailViewModel model = new ExceptionDetailViewModel();
                bool isFixed = false;//是否已修复
                OThinker.H3.Exceptions.ExceptionLog log = this.Engine.ExceptionManager.GetException(id);
                if (log != null)
                {
                    model.Action = log.Action.ToString();
                    model.Block = log.Block.ToString();
                    OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(log.InstanceId);
                    if (context == null)
                    {
                        result.Message = "ExceptionLog.WorkflowNotExist";
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    model.InstanceName = string.IsNullOrWhiteSpace(context.InstanceName) ? context.InstanceId : context.InstanceName;
                    model.InstanceUrl = this.GetInstanceUrl(log.InstanceId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    model.Message = log.Message.Replace("\r\n", "<BR>");
                    if (log.State == OThinker.H3.Exceptions.ExceptionState.Fixed)
                    {
                        model.Fix = "ExceptionLog.Fixed";
                        model.FixTime = log.FixedTime.ToString();
                        isFixed = true;
                    }
                    else
                    {
                        model.Fix = "ExceptionLog.UnFixed";
                        model.FixTime = null;
                        isFixed = false;
                    }
                    model.ObjectID = log.ObjectID;
                    model.WorkflowPackage = log.WorkflowCode;
                    model.WorkflowVersion = log.WorkflowVersion.ToString();
                    model.SourceName = log.SourceName;
                    model.ExceptionSource = log.SourceType.ToString();
                    model.OccurrenceTime = log.ThrownTime.ToString();
                }
                result.Success = true;
                result.Extend = new
                {
                    IsFixed = isFixed,
                    ExceptionDetail = model
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 修复异常
        /// </summary>
        /// <param name="id"异常ID></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FixException(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                OThinker.H3.Exceptions.ExceptionLog log = this.Engine.ExceptionManager.GetException(id);

                //查找到异常环节
                OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(log.InstanceId);
                Token ExceptionToken = null;
                foreach (Token t in context.Tokens)
                {
                    if (t.Exceptional)
                    {
                        ExceptionToken = t;
                        break;
                    }
                }

                //修复
                if (log.State != OThinker.H3.Exceptions.ExceptionState.Fixed)
                    this.Engine.ExceptionManager.FixException(id);

                System.Threading.Thread.Sleep(5000);

                //重新激活
                if (ExceptionToken != null)
                {
                    OThinker.H3.Messages.ActivateActivityMessage activateMessage
                        = new OThinker.H3.Messages.ActivateActivityMessage(
                            OThinker.H3.Messages.MessageEmergencyType.Normal,
                            context.InstanceId, ExceptionToken.Activity,
                            ExceptionToken.TokenId,
                            ExceptionToken.Participants,
                            null,
                            false,
                            H3.WorkItem.ActionEventType.Adjust);

                    this.Engine.InstanceManager.SendMessage(activateMessage);
                }
                result.Success = true;
                log = this.Engine.ExceptionManager.GetException(id);
                result.Extend = new
                {
                    IsFixed = true,
                    ExceptionDetail = new ExceptionDetailViewModel()
                    {
                        Fix = "ExceptionLog.Fixed",
                        FixTime = log.FixedTime.ToShortDateString(),
                    }

                };

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取异常数据Table
        /// </summary>
        /// <param name="pageIndex">页数</param>
        /// <param name="pageSize">尺寸</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束事件</param>
        /// <param name="instanceName">实例名称</param>
        /// <param name="sequenceNo">流水号</param>
        /// <param name="userId">用户ID</param>
        /// <param name="total">总数</param>
        /// <returns>异常日志结果集</returns>
        private DataTable QueryExceptionTable(int pageIndex,
            int pageSize,
            string startTime,
            string endTime,
            string instanceName,
            string sequenceNo,
            string userId,
            out int total)
        {
            #region 构造查询参数
            QueryConditonCollection conditions = new QueryConditonCollection();

            DateTime startTimeValue = DateTime.MinValue;
            DateTime endTimeValue = DateTime.MinValue;
            if (DateTime.TryParse(startTime, out startTimeValue))
            {
                conditions.Add(ComparisonOperatorType.NotBelow,
                    ExceptionLog.PropertyName_ThrownTime,
                    startTimeValue,
                   this.Engine.EngineConfig.LogDBType);
            }

            if (DateTime.TryParse(endTime, out endTimeValue))
            {
                endTimeValue = endTimeValue.AddDays(1);
                conditions.Add(ComparisonOperatorType.NotAbove,
                  ExceptionLog.PropertyName_ThrownTime,
                  endTimeValue,
                  this.Engine.EngineConfig.LogDBType);
            }

            conditions.Add(ComparisonOperatorType.Contain,
                  ExceptionLog.PropertyName_SourceName,
                  instanceName);

            conditions.Add(ComparisonOperatorType.Contain,
                  ExceptionLog.PropertyName_SequenceNo,
                  sequenceNo);

            conditions.Add(ComparisonOperatorType.Equal,
                  ExceptionLog.PropertyName_Originator,
                  userId);
            #endregion
            return this.Engine.Query.QueryException(conditions.ToArray(), pageIndex, pageSize, out total);
        }
    }
}
