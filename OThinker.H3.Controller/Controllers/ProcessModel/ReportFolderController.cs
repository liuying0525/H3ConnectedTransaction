using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class ReportFolderController : ControllerBase
    { 
        //qiancheng
        public ReportFolderController() { }

        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_Report_Code; }
        }

        #region 文件夹目录
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
                   // _Node.NodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), model.ObjectType.ToString());
                    _Node.NodeType = FunctionNodeType.ReportFolder;
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
        /// 删除文件夹目录
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
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.Report.ToString().ToLowerInvariant())
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.ReportFolder.ToString().ToLowerInvariant())
                    || nodeType.ToLowerInvariant().Equals(FunctionNodeType.ReportFolderPage.ToString().ToLowerInvariant())
                    )
                {
                    FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNode(nodeId);
                    if (node == null) {
                        node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(nodeCode);
                    }
                    if (node == null) {
                        result.Success = false;
                        return Json(result);
                    }
                     if (node.NodeType == FunctionNodeType.ReportFolder) // 删除报表模板目录
                    {
                        FunctionNode[] nodechildrens = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(node.Code);
                        //存在报表模板不能直接删除
                        if (nodechildrens != null && nodechildrens.Length > 0)
                        {
                            result.Success = false;
                            result.Message = "msgGlobalString.HasChildNotDelete";
                            return Json(result);
                        }
                        else
                        {
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                        }
                       
                    }
                    else
                    {
                         //删除报表页
                        if (node.NodeType == FunctionNodeType.ReportFolderPage)
                        {
                            //检测是否有子目录
                            FunctionNode[] Folders = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(nodeCode);
                         
                            if (Folders != null && Folders.Length > 0)
                            {
                                result.Success = false;
                                result.Message = "msgGlobalString.HasChildNotDelete";
                                return Json(result);
                            }
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                           bool reul= this.Engine.ReportManager.RemoveReportPage(node.Code);
                        }
                        else
                        {
                            result.Success = this.Engine.FunctionAclManager.RemoveFunctionNode(nodeId, false);
                        }

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
                        if (null!= ReportTemplates)
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
        #endregion


        #region 报表模型
        /// <summary>
        /// 获取当前注册授权类型
        /// </summary>
        public OThinker.H3.Configs.LicenseType LicenseType
        {
            get
            {
                return (OThinker.H3.Configs.LicenseType)this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType];
            }
        }

       
        /// <summary>
        /// 获取目录下拉列表
        /// </summary>
        /// <returns></returns>
        private List<Item> GetFloders(string objectType, string parentId)
        {
            List<Item> itemList = new List<Item>();
            FunctionNodeType nodeType = string.IsNullOrWhiteSpace(objectType) ? FunctionNodeType.BizWorkflowPackage : (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), objectType);
           FunctionNode[] reportnode= this.Engine.FunctionAclManager.GetFunctionNodesByNodeType(FunctionNodeType.Report);
           if (reportnode != null) 
           {
               foreach (FunctionNode report in reportnode)
               {
                   itemList.Add(new Item(string.IsNullOrWhiteSpace(report.DisplayName) ? report.Code : report.DisplayName,
                        report.Code));//提供一个可以选择新建报表页在跟目录的机会
               }
           }
            
            FunctionNode[] folders = nodeType == FunctionNodeType.ReportFolder ?
                   this.Engine.FunctionAclManager.GetFunctionNodesByNodeType(FunctionNodeType.ReportFolder) :
                  new FunctionNode[] { this.Engine.FunctionAclManager.GetFunctionNode(parentId) };
            if (folders != null)
            {
                foreach (FunctionNode folder in folders)
                {
                    itemList.Add(new Item(string.IsNullOrWhiteSpace(folder.DisplayName) ? folder.Code : folder.DisplayName,
                        folder.Code));
                }
            }
            return itemList;
        }

        /// <summary>
        /// 获取报表模型信息
        /// </summary>
        /// <param name="id">报表页ID</param>
        /// <param name="objectType">类型</param>
        /// <param name="parentId">父ID</param>
        /// <param name="parentCode">父编码</param>
        /// <returns>工作流信息</returns>
        [HttpGet]
        public JsonResult GetWorkflowPackage(string id, string objectType, string parentId, string parentCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                string bizTreeCode = string.Empty;
                if (!string.IsNullOrEmpty(id))
                {
                    var bizTreeNode = this.Engine.FunctionAclManager.GetFunctionNode(id);
                    bizTreeCode = bizTreeNode.Code;
                    OThinker.Reporting.ReportPage schema = this.Engine.ReportManager.GetReportPage(bizTreeNode.Code);
                    if (schema == null)
                    {
                        schema = this.Engine.ReportManager.GetReportPage(bizTreeNode.Code);
                    }
                    ReportPagePackageViewModel report = new ReportPagePackageViewModel()
                    {
                        ObjectID = bizTreeNode.ObjectID,
                        Code = bizTreeNode.Code,
                        DisplayName = bizTreeNode.DisplayName,
                        Folder = bizTreeNode.ParentCode,
                        SortKey = bizTreeNode.SortKey.ToString(),

                        CheckedUser = bizTreeNode.IsLocked ? this.Engine.Organization.GetName(bizTreeNode.LockedBy) : "Reporting.UnLocked",
                        ParentId = parentId,
                        ObjectType = objectType
                        

                    };
                   
                    result.Extend = new
                    {
                        WorkflowPackage = report,
                        Folders = GetFloders(objectType, parentId)
                    };
                }
                else
                {
                    result.Extend = new
                    {
                        WorkflowPackage = new ReportPagePackageViewModel()
                        {
                            Folder = string.IsNullOrWhiteSpace(parentCode) ? this.Engine.FunctionAclManager.GetFunctionNode(parentId).Code : parentCode,
                            SortKey = "1",
                            ParentId = parentId,
                            ObjectType = objectType
                           
                        },
                        Folders = GetFloders(objectType, parentId)
                    };
                }
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存报表页信息
        /// </summary>
        /// <param name="model">报表页</param>
        /// <returns是否成功></returns>
        [HttpPost]
        public JsonResult SaveWorkflowPackage(ReportPagePackageViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
              
                FunctionNodeType nodeType = string.IsNullOrWhiteSpace(model.ObjectType) ? FunctionNodeType.BizWorkflowPackage : (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), model.ObjectType);

                if (!Validator(nodeType, model, out result)) return Json(result, JsonRequestBehavior.AllowGet);

                if (nodeType == FunctionNodeType.ReportFolder) //报表页
                {
                    result = SavePackage(model);
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存报表数据到function
        /// </summary>
        /// <param name="model">工作流程信息</param>
        /// <returns>是否成功</returns>
        private ActionResult SavePackage(ReportPagePackageViewModel model)
        {
            ActionResult result = new ActionResult();
            FunctionNode functionNode = new FunctionNode();
            if (string.IsNullOrEmpty(model.ObjectID))
            {
                if (!ValidateExist(model.Code, out result)) return result;
                functionNode.ParentCode = model.Folder;
                functionNode.Code = model.Code.Trim();
                functionNode.NodeType = FunctionNodeType.ReportFolderPage;
                functionNode.IconCss = "fa icon-charubiaoge";
                int sortKey;
                if (Int32.TryParse(model.SortKey, out sortKey))
                {
                    functionNode.SortKey = sortKey;
                }
                functionNode.DisplayName = model.DisplayName.Trim();
                functionNode.Url = "Reporting/IndexInfo.html&RportCode=" + model.Code.Trim();
                if (model.Folder == "ReportModel") {
                    FunctionNode fun = this.Engine.FunctionAclManager.GetFunctionNodeByCode(model.Folder);
                    if (fun != null) 
                    {
                        result.Extend = fun.ObjectID;
                    }
                    
                } else {
                    result.Extend = model.ParentId;
                }
                
                //添加功能节点
                result.Success = this.Engine.FunctionAclManager.AddFunctionNode(functionNode) == ErrorCode.SUCCESS;
                string code = model.Code.Trim();
                //新增报表页
                OThinker.Reporting.ReportPage report = new OThinker.Reporting.ReportPage();
                report.ObjectID = Guid.NewGuid().ToString();
                report.Code = code;
                report.Creator = this.UserValidator.UserID;
                report.DisplayName = model.DisplayName;
                report.CreatedTime = DateTime.Now;
                report.ParentObjectID = model.ParentId;
                report.ParentPropertyName = "";
                if (result.Success == true)
                {
                    result.Success = this.Engine.ReportManager.AddReportPage(report);
                }
               
            }
            else
            {
                functionNode = this.Engine.FunctionAclManager.GetFunctionNode(model.ObjectID);
                int sortKey;
                if (Int32.TryParse(model.SortKey, out sortKey))
                {
                    functionNode.SortKey = sortKey;
                }
                functionNode.DisplayName = model.DisplayName.Trim();
                string nodeParentId = this.Engine.FunctionAclManager.GetFunctionNodeByCode(functionNode.ParentCode).ObjectID;
                functionNode.ParentCode = model.Folder;
                result.Success = this.Engine.FunctionAclManager.UpdateFunctionNode(functionNode) == ErrorCode.SUCCESS;
                result.Extend = nodeParentId;
            }
            return result;
        }

        /// <summary>
        /// 检查是否已经存在
        /// </summary>
        /// <param name="code">流程编码</param>
        /// <param name="result">out:检查结果</param>
        /// <returns>是否存在</returns>
        private bool ValidateExist(string code, out ActionResult result)
        {
            code = code.Trim();
            result = new ActionResult();
            if (code.Length > BizObjectSchema.MaxCodeLength)
            {
                result.Success = false;
                result.Message = "Reporting.Msg4";
                result.Extend = "Reporting.MaxCodeLength";
                return false;
            }

            //数据项必须以字母开始，不让创建到数据库表字段时报错
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
            if (!regex.Match(code).Success)
            {
                result.Success = false;
                result.Message = "Reporting.SchemaMsg";
                return false;
            }


            if (this.Engine.FunctionAclManager.GetFunctionNodeByCode(code) != null)
            {
                result.Success = false;
                result.Message = "Reporting.Msg5";
                return false;
            }
            if (this.Engine.ReportManager.GetReportPage(code).Code != null)
            {
                result.Success = false;
                result.Message = "Reporting.Msg6";
                return false;
            }
           
            return true;
        }

        /// <summary>
        /// 表单信息验证
        /// </summary>
        /// <param name="NodeType">节点类型</param>
        /// <param name="model">工作流程信息</param>
        /// <param name="result">out:验证结果</param>
        /// <returns>验证是否通过</returns>
        private bool Validator(FunctionNodeType NodeType, ReportPagePackageViewModel model, out ActionResult result)
        {
            result = new ActionResult();
            if (string.IsNullOrWhiteSpace(model.Code.Trim())
                || string.IsNullOrWhiteSpace(model.DisplayName.Trim()))
            {
                result.Success = false;
                result.Message = "Reporting.Msg2";
                return false;
            }
            if (model.Code.Trim().Replace(" ", string.Empty).Length != model.Code.Trim().Length)
            {
                result.Success = false;
                result.Message = "Reporting.Msg3";
                return false;
            }

            int MaxColeLength = NodeType == FunctionNodeType.BizWorkflowPackage ? BizObjectSchema.MaxCodeLength : FunctionNode.MaxCodeLength;
            if (model.Code.Trim().Length > MaxColeLength)
            {
                result.Success = false;
                result.Message = "Reporting.Msg4";
                result.Extend = MaxColeLength;
            }

            //校验编码规范
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
            if (!regex.Match(model.Code.Trim()).Success)
            {
                result.Success = false;
                result.Message = "Reporting.SchemaMsg";
                return false;
            }

            if (model.DisplayName.Trim().Length > FunctionNode.MaxNameLength)
            {
                result.Success = false;
                result.Message = "Reporting.Msg4";
                result.Extend = FunctionNode.MaxNameLength;
                return false;
            }
            return true;
        }

        #endregion
    }
}
