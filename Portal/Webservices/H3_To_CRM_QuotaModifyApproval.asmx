<%@ WebService Language="C#" Class="H3_To_CRM_QuotaModifyApproval" %>

using System.Data;
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
/// H3_To_CRM_QuotaModifyApproval 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class H3_To_CRM_QuotaModifyApproval : System.Web.Services.WebService
{

    public H3_To_CRM_QuotaModifyApproval()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    [WebMethod]
    public void QuotaModifyToCRM(string instanceid, string objectid, string dealerid, string dealername, string auditoremail, string auditor, string infocode, string originatetime, string currenttime, string auditidea1, string auditidea2, string auditidea3, string auditidea4, string finalBail, string finalQuota, string state)
    {
        ApprovalResult result = new ApprovalResult();
        JavaScriptSerializer json = new JavaScriptSerializer();
        string url = System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"] + "http/dongzheng/h3/auditremark";
        if (string.IsNullOrEmpty(instanceid))
        {
            instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'") + string.Empty;
        }
        //DataTable QuotaTable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM I_Relegation WHERE objectID = '" + objectid + "'");
        //if (QuotaTable != null && QuotaTable.Rows.Count > 0)
        //{
        //    dealerid = QuotaTable.Rows[0]["CrmDealerID"].ToString();
        //    dealername = QuotaTable.Rows[0]["Dealer"].ToString();
        //    auditidea1 = QuotaTable.Rows[0]["SalesApproval"].ToString();
        //    auditidea2 = QuotaTable.Rows[0]["CreditApproval"].ToString();
        //    auditidea3 = QuotaTable.Rows[0]["PneumaticDecision"].ToString();
        //    finalBail = QuotaTable.Rows[0]["FinalMargin"].ToString();
        //    finalQuota = QuotaTable.Rows[0]["FinalLine"].ToString();
        //}
        if (string.IsNullOrEmpty(originatetime))
        {
            InstanceContext instance = Engine.InstanceManager.GetInstanceContext(instanceid);
            var t = instance.Tokens.LastOrDefault(x => true);
            result.extend46 = t == null ? "" : t.CreatedTime.ToString();
        }
        else
        {
            result.extend46 = originatetime;
        }
        result.third_system_no = objectid;
        result.link_no = dealerid;
        result.link_name = dealername;
        result.extend3 = infocode;
        result.audit_remark = "销售审批：【" + auditidea1 + "】，信审审批：【" + auditidea2 + "】，风控决策：【" + auditidea3 + "】";
        result.extend6 = auditoremail;
        result.originator = auditor;
        result.final_status = state;
        result.audit_date = currenttime ?? System.DateTime.Now.ToString();
        result.finalbail = finalBail;
        result.finalquota = finalQuota;
        string data = json.Serialize(result);
        AppUtility.Engine.LogWriter.Write("H3_To_CRM升降机审核JSON：" + data);
        data = DESEncrypt(data, "B^@s(d)+");
        data = "{\"data\":\"" + data + "\"}";
        AppUtility.Engine.LogWriter.Write("H3_To_CRM升降机审核数据：" + data);
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
        public string from_type = "2";
        public string third_system_no;
        public string link_no;
        public string link_name;
        public string extend3;
        public string audit_remark;
        public string extend2;
        public string extend6;
        public string extend46;
        public string audit_date;
        public string originator;
        public string datetime;
        public string final_status;
        public string finalbail;
        public string finalquota;
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
