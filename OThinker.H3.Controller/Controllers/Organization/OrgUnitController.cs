using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OThinker.H3.Controllers.ViewModels;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    public class OrgUnitController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Data_Code; }
        }

        public OrgUnitController() { }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public JsonResult GetOrgInfo(string unitID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                OrgUnitViewModel model = new OrgUnitViewModel();

                //判断是否有组织编辑、查看权限
                OrgAclViewModel AclModel = GetUnitAcl(unitID);
                if (!AclModel.Edit && !AclModel.View)
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                model.Edit = AclModel.Edit;
                model.View = AclModel.View;

                if (string.IsNullOrEmpty(unitID)) { return Json(model, JsonRequestBehavior.AllowGet); }
                OThinker.Organization.OrganizationUnit unit = this.Engine.Organization.GetUnit(unitID) as OThinker.Organization.OrganizationUnit;

                if (unit == null) { return Json(model, JsonRequestBehavior.AllowGet); }

                model.ObjectID = unit.ObjectID;
                model.UnitName = unit.Name;
                model.Manager = unit.ManagerID;
                model.ParentUnit = unit.ParentID;
                model.SortKey = unit.SortKey;
                model.VisibleType = ((int)unit.Visibility).ToString();
                model.WorkflowCode = unit.WorkflowCode;
                model.Calendar = unit.CalendarID;
                model.OrgCategory = unit.CategoryCode??"";
                model.FullPath = this.Engine.Organization.GetFullName(unit.ObjectID);
                model.Description = unit.Description;

                result.Extend = model;

                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 保存/更新组织信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveOrgInfo(OrgUnitViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");

                if (string.IsNullOrEmpty(model.UnitName.Trim()))
                {
                    result.Success = false;
                    result.Message = "BizService.Msg4";
                    return Json(result);
                }

                //检查是否有编辑权限
                //如果是新增，检查父节点的编辑权限，更新：检查当前节点的权限
                if (!this.UserValidator.ValidateOrgEdit(string.IsNullOrEmpty(model.ObjectID) ? model.ParentUnit : model.ObjectID))
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl";
                    return Json(result);
                }

                //检测同一节点下，名称不能重复
                List<string> brothers = this.Engine.Organization.GetChildren(model.ParentUnit, OThinker.Organization.UnitType.OrganizationUnit, false, OThinker.Organization.State.Unspecified);
                Dictionary<string, string> dic = this.Engine.Organization.GetNames(brothers.ToArray());
                if (dic.Where(d => d.Value == model.UnitName).Where(d => d.Key != model.ObjectID).Count() > 0)
                {
                    result.Success = false;
                    result.Message = "EditOrgUnit.OrgNameExisted";
                    result.Extend = "";
                    return Json(result);
                }

                //名称必须以字母开始，不让创建到数据库表字段时报错
                //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                //if (!regex.Match(model.UnitName).Success)
                //{
                //    result.Success = false;
                //    result.Message = "EditOrgUnit.NameInvalid";
                //    return Json(result);
                //}

                try
                {
                    OThinker.Organization.VisibleType vt =
                        (OThinker.Organization.VisibleType)Enum.Parse(typeof(OThinker.Organization.VisibleType), model.VisibleType);

                    OThinker.Organization.OrganizationUnit unit = new OThinker.Organization.OrganizationUnit();
                    if (!string.IsNullOrEmpty(model.ObjectID))
                    {
                        unit = (OThinker.Organization.OrganizationUnit)this.Engine.Organization.GetUnit(model.ObjectID);
                    }

                    unit.Name = model.UnitName;
                    unit.ManagerID = model.Manager;
                    unit.ParentID = model.ParentUnit;
                    unit.SortKey = model.SortKey;
                    unit.Visibility = vt;
                    unit.WorkflowCode = model.WorkflowCode;
                    unit.CalendarID = string.IsNullOrEmpty(model.Calendar) ? "" : model.Calendar;
                    unit.CategoryCode = model.OrgCategory;
                    unit.Description = model.Description;

                    //ADD
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        this.Engine.Organization.AddUnit(this.UserValidator.UserID, unit);
                    }
                    //Update
                    else
                    {
                        unit.ObjectID = model.ObjectID;
                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, unit);

                        //如果是根节点更新菜单FunctionNode
                        if (unit.IsRootUnit)
                        {
                            //FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNode(unit.ObjectID);
                            //node.DisplayName = unit.Name;
                            //this.Engine.FunctionAclManager.UpdateFunctionNode(node);
                        }
                    }
                    result.Extend = new { UnitID = unit.ObjectID };//返回前台更新TabID
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    result.Extend = "," + ex.Message;
                }

                return Json(result);
            });
        }


        /// <summary>
        /// 删除指定组织，递归删除，先删除组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteOrgInfo(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.DeleteSucced");
                // 验证删除权限
                if (!this.UserValidator.ValidateOrgEdit(id))
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl";
                    return Json(result);
                }

                OThinker.Organization.OrganizationUnit Unit = this.Engine.Organization.GetUnit(id) as OThinker.Organization.OrganizationUnit;
                if (Unit == null) { return Json(result); }

                if (
                       Unit.ObjectID == this.Engine.Organization.RootUnit.ObjectID ||
                       Unit.UnitID == OThinker.Organization.User.AdministratorID)
                {
                    // 不允许删除公司
                    result.Success = false;
                    result.Message = "msgGlobalString.RootNoDeleted";
                    return Json(result);
                }

                else
                {
                    // this.Engine.Organization.RemoveUnit(this.UserValidator.UserID, Unit);
                    DeleteOrg(Unit);
                }

                return Json(result);

            });
        }

        /// <summary>
        /// 组织机构微信同步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult WeChatSync()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                if (this.Engine.WeChatAdapter.SyncOrgToWeChat()) result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 组织机构钉钉同步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DDSync()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                if (this.Engine.DingTalkAdapter.SyncOrgToDingTalk()) result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public void DeleteOrg(OThinker.Organization.Unit unit)
        {
            if (unit is OThinker.Organization.OrganizationUnit)
            {
                List<OThinker.Organization.Unit> lstUnit = this.Engine.Organization.GetChildUnits(unit.ObjectID, OThinker.Organization.UnitType.OrganizationUnit, false, OThinker.Organization.State.Unspecified);
                foreach (OThinker.Organization.Unit u in lstUnit)
                {
                    DeleteOrg(u);
                }
            }

            this.Engine.Organization.RemoveUnit(this.UserValidator.UserID, unit);

        }

        /// <summary>
        /// 重新加载组织架构
        /// </summary>
        /// <returns></returns>
        public JsonResult ReloadOrg()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "Orgnization.ReloadOrgSuccess");
                this.Engine.Organization.Reload();
                return Json(result);
            });
        }

        /// <summary>
        /// 工作日历
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadWorkCalendar()
        {
            return ExecuteFunctionRun(() =>
            {
                List<object> lstObj = new List<object>();
                Calendar.WorkingCalendar[] calendars = this.Engine.WorkingCalendarManager.GetCalendarList();
                if (calendars != null)
                {
                    calendars = calendars.OrderByDescending(c => c.IsDefault).ToArray();
                    foreach (Calendar.WorkingCalendar calendar in calendars)
                    {
                        lstObj.Add(new { Text = calendar.DisplayName, Value = calendar.ObjectID });
                    }
                }

                return Json(lstObj, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 填充组织机构平铺表，填充前需要删除数据
        /// </summary>
        public JsonResult SetOrganizationDim()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "Orgnization.OrganizationSucceed");
                try
                {
                    DeleteOrganizationDim();
                    this.Engine.Organization.SetOrganizationDim();
                }
                catch
                {
                    result.Success = false;
                    result.Message = "Orgnization.OrganizationFailed";
                }
                return Json(result);
            });
        }

        private void DeleteOrganizationDim()
        {
            string sql = "delete from OT_OrganizationDim";
            this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }
    }
}
