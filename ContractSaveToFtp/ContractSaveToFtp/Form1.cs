using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OThinker.H3.Controllers;
using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ContractSaveToFtp
{
    public partial class Form1 : Form
    {
        public static JObject objConfig = null;

        public static readonly log4net.ILog log = log4net.LogManager.GetLogger("Logging"); //Logging 名字要在 App.config 中能找到

        public Form1()
        {
            InitializeComponent();
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "config.json";

            using (StreamReader file = File.OpenText(configPath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    objConfig = (JObject)JToken.ReadFrom(reader);

                    foreach (var j in objConfig["ContractFileType"].Children())
                    {
                        cmb_FileType.Items.Add(new ListItem() { Value = j["ID"].ToString(), Text = j["TEMPLATENAME"].ToString() });
                    }
                    cmb_FileType.Items.Insert(0, new ListItem() { Value = "", Text = "--请选择--" });
                    cmb_FileType.SelectedIndex = 0;
                }
            }

            tb_FilePath.Text = @"D:\Contract Files";
        }

        private void btn_Process_Click(object sender, EventArgs e)
        {
            int processNumber = 0;
            SetBtnEnable(false);
            #region 条件判断
            if (string.IsNullOrEmpty(tb_FilePath.Text))
            {
                SetBtnEnable(true);
                MessageBox.Show("请选择合同目录");
                return;
            }
            if (string.IsNullOrEmpty(tb_ProcessedPath.Text))
            {
                SetBtnEnable(true);
                MessageBox.Show("请选择已处理完成的文件目录");
                return;
            }
            if (cmb_FileType.SelectedIndex == 0)
            {
                SetBtnEnable(true);
                MessageBox.Show("请选择文件类型");
                return;
            }
            if (string.IsNullOrEmpty(tb_SaveToH3DataField.Text))
            {
                SetBtnEnable(true);
                MessageBox.Show("请输入保存到H3的字段编码");
                return;
            }
            if (string.IsNullOrEmpty(tb_ProcessNumber.Text))
            {
                SetBtnEnable(true);
                MessageBox.Show("请输入处理的数量");
                return;
            }
            else
            {
                if (!int.TryParse(tb_ProcessNumber.Text, out processNumber))
                {
                    SetBtnEnable(true);
                    MessageBox.Show("处理数量，请输入整数");
                    return;
                }
            }
            #endregion

            string fileType = ((ListItem)cmb_FileType.SelectedItem).Text;

            //开始处理程序；
            WriteLog("*************开始处理****************");
            DateTime dtStart = DateTime.Now;
            string path = tb_FilePath.Text;
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            int hasProcessedNum = 0;
            foreach (var file in files)
            {
                string fileName = file.Name;
                //Br-A131534000
                if (fileName.Length < 13 + 36 + 4)
                {
                    //申请单号：13位，GUID：36位，文件后缀：4位
                    WriteLog($"文件：{fileName},格式错误");
                    continue;
                }

                if (!fileName.StartsWith("Br-A"))
                {
                    //申请单号：13位，GUID：36位，文件后缀：4位
                    WriteLog($"文件：{fileName},格式错误");
                    continue;
                }

                string templateName = fileName.Substring(13, fileName.Length - 13 - 36 - 4);

                //文件类型匹配，处理
                if (templateName == fileType)
                {
                    if (UploadToFtp(file))
                    {
                        hasProcessedNum += 1;
                        WriteLog($"数量【{hasProcessedNum}】,文件：{file.Name}成功上传FTP");
                    }
                    else
                    {
                        WriteLog($"文件：{file.Name}成功上传FTP");
                    }
                }
                else
                {
                    WriteLog($"模板名称：{templateName},目标名称:{tb_FilePath.Text},不匹配");
                }

                if (hasProcessedNum == processNumber)
                {
                    break;
                }
            }
            DateTime dtEnd = DateTime.Now;
            TimeSpan timeSpan = (dtEnd - dtStart).Duration();
            WriteLog($"*************结束处理(共花费：{timeSpan.TotalSeconds}秒)****************");
            SetBtnEnable(true);
        }

        

        public bool UploadToFtp(FileInfo fileInfo)
        {
            string fileName = fileInfo.Name;

            string ContractNumber = fileName.Substring(0, 13);
            string GUID = fileName.Substring(fileName.Length - 4 - 36, 36);

            string TemplateNameID = tb_TemplateId.Text;
            string TemplateName = fileName.Substring(13, fileName.Length - 13 - 4 - 36);
            string InstanceType = tb_InstanceType.Text;

            string SchemaCode = "";
            string BizObjectID = "";

            string sql = $@"
select a.* from (
select objectid,'APPLICATION' SchemaCode from i_application app where app.application_number='{ContractNumber}'
union all
select objectid,'RetailApp' SchemaCode from i_RetailApp app where app.applicationNo='{ContractNumber}'
union all
select objectid,'CompanyApp' SchemaCode from i_CompanyApp app where app.applicationNo='{ContractNumber}'
) a 
left join ot_instancecontext con on a.objectid=con.bizobjectid
where con.state in (2,4)
";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 1)
            {
                WriteLog(ContractNumber + "存在二条记录");
                return false;
            }

            if (dt.Rows.Count == 0)
            {
                WriteLog(ContractNumber + "未找到申请单记录");
                return false;
            }
            
            BizObjectID = dt.Rows[0]["objectid"] + string.Empty;
            SchemaCode = dt.Rows[0]["SchemaCode"] + string.Empty;

            string sqlGetInstanceId = "select objectid from ot_instancecontext where bizobjectid='" + BizObjectID + "'";
            string instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetInstanceId) + string.Empty;

            if (SaveAttachment(
                fileInfo: fileInfo,
                ApplicationNumber: ContractNumber,
                TemplateId: TemplateNameID,
                TemplateName: TemplateName,
                AttachmentId: Guid.NewGuid().ToString(),
                SchemaCode: SchemaCode,
                DataField: tb_SaveToH3DataField.Text,
                BizObjectId: BizObjectID,
                InstanceType: InstanceType,
                InstanceId: instanceid
                ))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void WriteLog(string msg)
        {
            log.Info(msg);
            rtb_Log.AppendText(msg + "\r\n");
        }

        private static byte[] GetFileContent(string path)
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

        #region 保存附件信息
        /// <summary>
        /// 保存生成的文件信息
        /// </summary>
        /// <param name="FileStream">文件流</param>
        /// <param name="ApplicationNumber">申请单号</param>
        /// <param name="TemplateId">模板id</param>
        /// <param name="TemplateName">模板名称</param>
        /// <param name="AttachmentId">附件id</param>
        /// <param name="FileName">文件名称</param>
        /// <param name="SchemaCode">流程编码</param>
        /// <param name="DataField">数据项</param>
        /// <param name="BizObjectId">上级id</param>
        /// <returns></returns>
        private bool SaveAttachment(FileInfo fileInfo, string ApplicationNumber, string TemplateId, string TemplateName, string AttachmentId,
            string SchemaCode, string DataField, string BizObjectId, string InstanceType, string InstanceId)
        {
            byte[] content = GetFileContent(fileInfo.FullName);
            // 添加这个附件
            Attachment attachment = new Attachment();
            attachment.ObjectID = AttachmentId;
            attachment.Content = content;
            attachment.ContentType = MimeMapping.GetMimeMapping(fileInfo.Name);
            attachment.CreatedBy = "";
            attachment.CreatedTime = DateTime.Now;
            attachment.FileName = fileInfo.Name;
            attachment.LastVersion = true;
            attachment.ModifiedBy = null;
            attachment.ModifiedTime = DateTime.Now;
            attachment.ContentLength = content.Length;

            attachment.BizObjectSchemaCode = SchemaCode;
            attachment.BizObjectId = BizObjectId;
            attachment.DataField = DataField;
            var id = AppUtility.Engine.BizObjectManager.AddAttachment(attachment);

            string sql = @"insert into C_ContractDetail(objectid,ApplicationNumber,TemplateId,TemplateName,filename,schemacode,bizobjectid,datafield,AttachmentId,CreatedTime,intancetype,instanceid)
values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', to_date('{9}', 'yyyy-MM-dd hh24:mi:ss'),'{10}','{11}')";

            sql = string.Format(sql, Guid.NewGuid().ToString(), ApplicationNumber, TemplateId, TemplateName, fileInfo.Name, SchemaCode, BizObjectId,
                DataField, AttachmentId, fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"), InstanceType, InstanceId);
            int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            //拷贝至已处理目录
            fileInfo.CopyTo(tb_ProcessedPath.Text + "\\" + fileInfo.Name, false);
            //删除现处理的文件
            File.Delete(fileInfo.FullName);
            return n > 0;
        }
        #endregion

        private void cmb_FileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            var item = (ListItem)comboBox.SelectedItem;
            if (item.Value == "")
            {
                tb_SaveToH3DataField.Text = "";
                tb_TemplateId.Text = "";
                tb_InstanceType.Text = "";
            }
            else
            {
                var jitem = objConfig["ContractFileType"].Children().FirstOrDefault(a => a["ID"].ToString() == item.Value);

                tb_SaveToH3DataField.Text = jitem["DATAFIELD"].ToString();
                tb_TemplateId.Text = jitem["OBJECTID"].ToString();
                tb_InstanceType.Text = jitem["INSTANCETYPE"].ToString();
            }
        }

        public void SetBtnEnable(bool enable)
        {
            btn_FilePath.Enabled = enable;
            btn_Process.Enabled = enable;
            cmb_FileType.Enabled = enable;
            tb_ProcessNumber.Enabled = enable;
            btn_ProcessedPath.Enabled = enable;
        }

        private void btn_ProcessedPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tb_ProcessedPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btn_FilePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tb_FilePath.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
