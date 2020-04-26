using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.Collections;
using System.Web.UI;
using OThinker.H3.Site;
using OThinker.H3.Controllers.ViewModels;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace OThinker.H3.Controllers
{
    public class HomePageController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HomePageController()
        {
        }

        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 默认的模板ID
        /// </summary>
        public static string DefaultPageTemplateID = Guid.Empty.ToString();

        /// <summary>
        /// 默认的页面ID
        /// </summary>
        public static string DefaultSitePageID = Guid.Empty.ToString();

        #region 获取当前页面

        ///// <summary>
        ///// 获取当前页面对象
        ///// </summary>
        //protected OThinker.H3.Site.SitePage CurrentSitePage
        //{
        //    get
        //    {
        //        SitePage page = this.Engine.SiteManager.GetPage(DefaultPageTemplateID);
        //        return page as OThinker.H3.Site.SitePage;
        //    }
        //}
        //OThinker.H3.Site.SitePageTemplate _CurrentPageTemplate = null;
        ///// <summary>
        ///// 获取当前页面对象
        ///// </summary>
        //protected OThinker.H3.Site.SitePageTemplate CurrentPageTemplate
        //{
        //    get
        //    {
        //        if (_CurrentPageTemplate == null)
        //        {
        //            if (CurrentSitePage != null)
        //            {
        //                _CurrentPageTemplate = this.Engine.SiteManager.GetPageTemplate(CurrentSitePage.TemplateId);
        //            }
        //        }
        //        return _CurrentPageTemplate;
        //    }
        //}

        ///// <summary>
        ///// 获取当前模板页面内容
        ///// </summary>
        //public string PageTemplateContent
        //{
        //    get
        //    {
        //        return CurrentPageTemplate == null ? string.Empty : CurrentPageTemplate.HtmlContent;
        //    }
        //}

        #endregion

        private OThinker.H3.Site.SitePage _pagedataobject = null;
        /// <summary>
        /// 当前页面的数据对象
        /// </summary>
        public OThinker.H3.Site.SitePage PageDataObject
        {
            get { return _pagedataobject; }
            set { _pagedataobject = value; }
        }

        /// <summary>
        /// 页面的控件加载方式（postback 或 ajax）
        /// </summary>
        public String LoadMode
        {
            get
            {
                return "postback";
            }
        }

        /// <summary>
        /// 当前页面的Id
        ///<para>如果没有参数，默认是Guid.Empty[首页]</para>
        /// </summary>
        public string GetPageId(string PageId)
        {
            if (string.IsNullOrEmpty(PageId))
                return Guid.Empty.ToString();
            Guid strguid;
            if (!Guid.TryParse(PageId, out strguid))
                return Guid.Empty.ToString();
            return PageId;
        }

        /// <summary>
        /// 加载页面模板
        /// </summary>
        /// <param name="PageID"></param>
        /// <returns></returns>
        public JsonResult RenderTemplateToPage(string PageId)
        {
            PageId = GetPageId(PageId);
            SitePage CurrentSitePage = this.Engine.SiteManager.GetPage(PageId);
            SitePageTemplate CurrentPageTemplate = this.Engine.SiteManager.GetPageTemplate(CurrentSitePage.TemplateId);

            string PageTemplateContent = CurrentPageTemplate == null ? string.Empty : CurrentPageTemplate.HtmlContent;

            //string jsvar = PageTemplateContent;
            string Title = this.SetPageTitle(CurrentSitePage, CurrentPageTemplate);
            //List<string> WebPartInstValue = this.WebPartInstValue(PageId);
            List<object> WebPartInstValue = this.WebPartInstValue(PageId);
            List<ListItem> AllWebParts = this.GetWebParts();
            var result = new
            {
                PageId = PageId,
                Title = Title,
                PageTemplateContent = PageTemplateContent,
                WebPartInstValue = WebPartInstValue,
                //WebPartInstValue_Bak = WebPartInstValue_Bak,
                AllWebParts = AllWebParts,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取移动端所有数据模型
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMobileAllWebParts()
        {
            List<ListItem> AllWebParts = this.GetWebParts();
            List<object> WebPartInstValue = this.GetMobileWebPartInstValue();
            var result = new
            {
                AllWebParts = AllWebParts,
                WebPartInstValue = WebPartInstValue
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public List<object> GetMobileWebPartInstValue()
        {
            List<object> result = new List<object>();
            SiteWebPartInstance[] webparts = this.Engine.SiteManager.GetAllWebPartInsts();
            ArrayList webPartsList = new ArrayList();
            ArrayList keyList = new ArrayList();
            for (int i = 0; i < webparts.Length; i++)
            {
                webPartsList.Add(webparts[i]);
                keyList.Add(webparts[i].SortKey);
            }
            webPartsList = OThinker.Data.Sorter.Sort(keyList, webPartsList);

            foreach (SiteWebPartInstance webpart in webPartsList)
            {
                if (webpart.PagePart == "MobileDataModel")
                {
                    if (!string.IsNullOrEmpty(webpart.FunctionCode))
                    {
                        string functionCode = webpart.FunctionCode;
                        if (!ValidateFunction(functionCode)) continue;
                    }
                    List<ListItem> Item = new List<ListItem>();
                    Item.Add(new ListItem("ObjectID", webpart.ObjectID));
                    Item.Add(new ListItem("PageID", webpart.PageId));
                    Item.Add(new ListItem("WebPartID", webpart.WebPartId));
                    Item.Add(new ListItem("WebPartPost", webpart.PagePart));
                    SiteWebPartInstanceValue[] InstanceValue = webpart.Attributes;
                    foreach (SiteWebPartInstanceValue item in InstanceValue)
                    {
                        string name = item.AttributeName;
                        string value = item.AttributeValue;
                        Item.Add(new ListItem(name, value));
                    }
                    result.Add(Item);
                }
            }
            return result;
        }

        /// <summary>
        /// 设置页面标题
        /// </summary>
        /// <returns></returns>
        public string SetPageTitle(SitePage CurrentSitePage, SitePageTemplate CurrentPageTemplate)
        {
            string Title = "";
            if (CurrentSitePage != null && !string.IsNullOrEmpty(CurrentSitePage.Title))
            {
                Title = CurrentSitePage.Title;
            }
            else if (CurrentPageTemplate != null && !string.IsNullOrEmpty(CurrentPageTemplate.TemplateName))
            {
                Title = CurrentPageTemplate.TemplateName;
            }
            else
            {
                Title = "Top_Home";//首页，多语言支持
            }
            return Title;
        }

        public List<ListItem> GetWebParts()
        {
            List<ListItem> AllWebParts = new List<ListItem>();
            SiteWebPart[] webparts = this.Engine.SiteManager.GetAllWebParts();
            foreach (SiteWebPart webpart in webparts)
            {
                var WebPart = new
                {
                    ObjectID = webpart.ObjectID,
                    DisplayName = webpart.DisplayName
                };
                AllWebParts.Add(new ListItem(webpart.DisplayName, webpart.ObjectID));
            }
            return AllWebParts;
        }

        public List<object> WebPartInstValue(string PageId)
        {
            List<object> result = new List<object>();
            SiteWebPartInstance[] webparts = this.Engine.SiteManager.GetAllWebPartInsts();
            ArrayList webPartsList = new ArrayList();
            ArrayList keyList = new ArrayList();
            for (int i = 0; i < webparts.Length; i++)
            {
                webPartsList.Add(webparts[i]);
                keyList.Add(webparts[i].SortKey);
            }
            webPartsList = OThinker.Data.Sorter.Sort(keyList, webPartsList);

            foreach (SiteWebPartInstance webpart in webPartsList)
            {
                if (webpart.PageId == PageId)
                {
                    if (!string.IsNullOrEmpty(webpart.FunctionCode))
                    {
                        string functionCode = webpart.FunctionCode;
                        if (!ValidateFunction(functionCode)) continue;
                    }
                    List<ListItem> Item = new List<ListItem>();
                    Item.Add(new ListItem("ObjectID", webpart.ObjectID));
                    Item.Add(new ListItem("PageID", webpart.PageId));
                    Item.Add(new ListItem("WebPartID", webpart.WebPartId));
                    Item.Add(new ListItem("WebPartPost", webpart.PagePart));
                    SiteWebPartInstanceValue[] InstanceValue = webpart.Attributes;
                    foreach (SiteWebPartInstanceValue item in InstanceValue)
                    {
                        string name = item.AttributeName;
                        string value = item.AttributeValue;
                        Item.Add(new ListItem(name, value));
                    }
                    result.Add(Item);
                }
            }
            return result;
        }


        public List<PortalPageViewModel> WebPartInstValue_Bak(string PageId)
        {
            List<PortalPageViewModel> griddata = new List<PortalPageViewModel>();

            string condition = "PageId='" + PageId + "'";
            SiteWebPartInstance[] webparts = this.Engine.SiteManager.GetAllWebPartInsts();
            ArrayList webPartsList = new ArrayList();
            ArrayList keyList = new ArrayList();
            for (int i = 0; i < webparts.Length; i++)
            {
                webPartsList.Add(webparts[i]);
                keyList.Add(webparts[i].SortKey);
            }
            webPartsList = OThinker.Data.Sorter.Sort(keyList, webPartsList);
            if (webPartsList != null && webPartsList.Count > 0)
            {
                if (this.PageDataObject == null)
                    this.PageDataObject = this.Engine.SiteManager.GetPage(PageId);
            }
            foreach (SiteWebPartInstance webpart in webPartsList)
            {
                if (webpart.PageId == PageId)
                {
                    if (!string.IsNullOrEmpty(webpart.FunctionCode))
                    {
                        string functionCode = webpart.FunctionCode;
                        if (!ValidateFunction(functionCode)) continue;
                    }
                    SiteWebPart SiteWebPart = this.Engine.SiteManager.GetWebPart(webpart.WebPartId);
                    var WebPartName = string.Empty;
                    if (SiteWebPart != null)
                    {
                        WebPartName = SiteWebPart.DisplayName;
                    }
                    griddata.Add(new PortalPageViewModel
                    {
                        ObjectID = webpart.ObjectID + string.Empty,
                        PageID = webpart.PageId + string.Empty,
                        WebPartID = webpart.WebPartId + string.Empty,
                        WebPartName = WebPartName,
                        WebPartPost = webpart.PagePart + string.Empty,
                        HtmlContent = webpart.HtmlContent + string.Empty,
                        Title = this.GetWebPartAttrValue(webpart, "Title"),
                        FunctionCode = this.GetWebPartAttrValue(webpart, "FunctionCode"),
                        TitleVisible = this.GetWebPartAttrValue(webpart, "TitleVisible").ToLower() == "false" ? 0 : 1,
                        TitleIcon = this.GetWebPartAttrValue(webpart, "TitleIcon"),
                        TitleIconVisible = this.GetWebPartAttrValue(webpart, "TitleIconVisible").ToLower() == "false" ? 0 : 1,
                        CssName = this.GetWebPartAttrValue(webpart, "CssName"),
                        Width = this.GetWebPartAttrValue(webpart, "Width"),
                        WidthUnit = this.GetWebPartAttrValue(webpart, "WidthUnit"),
                        Height = this.GetWebPartAttrValue(webpart, "Height"),
                        HeightUnit = this.GetWebPartAttrValue(webpart, "HeightUnit"),
                        ControlPath = this.GetWebPartAttrValue(webpart, "ControlPath"),
                        MoreLink = this.GetWebPartAttrValue(webpart, "MoreLink"),
                        MoreText = this.GetWebPartAttrValue(webpart, "MoreText"),
                        MorePos = this.GetWebPartAttrValue(webpart, "MorePos")
                    });
                }
            }
            return griddata;
        }

        /// <summary>
        /// 返回控件属性值
        /// </summary>
        /// <param name="webpart"></param>
        /// <param name="AttrName"></param>
        /// <returns></returns>
        public string GetWebPartAttrValue(SiteWebPartInstance webpart, string AttrName)
        {
            string value = webpart.Attributes.ToList().Where(e => e.AttributeName == AttrName).Count() == 0 ? "" : webpart.Attributes.ToList().Where(e => e.AttributeName == AttrName).FirstOrDefault().AttributeValue;
            return value;
        }

        /// <summary>
        /// 将UserControl添加到Page的ViewState
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        private Dictionary<string, string> AddControlToViewState(string id, string webpartId)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(id, webpartId);
            return dict;
        }

        /// <summary>
        /// 验证是否有权限访问当前编码
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private bool ValidateFunction(string FunctionCode)
        {
            if (FunctionCode == string.Empty) return true;

            return this.UserValidator.ValidateFunctionRun(FunctionCode);
        }
    }
}
