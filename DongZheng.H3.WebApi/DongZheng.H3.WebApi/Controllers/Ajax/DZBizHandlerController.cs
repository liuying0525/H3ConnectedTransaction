using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.Portal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WorkFlowFunction = DongZheng.H3.WebApi.Common.Portal.WorkFlowFunction;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    public class DZBizHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        [Xss]
        public string Index(string CommandName)
        {
            var context = HttpContext;
            string result = "";
            switch (CommandName)
            {
                case "indvidualHistoryFinancedAmount": result = indvidualHistoryFinancedAmount(context); break;
                case "getIndvidualApplicationHistory": result = getIndvidualApplicationHistory(context); break;
                case "getInstancedata": result = getInstancedata(context); break;
                //case "getRejectWorkItem": result = getRejectWorkItem(context); break;
                case "insertLYInfo": result = insertLYInfo(HttpContext); break;
                case "getLYInfo": result = getLYInfo(context); break;
                case "BizAssign": result = BizAssign(context); break;
                case "getCAPckje": result = getCAPckje(context); break;
                case "getCMSckje": result = getCMSckje(context); break;
                case "getGJRck": result = getGJRck(context); break;
                case "getinstenceid": result = getinstenceid(context); break;
                case "getCinstenceid": result = getCinstenceid(context); break;
                //case "getssx": result = getssx(context); break;
                //case "getdycs": result = getdycs(context); break;
                //case "searchdycs": result = searchdycs(context); break;
                //case "updatedycs": result = updatedycs(context); break;
                //case "downloaddycs": result = downloaddycs(context); break;
                //case "deldycs": result = deldycs(context); break;
                case "ldaplogin": result = ldaplogin(context); break;
                case "gethknl": result = gethknl(context); break;
                case "inserthknl": result = inserthknl(context); break;
                case "saveMessageToAttachment": result = saveMessageToAttachment(context); break;
                case "readMessageFromAttachment": result = readMessageFromAttachment(context); break;
                //获取经销商额度
                case "getjxsed": result = getjxsed(context); break;
                //判断申请单是否已经做抵押
                case "Is_Mortgaged": result = Is_Mortgaged(context); break;
                //获取经销商额度明细
                case "getjxsedmx": result = getjxsedmx(context); break;
            }
            return result;
        }
        
        private string getinstenceid(HttpContextBase context)
        {
            string rtn = "";
            string applicationno = context.Request["applicationno"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.getinstenceid(applicationno);
            object obj = new
            {
                Result = rtn
            };
            return JSSerializer.Serialize(obj);
        }

        [Xss]
        public string getinstenceid()
        {
            return getinstenceid(HttpContext);
        }

        private string getCinstenceid(HttpContextBase context)
        {
            string rtn = "";
            string applicationno = context.Request["applicationno"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.getCinstenceid(applicationno);
            object obj = new
            {
                Result = rtn
            };
            return JSSerializer.Serialize(obj);
        }

        [Xss]
        public string getCinstenceid()
        {
            return getCinstenceid(HttpContext);
        }

        private string getCAPckje(HttpContextBase context)
        {
            string rtn = "";
            string applicantcardid = context.Request["applicantcardid"];//身份证号
            string applicationnumber = context.Request["applicationnumber"];//单号
            string cap = context.Request["cap"];
            DataTable dt = new WorkFlowFunction().getCAPckje(applicantcardid, applicationnumber, cap);
            rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }

        [Xss]
        public string getCAPckje()
        {
            return getCAPckje(HttpContext);
        }

        private string getGJRck(HttpContextBase context)
        {
            string rtn = "";
            string rtn1 = "";
            string rtn2 = "";
            string applicantcardid = context.Request["applicantcardid"];//身份证号
            string applicationnumber = context.Request["applicationnumber"];//单号
            string cap = context.Request["cap"];
            DataTable dt1 = new WorkFlowFunction().getGJRCAPck(applicantcardid, applicationnumber, cap);
            DataTable dt2 = new WorkFlowFunction().getGJRCMSck(applicantcardid, applicationnumber, cap);
            rtn1 = WorkFlowFunction.DataToJSON(dt1);
            rtn2 = WorkFlowFunction.DataToJSON(dt2);
            rtn = "getGJRck:" + rtn1.ToString() + "getGJRck:" + rtn2.ToString();
            return rtn;
        }

        [Xss]
        public string getGJRck()
        {
            return getGJRck(HttpContext);
        }

        private string getCMSckje(HttpContextBase context)
        {
            string rtn = "";
            string applicantcardid = context.Request["applicantcardid"];//身份证号
            string applicationnumber = context.Request["applicationnumber"];//单号
            string cap = context.Request["cap"];
            DataTable dt = new WorkFlowFunction().getCMSckje(applicantcardid, applicationnumber, cap);
            rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }

        [Xss]
        public string getCMSckje()
        {
            return getCMSckje(HttpContext);
        }

        private string indvidualHistoryFinancedAmount(HttpContextBase context)
        {
            string rtn = "";
            string IdCardNo = context.Request["IdCardNo"];
            string applicationno = context.Request["applicationno"];
            string currentV = context.Request["currentV"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.indvidualHistoryFinancedAmount(IdCardNo, applicationno, currentV);
            object obj = new
            {
                Result = rtn
            };
            return JSSerializer.Serialize(obj);
        }

        [Xss]
        public string indvidualHistoryFinancedAmount()
        {
            return indvidualHistoryFinancedAmount(HttpContext);
        }

        private string getIndvidualApplicationHistory(HttpContextBase context)
        {
            string rtn = "";
            string IdCardNo = context.Request["IdCardNo"];
            string applicationno = context.Request["applicationno"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.getIndvidualApplicationHistory(IdCardNo, applicationno);
            return rtn;
        }

        [Xss]
        public string getIndvidualApplicationHistory()
        {
            return getIndvidualApplicationHistory(HttpContext);
        }

        private string getInstancedata(HttpContextBase context)
        {
            string rtn = "";
            string instanceid = context.Request["instanceid"];

            WorkFlowFunction wf = new WorkFlowFunction();
            string SchemaCode = wf.getSchemaCodeByInstanceID(instanceid);
            DataTable dt = wf.getInstanceData(SchemaCode, instanceid);
            rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }

        [Xss]
        public string getInstancedata()
        {
            return getInstancedata(HttpContext);
        }

        //private string getRejectWorkItem(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string objectids = context.Request["objectids"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.getRejectWorkItem(objectids);
        //    return rtn;
        //}

        //[Xss]
        //public string getRejectWorkItem()
        //{
        //    return getRejectWorkItem(HttpContext);
        //}

        private string insertLYInfo(HttpContextBase context)
        {
            int rtn = 0;
            string instanceid = context.Request["instanceid"];
            string userid = context.Request["userid"];
            string ly = context.Request["ly"];
            ly = System.Web.HttpUtility.HtmlEncode(ly);// 19.7.3 wangxg
            rtn = WorkFlowFunction.insertLYInfo(instanceid, userid, ly);
            object obj = new
            {
                Result = rtn
            };
            return JSSerializer.Serialize(obj);
        }

        [SqlInject]
        public string insertLYInfo()
        {
            return insertLYInfo(HttpContext);
        }

        private string getLYInfo(HttpContextBase context)
        {
            string rtn = "";
            string instanceid = context.Request["instanceid"];
            DataTable dt = WorkFlowFunction.getLYInfo(instanceid);
            //rtn = WorkFlowFunction.DataToJSON(dt);
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return rtn;

        }

        [Xss]
        public string getLYInfo()
        {
            return getLYInfo(HttpContext);

        }

        private string BizAssign(HttpContextBase context)
        {
            string rtn = "";
            string workitemids = context.Request["workitemids"];
            rtn = new WorkFlowFunction().BizAssign(workitemids);
            return rtn;
        }

        [Xss]
        public string BizAssign()
        {
            return BizAssign(HttpContext);
        }

        //private string getssx(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string pcode = context.Request["pcode"];
        //    string UserCode = context.Request["UserCode"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.getssx(UserCode, pcode);
        //    return rtn;
        //}

        //[Xss]
        //public string getssx()
        //{
        //    return getssx(HttpContext);
        //}

        //private string getdycs(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string UserCode = context.Request["UserCode"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.getdycs(UserCode);
        //    return rtn;
        //}

        //[Xss]
        //public string getdycs()
        //{
        //    return getdycs(HttpContext);
        //}

        //private string searchdycs(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string UserCode = context.Request["UserCode"];
        //    string ids = context.Request["ids"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.searchdycs(ids, UserCode);
        //    return rtn;
        //}

        //[Xss]
        //public string searchdycs()
        //{
        //    return searchdycs(HttpContext);
        //}

        //private string updatedycs(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string UserCode = context.Request["UserCode"];
        //    string shen = context.Request["shen"];
        //    string shi = context.Request["shi"];
        //    string xian = context.Request["xian"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.updatedycs(UserCode, shen, shi, xian);
        //    return rtn;
        //}

        //[Xss]
        //public string updatedycs()
        //{
        //    return updatedycs(HttpContext);
        //}

        //private string downloaddycs(HttpContextBase context)
        //{
        //    string rtn = "";
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.downloaddycs();
        //    return rtn;
        //}

        //[Xss]
        //public string downloaddycs()
        //{
        //    return downloaddycs(HttpContext);
        //}

        //private string deldycs(HttpContextBase context)
        //{
        //    string rtn = "";
        //    string UserCode = context.Request["UserCode"];
        //    string ids = context.Request["ids"];
        //    WorkFlowFunction wf = new WorkFlowFunction();
        //    rtn = wf.deldycs(ids, UserCode);
        //    return rtn;
        //}

        //[Xss]
        //public string deldycs()
        //{
        //    return deldycs(HttpContext);
        //}

        private string ldaplogin(HttpContextBase context)
        {
            string UserCode = context.Request["UserCode"];
            string ls_userid = "";
            string ls_username = "";
            string ls_password = "";
            string cn = "";
            try
            {
                string ldap_url = System.Configuration.ConfigurationManager.AppSettings["ldap_url"] + string.Empty;
                string sql = "select sn,dn,cn,userpassword from ldapuser where email='" + UserCode + "'";
                DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ls_userid = dt.Rows[0]["dn"].ToString();
                    ls_username = dt.Rows[0]["sn"].ToString();
                    ls_password = dt.Rows[0]["userpassword"].ToString();
                    cn = dt.Rows[0]["cn"].ToString();
                    jtLDAP a = new jtLDAP();
                    //a.LDAP_URL = "LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com";
                    //a.LDAP_USERID = "cn=" + ls_userid + ",dc=zhengtongauto,dc=com";
                    a.LDAP_URL = ldap_url;
                    a.LDAP_USERID = ls_userid;
                    a.LDAP_PASSWORD = ls_password;
                    a.GetEntry();
                    string r = a.f_getPersonMes(cn, "description");
                    //记录日志信息
                    LogManager.logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\ldap-log\";
                    LogManager.WriteLog(LogFile.Trace, "账号验证成功：" + ls_username + "，返回信息：" + r);
                }
            }
            catch (Exception ex)
            {
                //记录日志信息
                LogManager.logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\ldap-log\";
                LogManager.WriteLog(LogFile.Trace, "账号验证失败：" + ls_username + "，返回信息：" + ex.Message);
            }
            object obj = new
            {
                Result = "true"
            };
            return JSSerializer.Serialize(obj);
        }

        [Xss]
        public string ldaplogin()
        {
            return ldaplogin(HttpContext);
        }

        private string gethknl(HttpContextBase context)
        {
            string rtn = "";
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.gethknl();
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return rtn;
        }

        [Xss]
        public string gethknl()
        {
            return gethknl(HttpContext);
        }

        private string inserthknl(HttpContextBase context)
        {
            int rtn = 0;
            WorkFlowFunction wf = new WorkFlowFunction();
            string APPNO = context.Request["APPNO"];
            string ZJR = context.Request["ZJR"];
            string GJR = context.Request["GJR"];
            string DKJE = context.Request["DKJE"];
            string QS = context.Request["QS"];
            string ZJR_YSRGZ = context.Request["ZJR_YSRGZ"];
            string ZJR_YYHZW = context.Request["ZJR_YYHZW"];
            string ZJR_KHZCGZ = context.Request["ZJR_KHZCGZ"];
            string ZJR_KHYHKNL = context.Request["ZJR_KHYHKNL"];
            string GJR_YSRGZ = context.Request["GJR_YSRGZ"];
            string GJR_YYHZW = context.Request["GJR_YYHZW"];
            string GJR_KHZCGZ = context.Request["GJR_KHZCGZ"];
            string GJR_KHYHKNL = context.Request["GJR_KHYHKNL"];
            rtn = wf.inserthknl(APPNO, ZJR, GJR, DKJE, QS, ZJR_YSRGZ, ZJR_YYHZW, ZJR_KHZCGZ, ZJR_KHYHKNL, GJR_YSRGZ, GJR_YYHZW, GJR_KHZCGZ, GJR_KHYHKNL);
            return Newtonsoft.Json.JsonConvert.SerializeObject(rtn);
        }

        [Xss]
        public string inserthknl()
        {
            return inserthknl(HttpContext);
        }

        private string saveMessageToAttachment(HttpContextBase context)
        {
            string rtn = "";
            string BizObjectSchemaCode = context.Request["BizObjectSchemaCode"];
            string BizObjectId = context.Request["BizObjectId"];
            string DataField = context.Request["DataField"];
            string MessageInfo = context.Request["MessageInfo"];
            WorkFlowFunctionNew wf = new WorkFlowFunctionNew();
            rtn = wf.saveMessageToAttachment(BizObjectSchemaCode, BizObjectId, DataField, MessageInfo);
            return rtn;
        }

        [Xss]
        public string saveMessageToAttachment()
        {
            return saveMessageToAttachment(HttpContext);
        }

        private string readMessageFromAttachment(HttpContextBase context)
        {
            string rtn = "";
            string BizObjectSchemaCode = context.Request["BizObjectSchemaCode"];
            string BizObjectId = context.Request["BizObjectId"];
            string DataField = context.Request["DataField"];
            WorkFlowFunctionNew wf = new WorkFlowFunctionNew();
            rtn = wf.readMessageFromAttachment(BizObjectSchemaCode, BizObjectId, DataField);
            return rtn;
        }

        [Xss]
        public string readMessageFromAttachment()
        {
            return readMessageFromAttachment(HttpContext);
        }

        private string getjxsed(HttpContextBase context)
        {
            string rtn = "";
            string nww = context.Request["nww"];
            string jxs = context.Request["jxs"];
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.getJxsEdglList(nww, jxs);
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return rtn;
        }

        [Xss]
        public string getjxsed()
        {
            return getjxsed(HttpContext);
        }

        private string getjxsedmx(HttpContextBase context)
        {
            string rtn = "";
            string jxs = context.Request["jxs"];
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.getJxsedmx(jxs);
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return rtn;
        }

        [Xss]
        public string getjxsedmx()
        {
            return getjxsedmx(HttpContext);
        }

        private string Is_Mortgaged(HttpContextBase context)
        {
            string application_number = context.Request["applicationNo"];
            WorkFlowFunction wf = new WorkFlowFunction();
            int i = wf.Is_Mortgaged(application_number);
            object obj = new
            {
                value = i
            };
            return JSSerializer.Serialize(obj);
        }

        [Xss]
        public string Is_Mortgaged()
        {
            return Is_Mortgaged(HttpContext);
        }
        #region 序列化器

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private JavaScriptSerializer _JsSerializer = null;
        /// <summary>
        /// 获取JS序列化对象
        /// </summary>
        private JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new JavaScriptSerializer();
                }
                return _JsSerializer;
            }
        }
        

        #endregion


    }

}