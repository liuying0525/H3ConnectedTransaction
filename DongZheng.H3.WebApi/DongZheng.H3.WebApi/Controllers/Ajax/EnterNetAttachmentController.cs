using ICSharpCode.SharpZipLib.Zip;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class EnterNetAttachmentController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string objectId = context.Request["objectId"] ?? "";
            string url = "http://172.16.10.80/";// System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"];
            string[] fileCount = new[]
            {
            "ApprovalForm", "CollectionTable", "BusinessLicense", "CreditDocuments", "AuthorizationDocument", "Photo",
            "DiscountAgreement", "CargoAgreement", "HallPhoto", "OfficePhoto", "BusinessInformation",
            "FinancialInformation", "FinancialId", "CooperationAgreement", "RepresentativeId", "PersonnelPhoto",
            "Report", "SiteContract", "XFHDSPB", "QuotaForm", "SalesData", "FinancialInstitutions", "BankFlow",
            "PersonalInformation", "SecurityAgreement", "CreditReport", "QYCreditReport", "QualificationInformation",
            "QueryRecord", "LegalPerson"
        };
            string instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectId + "'") + string.Empty;
            OThinker.H3.Instance.InstanceData instanceData = new OThinker.H3.Instance.InstanceData(OThinker.H3.Controllers.AppUtility.Engine, instanceid, null);
            string path = context.Server.MapPath("EnterNetAttachment.ashx");
            path = Path.GetDirectoryName(path);
            try
            {
                MemoryStream ms = new MemoryStream();
                string tempPath = path + "\\temp";
                Directory.CreateDirectory(tempPath);
                byte[] buffer = null;
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();
                    foreach (string field in fileCount)
                    {
                        string[] filenames;
                        UTF8Encoding edc = new UTF8Encoding();
                        string filelist = (instanceData.BizObject.ValueTable[field] ?? "").ToString();
                        if (string.IsNullOrEmpty(filelist)) continue;
                        filenames = filelist.IndexOf("<>") > -1 ? System.Text.RegularExpressions.Regex.Split(filelist, "<>", System.Text.RegularExpressions.RegexOptions.IgnoreCase) : new[] { filelist };
                        foreach (string filename in filenames)
                        {
                            string name = Guid.NewGuid().ToString();
                            string tempFile = tempPath + "\\" + filename.Split('>')[0].Split('.')[0] + Guid.NewGuid().ToString().Replace("-", "_") + "." + filename.Split('.')[filename.Split('.').Length - 1];
                            if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);
                            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                            HttpWebRequest request = WebRequest.Create(url + filename.Split('>')[1]) as HttpWebRequest;
                            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                            Stream responseStream = response.GetResponseStream();
                            byte[] bArr = new byte[1024];
                            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                            while (size > 0)
                            {
                                fs.Write(bArr, 0, size);
                                size = responseStream.Read(bArr, 0, (int)bArr.Length);
                            }
                            fs.Close();
                            responseStream.Close();
                            file.Add(tempFile);
                        }
                    }
                    file.CommitUpdate();
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
                try
                {
                    context.Response.Clear();
                }
                catch
                {
                }
                DirectoryInfo TheFolder = new DirectoryInfo(tempPath);
                foreach (FileInfo NextFile in TheFolder.GetFiles()) System.IO.File.Delete(tempPath + "\\" + NextFile.Name);
                Directory.Delete(tempPath);
                context.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".zip", System.Text.Encoding.UTF8));
                context.Response.ContentType = "application/x-zip-compressed";
                context.Response.ContentEncoding = Encoding.Default;
                context.Response.BinaryWrite(buffer);
            }
            catch (Exception ex)
            {
            }
        }

        public static byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        public class MyNameTransfom : ICSharpCode.SharpZipLib.Core.INameTransform
        {
            #region INameTransform 成员
            public string TransformDirectory(string name)
            {
                return null;
            }
            public string TransformFile(string name)
            {
                return Path.GetFileName(name);
            }
            #endregion
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}