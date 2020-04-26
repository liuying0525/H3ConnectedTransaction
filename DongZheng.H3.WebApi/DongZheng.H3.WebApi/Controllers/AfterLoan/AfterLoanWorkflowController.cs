using DongZheng.H3.WebApi.Common;
using OThinker.H3.Instance;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OThinker.H3.Data;

namespace DongZheng.H3.WebApi.Controllers.AfterLoan
{    
    [Xss]
    public class AfterLoanWorkflowController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public static string host = ConfigurationManager.AppSettings["RabbitMQHost"] + string.Empty;
        public static string user = ConfigurationManager.AppSettings["RabbitMQUser"] + string.Empty;
        public static string password = ConfigurationManager.AppSettings["RabbitMQPassword"] + string.Empty;

        RabbitMQHelper rabbitMQHelper = new RabbitMQHelper(host, user, password);

        [HttpPost]
        public JsonResult UpdateState(string InstanceId,string Code,string Message)
        {
            rtn_data rtn = new rtn_data();
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            if (context == null)
            {
                rtn.code = -1;
                rtn.message = "instanceid错误或不存在";
                return Json(rtn);
            }
            string type = "";
            if (context.BizObjectSchemaCode == "TQHK")
            {
                type = "2";
            }
            else if (context.BizObjectSchemaCode == "HKKBG")
            {
                type = "3";
            }
            else if (context.BizObjectSchemaCode == "LPHHQ")
            {
                type = "4";
            }
            else if (context.BizObjectSchemaCode == "XGSJH")
            {
                type = "1";
            }
            else
            {
                rtn.code = -1;
                rtn.message = "流程错误";
                return Json(rtn);
            }
            InstanceData instanceData = new InstanceData(AppUtility.Engine, InstanceId, "");
            var msg = new
            {
                id = instanceData["id"].Value + string.Empty,
                code = Code,
                message = Message,
                time = DateTime.Now.ToString(),
                type = type
            };
            string str_msg = JsonConvert.SerializeObject(msg);
            AppUtility.Engine.LogWriter.Write($"【MQ】【h3_to_wxapp_changeState】开始发送消息：{str_msg}");
            rabbitMQHelper.SendMsg(str_msg, "h3_to_wxapp_changeState");
            AppUtility.Engine.LogWriter.Write($"【MQ】【h3_to_wxapp_changeState】结束发送消息：{str_msg}");
            rtn.code = 1;
            rtn.message = "发送成功";
            rtn.data = InstanceId;
            return Json(rtn);
        }

        [HttpPost]
        public JsonResult InstanceState(string InstanceId)
        {
            rtn_data rtn = new rtn_data();
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            if (context == null)
            {
                rtn.code = -1;
                rtn.message = "InstanceId错误或不存在";
                rtn.data = InstanceId;
            }
            else
            {
                rtn.code = 1;
                rtn.message = "Success";
                rtn.data = (int)context.State + string.Empty;
            }
            return Json(rtn);
        }

        [HttpPost]
        public JsonResult AddCancleComment(string WorkitemId, string DataField,string Comment)
        {
            rtn_data rtn = new rtn_data();
            if (string.IsNullOrEmpty(WorkitemId) || string.IsNullOrEmpty(DataField) || string.IsNullOrEmpty(Comment))
            {
                rtn.code = -1;
                rtn.message = "参数为空";
                return Json(rtn);
            }
            var item = AppUtility.Engine.WorkItemManager.GetWorkItem(WorkitemId);
            if (item == null)
            {
                rtn.code = -1;
                rtn.message = "WorkitemId错误";
                return Json(rtn);
            }
            string id= Guid.NewGuid() + string.Empty;
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            Comment comment = new Comment();
            comment.ObjectID = id;
            comment.BizObjectSchemaCode = context.BizObjectSchemaCode;
            comment.BizObjectId = context.BizObjectId;
            comment.InstanceId = item.InstanceId;
            comment.Activity = item.ActivityCode;
            comment.UserID = this.UserValidator.UserID;
            comment.UserName = this.UserValidator.UserName;
            comment.OUName = this.UserValidator.DepartmentName;
            comment.TokenId = item.TokenId;
            comment.DataField = DataField;
            comment.Text = Comment;
            comment.Approval = OThinker.Data.BoolMatchValue.False;
            comment.CreatedTime = DateTime.Now;
            comment.ModifiedTime = DateTime.Now;
            comment.ItemType = 0;
            AppUtility.Engine.BizObjectManager.AddComment(comment);
            rtn.code = 1;
            rtn.message = "Success";
            rtn.data = id;
            return Json(rtn);
        }

        [HttpPost]
        public JsonResult SetItemValue(string InstanceId, string ItemCode, string ItemValue)
        {
            try
            {
                var context = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
                var result = AppUtility.Engine.BizObjectManager.SetPropertyValue(context.BizObjectSchemaCode, context.BizObjectId, "", ItemCode, ItemValue);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
    }
}