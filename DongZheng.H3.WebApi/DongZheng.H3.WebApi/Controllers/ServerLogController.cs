using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class ServerLogController : CustomController
    {
        //private string path = @"D:\H3 Proposal\Server\Log\";
        private string path = @"C:\Program Files\Authine\H3 BPM\Server\Log\";
        // GET: ServerLog
        public string Index()
        {
            return "ServerLog API";
        }
        #region Attribute
        public override string FunctionCode
        {
            get { return ""; }
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

        protected bool IsMobile
        {
            get
            {
                return (Request["IsMobile"] ?? "").ToLowerInvariant() == "true";
            }
        }
        #endregion

        public string ReadServerLog(string logFolder, string logNameRule, string seachTime,string pageSize,string pageIndex)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            json.MaxJsonLength = Int32.MaxValue;
            object obj = new object();
            bool successState = false;
            string logFilePath = string.Empty;
            string logFileName = string.Empty;
            if (logFolder == "Default")
            {
                logFilePath = path;
            }
            else
            {
                logFilePath = path + logFolder + @"\";
            }
            logFileName = seachTime + "_" + logNameRule + ".txt";
            try
            {
                obj = readFile((logFilePath + logFileName), pageSize, pageIndex);
                successState = true;
            }
            catch (Exception ex)
            {
                obj = "文件：" + (logFilePath + logFileName) + "读取出错了，错误原因：" + ex.ToString();
            }
            return json.Serialize(new { Success = successState, Data = obj });
        }

        protected string readFile(string path, string pageSize, string pageIndex)
        {
            string fileData = string.Empty;
            StreamReader reader = new StreamReader(path, Encoding.UTF8);
            try
            {   ///读取文件的内容      
                fileData = reader.ReadToEnd().Replace('\n', ' ').Replace('\r', '~');
                int length = fileData.Length;
                int conPageSize =int.Parse(pageSize);
                int conPageIndex = int.Parse(pageIndex);
                fileData = fileData.Substring(length / conPageSize * (conPageIndex - 1), length / conPageSize);
            }
            catch (Exception ex)
            {
                //抛出异常  
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                reader.Close();
            }
            return fileData;
        }
    }

}