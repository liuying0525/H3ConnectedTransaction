using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Collections;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.Controllers.Reporting;
using OThinker.Reporting;
using OThinker.H3.DataModel;
using OThinker.H3.Data;
using System.Xml;
using OThinker.Data;
using OThinker.H3.Controllers.ViewModels;
using System.IO;
using Newtonsoft.Json;

namespace OThinker.H3.Controllers.Reporting
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ReportController : ReportBase
    {

        private string ReportCode = string.Empty;//用于报表上传xml时，存储数据
        private string ReportName = string.Empty;//报表页名称
        FunctionNode reportnode;//报表节点
        OThinker.Reporting.ReportPage reportpage;//报表模型
        private OThinker.H3.Acl.SystemAcl DataAclScope = new OThinker.H3.Acl.SystemAcl();//系统权限控制单元

        #region 公共方法
        /// <summary>
        /// 异常请求响应信息
        /// </summary>
        /// <param name="errorMsg"></param>
        public void ReponseError(string errorMsg)
        {
            this.ViewBag.IsDelete = true;
            this.Response.Write("<script>");
            this.Response.Write(" window.parent.$.IShowError('" + errorMsg.Replace("\r\n", "\\r\\n") + "');");
            this.Response.Write(" window.parent.$.ISideModal.CloseLastModal();");
            this.Response.Write("</script>");
            this.Response.End();
        }

        /// <summary>
        /// 构造一个报表页对象
        /// </summary>
        /// <param name="NewReportPage"></param>
        /// <returns></returns>
        public ReportPage NewReportPages(ReportPage NewReportPage)
        {

            if (NewReportPage != null)
            {
                if (NewReportPage.ReportSources != null)
                {
                    for (int i = 0; i < NewReportPage.ReportSources.Length; i++)
                    {
                        NewReportPage.ReportSources[i].ParentObjectID = NewReportPage.ObjectID;
                        NewReportPage.ReportSources[i].ParentPropertyName = "ReportSources";
                        NewReportPage.ReportSources[i].ParentIndex = i;
                        NewReportPage.ReportSources[i].CreatedTime = DateTime.Now;
                    }
                }

                if (NewReportPage.ReportWidgets != null)
                {
                    for (int i = 0; i < NewReportPage.ReportWidgets.Length; i++)
                    {
                        NewReportPage.ReportWidgets[i].ParentObjectID = NewReportPage.ObjectID;
                        NewReportPage.ReportWidgets[i].ParentPropertyName = "ReportWidgets";
                        NewReportPage.ReportWidgets[i].ParentIndex = i;
                        NewReportPage.ReportWidgets[i].CreatedTime = DateTime.Now;
                        //原字段
                        if (NewReportPage.ReportWidgets[i].Columns != null)
                        {
                            for (int j = 0; j < NewReportPage.ReportWidgets[i].Columns.Length; j++)
                            {
                                NewReportPage.ReportWidgets[i].Columns[j].ParentObjectID = NewReportPage.ReportWidgets[i].ObjectID;
                                NewReportPage.ReportWidgets[i].Columns[j].ParentPropertyName = "Columns";
                                NewReportPage.ReportWidgets[i].Columns[j].ParentIndex = j;

                            }
                        }
                        //系列
                        if (NewReportPage.ReportWidgets[i].Series != null)
                        {
                            for (int j = 0; j < NewReportPage.ReportWidgets[i].Series.Length; j++)
                            {
                                NewReportPage.ReportWidgets[i].Series[j].ParentObjectID = NewReportPage.ReportWidgets[i].ObjectID;
                                NewReportPage.ReportWidgets[i].Series[j].ParentPropertyName = "Series";
                                NewReportPage.ReportWidgets[i].Series[j].ParentIndex = j;
                            }
                        }
                        //分类
                        if (NewReportPage.ReportWidgets[i].Categories != null)
                        {
                            for (int j = 0; j < NewReportPage.ReportWidgets[i].Categories.Length; j++)
                            {
                                NewReportPage.ReportWidgets[i].Categories[j].ParentObjectID = NewReportPage.ReportWidgets[i].ObjectID;
                                NewReportPage.ReportWidgets[i].Categories[j].ParentPropertyName = "Categories";
                                NewReportPage.ReportWidgets[i].Categories[j].ParentIndex = j;
                            }
                        }
                    }
                }
            }

            return NewReportPage;
        }

        #endregion


        #region 报表模型的数据导入导出
        //声明xml树节点对象
        XmlDocument XmlDoc = new XmlDocument();
        //根据字段名称创建xml树节点
        private XmlElement CreateXmlElement(string name, string value)
        {
            XmlElement element = XmlDoc.CreateElement(name);
            element.InnerText = value;
            return element;
        }
        /// <summary>
        /// 导出报表页
        /// </summary>
        /// <param name="ReportCode">报表页编号</param>
        /// <returns></returns>
        public object Export(string ReportCode)
        {
            return ExecuteFileResultFunctionRun(() =>
            {
                //报表页菜单对象
                Acl.FunctionNode ReportNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(ReportCode);
                //报表页节点
                XmlElement ReportPage = XmlDoc.CreateElement(FunctionNodeType.ReportFolderPage.ToString());

                ReportPage.AppendChild(CreateXmlElement("ReportCode", ReportNode.Code));
                ReportPage.AppendChild(CreateXmlElement("ReportName", ReportNode.DisplayName));
                XmlDoc.AppendChild(ReportPage);
                ReportPage.InnerXml += Convertor.ObjectToXml(ReportNode);
                //获取报表页、报表数据源、报表配置页对象
                OThinker.Reporting.ReportPage report = this.Engine.ReportManager.GetReportPage(ReportCode);
                if (report != null)
                {
                    ReportPage.InnerXml += Convertor.ObjectToXml(report);
                }
                //导出文件
                string path = Server.MapPath("~/TempImages/");
                string fileName = ReportCode + ".xml";

                XmlDoc.Save(path + fileName);

                return File(path + fileName, "application/octect-stream", fileName);
            });
        }


        /// <summary>
        /// 导入报表模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Import(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportImportViewModel model = new ReportImportViewModel();
                model = JsonConvert.DeserializeObject<ReportImportViewModel>(formData);

                ReadXmlFile(model);
                ActionResult result = new ActionResult(false, "");
                if (reportnode == null)
                {
                    result.Message = "WorkflowPackageImport.PackageImportHandler_Mssg1";

                    return Json(result);
                }
                string resultStr = string.Empty;
                bool result2 = false;
                bool isCover = model.IsCover;
                #region 如果覆盖，先检查当前报表页code是否存在，如果存在则跟新，否则就新增
                if (isCover)
                {
                    if (model.ReportsPackage != null)
                    {
                        ReportPage ReportPages = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(model.ReportsPackage.Code));
                        //说明存在报表页，如果存在报表页，则跟新报表页中的相应数据
                        if (ReportPages.Code != null)
                        {   //更新
                            ReportPages.ParentObjectID = reportpage.ParentObjectID;
                            ReportPages.ParentPropertyName = reportpage.ParentPropertyName;
                            ReportPages.ParentIndex = reportpage.ParentIndex;
                            ReportPages.Layout = reportpage.Layout;
                            ReportPages.ModifiedBy = reportpage.ModifiedBy;
                            ReportPages.ModifiedTime = reportpage.ModifiedTime;
                            ReportPages.ReportSources = reportpage.ReportSources;
                            ReportPages.ReportWidgets = reportpage.ReportWidgets;
                            ReportPages.Filters = reportpage.Filters;
                            ReportPages.DisplayName = reportpage.DisplayName;
                            //ReportPages.Serialized = reportpage.Serialized;  无需此句,会导致旧的报表无法更新覆盖

                            //此方法会自动判断，有则更新，无则新增
                            result2 = this.Engine.ReportManager.UpdateReportPage(ReportPages);
                            if (result2)
                            {
                                this.Engine.FunctionAclManager.UpdateFunctionNode(reportnode);//更新功能节点
                            }
                            else
                            {
                                resultStr = "Reporting.ImporterrorMessage2";
                            }

                        }
                        else
                        {   //新增
                            //reportpage 为报表页对象
                            reportpage = NewReportPage(reportpage);//初始化数据关联关系ID
                            reportpage.Code = model.ReportsPackage.Code;
                            reportpage.DisplayName = model.ReportsPackage.Name;
                            reportpage.CreatedTime = DateTime.Now;
                            reportpage.Creator = this.UserValidator.UserID;

                            result2 = this.Engine.ReportManager.UpdateReportPage(reportpage);
                            if (result2)
                            {
                                reportnode.ObjectID = Guid.NewGuid().ToString();
                                reportnode.Serialized = false;
                                this.Engine.FunctionAclManager.AddFunctionNode(reportnode);//新增报表页功能节点
                            }
                            else
                            {
                                resultStr = "Reporting.ImporterrorMessage2";
                            }

                        }
                    }

                }
                //否则不覆盖时，检验是否存在，如果存在则不允许新增，只有修改了报表页编号以后才允许新增
                else
                {
                    ReportPage ReportPages = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(model.ReportsPackage.Code));
                    //说明存在报表页，如果存在报表页，则跟新报表页中的相应数据
                    if (ReportPages.Code != null)
                    {
                        result2 = false;
                        resultStr = "Reporting.ImporterrorMessage1";
                    }
                    else
                    {
                        reportpage = NewReportPage(reportpage);//初始化数据关联关系ID
                        reportpage.Code = model.ReportsPackage.Code;
                        reportpage.DisplayName = model.ReportsPackage.Name;
                        reportpage.CreatedTime = DateTime.Now;
                        reportpage.Creator = this.UserValidator.UserID;
                        result2 = this.Engine.ReportManager.UpdateReportPage(reportpage);
                        if (result2)
                        {
                            reportnode.Serialized = false;
                            reportnode.ObjectID = Guid.NewGuid().ToString();
                            this.Engine.FunctionAclManager.AddFunctionNode(reportnode);//新增报表页功能节点
                        }
                        else
                        {

                            resultStr = "Reporting.ImporterrorMessage2";
                        }
                    }
                }

                #endregion
                if (result2)
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.ImportSucceed";
                }
                else
                {
                    result.Message = resultStr;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        ///上传报表页
        /// </summary>
        /// <param name="ParentCode"></param>
        public JsonResult Upload(string ParentCode)
        {
            return ExecuteFunctionRun(() =>
            {

                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;//传输的文件
                ActionResult result = new ActionResult(false, "");
                ReportImportViewModel model = new ReportImportViewModel();

                if (files == null || files.Count == 0 || string.IsNullOrEmpty(files[0].FileName))
                {
                    result.Message = "WorkflowPackageImport.MasterData_Mssg10";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                string fileType = Path.GetExtension(TrimHtml(Path.GetFileName(files[0].FileName))).ToLowerInvariant();
                if (!fileType.Replace(".", "").Equals("xml"))
                {
                    result.Message = "WorkflowPackageImport.MasterData_Mssg11";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                int FileLen = files[0].ContentLength;
                byte[] input = new byte[FileLen];
                System.IO.Stream UpLoadStream = files[0].InputStream;

                UpLoadStream.Read(input, 0, FileLen);
                UpLoadStream.Position = 0;
                System.IO.StreamReader sr = new System.IO.StreamReader(UpLoadStream);

                string xmlStr = sr.ReadToEnd();
                sr.Close();

                //保存到服务器上
                string newName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + fileType;

                //验证是否为伪造的xml文件
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);
                }
                catch (Exception ex)
                {
                    result.Message = "WorkflowPackageImport.MasterData_Mssg11";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                model.ParentCode = ParentCode;

                model.XMLString = Guid.NewGuid().ToString();//Session Name

                Session[model.XMLString] = xmlStr;

                try
                {
                    ReadXmlFile(model);
                }
                catch
                {
                    result.Message = "WorkflowPackageImport.MasterData_Mssg11";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                //oldCOde 存储导入时修改前的编码
                model.ReportsPackage = new ReportsItemDetail { CodeType = FunctionNodeType.ReportFolderPage.ToString(), Name = ReportName, Code = ReportCode, OldCode = ReportCode, Index = 1 };
                result.Success = true;
                result.Message = "";
                result.Extend = model;
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                //BuildInterface();
            });

        }

        /// <summary>
        /// 读取导入的XML文件
        /// </summary>
        /// <param name="mapPath"></param>
        private void ReadXmlFile(ReportImportViewModel model)
        {
            //从服务器加载
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Session[model.XMLString].ToString());
            XmlElement BizWorkflowPackage = xmlDoc.DocumentElement;//根节点

            //报表编码、名称
            ReportCode = BizWorkflowPackage.GetElementsByTagName("ReportCode")[0].InnerXml;
            ReportName = BizWorkflowPackage.GetElementsByTagName("ReportName")[0].InnerXml;
            //报表节点菜单
            XmlNodeList packageNodes = BizWorkflowPackage.GetElementsByTagName("FunctionNode");
            if (packageNodes != null && packageNodes.Count > 0)
            {
                XmlNode openNewWindow = packageNodes[0].SelectSingleNode("OpenNewWindow");
                if (openNewWindow != null) openNewWindow.InnerText = openNewWindow.InnerText.ToLower();
                reportnode = Convertor.XmlToObject(typeof(FunctionNode), packageNodes[0].OuterXml) as FunctionNode;
                reportnode.ParentCode = model.ParentCode;
                if (model.ReportsPackage != null)
                {
                    reportnode.Code = model.ReportsPackage.Code;
                    reportnode.DisplayName = model.ReportsPackage.Name;
                    reportnode.Url = "Reporting/IndexInfo.html&RportCode=" + model.ReportsPackage.Code;
                }


            }

            //报表模型
            XmlNodeList ReportPageSchemaNodes = BizWorkflowPackage.GetElementsByTagName("ReportPage");
            if (ReportPageSchemaNodes != null && ReportPageSchemaNodes.Count > 0)
            {
                reportpage = Convertor.XmlToObject(typeof(ReportPage), ReportPageSchemaNodes[0].OuterXml) as ReportPage;

            }


        }

        /// <summary>
        /// 设置导入新增报表页签时，初始化数据
        /// </summary>
        /// <param name="reportpage"></param>
        /// <returns></returns>
        public ReportPage NewReportPage(ReportPage reportpage)
        {
            reportpage.ObjectID = Guid.NewGuid().ToString();
            if (reportpage.Filters != null)
            {
                for (var i = 0; i < reportpage.Filters.Length; i++)
                {
                    reportpage.Filters[i].ObjectID = Guid.NewGuid().ToString();
                    reportpage.Filters[i].ParentObjectID = reportpage.ObjectID;
                    reportpage.Filters[i].ModifiedTime = DateTime.Now;
                    reportpage.Filters[i].ModifiedBy = this.UserValidator.UserID;

                }
            }
            if (reportpage.ReportSources != null)
            {
                //先记住旧的 数据源objectID,用于匹配数据源与报表页签之间关系的ID
                string[] oldsourcesid = new string[reportpage.ReportSources.Length];
                //更新数据源ID
                for (var i = 0; i < reportpage.ReportSources.Length; i++)
                {
                    oldsourcesid[i] = reportpage.ReportSources[i].ObjectID;
                    reportpage.ReportSources[i].ObjectID = Guid.NewGuid().ToString();

                    //设置数据源主表列的关联关系值
                    if (reportpage.ReportSources[i].SqlColumns!=null)
                    {
                        for (int j = 0; j < reportpage.ReportSources[i].SqlColumns.Length; j++)
                        {
                            reportpage.ReportSources[i].SqlColumns[j].ObjectID = Guid.NewGuid().ToString();
                            reportpage.ReportSources[i].SqlColumns[j].ParentObjectID = reportpage.ReportSources[i].ObjectID;
                        }
                    }
                    //设置数据源计算字段的ID
                    if (reportpage.ReportSources[i].FunctionColumns != null)
                    {
                        for (int j = 0; j < reportpage.ReportSources[i].FunctionColumns.Length; j++)
                        {
                            reportpage.ReportSources[i].FunctionColumns[j].ObjectID = Guid.NewGuid().ToString();
                        }
                    }
                    //设置数据源联动报表的ID
                    if (reportpage.ReportSources[i].ReportSourceAssociations != null)
                    {
                        for (int j = 0; j < reportpage.ReportSources[i].ReportSourceAssociations.Length; j++)
                        {
                            reportpage.ReportSources[i].ReportSourceAssociations[j].ObjectID = Guid.NewGuid().ToString();
                        }
                    }
                }

                if (reportpage.ReportWidgets != null)
                {
                    //记住历史报表页签id,是为了设置关联字段ID时做比较，不容易出错
                    string[] oldreportwidgetsid = new string[reportpage.ReportWidgets.Length];
                    for (var i = 0; i < reportpage.ReportWidgets.Length; i++)
                    {
                        oldreportwidgetsid[i] = reportpage.ReportWidgets[i].ObjectID;
                    }
                    for (var i = 0; i < reportpage.ReportWidgets.Length; i++)
                    {
                        reportpage.ReportWidgets[i].ObjectID = Guid.NewGuid().ToString();
                        reportpage.ReportWidgets[i].Code = reportpage.ReportWidgets[i].ObjectID;
                        //循环旧的数据源ID,判断是否和当前的报表页签关联的数据源ＩＤ　匹配
                        for (int j = 0; j < oldsourcesid.Length; j++)
                        {
                            if (reportpage.ReportWidgets[i].ReportSourceId == oldsourcesid[j])
                            {
                                reportpage.ReportWidgets[i].ReportSourceId = reportpage.ReportSources[j].ObjectID;
                            }
                        }
                        //设置简易报表列值
                        if (reportpage.ReportWidgets[i].ReportWidgetSimpleBoard != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].ReportWidgetSimpleBoard.Length; j++)
                            {
                                if (reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].Columns!=null)
                                {
                                    for (int k = 0; k < reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].Columns.Length; k++)
                                    {
                                        reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].Columns[k].ObjectID= Guid.NewGuid().ToString();
                                        reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].Columns[k].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                                    }
                                }
                                reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].ObjectID = Guid.NewGuid().ToString();
                                reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].ReportSourceId = reportpage.ReportSources[j].ObjectID;
                                reportpage.ReportWidgets[i].ReportWidgetSimpleBoard[j].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                            }
                        }
                        //设置系列的关联关系值
                        if (reportpage.ReportWidgets[i].Series != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].Series.Length; j++)
                            {
                                reportpage.ReportWidgets[i].Series[j].ObjectID = Guid.NewGuid().ToString();
                                reportpage.ReportWidgets[i].Series[j].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                            }
                        }
                        //设置分类的关联关系值
                        if (reportpage.ReportWidgets[i].Categories != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].Categories.Length; j++)
                            {
                                reportpage.ReportWidgets[i].Categories[j].ObjectID = Guid.NewGuid().ToString();
                                reportpage.ReportWidgets[i].Categories[j].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                            }
                        }
                        //设置源列的关联关系值
                        if (reportpage.ReportWidgets[i].Columns != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].Columns.Length; j++)
                            {
                                reportpage.ReportWidgets[i].Columns[j].ObjectID = Guid.NewGuid().ToString();
                                reportpage.ReportWidgets[i].Columns[j].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                            }
                        }
                        //设置源列过滤条件的关联关系值
                        if (reportpage.ReportWidgets[i].FilterColumns != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].FilterColumns.Length; j++)
                            {
                                reportpage.ReportWidgets[i].FilterColumns[j].ObjectID = Guid.NewGuid().ToString();
                                reportpage.ReportWidgets[i].FilterColumns[j].ParentObjectID = reportpage.ReportWidgets[i].ObjectID;
                            }
                        }
                        //设置联动报表 ,因为关联的报表页签不可能比当前页签还多，所以可以使用当前页签的总数来循环判断
                        if (reportpage.ReportWidgets[i].LinkageReports != null)
                        {
                            for (int j = 0; j < reportpage.ReportWidgets[i].LinkageReports.Length; j++)
                            {
                                for (int k = 0; k < oldreportwidgetsid.Length; k++)
                                {
                                    if (oldreportwidgetsid[k] == reportpage.ReportWidgets[i].LinkageReports[j])
                                    {
                                        reportpage.ReportWidgets[i].LinkageReports[j] = reportpage.ReportWidgets[j].ObjectID;
                                    }
                                }

                            }
                        }
                        reportpage.ReportWidgets[i].ParentObjectID = reportpage.ObjectID;
                    }
                }
            }
            return reportpage;
        }
        #endregion


        #region 报表数据模型，数据展示相关操作方法

        /// <summary>
        /// 加载报表
        /// </summary>
        /// <param name="ReportCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadReportData()
        {
            string ErrorMsg = "";
            string ReportCode = this.Request["ReportCode"];
            ReportPage dd = this.Engine.ReportManager.GetReportPage(ReportCode);
            ReportPage ReportPages = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(ReportCode));
            ReportWidget[] reportwidgets = ReportPages.ReportWidgets;
            List<ReportFilter> resultFilters = new List<ReportFilter>();
            HashSet<string> sourceColumn = new HashSet<string>();
            HashSet<string> sqlwherecolumns = new HashSet<string>();
            if (ReportPages.ReportSources != null && ReportPages.ReportSources.Length > 0)
            {
                foreach (ReportSource q in ReportPages.ReportSources)
                {
                    if (!q.IsUseSql || q.SQLWhereColumns == null || q.SQLWhereColumns.Length == 0) continue;
                    foreach (ReportWidgetColumn k in q.SQLWhereColumns)
                    {
                        sqlwherecolumns.Add(k.ColumnCode);
                    }
                }
            }
            if (reportwidgets != null && reportwidgets.Length > 0)
            {
                List<ReportWidget> resultReportWidgets = new List<ReportWidget>();
                foreach (ReportWidget q in reportwidgets)
                {
                    string tempErrormsg = "";
                    List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(ReportPages, ReportPages.Filters, q, out tempErrormsg);
                    if (!string.IsNullOrEmpty(tempErrormsg))
                        ErrorMsg += q.DisplayName + ":" + tempErrormsg + "/n";
                    HashSet<string> Columns = new HashSet<string>();
                    List<ReportWidgetColumn> ResultSourceColumns = new List<ReportWidgetColumn>();
                    if (SourceColumns != null)
                    {
                        foreach (ReportWidgetColumn k in SourceColumns)
                        {
                            if (!Columns.Contains(k.ColumnCode))
                            {
                                Columns.Add(k.ColumnCode);
                                sourceColumn.Add(k.ColumnCode);
                            }
                        }
                    }


                    if (q.Columns != null && q.Columns.Length > 0)
                    {
                        ResultSourceColumns = new List<ReportWidgetColumn>();
                        foreach (ReportWidgetColumn k in q.Columns)
                        {
                            if (Columns.Contains(k.ColumnCode) || k.ColumnCode == OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                                ResultSourceColumns.Add(k);
                        }
                        q.Columns = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();

                    }
                    if (q.Series != null && q.Series.Length > 0)
                    {

                        ResultSourceColumns = new List<ReportWidgetColumn>();
                        foreach (ReportWidgetColumn k in q.Series)
                        {
                            if (Columns.Contains(k.ColumnCode))
                                ResultSourceColumns.Add(k);
                        }
                        q.Series = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                    }
                    if (q.Categories != null && q.Categories.Length > 0)
                    {
                        ResultSourceColumns = new List<ReportWidgetColumn>();
                        foreach (ReportWidgetColumn k in q.Categories)
                        {
                            if (Columns.Contains(k.ColumnCode))
                                ResultSourceColumns.Add(k);
                        }
                        q.Categories = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                    }
                    if (q.SortColumns != null && q.SortColumns.Length > 0)
                    {
                        ResultSourceColumns = new List<ReportWidgetColumn>();
                        foreach (ReportWidgetColumn k in q.SortColumns)
                        {
                            if (Columns.Contains(k.ColumnCode))
                                ResultSourceColumns.Add(k);
                        }
                        q.SortColumns = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                    }
                    //简易看板处理
                    if (q.WidgetType == WidgetType.SimpleBoard)
                    {
                        if (q.ReportWidgetSimpleBoard != null && q.ReportWidgetSimpleBoard.Length > 0)
                        {
                            foreach (ReportWidgetSimpleBoard t in q.ReportWidgetSimpleBoard)
                            {
                                tempErrormsg = "";
                                List<ReportWidgetColumn> SimpleSourceColumns = this.GetSourceColumns(ReportPages, ReportPages.Filters, t, this.Engine.EngineConfig.DBType, out tempErrormsg);
                                if (!string.IsNullOrEmpty(tempErrormsg))
                                    ErrorMsg += t.DisplayName + ":" + tempErrormsg + "/n";
                                HashSet<string> SimpleColumns = new HashSet<string>();
                                List<ReportWidgetColumn> SimpleResultSourceColumns = new List<ReportWidgetColumn>();
                                if (SimpleSourceColumns != null)
                                {
                                    foreach (ReportWidgetColumn k in SimpleSourceColumns)
                                    {
                                        if (!SimpleColumns.Contains(k.ColumnCode))
                                        {
                                            SimpleColumns.Add(k.ColumnCode);
                                            if (!sourceColumn.Contains(k.ColumnCode))
                                                sourceColumn.Add(k.ColumnCode);
                                        }
                                    }
                                    if (t.Columns != null && t.Columns.Length > 0)
                                    {
                                        SimpleResultSourceColumns = new List<ReportWidgetColumn>();
                                        foreach (ReportWidgetColumn k in t.Columns)
                                        {
                                            if (SimpleColumns.Contains(k.ColumnCode) || k.ColumnCode == OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                                                SimpleResultSourceColumns.Add(k);
                                        }
                                        t.Columns = SimpleResultSourceColumns == null ? null : SimpleResultSourceColumns.ToArray();
                                    }
                                }
                            }
                        }
                    }
                    resultReportWidgets.Add(q);
                }
                if (ReportPages.Filters != null && ReportPages.Filters.Length > 0)
                {
                    foreach (ReportFilter q in ReportPages.Filters)
                    {
                        if (sourceColumn.Contains(q.ColumnCode) || (sqlwherecolumns != null && sqlwherecolumns.Count > 0 && sqlwherecolumns.Contains(q.ColumnCode)))
                        {
                            resultFilters.Add(q);
                        }
                    }
                }
                ReportPages.Filters = resultFilters != null && resultFilters.Count > 0 ? resultFilters.ToArray() : null;
                ReportPages.ReportWidgets = resultReportWidgets != null && resultReportWidgets.Count > 0 ? resultReportWidgets.ToArray() : null;
            }
            // string[] DataDictItemsName = new string[0];
            //qiancheng
            object DataDictItemsName = GetCategoryList();
            //List<Dictionary<string, object>> SourceData = this.Engine.AnalyticalQuery.QueryTable(ReportSource.ExecuteTableName(DataAclScope), SourceColumns.ToArray(), null, 1, 5);
            return Json(new { ReportPage = ReportPages, DataDictItemsName = DataDictItemsName, ErrorMsg = ErrorMsg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 实时更新widget获取数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadWidgetDataSource()
        {
            string widgetCode = Request["WidgetCode"];
            ReportWidget curWidget = null;
            string ReportPageJson = this.Request["ReportPageStr"];
            if (string.IsNullOrEmpty(ReportPageJson))
            {
                return Json(new { Result = false, Msg = "参数为空!" });
            }
            //try
            //{
            int PageSize = 10;
            int BeginNum = 1;
            if (!int.TryParse(this.Request.Form["length"] ?? "", out PageSize))
            {
                PageSize = 10;
            }
            int Count;
            if (!int.TryParse(this.Request.Form["start"] ?? "", out BeginNum))
            {
                BeginNum = 1;
            }
            else
            {
                BeginNum = BeginNum + 1;
            }
            ReportPage reportPage = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportPage>(ReportPageJson);

            if (reportPage != null)
            {
                foreach (ReportWidget widget in reportPage.ReportWidgets)
                {
                    if (widgetCode == widget.Code)
                    {
                        curWidget = widget;
                        break;
                    }
                }
            }
            //排除系列和分类里的defautcode

            if (curWidget != null)
            {
                this.RemoveDefautcodeFromSC(curWidget);
                string errormsg = "";
                List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage, reportPage.Filters, curWidget, out errormsg);
                List<Dictionary<string, object>> data = LoadGridData(reportPage, curWidget, SourceColumns, BeginNum, PageSize, out Count);
                return Json(new { Result = data, Total = Count }, JsonRequestBehavior.AllowGet);
            }
            return null;

        }


        /// <summary>
        /// 保存报表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveReportPage()
        {
            string ErrorMsg = "";
            string ReportPageJson = this.Request["ReportPageStr"];
            if (string.IsNullOrWhiteSpace(ReportPageJson))
            {
                return Json(new { Result = false, Msg = "参数为空!" });
            }
            //try
            //{
            ReportPage NewReportPage = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportPage>(ReportPageJson);
            //构造一个新的报表页对象
            NewReportPage = NewReportPages(NewReportPage);

            ReportPage ReportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(NewReportPage.Code));

            List<ReportFilter> filters = new List<ReportFilter>();
            List<ReportWidget> reportWidgets = new List<ReportWidget>();
            List<ReportSource> reportSources = new List<ReportSource>();
            #region 全局过滤参数
            if (NewReportPage.Filters != null)
            {
                foreach (ReportFilter f in NewReportPage.Filters)
                {
                    filters.Add(new ReportFilter()
                    {
                        ColumnCode = f.ColumnCode,
                        DisplayName = f.DisplayName,
                        FilterType = f.FilterType,
                        FilterValue = f.FilterValue,
                        DefaultValue = f.DefaultValue,
                        ColumnName = f.ColumnName,
                        AllowNull = f.AllowNull,
                        Visible = f.Visible,
                        ColumnType = f.ColumnType,
                        OrganizationType = f.OrganizationType,
                        AssociationType = f.AssociationType,
                        AssociationSchemaCode = f.AssociationSchemaCode,
                        IsSqlWhere = f.IsSqlWhere

                    });
                }
            }
            #endregion

            #region 数据源，更新数据源
            if (NewReportPage.ReportSources != null)
            {

                Dictionary<string, ReportSource> dicOldReportSource = new Dictionary<string, ReportSource>();
                if (ReportPage.ReportSources != null && ReportPage.ReportSources.Length > 0)
                {
                    foreach (ReportSource q in ReportPage.ReportSources)
                    {
                        if (!dicOldReportSource.ContainsKey(q.ObjectID))
                            dicOldReportSource.Add(q.ObjectID, q);
                    }
                }

                List<string> objectids = new List<string>();
                foreach (ReportSource source in NewReportPage.ReportSources)
                {
                    if (objectids.Contains(source.ObjectID)) continue;
                    objectids.Add(source.ObjectID);
                    List<ReportWidgetColumn> functionColumns = new List<ReportWidgetColumn>();
                    List<ReportWidgetColumn> sqlColumns = new List<ReportWidgetColumn>();
                    List<ReportSourceAssociation> associations = new List<ReportSourceAssociation>();
                    List<ReportWidgetColumn> sqlWhereColumns = new List<ReportWidgetColumn>();
                    //函数计算字段
                    if (source.FunctionColumns != null)
                    {
                        foreach (ReportWidgetColumn c in source.FunctionColumns)
                        {
                            functionColumns.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format,
                                ColumnDataFormat = c.ColumnDataFormat
                            });
                        }
                    }
                    //SQL字段
                    if (source.SqlColumns != null)
                    {
                        foreach (ReportWidgetColumn c in source.SqlColumns)
                        {
                            sqlColumns.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format,
                                ColumnDataFormat = c.ColumnDataFormat
                            });
                        }
                    }
                    //SQL查询字段
                    if (source.SQLWhereColumns != null)
                    {
                        foreach (ReportWidgetColumn c in source.SQLWhereColumns)
                        {
                            sqlWhereColumns.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format,
                                ColumnDataFormat = c.ColumnDataFormat
                            });
                        }
                    }
                    //关联对象
                    if (source.ReportSourceAssociations != null)
                    {
                        foreach (ReportSourceAssociation c in source.ReportSourceAssociations)
                        {
                            associations.Add(new ReportSourceAssociation()
                            {
                                IsSubSheet = c.IsSubSheet,
                                MasterField = c.MasterField,
                                AssociationMode = c.AssociationMode,
                                SchemaCode = c.SchemaCode,
                                SubField = c.SubField,
                                RootSchemaCode = c.RootSchemaCode,
                                AssociationMethod = c.AssociationMethod
                            });
                        }
                    }
                    //sql时，不显示数据源的编辑按钮，当用调试方式显示编辑按钮时，不保存编辑后的sql语句，在验证sql时加是否开发者的验证；
                    //if (source.IsUseSql && !usersIsDiv)
                    //{
                    //    if (dicOldReportSource.ContainsKey(source.ObjectID))
                    //    {
                    //        dicOldReportSource[source.ObjectID].DisplayName = source.DisplayName;
                    //        dicOldReportSource[source.ObjectID].IsSubSheet = source.IsSubSheet;
                    //        dicOldReportSource[source.ObjectID].IsUseSql = source.IsUseSql;
                    //        dicOldReportSource[source.ObjectID].SqlColumns = sqlColumns.ToArray();
                    //        dicOldReportSource[source.ObjectID].SQLWhereColumns = sqlWhereColumns.ToArray();

                    //        dicOldReportSource[source.ObjectID].SchemaCode = source.SchemaCode;
                    //        dicOldReportSource[source.ObjectID].ReportSourceAssociations = associations.ToArray();
                    //        dicOldReportSource[source.ObjectID].FunctionColumns = functionColumns.ToArray();
                    //        dicOldReportSource[source.ObjectID].AppGroup = source.AppGroup;
                    //        reportSources.Add(dicOldReportSource[source.ObjectID]);
                    //    }
                    //    else
                    //    {
                    //        reportSources.Add(new ReportSource()
                    //        {
                    //            ObjectID = source.ObjectID,
                    //            DisplayName = source.DisplayName,
                    //            IsSubSheet = source.IsSubSheet,
                    //            IsUseSql = source.IsUseSql,
                    //            SqlColumns = sqlColumns.ToArray(),
                    //            SQLWhereColumns = sqlWhereColumns.ToArray(),
                    //            SchemaCode = source.SchemaCode,
                    //            ReportSourceAssociations = associations.ToArray(),
                    //            FunctionColumns = functionColumns.ToArray(),
                    //            AppGroup = source.AppGroup

                    //        });
                    //    }
                    //}
                    //else
                    //{
                    if (dicOldReportSource.ContainsKey(source.ObjectID))
                    {
                        dicOldReportSource[source.ObjectID].DisplayName = source.DisplayName;
                        dicOldReportSource[source.ObjectID].IsSubSheet = source.IsSubSheet;
                        dicOldReportSource[source.ObjectID].IsUseSql = source.IsUseSql;
                        dicOldReportSource[source.ObjectID].SqlColumns = sqlColumns.ToArray();
                        dicOldReportSource[source.ObjectID].SQLWhereColumns = sqlWhereColumns.ToArray();
                        dicOldReportSource[source.ObjectID].CommandText = source.CommandText;
                        dicOldReportSource[source.ObjectID].SchemaCode = source.SchemaCode;
                        dicOldReportSource[source.ObjectID].ReportSourceAssociations = associations.ToArray();
                        dicOldReportSource[source.ObjectID].FunctionColumns = functionColumns.ToArray();
                        dicOldReportSource[source.ObjectID].AppGroup = source.AppGroup;
                        reportSources.Add(dicOldReportSource[source.ObjectID]);
                    }
                    else
                    {
                        reportSources.Add(new ReportSource()
                        {
                            ObjectID = source.ObjectID,
                            DisplayName = source.DisplayName,
                            IsSubSheet = source.IsSubSheet,
                            IsUseSql = source.IsUseSql,
                            SqlColumns = sqlColumns.ToArray(),
                            SQLWhereColumns = sqlWhereColumns.ToArray(),
                            CommandText = source.CommandText,
                            SchemaCode = source.SchemaCode,
                            ReportSourceAssociations = associations.ToArray(),
                            FunctionColumns = functionColumns.ToArray(),
                            AppGroup = source.AppGroup
                        });
                    }

                    //  }
                }
            }
            #endregion

            #region ReportWidget 报表页
            if (NewReportPage.ReportWidgets != null)
            {
                foreach (ReportWidget r in NewReportPage.ReportWidgets)
                {

                    List<ReportWidgetColumn> columns = new List<ReportWidgetColumn>();
                    List<ReportWidgetColumn> series = new List<ReportWidgetColumn>();
                    List<ReportWidgetColumn> categories = new List<ReportWidgetColumn>();
                    List<ReportWidgetColumn> sortColumns = new List<ReportWidgetColumn>();
                    List<ReportWidgetColumn> functionColumns = new List<ReportWidgetColumn>();
                    List<ReportSourceAssociation> associactions = new List<ReportSourceAssociation>();
                    List<ReportWidgetSimpleBoard> simpleBoards = new List<ReportWidgetSimpleBoard>();
                    if (r.Columns != null)
                    {
                        foreach (ReportWidgetColumn c in r.Columns)
                        {
                            columns.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                //   AssociationMappings=c.AssociationMappings,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format,
                                ColumnDataFormat = c.ColumnDataFormat
                            });
                        }
                    }
                    if (r.Series != null)
                    {
                        foreach (ReportWidgetColumn c in r.Series)
                        {
                            series.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                // AssociationMappings = c.AssociationMappings,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format
                            });
                        }
                    }
                    else
                    {
                        series = null;
                    }
                    if (r.Categories != null)
                    {
                        foreach (ReportWidgetColumn c in r.Categories)
                        {
                            categories.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                // AssociationMappings = c.AssociationMappings,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format
                            });
                        }
                    }

                    if (r.SortColumns != null)
                    {
                        foreach (ReportWidgetColumn c in r.SortColumns)
                        {
                            sortColumns.Add(new ReportWidgetColumn()
                            {
                                ColumnCode = c.ColumnCode,
                                ColumnName = c.ColumnName,
                                Ascending = c.Ascending,
                                ColumnType = c.ColumnType,
                                DisplayName = c.DisplayName,
                                FunctionType = c.FunctionType,
                                Sortable = c.Sortable,
                                // AssociationMappings = c.AssociationMappings,
                                AssociationType = c.AssociationType,
                                AssociationSchemaCode = c.AssociationSchemaCode,
                                Formula = c.Formula,
                                Format = c.Format,
                                ColumnDataFormat = c.ColumnDataFormat
                            });
                        }
                    }


                    if (r.ReportWidgetSimpleBoard != null)
                    {
                        foreach (ReportWidgetSimpleBoard simpleBoard in r.ReportWidgetSimpleBoard)
                        {
                            List<ReportWidgetColumn> simpleBoardColumns = new List<ReportWidgetColumn>();
                            if (simpleBoard.Columns != null && simpleBoard.Columns.Length > 0)
                            {
                                var simpleBoardColumn = simpleBoard.Columns[0];
                                simpleBoardColumns.Add(new ReportWidgetColumn()
                                {
                                    ColumnCode = simpleBoardColumn.ColumnCode,
                                    ColumnName = simpleBoardColumn.ColumnName,
                                    Ascending = simpleBoardColumn.Ascending,
                                    ColumnType = simpleBoardColumn.ColumnType,
                                    DisplayName = simpleBoardColumn.DisplayName,
                                    FunctionType = simpleBoardColumn.FunctionType,
                                    Sortable = simpleBoardColumn.Sortable,
                                    //   AssociationMappings=c.AssociationMappings,
                                    AssociationType = simpleBoardColumn.AssociationType,
                                    AssociationSchemaCode = simpleBoardColumn.AssociationSchemaCode,
                                    Formula = simpleBoardColumn.Formula,
                                    Format = simpleBoardColumn.Format,
                                    ColumnDataFormat = simpleBoardColumn.ColumnDataFormat
                                });
                            }
                            ReportWidgetSimpleBoard board = new ReportWidgetSimpleBoard()
                            {
                                ObjectID = simpleBoard.ObjectID,
                                Code = simpleBoard.Code,
                                DisplayName = simpleBoard.DisplayName,
                                ReportSourceId = simpleBoard.ReportSourceId,
                                WidgetType = simpleBoard.WidgetType,

                                Columns = simpleBoardColumns == null || simpleBoardColumns.Count == 0 ? null : simpleBoardColumns.ToArray(),

                                RowIndex = simpleBoard.RowIndex,
                                ColumnIndex = simpleBoard.ColumnIndex,
                                LinkageReports = simpleBoard.LinkageReports

                            };
                            simpleBoards.Add(board);

                        }
                    }

                    ReportWidget reportWidget = new ReportWidget()
                    {
                        ObjectID = r.ObjectID,
                        Code = r.Code,
                        DisplayName = r.DisplayName,
                        ReportSourceId = r.ReportSourceId,
                        XAxisUnit = r.XAxisUnit,
                        YAxisUnit = r.YAxisUnit,
                        Columns = columns == null || columns.Count == 0 ? null : columns.ToArray(),
                        Categories = categories == null || categories.Count == 0 ? null : categories.ToArray(),
                        Series = series == null || series.Count == 0 ? null : series.ToArray(),
                        SortColumns = sortColumns == null || sortColumns.Count == 0 ? null : sortColumns.ToArray(),
                        ReportWidgetSimpleBoard = simpleBoards == null || simpleBoards.Count == 0 ? null : simpleBoards.ToArray(),
                        SimpleBoardRowNumber = r.SimpleBoardRowNumber,
                        SimpleBoardColumnNumber = r.SimpleBoardColumnNumber,
                        Exportable = r.Exportable,
                        Layout = r.Layout,
                        WidgetType = r.WidgetType,
                        FrozenHeaderType = r.FrozenHeaderType,
                        LinkageReports = r.LinkageReports,
                        DefaultSeriesData = r.DefaultSeriesData,
                        DefaultCategorysData = r.DefaultCategorysData
                    };

                    reportWidgets.Add(reportWidget);
                }
            }
            #endregion

            #region 报表页对应的数据源
            HashSet<string> sqlwherecolumns = new HashSet<string>();
            if (NewReportPage.ReportSources != null && NewReportPage.ReportSources.Length > 0)
            {
                foreach (ReportSource q in NewReportPage.ReportSources)
                {
                    if (!q.IsUseSql || q.SQLWhereColumns == null || q.SQLWhereColumns.Length == 0) continue;
                    foreach (ReportWidgetColumn k in q.SQLWhereColumns)
                    {
                        sqlwherecolumns.Add(k.ColumnCode);
                    }
                }
            }
            if (ReportPage != null)
            {
                ReportPage.Filters = null;
                ReportPage.ReportSources = reportSources.Count == 0 ? null : reportSources.ToArray(); ;
                ReportPage.ReportWidgets = null;

                ReportPage.Filters = filters.Count == 0 ? null : filters.ToArray();
                ReportPage.ReportWidgets = reportWidgets.Count == 0 ? null : reportWidgets.ToArray();
                ReportPage.Layout = NewReportPage.Layout;
                ReportWidget[] reportwidgets = ReportPage.ReportWidgets;
                List<ReportFilter> resultFilters = new List<ReportFilter>();
                HashSet<string> sourceColumn = new HashSet<string>();
                if (reportwidgets != null && reportwidgets.Length > 0)
                {
                    List<ReportWidget> resultReportWidgets = new List<ReportWidget>();
                    foreach (ReportWidget q in reportwidgets)
                    {
                        string tempErrorMsg = "";
                        List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(ReportPage, ReportPage.Filters, q, out tempErrorMsg);
                        if (!string.IsNullOrEmpty(tempErrorMsg))
                            ErrorMsg += q.DisplayName + ":" + tempErrorMsg + "/n";
                        HashSet<string> Columns = new HashSet<string>();
                        List<ReportWidgetColumn> ResultSourceColumns = new List<ReportWidgetColumn>();
                        if (SourceColumns != null)
                        {
                            foreach (ReportWidgetColumn k in SourceColumns)
                            {
                                if (!Columns.Contains(k.ColumnCode))
                                {
                                    Columns.Add(k.ColumnCode);
                                    if (!sourceColumn.Contains(k.ColumnCode))
                                        sourceColumn.Add(k.ColumnCode);
                                }
                            }
                        }

                        if (q.Columns != null && q.Columns.Length > 0)
                        {
                            ResultSourceColumns = new List<ReportWidgetColumn>();
                            foreach (ReportWidgetColumn k in q.Columns)
                            {
                                if (Columns.Contains(k.ColumnCode) || k.ColumnCode == OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                                    ResultSourceColumns.Add(k);
                            }
                            q.Columns = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                        }
                        if (q.Series != null && q.Series.Length > 0)
                        {

                            ResultSourceColumns = new List<ReportWidgetColumn>();
                            foreach (ReportWidgetColumn k in q.Series)
                            {
                                if (Columns.Contains(k.ColumnCode))
                                    ResultSourceColumns.Add(k);
                            }
                            q.Series = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                        }
                        if (q.Categories != null && q.Categories.Length > 0)
                        {
                            ResultSourceColumns = new List<ReportWidgetColumn>();
                            foreach (ReportWidgetColumn k in q.Categories)
                            {
                                if (Columns.Contains(k.ColumnCode))
                                    ResultSourceColumns.Add(k);
                            }
                            q.Categories = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                        }
                        if (q.SortColumns != null && q.SortColumns.Length > 0)
                        {
                            ResultSourceColumns = new List<ReportWidgetColumn>();
                            foreach (ReportWidgetColumn k in q.SortColumns)
                            {
                                if (Columns.Contains(k.ColumnCode))
                                    ResultSourceColumns.Add(k);
                            }
                            q.SortColumns = ResultSourceColumns == null ? null : ResultSourceColumns.ToArray();
                        }
                        //简易看板处理
                        if (q.WidgetType == WidgetType.SimpleBoard)
                        {
                            if (q.ReportWidgetSimpleBoard != null && q.ReportWidgetSimpleBoard.Length > 0)
                            {
                                foreach (ReportWidgetSimpleBoard t in q.ReportWidgetSimpleBoard)
                                {
                                    tempErrorMsg = "";
                                    List<ReportWidgetColumn> SimpleSourceColumns = this.GetSourceColumns(ReportPage, ReportPage.Filters, t, this.Engine.EngineConfig.DBType, out tempErrorMsg);
                                    if (!string.IsNullOrEmpty(tempErrorMsg))
                                        ErrorMsg += t.DisplayName + ":" + tempErrorMsg + "/n";
                                    HashSet<string> SimpleColumns = new HashSet<string>();
                                    List<ReportWidgetColumn> SimpleResultSourceColumns = new List<ReportWidgetColumn>();
                                    if (SimpleSourceColumns != null)
                                    {
                                        foreach (ReportWidgetColumn k in SimpleSourceColumns)
                                        {
                                            if (!SimpleColumns.Contains(k.ColumnCode))
                                            {
                                                SimpleColumns.Add(k.ColumnCode);
                                                if (!sourceColumn.Contains(k.ColumnCode))
                                                    sourceColumn.Add(k.ColumnCode);
                                            }
                                        }
                                        if (t.Columns != null && t.Columns.Length > 0)
                                        {
                                            SimpleResultSourceColumns = new List<ReportWidgetColumn>();
                                            foreach (ReportWidgetColumn k in t.Columns)
                                            {
                                                if (SimpleColumns.Contains(k.ColumnCode) || k.ColumnCode == OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                                                    SimpleResultSourceColumns.Add(k);
                                            }
                                            t.Columns = SimpleResultSourceColumns == null ? null : SimpleResultSourceColumns.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                        resultReportWidgets.Add(q);
                    }
                    if (ReportPage.Filters != null && ReportPage.Filters.Length > 0)
                    {
                        foreach (ReportFilter q in ReportPage.Filters)
                        {
                            if (sourceColumn.Contains(q.ColumnCode) || (sqlwherecolumns != null && sqlwherecolumns.Count > 0 && sqlwherecolumns.Contains(q.ColumnCode)))
                            {
                                resultFilters.Add(q);
                            }
                        }
                    }

                    ReportPage.Filters = resultFilters != null && resultFilters.Count > 0 ? resultFilters.ToArray() : null;
                    ReportPage.ReportWidgets = resultReportWidgets != null && resultReportWidgets.Count > 0 ? resultReportWidgets.ToArray() : null;
                }
            }
            #endregion
            //this.Engine.ReportManager.RemoveReportPage(NewReportPage.Code);
            //更新，没有的话就新增
            if (string.IsNullOrEmpty(ErrorMsg))
            {
                this.Engine.ReportManager.UpdateReportPage(NewReportPage);
                return Json(new { Result = true, Msg = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false, Msg = ErrorMsg }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 检查sql是否合格（该方法会防止sql注入）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckSQL()
        {

            string Sql = Request["Sql"];
            if (Sql.Length > 2000)
            {
                return Json(new { status = false, message = "SQL语句太长，建议通过在数据库创建视图来实现！" });
            }
            string ColumnsStr = Request["Columns"];
            string DbCode = Request["DbCode"];
            if (string.IsNullOrEmpty(DbCode))
            {
                DbCode = "Engine";
            }
            ReportWidgetColumn[] Columns = string.IsNullOrEmpty(ColumnsStr) ? null : this.JsSerializer.Deserialize<ReportWidgetColumn[]>(ColumnsStr);
            //防止SQL注入的 检验操作关键字
            string word = "exec|insert|update|delete|chr|mid|master|truncate|declare|drop";
            if (Sql == null)
            {
                return Json(new { status = false, message = "SQL语句不能为空！" });
            }
            else
            {
                //校验及提取{}条件
                string newSql = "";
                List<string> replaceitems = new List<string>();
                //检查SQL语句是否正常
                bool WhereRight = this.Engine.ReportQuery.CheckSQLWhere(Sql, Columns, out newSql, out replaceitems);
                if (!WhereRight)
                {
                    return Json(new { status = false, message = "条件设置错误！" });
                }
                foreach (string i in word.Split('|'))
                {
                    if (newSql.ToLower().IndexOf(i + " ") > -1 || newSql.ToLower().IndexOf(" " + i) > -1)
                    {
                        return Json(new { status = false, message = "SQL语句中存在非法关键字: " + i });
                    }
                }
                try
                {
                    //获取sql的列
                    List<OThinker.Reporting.ReportWidgetColumn> columns = this.Engine.ReportQuery.GetSqlColumns(this.Engine, newSql, DbCode);
                    if (columns == null)
                    {
                        return Json(new { status = false, message = "SQL语句错误，请检查" });
                    }
                    else
                    {
                        string Orders = "";
                        //这里要增加判断，如果是sql中携带了order by ** desc这样排序的 则要取出这个order by 的值
                        //作为排序条件
                        if (newSql.ToUpper().IndexOf(" ORDER ") != -1 && newSql.ToUpper().IndexOf(" BY ") != -1)
                        {
                            return Json(new { status = false, message = "SQL语句错误，此处不支持 order by排序，请在报表设计器中排序，或者在数据库视图中排序" });
                            //if (newSql.ToUpper().IndexOf(" ASC") != -1 || newSql.ToUpper().IndexOf(" DESC") != -1)
                            //{
                            //    if (newSql.ToUpper().IndexOf(" ASC") != -1)
                            //    {
                            //        Orders = newSql.Substring(Sql.ToUpper().IndexOf(" BY ") + 3, newSql.ToUpper().IndexOf(" ASC") - (newSql.ToUpper().IndexOf(" BY ") + 2)).Trim();
                            //        newSql = newSql.Remove(newSql.ToUpper().IndexOf(" ORDER "), newSql.ToUpper().IndexOf(" ASC") - newSql.ToUpper().IndexOf(" ORDER ") + 4);
                            //        Orders=Orders+ " ASC ";
                            //    }
                            //    if (newSql.ToUpper().IndexOf(" DESC") != -1)
                            //    {

                            //        Orders = newSql.Substring(Sql.ToUpper().IndexOf(" BY ") + 3, newSql.ToUpper().IndexOf(" DESC") - (newSql.ToUpper().IndexOf(" BY ") + 2)).Trim();
                            //        newSql = newSql.Remove(newSql.ToUpper().IndexOf(" ORDER "), newSql.ToUpper().IndexOf(" DESC") - newSql.ToUpper().IndexOf(" ORDER ") + 5);
                            //        Orders = Orders + " DESC ";
                            //    }

                            //}
                            //else
                            //{
                            //    return Json(new { status = false, message = "SQL语句错误，order by后面必须跟上 asc 或者 desc" });
                            //}
                        }
                        else
                        {
                            Orders = columns[0].ColumnCode;
                        }

                        //返回数据 开发模式查询前20行数据进行展示
                        //string top5Sql = newSql + " limit 0,20";// mysql 的写法
                        string selects = "SELECT * FROM (SELECT ROW_NUMBER() OVER ( ORDER BY " + Orders + ") AS RowNumber_ , tbgy23ym2m0erkh6zygprkciqa7.*  FROM (";
                        string tabels = " ) tbgy23ym2m0erkh6zygprkciqa7 ) T WHERE RowNumber_>=0 AND RowNumber_<=10";
                        string top5Sql = selects + newSql + tabels;

                        DataTable dtb = this.Engine.Query.QueryTable(this.Engine, top5Sql, DbCode);
                        if (dtb == null)
                        {
                            return Json(new { status = false, message = "获取数据出错，请检查SQL语句" });
                        }
                        else
                        {
                            ArrayList dic = new ArrayList();

                            foreach (DataRow row in dtb.Rows)
                            {
                                Dictionary<string, object> dRow = new Dictionary<string, object>();
                                foreach (DataColumn column in dtb.Columns)
                                {
                                    dRow.Add(column.ColumnName.ToLower(), row[column.ColumnName].ToString());
                                }
                                dic.Add(dRow);
                            }
                            return Json(new { status = true, cols = columns, rows = dic }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch (Exception e)
                {
                    string Inner = e.InnerException + string.Empty;

                    return Json(new { status = false, message = Inner.Substring(0, Inner.IndexOf("。") + 1) });
                }



            }
        }


        /// <summary>
        /// 获取数据源展示的列
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSchemas()
        {
            string SchemaCode = Request["SchemaCode"];
            ReportWidgetColumnSummary Reportwidget = new ReportWidgetColumnSummary();
            Reportwidget.OptionalValues = "";
            Reportwidget.DataDictItemName = "";

            // List<ReportWidgetColumnSummary> columns = this.Engine.ReportQuery.GetSoucrColumnsSummaryBySchemaCode(SchemaCode);
            List<ReportWidgetColumnSummary> columns = this.GetSoucrColumnsSummaryBySchemaCode(SchemaCode);
            return Json(new { result = columns }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取sql类型的数据源的列
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSqlSchemas()
        {
            string Sql = Request["Sql"];
            string DbCode = Request["DbCode"];//数据库连接池编码
            if (string.IsNullOrEmpty(DbCode))
            {
                DbCode = "Engine";
            }
            List<ReportWidgetColumn> columns = this.Engine.ReportQuery.GetSqlColumns(this.Engine, Sql, DbCode);
            return Json(new { result = columns }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取某个schema的字段列表，用于建立关联关系
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadBizObjectSchema()
        {
            List<BizProperty> fields = new List<BizProperty>();
            string code = Request["SchemaCode"];
            if (!string.IsNullOrEmpty(code))
            {
                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(code);
                if (schema != null)
                {
                    PropertySchema[] properties = schema.Properties;
                    if (properties != null && properties.Length > 0)
                    {
                        foreach (PropertySchema property in properties)
                        {
                            //去掉保留关键字
                            if (BizObjectSchema.IsBOReservedPropertiesOnly(property.Name))
                            {
                                continue;
                            }
                            if (property.LogicType == OThinker.H3.Data.DataLogicType.BizObject ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.BizStructure ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.BizStructureArray ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.ByteArray ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.Attachment ||
                                property.LogicType == OThinker.H3.Data.DataLogicType.Html
                                )
                            {
                                continue;
                            }
                            BizProperty bp = new BizProperty()
                            {
                                Code = property.Name,
                                DisplayName = property.DisplayName,
                                ParentCode = "I" + code,
                                //DataType = property.DataType
                                //数据类型转换
                            };
                            switch (property.LogicType)
                            {
                                case OThinker.H3.Data.DataLogicType.Int:
                                case OThinker.H3.Data.DataLogicType.Double:
                                case OThinker.H3.Data.DataLogicType.Long:
                                    bp.DataType = ColumnType.Numeric;
                                    break;
                                case OThinker.H3.Data.DataLogicType.DateTime:
                                    bp.DataType = ColumnType.DateTime;
                                    break;
                                case OThinker.H3.Data.DataLogicType.Bool:
                                    bp.DataType = ColumnType.Boolean;
                                    break;
                                case OThinker.H3.Data.DataLogicType.SingleParticipant:
                                    bp.DataType = ColumnType.Unit;
                                    break;
                                case OThinker.H3.Data.DataLogicType.MultiParticipant:
                                    bp.DataType = ColumnType.UnitArray;
                                    break;

                                default:
                                    bp.DataType = ColumnType.String;
                                    break;

                            }
                            fields.Add(bp);
                        }
                    }
                }
            }
            return Json(new { Result = true, Fields = fields }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 批量获取表单的显示名称
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSheetDisplayNames()
        {
            try
            {
                string sheetCodes = Request["Codes"];
                List<Sheet> sheets = new List<Sheet>();
                if (!string.IsNullOrEmpty(sheetCodes))
                {
                    string[] sheetArray = sheetCodes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    //由于关联子表只允许选择一个子表，并且主表编码始终是排在第一个

                    foreach (var sh in sheetArray)
                    {
                        var sheetCode = sheetArray[0];//始终是主表的编码
                        Sheet st = new Sheet();
                        st.SchemaCode = sh;
                        FunctionNode funNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(sh);
                        if (funNode != null)
                        {

                            st.DisplayName = funNode.DisplayName;

                        }
                        else
                        {
                            BizObjectSchema bizs = this.Engine.BizObjectManager.GetPublishedSchema(sheetCode);
                            if (bizs != null)
                            {
                                foreach (var ds in bizs.Fields)
                                {
                                    if (ds.LogicType == DataLogicType.BizObjectArray)
                                    {
                                        if (sh.IndexOf(ds.ChildSchemaCode) != -1)
                                        {
                                            st.DisplayName = ds.DisplayName;
                                        }
                                    }
                                }
                            }
                        }
                        //List<FunctionNode> funNode = this.Engine.FunctionAclManager.GetChildNodesByParentCode(sh);
                        //foreach (FunctionNode fun in funNode)
                        //{
                        //    //说明是流程包
                        //    if (fun.NodeType == OThinker.H3.Acl.FunctionNodeType.BizWorkflowPackage)
                        //    {
                        //        st.DisplayName = fun.DisplayName;

                        //    }
                        //}
                        sheets.Add(st);
                    }

                }
                return Json(new { State = true, Results = sheets });
            }
            catch (Exception ex)
            {
                return Json(new { State = false, Message = ex.Message });
            }
        }

        #endregion


        #region 内部私有方法

        /// <summary>
        /// 获取报表数据源源列集合
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="SchemacodePropertynameDisplayname"></param>
        /// <returns></returns>
        private List<ReportWidgetColumnSummary> GetSoucrColumnsSummaryBySchemaCode(string SchemaCode, Dictionary<string, Dictionary<string, string>> SchemacodePropertynameDisplayname = null)
        {
            List<ReportWidgetColumnSummary> Columns = new List<ReportWidgetColumnSummary>();
            BizObjectSchema Schema = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);

            if (Schema != null)
            {
                string tableName = OThinker.H3.DataModel.BizObjectSchema.GetTableName(SchemaCode).Replace("_", "");
                foreach (PropertySchema p in Schema.Properties)
                {
                    if (p.LogicType == DataLogicType.BizObjectArray
                        || p.LogicType == DataLogicType.Attachment) continue;
                    string Displayname = p.DisplayName;
                    if (SchemacodePropertynameDisplayname != null && SchemacodePropertynameDisplayname.ContainsKey(SchemaCode))
                    {
                        if (SchemacodePropertynameDisplayname[SchemaCode].ContainsKey(p.Name))
                            Displayname = SchemacodePropertynameDisplayname[SchemaCode][p.Name];
                    }
                    ReportWidgetColumnSummary ReportWidgetColumn = new ReportWidgetColumnSummary()
                    {
                        ReportWidgetColumn = new OThinker.Reporting.ReportWidgetColumn()
                        {
                            ColumnCode = tableName + "_" + p.Name,
                            ColumnName = tableName + "." + p.Name,
                            DisplayName = Displayname,

                        },
                        //OptionalValues = p.OptionalValues,
                        //DataDictItemName = p.DataDictItemName
                        OptionalValues = p.DefaultValue + string.Empty,
                        DataDictItemName = p.DefaultValue + string.Empty
                    };
                    switch (p.LogicType)
                    {
                        //case DataLogicType.Association:
                        //    ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Association;
                        //    //todo加code,mapping,type
                        //    {
                        //        ReportWidgetColumn.ReportWidgetColumn.AssociationSchemaCode = p.AssociationSchemaCode;
                        //        ReportWidgetColumn.ReportWidgetColumn.AssociationType = p.AssociationType;
                        //        // ReportWidgetColumn.AssociationMappings = p.AssociationMappings;
                        //    }
                        //    break;
                        case DataLogicType.ShortString:
                        case DataLogicType.String:
                            //case Data.BizDataType.Address
                            //case Data.BizDataType.Map:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.String;
                            break;
                        case DataLogicType.DateTime:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.DateTime;
                            break;
                        case DataLogicType.Int:
                        case DataLogicType.Long:
                        case DataLogicType.Double:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Numeric;
                            //if(p.ShowMode==PropertyShowMode.Kbit)
                            //{
                            //    ReportWidgetColumn.ReportWidgetColumn.Format = ",;";
                            //}
                            break;
                        case DataLogicType.SingleParticipant:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Unit;
                            break;
                        case DataLogicType.MultiParticipant:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.UnitArray;
                            break;
                        case DataLogicType.Bool:
                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Boolean;
                            break;
                        default: continue;
                    }
                    Columns.Add(ReportWidgetColumn);
                }
            }
            else
            {
                if (SchemaCode.IndexOf("___") > -1)
                {
                    string[] sch = SchemaCode.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                    BizObjectSchema bizsch = this.Engine.BizObjectManager.GetPublishedSchema(sch[0]);
                    if (bizsch != null)
                    {
                        foreach (var filed in bizsch.Fields)
                        {
                            if (filed.LogicType == DataLogicType.BizObjectArray && filed.ChildSchemaCode == sch[1])
                            {

                                foreach (var children in filed.Schema.Fields)
                                {

                                    ReportWidgetColumnSummary ReportWidgetColumn = new ReportWidgetColumnSummary()
                                    {
                                        ReportWidgetColumn = new OThinker.Reporting.ReportWidgetColumn()
                                        {
                                            ColumnCode = "I_" + sch[1] + "_" + children.Name,
                                            ColumnName = "I_" + sch[1] + "." + children.Name,
                                            DisplayName = children.DisplayName,

                                        },
                                        //OptionalValues = p.OptionalValues,
                                        //DataDictItemName = p.DataDictItemName
                                        OptionalValues = children.DefaultValue + string.Empty,
                                        DataDictItemName = children.DefaultValue + string.Empty
                                    };
                                    switch (children.LogicType)
                                    {
                                        //case DataLogicType.Association:
                                        //    ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Association;
                                        //    //todo加code,mapping,type
                                        //    {
                                        //        ReportWidgetColumn.ReportWidgetColumn.AssociationSchemaCode = p.AssociationSchemaCode;
                                        //        ReportWidgetColumn.ReportWidgetColumn.AssociationType = p.AssociationType;
                                        //        // ReportWidgetColumn.AssociationMappings = p.AssociationMappings;
                                        //    }
                                        //    break;
                                        case DataLogicType.ShortString:
                                        case DataLogicType.String:
                                            //case Data.BizDataType.Address
                                            //case Data.BizDataType.Map:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.String;
                                            break;
                                        case DataLogicType.DateTime:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.DateTime;
                                            break;
                                        case DataLogicType.Int:
                                        case DataLogicType.Long:
                                        case DataLogicType.Double:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Numeric;
                                            //if(p.ShowMode==PropertyShowMode.Kbit)
                                            //{
                                            //    ReportWidgetColumn.ReportWidgetColumn.Format = ",;";
                                            //}
                                            break;
                                        case DataLogicType.SingleParticipant:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Unit;
                                            break;
                                        case DataLogicType.MultiParticipant:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.UnitArray;
                                            break;
                                        case DataLogicType.Bool:
                                            ReportWidgetColumn.ReportWidgetColumn.ColumnType = ColumnType.Boolean;
                                            break;
                                        default: continue;
                                    }
                                    Columns.Add(ReportWidgetColumn);
                                }
                            }
                        }
                    }
                }

            }
            return Columns;
        }

        /// <summary>
        /// 根据报表源ID获取报表数据源
        /// </summary>
        /// <param name="reportPage"></param>
        /// <param name="reportSourceId"></param>
        /// <returns></returns>
        private ReportSource GetReportSourceById(ReportPage reportPage, string reportSourceId)
        {
            foreach (ReportSource source in reportPage.ReportSources)
            {
                if (source.ObjectID == reportSourceId)
                {
                    return source;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取报表展示数据（返回报表所需数据格式数据）
        /// </summary>
        /// <param name="reportPage"></param>
        /// <param name="reportWidget"></param>
        /// <param name="SourceColumns"></param>
        /// <param name="BeginNum"></param>
        /// <param name="PageSize"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> LoadGridData(ReportPage reportPage, ReportWidget reportWidget, List<ReportWidgetColumn> SourceColumns, int BeginNum, int PageSize, out int Count)
        {
            string DbCode = "";
            Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
            List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
            ReportSource reportSource = this.GetReportSourceById(reportPage, reportWidget.ReportSourceId);
            DbCode = reportSource.DataSourceCoding;
            List<string> Conditions = new List<string>();
            List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
            string tablename = reportWidget.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, reportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
            List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
            List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                tablename,
                SourceColumns.ToArray(),
                null,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                BeginNum,
                BeginNum + PageSize - 1,
                out Count,
                Conditions,
                Parameters,
                reportSource,
                 true);
            return SourceData;

        }



        /// <summary>
        /// 获取数据字典分类
        /// </summary>
        /// <returns>数据字典分类</returns>
        private object GetCategoryList()
        {
            Dictionary<string, string> table = this.Engine.MetadataRepository.GetCategoryTable();
            // 分类列表数据
            var category = table.Select(s => new
            {
                text = s.Value,
                id = s.Key
            });
            return category;
        }

        /// <summary>
        /// 获取数据源列的集合
        /// </summary>
        /// <param name="reportPage"></param>
        /// <param name="Filters"></param>
        /// <param name="reportWidget"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        private List<ReportWidgetColumn> GetSourceColumns(ReportPage reportPage, ReportFilter[] Filters, ReportWidget reportWidget, out string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                ReportSource curSource = null;
                if (reportPage.ReportSources != null && reportPage.ReportSources.Length > 0)
                {
                    foreach (ReportSource source in reportPage.ReportSources)
                    {
                        if (source.ObjectID == reportWidget.ReportSourceId)
                        {
                            curSource = source;
                            break;
                        }
                    }
                    if (curSource == null) return null;
                    List<ReportWidgetColumn> SourceColumns = this.Engine.ReportQuery.GetSourceColumns(this.Engine, reportWidget, curSource);
                    List<ReportWidgetColumn> results = new List<ReportWidgetColumn>();
                    foreach (ReportWidgetColumn c in SourceColumns)
                    {
                        if (reportWidget.ContainColumn(Filters, c.ColumnCode, curSource, this.Engine.BizObjectManager, this.Engine.Organization, this.Engine.EngineConfig.DBType, c.DisplayName))
                        {
                            results.Add(c);
                        }
                    }
                    return results;
                }

                return null;
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                return null;
            }

        }
        /// <summary>
        /// 获取简易看板数据源的列集合
        /// </summary>
        /// <param name="reportPage"></param>
        /// <param name="Filters"></param>
        /// <param name="simpleBoard"></param>
        /// <param name="dbType"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        private List<ReportWidgetColumn> GetSourceColumns(ReportPage reportPage, ReportFilter[] Filters, ReportWidgetSimpleBoard simpleBoard, OThinker.Data.Database.DatabaseType dbType, out string ErrorMsg)
        {
            ErrorMsg = "";
            try
            {
                ReportSource curSource = null;
                if (reportPage.ReportSources != null && reportPage.ReportSources.Length > 0)
                {
                    foreach (ReportSource source in reportPage.ReportSources)
                    {
                        if (source.ObjectID == simpleBoard.ReportSourceId)
                        {
                            curSource = source;
                            break;
                        }
                    }
                    if (curSource == null) return null;
                    List<ReportWidgetColumn> SourceColumns = this.Engine.ReportQuery.GetSourceColumns(this.Engine, simpleBoard, curSource);
                    List<ReportWidgetColumn> results = new List<ReportWidgetColumn>();
                    foreach (ReportWidgetColumn c in SourceColumns)
                    {
                        if (simpleBoard.ContainColumn(Filters, c.ColumnCode, curSource, this.Engine.BizObjectManager, this.Engine.Organization, dbType))
                        {
                            results.Add(c);
                        }
                    }
                    return results;
                }

                return null;
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message;
                return null;
            }
        }

        #endregion


        #region 未完成代码（已注释）
        ///// <summary>
        /// 获取H3计算字段的类型 （这个接口未完成）
        /// </summary>
        /// <param name="column"></param>
        /// <param name="ReportSource"></param>
        /// <param name="ReportPageCode"></param>
        /// <returns></returns>
        //public JsonResult GetFormulaRealyH3Type(ReportWidgetColumn column, ReportSource ReportSource, string ReportPageCode, string DbCode)
        //{
        //    AjaxContext AjaxContext = new AjaxContext();
        //    ReportWidget Setting = new ReportWidget();
        //    Setting.Columns = new ReportWidgetColumn[] { column };
        //    Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
        //    List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(null, Setting, ReportSource, out ChildCodes);
        //    List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
        //    Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
        //    List<string> Conditions = new List<string>();
        //    List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
        //    List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
        //    string tablename = Setting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, null, DataAclScope, SourceColumns, ReportPageCode, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
        //    List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
        //        tablename,
        //        SourceColumns.ToArray(),
        //        null,
        //        null,
        //        out statisticalColumn,
        //        out UnitTableAndAssociationTable,
        //        out TypeChangedColumns,
        //        Conditions,
        //        Parameters,
        //        ReportSource,
        //        0,
        //        1
        //        );
        //    if (TypeChangedColumns.Count == 0)
        //    {
        //        AjaxContext.ErrorMessage = "请添加数据，确定计算字段的类型";
        //        return Json(AjaxContext);
        //    }
        //    else
        //    {
        //        AjaxContext.Result["ColumnType"] = TypeChangedColumns[0].ColumnType;
        //        return Json(AjaxContext);
        //    }
        //}

        #endregion
    }

    #region 表单对象属性
    /// <summary>
    /// 表单对象属性
    /// </summary>
    public class Sheet
    {
        public string SchemaCode { get; set; }
        public string DisplayName { get; set; }
    }
    #endregion
}
