using OThinker.H3.BizBus.Filter;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Site;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Admin.Portal
{
    public class PortalAdminHandlerController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        #region 模板
        /// <summary>
        /// 加载所有的模板
        /// </summary>
        /// <param name="context"></param>
        public JsonResult LoadTemplates()
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                OThinker.H3.Site.SitePageTemplate[] pageTemplates = this.Engine.SiteManager.GetAllPageTemplates();
                result.Extend = pageTemplates;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveTemplate(string tempId, string tempName, string icon, string html)
        {
            return this.ExecuteFunctionRun(() =>
            {
                html = HttpContext.Server.UrlDecode(html);
                ActionResult result = new ActionResult(true);
                if (string.IsNullOrEmpty(tempName) || string.IsNullOrEmpty(html))
                {
                    result.Success = false;
                    result.Message = "模板名称和HTML不能为空";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                Guid guid = Guid.Empty;
                bool isNeedUpdate = Guid.TryParse(tempId, out guid) && !string.IsNullOrEmpty(tempId);
                OThinker.H3.Site.SitePageTemplate obj = new OThinker.H3.Site.SitePageTemplate();
                if (isNeedUpdate)
                {
                    obj = this.Engine.SiteManager.GetPageTemplate(tempId);
                }
                obj.HtmlContent = html;
                obj.TemplateName = tempName;
                obj.Icon = icon;
                bool b = true;
                if (isNeedUpdate)
                {
                    b = this.Engine.SiteManager.UpdatePageTemplate(obj);
                }
                else
                {
                    b = this.Engine.SiteManager.AddPageTemplate(obj);
                }
                if (!b)
                {
                    result.Success = false;
                    result.Message = "保存失败";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="context"></param>
        public JsonResult RemoveTemplate(string tempId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                if (this.Engine.SiteManager.RemovePageTemplateById(tempId))
                { }
                else
                {
                    result.Success = false;
                    result.Message = "删除失败";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult GetAllPages()
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);

                int recordCount = 0;
                DataTable dtPages = new DataTable();
                OThinker.H3.Site.SitePage[] sitepages = this.Engine.SiteManager.GetAllPages();
                recordCount = sitepages.Length;
                if (sitepages != null && recordCount > 0)
                {
                    int i = 0;
                    dtPages.Columns.Add("ObjectID");
                    dtPages.Columns.Add("RowNumber_");
                    dtPages.Columns.Add("Title");
                    dtPages.Columns.Add("OrgId");
                    dtPages.Columns.Add("CreatedBy");
                    dtPages.Columns.Add("CreatedTime");
                    dtPages.Columns.Add("LastModifiedTime");
                    dtPages.Columns.Add("TemplateId");

                    List<SitePagesViewModel> griddata = new List<SitePagesViewModel>();
                    foreach (OThinker.H3.Site.SitePage sitepage in sitepages)
                    {
                        DataRow r = dtPages.NewRow();
                        r["ObjectID"] = sitepage.ObjectID;
                        r["RowNumber_"] = i;
                        r["Title"] = sitepage.Title;
                        r["OrgId"] = sitepage.OrgId;
                        r["CreatedBy"] = sitepage.CreatedBy;
                        r["CreatedTime"] = sitepage.CreatedTime;
                        r["LastModifiedTime"] = sitepage.LastModifiedTime;
                        r["TemplateId"] = sitepage.TemplateId;
                        dtPages.Rows.Add(r);
                    }
                    //解析组织
                    Dictionary<string, string> unitNames = new Dictionary<string, string>();
                    if (dtPages != null && dtPages.Rows.Count > 0)
                    {
                        string[] columns = new string[] { OThinker.H3.Site.SitePage.PropertyName_OrgId, OThinker.H3.Site.SitePage.PropertyName_CreatedBy };
                        unitNames = this.GetUnitNamesFromTable(dtPages, columns);
                    }
                    foreach (OThinker.H3.Site.SitePage sitepage in sitepages)
                    {
                        i++;
                        //时间过滤
                        string CreatedTime = this.GetValueFromDate(sitepage.CreatedTime, WorkItemTimeFormat);
                        if (CreatedTime.IndexOf("1753") > -1 || CreatedTime.IndexOf("0001") > -1)
                        {
                            CreatedTime = "";
                        }
                        string LastModifiedTime = this.GetValueFromDate(sitepage.LastModifiedTime, WorkItemTimeFormat);
                        if (LastModifiedTime.IndexOf("1753") > -1 || LastModifiedTime.IndexOf("0001") > -1)
                        {
                            LastModifiedTime = "";
                        }

                        string TemplateName = "";
                        OThinker.H3.Site.SitePageTemplate pageTemplate = this.Engine.SiteManager.GetPageTemplate(sitepage.TemplateId);
                        if (pageTemplate != null)
                        {
                            TemplateName = pageTemplate.TemplateName;
                        }
                        griddata.Add(new SitePagesViewModel()
                        {
                            ObjectID = sitepage.ObjectID,
                            RowNumber = i,
                            Title = sitepage.Title,
                            OrgId = sitepage.OrgId,
                            OrgName = this.GetValueFromDictionary(unitNames, sitepage.OrgId),
                            CreatedBy = sitepage.CreatedBy,
                            CreatedName = this.GetValueFromDictionary(unitNames, sitepage.CreatedBy),
                            CreatedTime = CreatedTime,
                            LastModifiedTime = LastModifiedTime,
                            TemplateId = sitepage.TemplateId,
                            TemplateName = TemplateName
                        });
                    }
                    result.Extend = griddata;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region 页面

        /// <summary>
        /// 获取页面参数
        /// </summary>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public JsonResult LoadPage(string PageId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                //加载模板
                List<Template> Templates = new List<Template>();
                SitePage Page = new SitePage();

                OThinker.H3.Site.SitePageTemplate[] templates = this.Engine.SiteManager.GetAllPageTemplates();
                foreach (OThinker.H3.Site.SitePageTemplate template in templates)
                {
                    Templates.Add(new Template
                    {
                        TemplateName = template.TemplateName,
                        TemplateId = template.ObjectID
                    });
                }

                OThinker.H3.Site.SitePage SitePage = this.Engine.SiteManager.GetPage(PageId);
                if (SitePage != null)
                {
                    Page.PageId = PageId;
                    Page.PageTitle = SitePage.Title;
                    Page.TempId = SitePage.TemplateId;
                    Page.OrgId = SitePage.OrgId;
                }
                else
                {
                    Page = null;
                }
                var Extend = new
                {
                    Page = Page,
                    Templates = Templates,
                };
                result.Extend = Extend;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存页面
        /// </summary>
        /// <param name="PageId"></param>
        /// <param name="PageTitle"></param>
        /// <param name="TempId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public JsonResult SavePage(string PageId, string PageTitle, string TempId, string OrgId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);

                bool IsNewPage = string.IsNullOrEmpty(PageId);
                if (string.IsNullOrEmpty(PageTitle) || string.IsNullOrEmpty(TempId) || string.IsNullOrEmpty(OrgId))
                {
                    result.Success = false;
                    result.Message = "缺少参数[模板id, 页面标题, 组织结构id]";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                OThinker.H3.Site.SitePage SitePage;
                if (IsNewPage)
                {
                    SitePage = new OThinker.H3.Site.SitePage();
                    SitePage.CreatedTime = DateTime.Now;
                    SitePage.CreatedBy = UserValidatorFactory.CurrentUser.UserID;
                }
                else
                {
                    SitePage = this.Engine.SiteManager.GetPage(PageId);
                    if (SitePage == null)
                    {
                        result.Success = false;
                        result.Message = "模板id无效";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    SitePage.LastModifiedTime = DateTime.Now;
                    SitePage.LastModifiedBy = UserValidatorFactory.CurrentUser.UserID;
                }

                //TODO:是否有必要做保存校验

                SitePage.TemplateId = TempId;
                SitePage.Title = PageTitle;
                SitePage.OrgId = OrgId;
                if ((IsNewPage && !this.Engine.SiteManager.AddPage(SitePage))
                      || (!IsNewPage && !this.Engine.SiteManager.UpdatePage(SitePage)))
                {
                    result.Success = false;
                    result.Message = "保存失败";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Message = "保存成功";
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 删除页面
        /// </summary>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public JsonResult RemoveManagedPage(string PageId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);

                if (string.IsNullOrEmpty(PageId))
                {
                    result.Success = false;
                    result.Message = "无效的参数";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                Guid testGuid;
                if (!Guid.TryParse(PageId, out testGuid))
                {
                    result.Success = false;
                    result.Message = "无效的参数";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (testGuid == Guid.Empty)
                {
                    result.Success = false;
                    result.Message = "主页不允许删除";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                OThinker.H3.Site.SitePage obj = this.Engine.SiteManager.GetPage(PageId);
                if (obj == null || obj == default(OThinker.H3.Site.SitePage))
                {
                    result.Success = false;
                    result.Message = "不存在的页面";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var userValidator = UserValidatorFactory.CurrentUser;
                if (userValidator != null && (userValidator.ValidateAdministrator() || userValidator.ValidateOrgAdmin(obj.OrgId)))
                {
                    this.Engine.SiteManager.DeletePage(obj);
                    result.Message = "删除成功";
                }
                else
                {
                    result.Success = false;
                    result.Message = "没有权限";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region 部件

        public JsonResult GetPageWebParts(string PageId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                PageId = (PageId + string.Empty).Replace("'", string.Empty);
                SiteWebPartInstance[] webparts = this.Engine.SiteManager.GetAllWebPartInsts();
                ArrayList webPartsList = new ArrayList();
                ArrayList keyList = new ArrayList();
                for (int i = 0; i < webparts.Length; i++)
                {
                    webPartsList.Add(webparts[i]);
                    keyList.Add(webparts[i].SortKey);
                }
                webPartsList = OThinker.Data.Sorter.Sort(keyList, webPartsList);
                List<WebPart> WebParts = new List<WebPart>();
                foreach (SiteWebPartInstance webpart in webPartsList)
                {
                    if (webpart.PageId == PageId)
                    {
                        string WebPartTitle = "";
                        foreach (SiteWebPartInstanceValue webpartValue in webpart.Attributes)
                        {
                            if (webpartValue.AttributeName == "Title")
                            {
                                WebPartTitle = webpartValue.AttributeValue;
                            }
                        }
                        WebParts.Add(new WebPart
                        {
                            WebPartId = webpart.ObjectID,
                            WebPartTitle = WebPartTitle
                        });
                    }
                }
                result.Extend = WebParts;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除部件
        /// </summary>
        /// <param name="WebPartId"></param>
        /// <returns></returns>
        public JsonResult RemovePageWebPart(string WebPartId)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                WebPartId = (WebPartId + string.Empty).Replace("'", string.Empty);
                if (this.Engine.SiteManager.DeleteWebPartInstById(WebPartId))
                {
                    result.Message = "删除成功";
                }
                else
                {
                    result.Success = false;
                    result.Message = "删除失败";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult ChangeWorkflowCode(string WorkflowCode)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                List<ListItem> WorkflowItems = new List<ListItem>();
                if (string.IsNullOrEmpty(WorkflowCode))
                {
                    result.Success = false;
                    result.Message = "WorkflowCodeIsNull";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var bizobj = this.Engine.BizObjectManager.GetPublishedSchema(WorkflowCode);
                foreach (var prop in bizobj.Properties)
                {
                    switch (prop.LogicType)
                    {
                        case OThinker.H3.Data.DataLogicType.ShortString:
                        case OThinker.H3.Data.DataLogicType.String:
                        case OThinker.H3.Data.DataLogicType.Decimal:
                        case OThinker.H3.Data.DataLogicType.Int:
                        case OThinker.H3.Data.DataLogicType.Long:
                        case OThinker.H3.Data.DataLogicType.DateTime:
                            // ddl2.Items.Add(new ListItem(prop.DisplayName, prop.Name));
                            WorkflowItems.Add(new ListItem(prop.DisplayName, prop.Name));
                            break;
                    }
                }
                result.Extend = WorkflowItems;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult GetDataModelData(string DataModelCode, string QueryCode, string SortBy, int ShowCount, string BoundFiledList, string LinkFormat)
        {
            ActionResult result = new ActionResult(true);
            if (string.IsNullOrEmpty(BoundFiledList))
            {
                result.Message = "未设置绑定数据";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(DataModelCode);
            DataModel.BizQuery query = Engine.BizObjectManager.GetBizQuery(QueryCode);
            if (schema != null && query != null)
            {
                OThinker.H3.BizBus.Filter.Filter filter = GetFilter(schema, "GetList", query);
                filter.FromRowNum = 1;
                filter.ToRowNum = ShowCount;
                if (ShowCount <= 0) filter.ToRowNum = 5;
                //SORTBY是正常的语法 Column1,Column2 DESC
                if (string.IsNullOrEmpty(SortBy))
                {
                    SortBy = schema.GetPropertyNames()[0] + " DESC";
                }
                string[] arrSortKey = SortBy.Split(',');
                List<OThinker.H3.BizBus.Filter.SortBy> list = new List<OThinker.H3.BizBus.Filter.SortBy>();
                foreach (string str in arrSortKey)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    string str2 = str.Trim();
                    string[] ssarr = str2.Split(' ');
                    if (ssarr.Length == 0 || string.IsNullOrEmpty(ssarr[0]))
                        continue;
                    string sortitem = ssarr[0];
                    OThinker.H3.BizBus.Filter.SortDirection sd = OThinker.H3.BizBus.Filter.SortDirection.Ascending;
                    for (var k = 1; k < ssarr.Length; k++)
                    {
                        if (string.IsNullOrEmpty(ssarr[k]))
                            continue;
                        if (ssarr[k].ToLower() == "asc")
                        {
                            break;
                        }
                        if (ssarr[k].ToLower() == "desc")
                        {
                            sd = OThinker.H3.BizBus.Filter.SortDirection.Descending;
                            break;
                        }
                    }
                    filter.AddSortBy(sortitem, sd);
                }
                DataModel.BizObject[] objs = schema.GetList(
                    this.Engine.Organization,
                    this.Engine.MetadataRepository,
                    this.Engine.BizObjectManager,
                    this.UserValidator.UserID,
                    "GetList",
                    filter);
                // 开始绑定数据源
                DataTable tablesource = DataModel.BizObjectUtility.ToTable(schema, objs);

                List<object> tr = new List<object>();
                foreach (DataRow dr in tablesource.Rows)
                {
                    List<object> td = new List<object>();
                    foreach (string field in BoundFiledList.Split(','))
                    {
                        string[] arrFields = field.Split('|');
                        string column = arrFields[0];
                        int len = 0;
                        int.TryParse(arrFields[1], out len);
                        string format = arrFields[2];
                        if (!tablesource.Columns.Contains(column))
                        {
                            continue;
                        }
                        string fv = dr[column].ToString();
                        //格式化
                        if (!string.IsNullOrEmpty(format))
                        {
                            if (format.StartsWith("{0:"))
                            {   //{0:****}格式
                                if (tablesource.Columns[column].DataType == typeof(Decimal))
                                    fv = String.Format(format, Convert.ToDecimal(dr[column]));
                                if (tablesource.Columns[column].DataType == typeof(DateTime))
                                    fv = String.Format(format, Convert.ToDateTime(dr[column]));
                            }
                            else
                            {//****格式
                                if (tablesource.Columns[column].DataType == typeof(Decimal))
                                    fv = Convert.ToDecimal(dr[column]).ToString(format);
                                if (tablesource.Columns[column].DataType == typeof(DateTime))
                                    fv = Convert.ToDateTime(dr[column]).ToString(format);
                            }
                        }
                        //截取长度
                        if (len > 0 && fv.Length > len)
                            fv = fv.Substring(0, len);
                        string datalink = "";
                        //匹配链接参数
                        if (!string.IsNullOrEmpty(LinkFormat))
                        {
                            string[] paras = LinkFormat.Split('{');
                            string currlink = LinkFormat;
                            //如果包含有{}, pcol就是要找的替换格式字段
                            for (var p = 1; p < paras.Length; p++)
                            {
                                string pcol = paras[p].Split('}')[0];
                                if (tablesource.Columns.Contains(pcol))
                                    currlink = currlink.Replace("{" + pcol + "}", Convert.ToString(dr[pcol]));
                                else
                                    currlink = currlink.Replace("{" + pcol + "}", "");
                            }
                            //datalink = "href=\"" + currlink + "\"  target=\"_blank\"";
                            datalink = currlink;
                        }
                        var newRow = new
                        {
                            Text = fv,
                            Href = string.IsNullOrEmpty(datalink) ? "#" : datalink
                        };
                        td.Add(newRow);
                    }
                    tr.Add(td);
                }
                result.Extend = tr;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <param name="Schema"></param>
        /// <param name="ListMethod"></param>
        /// <param name="Query"></param>
        /// <returns></returns>
        private OThinker.H3.BizBus.Filter.Filter GetFilter(DataModel.BizObjectSchema Schema, string ListMethod, DataModel.BizQuery Query)
        {
            // 构造查询条件
            OThinker.H3.BizBus.Filter.Filter filter = new OThinker.H3.BizBus.Filter.Filter();

            OThinker.H3.BizBus.Filter.And and = new OThinker.H3.BizBus.Filter.And();
            filter.Matcher = and;
            ItemMatcher propertyMatcher = null;
            if (Query.QueryItems != null)
            {
                foreach (DataModel.BizQueryItem queryItem in Query.QueryItems)
                { // 增加系统参数条件
                    if (queryItem.FilterType == DataModel.FilterType.SystemParam)
                    {
                        propertyMatcher = new OThinker.H3.BizBus.Filter.ItemMatcher(queryItem.PropertyName,
                             OThinker.Data.ComparisonOperatorType.Equal,
                             SheetUtility.GetSystemParamValue(this.UserValidator, queryItem.SelectedValues));
                        and.Add(propertyMatcher);
                    }
                    else if (queryItem.Visible == OThinker.Data.BoolMatchValue.False)
                    {
                        and.Add(new ItemMatcher(queryItem.PropertyName,
                                queryItem.FilterType == DataModel.FilterType.Contains ? OThinker.Data.ComparisonOperatorType.Contain
                                : OThinker.Data.ComparisonOperatorType.Equal,
                                queryItem.DefaultValue));
                    }
                }
            }
            return filter;
        }
        #endregion
    }

    public class Template
    {
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
    }

    public class SitePage
    {
        public string PageId { get; set; }
        public string PageTitle { get; set; }
        public string TempId { get; set; }
        public string OrgId { get; set; }
    }
    public class WebPart
    {
        public string WebPartId { get; set; }
        public string WebPartTitle { get; set; }
    }


}


