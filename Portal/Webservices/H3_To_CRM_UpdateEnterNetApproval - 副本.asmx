<%@ WebService Language="C#" Class="H3_To_CRM_UpdateEnterNetApproval" %>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;

/// <summary>
/// H3_To_CRM_UpdateEnterNetApproval 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class H3_To_CRM_UpdateEnterNetApproval : System.Web.Services.WebService
{
    public H3_To_CRM_UpdateEnterNetApproval()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
    public string UpdateDealerData = "INSERT INTO DEALERDATA(\"ObjectID\",\"CrmDealerId\",\"Name\",\"Type\",\"Value\",\"DealerType\",\"DealerKind\")VALUES([VALUES])";
    public string UpdateDealerDataArray = "INSERT INTO DEALERDATAARRAY(\"ObjectID\",\"DealerDataObjectID\",\"Value\")VALUES([VALUES])";
    [WebMethod]
    public void EnterNetUpdate(string instanceid, string objectid, string dealerid, string dealername, string auditoremail, string auditor, string infocode, string originatetime, string currenttime, string auditidea1, string auditidea2, string auditidea3, string auditidea4, string state)
    {
        ApprovalResult result = new ApprovalResult();
        JavaScriptSerializer json = new JavaScriptSerializer();
        if (state == "3")
        {
            string allowin = "";
            string sql = "SELECT * FROM I_UPDATEENTERNET WHERE CRMDEALERID='" + dealerid + "'";
            DataTable update = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var updateList = update.Rows[0]["CHANGEFIELD"].ToString().Split(',');
            foreach (var field in updateList)
            {
                allowin += (allowin == "" ? field.Split(':')[0] : "," + field.Split(':')[0]) + "='" + update.Rows[0][field.Split(':')[0]] + "'";
                sql = "UPDATE DEALERDATA SET \"Value\" = '" + update.Rows[0][field.Split(':')[0]] + "' WHERE \"CrmDealerId\" = '" + dealerid + "' AND \"Name\" = '" + field.Split(':')[1] + "'";
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            }
            sql = "UPDATE I_ALLOWIN SET " + allowin + " WHERE CRMDEALERID='" + dealerid + "'";
            int g = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }
        string url = System.Configuration.ConfigurationManager.AppSettings["H3_To_CRM_URL"] + "http/dongzheng/h3/auditremark";
        if (string.IsNullOrEmpty(instanceid))
        {
            instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'")+string.Empty;
        }
        if (string.IsNullOrEmpty(originatetime))
        {
            InstanceContext instance = Engine.InstanceManager.GetInstanceContext(instanceid);
            var t = instance.Tokens.LastOrDefault(x => true);
            result.extend46 = t == null ? "" : t.CreatedTime.ToString();
        }
        else result.extend46 = originatetime;
        result.third_system_no = objectid;
        result.link_no = dealerid;
        result.link_name = dealername;
        result.extend3 = infocode;
        result.audit_remark = auditidea1;
        result.extend2 = auditidea2;
        result.extend5 = auditidea3;
        //result.audit_remark = auditidea4;
        result.extend4 = auditor;
        result.extend1 = dealername;
        result.extend5 = "";
        result.extend6 = auditoremail;
        result.final_status = state;
        result.audit_date = currenttime ?? DateTime.Now.ToString();;
        string data = json.Serialize(result);
        AppUtility.Engine.LogWriter.Write("H3_To_CRM修改入网流程审核JSON：" + data);
        data = DESEncrypt(data, "B^@s(d)+");
        data = "{\"data\":\"" + data + "\"}";
        AppUtility.Engine.LogWriter.Write("H3_To_CRM修改入网流程审核数据：" + data);
        string msg = PostUrl(url, data);
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
        public string from_type = "4";
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