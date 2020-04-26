using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class WorflowHistoryController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; ; }
        }
        /// <summary>
        /// 获取流程版本
        /// </summary>
        /// <param name="worflowCode"></param>
        /// <returns></returns>
        public JsonResult GetData(string worflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                Dictionary<string, string> publisher = new Dictionary<string, string>();
                List<Dictionary<string, object>> historyList = new List<Dictionary<string, object>>();
                Dictionary<string, object> data = new Dictionary<string, object>();
                WorkflowTemplate.PublishedWorkflowTemplateHeader[] wfTmpHeaders = this.Engine.WorkflowManager.GetPublishedTemplateHeaders(worflowCode);
                if (wfTmpHeaders != null)
                {
                    wfTmpHeaders = wfTmpHeaders.OrderByDescending(p => p.WorkflowVersion).ToArray();
                    foreach (WorkflowTemplate.PublishedWorkflowTemplateHeader header in wfTmpHeaders)
                    {
                        data = new Dictionary<string, object>();
                        data.Add("WorkflowCode", header.WorkflowCode);
                        data.Add("WorkflowVersion", header.WorkflowVersion);
                        if (!publisher.ContainsKey(header.Publisher))
                        {
                            publisher.Add(header.Publisher, this.Engine.Organization.GetName(header.Publisher));
                        }
                        data.Add("Publisher", publisher[header.Publisher]);
                        data.Add("PublishTime", header.PublishTime);
                        historyList.Add(data);
                    }
                }
             
                var gridData = CreateLigerUIGridData(historyList.ToArray());

                return Json(gridData,JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 删除流程版本
        /// </summary>
        /// <param name="worflowCode"></param>
        /// <param name="versionString"></param>
        /// <returns></returns>
        public JsonResult DeleteVersion(string worflowCode, string versionString)
        {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult(false,"");
                try
                {
                    string[] versions = versionString.Split(';');
                    foreach (string version in versions)
                    {
                        if (string.IsNullOrEmpty(version))
                        {
                            continue;
                        }
                        this.Engine.WorkflowManager.RemoveWorkflowTemplate(worflowCode, Int32.Parse(version));
                    }
                }
                catch (Exception ex)
                {
                    result.Message = "Error:"+ex.Message;
                    return Json(result,JsonRequestBehavior.AllowGet);
                }
                
                result.Success = true;
                result.Message = "WorkflowHistory.WorkflowSetting_RemoveVersionSuccess";
                return Json(result,JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 判断流程包ID锁定判断和处理
        /// </summary>
        /// <param name="BizObjectSchemaCode"></param>
        /// <returns>0，锁定，1未锁定</returns>
        [HttpGet]
        public JsonResult GetIsControlUsable(string BizObjectSchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {

                int s = BizWorkflowPackageLockByID(BizObjectSchemaCode);

                return Json(s,JsonRequestBehavior.AllowGet);
            });

        }
    }
}
