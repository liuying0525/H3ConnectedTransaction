using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class WrokflowPackageChildren : AbstractPortalTreeHandler
    {
        private ControllerBase _controller = null;

        protected override ControllerBase controller
        {
            get { return _controller; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller">控制器</param>
        public WrokflowPackageChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return ChildrenForWrokflowPackage(functionId, functionCode);
        }

        #region 流程包的子数据
        private object ChildrenForWrokflowPackage(string PackageID, string PackageCode)
        {
            FunctionNode package = _controller.Engine.FunctionAclManager.GetFunctionNode(PackageID);
            bool isLockedByCurrentUser = (package.IsLocked == true && _controller.UserValidator.UserID == package.LockedBy);
            List<FunctionNode> functionNodes = new List<FunctionNode>();
            //所属流程包
            //数据模型--流程包的编码，等于数据模型编码
            BizObjectSchema schema = _controller.Engine.BizObjectManager.GetDraftSchema(PackageCode);

            BizObjectSchema schemaShared = null;
            //如果不是共享包，看是不是引用了共享包
            BizSheet[] sheetsShared = null;
            WorkflowClause[] workflowClausesShared = null;
            if (!schema.IsShared)
            {
                //引用了共享包
                if (schema.IsQuotePacket)
                {
                    string bindSharedPacket = schema.BindPacket;
                    //改为绑定的共享流程包
                    schemaShared = _controller.Engine.BizObjectManager.GetDraftSchema(bindSharedPacket);
                    if (schemaShared != null)
                    {
                        //取共享的表单
                        sheetsShared = _controller.Engine.BizSheetManager.GetBizSheetBySchemaCode(schemaShared.SchemaCode);
                        workflowClausesShared = _controller.Engine.WorkflowManager.GetClausesBySchemaCode(schemaShared.SchemaCode);
                    }
                }

            }
            FunctionNode packageNode = null;
            if (schema.IsQuotePacket)
            {
                if (schemaShared != null)
                {

                    packageNode = new FunctionNode(schemaShared.SchemaCode, schema.DisplayName, "", "", FunctionNodeType.BizObject, PackageCode, 0);
                    packageNode.OwnSchemaCode = schema.SchemaCode;
                }
            }
            else
            {
                packageNode = new FunctionNode(schema.SchemaCode, schema.DisplayName, "", "", FunctionNodeType.BizObject, PackageCode, 0);
            }
            if (packageNode != null)
            {
                packageNode.IconCss = "fa icon-zhushujushili";
                //数据模型子节点继承父流程包有关锁定的属性
                packageNode.LockedBy = package.LockedBy;
                functionNodes.Add(packageNode);
            }


            //表单列表
            BizSheet[] ownSheets = _controller.Engine.BizSheetManager.GetBizSheetBySchemaCode(PackageCode);

            BizSheet[] sheets = null;

            //引用了共享包
            if (schema.IsQuotePacket && sheetsShared != null)
            {
                sheets = sheetsShared.Where(p => (p.OwnSchemaCode == schema.SchemaCode || p.IsShared)).ToArray();
                foreach (var sheet in sheets)
                {
                    sheet.ObjectID = Guid.NewGuid().ToString();
                }
                if (ownSheets != null)
                {
                    sheets = sheets.Concat(ownSheets).ToArray();
                }
            }
            else if (ownSheets != null)
            {
                sheets = ownSheets.Where(p => string.IsNullOrEmpty(p.OwnSchemaCode)).ToArray();
            }

            if (sheets != null)
            {
                foreach (BizSheet sheet in sheets)
                {
                    FunctionNode node = new FunctionNode(sheet.ObjectID, sheet.SheetCode, sheet.DisplayName
                        , "fa fa-file-o", "", FunctionNodeType.BizSheet, PackageCode, 0);
                    node.IconCss = "fa fa-file-o";
                    node.IsSystem = sheet.SheetType == SheetType.DefaultSheet;
                    //表单子节点继承父流程包有关锁定的属性
                    node.LockedBy = package.LockedBy;
                    if (sheet.IsShared&&schema.IsQuotePacket)
                    {
                        node.OwnSchemaCode = schema.SchemaCode;
                    }
                    functionNodes.Add(node);
                }
            }
            //流程模板
            WorkflowClause[] workflowClausesMySelf = _controller.Engine.WorkflowManager.GetClausesBySchemaCode(PackageCode);
            WorkflowClause[] workflowClauses = null;
            //加入共享包模板
            //如果是共享包
            if (schema.IsShared)
            {
                workflowClauses = workflowClausesMySelf.Where(p => p.IsShared).ToArray();
            }
            else
            {
                //引用了共享包
                if (schema.IsQuotePacket && workflowClausesShared != null)
                {

                    workflowClauses = workflowClausesShared.Where(p => p.OwnSchemaCode == schema.SchemaCode).ToArray();
                }
                //没有引用共享包
                else
                {
                    workflowClauses = workflowClausesMySelf;
                }
            }

            //WorkflowClause[] workflowClauses = this.Engine.WorkflowManager.GetClausesBySchemaCode(PackageCode);
            WorkflowTemplate.WorkflowClause template = _controller.Engine.WorkflowManager.GetClause(PackageCode);
            //引用了共享包，改用共享包模板
            if (template == null)
            {
                template = _controller.Engine.WorkflowManager.GetClause(schema.SchemaCode);
            }

            if (workflowClauses != null)
            {
                workflowClauses = workflowClauses.OrderBy(p => p.SortKey).ToArray(); ;
                foreach (WorkflowClause workflowClause in workflowClauses)
                {
                    FunctionNode node = new FunctionNode(workflowClause.ObjectID, workflowClause.WorkflowCode
                        , workflowClause.WorkflowName, "fa icon-liuchengshujumoxing", "", FunctionNodeType.BizWorkflow, PackageCode, 0);

                    if (template != null)
                    {
                        if (template.State == WorkflowState.Inactive)
                            node.IconCss = "fa icon-liuchengmoxing Inactive";
                        else
                            node.IconCss = "fa icon-liuchengmoxing";
                    }
                    else
                    {
                        //当流程包下面没有对应流程包编码的流程模板时，template==null,设置样式为默认流程模板样式
                        node.IconCss = "fa icon-liuchengmoxing";
                    }
                    //流程模板子节点继承父流程包有关锁定的属性
                    node.LockedBy = package.LockedBy;
                    if (workflowClause.IsShared && schema.IsQuotePacket)
                    {
                        node.OwnSchemaCode = schema.SchemaCode;
                    }
                    functionNodes.Add(node);
                }
            }
            //增加ExtendObject,存储DisplayName
            List<PortalTreeNode> list = ConvertToPortalTree(PackageID, functionNodes.ToArray());
            foreach (var node in list)
            {
                node.ExtendObject = new
                {
                    Text = functionNodes.FirstOrDefault(s => s.ObjectID == node.ObjectID).DisplayName,
                };
            }
            return list;
        }
        #endregion

    }
}
