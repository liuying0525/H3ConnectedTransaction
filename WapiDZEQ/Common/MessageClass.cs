using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OThinker.H3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Reflection;
using Newtonsoft.Json;
using OThinker.H3.Controllers;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;


namespace OThinker.H3.Portal
{
    public class MessageClass : OThinker.H3.Controllers.MvcPage
    {
        public MessageClass()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="USERID">接收人userid</param>
        /// <param name="USERCODE">接收人code</param>
        /// <param name="MESSAGE">消息内容</param>
        /// <param name="IsWechat">是否需要微信推送</param>
        /// <param name="MsgType">消息类型 
        /// 0 普通消息无弹窗
        /// 1 贷款审批-贷款申请状态转变（已放款，已拒绝）
        /// 2 还款清算-清算异常状态提醒
        /// 3 临时车架号提示修改消息 
        /// 4 临时车架号已修改为永久车架号提示</param>
        /// <param name="Key">弹出框查询条件</param>
        /// <returns></returns>
        public int InsertMSG(string USERID, string USERCODE, string MESSAGE, bool IsWechat, int MsgType, string Key)
        {
            int count = insertMessageList(USERID, USERCODE, MESSAGE, MsgType, Key);
            if (IsWechat)
            {
                MESSAGE = MESSAGE.Replace("<font color=\"red\">", "");
                MESSAGE = MESSAGE.Replace("</font>", "");
                SendWechatMsg_Text(USERCODE, MESSAGE);
            }

            return count;
        }




        public int insertMessageList(string USERID, string USERCODE, string MESSAGE, int MsgType, string Key)
        {
            string guid = Guid.NewGuid().ToString();
            string sql = string.Format(@" insert into i_messagelist(objectid,USERID,USERCODE,MESSAGE,RECEIVETIME,MESSAGESTATE,MessageType,QueryKey) values('{0}', '{1}','{2}','{3}',to_date('{4}','yyyy-mm-dd HH24:mi:ss'),'0','{5}','{6}' )", guid, USERID, USERCODE, MESSAGE, DateTime.Now, MsgType, Key);
            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);

            return count;
        }


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

        public class text
        {
            public string content { get; set; }
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

    }
}