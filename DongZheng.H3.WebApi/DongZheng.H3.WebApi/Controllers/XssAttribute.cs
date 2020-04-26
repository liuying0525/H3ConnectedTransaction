
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    /// <summary>
    /// 验证XSS注入 ErrCode = 801 sensitive information
    /// wangxg 19.7.4
    /// 用法：[Xss]加在类或方法上的特性
    /// </summary>
    public class XssAttribute : SqlInjectAttribute
    {
        #region add by chenghs 2019-06-21 ：漏洞修复；

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ContentResult Content = new ContentResult();

            // xss验证开关
            var xss = System.Configuration.ConfigurationManager.AppSettings.Get("Xss");
            if (xss != null && (xss.ToLower().Equals("0") || xss.ToLower().Equals("false")))
                return;

            #region 检验XSS注入

            if (param != null && param.Count > 0)
            {
                foreach (var p in param.AllKeys)
                {
                    string msg = "";
                    string v = param.Get(p);
                    if (IsContainXSSCharacter(v, out msg))
                    {
                        Content.Content = msg;
                        filterContext.Result = Content;
                        filterContext.HttpContext.Response.StatusCode = 801;
                        filterContext.HttpContext.Response.StatusDescription = "sensitive information";
                        return;
                    }
                }
            }

            #endregion

            #region application/json 方式请求要从流中读取

            var req = filterContext.RequestContext.HttpContext.Request;
            if (req.ContentType.ToLower().Contains("application/json") && req.InputStream.Length > 0)
            {
                System.IO.Stream stm = new MemoryStream();
                req.InputStream.CopyTo(stm);
                stm.Position = 0;
                req.InputStream.Position = 0;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stm))
                {
                    try
                    {
                        Newtonsoft.Json.Linq.JObject jo = Newtonsoft.Json.Linq.JObject.Parse(sr.ReadToEnd());
                        if (jo.HasValues)
                        {
                            foreach (JToken item in jo.Values())
                            {
                                var tmpMsg = "";
                                int ckResult = ChkJson(item, out tmpMsg);
                                if (ckResult != 0)
                                {
                                    Content.Content = tmpMsg;
                                    filterContext.Result = Content;
                                    filterContext.HttpContext.Response.StatusCode = ckResult;
                                    filterContext.HttpContext.Response.StatusDescription = "sensitive information";
                                    return;
                                }
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                        // 若输入流不是json对象不再校验
                    }

                }
            }

            #endregion

        }

        public bool IsContainXSSCharacter(object value, out string msg)
        {
            msg = "";
            if (value == null)
                return false;
            // 不过滤的字符  "|", ";", ",", 
            string[] specialCharacters = new string[] { "||", "&", "$", "%", "@", "'", "\"", "\\'", "\\\"",
                "<", ">", "(", ")", "+", "\r", "\n", "\\", "/", "*", "&" };

            string[] keyWords = new[] { "javascript", "vbscript", "expression", "applet", "meta", "xml", "blink", "link", "style", "script", "embed", "object", "iframe", "frame", "frameset", "ilayer", "layer", "bgsound", "title", "base"
        ,"onabort", "onactivate", "onafterprint", "onafterupdate", "onbeforeactivate", "onbeforecopy", "onbeforecut", "onbeforedeactivate", "onbeforeeditfocus", "onbeforepaste", "onbeforeprint", "onbeforeunload", "onbeforeupdate", "onblur", "onbounce", "oncellchange", "onchange", "onclick", "oncontextmenu", "oncontrolselect", "oncopy", "oncut", "ondataavailable", "ondatasetchanged", "ondatasetcomplete", "ondblclick", "ondeactivate", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "onerror", "onerrorupdate", "onfilterchange", "onfinish", "onfocus", "onfocusin", "onfocusout", "onhelp", "onkeydown", "onkeypress", "onkeyup", "onlayoutcomplete", "onload", "onlosecapture", "onmousedown", "onmouseenter", "onmouseleave", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onmove", "onmoveend", "onmovestart", "onpaste", "onpropertychange", "onreadystatechange", "onreset", "onresize", "onresizeend", "onresizestart", "onrowenter", "onrowexit", "onrowsdelete", "onrowsinserted", "onscroll", "onselect", "onselectionchange", "onselectstart", "onstart", "onstop", "onsubmit", "onunload"
            , "img"};

            if (value.GetType() == typeof(string))
            {
                string v = value + string.Empty;
                if (!string.IsNullOrEmpty(v))
                {
                    // 先判断提交信息是否包含关键字，若包含则kw=关键字
                    string kw = "";
                    foreach (var k in keyWords)
                    {
                        if (v.ToLower().Contains(k.ToLower()))
                        {
                            kw = k;
                            break;
                        }
                    }
                    if(!string.IsNullOrEmpty(kw))// 若包含关键字
                    {
                        foreach (var c in specialCharacters)
                        {
                            if (v.ToLower().Contains(c))
                            {
                                var match = Regex.IsMatch(v, $"(\\b({kw})\\b)");
                                if (match)
                                {
                                    msg = $"禁止敏感符号:[{kw}，{c}]。";
                                    return true;
                                }                               
                            }
                        }
                    }
                    
                }
            }
            return false;
        }

        /// <summary>
        /// 递归验证以application/json方式post上来的stream格式数据
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        protected new int ChkJson(JToken jo, out string msg)
        {
            msg = "";
            if (jo == null) return 0;
            if (jo.HasValues && jo.Values().Count() > 0)
            {
                foreach (JToken item in jo.Values())
                {
                    var result = ChkJson(item, out msg);
                    if (result != 0)
                        return result;
                }
            }
            else
            {
                string val = jo.ToString();
                if (IsContainXSSCharacter(val , out msg)){
                    return 801;
                }
            }

            return 0;
        }

        #endregion

    }
}
