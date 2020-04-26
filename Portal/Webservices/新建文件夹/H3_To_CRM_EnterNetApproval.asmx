<%@ WebService Language="C#" Class="H3_To_CRM_EnterNetApproval" %>
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Services;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;
using Stream = System.IO.Stream;

/// <summary>
/// H3_To_CRM_EnterNetApproval 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class H3_To_CRM_EnterNetApproval : System.Web.Services.WebService
{
    public H3_To_CRM_EnterNetApproval()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    [WebMethod]
    public void EnterNetToCrm(string instanceid,string objectid,string dealerid,string dealername, string auditoremail,string auditor, string infocode, string originatetime, string currenttime,string auditmode1,string auditidea1,string auditmode2,string auditidea2,string auditmode3,string auditidea3,string auditmode4,string auditidea4,string auditmode5,string auditidea5,string auditmode6,string auditidea6, string relationaccount, string state, string lendingmode, string verifyloan)
    {
        ApprovalResult result = new ApprovalResult();
        JavaScriptSerializer json = new JavaScriptSerializer();
        string url = System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"] + "http/dongzheng/h3/auditremark";
        instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'")+string.Empty;
        result.extend46 = originatetime;
        InstanceContext instance = Engine.InstanceManager.GetInstanceContext(instanceid);
        var t = instance == null || instance.Tokens == null ? null : instance.Tokens.LastOrDefault(x => true);
        result.extend46 = t == null ? "" : t.CreatedTime.ToString();
        result.third_system_no = objectid;
        result.link_no = dealerid;
        result.link_name = dealername;
        result.extend3 = infocode;
        switch (infocode)
        {
            case "销售初审":
                result.audit_remark = auditidea1;
                result.extend2 = auditmode1;
                break;
            case "销售终审":
                result.audit_remark = auditidea2;
                result.extend2 = auditmode2;
                break;
            case "信审初审":
                result.audit_remark = auditidea3;
                result.extend2 = auditmode3;
                break;
            case "信审复审":
                result.audit_remark = auditidea5;
                result.extend2 = auditmode5;
                break;
            case "信审终审":
                result.audit_remark = auditidea4;
                result.extend2 = auditmode4;
                break;
            case "总经理特批":
                result.audit_remark = auditidea6;
                result.extend2 = auditmode6;
                break;
            case "下载合作协议":
                result.audit_remark = "";
                result.extend2 = "";
                break;
            case "协议审核":
                result.audit_remark = "";
                result.extend2 = "";
                break;
            case "账户配置":
                result.audit_remark = "";
                result.extend2 = "";
                break;
            case "产品关联":
                result.audit_remark = "";
                result.extend2 = "";
                break;
        }
        result.lendingmode = lendingmode;
        result.verifyloan = verifyloan;
        result.extend4 = auditor;
        result.extend1 = dealername;
        result.extend5 = "";
        result.extend6 = auditoremail;
        result.account = relationaccount;
        result.final_status = "";
        result.audit_date = currenttime ?? DateTime.Now.ToString();;
        string data = json.Serialize(result);
        AppUtility.Engine.LogWriter.Write("H3_To_CRM入网流程审核JSON：" + data);
        data = DESEncrypt(data, "B^@s(d)+");
        data = "{\"data\":\"" + data + "\"}";
        AppUtility.Engine.LogWriter.Write("H3_To_CRM入网流程审核数据：" + data);
        string msg = PostUrl(url, data);
    }
    private IEngine _Engine = null;
    public IEngine Engine
    {
        get
        {
            if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
            {
                return OThinker.H3.Controllers.AppUtility.Engine;
            }
            return _Engine;
        }
        set { _Engine = value; }
    }
    public class ApprovalResult
    {
        public string third_system_no;
        public string link_no;
        public string link_name;
        public string deal_kind = "131";
        public string extend3;
        public string audit_remark;
        public string extend2;
        public string audit_date;
        public string extend4;
        public string extend46;
        public string extend1;
        public string from_type = "6";
        public string extend5;
        public string extend6;
        public string account;
        public string final_status;
        public string lendingmode;
        public string verifyloan;
    }
    public static string PostUrl(string url, string postData)
    {
        string result = "";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.Method = "POST";
        req.ContentType = "application/json";
        byte[] data = Encoding.UTF8.GetBytes(postData);
        req.ContentLength = data.Length;
        using (Stream reqStream = req.GetRequestStream())
        {
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();
        }
        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        Stream stream = resp.GetResponseStream();
        //获取响应内容
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            result = reader.ReadToEnd();
        }
        return result;
    }
/// <summary>
/// 加密
/// </summary>
/// <param name="pToDecrypt">加密字符串</param>
/// <param name="sKey">密钥</param>
/// <returns></returns>
    public static string DESEncrypt(string pToEncrypt, string sKey = "test")
    {
        // string pToEncrypt1 = HttpContext.Current.Server.UrlEncode(pToEncrypt);     
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        pToEncrypt = System.Web.HttpUtility.UrlEncode(pToEncrypt);
        byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
        //建立加密对象的密钥和偏移量      
        //原文使用ASCIIEncoding.ASCII方法的GetBytes方法      
        //使得输入密码必须输入英文文本      
        des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        StringBuilder ret = new StringBuilder();
        foreach (byte b in ms.ToArray())
        {
            ret.AppendFormat("{0:X2}", b);
        }
        ret.ToString();
        return ret.ToString();
    }
}