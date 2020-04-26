using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Data;
namespace postApplicationToSFTP
{

    class Program
    {
        static void Main(string[] args)
        {
            string host = System.Configuration.ConfigurationManager.AppSettings["host"] + string.Empty;// "139.199.250.133" ;
            string username = System.Configuration.ConfigurationManager.AppSettings["username"] + string.Empty;//"dongzheng";
            string password = System.Configuration.ConfigurationManager.AppSettings["password"] + string.Empty;//"dongzheng@123$";// ;
            int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"]);//22;
            string filename = System.DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            string root = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Excel\\";
            string fullname = root + filename + ".xls";
            string ftpDirectory = System.Configuration.ConfigurationManager.AppSettings["ftpDirectory"] + string.Empty;// "dzDocument";

            sftpService.ForExternalWebService service = new sftpService.ForExternalWebService();
            string date = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            //Console.Write("查询日期："+date);
            DataSet ds = service.getApplicationInfoForSFTP(date);
            if (ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Rows.Count < 1)
            {
                AddLog(filename, filename + "进件资料推送，共计上传资料" + 0 + "单，未查到进件资料！");
            }
            DataTable dt = ds.Tables[0];
            SaveExcel(dt, filename);
            try
            {
                using (var client = new SftpClient(host, port, username, password)) //创建连接对象
                {
                    client.Connect(); //连接
                    client.ChangeDirectory(ftpDirectory); //切换目录
                    //var listDirectory = client.ListDirectory("/" + ftpDirectory); //获取目录下所有文件
                    //foreach (var fi in listDirectory) //遍历文件
                    //{
                    //    Console.WriteLine(fi.Name);
                    //    if (fi.Name.Contains(".txt") || fi.Name.Contains(".xls"))
                    //    {
                    //        client.DeleteFile(fi.FullName);//删除文件
                    //    }
                    //}
                    using (var fileStream = new FileStream(fullname, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // bypass Payload error large
                        client.UploadFile(fileStream, Path.GetFileName(fullname)); //上传文件
                        //UploadFile方法没有返回值，无法判断文件是否上传成功，我想到的解决办法是，上传后再获取一下文件列表，如果文件列表count比上传之前大，说明上传成功。当然
                        //这样的前提是只有你一个人上传。不知各位大神有没有其它办法
                        //生成上传文件
                        AddLog(filename, filename + "进件资料推送，共计上传资料" + dt.Rows.Count + "单");
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog(filename + "_error", ex.ToString());
            }
            //Console.ReadKey();
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="folder">文件夹相对路径</param>
        /// <param name="logName">日志文件名称</param>
        /// <param name="logInfo">日志文件内容</param>
        public static string AddLog(string logName, string logInfo)
        {
            string rtn = "true";
            try
            {
                string LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\";
                if (!Directory.Exists(LogDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(LogDire);
                }
                string path = LogDire + logName + ".txt";
                if (!File.Exists(path))
                {
                    // 不存在，创建
                    FileStream fs = File.Create(path);
                    fs.Close();

                    File.WriteAllText(path, DateTime.Now.ToString() + ":" + logInfo, Encoding.Default);
                }
                else
                {
                    File.AppendAllText(path, "\r\n" + DateTime.Now.ToString() + ":" + logInfo, Encoding.Default);
                }
            }
            catch
            {
                rtn = "fase";
            }
            return rtn;
        }
        /// <summary>
        /// Datatable生成Excel表格并返回路径
        /// </summary>
        /// <param name="m_DataTable">Datatable</param>
        /// <param name="s_FileName">文件名</param>
        /// <returns></returns>
        public static string SaveExcel(System.Data.DataTable m_DataTable, string s_FileName)
        {
            string LogDire = AppDomain.CurrentDomain.BaseDirectory + "Excel\\";

            if (!Directory.Exists(LogDire))
            {
                // 不存在，创建
                Directory.CreateDirectory(LogDire);
            }
            string FileName = LogDire + s_FileName + ".xls";  //文件存放路径
            if (System.IO.File.Exists(FileName))              //存在则删除
            {
                System.IO.File.Delete(FileName);
            }
            System.IO.FileStream objFileStream;
            System.IO.StreamWriter objStreamWriter;
            string strLine = "";
            objFileStream = new System.IO.FileStream(FileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            objStreamWriter = new System.IO.StreamWriter(objFileStream, Encoding.Unicode);
            for (int i = 0; i < m_DataTable.Columns.Count; i++)
            {
                strLine = strLine + m_DataTable.Columns[i].Caption.ToString() + Convert.ToChar(9);      //写列标题
            }
            objStreamWriter.WriteLine(strLine);
            strLine = "";
            for (int i = 0; i < m_DataTable.Rows.Count; i++)
            {
                for (int j = 0; j < m_DataTable.Columns.Count; j++)
                {
                    if (m_DataTable.Rows[i].ItemArray[j] == null)
                        strLine = strLine + " " + Convert.ToChar(9);                                    //写内容
                    else
                    {
                        string rowstr = "";
                        rowstr = m_DataTable.Rows[i].ItemArray[j].ToString();
                        if (rowstr.IndexOf("\r\n") > 0)
                            rowstr = rowstr.Replace("\r\n", " ");
                        if (rowstr.IndexOf("\t") > 0)
                            rowstr = rowstr.Replace("\t", " ");
                        strLine = strLine + rowstr + Convert.ToChar(9);
                    }
                }
                objStreamWriter.WriteLine(strLine);
                strLine = "";
            }
            objStreamWriter.Close();
            objFileStream.Close();
            return FileName;        //返回生成文件的绝对路径

        }
    }
}
