using Aspose.Pdf;
using Newtonsoft.Json.Linq;
using System; 
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    /// <summary>
    /// 检测SQL注入 ErrCode = 800 sql inject
    /// wangxg 19.7.3
    /// 用法：[SqlInject]加在类或方法上的特性
    /// </summary>
    public class SqlInjectAttribute : ActionFilterAttribute
    {
        protected NameValueCollection param = new NameValueCollection();
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            param.Clear();

            ContentResult Content = new ContentResult();

            #region 获取URL和post参数，用于确认是否含有敏感信息

            try
            {
                var q = filterContext.RequestContext.HttpContext.Request;
                param.Add(q.QueryString);
                param.Add(q.Form);
            }
            catch (Exception err)// 在获取request参数时系统会自动检测是否含有危险信息
            {
                Content.Content = "您提交的数据中检测到有潜在危险的信息。";
                filterContext.Result = Content;
                filterContext.HttpContext.Response.StatusCode = 800;
                filterContext.HttpContext.Response.StatusDescription = "sensitive information";
                return;
            }

            #endregion

            // SQL注入验证开关
            var sqlInject = System.Configuration.ConfigurationManager.AppSettings.Get("SqlInject");
            if (sqlInject != null && (sqlInject.ToLower().Equals("0") || sqlInject.ToLower().Equals("false")))
                return;

            #region 检测SQL注入

            if (param != null && param.Count > 0)
            {
                foreach (var p in param.AllKeys)
                {
                    if (p == null || p.ToLower().Equals("authorization")) continue;
                    string msg = "";
                    string v = param.Get(p);
                    if (IsSqlInjectCharacter(v, out msg))
                    {
                        Content.Content = msg;
                        filterContext.Result = Content;
                        filterContext.HttpContext.Response.StatusCode = 800;
                        filterContext.HttpContext.Response.StatusDescription = "sql inject";
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
                                    filterContext.HttpContext.Response.StatusDescription = "sql inject";
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // 若输入流不是json对象不再校验
                    }                   
                }
            }

            #endregion
           
        }

        public bool IsSqlInjectCharacter(object value, out string msg)
        {
            msg = "";
            // 以下第一个注释掉的match是绿盟提供的正则，第二个是修改后的正则（由于业务存在关键字符） wangxg 19.7.4
            var match = Regex.IsMatch(value.ToString(), "(?:')|(?:--)|(/\\*(?:.|[\\n\\r])*?\\*/)|" + "(\\b(select|update|union|and|or|delete|insert|trancate|char|into|substr|ascii|declare|exec|count|master|into|drop|execute)\\b)");
            //var match = Regex.IsMatch(value.ToString(), "(?:--)|(/\\*(?:.|[\\n\\r])*?\\*/)|" + "(\\b(select|update|union|and|or|delete|insert|trancate|char|into|substr|ascii|declare|exec|count|master|into|drop|execute)\\b)");
            if (match)
            {
                msg = "您提交的内容检测到SQL攻击信息。";
                return true;
            }
            return false;
        }


        /// <summary>
        /// 递归验证以application/json方式post上来的stream格式数据
        /// </summary>
        /// <param name="jo"></param>
        /// <returns></returns>
        protected int ChkJson(JToken jo, out string msg)
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
                if (IsSqlInjectCharacter(val, out msg))
                {
                    return 800;
                }
            }

            return 0;
        }

    }
}
