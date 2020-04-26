using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class ProcessFolderController : ControllerBase
    {
        public ProcessFolderController() { }

        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_ProcessModel_Code; }
        }

        /// <summary>
        /// 保存文件夹
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveFolder(ProcessFolderViewModel model)
        {
            //根据Model的ObjectIDea判断是保存还是更新
            return ExecuteFunctionRun(() =>
            {
                bool isCreateNew = (string.IsNullOrEmpty(model.ObjectID));
                FunctionNode _Node = new FunctionNode();

                ActionResult result = new ActionResult();
                if (isCreateNew)
                {
                    _Node.ParentCode = model.ParentCode;
                    _Node.Code = model.Code;//此处编码不需要用户维护
                    _Node.NodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), model.ObjectType.ToString());
                }
                else
                {
                    _Node = this.Engine.FunctionAclManager.GetFunctionNode(model.ObjectID);
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    result.Success = false;
                    result.Message = "ProcessFolder.NameNotNull";
                    return Json(result);
                }

                //同目录下的兄弟节点
                FunctionNode[] brothers = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(model.ParentCode);

                //判断文件夹重名
                if (brothers != null && brothers.Any(p => !string.IsNullOrWhiteSpace(p.DisplayName) && p.DisplayName.Equals(model.Name)
                    && p.NodeType == _Node.NodeType
                    && p.ObjectID != _Node.ObjectID))
                {
                    result.Success = false;
                    result.Message = "ProcessFolder.NameExisted";
                    return Json(result);
                }

                _Node.DisplayName = model.Name;
                _Node.SortKey = model.SortKey;

                bool result2;
                if (isCreateNew)
                {
                    _Node.ObjectID = _Node.Code = Guid.NewGuid().ToString();
                    _Node.IconCss = "fa fa-folder-o";
                    result2 = this.Engine.FunctionAclManager.AddFunctionNode(_Node) == ErrorCode.SUCCESS;

                    //判断当前用户是否管理员，如果不是管理员，添加目录权限
                    if (result2 && !this.UserValidator.User.IsAdministrator)
                    {
                        WorkflowFolderAclViewModel aclmodel = new WorkflowFolderAclViewModel();
                        aclmodel.FunctionRun = true;
                        aclmodel.UserID = this.UserValidator.UserID;
                        aclmodel.WorkflowFolderCode = _Node.Code;
                        aclmodel.WorkflowFolderName = _Node.DisplayName;
                        aclmodel.UserName = this.UserValidator.UserName;
                        aclmodel.Run = true;

                        SaveWorkflowFloderAclBase(aclmodel);
                    }
                }
                else
                {
                    result2 = this.Engine.FunctionAclManager.UpdateFunctionNode(_Node) == ErrorCode.SUCCESS;
                }

                if (result2)
                {
                    result.Message = "ProcessFolder.Msg_SaveSuccess";
                    result.Success = true;
                }
                else
                {
                    result.Message = "ProcessFolder.Msg_SaveFailed";
                    result.Success = false;
                }
                return Json(result);
            });
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult DeleteFolder(string nodeId, string nodeCode, string objectType)
        {
            return ExecuteFunctionRun(() =>
            {
                nodeCode = HttpUtility.UrlDecode(nodeCode);
                string nodeType = objectType.ToString() ?? "";
                ActionResult result = new ActionResult(true);
                //Node.ID 与Node.Code值相同

                if (string.IsNullOrWhiteSpace(nodeType)
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.BizWorkflowPackage.ToString().ToLowerInvariant())
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.BizFolder.ToString().ToLowerInvariant())
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.BizWFFolder.ToString().ToLowerInvariant())
                    )
                {
                    FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNode(nodeId);
                    if (node.NodeType == FunctionNodeType.BizWorkflowPackage)
                    {
                        DeleteReportSourceAndTemplate(node.Code);//删除关联的报表模板和报表数据源模板
                        FunctionNode[] Fs = this.Engine.FunctionAclManager.GetFunctionNodes();
                        foreach (FunctionNode f in Fs)
                        {
                            if (!string.IsNullOrEmpty(f.Url))
                            {
                                if (f.Url.IndexOf("SchemaCode") > -1)
                                {
                                    string[] memebrs = f.Url.Split('&');
                                    foreach (string schemastring in memebrs)
                                    {
                                        if (schemastring.IndexOf("SchemaCode") > -1)
                                        {
                                            string schemacode = schemastring.Replace("SchemaCode=", "");
                                            if (node.Code == schemacode)
                                            {
                                                this.Engine.FunctionAclManager.RemoveFunctionNode(f.ObjectID, false);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.Engine.LogWriter.Write(string.Format("--------------------->删除节点{0}，操作用户{1}",
                            node.DisplayName,
                            this.UserValidator.UserCode));
                        this.Engine.AppPackageManager.DeleteAppPackage(node);
                        this.Engine.WorkflowConfigManager.DeleteFavoriteWorkflow(this.UserValidator.UserID, node.Code);

                    }
                    else if (node.NodeType == FunctionNodeType.ReportTemplateFolder) // 删除报表模板目录
                    {
                        Analytics.Reporting.ReportTemplate[] reportTemplates =
                            this.Engine.Analyzer.GetReportTemplateByFolderCode(node.Code);
                        //存在报表模板不能直接删除
                        if (reportTemplates != null && reportTemplates.Length > 0)
                        {
                            result.Success = false;
                        }
                        else
                        {
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                        }
                    }
                    else
                    {
                        if (node.NodeType == FunctionNodeType.RuleFolder)
                        {
                            //检测是否有子目录
                            FunctionNode[] Folders = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(nodeCode);
                            BizService[] bizServices = this.Engine.BizBus.GetBizServicesByFolderCode(nodeCode);
                            if ((Folders != null && Folders.Length > 0) || (bizServices != null && bizServices.Length > 0))
                            {
                                result.Success = false;
                                result.Message = "msgGlobalString.HasChildNotDelete";
                                return Json(result);
                            }
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                        }//流程目录
                        else if (node.NodeType == FunctionNodeType.BizWFFolder)
                        {
                            //检测是否有子目录
                            FunctionNode[] Folders = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(nodeCode);
                            DataModel.BizObjectSchema bizSchema = this.Engine.BizObjectManager.GetPublishedSchema(nodeCode);
                            if ((Folders != null && Folders.Length > 0) || bizSchema != null)
                            {
                                result.Success = false;
                                result.Message = "msgGlobalString.HasChildNotDelete";
                                return Json(result);
                            }
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                        }
                        else
                        {
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);

                        }

                    }
                }
                else
                {
                    FunctionNodeType type = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), nodeType);
                    switch (type)
                    {
                        case FunctionNodeType.BizSheet:
                            var sheet = this.Engine.BizSheetManager.GetBizSheetByCode(nodeCode);
                            if (sheet.IsShared)
                            {
                                //共享流程包,只有主包可以删除共享表单
                                if (nodeId == sheet.ObjectID)
                                    result.Success = this.Engine.BizSheetManager.DeleteBizSheet(nodeCode);
                                else result.Success = false;
                            }
                            else
                            {
                                result.Success = this.Engine.BizSheetManager.DeleteBizSheet(nodeCode);
                            }
                            break;
                        case FunctionNodeType.BizWorkflow:
                            this.Engine.WorkflowManager.RemoveClause(nodeCode);
                            result.Success = true;
                            break;


                    }
                }

                if (result.Success)
                {

                    result.Message = "ProcessFolder.Msg_DeleteSuccess";
                }
                else
                {
                    result.Message = "ProcessFolder.Msg_DeleteFailed";
                    //ajaxResult.ResultMsg = this.PortalResource.GetString("Msg_DeleteFailed") + ":" + this.PortalResource.GetString("AddProcessFolder_Mssg1");
                }

                return Json(result);
            });
        }

        /// <summary>
        /// 删除报表数据源和报表模板
        /// </summary>
        /// <param name="code">流程模板编码</param>
        private void DeleteReportSourceAndTemplate(string code)
        {
            ReportSource[] ReportSources = this.Engine.Analyzer.GetReportSources();
            ReportTemplate[] ReportTemplates = this.Engine.Analyzer.GetAllReportTemplates();
            if (null != ReportSources)
            {
                foreach (var m in ReportSources)
                {
                    if (m.SchemaCode == code)
                    {
                        this.Engine.Analyzer.RemoveReportSource(m.ObjectID);
                        if (null != ReportTemplates)
                        {
                            foreach (var v in ReportTemplates)
                            {
                                if (v.SourceCode == m.Code)
                                {
                                    this.Engine.Analyzer.RemoveReportTemplate(v.ObjectID);
                                }
                            }
                        }

                    }
                }
            }
        }


        /// <summary>
        /// 获取文件夹节点信息
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public JsonResult GetFolderInfo(string nodeId)
        {
            return ExecuteFunctionRun(() =>
            {

                FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNode(nodeId);
                ProcessFolderViewModel model = new ProcessFolderViewModel();

                model.ObjectID = node.ObjectID;
                model.Code = node.Code;
                model.Name = node.DisplayName;
                model.ParentID = node.ParentObjectID;
                model.ParentCode = node.ParentCode;
                model.SortKey = node.SortKey;
                FunctionNode pNode = this.Engine.FunctionAclManager.GetFunctionNode(node.ParentCode);
                if (pNode != null)
                {
                    model.ParentName = pNode.DisplayName;
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
