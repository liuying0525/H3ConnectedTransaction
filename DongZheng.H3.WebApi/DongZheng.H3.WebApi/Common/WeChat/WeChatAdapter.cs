using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
//using System.Web.Script.Serialization;
using OThinker.Organization;
using OThinker.H3.WeChat;
using OThinker.H3.Settings;
using OThinker.Clusterware;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Web;
using OThinker;

using OThinker.H3;
using OThinker.H3.DataModel;
using System.Data;
using System.Configuration;

namespace DongZheng.H3.WebApi
{
    /// <summary>
    /// 微信服务类
    /// </summary>
    public class WeChatAdapter
    {
        public static string NotSyncToWechatUserCodes = ConfigurationManager.AppSettings["NotSyncToWechatUserCodes"] + string.Empty;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Engine"></param>
        public WeChatAdapter(IEngine Engine)
        {
            Engine.LogWriter.Write("Starting WeChat...");
            this.Organization = Engine.Organization;
            this.SettingManager = Engine.SettingManager;
            this.LogWriter = Engine.LogWriter;
            this.BizObjectManager = Engine.BizObjectManager;
        }

        /// <summary>
        /// 系统加载方法
        /// </summary>
        /// <param name="LogWriter"></param>
        /// <param name="SettingManager"></param>
        /// <param name="Organization"></param>
        /// <param name="BizObjectManager"></param>
        public void OnLoad(
            ILogWriter LogWriter,
            ISettingManager SettingManager,
            IOrganization Organization,
            OThinker.H3.DataModel.IBizObjectManager BizObjectManager)
        {
            LogWriter.Write("Starting WeChat...");
            this.Organization = Organization;
            this.SettingManager = SettingManager;
            this.LogWriter = LogWriter;
            this.BizObjectManager = BizObjectManager;
        }


        private IOrganization Organization = null;
        private ILogWriter LogWriter = null;
        private ISettingManager SettingManager = null;
        private IBizObjectManager BizObjectManager;

        private System.Threading.ReaderWriterLock RWL = new System.Threading.ReaderWriterLock();

        #region 微信接口常量 -------------------------
        /// <summary>
        /// 获取AccessToken{0}{1}的URL
        /// </summary>
        public const string URL_AccessToken = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
        /// <summary>
        /// 获取创建组织的URL
        /// </summary>
        public const string URL_CreateOrg = "https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}";
        /// <summary>
        /// 更新组织的URL
        /// </summary>
        public const string URL_UpdateOrg = "https://qyapi.weixin.qq.com/cgi-bin/department/update?access_token={0}";

        /// <summary>
        /// 删除组织的URL
        /// </summary>
        public const string URL_deleteOrg = "https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token={0}";

        /// <summary>
        /// 获取微信用户ID
        /// </summary>
        public const string URL_GetUser = "https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";
        /// <summary>
        /// 创建用户的链接
        /// </summary>
        public const string URL_CreateUser = "https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}";
        /// <summary>
        /// 更新用户的链接
        /// </summary>
        public const string URL_UpdateUser = "https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token={0}";
        /// <summary>
        /// 删除用户的链接
        /// </summary>
        public const string URL_DeleteUser = "https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token={0}&userid={1}";
        /// <summary>
        /// 发送新闻消息URL
        /// </summary>
        public const string URL_NewsMessage = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
        /// <summary>
        /// 微信打开的URL
        /// </summary>
        public const string URL_OAuth = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state={2}#wechat_redirect";
        /// <summary>
        /// 获取用户账号
        /// </summary>
        public const string URL_UserCode = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}&agentid={2}";

        /// <summary>
        /// 上传文件
        /// </summary>
        public const string URL_UploadFile = "https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";

        /// <summary>
        /// 下载文件
        /// </summary>
        public const string URL_DownloadFile = "https://qyapi.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";
        /// <summary>
        /// 获取OU
        /// </summary>
        public const string URL_LoadOrg = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id={1}";
        /// <summary>
        /// 获取Token的超时时间，微信是7200秒，这里使用7000秒
        /// </summary>
        public const int TokenTimeOut = 7000;
        #endregion

        #region 属性信息 -----------------------------
        /// <summary>
        /// 获取或设置开发者凭据ID
        /// </summary>
        private string CorpID;
        /// <summary>
        /// 获取或设置权限组 Secret
        /// </summary>
        private string Secret;
        /// <summary>
        /// 获取或设置流程中心推送消息的ID
        /// </summary>
        private int AgentID;
        /// <summary>
        /// 获取Token的获取时间
        /// </summary>
        private DateTime TokenReceiveTime;
        /// <summary>
        /// 微信的AccessToken
        /// </summary>
        private string AccessToken;
        /// <summary>
        /// 获取默认的编码方式
        /// </summary>
        private Encoding WeChatEncoding = Encoding.GetEncoding("UTF-8");

        #endregion

        #region 微信接口 -----------------------------

        public void LoadOrgToken()
        {
            // 重新读取配置
            this.CorpID = SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatCorpID);
            this.Secret = SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatSecret);
            this.AccessToken = LoadToken(CorpID, Secret);
            this.TokenReceiveTime = DateTime.Now;

        }

        private void LoadMessageToken()
        {
            // 重新读取配置
            this.CorpID = SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_WeChatCorpID);
            this.Secret = SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatMessageSecret);
            this.AccessToken = LoadToken(CorpID, Secret);
            this.TokenReceiveTime = DateTime.Now;


        }
        
        /// <summary>
        /// 获取Token
        /// </summary>
        public string LoadToken(string corpId, string secret)
        {
            if (DateTime.Now.Subtract(TokenReceiveTime).TotalSeconds > TokenTimeOut)
            {
                int agentId = 0;
                int.TryParse(SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatAgentId), out agentId);
                this.AgentID = agentId;
                // 重新获取Token
                TokenReceiveTime = DateTime.Now;
                string url = string.Format(URL_AccessToken, corpId, secret);
                try
                {
                    string res = GetWebRequest(url, WeChatEncoding);

                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(res);
                    if (string.IsNullOrEmpty(token.access_token))
                    {
                        // throw new Exception("微信接口调用失败，返回值" + res);
                    }
                    this.AccessToken = token.access_token;
                }
                catch (Exception ex)
                {
                    this.LogWriter.Write(ex.ToString());
                }

            }
            return AccessToken;

        }


        /// <summary>
        /// 重置微信链接状态
        /// </summary>
        [OThinker.Clusterware.LogicUnitMethod(LogicUnitMethodType.Client)]
        public void Reload()
        {
            TokenReceiveTime = DateTime.Now.AddDays(-2);
        }

        #endregion

        #region 组织机构同步 -------------------------



        /// <summary>
        /// 同步组织到微信，这个地方采用Mono目的是为了避免多线程同时同步
        /// </summary>
        /// <returns></returns>
        [OThinker.Clusterware.LogicUnitMethod(OThinker.Clusterware.LogicUnitMethodType.MonoFusion)]
        public SyncResult SyncOrgToWeChat(DateTime LastSyncTime)
        {
            SyncResult sr = new SyncResult();
            try
            {
                LoadOrgToken();
                this.RWL.AcquireWriterLock(-1);
                
                if (string.IsNullOrEmpty(AccessToken))
                {
                    sr.Success = false;
                    sr.ErrMsg = "获取Token错误";
                    return sr;
                }
                string companyId = Organization.Company.ObjectID;

                this.LogWriter.Write("微信组织同步开始...");
                DataTable userRelation = GetWechatUserRelation();
                SyncOrg(LastSyncTime, companyId, 1, userRelation,ref sr);
                //完整同步时执行
                if (LastSyncTime==DateTime.MinValue)
                {
                    this.LogWriter.Write("H3中删除的用户，在企业微信中一起删除");
                    #region H3中删除的用户，在企业微信中一起删除；
                    string sql = "SELECT * FROM C_H3_Wechat_OrgSync WHERE code NOT IN (SELECT code FROM OT_User where state=1)";
                    DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    foreach (DataRow row in dt.Rows)
                    {
                        if (DeleteWeChatUser(row["code"] + string.Empty))
                            sr.delete_User_Success_Num++;
                    }
                    #endregion
                }
                return sr;
            }
            catch (Exception ex)
            {
                this.LogWriter.Write(ex.ToString());
                sr.Success = false;
                sr.ErrMsg = "Exception:" + ex.ToString();
                return sr;
            }
            finally
            {
                this.RWL.ReleaseWriterLock();
            }
        }

        private WeChatOrgResult_New _WeChatOrgList = null;
        private WeChatOrgResult_New WeChatOrgList
        {
            get
            {
                return this._WeChatOrgList;
            }
            set
            {
                this._WeChatOrgList = value;
            }
        }

        private DataTable GetWechatUserRelation()
        {
            try
            {
                string sql = "SELECT * FROM C_H3_Wechat_OrgSync";
                return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            }
            catch
            {
                return null;
            }
        }
        private bool InsertToWechatUserRelation(string code,string name,string mobile)
        {
            try
            {
                string sql = "Insert into C_H3_Wechat_OrgSync(code,name,tel) values('{0}','{1}','{2}')";
                int n = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format(sql, code, name, mobile));
                this.LogWriter.Write("Insert到C_H3_Wechat_OrgSync表：" + code + "," + name + "," + mobile + ",受影响行数：" + n);
                if (n > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        [Obsolete("此方法不能正常调用，微信返回数据有BUG，组织机构过多不能返回完整数据")]
        private WeChatOrgResult_New GetAllWeChatOrg(int ParentID)
        {
            LoadOrgToken();
            string result = GetWebRequest(string.Format(URL_LoadOrg, AccessToken, ParentID), WeChatEncoding);
            WeChatOrgList = JsonConvert.DeserializeObject<WeChatOrgResult_New>(result);
            return WeChatOrgList;
        }

        /// <summary>
        /// 获取微信中的组织
        /// </summary>
        /// <param name="WeChatParent"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public WeChatOrg_New GetWeChatOrg(int WeChatParent, string Name)
        {
            if (this.WeChatOrgList == null || this.WeChatOrgList.department == null)
            {
                return null;
            }

            var unit = WeChatOrgList.department.Where(a => a.parentid == WeChatParent).Where(a => a.name == Name);
            if (unit.Count() > 0)
            {
                return unit.First();
            }
            return null;
        }

        /// <summary>
        /// 根据微信ID获取微信组织
        /// </summary>
        /// <param name="WeChatParent">微信上级ID</param>
        /// <param name="Name">微信组织名称</param>
        /// <returns></returns>
        private WeChatOrg_New GetWeChatOrgByParentId(int WeChatParent, int WeChatID)
        {
            try
            {
                //LoadToken();
                var url = string.Format(URL_LoadOrg, AccessToken, WeChatParent);
                string result = get(url);
                var orgresult = JsonConvert.DeserializeObject<WeChatOrgResult_New>(result);
                var unit = orgresult.department.Where(a => a.parentid == WeChatParent).Where(a => a.id == WeChatID);
                if (unit.Count() > 0)
                {
                    return unit.First();
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 同步微信的组织
        /// </summary>
        /// <param name="SourceUnits"></param>
        /// <param name="ParentID"></param>
        /// <param name="WeChatParent"></param>
        /// <returns></returns>
        private bool SyncOrg(DateTime LastSyncTime, string ParentID, int WeChatParent, DataTable dtUserRelation, ref SyncResult syncResult)
        {
            LoadOrgToken();

            var childList = Organization.GetChildUnits(ParentID, UnitType.OrganizationUnit, false, State.Active);
            var ChildList = childList.OrderBy(a => a.SortKey);

            #region  用户同步
            // 同步用户
            OThinker.Organization.Unit[] users = Organization.GetChildUnits(ParentID, UnitType.User, false, State.Unspecified).OrderBy(a => a.SortKey).ToArray();
            if (users != null)
            {
                foreach (OThinker.Organization.User user in users)
                {
                    if (string.IsNullOrEmpty(user.WeChatAccount)
                        && string.IsNullOrEmpty(user.Email)
                        && string.IsNullOrEmpty(user.Mobile)) continue;
                    //1.增量同步且用户的修改时间少于上次的同步时间，不做增量同步
                    if (LastSyncTime != DateTime.MinValue && user.ModifiedTime < LastSyncTime)
                    {
                        continue;
                    }
                    this.LogWriter.Write("微信用户同步,Code=" + user.Code + ",Name=" + user.Name + ",Mobile=" + user.Mobile + ",WeChatParent=" + WeChatParent);
                    if (NotSyncToWechatUserCodes.Replace("'", "").Split(',').Contains(user.Code))
                    {
                        this.LogWriter.Write("微信用户同步跳过，存在于不同步配置中，Code=" + user.Code + ",Name=" + user.Name + ",Mobile=" + user.Mobile + ",WeChatParent=" + WeChatParent);
                        continue;
                    }
                    var wxuser = GetWeChatUser(user.Code);
                    if (wxuser.errcode != "0")//0表示正常返回
                    {
                        //H3禁用或者离职：不用同步到微信
                        if (user.State == State.Inactive || user.ServiceState == UserServiceState.Dismissed)
                        {
                            
                        }
                        else
                        {
                            if (user.Appellation != null && user.Appellation.Contains("金融专员"))
                            {
                                this.LogWriter.Write("微信用户同步跳过:金融专员不做处理");
                                //金融专员不做处理：
                                //金融专员有CAP账号，又有263邮箱，会导致手机号码重复；
                                //所以不同步
                            }
                            else
                            {
                                // 不存在,
                                if (CreateWeChatUser(user, WeChatParent))
                                {
                                    DataRow[] rows = dtUserRelation.Select("CODE='" + user.Code + "'");
                                    if (rows.Length == 0)
                                    {
                                        InsertToWechatUserRelation(user.Code, user.Name, user.Mobile);
                                    }
                                    syncResult.create_User_Success_Num++;
                                }
                                else
                                {
                                    syncResult.create_User_Fail_Num++;
                                }
                            }
                        }
                    }
                    else//微信存在此用户
                    {
                        //微信中有这个用户，但是H3中是禁用或者离职状态的，删除企业微信
                        //如果是金融专员，删除：保证一个人有一个号码；
                        if (user.State == State.Inactive || user.ServiceState == UserServiceState.Dismissed
                            || (user.Appellation != null && user.Appellation.Contains("金融专员")))
                        {
                            this.LogWriter.Write("微信用户删除:禁用/离职/金融专员");
                            if (DeleteWeChatUser(user.Code))
                            {
                                syncResult.delete_User_Success_Num++;
                            }
                            else
                            {
                                syncResult.delete_User_Fail_Num++;
                            }
                        }
                        else
                        {
                            #region 手机号码被修改了
                            //手机号码被修改了                            
                            if (user.Mobile != wxuser.mobile)
                            {
                                this.LogWriter.Write($"微信用户手机号码不一致,微信{wxuser.mobile},H3{user.Mobile}");
                                //这块删除逻辑有问题,先注释;
                                //DataTable dtTel = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_H3_Wechat_OrgSync WHERE tel='" + user.Mobile + "'");
                                //if (dtTel != null && dtTel.Rows.Count > 0)//此修改的手机号码有用户在使用，先删除此用户；
                                //{
                                //    foreach (DataRow row in dtTel.Rows)
                                //    {
                                //        var wxuserCF = GetWeChatUser(row["code"] + string.Empty);
                                //        if (DeleteWeChatUser(wxuserCF.userid))
                                //        {
                                //            syncResult.delete_User_Success_Num++;
                                //        }
                                //    }
                                //}

                                //微信认证后，默认会绑定微信对应的手机号码，会修改掉原系统中的手机号，导致2个系统的手机号不一致
                                //此时下面的删除逻辑会有问题，故注释；

                                //手机号码被修改了，但是此被激活了，需要先删除此用户，然后再同步
                                //if (wxuser.status == 1)
                                //{
                                //    this.LogWriter.Write($"微信用户删除:手机号码修改且已激活,原{wxuser.mobile},现{user.Mobile}");
                                //    if (DeleteWeChatUser(wxuser.userid))
                                //    {
                                //        syncResult.delete_User_Success_Num++;
                                //    }
                                //    this.LogWriter.Write($"微信用户删除后创建:手机号码修改且已激活,原{wxuser.mobile},现{user.Mobile}");
                                //    if (CreateWeChatUser(user, WeChatParent))
                                //    {
                                //        syncResult.create_User_Success_Num++;
                                //        InsertToWechatUserRelation(user.Code, user.Name, user.Mobile);
                                //    }
                                //    else
                                //    {
                                //        syncResult.create_User_Fail_Num++;
                                //    }
                                //    continue;
                                //}
                            }
                            #endregion

                            #region 手机号码没有被修改
                            //手机号码没有被修改
                            if (UpdateWeChatUser(user, WeChatParent))
                            {
                                //判断中间表是否有数据，有数据就更新，没有数据就插入
                                DataRow[] rows = dtUserRelation.Select("CODE='" + user.Code + "'");
                                if (rows.Length == 0)
                                {
                                    InsertToWechatUserRelation(user.Code, user.Name, user.Mobile);
                                }
                                else
                                {
                                    if (user.Mobile != wxuser.mobile)
                                    {
                                        //手机号码修改了才去Update中间表；
                                        string sqlUpdate = "UPDATE C_H3_Wechat_OrgSync SET name='" + user.Name + "',tel='" + user.Mobile + "' WHERE code='" + user.Code + "'";
                                        int n = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlUpdate);
                                        this.LogWriter.Write("Update C_H3_Wechat_OrgSync表，Code：" + user.Code + ",Name:" + user.Name + ",Tel:" + user.Mobile + ",受影响行数：" + n);
                                    }
                                }
                                syncResult.update_User_Success_Num++;
                            }
                            else
                            {
                                syncResult.update_User_Fail_Num++;
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion

            #region OU同步
            foreach (OThinker.Organization.OrganizationUnit sourceUnit in ChildList)
            {
                this.LogWriter.Write("微信OU同步,ID=" + sourceUnit.ObjectID + ",Name=" + sourceUnit.Name + ",WeChatParent=" + WeChatParent + ",WeChatID=" + sourceUnit.WeChatID);

                // 同步当前组织
                WeChatOrg_New org = this.GetWeChatOrgByParentId(WeChatParent, sourceUnit.WeChatID);
                if (org != null)
                {
                    // 已存在
                    if (sourceUnit.WeChatID != org.id)
                    {
                        sourceUnit.WeChatID = org.id;
                        Organization.UpdateUnit(OThinker.Organization.User.AdministratorID, sourceUnit);
                    }

                    //更新组织顺序
                    if (sourceUnit.SortKey != 5000 - sourceUnit.SortKey)
                    {
                        org.order = 5000 - sourceUnit.SortKey;
                        org.parentid = WeChatParent;
                        org.name = sourceUnit.Name;
                        if (UpdateWeChatOrg(org))
                        {
                            syncResult.update_OU_Success_Num++;
                        }
                        else
                        {
                            syncResult.update_OU_Fail_Num++;
                        }
                    }
                }
                else
                {// 不存在
                    org = CreateWeChatOrg(sourceUnit.WeChatID, sourceUnit.Name, WeChatParent, 5000 - sourceUnit.SortKey);
                    if (org.id != 0)
                    {
                        sourceUnit.WeChatID = org.id;
                        Organization.UpdateUnit(OThinker.Organization.User.AdministratorID, sourceUnit);
                    }
                    if (org.errcode == "0")
                        syncResult.create_OU_Success_Num++;
                    else
                        syncResult.create_OU_Fail_Num++;
                }

                // 递归同步子结构
                SyncOrg(LastSyncTime, sourceUnit.ObjectID, sourceUnit.WeChatID, dtUserRelation, ref syncResult);
            }
            #endregion

            return true;
        }


        /// <summary>
        /// 更新组织父节点和排序
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        private bool UpdateWeChatOrg(WeChatOrg_New org)
        {
            string res = post(string.Format(URL_UpdateOrg, AccessToken), JsonConvert.SerializeObject(org));
            this.LogWriter.Write("更新微信组织返回：Result=" + res + "，参数=" + JsonConvert.SerializeObject(org));
            WXResult wxRes = JsonConvert.DeserializeObject<WXResult>(res);
            if (wxRes.errcode == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 创建微信组织
        /// </summary>
        /// <param name="wechatId"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private WeChatOrg_New CreateWeChatOrg(int wechatId, string name, int parent, int order)
        {
            //此方式会有id，传到微信接口报错；
            //WeChatOrg org = new WeChatOrg()
            //{
            //    name = name,
            //    parentid = parent,
            //    order = order
            //};
            var org = new { name = name, parentid = parent, order = order };
            string res = PostWebRequest(string.Format(URL_CreateOrg, AccessToken), JsonConvert.SerializeObject(org), WeChatEncoding);
            this.LogWriter.Write("创建微信组织返回：" + name + ",微信Parent=" + parent + ",Result=" + res);
            this.LogWriter.Write("创建微信组织：" + JsonConvert.SerializeObject(org));
            var result = JsonConvert.DeserializeObject<WeChatOrg_New>(res);
            if (result.id < 1) result.id = wechatId;
            return result; 
        }

        #region 微信用户：读取、新增、修改、删除操作
        /// <summary>
        /// 获取微信用户
        /// </summary>
        /// <param name="userCode">用户账号</param>
        /// <returns></returns>
        public WXUser GetWeChatUser(string userCode)
        {
            string res = GetWebRequest(string.Format(URL_GetUser, AccessToken, userCode), WeChatEncoding);
            WXUser user = JsonConvert.DeserializeObject<WXUser>(res);
            return user;
        }
        
        /// <summary>
        /// 创建微信用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="department"></param>
        public bool CreateWeChatUser(OThinker.Organization.User user, int department)
        {
            WXUser wechatUser = new WXUser()
            {
                userid = user.Code,
                name = user.Name,
                department = new int[] { department },
                email = string.IsNullOrEmpty(user.Email) ? "" : user.Email.Replace("null", "").Replace("undefined", ""),
                enable = user.State == State.Active ? 1 : 0,
                mobile = string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile.Replace("null", "").Replace("undefined", ""),
                weixinid = user.WeChatAccount,
                position = user.Appellation,
                order = new int[] { 5000 - user.SortKey },
                telephone = string.IsNullOrEmpty(user.OfficePhone) ? "" : user.OfficePhone.Replace("null", "").Replace("undefined", ""),
                gender = ((int)user.Gender).ToString()
            };
            string res = PostWebRequest(string.Format(URL_CreateUser, AccessToken), JsonConvert.SerializeObject(wechatUser), WeChatEncoding);
            this.LogWriter.Write("创建微信用户返回：" + user.Code + ",Result=" + res);
            WXResult wxRes = JsonConvert.DeserializeObject<WXResult>(res);
            if (wxRes.errcode == 0)
            {
                #region insert to C_H3_Wechat_OrgSync表
                InsertToWechatUserRelation(user.Code, user.Name, user.Mobile);
                return true;
                #endregion
            }
            else
            {
                this.LogWriter.Write("创建微信用户失败的参数：" + JsonConvert.SerializeObject(wechatUser));
                return false;
            }
        }

        /// <summary>
        /// 更新微信用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="department"></param>
        private bool UpdateWeChatUser(OThinker.Organization.User user, int department)
        {
            WXUser wechatUser = new WXUser()
            {
                userid = user.Code,
                name = user.Name,
                department = new int[] { department },
                email = string.IsNullOrEmpty(user.Email) ? "" : user.Email.Replace("null", "").Replace("undefined", ""),
                enable = user.State == State.Active ? 1 : 0,
                mobile = string.IsNullOrEmpty(user.Mobile) ? "" : user.Mobile.Replace("null", "").Replace("undefined", ""),
                weixinid = user.WeChatAccount,
                position = user.Appellation,
                order = new int[] { 5000 - user.SortKey },
                telephone = string.IsNullOrEmpty(user.OfficePhone) ? "" : user.OfficePhone.Replace("null", "").Replace("undefined", ""),
                gender = ((int)user.Gender).ToString()
            };
            //加上离职的用户：禁用企业微信；
            if (user.ServiceState == UserServiceState.Dismissed)
            {
                wechatUser.enable = 0;
            }

            string res = PostWebRequest(string.Format(URL_UpdateUser, AccessToken), JsonConvert.SerializeObject(wechatUser), WeChatEncoding);
            this.LogWriter.Write("更新微信用户返回：" + res);
            WXResult wxRes = JsonConvert.DeserializeObject<WXResult>(res);
            if (wxRes.errcode == 0)
            {
                return true;
            }
            else
            {
                this.LogWriter.Write("更新微信用户失败的参数：" + JsonConvert.SerializeObject(wechatUser));
                return false;
            }
        }

        /// <summary>
        /// 删除微信用户
        /// </summary>
        /// <param name="usercode"></param>
        /// <returns></returns>
        private bool DeleteWeChatUser(string usercode)
        {
            string res = GetWebRequest(string.Format(URL_DeleteUser, AccessToken, usercode), WeChatEncoding);
            this.LogWriter.Write("删除成员:" + usercode + ",Msg:" + res);
            WXResult wxRes = JsonConvert.DeserializeObject<WXResult>(res);
            if (wxRes.errcode == 0)
            {
                string sqlDelete = "DELETE FROM C_H3_Wechat_OrgSync WHERE code='" + usercode + "'";
                int n = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlDelete);
                this.LogWriter.Write("删除C_H3_Wechat_OrgSync表用户：" + usercode + "，受影响行数：" + n);
                return true;                   
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region 消息推送 -----------------------------
        /// <summary>
        /// 发送新闻类消息,URL会自动进行Encode
        /// </summary>
        /// <param name="news">微信消息对象</param>
        [OThinker.Clusterware.LogicUnitMethod(OThinker.Clusterware.LogicUnitMethodType.Client)]
        public void SendNewsMessage(WeChatNews news)
        {
            LoadMessageToken();
            if (string.IsNullOrEmpty(AccessToken))
            {
                return;
            }
            foreach (WeChatArticle art in news.news.articles)
            {
                this.LogWriter.Write("WeChat Send Url:" + art.url);

                if (string.IsNullOrEmpty(art.url))
                {
                    break;
                }
                art.url = WebUtility.HtmlEncode(art.url);
                string flag = news.flag;
                if (art.url.IndexOf("state=") > -1)
                {
                    flag = art.url.Split(new string[] { "state=" }, StringSplitOptions.None)[1];
                    if (flag.IndexOf("&") > -1) flag = flag.Substring(0, flag.IndexOf("&"));
                }
                art.url = string.Format(URL_OAuth, CorpID, art.url, flag);
            }
            if (news.agentid == -1)
            {
                news.agentid = this.AgentID;
            }
            string res = JsonConvert.SerializeObject(news);
            res = res.Replace("\\u0026", "&");
            // 处理URL
            res = PostWebRequest(string.Format(URL_NewsMessage, AccessToken), res, WeChatEncoding);
            this.LogWriter.Write("WeChat result:" + res + ", touser:" + news.touser);
        }

        /// <summary>
        /// 根据微信CODE获取用户账号
        /// </summary>
        /// <param name="WeChatCode">微信传递过来的编码</param>
        /// <returns>返回用户登录帐号</returns>
        [OThinker.Clusterware.LogicUnitMethod(LogicUnitMethodType.Client)]
        public string GetUserCode(string WeChatCode)
        {
            LoadOrgToken();
            if (string.IsNullOrEmpty(AccessToken)) return string.Empty;
            string res = GetWebRequest(string.Format(URL_UserCode, AccessToken, WeChatCode, this.AgentID), WeChatEncoding);
            //this.LogWriter.Write("WeiXin Login:" + res);
            return JsonConvert.DeserializeObject<WXUser>(res).weixinid;
        }
        #endregion

        #region 微信中查看附件 -----------------------
        /// <summary>
        /// 获取文件在微信中查看地址
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BizObjectSchemaCode"></param>
        /// <param name="BizObjectId"></param>
        /// <param name="AttachmentID"></param>
        /// <returns></returns>
        [OThinker.Clusterware.LogicUnitMethod(LogicUnitMethodType.Client)]
        public string GetAttachmentWeChatViewUrl(string UserId, string BizObjectSchemaCode, string BizObjectId, string AttachmentID)
        {
            OThinker.H3.Data.AttachmentHeader attachmentHeader = this.BizObjectManager.GetAttachmentHeader(BizObjectSchemaCode, BizObjectId, AttachmentID);
            if (attachmentHeader != null)
            {
                string MediaID = null;
                bool needUpload = true;
                if (!string.IsNullOrEmpty(attachmentHeader.WeChatMediaID)
                    && attachmentHeader.WeChatMediaExpireTime > DateTime.Now)
                {
                    MediaID = attachmentHeader.WeChatMediaID;
                    needUpload = false;
                }

                if (needUpload)
                {
                    LoadMessageToken();
                    if (!string.IsNullOrEmpty(AccessToken))
                    {
                        var Attachment = this.BizObjectManager.GetAttachment(
                            OThinker.Organization.User.AdministratorID,
                            BizObjectSchemaCode,
                            BizObjectId,
                            AttachmentID);

                        if (Attachment != null)
                        {
                            MediaID = UploadFile(Attachment);
                            if (!string.IsNullOrEmpty(MediaID))
                            {
                                //微信5天后过期,保险起见,H3内设置4天后过期
                                DateTime WeChatExpireTime = DateTime.Now.AddDays(4);

                                this.BizObjectManager.UpdateAttachmentWeChatMediaID(AttachmentID, MediaID, WeChatExpireTime);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(MediaID))
                {
                    return string.Format(URL_DownloadFile, AccessToken, MediaID);
                }
            }
            return null;
        }

        /// <summary>
        /// 上传文件到微信并获取MediaID
        /// </summary>
        /// <param name="Attachment"></param>
        /// <returns>上传微信后得到的MediaID</returns>
        string UploadFile(OThinker.H3.Data.Attachment Attachment)
        {
            NameValueCollection nvc = new NameValueCollection();
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(string.Format(URL_UploadFile, AccessToken, "file"));
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
            string header = string.Format(headerTemplate, Attachment.FileName, Attachment.ContentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(Attachment.Content, 0, Attachment.Content.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                byte[] reByte = new Byte[wresp.ContentLength];

                Stream s = wresp.GetResponseStream();
                s.Read(reByte, 0, (int)wresp.ContentLength);

                string ret = System.Text.Encoding.UTF8.GetString(reByte, 0, reByte.Length).ToString();
                Dictionary<string, string> JSON =
                    (Dictionary<string, string>)Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ret);
                
                if (JSON != null && JSON.ContainsKey("media_id"))
                {
                    return JSON["media_id"];
                }
            }
            catch (Exception ex)
            {
                this.LogWriter.Write("上传文件到微信错误：" + ex.ToString());
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return null;
        }
        #endregion

        #region Post和Get请求 ------------------------
        /// <summary>
        /// 发送 Post Web 请求
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData);  // 转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteArray.Length;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);    // 写入参数
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), WeChatEncoding))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogWriter.Write(ex.ToString());
            }
            return ret;
        }

        /// <summary>
        /// 发送 Get Web 请求
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        private string GetWebRequest(string getUrl, Encoding dataEncode)
        {
            string ret = string.Empty;

            try
            {
                HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                using (HttpWebResponse webReponse = (HttpWebResponse)webReq.GetResponse())
                {
                    using (Stream stream = webReponse.GetResponseStream())
                    {
                        byte[] rsByte = new Byte[webReponse.ContentLength];

                        stream.Read(rsByte, 0, (int)webReponse.ContentLength);
                        ret = System.Text.Encoding.UTF8.GetString(rsByte, 0, rsByte.Length).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogWriter.Write(ex.ToString());
            }
            return ret;


        }
        /// <summary>
        /// get数据通用方法
        /// </summary>
        /// <param name="strURL">拼接好的链接</param>
        /// <returns></returns>
        public string get(string strURL)//get数据通用方法
        {
            string json = "";
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (HttpWebRequest)WebRequest.Create(strURL);
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            json = myreader.ReadToEnd();
            myreader.Close();
            return json;
        }

        /// <summary>
        /// 判断配置文件，获取accesstoken字符串
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// post数据通用方法
        /// </summary>
        /// <param name="strURL">要post到的URL</param>
        /// <param name="paraUrlCoded">要post过去的字符串</param>
        /// <returns></returns>
        public string post(string strURL, string paraUrlCoded)//post数据通用方法
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            System.Net.HttpWebResponse response;
            //Post请求方式
            request.Method = "POST";
            // 内容类型
            request.ContentType = "application/x-www-form-urlencoded";
            // 参数经过URL编码
            byte[] payload;
            //将URL编码后的字符串转化为字节
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的 ContentLength 
            request.ContentLength = payload.Length;
            //获得请 求流
            System.IO.Stream writer = request.GetRequestStream();
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            // 关闭请求流
            writer.Close();
            // 获得响应流
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
            return responseText;
        }
        #endregion


    }

    //
    // 摘要:
    //     微信组织
    public class WXUser
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string userid { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public int[] department { get; set; }
        public int[] order { get; set; }
        public string position { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string english_name { get; set; }
        public int status { get; set; }

        public string weixinid { get; set; }               
        public int enable { get; set; }  
    }

    public class WXResult
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
    }

    public class SyncResult
    {
        public bool Success { get; set; }
        public string ErrMsg { get; set; }

        public int create_OU_Success_Num { get; set; }
        public int create_OU_Fail_Num { get; set; }
        public int update_OU_Success_Num { get; set; }
        public int update_OU_Fail_Num { get; set; }

        public int create_User_Success_Num { get; set; }
        public int create_User_Fail_Num { get; set; }
        public int update_User_Success_Num { get; set; }
        public int update_User_Fail_Num { get; set; }

        public int delete_User_Success_Num { get; set; }
        public int delete_User_Fail_Num { get; set; }

        public override string ToString()
        {
            string msg = "";
            if (create_OU_Success_Num != 0 || create_OU_Fail_Num != 0)
                msg += "OU创建：成功" + create_OU_Success_Num + "个，失败" + create_OU_Fail_Num + "个；\n";

            if (update_OU_Success_Num != 0 || update_OU_Fail_Num != 0)
                msg += "OU更新：成功" + update_OU_Success_Num + "个，失败" + update_OU_Fail_Num + "个；\n";

            if (create_User_Success_Num != 0 || create_User_Fail_Num != 0)
                msg += "User创建：成功" + create_User_Success_Num + "个，失败" + create_User_Fail_Num + "个；\n";

            if (update_User_Success_Num != 0 || update_User_Fail_Num != 0)
                msg += "User更新：成功" + update_User_Success_Num + "个，失败" + update_User_Fail_Num + "个；\n";

            if (delete_User_Success_Num != 0 || delete_User_Fail_Num != 0)
                msg += "User刪除：成功" + delete_User_Success_Num + "个，失败" + delete_User_Fail_Num + "个；\n";

            if (msg == "")
                msg = "没有任何修改";
            return msg;
        }
    }

    public class WeChatOrgResult_New
    {
        public List<WeChatOrg_New> department { get; set; }
        public double errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class WeChatOrg_New
    {
        public string errcode { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public double order { get; set; }
        public int parentid { get; set; }
    }
}