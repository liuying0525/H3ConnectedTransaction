using System;
using System.IO;
using System.Data;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Attachment_Aarchive
{
    class Program
    {
        public static string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            string role = System.Configuration.ConfigurationManager.AppSettings["role"] + string.Empty; ;

            DataTable dtExcel = new DataTable();
            var dt1 = DateTime.Now;
            WriteLog("**********************************************");
            WriteLog("*************开始执行附件归档程序*************");
            WriteLog("**********************************************");


            AttachmentService.AttachmentService att = new AttachmentService.AttachmentService();

            //H3附件保存的文件位置(备份位置)
            string h3AttachmentFilePath = System.Configuration.ConfigurationManager.AppSettings["H3AttachmentFilePath"] + string.Empty;
            //生成的文件目录
            string GeneratePath = System.Configuration.ConfigurationManager.AppSettings["GenerateFilePath"] + string.Empty;
            //复制文件是否需要覆盖原文件;
            bool OverWrite = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OverWrite"] + string.Empty);

            if (role==null || role == "")
            {
                //Excel申请号文件
                string excelPath = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"] + string.Empty;
                //Excel中取数据的Sheet页
                string sheetName = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplateSheetName"] + string.Empty;
                WriteLog("Excel模板路径-->" + excelPath);
                WriteLog("文件生成的路径-->" + GeneratePath);
                dtExcel = GetExcelContent(currentPath + excelPath, sheetName);
                WriteLog("单据数据-->" + dtExcel.Rows.Count);
            }
            else if (role == "贷后资料查询")
            {
                GeneratePath = AppDomain.CurrentDomain.BaseDirectory +"\\运营贷后资料";
                if (!Directory.Exists(GeneratePath))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(GeneratePath);//创建该文件夹
                }
                string psd="",account="";
                string startTime="" , endTime="";
                Console.WriteLine("请输入你的账号：");
                account = Console.ReadLine();
                Console.WriteLine("密码：");
                psd = Console.ReadLine();
                string result=att.ValidateUser(account,psd);
                if(result.Contains("验证成功"))
                {
                    Console.WriteLine("验证通过！");
                }
                
                Console.WriteLine("请输入开始日期：");
                try
                {
                    startTime =Console.ReadLine();
                }
                catch
                {
                }
                
                Console.WriteLine("请输入结束日期：");
                try
                {
                    endTime =Console.ReadLine();
                }
                catch
                {
                }
                DataSet ds = att.GetPickApplicationNo(Convert.ToDateTime(startTime), Convert.ToDateTime(endTime));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dtExcel = ds.Tables[0];
                }
            }
            foreach (DataRow row in dtExcel.Rows)
            {
                string appNO = row[0].ToString();

                AttachmentService.AttachmentHeader[] atts = att.GetAttachmentContent(appNO);
                //Attachment[] atts = GetAttachmentContent(appNO).ToArray();
                WriteLog("开始处理单据-->" + appNO + ",附件数-->" + atts.Length);

                Dictionary<string, int> fileNumberInfo = new Dictionary<string, int>();              

                foreach (var attachment in atts)
                {
                    try
                    {
                        //保存文件
                        string FilePath = GeneratePath + "/" + appNO;

                        //决断目录是否存在,不存在则创建
                        DirectoryInfo di = new DirectoryInfo(FilePath);
                        if (!di.Exists)
                            di.Create();

                        //按文件名称进行计数
                        if (fileNumberInfo.Keys.Contains(attachment.FileName))
                            fileNumberInfo[attachment.FileName] = fileNumberInfo[attachment.FileName] + 1;
                        else
                            fileNumberInfo[attachment.FileName] = 1;
                        //根据同名文件的数量对文件名称进行重命名
                        string fileName = "";
                        if (fileNumberInfo[attachment.FileName] == 1)
                            fileName = attachment.FileName;
                        else
                            fileName = Path.GetFileNameWithoutExtension(attachment.FileName) + "_" + fileNumberInfo[attachment.FileName] + Path.GetExtension(attachment.FileName);
                        //文件复制
                        File.Copy(h3AttachmentFilePath + "/" + attachment.StoragePath + "/" + attachment.StorageFileName, FilePath + "/" + fileName, OverWrite);

                        WriteLog("成功生成文件-->" + GeneratePath + "/" + appNO + "/" + fileName);
                    }
                    catch (Exception ex)
                    {
                        WriteLog("生成文件失败-->" + ex.Message);
                    }
                }

            }
            WriteLog("*************附件归档程序执行结束*************");
            WriteLog("*************日志路径:log/Overall*************");
            var dt2 = DateTime.Now;
            TimeSpan ts = dt2 - dt1;
            WriteLog("执行时间:" + ts.Hours + "小时" + ts.Minutes + "分钟" + ts.Seconds + "秒" + ts.Milliseconds + "毫秒");
            Console.ReadLine();


            //bool status = true;

            ////连接共享文件夹
            ////status = connectState(@"\\172.16.0.3\文件\IT专用\NETSOL文件备份\file-backup-52\h3\H3\2018\03", "chenghs", "chenghs123");
            //if (status)
            //{
            //    DirectoryInfo dir = new DirectoryInfo(@"\\172.16.0.3\文件\IT专用\NETSOL文件备份\file-backup-52\h3\H3\2018\03\02");

            //    string destPath = @"\\172.16.0.3\文件\IT专用\test";

            //    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            //    foreach (FileSystemInfo i in fileinfo)
            //    {
            //        if (i is DirectoryInfo)     //判断是否文件夹
            //        {
            //            if (!Directory.Exists(destPath + "\\" + i.Name))
            //            {
            //                Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
            //            }
            //            //CopyDir(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
            //        }
            //        else
            //        {
            //            File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
            //        }
            //    }
            //}




        }

        public static void WriteLog(string message)
        {
            Console.WriteLine(message);
            LogHelper.Info(message);
        }


        public static bool connectState(string path)
        {
            return connectState(path, "", "");
        }

        public static bool connectState(string path, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }


        //read file
        public static void ReadFiles(string path)
        {
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(path))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);

                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

        }

        //write file
        public static void WriteFiles(string path)
        {
            try
            {
                // Create an instance of StreamWriter to write text to a file.
                // The using statement also closes the StreamWriter.
                using (StreamWriter sw = new StreamWriter(path))
                {
                    // Add some text to the file.
                    sw.Write("This is the ");
                    sw.WriteLine("header for the file.");
                    sw.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.
                    sw.Write("The date is: ");
                    sw.WriteLine(DateTime.Now);
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }



        public static DataTable GetExcelContent(string filePath, string sheetName)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            int startRow = 0;
            DataTable data = new DataTable();
            bool isFirstRowColumn = true;
            try
            {

                workbook = WorkbookFactory.Create(filePath);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                    if (isFirstRowColumn)
                    {
                        for (int j = firstRow.FirstCellNum; j < cellCount; ++j)
                        {
                            DataColumn column = new DataColumn(firstRow.GetCell(j).StringCellValue);
                            data.Columns.Add(column);
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int j = startRow; j <= rowCount; ++j)
                    {
                        IRow row = sheet.GetRow(j);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int k = row.FirstCellNum; k < cellCount; ++k)
                        {
                            if (row.GetCell(k) != null) //同理，没有数据的单元格都默认是null
                                dataRow[k] = row.GetCell(k).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
            }
            catch
            { }
            finally
            {
                // stream.Close();
            }

            return data;
        }
    }
}
