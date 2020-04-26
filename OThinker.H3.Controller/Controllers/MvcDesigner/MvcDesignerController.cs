using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.MvcDesigner
{
    public class MvcDesignerController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_ProcessModel_Code; }
        }

        /// <summary>
        /// 占2/12的css
        /// </summary>
        private const string CSS2 = "col-md-2";
        /// <summary>
        /// 占4/12的css
        /// </summary>
        private const string CSS4 = "col-md-4";
        /// <summary>
        /// 占10/12的css
        /// </summary>
        private const string CSS10 = "col-md-10";
        /// <summary>
        /// 节点HTML
        /// </summary>
        private const string NodeHtml = "<span data-datafield=\"{0}\" data-system=\"{1}\" data-displayname=\"{4}\" data-logictype=\"{2}\">{3}</span>";


        #region 获取引擎编码

        /// <summary>
        /// 获取当前引擎编码
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEngineCode()
        {
            return ExecuteFunctionRun(() =>
            {

                return Json(this.Engine.EngineConfig.Code, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region 获取当前正在编辑的表单对象

        /// <summary>
        /// 获取当前正在编辑的表单对象
        /// </summary>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        private BizSheet GetSheet(string SheetCode)
        {

            return this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);

        }

        #endregion


        /// <summary>
        /// 获取表单设计页面内容
        /// </summary>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        public JsonResult LoadPageContent(string SheetCode, string ParentID)
        {
            return ExecuteFunctionRun(() =>
            {

                MvcSheetViewModel model = new MvcSheetViewModel();
                BizSheet Sheet = GetSheet(SheetCode);
                if (Sheet == null) { return null; }
                BizObjectSchema Schema = this.Engine.BizObjectManager.GetPublishedSchema(Sheet.BizObjectSchemaCode);
                if (Schema == null)
                {
                    model.IsControlUsable = 0;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }

                //如果引用了流程包,将页面设置为不可编辑
                var bizTreeNode = this.Engine.FunctionAclManager.GetFunctionNode(ParentID);
                BizObjectSchema ownSchema = null;
                int isLocked = BizWorkflowPackageLockByID(Schema.SchemaCode);
                if (bizTreeNode.Code != Schema.SchemaCode)
                {
                    ownSchema = this.Engine.BizObjectManager.GetDraftSchema(bizTreeNode.Code);
                    if (ownSchema.IsQuotePacket && string.IsNullOrEmpty(Sheet.OwnSchemaCode))
                    {
                        isLocked = 0;
                    }
                }
                model.SheetCode = SheetCode;
                model.SchemaCode = Sheet.BizObjectSchemaCode;
                model.SheetName = GetDisplayName(SheetCode);
                model.DesignerName = GetDesignerName(SheetCode);
                model.PropertyNames = GetUserPropertyNames();
                model.DataItemTreeData = LoadDataItems(Schema.SchemaCode);//数据项
                model.IsHtmlFromDB = false;
                bool isHtmlFromDB = false;
                model.DesignModeContent = LoadDesignModeContent(SheetCode, out isHtmlFromDB);//HTML,为null时需要前台构造
                model.IsHtmlFromDB = isHtmlFromDB;
                model.CSharpCode = LoadCSharpCodeContent(SheetCode);
                model.EnabledCode = Sheet.EnabledCode;//是否启用自定义代码
                model.BizObjectFields = loadBizObjectField(Schema);
                model.ComputationRuleDataItem = LoadDdlDataItem(Schema);
                model.IsControlUsable = isLocked;
                model.Sheet = Sheet;

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 将Organization.User的公共属性拼接成以","号分隔的字符串
        /// </summary>
        /// <returns></returns>
        private string GetUserPropertyNames()
        {
            //将Organization.User的公共属性拼接成以","号分隔的字符串
            string propertyNames = string.Empty;
            PropertyInfo[] propertyInfos = typeof(OThinker.Organization.User).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                propertyNames += propertyInfo.Name + ",";
            }
            return propertyNames.Substring(0, propertyNames.Length - 1);
        }

        /// <summary>
        /// 获取表单显示名称
        /// </summary>
        protected string GetDisplayName(string SheetCode)
        {
            BizSheet Sheet = GetSheet(SheetCode);

            string _DisplayName = this.Engine.FunctionAclManager.GetFunctionNodeByCode(Sheet.BizObjectSchemaCode).DisplayName;
            if (string.IsNullOrEmpty(_DisplayName))
            {
                _DisplayName = Sheet.DisplayName;
            }

            return _DisplayName;

        }

        /// <summary>
        /// 获取设计器的显示名称
        /// </summary>
        /// <returns></returns>
        protected string GetDesignerName(string SheetCode)
        {
            string designerName = "设计器";
            BizSheet Sheet = GetSheet(SheetCode);
            if (Sheet != null)
            {
                designerName = Sheet.DisplayName + "[" + SheetCode + "]_" + designerName;
            }

            return designerName;
        }


        /// <summary>
        /// 将数据模型的自定义数据项放在js变量中，供配置向导使用
        /// </summary>
        private List<MyBizObjectField> loadBizObjectField(BizObjectSchema Schema)
        {
            if (Schema != null)
            {
                List<MyBizObjectField> fields = new List<MyBizObjectField>();
                // 流程数据项
                foreach (DataModel.FieldSchema field in Schema.Fields)
                {
                    if (field.Name.ToLower() == BizSheet.PropertyName_ObjectID.ToLower()) continue;
                    if (DataModel.BizObjectSchema.IsReservedProperty(field.Name)) continue;
                    if (field.LogicType == DataLogicType.HyperLink || field.LogicType == DataLogicType.Attachment) { continue; }

                    if (field.LogicType == DataLogicType.BizObject || field.LogicType == DataLogicType.BizObjectArray)
                    {
                        DataModel.BizObjectSchema childSchema = null;
                        if (Schema.GetProperty(field.Name) != null)
                        {
                            childSchema = Schema.GetProperty(field.Name).ChildSchema;
                        }
                        else
                        {
                            childSchema = this.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                        }
                        if (childSchema == null) continue;

                        foreach (DataModel.FieldSchema childField in childSchema.Fields)
                        {
                            if (DataModel.BizObjectSchema.IsReservedProperty(childField.Name)) continue;
                            if (childField.LogicType == DataLogicType.HyperLink || childField.LogicType == DataLogicType.Attachment) { continue; }

                            fields.Add(new MyBizObjectField()
                            {
                                Code = string.Format("{0}.{1}", field.Name, childField.Name),
                                Name = childField.DisplayName,
                                IsChildField = "1"
                            });
                        }
                    }
                    else
                    {
                        fields.Add(new MyBizObjectField()
                        {
                            Code = field.Name,
                            Name = field.DisplayName,
                            IsChildField = "0"
                        });
                    }
                }

                return fields;
                //前台处理
                //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "BizObjectFields", "var bizObjectFields=" + JSSerializer.Serialize(fields) + ";", true);
            }
            return null;
        }


        /// <summary>
        /// 给用于ComputationRule配置的数据项下拉框绑定数据
        /// </summary>
        private object LoadDdlDataItem(BizObjectSchema Schema)
        {
            List<object> dicDataItem = new List<object>();
            if (Schema != null)
            {
                // 流程数据项
                foreach (DataModel.FieldSchema field in Schema.Fields)
                {
                    //如果是子表控件
                    if (field.LogicType == DataLogicType.BizObject
                        || field.LogicType == DataLogicType.BizObjectArray)
                    {
                        DataModel.BizObjectSchema childSchema = null;

                        if (Schema.GetProperty(field.Name) != null)
                        {
                            childSchema = Schema.GetProperty(field.Name).ChildSchema;
                        }
                        else
                        {
                            childSchema = this.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                        }
                        if (childSchema == null) continue;

                        foreach (DataModel.FieldSchema f in childSchema.Fields)
                        {
                            dicDataItem.Add(new { Key = field.Name + "." + f.Name, Value = field.DisplayName + "." + f.DisplayName });
                        }
                    }
                    else
                    {
                        dicDataItem.Add(new { Key = field.Name, Value = field.DisplayName });
                    }
                }
            }
            return dicDataItem;
        }

        #region 填充表单控件 ------------------------------
        /// <summary>
        /// PC时填充表格控件
        /// </summary>
        /// <param name="fields"></param>
        private string FillTableControlOnPC(FieldSchema[] fields, BizObjectSchema schema)
        {
            // 控件的索引号
            int controlIndex = 0;
            string labelId, controlId;
            // 当前控件是否单独行
            bool currentRowIsLarg = false;
            // 下一控件是否单独行
            bool nextRowIsLarg = false;
            int cellIndex = 0;
            HtmlGenericControl row = null;
            string colCss = string.Empty;
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            foreach (FieldSchema field in fields)
            {
                controlIndex++;
                if (BizObjectSchema.IsReservedProperty(field.Name)) continue;
                if (!BizObjectSchema.IsSheetLogicType(field.LogicType)) continue;
                cellIndex++;
                labelId = "Label" + controlIndex;
                controlId = "Control" + controlIndex;
                currentRowIsLarg = OThinker.H3.Data.DataLogicTypeConvertor.IsLargType(field.LogicType);
                nextRowIsLarg = (controlIndex < schema.Fields.Length) ? OThinker.H3.Data.DataLogicTypeConvertor.IsLargType(schema.Fields[controlIndex].LogicType) : false;

                if (cellIndex % 2 == 1)
                {// 奇数或者是单独行，那么写入一个 TR
                    row = new HtmlGenericControl("div");
                    row.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    if (field.LogicType == DataLogicType.BizObject
                        || field.LogicType == DataLogicType.BizObjectArray
                        || field.LogicType == DataLogicType.Html
                        || field.LogicType == DataLogicType.String
                        || field.LogicType == DataLogicType.Comment)
                        row.Attributes.Add("class", "row tableContent");
                    else
                        row.Attributes.Add("class", "row");
                    //this.divSheet.Controls.Add(row);
                }
                colCss = currentRowIsLarg ? CSS10 : CSS4;

                // 行标题
                HtmlGenericControl rowTitle = new HtmlGenericControl("div") { ID = "title" + cellIndex };
                rowTitle.Attributes.Add("class", CSS2);
                rowTitle.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                Label lblTitle = new Label() { ID = labelId, Text = field.DisplayName };
                lblTitle.Attributes.Add("data-type", "SheetLabel");
                lblTitle.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                lblTitle.Attributes.Add("data-datafield", field.Name);
                rowTitle.Controls.Add(lblTitle);
                row.Controls.Add(rowTitle);

                HtmlGenericControl rowControl = new HtmlGenericControl("div") { ID = "control" + cellIndex };
                rowControl.Attributes.Add("class", colCss);
                rowControl.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                HtmlGenericControl cellControl = this.GetCellControl(schema, field, string.Empty, controlId, false);
                if (cellControl != null)
                {
                    rowControl.Controls.Add(cellControl);
                }
                row.Controls.Add(rowControl);

                if (cellIndex % 2 == 1 && (currentRowIsLarg || nextRowIsLarg || controlIndex == schema.Fields.Length))
                {
                    cellIndex++;
                    if (!currentRowIsLarg)
                    {// 补充一个控件区域
                        HtmlGenericControl spaceTitle = new HtmlGenericControl("div") { ID = "space" + cellIndex };
                        spaceTitle.Attributes.Add("class", CSS2);
                        spaceTitle.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        row.Controls.Add(spaceTitle);

                        HtmlGenericControl spaceControl = new HtmlGenericControl("div") { ID = "spaceControl" + cellIndex };
                        spaceControl.Attributes.Add("class", CSS4);
                        spaceControl.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                        row.Controls.Add(spaceControl);
                    }
                }

                if (cellIndex % 2 == 0 || currentRowIsLarg || nextRowIsLarg || controlIndex == schema.Fields.Length)
                {
                    row.RenderControl(htw);
                }

                //// 条件一个用于读取数据日志的单元
                //if (field.Trackable)
                //{
                //    if (cellControl != null)
                //    {
                //        HtmlGenericControl track = new HtmlGenericControl("div");
                //        track.Attributes.Add("data-datafield", field.Name);
                //        track.Attributes.Add("data-type", "SheetDataTrackLink");
                //        rowControl.Controls.Add(track);
                //    }
                //}
            }
            var designerContent = sw.ToString();
            return designerContent;
        }
        #endregion

        /// <summary>
        /// 获取单元格控件
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="field"></param>
        /// <param name="parentFieldName"></param>
        /// <param name="controlId"></param>
        /// <param name="isCount"></param>
        /// <returns></returns>
        private HtmlGenericControl GetCellControl(BizObjectSchema schema, FieldSchema field,
            string parentFieldName, string controlId, bool isCount)
        {
            HtmlGenericControl cellControl = null;
            string datafield = field.Name;
            if (!string.IsNullOrEmpty(parentFieldName)) datafield = parentFieldName + "." + field.Name;
            if (!isCount)
            {
                switch (field.LogicType)
                {
                    #region 生成控件 ---------------------------
                    case OThinker.H3.Data.DataLogicType.Comment:
                        cellControl = new HtmlGenericControl("div");
                        cellControl.InnerHtml = "<textarea rows=\"10\" class=\"OnlyDesigner\" style=\"width:100%\"></textarea>";
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetComment");
                        break;
                    case OThinker.H3.Data.DataLogicType.Attachment:
                        cellControl = new HtmlGenericControl("div");
                        cellControl.InnerHtml = "<span class=\"OnlyDesigner\">未选择任何文件<input type=\"button\" value=\"浏览...\" /></span>";
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetAttachment");
                        break;
                    case OThinker.H3.Data.DataLogicType.String:
                    case Data.DataLogicType.Xml:
                        cellControl = new HtmlGenericControl("textarea");
                        cellControl.ID = Guid.NewGuid().ToString();
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetRichTextBox");
                        break;
                    case OThinker.H3.Data.DataLogicType.Html:
                        cellControl = new HtmlGenericControl("textarea");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-RichTextBox", "true");
                        cellControl.Attributes.Add("data-type", "SheetRichTextBox");
                        break;
                    case OThinker.H3.Data.DataLogicType.Bool:
                        cellControl = new HtmlGenericControl("input");
                        cellControl.Attributes.Add("type", "checkbox");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetCheckbox");
                        break;
                    case OThinker.H3.Data.DataLogicType.DateTime:
                        cellControl = new HtmlGenericControl("input");
                        cellControl.Attributes.Add("type", "text");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetTime");
                        break;
                    case OThinker.H3.Data.DataLogicType.TimeSpan:
                        cellControl = new HtmlGenericControl("div");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.InnerHtml = "<span class=\"OnlyDesigner\"><input type=\"text\"/></span>";
                        cellControl.Attributes.Add("data-type", "SheetTimeSpan");
                        break;
                    case OThinker.H3.Data.DataLogicType.Double:
                    case Data.DataLogicType.Decimal:
                    case OThinker.H3.Data.DataLogicType.Int:
                    case OThinker.H3.Data.DataLogicType.Long:
                    case OThinker.H3.Data.DataLogicType.ShortString:
                    case Data.DataLogicType.Guid:
                        cellControl = new HtmlGenericControl("input");
                        cellControl.Attributes.Add("type", "text");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetTextBox");
                        break;
                    case OThinker.H3.Data.DataLogicType.HyperLink:  // 链接控件
                        cellControl = new HtmlGenericControl("a");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.InnerHtml = "<span class=\"OnlyDesigner\">Link</span>";
                        cellControl.Attributes.Add("data-type", "SheetHyperLink");
                        break;
                    case OThinker.H3.Data.DataLogicType.SingleParticipant:  // 选人控件 单选
                    case OThinker.H3.Data.DataLogicType.MultiParticipant:   // 选人控件 多选
                        cellControl = new HtmlGenericControl("div");
                        cellControl.InnerHtml = "<input type=\"text\" style=\"width:100%\" class=\"OnlyDesigner\"/>";
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetUser");
                        break;
                    case OThinker.H3.Data.DataLogicType.Association:
                        cellControl = new HtmlGenericControl("input");
                        cellControl.Attributes.Add("type", "text");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetAssociation");
                        cellControl.Attributes.Add("readonly", "true");
                        cellControl.Attributes.Add("data-schemacode", field.DefaultValue + string.Empty);
                        break;
                    case OThinker.H3.Data.DataLogicType.ByteArray:
                    case OThinker.H3.Data.DataLogicType.Object:
                        cellControl = null;
                        break;
                    case Data.DataLogicType.BizObject:
                    case Data.DataLogicType.BizObjectArray:
                        cellControl = new HtmlGenericControl("table");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.Attributes.Add("data-type", "SheetGridView");
                        cellControl.Attributes.Add("class", "SheetGridView");
                        BizObjectSchema childSchema = this.GetChildSchema(schema, field);
                        if (childSchema == null) break;
                        HtmlGenericControl designerRow = new HtmlGenericControl("tr");
                        designerRow.Attributes.Add("class", "OnlyDesigner");
                        cellControl.Controls.Add(designerRow);

                        HtmlGenericControl titleRow = new HtmlGenericControl("tr");
                        titleRow.Attributes.Add("class", "header");
                        cellControl.Controls.Add(titleRow);

                        HtmlGenericControl templateRow = new HtmlGenericControl("tr");
                        templateRow.Attributes.Add("class", "SheetGridViewTemplate");
                        cellControl.Controls.Add(templateRow);

                        HtmlGenericControl countRow = new HtmlGenericControl("tr");
                        countRow.Attributes.Add("class", "footer");
                        cellControl.Controls.Add(countRow);

                        if (field.LogicType == Data.DataLogicType.BizObjectArray)
                        {
                            // 序号列
                            HtmlGenericControl serialTitleTd = new HtmlGenericControl("td");
                            serialTitleTd.InnerText = "序号";
                            serialTitleTd.ID = controlId + "_SerialNo";
                            serialTitleTd.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                            serialTitleTd.Attributes.Add("class", "rowSerialNo");
                            titleRow.Controls.Add(serialTitleTd);

                            HtmlGenericControl serialControlTd = new HtmlGenericControl("td");
                            serialControlTd.Attributes.Add("class", "rowOption");
                            serialControlTd.ID = controlId + "_Option";
                            serialControlTd.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                            templateRow.Controls.Add(serialControlTd);

                            HtmlGenericControl countTd = new HtmlGenericControl("td");
                            countTd.Attributes.Add("class", "rowOption");
                            countRow.Controls.Add(countTd);
                        }

                        int cols = field.LogicType == Data.DataLogicType.BizObjectArray ? 2 : 0;
                        // 数据列
                        foreach (PropertySchema property in childSchema.Properties)
                        {
                            if (BizObjectSchema.IsReservedProperty(property.Name)) continue;
                            cols++;
                            HtmlGenericControl headerTd = new HtmlGenericControl("td");
                            headerTd.Attributes.Add("data-datafield", field.Name + "." + property.Name);
                            headerTd.ID = controlId + "_Header" + cols;
                            headerTd.ClientIDMode = System.Web.UI.ClientIDMode.Static;

                            HtmlGenericControl headerLabel = new HtmlGenericControl("label");
                            headerLabel.ID = controlId + "_Label" + cols;
                            headerLabel.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                            headerLabel.Attributes.Add("data-datafield", field.Name + "." + property.Name);
                            headerLabel.Attributes.Add("data-type", "SheetLabel");
                            headerLabel.InnerText = property.DisplayName;
                            headerTd.Controls.Add(headerLabel);

                            titleRow.Controls.Add(headerTd);

                            HtmlGenericControl controlTd = new HtmlGenericControl("td");
                            controlTd.Attributes.Add("data-datafield", field.Name + "." + property.Name);
                            HtmlGenericControl subCellControl = GetCellControl(schema, new FieldSchema(property), field.Name, controlId + "_ctl" + cols, false);
                            if (subCellControl != null)
                            {
                                controlTd.Controls.Add(subCellControl);
                            }
                            templateRow.Controls.Add(controlTd);

                            HtmlGenericControl countTd = new HtmlGenericControl("td");
                            HtmlGenericControl countCellControl = GetCellControl(schema, new FieldSchema(property), field.Name, controlId + "_stat" + cols, true);
                            if (countCellControl != null) countTd.Controls.Add(countCellControl);
                            countTd.Attributes.Add("data-datafield", field.Name + "." + property.Name);
                            countRow.Controls.Add(countTd);
                        }
                        // 设计列
                        HtmlGenericControl designerTd = new HtmlGenericControl("td");
                        designerTd.Attributes.Add("colspan", cols.ToString());
                        designerTd.Attributes.Add("class", "OnlyDesigner");

                        HtmlGenericControl designerLabel = new HtmlGenericControl("label");
                        designerLabel.Attributes.Add("data-datafield", field.Name);
                        designerLabel.Attributes.Add("data-for", controlId);
                        designerLabel.Attributes.Add("data-type", "SheetGridView");
                        designerLabel.InnerText = field.Name + "属性";
                        designerTd.Controls.Add(designerLabel);

                        designerRow.Controls.Add(designerTd);

                        if (field.LogicType == Data.DataLogicType.BizObjectArray)
                        {// 删除列
                            HtmlGenericControl deleteTitleTd = new HtmlGenericControl("td");
                            deleteTitleTd.InnerText = "删除";
                            deleteTitleTd.Attributes.Add("class", "rowOption");
                            titleRow.Controls.Add(deleteTitleTd);

                            HtmlGenericControl deleteControlTd = new HtmlGenericControl("td");
                            deleteControlTd.InnerHtml = "<a class=\"delete\"><div class=\"fa fa-minus\"></div></a><a class=\"insert\"><div class=\"fa fa-arrow-down\"></div></a>";
                            deleteControlTd.Attributes.Add("class", "rowOption");
                            templateRow.Controls.Add(deleteControlTd);

                            HtmlGenericControl countTd = new HtmlGenericControl("td");
                            countTd.Attributes.Add("class", "rowOption");
                            countRow.Controls.Add(countTd);
                        }
                        break;
                    default:
                        break;

                    #endregion
                }
            }
            else
            {
                switch (field.LogicType)
                {
                    case OThinker.H3.Data.DataLogicType.Double:
                    case Data.DataLogicType.Decimal:
                    case OThinker.H3.Data.DataLogicType.Int:
                    case OThinker.H3.Data.DataLogicType.Long:
                        cellControl = new HtmlGenericControl("label");
                        cellControl.Attributes.Add("data-datafield", datafield);
                        cellControl.InnerHtml = "<span class=\"OnlyDesigner\">统计</span>";
                        cellControl.Attributes.Add("data-type", "SheetCountLabel");
                        break;
                    default:
                        break;
                }
            }
            if (cellControl != null)
            {
                cellControl.ID = controlId;
                cellControl.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            }
            return cellControl;
        }

        /// <summary>
        /// 获取子表结构
        /// </summary>
        /// <param name="Schema"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        private BizObjectSchema GetChildSchema(BizObjectSchema schema, FieldSchema field)
        {
            PropertySchema property = schema.GetProperty(field.Name);
            if (property != null && property.ChildSchema != null) return property.ChildSchema;
            return this.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
        }


        #region 获取数据项
        /// <summary>
        /// 加载数据项，表单设置左边数据项树
        /// </summary>
        private List<MvcTreeData> LoadDataItems(string SchemaCode)
        {
            #region 加载数据项 ----------------------------
            //加载数据项
            DataModel.BizObjectSchema Schema = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
            List<MvcTreeData> responseDatas = new List<MvcTreeData>();

            responseDatas.Add(new MvcTreeData()
            {
                Code = "System",
                Name = "SheetDesigner.MvcDesigner_SystemDataItem",
                ParentCode = string.Empty
            });
            responseDatas.Add(new MvcTreeData()
            {
                Code = "Schema",
                Name = "SheetDesigner.MvcDesigner_InstanceDataItem",
                ParentCode = string.Empty
            });
            // 系统数据项
            string[] systemDatas = OThinker.H3.Instance.Keywords.ParserFactory.GetSheetSystemData();
            foreach (string data in systemDatas)
            {
                responseDatas.Add(new MvcTreeData()
                {
                    Code = data,
                    Name = string.Format(NodeHtml, data, true, string.Empty, data, data),
                    ParentCode = "System",
                    isexpand = true
                });
            }
            if (Schema != null)
            {
                // 流程数据项
                foreach (DataModel.FieldSchema field in Schema.Fields)
                {
                    if (field.Name.ToLower() == BizSheet.PropertyName_ObjectID.ToLower()) continue;
                    if (DataModel.BizObjectSchema.IsReservedProperty(field.Name)) continue;
                    responseDatas.Add(new MvcTreeData()
                    {
                        Code = field.Name,
                        Name = string.Format(NodeHtml, field.Name, false, (int)field.LogicType, "[" + field.Name + "]" + field.DisplayName, field.DisplayName),
                        ParentCode = "Schema"
                    });
                    //如果是子表控件
                    if (field.LogicType == DataLogicType.BizObject
                        || field.LogicType == DataLogicType.BizObjectArray)
                    {
                        DataModel.BizObjectSchema childSchema = null;

                        if (Schema.GetProperty(field.Name) != null)
                        {
                            childSchema = Schema.GetProperty(field.Name).ChildSchema;
                        }
                        else
                        {
                            childSchema = this.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                        }
                        if (childSchema == null) continue;

                        foreach (DataModel.FieldSchema f in childSchema.Fields)
                        {
                            if (DataModel.BizObjectSchema.IsReservedProperty(f.Name)) continue;
                            responseDatas.Add(new MvcTreeData()
                            {
                                Code = f.Name,
                                Name = string.Format(NodeHtml, field.Name + "." + f.Name, false, (int)f.LogicType, "[" + f.Name + "]" + f.DisplayName, f.DisplayName),
                                ParentCode = field.Name
                            });
                        }
                    }
                }

                //#region 加载设计区域内容 ----------------------
                //if (!string.IsNullOrEmpty(Sheet.DesignModeContent))  //编辑
                //{
                //    this.divContent.InnerHtml = Sheet.DesignModeContent;
                //}
                //else
                //{
                //    FillTableControlOnPC(Schema.Fields);
                //}
                //#endregion
            }
            else
            {
                return null;//前台处理弹出警告框
                //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "SchemaIsNull", "$.H3Dialog.Warn({ content: '" + this.PortalResource.GetString("EditScheduleInvoker_Mssg") + "' });", true);
            }
            return responseDatas;
            //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "ProcessData", "var processDatas=" + JSSerializer.Serialize(responseDatas) + ";", true);
            #endregion
        }
        #endregion

        #region 获取设计页面内容及C# 代码
        /// <summary>
        /// 加载设计区域内容
        /// </summary>
        /// <param name="SheetCode"></param>
        /// <param name="FromDb">是否从数据库加载Html，如果是从数据库加载，需要将Html赋值到最外层的#divContent，否则只需要赋值给#divSheet</param>
        /// <returns></returns>
        public string LoadDesignModeContent(string SheetCode, out bool FromDb)
        {

            BizSheet Sheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);

            if (Sheet == null) { FromDb = false; return null; }
            DataModel.BizObjectSchema Schema = this.Engine.BizObjectManager.GetPublishedSchema(Sheet.BizObjectSchemaCode);

            #region 加载设计区域内容 ----------------------
            if (!string.IsNullOrEmpty(Sheet.DesignModeContent))  //编辑
            {
                FromDb = true;
                return Sheet.DesignModeContent;
            }
            else
            {
                //前台处理，生成表单页面
                //FillTableControlOnPC(Schema.Fields);
                FromDb = false;
                return FillTableControlOnPC(Schema.Fields, Schema);
            }
            #endregion

            //return null;

        }


        /// <summary>
        /// 获取C# 代码内容
        /// </summary>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        private string LoadCSharpCodeContent(string SheetCode)
        {

            BizSheet Sheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);
            if (Sheet == null) { return null; }
            if (!string.IsNullOrEmpty(Sheet.CodeContent))
            {
                return Sheet.CodeContent;
            }
            else
            {
                var CodeContent = this.GetMvcCSharpCode(this.Engine.EngineConfig.Code, SheetCode);
                return CodeContent;
            }

        }

        /// <summary>
        /// 获取C#代码
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        private string GetMvcCSharpCode(string EngineCode, string SheetCode)
        {
            // 保存为 cs 文件
            string cs = "using System;\r\n";
            cs += "using System.Collections;\r\n";
            cs += "using System.Configuration;\r\n";
            cs += "using System.Data;\r\n";
            cs += "using System.Web;\r\n";
            cs += "using System.Web.UI;\r\n";
            cs += "using System.Web.UI.WebControls;\r\n";
            cs += "\r\n";
            cs += "namespace OThinker.H3.Portal.Sheets." + EngineCode + "\r\n";
            cs += "{\r\n";
            cs += "    public partial class " + SheetCode + " : OThinker.H3.Controllers.MvcPage\r\n";
            cs += "    {\r\n";
            cs += "        protected void Page_Load(object sender, EventArgs e)\r\n";
            cs += "        {\r\n";
            cs += "        }\r\n";
            cs += "    }\r\n";
            cs += "}\r\n";

            return cs;
        }

        #endregion

        #region 获取子表结构，生成子表控件时需要

        /// <summary>
        /// 获取子表结构，生成子表控件时需要
        /// </summary>
        /// <param name="SchemaCode">数据模型编码</param>
        /// <param name="FieldName">数据项编码</param>
        /// <param name="ChildScheaCode">数据项的子数据模型编码</param>
        /// <returns></returns>
        public JsonResult GetChildSchema(string SchemaCode, string FieldName, string ChildScheaCode)
        {
            return ExecuteFunctionRun(() =>
            {

                DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
                PropertySchema property = schema.GetProperty(FieldName);
                if (property != null && property.ChildSchema != null) return Json(property.ChildSchema, JsonRequestBehavior.AllowGet);
                return Json(this.Engine.BizObjectManager.GetPublishedSchema(ChildScheaCode), JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region 获取 Worksheet 控件的属性值 ------------------
        /// <summary>
        /// 获取控件的属性输出的值
        /// </summary>
        /// <param name="context"></param>
        public JsonResult GetControlPropertyValues(string ControlFullName)
        {

            //string resultValue = OThinker.H3.WorkSheet.SheetUtility.GetControlPropertyStringValue(ControlFullName, "worksheet");
            return null;
        }
        #endregion

        #region 获取数据模型信息 ------------------
        /// <summary>
        /// 获取数据模型信息
        /// </summary>
        /// <param name="context"></param>
        public JsonResult GetBizObjectSchema(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {

                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                if (schema == null)
                {
                    return null;
                }
                DataModel.BizQuery[] queries = this.Engine.BizObjectManager.GetBizQueries(schemaCode);

                Dictionary<string, string> BizQueries = new Dictionary<string, string>();
                Dictionary<string, Dictionary<string, string>> QueryItems = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, Dictionary<string, string>> queryProperties = new Dictionary<string, Dictionary<string, string>>();
                if (queries != null)
                {
                    Dictionary<string, string> properties = new Dictionary<string, string>();
                    foreach (DataModel.PropertySchema property in schema.Properties)
                    {
                        if (property.LogicType == Data.DataLogicType.ShortString
                            || property.LogicType == Data.DataLogicType.Guid
                            || property.LogicType == Data.DataLogicType.Int
                            || property.LogicType == Data.DataLogicType.Bool
                            || property.LogicType == Data.DataLogicType.DateTime
                            || property.LogicType == Data.DataLogicType.Double
                            || property.LogicType == Data.DataLogicType.Long
                            || property.LogicType == Data.DataLogicType.String
                            || property.LogicType == Data.DataLogicType.Decimal
                            || property.LogicType == Data.DataLogicType.SingleParticipant)
                        {
                            properties.Add(property.Name,
                                string.IsNullOrEmpty(property.DisplayName) ? property.Name : property.DisplayName);
                        }
                    }

                    foreach (DataModel.BizQuery query in queries)
                    {
                        BizQueries.Add(query.QueryCode, query.DisplayName);
                        Dictionary<string, string> queryItemDic = new Dictionary<string, string>();
                        if (query.QueryItems != null)
                        {
                            foreach (DataModel.BizQueryItem item in query.QueryItems)
                            {
                                if (item.FilterType == DataModel.FilterType.SystemParam) continue;
                                string text = item.PropertyName;
                                foreach (DataModel.PropertySchema propertySchema in schema.Properties)
                                {
                                    if (propertySchema.Name == item.PropertyName)
                                    {
                                        text = propertySchema.DisplayName;
                                        break;
                                    }
                                }
                                queryItemDic.Add(item.PropertyName, text);
                            }
                        }
                        QueryItems.Add(query.QueryCode, queryItemDic);

                        Dictionary<string, string> propertyDic = new Dictionary<string, string>();
                        foreach (BizQueryColumn column in query.Columns)
                        {
                            if (properties.ContainsKey(column.PropertyName))
                            {
                                propertyDic.Add(column.PropertyName, properties[column.PropertyName]);
                            }
                        }
                        queryProperties.Add(query.QueryCode, propertyDic);
                    }
                }

                var resultValue = new
                {
                    Properties = queryProperties,
                    BizQueries = BizQueries,
                    QueryItems = QueryItems
                };

                return Json(resultValue);
            });
        }

        /// <summary>
        /// 获取数据模型的名称
        /// </summary>
        /// <param name="context"></param>
        public JsonResult GetBizObjectFullName(string schemaCode, string isDisplayCode)
        {
            return ExecuteFunctionRun(() =>
            {
                if (string.IsNullOrEmpty(schemaCode))
                {
                    return null;
                }
                DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                if (schema == null)
                {
                    return null;
                }
                string fullName = schema.DisplayName;
                bool displayCode = false;
                if (bool.TryParse(isDisplayCode, out displayCode) && displayCode)
                {
                    fullName += string.Format("[{0}]", schemaCode);
                }
                return Json(fullName, JsonRequestBehavior.AllowGet);
            });

        }
        #endregion

        #region 正则表达式模板

        public JsonResult GetRegularExpression()
        {
            return ExecuteFunctionRun(() =>
            {
                List<object> items = new List<object>();
                var item = new
                {
                    text = "请输入一个整数.",
                    value = "/^(-|\\+)?(\\d)*$/"
                };
                items.Add(item);
                item = new
                {
                    text = "请输入一个数字.",
                    value = "/^[\\+\\-]?\\d*?\\.?\\d*?$/"
                };
                items.Add(item);
                item = new
                {
                    text = "请输入一个有效的邮箱地址.",
                    value = "/^\\w+([-+.]\\w+)*@\\w+([-.]\\\\w+)*\\.\\w+([-.]\\w+)*$/"
                };
                items.Add(item);
                //item = new
                //{
                //    text = "请输入一个有效的身份证.",
                //    //value = "/^\\d{15}(\\d{2}[A-Za-z0-9])?$/"
                //    value = "/^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{4}$/" 
                //};
                //items.Add(item);
                item = new
                {
                    text = "请输入一个有效的手机号码.",
                    //value = "/^((\\(\\d{3}\\))|(\\d{3}\\-))?13\\d{9}$/"
                    value = "/^1[3|4|5|8][0-9]{9}$/"
                };
                items.Add(item);
                item = new
                {
                    text = "请输入一个有效的电话号码.",
                    value = "/^0\\d{2,3}-?\\d{7,8}$/"
                };
                items.Add(item);

                return Json(items);
            });


        }

        #endregion

        #region 获取数据字典

        /// <summary>
        /// 获取主数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMasterDataCategory()
        {
            return ExecuteFunctionRun(() =>
            {
                List<string> items = new List<string>();
                Dictionary<string, string> table = this.Engine.MetadataRepository.GetCategoryTable();
                foreach (string key in table.Keys)
                {
                    items.Add(table[key]);
                }

                return Json(items, JsonRequestBehavior.AllowGet);
            });

        }

        #endregion

        #region 保存表单 ------------------
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="context"></param>
        public JsonResult SaveSheet(MvcSheetViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "msgGlobalString.SaveFailed");
                try
                {
                    BizSheet Sheet = this.Engine.BizSheetManager.GetBizSheetByCode(Server.UrlDecode(model.SheetCode));
                    if (Sheet == null)
                    {
                        result.Message = "msgGlobalString.SaveFailed";
                        return Json(result);
                    }

                    Sheet.DisplayName = model.SheetName;
                    Sheet.PrintModel = Server.UrlDecode(model.PrintModel);
                    Sheet.Javascript = Server.UrlDecode(model.Javascript);
                    Sheet.DesignModeContent = Server.UrlDecode(model.DesignModeContent);
                    Sheet.RuntimeContent = Server.UrlDecode(model.RuntimeContent);
                    Sheet.EnabledCode = model.EnabledCode;//
                    Sheet.LastModifiedBy = model.Editor;
                    Sheet.LastModifiedTime = DateTime.Now;
                    Sheet.SheetAddress = model.SheetCode + ".aspx";
                    Sheet.CodeContent = model.CSharpCode;

                    bool flag = this.Engine.BizSheetManager.UpdateBizSheet(Sheet);

                    if (!flag)
                    {
                        //throw new Exception("保存失败！");
                        return Json(result);
                    }
                    result.Success = true;
                    result.Message = "msgGlobalString.SaveSucced";
                    return Json(result);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(ex.Message);
                }
            });
        }
        #endregion

        #region 格式化ASPX输出

        /// <summary>
        /// 格式化ASPX输出
        /// </summary>
        /// <param name="aspx"></param>
        /// <returns></returns>
        private string GetFormatAspx(string aspx)
        {
            if (aspx.IndexOf("\n") == 0)
            {
                aspx = aspx.Substring(2);
            }
            else
            {
                return aspx;
            }
            int spaceCount = 0;
            while (aspx.IndexOf("\n                    ") > -1)
            {
                aspx = aspx.Replace("\n                    ", "\n");
            }
            while (aspx.IndexOf("\n ") > -1)
            {
                aspx = aspx.Replace("\n ", "\n");
            }
            aspx = aspx.Replace("\n", string.Empty);
            aspx = aspx.Replace("\t", string.Empty);
            StringBuilder builder = new StringBuilder();
            string[] arrs = aspx.Split(new string[] { "<" }, StringSplitOptions.None);
            for (int i = 0; i < arrs.Length; i++)
            {
                if (i == 0)
                {
                    if (arrs[i].Trim() != string.Empty)
                    {
                        builder.Append(arrs[i]);
                    }
                    continue;
                }

                if (arrs[i].StartsWith("/") && spaceCount > 0) spaceCount--;

                builder.Append(GetSapceValue(spaceCount));
                builder.Append("<" + arrs[i].Replace(">", ">\n"));
                if (arrs[i].IndexOf("/>") > -1)
                {
                    // if (spaceCount > 1) spaceCount--;
                }
                else if (!arrs[i].StartsWith("/"))
                {
                    if (arrs[i].StartsWith("br ")
                        || arrs[i].StartsWith("br>")
                        || arrs[i].StartsWith("br/>")
                        || arrs[i].StartsWith("hr ")
                        || arrs[i].StartsWith("hr>")
                        || arrs[i].StartsWith("hr/>"))
                    {

                    }
                    else
                    {
                        spaceCount++;
                    }
                }
            }
            return builder.ToString();
            // return aspx;
        }

        private string GetSapceValue(int spaceCount)
        {
            string result = string.Empty;
            for (int i = 0; i < spaceCount; i++)
            {
                result += "\t";
            }
            return result;
        }

        #endregion

        #region 导出 ASPX 表单 -------------
        /// <summary>
        /// 导出表单
        /// </summary>
        /// <param name="context"></param>
        public JsonResult GetDesignerASPX(MvcSheetViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                try
                {
                    string SheetHtml = model.SheetHtml + string.Empty;
                    string SheetJson = model.SheetJson + string.Empty;
                    string SheetCode = model.SheetCode + string.Empty;
                    BizSheet Sheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);

                    string SheetAspx = "";

                    SheetAspx = GetFormatAspx(SheetAspx);

                    var o = new
                    {
                        ASPX = SheetAspx
                    };

                    return Json(o);
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            });

        }
        #endregion

        #region 获取表单设计器转换成的 ASPX 表单 -----------
        /// <summary>
        /// 根据表单设计器生成 ASPX 文件
        /// </summary>
        /// <param name="SheetCode"></param>
        /// <param name="SheetHtml"></param>
        /// <param name="SheetJson"></param>
        /// <returns></returns>
        //public string GetAspxFromSheetDesigner(
        //    string SheetCode,
        //    string SheetHtml,
        //    string SheetJson)
        //{
        //    HtmlDocument document = new HtmlDocument();
        //    document.LoadHtml(SheetHtml);
        //    // document.GetElementbyId("divBlack").Attributes["style"].Value = "height:63px";
        //    SheetHtml = document.DocumentNode.OuterHtml;

        //    //1.替换设计器Hidden控件
        //    //HtmlNode hiddenArea = document.GetElementbyId("hiddenArea");
        //    //SheetHtml = SheetHtml.Replace(hiddenArea.OuterHtml, string.Empty);

        //    //2.替代所有Property控件
        //    ControlProperty[] propertys = JSSerializer.Deserialize<ControlProperty[]>(SheetJson);
        //    string _oldCtrlHtml = string.Empty;  //源HTML控件部分 
        //    string _newCtrlHtml = string.Empty;  //替换后的控件HTML部分  
        //    string _footSheetActionPanel = string.Empty;
        //    BizSheet sheet = OThinker.H3.WorkSheet.AppUtility.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);
        //    OThinker.H3.DataModel.BizObjectSchema schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(sheet.BizObjectSchemaCode);

        //    foreach (ControlProperty p in propertys)
        //    {
        //        HtmlNode propertyNode = document.GetElementbyId(p.Id);
        //        if (propertyNode == null) continue;

        //        _oldCtrlHtml = propertyNode.OuterHtml;
        //        _newCtrlHtml = GeneralControl(p);
        //        if (!string.IsNullOrEmpty(p.ItemName) && p.LogicType != "SheetLabel")
        //        {// 增加查看痕迹控件
        //            OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(p.ItemName);
        //            if (property != null && property.Trackable)
        //            {
        //                if (SheetHtml.IndexOf(p.Id + "_Track") == -1)
        //                {
        //                    _newCtrlHtml += "<SheetControls:SheetDataTrackLink ID=\"" + p.Id + "_Track\" runat=\"server\" DataField=\"" + p.ItemName + "\"/>";
        //                }
        //            }
        //        }

        //        if (p.FullName == typeof(SheetActionPane).FullName)
        //        {
        //            _footSheetActionPanel = _newCtrlHtml;
        //        }

        //        if (_newCtrlHtml != string.Empty)
        //        {
        //            SheetHtml = SheetHtml.Replace(_oldCtrlHtml, _newCtrlHtml);
        //            // document.LoadHtml(SheetHtml);  // 加载重新加载被替换的Html // ---- 注释掉
        //        }
        //    }
        //    return SheetHtml;
        //}

        #endregion


    }
}
