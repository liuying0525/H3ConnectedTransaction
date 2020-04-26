using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class WorkflowFolderAclController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }


        #region  流程目录权限的判断
        
        //判断是否有流程目录权限
        public bool BizWFFolderAclDoAction(string BizWFFolderCode)
        {
            bool result = this.UserValidator.ValidateFunctionRun(BizWFFolderCode);

            return result;
        }
        #endregion


        /// <summary>
        /// 判断是否有流程目录权限，返回False时，前台禁用按钮操作
        /// </summary>
        /// <param name="WorkflowFolderCode"></param>
        /// <returns></returns>
        public JsonResult GetFolderAcl(string WorkflowFolderCode)
        {
            return ExecuteFunctionRun(() => {

                return Json(BizWFFolderAclDoAction(WorkflowFolderCode),JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取流程文件夹权限数据
        /// </summary>
        /// <param name="WorkflowFolderCode"></param>
        /// <returns></returns>
        public JsonResult GetData(string WorkflowFolderCode)
        {
            return ExecuteFunctionRun(() => {

                Acl.FunctionAcl[] acls = this.Engine.FunctionAclManager.GetFunctionAclByCode(WorkflowFolderCode);
                if (acls != null)
                {
                    for (int i = 0, j = acls.Length; i < j; i++)
                    {
                        var unitId = acls[i].UserID;
                        acls[i].UserID = this.Engine.Organization.GetFullName(acls[i].UserID);

                        OThinker.Organization.Unit unit = this.Engine.Organization.GetUnit(unitId);
                        if (unit.UnitType != OThinker.Organization.UnitType.OrganizationUnit)
                        {
                            acls[i].UserID += "/" + unit.Name;
                        }
                    }
                }

                var GridData = CreateLigerUIGridData(acls);

                return Json(GridData, JsonRequestBehavior.AllowGet);
               
            });
        }

        /// <summary>
        /// 删除流程文件夹权限
        /// </summary>
        /// <param name="delIds"></param>
        /// <returns></returns>
        public JsonResult Delete(string delIds)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.DeleteSucced");

                delIds = delIds.Substring(0, delIds.Length - 1);
                string[] ids = delIds.Split(';');
                for (int i = 0; i < ids.Length; i++)
                {
                    string id = ids[i];
                    if (!string.IsNullOrEmpty(id))
                    {
                        this.Engine.FunctionAclManager.Delete(id);
                    }
                }

                return Json(result);

            });
        }

        /// <summary>
        /// 添加、保存流程文件夹权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveWorkflowFolderAcl(WorkflowFolderAclViewModel model)
        {
             return ExecuteFunctionRun(() => {

                 ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");

                 SaveWorkflowFloderAclBase(model);

                 ///this.CloseCurrentDialog();
                 return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
       
        /// <summary>
        /// 加载单条流程文件夹权限
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadWorkflowFolderAcl(string WorkflowFolderCode,string AclID)
        {
            return ExecuteFunctionRun(() => {
               
                WorkflowFolderAclViewModel model = new WorkflowFolderAclViewModel();
                model.WorkflowFolderCode = WorkflowFolderCode;

                if(!string.IsNullOrEmpty(AclID))
                {
                    Acl.FunctionAcl FunctionAcl = this.Engine.FunctionAclManager.GetAcl(AclID);

                    model.ObjectID = FunctionAcl.AclID;
                    model.UserID = FunctionAcl.UserID;
                    OThinker.Organization.Unit unit= this.Engine.Organization.GetUnit(model.UserID);
                    if (unit != null) {
                        model.UserName = unit.Name;
                    }
                   
                    model.WorkflowFolderCode = FunctionAcl.FunctionCode;
                    model.Run = FunctionAcl.Run;
                }
                FunctionNode FunctionNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(WorkflowFolderCode);

                model.WorkflowFolderName = FunctionNode.DisplayName;
                model.FunctionRun = this.UserValidator.ValidateFunctionRun(WorkflowFolderCode);

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }
    }

}
