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
    /// <summary>
    /// 数据模型权限控制器
    /// </summary>
    [Authorize]
    public class BizObjectAclController : ControllerBase
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        /// <summary>
        /// 解析参数是否正确
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>是否成功</returns>
        private bool ParseParam(string schemaCode)
        {
            string code = schemaCode;
            var schema = this.Engine.BizObjectManager.GetDraftSchema(code);

            if (schema == null)
            {
                //业务对象模式不存在，或者已经被删除
                return false;
            }
            return true;
        }

        #region 权限控制
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>权限列表数据</returns>
        [HttpPost]
        public JsonResult GetBizObjectAclList(PagerInfo pagerInfo, string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                // 获取所有权限
                Acl.BizObjectAcl[] bizObjectAcl = this.Engine.BizObjectManager.GetBizObjectAcls(schemaCode, null);
                List<Acl.BizObjectAcl> acls = new List<Acl.BizObjectAcl>();
                if (bizObjectAcl != null)
                    acls = bizObjectAcl.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                int total = bizObjectAcl == null ? 0 : bizObjectAcl.Length;
                List<BizObjectAclViewModel> list = new List<BizObjectAclViewModel>();
                //获取用户ID
                List<string> userIds = acls.Where(s => s.UserID != null).Select(s => s.UserID).ToList();
                List<string> orgIds = acls.Where(s => s.OrgScope != null).Select(s => s.OrgScope).ToList();
                Dictionary<string, string> unitNames = this.Engine.Organization.GetNames(userIds.ToArray());
                Dictionary<string, string> orgScopeNames = this.Engine.Organization.GetFullNames(orgIds.ToArray());
                string rootName = this.Engine.Organization.GetName(OThinker.Organization.OrganizationUnit.DefaultRootID);
                //构造组织权限显示模型
                foreach (Acl.BizObjectAcl acl in acls)
                {
                    if (!string.IsNullOrEmpty(acl.UserID))
                        list.Add(new BizObjectAclViewModel()
                        {
                            ObjectID = acl.ObjectID,
                            IsAdmin = acl.Administrator,
                            OrgScope = acl.OrgScope == null ? rootName : orgScopeNames[acl.OrgScope],
                            UserID = string.IsNullOrEmpty(acl.UserID) ? "" : unitNames[acl.UserID],
                            IsView = acl.ViewData,
                            FolderId = acl.FolderId ?? "",
                            IsCreate = acl.CreateBizObject,
                            SchemaCode = acl.SchemaCode
                        });
                }
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取用户权限页面数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="aclID">权限ID</param>
        /// <returns>权限数据</returns>
        [HttpGet]
        public JsonResult GetBizObjectAclPageData(string schemaCode, string aclID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (!this.ParseParam(schemaCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                List<Item> orgScopes = InitOrgScopes();
                FunctionNode functionNode = this.UserValidator.GetFunctionNode(schemaCode, false);
                string schemaName = functionNode != null ? functionNode.DisplayName : string.Empty;
                if (string.IsNullOrEmpty(aclID))//新增
                {
                    result.Extend = new
                    {
                        OrgScopeTypes = orgScopes,
                        BizObjectAcl = new BizObjectAclViewModel()
                        {
                            SchemaCode = schemaCode,
                            OrgScopeType = "0",
                            SchemaName = schemaName,
                            IsView = true,
                            IsAdmin = true,
                            IsCreate = true
                        }
                    };
                    result.Success = true;
                }

                else//编辑
                {
                    BizObjectAclViewModel model = new BizObjectAclViewModel();
                    BizObjectAcl acl = this.Engine.BizObjectManager.GetBizObjectAcl(schemaCode, null, aclID);
                    if (null != acl)
                    {
                        model = new BizObjectAclViewModel()
                        {
                            ObjectID = acl.ObjectID,
                            IsAdmin = acl.Administrator,
                            OrgScope = acl.OrgScope,
                            OrgScopeType = ((int)acl.OrgScopeType).ToString(),
                            UserID = acl.UserID,
                            IsView = acl.ViewData,
                            FolderId = acl.FolderId,
                            IsCreate = acl.CreateBizObject,
                            SchemaCode = acl.SchemaCode,
                            SchemaName = schemaName
                        };
                    }
                    result.Extend = new
                    {
                        OrgScopeTypes = orgScopes,
                        BizObjectAcl = model
                    };
                    result.Success = true;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 保存权限设置
        /// </summary>
        /// <param name="model">数据模型权限模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveBizObjectAcl(BizObjectAclViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Success = true;
                BizObjectAcl acl = null;
                List<string> errors = new List<string>();//错误信息数组
                if (string.IsNullOrEmpty(model.UserID))
                {
                    result.Success = false;
                    result.Message = "BizObjectAcl.NameRequired";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.ObjectID))//新增
                {
                    string[] UserIDarray = model.UserID.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
                    if (model.OrgScopeType == ((int)OrgScopeType.Specific).ToString())
                    {
                        string[] scopes = model.OrgScopeArr;
                        if (scopes == null || scopes.Length == 0 || scopes.All(s => s == ""))
                        {
                            result.Success = false;
                            result.Message = "BizObjectAcl.Msg6";
                        }
                        else
                        {
                            for (int i = 0; i < UserIDarray.Length; i++)
                            {
                                foreach (string scope in scopes)
                                {
                                    acl = new BizObjectAcl();
                                    acl.SchemaCode = model.SchemaCode;
                                    acl.UserID = UserIDarray[i];
                                    acl.OrgScope = scope;
                                    acl.OrgScopeType = OrgScopeType.Specific;
                                    acl.CreateBizObject = model.IsCreate;
                                    acl.ViewData = model.IsView;
                                    acl.Administrator = model.IsAdmin;
                                    if (!this.Engine.BizObjectManager.AddBizObjectAcl(acl))
                                    {
                                        //result.Success = false;
                                        //result.Message = "BizObjectAcl.AddFailed";
                                        //errors.Add(this.Engine.Organization.GetFullName(scope));
                                    }
                                }
                            }
                            //result.Extend = errors;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < UserIDarray.Length; i++)
                        {
                            acl = new BizObjectAcl();
                            acl.SchemaCode = model.SchemaCode;
                            acl.UserID = UserIDarray[i];
                            acl.OrgScopeType = model.OrgScopeType == "1" ? OrgScopeType.Self : OrgScopeType.All;
                            acl.CreateBizObject = model.IsCreate;
                            acl.ViewData = model.IsView;
                            acl.Administrator = model.IsAdmin;

                            if (!this.Engine.BizObjectManager.AddBizObjectAcl(acl))
                            {
                                //result.Success = false;
                                //result.Message = "BizObjectAcl.AddFailed";
                                //errors.Add("BizObjectAcl.OrgScopes." + acl.OrgScopeType.ToString());
                                //result.Extend = errors;
                            }

                        }
                    }
                }
                else
                {
                    acl = this.Engine.BizObjectManager.GetBizObjectAcl(model.SchemaCode, null, model.ObjectID);
                    acl.CreateBizObject = model.IsCreate;
                    acl.ViewData = model.IsView;
                    acl.Administrator = model.IsAdmin;
                    if (!this.Engine.BizObjectManager.UpdateBizObjectAcl(acl))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.UpdateFailed";
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除组织权限
        /// </summary>
        /// <param name="ids">组织权限ID</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>删除结果</returns>
        [HttpPost]
        public JsonResult DeleteBizObjectAcl(string ids, string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (!this.ParseParam(schemaCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string ObjectIds = ids;
                string[] AclIds = ObjectIds.Split(',');
                if (AclIds == null || AclIds.Length == 0)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectItem";
                }
                else
                {
                    this.Engine.BizObjectManager.RemoveBizObjectAcls(schemaCode, "", AclIds);
                    result.Success = true;
                    result.Message = "msgGlobalString.DeleteSucced";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion


        /// <summary>
        /// 初始化组织选择范围
        /// </summary>
        /// <returns>组织范围</returns>
        private List<Item> InitOrgScopes()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item("BizObjectAcl.OrgScopes.Specific", "0"));
            list.Add(new Item("BizObjectAcl.OrgScopes.Self", "1"));
            list.Add(new Item("BizObjectAcl.OrgScopes.All", "2"));
            return list;
        }

    }
}
