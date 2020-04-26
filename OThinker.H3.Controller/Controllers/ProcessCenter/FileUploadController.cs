using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessCenter
{
    public class FileUploadController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        private string _SchemaCode;
        /// <summary>
        /// SchemaCode
        /// </summary>
        protected string SchemaCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._SchemaCode))
                {
                    this._SchemaCode = HttpUtility.UrlDecode(this.Request[Param_SchemaCode]);
                }
                return this._SchemaCode;
            }
        }

        private string _BizObjectID;
        /// <summary>
        /// BizObjectID
        /// </summary>
        protected string BizObjectID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._BizObjectID))
                {
                    this._BizObjectID = HttpUtility.UrlDecode(this.Request[Param_BizObjectID]);
                }
                return this._BizObjectID;
            }
        }

        private string _DataField;
        /// <summary>
        /// DataField
        /// </summary>
        protected string DataField
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._DataField))
                {
                    this._DataField = HttpUtility.UrlDecode(this.Request[Param_DataField]);
                }
                return this._DataField;
            }
        }

        private string _PostForm = "";
        private bool IsPostForm
        {
            get
            {
                if (string.IsNullOrEmpty(this._PostForm))
                {
                    this._PostForm = this.Request["PostForm"] ?? "false";
                }
                return this._PostForm == "true";
            }
        }

        private string _FileID;
        /// <summary>
        /// 页面上对应的File显示ID
        /// </summary>
        protected string FileID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._FileID))
                {
                    this._FileID = this.Request["fileid"] ?? "";
                }
                return this._FileID;
            }
        }

        private int _maxSize = 0;
        /// <summary>
        /// 获取允许上传的文件大小(KB)
        /// </summary>
        protected int MaxSize
        {
            get
            {
                if (!int.TryParse(Request.Params["MaxSize"], out _maxSize))
                {
                    _maxSize = 10 * 1024;
                }
                return _maxSize;
            }
        }

        private string fileExtensions = string.Empty;
        /// <summary>
        /// 获取允许上传的文件后缀名
        /// </summary>
        protected string FileExtensions
        {
            get
            {
                if (fileExtensions == string.Empty)
                    fileExtensions = (Request["FileExtensions"] + string.Empty).Trim();
                return fileExtensions;
            }
        }

        protected bool IsMobile
        {
            get
            {
                return (Request["IsMobile"] ?? "").ToLowerInvariant() == "true";
            }
        }
        public JsonResult UploadFile()
        {
            System.Web.HttpFileCollectionBase files = HttpContext.Request.Files; files = Request.Files;
            if (files == null || files.Count == 0) return null;
            string attachmentId = Guid.NewGuid().ToString();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i] as System.Web.HttpPostedFileBase;
                if (file.ContentLength == 0) continue;
                if (this.IsPostForm)
                {
                    attachmentId = files.AllKeys[i];
                }

                int checkResult = this.CheckFile(file);
                if (checkResult != 0 && checkResult != 1)
                {
                    //检查失败
                    var result = new HandlerResult(this.FileID, string.Empty, checkResult);
                    return Json(result);
                }
                if (!string.IsNullOrEmpty(this.BizObjectID))
                {
                    if (this.Engine.BizObjectManager.GetAttachmentHeader(this.SchemaCode, this.BizObjectID, attachmentId) != null)
                    {
                        continue;
                    }
                }
                UploadFile(file, attachmentId);
            }

            if (!this.IsPostForm)
            {
                string url = AppConfig.GetReadAttachmentUrl(
                       this.IsMobile,
                       this.SchemaCode,
                       "",
                       attachmentId);
                var result = new HandlerResult(this.FileID, attachmentId, url);
                return Json(result);
            }

            return null;
        }

        /// <summary>
        /// 上传文件d
        /// </summary>
        /// <param name="file"></param>
        private string UploadFile(HttpPostedFileBase file, string attachmentId)
        {
            // 添加这个附件
            byte[] contents = new byte[file.ContentLength];
            file.InputStream.Read(contents, 0, file.ContentLength);
            OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment();
            attachment.ObjectID = attachmentId;
            attachment.Content = contents;
            attachment.ContentType = file.ContentType;
            attachment.CreatedBy = this.UserValidator.UserID;
            attachment.CreatedTime = System.DateTime.Now;
            attachment.FileName = System.IO.Path.GetFileName(ReplaceFileName(file.FileName));
            attachment.LastVersion = true;
            attachment.ModifiedBy = null;
            attachment.ModifiedTime = System.DateTime.Now;
            attachment.BizObjectSchemaCode = this.SchemaCode;
            attachment.ContentLength = file.ContentLength;
            attachment.BizObjectId = this.BizObjectID;
            attachment.DataField = this.DataField;
            // 禁止下载
            //if (this.chkDisableDownload.Checked)
            //{
            //    attachment.FileFlag |= Data.Attachment.FileFlag_DisableDownload;
            //}
            //// 禁止打印
            //if (this.chkDisablePrint.Checked)
            //{
            //    attachment.FileFlag |= Data.Attachment.FileFlag_DisablePrint;
            //}

            return this.Engine.BizObjectManager.AddAttachment(attachment);
        }

        /// <summary>
        /// 检查文件是否符合条件
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// 0:默认
        /// 1:成功
        /// 2:超过文件限制大小
        /// 3:不是限制的文件类型
        /// </returns>
        private int CheckFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return 0;
            string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            // 检查文件大小
            if (file.ContentLength > MaxSize * 1024)
            {
                return 2;
            }
            // 检查文件后缀
            if (FileExtensions != string.Empty)
            {
                return 3;
            }
            if (ForbidExtension != string.Empty)
            {
                if ((ForbidExtension + ",").ToLower().IndexOf(extension + ",") > -1)
                {
                    return 3;
                }
            }
            return 0;
        }

        private string forbidExtension = null;
        /// <summary>
        /// 获取被禁止上传的文件后缀名
        /// </summary>
        public string ForbidExtension
        {
            get
            {
                if (forbidExtension == null)
                    forbidExtension = System.Configuration.ConfigurationManager.AppSettings["ForbidExtension"] + string.Empty;
                return this.forbidExtension;
            }
        }

        //替换文件名中不合法的字符
        private string ReplaceFileName(string fileName)
        {
            string newFileName = string.Empty;
            try
            {
                //文件名，不含后缀
                newFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                //截取后缀名
                string exStr = System.IO.Path.GetExtension(fileName);

                string[] beforeString = { "#", "~", "&", "!", "@", "$", "%", "^", "*", "(", ")", "?", "{", "}", "." };
                string[] afterString = { "＃", "～", "＆", "！", "＠", "＄", "％", "︿", "＊", "（", "）", "？", "｛", "｝", "．" };
                for (int i = 0; i < beforeString.Length; i++)
                {
                    newFileName = newFileName.Replace(beforeString[i], afterString[i]);
                }
                newFileName += exStr;
            }
            catch (Exception ex)
            {
                newFileName = fileName;
            }
            return newFileName;
        }
        protected class HandlerResult
        {
            public string FileId;
            public string AttachmentId;
            /// <summary>
            /// 状态：
            /// 0:默认
            /// 1:成功
            /// 2:超过文件限制大小
            /// 3:不是限制的文件类型
            /// </summary>
            public int State;
            public string ResultStr = string.Empty;
            public string Url = "";
            public HandlerResult(string fileId, string attachmentId, string url)
            {
                this.FileId = fileId;
                this.AttachmentId = attachmentId;
                this.State = 1;
                this.Url = url;
            }

            public HandlerResult(string fileId, string attachmentId, int state)
            {
                this.FileId = fileId;
                this.AttachmentId = attachmentId;
                this.State = state;
                this.ResultStr = "";
                switch (this.State)
                {
                    case 0:
                    case 1:
                        break;
                    case 2:
                        this.ResultStr = "超过文件限制大小";
                        break;
                    case 3:
                        this.ResultStr = "限制的文件类型";
                        break;
                }
            }
        }
    }


}
