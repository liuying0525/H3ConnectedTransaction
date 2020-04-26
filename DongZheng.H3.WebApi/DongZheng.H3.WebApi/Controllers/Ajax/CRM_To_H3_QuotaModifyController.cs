using DongZheng.H3.WebApi.Common.Portal;
using Newtonsoft.Json.Linq;
using OThinker.H3;
using OThinker.H3.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class CRM_To_H3_QuotaModifyController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => throw new NotImplementedException();

        public void Index()
        {
            var context = HttpContext;
            string data = "";
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                data = sr.ReadToEnd();
                JObject jtoken = JObject.Parse(data);
                data = jtoken["data"].ToString();
            }
            AppUtility.Engine.LogWriter.Write("CRM_To_H3接口升降机流程数据：" + data);
            JavaScriptSerializer json = new JavaScriptSerializer();
            WorkFlowMethod.Return result = new WorkFlowMethod.Return();
            try
            {
                List<DataItemParam> datalist = new List<DataItemParam>();
                //data = "E99C2DDDCA89444C64102DD9AB6A7CD99D73DDAAB7C174C19859B613653E5B03F933447C9682F656CFE7E3B38454BE98759EE1BBBF00AE67728F14B76B9C53C53DE16AF0CB4AC98C143BCC29022376AE979300301E5793AA8DB402584089EF0FDA33A90A31CB18BEC19BD6662D6C9D81CA0CF189B4D2AC583B26BA14AC0429482FFCC1D05149541728B69DD17277DD067317B52A5EAB7D78AC3CD57DCEBA2ACCE5FEC74536BA6E94823CD194B44F43F05826F3EF1BB049F70A42783829C539C55BDA9F5A19A242E2C579D96BD1D33E55B63E884CF5630D85EF87D03CC861867C5F4198BCDEBCF772DC8F9EBFF84BE4C88E33829EC379CFCB02D4EF2CB79CAC7E20C0CF7760F26BBEF7B670489B8598994E9404F21854669E13486354B647690527B23450B88148D6782DC374B97BE7B1A9E58ACAADFFE9168A547850087B839F1793853A616B01FB7ADBB20F6CF2D00CE7E1333EC620B1F4DF69DB16DC86BA7B768B65C5BDA0456556BBF08D98551A8F1F20548421C75BDE6AB9DF3068039333FF1393DEF1622CE397E26687623C87A7827DF192789537C96098ACE89CA21F754DDE94F9742C6E9AD6BF8723264816CB74C1CEFF6F285132C5FBB58C6B9052B811A36DC5306A7942F89D201D1BD6BA1956A704BEF076330614C8FA14EA1480420C0F455F133D5853BEC970B4AC583B81";
                data = DESDecrypt(data, "B^@s(d)+");
                AppUtility.Engine.LogWriter.Write("CRM_To_H3接口升降机解密数据：" + data);
                JObject jsonData = JObject.Parse(data);
                datalist.Add(new DataItemParam { ItemName = "Dealer", ItemValue = jsonData["distributor"].ToString() });
                datalist.Add(new DataItemParam { ItemName = "DealerClass", ItemValue = jsonData["distributortype"].ToString() });
                datalist.Add(new DataItemParam { ItemName = "CrmDealerId", ItemValue = jsonData["third_system_no"].ToString() });
                //datalist.Add(new DataItemParam { ItemName = "originator", ItemValue = jsonData["originator"].ToString() });
                datalist.Add(new DataItemParam { ItemName = "TargetLine", ItemValue = jsonData["targetquota"].ToString() });
                //datalist.Add(new DataItemParam { ItemName = "currentQuota", ItemValue = jsonData["currentquota"].ToString() });
                datalist.Add(new DataItemParam { ItemName = "TargetMargin", ItemValue = jsonData["targetbail"].ToString() });
                datalist.Add(new DataItemParam { ItemName = "Relegation", ItemValue = jsonData["levelchangetype"].ToString() == "1" ? "升级" : "降级" });
                datalist.Add(new DataItemParam { ItemName = "Remark", ItemValue = jsonData["modifycontent"].ToString() });

                string sql = "SELECT DISTRIBUTOR,TYPE,DISTRIBUTORTYPE,PROVINCE,CITY,COMPANYADDR,BELONGTO,BRAND,QYWYKT,LOANTYPE,MEMO,LICENSE,ENTERPRISEREGISTRATION,ZHTIME," +
                            " REGISTRATIONDATE,CREATDATE,LEGALREPRESENTATIVE,CORPORATEIDENTITYCARD,REGISTEREDCAPITAL,BANKBRANCH,ACCOUNTTYPE,BANKNAME,BANKACCOUNT,COUPLETNUMBER " +
                            " FROM I_ALLOWIN WHERE CRMDEALERID='" + jsonData["third_system_no"].ToString() + "'";

                System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (table != null && table.Rows.Count > 0)
                {
                    datalist.Add(new DataItemParam { ItemName = "Channels", ItemValue = table.Rows[0]["TYPE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Province", ItemValue = table.Rows[0]["PROVINCE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "City", ItemValue = table.Rows[0]["CITY"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Address", ItemValue = table.Rows[0]["COMPANYADDR"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Company", ItemValue = table.Rows[0]["BELONGTO"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Brand", ItemValue = table.Rows[0]["BRAND"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "NetSilver", ItemValue = table.Rows[0]["QYWYKT"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Loan", ItemValue = table.Rows[0]["LOANTYPE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Memo", ItemValue = table.Rows[0]["MEMO"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "License", ItemValue = table.Rows[0]["LICENSE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Register", ItemValue = table.Rows[0]["ENTERPRISEREGISTRATION"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "RegisterDate", ItemValue = table.Rows[0]["REGISTRATIONDATE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "OriginateDate", ItemValue = table.Rows[0]["CREATDATE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Representative", ItemValue = table.Rows[0]["LEGALREPRESENTATIVE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Card", ItemValue = table.Rows[0]["CORPORATEIDENTITYCARD"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Capital", ItemValue = table.Rows[0]["REGISTEREDCAPITAL"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "OpenBank", ItemValue = table.Rows[0]["BANKBRANCH"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "AccountType", ItemValue = table.Rows[0]["ACCOUNTTYPE"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "AccountName", ItemValue = table.Rows[0]["BANKNAME"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Account", ItemValue = table.Rows[0]["BANKACCOUNT"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "Couplet", ItemValue = table.Rows[0]["COUPLETNUMBER"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "OpenDate", ItemValue = table.Rows[0]["ZHTime"].ToString() });


                    BPMServiceResult bpmresult = StartWorkFlow("Relegation", jsonData["originator"].ToString(), false, datalist);   //"liuwanying@dongzhengafc.com"    jsonData["originator"].ToString()   "28f5d358-68fa-4d9c-982d-281ed216bd36"
                    if (bpmresult.Success)
                    {
                        result.Result = "0";
                        result.Message = result.Message;
                    }
                    else
                    {
                        result.Result = "-1";
                        result.Message = result.Message;
                    }
                }
                else
                {
                    result.Result = "-1";
                    result.Message = "未找到经销商信息";
                }
            }
            catch (Exception e)
            {
                result.Result = "-1";
                result.Message = e.Message;
            }

            context.Response.Write(json.Serialize(result));
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

        //public string GetUserIDByCode(string UserCode)
        //{
        //    var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(this.Engine, UserCode);
        //    return CurrentUserValidator?.UserID;
        //}

        public string getBizobjectByInstanceid(string Instanceid)
        {
            string sql = "select bizobjectid from H3.OT_instancecontext where objectid='" + Instanceid + "'";
            return this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }

        /// <summary>
        /// 启动H3流程实例
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="userCode">启动流程的用户编码</param>
        /// <param name="finishStart">是否结束第一个活动</param>
        /// <param name="paramValues">流程实例启动初始化数据项集合</param>
        /// <returns></returns>
        public BPMServiceResult StartWorkFlow(string workflowCode, string userCode, bool finishStart, List<DataItemParam> paramValues)
        {
            //ValidateSoapHeader();
            BPMServiceResult result = new BPMServiceResult();
            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(workflowCode);
                if (workflowTemplate == null)
                {
                    return new BPMServiceResult(false, "流程启动失败,流程模板不存在，模板编码:" + workflowCode + "。");
                }
                // 查找流程发起人
                //OThinker.Organization.User user = Engine.Organization.GetUnitByCode(userCode) as Organization.User;
                //string user = GetUserIDByCode(userCode);
                string user = UserValidatorFactory.GetUserValidator(Engine, userCode)?.UserID;
                if (user == null)
                {
                    return new BPMServiceResult(false, "流程启动失败,用户{" + userCode + "}不存在。");
                }
                OThinker.H3.DataModel.BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema, user);
                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in paramValues)
                    {
                        if (bo.Schema.GetProperty(param.ItemName).LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                        {
                            var t = new List<OThinker.H3.DataModel.BizObject>();
                            foreach (List<DataItemParam> list in (IEnumerable)param.ItemValue)
                            {
                                var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema.Fields.FirstOrDefault(x => x.ChildSchemaCode == param.ItemName).Schema, user);
                                foreach (DataItemParam item in list)
                                {
                                    if (m.Schema.ContainsField(item.ItemName)) m[item.ItemName] = item.ItemValue;
                                }
                                t.Add(m);
                            }
                            bo[param.ItemName] = t.ToArray();
                        }
                        else if (bo.Schema.ContainsField(param.ItemName))
                        {
                            bo[param.ItemName] = param.ItemValue;
                        }
                    }
                }
                bo.Create();
                // 创建流程实例
                //string InstanceId = this.Engine.InstanceManager.CreateInstance(
                //    bo.ObjectID,
                //    workflowTemplate.WorkflowCode,
                //    workflowTemplate.WorkflowVersion,
                //    null,
                //    null,
                //    user,
                //    null, // 以组的身份发起
                //    null, // 以岗位的身份发起
                //    false, //
                //    OThinker.H3.Instance.InstanceContext.UnspecifiedID,
                //    null,
                //    OThinker.H3.Instance.Token.UnspecifiedID);
                string InstanceId = this.Engine.InstanceManager.CreateInstanceByDefault(
                    bo.ObjectID, workflowTemplate.WorkflowCode, null, user);
                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();
                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage =
                    new OThinker.H3.Messages.StartInstanceMessage(emergency, InstanceId, null, paramTables,
                        OThinker.H3.Instance.PriorityType.Normal, true, null, false, OThinker.H3.Instance.Token.UnspecifiedID, null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, null, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
            }
            return result;
        }


        public Authentication authentication = new Authentication("administrator", "shdzretail1");
        public OThinker.H3.Controllers.UserValidator UserValidator = null;

        /// <summary>
        /// 验证当前用户是否正确
        /// </summary>
        public void ValidateSoapHeader()
        {
            if (authentication == null)
            {
                throw new Exception("请输入身份认证信息!");
            }
            UserValidator = OThinker.H3.Controllers.UserValidatorFactory.Validate(authentication.UserCode, authentication.Password);
            if (UserValidator == null)
            {
                throw new Exception("账号或密码不正确!");
            }
            Engine = UserValidator.Engine;
        }

        /// <summary>
        /// 身份验证类
        /// </summary>
        public class Authentication : System.Web.Services.Protocols.SoapHeader
        {
            public Authentication()
            {

            }

            public Authentication(string UserCode, string Password)
            {
                this.UserCode = UserCode;
                this.Password = Password;
            }

            public string UserCode { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <returns></returns>
        public OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
        {
            return GetWorkflowTemplate(workflowCode, Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode));
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="workflowVersion"></param>
        /// <returns></returns>
        public OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            return Engine.WorkflowManager.GetPublishedTemplateHeader(workflowCode, workflowVersion);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pToDecrypt">解密字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>

        public static string DESDecrypt(string pToDecrypt, string sKey = "test")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
        }

        [Serializable]
        public class DataItemParam
        {
            private string itemName = string.Empty;
            public string ItemName
            {
                get { return itemName; }
                set { this.itemName = value; }
            }

            private object itemValue = string.Empty;
            public object ItemValue
            {
                get { return itemValue; }
                set { itemValue = value; }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}