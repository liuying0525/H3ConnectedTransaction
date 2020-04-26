using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 系统管理员控制器
    /// </summary>
    [Authorize]
    public class SystemAclController : ControllerBase
    {
        /// <summary>
        /// 当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysParam_SystemAcl_Code;
            }
        }

        /// 获取系统管理员列表
        /// </summary>
        /// <param name="pagerInfo">系统管理员分页数据</param>
        /// <returns>返回系统管理员列表,不包含系统默认用户</returns>
        [HttpPost]
        public JsonResult GetSystemAclList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                // 获取所有系统管理员,不包含SystemUser
                List<User> admins = this.Engine.Organization.GetAllAdministrators();
                admins = admins.Where(s => s.IsSystemUser == false && s.ObjectID != OThinker.Organization.User.AdministratorID).ToList();
                int total = admins.Count;
                if (null != admins && admins.Count > 0)
                {
                    admins = admins.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                }
                List<string> userIds = new List<string>();
                List<SystemAclViewModel> list = new List<SystemAclViewModel>();
                // 获取管理员ID集合
                userIds = admins.Select(s => s.ObjectID).Union(admins.Select(s => s.ParentID)).ToList();
                Dictionary<string, string> unitNames = this.Engine.Organization.GetNames(userIds.ToArray());
                // 构造管理员显示模型.
                list = admins.Select(s => new SystemAclViewModel
                {
                    ObjectID = s.ObjectID,
                    UserName = unitNames[s.ObjectID] + string.Empty,
                    LoginName = s.Code,
                    Organization = unitNames[s.ParentID] + string.Empty
                }).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 设置为系统管理员
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>成功:true 失败:false</returns>
        [HttpPost]
        public JsonResult SaveSystemAcl(string[] ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (ids == null || ids.Length == 0)
                {
                    result.Message = "msgGlobalString.SelectUser";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var users = this.Engine.Organization.GetAllAdministrators();
                for (int i = 0; i < ids.Length; i++)
                {
                    if (users != null && users.Any(s => s.ObjectID == ids[i]))
                    {
                        continue;
                        //result.Message = "SystemAcl.UserExist";
                        //result.Success = false;
                        //return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    try
                    {
                        this.Engine.Organization.SetUserIsAdministrator(this.UserValidator.UserID, ids[i], true);
                        result.Success = true;
                    }
                    catch (Exception e)
                    {
                        result.Success = false;
                        result.Message = e.Message;
                    }

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除系统用户
        /// </summary>
        /// <param name="ids">系统用户id</param>
        /// <returns>失败:错误消息;成功:空对象</returns>
        [HttpPost]
        public JsonResult DelSystemAcl(string ids)
        {
            return ExecuteFunctionRun(() =>
            {

                ids = ids.TrimEnd(',');
                string[] idArr = ids.Split(',');
                ActionResult result = new ActionResult();
                try
                {
                    foreach (string id in idArr)
                    {
                        this.Engine.Organization.SetUserIsAdministrator(this.UserValidator.UserID, id, false);
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

    }
}
