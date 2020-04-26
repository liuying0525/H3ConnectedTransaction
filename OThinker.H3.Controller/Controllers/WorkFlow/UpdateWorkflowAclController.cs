using Newtonsoft.Json;
using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class UpdateWorkflowAclController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; ; }
        }

        public JsonResult Load(string WorkflowCode,string AclId)
        {
            return ExecuteFunctionRun(() => {

                OThinker.H3.Acl.WorkflowAcl CurrentAcl=null;
                if (!string.IsNullOrEmpty(AclId))
                {
                    CurrentAcl = this.Engine.WorkflowAclManager.GetWorkflowAcl(AclId);
                }
                
               var WorflowName = this.Engine.WorkflowManager.GetClauseDisplayName(WorkflowCode);
               var UserAlias = string.Empty;
               var CreateInstance = false;

                //按钮显示 ，前台判断
                if (CurrentAcl != null)
                {
                    // 编辑模式
                    OThinker.Organization.Unit u = this.Engine.Organization.GetUnit(CurrentAcl.UserID);
                    if (u is OThinker.Organization.User)
                    {
                        UserAlias = this.Engine.Organization.GetFullName(u.ParentID)+"/"+u.Name ;
                    }
                    else {
                        UserAlias = this.Engine.Organization.GetFullName(CurrentAcl.UserID);
                    }
                    

                    CreateInstance = CurrentAcl.CreateInstance;
                }

                //BizWorkflowPackageLockByID(WorkflowCode);
                var obj = new { WorkflowName = WorflowName,UserAlias = UserAlias, CreateInstance = CreateInstance };

                return Json(obj,JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult Save(string AclId, string WorkflowCode, string SelectUsers,bool CreateInstance)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false,"");
                OThinker.H3.Acl.WorkflowAcl CurrentAcl=null;
                if (!string.IsNullOrEmpty(AclId))
                {
                    CurrentAcl = this.Engine.WorkflowAclManager.GetWorkflowAcl(AclId);
                }

                if (CurrentAcl == null)
                {
                    // 添加模式
                    string[] users = (string[])JsonConvert.DeserializeObject(SelectUsers, typeof(string[]));
                    // 检查选中的用户
                    if (users == null || users.Length == 0)
                    {
                        result.Message = "WorkflowSetting.UpdateWorkflowAcl_NoSetUnit";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // 检查是否已经存在
                        foreach (string user in users)
                        {
                            // 查询权限表
                            System.Data.DataTable table = this.Engine.WorkflowAclManager.QueryDataTable(new string[] { user }, WorkflowCode);
                            if (table != null && table.Rows.Count != 0)
                            {
                                string userFullName = this.Engine.Organization.GetFullName(user);
                                result.Message = "WorkflowSetting.UpdateWorkflowAcl_Duplicated";
                                result.Extend = userFullName;
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    foreach (string user in users)
                    {
                        OThinker.H3.Acl.WorkflowAcl acl = new OThinker.H3.Acl.WorkflowAcl();
                        acl.UserID = user;
                        acl.WorkflowCode = WorkflowCode;

                        acl.CreateInstance = CreateInstance;

                        acl.CreatedBy = this.UserValidator.UserID;
                        acl.CreatedTime = System.DateTime.Now;

                        this.Engine.WorkflowAclManager.Add(acl);
                    }
                }
                else
                {
                    CurrentAcl.CreateInstance = CreateInstance;
                    CurrentAcl.ModifiedTime = System.DateTime.Now;
                    CurrentAcl.ModifiedBy = this.UserValidator.UserID;
                    this.Engine.WorkflowAclManager.Update(new OThinker.H3.Acl.WorkflowAcl[] { CurrentAcl });
                }

                result.Success = true;
                result.Message = "msgGlobalString.SaveSucced";
                return Json(result,JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult Delete(string AclId)
        {
            return ExecuteFunctionRun(() =>
            {
                this.Engine.WorkflowAclManager.Delete(AclId);
                return Json(new ActionResult(true,""));
            });
        }
    }
}
