using NPOI.SS.UserModel;
using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 数据模型控制器
    /// </summary>
    [Authorize]
    public class BizObjectSchemaController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        private const string ExcelName = "ImportTemplate";

        #region SchemaCode参数
        protected string SchemaCode { get; set; }

        #endregion
        #region 初始化BizObjectSchme当前对象
        /// <summary>
        /// 初始化当前对象
        /// </summary>
        protected DataModel.BizObjectSchema Schema;
        /// <summary>
        /// 判断是否发布后的数据模型，来控制数据项显示是否发布
        /// </summary>
        protected DataModel.BizObjectSchema PublishedSchema;
        private bool ParseParam()
        {
            string code = SchemaCode;
            if (string.IsNullOrEmpty(code))
            {
                this.Schema = null;
            }
            else
            {
                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(code);
            }

            if (this.Schema == null)
            {
                return false;
                //业务对象模式不存在，或者已经被删除
                //this.ShowErrorMessage(this.PortalResource.GetString("EditBizObjectSchema_Msg0"));
                //this.CloseCurrentTabPage();
                //Response.End();
            }
            this.PublishedSchema = this.Engine.BizObjectManager.GetPublishedSchema(this.Schema.SchemaCode);
            return true;
        }

        /// <summary>
        /// 是否主数据
        /// </summary>
        protected bool isMaster
        {
            get
            {
                FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(this.SchemaCode);
                if (node != null)
                {
                    FunctionNode parentNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(node.ParentCode);
                    if (parentNode != null)
                    {
                        return parentNode.NodeType == FunctionNodeType.BizFolder;
                    }
                }
                return false;
            }
        }

        #endregion
        #region 导出主数据变量
        XmlDocument XmlDoc = new XmlDocument();

        private XmlElement CreateXmlElement(string name, string value)
        {
            XmlElement element = XmlDoc.CreateElement(name);
            element.InnerText = value;
            return element;
        }
        #endregion


        /// <summary>
        /// 获取数据模型基本信息
        /// </summary>
        /// <param name="parentId">父流程包ID</param>
        /// <param name="SchemaCode">流程包编码</param>
        /// <returns>数据模型基本信息</returns>
        [HttpGet]
        public JsonResult GetBizObjectSchemaPageData(string parentId, string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                bool IsBizObject;
                BizObjectSchemaViewModel model = GetBizObjectSchema(parentId, out IsBizObject);
                var propertySchemaList = GetPropertySchemaList();
                Dictionary<string, string> constString = new Dictionary<string, string>();
                constString.Add("SchemCodeStr", ConstantString.Param_SchemaCode);
                constString.Add("ParentProperty", ConstantString.Param_ParentProperty);
                constString.Add("ParamProperty", ConstantString.Param_Property);

                bool isImport = false;
                if (this.PublishedSchema == null || (this.PublishedSchema != null && this.PublishedSchema.StorageType != StorageType.DataList && isMaster))
                {
                    isImport = true;
                }
                int isLocked = 1;
                if (BizWorkflowPackageLockByID(this.SchemaCode) == 0 || model.IsQuotePacket)
                {
                    isLocked = 0;
                }
                result.Extend = new
                {
                    BizObjectSchema = model,
                    IsBizObject = IsBizObject,
                    IsMaster = isMaster,
                    IsImport = isImport,
                    IsLocked = isLocked,
                    PropertySchemaList = propertySchemaList,
                    ConstString = constString
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);

            });
        }

        /// <summary>
        /// 获取属性列表数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>属性列表</returns>
        [HttpPost]
        public JsonResult GetPropertySchemaList(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var propertySchemaList = GetPropertySchemaList();
                result.Success = true;
                result.Extend = propertySchemaList;
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除主数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteBizObjectSchema(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(this.SchemaCode);
                if (node.NodeType != FunctionNodeType.BizObject)
                {
                    result.Message = "EditBizObjectSchema.Msg4";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //该接口会删除数据模型数据
                this.Engine.LogWriter.Write(string.Format("--------------------->删除节点{0}，操作用户{1}",
                            node.DisplayName,
                            this.UserValidator.UserCode));
                this.Engine.AppPackageManager.DeleteAppPackage(node);
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 拖拽调整属性的排序
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="currentItemName">当前属性名称</param>
        /// <param name="targetItemName">调整位置属性名称</param>
        /// <param name="parentItemName">父属性名称</param>
        /// <param name="changeType">改变类型:上/下</param>
        /// <returns>调整结果</returns>
        [HttpGet]
        public JsonResult DragProperty(string schemaCode, string currentItemName, string targetItemName, string parentItemName, string changeType)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                BizObjectSchema currentSchema = string.IsNullOrWhiteSpace(parentItemName) ? this.Schema : this.Schema.GetProperty(parentItemName).ChildSchema;
                List<PropertySchema> propertys = currentSchema.Properties.Where(p => !DataModel.BizObjectSchema.IsReservedProperty(p.Name)).ToList();
                int targetIndex = propertys.FindIndex(p => p.Name.Equals(targetItemName, StringComparison.InvariantCultureIgnoreCase));
                propertys.Remove(currentSchema.GetProperty(currentItemName));
                propertys.Insert(targetIndex, currentSchema.GetProperty(currentItemName));
                //foreach (PropertySchema p in propertys)
                //{
                //    currentSchema.RemoveProperty(p.Name);
                //}
                foreach (PropertySchema p in propertys)
                {
                    currentSchema.SetPropertyIndex(p);
                }
                result.Success = this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 发布数据模型
        /// </summary>
        /// <param name="SchemaCode">流程包编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult PublishBizObjectSchema(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string message = null;
                if (!this.Engine.BizObjectManager.PublishSchema(this.Schema.SchemaCode, out message))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.PublishFailed";
                    result.Extend = message;
                }
                else
                {
                    this.Schema = this.Engine.BizObjectManager.GetDraftSchema(this.Schema.SchemaCode);
                    result.Success = true;
                    result.Message = "EditBizObjectSchema.PublishedSuccess";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存数据模型
        /// </summary>
        /// <param name="model">数据模型信息</param>
        /// <returns>是否保存成功</returns>
        [HttpPost]
        public JsonResult SaveBizObjectSchema(BizObjectSchemaViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = model.Code;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.DisplayName))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.DisplayNameNotNull";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                this.Schema.DisplayName = model.DisplayName;
                this.Schema.Description = model.Description;
                this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);

                // 状态
                DataModel.BizObjectSchemaState state = DataModel.BizObjectSchemaState.Active;
                // 更新状态
                this.Engine.BizObjectManager.SetPublishedSchemaState(this.Schema.SchemaCode, state);
                //更新FuntionNode
                FunctionNode funcNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(this.Schema.SchemaCode);
                if (funcNode.NodeType == FunctionNodeType.BizObject)
                {
                    int sortKey;
                    if (Int32.TryParse(model.SortKey, out sortKey))
                        funcNode.SortKey = sortKey;
                    funcNode.DisplayName = model.DisplayName;
                    this.Engine.FunctionAclManager.UpdateFunctionNode(funcNode);
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);

            });
        }

        /// <summary>
        /// 上传导入文件
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>上传结果</returns>
        [HttpPost]
        public JsonResult UploadMasterDataFile(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
                var files = Request.Files;
                List<ImportError> msg = new List<ImportError>();
                DataTable data = new DataTable();
                var file = files[0];
                if (file.ContentLength == 0)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectItem";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);

                };
                string sheetName = "sheet1";
                bool isFirstRowColumn = true;
                IWorkbook workbook = null;
                ISheet sheet = null;
                int startRow = 0;
                try
                {
                    workbook = WorkbookFactory.Create(file.InputStream);

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
                        {//每一行
                            IRow row = sheet.GetRow(j);
                            if (row == null) continue; //没有数据的行默认是null　　　　　　　

                            DataRow dataRow = data.NewRow();
                            for (int k = row.FirstCellNum; k < cellCount; ++k)
                            {//每一列
                                if (row.GetCell(k) != null) //同理，没有数据的单元格都默认是null
                                {
                                    switch (row.GetCell(k).CellType)
                                    {
                                        case CellType.String:
                                            dataRow[k] = row.GetCell(k).StringCellValue;
                                            break;
                                        case CellType.Numeric:
                                            var logicType = this.Schema.GetLogicType(firstRow.Cells[k].StringCellValue);
                                            if (logicType == OThinker.H3.Data.DataLogicType.DateTime)
                                            {
                                                dataRow[k] = row.GetCell(k).DateCellValue;
                                            }
                                            else if (logicType == OThinker.H3.Data.DataLogicType.TimeSpan)
                                            {
                                                dataRow[k] = row.GetCell(k).DateCellValue.TimeOfDay;
                                            }
                                            else
                                            {
                                                dataRow[k] = row.GetCell(k).NumericCellValue;
                                            }

                                            break;
                                        case CellType.Boolean:
                                            dataRow[k] = row.GetCell(k).BooleanCellValue;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            data.Rows.Add(dataRow);
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    //stream.Close();
                }
                result = SaveUploadBizMasterData(data);

                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取模板下载地址
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetImportTemplete(string schemaCode)
        {
            this.SchemaCode = schemaCode;
            ActionResult result = new ActionResult();
            if (!this.ParseParam())
            {
                result.Success = false;
                result.Message = "EditBizObjectSchema.Msg0";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            result.Extend = ExportExcel();
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 导出主数据
        /// </summary>
        /// <param name="bizMasterDataCode"></param>
        [HttpGet]
        public void ExportBizMasterData(string bizMasterDataCode)
        {
            ExecuteFunctionRun(() =>
           {

               Acl.FunctionNode masterDataNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(bizMasterDataCode);

               //主数据根节点
               XmlElement bizMasterData = XmlDoc.CreateElement(FunctionNodeType.BizObject.ToString());
               //数据模型
               BizObjectSchema Schema = this.Engine.BizObjectManager.GetDraftSchema(bizMasterDataCode);
               if (Schema != null) { bizMasterData.InnerXml += Convertor.ObjectToXml(Schema); }
               //监听实例
               BizListenerPolicy policy = this.Engine.BizObjectManager.GetListenerPolicy(bizMasterDataCode);
               if (policy != null)
               {
                   bizMasterData.InnerXml += Convertor.ObjectToXml(policy);
               }
               //定时作业
               ScheduleInvoker[] scheduleInvokers = this.Engine.BizObjectManager.GetScheduleInvokerList(bizMasterDataCode);
               if (scheduleInvokers != null && scheduleInvokers.Length > 0)
               {
                   XmlElement invokers = XmlDoc.CreateElement("ScheduleInvokers");
                   foreach (ScheduleInvoker item in scheduleInvokers)
                   {
                       invokers.InnerXml += Convertor.ObjectToXml(item);
                   }
                   bizMasterData.AppendChild(invokers);
               }
               //查询列表
               BizQuery[] queries = this.Engine.BizObjectManager.GetBizQueries(bizMasterDataCode);
               if (queries != null && queries.Length > 0)
               {
                   XmlElement bizQueries = XmlDoc.CreateElement("BizQueries");
                   foreach (BizQuery query in queries)
                   {
                       bizQueries.InnerXml += Convertor.ObjectToXml(query);
                   }
                   bizMasterData.AppendChild(bizQueries);
               }
               XmlDoc.AppendChild(bizMasterData);

               //导出文件
               string path = Server.MapPath("~/TempImages/");
               string fileName = bizMasterDataCode + ".xml";

               XmlDoc.Save(path + fileName);
               this.Response.Clear();
               this.Response.ContentType = "text/xml";
               this.Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
               this.Response.TransmitFile(path + fileName);
               this.Response.End();
               System.IO.File.Delete(path + fileName);
               return null;
           });
        }

        /// <summary>
        /// 校验数据
        /// </summary>
        /// <param name="model">数据模型</param>
        /// <returns>校验结果</returns>
        [HttpPost]
        public JsonResult ValidateBizObjectSchema(BizObjectSchemaViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = model.Code;
                if (!this.ParseParam())
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                H3.ValidationResult r = this.Engine.BizObjectManager.ValidateBizObjectSchema(this.Schema.SchemaCode);
                result.Success = r.Valid;
                result.Message = r.Valid ? "EditBizObjectSchema.Msg18" : r.ToString().Replace(System.Environment.NewLine, "<br>").Replace(";", "<br>");

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 保存数据模板导入数据到数据库
        /// </summary>
        /// <param name="dt">要保存的数据表</param>
        /// <returns>保存结果</returns>
        public ActionResult SaveUploadBizMasterData(DataTable dt)
        {

            ActionResult result = new ActionResult();
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    List<string> columNames = new List<string>();
                    List<string> fields = new List<string>();
                    foreach (DataColumn c in dt.Columns)
                    {
                        columNames.Add(c.ColumnName);
                    }
                    BizObjectSchema[] schemas = this.Engine.BizObjectManager.GetPublishedSchemas();
                    foreach (DataRow r in dt.Rows)
                    {
                        DataModel.BizObject bo = new DataModel.BizObject(
                                    this.Engine.Organization,
                                    this.Engine.MetadataRepository,
                                    this.Engine.BizObjectManager,
                                    null,//bizBus
                                    this.PublishedSchema,
                                    this.UserValidator.UserID,
                                    this.UserValidator.User.ParentID);
                        bool success = true;
                        foreach (string columName in columNames)
                        {
                            if (bo.Schema.GetField(columName) != null)
                                bo[columName] = r[columName];
                            else
                            {
                                if (!fields.Contains(columName))
                                {
                                    fields.Add(columName);
                                    success = false;
                                }
                            }
                        }
                        if (success)
                            bo.Create();
                    }
                    if (fields.Count > 0)
                    {
                        List<string> errors = new List<string>();
                        foreach (string field in fields.ToArray())
                        {
                            errors.Add(field);
                            //faileds += field + "、";
                        }
                        result.Success = false;
                        result.Message = "EditBizObjectSchema.Msg15";//MasterData_Mssg15
                        result.Extend = errors;
                    }
                    else
                    {
                        result.Success = true;
                        result.Message = "EditBizObjectSchema.Msg9";//MasterData_Mssg9
                        //this.CloseCurrentDialog();
                        //RegisterScriptManager("CloseCurrentDialog", "setTimeout('CloseCurrentDialog()', 2000)");
                    }
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg7";//7
                    result.Extend = e.Message;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "EditBizObjectSchema.Msg17";//17
            }
            return result;
        }

        /// <summary>
        /// 导出Excel表格
        /// </summary>
        /// <returns>下载路径</returns>
        protected string ExportExcel()
        {
            Dictionary<string, string> heads = new Dictionary<string, string>();
            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (PropertySchema p in this.PublishedSchema.Properties)
            {
                if (this.PublishedSchema.GetAssociation(p.Name) == null)
                {
                    if (!DataModel.BizObjectSchema.IsReservedProperty(p.Name))
                    {
                        heads.Add(p.Name, p.Name);
                        data.Add(p.Name, "");
                    }
                }
            }
            datas.Add(data);
            return OThinker.H3.Controllers.ExportExcel.ExportExecl(ExcelName, heads, datas);
        }
        /// <summary>
        /// 获取属性列表并转化为ligerUI格式
        /// </summary>
        /// <returns>属性列表</returns>
        private object GetPropertySchemaList()
        {

            List<PropertySchemaViewModel> dataList = new List<PropertySchemaViewModel>();
            dataList.AddRange(GetBizObjectChildren(this.Schema.Properties, this.PublishedSchema));
            var jsonObj = new { Rows = dataList, Total = dataList.Count };
            return jsonObj;
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="properties">业务对象属性模式</param>
        /// <param name="publishedSchema">数据模型</param>
        /// <param name="parentPropertyPath">父级路径</param>
        /// <returns>数据模型属性列表</returns>
        private List<PropertySchemaViewModel> GetBizObjectChildren(DataModel.PropertySchema[] properties, DataModel.BizObjectSchema publishedSchema, string parentPropertyPath = "")
        {
            //排序
            //properties = properties.OrderBy(p => p.Seq).ToArray();
            List<PropertySchemaViewModel> list = new List<PropertySchemaViewModel>();
            string parentProperty;
            DataModel.BizObjectSchema chirenSchema = null;
            DataModel.PropertySchema publishedProperty = null;
            foreach (DataModel.PropertySchema item in properties)
            {
                if (this.Schema.GetAssociation(item.Name) == null)
                {
                    parentProperty = parentPropertyPath + "\\" + item.Name;
                    //全局变量
                    var logicType = item.SourceType == SourceType.Metadata ? OThinker.H3.Data.DataLogicType.GlobalData : item.LogicType;
                    PropertySchemaViewModel model = new PropertySchemaViewModel()
                    {
                        ParentProperty = parentProperty,
                        ParentItemName = parentPropertyPath,
                        ItemName = item.Name,
                        LogicType = logicType.ToString(),
                        LogicTypeName = Data.DataLogicTypeConvertor.ToLogicTypeName((Data.DataLogicType)logicType),
                        ChildSchemaCode = item.ChildSchema == null ? null : item.ChildSchema.SchemaCode,
                        DisplayName = string.Format("{0} [{1}]", item.DisplayName, item.Name),
                        DefaultValue = item.DefaultValue == null ? null : item.DefaultValue.ToString(),
                        Trackable = item.Trackable,
                        Indexed = item.Indexed,
                        Searchable = item.Searchable,
                        Abstract = item.Abstract,
                        IsReserved = DataModel.BizObjectSchema.IsReservedProperty(item.Name)

                    };
                    //判断是否发布
                    publishedProperty = publishedSchema != null ? publishedSchema.GetProperty(item.Name) : null;
                    if (publishedProperty != null
                        && publishedProperty.LogicType == item.LogicType)
                    {
                        model.IsPublished = true;
                        chirenSchema = publishedSchema.GetProperty(item.Name).ChildSchema;
                    }
                    else
                        model.IsPublished = false;

                    if (item.ChildSchema != null && item.ChildSchema.Properties != null)
                        model.children = GetBizObjectChildren(item.ChildSchema.Properties, chirenSchema, parentProperty);
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取数据模型对象基础信息
        /// </summary>
        /// <param name="parentId">父ID</param>
        /// <param name="IsBizObject">是否是数据模型</param>
        /// <returns>数据模型对象基础信息</returns>
        private BizObjectSchemaViewModel GetBizObjectSchema(string parentId, out bool IsBizObject)
        {

            var bizTreeNode = this.Engine.FunctionAclManager.GetFunctionNode(parentId);

            BizObjectSchema ownSchema = null;
            if (bizTreeNode.Code != this.Schema.SchemaCode) {
                ownSchema = this.Engine.BizObjectManager.GetDraftSchema(bizTreeNode.Code);
            }

            BizObjectSchemaViewModel model = new BizObjectSchemaViewModel()
            {
                Code = this.Schema.SchemaCode,
                Description = this.Schema.Description,
                DisplayName = this.Schema.DisplayName,
                Type = "WorkflowPackage." + this.Schema.StorageType.ToString(),
                ParentId = parentId,
                IsQuotePacket = ownSchema==null?false:ownSchema.IsQuotePacket
                //ObjectID=this.Schema.ObjectTypeId,
            };
            IsBizObject = false;
            //是否可以删除
            if (!string.IsNullOrEmpty(this.SchemaCode))
            {
                FunctionNode node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(this.SchemaCode);
                if (node != null)
                {
                    model.SortKey = node.SortKey.ToString();
                    IsBizObject = (node.NodeType == FunctionNodeType.BizObject);//TODO 感觉这里永远为False?
                }
            }
            return model;
        }
    }


}
