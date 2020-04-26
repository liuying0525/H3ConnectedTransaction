using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class FunctionNodeLockerController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 执行锁定
        /// </summary>
        public JsonResult DoLock(string SchemaCode)
        {
            return ExecuteFunctionRun(() => {
                ActionResult Result = new ActionResult();
                if (this.Engine.FunctionAclManager.LockNodeByUserId(SchemaCode, this.UserValidator.UserID))
                {
                    Result.Success = true;
                    Result.Message = "FunctionNodeLocker.FunctionNodeLocker_Msg0";
                }
                else
                {
                    Result.Success = false;
                    Result.Message = "FunctionNodeLocker.FunctionNodeLocker_Msg1";
                }

                return Json(Result, JsonRequestBehavior.AllowGet);
            });
           
           
        }

        /// <summary>
        /// 执行解锁
        /// </summary>
        public JsonResult DoUnLock(string SchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult Result = new ActionResult();
                if (this.Engine.FunctionAclManager.UnlockNodeByUserId(SchemaCode, this.UserValidator.UserID))
                {
                    Result.Success = true;
                    Result.Message = "FunctionNodeLocker.FunctionNodeLocker_Msg2";
                }
                else
                {
                    Result.Success = false;
                    Result.Message = "FunctionNodeLocker.FunctionNodeLocker_Msg3";
                }
                return Json(Result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
