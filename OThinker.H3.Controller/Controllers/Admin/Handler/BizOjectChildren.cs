using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class BizOjectChildren : AbstractPortalTreeHandler
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
        public BizOjectChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return this.CreatePortalTree(functionId, functionCode, "");
        }

        public override object CreatePortalTree(string functionId, string functionCode, string ownSchemaCode)
        {
            return ChildrenForBizOject(functionId, functionCode, ownSchemaCode);
        }

        #region 构造数据模型(业务对象)的子菜单，代码写死的
        /// <summary>
        /// 构造数据模型(业务对象)的子菜单，代码写死的
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        private object ChildrenForBizOject(string schemaID, string schemaCode, string ownSchemaCode)
        {
            BizObjectSchema schema = _controller.Engine.BizObjectManager.GetDraftSchema(schemaCode);
            List<PortalTreeNode> childrens = new List<PortalTreeNode>();

            string objectID = string.Empty;
            if (schema.Views != null && schema.Views.Count > 0)
            {
                objectID = Guid.NewGuid().ToString();
                childrens.Add(new PortalTreeNode()
                {
                    ObjectID = objectID,
                    Code = schemaCode + "_BizSchemaView",
                    Text = "MenuTreeCategory.ProcessModelHandler.BizSchemaView",
                    IsLeaf = true,
                    Icon = "fa icon-shujumoxingxitongfangfa",
                    ShowPageUrl = ConstantString.PagePath_EditBizObjectSchemaViewList + "?" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(schemaCode) + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                    ParentID = schemaID
                });
            }

            //自定义业务方法
            foreach (DataModel.MethodGroupSchema methodGroupSchema in schema.Methods)
            {
                objectID = Guid.NewGuid().ToString();
                childrens.Add(new PortalTreeNode()
                {
                    ObjectID = objectID,
                    Code = string.Format("{0}_{1}", schemaCode, methodGroupSchema.MethodName),
                    Text = string.IsNullOrWhiteSpace(methodGroupSchema.DisplayName) ? methodGroupSchema.MethodName : methodGroupSchema.DisplayName,
                    IsLeaf = true,
                    ShowPageUrl = ConstantString.PagePath_BizObjectSchemaMethod + "&" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(schemaCode) + "&" + ConstantString.Param_Method + "=" + HttpUtility.UrlEncode(methodGroupSchema.MethodName) + "&" + ConstantString.Param_ParentID + "=" + schemaID + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                    ParentID = schemaID,
                    Icon = BizObjectSchema.GetDefaultMethods().Contains(methodGroupSchema.MethodName) ? "fa icon-hammer" : "fa icon-shujumoxingzidingyifangfa",
                    ExtendObject = new
                    {
                        Text = string.IsNullOrWhiteSpace(methodGroupSchema.DisplayName) ? methodGroupSchema.MethodName : methodGroupSchema.DisplayName
                    }
                });
            }

            childrens.Add(new PortalTreeNode()
            {
                ObjectID = Guid.NewGuid().ToString(),
                Code = schemaCode + "_BizListener",
                Text = "MenuTreeCategory.ProcessModelHandler.BizListener",
                //Icon ="/WFRes/_Content/themes/ligerUI/icons/H3_icons/BizListener.png",
                Icon = "fa icon-jiantingshili",
                IsLeaf = true,
                ShowPageUrl = ConstantString.PagePath_EditListenerPolicy + "&" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(schemaCode) + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                ParentID = schemaID,
            });

            ScheduleInvoker[] invokers = _controller.Engine.BizObjectManager.GetScheduleInvokerList(schemaCode);
            if (invokers != null && invokers.Length > 0)
            {
                childrens.Add(new PortalTreeNode()
                {
                    ObjectID = Guid.NewGuid().ToString(),
                    Code = schemaCode + "_BizScheduleInvoker",
                    Text = "MenuTreeCategory.ProcessModelHandler.BizScheduleInvoker",
                    //Icon ="/WFRes/_Content/themes/ligerUI/icons/H3_icons/BizScheduleInvoker.png",
                    Icon = "fa fa-clock-o",
                    IsLeaf = true,
                    ShowPageUrl = ConstantString.PagePath_ListScheduleInvoker + "&" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(schemaCode) + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                    ParentID = schemaID
                });
            }

            if (schema.Associations.Length > 0)
            {
                childrens.Add(new PortalTreeNode()
                {
                    ObjectID = Guid.NewGuid().ToString(),
                    Code = schemaCode + "_BizScheduleInvoker",
                    Text = "MenuTreeCategory.ProcessModelHandler.BizObjectAssociation",
                    //Icon ="/WFRes/_Content/themes/ligerUI/icons/H3_icons/BizSchemaAssociation.jpg",
                    Icon = "fa fa-exchange",
                    IsLeaf = true,
                    ShowPageUrl = ConstantString.PagePath_ListBizObjectSchemaAssociation + "&" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(schemaCode) + "&" + ConstantString.Param_ParentID + "=" + schemaID + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                    ParentID = schemaID
                });
            }

            childrens.Add(new PortalTreeNode()
            {
                ObjectID = Guid.NewGuid().ToString(),
                Code = schema.SchemaCode + "_BizQuery",
                Text = "MenuTreeCategory.ProcessModelHandler.BizQuery",
                //Icon ="/WFRes/_Content/themes/ligerUI/icons/H3_icons/BizQuery.png",
                Icon = "fa icon-chaxunliebiao-01",
                IsLeaf = true,
                ShowPageUrl = ConstantString.PagePath_ListBizQuery + "&" + ConstantString.Param_SchemaCode + "=" + schema.SchemaCode + "&" + ConstantString.Param_OwnSchemaCode + "=" + ownSchemaCode,
                ParentID = schemaID
            });

            return childrens;
        }
        #endregion

    }
}
