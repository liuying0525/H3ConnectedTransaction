<%@ WebService Language="C#" Class="H3_To_CRM_EnterNetReject" %>

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Services;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;

/// <summary>
/// H3_To_CRM_EnterNetReject 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class H3_To_CRM_EnterNetReject : System.Web.Services.WebService
{
    public H3_To_CRM_EnterNetReject()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    public string DealerAccede = "INSERT INTO DEALERACCEDE(\"ObjectID\",\"CrmDealerId\",\"DealerName\",\"DealerAccount\",\"DealerId\")VALUES([VALUES])";
    [WebMethod]
    public void EnterNetReject(string instanceid, string objectid, string dealerid, string dealername, string auditoremail, string auditor, string infocode, string originatetime, string currenttime, string auditmode1, string auditidea1, string auditmode2, string auditidea2, string auditmode3, string auditidea3, string auditmode4, string auditidea4, string relationaccount)
    {
        if (!string.IsNullOrEmpty(objectid))
        {
            instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'")+string.Empty;
            InstanceContext instance = Engine.InstanceManager.GetInstanceContext(instanceid);
            if (instance != null && instance.Tokens != null && instance.Tokens.Length > 1)
            {
                ApprovalResult result = new ApprovalResult();
                JavaScriptSerializer json = new JavaScriptSerializer();
                if (auditidea4 == "3")
                {
                    auditidea4 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT CASE WHEN (RESULT0='不同意' OR RESULT1='不同意' OR RESULT2='不同意' OR RESULT3='不同意' OR RESULT4='不同意' OR RESULT5='不同意') THEN '1' ELSE '3' END FROM I_ALLOWIN WHERE OBJECTID='" + instance.BizObjectId + "'") + string.Empty;
                    if (auditidea4 == "3")
                    {
                        string fcode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT FINANCIALCODE FROM I_ALLOWIN WHERE OBJECTID='" + objectid + "'") + string.Empty;
                        string sql0 = DealerAccede.Replace("[VALUES]","'" + Guid.NewGuid() + "','" + dealerid + "','" + dealername + "','" + relationaccount + "','" + fcode + "'");
                        string sql1 = "UPDATE I_ALLOWIN SET ZHTIME='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE OBJECTID='" + objectid + "'";
                        string sql2 = "UPDATE DEALERDATA SET \"DealerId\" = " + fcode + " WHERE \"CrmDealerId\" = '" + dealerid + "'";//2019-04-15
                        //2019-04-18 string sql3 = "UPDATE DEALERACCEDE SET \"DealerId\" = " + fcode + " WHERE \"CrmDealerId\" = '" + dealerid + "'";//2019-04-15
                        try
                        {
                            int i0 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql0);
                            int i1 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql1);
                            int i2 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql2);
                            //2019-04-18int i3 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql3);
                        }
                        catch(Exception e) { }
                    }
                }
                string url = System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"] + "http/dongzheng/h3/rejectauditremark";
                result.third_system_no = objectid;
                result.link_no = dealerid;
                result.link_name = dealername;
                result.extend3 = "";
                result.audit_date = "";
                result.extend4 = "";
                result.extend46 = "";
                result.extend1 = "";
                result.extend5 = "";
                result.extend6 = "";
                result.account = "";
                result.final_status = auditidea4;
                string data = json.Serialize(result);
                AppUtility.Engine.LogWriter.Write("H3_To_CRM入网流程驳回JSON：" + data);
                data = DESEncrypt(data, "B^@s(d)+");
                data = "{\"data\":\"" + data + "\"}";
                AppUtility.Engine.LogWriter.Write("H3_To_CRM入网流程驳回数据：" + data);
                string msg = PostUrl(url, data);
            }
        }
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
}