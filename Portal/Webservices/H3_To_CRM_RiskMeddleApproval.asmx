<%@ WebService Language="C#" Class="H3_To_CRM_RiskMeddleApproval" %>

using System.Collections.Generic;
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
/// H3_To_CRM_RiskMeddleApproval 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class H3_To_CRM_RiskMeddleApproval : System.Web.Services.WebService
{

    public H3_To_CRM_RiskMeddleApproval()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    [WebMethod]
    public void RiskMeddleApprovalToCRM(string instanceid, string objectid, string dealerid, string dealername, string auditoremail, string auditor, string infocode, string originatetime, string currenttime, string state)
    {
        ApprovalResult result = new ApprovalResult();
        JavaScriptSerializer json = new JavaScriptSerializer();
        string url = System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"] + "http/dongzheng/h3/auditremark";
        //string url = "http://172.16.7.39:8888/crm_new/http/dongzheng/h3/auditremark";
        if (string.IsNullOrEmpty(instanceid))
            instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'") + string.Empty;

        DataTable RiskData = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM I_RISKPOINT WHERE OBJECTID='" + objectid + "'");
        string sql = "SELECT DISTINCT rpd.Dealer,rpd.RiskDecision,rpd.TargetLimit,rpf.DealerID FROM I_RISKPOINTDECISION rpd " +
                     " LEFT JOIN I_RISKPOINTFORM rpf ON rpd.Dealer = rpf.dealer " +
                     " WHERE rpd.PARENTOBJECTID='" + objectid + "'";
        DataTable RiskDec = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        string opinion = "";

        if (RiskData != null && RiskData.Rows.Count > 0)
        {
            result.link_no = RiskData.Rows[0]["DealerID"].ToString();
            result.third_system_no = RiskData.Rows[0]["DealerID"].ToString();
            result.originator = RiskData.Rows[0]["originator"].ToString();
            opinion = RiskData.Rows[0]["Opinion"].ToString();
        }

        if (RiskDec != null && RiskDec.Rows.Count > 0)
        {
            List<riskPointDecision> rpdList = new List<riskPointDecision>();
            for (int i = 0; i < RiskDec.Rows.Count; i++)
            {
                riskPointDecision rpd = new riskPointDecision();
                rpd.third_system_no = RiskDec.Rows[i]["DealerID"].ToString();
                rpd.DealerName = RiskDec.Rows[i]["Dealer"].ToString();
                rpd.RiskDecision = RiskDec.Rows[i]["RiskDecision"].ToString();
                rpd.finalQuota = RiskDec.Rows[i]["TargetLimit"].ToString();
                rpdList.Add(rpd);
            }
            result.list = rpdList;
        }

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

        result.audit_remark = "信审意见:【" + opinion + "】";
        result.datetime = currenttime;
        result.audit_date = currenttime ?? System.DateTime.Now.ToString();
        result.final_status = state;
        string data = json.Serialize(result);
        AppUtility.Engine.LogWriter.Write("H3_To_CRM风险点干预审核JSON：" + data);
        data = DESEncrypt(data, "B^@s(d)+");
        data = "{\"data\":\"" + data + "\"}";
        AppUtility.Engine.LogWriter.Write("H3_To_CRM风险点干预审核数据：" + data);
        string msg = PostUrl(url, data);
    }

    public class ApprovalResult
    {
        public string from_type = "5";
        public string link_no = "0";
        public string third_system_no;
        public string audit_date;
        public string audit_remark;
        public string originator;
        public string datetime;
        public string extend46;
        public string final_status;
        public List<riskPointDecision> list;
    }

    public class riskPointDecision
    {
        public string third_system_no;
        public string DealerName;
        public string RiskDecision;
        public string finalQuota;
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
