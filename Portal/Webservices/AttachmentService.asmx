<%@ WebService Language="C#" Class="OThinker.H3.Attachment.AttachmentService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Data;
using OThinker.H3.Controllers;
using OThinker.H3.WorkItem;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using Aspose.Words;
using System.IO;
using System.Drawing;
using Aspose.Words.Drawing;

namespace OThinker.H3.Attachment
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AttachmentService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set
            {
                _Engine = value;
            }
        }
        [WebMethod(Description = "权限验证")]
        public string ValidateH3FileAuthority(string role, string userid)
        {
            string result = "";
            string sql = "select datafield from DZ_H3File_Authority where role='" + role + "' and userid='" + userid + "'";
            object obj = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
            if (obj != null)
            {
                result = obj.ToString();
            }
            return result;

        }
        [WebMethod(Description = "帐号信息验证")]
        public string ValidateUser(string userCode, string password)
        {
            OThinker.H3.Controllers.UserValidator user = OThinker.H3.Controllers.UserValidatorFactory.Validate(userCode, password);
            if (user != null)
            {
                return "{ \"errcode\": \"0\", \"errmsg\": \"验证成功\", \"data\":[]}";
            }
            else
            {
                return "{ \"errcode\": \"1\", \"errmsg\": \"帐号或密码不正确\", \"data\":[]}";
            }

        }
        [WebMethod(Description = "获取放款单信息")]
        public DataSet GetPickApplicationNo(DateTime StartTime, DateTime EndTime)
        {
            DataSet ds = new DataSet();
            string sql = "select PROPOSAL_NBR from (";
            sql += " SELECT ct.PROPOSAL_NBR,a.bankname || b.bankname || c.bankname bankname from ( ";
            sql += " SELECT PROPOSAL_NBR FROM CONTRACT@TO_CMS C where C.PROPOSAL_NBR is not null and C.CONTRACT_ACTIVATION_DTE between TO_DATE('" + StartTime + "','YYYY/MM/DD HH24:mi:ss') and  TO_DATE('" + EndTime + "','YYYY/MM/DD HH24:mi:ss') ) ct";
            sql += " left join  I_RetailApp a on ct.PROPOSAL_NBR = a.applicationno";
            sql += " left join  I_CompanyApp b on ct.PROPOSAL_NBR = b.applicationno";
            sql += " left join  i_Application c on ct.PROPOSAL_NBR = c.application_number ) where bankname='中国工商银行'";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            ds.Tables.Add(dt);
            return ds;
        }
        [WebMethod(Description = "获取附件信息")]
        public Data.AttachmentHeader[] GetAttachmentContent(string applicationNo)
        {
            List<Data.AttachmentHeader> result = new List<Data.AttachmentHeader>();
            if (string.IsNullOrEmpty(applicationNo))
                return result.ToArray();
            string sql = @"
select objectid,bizobjectschemacode,bizobjectid from Ot_Attachment where bizobjectid in (
select objectid from I_RetailApp where applicationno='{0}'
union all
select objectid from I_CompanyApp where applicationno='{0}'
union all
select objectid from I_Application where application_number='{0}'
) order by createdtime 
";
            sql = string.Format(sql, applicationNo);
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return result.ToArray();
            string schemacode = dt.Rows[0]["bizobjectschemacode"] + string.Empty;
            string bizobjectid = dt.Rows[0]["bizobjectid"] + string.Empty;
            List<string> objs = new List<string>();
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                objs.Add(dt.Rows[index]["objectid"] + string.Empty);
            }

            return this.Engine.BizObjectManager.GetAttachmentHeaders(schemacode, bizobjectid, objs.ToArray());
        }
        [WebMethod(Description = "贷后-获取工行附件")]
        public Data.AttachmentHeader[] GetGongHangAttachmentContent(string applicationNo, string Datafields)
        {
            List<Data.AttachmentHeader> result = new List<Data.AttachmentHeader>();
            if (string.IsNullOrEmpty(applicationNo))
                return result.ToArray();
            string sql = " select objectid,bizobjectschemacode,bizobjectid from Ot_Attachment where datafield in (" + Datafields + ") and bizobjectid in (";
            sql += " select objectid from I_RetailApp where applicationno='" + applicationNo + "'";
            sql += " union all";
            sql += " select objectid from I_CompanyApp where applicationno='" + applicationNo + "'";
            sql += " union all";
            sql += " select objectid from I_Application where application_number='" + applicationNo + "'";
            sql += " ) order by createdtime ";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return result.ToArray();
            string schemacode = dt.Rows[0]["bizobjectschemacode"] + string.Empty;
            string bizobjectid = dt.Rows[0]["bizobjectid"] + string.Empty;
            List<string> objs = new List<string>();
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                objs.Add(dt.Rows[index]["objectid"] + string.Empty);
            }

            return this.Engine.BizObjectManager.GetAttachmentHeaders(schemacode, bizobjectid, objs.ToArray());
        }
        [WebMethod(Description = "获取附件二进制")]
        public Byte[] GetAttachmentByteContent(string BizObjectSchemaCode, string BizObjectId,string AttachmentID)
        {
             OThinker.H3.Data.Attachment attachment1 = OThinker.H3.Controllers.AppUtility.Engine.BizObjectManager.GetAttachment
                            (string.Empty,
                             BizObjectSchemaCode,
                             BizObjectId,
                             AttachmentID);
                return attachment1.Content;
        }
        /// <summary>
        /// 生成放款承诺函，并添加到表单的附件中
        /// </summary>
        /// <param name="applicationNum">申请单号</param>
        /// <param name="customername">客户名称</param>
        /// <param name="id">InstanceID</param>
        /// <param name="field">数据项名称</param>
        /// <returns></returns>
        [WebMethod(Description = "生成放款承诺函，并添加到表单的附件中")]
        public string GenerateWordAttachment(string applicationNum, string customername, string id, string field)
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                AppUtility.Engine.LogWriter.Write("GenerateWordAttachment：ApplicationNumber-->" + applicationNum + "，CustomerName-->" + customername +
                    "，InstanceID-->" + id + "，DataField-->" + field);
                if (string.IsNullOrEmpty(applicationNum) || string.IsNullOrEmpty(customername)
                        || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(field))
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "参数错误，一个或多个参数值为空" });
                }
                InstanceContext con = AppUtility.Engine.InstanceManager.GetInstanceContext(id);
                if (con == null)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "ID错误或不存在" });
                }

                var document = new Document(path + "/template/FileTemplate/放款承诺函模板.docx");

                //DateTime dtNow = DateTime.Now;
                //string fileName = "放款承诺函_" + applicationNum + "_" + customername + "_" + dtNow.ToString("yyyyMMdd") + ".pdf";
                //string filePath = System.Web.HttpContext.Current.Server.MapPath("/Portal/TempImages/" + fileName);
                DateTime dtNow = DateTime.Now;
                string fileName = "放款承诺函_" + applicationNum + "_" + customername + "_" + dtNow.ToString("yyyyMMdd") + ".pdf";
                string rootPath = System.Web.HttpContext.Current.Server.MapPath("/Portal/TemplateFile/放款承诺函/");
                DirectoryInfo di = new DirectoryInfo(rootPath);
                if (!di.Exists)
                    di.Create();
                string filePath = rootPath + fileName;

                document.Range.Bookmarks["ApplicationNumber"].Text = applicationNum;
                document.Range.Bookmarks["CustomerName"].Text = customername;
                document.Range.Bookmarks["Day"].Text = dtNow.Day > 9 ? dtNow.Day.ToString() : "0" + dtNow.Day;
                document.Range.Bookmarks["Month"].Text = dtNow.Month > 9 ? dtNow.Month.ToString() : "0" + dtNow.Month;
                document.Range.Bookmarks["Year"].Text = dtNow.Year.ToString();


                InsertWatermarkText(document, "上海东正汽车金融股份有限公司");

                document.Save(filePath, SaveFormat.Pdf);//保存文档


                byte[] contents = GetFileContent(filePath);
                OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment();
                attachment.ObjectID = Guid.NewGuid().ToString();
                attachment.Content = contents;
                attachment.ContentType = MimeMapping.GetMimeMapping(fileName);
                attachment.CreatedBy = OThinker.Organization.User.AdministratorID;
                attachment.CreatedTime = System.DateTime.Now;
                attachment.FileName = fileName;
                attachment.LastVersion = true;
                attachment.ModifiedBy = null;
                attachment.ModifiedTime = System.DateTime.Now;
                attachment.BizObjectSchemaCode = con.BizObjectSchemaCode;
                attachment.ContentLength = contents.Length;
                attachment.BizObjectId = con.BizObjectId;
                attachment.DataField = field;

                string attID = AppUtility.Engine.BizObjectManager.AddAttachment(attachment);
                AppUtility.Engine.LogWriter.Write("添加附件成功：FileName-->" + fileName + "，ID-->" + attID);
                //File.Delete(filePath);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = "", Msg = "添加附件成功：FileName-->" + fileName + "，ID-->" + attID });

            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(ex.ToString());
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = ex.ToString() });
            }
        }


        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        //文件转为二进制
        public static byte[] GetFileContent(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new byte[0];
            }

            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];

            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }

        /// <summary>
        /// Inserts a watermark into a document.
        /// </summary>
        /// <param name="doc">The input document.</param>
        /// <param name="watermarkText">Text of the watermark.</param>
        private static void InsertWatermarkText(Document doc, string watermarkText)
        {
            // Create a watermark shape. This will be a WordArt shape.
            // You are free to try other shape types as watermarks.
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = Color.LightGray; // Try LightGray to get more Word-style watermark
            watermark.StrokeColor = Color.LightGray; // Try LightGray to get more Word-style watermark

            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = VerticalAlignment.Center;
            watermark.HorizontalAlignment = HorizontalAlignment.Center;

            // Create a new paragraph and append the watermark to this paragraph.
            Paragraph watermarkPara = new Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            // Insert the watermark into all headers of each document section.
            foreach (Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        private static void InsertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            HeaderFooter header = sect.HeadersFooters[headerType];

            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }

            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }
    }
}

