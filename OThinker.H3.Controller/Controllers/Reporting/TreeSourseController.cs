using OThinker.H3.Acl;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Reporting
{
    public class TreeSourseController : ControllerBase
    {

        public bool ShowSubsheetFiled, ShowCheckbox;

        private List<string> cheackTypes = new List<string>();

        //该页面没有受权限控制，不需要注册节点编码
        public override string FunctionCode
        {
            get { return ""; }
        }
        /// <summary>
        /// 树形菜单加载入口
        /// </summary>
        /// <returns></returns>
        public JsonResult DoAction()
        {
            List<AppTreeNode> childNodes = new List<AppTreeNode>();
            string Paramters = Request["ActionName"];//操作的方法
            string LoadTableAndView = Request["command"];
            string ParentCode = Request["id"] == null ? "root" : Request["id"];
            string relationCode = Request["Code"];
            string NodeType = Request["NodeType"];
            string CurSchemeCode = Request["SchemeCode"];

            string Codes = Request["Codes"];//表单对象编码，多个以分号隔开
            if (Paramters == "GetSheetDisplayNames" || LoadTableAndView == "GetSheetDisplayNames")//报表数据源计算函数规则设计器
            {
                #region 报表数据源计算函数规则设计器，数据源树形展示;
                List<string> li = new List<string>();
                var code = Codes.Split(';');
                if (code.Length == 1)
                {
                    li.Add(Codes.ToString());
                }
                else
                {
                    for (int i = 0; i < code.Length; i++)
                    {
                        li.Add(code[i].ToString());
                    }
                }

                childNodes = GetAppSchemaTrees(ParentCode, li);
                return Json(childNodes, JsonRequestBehavior.AllowGet);
                #endregion
            }
            else
            {
                #region 初始化数据源展示菜单


                ShowSubsheetFiled = Request["ShowSubsheetFiled"] == "true" ? true : false;
                string CanCheckTypes = Request["CanCheckTypes"];//记录的是数据链接池的编码
                ShowCheckbox = Request["ShowCheckbox"] == "true" ? true : false;
                if (!string.IsNullOrEmpty(CanCheckTypes))
                {
                    CanCheckTypes = CanCheckTypes.TrimEnd(',');
                }
                if (string.IsNullOrEmpty(CanCheckTypes))
                {
                    CanCheckTypes = "Engine";//默认设置为H3引擎编码
                }

                //该方法是读取数据库中，数据表和视图的方法
                if (LoadTableAndView == "LoadTableAndView")
                {
                    #region 该方法是读取数据库中，数据表和视图的方法
                    //该方法是读取业务表单的站点
                    if (ParentCode == "root")
                    {
                        AppTreeNode appTreeTable = CreateAppTreeNode("DbTable", "数据表", "DbTable", TreeNodeType.AppPackge.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                        childNodes.Add(appTreeTable);
                        AppTreeNode appTreeView = CreateAppTreeNode("DbView", "视图", "DbView", TreeNodeType.AppPackge.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                        childNodes.Add(appTreeView);
                    }
                    else
                    {
                        //如果是数据表
                        if (ParentCode == "DbTable")
                        {
                            string[] tableNames = this.Engine.SettingManager.GetBizDbTableNames(CanCheckTypes);
                            foreach (var table in tableNames)
                            {
                                AppTreeNode appTreeTable = CreateAppTreeNode(table, table, table, TreeNodeType.AppMenu.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                                childNodes.Add(appTreeTable);
                            }
                        }
                        else if (ParentCode == "DbView") //如果是视图
                        {
                            string[] viewNames = this.Engine.SettingManager.GetBizDbViewNames(CanCheckTypes);
                            foreach (var view in viewNames)
                            {
                                AppTreeNode appTreeTable = CreateAppTreeNode(view, view, view, TreeNodeType.AppMenu.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                                childNodes.Add(appTreeTable);
                            }
                        }
                        else //否则是查询表的具体字段的
                        {
                            if (NodeType == "Field") { }
                            else
                            {
                                string newSql = "select * from " + ParentCode;
                                List<OThinker.Reporting.ReportWidgetColumn> columns = this.Engine.ReportQuery.GetSqlColumns(this.Engine, newSql, CanCheckTypes);
                                foreach (var col in columns)
                                {
                                    AppTreeNode appTreeTable = CreateAppTreeNode(col.ColumnName, col.ColumnName, col.ColumnName, TreeNodeType.Field.ToString(), true, ParentCode, "", true, false);
                                    childNodes.Add(appTreeTable);
                                }
                            }
                        }

                    }

                    return Json(childNodes, JsonRequestBehavior.AllowGet);
                    #endregion
                }
                else
                {
                    #region 加载业务表单
                    //该方法是读取业务表单的站点
                    if (ParentCode == "root")
                    {
                        string NodePath = string.Empty;
                        if (!string.IsNullOrEmpty(CurSchemeCode))
                        {
                            FunctionNode CheckedNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(CurSchemeCode);
                            List<string> parentCodes = new List<string>();
                            while (CheckedNode.ParentCode != "ProcessModel")
                            {
                                parentCodes.Add(CheckedNode.ParentCode);
                                CheckedNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(CheckedNode.ParentCode);
                            }
                            if (CheckedNode.ParentCode == "ProcessModel")
                            {
                                parentCodes.Add(CheckedNode.Code);
                            }
                            NodePath = string.Join(",", parentCodes);
                        }
                       
                        //加载所有流程模型 
                        List<AppTreeNode> list = new List<AppTreeNode>();
                        List<FunctionNode> funNode = this.Engine.FunctionAclManager.GetChildNodesByParentCode("ProcessModel");

                        //按照后台流程模型节点展示的顺序排序数据
                        funNode = funNode.OrderBy(s => s.DisplayName).OrderBy(s => s.SortKey).ToList();
                        AppTreeNode masterNode =null;
                        foreach (FunctionNode fn in funNode)
                        {
                            if (fn.ParentCode == "ProcessModel")
                            {
                                //判断是否该节点是否有子项
                                bool IsParentNode = false;
                                if (this.Engine.FunctionAclManager.GetChildNodesByParentCode(fn.Code).Count() > 0)
                                {
                                    IsParentNode = true;
                                }

                                AppTreeNode appTree = CreateAppTreeNode(fn.Code, fn.DisplayName, fn.Code, TreeNodeType.AppPackge.ToString(), IsParentNode, ParentCode, "fa fa-folder-open-o", true, false, NodePath);
                                //判断是否是主数据节点
                                if (fn.NodeType == OThinker.H3.Acl.FunctionNodeType.BizFolder)
                                {
                                    masterNode = appTree;
                                    continue;
                                }
                                childNodes.Add(appTree);
                            }
                        }
                        //主数据节点置顶
                        childNodes.Insert(0, masterNode);
                    }
                    else
                    {
                        //BizWorkflowPackage
                        List<FunctionNode> funNode = this.Engine.FunctionAclManager.GetChildNodesByParentCode(ParentCode);
                        List<FunctionNode> funnode = new List<FunctionNode>();
                        if (funNode != null)
                        {
                            funNode = funNode.OrderBy(i => i.DisplayName).ToList();
                            funNode = funNode.OrderBy(i => i.SortKey).ToList();
                        }
                        BizObjectSchema bizobjectSchema = null;
                        foreach (FunctionNode fun in funNode)
                        {

                            if (ParentCode == fun.ParentCode)
                            {
                                BizObjectSchema[] das = this.Engine.BizObjectManager.GetPublishedSchemas();

                                //说明是流程包
                                if (fun.NodeType == OThinker.H3.Acl.FunctionNodeType.BizWorkflowPackage)
                                {
                                    bizobjectSchema = this.Engine.BizObjectManager.GetPublishedSchema(fun.Code);
                                    if (bizobjectSchema != null)
                                    {
                                        //判断是否该节点是否有子项
                                        bool IsParentNode = false;
                                        foreach (var item in bizobjectSchema.Properties)
                                        {
                                            if (item.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                                            {
                                                IsParentNode = true;
                                                break;
                                            }
                                        }
                                        AppTreeNode appTree = CreateAppTreeNode(fun.Code, fun.DisplayName, fun.Code, TreeNodeType.AppMenu.ToString(), IsParentNode, ParentCode, "", false, false);
                                        childNodes.Add(appTree);
                                    }
                                }
                                else if (fun.NodeType == OThinker.H3.Acl.FunctionNodeType.BizObject)
                                {
                                    BizObjectSchema bizobjectSchemas = this.Engine.BizObjectManager.GetPublishedSchema(fun.Code);
                                    if (bizobjectSchemas != null)
                                    {
                                        //判断是否该节点是否有子项
                                        bool IsParentNode = false;
                                        foreach (var item in bizobjectSchemas.Properties)
                                        {
                                            if (item.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                                            {
                                                IsParentNode = true;
                                                break;
                                            }
                                        }
                                        AppTreeNode appTree = CreateAppTreeNode(fun.Code, fun.DisplayName, fun.Code, TreeNodeType.AppMenu.ToString(), IsParentNode, ParentCode, "", false, false);
                                        childNodes.Add(appTree);
                                    }
                                }
                                else
                                {
                                    //还是文件夹
                                    AppTreeNode appTree = CreateAppTreeNode(fun.Code, fun.DisplayName, fun.Code, TreeNodeType.AppGroup.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                                    childNodes.Add(appTree);
                                }
                            }

                        }

                        // 加载业务对象子元素
                        if (funNode.Count == 0)
                        {
                            string SchemaCode = ParentCode;
                            BizObjectSchema sch = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
                            if (sch != null)
                            {
                                PropertySchema[] properties = sch.Properties;
                                childNodes = GetTreeChildNodes(properties, ParentCode, false);
                            }
                        }
                    }



                    return Json(childNodes, JsonRequestBehavior.AllowGet);
                    #endregion

                }
                #endregion
            }
        }

        /// <summary>
        /// 根据父编码，展示业务数据表单的字段
        /// </summary>
        /// <param name="ParentCode"></param>
        /// <param name="Codes"></param>
        /// <returns></returns>
        private List<AppTreeNode> GetAppSchemaTrees(string ParentCode, List<string> Codes)
        {
            List<AppTreeNode> childNodes = new List<AppTreeNode>();
            if (ParentCode == "root")//如果是根目录则展示根目录菜单
            {
                for (int i = 0; i < Codes.Count; i++)
                {
                    List<AppTreeNode> list = new List<AppTreeNode>();
                    FunctionNode funNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(Codes[i].ToString());
                    if (funNode != null)
                    {
                        AppTreeNode appTree = CreateAppTreeNode(funNode.Code, funNode.DisplayName, funNode.Code, TreeNodeType.AppPackge.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                        childNodes.Add(appTree);
                    }
                    else
                    {

                        BizObjectSchema sch = this.Engine.BizObjectManager.GetPublishedSchema(Codes[i].Substring(0, Codes[i].IndexOf("___")));
                        if (sch != null)
                        {

                            foreach (var pro in sch.Properties)
                            {
                                if (pro.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                                {
                                    if (Codes[i].IndexOf(pro.Name) != -1)
                                    {
                                        AppTreeNode appTree = CreateAppTreeNode(pro.Name, pro.DisplayName, pro.Name, TreeNodeType.AppPackge.ToString(), true, ParentCode, "fa fa-folder-open-o", true, false);
                                        childNodes.Add(appTree);
                                    }
                                }
                            }
                        }
                        else
                        {

                        }

                    }


                }

            }
            else //否则根据父ID 加载子菜单
            {
                string SchemaCode = ParentCode;
                BizObjectSchema sch = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
                if (sch != null)
                {
                    PropertySchema[] properties = sch.Properties;
                    childNodes = GetTreeChildNodes(properties, ParentCode, true);
                }
                else
                {
                    BizObjectSchema schs = this.Engine.BizObjectManager.GetPublishedSchema(Codes[0]);
                    if (schs != null)
                    {
                        foreach (var code in schs.Properties)
                        {
                            if (code.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                            {
                                if (code.Name == SchemaCode)
                                {
                                    childNodes = GetTreeChildNodes(code.ChildSchema.Properties, ParentCode, true);
                                }
                            }
                        }
                    }

                }
            }
            return childNodes;
        }


        /// <summary>
        /// 获取该节点的子节点
        /// </summary>
        /// <param name="Properties"></param>
        /// <param name="ParnentCode"></param>
        /// <param name="IsChild"></param>
        /// <returns></returns>
        private List<AppTreeNode> GetTreeChildNodes(PropertySchema[] Properties, string ParnentCode, bool IsChild)
        {
            List<AppTreeNode> list = new List<AppTreeNode>();

            foreach (PropertySchema property in Properties)
            {
                if (property.ChildSchema != null && !IsChild)
                {
                    //说明是子表
                    list.Add(CreateAppTreeNode(ParnentCode + "___" + property.Name, property.DisplayName.Trim(), ParnentCode + "___" + property.Name, TreeNodeType.SubSheet.ToString(), false, ParnentCode, "fa-th-list", false, false));
                }
                else
                {
                    if (IsChild)
                    {
                        if (property.LogicType == OThinker.H3.Data.DataLogicType.ShortString || property.LogicType == OThinker.H3.Data.DataLogicType.DateTime || property.LogicType == OThinker.H3.Data.DataLogicType.Decimal || property.LogicType == OThinker.H3.Data.DataLogicType.Int || property.LogicType == OThinker.H3.Data.DataLogicType.Double || property.LogicType == OThinker.H3.Data.DataLogicType.Long)
                        {
                            list.Add(CreateAppTreeNode(property.Name, property.DisplayName.Trim(), property.Name, TreeNodeType.Association.ToString(), false, ParnentCode, "fa-th-list", true, false));
                        }
                    }

                }

            }

            return list;
        }

        /// <summary>
        /// 构造一个树形对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <param name="isParent"></param>
        /// <param name="pId"></param>
        /// <param name="iconSkin"></param>
        /// <param name="ShowCheckbox"></param>
        /// <param name="CanCheck"></param>
        /// <returns></returns>
        private AppTreeNode CreateAppTreeNode(string id, string name, string code, string type, bool isParent, string pId, string iconSkin, bool ShowCheckbox, bool CanCheck, string Nodepath = "")
        {
            AppTreeNode node = new AppTreeNode()
            {
                id = id,
                name = name,
                Code = code,
                NodeType = type,
                isParent = isParent,
                pId = pId,
                iconSkin = iconSkin,
                nocheck = ShowCheckbox && !CanCheck,
                NodePath = Nodepath

            };
            return node;
        }
    }

    #region 树形节点类和树形菜单类型
    /// <summary>
    /// 树形节点类
    /// </summary>
    class AppTreeNode
    {
        public string id { get; set; }
        public string pId { get; set; }
        public string name { get; set; }
        public string Code { get; set; }
        public string NodeType { get; set; }
        public string ParentCode { get; set; }
        public string iconSkin { get; set; }
        public bool isParent { get; set; }
        public bool nocheck { get; set; }
        //用于返回已勾选节点的节点的所有parentcode 路径
        public string NodePath { get; set; }

    }
    //声明枚举类型，树形菜单展示类型
    enum TreeNodeType
    {
        AppPackge = 1,
        AppMenu = 2,
        SubSheet = 3,
        Field = 4,
        Association = 5,
        AppGroup = 15
    }
    #endregion
}
