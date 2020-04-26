using DongZheng.H3.WebApi.Common.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class WorkFlowHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public string Index(string CommandName)
        {
            var context = HttpContext;// 当前HTTP请求的特定信息
            string result = "";
            switch (CommandName)
            {
                case "all": result = getAllWorkItemCount(context); break;
                case "UnfinishedWorkItemCount": result = getUnfinishedWorkItemCount(context); break;
                case "UnfinishedWorkItem": result = getUnfinishedWorkItem(context); break;
                case "FinishedWorkItemCount": result = getFinishedWorkItemCount(context); break;
                case "FinishedWorkItem": result = getFinishedWorkItem(context); break;
                case "UserUnReadedWorkItemCount": result = GetUserUnReadedWorkItemCount(context); break;
                case "UserUnReadedWorkItem": result = GetUserUnReadedWorkItem(context); break;
                case "UserReadedWorkItemCount": result = GetUserReadedWorkItemCount(context); break;
                case "UserReadedWorkItem": result = GetUserReadedWorkItem(context); break;

            }
            return result;
        }

        /// <summary>
        /// 获取用户的待办任务总数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getAllWorkItemCount(HttpContextBase context)
        {
            string workitem = "{\"result\":[";//{"":"","":""}
            workitem += "{\"UnfinishedWorkItemCount\":" + getUnfinishedWorkItemCount(context) + "," + "\"WorkItem\":" + getUnfinishedWorkItem(context) + "},";
            workitem += "{\"FinishedWorkItemCount\":" + getFinishedWorkItemCount(context) + "," + "\"WorkItem\":" + getFinishedWorkItem(context) + "},";
            workitem += "{\"UserUnReadedWorkItemCount\":" + GetUserUnReadedWorkItemCount(context) + "," + "\"WorkItem\":" + GetUserUnReadedWorkItem(context) + "},";
            workitem += "{\"UserReadedWorkItemCount\":" + GetUserReadedWorkItemCount(context) + "," + "\"WorkItem\":" + GetUserReadedWorkItem(context) + "}]}";
            return JSSerializer.Serialize(workitem);
        }

        public string all()
        {
            return getAllWorkItemCount(HttpContext);
        }

        /// <summary>
        /// 获取用户的待办任务总数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getUnfinishedWorkItemCount(HttpContextBase context)
        {
            int count = 0;
            string UserCode = context.Request["UserCode"];
            count = new WorkFlowFunction().GetUserUnfinishedWorkItemCount(UserCode);
            object obj = new
            {
                Result = count
            };
            return JSSerializer.Serialize(obj);
        }

        public string UnfinishedWorkItemCount()
        {
            return getUnfinishedWorkItemCount(HttpContext);
        }

        /// <summary>
        /// 查询用户的待办
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getUnfinishedWorkItem(HttpContextBase context)
        {
            string rtn = "";
            string UserCode = context.Request["UserCode"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int startIndex = 0, endIndex = 10;
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> lstModel = wf.GetUnFinishWorkItems(UserCode, null, null, startIndex, endIndex, "", "");
            rtn = CommonFunction.Obj2Json(lstModel);
            return JSSerializer.Serialize(rtn);
        }

        public string UnfinishedWorkItem()
        {
            return getUnfinishedWorkItem(HttpContext);
        }

        /// <summary>
        /// 获取用户的已办任务总数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getFinishedWorkItemCount(HttpContextBase context)
        {
            string count = "0";
            string UserCode = context.Request["UserCode"];
            count = new WorkFlowFunction().GetUserFinishedWorkItemCountBySql(UserCode);
            object obj = new
            {
                Result = count
            };
            return JSSerializer.Serialize(obj);
        }

        public string FinishedWorkItemCount()
        {
            return getFinishedWorkItemCount(HttpContext);
        }
        
        /// <summary>
        /// 查询用户的已办
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getFinishedWorkItem(HttpContextBase context)
        {
            string rtn = "";
            string UserCode = context.Request["UserCode"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int startIndex = 0, endIndex = 10;
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> lstModel = wf.GetFinishWorkItems(UserCode, null, null, startIndex, endIndex, "", "");
            rtn = CommonFunction.Obj2Json(lstModel);
            return JSSerializer.Serialize(rtn);
        }

        public string FinishedWorkItem()
        {
            return getFinishedWorkItem(HttpContext);
        }

        /// <summary>
        /// 获取用户的待阅任务总数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUserUnReadedWorkItemCount(HttpContextBase context)
        {
            string count = "0";
            string UserCode = context.Request["UserCode"];
            count = new WorkFlowFunction().GetUserUnReadWorkItemCountBySql(UserCode);
            object obj = new
            {
                Result = count
            };
            return JSSerializer.Serialize(obj);
        }

        public string UserUnReadedWorkItemCount()
        {
            return GetUserUnReadedWorkItemCount(HttpContext);
        }

        /// <summary>
        /// 查询用户的待阅任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUserUnReadedWorkItem(HttpContextBase context)
        {
            string rtn = "";
            string UserCode = context.Request["UserCode"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int startIndex = 0, endIndex = 10;
            List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> lstModel = wf.GetUnReadWorkItems(UserCode, null, null, startIndex, endIndex, "", "");
            rtn = CommonFunction.Obj2Json(lstModel);
            return JSSerializer.Serialize(rtn);
        }

        public string UserUnReadedWorkItem()
        {
            return GetUserUnReadedWorkItem(HttpContext);
        }

        /// <summary>
        /// 获取用户的已阅任务总数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUserReadedWorkItemCount(HttpContextBase context)
        {
            string count = "0";
            string UserCode = context.Request["UserCode"];
            count = new WorkFlowFunction().GetUserReadWorkItemCountBySql(UserCode);
            object obj = new
            {
                Result = count
            };
            return JSSerializer.Serialize(obj);
        }

        public string UserReadedWorkItemCount()
        {
            return GetUserReadedWorkItemCount(HttpContext);
        }

        /// <summary>
        /// 查询用户的已阅任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUserReadedWorkItem(HttpContextBase context)
        {
            string rtn = "";
            string UserCode = context.Request["UserCode"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int startIndex = 0, endIndex = 100;
            List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> lstModel = wf.GetReadWorkItems(UserCode, null, null, startIndex, endIndex, "", "");
            rtn = CommonFunction.Obj2Json(lstModel);
            return JSSerializer.Serialize(rtn);
        }

        public string UserReadedWorkItem()
        {
            return GetUserReadedWorkItem(HttpContext);
        }

        /// <summary>
        /// 查询用户的已阅任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUnReadWorkItems(HttpContextBase context)
        {
            string rtn = "";
            string UserCode = context.Request["UserCode"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int startIndex = 0, endIndex = 100;
            List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> lstModel = wf.GetUnReadWorkItems(UserCode, null, null, startIndex, endIndex, "", "");
            rtn = CommonFunction.Obj2Json(lstModel);
            return JSSerializer.Serialize(rtn);
        }

        public string UnReadWorkItems()
        {
            return GetUnReadWorkItems(HttpContext);
        }

        #region 序列化器

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private JavaScriptSerializer _JsSerializer = null;
        /// <summary>
        /// 获取JS序列化对象
        /// </summary>
        private JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new JavaScriptSerializer();
                }
                return _JsSerializer;
            }
        }

        #endregion

    }
}