using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Settings;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    public class OrgUserController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Data_Code; }
        }

        public OrgUserController() { }

        /// <summary>
        /// 获取组织信息
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public JsonResult GetUserInfo(string unitID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                OrgUserViewModel model = new OrgUserViewModel();
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
                OThinker.Organization.User unit = this.Engine.Organization.GetUnit(unitID) as OThinker.Organization.User;

                if (unit == null)
                {
                    result.Success = false;
                    result.Message = "EditUser.UserNotExist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                model.ObjectID = unit.ObjectID;
                model.LoginName = string.IsNullOrEmpty(unit.Code) ? "" : unit.Code;
                model.UserName = unit.Name;
                model.Manager = string.IsNullOrEmpty(unit.ManagerID) ? "" : unit.ManagerID;
                model.ParentID = string.IsNullOrEmpty(unit.ParentID) ? "" : unit.ParentID;
                model.DingTalkAccount = string.IsNullOrEmpty(unit.DingTalkAccount) ? "" : unit.DingTalkAccount;

                model.ServiceState = unit.ServiceState;
                model.State = unit.State;

                model.EmployeeNumber = string.IsNullOrEmpty(unit.EmployeeNumber) ? "" : unit.EmployeeNumber;
                model.EmployeeRank = unit.EmployeeRank;
                model.Appellation = string.IsNullOrEmpty(unit.Appellation) ? "" : unit.Appellation;
                model.Secretary = string.IsNullOrEmpty(unit.SecretaryID) ? "" : unit.SecretaryID;
                model.EntryDate = unit.EntryDate < new DateTime(1900, 1, 1) ? "1980-01-01" : unit.EntryDate.ToString("yyyy-MM-dd");
                model.DepartureDate = unit.DepartureDate < new DateTime(1900, 1, 1) ? "1980-01-01" : unit.DepartureDate.ToString("yyyy-MM-dd");
                model.Privacy = unit.PrivacyLevel;
                model.IDNumber = string.IsNullOrEmpty(unit.IDNumber) ? "" : unit.IDNumber;
                model.Gender = unit.Gender;
                model.Birthday = unit.Birthday < new DateTime(1900, 1, 1) ? "1980-01-01" : unit.Birthday.ToString("yyyy-MM-dd");
                model.Email = string.IsNullOrEmpty(unit.Email) ? "" : unit.Email;
                model.OfficePhone = string.IsNullOrEmpty(unit.OfficePhone) ? "" : unit.OfficePhone;
                model.Mobile = string.IsNullOrEmpty(unit.Mobile) ? "" : unit.Mobile;
                model.Fax = string.IsNullOrEmpty(unit.FacsimileTelephoneNumber) ? "" : unit.FacsimileTelephoneNumber;
                model.SID = string.IsNullOrEmpty(unit.SID) ? "" : unit.SID;
                model.SystemAdmin = unit.IsAdministrator;
                model.PortalAdmin = unit.IsConsoleUser;
                model.SystemUser = unit.IsSystemUser;
                model.VirtualUser = unit.IsVirtualUser;

                //如果虚拟用户取消勾选，清空关联用户
                if (model.VirtualUser)
                {
                    model.RelationUser = unit.RelationUserID;
                }
                else
                {
                    model.RelationUser = "";
                }


                model.EmailNotify = unit.CheckNotifyType(NotifyType.Email);
                model.AppNotify = unit.CheckNotifyType(NotifyType.App);
                model.WechatNotify = unit.CheckNotifyType(NotifyType.WeChat);
                model.MessageNotify = unit.CheckNotifyType(NotifyType.MobileMessage);
                model.DingTalkNotify = unit.CheckNotifyType(NotifyType.DingTalk);
                model.SortKey = unit.SortKey;

                model.UserFace = GetUserImageUrl(unit);

                //读取用户已经绑定的角色ID
                List<string> lstPosts = this.Engine.Organization.GetParents(unitID, UnitType.Post, true, State.Active);
                if (lstPosts != null && lstPosts.Count > 0)
                {
                    model.OrgPostIDS = string.Join(";", lstPosts.ToArray());
                }

                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 获取用户对应的所有角色
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public JsonResult GetUserRoleList(string userID)
        {
            return ExecuteFunctionRun(() =>
           {
               List<string> lstPosts = this.Engine.Organization.GetParents(userID, UnitType.Post, true, State.Active);
               List<RoleUserViewModel> lstRoleUsers = new List<RoleUserViewModel>();
               if (lstPosts != null && lstPosts.Count > 0)
               {
                   foreach (string postid in lstPosts)
                   {
                       OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(postid) as OThinker.Organization.OrgPost;
                       foreach (OrgStaff staff in post.ChildList)
                       {
                           if (staff.UserID == userID)
                           {

                               string userid = staff.UserID;
                               string _userCode = ((OThinker.Organization.User)this.Engine.Organization.GetUnit(userid)).Code;
                               string _userName = this.Engine.Organization.GetUnit(userid).Name;


                               //范围组织名称
                               string[] scopeids = staff.OUScope;
                               Dictionary<string, string> dicNames = this.Engine.Organization.GetNames(scopeids);
                               string scopeNames = "";
                               foreach (string key in dicNames.Keys) { scopeNames += dicNames[key] + ";"; }

                               lstRoleUsers.Add(new RoleUserViewModel
                               {
                                   ObjectID = staff.ObjectID,
                                   RoleID = post.ObjectID,
                                   RoleCode = post.Code,
                                   RoleName = post.Name,
                                   UserID = staff.UserID,
                                   UserCode = _userCode,
                                   UserName = _userName,
                                   Sortkey = staff.ParentIndex,
                                   ManagerScope = scopeNames,
                                   ManagerScopeIds = string.Join(";", staff.OUScope),
                                   Description = staff.Description

                               });
                           }
                       }
                   }

                   var gridData = CreateLigerUIGridData(lstRoleUsers.ToArray());
                   return Json(gridData, JsonRequestBehavior.AllowGet);
               }

               return Json(new { Rows = "", Total = 0 }, JsonRequestBehavior.AllowGet);
           });
        }

        /// <summary>
        /// 保存/更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveUserInfo(OrgUserViewModel model)
        {
            System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;

            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");

                //检查是否有编辑权限
                //如果是新增，检查父节点的编辑权限，更新：检查当前节点的权限
                if (!this.UserValidator.ValidateOrgEdit(string.IsNullOrEmpty(model.ObjectID) ? model.ParentID : model.ObjectID))
                {
                    result.Success = false;
                    result.Message = "Orgnization.NoAcl";
                    return Json(result, "text/html");
                }

                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    //检测编码是否重复
                    OThinker.Organization.User u = this.Engine.Organization.GetUserByCode(model.LoginName);
                    if (u != null)
                    {
                        result.Success = false;
                        result.Message = "EditUser.CodeExisted";
                        result.Extend = "";
                        return Json(result, "text/html");
                    }

                    //编码必须以字母开始，不让创建到数据库表字段时报错
                    //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z@/.\\u4e00-\\u9fa5_]*$");
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[\u4E00-\u9FA5a-zA-Z0-9_@\\\\.]*$");
                    if (!regex.Match(model.LoginName).Success)
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg4";
                        return Json(result, "text/html");
                    }
                }

                #region add by chenghuashan 2018-01-22
                if (!string.IsNullOrEmpty(model.Mobile))
                {

                    string sql = "SELECT Code,Name FROM OT_User WHERE Mobile='" + model.Mobile + "' ";
                    if (!string.IsNullOrEmpty(model.ObjectID))
                        sql += "and ObjectID <>'" + model.ObjectID + "'";

                    DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        string msg = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            msg += "<br>" + row["Name"] + "（" + row["Code"] + "）";
                        }
                        result.Success = false;
                        result.Message = "手机号码不允许重复，以下用户使用此手机号" + msg;
                        return Json(result, "text/html");
                    }
                }
                #endregion

                try
                {
                    OThinker.Organization.User unit = null;
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        unit = new OThinker.Organization.User();
                        unit.Password = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UserInitialPassword);
                    }
                    else
                    {
                        unit = this.Engine.Organization.GetUnit(model.ObjectID) as OThinker.Organization.User;
                    }

                    unit.Code = model.LoginName;
                    unit.Name = model.UserName;
                    unit.ManagerID = model.Manager;
                    unit.ParentID = model.ParentID;
                    unit.SortKey = model.SortKey;
                    unit.ServiceState = model.ServiceState;
                    unit.State = model.State;
                    unit.EmployeeNumber = string.IsNullOrEmpty(model.EmployeeNumber) ? "" : model.EmployeeNumber;
                    unit.EmployeeRank = model.EmployeeRank;
                    unit.Appellation = string.IsNullOrEmpty(model.Appellation) ? "" : model.Appellation;
                    unit.SecretaryID = model.Secretary;
                    unit.DingTalkAccount = model.DingTalkAccount;

                    if (model.EntryDate != null)
                    {
                        unit.EntryDate = DateTime.Parse(model.EntryDate);
                    }
                    if (model.DepartureDate != null)
                    {
                        unit.DepartureDate = DateTime.Parse(model.DepartureDate);
                    }
                    if (model.Birthday != null)
                    {
                        unit.Birthday = DateTime.Parse(model.Birthday);
                    }

                    unit.PrivacyLevel = model.Privacy;
                    unit.IDNumber = string.IsNullOrEmpty(model.IDNumber) ? "" : model.IDNumber;
                    unit.Gender = model.Gender;
                    unit.Email = string.IsNullOrEmpty(model.Email) ? "" : model.Email;
                    unit.OfficePhone = string.IsNullOrEmpty(model.OfficePhone) ? "" : model.OfficePhone;
                    unit.Mobile = string.IsNullOrEmpty(model.Mobile) ? "" : model.Mobile;
                    unit.FacsimileTelephoneNumber = string.IsNullOrEmpty(model.Fax) ? "" : model.Fax;
                    unit.SID = string.IsNullOrEmpty(model.SID) ? "" : model.SID;
                    unit.IsSystemUser = model.SystemUser;
                    //unit.IsAdministrator = model.SystemAdmin;
                    unit.IsConsoleUser = model.PortalAdmin;
                    unit.IsVirtualUser = model.VirtualUser;
                    if (model.VirtualUser)
                    {
                        unit.RelationUserID = model.RelationUser;
                    }
                    else
                    {
                        //取消关联用户的时候，需要清空对应的实际用户属性值
                        unit.RelationUserID = "";
                    }

                    // 检查文件大小
                    if (files.Count > 0 && files[0].ContentLength > 200000)
                    {
                        result.Success = false;
                        result.Message = "图片大小超过200K";
                        return Json(result, "text/html");
                    }

                    //ADD
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        unit.ObjectID = Guid.NewGuid().ToString();
                        this.Engine.Organization.AddUnit(this.UserValidator.UserID, unit);

                        //保存接收消息类型
                        SaveUserNotify(unit, model);
                        //保存图片
                        SaveUserFaceImage(files, unit);
                    }
                    //Update
                    else
                    {
                        unit.ObjectID = model.ObjectID;
                        unit.ModifiedTime = DateTime.Now;
                        //更新接收消息类型
                        SaveUserNotify(unit, model);

                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, unit);
                        //保存图片
                        SaveUserFaceImage(files, unit);
                    }

                    string imageUrl = GetUserImageUrl(unit);
                    result.Extend = new { UserID = unit.ObjectID };
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    result.Extend = "，" + ex.Message;
                }


                return Json(result, "text/html");
            });
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResult SetPassword(string userid, string password)
        {
            return ExecuteFunctionRun(() =>
           {
               OThinker.Organization.User user = this.Engine.Organization.GetUnit(userid) as OThinker.Organization.User;
               if (user != null)
               {
                   user.Password = password;
                   this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, user);
               }

               ActionResult result = new ActionResult(true, "EditUser.PasswordSuccess");
               return Json(result);
           });
        }


        #region 签章

        /// <summary>
        /// 获取用户签章列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JsonResult GetSignaureList(string userId)
        {
            return ExecuteFunctionRun(() =>
            {
                // 从数据库中加载
                List<object> objs = GetSignaureByUserID(userId);

                if (objs == null) { return null; }

                var griddata = CreateLigerUIGridData(objs.ToArray());

                return Json(griddata, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 修改是否默认签章
        /// </summary>
        /// <param name="signaureId"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public JsonResult ChangeSignaureDefault(string signaureId, bool isDefault)
        {
            return ExecuteFunctionRun(() =>
            {
                this.Engine.Organization.SetDefaultSignature(signaureId, isDefault);
                ActionResult result = new ActionResult();
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 添加签章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveSignaure(SignatureAdminViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                ActionResult result = new ActionResult(true, "");
                // 检查排序码
                int sortKey = model.SortKey;

                if (files.Count > 0)
                {
                    // 检查文件大小
                    if (files[0].ContentLength > 200000)
                    {
                        result.Success = false;
                        result.Message = "图片大小超过200K";
                        return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                    }

                    OThinker.Organization.Signature s = new OThinker.Organization.Signature();
                    s.Name = model.Name;
                    s.UnitID = model.UserID;
                    s.Description = model.Description;
                    s.ContentType = files[0].ContentType;
                    s.Content = GetBytesFromStream(files[0].InputStream);
                    s.SortKey = sortKey;
                    s.State = OThinker.Organization.State.Active;
                    s.IsDefault = false;

                    if (this.Engine.Organization.AddSignature(s))
                    {
                        result.Message = "EditSignature.AddSuccess";
                        if (model.IsDefault)
                        {
                            ChangeSignaureDefault(s.ObjectID, true);
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "EditSignature.AddFailed";
                    }
                }
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult AddSignature(string Name, string Description, string SortKeyText)
        {
            return this.ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                ActionResult result = new ActionResult(true);
                if (files.Count > 0)
                {
                    // 检查文件大小
                    if (files[0].ContentLength > 200000)
                    {
                        result.Success = false;
                        result.Message = "EditSignatureDetail_ImgOver";
                        return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    byte[] content;
                    content = GetBytesFromStream(files[0].InputStream);
                    int SortKey = 0;
                    int.TryParse(SortKeyText, out SortKey);
                    string Type = files[0].ContentType;
                    OThinker.Organization.Signature signature = new OThinker.Organization.Signature()
                    {
                        UnitID = this.UserValidator.UserID,
                        Name = Name,
                        Content = content,
                        Description = Description,
                        SortKey = SortKey,
                        ContentType = Type,
                        State = OThinker.Organization.State.Active
                    };
                    result.Success = Engine.Organization.AddSignature(signature);
                    if (result.Success == false)
                    {
                        result.Message = "EditSignatureDetail_AddFailed";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "EditSignatureDetail_NoFile";
                }
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 删除签章
        /// </summary>
        /// <param name="SignaureIds"></param>
        /// <returns></returns>
        public JsonResult RemoveSignaure(string SignaureIds)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] ids = SignaureIds.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in ids)
                {
                    this.Engine.Organization.RemoveSignature(id);
                }
                ActionResult result = new ActionResult();
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }


        private string RetrunImage(DataRow row)
        {
            string imgSrc = Guid.NewGuid() + "." + row[OThinker.Organization.Signature.PropertyName_ContentType].ToString().Replace("image/", "");
            byte[] streamByte = Convert.FromBase64String(row[OThinker.Organization.Signature.PropertyName_Content].ToString());
            if (streamByte.Length == 0) { return ""; }
            System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(streamByte));
            image.Save(Server.MapPath("~/TempImages/" + imgSrc));
            return "<img src='" + this.PortalRoot + "/TempImages/" + imgSrc + "' style='width:16px;height:16px' />";
        }

        private string RetrunImageObj(Signature row)
        {
            string imgSrc = Guid.NewGuid() + "." + row.ContentType.ToString().Replace("image/", "");
            byte[] streamByte = row.Content;
            if (streamByte.Length == 0) { return ""; }
            System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(streamByte));
            image.Save(Server.MapPath("~/TempImages/" + imgSrc));
            return this.PortalRoot + "/TempImages/" + imgSrc;
        }

        private List<object> GetSignaureByUserID(string userId)
        {
            OThinker.Organization.Signature[] signatures = this.UserValidator.Engine.Organization.GetSignaturesByUnit(userId);
            if (signatures == null) signatures = new OThinker.Organization.Signature[0] { };

            //排序
            List<Signature> lstSignatures = signatures.OrderBy(x => x.SortKey).ToList();
            var returnList = lstSignatures.Where(s => s.State == OThinker.Organization.State.Active).Select(
            s => new
            {
                ObjectID = s.ObjectID,
                Content = s.Content,
                ContentType = s.ContentType,
                Description = s.Description,
                IsDefault = s.IsDefault,
                Name = s.Name,
                SortKey = s.SortKey,
                State = s.State,
                Icon = RetrunImageObj(s),
                UnitID = s.UnitID
            });
            return returnList.ToList<object>();
        }

        #endregion


        #region 调度/离职


        /// <summary>
        /// 将用户从角色中移出
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roleids"></param>
        /// <returns></returns>
        public JsonResult RemoveUserFromRole(string userid, string roleids)
        {
            return ExecuteFunctionRun(() =>
            {

                if (!string.IsNullOrEmpty(roleids))
                {
                    string[] ids = roleids.Split(';');
                    foreach (string roleid in ids)
                    {
                        RemoveUserRole(userid, roleid);
                    }
                }

                ActionResult result = new ActionResult(true, "TransferUser.RemoveSucess");//TODO  信息提示
                return Json(result);
            });
        }

        /// <summary>
        /// 将角色中用户替换为指定用户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roleids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult ReplaceUserFromRole(string userid, string roleids, string replaceuserid)
        {
            return ExecuteFunctionRun(() =>
            {
                if (!string.IsNullOrEmpty(roleids) && !string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(replaceuserid))
                {
                    string[] ids = roleids.Split(';');
                    foreach (string roleid in ids)
                    {
                        if (roleid == "") { continue; }
                        OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(roleid) as OThinker.Organization.OrgPost;

                        foreach (OThinker.Organization.OrgStaff staff in post.ChildList)
                        {
                            if (staff.UserID == userid)
                            {   //替换用户
                                post.RemoveChild(staff.UserID);
                                staff.UserID = replaceuserid;
                                post.AddChildUnit(staff);
                            }
                        }

                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);

                    }
                }

                ActionResult result = new ActionResult(true, "TransferUser.RepalceSucess");//信息提示
                return Json(result);
            });
        }

        /// <summary>
        /// 将用户从组中移出
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="groupids"></param>
        /// <returns></returns>
        public JsonResult RemoveUserFromGroup(string userid, string groupids)
        {
            return ExecuteFunctionRun(() =>
            {

                if (!string.IsNullOrEmpty(groupids))
                {
                    string[] ids = groupids.Split(';');
                    foreach (string groupid in ids)
                    {
                        if (groupid == "") { continue; }
                        OThinker.Organization.Group group = this.Engine.Organization.GetUnit(groupid) as OThinker.Organization.Group;

                        if (group == null) { continue; }

                        group.RemoveChild(userid);
                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, group);
                    }
                }

                ActionResult result = new ActionResult(true, "TransferUser.RemoveSucess");//TODO  信息提示
                return Json(result);
            });
        }


        /// <summary>
        /// 将用户从组中替换
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="groupids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult ReplaceUserFromGroup(string userid, string groupids, string replaceuserid)
        {
            return ExecuteFunctionRun(() =>
            {

                if (!string.IsNullOrEmpty(groupids) && !string.IsNullOrEmpty(groupids) && !string.IsNullOrEmpty(replaceuserid))
                {
                    string[] ids = groupids.Split(';');
                    foreach (string groupid in ids)
                    {
                        if (groupid == "") { continue; }
                        OThinker.Organization.Group group = this.Engine.Organization.GetUnit(groupid) as OThinker.Organization.Group;

                        group.RemoveChild(userid);
                        group.AddChild(replaceuserid);
                        this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, group);
                    }
                }

                ActionResult result = new ActionResult(true, "TransferUser.RepalceSucess");//TODO  信息提示
                return Json(result);
            });
        }

        /// <summary>
        /// 交接选择的工作项
        /// </summary>
        /// <param name="worlitemids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult TransferUser(string worlitemids, string replaceuserid)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] workItems = worlitemids.Split(';');
                // 转移工作项
                int successCount = 0;
                if (workItems != null)
                {
                    foreach (string itemId in workItems)
                    {
                        long result1 = this.Engine.WorkItemManager.Transfer(itemId, replaceuserid);
                        if (result1 == ErrorCode.SUCCESS)
                        {
                            successCount++;
                        }
                    }
                }

                ActionResult result = new ActionResult(true, "TransferUser.TransferSucess");//TODO  信息提示
                return Json(result);
            });
        }
        /// <summary>
        /// 人工交接选择的工作项
        /// </summary>
        /// <param name="worlitemids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult TransferUserByRG(string worlitemids, string replaceuserid)
        {
            string[] workItems = worlitemids.Split(';');
            // 转移工作项
            int successCount = 0;
            if (workItems != null)
            {
                foreach (string itemId in workItems)
                {
                    long result1 = this.Engine.WorkItemManager.Transfer(itemId, replaceuserid);
                    if (result1 == ErrorCode.SUCCESS)
                    {
                        this.Engine.LogWriter.Write(string.Format("TransferUserTransferUserByRG itemId:{0},replaceuserid:{1},by:{2}", itemId, replaceuserid, this.UserValidator.UserName + "[" + this.UserValidator.UserCode + "]"));
                        successCount++;
                    }
                }
            }

            ActionResult result = new ActionResult(true, "TransferUser.TransferSucess");//TODO  信息提示
            return Json(result);
        }
        /// <summary>
        /// 交接符合条件的任务
        /// </summary>
        /// <param name="worlitemids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult TransferUserCondition(string replaceuserid, string workflowCode, string userid, WorkItem.WorkItemState state)
        {
            return ExecuteFunctionRun(() =>
            {
                DataTable dtWorkItems = this.Engine.Query.QueryWorkItem(userid, state, OThinker.Data.BoolMatchValue.Unspecified);
                if (dtWorkItems != null && dtWorkItems.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtWorkItems.Rows)
                    {
                        if (dr[WorkItem.WorkItem.PropertyName_WorkflowCode].ToString().Contains(workflowCode))
                        {
                            string workitemid = dr[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty;

                            //交接任务
                            long result1 = this.Engine.WorkItemManager.Transfer(workitemid, replaceuserid);
                        }
                    }
                }
                ActionResult result = new ActionResult(true, "TransferUser.TransferSucess");//TODO  信息提示
                return Json(result);
            });
        }

        /// <summary>
        /// 人工交接符合条件的任务
        /// </summary>
        /// <param name="worlitemids"></param>
        /// <param name="replaceuserid"></param>
        /// <returns></returns>
        public JsonResult TransferUserConditionByRG(string replaceuserid, string workflowCode, string userid, WorkItem.WorkItemState state)
        {
            DataTable dtWorkItems = this.Engine.Query.QueryWorkItem(userid, state, OThinker.Data.BoolMatchValue.Unspecified);
            if (dtWorkItems != null && dtWorkItems.Rows.Count > 0)
            {
                foreach (DataRow dr in dtWorkItems.Rows)
                {
                    if (dr[WorkItem.WorkItem.PropertyName_WorkflowCode].ToString().Contains(workflowCode))
                    {
                        string workitemid = dr[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty;

                        //交接任务
                        long result1 = this.Engine.WorkItemManager.Transfer(workitemid, replaceuserid);
                        this.Engine.LogWriter.Write(string.Format("TransferUser TransferUserConditionByRG workitemid:{0},replaceuserid:{1},by:{2}", workitemid, replaceuserid, this.UserValidator.UserName + "[" + this.UserValidator.UserCode + "]"));
                    }
                }
            }
            ActionResult result = new ActionResult(true, "TransferUser.TransferSucess");//TODO  信息提示
            return Json(result);
        }


        /// <summary>
        /// 获取用户群组
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JsonResult GetUserGroupList(string userid)
        {
            return ExecuteFunctionRun(() =>
            {

                List<string> lstGroups = this.Engine.Organization.GetParents(userid, UnitType.Group, true, State.Active);
                List<object> lstUserGroups = new List<object>();
                var total = 0;
                if (lstGroups != null && lstGroups.Count > 0)
                {
                    foreach (string id in lstGroups)
                    {
                        OThinker.Organization.Group group = this.Engine.Organization.GetUnit(id) as OThinker.Organization.Group;
                        lstUserGroups.Add(
                            new
                            {
                                ObjectID = group.ObjectID,
                                GroupName = group.Name,
                                GroupDept = this.Engine.Organization.GetUnit(group.ParentID).Name
                            }
                            );

                    }

                    total = lstUserGroups.Count;

                }
                var gridData = CreateLigerUIGridData(lstUserGroups.ToArray());
                return Json(gridData, JsonRequestBehavior.AllowGet);

            });
        }

        /// <summary>
        /// 获取工作任务 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="workflowcode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public JsonResult GetUserWorkItems(string userid, string workflowcodes, WorkItem.WorkItemState state)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] workflowcodeArray = null;
                if (!string.IsNullOrEmpty(workflowcodes))
                {
                    workflowcodeArray = workflowcodes.Split(',');
                }
                DataTable dtWorkItems = this.Engine.Query.QueryWorkItem(userid, state, OThinker.Data.BoolMatchValue.Unspecified);
                var workitems = dtWorkItems.AsEnumerable().Where(row =>(workflowcodeArray!=null? workflowcodeArray.Contains(row.Field<string>(WorkItem.WorkItem.PropertyName_WorkflowCode)):true)).Select(
                    s => new
                    {
                        ObjectID = s.Field<string>(WorkItem.WorkItem.PropertyName_ObjectID),
                        Task = s.Field<string>(WorkItem.WorkItem.PropertyName_DisplayName),
                        WorkFlowName = s.Field<string>(Instance.InstanceContext.PropertyName_InstanceName),
                        Originator = s.Field<string>(Instance.InstanceContext.PropertyName_OriginatorName),
                        OriginateUnit = GetUnitName(s.Field<string>(Instance.InstanceContext.PropertyName_OrgUnit)),
                        ReceiveTime = s.Field<DateTime>(WorkItem.WorkItem.PropertyName_ReceiveTime).ToString("yyyy-MM-dd")
                    });

                int total = 0;
                if (workitems != null) { total = workitems.Count(); }

                var griddata = CreateLigerUIGridData(workitems.ToArray());
                return Json(griddata);
            });
        }
        public JsonResult GetUserWorkItemsByRG(string userid, string workflowcodes, WorkItem.WorkItemState state)
        {
            string[] workflowcodeArray = null;
            if (!string.IsNullOrEmpty(workflowcodes))
            {
                workflowcodeArray = workflowcodes.Split(',');
            }
            //DataTable dtWorkItems = this.Engine.Query.QueryWorkItem(userid, state, OThinker.Data.BoolMatchValue.Unspecified);
            string sql = string.Format(@"
select wi.*,con.createdtime,con.instancename,con.originatorname from ot_workitem wi
left join ot_instancecontext con on wi.instanceid=con.objectid 
where con.bizobjectschemacode='{0}' and wi.participant='{1}'
order by con.createdtime", workflowcodes, userid);
            DataTable dtWorkItems = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var workitems = dtWorkItems.AsEnumerable().Select(
                s => new
                {
                    ObjectID = s.Field<string>(WorkItem.WorkItem.PropertyName_ObjectID),
                    Task = s.Field<string>(WorkItem.WorkItem.PropertyName_DisplayName),
                    WorkFlowName = s.Field<string>(Instance.InstanceContext.PropertyName_InstanceName),
                    Originator = s.Field<string>(Instance.InstanceContext.PropertyName_OriginatorName),
                    OriginateUnit = GetUnitName(s.Field<string>(Instance.InstanceContext.PropertyName_OrgUnit)),
                    ReceiveTime = s.Field<DateTime>(WorkItem.WorkItem.PropertyName_ReceiveTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedTime = s.Field<DateTime>(Instance.InstanceContext.PropertyName_CreatedTime).ToString("yyyy-MM-dd HH:mm:ss")
                });

            int total = 0;
            if (workitems != null) { total = workitems.Count(); }

            var griddata = CreateLigerUIGridData(workitems.ToArray());
            return Json(griddata);
        }
        #endregion

        private string GetUnitName(string unitId)
        {
            if (string.IsNullOrEmpty(unitId)) { return ""; }

            var unit = this.Engine.Organization.GetUnit(unitId);
            if (unit == null) { return unitId; }

            return unit.Name;
        }

        /// <summary>
        /// 保存用户角色关系
        /// 如果当前已经存在的角色不在选择的角色里面就删除
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="OrgPostIDS"></param>
        /// <returns></returns>
        public JsonResult SaveUserRoles(string UserID, string OrgPostIDS)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "msgGlobalString.SaveSucced");
                try
                {
                    SaveUserRole(UserID, OrgPostIDS);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.Failed";
                    result.Extend = ex.Message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存用户角色及通知接收方式
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="model"></param>
        private void SaveUserNotify(User unit, OrgUserViewModel model)
        {
            //添加接收消息类型
            unit.SetNotifyType(NotifyType.Email, model.EmailNotify);
            unit.SetNotifyType(NotifyType.App, model.AppNotify);
            unit.SetNotifyType(NotifyType.WeChat, model.WechatNotify);
            unit.SetNotifyType(NotifyType.MobileMessage, model.MessageNotify);
            unit.SetNotifyType(NotifyType.DingTalk, model.DingTalkNotify);
        }


        private void SaveUserRole(string UserID, string OrgPostIDS)
        {
            //当前拥有角色列表
            List<string> lstPosts = this.Engine.Organization.GetParents(UserID, UnitType.Post, true, State.Active);
            List<string> lstRemovePosts = new List<string>();

            //添加角色用户对应关系
            if (!string.IsNullOrEmpty(OrgPostIDS))
            {
                string[] postIDs = OrgPostIDS.Split(';');
                if (postIDs != null && postIDs.Length > 0)
                {
                    foreach (string postId in lstPosts)
                    {
                        if (!postIDs.Contains(postId))
                        {
                            //保存的时候不删除用户角色，因为搜索的结果不包含所有的用户角色
                            //lstRemovePosts.Add(postId);
                        }
                    }

                    foreach (string postId in postIDs)
                    {
                        if (lstPosts.Contains(postId)) { continue; }//已经存在的不需要添加
                        if (!string.IsNullOrEmpty(postId))
                        {
                            OrgPost post = this.Engine.Organization.GetUnit(postId) as OrgPost;
                            if (post != null)
                            {
                                //
                                if (post.ChildList.FirstOrDefault(p => p.UserID == UserID) == null)
                                {
                                    post.AddChildUnit(new OrgStaff()
                                    {
                                        ObjectID = Guid.NewGuid().ToString(),
                                        UserID = UserID,
                                        OUScope = new string[] { }
                                    });

                                    this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);
                                }
                            }
                        }
                    }
                }
                else
                {
                    lstRemovePosts = lstPosts;
                }

            }
            else
            {
                lstRemovePosts = lstPosts;
            }

            //如果当前已经存在的角色不在选择的角色里面就删除
            foreach (string postId in lstRemovePosts)
            {
                RemoveUserRole(UserID, postId);
            }
        }


        /// <summary>
        /// 删除用户角色关系
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="RoleID"></param>
        private void RemoveUserRole(string UserID, string RoleID)
        {
            OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(RoleID) as OThinker.Organization.OrgPost;
            if (post != null)
            {
                List<OThinker.Organization.OrgStaff> listStaff = new List<OThinker.Organization.OrgStaff>();
                listStaff.AddRange(post.ChildList);
                for (int i = listStaff.Count - 1; i >= 0; i--)
                {
                    if (listStaff[i].UserID == UserID)
                    {
                        // post.RemoveChild(listStaff[i].ObjectID);
                        listStaff.RemoveAt(i);
                        break;
                    }
                }

                post.ChildList = listStaff.ToArray();
                this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, post);
            }
        }

        private void SaveUserFaceImage(HttpFileCollectionBase files, User unit)
        {
            //保存用户头像图片
            if (files.Count > 0)
            {
                string filePath = files[0].FileName;
                if (string.IsNullOrEmpty(filePath)) { return; }

                string fileName = TrimHtml(Path.GetFileName(filePath)).ToLowerInvariant();

                //已存在用户上传图片，header=null
                AttachmentHeader header = this.Engine.BizObjectManager.GetAttachmentHeader(OThinker.Organization.User.TableName, unit.ObjectID, unit.ObjectID);
                if (header != null)
                {
                    try
                    {
                        this.Engine.BizObjectManager.UpdateAttachment(header.BizObjectSchemaCode,
                            header.BizObjectId,
                            header.AttachmentID, this.UserValidator.UserID, header.FileName, header.ContentType, GetBytesFromStream(files[0].InputStream), header.FileFlag);
                    }
                    catch { }

                }
                else
                {
                    // 用户头像采用附件上传方式
                    Attachment attach = new Attachment()
                    {
                        ObjectID = unit.ObjectID,
                        BizObjectId = unit.UnitID,
                        BizObjectSchemaCode = OThinker.Organization.User.TableName,
                        FileName = fileName,
                        Content = GetBytesFromStream(files[0].InputStream),
                        ContentLength = (int)files[0].ContentLength,
                        CreatedBy = this.UserValidator.UserID,
                        ModifiedBy = this.UserValidator.UserID
                    };
                    this.Engine.BizObjectManager.AddAttachment(attach);
                }
            }
        }


        public JsonResult CanDeleteUser(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "qqq");
                Dictionary<string, int> Extend = new Dictionary<string, int>();
                //获取被删除用户发起的流程、待办、待阅，
                //待办
                string[] conditions1 = Engine.PortalQuery.GetWorkItemConditions(id, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
                //待阅
                string[] conditions2 = Engine.PortalQuery.GetWorkItemConditions(id, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.CirculateItem.TableName);
                // 我的流程（所有）
                string[] conditions3 = this.Engine.PortalQuery.GetInstanceConditions(
                    new string[] { id },
                    null,
                    Instance.InstanceContext.UnspecifiedID,
                    OThinker.H3.Instance.InstanceState.Unspecified,
                    null,
                    WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    string.Empty,
                    OThinker.H3.Instance.PriorityType.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified);
                int total1 = Engine.PortalQuery.CountWorkItem(conditions1, WorkItem.WorkItem.TableName); // 待办总数
                int total2 = Engine.PortalQuery.CountWorkItem(conditions2, WorkItem.CirculateItem.TableName); // 待阅总数
                int total3 = Engine.PortalQuery.CountInstance(conditions3); // 我的流程总数

                Extend["UnfinishedWorkItemCount"] = total1;
                Extend["UnreadWorkItemCount"] = total2;
                Extend["MyWorkflowCount"] = total3;
                result.Extend = Extend;

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult DeleteUserInfo(string id)
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

                OThinker.Organization.User Unit = this.Engine.Organization.GetUnit(id) as OThinker.Organization.User;
                if (Unit == null) { return Json(result); }

                if (
                       Unit.ObjectID == this.Engine.Organization.RootUnit.ObjectID ||
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

                //前台刷新组织树，关闭TAB页

            });
        }

        /// <summary>
        /// 移动修改用户图片
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="MobileToken"></param>
        public JsonResult UploadUserImage(string UserID, string MobileToken)
        {
            ActionResult result = new ActionResult(true);
            OThinker.Organization.User User = this.Engine.Organization.GetUnit(UserID) as OThinker.Organization.User;
            if (User == null) return Json(result, JsonRequestBehavior.AllowGet);
            if (!User.ValidateMobileToken(MobileToken))
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            };
            if (this.Request.Files == null || this.Request.Files.Count == 0)
                return Json(result, JsonRequestBehavior.AllowGet);
            // 本地临时存储
            System.Web.HttpPostedFileBase postFile = this.Request.Files[0];

            // 以下是获取用户图片
            string fileName = Path.Combine("face//" + Engine.EngineConfig.Code + "//" + User.ParentID, User.ObjectID + ".jpg");
            string TempImages = Server.MapPath(this.PortalRoot + @"/TempImages/");
            string savePath = Path.Combine(TempImages, fileName);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                postFile.SaveAs(savePath);
            }
            catch (Exception ex)
            {
                // 这里如果直接输出异常，那么用户不能登录
                Engine.LogWriter.Write("加载用户图片出现异常,UserValidator:" + ex.ToString());
            }
            FileInfo file = new FileInfo(savePath);
            if (file != null)
            {
                FileStream stream = file.OpenRead();
                int len = (int)file.Length;
                byte[] bytes = new byte[file.Length];
                stream.Read(bytes, 0, len);
                stream.Close();

                AttachmentHeader header = AppUtility.Engine.BizObjectManager.GetAttachmentHeader(OThinker.Organization.User.TableName,
                    User.ObjectID,
                    User.ObjectID);
                if (header != null)
                {
                    AppUtility.Engine.BizObjectManager.UpdateAttachment(header.BizObjectSchemaCode,
                            header.BizObjectId,
                            header.AttachmentID, User.ObjectID,
                            fileName,
                            postFile.ContentType,
                            bytes,
                            header.FileFlag);
                }
                else
                {
                    // 用户头像采用附件上传方式
                    Attachment attach = new Attachment()
                    {
                        ObjectID = User.ObjectID,
                        BizObjectId = User.ObjectID,
                        BizObjectSchemaCode = OThinker.Organization.User.TableName,
                        FileName = User.Code + ".jpg",
                        Content = bytes,
                        ContentType = postFile.ContentType,
                        ContentLength = (int)file.Length
                    };
                    AppUtility.Engine.BizObjectManager.AddAttachment(attach);
                }
            }
            result.Message = Path.Combine("/face/" + Engine.EngineConfig.Code + "/" + User.ParentID, User.ObjectID + ".jpg");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
