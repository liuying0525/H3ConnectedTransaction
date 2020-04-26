using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Controls
{
    public class OfficeServiceController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 保存NTKO附件
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveOfficeAttachment()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                if (Request.Files.Count == 0)
                {
                    Response.End();
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string ID = Request.Form["ID"];
                string dataField = Request.Form["dataField"];
                string instanceID = Request.Form["InstanceID"] + string.Empty;
                string download = Request.Form["download"] + string.Empty;
                string bizObjectID = Request.Form["BizObjectID"] + string.Empty;
                string schemaCode = Request.Form["SchemaCode"] + string.Empty;
                //string workflowPackage = Request.Form["WorkflowPackage"] + string.Empty;
                //string workflowName = Request.Form["WorkflowName"] + string.Empty;

                byte[] fileValue = new byte[Request.Files[0].ContentLength];
                if (fileValue.Length == 0) return null; // 没有接收到数据，则不做保存
                Request.Files[0].InputStream.Read(fileValue, 0, Request.Files[0].ContentLength);
                OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment()
                {
                    ObjectID = ID,
                    FileName = Server.UrlDecode(Request.Files[0].FileName),
                    ContentType = Request.Files[0].ContentType,
                    Content = fileValue,
                    DataField = Server.UrlDecode(dataField),
                    CreatedBy = this.UserValidator.UserID,
                    Downloadable = true,
                    BizObjectId = bizObjectID,
                    BizObjectSchemaCode = schemaCode
                };
                try
                {
                    this.Engine.BizObjectManager.RemoveAttachment(string.Empty,
                        schemaCode,
                        attachment.BizObjectId,
                        attachment.AttachmentID);
                    this.Engine.BizObjectManager.AddAttachment(attachment);
                }
                catch (Exception ex)
                {
                    this.Engine.LogWriter.Write("WordSave Error:" + ex.ToString());
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 打开NTKO附件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult OpenOfficeAttachment()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                string dataField, attachmentId;
                dataField = OThinker.Data.Convertor.SqlInjectionPrev(Server.UrlDecode(Request.QueryString["dataField"]));
                attachmentId = OThinker.Data.Convertor.SqlInjectionPrev(Request.QueryString["AttachmentID"]);
                string bizObjectID = OThinker.Data.Convertor.SqlInjectionPrev(Request.QueryString["BizObjectID"] + string.Empty);
                string schemaCode = OThinker.Data.Convertor.SqlInjectionPrev(Server.UrlDecode(Request.QueryString["SchemaCode"] + string.Empty));
                if (string.IsNullOrEmpty(attachmentId))
                {
                    OThinker.H3.Data.AttachmentHeader[] attachments = this.Engine.BizObjectManager.QueryAttachment(schemaCode, bizObjectID, dataField, OThinker.Data.BoolMatchValue.True, null);
                    if (attachments != null && attachments.Length > 0)
                        attachmentId = attachments[0].ObjectID;
                }

                if (!string.IsNullOrEmpty(attachmentId))
                {
                    OThinker.H3.Data.Attachment attachment = this.Engine.BizObjectManager.GetAttachment(string.Empty,
                        schemaCode,
                        bizObjectID,
                        attachmentId);

                    //设定输出文件类型
                    Response.ContentType = attachment.ContentType;
                    // 输出文件内容
                    Response.OutputStream.Write(attachment.Content, 0, attachment.Content.Length);
                    Response.End();
                }
                //else
                //    Response.Write("<script>alert('" + this.PortalResource.GetString("SheetOffcie_HasPDF") + "');</script>");

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult UpFileData()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                string strNewAttachIds = string.Empty;
                string dataField = Request.Form["dataField"];
                string instanceID = Request.Form["InstanceID"] + string.Empty;
                //新加的属性，待确认
                //ERROR: 
                string schemaCode = Request.Form["SchemaCode"] + string.Empty;
                string bizObjectID = Request.Form["BizObjectID"] + string.Empty;
                //string workflowPackage = Request.Form["WorkflowPackage"] + string.Empty;
                //string workflowName = Request.Form["WorkflowName"] + string.Empty;
                if (dataField == string.Empty)
                {
                    result.Success = false;
                    result.Message = "数据项名称为空";
                    return Json(result);
                }
                System.Web.HttpPostedFileBase theFile;
                //说明:若控件的参数 <PARAM NAME="DelFileField" VALUE="DELETE_FILE_NAMES">
                //则,从Request.Form得到的DELETE_FILE_NAMES集合包含了所有需要删除的文件
                //也就是客户端修改过的文件名
                string[] delFileNames = Request.Form.GetValues("DELETE_FILE_NAMES");
                System.Web.HttpFileCollectionBase uploadFiles = Request.Files;

                try
                {
                    OThinker.H3.Data.AttachmentHeader[] headers = null;
                    if (instanceID != string.Empty)
                    {
                        headers = AppUtility.Engine.BizObjectManager.QueryAttachment(schemaCode, bizObjectID, dataField, OThinker.Data.BoolMatchValue.True, string.Empty);
                    }

                    // 增加保存附件文件
                    for (int i = 0; i < uploadFiles.Count; i++)
                    {
                        theFile = uploadFiles[i];
                        // "EDITFILE"和客户端的BeginSaveToURL的第二个参数一致
                        if (uploadFiles.GetKey(i).ToUpper() == "EDITFILE")
                        {
                            // 写入FILE数据
                            string fname = theFile.FileName;
                            int fsize = theFile.ContentLength;
                            string contentType = theFile.ContentType;
                            byte[] buffer = new byte[theFile.ContentLength];
                            theFile.InputStream.Read(buffer, 0, fsize);

                            OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment()
                            {
                                ObjectID = Guid.NewGuid().ToString(),
                                FileName = fname,
                                ContentType = string.Empty,
                                Content = buffer,
                                DataField = Server.UrlDecode(dataField),
                                CreatedBy = this.UserValidator.UserID,
                                Downloadable = true,
                                BizObjectId = bizObjectID,
                                BizObjectSchemaCode = schemaCode
                            };

                            strNewAttachIds += attachment.ObjectID + ",";

                            this.Engine.BizObjectManager.AddAttachment(attachment);
                        }
                    }
                    if (headers != null && delFileNames != null)
                    {
                        foreach (OThinker.H3.Data.AttachmentHeader header in headers)
                        {
                            if (Array.IndexOf<string>(delFileNames, header.FileName) > -1)
                                this.Engine.BizObjectManager.RemoveAttachment(this.UserValidator.UserID,
                                    schemaCode,
                                    header.BizObjectId,
                                    header.ObjectID);
                        }
                    }
                    // Response.Write(strNewAttachIds);
                    result.Message = strNewAttachIds;
                    return Json(result);
                }
                catch (Exception err)
                {
                    result.Success = false;
                    result.Message = "NTKO Attachment ERROR:" + err.ToString();
                    AppUtility.Engine.LogWriter.Write("NTKO Attachment ERROR:" + err.ToString());
                    return Json(result);
                }
            });
        }
    }
}
