using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    /// <summary>
    /// 定义职务控制器
    /// </summary>
    [Authorize]
    public class OrgJobController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.Organization_Data_Code;
            }
        }

        public OrgJobController() { }


        #region 角色

        /// <summary>
        /// 获取所有角色列表（下拉框）
        /// </summary>
        /// <param name="roleId">需要排除的角色</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRoleListSelect(string roleId)
        {
            return ExecuteFunctionRun(() =>
            {

                List<OrgJobViewModel> listRole = new List<OrgJobViewModel>();
                //listRole.Add(new OrgJobViewModel() { ObjectID = "", DisplayName = "" });//空记录


                List<OThinker.Organization.Unit> listunits = this.Engine.Organization.GetAllUnits(OThinker.Organization.UnitType.Post);

                if (listunits != null && listunits.Count > 0)
                {
                    foreach (OThinker.Organization.Unit u in listunits.OrderBy(e => e.SortKey))
                    {
                        listRole.Add(new OrgJobViewModel { ObjectID = u.ObjectID, DisplayName = u.Name });
                    }
                }

                var lists = listRole.Where(s => s.ObjectID != roleId).Select(s => new
                {
                    text = s.DisplayName,
                    id = s.ObjectID
                });

                return Json(lists, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取所有角色列表（下拉框）
        /// </summary>
        /// <param name="roleId">需要排除的角色</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRoleListSelectByName(string roleName)
        {
            return ExecuteFunctionRun(() =>
            {
                List<OrgJobViewModel> listRole = new List<OrgJobViewModel>();
                //listRole.Add(new OrgJobViewModel() { ObjectID = "", DisplayName = "" });//空记录


                List<OThinker.Organization.Unit> listunits = this.Engine.Organization.GetAllUnits(OThinker.Organization.UnitType.Post);

                if (listunits != null && listunits.Count > 0)
                {
                    foreach (OThinker.Organization.Unit u in listunits.OrderBy(e => e.SortKey))
                    {
                        listRole.Add(new OrgJobViewModel { ObjectID = u.ObjectID, DisplayName = u.Name });
                    }
                }

                var lists = listRole.Where(s => s.DisplayName.Contains(roleName)).Select(s => new
                {
                    text = s.DisplayName,
                    id = s.ObjectID
                });

                return Json(lists, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取所有的角色列表（筛选）
        /// </summary>
        /// <returns>所有的角色列表</returns>
        public JsonResult GetRoleList(string roleCode, string roleName)
        {
            return ExecuteFunctionRun(() =>
            {
                List<OrgJobViewModel> listRole = new List<OrgJobViewModel>();
                List<OThinker.Organization.Unit> listunits = this.Engine.Organization.GetAllUnits(OThinker.Organization.UnitType.Post);
                if (listunits != null && listunits.Count > 0)
                {
                    foreach (OThinker.Organization.Unit u in listunits)
                    {
                        OThinker.Organization.OrgPost post = u as OThinker.Organization.OrgPost;
                        //用户数
                        int userCount = (post.ChildList == null) ? 0 : post.ChildList.Length;

                        //上级角色
                        OThinker.Organization.OrgPost parentPost = this.Engine.Organization.GetUnit(post.SuperiorID) as OThinker.Organization.OrgPost;

                        listRole.Add(
                            new OrgJobViewModel
                            {
                                Code = post.Code,
                                DisplayName = post.Name,
                                ObjectID = post.ObjectID,
                                Sortkey = post.SortKey,
                                RoleLevel = post.JobLevel,
                                UserCount = userCount,
                                ParentPostObjectID = parentPost == null ? "" : parentPost.Code,
                                ParentPostName = parentPost == null ? "" : parentPost.Name,
                                Description = post.Description
                            });
                    }
                }
                if (!string.IsNullOrEmpty(roleCode))
                {
                    listRole = listRole.Where(r => r.Code.ToLower().Contains(roleCode.ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(roleName) && listRole != null)
                {
                    //筛选
                    listRole = listRole.Where(r => r.DisplayName.Contains(roleName)).ToList();
                }

                listRole = listRole.OrderBy(r => r.Sortkey).ToList();

                var gridData = CreateLigerUIGridData(listRole.ToArray());
                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存或更新角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveRole(OrgJobViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                try
                {
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        //编码必须以字母开始，不让创建到数据库表字段时报错
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                        if (!regex.Match(model.Code).Success)
                        {
                            result.Success = false;
                            result.Message = "EditBizObjectSchemaProperty.Msg4";
                            return Json(result);
                        }
                        List<OThinker.Organization.Unit> listunits = this.Engine.Organization.GetAllUnits(OThinker.Organization.UnitType.Post);

                        var filterPost = listunits.Where(s => ((OThinker.Organization.OrgPost)s).Code == model.Code).FirstOrDefault();
                        if (filterPost != null)
                        {
                            result.Success = false;
                            result.Message = "msgGlobalString.CodeDuplicate";
                            return Json(result, JsonRequestBehavior.DenyGet);
                        }
                        //新增
                        OThinker.Organization.OrgPost post = new OThinker.Organization.OrgPost()
                        {
                            Code = model.Code,
                            Name = model.DisplayName,
                            SuperiorID = model.ParentPostObjectID,
                            Description = model.Description,
                            SortKey = model.Sortkey,
                            JobLevel = model.RoleLevel
                        };

                        this.Engine.Organization.AddUnit(this.UserValidator.UserID, post);
                    }
                    else
                    {
                        //更新
                        OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(model.ObjectID) as OThinker.Organization.OrgPost;
                        if (post != null)
                        {
                            post.Name = model.DisplayName;
                            post.SuperiorID = model.ParentPostObjectID;
                            post.Description = model.Description;
                            post.JobLevel = model.RoleLevel;
                            post.SortKey = model.Sortkey;
                        }

                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);
                    }
                }
                catch (Exception ex)
                {
                    result.Message = "msgGlobalString.SaveFailed";
                    result.Extend = ex.Message;
                    return Json(result, JsonRequestBehavior.DenyGet);
                }

                result.Success = true;
                result.Message = "msgGlobalString.SaveSucced";
                return Json(result, JsonRequestBehavior.DenyGet);

            });
        }

        /// <summary>
        /// 根据角色ID获取角色信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetRoleByID(string roleId)
        {
            return ExecuteFunctionRun(() =>
            {
                OrgJobViewModel model = new OrgJobViewModel();
                OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(roleId) as OThinker.Organization.OrgPost;

                if (post == null)
                {
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                //上级角色
                OThinker.Organization.OrgPost parentPost = this.Engine.Organization.GetUnit(post.SuperiorID) as OThinker.Organization.OrgPost;

                model = new OrgJobViewModel
                {
                    ObjectID = post.ObjectID,
                    Code = post.Code,
                    DisplayName = post.Name,
                    Description = post.Description,
                    ParentPostObjectID = parentPost == null ? "" : parentPost.ObjectID,
                    ParentPostName = parentPost == null ? "" : parentPost.Name,
                    Sortkey = post.SortKey,
                    RoleLevel = post.JobLevel
                };

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult DelRole(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");

                string[] idArr = id.Split(';');
                if (idArr != null && idArr.Length > 0)
                {
                    foreach (string d in idArr)
                    {
                        OThinker.Organization.Unit post = this.Engine.Organization.GetUnit(d);
                        this.Engine.Organization.RemoveUnit(OThinker.Organization.User.AdministratorID, post);
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        #endregion

        #region 角色用户关系

        /// <summary>
        /// 角色用户关系列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleUserList(string roleId, string userCode, string userName)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(roleId) as OThinker.Organization.OrgPost;

                List<RoleUserViewModel> lstRoleUsers = new List<RoleUserViewModel>();

                if (post != null && post.ChildList != null && post.ChildList.Length > 0)
                {
                    foreach (OThinker.Organization.OrgStaff staff in post.ChildList)
                    {
                        string userid = staff.UserID;
                        OThinker.Organization.User _user = this.Engine.Organization.GetUnit(userid) as OThinker.Organization.User;
                        if (_user == null) { continue; }
                        string _userCode = _user.Code;
                        string _userName = this.Engine.Organization.GetUnit(userid).Name;
                        string OrgName = this.Engine.Organization.GetName(_user.ParentID);
                        _userName = _userName + "(" + OrgName + ")";

                        //范围组织名称
                        string[] scopeids = staff.OUScope;
                        Dictionary<string, string> dicNames = this.Engine.Organization.GetNames(scopeids);
                        string scopeNames = "";
                        foreach (string key in dicNames.Keys) { scopeNames += dicNames[key] + ";"; }

                        lstRoleUsers.Add(new RoleUserViewModel
                        {
                            ObjectID = staff.ObjectID,
                            RoleID = roleId,
                            UserID = staff.UserID,
                            UserCode = _userCode,
                            UserName = _userName,
                            Sortkey = staff.ParentIndex,//staff 没有对应属性
                            ManagerScope = scopeNames,
                            ManagerScopeIds = string.Join(";", staff.OUScope),
                            Description = staff.Description

                        });
                    }
                }

                //筛选
                if (userCode != null)
                {
                    lstRoleUsers = lstRoleUsers.Where(r => r.UserCode.ToLower().Contains(userCode.ToLower())).ToList();
                }

                if (userCode != null && lstRoleUsers != null)
                {
                    lstRoleUsers = lstRoleUsers.Where(r => r.UserName.Contains(userName)).ToList();
                }
                if (lstRoleUsers.Count > 0)
                {
                    lstRoleUsers = lstRoleUsers.OrderBy(r => r.Sortkey).ToList();
                }

                var gridData = CreateLigerUIGridData(lstRoleUsers.ToArray());
                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存角色用户关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveRoleUsers(RoleUserViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");
                List<RoleUserViewModel> listusers = new List<RoleUserViewModel>();
                try
                {
                    OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(model.RoleID) as OThinker.Organization.OrgPost;
                    if (post != null)
                    {
                        if (model.UserID != null)
                        {
                            var ids = model.UserID.Split(';');
                            for (int i = 0; i < ids.Length; i++)
                            {
                                OThinker.Organization.OrgStaff staff = new OThinker.Organization.OrgStaff()
                                {
                                    ObjectID = string.IsNullOrEmpty(model.ObjectID) ? "" : model.ObjectID,
                                    OUScope = model.ManagerScopeIds == null ? new string[] { } : model.ManagerScopeIds.Split(';'),
                                    UserID = ids[i],
                                    ParentObjectID = model.RoleID,
                                    ParentIndex = model.Sortkey, //ParentIndex 存储排序键
                                    Description = model.Description

                                };

                                //新增
                                if (string.IsNullOrEmpty(staff.ObjectID))
                                {

                                    //新增时判断是否有添加过
                                    if (post.ChildList != null && post.ChildList.Count() > 0)
                                    {
                                        bool flag = false;
                                        foreach (OThinker.Organization.OrgStaff sf in post.ChildList)
                                        {
                                            if (sf.UserID == ids[i])
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }

                                        if (flag)
                                        {
                                            result.Success = false;
                                            result.Message = "用户[" + this.Engine.Organization.GetName(ids[i]) + "]已经存在记录，请选择编辑!";
                                            return Json(result);
                                        }
                                    }

                                    staff.ObjectID = Guid.NewGuid().ToString();
                                    post.AddChildUnit(staff);
                                }
                                //更新
                                else
                                {
                                    post.UpdateChildUnit(staff);
                                }
                            }
                        }


                        post.ChildList = post.ChildList.OrderBy(c => c.ParentIndex).ToArray();
                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);
                    }

                    return Json(result);
                }
                catch (Exception ex)
                {
                    result = new ActionResult(false, "保存失败，" + ex.Message);
                    return Json(result);
                }
            });
        }


        /// <summary>
        /// 获取角色用户关系中单条信息
        /// </summary>
        /// <param name="roleUserId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public JsonResult GetRoleUserById(string roleUserId, string roleId)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(roleId) as OThinker.Organization.OrgPost;
                if (post != null)
                {
                    foreach (OThinker.Organization.OrgStaff s in post.ChildList)
                    {
                        if (s.ObjectID == roleUserId)
                        {
                            RoleUserViewModel model = new RoleUserViewModel()
                            {
                                ObjectID = s.ObjectID,
                                RoleID = roleId,
                                UserID = s.UserID,
                                ManagerScopeIds = string.Join(";", s.OUScope),
                                Description = s.Description,
                                Sortkey = s.ParentIndex

                            };
                            return Json(model, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 删除角色用户关系
        /// </summary>
        /// <param name="roleUserId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public JsonResult DelRoleUser(string roleUserId, string roleId)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(roleId) as OThinker.Organization.OrgPost;
                if (post != null)
                {
                    List<OThinker.Organization.OrgStaff> listStaff = new List<OThinker.Organization.OrgStaff>();
                    listStaff.AddRange(post.ChildList);
                    for (int i = listStaff.Count - 1; i >= 0; i--)
                    {
                        if (listStaff[i].ObjectID == roleUserId)
                        {
                            // post.RemoveChild(listStaff[i].ObjectID);
                            listStaff.RemoveAt(i);
                            break;
                        }
                    }

                    post.ChildList = listStaff.ToArray();
                    this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);
                }


                ActionResult result = new ActionResult(true, "");
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        #endregion
    }
}
