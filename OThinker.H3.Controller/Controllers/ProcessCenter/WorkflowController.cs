using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class WorkflowController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkflowController()
        {
        }

        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";//FunctionNode.Workspace_MyWorkflow_Code;
            }
        }
        /// <summary>
        /// 获取发起时流程状态信息
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="WorkflowVersion"></param>
        /// <returns></returns>
        public JsonResult GetWorkflowInfo(string WorkflowCode, int WorkflowVersion)
        {
            return this.ExecuteFunctionRun(() =>
            {
                WorkflowTemplate.PublishedWorkflowTemplate Workflow = this.Engine.WorkflowManager.GetPublishedTemplate(
                        WorkflowCode,
                        WorkflowVersion);
                if (Workflow == null)
                {
                    var result = new
                    {
                        SUCCESS = false,
                        Messages = "WorkflowInfo_WorkflowInfoNotExist"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DataTable dt = OThinker.H3.WorkflowTemplate.WorkflowUtility.GetSerialTable(Workflow);
                    //流程步骤
                    List<ActivitySteps> ActivitySteps = new List<H3.Controllers.ActivitySteps>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ActivitySteps.Add(new ActivitySteps
                        {
                            SN = row[0] + string.Empty,
                            Code = row[1] + string.Empty,
                            Name = row[2] + string.Empty,
                            Post = row[3] + string.Empty
                        });
                    }
                    var result = new
                    {
                        SUCCESS = true,
                        ActivitySteps = ActivitySteps
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            });
        }

        /// <summary>
        /// 获取所有可发起的流程模板
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="ShowFavorite"></param>
        /// <param name="IsMobile"></param>
        /// <param name="IsShared"></param>
        /// <param name="ParentCode"></param>
        /// <param name="SearchKey"></param>
        /// <param name="BizSchema">0不包含数据模型的权限，1包含数据模型的权限</param>
        /// <returns></returns>
        public JsonResult QueryWorkflowNodes(string Mode = "WorkflowTemplate",
            bool ShowFavorite = true,
            bool IsMobile = false,
            bool IsShared = false,
            string ParentCode = "",
            string SearchKey = "",
            bool Isbilingual = false,
            int BizSchema = 0)
        {
            return this.ExecuteFunctionRun(() =>
            {
                //获取所有有权限发起的流程模板
                DataTable dtworkflows = Engine.PortalQuery.QueryWorkflow(this.UserValidator.RecursiveMemberOfs, this.UserValidator.ValidateAdministrator());
                //根据可以发起的流程模板编码，倒推获取所有的节点集合
                List<string> aclWorkflowCodes = new List<string>();
                foreach (DataRow row in dtworkflows.Rows)
                {
                    if (aclWorkflowCodes.Contains(row[WorkItem.WorkItem.PropertyName_WorkflowCode])) continue;
                    if (IsShared && (row[WorkflowClause.PropertyName_IsShared] + string.Empty) != "1") continue;
                    aclWorkflowCodes.Add(row[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty);
                }

                #region WANGXG 10.19 将当前用户的，业务数据模型的权限，添加到允许查询流程模板列表中
                if(BizSchema == 1)
                {
                    string sql = $@"SELECT A.SCHEMACODE, C.NAME ORGSCOPE, B.NAME USR, B.CODE USR_CODE, A.ADMINISTRATOR, A.CREATEBIZOBJECT, A.VIEWDATA
FROM H3.OT_BIZOBJECTACL A 
LEFT JOIN H3.OT_USER B ON A.USERID=B.OBJECTID
LEFT JOIN H3.OT_ORGANIZATIONUNIT C ON A.ORGSCOPE = C.OBJECTID

LEFT JOIN H3.OT_GROUP Q ON A.USERID=Q.OBJECTID
LEFT JOIN H3.OT_GROUPCHILD W ON Q.OBJECTID = W.PARENTOBJECTID
LEFT JOIN H3.OT_USER Y ON W.CHILDID = Y.OBJECTID

LEFT JOIN H3.OT_ORGANIZATIONUNIT X ON A.USERID=X.OBJECTID
WHERE 1=1 AND (A.ADMINISTRATOR=1 OR A.CREATEBIZOBJECT=1 OR A.VIEWDATA=1 ) 
AND (B.CODE='{UserValidator.UserCode}' 
    OR Y.CODE = '{UserValidator.UserCode}' 
    OR EXISTS --UUID是当前用户所属组织
        (
            -- 联合组织和分组及用户
            SELECT DISTINCT R.CODE, P.CODE
            FROM H3.OT_ORGANIZATIONUNIT T
            LEFT JOIN H3.OT_GROUP M ON T.OBJECTID = M.PARENTID
            LEFT JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
            LEFT JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
            LEFT JOIN H3.OT_USER R ON R.PARENTID = T.OBJECTID
            WHERE  R.CODE='{UserValidator.UserCode}' OR P.CODE='{UserValidator.UserCode}' 
            START WITH T.OBJECTID = A.USERID -- UUID是组织节点
            CONNECT BY PRIOR T.OBJECTID = T.PARENTID
        )
    )";// WANGXG 19.10
                    DataTable dt = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (aclWorkflowCodes.Contains(row["SCHEMACODE"])) continue;
                            aclWorkflowCodes.Add(row["SCHEMACODE"] + string.Empty);
                        }
                    }
                }               
                #endregion

                //FunctionAclManager
                List<FunctionNode> nodes = Engine.WorkflowManager.GetParentNodesByWorkflowCodes(aclWorkflowCodes);
                if (nodes != null) nodes = nodes.OrderBy(s => s.DisplayName).OrderBy(s => s.SortKey).ToList();
                //获取所有的流程模板头，用于获取发布时间
                //WorkflowTemplateManager
                Dictionary<string, PublishedWorkflowTemplateHeader> dicHeaders = Engine.WorkflowManager.GetDefaultWorkflowHeaders(aclWorkflowCodes.ToArray());
                List<PublishedWorkflowTemplateHeader> headers = new List<PublishedWorkflowTemplateHeader>();
                //移动端可发起的流程编码
                List<string> mobileVisibleWorkflowCodes = new List<string>();
                //图标集<WorkflowCode.ImagePath>
                Dictionary<string, string> workflowIcons = new Dictionary<string, string>();
                Dictionary<string, string> workflowIconFileNames = new Dictionary<string, string>();

                if (IsMobile)
                {
                    #region 手机端 --------------
                    // 检测是否允许手机端发起
                    List<string> lstSchemaCodes = new List<string>();
                    foreach (PublishedWorkflowTemplateHeader header in dicHeaders.Values)
                    {
                        if (!lstSchemaCodes.Contains(header.BizObjectSchemaCode.ToLower()))
                        {
                            lstSchemaCodes.Add(header.BizObjectSchemaCode.ToLower());
                        }
                    }

                    Dictionary<string, WorkflowClause[]> dicClauses = this.Engine.WorkflowManager.GetClausesBySchemaCodes(lstSchemaCodes.ToArray());

                    foreach (string key in dicClauses.Keys)
                    {
                        foreach (WorkflowClause c in dicClauses[key])
                        {
                            if (c.MobileStart && !mobileVisibleWorkflowCodes.Contains(c.WorkflowCode.ToLower()))
                            {
                                mobileVisibleWorkflowCodes.Add(c.WorkflowCode.ToLower());
                                if (!string.IsNullOrEmpty(c.IconFileName))
                                {
                                    workflowIconFileNames.Add(c.WorkflowCode.ToLower(), c.IconFileName);
                                }
                                else if (!string.IsNullOrEmpty(c.Icon))
                                {
                                    workflowIcons.Add(c.WorkflowCode.ToLower(), c.Icon);
                                }
                            }
                        }
                    }
                    #endregion
                }

                foreach (PublishedWorkflowTemplateHeader header in dicHeaders.Values)
                {
                    if (!IsMobile || mobileVisibleWorkflowCodes.Contains(header.WorkflowCode.ToLower()))
                    {
                        headers.Add(header);
                    }
                }

                List<string> favoriteWorkflowCodes = this.Engine.WorkflowConfigManager.GetFavoriteWorkflowCodes(this.UserValidator.UserID);

                string parentCode = string.IsNullOrEmpty(ParentCode) ? OThinker.H3.Acl.FunctionNode.Category_ProcessModel_Code : ParentCode;
                if (Mode != "WorkflowTemplate")
                { // 如果是数据模型
                    List<FunctionNode> bizObjectNodes = null;
                    if (parentCode == OThinker.H3.Acl.FunctionNode.Category_ProcessModel_Code)
                    {
                        bizObjectNodes = this.Engine.FunctionAclManager.GetChildNodesByParentCode(OThinker.H3.Acl.FunctionNode.ProcessModel_BizMasterData_Code);
                        if (bizObjectNodes != null && bizObjectNodes.Count > 0)
                        {
                            bizObjectNodes.Add(new FunctionNode()
                            {
                                Code = OThinker.H3.Acl.FunctionNode.ProcessModel_BizMasterData_Code,
                                ParentCode = OThinker.H3.Acl.FunctionNode.Category_ProcessModel_Code,
                                NodeType = FunctionNodeType.BizFolder,
                                SortKey = -1,
                                DisplayName = "主数据"
                            });
                        }
                    }
                    else
                    {
                        bizObjectNodes = this.Engine.FunctionAclManager.GetChildNodesByParentCode(parentCode);
                    }
                    if (bizObjectNodes != null) nodes.AddRange(bizObjectNodes);
                }

                List<ViewModels.WorkflowNode> result = GetWorkflowNodeByParentCode(
                    parentCode,
                    SearchKey,
                    Mode == "WorkflowTemplate" ? FunctionNodeType.BizWorkflow : FunctionNodeType.BizWorkflowPackage,
                    ShowFavorite,
                    nodes,
                    favoriteWorkflowCodes,
                    aclWorkflowCodes,
                    headers,
                    workflowIcons,
                    workflowIconFileNames,
                    IsShared,
                    Isbilingual);
                if (result.Count > 0)
                {
                    //result.ar[0]
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取Workflows集合
        /// </summary>
        /// <param name="WorkflowCodes"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWorkflows(string[] WorkflowCodes)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<Item> results = new List<Item>();

                if (WorkflowCodes == null || WorkflowCodes.Length == 0)
                {
                    return Json(results, JsonRequestBehavior.DenyGet);
                }

                Dictionary<string, PublishedWorkflowTemplateHeader> headers = this.Engine.WorkflowManager.GetDefaultWorkflowHeaders(WorkflowCodes);
                if (headers != null)
                {
                    foreach (string key in headers.Keys)
                    {
                        results.Add(new Item()
                        {
                            Value = key,
                            Text = this.GetWorkflowDisplayName(key, headers[key].DisplayName)
                        });
                    }
                }

                List<string> masterDatas = new List<string>();
                foreach (string workflowCode in WorkflowCodes)
                {
                    if (headers == null || !headers.ContainsKey(workflowCode))
                    {
                        masterDatas.Add(workflowCode);
                    }
                }
                if (masterDatas.Count > 0)
                {// 从主数据获取
                    foreach (string masterCode in masterDatas)
                    {
                        FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(masterCode);
                        if (node != null)
                        {
                            results.Add(new Item() { Value = node.Code, Text = node.DisplayName });
                        }
                    }
                }

                return Json(results, JsonRequestBehavior.DenyGet);
            });
        }

        private string GetWorkflowDisplayName(string workflowCode, string displayName)
        {
            if (!string.IsNullOrEmpty(displayName)) return displayName;
            displayName = this.Engine.WorkflowManager.GetClauseDisplayName(workflowCode);
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = this.Engine.FunctionAclManager.GetFunctionNodeByCode(workflowCode).DisplayName;
            }
            return displayName;
        }

        /// <summary>
        /// 设置常用流程
        /// </summary>
        /// <param name="WorkflowCode">流程模板</param>
        /// <param name="value">设为常用？ true ：false</param>
        public JsonResult ChangeFrequence(string WorkflowCode, bool IsFrequence)
        {
            // wangxg 19.8 验证权限
            DataTable dtworkflows = Engine.PortalQuery.QueryWorkflow(this.UserValidator.RecursiveMemberOfs, this.UserValidator.ValidateAdministrator());
            bool auth = false;
            foreach (DataRow row in dtworkflows.Rows)
            {
                var tem = row[WorkItem.WorkItem.PropertyName_WorkflowCode];
                if (tem != null && tem != DBNull.Value && tem.ToString().ToLower() == WorkflowCode.ToLower())
                {
                    auth = true;
                    break;
                }
            }
            if (!auth) { return Json("无权限", JsonRequestBehavior.AllowGet); }

            return this.ExecuteFunctionRun(() =>
            {
                string userId = this.UserValidator.UserID;
                if (IsFrequence)
                {
                    Engine.WorkflowConfigManager.AddFavoriteWorkflow(userId, WorkflowCode);
                }
                else
                {
                    Engine.WorkflowConfigManager.DeleteFavoriteWorkflow(userId, WorkflowCode);
                }
                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            });
        }

        private string GetDisplayName(List<FunctionNode> nodes, string workflowCode, string displayName)
        {
            if (!string.IsNullOrEmpty(displayName)) return displayName;
            foreach (FunctionNode node in nodes)
            {
                if (node.Code == workflowCode)
                {
                    return node.DisplayName;
                }
            }
            return workflowCode;
        }

        private List<FunctionNode> GetChildNodesByParentCode(string parentCode, List<FunctionNode> nodes, FunctionNodeType targetNodeType)
        {
            if (targetNodeType == FunctionNodeType.BizWorkflowPackage)
            {
                List<FunctionNode> children = nodes.Where(p => p.ParentCode.ToLower() == parentCode.ToLower()).ToList();
                return children;
            }
            else
            {
                List<FunctionNode> results = new List<FunctionNode>();
                nodes = nodes.OrderBy(s => s.DisplayName).OrderBy(s => s.SortKey).ToList();
                foreach (FunctionNode node in nodes)
                {
                    if (node.NodeType == targetNodeType)
                    {
                        FunctionNode parentNode = nodes.Find(n => n.Code == node.ParentCode && n.NodeType == FunctionNodeType.BizWorkflowPackage);
                        if (parentNode != null && parentNode.ParentCode == parentCode)
                        {
                            node.SortKey = parentNode.SortKey;
                            results.Add(node);
                        }
                        if (parentNode != null && parentNode.ParentCode == FunctionNodeType.BizWorkflowPackage.ToString())
                        {
                            node.SortKey = parentNode.SortKey;
                            results.Add(node);
                        }
                    }
                    else if ((node.NodeType == FunctionNodeType.BizFolder || node.NodeType == FunctionNodeType.BizWFFolder)
                        && node.ParentCode == parentCode)
                    {
                        results.Add(node);
                    }
                }
                return results;
            }
        }

        public List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodeByParentCode(
          string parentCode,
          string searchKey,
          FunctionNodeType targetNodeType,
          bool ShowFavorite,
          List<FunctionNode> nodes,
          List<string> favoriteWorkflowCodes,
          List<string> aclWorkflowCodes,
          List<PublishedWorkflowTemplateHeader> headers,
          Dictionary<string, string> WorkflowIcons,
          Dictionary<string, string> WorkflowIconFileNames)
        {
            return GetWorkflowNodeByParentCode(parentCode, searchKey, targetNodeType, ShowFavorite, nodes, favoriteWorkflowCodes, aclWorkflowCodes, headers, WorkflowIcons, WorkflowIconFileNames, false, false);
        }

        public List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodeByParentCode(
           string parentCode,
           string searchKey,
           FunctionNodeType targetNodeType,
           bool ShowFavorite,
           List<FunctionNode> nodes,
           List<string> favoriteWorkflowCodes,
           List<string> aclWorkflowCodes,
           List<PublishedWorkflowTemplateHeader> headers,
           Dictionary<string, string> WorkflowIcons,
           Dictionary<string, string> WorkflowIconFileNames,
            bool IsShared,
            bool Isbilingual)
        {
            List<OThinker.H3.Controllers.ViewModels.WorkflowNode> WorkflowNodes = new List<OThinker.H3.Controllers.ViewModels.WorkflowNode>();

            List<FunctionNode> childrenNodes = this.GetChildNodesByParentCode(parentCode, nodes, targetNodeType);// nodes.Where(p => p.ParentCode.ToLower() == parentCode.ToLower()).ToList();
            if (childrenNodes == null || childrenNodes.Count == 0)
                return null;
            if (IsShared)
            {
                childrenNodes = childrenNodes.Where(s => s.NodeType != FunctionNodeType.BizObject).ToList();
            }
            System.Collections.ArrayList sortedChildrenNodes = new System.Collections.ArrayList();
            System.Collections.ArrayList keyList = new System.Collections.ArrayList();
            foreach (FunctionNode c in childrenNodes)
            {
                sortedChildrenNodes.Add(c);
                keyList.Add(c.SortKey);
            }
            sortedChildrenNodes = OThinker.Data.Sorter.Sort(keyList, sortedChildrenNodes);

            List<PublishedWorkflowTemplateHeader> tmpHeaders;

            //读取常用流程
            if (parentCode == FunctionNode.Category_ProcessModel_Code && ShowFavorite)
            {
                string DisplayName = string.Empty;
                string Icon = string.Empty;
                if (Isbilingual) { DisplayName = "常用流程"; Icon = "fa fa-folder-o"; } else { DisplayName = "FrequentFlow"; Icon = null; }
                OThinker.H3.Controllers.ViewModels.WorkflowNode FavoriteNode = new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                {
                    IsLeaf = false,
                    ObjectID = Guid.NewGuid().ToString(),
                    DisplayName = DisplayName,
                    children = new List<OThinker.H3.Controllers.ViewModels.WorkflowNode>(), 
                    Icon = Icon
                };
                foreach (string workflowcode in aclWorkflowCodes)
                {
                    tmpHeaders = FindTemplateHeaders(headers, workflowcode);
                    if (tmpHeaders != null)
                    {
                        foreach (PublishedWorkflowTemplateHeader tmpHeader in tmpHeaders)
                        {
                            if (favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0)
                            {
                                FavoriteNode.children.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                                {
                                    IsLeaf = true,
                                    ObjectID = tmpHeader.ObjectID,
                                    Code = tmpHeader.WorkflowCode,
                                    DisplayName = this.GetDisplayName(nodes, tmpHeader.WorkflowCode, tmpHeader.DisplayName),
                                    PublishedTime = tmpHeader.PublishTime.ToShortDateString(),
                                    Version = tmpHeader.WorkflowVersion,
                                    Frequent = 1,
                                    Icon = "icon-liuchengmoxing"
                                });
                            }
                        }
                    }
                }
                if (FavoriteNode.children.Count > 0)
                {
                    WorkflowNodes.Add(FavoriteNode);

                }
            }

            foreach (FunctionNode node in sortedChildrenNodes)
            {
                if (targetNodeType != node.NodeType && node.NodeType != FunctionNodeType.BizObject && node.NodeType != FunctionNodeType.BizFolder && node.NodeType != FunctionNodeType.BizWFFolder) continue;

                if (searchKey != string.Empty &&
                    (node.DisplayName.IndexOf(searchKey) == -1 && (node.NodeType == FunctionNodeType.BizWorkflowPackage || node.NodeType == FunctionNodeType.BizObject || node.NodeType == FunctionNodeType.BizWorkflow))) continue;

                switch (node.NodeType)
                {
                    case FunctionNodeType.BizWorkflowPackage:
                        tmpHeaders = FindTemplateHeaders(headers, node.Code);

                        WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                        {
                            IsLeaf = true,
                            ObjectID = node.ObjectID,// tmpHeader.ObjectID,
                            Code = node.Code,//.WorkflowCode,
                            DisplayName = node.DisplayName,// string.IsNullOrEmpty(tmpHeader.DisplayName) ? node.DisplayName : tmpHeader.DisplayName,
                            PublishedTime = DateTime.MinValue.ToShortDateString(),// tmpHeader.PublishTime,
                            Version = -1,// tmpHeader.WorkflowVersion,
                            Icon = "icon-liuchengshujumoxing",
                            Frequent = 0 //(favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0 ? 1 : 0)
                        });
                        break;
                    case FunctionNodeType.BizObject:
                        WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                        {
                            IsLeaf = true,
                            ObjectID = node.ObjectID,// tmpHeader.ObjectID,
                            Code = node.Code,//.WorkflowCode,
                            DisplayName = node.DisplayName,// string.IsNullOrEmpty(tmpHeader.DisplayName) ? node.DisplayName : tmpHeader.DisplayName,
                            PublishedTime = DateTime.MinValue.ToShortDateString(),// tmpHeader.PublishTime,
                            Version = -1,// tmpHeader.WorkflowVersion,
                            Icon = "icon-zhushujushili",
                            Frequent = 0 //(favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0 ? 1 : 0)
                        });
                        break;
                    case FunctionNodeType.BizWorkflow:
                        tmpHeaders = FindTemplateHeaders(headers, node.Code);
                        foreach (PublishedWorkflowTemplateHeader tmpHeader in tmpHeaders)
                        {
                            WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                            {
                                IsLeaf = true,
                                ObjectID = tmpHeader.ObjectID,
                                Code = tmpHeader.WorkflowCode,
                                DisplayName = string.IsNullOrEmpty(tmpHeader.DisplayName) ? node.DisplayName : tmpHeader.DisplayName,
                                PublishedTime = tmpHeader.PublishTime.ToShortDateString(),
                                Version = tmpHeader.WorkflowVersion,
                                Icon = "icon-liuchengmoxing",
                                Frequent = (favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0 ? 1 : 0)
                            });
                            headers.Remove(tmpHeader);
                        }
                        break;
                    default:
                        List<OThinker.H3.Controllers.ViewModels.WorkflowNode> nodeChildren = GetWorkflowNodeByParentCode(node.Code,
                            searchKey,
                            targetNodeType,
                            ShowFavorite,
                            nodes, favoriteWorkflowCodes, aclWorkflowCodes, headers,
                            WorkflowIcons,
                            WorkflowIconFileNames,
                            IsShared,
                            false);
                        if (nodeChildren != null && nodeChildren.Count > 0)
                        {
                            WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                            {
                                IsLeaf = false,
                                ObjectID = node.ObjectID,
                                Code = node.Code,
                                DisplayName = node.DisplayName,
                                children = nodeChildren,
                                Icon = "fa fa-folder-o"
                            });
                        }
                        break;
                }
            }
            //设置图标
            foreach (OThinker.H3.Controllers.ViewModels.WorkflowNode n in WorkflowNodes)
            {
                if (n.Code != null)
                {
                    if (WorkflowIcons.ContainsKey(n.Code.ToLower()))
                    {
                        n.Icon = WorkflowIcons[n.Code.ToLower()];
                    }
                    if (WorkflowIconFileNames.ContainsKey(n.Code.ToLower()))
                    {
                        n.IconFileName = WorkflowIconFileNames[n.Code.ToLower()];
                    }
                }
                if (n.children != null)
                {
                    foreach (OThinker.H3.Controllers.ViewModels.WorkflowNode child in n.children)
                    {
                        if (child.Code != null)
                        {
                            if (WorkflowIcons.ContainsKey(child.Code.ToLower()))
                            {
                                child.Icon = WorkflowIcons[child.Code.ToLower()];
                            }
                            if (WorkflowIconFileNames.ContainsKey(child.Code.ToLower()))
                            {
                                child.IconFileName = WorkflowIconFileNames[child.Code.ToLower()];
                            }
                        }
                    }
                }
            }
            return WorkflowNodes;
        }

        public List<PublishedWorkflowTemplateHeader> FindTemplateHeaders(List<PublishedWorkflowTemplateHeader> headers, string objectCode)
        {
            List<PublishedWorkflowTemplateHeader> lstHeaders = new List<PublishedWorkflowTemplateHeader>();
            foreach (PublishedWorkflowTemplateHeader header in headers)
            {
                if (header.WorkflowCode == objectCode)
                {
                    lstHeaders.Add(header);
                }
            }
            return lstHeaders;
        }
        // End Class
    }


    public class ActivitySteps
    {
        public string SN { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Post { get; set; }

    }
}