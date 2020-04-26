using Newtonsoft.Json;
using OThinker.H3.Sheet;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.StartIntance
{
    public class StartInstanceController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult StartInstance(string paramString)
        {
            ActionResult result = new ActionResult(false, "");
            Dictionary<string, string> dicParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString);
            string WorkflowCode = string.Empty, SchemaCode = string.Empty, InstanceName = string.Empty; int WorkflowVersion = WorkflowTemplate.WorkflowDocument.NullWorkflowVersion; bool IsMobile = false;
            string SheetCode = string.Empty; string LoginName = string.Empty; string MobileToken = string.Empty;
            string WechatCode = string.Empty; string EngineCode = string.Empty;
            Dictionary<string, string> dicOtherParams = new Dictionary<string, string>();

            //读取URL参数
            foreach (string key in dicParams.Keys)
            {
                if (key == Param_WorkflowCode) { WorkflowCode = dicParams[key]; continue; }
                if (key == Param_SchemaCode) { SchemaCode = dicParams[key]; continue; }
                if (key == Param_WorkflowVersion) { int.TryParse(dicParams[key], out WorkflowVersion); continue; }
                if (key == Param_IsMobile) { bool.TryParse(dicParams[key], out IsMobile); continue; }
                if (key == Param_SheetCode) { SheetCode = dicParams[key]; continue; }
                if (key == Param_InstanceName) { InstanceName = dicParams[key]; continue; }
                if (key.ToLower() == "loginname") { LoginName = dicParams[key]; continue; }
                if (key.ToLower() == "mobiletoken") { MobileToken = dicParams[key]; continue; }
                if (key.ToLower() == "code") { WechatCode = dicParams[key]; }
                if (key.ToLower() == "state") { EngineCode = dicParams[key]; }
                dicOtherParams.Add(key, dicParams[key]);
            }
            //TODO:微信不需要做单点登录
            ////实现微信单点登录
            //if (!string.IsNullOrEmpty(WechatCode) && !string.IsNullOrEmpty(EngineCode)
            //    && System.Web.HttpContext.Current.Session[Sessions.GetUserValidator()] != null)
            //{
            //    IsMobile = true;
            //    UserValidatorFactory.LoginAsWeChat(EngineCode, WechatCode);
            //}
            //APP打开表单验证
            if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(MobileToken) && this.UserValidator == null)
            {
                if (!SSOopenSheet(LoginName, MobileToken))
                {
                    result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else if (this.UserValidator == null)
            {
                result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //计算 WorkflowCode
            if (WorkflowCode == string.Empty && !string.IsNullOrEmpty(SchemaCode))
            {
                WorkflowClause[] clauses = this.Engine.WorkflowManager.GetClausesBySchemaCode(SchemaCode);
                if (clauses != null)
                {
                    foreach (WorkflowClause clause in clauses)
                    {
                        if (clause.DefaultVersion > 0)
                        {
                            WorkflowCode = clause.WorkflowCode;
                        }
                    }
                }
            }
            //流程版本号
            if (WorkflowVersion == WorkflowTemplate.WorkflowDocument.NullWorkflowVersion || WorkflowVersion == 0)
            {
                WorkflowVersion = this.Engine.WorkflowManager.GetWorkflowDefaultVersion(WorkflowCode);
            }

            //流程优先级
            OThinker.H3.Instance.PriorityType Priority = OThinker.H3.Instance.PriorityType.Normal;
            if (dicParams.ContainsKey(Param_Priority))
            {
                string strPriority = dicParams[Param_Priority];

                Priority = (OThinker.H3.Instance.PriorityType)Enum.Parse(typeof(OThinker.H3.Instance.PriorityType), strPriority);
            }

            try
            {
                if (string.IsNullOrEmpty(WorkflowCode))
                {
                    result.Message = "StartInstance.StartInstance_Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                // 获得工作流模板
                PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetPublishedTemplate(WorkflowCode, WorkflowVersion);
                if (workflow == null)
                {
                    result.Message = "StartInstance.StartInstance_Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workflow.BizObjectSchemaCode);
                // 开始节点
                ClientActivity startActivity = null;
                // 开始的表单
                BizSheet startSheet = null;
                System.Text.StringBuilder instanceParamBuilder = new System.Text.StringBuilder();
                if (dicOtherParams.Count > 0)
                {
                    foreach (string key in dicOtherParams.Keys)
                    {
                        instanceParamBuilder.Append(string.Format("&{0}={1}", key, dicOtherParams[key]));
                    }
                }

                startActivity = workflow.GetActivityByCode(workflow.StartActivityCode) as ClientActivity;
                if (!string.IsNullOrEmpty(SheetCode))
                {
                    startSheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);
                }
                if (startSheet == null)
                {
                    if (!string.IsNullOrEmpty(startActivity.SheetCode))
                    {
                        startSheet = this.Engine.BizSheetManager.GetBizSheetByCode(startActivity.SheetCode);
                    }
                    else
                    {
                        BizSheet[] sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(workflow.BizObjectSchemaCode);
                        if (sheets != null && sheets.Length > 0)
                        {
                            startSheet = sheets[0];
                        }
                    }
                }

                if (workflow.StartWithSheet &&
                    startActivity != null &&
                    startSheet != null)
                {
                    string url = this.GetSheetBaseUrl(
                        SheetMode.Originate,
                        IsMobile,
                        startSheet);
                    url = url +
                        SheetEnviroment.Param_Mode + "=" + SheetMode.Originate + "&" +
                        Param_WorkflowCode + "=" + System.Web.HttpUtility.UrlEncode(workflow.WorkflowCode) + "&" +
                        Param_WorkflowVersion + "=" + workflow.WorkflowVersion +
                        //(string.IsNullOrEmpty(SheetCode)?"":("&"+Param_SheetCode+"="+SheetCode))+
                        (instanceParamBuilder.Length == 0 ? string.Empty : ("&" + instanceParamBuilder.ToString()));
                    //URL
                    result.Message = url;
                }
                else
                {
                    // 发起流程
                    string instanceId = Guid.NewGuid().ToString().ToLower();
                    string workItemId = null;
                    string error = null;
                    OThinker.H3.DataModel.BizObject bo = new DataModel.BizObject(this.UserValidator.Engine, schema, this.UserValidator.UserID);
                    bo.Create();

                    if (!SheetUtility.OriginateInstance(
                        this.Engine,
                        bo.ObjectID,
                        workflow,
                        schema,
                        ref instanceId,
                        this.UserValidator.UserID,
                        null,
                        null,
                        OThinker.H3.Instance.PriorityType.Unspecified,
                        null,
                        null,
                        null,
                        ref workItemId,
                        ref error, false))
                    {
                        result.Message = error;
                    }
                    string startInstanceUrl = this.GetInstanceUrl(instanceId, workItemId);
                    startInstanceUrl += (instanceParamBuilder.Length == 0 ? null : ("&" + instanceParamBuilder.ToString()));
                    result.Message = startInstanceUrl;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
