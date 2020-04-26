using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using System.Configuration;
using DZ.External.WebApi.Common;

namespace DZ.External.WebApi.Controllers
{
    public class GetTokenController : Controller
    {
       public static string time = ConfigurationManager.AppSettings["expires_time"];
        public static string key = "h3bpmsys_key";
        // GET: GetToken
        public JsonResult Index(string corpid, string secret)
        {
            int errCode = 0;
            string errMsg = "";
            string token = "";
            int expires_time = 0;
            #region 条件判断
            if (string.IsNullOrEmpty(time))
            {
                errCode = 10000;
                errMsg = "expires_time setting missing";
            }
            else if (string.IsNullOrEmpty(corpid))
            {
                errCode = 10001;
                errMsg = "corpid missing";
            }
            else if (string.IsNullOrEmpty(secret))
            {
                errCode = 10002;
                errMsg = "secret missing";
            }
            if (errCode != 0)
            {
                return Json(new
                {
                    errcode = errCode,
                    errmsg = errMsg,
                    access_token = token,
                    expires_in = expires_time
                }, JsonRequestBehavior.AllowGet);
            }
            var companyInfo = AppUtility.Engine.SSOManager.GetSSOSystem(corpid);
            if (companyInfo == null)
            {
                errCode = 10003;
                errMsg = "corpid error";
            }
            else if (companyInfo.Secret != MD5Encryptor.GetMD5(secret))
            {
                errCode = 10004;
                errMsg = "secret error";
            }
            if (errCode != 0)
            {
                return Json(new
                {
                    errcode = errCode,
                    errmsg = errMsg,
                    access_token = token,
                    expires_in = expires_time
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            int T = Convert.ToInt32(time);
            string encryptString = corpid + "|" + secret + "|" + System.DateTime.Now.Ticks + "|" + T;

            token = EncryptHelper.Encrypt(encryptString, key);

            return Json(new
            {
                errcode = 0,
                errmsg = "",
                access_token = token,
                expires_in = T
            }, JsonRequestBehavior.AllowGet);
        }


    }
}