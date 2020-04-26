using OThinker;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class OrganizationHandler:AbstractPortalTreeHandler
    {
        //是否管理员
        public bool isAdmin { get; set; }

        public OrganizationHandler()
        {
            //this.isAdmin =UserValidator.ValidateAdministrator();
        }

        /// <summary>
        /// 控制器
        /// </summary>
        private ControllerBase _controller;
        private System.Web.HttpContextBase HttpContext;
        private bool isValidate;
        protected override ControllerBase controller
        {
            get
            {
                return _controller;
            }
        }
        public OrganizationHandler(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
                this.isAdmin = controller.UserValidator.ValidateAdministrator();
            }
        }

        public OrganizationHandler(System.Web.HttpContextBase HttpContext, bool isValidate)
        {
            // TODO: Complete member initialization
            this.HttpContext = HttpContext;
            this.isValidate = isValidate;
        }

        //引擎
        public IEngine Engine
        {
            get
            {
                return this.controller.Engine;
            }
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public UserValidator UserValidator
        {
            get 
            { 
                return this.controller.UserValidator;
            }
        }
       
        public override object CreatePortalTree(string functionId, string functionCode)
        {
            //return null;
            if (string.IsNullOrWhiteSpace(functionId)) functionId = this.Engine.Organization.RootUnit.ObjectID;
            return LoadOrgChildNode(functionId);
        }
       
        /// <summary>
        /// 创建组织结构孩子节点
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public List<PortalTreeNode> LoadOrgChildNode(
            string unitID,
            OThinker.Organization.UnitType UnitType = OThinker.Organization.UnitType.Unspecified,
            bool Recursive = false)
        {
            //哪些是可见的
            List<Unit> stateFilteredChildren = this.Engine.Organization.GetChildUnits(
                unitID,
                UnitType,
                Recursive,
                 OThinker.Organization.State.Active);

            // 按照可见类型进行过滤
            OThinker.Organization.Unit[] children = FilterByAdmin(this.isAdmin, stateFilteredChildren);
            if (children == null || children.Length == 0) { return null; }

            // 按照OrganizationUnit, Group, User的顺序进行排列
            ArrayList unitList = SortOrgList(children);

            List<PortalTreeNode> treeNode = new List<PortalTreeNode>();
            foreach (OThinker.Organization.Unit child in unitList)
            {
                if (treeNode.Any(p => p.ObjectID == child.ObjectID)) continue;

                switch (child.UnitType)
                {
                  
                    case OThinker.Organization.UnitType.OrganizationUnit:
                        //treeNode.Add(BuildOrgChildNode(unitID, child, "/IB_OU.gif", ConstantString.PagePath_EditOrgUnit));
                        
                        treeNode.Add(BuildOrgChildNode(unitID, child, "fa icon-zuzhitubiao", ConstantString.PagePath_EditOrgUnit));
                        break;
                    case OThinker.Organization.UnitType.Group:
                        //treeNode.Add(BuildOrgChildNode(unitID, child, "/IB_SelectUser.gif", ConstantString.PagePath_EditGroup));
                        treeNode.Add(BuildOrgChildNode(unitID, child, "fa fa-users", ConstantString.PagePath_EditGroup));
                        break;
                    case OThinker.Organization.UnitType.User:
                        //string iconName = ((OThinker.Organization.User)child).State == Organization.State.Active ? "/IB_Man.gif" : "/IB_Man2.gif";
                        string iconName = "fa fa-user";
                        treeNode.Add(BuildOrgChildNode(unitID, child, iconName, ConstantString.PagePath_EditUser));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return treeNode;
        }
        #region 创建组织机构的树节点
        /// <summary>
        /// 创建组织机构的树节点
        /// </summary>
        /// <param name="unitID"></param>
        /// <param name="child"></param>
        /// <param name="imgName"></param>
        /// <returns></returns>
        private PortalTreeNode BuildOrgChildNode(string unitID, OThinker.Organization.Unit child, string imgName, string pagePath)
        {
            //组织机构没有Code使用ObjectIDea，user对象使用Code
            string Code = child.ObjectID;
            if (child.UnitType == OThinker.Organization.UnitType.User) { Code = ((OThinker.Organization.User)child).Code; }
            
            PortalTreeNode node = new PortalTreeNode()
            {
                Text = "_"+child.Name,//在资源文件解析的时候，如果以_开头，则返回_之后的字符串，否则会根据资源文件里面的字符串替换，组织结构名称会找不到
                ObjectID = child.ObjectID,
                Code = Code,
                IsLeaf = child.UnitType == OThinker.Organization.UnitType.User,
                //Icon = this.PortalImageRoot + imgName,
                Icon = imgName,
                ParentID = unitID,
                ShowPageUrl = GetOrgTreeNodeEditUrl(pagePath, child.UnitID, unitID),
                NodeType = FunctionNodeType.Organization
            };
            if (!node.IsLeaf)
            {
                //if (string.IsNullOrWhiteSpace(LoadDataUrl))
                    node.LoadDataUrl = GetLoadDataUrl(node.ObjectID, node.Code, FunctionNodeType.Organization);
                //else
                //    node.LoadDataUrl = LoadDataUrl + node.ObjectID;
            }
            return node;
        }
        #endregion

        #region 组织结构树节点的编辑界面
        
        /// <summary>
        /// 组织结构树节点的编辑界面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="editId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        /// , EditUserType userType = EditUserType.View 编辑模式参数先去掉,保证显示 //TODO
        public string GetOrgTreeNodeEditUrl(string url, string editId, string parentId = "",EditUserType userType = EditUserType.View)
        {
            return url + "&" + Param_Mode + "=" + userType + "&" +
               Param_ID + "=" + editId + "&" +
               Param_Parent + "=" + parentId;

        }
        #endregion

        #region 排序组织列表
        /// <summary>
        /// 排序组织列表
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private ArrayList SortOrgList(OThinker.Organization.Unit[] children)
        {
            ArrayList unitList = new ArrayList();
            ArrayList keyList = new ArrayList();
            foreach (OThinker.Organization.Unit child in children)
            {
                //编制已取消
                //if (child.UnitType == OThinker.Organization.UnitType.Staff) continue;
                string key = null;
                switch (child.UnitType)
                {
                    case OThinker.Organization.UnitType.OrganizationUnit:
                        key = "c" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    case OThinker.Organization.UnitType.Group:
                        key = "e" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    case OThinker.Organization.UnitType.User:
                        key = "f" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                keyList.Add(key);
                unitList.Add(child);
            }
            unitList = OThinker.Data.Sorter.Sort(keyList, unitList);
            return unitList;
        }
        #endregion

        #region 组织结构按照可见类型进行过滤
        /// <summary>
        /// 按照可见类型进行过滤
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="stateFilteredChildren"></param>
        /// <returns></returns>
        private OThinker.Organization.Unit[] FilterByAdmin(bool isAdmin, List<Unit> stateFilteredChildren)
        {
            OThinker.Organization.Unit[] children = null;
            if (isAdmin)
            {
                children = stateFilteredChildren.ToArray();
            }
            else
            {
                // 如果不是管理员，那么去掉仅管里可见的
                List<OThinker.Organization.Unit> childList = new List<OThinker.Organization.Unit>();
                foreach (OThinker.Organization.Unit unit in stateFilteredChildren)
                {
                    if (unit is OThinker.Organization.User)
                    {
                        //管理员和自己
                        if (((OThinker.Organization.User)unit).PrivacyLevel == OThinker.Organization.PrivacyLevel.Confidential && this.UserValidator.UserID !=unit.ObjectID) continue;
                        //仅本部门的成员
                        if (((OThinker.Organization.User)unit).PrivacyLevel == OThinker.Organization.PrivacyLevel.PublicToDept &&
                                 this.UserValidator.User.ParentID != ((OThinker.Organization.User)unit).ParentID) continue;

                        //仅本部门的成员和上级部门成功
                        if (((OThinker.Organization.User)unit).PrivacyLevel == OThinker.Organization.PrivacyLevel.PublicToDepts)
                        {
                            if (this.UserValidator.User.ParentID != ((OThinker.Organization.User)unit).ParentID //不是本部门
                                && this.UserValidator.User.ParentID != this.Engine.Organization.GetParent(((OThinker.Organization.User)unit).ParentID))//不是上级部门
                                continue;
                        }
                    }
                    if (
                       //unit is OThinker.Organization.NonCompany &&
                       //(
                           unit.Visibility == OThinker.Organization.VisibleType.OnlyAdmin ||
                           (
                               (unit.Visibility == OThinker.Organization.VisibleType.OnlyOrganization &&
                               !this.Engine.Organization.GetChildren(unit.UnitID, OThinker.Organization.UnitType.User, true, OThinker.Organization.State.Unspecified)
                               .Contains(this.UserValidator.UserID)
                           )
                       )
                      )
                    {
                        continue;
                    }
                    //添加
                    childList.Add(unit);
                }
                children = childList.ToArray();
            }
            return children;
        }
        #endregion


    }
}
