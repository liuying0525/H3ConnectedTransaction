using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using OThinker.H3.WorkflowTemplate;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Web.Http;
using System.Security.Cryptography;
using System.Web.Security;
using System.IO;
using OThinker.H3;
using OThinker.H3.WorkItem;
using DongZheng.H3.WebApi.Common;

namespace DongZheng.H3.WebApi.Controllers
{
    /// <summary>
    /// BPM服务接口
    /// </summary>
    [ValidateInput(false)]
    [AllowAnonymous,Xss]
    public class BPMApiController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (this._Engine == null)
                {
                    OThinker.H3.Connection conn = new Connection();
                    conn.Open(ConfigurationManager.AppSettings["BPMEngine"]);
                    _Engine = conn.Engine;
                }
                return _Engine;
            }
        }

        private System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        /// <summary>
        /// 获取JOSN序列化对象
        /// </summary>
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (jsSerializer == null)
                {
                    jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return jsSerializer;
            }
        }

        public System.Web.Mvc.ActionResult Index()
        {
            return Json("BPM接口服务.", JsonRequestBehavior.AllowGet);
        }

        #region 新的服务 ----------------------------------

        [HttpPost]
        public System.Web.Mvc.ActionResult StartWorkflow(string USER_CODE, string WORKFLOW_CODE, string FINISH_START, string INSTANCE_ID, string PARAM_VALUES)
        {
            bool finishStart = FINISH_START == "1" ? true : false;
            BPM bpm = new BPM();
            var result = bpm.StartWorkflow_Base(USER_CODE, WORKFLOW_CODE, finishStart, INSTANCE_ID, PARAM_VALUES);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateAttachmentBizId(string NewBizObjectId, List<BPM.ImgFile> imgFiles)
        {
            rtn_data rtn = new rtn_data();
            if (string.IsNullOrEmpty(NewBizObjectId))
            {
                rtn.code = -1;
                rtn.message = "BizObjectId参数为空";
                return Json(rtn);
            }
            if (imgFiles == null)
            {
                rtn.code = -1;
                rtn.message = "imgFiles参数为空";
                return Json(rtn);
            }
            string sql = "Update ot_attachment set bizobjectid='" + NewBizObjectId + "' where objectid='{0}'";
            foreach (var file in imgFiles)
            {
                this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format(sql, file.id));
            }
            rtn.code = 1;
            rtn.message = "成功";
            this.Engine.LogWriter.Write("UpdateAttachmentBizId-->" + Newtonsoft.Json.JsonConvert.SerializeObject(rtn));
            return Json(rtn);
        }



        /// <summary>
        /// 取消指定的流程
        /// </summary>
        /// <param name="INSTANCEID">流程实例ID</param>
        /// <param name="EXTERNAL_SYSTEM">外部系统编码</param>
        /// <returns></returns>
        //(Description = "取消指定的流程")]
        [HttpPost]
        public System.Web.Mvc.ActionResult CancelInstance(string INSTANCEID, string EXTERNAL_SYSTEM)
        {
            BPM.RestfulResultCancleInstance result = new BPM.RestfulResultCancleInstance();
            try
            {
                #region 判断INSTANCEID 是否正常
                InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(INSTANCEID);
                if (ic == null)
                {
                    result.STATUS = "0";
                    result.MESSAGE = "流程取消失败";
                    result.DATA = "取消流程异常，INSTANCEID不存在" + INSTANCEID;
                    this.Engine.LogWriter.Write(" 取消流程服务，方法：CancelInstance，异常，流程ID不存在，INSTANCEID=" + INSTANCEID);
                    return Json(result);
                }
                else if (ic.IsFinished || ic.IsCanceled)
                {
                    result.STATUS = "1";
                    result.MESSAGE = "流程已经结束或者已经取消。";
                    result.DATA = INSTANCEID;
                    this.Engine.LogWriter.Write(" 取消流程服务，方法：CancelInstance，异常，流程已经结束或者已经取消，INSTANCEID=" + INSTANCEID);
                    return Json(result);
                }

                #endregion
                OThinker.H3.Messages.CancelInstanceMessage cancelMessage
                = new CancelInstanceMessage(MessageEmergencyType.Normal, INSTANCEID, false);
                this.Engine.InstanceManager.SendMessage(cancelMessage);
                result.STATUS = "1";
                result.MESSAGE = "流程取消成功";
                result.DATA = INSTANCEID;
            }
            catch (Exception ex)
            {
                result.STATUS = "0";
                result.MESSAGE = "取消流程异常" + ex;
                result.DATA = INSTANCEID;
                this.Engine.LogWriter.Write(" 取消流程服务，方法：CancelInstance，异常" + ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取流程相关信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode"></param>
        /// <param name="workItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult GetInstanceInfo(string appId, string pwd, string userCode, string workItemId)
        {
            OThinker.H3.WorkItem.WorkItem workItem = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (workItem == null) throw new Exception("工作任务不存在");
            OThinker.H3.Instance.InstanceContext instance = this.Engine.InstanceManager.GetInstanceContext(workItem.InstanceId);

            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate template = this.Engine.WorkflowManager.GetPublishedTemplate(instance.WorkflowCode, instance.WorkflowVersion);
            OThinker.H3.WorkflowTemplate.Activity activity = template.GetActivityByCode(workItem.ActivityCode);

            InstanceInfo result = new InstanceInfo()
            {
                ActivityCode = activity.ActivityCode,
                ActivityName = activity.DisplayName,
                DataPermissions = (activity is OThinker.H3.WorkflowTemplate.ParticipativeActivity) ? ((OThinker.H3.WorkflowTemplate.ParticipativeActivity)activity).DataPermissions : null,
                InstanceId = instance.InstanceId,
                InstanceState = instance.State,
                Originator = instance.Originator,
                TokenId = workItem.TokenId,
                WorkflowCode = template.WorkflowCode,
                WorkflowName = template.WorkflowFullName,
                SequenceNo = instance.SequenceNo,
                WorkItemState = workItem.State
            };

            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user != null)
            {
                OThinker.Organization.Signature[] signatures = this.Engine.Organization.GetSignaturesByUnit(user.ObjectID);
                result.MySignatures = new List<SignatureParam>();
                if (signatures != null)
                {
                    foreach (OThinker.Organization.Signature signature in signatures)
                    {
                        result.MySignatures.Add(new SignatureParam()
                        {
                            IsDefault = signature.IsDefault,
                            SignatureName = signature.Name,
                            SignautreId = signature.ObjectID,
                            SortKey = signature.SortKey
                        });
                    }
                }

                result.FrequentlyComment = this.Engine.Organization.GetFrequentlyUsedCommentTextsByUser(user.ObjectID);

                if (activity is OThinker.H3.WorkflowTemplate.ParticipativeActivity)
                {
                    // PermittedActions actions = new PermittedActions();
                    OThinker.H3.WorkflowTemplate.ParticipativeActivity participative = activity as OThinker.H3.WorkflowTemplate.ParticipativeActivity;
                    //actions.AdjustParticipant = participative.PermittedActions.AdjustParticipant;
                    //actions.Assist = participative.PermittedActions.Assist;
                    //actions.CancelIfUnfinished = participative.PermittedActions.CancelIfUnfinished;
                    //actions.Choose = participative.PermittedActions.Choose;
                    //actions.Circulate = participative.PermittedActions.Circulate;
                    //actions.Consult = participative.PermittedActions.Consult;
                    //actions.Forward = participative.PermittedActions.Forward;
                    //actions.Reject = participative.PermittedActions.Reject || participative.PermittedActions.RejectToAny || participative.PermittedActions.RejectToFixed;

                    if (participative.PermittedActions.RejectToAny)
                    {// 获取允许驳回的节点
                        List<ActivityParam> rejectActivies = new List<ActivityParam>();
                        foreach (OThinker.H3.Instance.Token token in instance.Tokens)
                        {
                            if (token.Activity == activity.ActivityCode) continue;
                            ParticipativeActivity act = template.GetActivityByCode(token.Activity) as ParticipativeActivity;
                            if (act == null) continue;
                            rejectActivies.Add(new ActivityParam()
                            {
                                ActivityCode = act.ActivityCode,
                                DisplayName = act.DisplayName
                            });
                        }
                        result.RejectActivies = rejectActivies;
                    }

                    result.PermittedActions = participative.PermittedActions;
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 提交工作任务
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode"></param>
        /// <param name="workItemId"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitWorkItem(string userCode, string workItemId, string paramValues)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (item == null) throw new Exception("工作任务不存在");
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user == null) throw new Exception("用户不存在");
            List<DataItemParam> listParamValues = JSSerializer.Deserialize<List<DataItemParam>>(paramValues);
            // 保存数据项
            SaveBizObject(item, user, OThinker.Data.BoolMatchValue.True, listParamValues);

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                user.UnitID,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                null,
                OThinker.Data.BoolMatchValue.True,
                string.Empty,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                11);

            // 需要通知实例事件管理器结束事件
            AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.True,
                    false,
                    OThinker.Data.BoolMatchValue.True,
                    true,
                    null);
            this.Engine.InstanceManager.SendMessage(endMessage);
            return Json(new
            {
                Result = true,
                Message = string.Empty
            });
        }

        /// <summary>
        /// 驳回工作任务
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode">用户账号</param>
        /// <param name="workItemId">工作任务ID</param>
        /// <param name="rejectToActivity">要驳回的指定节点编码，如果为空则由系统按照流程模板设置处理</param>
        /// <param name="paramValues">驳回时的表单参数</param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult RejectWorkItem(string appId, string pwd, string userCode, string workItemId, string rejectToActivity, string paramValues)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (item == null) throw new Exception("工作任务不存在");
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user == null) throw new Exception("用户不存在");
            List<DataItemParam> listParamValues = JSSerializer.Deserialize<List<DataItemParam>>(paramValues);
            // 保存数据项
            SaveBizObject(item, user, OThinker.Data.BoolMatchValue.True, listParamValues);

            // 获取驳回节点
            if (string.IsNullOrEmpty(rejectToActivity))
            {// 目标节点为空，则判断当前节点允许驳回的环节 
                PublishedWorkflowTemplate template = this.Engine.WorkflowManager.GetPublishedTemplate(item.WorkflowCode, item.WorkflowVersion);
                ParticipativeActivity activity = template.GetActivityByCode(item.ActivityCode) as ParticipativeActivity;
                if (activity == null) throw new Exception("当前环节不允许驳回");
                if (activity.PermittedActions.RejectToStart) rejectToActivity = template.StartActivityCode; // 驳回到开始
                else if (activity.PermittedActions.RejectToFixed || activity.PermittedActions.RejectToAny)
                {
                    if (activity.PermittedActions.RejectToActivityCodes != null && activity.PermittedActions.RejectToActivityCodes.Length > 0)
                    {
                        rejectToActivity = activity.PermittedActions.RejectToActivityCodes[0];
                    }
                }
                else if (activity.PermittedActions.Reject)
                {// 驳回上一步 
                    InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
                    int[] tokens = context.GetToken(item.TokenId).PreTokens;
                    if (tokens != null && tokens.Length > 0)
                    {
                        rejectToActivity = context.GetToken(tokens[0]).Activity;
                    }
                }

                if (string.IsNullOrEmpty(rejectToActivity))
                {
                    rejectToActivity = template.StartActivityCode;
                }
            }

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                  item.ObjectID,
                  user.ObjectID,
                  AccessPoint.ExternalSystem,
                  null,
                  OThinker.Data.BoolMatchValue.False,
                  string.Empty,
                  null,
                  ActionEventType.Backward,
                  10);
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                rejectToActivity,
                OThinker.H3.Instance.Token.UnspecifiedID,
                null,
                new int[] { item.TokenId },
                false,
                ActionEventType.Backward);

            // 通知该Activity已经完成
            OThinker.H3.Messages.AsyncEndMessage endMessage =
                new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.False,
                    true,
                    OThinker.Data.BoolMatchValue.False,
                    false,
                    activateMessage);
            this.Engine.InstanceManager.SendMessage(endMessage);
            return Json(new
            {
                Result = true,
                Message = string.Empty
            });
        }
        #endregion

        #region  Token验证相关方法
        public String GetSecretBySystemcode(String targetCode)
        {
            var targetsystem = OThinker.H3.Controllers.AppUtility.Engine.SSOManager.GetSSOSystem(targetCode);
            if (targetsystem != null)
            {
                return targetsystem.Secret;

            }
            else
            {
                return targetCode;
            }

        }
        #endregion
        #region  Token解密方法
        /// <summary>
        /// DES数据解密
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string targetValue, string key)
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                return string.Empty;
            }
            // 定义DES加密对象
            var des = new DESCryptoServiceProvider();
            int len = targetValue.Length / 2;
            var inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(targetValue.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            // 通过两次哈希密码设置对称算法的初始化向量   
            des.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                  (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5").
                                                       Substring(0, 8), "sha1").Substring(0, 8));
            // 通过两次哈希密码设置算法的机密密钥   
            des.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile
                                                 (FormsAuthentication.HashPasswordForStoringInConfigFile(key, "md5")
                                                      .Substring(0, 8), "md5").Substring(0, 8));
            // 定义内存流
            var ms = new MemoryStream();
            // 定义加密流
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        #endregion

        #region 私有方法 ----------------------------------
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="workItem"></param>
        /// <param name="user"></param>
        /// <param name="boolMatchValue"></param>
        /// <param name="paramValues"></param>
        private void SaveBizObject(OThinker.H3.WorkItem.WorkItem workItem, OThinker.Organization.User user, OThinker.Data.BoolMatchValue boolMatchValue, List<DataItemParam> paramValues)
        {
            InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(workItem.InstanceId);
            string bizObjectId = InstanceContext == null ? string.Empty : InstanceContext.BizObjectId;
            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(InstanceContext.BizObjectSchemaCode);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(this.Engine, schema, workItem.Participant);
            bo.ObjectID = bizObjectId;
            bo.Load();

            // 设置数据项的值
            foreach (DataItemParam param in paramValues)
            {
                OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.ItemName);
                if (property.LogicType == DataLogicType.Comment)
                {// 审核意见
                    CommentParam comment = JSSerializer.Deserialize<CommentParam>(param.ItemValue + string.Empty);
                    this.Engine.BizObjectManager.AddComment(new Comment()
                    {
                        BizObjectId = bo.ObjectID,
                        BizObjectSchemaCode = schema.SchemaCode,
                        InstanceId = workItem.InstanceId,
                        TokenId = workItem.TokenId,
                        Approval = boolMatchValue,
                        Activity = workItem.ActivityCode,
                        DataField = property.Name,
                        UserID = user.ObjectID,
                        SignatureId = comment.SignatureId,
                        Text = comment.Text
                    });
                }
                else
                {
                    new BPM().SetItemValue(bo, property, param.ItemValue);
                }
            }

            bo.Update();
        }
        #endregion

    }

}
