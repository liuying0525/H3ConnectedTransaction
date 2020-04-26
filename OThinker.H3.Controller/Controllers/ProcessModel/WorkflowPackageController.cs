using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 工作流程控制器
    /// </summary>
    public class WorkflowPackageController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }


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
        /// 获取流程模板授权数
        /// </summary>
        public int WorkflowLimit
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return 0;
                return int.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_WorkflowLimit] + string.Empty);
            }
        }

        /// <summary>
        /// 获取工作流程信息
        /// </summary>
        /// <param name="id">流程包ID</param>
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
                    BizObjectSchema schema = this.Engine.BizObjectManager.GetDraftSchema(bizTreeNode.Code);
                    if (schema == null)
                    {
                        schema = this.Engine.BizObjectManager.GetPublishedSchema(bizTreeNode.Code);
                    }
                    WorkflowPackageViewModel model = new WorkflowPackageViewModel()
                    {
                        ObjectID = bizTreeNode.ObjectID,
                        Code = bizTreeNode.Code,
                        DisplayName = bizTreeNode.DisplayName,
                        Folder = string.IsNullOrWhiteSpace(parentCode) ? this.Engine.FunctionAclManager.GetFunctionNode(parentId).Code : parentCode,
                        SortKey = bizTreeNode.SortKey.ToString(),
                        StorageType = ((int)schema.StorageType).ToString(),
                        CheckedUser = bizTreeNode.IsLocked ? this.Engine.Organization.GetName(bizTreeNode.LockedBy) : "WorkflowPackage.UnLocked",
                        ParentId = parentId,
                        ObjectType = objectType,
                        IsShared = schema.IsShared,
                        IsQuotePacket = schema.IsQuotePacket,
                        BindPacket = schema.BindPacket
                    };
                    result.Extend = new
                    {
                        WorkflowPackage = model,
                        Folders = GetFloders(objectType, parentId),
                        CurrentUser = this.UserValidator.UserName
                    };
                }
                else
                {
                    result.Extend = new
                    {
                        WorkflowPackage = new WorkflowPackageViewModel()
                        {
                            Folder = string.IsNullOrWhiteSpace(parentCode) ? this.Engine.FunctionAclManager.GetFunctionNode(parentId).Code : parentCode,
                            SortKey = "1",
                            StorageType = "0",
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
        /// 保存工作流程信息
        /// </summary>
        /// <param name="model">工作流程信息</param>
        /// <returns是否成功></returns>
        [HttpPost]
        public JsonResult SaveWorkflowPackage(WorkflowPackageViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                if (this.WorkflowLimit != 0)
                {
                    int workflowCount = this.Engine.Query.Count(OThinker.H3.WorkflowTemplate.WorkflowClause.TableName,
                        new string[] { 
                        OThinker. H3.WorkflowTemplate.WorkflowClause.PropertyName_State +"="+(int)OThinker.H3.WorkflowTemplate.WorkflowState.Active
                });
                    if (workflowCount >= this.WorkflowLimit)
                    {
                        result.Success = false;
                        result.Message = "WorkflowPackage.OverLimit";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                FunctionNodeType nodeType = string.IsNullOrWhiteSpace(model.ObjectType) ? FunctionNodeType.BizWorkflowPackage : (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), model.ObjectType);

                if (!Validator(nodeType, model, out result)) return Json(result, JsonRequestBehavior.AllowGet);

                if (nodeType == FunctionNodeType.BizWorkflowPackage) //流程包
                {
                    result = SavePackage(model);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else //流程
                {
                    string code = model.Code.Trim();
                    if (this.Engine.WorkflowManager.GetDraftTemplate(code) != null)
                    {
                        result.Success = false;
                        result.Message = "WorkflowPackage.Msg8";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    int sortKey;
                    Int32.TryParse(model.SortKey, out sortKey);
                    OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetDraftSchema(model.Folder);

                    WorkflowTemplate.WorkflowClause wClause = new WorkflowTemplate.WorkflowClause(model.Folder, code, model.DisplayName, sortKey);
                    wClause.IsShared = schema.IsShared;
                    if (schema.IsQuotePacket)
                    {
                        //使用共享包的，用共享包的模型创建
                        wClause = new WorkflowTemplate.WorkflowClause(schema.BindPacket, code, model.DisplayName, sortKey);
                        //加自己的标识
                        wClause.OwnSchemaCode = schema.SchemaCode;
                    }
                    result.Success = this.Engine.WorkflowManager.AddClause(wClause);
                    result.Extend = model.ParentId;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存工作流程信息
        /// </summary>
        /// <param name="model">工作流程信息</param>
        /// <returns>是否成功</returns>
        private ActionResult SavePackage(WorkflowPackageViewModel model)
        {
            ActionResult result = new ActionResult();
            FunctionNode functionNode = new FunctionNode();
            if (string.IsNullOrEmpty(model.ObjectID))
            {
                if (!ValidateExist(model.Code, out result)) return result;
                functionNode.ParentCode = model.Folder;
                functionNode.Code = model.Code.Trim();
                functionNode.NodeType = FunctionNodeType.BizWorkflowPackage;
                int sortKey;
                if (Int32.TryParse(model.SortKey, out sortKey))
                {
                    functionNode.SortKey = sortKey;
                }
                functionNode.DisplayName = model.DisplayName.Trim();
                result.Extend = model.Folder;
                //添加流程包里的数据
                result.Success = this.Engine.AppPackageManager.AddAppPackage(functionNode,
                    (StorageType)int.Parse(model.StorageType), model.IsShared, model.IsQuotePacket, model.BindPacket);

                //使用了共享包，复制共享包的流程模型
                if (model.IsQuotePacket)
                {
                    //复制流程包的流程模板
                    OThinker.H3.WorkflowTemplate.WorkflowClause[] WorkflowClauses = this.Engine.WorkflowManager.GetClausesBySchemaCode(model.BindPacket);
                    if (null != WorkflowClauses && WorkflowClauses.Length != 0)
                    {
                        //复制所有的父流程模板
                        WorkflowClauses = WorkflowClauses.Where(s => s.IsShared).ToArray();
                        for (int i = 0; i < WorkflowClauses.Length; i++)
                        {
                            string workflowClauseCode = i == 0 ? model.Code : model.Code + i;
                            //使用共享包的，用共享包的模型创建
                            WorkflowTemplate.DraftWorkflowTemplate template = this.Engine.WorkflowManager.GetDraftTemplate(WorkflowClauses[i].WorkflowCode);
                            if (template == null) continue;
                            template.WorkflowCode = workflowClauseCode;
                            this.Engine.WorkflowManager.SaveDraftTemplate(this.UserValidator.UserID, template);
                            //添加流程包时已经默认增加了一个流程模板
                            if (i > 0)
                            {
                                WorkflowTemplate.WorkflowClause wClause = new WorkflowTemplate.WorkflowClause(model.BindPacket, workflowClauseCode, model.DisplayName, WorkflowClauses[i].SortKey);
                                wClause.OwnSchemaCode = model.Code;
                                this.Engine.WorkflowManager.AddClause(wClause);
                            }
                        }
                    }
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
        /// 
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="isShared"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetWorkflowPackageShared(string schemaCode, bool isShared)
        {
            ActionResult result = new ActionResult(true);
            BizObjectSchema bizObjectSchema = this.Engine.BizObjectManager.GetDraftSchema(schemaCode);
            BizObjectSchema[] quoteBizObjectSchemas = this.Engine.BizObjectManager.GetQuoteDraftSchemas(schemaCode);
            //被其他流程包应用,禁止修改应用状态
            if (quoteBizObjectSchemas.Length > 0)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.QuotePackagesExist";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            //数据模型不存在
            if (bizObjectSchema == null)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchema.Msg0";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            try
            {
                //更新数据模型
                bizObjectSchema.IsShared = isShared;
                this.Engine.BizObjectManager.UpdateDraftSchema(bizObjectSchema);

                //更新表单
                OThinker.H3.Sheet.BizSheet[] bizSheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(schemaCode);
                if (bizSheets != null)
                    for (int i = 0; i < bizSheets.Length; i++)
                    {
                        bizSheets[i].IsShared = isShared;
                        this.Engine.BizSheetManager.UpdateBizSheet(bizSheets[i]);
                    }
                //更新流程模板
                WorkflowTemplate.WorkflowClause[] workflowClauses = this.Engine.WorkflowManager.GetClausesBySchemaCode(schemaCode);
                if (workflowClauses != null)
                    for (int i = 0; i < workflowClauses.Length; i++)
                    {
                        workflowClauses[i].IsShared = isShared;
                        this.Engine.WorkflowManager.UpdateClause(workflowClauses[i]);
                    }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

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
                result.Message = "WorkflowPackage.Msg4";
                result.Extend = "WorkflowPackage.MaxCodeLength";
                return false;
            }

            //数据项必须以字母开始，不让创建到数据库表字段时报错
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
            if (!regex.Match(code).Success)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.SchemaMsg";
                return false;
            }


            if (this.Engine.FunctionAclManager.GetFunctionNodeByCode(code) != null)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg5";
                return false;
            }
            if (this.Engine.BizObjectManager.GetDraftSchema(code) != null)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg6";
                return false;
            }
            if (this.Engine.BizSheetManager.GetBizSheetByCode(code) != null)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg7";
                return false;
            }
            if (this.Engine.WorkflowManager.GetDraftTemplate(code) != null)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg8";
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
        private bool Validator(FunctionNodeType NodeType, WorkflowPackageViewModel model, out ActionResult result)
        {
            result = new ActionResult();
            if (string.IsNullOrWhiteSpace(model.Code.Trim())
                || string.IsNullOrWhiteSpace(model.DisplayName.Trim()))
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg2";
                return false;
            }
            if (model.Code.Trim().Replace(" ", string.Empty).Length != model.Code.Trim().Length)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg3";
                return false;
            }

            int MaxColeLength = NodeType == FunctionNodeType.BizWorkflowPackage ? BizObjectSchema.MaxCodeLength : FunctionNode.MaxCodeLength;
            if (model.Code.Trim().Length > MaxColeLength)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg4";
                result.Extend = MaxColeLength;
            }

            //校验编码规范
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
            if (!regex.Match(model.Code.Trim()).Success)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.SchemaMsg";
                return false;
            }

            if (model.DisplayName.Trim().Length > FunctionNode.MaxNameLength)
            {
                result.Success = false;
                result.Message = "WorkflowPackage.Msg4";
                result.Extend = FunctionNode.MaxNameLength;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取目录下拉列表
        /// </summary>
        /// <returns></returns>
        private List<Item> GetFloders(string objectType, string parentId)
        {
            List<Item> itemList = new List<Item>();
            FunctionNodeType nodeType = string.IsNullOrWhiteSpace(objectType) ? FunctionNodeType.BizWorkflowPackage : (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), objectType);

            FunctionNode[] folders = nodeType == FunctionNodeType.BizWorkflowPackage ?
                   this.Engine.FunctionAclManager.GetFunctionNodesByNodeType(FunctionNodeType.BizWFFolder) :
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
    }
}
