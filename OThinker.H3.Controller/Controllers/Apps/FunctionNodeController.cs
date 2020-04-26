using Newtonsoft.Json;
using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.Apps;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using OThinker.H3.Controllers.Controllers.Admin.Handler;

namespace OThinker.H3.Controllers.Controllers.Apps
{
    public class FunctionNodeController : ControllerBase
    {
        /// <summary>
        /// 权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_Apps_Code; }
        }

        /// <summary>
        /// 获取功能节点信息
        /// </summary>
        /// <param name="id">功能ID</param>
        /// <param name="appCode">应用编码</param>
        /// <param name="parentCode">父节点编码</param>
        /// <returns>功能节点信息</returns>
        [HttpGet]
        public JsonResult GetFunctionNode(string id, string appCode, string parentCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                FunctionNode functionNode = this.Engine.FunctionAclManager.GetFunctionNode(id);
                //父节点不为空,则取父节点名称
                if (!string.IsNullOrEmpty(parentCode)) appCode = parentCode;
                string parentName = GetParentName(appCode);
                //获取功能节点信息
                if (functionNode != null && !string.IsNullOrEmpty(functionNode.Code))
                {
                    FunctionUrlViewModel functionUrl = new FunctionUrlViewModel();
                    try
                    {
                        functionUrl = ConvertUrl(functionNode.Url);
                    }
                    catch (Exception e)
                    {
                        functionUrl = new FunctionUrlViewModel()
                        {
                            PageUrl = "0"
                        };
                    }
                    FunctionViewModel model = new FunctionViewModel()
                    {
                        ObjectID = functionNode.ObjectID,
                        Code = functionNode.Code,
                        DisplayName = functionNode.DisplayName,
                        Description = functionNode.Description,
                        SortKey = functionNode.SortKey,
                        OpenNewWindow = functionNode.OpenNewWindow,
                        IconUrl = functionNode.IconUrl,
                        Url = functionNode.Url,
                        IconCss = functionNode.IconCss,
                        IconType = (int)functionNode.IconType,
                        ParentName = parentName,
                        PageUrl = functionUrl.PageUrl,
                        FunctionUrl = functionUrl

                    };
                    result.Success = true;
                    result.Extend = new
                    {
                        FunctionNode = model,
                        PageUrl = InitPageUrl(),
                        WorkItemState = InitWorkItemState(),
                        InstanceState = InitInstanceState(),
                        ReportCode = InitReportCodeAndReporting(),
                        ReportTree=InitReportCodeAndReportingN(),
                        QueryCodes = GetQueries(model.FunctionUrl.SchemaCode),
                        SheetCodes = GetSheets(model.FunctionUrl.SchemaCode)
                    };
                }
                else if (!string.IsNullOrEmpty(appCode))
                {
                    result.Success = true;
                    var workItemState = InitWorkItemState();
                    var instanceState = InitInstanceState();
                    var reportCode = InitReportCodeAndReporting();
                    var ReportTree = InitReportCodeAndReportingN();
                    FunctionUrlViewModel functionUrl = new FunctionUrlViewModel()
                    {
                        WorkItemState = workItemState.FirstOrDefault().Value,
                        InstanceState = instanceState.FirstOrDefault().Value,
                        ReportCode = reportCode.FirstOrDefault().Value
                    };
                    result.Extend = new
                    {
                        FunctionNode = new FunctionViewModel() { ParentName = parentName, ParentCode = appCode, PageUrl = "0", FunctionUrl = functionUrl },
                        PageUrl = InitPageUrl(),
                        WorkItemState = workItemState,
                        InstanceState = instanceState,
                        ReportCode = reportCode,
                        ReportTree = ReportTree,
                        QueryCodes = new { },
                        SheetCodes = new { }
                    };
                }
                else
                {
                    result.Success = false;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 保存功能节点
        /// </summary>
        /// <param name="model">功能节点模型</param>
        /// <param name="appCode"应用编码></param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveFunctionNode(string functionNodeStr, string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                FunctionViewModel model = JsonConvert.DeserializeObject<FunctionViewModel>(functionNodeStr);
                string FunctionID = model.ObjectID;
                string ParentCode = model.ParentCode;
                string FunctionCode = model.Code;
                string Name = model.DisplayName;
                string Description = model.Description;
                int SortKey = model.SortKey;
                bool OpenNewWindow = model.OpenNewWindow;
                string IconUrl = model.IconUrl;
                string IconCss = model.IconCss;
                int IconType = model.IconType;
                string Url = model.Url;

                //TopAppCode全局Url参数，必须要有，且值为所在一级目录的编码
                //此处要做参数是否存在，以及参数的正确性
                if (!string.IsNullOrEmpty(Url))
                {
                    if (Url.IndexOf("({") > 0)
                    {
                        if (Url.IndexOf("TopAppCode:") > 0)
                        {
                            //正确性
                            string[] urlArry = Url.Split(new string[] { "TopAppCode:" }, StringSplitOptions.RemoveEmptyEntries);
                            if (urlArry.Length > 2)
                            {
                                //参数重复
                                result.Message = "FunctionNode.ParemRepeat";
                                result.Success = false;
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            if (urlArry[1].IndexOf(',') == -1)
                            {
                                Url = urlArry[0] + "TopAppCode:" + "\"" + model.TopAppCode + "\"})";
                            }
                            else
                            {
                                string[] rightArry = urlArry[1].Split(',');
                                Url = urlArry[0] + "TopAppCode:\"" + model.TopAppCode + "\"," + rightArry[1];
                                if (rightArry.Length > 2)
                                {
                                    for (int i = 2; i < rightArry.Count(); i++)
                                    {
                                        Url = Url + "," + rightArry[i];
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] urlArry = Url.Split(new string[] { "({" }, StringSplitOptions.RemoveEmptyEntries);
                            Url = urlArry[0] + "({TopAppCode:\"" + model.TopAppCode + "\"," + urlArry[1];
                        }
                    }
                    else if (Url.ToLower().IndexOf("http://") == -1)
                    {
                        //存在性
                        Url += "({TopAppCode:\"" + model.TopAppCode + "\"})";
                    }
                }

                if (string.IsNullOrEmpty(FunctionCode))
                {
                    result.Message = "FunctionNode.CodeNeed";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                FunctionNode F = null;
                if (!string.IsNullOrEmpty(FunctionID))
                {
                    F = this.Engine.FunctionAclManager.GetFunctionNode(FunctionID);
                }
                else
                {
                    F = new FunctionNode()
                    {
                        ParentCode = ParentCode,
                        Code = FunctionCode
                    };
                }

                if (F == null)
                {
                    result.Message = "msgGlobalString.NullObjectException";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                F.OpenNewWindow = OpenNewWindow;
                F.DisplayName = Name;
                F.Description = Description;
                F.SortKey = SortKey;
                F.IconUrl = IconUrl;
                F.Url = Url;
                F.NodeType = FunctionNodeType.Function;
                F.IconCss = IconCss;
                F.IconType = (Acl.IconType)IconType;

                long isSuccess = ErrorCode.SUCCESS;
                if (string.IsNullOrEmpty(FunctionID))
                {
                    isSuccess = this.Engine.FunctionAclManager.AddFunctionNode(F);
                }
                else
                {
                    isSuccess = this.Engine.FunctionAclManager.UpdateFunctionNode(F);
                }

                if (isSuccess == ErrorCode.SUCCESS)
                {
                    AppNavigation app = this.Engine.AppNavigationManager.GetApp(appCode);
                    if (app != null)
                    {
                        this.Engine.AppNavigationManager.UpdateApp(app);
                    }
                    result.Success = true;
                    result.Extend = F.ObjectID;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (isSuccess == ErrorCode.CodeIsExists)
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.CodeDuplicate";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.CodeDuplicate";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            });
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>权限列表</returns>
        [HttpPost]
        public JsonResult GetFunctionAclList(string code, PagerInfo pageInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                DataTable dt = Engine.Query.QueryFunctionAcl(code);
                Dictionary<string, string> fullNames = GetUnitNamesFromTable(dt, new string[] { FunctionAcl.PropertyName_UserID });
                int total = dt.Rows.Count;
                List<FunctionAclViewModel> list = new List<FunctionAclViewModel>();

                foreach (DataRow row in dt.Rows)
                {
                    FunctionAclViewModel result = new FunctionAclViewModel();
                    result.UserID = fullNames[row[FunctionAcl.PropertyName_UserID] + string.Empty];
                    result.Run = (row[FunctionAcl.PropertyName_Run] + string.Empty) == "1";
                    result.ObjectID = row[FunctionAcl.PropertyName_ObjectID] + string.Empty;
                    list.Add(result);
                };
                list = list.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="code">编码</param>
        /// <param name="appCode">应用编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveFunctionAcl(string[] userIds, string code, string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                if (userIds == null || userIds.Length ==0)
                {
                    // 没有选中的用户
                    result.Message = "msgGlobalString.SelectUser";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                for (int i = 0; i < userIds.Length; i++)
                {
                    // 验证是否有权限
                    if (!this.UserValidator.ValidateOrgAdmin(userIds[i]))
                    {
                        result.Message = "FunctionAcl.NotEnoughAuthorization";
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    OThinker.H3.Acl.FunctionAcl acl = new OThinker.H3.Acl.FunctionAcl(
                        userIds[i],
                       code,
                        true,
                        this.UserValidator.UserID);

                    // 添加
                    if (!this.Engine.FunctionAclManager.Add(acl))
                    {
                        //因改为一次可添加多人 暂时去掉已存在节点添加失败的提醒
                        // 添加失败
                        //result.Success = false;
                        //result.Extend = this.Engine.Organization.GetName(userIds[i]);
                        //result.Message = "FunctionNode.AddAclFailed";
                    }
                    else
                    {
                        result.Success = true;

                    } 
                }
                
                UpdateModifiedTime(code, appCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="ids">权限ID</param>
        /// <param name="code">编码</param>
        /// <param name="appCode">应用编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelFunctionAcl(string ids, string code, string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (string.IsNullOrWhiteSpace(ids))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectItem";
                }
                else
                {
                    ids = ids.TrimEnd(',');
                    string[] idList = ids.Split(',');
                    // 删除系统权限
                    foreach (string list in idList)
                    {
                        this.Engine.FunctionAclManager.Delete(list);
                    }
                    UpdateModifiedTime(code, appCode);
                    result.Success = true;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取查询列表和表单列表
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>查询列表和表单列表</returns>
        [HttpPost]
        public JsonResult GetSheetsAndQueries(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<Item> dicQueries = GetQueries(schemaCode);
                List<Item> dicSheets = GetSheets(schemaCode);
                result.Extend = new
                {
                    Queries = dicQueries,
                    Sheets = dicSheets
                };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 获取查询列表
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>查询列表</returns>
        private List<Item> GetQueries(string schemaCode)
        {
            BizQuery[] Queries = this.Engine.BizObjectManager.GetBizQueries(schemaCode);
            List<Item> dicQueries = new List<Item>();
            if (Queries != null && Queries.Length > 0)
            {
                foreach (BizQuery query in Queries)
                {
                    dicQueries.Add(new Item(query.DisplayName, query.QueryCode));
                }
            }
            return dicQueries;
        }

        /// <summary>
        /// 获取表单列表
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>表单列表</returns>
        private List<Item> GetSheets(string schemaCode)
        {
            BizSheet[] Sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(schemaCode);
            List<Item> dicSheets = new List<Item>();
            if (Sheets != null && Sheets.Length > 0)
            {
                foreach (BizSheet sheet in Sheets)
                {
                    dicSheets.Add(new Item(sheet.DisplayName, sheet.SheetCode));
                }
            }
            return dicSheets;
        }


        /// <summary>
        /// 更新AppNavigation的ModifiedTime
        /// </summary>
        private void UpdateModifiedTime(string code, string appCode)
        {
            AppNavigation app = this.Engine.AppNavigationManager.GetApp(code);
            if (app != null)
            {
                ////给App设置权限时，更新App的ModifiedTime
                this.Engine.AppNavigationManager.UpdateApp(app);
            }
            else
            {
                //给App的应用菜单设置权限时，更新App的ModifiedTime
                app = this.Engine.AppNavigationManager.GetApp(appCode);
                if (app != null)
                {
                    this.Engine.AppNavigationManager.UpdateApp(app);
                }
            }
        }

        /// <summary>
        /// 获取父节点名称
        /// </summary>
        /// <param name="parentCode">父节点编码</param>
        /// <returns>父节点名称</returns>
        private string GetParentName(string parentCode)
        {
            FunctionNode parentNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(parentCode);
            return parentNode == null ? "" : parentNode.DisplayName;
        }

        /// <summary>
        /// 初始化报表数据选项qiancheng
        /// </summary>
        /// <returns>报表数据下拉框选项</returns>
        private List<Item> InitReportCodeAndReporting()
        {
            Analytics.Reporting.ReportTemplate[] ReportNodes = this.Engine.Analyzer.GetAllReportTemplates();
            OThinker.Reporting.ReportPage[] ReportNode = this.Engine.ReportManager.GetAllReportPage();
            List<Item> list = new List<Item>();
            if (ReportNodes != null && ReportNodes.Length > 0)
            {
                foreach (Analytics.Reporting.ReportTemplate r in ReportNodes)
                {
                    list.Add(new Item(r.DisplayName, r.Code)); 
                }
            }
            if (ReportNode != null && ReportNode.Length > 0)
            {
                var NewReportNode = ReportNode.OrderBy(s => s.DisplayName);
                foreach (OThinker.Reporting.ReportPage re in NewReportNode)
                {
                    list.Add(new Item(re.DisplayName, re.Code));
                }
            }
            return list;
        }

        private List<PortalTreeNode> InitReportCodeAndReportingN()
        {
            var result = new SysDeployment.SysDeploymentController().GetReportTreeNodes(this);
            return result;
        }

        /// <summary>
        /// 初始化报表数据选项
        /// </summary>
        /// <returns>报表数据下拉框选项</returns>
        private List<Item> InitReportCode()
        {
            Analytics.Reporting.ReportTemplate[] ReportNodes = this.Engine.Analyzer.GetAllReportTemplates();
            List<Item> list = new List<Item>();
            if (ReportNodes != null && ReportNodes.Length > 0)
            {
                foreach (Analytics.Reporting.ReportTemplate r in ReportNodes)
                {
                    list.Add(new Item(r.DisplayName, r.Code));

                }
            }
            return list;
        }

        /// <summary>
        /// 初始化流程状态选项
        /// </summary>
        /// <returns>流程状态选项</returns>
        private List<Item> InitInstanceState()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item("msgGlobalString.UnFinished", OThinker.H3.Instance.InstanceState.Unfinished.ToString()));
            list.Add(new Item("msgGlobalString.Finished", OThinker.H3.Instance.InstanceState.Finished.ToString()));
            return list;
        }

        /// <summary>
        /// 初始化工作流程状态选项
        /// </summary>
        /// <returns>工作流程状态选项</returns>
        private List<Item> InitWorkItemState()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item("msgGlobalString.UnAsigned", OThinker.H3.WorkItem.WorkItemState.Unspecified.ToString()));
            list.Add(new Item("msgGlobalString.UnFinished", OThinker.H3.WorkItem.WorkItemState.Unfinished.ToString()));
            list.Add(new Item("msgGlobalString.Finished", OThinker.H3.WorkItem.WorkItemState.Finished.ToString()));
            return list;
        }

        /// <summary>
        /// 初始化连接地址选项
        /// </summary>
        /// <returns>连接地址选项</returns>
        private List<Item> InitPageUrl()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item("FunctionNode.BizQuery", "app.BizQueryView"));
            list.Add(new Item("FunctionNode.WorkItemList", "app.MyWorkItem"));
            list.Add(new Item("FunctionNode.InstanceList", "app.MyInstance"));
            list.Add(new Item("FunctionNode.Report", "app.ShowReport"));
            list.Add(new Item("FunctionNode.Sheet", "app.EditBizObject"));
            list.Add(new Item("FunctionNode.UserDefined", "0"));
            return list;
        }

        /// <summary>
        /// 解析URL中的数据
        /// </summary>
        /// <param name="url">目标地址</param>
        /// <returns>Url参数模型</returns>
        private FunctionUrlViewModel ConvertUrl(string url)
        {
            FunctionUrlViewModel model = new FunctionUrlViewModel();
            //截取pageUrl
            if (url.IndexOf('(') > 0)
                model.PageUrl = url.Substring(0, url.IndexOf('('));
            else model.PageUrl = "0";
            if (model.PageUrl != "0")
            {
                //截取参数,反序列化为动态对象,并赋值
                int startIndex = url.IndexOf('{');
                int endIndex = url.IndexOf('}') + 1;
                if (startIndex > 0 && endIndex > 0)
                {
                    string paramStr = url.Substring(startIndex, endIndex - startIndex);
                    dynamic param = JsonConvert.DeserializeObject(paramStr);
                    //解析动态对象
                    switch (model.PageUrl)
                    {
                        case "app.BizQueryView":
                            model.SchemaCode = param.SchemaCode;
                            model.QueryCode = param.QueryCode;
                            model.FunctionCode = param.FunctionCode;
                            break;
                        case "app.MyWorkItem":
                            model.SchemaCode = param.SchemaCode;
                            model.WorkItemState = param.State;
                            model.FunctionCode = param.FunctionCode;
                            break;
                        case "app.MyInstance":
                            model.SchemaCode = param.SchemaCode;
                            model.InstanceState = param.State;
                            model.FunctionCode = param.FunctionCode;
                            break;
                        case "app.ShowReport":
                            model.ReportCode = param.ReportCode;
                            break;
                        case "app.EditBizObject":
                            model.SchemaCode = param.SchemaCode;
                            model.SheetCode = param.SheetCode;
                            model.FunctionCode = param.FunctionCode;
                            model.Mode = param.Mode;
                            break;
                        default:
                            model.PageUrl = "0";
                            break;
                    }
                }
            }
            return model;
        }

    }
}
