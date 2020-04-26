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
using OThinker.Organization;
using System.Reflection;
using System.Collections;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 选人控件服务
    /// </summary>
    [Authorize]
    public class SheetUserController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SheetUserController()
        {
        }

        #region SheetUser 属性 ------------------
        private string _Options = string.Empty;
        /// <summary>
        /// 获取或设置可见参数
        /// </summary>
        private string Options
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Options))
                {
                    _Options = this.Request["o"] + string.Empty;
                }
                return _Options;
            }
        }

        /// <summary>
        /// 获取或设置OU是否可见
        /// </summary>
        private bool OrgUnitVisible
        {
            get
            {
                return Options.IndexOf('O') > -1;
            }
        }

        /// <summary>
        /// 获取或设置组是否可见
        /// </summary>
        private bool GroupVisible
        {
            get
            {
                return Options.IndexOf('G') > -1;
            }
        }

        /// <summary>
        /// 获取或设置用户是否可见
        /// </summary>
        private bool UserVisible
        {
            get
            {
                return Options.IndexOf('U') > -1;
            }
        }

        private string _RootUnitID = null;
        /// <summary>
        /// 获取选人控件是否有设置根节点
        /// </summary>
        private string RootUnitID
        {
            get
            {
                if (_RootUnitID == null)
                {
                    _RootUnitID = this.Request["RootUnitID"] + string.Empty;
                    if (this.VisibleUnits.Length == 1)
                    { // 只有1个可见的组织单元，则以该组织单元为顶点显示
                        this._RootUnitID = this.VisibleUnits[0];
                    }

                    if (_RootUnitID == string.Empty)
                    {
                        this._RootUnitID = this.Engine.Organization.RootUnit.ObjectID;
                    }
                    else if (_RootUnitID.ToLower() == "{current}" || _RootUnitID.ToLower() == "current")
                    {
                        _RootUnitID = this.UserValidator.User.ParentID;
                    }
                }
                return this._RootUnitID;
            }
        }

        private bool? _Recursive = null;
        /// <summary>
        /// 获取是否允许递归显示
        /// </summary>
        private bool Recursive
        {
            get
            {
                if (_Recursive == null)
                {
                    bool b = false;
                    if (!bool.TryParse(this.Request["Recursive"] + string.Empty, out b))
                    {
                        _Recursive = false;
                    }
                    else
                    {
                        _Recursive = b;
                    }
                }
                return (bool)_Recursive;
            }
        }

        private bool? _LoadTree = null;
        /// <summary>
        /// 获取是否在加载树节点
        /// </summary>
        private bool LoadTree
        {
            get
            {
                if (_LoadTree == null)
                {
                    bool b = false;
                    if (!bool.TryParse(this.Request["LoadTree"] + string.Empty, out b))
                    {
                        _LoadTree = false;
                    }
                    else
                    {
                        _LoadTree = b;
                    }
                }
                return (bool)_LoadTree;
            }
        }

        private string[] _VisibleUnits = null;
        /// <summary>
        /// 获取允许显示的组织单元ID集合
        /// </summary>
        private string[] VisibleUnits
        {
            get
            {
                if (_VisibleUnits == null)
                {
                    string visibleUnits = string.IsNullOrEmpty(this.Request["VisibleUnits"] + string.Empty) ? (this.Request["V"] + string.Empty) : (this.Request["VisibleUnits"] + string.Empty);
                    _VisibleUnits = visibleUnits.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                return this._VisibleUnits;
            }
        }

        private List<string> _VisibleUnitIds = null;
        /// <summary>
        /// 获取所有可见的组织单元
        /// </summary>
        private List<string> VisibleUnitIds
        {
            get
            {
                if (this._VisibleUnitIds == null)
                {
                    this._VisibleUnitIds = new List<string>();

                    foreach (string unit in this.VisibleUnits)
                    {
                        if (!this._VisibleUnitIds.Contains(unit))
                        {
                            this._VisibleUnitIds.Add(unit);
                        }

                        List<string> parents = this.Engine.Organization.GetParents(unit,
                            OThinker.Organization.UnitType.OrganizationUnit,
                            true,
                            Organization.State.Active);

                        List<string> childs = this.Engine.Organization.GetParents(unit,
                            OThinker.Organization.UnitType.OrganizationUnit,
                            true,
                            Organization.State.Active);

                        if (parents != null)
                        {
                            foreach (string parent in parents)
                            {
                                if (!this._VisibleUnitIds.Contains(parent))
                                {
                                    this._VisibleUnitIds.Add(parent);
                                }
                            }
                        }
                        if (childs != null)
                        {
                            foreach (string parent in childs)
                            {
                                if (!this._VisibleUnitIds.Contains(parent))
                                {
                                    this._VisibleUnitIds.Add(parent);
                                }
                            }
                        }
                    }
                }

                return this._VisibleUnitIds;
            }
        }

        private string _OrgPostCode = null;
        /// <summary>
        /// 获取显示的岗位编码
        /// </summary>
        public string OrgPostCode
        {
            get
            {
                if (_OrgPostCode == null)
                {
                    _OrgPostCode = HttpUtility.UrlDecode(Request["OrgPostCode"] + string.Empty);
                }
                return this._OrgPostCode;
            }
        }

        private string _UserCodes = null;
        /// <summary>
        /// 获取显示指定的用户帐号集合
        /// </summary>
        public string UserCodes
        {
            get
            {
                if (_UserCodes == null)
                {
                    _UserCodes = HttpUtility.UrlDecode(Request["UserCodes"] + string.Empty);
                }
                return this._UserCodes;
            }
        }
        #endregion

        #region 成员属性 ------------------------
        /// <summary>
        /// 组织机构权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return string.Empty;
            }
        }


        private Unit _RootUnit = null;
        /// <summary>
        /// 获取选人组织树的根节点
        /// </summary>
        public Unit RootUnit
        {
            get
            {
                if (this._RootUnit == null)
                {
                    this._RootUnit = this.Engine.Organization.GetUnit(this.RootUnitID);
                }
                return this._RootUnit;
            }
        }

        private string _ParentID;
        /// <summary>
        /// 获取当前ParentID
        /// </summary>
        private string ParentID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ParentID))
                {
                    _ParentID = this.Request["ParentID"] + string.Empty;
                    if (_ParentID == string.Empty) _ParentID = RootUnitID;
                }
                return _ParentID;
            }
        }

        private Unit _ParentUnit = null;
        /// <summary>
        /// 获取当前ParentUnit
        /// </summary>
        private Unit ParentUnit
        {
            get
            {
                if (this._ParentUnit == null)
                {
                    this._ParentUnit = this.Engine.Organization.GetUnit(this.ParentID);
                }
                return _ParentUnit;
            }
        }

        private string _SearchKey;
        /// <summary>
        /// 搜索关键字
        /// </summary>
        private string SearchKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_SearchKey))
                {
                    _SearchKey = HttpUtility.UrlDecode(this.Request["SearchKey"] ?? string.Empty);
                }
                return _SearchKey;
            }
        }

        private string _IsMobile = string.Empty;
        /// <summary>
        /// 获取是否是移动办公访问
        /// </summary>
        private bool IsMobile
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_IsMobile))
                {
                    _IsMobile = this.Request["IsMobile"] ?? "";
                }
                return string.Compare(_IsMobile, true.ToString(), true) == 0;
            }
        }
        #endregion

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Propertystr"></param>
        /// <returns></returns>
        public JsonResult GetUserProperty(string UserID, string Propertystr)
        {
            return ExecuteFunctionRun(() =>
            {
                if (string.IsNullOrWhiteSpace(UserID)) { return null; }
                string[] Propertys = Propertystr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (Propertys == null) { return null; }

                Dictionary<string, object> PropertyJson = new Dictionary<string, object>();


                Organization.Unit unit = this.Engine.Organization.GetUnit(UserID);
                if (unit == null)
                {
                    unit = this.Engine.Organization.GetUserByCode(UserID);
                    if (unit == null)
                        return null;
                }

                foreach (string p in Propertys)
                {
                    if (PropertyJson.ContainsKey(p))
                    {
                        continue;
                    }
                    switch (p)
                    {
                        case Organization.User.PropertyName_DepartmentName:
                            if (unit.UnitType == Organization.UnitType.User)
                            {
                                PropertyJson.Add(p, ((OThinker.Organization.User)unit).DepartmentName);
                            }
                            break;
                        default:
                            PropertyInfo PropertyInfo = unit.GetType().GetProperty(p);
                            if (PropertyInfo == null)
                            {
                                PropertyJson.Add(p, null);
                            }
                            else
                            {
                                PropertyJson.Add(p, PropertyInfo.GetValue(unit, null));
                            }
                            break;
                    }
                }

                return Json(PropertyJson, JsonRequestBehavior.AllowGet);

            }, string.Empty);
        }

        /// <summary>
        /// 加载组织机构树  
        /// </summary>
        [AllowAnonymous]
        public JsonResult LoadOrgTreeNodes()
        {
            if (!string.IsNullOrEmpty(this.OrgPostCode))
            { // 显示指定角色的用户
                List<PortalTreeNode> results = this.LoadNodesByPostCode(this.OrgPostCode);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else if (!string.IsNullOrEmpty(this.UserCodes))
            { // 显示指定帐号的用户集合
                List<PortalTreeNode> results = this.LoadNodesByUserCodes(this.UserCodes);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (SearchKey != string.Empty && !LoadTree)
                {// 搜索用户
                    List<PortalTreeNode> results = this.SearchUser(this.SearchKey, this.ParentID);
                    return Json(results, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    PortalTreeNode treeNode = this.GetTreeNodeFromUnit(ParentUnit);
                    this.LoadChildrenNodes(treeNode);
                    if (Recursive)
                    {
                        return Json(new PortalTreeNode[1] { treeNode }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(treeNode.children, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        #region 已整理方法 ---------------------
        /// <summary>
        /// 根据岗位名称查找
        /// </summary>
        /// <param name="postCode"></param>
        /// <returns></returns>
        private List<PortalTreeNode> LoadNodesByPostCode(string postCode)
        {
            List<string> postUsers = this.Engine.Organization.GetUsersByJobCode(this.UserValidator.UserID, postCode, string.Empty);

            List<PortalTreeNode> childNodes = new List<PortalTreeNode>();
            if (postUsers == null || postUsers.Count == 0) return childNodes;

            List<Unit> units = this.Engine.Organization.GetUnits(postUsers.ToArray());
            if (units == null || units.Count == 0) return childNodes;

            foreach (Unit childUnit in units)
            {
                PortalTreeNode childNode = this.GetTreeNodeFromUnit(childUnit);
                childNodes.Add(childNode);
            }
            return childNodes;
        }

        /// <summary>
        /// 根据用户帐号查找
        /// </summary>
        /// <param name="userCodes"></param>
        /// <returns></returns>
        private List<PortalTreeNode> LoadNodesByUserCodes(string userCodes)
        {
            List<PortalTreeNode> childNodes = new List<PortalTreeNode>();
            string[] codes = userCodes.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
            if (codes.Length == 0) return childNodes;
            foreach (string code in codes)
            {
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(code);
                if (user != null)
                {
                    PortalTreeNode childNode = this.GetTreeNodeFromUnit(user);
                    childNodes.Add(childNode);
                }
            }

            return childNodes;
        }

        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<PortalTreeNode> SearchUser(string searchKey, string parentId)
        {
            List<PortalTreeNode> childNodes = new List<PortalTreeNode>();

            //OU
            if (this.OrgUnitVisible)
            {
                searchKey = searchKey.Replace("'", string.Empty).Replace("-", string.Empty);
                var querySql = "SELECT TOP 20 ObjectID from OT_OrganizationUnit WHERE Name LIKE '%" + searchKey + "%'";
                System.Data.DataTable dtUnit = Engine.Query.QueryTable(string.Format(querySql, searchKey, parentId));
                if (dtUnit != null)
                {
                    List<string> UnitIDs = new List<string>();
                    foreach (System.Data.DataRow row in dtUnit.Rows)
                    {
                        var id = row[OThinker.Organization.User.PropertyName_ObjectID] + string.Empty;
                        if (!string.IsNullOrEmpty(id))
                        {
                            UnitIDs.Add(id);
                        }
                    }
                    if (UnitIDs.Count > 0)
                    {
                        List<Organization.Unit> units = this.Engine.Organization.GetUnits(UnitIDs.ToArray());
                        foreach (Unit childUnit in units)
                        {
                            PortalTreeNode childNode = this.GetTreeNodeFromUnit(childUnit);
                            childNode.Code = "";
                            childNodes.Add(childNode);
                        }
                    }
                }
            }
            //用户
            if (this.UserVisible)
            {
                List<User> users = this.Engine.Organization.QueryUserByParentID(searchKey, State.Active, parentId, 10);
                if (users == null || users.Count() == 0) return childNodes;
                foreach (Unit childUnit in users)
                {
                    PortalTreeNode childNode = this.GetTreeNodeFromUnit(childUnit);
                    childNodes.Add(childNode);
                }
            }
            return childNodes;
        }

        /// <summary>
        /// 递归获取子组织单元
        /// </summary>
        /// <param name="parentTreeNode"></param>
        private void LoadChildrenNodes(PortalTreeNode parentTreeNode)
        {
            List<Unit> childUnits = null;
            childUnits = this.Engine.Organization.GetChildUnits(parentTreeNode.Code,
                 this.getChildUnitType(),
                 false,
                 State.Active);
            if (!UserValidator.User.IsAdministrator)
            {
                childUnits = childUnits.Where(w => w.Visibility != OThinker.Organization.VisibleType.OnlyAdmin).ToList();
            }

            // 按照OrganizationUnit, Group, User的顺序进行排列
            ArrayList unitList = SortOrgList(childUnits.ToArray());
            if (childUnits != null)
            {
                List<PortalTreeNode> childNodes = new List<PortalTreeNode>();
                foreach (Unit childUnit in unitList)
                {
                    if (this.VisibleUnits.Length > 0)
                    {
                        if (childUnit.UnitType == UnitType.OrganizationUnit) {
                            if (!this.VisibleUnitIds.Contains(childUnit.ObjectID)) continue;
                        }
                        else if (childUnit.UnitType==UnitType.User) {
                            if (!this.VisibleUnitIds.Contains(childUnit.ParentID)) continue;
                        }
                    }
                    if (childUnit is Organization.User)
                    {
                        //系统、禁用、离职用户无法搜索
                       //if (((Organization.User)childUnit).IsSystemUser == true ||
                       //     ((Organization.User)childUnit).State == State.Inactive ||
                       //     ((Organization.User)childUnit).ServiceState == UserServiceState.Dismissed)
                        if (((Organization.User)childUnit).State == State.Inactive ||
                            ((Organization.User)childUnit).ServiceState == UserServiceState.Dismissed)
                        {
                            continue;
                        }
                    }
                    PortalTreeNode childNode = this.GetTreeNodeFromUnit(childUnit);
                    childNodes.Add(childNode);
                }
                parentTreeNode.children = childNodes;
            }
        }

        /// <summary>
        /// 获取子节点类型
        /// </summary>
        /// <returns></returns>
        private UnitType getChildUnitType()
        {
            if (this.LoadTree)
            {
                return UnitType.OrganizationUnit | UnitType.Group;
            }
            else
            {
                UnitType t = UnitType.Post;
                if (this.IsMobile)
                {
                    if (this.UserVisible)
                    {
                        t = t | UnitType.User;
                        t = t | UnitType.Group;
                        t = t | UnitType.OrganizationUnit;
                        return t;
                    }
                    if (this.GroupVisible)
                    {
                        t = t | UnitType.Group;
                        t = t | UnitType.OrganizationUnit;
                        return t;
                    }
                    if (this.OrgUnitVisible)
                    {
                        t = t | UnitType.OrganizationUnit;
                        return t;
                    }
                }
                else
                {
                    if (this.OrgUnitVisible)
                    {
                        t = t | UnitType.OrganizationUnit;
                    }
                    if (this.GroupVisible)
                    {
                        t = t | UnitType.Group;
                    }
                    if (this.UserVisible)
                    {
                        t = t | UnitType.User;
                    }
                }
                return t;
            }
        }

        /// <summary>
        /// 获取一个树节点
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private PortalTreeNode GetTreeNodeFromUnit(Unit unit)
        {
            PortalTreeNode treeNode = new PortalTreeNode();
            treeNode.ObjectID = unit.ObjectID;

            if (unit is Organization.User)
            {
                treeNode.Code = ((Organization.User)unit).Code;
                treeNode.ExtendObject = new
                {
                    UnitType = ((Organization.User)unit).UnitType.ToString().Substring(0, 1),
                    DepartmentName=this.Engine.Organization.GetName(unit.ParentID)
                };
            }
            else
            {
                var childUnits = this.Engine.Organization.GetChildUnits(unit.UnitID, UnitType.Group | UnitType.OrganizationUnit|UnitType.User, false, State.Active);
                bool hasChildren = childUnits != null && childUnits.Any(s => s.UnitType == UnitType.Group || s.UnitType == UnitType.OrganizationUnit);
                treeNode.Code = unit.UnitID;
                treeNode.ExtendObject = new
                {
                    UnitType = unit.UnitType.ToString().Substring(0, 1),
                    HasChildren = hasChildren,//是否有子组织单元或者组
                    ChildrenCount = childUnits.Count()
                };
            }

            treeNode.Text = unit.Name;
            treeNode.IsLeaf = unit.UnitType == UnitType.Group;
            if (unit.UnitType == UnitType.OrganizationUnit)
            {
                treeNode.Icon = "icon-zuzhitubiao";
            }
            else if (unit.UnitType == UnitType.Group)
            {
                treeNode.Icon = "fa fa-users";
            }
            else
            {
                treeNode.Icon = "fa fa-user";
            }
            return treeNode;
        }
        #endregion

        /// <summary>
        /// 增加用户节点
        /// </summary>
        /// <param name="treeObj"></param>
        /// <param name="user"></param>
        private void AddUserItem(List<PortalTreeNode> treeObj, User user)
        {
            AddUserItem(treeObj, user, user.Name + "[" + user.Code + "]");
        }

        private List<string> ExistsOrg = new List<string>();

        /// <summary>
        /// 增加用户节点
        /// </summary>
        /// <param name="treeObj"></param>
        /// <param name="user"></param>
        /// <param name="DisplayName"></param>
        private void AddUserItem(List<PortalTreeNode> treeObj, User user, string DisplayName)
        {
            if (!ExistsOrg.Contains(user.ObjectID))
            {
                treeObj.Add(new PortalTreeNode()
                {
                    Code = user.Code,
                    Text = DisplayName,
                    ObjectID = user.ObjectID,
                    // IsLeaf = true,
                    Icon = string.Empty,
                    ParentID = user.ParentID,
                    ExtendObject = new { UnitType = user.UnitType.ToString().Substring(0, 1) },
                    NodeType = Acl.FunctionNodeType.Organization
                });

                ExistsOrg.Add(user.ObjectID);
            }
        }



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
        // End Class
    }
}