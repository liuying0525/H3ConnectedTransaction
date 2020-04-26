using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Sheets
{
    public class InstanceSheetsController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        private bool _IsMobile = false;
        private bool IsMobile
        {
            get
            {
                return this._IsMobile;
            }
            set
            {
                _IsMobile = value;
            }
        }

        private string _InstanceID = string.Empty;
        /// <summary>
        /// 获取流程实例ID
        /// </summary>
        private string InstanceID
        {
            get
            {
                return _InstanceID;
            }
            set
            {
                _InstanceID = value;
            }
        }
        Instance.InstanceContext _InstanceContext;
        Instance.InstanceContext InstanceContext
        {
            get
            {
                if (this._InstanceContext == null && !string.IsNullOrEmpty(this.InstanceID))
                {
                    this._InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                }
                return this._InstanceContext;
            }
        }
        Sheet.BizSheet[] _AllSheets;
        Sheet.BizSheet[] AllSheets
        {
            get
            {
                if (this._AllSheets == null)
                {
                    this._AllSheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(InstanceContext.BizObjectSchemaCode);
                }
                return this._AllSheets;
            }
        }

        Sheet.BizSheet DefaultSheet
        {
            get
            {
                if (this.AllSheets != null && this.AllSheets.Length > 0)
                {
                    return this.AllSheets[0];
                }
                return null;
            }
        }


        /// <summary>
        /// 打开流程表单
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public JsonResult InstanceSheets(string paramString)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                Dictionary<string, string> dicParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString);
                bool isMobile = false;
                foreach (string key in dicParams.Keys)
                {
                    if (key == Param_InstanceId) { InstanceID = dicParams[key]; continue; }
                    if (key == Param_IsMobile)
                    {
                        bool.TryParse(dicParams[key], out isMobile);
                        IsMobile = isMobile;
                        continue;
                    }
                }

                if (InstanceContext == null)
                {
                    result.Message = "InstanceSheets_InstanceNotExist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 获取流程模板信息
                WorkflowTemplate.PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetPublishedTemplate(InstanceContext.WorkflowCode, InstanceContext.WorkflowVersion);
                if (workflow == null)
                {
                    result.Message = "InstanceSheets_WorkflowNotExist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //WorkflowVersion,InstanceID,UserID
                DataTable dtWorkItem = this.GetItemTable(InstanceID);
                string activity, sheetCode;
                Dictionary<string, string> sheetWorkItemIds = new Dictionary<string, string>();
                //当前用户参与过流程时，则打开参与过的流程表单，否则打开系统流程表单
                if (dtWorkItem != null && dtWorkItem.Rows.Count > 0)
                {
                    foreach (DataRow row in dtWorkItem.Rows)
                    {
                        activity = row[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty;
                        sheetCode = this.GetSheetCodeByActivity(((WorkflowTemplate.ClientActivity)workflow.GetActivityByCode(activity)));
                        if (!sheetWorkItemIds.ContainsKey(sheetCode))
                        {
                            sheetWorkItemIds.Add(sheetCode, row[WorkItem.WorkItem.PropertyName_ObjectID].ToString());
                        }
                    }
                }
                else
                { // 管理员，未参与过流程
                    foreach (WorkflowTemplate.Activity act in workflow.Activities)
                    {
                        if (act is WorkflowTemplate.ClientActivity)
                        {
                            sheetCode = this.GetSheetCodeByActivity(((WorkflowTemplate.ClientActivity)act));
                            if (!string.IsNullOrEmpty(sheetCode) && !sheetWorkItemIds.ContainsKey(sheetCode))
                            {
                                sheetWorkItemIds.Add(sheetCode, string.Empty);
                            }
                        }
                    }
                }
                if (sheetWorkItemIds.Count == 0)
                {
                    Instance.InstanceContext instanceContext = InstanceContext;
                    if (instanceContext != null)
                    {
                        // 未参与过流程，并且未设置默认表单，那么再获取默认表单
                        Sheet.BizSheet[] sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(instanceContext.BizObjectSchemaCode);
                        if (sheets != null && sheets.Length > 0)
                        {
                            foreach (Sheet.BizSheet sheet in sheets)
                            {
                                if (!string.IsNullOrEmpty(sheet.SheetCode) && !sheetWorkItemIds.ContainsKey(sheet.SheetCode))
                                {
                                    sheetWorkItemIds.Add(sheet.SheetCode, string.Empty);
                                }
                            }
                        }
                    }
                }
                WorkItem.WorkItem workItem = null;
                WorkItem.CirculateItem circulateItem = null;
                string url, workItemId;
                List<ListUrl> ListUrl = new List<ListUrl>();
                foreach (string key in sheetWorkItemIds.Keys)
                {
                    Sheet.BizSheet sheet = this.Engine.BizSheetManager.GetBizSheetByCode(key);
                    workItemId = sheetWorkItemIds[key];
                    if (workItemId == string.Empty)
                    {
                        url = this.GetViewSheetUrl(
                            sheet,
                            InstanceID,
                            SheetMode.View,
                            IsMobile);
                    }
                    else
                    {
                        workItem = this.Engine.WorkItemManager.GetWorkItem(workItemId);
                        if (workItem != null)
                        {
                            url = this.GetViewSheetUrl(
                                    workItem,
                                    sheet,
                                    SheetMode.View,
                                    this.IsMobile);
                        }
                        else
                        {
                            circulateItem = this.Engine.WorkItemManager.GetCirculateItem(workItemId);
                            if (circulateItem == null) continue;
                            url = this.GetViewSheetUrl(
                                    circulateItem,
                                    sheet,
                                    SheetMode.View,
                                    this.IsMobile);
                        }

                    }
                    // 处理缓存
                    DateTime t = DateTime.Now;
                    url += "&T=" + t.ToString("HHmmss");
                    //只有一个表单直接跳转打开
                    if (sheetWorkItemIds.Count == 1)
                    {
                        result.Success = true;
                        result.Message = url;
                        result.Extend = "Redirect";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else //多个表单在界面上呈现连接选择打开
                    {
                        ListUrl.Add(new ListUrl
                        {
                            Title = "InstanceSheets_Sheet",
                            Text = string.Format("{0}[{1}]", sheet.DisplayName, key),
                            Url = url
                        });
                    }
                }
                result.Success = true;
                result.Message = "MultiSheets";
                result.Extend = ListUrl;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取活动对应的表单
        /// </summary>
        /// <param name="Activity"></param>
        /// <returns></returns>
        string GetSheetCodeByActivity(WorkflowTemplate.Activity Activity)
        {
            if (this.DefaultSheet != null && Activity is WorkflowTemplate.ClientActivity)
            {
                string sheetCode = ((WorkflowTemplate.ClientActivity)Activity).SheetCode;
                if (string.IsNullOrEmpty(sheetCode))
                {
                    return DefaultSheet.SheetCode;
                }
                return sheetCode;
            }
            return null;
        }

        private DataTable GetItemTable(string InstanceID)
        {
            DataTable dt1 = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion, new[] { InstanceID }, new[] { this.UserValidator.UserID }, WorkItem.WorkItem.TableName);
            DataTable dt2 = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion, new[] { InstanceID }, new[] { this.UserValidator.UserID }, WorkItem.WorkItemFinished.TableName);
            DataTable dt3 = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion, new[] { InstanceID }, new[] { this.UserValidator.UserID }, WorkItem.CirculateItem.TableName);
            DataTable dt4 = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion, new[] { InstanceID }, new[] { this.UserValidator.UserID }, WorkItem.CirculateItemFinished.TableName);

            string[] columns = new string[] { WorkItem.WorkItem.PropertyName_ActivityCode, WorkItem.WorkItem.PropertyName_ObjectID };
            DataTable dtWorkItem = new DataTable();
            foreach (string col in columns)
            {
                dtWorkItem.Columns.Add(col);
            }

            foreach (DataRow row in dt1.Rows)
            {
                DataRow newRow = dtWorkItem.NewRow();
                newRow[WorkItem.WorkItem.PropertyName_ActivityCode] = row[WorkItem.WorkItem.PropertyName_ActivityCode];
                newRow[WorkItem.WorkItem.PropertyName_ObjectID] = row[WorkItem.WorkItem.PropertyName_ObjectID];
                dtWorkItem.Rows.Add(newRow);
            }
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = dtWorkItem.NewRow();
                newRow[WorkItem.WorkItem.PropertyName_ActivityCode] = row[WorkItem.WorkItem.PropertyName_ActivityCode];
                newRow[WorkItem.WorkItem.PropertyName_ObjectID] = row[WorkItem.WorkItem.PropertyName_ObjectID];
                dtWorkItem.Rows.Add(newRow);
            }
            foreach (DataRow row in dt3.Rows)
            {
                DataRow newRow = dtWorkItem.NewRow();
                newRow[WorkItem.WorkItem.PropertyName_ActivityCode] = row[WorkItem.WorkItem.PropertyName_ActivityCode];
                newRow[WorkItem.WorkItem.PropertyName_ObjectID] = row[WorkItem.WorkItem.PropertyName_ObjectID];
                dtWorkItem.Rows.Add(newRow);
            }
            foreach (DataRow row in dt4.Rows)
            {
                DataRow newRow = dtWorkItem.NewRow();
                newRow[WorkItem.WorkItem.PropertyName_ActivityCode] = row[WorkItem.WorkItem.PropertyName_ActivityCode];
                newRow[WorkItem.WorkItem.PropertyName_ObjectID] = row[WorkItem.WorkItem.PropertyName_ObjectID];
                dtWorkItem.Rows.Add(newRow);
            }
            return dtWorkItem;
        }

    }

    public class ListUrl
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
