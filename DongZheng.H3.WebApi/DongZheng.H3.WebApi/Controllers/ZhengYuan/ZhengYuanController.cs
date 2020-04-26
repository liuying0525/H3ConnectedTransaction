using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.MySetting;
using DongZheng.H3.WebApi.Common.Util;
using DongZheng.H3.WebApi.Models;
using DongZheng.H3.WebApi.Models.ZhengYuanModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataItemParam = DongZheng.H3.WebApi.Models.DataItemParam;

namespace DongZheng.H3.WebApi.Controllers.ZhengYuan
{
    /// <summary>
    /// 正源科技
    /// </summary>
    [ValidateInput(false)]
    [Xss]
    public class ZhengYuanController : OThinker.H3.Controllers.ControllerBase
    {
        /// <summary>
        /// 正源Api地址Ip
        /// </summary>
        public static string zyApiIp = System.Configuration.ConfigurationManager.AppSettings["ZY_ApiIp"] + string.Empty;
        
        /// <summary>
        /// 正源产品编号
        /// </summary>
        public static string zyProductNumber = System.Configuration.ConfigurationManager.AppSettings["ZY_ProductNumber"] + string.Empty;

        /// <summary>
        /// 正源Api地址
        /// </summary>
        public static string zyApi = System.Configuration.ConfigurationManager.AppSettings["ZY_Api"] + string.Empty;

        /// <summary>
        /// 正源终审回调uri
        /// </summary>
        public static string zyFinalAuditCallBackUri = System.Configuration.ConfigurationManager.AppSettings["ZY_FinalAuditCallback"] + string.Empty;

        /// <summary>
        /// 正源放款通知回调uri
        /// </summary>
        public static string zyGiveMoneyCallBackUri = System.Configuration.ConfigurationManager.AppSettings["ZY_GiveMoneyCallback"] + string.Empty;


        /// <summary>
        /// 风控评估接口地址
        /// </summary>
        public static string url_RiskEvaluation = System.Configuration.ConfigurationManager.AppSettings["riskEvaluation"] + string.Empty;

        /// <summary>
        /// 风控评估回调地址
        /// </summary>
        public static string url_RiskResult = System.Configuration.ConfigurationManager.AppSettings["ZY_FkRiskResult"] + string.Empty;


        /// <summary>
        /// 正源sftp地址
        /// </summary>
        public static string zySftpHost = System.Configuration.ConfigurationManager.AppSettings["ZY_SftpHost"] + string.Empty;

        /// <summary>
        /// 正源sftp端口
        /// </summary>
        public static string zySftpPort = System.Configuration.ConfigurationManager.AppSettings["ZY_SftpPort"] + string.Empty;

        /// <summary>
        /// 正源sftp账户
        /// </summary>
        public static string zySftpUserName = System.Configuration.ConfigurationManager.AppSettings["ZY_SftpUserName"] + string.Empty;

        /// <summary>
        /// 正源sftp密码
        /// </summary>
        public static string zySftpPassword = System.Configuration.ConfigurationManager.AppSettings["ZY_SftpPassword"] + string.Empty;

        /// <summary>
        /// 银行卡报告接口地址
        /// </summary>
        public static string url_BankCardResult = System.Configuration.ConfigurationManager.AppSettings["queryBankCardReport"] + string.Empty;

        /// <summary>
        /// 主动查询风控评估结果的接口地址
        /// </summary>
        public static string url_RiskEvaluationResult = System.Configuration.ConfigurationManager.AppSettings["queryRiskEvaluationResult"] + string.Empty;

        public override string FunctionCode
        {
            get { return ""; }
        }

        public string Index()
        {
            return "ZhengYuan service";
        }

        /// <summary>
        /// 获取正源验证码
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetZyVerifyCode(string InstanceId)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            if (instanceContext == null)
            {
                rtn.code = -10000;
                rtn.message = "InstanceId不存在";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            //检查流程是否处于东正风控节点下
            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(InstanceId, "获取验证码");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("当前流程不处于获取验证码节点：" + Newtonsoft.Json.JsonConvert.SerializeObject(InstanceId));
                rtn.code = -10000;
                rtn.message = "当前流程错误，无法获取验证码";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            var codePara = GetVerifyCodePara(instanceContext.BizObjectId);

            if (string.IsNullOrEmpty(codePara["zy_SYS_XM"]) || string.IsNullOrEmpty(codePara["zy_SYS_IDCARD"]) || string.IsNullOrEmpty(codePara["zy_SYS_PHONE"]) || (string.IsNullOrEmpty(codePara["zy_SYS_BANKCARDNUMBER"]) && string.IsNullOrEmpty(codePara["zy_SYS_BANKCARDNUMBER2"])))
            {
                rtn.code = -10000;
                rtn.message = "四要素不全";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var codeRequest = new ZyVerifyCodeRequest();
            codeRequest.name = codePara["zy_SYS_XM"];
            codeRequest.idNo = codePara["zy_SYS_IDCARD"];
            codeRequest.mobile = codePara["zy_SYS_PHONE"];
            codeRequest.bankCardNo = !string.IsNullOrEmpty(codePara["zy_SYS_BANKCARDNUMBER"]) ? codePara["zy_SYS_BANKCARDNUMBER"] : codePara["zy_SYS_BANKCARDNUMBER2"];

            string str_paras = JsonConvert.SerializeObject(codeRequest);
            AppUtility.Engine.LogWriter.Write("正源获取验证码GetZyVerifyCode InstanceId=" + InstanceId + "--->request：" + str_paras);

            var result = HttpHelper.PostWebRequest(zyApi, "application/json", str_paras, 10);

            if (string.IsNullOrEmpty(result))
            {
                rtn.code = -10001;
                rtn.message = "获取验证码失败";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            AppUtility.Engine.LogWriter.Write("正源获取验证码GetZyVerifyCode InstanceId=" + InstanceId + "---> response：" + result);

            var codeResponse = JsonConvert.DeserializeObject<ZyVerifyCodeResponse>(result);

            if (codeResponse.returnCode != "000000" || string.IsNullOrEmpty(codeResponse.transactionNo))
            {
                rtn.code = -10001;
                rtn.message = codeResponse.returnMsg;
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var resultValue = new Dictionary<string, object>();
            resultValue.Add("zy_transactionNo", codeResponse.transactionNo);
            resultValue.Add("zy_signNo", codeResponse.signNo);
            AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 正源初审
        /// </summary>
        /// <returns></returns>
        public JsonResult ZyFirstAudit(string InstanceId,string VerifyCode,string TransactionNo)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "InstanceId不存在";
                return Json(rtn);
            }

            var resultValue = new Dictionary<string, object>();

            //无验证码或流水号为空
            if (string.IsNullOrEmpty(TransactionNo) || string.IsNullOrEmpty(VerifyCode))
            {
                resultValue.Add("flag_zyverifycode", 0);
                resultValue.Add("zy_firstAuditResult", "验证码错误，请重新获取验证码");
                AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);
                //驳回流程，返回到上一步
                WorkFlowFunction.ReturnWorkItemByInstanceid(InstanceId, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");

            var traceNo = data["zy_traceNo"].Value + string.Empty;

            var codePara = GetVerifyCodePara(instanceContext.BizObjectId);

            var auditRequest = new ZyFirstAuditRequest();
            auditRequest.name = codePara["zy_SYS_XM"];
            auditRequest.idNo = codePara["zy_SYS_IDCARD"];
            auditRequest.bankCardReservePhone = codePara["zy_SYS_PHONE"];
            auditRequest.bankCardNo = !string.IsNullOrEmpty(codePara["zy_SYS_BANKCARDNUMBER"]) ? codePara["zy_SYS_BANKCARDNUMBER"] : codePara["zy_SYS_BANKCARDNUMBER2"];
            auditRequest.identityCode = VerifyCode;
            auditRequest.transactionNo = TransactionNo;

            string str_paras = JsonConvert.SerializeObject(auditRequest);
            AppUtility.Engine.LogWriter.Write("正源初审ZyFirstAudit初审 InstanceId=" + InstanceId + "---> request：" + str_paras);

            var result = HttpHelper.PostWebRequest(zyApi, "application/json", str_paras, 10);

            AppUtility.Engine.LogWriter.Write("正源初审ZyFirstAudit初审 InstanceId=" + InstanceId + "---> response：" + result);

            

            if (string.IsNullOrEmpty(result))
            {
                resultValue.Add("flag_zyverifycode", 0);
                resultValue.Add("zy_firstAuditResult", "出现错误，请重新获取验证码提交");
                AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);
                //驳回流程，返回到上一步
                WorkFlowFunction.ReturnWorkItemByInstanceid(InstanceId, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var jObj = JObject.Parse(result);

            var zy_traceNo = string.Empty;

            if (string.IsNullOrEmpty(traceNo))
            {
                zy_traceNo = auditRequest.traceNo;
            }
            else
            {
                zy_traceNo = string.Format("{0},{1}", traceNo, auditRequest.traceNo);
            }

            resultValue.Add("zy_traceNo", zy_traceNo);

            //返回获取短信节点
            if (jObj["returnCode"].ToString() != "000000")
            {
                resultValue.Add("flag_zyverifycode", 0);
                resultValue.Add("zy_firstAuditResult", jObj["returnMsg"].ToString());
            }

            AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

            if (jObj["returnCode"].ToString() != "000000")
            {
                //驳回流程，返回到上一步
                WorkFlowFunction.ReturnWorkItemByInstanceid(InstanceId, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 正源初审结果回调
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ZyFirstAuditCallBack(ZyFirstAuditCallBackResponse callBack)
        {
            var rtn = new rtn_data { code = 1, message = "接收成功" };

            AppUtility.Engine.LogWriter.Write("正源初审结果回调：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));

            if (string.IsNullOrEmpty(callBack.transactionNo) || string.IsNullOrEmpty(callBack.apsStatus) || string.IsNullOrEmpty(callBack.traceNo))
            {
                AppUtility.Engine.LogWriter.Write("正源初审结果回调参数错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "参数有误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var instanceId = GetInstanceIdByTransactionNo(callBack.transactionNo, callBack.traceNo);

            if (string.IsNullOrEmpty(instanceId))
            {
                AppUtility.Engine.LogWriter.Write("正源初审结果回调：根据transactionNo获取instanceId错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "transactionNo错误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            
            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write("正源初审结果回调：未查询到instanceContext：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                rtn.code = 1;
                rtn.message = "";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

        
            //检查流程是否处于等待验证节点下
            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(instanceId, "等待验证");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("正源初审结果回调：当前流程不处于等待验证节点：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                rtn.code = 1;
                rtn.message = "";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var resultValue = new Dictionary<string, object>();

            if (callBack.apsStatus == "APP_SUCC")
            {
                resultValue.Add("flag_zyverifycode", 1);
                resultValue.Add("zy_firstAuditResultCode", callBack.apsStatus);
                var success = WorkFlowFunction.SubmitWorkItemByInstanceID(instanceId, "");
            }
            else
            {
                resultValue.Add("flag_zyverifycode", 0);
                resultValue.Add("zy_firstAuditResult", callBack.apsReason);
                resultValue.Add("zy_firstAuditResultRules", callBack.bankCreditRules);

                //驳回流程，返回到上一步
                WorkFlowFunction.ReturnWorkItemByInstanceid(instanceId, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
            }

            AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 正源终审
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ZyFinalAudit(string InstanceId)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);


            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "InstanceId不存在";
                return Json(rtn);
            }

            var finalRequest = BuildZyFinalAuditRequest(InstanceId, instanceContext.BizObjectId);

            string str_paras = JsonConvert.SerializeObject(finalRequest);

            AppUtility.Engine.LogWriter.Write("正源终审ZyFinalAudit终审 InstanceId=" + InstanceId + "---> request：" + str_paras);

            if (finalRequest == null)
            {
                rtn.code = -10001;
                rtn.message = "未查询到数据";
                return Json(rtn);
            }

            var result = HttpHelper.PostWebRequest(zyApi, "application/json", str_paras, 10);

            AppUtility.Engine.LogWriter.Write("正源终审ZyFinalAudit终审 InstanceId=" + InstanceId + "---> response：" + result);

            if (string.IsNullOrEmpty(result))
            {
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var jObj = JObject.Parse(result);

            var resultValue = new Dictionary<string, object>();

            //
            if (jObj["returnCode"].ToString() != "000000")
            {
                resultValue.Add("zy_finalAuditResult", jObj["returnMsg"].ToString());
            }
            else
            {
                resultValue.Add("zy_finalAuditDate", DateTime.Now.ToString("yyyyMMdd"));
            }

            AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 正源终审结果回调
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ZyFinalAuditCallBack(ZyFinalAuditCallBackResponse callBack)
        {
            var rtn = new rtn_data { code = 1, message = "接收成功" };

            AppUtility.Engine.LogWriter.Write("正源终审结果回调：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));

            if (string.IsNullOrEmpty(callBack.appSeq) || string.IsNullOrEmpty(callBack.apsStatus) || string.IsNullOrEmpty(callBack.traceNo))
            {
                AppUtility.Engine.LogWriter.Write("正源终审结果回调参数错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "参数有误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var instanceId = GetInstanceIdByApplicationNumber(callBack.appSeq, string.Empty);

            if (string.IsNullOrEmpty(instanceId))
            {
                AppUtility.Engine.LogWriter.Write("正源终审结果回调：根据appSeq获取instanceId错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "appSeq错误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write("正源终审结果回调：未查询到instanceContext：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                rtn.code = 1;
                rtn.message = "";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(instanceId, "等待审核");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("正源终审结果回调：当前流程不处于等待审核节点：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var resultValue = new Dictionary<string, object>();

            if (callBack.apsStatus == "APP_SUCC")
            {
                resultValue.Add("flag_finalaudit", "1");

                AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                var success = WorkFlowFunction.SubmitWorkItemByInstanceID(instanceId, "");
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 正源上传文件SFTP
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ZyUploadFile(string InstanceId, string Type)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            AppUtility.Engine.LogWriter.Write("正源上传文件SFTP Type==" + Type + "，InstanceId=" + InstanceId);

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "InstanceId不存在";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");

            var uploadFileName = "loanContract.pdf";

            var dkhtFileName = string.Empty;
            if (Type == "DKHT")
            {
                dkhtFileName = data["DKHT_FileName"].Value.ToString();

                if (string.IsNullOrEmpty(dkhtFileName))
                {
                    AppUtility.Engine.LogWriter.Write("上传文件至正源Sftp失败,为查询到贷款合同pdf附件 InstanceId=" + InstanceId);
                    rtn.code = -1001;
                    rtn.message = "上传文件至正源Sftp失败";
                    return Json(rtn, JsonRequestBehavior.AllowGet);
                }

            }
            if (Type == "Mortgage")
            {
                Type = "diyafiles";
            }

            //上传文件
            UploadFile(Type, uploadFileName, InstanceId, data);

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        public async Task UploadFile(string Type,string uploadFileName,string InstanceId, InstanceData data)
        {
            try
            {
                //校验instanceId准确性
                var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);


                using (var client = new SftpClient(zySftpHost, int.Parse(zySftpPort), zySftpUserName, zySftpPassword)) //创建连接对象
                {
                    client.Connect(); //连接

                    var rootPath = "DONGZHENGDOC";

                    var sftpPath = string.Format("{0}/{1}/{2}/{3}", rootPath, data["zy_finalAuditDate"].Value.ToString(), data["APPLICATION_NUMBER"].Value.ToString(), "04ContractData");

                    if (!client.Exists(sftpPath))
                    {
                        var paths = sftpPath.Split('/').Where(p => p != "DONGZHENGDOC").ToList();
                        if (paths.Count > 1)
                        {
                            var path = rootPath;
                            foreach (var p in paths)
                            {
                                path += "/" + p;
                                if (!client.Exists(path))
                                {
                                    client.CreateDirectory(path);
                                }
                            }
                        }
                        else
                        {
                            client.CreateDirectory(sftpPath);
                        }
                    }

                    client.ChangeDirectory(sftpPath); //切换目录

                    //根据DKHT_FileName，获取pdf合同，然后传给正源
                    if (Type == "DKHT")//贷款合同pdf
                    {
                        var dkhtPath = System.Configuration.ConfigurationManager.AppSettings["PrintPath"] + data["DKHT_FileName"].Value.ToString() + ".pdf";

                        using (var filestream = new FileStream(dkhtPath, FileMode.Open))
                        {
                            client.UploadFile(filestream, uploadFileName); //上传文件
                        }
                    }
                    else//抵押凭证jpg
                    {
                        uploadFileName = "pledgeContract.jpg";

                        var attachmentProperty = GetAttachmentProperty(instanceContext.BizObjectId, Type);

                        var attachment = AppUtility.Engine.BizObjectManager.GetAttachment(string.Empty, attachmentProperty["bizobjectschemacode"] + string.Empty, attachmentProperty["bizobjectid"] + string.Empty, attachmentProperty["objectid"] + string.Empty);

                        if (attachment == null || attachment.Content.Length <= 0)
                        {
                            AppUtility.Engine.LogWriter.Write("上传文件至正源Sftp失败,为查询到抵押凭证附件 InstanceId=" + InstanceId);

                            return;
                        }

                        Stream stream = new MemoryStream(attachment.Content);

                        client.UploadFile(stream, uploadFileName); //上传文件
                    }


                    AppUtility.Engine.LogWriter.Write("上传文件至正源Sftp InstanceId=" + InstanceId + "，--》path：" + sftpPath + "，-->filename：" + uploadFileName);

                    if (Type == "diyafiles")
                    {
                        var resultValue = new Dictionary<string, object>();
                        resultValue.Add("zy_diyaSftpPath", sftpPath + "/" + uploadFileName);
                        AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);
                    }

                    client.Disconnect();
                }

                //如果是合同号，生成二维码
                if (Type == "DKHT")
                {
                    var ms = QRCodeGenerator.GeneratorShopQrCode(string.Format("http://{0}/car/loanConfirmServlet?orgId=Par_dz&type=2&appSeq={1}&contractNo={2}&productNum={3}", zyApiIp, data["APPLICATION_NUMBER"].Value.ToString(), data["APPLICATION_NUMBER"].Value.ToString(), zyProductNumber));

                    SaveAttachment(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "zy_contractQrCode", "image/jpeg", "jpg", ms.ToArray());

                }
                else if (Type == "diyafiles")
                {
                    //推送抵押接口
                    ZyMortgage(InstanceId);
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("上传文件至正源Sftp失败 InstanceId=" + InstanceId + "---> ex：" + ex.ToString());
            }
        }

        /// <summary>
        /// 签章回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ZySignOrderCallBack(ZySignOrderCallBackResponse callBack)
        {
            var rtn = new rtn_data { code = 1, message = "接收成功" };

            AppUtility.Engine.LogWriter.Write("正源签章结果回调：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));

            if (string.IsNullOrEmpty(callBack.appSeq) || string.IsNullOrEmpty(callBack.status))
            {
                AppUtility.Engine.LogWriter.Write("正源签章结果回调参数错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "参数有误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var instanceId = GetInstanceIdByApplicationNumber(callBack.appSeq, string.Empty);

            if (string.IsNullOrEmpty(instanceId))
            {
                AppUtility.Engine.LogWriter.Write("正源签章结果回调：根据appSeq获取instanceId错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "appSeq错误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write("正源签章结果回调：未查询到instanceContext：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                rtn.code = 1;
                rtn.message = "";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(instanceId, "确认借款");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("正源签章结果回调：当前流程不处于确认借款节点：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var resultValue = new Dictionary<string, object>();

            if (callBack.status == "1")
            {
                resultValue.Add("flag_contract", "1");
                resultValue.Add("zy_loanId", callBack.loanId);
                resultValue.Add("zy_contractNo", callBack.contractNo);
                resultValue.Add("zy_contractSftpPath", callBack.sftpPath);

                AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                //下载签章合同
                Task.Run(() =>
                {
                    SaveDKHT(instanceId, instanceContext.BizObjectId, callBack.appSeq, callBack.sftpPath);
                });

                //等待1s，提交
                System.Threading.Thread.Sleep(1000);

                var success = WorkFlowFunction.SubmitWorkItemByInstanceID(instanceId, "");

                AppUtility.Engine.LogWriter.Write("正源签章结果回调：提交流程" + success.ToString());
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载签章合同，保存入DKHT字段
        /// </summary>
        [HttpGet]
        public void SaveDKHT(string instanceId, string bizObjectId, string appNumber, string dkhtSignPath)
        {
            try
            {
                using (var client = new SftpClient(zySftpHost, int.Parse(zySftpPort), zySftpUserName, zySftpPassword))
                {
                    client.Connect(); //连接

                    var bytes = client.ReadAllBytes(dkhtSignPath + "loanSignContract.pdf");

                    if (bytes.Length > 0)
                    {
                        Attachment attach = new Attachment()
                        {
                            ObjectID = Guid.NewGuid().ToString(),
                            BizObjectId = bizObjectId,
                            BizObjectSchemaCode = "APPLICATION_ZY",
                            FileName = "贷款合同(" + appNumber + ").pdf",
                            Content = bytes,
                            ContentType = "application/pdf",
                            ContentLength = bytes.Length,
                            DataField = "DKHT"
                        };
                        AppUtility.Engine.BizObjectManager.AddAttachment(attach);
                    }
                    else
                    {
                        AppUtility.Engine.LogWriter.Write("下载签章合同失败：instanceId=" + instanceId + "，dkhtSignPath=" + dkhtSignPath + "loanSignContract.pdf");
                    }

                    client.Disconnect();

                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("下载签章合同失败：" + ex.ToString());
            }
        }

        /// <summary>
        /// 调用正源抵押接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ZyMortgage(string InstanceId)
        {
            var rtn = new rtn_data { code = 1, message = "接收成功" };

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "InstanceId不存在";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");

            var diyaSftpFilePath = data["zy_diyaSftpPath"].Value + string.Empty;

            var appNumber = data["APPLICATION_NUMBER"].Value + string.Empty;

            var contractNo = data["zy_contractNo"].Value + string.Empty;

            var gpsInstallStatus = data["gpsInstallStatus"].Value + string.Empty;

            var mortgageRequest = new ZyMortgageRequest();
            mortgageRequest.appSeq = appNumber;
            mortgageRequest.returnUrl = zyGiveMoneyCallBackUri;
            mortgageRequest.gpsInstallStatus = gpsInstallStatus;
            mortgageRequest.pledgeTime = DateTime.Now.ToString("yyyy-MM-dd");

            var contractInfos = new List<ContractInfo>();
            contractInfos.Add(new ContractInfo { contractNo = contractNo, sftpPath = diyaSftpFilePath });
            mortgageRequest.contractInfo = contractInfos;
            

            string str_paras = JsonConvert.SerializeObject(mortgageRequest);

            AppUtility.Engine.LogWriter.Write("正源抵押ZyMortgage抵押 InstanceId=" + InstanceId + "---> request：" + str_paras);

            var result = HttpHelper.PostWebRequest(zyApi, "application/json", str_paras, 10);

            AppUtility.Engine.LogWriter.Write("正源抵押ZyMortgage抵押 InstanceId=" + InstanceId + "---> response：" + result);

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 正源放款通知
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>          
        [HttpPost]
        public JsonResult ZyGiveMoneyCallBack(ZyGiveMoneyResponse callBack)
        {
            var rtn = new rtn_data { code = 1, message = "接收成功" };

            AppUtility.Engine.LogWriter.Write("正源放款通知回调：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));

            var instanceId = GetInstanceIdByApplicationNumber(callBack.lpbApplDnList.FirstOrDefault().appSeq, string.Empty);

            if (string.IsNullOrEmpty(instanceId))
            {
                AppUtility.Engine.LogWriter.Write("正源放款通知回调：根据appSeq获取instanceId错误：" + Newtonsoft.Json.JsonConvert.SerializeObject(callBack));
                rtn.code = 0;
                rtn.message = "appSeq错误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write("正源放款通知回调：未查询到instanceContext：" + Newtonsoft.Json.JsonConvert.SerializeObject(instanceId));
                rtn.code = 1;
                rtn.message = "";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            var resultValues = new Dictionary<string, object>();

            resultValues.Add("zy_giveMoneyStatus", callBack.lpbApplDnList.FirstOrDefault().fkStatus);

            if (callBack.lpbApplDnList.FirstOrDefault().fkStatus == "0")
            {
                resultValues.Add("zy_giveMoneyReason", callBack.lpbApplDnList.FirstOrDefault().errorMsg);
            }

            AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValues);

            if (callBack.lpbApplDnList.FirstOrDefault().fkStatus == "1")
            {
                Task.Run(() =>
                {
                    ZyGetRepaymentPlan(instanceId, callBack.lpbApplDnList.FirstOrDefault().loanNo);
                });

                System.Threading.Thread.Sleep(500);

                WorkFlowFunction.SubmitWorkItemByInstanceID(instanceId, "");

            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询还款计划
        /// </summary>
        /// <param name="loanId">借据号</param>
        [HttpGet]
        public void ZyGetRepaymentPlan(string instanceId, string loanId)
        {
            var repaymentPlanRequest = new ZyQueryRepaymentPlanRequest { loanId = loanId };

            string str_paras = JsonConvert.SerializeObject(repaymentPlanRequest);

            AppUtility.Engine.LogWriter.Write("正源查询还款计划 InstanceId=" + instanceId + "，loanId=" + loanId + "---> request：" + str_paras);

            var result = HttpHelper.PostWebRequest(zyApi, "application/json", str_paras, 10);

            AppUtility.Engine.LogWriter.Write("正源查询还款计划 InstanceId=" + instanceId + "，loanId=" + loanId + "---> response：" + result);

            if (string.IsNullOrEmpty(result))
            {
                return;
            }

            var jObj = JObject.Parse(result);

            if (jObj["returnCode"].ToString() != "000000")
            {
                return;
            }

            var plans = JsonConvert.DeserializeObject<ZyRepaymentPlanResponse>(result);


            if (plans == null || plans.rows == null || !plans.rows.Any())
            {
                return;
            }

            //还款详情保存入库
            foreach (var detailPlan in plans.rows)
            {
                var sql = string.Format("insert into H3.C_ZYRETURN_DETAIL(objectid,parentobjectid,repayingplandetailid,repayingplanid,currentperiod,currentenddate,currentprincipal,currentinterest,currentprincipalinterest,endcurrentprincipal,endcurrentprincipalbalance,endcurrentinterest,overfee,overint,repayingdate,repayedprincipal,repayedinterest,repayedimposeinterest,repayedtotalamount,currentfee,repayedfee,deratefee,createdtime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}',to_date('{22}','yyyy/mm/dd HH24:MI:SS'))", Guid.NewGuid().ToString(), instanceId, detailPlan.repayingPlanDetailId, detailPlan.repayingPlanId, detailPlan.currentPeriod, detailPlan.currentEndDate, detailPlan.currentPrincipal, detailPlan.currentInterest, detailPlan.currentPrincipalInterest, detailPlan.endCurrentPrincipal, detailPlan.endCurrentPrincipalBalance, detailPlan.endCurrentInterest, detailPlan.overFee, detailPlan.overInt, detailPlan.repayingDate, detailPlan.repayedPrincipal, detailPlan.repayedInterest, detailPlan.repayedImposeInterest, detailPlan.repayedTotalamount, detailPlan.currentFee, detailPlan.repayedFee, detailPlan.derateFee, DateTime.Now.ToString());

                OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql.ToString());
            }
            
        }



        /// <summary>
        /// 风控自动审批
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        public JsonResult FkAutoAuit(string InstanceId)
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            var recordNumber = string.Empty;

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("正源 InstanceId={0},上下文不存在", InstanceId));

                return Json(new { reqID = InstanceId, recordId = "" }, JsonRequestBehavior.AllowGet);
            }

            recordNumber = RiskEvaluation(InstanceId);

            if (!string.IsNullOrEmpty(recordNumber))
            {
                AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "FK_SYS_TYPE", "1");

                AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "FK_RECORDID", recordNumber);

            }

            return Json(new { reqID = InstanceId, recordId = recordNumber }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 风控自动审批回调
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RiskSysResult(Risk.Parameters_Result para)
        {
            AppUtility.Engine.LogWriter.Write("正源-风控系统回调参数：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));

            rtn_data rtn = new rtn_data { code = 1, message = "接受成功" };

            //校验参数
            if (para.IsEmpty())
            {
                rtn.code = -10001;
                rtn.message = "一个或多个参数为空";

                return Json(rtn);
            }

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(para.reqId);

            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "reqId不合法";
                return Json(rtn);
            }

            //检查流程是否处于东正风控节点下
            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(para.reqId, "风控");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("当前流程不处于东正风控：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                rtn.code = -10001;
                rtn.message = "当前流程不处于东正风控";
                return Json(rtn);
            }


            var fkresult = "转人工";

            //流程
            List<DataItemParam> keyValues = new List<DataItemParam>();

            //更新结果
            var updateResultFlag = false;

            //审核结果
            var resultValue = new Dictionary<string, object>();
            resultValue.Add("FK_SHOWRESULT", para.showResult == null ? "null" : para.showResult);

            switch (para.finalResult)
            {
                case "00":
                    //初始化 记录日志，不处理
                    AppUtility.Engine.LogWriter.Write("风控系统返回00-初始化：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                    fkresult = "初始化";

                    resultValue.Add("FK_RESULT", "初始化");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                    break;

                case "01":
                    //通过
                    resultValue.Add("FK_RESULT", "通过");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);


                    //1.Automatic_approval = 1 
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 1
                    });

                    var userId = WorkFlowFunction.GetUserIDByCode("rsfk");

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, userId);


                    //2.提交当前任务
                    var result = WorkFlowFunction.SubmitWorkItemByInstanceID(para.reqId, "");
                    if (!result)
                    {
                        AppUtility.Engine.LogWriter.Write("风控系统返回01-通过，提交当前任务失败" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                    }

                    fkresult = "通过";

                    break;

                case "02":
                    //拒绝
                    resultValue.Add("FK_RESULT", "拒绝");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);


                    //1.Automatic_approval = -1 
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = -1
                    });

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, WorkFlowFunction.GetUserIDByCode("rsfk"));

                    //结束流程
                    WorkFlowFunction.FinishedInstance(para.reqId);

                    fkresult = "拒绝";

                    break;
                case "03":
                    //转人工
                    resultValue.Add("FK_RESULT", "转人工");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                    //1.Automatic_approval = 0

                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 0
                    });

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, WorkFlowFunction.GetUserIDByCode("rsfk"));

                    //2.提交当前任务
                    var success = WorkFlowFunction.SubmitWorkItemByInstanceID(para.reqId, "");
                    if (!success)
                    {
                        AppUtility.Engine.LogWriter.Write("风控系统返回03-转人工，提交当前任务失败" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                    }
                    fkresult = "转人工";

                    break;
                default:
                    rtn.code = -10001;
                    rtn.message = "finalResult不合法";
                    break;
            }

            //更新风控返回结果
            //AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "fkResult", fkresult);

            AppUtility.Engine.LogWriter.Write("风控系统回调结果fkResult：" + fkresult);


            AppUtility.Engine.LogWriter.Write("风控系统回调结果更新fkResult：" + updateResultFlag.ToString());


            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AutoFkAudit()
        {
            AppUtility.Engine.LogWriter.Write("自动通过东正风控评估节点" );

            string sql = "select * from H3.OT_WORKITEM where workflowcode='APPLICATION_ZY' and displayname like '%风控%' ";

            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(row["instanceid"]+string.Empty);

                    var resultValue = new Dictionary<string, object>();
                    resultValue.Add("FK_SHOWRESULT", "建议转人工");

                    resultValue.Add("FK_RESULT", "转人工");
                    AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                    //1.Automatic_approval = 0
                    List<DataItemParam> keyValues = new List<DataItemParam>();
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 0
                    });
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "FK_SYS_TYPE",
                        ItemValue = "1"
                    });
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "FK_RECORDID",
                        ItemValue = DateTime.Now.Ticks.ToString()
                    });

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, WorkFlowFunction.GetUserIDByCode("rsfk"));

                    //2.提交当前任务
                    var success = WorkFlowFunction.SubmitWorkItemByInstanceID(row["instanceid"] + string.Empty, "");
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询银行卡信息报告
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="ApplicationNumber"></param>
        /// <returns></returns>
        public JsonResult QueryBankCardReport(string InstanceId, string IdCard, string Name)
        {
            var getReport = GetFkReport(InstanceId, "bankcardinfo");

            if (!string.IsNullOrEmpty(getReport))
            {
                return Json(getReport, JsonRequestBehavior.AllowGet);
            }

            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");

            var obj_paras = new { idCard = IdCard, card1 = data["zy_SYS_BANKCARDNUMBER"].Value + string.Empty, card2 = data["zy_SYS_BANKCARDNUMBER2"].Value + string.Empty };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询银行卡信息报告：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_BankCardResult, "application/json", str_paras);
            AppUtility.Engine.LogWriter.Write("查询银行卡信息报告结果：" + result);


            if (result == null || result == "")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var jObject = JObject.Parse(result);

            if (jObject["code"].ToString() != "00")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //保存入数据库
            SaveFkReport(result, InstanceId, "bankcardinfo", string.Empty);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询风控评估结果
        /// </summary>
        /// <param name="recordId">风控系统ID</param>
        /// <param name="InstanceId">流程实例ID</param>
        /// <returns></returns>
        public JsonResult QueryRiskEvaluationResult(string InstanceId, string idCard)
        {
            var dicResult = GetFkReportId(InstanceId);

            if (string.IsNullOrEmpty(dicResult["recordId"]))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            var recordId = dicResult["recordId"];

            var getReport = GetFkReport(InstanceId, "fkreport", recordId);

            if (!string.IsNullOrEmpty(getReport))
            {
                return Json(getReport, JsonRequestBehavior.AllowGet);
            }

            var obj_paras = new { recordId = recordId, reqId = InstanceId, idCard = idCard };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询风控评估结果：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_RiskEvaluationResult, "application/json", str_paras);

            if (result == null || result == "")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var jObject = JObject.Parse(result);

            if (jObject["code"].ToString() != "00")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //保存入数据库
            SaveFkReport(result, InstanceId, "fkreport", recordId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Build Method

        /// <summary>
        /// 构建正源终审请求参数
        /// </summary>
        /// <returns></returns>
        public ZyFinalAuditRequest BuildZyFinalAuditRequest(string instanceId, string objectId)
        {
            var orderRow = GetOrderInfo(objectId);
            if (orderRow == null)
            {
                return null;
            }

            var finalAuditRequest = new ZyFinalAuditRequest();

            //1.基础数据
            finalAuditRequest.returnUrl = zyFinalAuditCallBackUri;

            InstanceData data = new InstanceData(AppUtility.Engine, instanceId, "");

            finalAuditRequest.appSeq = data["APPLICATION_NUMBER"].Value.ToString();


            //2.applyBaseInfo 申请人基本信息域
            var zyApplyBaseInfoPara = GetVerifyCodePara(objectId);

            var zyApplyBaseInfo = new ZYApplyBaseInfo();
            zyApplyBaseInfo.bankCardNo = !string.IsNullOrEmpty(zyApplyBaseInfoPara["zy_SYS_BANKCARDNUMBER"]) ? zyApplyBaseInfoPara["zy_SYS_BANKCARDNUMBER"] : zyApplyBaseInfoPara["zy_SYS_BANKCARDNUMBER2"];

            zyApplyBaseInfo.bankCardReservePhone = zyApplyBaseInfoPara["zy_SYS_PHONE"];

            finalAuditRequest.applyBaseInfo = zyApplyBaseInfo;

            //3.baseInfo 申请信息
            var zyBaseInfo = new ZYBaseInfo();
            zyBaseInfo.applyAmt = Convert.ToDecimal(orderRow["AMOUNT_FINANCED"].ToString()).ToString("F2");
            zyBaseInfo.applyTerm = orderRow["LEASE_TERM_IN_MONTH"].ToString();

            //获取职业身份 移至构建数据 赋值
            //zyBaseInfo.occupationalIdentity = "";


            //4.reviewResultInfo 高审结果信息域
            var zyReviewResult = new ZYReviewResultInfo();
            zyReviewResult.approAmt = Convert.ToDecimal(orderRow["AMOUNT_FINANCED"].ToString()).ToString("F2");
            zyReviewResult.contractAmt = Convert.ToDecimal(orderRow["AMOUNT_FINANCED"].ToString()).ToString("F2");
            zyReviewResult.reconsiderAmt = Convert.ToDecimal(orderRow["NEW_PRICE"].ToString()).ToString("F2");
            zyReviewResult.approTerm = orderRow["LEASE_TERM_IN_MONTH"].ToString();
            zyReviewResult.monthTotalRate = ((Convert.ToDecimal(orderRow["BASE_CUSTOMER_RATE"].ToString()) / 12) / 100m).ToString("F6");

            //TODO：获取还款方式
            var paymTypeDic= GetDropDownListSource("ZYpaymType", "get_financial_product_group", "FP_GROUP_ID", "FP_GROUP_DSC");

            var paymTypeName = paymTypeDic[orderRow["FP_GROUP_ID"] + string.Empty] + string.Empty;

            //TODO：后面删除
            zyReviewResult.paymType = "01";

            if (paymTypeName.Contains("等额本息"))
            {
                zyReviewResult.paymType = "01";
            }
            if (paymTypeName.Contains("等额本金"))
            {
                zyReviewResult.paymType = "03";
            }

            //5.personalInfo 个人信息  
            //获取个人信息
            var personalInfo = new ZYPersonalInfo();

            //7.componyInfo 公司信息
            var companyInfo = new ZYComponyInfo();

            //8.spouseInfo 配偶/直系亲属
            var spouseInfo = new ZYSpouseInfo();

            //构建数据
            ZyFinalAuditModel(objectId, zyBaseInfo, personalInfo, companyInfo, spouseInfo, data);

            //6.houseInfo 房产信息
            var houseInfo = new ZYHouseInfo { isExistHome = "1" };

            //9.carInfo 车辆信息
            var carInfo = new ZYCarInfo();

            carInfo.shop = data["BUSINESS_PARTNER_ID"].Value + string.Empty;
            carInfo.city = GetMappingCity(GetCityName(orderRow["state_cde"] + string.Empty, orderRow["CITY_CDE"] + string.Empty));
            carInfo.provide = GetProvideName(orderRow["state_cde"] + string.Empty);
            carInfo.carCode = orderRow["vin_number"].ToString();
            carInfo.engineCode = orderRow["engine_number"].ToString();
            carInfo.carBrand = orderRow["ASSET_MAKE_DSC"].ToString();
            carInfo.carCategory = orderRow["ASSET_BRAND_DSC"].ToString();
            carInfo.carType = orderRow["POWER_PARAMETER"].ToString();
            carInfo.purchasePrice = Convert.ToDecimal(orderRow["AMOUNT_FINANCED"].ToString()).ToString("F2");

            //10.merchant  商户信息（放款银行信息）

            var merchant = new ZYMerchant();
            var userCode = orderRow["USER_CODE"] + string.Empty;
            var merchantDt = CommonFunction.ExecuteDataTableSql("CAPDB", string.Format("select * from  in_cms.v_dealer_bank_info where user_name='{0}'", userCode));

            var merchantName = merchantDt.Rows[0]["ACCOUNT_NME"] + string.Empty;
            merchant.merchantName = merchantName;
            merchant.merchantIdType = merchantName.Contains("公司") ? "10" : "01";
            merchant.merchantIdNo = merchantDt.Rows[0]["organization_cde"] + string.Empty;
            merchant.merchantAcctType = merchantName.Contains("公司") ? "02" : "01"; ;

            //总行
            var bankName = merchantDt.Rows[0]["BANK_NAME"] + string.Empty;
            //支行
            var bankBranchName = merchantDt.Rows[0]["BRANCH_BANK_NAME"] + string.Empty;

            merchant.merchantBankName = bankName;

            if (merchant.merchantIdType == "10")
            {
                merchant.bankChildName = bankBranchName;
                merchant.bankProvince = merchantDt.Rows[0]["STATE_NME"] + string.Empty;
                merchant.bankCity = merchantDt.Rows[0]["CITY_NME"] + string.Empty;
            }
            

            var banks = GetZyBankCode();
            var bankCode = string.Empty;
            var bankByName = banks.Where(p => bankName.Trim() == p.name.Trim()).FirstOrDefault();
            if (bankByName != null)
            {
                bankCode = bankByName.code;
            }
            else
            {
                var bankByBranchName = banks.Where(p => bankBranchName.Trim() == p.name.Trim()).FirstOrDefault();

                bankCode = bankByBranchName == null ? "000" : bankByBranchName.code;
            }

            merchant.merchantBankCode = bankCode;
            //TODO：正式环境需修改
            merchant.merchantBankCardNo = merchantDt.Rows[0]["ACCOUNT_NBR"] + string.Empty;// "110101199003072017"; //

            //最后 赋值
            finalAuditRequest.baseInfo = zyBaseInfo;
            finalAuditRequest.reviewResultInfo = zyReviewResult;
            finalAuditRequest.personalInfo = personalInfo;
            finalAuditRequest.componyInfo = companyInfo;
            finalAuditRequest.spouseInfo = spouseInfo;
            finalAuditRequest.houseInfo = houseInfo;
            finalAuditRequest.carInfo = carInfo;
            finalAuditRequest.merchant = merchant;
            return finalAuditRequest;
        }


        /// <summary>
        /// 风控系统评估Api
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public string RiskEvaluation(string InstanceId)
        {
            Risk.Parameters_Evaluation para = GetRiskEvaluationParameters(InstanceId);
            string str_paras = JsonConvert.SerializeObject(para);
            AppUtility.Engine.LogWriter.Write("正源-零售调用风控评估参数：" + str_paras);

            var result = HttpHelper.PostWebRequest(url_RiskEvaluation, "application/json", str_paras);
            AppUtility.Engine.LogWriter.Write("正源-零售风控评估结果:" + result);
            if (string.IsNullOrEmpty(result))
            {
                return string.Empty;
            }
            //调试；
            DZCommonApiResult<object> rtn = JsonConvert.DeserializeObject<DZCommonApiResult<object>>(result);

            AppUtility.Engine.LogWriter.Write(string.Format("正源-零售调用风控风控评估结果：{0}", JsonConvert.SerializeObject(rtn)));

            if (rtn.code == "00" || rtn.code == "04")
            {

                var data = JsonConvert.SerializeObject(rtn.data);

                var jobj = JObject.Parse(data);

                return jobj["recordId"].ToString();
            }
            else
            {
                return string.Empty; ;
            }
        }

        /// <summary>
        /// 构建自动审批参数
        /// </summary>
        /// <param name="InstanceId"></param>
        public Risk.Parameters_Evaluation GetRiskEvaluationParameters(string InstanceId)
        {

            InstanceContext ic = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");

            #region 判断是第一次请求还是多次请求
            string requestFlags = "";
            var rtn = WorkflowSetting.GetCustomSettingValue(InstanceId, "requestFlags");
            if (string.IsNullOrEmpty(rtn))
            {
                requestFlags = "00";
                WorkflowSetting.SetCustomSettingValue(InstanceId, ic.BizObjectSchemaCode, "requestFlags", "1");
            }
            else
            {
                requestFlags = "01";
                WorkflowSetting.SetCustomSettingValue(InstanceId, ic.BizObjectSchemaCode, "requestFlags", (Convert.ToInt32(rtn) + 1) + string.Empty);
            }
            #endregion

            Risk.Parameters_Evaluation para = new Risk.Parameters_Evaluation();
            para.reqId = InstanceId;
            para.cbUrl = url_RiskResult;
            para.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            para.requestId = data["APPLICATION_NUMBER"].Value + string.Empty;//申请单号
            para.requestFlags = requestFlags;//00新增请求;01变更请求，重新跑规则

            var personsInfo = GetPersonsInfo(ic.BizObjectId);

            string[] cyzdArray = new string[1];
            var userCode = data["user_name"] + string.Empty;//供应商账号
            var idCard = personsInfo.Persons.Where(p=>p.personnelRelationship== "OneSelf本人").First().certNo;// data["id_card_nbr"] + string.Empty;//身份证号码

            var row_Order = GetOrderInfo(ic.BizObjectId);

            Risk.Parameters_Evaluation.Order o = new Risk.Parameters_Evaluation.Order();
            o.businessType = row_Order["condition"] + string.Empty == "N" ? "新车 New" : "二手车 Uesd";
            o.foursNumber = row_Order["USER_CODE"] + string.Empty;
            o.dealerNam = row_Order["USER_NAME"] + string.Empty;
            o.vehicleModel = row_Order["ASSET_BRAND_DSC"] + string.Empty;
            o.vehicleBrand = row_Order["ASSET_MAKE_DSC"] + string.Empty;
            o.vehiclePurpose = row_Order["USAGE7"] + string.Empty == "1" ? "自用 Self-Use" : "商用 Corporate-Use";
            o.vehiclePrice = row_Order["SALE_PRICE"] + string.Empty;
            o.applicationAmount = row_Order["AMOUNT_FINANCED"] + string.Empty;
            o.applicationTerm = row_Order["LEASE_TERM_IN_MONTH"] + string.Empty;
            o.productGroup = row_Order["FP_GROUP_NAME"] + string.Empty;
            o.productType = row_Order["FINANCIAL_PRODUCT_NAME"] + string.Empty;
            o.firstPaymentsRatio = row_Order["SECURITY_DEPOSIT_PCT"] + string.Empty;
            o.firstPaymentsAmount = row_Order["CASH_DEPOSIT"] + string.Empty;
            o.vehicleIdentificationValue = row_Order["NEW_PRICE"] + string.Empty;
            o.extraChargeLoan = row_Order["ACCESSORY_AMT"] + string.Empty;
            o.ifRereqComplete = "是";
            o.ifMortgage = personsInfo.Ifmortgage;
            o.vehicleManufactureDate = GetDate(row_Order["RELEASE_DTE"] + string.Empty);
            o.vehicleKilometers = row_Order["ODOMETER_READING"] + string.Empty;
            o.ifDataComplete = "是";
            o.plegeAddressCity = row_Order["MortgageCity"] + string.Empty;
            o.resideAddressProvince = GetProvideName(row_Order["STATE_CDE"] + string.Empty); 
            o.dealerAddressCity = GetMappingCity(GetCityName(row_Order["STATE_CDE"] + string.Empty, row_Order["CITY_CDE"] + string.Empty));
            o.customerSource = getFIType(row_Order["USER_CODE"] + string.Empty);
            o.ifStockSimplifiedLoan = "否";
            o.exposureValue = "0";
            o.bankNo = data["Caccountnum"].Value + string.Empty;
            o.interestRates = Convert.ToDecimal(row_Order["CALC_SUBSIDY_RTE"].ToString());
            o.dealerType = row_Order["ASSET_MAKE_DSC"] + string.Empty;
            var maleCardLoans = row_Order["isgpd"] + string.Empty;
            if (string.IsNullOrEmpty(maleCardLoans))
            {
                o.maleCardLoans = 0;
            }
            else
            {
                o.maleCardLoans = Convert.ToInt32(maleCardLoans);
            }

            //判断贷款类型
            //机构贷   ：申请类型：公司
            //公牌贷   ：申请类型：个人，共同借款人为公司
            //借牌贷   ：申请类型：个人， 主贷人非抵押人
            //个人贷   ：非上述三种
            var loanTypeName = data["APPLICATION_TYPE_CODE"].Value + string.Empty == "00001" ? "个人" : "机构";

            var loanType = "个人贷";

            if (loanTypeName == "机构")
            {
                loanType = "机构贷";
            }
            else
            {
                if (o.maleCardLoans > 0)
                {
                    loanType = "公牌贷";
                }
                else if (o.ifMortgage == "否")
                {
                    var person = personsInfo.Persons.Where(p => p.personnelCategory == "B" && p.personnelRelationship != "Husband-Wife配偶关系" && p.lienee == "1");
                    if (person != null && person.Any())
                    {
                        loanType = "借牌贷";
                    }
                }
            }
            o.loanType = loanType;

            para.personnelList = personsInfo.Persons;
            para.order = o;

            return para;
        }

        #endregion


        #region SqlMethod

        /// <summary>
        /// 获取短信接口的参数
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetVerifyCodePara(string objectId)
        {
            string sql = "select zy_SYS_XM,zy_SYS_IDCARD,zy_SYS_PHONE,zy_SYS_BANKCARDNUMBER,zy_SYS_BANKCARDNUMBER2 from  h3.i_application_zy  where objectid ='" + objectId + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var xm = string.Empty;
            var idCard = string.Empty;
            var phone = string.Empty;
            var bankCard1 = string.Empty;
            var bankCard2 = string.Empty;

            var dic = new Dictionary<string, string>();

            if (dt.Rows.Count > 0)
            {
                xm = dt.Rows[0][0].ToString();
                idCard = dt.Rows[0][1].ToString();
                phone = dt.Rows[0][2].ToString();
                bankCard1 = dt.Rows[0][3].ToString();
                bankCard2 = dt.Rows[0][4].ToString();
            }
            dic.Add("zy_SYS_XM", xm);
            dic.Add("zy_SYS_IDCARD", idCard);
            dic.Add("zy_SYS_PHONE", phone);
            dic.Add("zy_SYS_BANKCARDNUMBER", bankCard1);
            dic.Add("zy_SYS_BANKCARDNUMBER2", bankCard2);
            return dic;
        }

        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public DataRow GetOrderInfo(string objectId)
        {
            string sql_order = @"
select asset.condition,app.STATE_CDE,app.CITY_CDE ,app.USER_Name USER_CODE,us.name USER_NAME,app.dycs,a.codename MortgageCity,
asset.ASSET_BRAND_DSC,asset.ASSET_MAKE_DSC,asset.POWER_PARAMETER, asset.USAGE7,asset.RELEASE_DTE,asset.ODOMETER_READING,
contract.SALE_PRICE,contract.AMOUNT_FINANCED,contract.LEASE_TERM_IN_MONTH,contract.fp_group_name,contract.financial_product_name,contract.Security_Deposit_Pct,
contract.CASH_DEPOSIT,asset.NEW_PRICE,contract.ACCESSORY_AMT,contract.CALC_SUBSIDY_RTE,app.isgpd,contract.BASE_CUSTOMER_RATE,app.engine_number,app.vin_number,contract.FP_GROUP_ID
from i_application_zy app 
left join i_vehicle_detail_zy asset on app.objectid=asset.parentobjectid
left join i_contract_detail_zy contract on asset.parentobjectid=contract.parentobjectid
left join ot_user us on app.user_name=us.code
left join area a on app.dycs=a.codeid
where app.objectid='{0}'
";
            sql_order = string.Format(sql_order, objectId);
            DataTable dt_order = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_order);

            if (CommonFunction.hasData(dt_order))
            {
                return dt_order.Rows[0]; ;
            }

            return null;
        }

        /// <summary>
        /// 自动审批--获取人员信息
        /// </summary>
        /// <param name="BizObjectId"></param>
        /// <returns></returns>
        public Risk.Parameters_Evaluation.PersonsInfo GetPersonsInfo(string BizObjectId)
        {
            ///关系
            var dic_RelationShip = GetDropDownListSource("RELATIONSHIP", "get_relationship", "RELATIONSHIP_CDE", "RELATIONSHIP_DSC");
            if (!dic_RelationShip.ContainsKey(""))
            {
                dic_RelationShip.Add("", "OneSelf本人");
            }

            List<Risk.Parameters_Evaluation.Person> persons = new List<Risk.Parameters_Evaluation.Person>();
            string ifmortgage = "";

            var areaAll = GetAreaAllCache();

            #region 人员信息
            #region sql
            string sql = @"
select det.parentobjectid,type.identification_code1,type.applicant_type,type.guarantor_type,type.main_applicant,type.is_inactive_ind,
det.GUARANTOR_RELATIONSHIP_CDE,det.FORMER_NAME,det.FIRST_THI_NME,det.ID_CARD_TYP,det.ID_CARD_NBR,det.ID_CARDEXPIRY_DTE,det.SEX,det.AGE_IN_YEAR,det.AGE_IN_MONTH,det.DATE_OF_BIRTH,
det.RACE_CDE,det.NATION_CDE,det.EDUCATION_CDE,edu.enumvalue EDUCATION_NME,det.MARITAL_STATUS_CDE,det.MARITAL_STATUS_CDE MARITAL_STATUS_NME,det.ACTUAL_SALARY,det.LIENEE 
from i_Applicant_Detail_zy det 
left join i_applicant_type_zy type on det.parentobjectid=type.parentobjectid and det.identification_code2=type.identification_code1
left join ot_enumerablemetadata edu on det.education_cde=edu.code and edu.category='教育程度' 
where det.parentobjectid='{0}' 
order by type.identification_code1
";
            #endregion
            sql = string.Format(sql, BizObjectId);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


            foreach (DataRow row in dt.Rows)
            {
                if (row["main_applicant"] + string.Empty == "Y")
                {
                    if (row["LIENEE"] + string.Empty == "1")
                    {
                        ifmortgage = "是";
                    }
                    else
                    {
                        ifmortgage = "否";
                    }
                }

                var phoneNumber = GetPersonPhone(BizObjectId, row["identification_code1"] + string.Empty);

                var addressInfo = GetPersonAddress(BizObjectId, row["identification_code1"] + string.Empty);

                var employeeInfo = GetPersonEmployer(BizObjectId, row["identification_code1"] + string.Empty);

                Risk.Parameters_Evaluation.Person person = new Risk.Parameters_Evaluation.Person();
                person.personnelCategory = GetPersonCategory(row["applicant_type"] + string.Empty, row["guarantor_type"] + string.Empty, row["main_applicant"] + string.Empty);
                person.personnelRelationship = dic_RelationShip[row["GUARANTOR_RELATIONSHIP_CDE"] + string.Empty];
                person.formerName = row["FORMER_NAME"] + string.Empty;
                person.custName = row["FIRST_THI_NME"] + string.Empty;
                person.certType = GetCardType(row["ID_CARD_TYP"] + string.Empty);
                person.certNo = row["ID_CARD_NBR"] + string.Empty;
                person.certEndDate = GetDate(row["ID_CARDEXPIRY_DTE"] + string.Empty);
                person.mobile = phoneNumber + string.Empty;
                person.gender = row["SEX"] + string.Empty == "F" ? "女" : "男";
                person.birthdayYear = row["AGE_IN_YEAR"] + string.Empty;
                person.birthdayMonth = row["AGE_IN_MONTH"] + string.Empty;
                person.custBirthday = GetDate(row["DATE_OF_BIRTH"] + string.Empty);
                person.nationality = row["RACE_CDE"] + string.Empty == "00001" ? "中国 Chinese" : "其他 Other";
                person.nation = row["NATION_CDE"] + string.Empty == "0001" ? "汉族" : "其他";
                person.educationLevel =  row["EDUCATION_NME"] + string.Empty;
                person.maritalStatus = GetMaritalStatus(row["MARITAL_STATUS_NME"] + string.Empty);
                person.houseStatus = GetAddressPropertyType(addressInfo["PROPERTY_TYPE_CDE"] + string.Empty);
                person.employer = employeeInfo["NAME_2"] + string.Empty;
                person.employerTelphone = employeeInfo["PHONE"] + string.Empty;
                person.employerAddressProvince = areaAll[employeeInfo["STATE_CDE2"] + string.Empty];
                person.employerAddressCity = areaAll[employeeInfo["CITY_CDE6"] + string.Empty];
                person.employerAddress = employeeInfo["ADDRESS_ONE_2"] + string.Empty;
                person.occupation = employeeInfo["DESIGNATION_NME"] + string.Empty;
                person.workingYears = employeeInfo["TIME_IN_YEAR_2"] + string.Empty;
                person.incomeMonth = row["ACTUAL_SALARY"] + string.Empty;
                person.birthPalaceProvince = addressInfo["BIRTHPLEASE_PROVINCE"] + string.Empty;
                person.nativeDistrict = addressInfo["NATIVE_DISTRICT"] + string.Empty;
                person.domicilePlace = areaAll[addressInfo["state_cde4"] + string.Empty] + areaAll[addressInfo["city_cde4"] + string.Empty];//省+市
                person.currentAddressProvince = areaAll[addressInfo["currentstate"] + string.Empty];
                person.currentAddressCity = areaAll[addressInfo["currentcity"] + string.Empty];
                person.currentLivingAddress = addressInfo["UNIT_NO"] + string.Empty;
                person.isInvokePboc = row["IS_INACTIVE_IND"] + string.Empty == "T" ? "0" : "1";
                person.lienee = row["LIENEE"] + string.Empty;
                persons.Add(person);
            }
            #endregion
            return new Risk.Parameters_Evaluation.PersonsInfo() { Persons = persons, Ifmortgage = ifmortgage };
        }


        /// <summary>
        /// 正源终审个人信息
        /// </summary>
        public void ZyFinalAuditModel(string objectId, ZYBaseInfo zYBase, ZYPersonalInfo zYPersonal, ZYComponyInfo zYCompony, ZYSpouseInfo zYSpouse, InstanceData data)
        {
            string sql = @"
select det.parentobjectid,type.identification_code1,type.applicant_type,type.guarantor_type,type.main_applicant,type.is_inactive_ind,
det.GUARANTOR_RELATIONSHIP_CDE,det.FORMER_NAME,det.FIRST_THI_NME,det.ID_CARD_TYP,det.ID_CARD_NBR,det.ID_CARDEXPIRY_DTE,det.SEX,det.AGE_IN_YEAR,det.AGE_IN_MONTH,det.DATE_OF_BIRTH,
det.RACE_CDE,det.NATION_CDE,det.EDUCATION_CDE,edu.enumvalue EDUCATION_NME,det.MARITAL_STATUS_CDE ,det.ACTUAL_SALARY,det.LIENEE ,det.EMPLOYMENT_TYPE_CDE,det.EMAIL_ADDRESS from i_Applicant_Detail_zy det 
left join i_applicant_type_zy type on det.parentobjectid=type.parentobjectid and det.identification_code2=type.identification_code1
left join ot_enumerablemetadata edu on det.education_cde=edu.code and edu.category='教育程度' 
where det.parentobjectid='{0}' and type.main_applicant='Y'
order by type.identification_code1
";
            sql = string.Format(sql, objectId);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


            var areaAll = GetAreaAllCache();

            //有且只有一个主借人
            foreach (DataRow row in dt.Rows)
            {

                var personCategory = GetPersonCategory(row["applicant_type"] + string.Empty, row["guarantor_type"] + string.Empty, row["main_applicant"] + string.Empty);


                if (personCategory != "A")
                {
                    continue;
                }

                var phoneNumber = GetPersonPhone(objectId, row["identification_code1"] + string.Empty);

                var addressInfo = GetPersonAddress(objectId, row["identification_code1"] + string.Empty);

                var employeeInfo = GetPersonEmployer(objectId, row["identification_code1"] + string.Empty);

                var personalReference = GetPersonalReference(objectId, row["identification_code1"] + string.Empty);

                //1.获取职业身份
                zYBase.occupationalIdentity = row["EMPLOYMENT_TYPE_CDE"] + string.Empty; //GetEmployeementType(row["EMPLOYMENT_TYPE_CDE"] + string.Empty);

                //2.personalInfo 个人信息 
                zYPersonal.name = row["FIRST_THI_NME"] + string.Empty;
                zYPersonal.email = row["EMAIL_ADDRESS"] + string.Empty; ;
                zYPersonal.sex = row["SEX"] + string.Empty == "F" ? "2" : "1";
                zYPersonal.birthday = GetDate(row["DATE_OF_BIRTH"] + string.Empty);
                zYPersonal.age = row["AGE_IN_YEAR"] + string.Empty;
                zYPersonal.idNo = row["ID_CARD_NBR"] + string.Empty;
                zYPersonal.curProvice = areaAll[addressInfo["currentstate"] + string.Empty];
                zYPersonal.curCity = areaAll[addressInfo["currentcity"] + string.Empty];
                zYPersonal.curCounty = areaAll[addressInfo["currentarea"] + string.Empty];
                zYPersonal.curAddress = zYPersonal.curProvice + zYPersonal.curCity + zYPersonal.curCounty + addressInfo["UNIT_NO"] + string.Empty;
                zYPersonal.perProvice = areaAll[addressInfo["state_cde4"] + string.Empty];
                zYPersonal.perCity = areaAll[addressInfo["city_cde4"] + string.Empty];
                zYPersonal.perCounty = areaAll[addressInfo["area4"] + string.Empty];
                zYPersonal.perAddr = zYPersonal.perProvice + zYPersonal.perCity + zYPersonal.perCounty + addressInfo["ADDRESS_ID"] + string.Empty;
                zYPersonal.ageLimi = ((DateTime.Now - Convert.ToDateTime(addressInfo["LIVING_FROM_DTE"])).Days / 365).ToString();
                zYPersonal.mariStatus = row["MARITAL_STATUS_CDE"] + string.Empty; //GetMaritalStatus(row["MARITAL_STATUS_CDE"].ToString());
                zYPersonal.houseType = addressInfo["PROPERTY_TYPE_CDE"] + string.Empty; //GetAddressPropertyType(addressInfo["PROPERTY_TYPE_CDE"] + string.Empty);
                zYPersonal.mobile = data["zy_SYS_PHONE"].Value + string.Empty;

                //3.公司信息
                zYCompony.componyName = employeeInfo["NAME_2"];
                zYCompony.comProvice = areaAll[employeeInfo["STATE_CDE2"] + string.Empty];
                zYCompony.comCity = areaAll[employeeInfo["CITY_CDE6"] + string.Empty];
                zYCompony.comCounty = areaAll[employeeInfo["AREA"] + string.Empty];
                zYCompony.comAddrDetail = zYCompony.comProvice + zYCompony.comCity + zYCompony.comCounty + employeeInfo["ADDRESS_ONE_2"] + string.Empty;
                zYCompony.componyPhone = employeeInfo["PHONE"] + string.Empty;
                zYCompony.position = employeeInfo["DESIGNATION_CDE_Detail"] + string.Empty; //GetDesignation(employeeInfo["DESIGNATION_CDE_Detail"] + string.Empty);

                //4.配偶、直系情书
                if (row["MARITAL_STATUS_CDE"].ToString() == "1" || row["MARITAL_STATUS_CDE"].ToString() == "2")
                {
                    zYSpouse.name = data["MATE_NAME"].Value + string.Empty;
                    zYSpouse.phone = data["MATE_MOBILE"].Value + string.Empty;
                    zYSpouse.relation = data["MATE_RELATIONSHIP_CDE"].Value + string.Empty;
                }
                else
                {
                    zYSpouse.name = personalReference["NAME10"];
                    zYSpouse.phone = personalReference["MOBILE"];
                    zYSpouse.relation = personalReference["RELATIONSHIP_CDE"];
                }
            }

        }

        /// <summary>
        /// 获取区域
        /// </summary>
        /// <param name="codeId"></param>
        /// <returns></returns>
        public Dictionary<string,string> GetAreaAll()
        {
            var dic = new Dictionary<string, string>();

            string sql = @"select * from area ";


            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["CodeId"] + string.Empty == "")
                        continue;
                    if (dic.ContainsKey(row["CodeId"] + string.Empty))
                        continue;
                    dic.Add(row["CodeId"] + string.Empty, row["CodeName"] + string.Empty);
                }
            }

            return dic;
        }

        /// <summary>
        /// 获取人员手机号
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        public string GetPersonPhone(string bizObjectId, string identificationCode)
        {

            var phoneSql = @"select tel.PHONE_NUMBER from i_Applicant_Detail_zy det 
            left join i_applicant_phone_fax_zy tel on det.parentobjectid=tel.parentobjectid  and tel.phone_type_cde='00003' and tel.identification_code5 = det.identification_code2 where det.parentobjectid='{0}' and det.identification_code2='{1}'  ";
            phoneSql = string.Format(phoneSql, bizObjectId, identificationCode);
            DataTable phoneDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(phoneSql);

            var phoneNumber = string.Empty;
            if (CommonFunction.hasData(phoneDt))
            {
                phoneNumber = phoneDt.Rows[0][0].ToString();
            }

            return phoneNumber;
        }

        /// <summary>
        /// 获取人员地址
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPersonAddress(string bizObjectId, string identificationCode)
        {
            var addressSql = @"select  ads.PROPERTY_TYPE_CDE,ads.BIRTHPLEASE_PROVINCE,ads.NATIVE_DISTRICT,ads.UNIT_NO,ads.state_cde4,ads.city_cde4,ads.currentstate,ads.currentcity,ads.AREA4,ads.CurrentArea,ads.ADDRESS_ID,ads.LIVING_FROM_DTE from i_Applicant_Detail_zy det 
              left join i_address_zy ads on ads.parentobjectid=det.parentobjectid and ads.identification_code4=det.identification_code2 where  det.parentobjectid='{0}' and det.identification_code2='{1}' ";

            addressSql = string.Format(addressSql, bizObjectId, identificationCode);
            DataTable addressDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(addressSql);

            var addressInfo = new Dictionary<string, string>();

            if (CommonFunction.hasData(addressDt))
            {
                addressInfo.Add("PROPERTY_TYPE_CDE", addressDt.Rows[0][0].ToString());
                addressInfo.Add("BIRTHPLEASE_PROVINCE", addressDt.Rows[0][1].ToString());
                addressInfo.Add("NATIVE_DISTRICT", addressDt.Rows[0][2].ToString());
                addressInfo.Add("UNIT_NO", addressDt.Rows[0][3].ToString());
                addressInfo.Add("state_cde4", addressDt.Rows[0][4].ToString());
                addressInfo.Add("city_cde4", addressDt.Rows[0][5].ToString());
                addressInfo.Add("currentstate", addressDt.Rows[0][6].ToString());
                addressInfo.Add("currentcity", addressDt.Rows[0][7].ToString());
                addressInfo.Add("area4", addressDt.Rows[0][8].ToString());
                addressInfo.Add("currentarea", addressDt.Rows[0][9].ToString());
                addressInfo.Add("ADDRESS_ID", addressDt.Rows[0][10].ToString());
                addressInfo.Add("LIVING_FROM_DTE", addressDt.Rows[0][11].ToString());
            }

            return addressInfo;
        }

        /// <summary>
        /// 获取工作信息
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPersonEmployer(string bizObjectId, string identificationCode)
        {
            var sql = @"select emp.NAME_2,emp.ADDRESS_ONE_2,emp.PHONE,emp.STATE_CDE2,emp.CITY_CDE6,emp.DESIGNATION_CDE,jobs.enumvalue  DESIGNATION_NME,emp.TIME_IN_YEAR_2,det.DESIGNATION_CDE DESIGNATION_CDE_Detail,emp.AREA from i_Applicant_Detail_zy det 
              left join i_employer_zy emp on emp.parentobjectid=det.parentobjectid and emp.identification_code6=det.identification_code2 
              left join ot_enumerablemetadata jobs on emp.DESIGNATION_CDE=jobs.code and jobs.category='职位' where  det.parentobjectid='{0}' and det.identification_code2='{1}' ";

            sql = string.Format(sql, bizObjectId, identificationCode);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var employeeInfo = new Dictionary<string, string>();

            if (CommonFunction.hasData(dt))
            {
                employeeInfo.Add("NAME_2", dt.Rows[0][0].ToString());
                employeeInfo.Add("ADDRESS_ONE_2", dt.Rows[0][1].ToString());
                employeeInfo.Add("PHONE", dt.Rows[0][2].ToString());
                employeeInfo.Add("STATE_CDE2", dt.Rows[0][3].ToString());
                employeeInfo.Add("CITY_CDE6", dt.Rows[0][4].ToString());
                employeeInfo.Add("DESIGNATION_CDE", dt.Rows[0][5].ToString());
                employeeInfo.Add("DESIGNATION_NME", dt.Rows[0][6].ToString());
                employeeInfo.Add("TIME_IN_YEAR_2", dt.Rows[0][7].ToString());
                employeeInfo.Add("DESIGNATION_CDE_Detail", dt.Rows[0][8].ToString());
                employeeInfo.Add("AREA", dt.Rows[0][9].ToString());
            }

            return employeeInfo;
        }


        /// <summary>
        /// 获取联系人
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPersonalReference(string bizObjectId, string identificationCode)
        {
            var personal = new Dictionary<string, string>();

            var Sql = @"select per.NAME10,per.MOBILE,per.RELATIONSHIP_CDE from i_Applicant_Detail_zy det 
            left join i_personnal_reference_zy per on det.parentobjectid=per.parentobjectid and per.identification_code10 = det.identification_code2 where det.parentobjectid='{0}' and det.identification_code2='{1}'  ";

            Sql = string.Format(Sql, bizObjectId, identificationCode);

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(Sql);
            
            if (CommonFunction.hasData(dt))
            {
                personal.Add("NAME10", dt.Rows[0][0].ToString());
                personal.Add("MOBILE", dt.Rows[0][1].ToString());
                personal.Add("RELATIONSHIP_CDE", dt.Rows[0][2].ToString());
            }

            return personal;
        }

        /// <summary>
        /// 根据绑定流水号 transactionNo 获取instanceId
        /// </summary>
        /// <returns></returns>
        public string GetInstanceIdByTransactionNo(string transactionNo,string traceNo)
        {
            var instanceId = string.Empty;

            var sql = "select ot.objectid from h3.ot_instancecontext ot join h3.i_application_zy ap on ot.bizobjectid = ap.objectid where ap.zy_transactionNo = '" + transactionNo + "'  and ap.zy_traceNo like '%" + traceNo + "%' ";

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                instanceId =dt.Rows[0][0].ToString();
            }

            return instanceId;
        }

        /// <summary>
        /// 根据 ApplicationNumber 获取instanceId
        /// </summary>
        /// <returns></returns>
        public string GetInstanceIdByApplicationNumber(string applicationNumber, string traceNo)
        {
            var instanceId = string.Empty;

            var sql = "select ot.objectid from h3.ot_instancecontext ot join h3.i_application_zy ap on ot.bizobjectid = ap.objectid where ap.application_number = '" + applicationNumber + "'";

            if (!string.IsNullOrEmpty(traceNo))
            {
                sql += "  and ap.zy_traceNo like '%" + traceNo + "%'";
            }

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                instanceId = dt.Rows[0][0].ToString();
            }

            return instanceId;
        }

        /// <summary>
        /// 获取附件属性
        /// </summary>
        /// <param name="bizobjectId"></param>
        /// <param name="datafiled"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAttachmentProperty(string bizobjectId,string datafiled)
        {
            var attachmentProperty = new Dictionary<string, string>();

            string sql = " select objectid,bizobjectschemacode,bizobjectid from Ot_Attachment where datafield ='" + datafiled + "' and bizobjectid ='" + bizobjectId + "'";


            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                attachmentProperty.Add("objectid", dt.Rows[0][0].ToString());
                attachmentProperty.Add("bizobjectschemacode", dt.Rows[0][1].ToString());
                attachmentProperty.Add("bizobjectid", dt.Rows[0][2].ToString());
            }
            return attachmentProperty;
        }


        /// <summary>
        /// 获取正源映射银行编码
        /// </summary>
        public List<ZyBankCodesModel> GetZyBankCode()
        {
            var bankCodes = new List<ZyBankCodesModel>();

            string sql = @"select * from H3.C_ZYBANKCODES";


            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    bankCodes.Add(new ZyBankCodesModel { code = row["code"] + string.Empty, name = row["name"] + string.Empty });
                }
            }

            return bankCodes;
        }


        /// <summary>
        /// 查询风控报告Id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFkReportId(string instanceId)
        {
            //var sql = "select ap.FK_Report,ap.FK_RECORDID from h3.ot_instancecontext ot join h3.i_application ap on ot.bizobjectid = ap.objectid where ot.objectid = '"+ instanceId + "' ";
            string sql = "select ap.FK_Report,ap.FK_RECORDID from h3.ot_instancecontext ot join h3.i_application_zy ap on ot.bizobjectid = ap.objectid where ot.objectid ='" + instanceId + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;
            var recordId = string.Empty;

            var dic = new Dictionary<string, string>();

            if (dt.Rows.Count > 0)
            {
                report = dt.Rows[0][0].ToString();
                recordId = dt.Rows[0][1].ToString();
            }
            dic.Add("report", report);
            dic.Add("recordId", recordId);
            return dic;
        }

        /// <summary>
        /// 查询风控报告
        /// </summary>
        /// <returns></returns>
        public int SaveFkReport(string fkReport, string instanceId, string sourceType, string recordId)
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            //string sql = "update H3.i_application set FK_REPORT='" + fkReport + "' where objectid ='" + instanceContext.BizObjectId + "'";

            var i = 0;

            string sql = "insert into H3.C_APPLICATION_CLOB(fk_report,objectid,sourcetype,recordid) values(:content,'" + instanceContext.BizObjectId + "','" + sourceType + "','" + recordId + "')";
            try
            {
                string connectionCode = "Engine";
                var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                OracleConnection connection = new OracleConnection(dbObject.DbConnectionString);
                connection.Open();
                OracleCommand Cmd = new OracleCommand(sql, connection);
                OracleParameter Temp = new OracleParameter("content", OracleType.NClob);
                Temp.Direction = ParameterDirection.Input;
                Temp.Value = fkReport;
                Cmd.Parameters.Add(Temp);
                i = Cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("保存风控报告数据异常：" + ex.ToString());
            }
            return i;

        }


        /// <summary>
        /// 获取风控报告
        /// </summary>
        /// <returns></returns>
        public string GetFkReport(string instanceId, string sourceType, string recordId = "")
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            string sql = "select fk_report from h3.C_APPLICATION_CLOB where sourcetype='" + sourceType + "' and  objectid ='" + instanceContext.BizObjectId + "'";

            if (!string.IsNullOrEmpty(recordId))
            {
                sql += " and recordid='" + recordId + "'";
            }

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;

            if (dt.Rows.Count > 0)
            {
                report = dt.Rows[0][0].ToString();
            }
            return report;
        }


        #endregion


        #region Common Method

        /// <summary>
        /// 获取身份名称
        /// </summary>
        /// <param name="dicKey"></param>
        /// <returns></returns>
        public string GetProvideName(string dicKey)
        {
            var provideName = string.Empty;

            var provides = GetDropDownListSource("PROVINCE", "get_state_code", "STATE_CDE", "STATE_NME");

            if (!provides.Any())
            {
                return provideName;
            }

            try
            {
                if (provides.ContainsKey(dicKey))
                {
                    provideName = provides[dicKey].ToString();
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CMS映射省份错误,dicKey=" + dicKey + "-->" + ex.ToString());
            }
            return provideName;
        }

        /// <summary>
        /// 获取城市
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public string GetCityName(string stateCode, string cityCode)
        {
            var cityName = string.Empty;

            Dictionary<string, object> dealer_Para = new Dictionary<string, object>();
            dealer_Para.Add("state_cde", stateCode);
            var dic_Dealer_City = GetDropDownListSource("PROVINCE_" + stateCode, "get_city_all", dealer_Para, "CITY_CDE", "CITY_NME");

            if (!dic_Dealer_City.Any())
            {
                return cityName;
            }

            try
            {
                if (dic_Dealer_City.ContainsKey(cityCode))
                {
                    cityName = dic_Dealer_City[cityCode].ToString();
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CMS映射DealerCity错误,stateCode=" + stateCode + ",cityCode=" + cityCode + "-->" + ex.ToString());
            }
            return cityName;
        }

        /// <summary>
        /// 获取映射城市
        /// </summary>
        /// <returns></returns>
        public string GetMappingCity(string dicName)
        {
            try
            {
                ///获取Mapping的城市；
                var mapping_Cities = GetMappingCities();

                if (mapping_Cities.ContainsKey(dicName))
                {
                    return mapping_Cities[dicName].ToString();
                }
                else
                {
                    return dicName;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CMS映射城市错误,dicName=" + dicName + "-->" + ex.ToString());

                return string.Empty;
            }
        }

        public Dictionary<string, string> GetMappingCities()
        {
            if (Cache.GetCache("MAPPINGCITIES") == null)
            {
                Proposal.ProposalController p = new Proposal.ProposalController();
                var r = p.GetAllMappingCities();

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(r.Data));
                var rtn = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(result["Data"]));
                Cache.SetCache("MAPPINGCITIES", rtn);
                return rtn;
            }
            else
            {
                return (Dictionary<string, string>)Cache.GetCache("MAPPINGCITIES");
            }
        }

        public Dictionary<string, string> GetDropDownListSource(string DicKey, string ServerFunction, string DDLCode, string DDLText)
        {
            if (Cache.GetCache(DicKey) == null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var r = BizService.ExecuteBizNonQuery("DropdownListDataSource", ServerFunction, null);
                OThinker.H3.BizBus.BizService.BizStructure[] result = (OThinker.H3.BizBus.BizService.BizStructure[])r["List"];
                foreach (var biz in result)
                {
                    dic.Add(biz[DDLCode] + string.Empty, biz[DDLText] + string.Empty);
                }
                Cache.SetCache(DicKey, dic);
                return dic;
            }
            else
            {
                var r = (Dictionary<string, string>)Cache.GetCache(DicKey);
                return r;
            }
        }

        public Dictionary<string, string> GetDropDownListSource(string DicKey, string ServerFunction, Dictionary<string, object> Parameters, string DDLCode, string DDLText)
        {
            if (Cache.GetCache(DicKey) == null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var r = BizService.ExecuteBizNonQuery("DropdownListDataSource", ServerFunction, Parameters);
                OThinker.H3.BizBus.BizService.BizStructure[] result = (OThinker.H3.BizBus.BizService.BizStructure[])r["List"];
                foreach (var biz in result)
                {
                    dic.Add(biz[DDLCode] + string.Empty, biz[DDLText] + string.Empty);
                }
                Cache.SetCache(DicKey, dic);
                return dic;
            }
            else
            {
                var r = (Dictionary<string, string>)Cache.GetCache(DicKey);
                return r;
            }
        }

        public Dictionary<string, string> GetAreaAllCache()
        {
            if (Cache.GetCache("AreaAll") == null)
            {
                var rtn = GetAreaAll();

                Cache.SetCache("AreaAll", rtn);
                return rtn;
            }
            else
            {
                return (Dictionary<string, string>)Cache.GetCache("AreaAll");
            }
        }

        /// <summary>
        /// 人员类型-主借人、共借人、担保人
        /// </summary>
        /// <param name="app_type"></param>
        /// <param name="gur_type"></param>
        /// <param name="main_type"></param>
        /// <returns></returns>
        public string GetPersonCategory(string app_type, string gur_type, string main_type)
        {
            if (main_type == "Y")
                return "A";
            else if (app_type == "I")
                return "B";
            else return "C";
        }


        public string GetCardType(string type)
        {
            switch (type)
            {
                case "00001": return "0";
                case "00002": return "1";
                case "00003": return "2";
                case "00004": return "3";
                case "00005": return "5";
                case "00006": return "0";
                case "00007": return "6";
            }
            return "";
        }
        public string GetDate(string Date)
        {
            if (Date == "")
                return "";
            string d = Convert.ToDateTime(Date).ToString("yyyy-MM-dd");
            if (d == "1753-01-01")
                return "";
            return d;
        }

        /// <summary>
        /// 主借人房产类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetAddressPropertyType(string type)
        {
            switch (type)
            {
                case "1": return "商业按揭购房";
                case "2": return "无按揭购房";
                case "3": return "公积金按揭购房";
                case "4": return "自建房";
                case "5": return "租用";
                case "6": return "暂住";
                case "7": return "亲属住房";
                case "8": return "单位住房";
            }
            return "";
        }

        /// <summary>
        /// 主借人婚姻状况
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetMaritalStatus(string type)
        {
            switch (type)
            {
                case "0": return "未婚";
                case "1": return "已婚无子女";
                case "2": return "已婚有子女";
                case "3": return "其他（离婚、丧偶、再婚）";
            }
            return "";
        }

        /// <summary>
        /// 主借人职位
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDesignation(string type)
        {
            switch (type)
            {
                case "1": return "厅局级及以上";
                case "2": return "处级";
                case "3": return "科级";
                case "4": return "一般干部";
                case "5": return "其他";
                case "6": return "总经理级";
                case "7": return "部门经理级";
                case "8": return "职员";
            }
            return "";
        }

        public string GetEmployeementType(string type)
        {
            switch (type)
            {
                case "0": return "企业股东";
                case "1": return "受薪人士";
                case "2": return "自雇人士";
            }
            return "";
        }

        /// <summary>
        /// 获取经销商类型：内网还是外网
        /// </summary>
        /// <param name="serCode"></param>
        /// <returns></returns>
        public string getFIType(string UserCode)
        {
            if (UserCode.IndexOf("98") == 0 || UserCode.IndexOf("80000") == 0)
                return "内网";
            else
                return "外网";
        }


        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="schemaCode">流程Id</param>
        /// <param name="bizObjectId"></param>
        /// <param name="dataField">字段名称</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="content"></param>
        public void SaveAttachment(string schemaCode, string bizObjectId, string dataField, string contentType, string fileType, byte[] content)
        {
            Attachment attachment = new Attachment();
            attachment.BizObjectSchemaCode = schemaCode;
            attachment.BizObjectId = bizObjectId;
            attachment.DataField = dataField;
            attachment.Content = content;
            attachment.CreatedBy = "";
            attachment.CreatedTime = DateTime.Now;
            attachment.ContentLength = content.Length;
            attachment.ContentType = contentType;
            attachment.LastVersion = true;
            attachment.FileName = bizObjectId + "." + fileType;
            attachment.FileFlag = 0;
            AppUtility.Engine.BizObjectManager.AddAttachment(attachment);
        }

        #endregion
    }
}