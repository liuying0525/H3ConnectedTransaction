using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OThinker.H3.Site;

namespace OThinker.H3.Controllers
{
    public class PortalWebPartSettingPage : ControllerBase
    {
        public PortalWebPartSettingPage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public JsonResult Test()
        {
            return Json("111", JsonRequestBehavior.AllowGet);
        }

        private OThinker.H3.IEngine _Engine = null;
        /// <summary>
        /// 获取引擎对象
        /// </summary>
        public OThinker.H3.IEngine Engine
        {
            get
            {
                if (_Engine == null)
                {
                    _Engine = AppUtility.Engine;
                }
                return _Engine;
            }
        }

        private string _InstId = null;
        /// <summary>
        /// 当前设置页面的实例ID
        /// </summary>
        protected string InstId
        {
            get
            {
                return _InstId;
            }
            set
            {
                this._InstId = value;
            }
        }

        /// <summary>
        /// 保存插件实例的私有配置
        /// </summary>
        /// <param name="dict">dictionary可以的值可以保存string, list&lt;string&gt;</param>
        protected void SavePrivateConfigs(Dictionary<string, object> dict)
        {
            SaveConfigs(dict, OThinker.H3.Site.AttributeType.PrivateAttribute);
        }

        /// <summary>
        /// 保存插件实例的公有配置
        /// </summary>
        /// <param name="dict">dictionary可以的值可以保存string, list&lt;string&gt;</param>
        protected void SavePublicConfigs(Dictionary<string, object> dict)
        {
            SaveConfigs(dict, AttributeType.PublicAttribute);
        }

        /// <summary>
        /// 统一保存配置
        /// </summary>
        /// <param name="dict"></param>
        private void SaveConfigs(Dictionary<string, object> dict, AttributeType AttrType)
        {
            string instId = this.InstId;
            if (string.IsNullOrEmpty(instId))
                return;

            //先找到PluginInstVal 对象，先删除再重新添加

            SiteWebPartInstance instance = Engine.SiteManager.GetWebPartInst(instId);
            if (instance != null)
            {
                List<SiteWebPartInstanceValue> source = new List<SiteWebPartInstanceValue>();
                if (instance.Attributes != null)
                {
                    source = instance.Attributes.ToList();
                    var values = source.Where(e => e.AttributeType == AttrType).ToList();
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            source.Remove((SiteWebPartInstanceValue)value);
                        }
                    }
                }

                //移除缓存，如果有的话
                //PortalCache<string, string, string>.Instance.FindItem(PortalConst.CacheName_WebPartInstContent).Remove(instId);
                List<string> attrinfo = null;
                foreach (var pv in dict)
                {
                    //如果不存在，就不保存了，确保属性一定要定义
                    if (!IsAttribteExists(pv.Key, AttrType))
                        continue;
                    attrinfo = GetAttributeTypeAndFormat(pv.Key, AttrType);
                    //多值状态，一个属性多个值，用List<string>传进来，其余的都用string
                    if (pv.Value is List<string>)
                    {
                        List<string> multiattrs = pv.Value as List<string>;
                        int ix = 1;//给多值保存一个顺序
                        foreach (string str in multiattrs)
                        {
                            OThinker.H3.Site.SiteWebPartInstanceValue obj = new OThinker.H3.Site.SiteWebPartInstanceValue();
                            obj.AttributeName = pv.Key;
                            obj.AttributeType = AttrType;
                            obj.AttributeValue = str;
                            // obj.BindType = attrinfo[0];
                            obj.Format = attrinfo[1];
                            obj.InstanceId = instId;
                            obj.ParentIndex = ix;
                            ix++;
                            source.Add(obj);
                        }
                    }
                    else
                    {
                        OThinker.H3.Site.SiteWebPartInstanceValue obj = new OThinker.H3.Site.SiteWebPartInstanceValue();
                        obj.AttributeName = pv.Key;
                        obj.AttributeType = AttrType;
                        obj.AttributeValue = pv.Value.ToString();
                        // obj.BindType = attrinfo[0];
                        obj.Format = attrinfo[1];
                        obj.InstanceId = instId;
                        source.Add(obj);
                    }
                }
                instance.Attributes = source.ToArray();
                Engine.SiteManager.UpdateWebPartInst(instance);
            }

        }


        private List<OThinker.H3.Site.SiteWebPartPrivateAttribute> _privateattrlist = null;
        private List<OThinker.H3.Site.SiteWebPartPublicAttribute> _publicattrlist = null;
        /// <summary>
        /// 检查属性名字是否合法
        /// </summary>
        /// <param name="AttributeName">属性名</param>
        /// <param name="AttributeType">public, private</param>
        private bool IsAttribteExists(string AttributeName, AttributeType AttrType)
        {
            if (_privateattrlist == null || _privateattrlist.Count == 0)
                _privateattrlist = GetPrivateAttributes();
            if (_publicattrlist == null)
                _publicattrlist = GetPublicAttributes();

            bool found = false;
            if (AttrType == AttributeType.PublicAttribute)
            {
                foreach (var attr in _publicattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        found = true;
                        break;
                    }
                }
            }
            if (AttrType == AttributeType.PrivateAttribute)
            {
                foreach (var attr in _privateattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }
        /// <summary>
        /// 获取属性的BindType与Format, 返回2个长度的List&lt;string&gt;
        /// </summary>
        /// <param name="AttributeName">属性名</param>
        /// <param name="AttributeType">公有, 私有</param>
        /// <returns></returns>
        private List<string> GetAttributeTypeAndFormat(string AttributeName, AttributeType AttrType)
        {
            List<string> list = new List<string>();
            if (AttrType == AttributeType.PublicAttribute)
            {
                foreach (var attr in _publicattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        list.Add(attr.BindType);
                        list.Add(attr.Format);
                        break;
                    }
                }
            }
            if (AttrType == AttributeType.PrivateAttribute)
            {
                foreach (var attr in _privateattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        list.Add(attr.BindType);
                        list.Add(attr.Format);
                        break;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取当前实例下plugin的所有私有属性
        /// </summary>
        /// <returns></returns>
        protected List<OThinker.H3.Site.SiteWebPartPrivateAttribute> GetPrivateAttributes()
        {
            if (string.IsNullOrEmpty(this.InstId))
                return new List<OThinker.H3.Site.SiteWebPartPrivateAttribute>();

            SiteWebPartInstance instance = Engine.SiteManager.GetWebPartInst(this.InstId);

            SiteWebPart[] parts = Engine.SiteManager.GetAllWebParts();
            var attrs = parts.ToList().Where(e => e.ObjectID == instance.WebPartId).FirstOrDefault().PrivateAttributes;
            //替换本地语言
            if (attrs != null)
            {
                foreach (var attr in attrs)
                {
                    //attr.AttributeTitle = GetPrivateLanguageText(attr.AttributeName, attr.AttributeTitle);
                }
                return attrs.ToList();
            }
            return null;
        }

        /// <summary>
        /// 获取当前实例下plugin的所有公有属性
        /// </summary>
        /// <returns></returns>
        protected List<OThinker.H3.Site.SiteWebPartPublicAttribute> GetPublicAttributes()
        {
            List<OThinker.H3.Site.SiteWebPartPublicAttribute> list = new List<OThinker.H3.Site.SiteWebPartPublicAttribute>();
            if (string.IsNullOrEmpty(this.InstId))
                return list;

            SiteWebPartPublicAttribute[] attrs = Engine.SiteManager.GetAllWebPartPublicAttrs();
            if (attrs != null)
            {
                foreach (SiteWebPartPublicAttribute attr in attrs)
                {
                    list.Add(attr);
                }
            }

            //替换本地语言
            //for (var i = 0; i < list.Count; i++)
            //{
            //    list[i].AttributeTitle = GetPublicLanguageText("UI_PUBLICATTR_" + list[i].AttributeName.ToUpper(), list[i].AttributeTitle);
            //}
            return list;
        }

        /// <summary>
        /// 获取属性的描述
        /// </summary>
        /// <param name="AttributeName">属性名</param>
        /// <param name="AttributeType">public, private</param>
        /// <returns></returns>
        private string GetAttributeTitle(string AttributeName, AttributeType AttrType)
        {
            string title = "";
            if (AttrType == AttributeType.PublicAttribute)
            {
                foreach (var attr in _publicattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        title = attr.AttributeTitle;
                        break;
                    }
                }
            }
            if (AttrType == AttributeType.PrivateAttribute)
            {
                foreach (var attr in _privateattrlist)
                {
                    if (attr.AttributeName == AttributeName)
                    {
                        title = attr.AttributeTitle;
                        break;
                    }
                }
            }
            return title;
        }


        public override string FunctionCode
        {
            get { return ""; }
        }
    }


    public class WebPartInstValComparison
    {
        public int Compare(OThinker.H3.Site.SiteWebPartInstanceValue x, OThinker.H3.Site.SiteWebPartInstanceValue y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int value = x.SortKey.CompareTo(y.SortKey);
                    if (value == 0)
                        value = x.AttributeName.CompareTo(y.AttributeName);
                    return value;
                }
            }
        }
    }
}
