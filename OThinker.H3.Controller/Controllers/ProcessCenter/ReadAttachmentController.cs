using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessCenter
{
    public class ReadAttachmentController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 获取可以直接使用浏览器打开的附件类型
        /// </summary>
        protected string Browse_Extension = ".pdf,.jpg,.gif,.jpeg,.png";

        #region 获取 URL 参数 ---------------------

        private string _DataField = string.Empty;
        /// <summary>
        /// 获取数据项ID
        /// </summary>
        protected string DataField
        {
            get
            {
                if (_DataField == string.Empty)
                {
                    _DataField = this.Request[Param_DataField] + string.Empty;
                }
                return _DataField;
            }
        }

        private string _DisplayName = string.Empty;
        /// <summary>
        /// 获取数据模型显示名称
        /// </summary>
        protected string DisplayName
        {
            get
            {
                if (_DisplayName == string.Empty)
                {
                    _DisplayName = this.Request["DisplayName"] + string.Empty;
                    _DisplayName = HttpUtility.UrlDecode(_DisplayName);
                }
                return _DisplayName;
            }
        }

        private string _AttachmentID = string.Empty;
        /// <summary>
        /// 获取附件ID
        /// </summary>
        protected string AttachmentID
        {
            get
            {
                if (_AttachmentID == string.Empty)
                {
                    _AttachmentID = this.Request[Param_AttachmentID] + string.Empty;
                }
                return _AttachmentID;
            }
        }

        private string _BizObjectSchemaCode = string.Empty;
        /// <summary>
        /// 获取数据模型编码
        /// </summary>
        protected string BizObjectSchemaCode
        {
            get
            {
                if (_BizObjectSchemaCode == string.Empty)
                {
                    _BizObjectSchemaCode = this.Request[Param_SchemaCode] + string.Empty;
                    _BizObjectSchemaCode = HttpUtility.UrlDecode(_BizObjectSchemaCode);
                }
                return _BizObjectSchemaCode;
            }
        }

        private string _BizObjectID = string.Empty;
        /// <summary>
        /// 获取数据模型编码
        /// </summary>
        protected string BizObjectID
        {
            get
            {
                if (_BizObjectID == string.Empty)
                {
                    _BizObjectID = this.Request[Param_BizObjectID] + string.Empty;
                    _BizObjectID = HttpUtility.UrlDecode(_BizObjectID);
                }
                return _BizObjectID;
            }
        }

        /// <summary>
        /// 获取是否移动办公访问
        /// </summary>
        protected bool IsMobile
        {
            get
            {
                return this.Request[SheetEnviroment.Param_IsMobile] + string.Empty != string.Empty;
            }
        }

        /// <summary>
        /// 是否是安卓设备
        /// </summary>
        protected bool IsAndroid
        {
            get
            {
                return Request.UserAgent.ToLower().IndexOf("android") >= 0;
            }
        }

        /// <summary>
        /// 是否是App访问
        /// </summary>
        protected bool AppLogin
        {
            get
            {
                return this.Request["AppLogin"] + string.Empty != string.Empty;
            }
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        protected string UserID
        {
            get
            {
                if (IsMobile)
                {
                    if (this.UserValidator != null) { return this.UserValidator.UserID; }
                    else
                    {
                        return this.Request["UserId"] + string.Empty;
                    }
                }
                else
                {
                    return this.UserValidator.UserID;
                }
            }
        }

        private int openMethod = -1;
        /// <summary>
        /// 获取附件的打开方式
        /// </summary>
        private int OpenMethod
        {
            get
            {
                if (openMethod == -1)
                {
                    string o = this.Request.QueryString["OpenMethod"] + string.Empty;
                    if (!int.TryParse(o, out openMethod))
                    {
                        openMethod = (int)OThinker.H3.Controllers.AttachmentOpenMethod.Download;
                    }
                }
                return openMethod;
            }
        }

        /// <summary>
        /// 是否是微信端
        /// </summary>
        protected bool IsWeChat
        {
            get
            {
                return Session[OThinker.H3.Controllers.Sessions.GetWeChatLogin()] != null;
            }
        }

        /// <summary>
        /// 是否是钉钉端
        /// </summary>
        protected bool IsDingTalk
        {
            get
            {
                return Session[OThinker.H3.Controllers.Sessions.GetDingTalkLogin()] != null;
            }
        }
        #endregion

        #region 页面自定义属性 --------------------
        private string portalUri = string.Empty;
        /// <summary>
        /// 获取 Portal 访问的URI路径
        /// </summary>
        public string PortalUri
        {
            get
            {
                if (portalUri == string.Empty)
                {
                    portalUri = ConfigurationManager.AppSettings["PortalRoot"] + string.Empty;
                }
                return portalUri;
            }
        }

        /// <summary>
        /// 获取当前附件的打开方式
        /// </summary>
        private AttachmentOpenMethod AttachmentOpenMethod
        {
            get
            {
                return (AttachmentOpenMethod)OpenMethod;
            }
        }

        #endregion

        /// <summary>
        /// 单个文件下载
        /// </summary>
        /// <returns></returns>
        public object Read()
        {
            OThinker.H3.Tracking.UserLog log = null;
            //安卓微信/钉钉中打开文件  弃用
            if (this.IsMobile && (IsWeChat || IsDingTalk) && IsAndroid && false)
            {
                // 记录打开日志
                log = new Tracking.UserLog(
                    Tracking.UserLogType.OpenAttachment,
                    this.UserValidator.UserID,
                    this.BizObjectSchemaCode,
                    this.BizObjectID,
                    this.BizObjectID,
                    string.Empty,
                    string.Empty,
                    null,
                    SheetUtility.GetClientIP(Request),
                    SheetUtility.GetClientPlatform(Request),
                    SheetUtility.GetClientBrowser(Request));
                this.Engine.UserLogWriter.Write(log);

                string attachmentUrl = this.GetAttachmentRedirectUrl();

                if (!string.IsNullOrEmpty(attachmentUrl))
                {
                    Response.Redirect(attachmentUrl);
                }
                return null;
            }

            Attachment attachment = this.Engine.BizObjectManager.GetAttachment(
                UserID,
                this.BizObjectSchemaCode,
                this.BizObjectID,
                this.AttachmentID);
            if (attachment == null || attachment.Content == null || attachment.Content.Length == 0)
            {
                //AlertAndClose(this.PortalResource.GetString("ReadAttachment_AttachmentIsNull"));
                //return;
            }

            // 记录打开日志
            log = new Tracking.UserLog(
                Tracking.UserLogType.OpenAttachment,
                UserID,
                attachment.BizObjectSchemaCode,
                attachment.BizObjectId,
                attachment.BizObjectId,
                string.Empty,
                attachment.FileName,
                null,
                SheetUtility.GetClientIP(Request),
                SheetUtility.GetClientPlatform(Request),
                SheetUtility.GetClientBrowser(Request));
            this.Engine.UserLogWriter.Write(log);

            //保存文件到服务器
            string fileName = attachment.ObjectID + Path.GetExtension(attachment.FileName);
            string savePath = Path.Combine(Server.MapPath("~/TempImages"), fileName);
            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(attachment.Content);
                    bw.Close();
                }
            }

            if (this.IsMobile)
            {
                // 移动办公app打开方式，则先下载到临时文件夹，再输出文件地址
                if (this.AppLogin)
                {
                    var url = string.Format(this.PortalRoot + "/TempImages/{0}", fileName);
                    var result = new
                    {
                        Url = string.Format(this.PortalRoot + "/TempImages/{0}", fileName),
                        Extension = Path.GetExtension(attachment.FileName).ToLower()
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Response.Redirect(Path.Combine(PortalUri, "TempImages/" + fileName));
                    return null;
                }
            }

            string inLine = string.Empty;
            if (Browse_Extension.IndexOf(Path.GetExtension(attachment.FileName).ToLower()) != -1)
            {
                inLine = "inline;";
            }
            else
            {
                inLine = "attachment;";
            }

            // 输出附件
            Response.Buffer = true;
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", inLine + "filename=" + HttpUtility.UrlEncode(attachment.FileName, System.Text.Encoding.UTF8));
            Response.AddHeader("Content-Type", attachment.ContentType);
            //Response.AddHeader("Content-Length", attachment.Content.LongLength.ToString());
            Response.TransmitFile(savePath);
            Response.End();
            System.IO.File.Delete(savePath);
            return null;
        }

        /// <summary>
        /// 获取附件下载地址
        /// </summary>
        /// <returns>附件下载地址</returns>
        private string GetAttachmentRedirectUrl()
        {
            string url = string.Empty;
            if (IsWeChat)
                url = this.Engine.WeChatAdapter.GetAttachmentWeChatViewUrl(
                     UserID,
                     this.BizObjectSchemaCode,
                     this.BizObjectID,
                     this.AttachmentID);
            else if (IsDingTalk)
                url = this.Engine.DingTalkAdapter.GetAttachmentDingTalkViewUrl(
               UserID,
               this.BizObjectSchemaCode,
               this.BizObjectID,
               this.AttachmentID);
            return url;

        }

        /// <summary>
        /// 批量下载
        /// </summary>
        /// <returns></returns>
        public object ReadBatch()
        {
            return ExecuteFileResultFunctionRun(() =>
            {

                AttachmentHeader[] headers = this.Engine.BizObjectManager.QueryAttachment(this.BizObjectSchemaCode,
                   this.BizObjectID,
                   DataField,
                   OThinker.Data.BoolMatchValue.True,
                   string.Empty);
                if (headers == null || headers.Length == 0)
                {
                    return null;
                }

                string directory = Path.Combine(Server.MapPath("~/TempImages"), BizObjectID);
                string storagePath = Path.Combine(directory, this.DataField);
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }

                string storageFile;
                byte[] fileContent = null;
                bool hasNewFile = false;
                List<string> files = new List<string>();

                // 1.先生成所有临时文件
                foreach (AttachmentHeader header in headers)
                {
                    files.Add(header.FileName);
                    storageFile = Path.Combine(storagePath, header.FileName);
                    if (System.IO.File.Exists(storageFile))
                    {
                        if (System.IO.File.GetLastWriteTime(storageFile) == header.CreatedTime) continue;
                    }
                    hasNewFile = true;
                    Attachment att = this.Engine.BizObjectManager.GetAttachment(UserID,
                        BizObjectSchemaCode,
                        BizObjectID,
                        header.ObjectID);
                    if (att != null) fileContent = att.Content;

                    using (FileStream fs = new FileStream(storageFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(fileContent);
                            bw.Close();
                        }
                    }
                    System.IO.File.SetCreationTime(storageFile, header.CreatedTime); // 设置文件创建时间和数据库表一致
                }

                // 2.开始删除多余的文件
                string[] storageFiles = Directory.GetFiles(storagePath);
                foreach (string f in storageFiles)
                {
                    if (!files.Contains(Path.GetFileName(f))) System.IO.File.Delete(f);
                }

                // 3.开始压缩
                string destFile = Path.Combine(directory, this.DisplayName + ".zip");
                if (!hasNewFile && System.IO.File.Exists(destFile))
                {
                    Response.Redirect(PortalRoot + "/TempImages/" + BizObjectID + "/" + this.DisplayName + ".zip");
                }
                else if (System.IO.File.Exists(destFile))
                {
                    System.IO.File.Delete(destFile);
                }
                ZipHelper.Zip(storagePath, destFile, string.Empty);
                Response.Redirect(PortalRoot + "/TempImages/" + BizObjectID + "/" + this.DisplayName + ".zip");

                return null;
            });
        }

    }
}
