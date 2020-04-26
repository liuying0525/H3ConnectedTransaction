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
    public class SSOManagerController : ControllerBase
    {
        /// <summary>
        /// 获取权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                // TODO:未设置权限编码
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取单点登录信息列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>带分页信息的单点登录数据</returns>
        [HttpPost]
        public JsonResult GetSSOManagerList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                List<SSOManagerViewModel> list = new List<SSOManagerViewModel>();
                List<SSOSystem> ssms = this.Engine.SSOManager.GetSSOSystemList().Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                int total = ssms.Count;
                if (ssms != null)
                {
                    list = ssms.Select(s => new SSOManagerViewModel()
                    {
                        ObjectID = s.ObjectID,
                        SystemName = s.SystemName,
                        SystemCode = s.SystemCode,
                        SubmitUserNameControlID = s.SubmitUserNameControlID,
                        SubmitUrl = s.DefaultUrl,
                        DefaultUrl = s.DefaultUrl,
                        SubmitPasswordID = s.SubmitPasswordID,
                        State = s.State,
                        Secret = s.Secret,
                        Description = s.Description,
                        AllowGetToken = s.AllowGetToken
                    }).ToList();
                }
                var griddate = new { Rows = list, Total = total };
                return Json(griddate, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取单点登录信息
        /// </summary>
        /// <param name="code">系统编码</param>
        /// <returns>单点登录显示模型</returns>
        [HttpGet]
        public JsonResult GetSSOManager(string code)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                SSOSystem ssm = this.Engine.SSOManager.GetSSOSystem(code);
                if (ssm == null)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.NullObjectException";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                SSOManagerViewModel model = new SSOManagerViewModel()
                {
                    ObjectID = ssm.ObjectID,
                    SystemName = ssm.SystemName,
                    SystemCode = ssm.SystemCode,
                    SubmitUserNameControlID = ssm.SubmitUserNameControlID,
                    SubmitUrl = ssm.DefaultUrl,
                    DefaultUrl = ssm.DefaultUrl,
                    SubmitPasswordID = ssm.SubmitPasswordID,
                    State = ssm.State,
                    Secret = ssm.Secret,
                    Description = ssm.Description,
                    AllowGetToken = ssm.AllowGetToken
                };
                result.Success = true;
                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除单点登录信息
        /// </summary>
        /// <param name="ids">单点登录ID</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult DelSSOManager(string ids)
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
                        if (!this.Engine.SSOManager.RemoveSSOSystem(id)) result.Success = false;
                        else result.Success = true;
                    }
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
        /// 保存单点登录信息
        /// </summary>
        /// <param name="model">单点登录显示模型</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult SaveSSOManager(SSOManagerViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                SSOSystem ssm = this.Engine.SSOManager.GetSSOSystem(model.SystemCode);
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    //单点登录信息
                    if (ssm != null && !string.IsNullOrEmpty(ssm.ObjectID))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.CodeDuplicate";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    ssm = new SSOSystem()
                    {
                        AllowGetToken = model.AllowGetToken,
                        DefaultUrl = model.DefaultUrl,
                        Description = model.Description,
                        Secret = model.Secret,
                        State = model.State,
                        SubmitPasswordID = model.SubmitPasswordID,
                        SubmitUrl = model.SubmitUrl,
                        SubmitUserNameControlID = model.SubmitUserNameControlID,
                        SystemCode = model.SystemCode,
                        SystemName = model.SystemName
                    };
                    if (!this.Engine.SSOManager.AddSSOSystem(ssm))
                    {
                        result.Message = "msgGlobalString.SaveFailed";
                        result.Success = false;
                    }
                    else
                    {
                        result.Success = true;
                    }
                }
                else
                {
                    if (ssm == null)
                    {
                        result.Message = "msgGlobalString.SaveFailed";
                        result.Success = false;
                    }
                    else
                    {
                        ssm.AllowGetToken = model.AllowGetToken;
                        ssm.DefaultUrl = model.DefaultUrl;
                        ssm.Description = model.Description;
                        //ssm.Secret = model.Secret;
                        ssm.State = model.State;
                        ssm.SubmitPasswordID = model.SubmitPasswordID;
                        ssm.SubmitUrl = model.SubmitUrl;
                        ssm.SubmitUserNameControlID = model.SubmitUserNameControlID;
                        ssm.SystemCode = model.SystemCode;
                        ssm.SystemName = model.SystemName;
                        if (!this.Engine.SSOManager.UpdateSSOSystem(ssm))
                        {
                            result.Message = "msgGlobalString.SaveFailed";
                            result.Success = false;
                        }
                        else
                        {
                            result.Success = true;
                        }
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

    }
}
