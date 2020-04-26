using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OThinker.H3.Site;
using OThinker.H3.Controllers.ViewModels;
using System.Reflection;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers
{
    public class PortalHandlerController : PortalWebPartSettingPage
    {

        public String GetPageId(string PageId)
        {
            if (string.IsNullOrEmpty(PageId))
            {
                return Guid.Empty.ToString();
            }
            Guid strguid;
            if (!Guid.TryParse(PageId, out strguid))
            {
                return Guid.Empty.ToString();
            }
            return PageId;
        }

        [HttpPost]
        public JsonResult SaveWebPart(string PageId, string WebPartObjectID, string WebPartID, string WebPartPost, string PublicValue, string PrivateValue)
        {
            return this.ExecuteFunctionRun(() =>
            {
                PageId = this.GetPageId(PageId);
                ActionResult result = new ActionResult(true);
                SiteWebPartInstance siteWebPaerInst = this.Engine.SiteManager.GetWebPartInst(WebPartObjectID);
                if (siteWebPaerInst != null)
                {
                    this.InstId = siteWebPaerInst.ObjectID;
                }
                else
                {
                    //添加插件到页面,初始化
                    this.AddWebPartToPage(WebPartID, PageId, WebPartPost, "");
                }

                Dictionary<string, object> PublicParams = this.GetValue(PublicValue);
                Dictionary<string, object> PrivateParams = this.GetValue(PrivateValue);

                this.SavePublic(PublicParams);
                this.SavePrivate(PrivateParams);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public Dictionary<string, object> GetValue(string Params)
        {
            Dictionary<string, object> dicParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(Params);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string key in dicParams.Keys)
            {
                try
                {
                    dict.Add(key, dicParams[key]);
                }
                catch
                {
                    dict.Add(key, "");
                }
            }
            return dict;
        }

        /// <summary>
        /// 移除部件
        /// </summary>
        /// <param name="WebPartID">部件实例ID</param>
        /// <returns></returns>
        public JsonResult RemoveWebPartFromPage(string InstanceID)
        {
            if (string.IsNullOrEmpty(InstanceID))
            {
                return Json("缺少参数[实例ID]", JsonRequestBehavior.AllowGet);
            }
            //删除实例             
            bool result = this.Engine.SiteManager.DeleteWebPartInstById(InstanceID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取编辑部件的属性信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult GetWebPartInstEditValue(string InstanceID)
        {
            if (string.IsNullOrEmpty(InstanceID))
            {
                return Json("缺少参数[实例ID]", JsonRequestBehavior.AllowGet);
            }
            SiteWebPartInstance webpart = this.Engine.SiteManager.GetWebPartInst(InstanceID);
            if (webpart == null)
            {
                return Json("部件实例不存在", JsonRequestBehavior.AllowGet);
            }

            List<ListItem> Item = new List<ListItem>();

            SiteWebPartInstanceValue[] InstanceValue = webpart.Attributes;
            foreach (SiteWebPartInstanceValue item in InstanceValue)
            {
                string name = item.AttributeName;
                string value = item.AttributeValue;
                Item.Add(new ListItem(name, value));
            }
            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 拖拽插件到其他位置
        /// </summary>
        /// <param name="WebPartIDs">该位置的所有插件</param>
        /// <param name="PartID">位置id</param>
        /// <returns></returns>
        public JsonResult PageWebPartSort(string[] WebPartIDs, string PartID)
        {
            bool result = false;
            if (WebPartIDs == null || WebPartIDs.Length == 0)
            { }
            else
            {
                for (int i = 0; i < WebPartIDs.Length; i++)
                {
                    SiteWebPartInstance webPart = this.Engine.SiteManager.GetWebPartInst(WebPartIDs[i]);
                    if (webPart != null)
                    {
                        webPart.SortKey = i;
                        webPart.PagePart = PartID;
                        this.Engine.SiteManager.UpdateWebPartInst(webPart);
                    }
                }
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加插件到页面，保存时
        /// </summary>
        /// <param name="webpartId">部件id,唯一部件</param>
        /// <param name="pageId">00000000-0000-0000-0000-000000000000  Guid.NewGuid().ToString()</param>
        /// <param name="part">部件显示区域</param>
        /// <param name="webpartname"></param>
        public Dictionary<string, string> AddWebPartToPage(string webpartId, string pageId, string part, string webpartname)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            OThinker.H3.Site.SitePage page = this.Engine.SiteManager.GetPage(pageId);
            if (page == null || page == default(OThinker.H3.Site.SitePage))
            {
                result.Add("Error", "页面不存在");
                return result;
            }

            SiteWebPartInstance inst = new SiteWebPartInstance();
            int recordCount = 0;
            int pageCount = 0;
            SiteWebPartInstance[] webparts = this.Engine.SiteManager.GetAllWebPartInsts();
            if (webparts != null && webparts.Length > 0)
            {
                int i = 0;
                foreach (SiteWebPartInstance webpart in webparts)
                {
                    if (webpart.PagePart == part && webpart.PageId == pageId)
                    {
                        i++;
                    }
                }
                recordCount = i;
                pageCount = recordCount % 1 > 0 ? recordCount / 1 + 1 : recordCount / 1;
            }
            inst.CreatedTime = DateTime.Now;
            inst.CreatedBy = UserValidatorFactory.CurrentUser.UserID;
            inst.PageId = pageId;
            inst.PagePart = part;
            inst.SortKey = recordCount + 1;
            inst.WebPartId = webpartId;

            List<SiteWebPartInstanceValue> source = new List<SiteWebPartInstanceValue>();
            OThinker.H3.Site.SiteWebPartInstanceValue obj = new OThinker.H3.Site.SiteWebPartInstanceValue();
            obj.AttributeName = "Title";
            obj.AttributeType = AttributeType.PublicAttribute;
            obj.AttributeValue = webpartname;
            obj.InstanceId = webpartId;
            obj.ParentIndex = 1;

            source.Add(obj);
            inst.Attributes = source.ToArray();
            this.Engine.SiteManager.AddWebPartInst(inst);
            result.Add("Success", inst.ObjectID);
            this.InstId = inst.ObjectID;
            return result;
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
        /// 保存私有配置
        /// </summary>
        /// <param name="dict"></param>
        protected void SavePrivate(Dictionary<string, object> dict)
        {
            base.SavePrivateConfigs(dict);
        }

        /// <summary>
        /// 保存公共配置
        /// </summary>
        /// <param name="dict"></param>
        public void SavePublic(Dictionary<string, object> dict)
        {
            // 更新权限编码
            //WebPartInstance.FunctionCode = this.txtFunctionCode.Text.Trim();
            //this.Engine.SiteManager.UpdateWebPartInst(WebPartInstance);
            // 调用保存方法
            base.SavePublicConfigs(dict);
        }



        private SiteWebPartInstance _WebPartInstance = null;
        /// <summary>
        /// 获取WebPart部件的实例
        /// </summary>
        private SiteWebPartInstance WebPartInstance
        {
            get
            {
                if (this._WebPartInstance == null)
                {
                    this._WebPartInstance = this.Engine.SiteManager.GetWebPartInst(this.InstId);
                }
                return this._WebPartInstance;
            }
        }
    }
}
