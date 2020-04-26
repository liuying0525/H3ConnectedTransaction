using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Attachment_Aarchive_New
{
    public partial class Form1 : Form
    {
        public static string currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string apiurl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"] + string.Empty;

        public Form1()
        {
            InitializeComponent();
            InitCheckBox();
        }

        public void InitCheckBox()
        {
            groupBox1.Controls.Clear();
            int ckb_width = 230;
            var r = HttpHelper.GetWebRequest(apiurl + "?SchemaCode=APPLICATION&FieldType=24");
            DataFieldList result= Newtonsoft.Json.JsonConvert.DeserializeObject<DataFieldList>(r);
            var num = groupBox1.Width / ckb_width;
            if (result.Success)
            {
                for (int i = 0; i < result.Data.Count; i++)
                {
                    DataFieldList.data d = result.Data[i];
                    CheckBox ckb = new CheckBox();
                    ckb.Name = "ckb_" + d.Code;
                    ckb.Location = new Point(20 + ckb_width * (i % num), 30 + 30 * (i / num));
                    //MessageBox.Show(40 * (i / num) + string.Empty);
                    ckb.Text = "[" + d.Code + "]" + d.Name;
                    ckb.Tag = d.Code;
                    ckb.Width = ckb_width - 5;
                    groupBox1.Controls.Add(ckb);
                }
            }
            else
            {
                MessageBox.Show("ERROR:" + result.Msg);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> datafields = new Dictionary<string, string>();
            foreach (var control in this.groupBox1.Controls)
            {
                var ck = control as CheckBox;
                if (ck.Checked)
                {
                    datafields.Add(ck.Tag.ToString(), ck.Text.Replace('\\', '_').Replace('/', '_').Replace('|', '_').Replace('"', ' ').Replace('?', ' '));
                }
            }
            if (datafields.Count == 0)
            {
                MessageBox.Show("请勾选附件项");
                return;
            }
            if (ckb_engine.Checked)
            {
                GenerateByEngine(datafields);
            }
            else
            {
                Generate(datafields);
            }
        }

        public void Generate(Dictionary<string, string> datafields)
        {
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

            //Excel申请号文件
            string excelPath = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"] + string.Empty;
            //Excel中取数据的Sheet页
            string sheetName = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplateSheetName"] + string.Empty;
            WriteLog("Excel模板路径-->" + excelPath);
            WriteLog("文件生成的路径-->" + GeneratePath);
            dtExcel = GetExcelContent(currentPath + excelPath, sheetName);
            WriteLog("单据数据-->" + dtExcel.Rows.Count);

            foreach (DataRow row in dtExcel.Rows)
            {
                string appNO = row[0].ToString();
                if (appNO.Trim() == "")
                {
                    continue;
                }
                AttachmentService.AttachmentHeader[] atts = att.GetAttachmentContent(appNO);
                //Attachment[] atts = GetAttachmentContent(appNO).ToArray();
                WriteLog("开始处理单据-->" + appNO + ",附件数-->" + atts.Length);

                Dictionary<string, int> fileNumberInfo = new Dictionary<string, int>();

                foreach (var attachment in atts)
                {
                    try
                    {
                        //不包括当前选项
                        if (!datafields.Keys.Contains(attachment.DataField))
                            continue;
                        //保存文件
                        string FilePath = GeneratePath + "/" + appNO + "/" + datafields[attachment.DataField];

                        //决断目录是否存在,不存在则创建
                        DirectoryInfo di = new DirectoryInfo(FilePath);
                        if (!di.Exists)
                            di.Create();

                        //按文件名称进行计数
                        string key_fileName = attachment.DataField + "_" + attachment.FileName;
                        if (fileNumberInfo.Keys.Contains(key_fileName))
                            fileNumberInfo[key_fileName] = fileNumberInfo[key_fileName] + 1;
                        else
                            fileNumberInfo[key_fileName] = 1;
                        //根据同名文件的数量对文件名称进行重命名
                        string fileName = "";
                        if (fileNumberInfo[key_fileName] == 1)
                            fileName = attachment.FileName;
                        else
                            fileName = Path.GetFileNameWithoutExtension(attachment.FileName) + "_" + fileNumberInfo[key_fileName] + Path.GetExtension(attachment.FileName);
                        //文件复制
                        File.Copy(h3AttachmentFilePath + "/" + attachment.StoragePath + "/" + attachment.StorageFileName, FilePath + "/" + fileName, OverWrite);

                        WriteLog("成功生成文件-->" + FilePath + "/" + fileName);
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
            MessageBox.Show("导出结束");
        }

        public void GenerateByEngine(Dictionary<string, string> datafields)
        {
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

            //Excel申请号文件
            string excelPath = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplatePath"] + string.Empty;
            //Excel中取数据的Sheet页
            string sheetName = System.Configuration.ConfigurationManager.AppSettings["ExcelTemplateSheetName"] + string.Empty;
            WriteLog("Excel模板路径-->" + excelPath);
            WriteLog("文件生成的路径-->" + GeneratePath);
            dtExcel = GetExcelContent(currentPath + excelPath, sheetName);
            WriteLog("单据数据-->" + dtExcel.Rows.Count);

            foreach (DataRow row in dtExcel.Rows)
            {
                string appNO = row[0].ToString();
                if (appNO.Trim() == "")
                {
                    continue;
                }
                AttachmentService.AttachmentHeader[] atts = att.GetAttachmentContent(appNO);
                //Attachment[] atts = GetAttachmentContent(appNO).ToArray();
                WriteLog("开始处理单据-->" + appNO + ",附件数-->" + atts.Length);

                Dictionary<string, int> fileNumberInfo = new Dictionary<string, int>();

                foreach (var attachment in atts)
                {
                    try
                    {
                        //不包括当前选项
                        if (!datafields.Keys.Contains(attachment.DataField))
                            continue;
                        //保存文件
                        string FilePath = GeneratePath + "/" + appNO + "/" + datafields[attachment.DataField];

                        //决断目录是否存在,不存在则创建
                        DirectoryInfo di = new DirectoryInfo(FilePath);
                        if (!di.Exists)
                            di.Create();

                        //按文件名称进行计数
                        string key_fileName = attachment.DataField + "_" + attachment.FileName;
                        if (fileNumberInfo.Keys.Contains(key_fileName))
                            fileNumberInfo[key_fileName] = fileNumberInfo[key_fileName] + 1;
                        else
                            fileNumberInfo[key_fileName] = 1;
                        //根据同名文件的数量对文件名称进行重命名
                        string fileName = "";
                        if (fileNumberInfo[key_fileName] == 1)
                            fileName = attachment.FileName;
                        else
                            fileName = Path.GetFileNameWithoutExtension(attachment.FileName) + "_" + fileNumberInfo[key_fileName] + Path.GetExtension(attachment.FileName);
                        //文件路径
                        string path = FilePath + "/" + fileName;
                        
                        //保存文件到本地
                        try
                        {
                            Byte[] Content = att.GetAttachmentByteContent
                            (attachment.BizObjectSchemaCode,
                             attachment.BizObjectId,
                             attachment.ObjectID);
                            Bytes2File(Content, path);
                        }
                        catch (Exception ex)
                        {
                            WriteLog(string.Format("写入文件出错：消息={0},堆栈={1}", ex.Message, ex.StackTrace));
                        }
                        WriteLog("成功生成文件-->" + FilePath + "/" + fileName);
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
            MessageBox.Show("导出结束");
        }
        /// <summary>
        /// 将byte数组转换为文件并保存到指定地址
        /// </summary>
        /// <param name="buff">byte数组</param>
        /// <param name="savepath">保存地址</param>
        public static void Bytes2File(byte[] buff, string savepath)
        {
            if (System.IO.File.Exists(savepath))
            {
                System.IO.File.Delete(savepath);
            }

            FileStream fs = new FileStream(savepath, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(buff, 0, buff.Length);
            bw.Close();
            fs.Close();
        }
        public void WriteLog(string message)
        {
            Console.WriteLine(message);
            LogHelper.Info(message);
            tbLog.AppendText(message);
            tbLog.AppendText(Environment.NewLine);
            tbLog.ScrollToCaret();
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
            catch(Exception ex)
            { }
            finally
            {
                // stream.Close();
            }

            return data;
        }
        
        private void Form1_MaximumSizeChanged(object sender, EventArgs e)
        {
            InitCheckBox();
        }

        private void ckbAll_Click(object sender, EventArgs e)
        {
            foreach (var control in this.groupBox1.Controls)
            {
                var ck = control as CheckBox;
                ck.Checked = ckbAll.Checked;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            InitCheckBox();
        }

        private void loadplan_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.ComboBox.Items.Clear();
            using (StreamReader sr = new StreamReader(currentPath + "config.txt", Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    toolStripComboBox1.ComboBox.Items.Add(sr.ReadLine().Split('|')[0]);
                }
            }
        }

        private void saveplan_Click(object sender, EventArgs e)
        {
            string str = Interaction.InputBox("保存为常用方案", "保存", "名称", -1, -1);
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            
            var ids = "";
            foreach (var control in this.groupBox1.Controls)
            {
                var ck = control as CheckBox;
                if (ck.Checked)
                {
                    ids += ck.Tag.ToString() + ";";
                }
            }
            if (ids == "")
            {
                MessageBox.Show("没有勾选任何选项");
                return;
            }

            bool contailn_keys = false;
            using (StreamReader sr = new StreamReader(currentPath + "config.txt", Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    if (str == sr.ReadLine().Split('|')[0])
                    {
                        contailn_keys = true;
                    }
                }
            }
            if (contailn_keys)
            {
                MessageBox.Show("项[" + str + "]已存在");
                return;
            }


            //使用StreamWrite写入文本文件
            using (StreamWriter sw = new StreamWriter(currentPath + "config.txt",true))
            {
                sw.WriteLine(str + "|" + ids);
            }
            MessageBox.Show("保存成功");
        }
        
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(currentPath + "config.txt", Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    string row = sr.ReadLine();
                    if (row.Split('|')[0] == toolStripComboBox1.Text)
                    {
                        var datafields = row.Split('|')[1].Split(';');
                        foreach (var control in this.groupBox1.Controls)
                        {
                            var ck = control as CheckBox;
                            if (datafields.Contains(ck.Tag.ToString()))
                            {
                                ck.Checked = true;
                            }
                            else
                            {
                                ck.Checked = false;
                            }
                        }
                    }
                }
            }
        }
    }

    #region Class
    public class DataFieldList
    {
        public bool Success { get; set; }
        public List<data> Data { get; set; }
        public string Msg { get; set; }

        public class data
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }
    }
    #endregion
}
