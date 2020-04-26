using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Document = Aspose.Words.Document;
using PDFDocument = Aspose.Pdf.Document;
using Image = System.Drawing.Image;
using SaveFormat = Aspose.Words.SaveFormat;
using OThinker.H3.DataModel;
using System.IO;
using Aspose.Words;
using OThinker.H3.Data;
using Aspose.Words.Saving;
using OThinker.H3.Controllers;
using System.Linq;

namespace DongZheng.H3.WebApi.Common.Portal
{
    public class TemplatePrintClass : MvcPage
    {
        public TemplatePrintClass()
        { }

        #region 前端JS请求的方法
        public object SetWordFile(string TemplateNameID, string ContractNumber)
        {
            string GUID = Guid.NewGuid().ToString();
            var flag = true;
            var message = "";
            var TemplateName = "";
            var instanceType = "";
            var SavedDataField = "";
            string fileName = "";

            double rate_fixed = 0;
            double rate_base = 0;
            double rate_float = 0;

            bool isNewContract = false;

            try
            {
                //获取sql语句、模版名称
                string SQL = "SELECT TemplateName,sqlstr,DataBaseUrl,instancetype,SavedDataField FROM  I_TemplateFile WHERE ObjectID ='" + TemplateNameID + "'";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(SQL);
                //模版名称
                TemplateName = dt.Rows[0]["TemplateName"] + string.Empty;
                instanceType = dt.Rows[0]["instancetype"] + string.Empty;
                SavedDataField = dt.Rows[0]["SavedDataField"] + string.Empty;
                string loanContractField = ConfigurationManager.AppSettings["LoanContractSavedDataField"] + string.Empty;

                if (string.IsNullOrEmpty(SavedDataField))
                {
                    return new
                    {
                        Flag = false,
                        FileName = "",
                        Id = "",
                        Message = "未设置保存的数据项编码，请联系管理员"
                    };
                }

                #region 处理打印合同
                // 打印合同
                if (loanContractField.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Contains(SavedDataField))
                {
                    string sql = $"SELECT ACTUAL_RTE FROM APPLICATION_CONTRACT_VW WHERE APPLICATION_NUMBER = '{ContractNumber}'";
                    decimal actualRte = Convert.ToDecimal((ExecuteDataTableSql("cap", sql).Rows[0]["ACTUAL_RTE"] + string.Empty));
                    sql = $"SELECT STATUS_CODE FROM APPLICATION@TO_CMS WHERE APPLICATION_NUMBER = '{ContractNumber}'";
                    string statusCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;

                    if (statusCode == "55")
                    {
                        // 已放款状态，取上一次的打印记录
                        sql = $@"SELECT * FROM (
                            SELECT TEMPLATENAME, RATE_FIXED, RATE_BASE, RATE_FLOAT  FROM C_CONTRACTDETAIL
                            WHERE APPLICATIONNUMBER = '{ContractNumber}'
                            ORDER BY CREATEDTIME DESC)
                            WHERE ROWNUM = 1";
                        DataTable recordTable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                        if (recordTable.Rows.Count == 0)
                        {
                            // 没有打印记录，返回false
                            return new
                            {
                                Flag = false,
                                Message = "没有找到打印记录"
                            };
                        }
                        else
                        {
                            TemplateName = recordTable.Rows[0]["TEMPLATENAME"] + string.Empty;
                            string oldUserTemplateName = ConfigurationManager.AppSettings["OldUserLoanContractName"] + string.Empty;
                            string oldOrgTemplateName = ConfigurationManager.AppSettings["OldOrgLoanContractName"] + string.Empty;
                            
                            // 有打印记录
                            // 个人模板并且模板名称不等于维护的老个人模板名称
                            // 机构模板并且模板名称不等于维护的老机构模板名称
                            if ((instanceType == "个人" && TemplateName != oldUserTemplateName)
                                || (instanceType == "机构" && TemplateName != oldOrgTemplateName))
                            {
                                isNewContract = true;
                                rate_fixed = Convert.ToDouble(recordTable.Rows[0]["RATE_FIXED"]);
                                rate_base = Convert.ToDouble(recordTable.Rows[0]["RATE_BASE"]);
                                rate_float = Convert.ToDouble(recordTable.Rows[0]["RATE_FLOAT"]);
                            }
                        }
                    }
                    else
                    {
                        //未放款状态，打印新合同
                        string rateSql = $@"
                        SELECT {actualRte} RATE_FIXED, RATEBASE RATE_BASE, {actualRte} - RATEBASE AS RATE_FLOAT FROM (
                          SELECT HTLL.RATEBASE FROM I_HTLL2 HTLL
                          INNER JOIN (
                            SELECT
                              CASE
                                WHEN LEASE_TERMS/12 > 1 THEN '5年'
                                ELSE '1年'
                              END LEASE_TERMS, createdtime FROM 
                              (select a.createdtime, b.lease_term_in_month as lease_terms
                               from i_application a
                               inner join i_Contract_Detail b on b.parentobjectid = a.objectid
                               where a.application_number = '{ContractNumber}'
                              )
                            ) CONTRACT
                            ON HTLL.DURATION = CONTRACT.LEASE_TERMS and to_date('{DateTime.Now.ToShortDateString()}', 'yyyy/MM/dd') >= htll.enabledtime
                            WHERE HTLL.ISDELETE = 0
                          ORDER BY HTLL.ENABLEDTIME DESC
                        ) ";
                        DataTable rateTable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(rateSql);

                        // 没有维护LPR，就不让打印
                        if (rateTable.Rows.Count == 0)
                        {
                            return new
                            {
                                Flag = false,
                                Message = "没有找到对应LPR记录,请维护"
                            };
                        }
                        else
                        {
                            isNewContract = true;
                            rate_fixed = Convert.ToDouble(rateTable.Rows[0]["RATE_FIXED"]);
                            rate_base = Convert.ToDouble(rateTable.Rows[0]["RATE_BASE"]);
                            rate_float = Convert.ToDouble(rateTable.Rows[0]["RATE_FLOAT"]);
                        }
                    }

                }
                #endregion
                
                var datasource = dt.Rows[0]["DataBaseUrl"] + string.Empty;
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = "cap";
                }

                string SQL1 = "SELECT sqlstring FROM  I_TemplateFile_SQL WHERE ParentObjectID ='" + TemplateNameID + "'";
                DataTable dtstr = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(SQL1);

                //模版地址
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path += "Sheets\\Model\\" + TemplateName + ".doc";
                var document = new Document(path);
                var builder = new DocumentBuilder(document);

                //查模版维护表获取标签值 
                for (int i = 0; i < dtstr.Rows.Count; i++)
                {
                    string sqlstr = dtstr.Rows[i]["sqlstring"].ToString();
                    sqlstr = HttpUtility.HtmlDecode(sqlstr); // wangxg 19.12 HTML解码
                    string sql1 = string.Format(sqlstr, ContractNumber.Trim());// "Br-A000056000"); 
                    DataTable dataSource = ExecuteDataTableSql(datasource, sql1);
                    //记录打印查询出的数据源；
                    AppUtility.Engine.LogWriter.Write(ContractNumber + TemplateName + GUID + "数据源" + (i + 1) + "\r\n" +
                        Newtonsoft.Json.JsonConvert.SerializeObject(dataSource));
                    if (dataSource.Rows.Count > 0)
                    {
                        //确定为还款计划表
                        if (dataSource.Columns[0].ColumnName == "CUSTOMER_RENTAL_ID")
                        {
                            NodeCollection allTables = document.GetChildNodes(NodeType.Table, true); //拿到所有表格
                            Aspose.Words.Tables.Table table = allTables[2] as Aspose.Words.Tables.Table; //拿到第二个表格

                            for (int l = 0; l < dataSource.Rows.Count; l++)
                            {
                                // 复制上一行
                                Aspose.Words.Tables.Row clonedRow = (Aspose.Words.Tables.Row)table.LastRow.Clone(true);
                                for (int j = 0; j < clonedRow.Count; j++)
                                {
                                    clonedRow.Cells[j].RemoveAllChildren(); //清除单元格中内容 
                                    Aspose.Words.Paragraph p = new Aspose.Words.Paragraph(document);
                                    var CellVal = dataSource.Rows[l][j] + string.Empty;
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
                            foreach (DataColumn dc in dataSource.Columns)
                            {
                                if (document.Range.Bookmarks[dc.ColumnName] != null)
                                {
                                    if (instanceType.ToString() == "正源" && dc.DataType.ToString() == "System.DateTime")
                                    {
                                        document.Range.Bookmarks[dc.ColumnName].Text = Convert.ToDateTime(dataSource.Rows[0][dc.ColumnName].ToString()).ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        document.Range.Bookmarks[dc.ColumnName].Text = dataSource.Rows[0][dc.ColumnName].ToString();
                                    }
                                }
                            }
                        }
                    }

                }

                // 如果是新合同，给rate_fixed, rate_base, rate_float三个书签赋值
                if (isNewContract)
                {
                    document.Range.Bookmarks["rate_fixed"].Text = rate_fixed.ToString();
                    document.Range.Bookmarks["rate_base"].Text = rate_base.ToString();
                    document.Range.Bookmarks["rate_float"].Text = rate_float.ToString();
                }

                fileName = ContractNumber + TemplateName + GUID;

                #region Save
                string tempfilePath = Server.MapPath("~/TempImages/") + GUID + ".doc";
                document.Save(tempfilePath, SaveFormat.Doc);
                SaveAttachment(
                    FilePath: tempfilePath,
                    ApplicationNumber: ContractNumber,
                    TemplateId: TemplateNameID,
                    TemplateName: TemplateName,
                    AttachmentId: GUID,
                    FileName: fileName + ".doc",
                    SchemaCode: ActionContext.SchemaCode,
                    DataField: SavedDataField,
                    BizObjectId: ActionContext.BizObjectID,
                    InstanceType: instanceType,
                    InstanceId: ActionContext.InstanceId,
                    Rate_Fixed: rate_fixed,
                    Rate_Base: rate_base,
                    Rate_Float: rate_float
                    );

                if (instanceType.ToString() == "正源")
                {
                    string tempfilePathPDF = Server.MapPath("~/TempImages/") + GUID + ".pdf";
                    document.Save(tempfilePathPDF, SaveFormat.Pdf);
                    SaveAttachment(
                        FilePath: tempfilePathPDF,
                        ApplicationNumber: ContractNumber,
                        TemplateId: TemplateNameID,
                        TemplateName: TemplateName,
                        AttachmentId: Guid.NewGuid().ToString(),
                        FileName: fileName + ".pdf",
                        SchemaCode: ActionContext.SchemaCode,
                        DataField: SavedDataField,
                        BizObjectId: ActionContext.BizObjectID,
                        InstanceType: instanceType,
                        InstanceId: ActionContext.InstanceId,
                        Rate_Fixed: rate_fixed,
                        Rate_Base: rate_base,
                        Rate_Float: rate_float
                        );
                }
                #endregion
            }
            catch (Exception e)
            {
                AppUtility.Engine.LogWriter.Write(e.ToString());
                flag = false;
                message = e.Message;
            }

            return new
            {
                Flag = flag,
                FileName = fileName,
                Id = GUID,
                Message = message
            };
        }


        public object GetExistAttachmentID(string TemplateID, string ContractNumber)
        {
            string sql = @"select * from C_ContractDetail where ApplicationNumber='{0}' and TemplateId='{1}' and bizobjectid='{2}' order by CreatedTime desc";
            sql = string.Format(sql, ContractNumber, TemplateID, ActionContext.BizObjectID);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            string id = "";
            if (dt.Rows.Count > 0)
                id = dt.Rows[0]["AttachmentId"] + string.Empty;
            return new { Success = true, AttachmentID = id };
        }

        public object Getfiledata(string Type, bool isView, string applicationNumber)
        {
            var orderStatusSql = "select * from application@to_cms where application_number='" + applicationNumber + "'";
            var statusDt = ExecuteDataTableSql("CAPDB", orderStatusSql);

            var cmsStatus = string.Empty;

            if (statusDt != null)
            {
                cmsStatus = statusDt.Rows[0]["status_code"] + string.Empty;
            }

            string sql = string.Format("select OBJECTID, TEMPLATENAME from I_TemplateFile  where INSTANCETYPE ='{0}'", Type);

            if (isView)
            {
                sql = string.Format("select OBJECTID, TEMPLATENAME from I_TemplateFile  where INSTANCETYPE ='{0}' and ViewIsPrint = 1 ", Type);

            }

            if (!string.IsNullOrEmpty(cmsStatus))
            {
                sql = sql + " AND visablestate like '%" + cmsStatus + "%'";
            }

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return dt;

        }

        public string GetStatusCode(string applicationNumber)
        {
            var statusSql = $"select status_code from application@to_cms where application_number='{applicationNumber}'";
            var statusDt = ExecuteDataTableSql("CAPDB", statusSql);
            var cmsStatus = string.Empty;

            if (statusDt != null)
            {
                cmsStatus = statusDt.Rows[0]["status_code"] + string.Empty;
            }

            return cmsStatus;
        }

        public bool isAdmin(string UserID)
        {
            return AppUtility.Engine.Organization.IsAdministrator(UserID);
        }

        public bool IsFinished(string intanceid)
        {
            WorkFlowFunction WorkFlow = new WorkFlowFunction();
            return WorkFlow.GetIsFinished(intanceid);
        }

        #endregion

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
        private bool SaveAttachment(string FilePath, string ApplicationNumber, string TemplateId, string TemplateName, string AttachmentId,
            string FileName, string SchemaCode, string DataField, string BizObjectId, string InstanceType, string InstanceId,
            double Rate_Fixed, double Rate_Base, double Rate_Float)
        {
            byte[] content = GetFileContent(FilePath);
            // 添加这个附件
            Attachment attachment = new Attachment();
            attachment.ObjectID = AttachmentId;
            attachment.Content = content;
            attachment.ContentType = MimeMapping.GetMimeMapping(FileName);
            attachment.CreatedBy = "";
            attachment.CreatedTime = DateTime.Now;
            attachment.FileName = FileName;
            attachment.LastVersion = true;
            attachment.ModifiedBy = null;
            attachment.ModifiedTime = System.DateTime.Now;
            attachment.ContentLength = content.Length;

            attachment.BizObjectSchemaCode = SchemaCode;
            attachment.BizObjectId = BizObjectId;
            attachment.DataField = DataField;
            var id = AppUtility.Engine.BizObjectManager.AddAttachment(attachment);

            //删除临时文件
            File.Delete(FilePath);

            string sql = @"insert into C_ContractDetail(objectid,ApplicationNumber,TemplateId,TemplateName,filename,schemacode,bizobjectid,datafield,AttachmentId,CreatedTime,intancetype,instanceid,rate_fixed,rate_base,rate_float)
values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', to_date('{9}', 'yyyy-MM-dd hh24:mi:ss'),'{10}','{11}',{12},{13},{14})";

            sql = string.Format(sql, Guid.NewGuid().ToString(), ApplicationNumber, TemplateId, TemplateName, FileName, SchemaCode, BizObjectId,
                DataField, AttachmentId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), InstanceType, InstanceId, Rate_Fixed, Rate_Base, Rate_Float);
            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql) > 0;
        }
        #endregion

        #region Basic Method
        //文件转为二进制
        private static byte[] GetFileContent(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        //文件转为二进制
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


        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
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
            var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteScalar(sql);
            }
            return null;
        }
        #endregion
    }
}