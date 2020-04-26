using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    public class OrgGroupController:ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Data_Code; }
        }


        public OrgGroupController() { }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public JsonResult GetGroupInfo(string unitID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                OrgGroupViewModel model = new OrgGroupViewModel();
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
                OThinker.Organization.Group unit = this.Engine.Organization.GetUnit(unitID) as OThinker.Organization.Group;
                if (unit == null) { return Json(model, JsonRequestBehavior.AllowGet); }

                model.ObjectID = unit.ObjectID;
                model.GroupName = unit.Name;
              
                model.ParentUnit = unit.ParentID;
                model.SortKey = unit.SortKey;
                model.VisibleType = unit.Visibility;
              
                model.Description = unit.Description;

                if (unit.ChildList.Count() > 0) {
                    string members = "";
                    foreach (GroupChild gc in unit.ChildList)
                    {
                        members += gc.ChildID + ";";
                    }

                    model.Members = members;
                }

                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 保存/更新组织信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveGroupInfo(OrgGroupViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");

                //检查是否有编辑权限
                //如果是新增，检查父节点的编辑权限，更新：检查当前节点的权限
                if (!this.UserValidator.ValidateOrgEdit(string.IsNullOrEmpty(model.ObjectID)?model.ParentUnit:model.ObjectID))
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl";
                    return Json(result);
                }

                //新增组是，检测同一节点下，名称不能重复
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    List<string> brothers = this.Engine.Organization.GetChildren(model.ParentUnit, OThinker.Organization.UnitType.Group, false, OThinker.Organization.State.Unspecified);

                    Dictionary<string, string> dic = this.Engine.Organization.GetNames(brothers.ToArray());
                    if (dic.Where(d => d.Value == model.GroupName).Count() > 0)
                    {
                        result.Success = false;
                        result.Message = "EditOrgUnit.OrgNameExisted";
                        result.Extend = "";
                        return Json(result);
                    }
                }
                try
                {
                    //名称必须以字母开始，不让创建到数据库表字段时报错
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                    if (!regex.Match(model.GroupName).Success)
                    {
                        result.Success = false;
                        result.Message = "EditOrgUnit.NameInvalid";
                        return Json(result);
                    }

                    OThinker.Organization.Group unit = null;
                     //ADD
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        unit = new Group();
                        unit.ObjectID=Guid.NewGuid().ToString();
                    }
                    else
                    {
                        unit =this.Engine.Organization.GetUnit(model.ObjectID) as Group;
                    }

                    unit.Name = model.GroupName;
                    unit.ParentID = model.ParentUnit;
                    unit.SortKey = model.SortKey;
                    unit.Visibility = model.VisibleType;
                    unit.Description = model.Description;

                    //添加子成员
                    if (model.Members == null) 
                    { unit.Children = null; }
                    else 
                    { unit.Children = model.Members.Split(';'); }

                    // 检测循环引用
                    if (unit.Children != null)
                    {
                        foreach (string child in unit.Children)
                        {
                            if (child == unit.ObjectID)
                            {
                                result.Success = false;
                                result.Message = "EditGroup.EditGroup_Msg4";
                                return Json(result);
                               
                            }

                            List<OThinker.Organization.Unit> units = this.Engine.Organization.GetChildUnits(child, UnitType.Group, true, State.Active);
                            if (units != null)
                            {
                                foreach (OThinker.Organization.Unit u in units)
                                {
                                    if (u.ObjectID == unit.ObjectID)
                                    {
                                        result.Success = false;
                                        result.Message = "EditGroup.EditGroup_Msg5";
                                        return Json(result);
                                    }
                                }
                            }
                        }
                    }
                   
                    //ADD
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        this.Engine.Organization.AddUnit(this.UserValidator.UserID, unit);
                    }
                    //Update
                    else
                    {
                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, unit);
                    }

                    result.Extend = new { UnitID=unit.ObjectID };
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


        public JsonResult DeleteGroupInfo(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.DeleteSucced");

                if (!this.UserValidator.ValidateOrgEdit(id))
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl!";
                    return Json(result);
                }

                OThinker.Organization.Group Unit = this.Engine.Organization.GetUnit(id) as OThinker.Organization.Group;
                if (Unit == null) { return Json(result); }

                if ( Unit.ObjectID == this.Engine.Organization.RootUnit.ObjectID ||
                       Unit.UnitID == OThinker.Organization.User.AdministratorID)
                {
                    // 不允许删除公司
                    //this.ShowWarningMessage("不允许删除");
                    result.Success = false;
                    result.Message = "msgGlobalString.RootNoDeleted";
                    return Json(result);

                }
                else
                {
                    this.Engine.Organization.RemoveUnit(this.UserValidator.UserID, Unit);
                }

                return Json(result);
                //刷新组织树，关闭当前Tab页，前台处理
               
            });
        }
    }
}
