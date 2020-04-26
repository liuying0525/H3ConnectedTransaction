using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public string Index()
        {
            return "Test Control";
        }

        [HttpPost]
        public JsonResult HttpPost(List<PageInfo> p)
        {
            var pi = p.Where(a => a.code == "123").FirstOrDefault();
            if (pi == null)
                return Json(new { aa = "", bb = "" });
            else
                return Json(new { aa = pi.size + pi.code, bb = "bb" + pi.name });
        }

        public JsonResult GetPageInfoList(string n)
        {
            int num = Convert.ToInt32(n);
            List<PageInfo> data = new List<PageInfo>();
            for (int i = 0; i < num; i++)
            {
                PageInfo pi = new PageInfo();
                pi.size = i.ToString();
                pi.code = Guid.NewGuid().ToString();
                pi.name = "Name" + i;
                data.Add(pi);
            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult test(string s)
        {
            string url = "http://172.16.11.23:80/api/batchQueryCustomerPrecredit";
            List<object> body = new List<object>();
            body.Add(new
            {
                name = "test",
                idCardNo = "12345678901211",
                phone = "15012345321"
            });
            body.Add(new
            {
                name = "test1",
                idCardNo = "123456789013333",
                phone = "15012345322"
            });
            var paras = new
            {
                requestSource = "h3",
                instanceId = "56789",
                clientQueryList = body
            };
            //var ret = HttpHelper.PostWebRequest(url, "application/json", Newtonsoft.Json.JsonConvert.SerializeObject(paras));
            return Json(new { reqId="123",recordId="1234" }, JsonRequestBehavior.AllowGet);
            //NameValueCollection nv = new NameValueCollection();
            //nv.Add("requestSource", "h3");
            //nv.Add("instanceId", "56789");
            //nv.Add("clientQueryList",);
            //HttpHelper.postFormData("",null,)
            //HttpHelper.postFormData(url, new NameValueCollection(), body)
        }

        public class PageInfo
        {
            public string size { get; set; }
            public string code { get; set; }
            public string name { get; set; }
        }
    }
}