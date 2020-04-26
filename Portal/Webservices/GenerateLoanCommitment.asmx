<%@ WebService Language="C#" Class="OThinker.H3.Attachment.GenerateLoanCommitment" %>

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
    public class GenerateLoanCommitment : System.Web.Services.WebService
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

        [WebMethod(Description = "生成放款承诺函")]
        public string Generate()
        {
            string sqlFinished = @"select distinct app.applicationno,con.objectid instanceid,app.Thaifirstname,con.bizobjectid from Ot_Workitemfinisheddistinct item
left join OT_InstanceContext con on item.instanceid=con.objectid
left join i_Retailapp app on con.bizobjectid=app.objectid
 where item.activitycode='Activity18' and item.finishtime>to_date('2018-04-27 16:00:00','yyyy-MM-dd hh24:mi:ss') 
 and item.state=2 and con.state=4 and con.bizobjectschemacode='RetailApp'";
            string sqlUnfinished = @"
 select app.applicationno,con.objectid instanceid,app.Thaifirstname,con.bizobjectid from Ot_Workitem item
left join OT_InstanceContext con on item.instanceid=con.objectid
left join i_Retailapp app on con.bizobjectid=app.objectid
 where item.activitycode='Activity17'  
 and con.state=2 and con.bizobjectschemacode='RetailApp'
";
            DataTable dtFinished = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlFinished);
            DataTable dtUnfinished = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlUnfinished);
            foreach (DataRow row in dtFinished.Rows)
            {
                GenerateWordAttachment(row["bizobjectid"] + string.Empty, row["applicationno"] + string.Empty, row["Thaifirstname"] + string.Empty, row["instanceid"] + string.Empty);

            }
            foreach (DataRow row in dtUnfinished.Rows)
            {
                GenerateWordAttachment(row["bizobjectid"] + string.Empty, row["applicationno"] + string.Empty, row["Thaifirstname"] + string.Empty, row["instanceid"] + string.Empty);
            }
            return "执行完成";
        }

        public string GenerateWordAttachment(string bizObjectID,string applicationNum, string customername, string id)
        {
            string field = "FKH";
            try
            {
                if (IsHaveFKH(bizObjectID, field))
                {
                    AppUtility.Engine.LogWriter.Write("已经生成过放款函，不做处理，单号-->" + applicationNum);
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = "", Msg = "已经生成过放款函，不做处理，单号-->" + applicationNum });
                }
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                AppUtility.Engine.LogWriter.Write("GenerateWordAttachment：ApplicationNumber-->" + applicationNum + "，CustomerName-->" + customername +
                    "，InstanceID-->" + id + "，DataField-->" + field);
                if (string.IsNullOrEmpty(applicationNum) || string.IsNullOrEmpty(customername)
                        || string.IsNullOrEmpty(id))
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "参数错误，一个或多个参数值为空" });
                }
                InstanceContext con = AppUtility.Engine.InstanceManager.GetInstanceContext(id);
                if (con == null)
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "ID错误或不存在" });
                }

                var document = new Document(path + "/template/FileTemplate/放款承诺函模板.docx");

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


                InsertWatermarkText(document, "上海东正汽车金融有限责任公司");

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

        public bool IsHaveFKH(string bizid, string datafield)
        {
            string sql = "select objectid from Ot_Attachment where bizobjectid='" + bizid + "' and datafield='" + datafield + "'";
            string obj = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            if (obj == "")
                return false;
            else
            {
                //string sqldelete = "delete from OT_attachment where objectid='" + obj + "'";
                //AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqldelete);
                //return false;
                return true;
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

