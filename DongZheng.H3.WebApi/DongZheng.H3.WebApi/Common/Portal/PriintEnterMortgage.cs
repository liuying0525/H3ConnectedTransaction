using Aspose.Words;
using System;
using System.Configuration;
using System.Data;
using System.Web;
using Document = Aspose.Words.Document;
using SaveFormat = Aspose.Words.SaveFormat;



namespace DongZheng.H3.WebApi.Common.Portal
{

    /// <summary>
    /// TemplateChangeClass 的摘要说明
    /// </summary>
    public class PriintEnterMortgage : OThinker.H3.Controllers.MvcPage
    {
        public PriintEnterMortgage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        public object SetWordFile(string TemplateNameID, string ContractNumber, string fileName)
        {
            string GUID = System.Guid.NewGuid().ToString();
            var flag = true;
            try
            {
                //获取sql语句、模版名称
                string SQL = "SELECT TemplateName,sqlstr,DataBaseUrl FROM  I_TemplateFile WHERE ObjectID ='" + TemplateNameID + "'";
                DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(SQL);
                //模版名称
                var TemplateName = dt.Rows[0]["TemplateName"];
                var datasource = dt.Rows[0]["DataBaseUrl"].ToString();
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = "cap";
                }

                string SQL1 = "SELECT sqlstring FROM  I_TemplateFile_SQL WHERE ParentObjectID ='" + TemplateNameID + "'";
                DataTable dtstr = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(SQL1);

                //模版地址
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                path += "Sheets\\Model\\" + TemplateName + ".doc";
                var document = new Document(path);
                var builder = new DocumentBuilder(document);
                //查模版维护表获取标签值 
                for (int i = 0; i < dtstr.Rows.Count; i++)
                {
                    string sqlstr = dtstr.Rows[i]["sqlstring"].ToString();
                    sqlstr = HttpUtility.HtmlDecode(sqlstr); // wangxg 19.12 HTML解码
                    string sql1 = string.Format(sqlstr, ContractNumber.Trim());// "Br-A000056000"); 
                    DataTable dt1 = ExecuteDataTableSql(datasource, sql1);
                    if (dt1.Rows.Count > 0)
                    {
                        //确定为还款计划表
                        if (dt1.Columns[0].ColumnName == "CUSTOMER_RENTAL_ID")
                        {
                            NodeCollection allTables = document.GetChildNodes(NodeType.Table, true); //拿到所有表格
                            Aspose.Words.Tables.Table table = allTables[2] as Aspose.Words.Tables.Table; //拿到第二个表格

                            for (int l = 0; l < dt1.Rows.Count; l++)
                            {
                                // 复制上一行
                                Aspose.Words.Tables.Row clonedRow = (Aspose.Words.Tables.Row)table.LastRow.Clone(true);
                                for (int j = 0; j < clonedRow.Count; j++)
                                {
                                    clonedRow.Cells[j].RemoveAllChildren(); //清除单元格中内容 
                                    Aspose.Words.Paragraph p = new Aspose.Words.Paragraph(document);
                                    var CellVal = dt1.Rows[l][j] + string.Empty;
                                    CellVal = CellVal.Replace("/", "-").Replace("0:00:00", "");
                                    p.AppendChild(new Aspose.Words.Run(document, CellVal));
                                    clonedRow.Cells[j].AppendChild(p);
                                }
                                table.Rows.Add(clonedRow); //添加一行
                            }
                        }
                        else
                        {
                            //取标签 给标签赋值
                            foreach (DataColumn dc in dt1.Columns)
                            {
                                if (document.Range.Bookmarks[dc.ColumnName] != null)
                                {
                                    document.Range.Bookmarks[dc.ColumnName].Text = dt1.Rows[0][dc.ColumnName].ToString();
                                }
                            }
                        }


                    }

                }
                //文件名称//存在则覆盖，采用原来fileName
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = ContractNumber + TemplateName + GUID;
                }

                //测试  判断是否重新生成
                //else
                //{
                //    string filePath = Server.MapPath("~/TemplateFile/") + fileName + ".pdf";
                //    document.Save(filePath, SaveFormat.Pdf);
                //}

                //string filePathDocold = Server.MapPath("~/TemplateFile/Doc/") + fileName + ".DOC"; 
                //string filePathDoc = System.AppDomain.CurrentDomain.BaseDirectory + "\\PrintFilePath\\" + fileName + ".doc";
                string filePathDoc = GetfilePath() + fileName + ".DOC";
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("filePathDoc=" + filePathDoc);
                //PdfSaveOptions saveOption = new PdfSaveOptions();
                //saveOption.SaveFormat = Aspose.Words.SaveFormat.Pdf;
                ////user pass 设置了打开时，需要密码
                ////owner pass 控件编辑等权限  
                //PdfEncryptionDetails encryptionDetails = new PdfEncryptionDetails(string.Empty, "PasswordHere", PdfEncryptionAlgorithm.RC4_128);
                //encryptionDetails.Permissions = PdfPermissions.DisallowAll;
                //saveOption.EncryptionDetails = encryptionDetails;

                //加密文档保存到指定地点---//使用ntko不需要使用pdf暂时注释
                // document.Save(filePath, saveOption);
                //如不不需要保存doc 请注释下句
                document.Save(filePathDoc, SaveFormat.Doc);

            }
            catch (Exception e)
            {
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(e.ToString());
                flag = false;
            }

            return new
            {
                Flag = flag,
                FileName = fileName
            };
        }

        // add 527
        public string GetfilePath()
        {
            return ConfigurationManager.AppSettings["PrintPath"];
        }

        // add 527
        public object Getfiledata(string Type, bool isView)
        {

            string sql = string.Format("SELECT OBJECTID, TEMPLATENAME FROM I_TemplateFile  WHERE INSTANCETYPE ='{0}'", Type);
            if (isView)
            {
                sql = string.Format("SELECT OBJECTID, TEMPLATENAME FROM I_TemplateFile  WHERE INSTANCETYPE ='{0}' AND ViewIsPrint = 1 ", Type);

            }
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return dt;

        }
        //add 527
        public bool isAdmin(string UserID)
        {
            return OThinker.H3.Controllers.AppUtility.Engine.Organization.IsAdministrator(UserID);
        }

        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }

        public object ExecuteScalarSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteScalar(sql);
            }
            return null;
        }

        public bool IsFinished(string intanceid)
        {
            WorkFlowFunction WorkFlow = new WorkFlowFunction();
            return WorkFlow.GetIsFinished(intanceid);
        }
    }
}