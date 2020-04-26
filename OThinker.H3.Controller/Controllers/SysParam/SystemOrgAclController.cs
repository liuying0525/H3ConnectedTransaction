using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 组织权限控制器
    /// </summary>
    [Authorize]
    public class SystemOrgAclController : ControllerBase
    {
        /// <summary>
        /// 获取当前权限码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_SystemOrgAcl_Code; }
        }

        /// <summary>
        /// 获取组织权限列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>组织权限分页列表</returns>
        [HttpPost]
        public JsonResult GetSystemOrgAclList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                // 获取所有权限
                List<Acl.SystemOrgAcl> acls = this.Engine.SystemOrgAclManager.GetAllAcls();
                switch (pagerInfo.sortname)
                {
                    case "UserName":
                        if (pagerInfo.sortorder == "asc")
                            acls = acls.OrderBy(a => a.UserID).ToList();
                        else
                        {
                            acls = acls.OrderByDescending(a => a.UserID).ToList();
                        }
                        break;
                    case "OrgScope":
                        if (pagerInfo.sortorder == "asc")
                            acls = acls.OrderBy(a => a.OrgScope).ToList();
                        else
                        {
                            acls = acls.OrderByDescending(a => a.OrgScope).ToList();
                        }
                        break;
                }
                int total = acls == null ? 0 : acls.Count;
                if (null != acls)
                    acls = acls.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                List<SystemOrgAclViewModel> list = new List<SystemOrgAclViewModel>();
                //获取用户ID
                List<string> userIds = acls.Select(s => s.UserID).ToList();
                List<string> orgIds = acls.Select(s => s.OrgScope).ToList();
                Dictionary<string, string> unitNames = this.Engine.Organization.GetNames(userIds.ToArray());
                Dictionary<string, string> orgScopeNames = this.Engine.Organization.GetFullNames(orgIds.ToArray());
                //构造组织权限显示模型
                foreach (Acl.SystemOrgAcl acl in acls)
                {
                    list.Add(new SystemOrgAclViewModel()
                    {
                        ObjectID = acl.ObjectID,
                        Administrator = acl.Administrator,
                        OrgScope = orgScopeNames[acl.OrgScope],
                        UserName = unitNames[acl.UserID],
                        ViewOrgData = acl.ViewOrgData
                    });
                }
               
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取组织权限列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>组织权限分页列表</returns>
        [HttpPost]
        public JsonResult GetSystemOrgAclListParams(PagerInfo pagerInfo,QuerySystemOrgAclListParam param)
        {
            return ExecuteFunctionRun(() =>
            {
                // 获取所有权限
                List<Acl.SystemOrgAcl> acls = this.Engine.SystemOrgAclManager.GetAllAcls();
               
                string UserID = string.Empty;
                string OUName = string.Empty;
                List<Acl.SystemOrgAcl> orgAcl = new List<SystemOrgAcl>();
                if (param.UserName != null)
                {
                    UserID = param.UserName[0] + string.Empty;
                }
                if (param.OUName != null)
                {
                    OUName = param.OUName[0] + string.Empty;
                }
                if (OUName == "" && UserID == "") { orgAcl = acls; }
                else if(OUName != "" && UserID == "")
                {         
                   var OUNameArray= OUName.Split(',');
                   orgAcl = acls.Where(w => OUNameArray.Contains(w.OrgScope)).ToList();
                }
                else if (OUName == "" && UserID != "")
                {
                    var UserIDArray = UserID.Split(',');
                    orgAcl = acls.Where(w => UserIDArray.Contains(w.UserID)).ToList();
                }
                else if (OUName != "" && UserID != "")
                {
                    var OUNameArray = OUName.Split(',');
                    var UserIDArray = UserID.Split(',');
                    orgAcl = acls.Where(w => OUNameArray.Contains(w.OrgScope) && UserIDArray.Contains(w.UserID)).ToList();
                }
                switch (pagerInfo.sortname)
                {
                    case "UserName":
                        if (pagerInfo.sortorder == "asc")
                            orgAcl = orgAcl.OrderBy(a => a.UserID).ToList();
                        else
                        {
                            orgAcl = orgAcl.OrderByDescending(a => a.UserID).ToList();
                        }
                        break;
                    case "OrgScope":
                        if (pagerInfo.sortorder == "asc")
                            orgAcl = orgAcl.OrderBy(a => a.OrgScope).ToList();
                        else
                        {
                            orgAcl = orgAcl.OrderByDescending(a => a.OrgScope).ToList();
                        }
                        break;
                }
                int total = orgAcl == null ? 0 : orgAcl.Count;
                if (null != orgAcl)
                    orgAcl = orgAcl.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                List<SystemOrgAclViewModel> list = new List<SystemOrgAclViewModel>();
                //获取用户ID
                List<string> userIds = orgAcl.Select(s => s.UserID).ToList();
                List<string> orgIds = orgAcl.Select(s => s.OrgScope).ToList();
                Dictionary<string, string> unitNames = this.Engine.Organization.GetNames(userIds.ToArray());
                Dictionary<string, string> orgScopeNames = this.Engine.Organization.GetFullNames(orgIds.ToArray());
                //构造组织权限显示模型
                foreach (Acl.SystemOrgAcl acl in orgAcl)
                {
                    list.Add(new SystemOrgAclViewModel()
                    {
                        ObjectID = acl.ObjectID,
                        Administrator = acl.Administrator,
                        OrgScope = orgScopeNames[acl.OrgScope],
                        UserName = unitNames[acl.UserID],
                        ViewOrgData = acl.ViewOrgData
                    });
                }
               
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 删除组织权限
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult DelSystemOrgAcl(string ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ids = ids.TrimEnd(',');
                string[] idArr = ids.Split(',');
                try
                {
                    foreach (string id in idArr)
                    {
                        this.Engine.SystemOrgAclManager.Delete(id);
                    }
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存组织权限
        /// </summary>
        /// <param name="model">组织权限模型</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult SaveSystemOrgAcl(SystemOrgAclViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                // 获得选中的用户

                string[] users = model.UserIDArr.Where(s => s != "").ToArray();
                string[] scopes = model.OrgScopeArr.Where(s => s != "").ToArray();
                if (users == null || users.Length == 0 || scopes == null || scopes.Length == 0)
                {
                    result.Success = false;
                    result.Message = "SystemOrgAcl.NoSelectedUserUnit";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 验证是否具有管理员权限
                foreach (string scope in scopes)
                {
                    if (!this.UserValidator.ValidateOrgAdmin(scope))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.LackOfAuth";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                // 添加权限
                string errorUsers = null;
                foreach (string user in users)
                {
                    foreach (string orgScope in scopes)
                    {
                        OThinker.H3.Acl.SystemOrgAcl acl = new OThinker.H3.Acl.SystemOrgAcl();
                        acl.UserID = user;
                        acl.OrgScope = orgScope;
                        acl.ViewOrgData = model.ViewOrgData;
                        acl.Administrator = model.Administrator;
                        acl.CreatedBy = this.UserValidator.UserID;
                        acl.CreatedTime = System.DateTime.Now;

                        if (!this.Engine.SystemOrgAclManager.Add(acl))
                        {
                            // 添加不成功
                            errorUsers += (this.Engine.Organization.GetName(user) + "->" + this.Engine.Organization.GetName(orgScope) + "；");
                        }
                    }
                }
                if (errorUsers != null)
                {
                    result.Success = false;
                    result.Message = "SystemOrgAcl.AddFailed";
                    result.Extend = errorUsers;
                }
                else
                {
                    // 添加成功
                    result.Success = true;
                }


                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 更改状态的值
        /// </summary>
        /// <param name="id">组织权限ID</param>
        /// <param name="isDefault">状态</param>
        /// <param name="colunmName">列名</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult SetDefault(string id, bool isDefault, string columnName)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                SystemOrgAcl acl = this.Engine.SystemOrgAclManager.GetAcl(id);
                if (acl == null)
                {
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                switch (columnName)
                {
                    case "Administrator":
                        acl.Administrator = isDefault;
                        break;
                    case "ViewOrgData":
                        acl.ViewOrgData = isDefault;
                        break;

                };
                this.Engine.SystemOrgAclManager.Update(new SystemOrgAcl[] { acl });
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
