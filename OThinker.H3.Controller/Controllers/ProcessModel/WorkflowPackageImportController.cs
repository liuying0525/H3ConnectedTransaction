using Newtonsoft.Json;
using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Sheet;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class WorkflowPackageImportController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }


        private string PackageCode = string.Empty;
        private string PackageName = string.Empty;

        protected string BizQueryCode = "BizQueryCode";

        //流程包
        FunctionNode package;
        //数据模型
        DataModel.BizObjectSchema BizObjectSchema;
        DataModel.BizListenerPolicy bizListenerPolicy;
        List<DataModel.ScheduleInvoker> scheduleInvokerList = new List<ScheduleInvoker>();
        List<DataModel.BizQuery> bizQueryList = new List<BizQuery>();
        //表单
        List<BizSheet> BizSheets = new List<BizSheet>();
        //流程
        List<DraftWorkflowTemplate> WorkflowTemplates = new List<DraftWorkflowTemplate>();
        List<InstanceSimulation> Simulations = new List<InstanceSimulation>();
        List<WorkflowClause> clauses = new List<WorkflowClause>();

        Dictionary<string, string> WorkflowNames = new Dictionary<string, string>();
        XmlDocument XmlDoc = new XmlDocument();
        private XmlElement CreateXmlElement(string name, string value)
        {
            XmlElement element = XmlDoc.CreateElement(name);
            element.InnerText = value;
            return element;
        }

        /// <summary>
        /// 获取上级目录名称
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public JsonResult GetParentFolderName(string ParentID)
        {
            return ExecuteFunctionRun(() =>
            {
                var displayName = this.Engine.FunctionAclManager.GetFunctionNode(ParentID).DisplayName;
                return Json(displayName, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 导出流程包
        /// </summary>
        /// <param name="PackageCode"></param>
        /// <returns></returns>
        public object Export(string PackageCode)
        {
            return ExecuteFileResultFunctionRun(() =>
            {

                Acl.FunctionNode PackageNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(PackageCode);

                //流程包
                XmlElement BizWorkflowPackage = XmlDoc.CreateElement(FunctionNodeType.BizWorkflowPackage.ToString());
                BizWorkflowPackage.AppendChild(CreateXmlElement("PackageCode", PackageNode.Code));
                BizWorkflowPackage.AppendChild(CreateXmlElement("PackageName", PackageNode.DisplayName));
                XmlDoc.AppendChild(BizWorkflowPackage);
                BizWorkflowPackage.InnerXml += Convertor.ObjectToXml(PackageNode);

                //数据模型
                BizObjectSchema Schema = this.Engine.BizObjectManager.GetDraftSchema(PackageCode);
                if (Schema != null)
                {
                    BizWorkflowPackage.InnerXml += Convertor.ObjectToXml(Schema);

                    //监听实例
                    BizListenerPolicy policy = this.Engine.BizObjectManager.GetListenerPolicy(Schema.SchemaCode);
                    if (policy != null)
                    {
                        BizWorkflowPackage.InnerXml += Convertor.ObjectToXml(policy);
                    }
                    //定时作业
                    ScheduleInvoker[] scheduleInvokers = this.Engine.BizObjectManager.GetScheduleInvokerList(Schema.SchemaCode);
                    if (scheduleInvokers != null && scheduleInvokers.Length > 0)
                    {
                        XmlElement invokers = XmlDoc.CreateElement("ScheduleInvokers");
                        foreach (ScheduleInvoker item in scheduleInvokers)
                        {
                            invokers.InnerXml += Convertor.ObjectToXml(item);
                        }
                        BizWorkflowPackage.AppendChild(invokers);
                    }
                    //查询列表
                    BizQuery[] queries = this.Engine.BizObjectManager.GetBizQueries(Schema.SchemaCode);
                    if (queries != null && queries.Length > 0)
                    {
                        XmlElement bizQueries = XmlDoc.CreateElement("BizQueries");
                        foreach (BizQuery query in queries)
                        {
                            bizQueries.InnerXml += Convertor.ObjectToXml(query);
                        }
                        BizWorkflowPackage.AppendChild(bizQueries);
                    }
                }

                //表单
                BizSheet[] Sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(PackageCode);
                XmlElement workflowSheets = XmlDoc.CreateElement("BizSheets");
                if (Sheets != null)
                {
                    foreach (BizSheet s in Sheets)
                    {
                        workflowSheets.InnerXml += Convertor.ObjectToXml(s);
                    }
                }
                BizWorkflowPackage.AppendChild(workflowSheets);

                //流程
                WorkflowTemplate.WorkflowClause[] WorkflowClauses = this.Engine.WorkflowManager.GetClausesBySchemaCode(PackageCode);
                XmlElement workflowTemps = XmlDoc.CreateElement("WorkflowTemplates");
                if (WorkflowClauses != null)
                {
                    foreach (WorkflowTemplate.WorkflowClause WorkflowClause in WorkflowClauses)
                    {
                        WorkflowTemplate.DraftWorkflowTemplate workflowTemplate = this.Engine.WorkflowManager.GetDraftTemplate(WorkflowClause.WorkflowCode);
                        //发布后才能导出
                        if (workflowTemplate == null) continue;
                        XmlElement workflowTempElement = XmlDoc.CreateElement("WorkflowTemplate");
                        //流程名称
                        workflowTempElement.AppendChild(this.CreateXmlElement("WorkflowTemplateName", WorkflowClause.WorkflowName));
                        //流程图
                        XmlElement workflowDocument = XmlDoc.CreateElement("WorkflowDocument");
                        workflowTemplate.SaveAsXml(XmlDoc, workflowDocument);
                        workflowTempElement.AppendChild(workflowDocument);

                        //流程模拟
                        XmlElement simulationElement = XmlDoc.CreateElement("Simulations");
                        InstanceSimulation[] simulations = this.Engine.SimulationManager.GetSimulationByWorkflow(workflowTemplate.WorkflowCode);
                        if (simulations != null && simulations.Length > 0)
                            foreach (InstanceSimulation simulation in simulations)
                            {
                                simulationElement.InnerXml += Convertor.ObjectToXml(simulation);
                            }
                        workflowTempElement.AppendChild(simulationElement);

                        //流程模板簇
                        workflowTempElement.InnerXml += Convertor.ObjectToXml(WorkflowClause);

                        //添加流程模板
                        workflowTemps.AppendChild(workflowTempElement);
                    }
                }
                BizWorkflowPackage.AppendChild(workflowTemps);

                //导出文件
                string path = Server.MapPath("~/TempImages/");
                string fileName = PackageCode + ".xml";

                XmlDoc.Save(path + fileName);

                return File(path + fileName, "application/octect-stream", fileName);
            });
        }

        /// <summary>
        /// 导入模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Import(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                WorkflowPackageImportViewModel model = new WorkflowPackageImportViewModel();
                model = JsonConvert.DeserializeObject<WorkflowPackageImportViewModel>(formData);

                ReadXmlFile(model);
                ActionResult result = new ActionResult(false, "");
                if (package == null)
                {
                    result.Message = "WorkflowPackageImport.PackageImportHandler_Mssg1";

                    return Json(result);
                }

                #region 将界面修改的code更新到导入的实体中

                if (model.QueryList != null && model.QueryList.Count > 0)
                {
                    int index = 0;
                    foreach (ItemDetail item in model.QueryList)
                    {
                        this.bizQueryList.ForEach(i => { if (i.QueryCode == item.OldCode) { i.QueryCode = item.Code; } });
                    }
                }

                //
                if (model.WorkflowPackage != null)
                {
                    this.PackageCode = this.package.Code = this.BizObjectSchema.SchemaCode = model.WorkflowPackage.Code;
                    //更新流程模板对象的数据模型编码
                    this.WorkflowTemplates.ForEach(i => i.BizObjectSchemaCode = model.WorkflowPackage.Code);
                    this.clauses.ForEach(i => i.BizSchemaCode = model.WorkflowPackage.Code);
                    //更新表单对象的数据模型编码
                    this.BizSheets.ForEach(i => i.BizObjectSchemaCode = model.WorkflowPackage.Code);
                    //更新数据模型中子对象的数据模型code
                    if (scheduleInvokerList != null)
                    {
                        scheduleInvokerList.ForEach(i => i.SchemaCode = model.WorkflowPackage.Code);
                    }
                    if (bizQueryList != null)
                    {
                        bizQueryList.ForEach(i => i.SchemaCode = model.WorkflowPackage.Code);
                    }
                }

                //
                if (model.BizSheets != null && model.BizSheets.Count > 0)
                {
                    foreach (ItemDetail item in model.BizSheets)
                    {
                        this.BizSheets.ForEach(i => { if (i.SheetCode == item.OldCode) { i.SheetCode = item.Code; } });
                    }
                }
                //流程模板
                if (model.WorkFlows != null)
                {
                    foreach (ItemDetail im in model.WorkFlows)
                    {
                        if (im.OldCode != im.Code)
                        {
                            this.WorkflowTemplates.ForEach(i => { if (i.WorkflowCode == im.OldCode) { i.WorkflowCode = im.Code; } });
                            this.Simulations.ForEach(i => { if (i.WorkflowCode == im.OldCode) { i.WorkflowCode = im.Code; } });
                            this.clauses.ForEach(i => { if (i.WorkflowCode == im.OldCode) { i.WorkflowCode = im.Code; } });

                            WorkflowNames.Add(im.Code, WorkflowNames[im.OldCode]);
                            WorkflowNames.Remove(im.OldCode);
                        }
                    }
                }

                #endregion

                #region 处理脏数据：是否序列化，ObjectID
                bool isCover = model.IsCover;
                //bool.TryParse(model.IsCover, out isCover);
                //查询
                foreach (BizQuery query in this.bizQueryList)
                {
                    //BizQuery serverQuery = this.Engine.BizObjectManager.GetBizQuery(query.QueryCode);
                    //if (serverQuery == null)
                    //{
                    query.Serialized = false;
                    query.ObjectID = Guid.NewGuid().ToString();

                    if (query.Columns != null)
                    {
                        foreach (BizQueryColumn c in query.Columns)
                        {
                            c.ObjectID = Guid.NewGuid().ToString();
                            c.Serialized = false;
                        }
                    }

                    if (query.BizActions != null)
                    {
                        foreach (BizQueryAction a in query.BizActions)
                        {
                            a.ObjectID = Guid.NewGuid().ToString();
                            a.Serialized = false;
                        }
                    }

                    if (query.QueryItems != null)
                    {
                        foreach (BizQueryItem i in query.QueryItems)
                        {
                            i.ObjectID = Guid.NewGuid().ToString();
                            i.Serialized = false;
                        }
                    }
                    //}
                    //else if (isCover)
                    //{
                    //    query.ObjectID = serverQuery.ObjectID;
                    //}
                }

                //定时作业
                foreach (var scheduleInvoker in scheduleInvokerList)
                {
                    scheduleInvoker.ObjectID = Guid.NewGuid().ToString();
                    scheduleInvoker.Serialized = false;
                }

                //表单
                foreach (BizSheet sheet in this.BizSheets)
                {
                    BizSheet serverSheet = this.Engine.BizSheetManager.GetBizSheetByCode(sheet.SheetCode);
                    if (serverSheet == null)
                    {
                        sheet.Serialized = false;
                        sheet.ObjectID = Guid.NewGuid().ToString();
                    }
                    else if (isCover)
                    {
                        sheet.ObjectID = serverSheet.ObjectID;
                    }
                }

                //流程模拟
                foreach (InstanceSimulation s in this.Simulations)
                {
                    s.Serialized = false;
                    s.ObjectID = Guid.NewGuid().ToString();
                }

                #endregion

                string resultStr = string.Empty;
                bool result2 = this.Engine.AppPackageManager.ImportAppPackage(
                        this.UserValidator.UserID,
                        model.ParentCode,
                        this.PackageCode,
                        this.PackageName,
                        this.BizObjectSchema,
                        this.bizListenerPolicy,
                        this.bizQueryList,
                        this.scheduleInvokerList,
                        this.BizSheets,
                        this.WorkflowTemplates,
                        WorkflowNames,
                    //this.Simulations,
                        isCover,
                        out resultStr);
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
        /// 上传模板
        /// </summary>
        /// <param name="ParentCode"></param>
        /// <returns></returns>
        public JsonResult Upload(string ParentCode)
        {
            return ExecuteFunctionRun(() =>
            {

                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;//传输的文件
                ActionResult result = new ActionResult(false, "");
                WorkflowPackageImportViewModel model = new WorkflowPackageImportViewModel();

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
                model.WorkflowPackage = new ItemDetail { CodeType = FunctionNodeType.BizWorkflowPackage.ToString(), Name = PackageName, Code = PackageCode, OldCode = PackageCode, Index = 1 };
                model.BizSchema = new ItemDetail { CodeType = FunctionNodeType.BizObject.ToString(), Name = BizObjectSchema.DisplayName, Code = BizObjectSchema.SchemaCode, OldCode = BizObjectSchema.SchemaCode, Index = 1 };


                if (BizSheets.Count > 0)
                {
                    List<ItemDetail> listSheets = new List<ItemDetail>();
                    int index = 1;
                    foreach (BizSheet sheet in BizSheets)
                    {
                        ItemDetail dl = new ItemDetail();
                        dl.Code = sheet.SheetCode;
                        dl.OldCode = sheet.SheetCode;
                        dl.Name = sheet.DisplayName;
                        dl.Index = index;
                        dl.CodeType = FunctionNodeType.BizSheet.ToString();
                        index++;

                        listSheets.Add(dl);
                    }

                    model.BizSheets = listSheets;
                }

                if (WorkflowTemplates.Count > 0)
                {
                    List<ItemDetail> listflow = new List<ItemDetail>();
                    int index = 1;
                    foreach (DraftWorkflowTemplate flow in WorkflowTemplates)
                    {
                        ItemDetail dl = new ItemDetail();
                        dl.Code = flow.WorkflowCode;
                        dl.OldCode = flow.WorkflowCode;
                        dl.Name = WorkflowNames.ContainsKey(flow.WorkflowCode) ? WorkflowNames[flow.WorkflowCode] : flow.WorkflowCode;
                        dl.Index = index;
                        dl.CodeType = FunctionNodeType.BizWorkflow.ToString();
                        index++;

                        listflow.Add(dl);
                    }

                    model.WorkFlows = listflow;
                }

                if (this.bizQueryList != null && this.bizQueryList.Count > 0)
                {
                    List<ItemDetail> listQuery = new List<ItemDetail>();
                    int index = 1;
                    foreach (BizQuery query in this.bizQueryList)
                    {
                        ItemDetail dl = new ItemDetail();
                        dl.Code = query.QueryCode;
                        dl.OldCode = query.QueryCode;
                        dl.Name = query.DisplayName;
                        dl.Index = index;
                        dl.CodeType = BizQueryCode;
                        index++;

                        listQuery.Add(dl);
                    }
                    model.QueryList = listQuery;
                }

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
        private void ReadXmlFile(WorkflowPackageImportViewModel model)
        {
            //从服务器加载
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Session[model.XMLString].ToString());
            XmlElement BizWorkflowPackage = xmlDoc.DocumentElement;//根节点

            //流程包编码、名称
            PackageCode = BizWorkflowPackage.GetElementsByTagName("PackageCode")[0].InnerXml;
            PackageName = BizWorkflowPackage.GetElementsByTagName("PackageName")[0].InnerXml;
            //流程包
            XmlNodeList packageNodes = BizWorkflowPackage.GetElementsByTagName("FunctionNode");
            if (packageNodes != null && packageNodes.Count > 0)
            {
                XmlNode openNewWindow = packageNodes[0].SelectSingleNode("OpenNewWindow");
                if (openNewWindow != null) openNewWindow.InnerText = openNewWindow.InnerText.ToLower();
                package = Convertor.XmlToObject(typeof(FunctionNode), packageNodes[0].OuterXml) as FunctionNode;
                package.ParentCode = model.ParentCode;
            }

            //数据模型
            XmlNodeList bizObjectSchemaNodes = BizWorkflowPackage.GetElementsByTagName("BizObjectSchema");
            if (bizObjectSchemaNodes != null && bizObjectSchemaNodes.Count > 0)
            {
                BizObjectSchema = (DataModel.BizObjectSchema)Convertor.XmlToObject(typeof(DataModel.BizObjectSchema), bizObjectSchemaNodes[0].OuterXml);
                //监听实例
                XmlNodeList bizListenerPolicyNodes = BizWorkflowPackage.GetElementsByTagName("BizListenerPolicy");
                if (bizListenerPolicyNodes != null && bizListenerPolicyNodes.Count > 0)
                {
                    bizListenerPolicy = Convertor.XmlToObject(typeof(BizListenerPolicy), bizListenerPolicyNodes[0].OuterXml) as BizListenerPolicy;
                }
                //定时作业
                XmlNodeList scheduleInvokerNodes = BizWorkflowPackage.GetElementsByTagName("ScheduleInvokers");
                if (scheduleInvokerNodes != null && scheduleInvokerNodes.Count > 0)
                {
                    scheduleInvokerList = new List<ScheduleInvoker>();
                    foreach (XmlNode scheduleInvoker in scheduleInvokerNodes[0].ChildNodes)
                    {
                        scheduleInvokerList.Add(Convertor.XmlToObject(typeof(ScheduleInvoker), scheduleInvoker.OuterXml) as ScheduleInvoker);
                    }
                }
                //查询列表
                XmlNodeList bizQueryNodes = BizWorkflowPackage.GetElementsByTagName("BizQueries");
                if (bizQueryNodes != null && bizQueryNodes.Count > 0)
                {
                    bizQueryList = new List<BizQuery>();
                    foreach (XmlNode query in bizQueryNodes[0].ChildNodes)
                    {
                        bizQueryList.Add(Convertor.XmlToObject(typeof(BizQuery), query.OuterXml) as BizQuery);
                    }
                }
            }

            //流程表单
            XmlNode SheetRoot = BizWorkflowPackage.GetElementsByTagName("BizSheets")[0];
            XmlNodeList Sheets = ((XmlElement)SheetRoot).GetElementsByTagName("BizSheet");
            BizSheets = new List<BizSheet>();
            foreach (XmlNode node in Sheets)
            {
                BizSheet sheet = (BizSheet)Convertor.XmlToObject(typeof(BizSheet), node.OuterXml);
                if (sheet.SheetType == SheetType.DefaultSheet)
                {//清空默认表单
                    sheet.SheetAddress = string.Empty;
                }
                var BizSheetModel = (BizSheet)Convertor.XmlToObject(typeof(BizSheet), node.OuterXml);
                //处理旧版本的默认表单的发起时间绑定字段是OriginateDate 而新版本的是OriginateTime 做兼容处理
                if ( BizSheetModel.DesignModeContent!=null) 
                BizSheetModel.DesignModeContent=BizSheetModel.DesignModeContent.Replace(@"OriginateDate", "OriginateTime");
                if (BizSheetModel.RuntimeContent != null) 
                BizSheetModel.RuntimeContent = BizSheetModel.RuntimeContent.Replace(@"OriginateDate", "OriginateTime");

                 
                BizSheets.Add(BizSheetModel);
            }

            //流程模板
            XmlNode TemplateRoot = BizWorkflowPackage.GetElementsByTagName("WorkflowTemplates")[0];
            XmlNodeList Templates = ((XmlElement)TemplateRoot).GetElementsByTagName("WorkflowTemplate");
            if (Templates.Count > 0)
            {
                WorkflowTemplates = new List<DraftWorkflowTemplate>();
                foreach (XmlNode node in Templates)
                {
                    DraftWorkflowTemplate draftWorkflowTemplate = new DraftWorkflowTemplate(((XmlElement)node).GetElementsByTagName("WorkflowDocument")[0].OuterXml);
                    WorkflowNames.Add(draftWorkflowTemplate.WorkflowCode, ((XmlElement)node).GetElementsByTagName("WorkflowTemplateName")[0].InnerXml);
                    WorkflowTemplates.Add(draftWorkflowTemplate);
                }
                //流程模拟
                XmlNode SimulationRoot = BizWorkflowPackage.GetElementsByTagName("Simulations")[0];
                XmlNodeList SimulationNodes = ((XmlElement)SimulationRoot).GetElementsByTagName("InstanceSimulation");
                this.Simulations = new List<InstanceSimulation>();
                foreach (XmlNode simulation in SimulationNodes)
                {
                    this.Simulations.Add((InstanceSimulation)Convertor.XmlToObject(typeof(InstanceSimulation), simulation.OuterXml));
                }

                XmlNodeList clauseNodes = BizWorkflowPackage.GetElementsByTagName("WorkflowClause");
                if (clauseNodes != null && clauseNodes.Count > 0)
                {
                    foreach (XmlNode clauseNode in clauseNodes)
                    {
                        this.clauses.Add((WorkflowClause)Convertor.XmlToObject(typeof(WorkflowClause), clauseNode.OuterXml));
                    }
                }
            }
        }



    }
}
