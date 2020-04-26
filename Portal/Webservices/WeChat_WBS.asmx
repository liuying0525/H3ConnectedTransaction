<%@ WebService Language="C#" Class="WeChat_WBS" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using OThinker.H3.WeChat;
using OThinker.H3.Controllers;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OThinker.Organization;
using OThinker.H3;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class WeChat_WBS : System.Web.Services.WebService
{
    public string corpID = AppUtility.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatCorpID);
    public string secretID = AppUtility.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatMessageSecret);

    public string AgentId = AppUtility.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatAgentId);

    public static string MsgUrl = AppUtility.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_PortalUrl);
    public bool wechatPush = Convert.ToBoolean(AppUtility.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatPush));

    public string getTokenURL = "https://qyapi.weixin.qq.com/cgi-bin/gettoken";
    public string sendMsgURL = "https://qyapi.weixin.qq.com/cgi-bin/message/send";

    public string h3URL = "http://192.168.41.1:8010/Portal/";

    Encoding WeChatEncoding = Encoding.GetEncoding("UTF-8");

    string ssoURL = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&agentid={2}&state=Mobile#wechat_redirect";


    [WebMethod]
    public string GetToken(string corpid, string corpsecret)
    {
        string respText = string.Empty;
        string Url = getTokenURL + "?corpid=" + corpid + "&corpsecret=" + corpsecret;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.KeepAlive = false;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        try
        {
            using (Stream resStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                respText = reader.ReadToEnd();
                resStream.Close();
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            response.Close();
        }
        JObject obj = (JObject)JsonConvert.DeserializeObject(respText);
        string tokenid = obj["access_token"].ToString();
        return tokenid;

    }

    [WebMethod]
    public string SendWechatMsgByH3OrgCode(string h3Code, string workItemid, string instanceId, string title, string body)
    {
        if (string.IsNullOrEmpty(h3Code))
            return "H3 Unit Code 为空！";
        Unit u = AppUtility.Engine.Organization.GetUserByCode(h3Code);
        if (u == null)
            return "H3 Unit Code 错误或不存在！";
        return SendWechatMsgByH3OrgID(u.ObjectID, workItemid, instanceId, title, body);
    }

    [WebMethod]
    public string SendWechatMsgByH3OrgID(string h3Id, string workItemid, string instanceId, string title, string body)
    {
        if (string.IsNullOrEmpty(h3Id))
            return "H3 Unit ID 为空！";

        string ids = "";
        string url = "";

        Unit unit = AppUtility.Engine.Organization.GetUnit(h3Id);
        if (unit.UnitType == UnitType.User)
            ids = ((User)unit).Code;
        else
        {
            List<Unit> users = AppUtility.Engine.Organization.GetChildUnits(h3Id, UnitType.User, true, State.Active);
            foreach (User user in users)
            {
                ids += user.Code + "|";
            }
            if (ids != "")
                ids = ids.Substring(0, ids.Length - 1);

        }
        if (!string.IsNullOrEmpty(workItemid))
            url = h3URL + "WorkItemDetail.aspx?WorkItemID=" + workItemid;
        else if (!string.IsNullOrEmpty(instanceId))
            url = h3URL + "InstanceSheets.aspx?InstanceId=" + instanceId;

        AppUtility.Engine.LogWriter.Write("URL：" + url);
        return SendWechatMsg(ids, url, title, body);
    }

    //[WebMethod]
    //public string SendWechatMsgByH3OrgIDs(string[] h3Ids, string workItemid, string instanceId, string title, string body)
    //{
    //    if (h3Ids == null || h3Ids.Length == 0)
    //        return "H3 Unit ID 为空！";

    //    string ids = "";
    //    string url = "";

    //    List<Unit> users = AppUtility.Engine.Organization.chi(h3Ids, UnitType.User, true, State.Active);
    //    foreach (User user in users)
    //    {
    //        ids += user.Code + "|";
    //    }
    //    if (ids != "")
    //        ids = ids.Substring(0, ids.Length - 1);

    //    if (!string.IsNullOrEmpty(workItemid))
    //        url = h3URL + "WorkItemDetail.aspx?WorkItemID=" + workItemid;
    //    else if (!string.IsNullOrEmpty(instanceId))
    //        url = h3URL + "InstanceSheets.aspx?InstanceId=" + instanceId;

    //    AppUtility.Engine.LogWriter.Write("URL：" + url);
    //    return SendWechatMsg(ids, url, title, body);
    //}

    //[WebMethod]
    //public string SendWechatMsg_TextByH3OrgIDs(string[] h3Ids, string content)
    //{
    //    if (h3Ids == null || h3Ids.Length == 0)
    //        return "H3 Unit ID 为空！";

    //    string ids = "";
    //    string url = "";

    //    List<Unit> users = AppUtility.Engine.Organization.GetChildUnitsByIds(h3Ids, UnitType.User, true, State.Active);
    //    foreach (User user in users)
    //    {
    //        ids += user.Code + "|";
    //    }
    //    if (ids != "")
    //        ids = ids.Substring(0, ids.Length - 1);
    //    return SendWechatMsg_Text(ids, content);
    //}
    [WebMethod]
    public string SendWechatMsg_TextByH3OrgCode(string h3Code, string content)
    {
        if (string.IsNullOrEmpty(h3Code))
            return "H3 Unit Code 为空！";
        Unit u = AppUtility.Engine.Organization.GetUserByCode(h3Code);
        if (u == null)
            return "H3 Unit Code 错误或不存在！";
        return SendWechatMsg_TextByH3OrgID(u.ObjectID, content);
    }

    [WebMethod]
    public string SendWechatMsg_TextByH3OrgID(string h3Id, string content)
    {
        if (string.IsNullOrEmpty(h3Id))
            return "H3 Unit ID 为空！";

        string ids = "";
        string url = "";

        Unit unit = AppUtility.Engine.Organization.GetUnit(h3Id);
        if (unit.UnitType == UnitType.User)
            ids = ((User)unit).Code;
        else
        {
            List<Unit> users = AppUtility.Engine.Organization.GetChildUnits(h3Id, UnitType.User, true, State.Active);
            foreach (User user in users)
            {
                ids += user.Code + "|";
            }
            if (ids != "")
                ids = ids.Substring(0, ids.Length - 1);

        }

        AppUtility.Engine.LogWriter.Write("URL：" + url);
        return SendWechatMsg_Text(ids, content);
    }

    [WebMethod]
    public string SendWechatMsg_Text(string userids, string content)
    {
        TextMsg msg = new TextMsg();
        msg.agentid = AgentId;
        msg.msgtype = AgentId;
        msg.msgtype = "text";
        msg.touser = userids;
        text textcontent = new text();
        textcontent.content = content;
        msg.text = textcontent;
        msg.safe = "0";
        string paraJsonStr = JsonConvert.SerializeObject(msg);
        return SendWechatMessage(paraJsonStr);
    }
    [WebMethod]
    public string SendWechatMsg(string userids, string url, string title, string body)
    {
        TextCardMsg msg = new TextCardMsg();
        msg.agentid = AgentId;
        msg.msgtype = "textcard";
        msg.touser = userids;
        textcard cardMsg = new textcard();
        cardMsg.title = title;
        cardMsg.description = body;
        cardMsg.url = string.Format(ssoURL, corpID, url, AgentId);
        cardMsg.btntxt = "详情";
        msg.textcard = cardMsg;

        string paraJsonStr = JsonConvert.SerializeObject(msg);
        return SendWechatMessage(paraJsonStr);
    }

    private string SendWechatMessage(string paraJson)
    {
        string tokenid = GetToken(corpID, secretID);
        string paraJsonStr = paraJson;
        string strUrl = sendMsgURL + "?access_token=" + tokenid;
        AppUtility.Engine.LogWriter.Write("发送微信消息参数：" + paraJsonStr);
        ServicePointManager.DefaultConnectionLimit = 100;
        if (strUrl.ToLower().Contains("https"))
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        }

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.CookieContainer = new CookieContainer();
        request.KeepAlive = false;
        byte[] data = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(paraJsonStr);
        request.ContentLength = data.Length;
        request.GetRequestStream().Write(data, 0, data.Length);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        string respText = "";
        try
        {
            using (System.IO.Stream resStream = response.GetResponseStream())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(resStream, System.Text.Encoding.UTF8);
                respText = reader.ReadToEnd();
                resStream.Close();
            }
        }
        catch (Exception ex)
        {
            AppUtility.Engine.LogWriter.Write("发送微信消息异常：" + ex.ToString());
        }
        finally
        {
            response.Close();
        }
        string result = "Send Wechat Msg Result:" + respText;
        AppUtility.Engine.LogWriter.Write(result);
        return result;
    }


    #region class
    public class TextCardMsg
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public string agentid { get; set; }
        public textcard textcard { get; set; }

    }

    public class textcard
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string btntxt { get; set; }
    }

    public class TextMsg
    {
        public string touser { get; set; }
        public string toparty { get; set; }
        public string totag { get; set; }
        public string msgtype { get; set; }
        public string agentid { get; set; }
        public text text { get; set; }
        public string safe { get; set; }
    }
    public class text
    {
        public string content { get; set; }
    }
    #endregion
}