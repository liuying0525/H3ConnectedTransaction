using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Acl;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 个人信息类，包括任务委托，常用意见
    /// </summary>
    [Authorize]
    public class PersonalInfoController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalInfoController()
        {
        }

        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return string.Empty;
            }
        }
        #region 用户信息
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public JsonResult GetUserInfo(string UserID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Organization.User user = (Organization.User)Engine.Organization.GetUnit(UserID);
                if (user != null)
                {
                    user.ImageUrl = GetUserImageUrl(user);
                }
                var result = new
                {
                    Success = user != null,
                    User = user,
                    PortalRoot = this.PortalRoot,
                    ManagerName = GetUserManagerName(user),
                    OUDepartName = GetUserParentName(user),
                    chkEmail = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.Email),
                    chkMobileMessage = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.MobileMessage),
                    chkWeChat = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.WeChat),
                    chkApp = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.App),
                    chkDingTalk = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.DingTalk),
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        ///更新用户信息
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public JsonResult UpdateUserInfo(string UserID, string Mobile, string OfficePhone, string Email, string FacsimileTelephoneNumber, bool chkEmail, bool chkApp, bool chkWeChat, bool chkMobileMessage, bool chkDingTalk)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);

                #region add by chenghuashan 2018-02-24
                if (!string.IsNullOrEmpty(Mobile))
                {
                    string sql = "SELECT Code,Name FROM OT_User WHERE Mobile='" + Mobile + "' and ObjectID <>'" + UserID + "'";
                    System.Data.DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        string msg = "";
                        foreach (System.Data.DataRow row in dt.Rows)
                        {
                            msg += "\n" + row["Name"] + "（" + row["Code"] + "）";
                        }
                        result.Success = false;
                        result.Message = "手机号码不允许重复，以下用户使用此手机号" + msg;
                        return Json(result, "text/html");
                    }
                }
                #endregion

                //UserID修改人的ID
                Organization.User EditUnit = (OThinker.Organization.User)Engine.Organization.GetUnit(UserID);
                string dirPath = AppDomain.CurrentDomain.BaseDirectory + "TempImages//face//" + Engine.EngineConfig.Code + "//" + this.UserValidator.User.ParentID + "//";
                string ID = this.UserValidator.UserID;

                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;

                if (files.Count > 0)
                {
                    byte[] content = GetBytesFromStream(files[0].InputStream);
                    if (content.Length > 0)
                    {
                        string Type = files[0].ContentType;
                        string fileExt = ".jpg";
                        string savepath = dirPath + ID + fileExt;
                        try
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(savepath)))
                                Directory.CreateDirectory(Path.GetDirectoryName(savepath));
                            using (FileStream fs = new FileStream(savepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    bw.Write(content);
                                    bw.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // 这里如果直接输出异常，那么用户不能登录
                            Engine.LogWriter.Write("加载用户图片出现异常,UserValidator:" + ex.ToString());
                        }

                        if (UserID == this.UserValidator.UserID)
                        {
                            this.UserValidator.ImagePath = ID + fileExt;
                        }
                        FileInfo file = GetFile(dirPath, ID + fileExt);
                        if (file != null)
                        {
                            AttachmentHeader header = this.Engine.BizObjectManager.GetAttachmentHeader(OThinker.Organization.User.TableName, this.UserValidator.UserID, this.UserValidator.UserID);
                            if (header != null)
                            {
                                Engine.BizObjectManager.UpdateAttachment(header.BizObjectSchemaCode,
                                        header.BizObjectId,
                                        header.AttachmentID, this.UserValidator.UserID, file.Name, Type, content, header.FileFlag);
                            }
                            else
                            {
                                // 用户头像采用附件上传方式
                                Attachment attach = new Attachment()
                                {
                                    ObjectID = this.UserValidator.UserID,
                                    BizObjectId = this.UserValidator.UserID,
                                    BizObjectSchemaCode = OThinker.Organization.User.TableName,
                                    FileName = file.Name,
                                    Content = content,
                                    ContentType = Type,
                                    ContentLength = (int)file.Length
                                };
                                Engine.BizObjectManager.AddAttachment(attach);
                            }
                            // 用户头像
                            EditUnit.ImageID = UserID;
                            EditUnit.ImageUrl = "TempImages//face//" + Engine.EngineConfig.Code + "//" + this.UserValidator.User.ParentID + "//" + file.Name;
                            result.Extend = new
                            {
                                ImageUrl = EditUnit.ImageUrl
                            };
                        }
                    }
                }

                EditUnit.Mobile = Mobile;
                EditUnit.OfficePhone = OfficePhone;
                EditUnit.Email = Email;
                EditUnit.FacsimileTelephoneNumber = FacsimileTelephoneNumber;
                EditUnit.ModifiedTime = DateTime.Now;

                //接收消息
                EditUnit.SetNotifyType(Organization.NotifyType.App, chkApp);
                EditUnit.SetNotifyType(Organization.NotifyType.Email, chkEmail);
                EditUnit.SetNotifyType(Organization.NotifyType.MobileMessage, chkMobileMessage);
                EditUnit.SetNotifyType(Organization.NotifyType.WeChat, chkWeChat);
                EditUnit.SetNotifyType(Organization.NotifyType.DingTalk, chkDingTalk);

                // 写入服务器
                Organization.HandleResult HandleResult = Engine.Organization.UpdateUnit(this.UserValidator.UserID, EditUnit);
                if (HandleResult == Organization.HandleResult.SUCCESS && EditUnit.ObjectID == this.UserValidator.UserID)
                {
                    this.UserValidator.User = EditUnit as OThinker.Organization.User;
                }
                else
                {
                    result.Success = false;
                    result.Extend = null;
                }
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="OldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        public JsonResult SetPassword(string OldPassword, string NewPassword)
        {
            return this.ExecuteFunctionRun(() =>
            {
                var result = false;
                //验证
                OldPassword = OldPassword.Trim();
                bool success = UserValidatorFactory.Login(Clusterware.AuthenticationType.Forms, null, this.UserValidator.UserCode, OldPassword, Site.PortalType.Portal);
                if (success)
                {
                    this.UserValidator.User.Password = NewPassword;
                    Engine.Organization.UpdateUnit(this.UserValidator.User.Code, this.UserValidator.User);
                    result = true;
                    Session[Sessions.GetUserValidator()] = this.UserValidator;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }
        #endregion

        #region 常用意见
        /// <summary>
        /// 获取一个常用意见
        /// </summary>
        /// <param name="Comment"></param>
        /// <returns></returns>
        public JsonResult GetFrequentlyUsedComment(string CommentID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Organization.FrequentlyUsedComment Comment = Engine.Organization.GetFrequentlyUsedComment(CommentID);
                List<CommentViewModel> griddata = new List<CommentViewModel>();
                griddata.Add(new CommentViewModel()
                {
                    CommentID = Comment.CommentID + string.Empty,
                    CommentIndex = Comment.SortKey + string.Empty,
                    CommentText = Comment.CommentText + string.Empty
                });
                GridViewModel<CommentViewModel> result = new GridViewModel<CommentViewModel>(1, griddata);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }
        /// <summary>
        /// 获取用户常用意见
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFrequentlyUsedCommentsByUser()
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<Organization.FrequentlyUsedComment> Comments = Engine.Organization.GetFrequentlyUsedCommentsByUser(this.UserValidator.UserID);
                int total = Comments.Count;
                List<CommentViewModel> griddata = new List<CommentViewModel>();
                foreach (Organization.FrequentlyUsedComment comment in Comments)
                {
                    griddata.Add(new CommentViewModel()
                    {
                        CommentID = comment.CommentID + string.Empty,
                        CommentIndex = comment.SortKey + string.Empty,
                        CommentText = comment.CommentText + string.Empty
                    });
                }
                GridViewModel<CommentViewModel> result = new GridViewModel<CommentViewModel>(total, griddata);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 添加、编辑常用意见
        /// </summary>
        /// <param name="SortKeyText">排序号</param>
        /// <param name="CommentText">意见</param>

        public JsonResult AddFrequentlyUsedComment(string CommentID, string SortKeyText, string CommentText)
        {
            return this.ExecuteFunctionRun(() =>
            {
                int SortKey = 0;
                CommentText = CommentText.Trim();
                if (CommentText.Length > 120) CommentText = CommentText.Substring(0, 120);
                int.TryParse(SortKeyText, out SortKey);

                ActionResult result = new ActionResult(true);
                if (CommentID == "")
                {
                    //添加
                    Organization.FrequentlyUsedComment Comment = new Organization.FrequentlyUsedComment()
                    {
                        CommentText = CommentText,
                        SortKey = SortKey,
                        UserID = this.UserValidator.UserID,
                        CreatedTime = DateTime.Now
                    };
                    result.Success = Engine.Organization.AddFrequentlyUsedComment(Comment);
                    if (result.Success == false)
                    {
                        result.Message = "FrequentlyUsedComment_AddFailed";
                    }
                }
                else
                {
                    //编辑
                    Organization.FrequentlyUsedComment Comment = Engine.Organization.GetFrequentlyUsedComment(CommentID);
                    if (Comment != null)
                    {
                        Comment.SortKey = SortKey;
                        Comment.CommentText = CommentText;
                    }
                    result.Success = Engine.Organization.UpdateFrequentlyUsedComment(Comment);
                    if (result.Success == false)
                    {
                        result.Message = "FrequentlyUsedComment_AddFailed";
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 删除意见
        /// </summary>
        /// <param name="CommentID">ObjectID</param>
        public JsonResult RemoveFrequentlyUsedComment(string[] CommentsID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                var result = false;
                foreach (string commentID in CommentsID)
                {
                    result = Engine.Organization.RemoveFrequentlyUsedComment(commentID);
                }
                //var result = Engine.Organization.RemoveFrequentlyUsedComment(commentID);//(this.UserValidator.UserID, CommentID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }
        #endregion

        private FileInfo GetFile(string filePath, string fileName)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            System.IO.DirectoryInfo dir = new DirectoryInfo(filePath);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.FullName.Contains(fileName))
                    return file;
            }
            return null;
        }
    }
}
