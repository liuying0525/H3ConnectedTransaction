using DongZheng.H3.WebApi.Common.Portal;
using Newtonsoft.Json.Linq;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.DataModel;
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
    public class CRM_To_H3_RiskMeddleController : OThinker.H3.Controllers.ControllerBase
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
            AppUtility.Engine.LogWriter.Write("CRM_To_H3接口风险点干预流程数据：" + data);
            JavaScriptSerializer json = new JavaScriptSerializer();
            WorkFlowMethod.Return result = new WorkFlowMethod.Return();
            try
            {
                //data = "D7756CA4CAD69B232FCBDD7F9DBE3AE965C8D1AC54FB50518BCDC1725CCA1CD88A5FD5BC64BACA0C393440E2940EE851E96781BF17513B4C265C3F2822F905DDD35C10E28677CEBA042CBB142ED38015B630FF9A787E5395E8C0EEDF61175D634644B3B9998378480001C2F6691A47F8351CD6F68D4E217995FE481F77494D004F52C83F29747F70B8E66148858F850A0A6BEEFF0706424E1C500CD3DA0BEAD5C66275A6796B5E3C36221262678A70A00E7986BAE12506D81A73A67089861153802A6A743C0224990584045AD1B7381F9DE91BE2A503E92E94A1E552AE9E0E405CDA65C9FB3984C07D3EB2D0FE29A948E94146B6E5BFF61EE895A9B2FAE7A8D1E40C25C0DCE11BBCF46BCC1A3C6B5709C19B238F5ABC124D83332E3B2A434EFE063579C371DA886DA58702315F9356C2850B91BFBE508973EE7A582BEC10AFBEBE0107A687F4A5C45ABCB7CDFDB461C685A23AE70E22CCF21E0024A7B9E0024657140D80566B172422D3E02DA2BE4B9C3D9A82759DF0729268E792246AC9ABAD1AE8CAF2685C6C981BD059D752B5AE7474A3EF64FAC6CBA09ABF2D084507F5925B29ED591C208DB79261A47DA7EDC48A3A8460C884CFC8A48B24C7566F67868669D0ADBDF7733F60F1ED356E7DFAC66005D4EA260FF3B882C6A475AB8A733286C56CD4C53668516B5A0B0FDBA04F2F1C82AFA1EB4C06EFE5D9A1665A4359CF487B996E42BD9D74579E3CC8661A0F30FCFA7AC56DF8AD5A13416F2C4472AD90A098C50FD07C3FF5EFB9E6CABD5383C801E61B68F767CEFD60F84136EA91280723930FB23D7192E11F8683F3DE2508E4B3558B572BD80A329834AC2380BC1483A80F72DD397B1242628BAAD174E7F46F61B2E690E1BD4C4908A1AE7D9EF66DED5469085E166DDBE4F49A7B2C735FEADECB";
                List<DataItemParam> dataList = new List<DataItemParam>();
                data = DESDecrypt(data, "B^@s(d)+");
                AppUtility.Engine.LogWriter.Write("CRM_To_H3接口风险点干预解密数据：" + data);
                JObject jsonData = JObject.Parse(data);
                RiskPoint rp = new RiskPoint();
                rp.DealerID = jsonData["third_system_no"].ToString();
                rp.Originator = jsonData["originator"].ToString();
                List<RiskPointForm> rpfList = new List<RiskPointForm>();
                foreach (var item in jsonData["distributorlist"])
                {
                    RiskPointForm rpf = new RiskPointForm();
                    rpf.Dealer = item["distributor"].ToString();
                    rpf.Reason = item["message"].ToString();
                    rpf.DealerID = item["third_system_no"].ToString();
                    rpfList.Add(rpf);
                }
                rp.riskpointform = rpfList;

                string str = JSSerializer.Serialize(rp);
                BPMServiceResult bpmresult = startWorkflowByEntityTrinsJson("RiskPoint", jsonData["originator"].ToString(), false, str);  //;jsonData["originator"].ToString()    "liuwanying@dongzhengafc.com"   "18f923a7-5a5e-426d-94ae-a55ad1a4b239"
                                                                                                                                          //BPMServiceResult bpmresult = StartWorkflow("RiskPoint", jsonData["originator"].ToString(), false, dataList);  //"liuwanying@dongzhengafc.com"    "18f923a7-5a5e-426d-94ae-a55ad1a4b239"

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

        public string GetUserIDByCode(string UserCode)
        {
            var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, UserCode);
            return CurrentUserValidator?.UserID;
        }

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
        public BPMServiceResult StartWorkflow(string workflowCode, string userCode, bool finishStart, List<DataItemParam> paramValues)
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
                string user = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";// GetUserIDByCode(userCode);
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

        /// <summary>
        /// 启动H3流程实例(包含子表)
        /// </summary>
        /// <typeparam name="T">实体类-泛型</typeparam>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="userCode">启动流程的用户编码</param>
        /// <param name="finishStart">是否结束第一个活动</param>
        /// <param name="EntityParamValues">流程实例启动初始化数据项实体类集合</param>
        /// <returns></returns>
        private BPMServiceResult startWorkflowByEntityTrinsJson(
            string workflowCode,
            string userCode,
            bool finishStart,
             object EntityParamValues)
        {
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            BPMServiceResult result;

            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(workflowCode);
                if (workflowTemplate == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,流程模板不存在，模板编码:" + workflowCode + "。");
                    return result;
                }
                // 查找流程发起人
                //OThinker.Organization.User user = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
                string user = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";// GetUserIDByCode(userCode);  //"18f923a7-5a5e-426d-94ae-a55ad1a4b239"; //
                if (user == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,用户{" + userCode + "}不存在。");
                    return result;
                }

                OThinker.H3.DataModel.BizObjectSchema schema = OThinker.H3.Controllers.AppUtility.Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(
                    OThinker.H3.Controllers.AppUtility.Engine.Organization,
                    OThinker.H3.Controllers.AppUtility.Engine.MetadataRepository,
                    OThinker.H3.Controllers.AppUtility.Engine.BizObjectManager,
                    OThinker.H3.Controllers.AppUtility.Engine.BizBus,
                    schema,
                    OThinker.Organization.User.AdministratorID,
                    OThinker.Organization.OrganizationUnit.DefaultRootID);

                if (EntityParamValues != null)
                {
                    Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(EntityParamValues.ToString());
                    foreach (KeyValuePair<string, Newtonsoft.Json.Linq.JToken> jo in jObject)
                    {
                        //如果字段是GTAttachment则是附件
                        if (bo.Schema.ContainsField(jo.Key))
                        {
                            if (bo.Schema.GetLogicType(jo.Key) == OThinker.H3.Data.DataLogicType.Comment) continue;
                            //判断是否子表
                            else if (bo.Schema.GetLogicType(jo.Key) != OThinker.H3.Data.DataLogicType.BizObjectArray)
                            {
                                //给对象赋值
                                bo[jo.Key] = jo.Value;
                            }
                            else
                            {

                                List<object> list = JSSerializer.Deserialize<List<object>>(Newtonsoft.Json.JsonConvert.SerializeObject(jo.Value) as string);
                                //获取子表的属性
                                BizObjectSchema childSchema = schema.GetProperty(jo.Key).ChildSchema;
                                BizObject[] bizObjects = new BizObject[list.Count];
                                int i = 0;
                                foreach (object objT in list)
                                {
                                    bizObjects[i] = new BizObject(OThinker.H3.Controllers.AppUtility.Engine, childSchema, userCode);
                                    foreach (KeyValuePair<string, object> t in (dynamic)objT)
                                    {
                                        if (childSchema.ContainsField(t.Key))
                                        {
                                            //给对象赋值
                                            bizObjects[i][t.Key] = t.Value;
                                            continue;
                                        }
                                    }
                                    i++;
                                }
                                //子表回写给流程子表
                                bo[jo.Key] = bizObjects;
                            }
                        }
                    }

                }
                bo.Create();

                // 创建流程实例
                string InstanceId = OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.CreateInstance(
                    bo.ObjectID,
                    workflowTemplate.WorkflowCode,
                    workflowTemplate.WorkflowVersion,
                    null,
                    null,
                    user,
                    null,
                    false,
                    OThinker.H3.Instance.InstanceContext.UnspecifiedID,
                    null,
                    OThinker.H3.Instance.Token.UnspecifiedID);

                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        emergency,
                        InstanceId,
                        workItemID,
                        paramTables,
                        OThinker.H3.Instance.PriorityType.Normal,
                        true,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex + string.Empty);
            }
            return result;
        }

        private System.Web.Script.Serialization.JavaScriptSerializer _JsSerializer = null;
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return _JsSerializer;
            }
        }

        public class RiskPoint
        {
            public string Originator;
            public string RiskDealer;
            public string Opinion;
            public string DealerID;
            public List<RiskPointForm> riskpointform;
        }

        public class RiskPointForm
        {
            public string Dealer;
            public string Grade;
            public string Reason;
            public string CollateralRate;
            public string DealerID;
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
        /// 加密
        /// </summary>
        /// <param name="pToDecrypt">加密字符串</param>
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

        /// <summary>
        ///// 输出日志至引擎服务器
        /// </summary>
        /// <param name="message"></param>
        //[WebMethod(Description = "输出日志至引擎服务器")]
        public void WriteLog(string message)
        {
            this.Engine.LogWriter.Write(message);
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
                set { this.itemValue = value; }
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