using DongZheng.H3.WebApi.Common;
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
using System.Data.SqlClient;
using System.Web.Mvc;
using DataItemParam = DongZheng.H3.WebApi.Models.DataItemParam;
using DongZheng.H3.WebApi.Models.CallCenter;

namespace DongZheng.H3.WebApi.Controllers
{
    public class CallCenterController : OThinker.H3.Controllers.ControllerBase
    {
        public string callCenterUrl = System.Configuration.ConfigurationManager.AppSettings["Call_Center_Url"] + string.Empty;
        public string callCenterAssemblyIp = System.Configuration.ConfigurationManager.AppSettings["Call_Center_AssemblyIp"] + string.Empty;
        public string callCenterInnerIp = System.Configuration.ConfigurationManager.AppSettings["Call_Center_InnerIp"] + string.Empty;


        public override string FunctionCode => throw new NotImplementedException();

        //GET: Home
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public JavaScriptResult InitPopJS(string extension, string pop_out)
        {
            var url = "http://" + callCenterUrl + "/admin/?m=interface&c=api&a=popscreen&extension=" + extension + "&pop_type=LINK&pop_out=" + pop_out + "&open_type=2&mixcallback=mixcallback";

            AppUtility.Engine.LogWriter.Write("PopJs Url:" + url);

            var js = HttpHelper.get(url);

            if (callCenterUrl.Contains(callCenterAssemblyIp))
            {

                js = js.Replace(callCenterUrl, callCenterInnerIp);

                js = js.Replace(callCenterAssemblyIp, callCenterInnerIp);

                js = js.Replace("http://" + callCenterInnerIp, "https://" + callCenterInnerIp);

            }

            AppUtility.Engine.LogWriter.Write("PopJs after:" + js);

            var result = new JavaScriptResult();
            result.Script = js;
            return result;
        }

        public JavaScriptResult InitCommandJS()
        {
            var url = "http://" + callCenterUrl + "/interface/api/?action=command";

            AppUtility.Engine.LogWriter.Write("CommandJS Url:" + url);

            var js = HttpHelper.get(url);

            if (callCenterUrl.Contains(callCenterAssemblyIp))
            {

                js = js.Replace(callCenterUrl, callCenterInnerIp);

                js = js.Replace(callCenterAssemblyIp, callCenterInnerIp);

                js = js.Replace("http://" + callCenterInnerIp, "https://" + callCenterInnerIp);

                js = js.Replace("%3A9100", "");

            }

            AppUtility.Engine.LogWriter.Write("CommandJS after:" + js);

            var result = new JavaScriptResult();
            result.Script = js;
            return result;
        }

        #region 信审审核页面

        /// <summary>
        /// 获取手机号码后4位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPhoeNumber(string code)
        {
            var dic = new Dictionary<string, string>();

            string p = string.Empty;
            string phone = string.Empty;
            string sql = "select u.code,u.name,o.name departname,u.officephone,u.appellation from ot_user u join ot_organizationunit o on u.parentid=o.objectid where (o.Name = '信贷审批部' or  o.Name = '贷后管理') and u.state=1 and u.code='" + code + "' ";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                p = dt.Rows[0]["officephone"].ToString();
                phone = p.Substring(p.Length - 4, p.Length - (p.Length - 4));

                var departname = dt.Rows[0]["departname"].ToString();
                var position = dt.Rows[0]["appellation"].ToString();

                dic.Add("OfficePhone", phone);
                dic.Add("DepartName", departname);
                dic.Add("Position", position);
            }
            return dic;
        }
        /// <summary>
        /// 动态加载js 获取手机号码后4位
        /// </summary>
        /// <param name="code"></param>
        /// 
        /// <returns></returns>
        [HttpGet]
        public JsonResult SelectPhone(string code)
        {
            rtn_data rtn = new rtn_data { code = 1 };
            var dic = GetPhoeNumber(code);
            if (dic.Any())
            {
                rtn.data = dic;
            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }



        //查询电话信息
        public List<PhoneExtensionResponse> InquiryTelephone(string Applicationnumber)
        {
            List<PhoneExtensionResponse> responses = new List<PhoneExtensionResponse>();
            string sql = "select * from  c_application_phoneextension where APPLICATIONNUMBER='" + Applicationnumber + "' and STATUS=1 order by CREATEDTIME DESC ";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    PhoneExtensionResponse response = new PhoneExtensionResponse();
                    response.ObjectId = item["OBJECTID"].ToString();
                    response.Calledpartyname = item["CALLEDPARTYNAME"].ToString();
                    response.Calledpartytype = item["CALLEDPARTYTYPE"].ToString();
                    response.Calledpartynumber = item["CALLEDPARTYNUMBER"].ToString();
                    response.Calledpartynumbertype = item["CALLEDPARTYNUMBERTYPE"].ToString();
                    responses.Add(response);
                }
            }
            return responses;
        }
        /// <summary>
        /// 查询电话信息 通过贷款申请编号
        /// </summary>
        /// <param name="APPLICATIONNUMBER"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SelectApplicationnumber(string Applicationnumber)
        {

            rtn_data rtn = new rtn_data { code = 1 };
            var phoneExtensions = InquiryTelephone(Applicationnumber);
            rtn.data = phoneExtensions;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        //查询电话信息 按电话号码查询
        public List<PhoneExtensionResponse> InquiryCalledpartynumber(string Calledpartynumber)
        {
            List<PhoneExtensionResponse> responses = new List<PhoneExtensionResponse>();
            string sql = "select * from  c_application_phoneextension where CALLEDPARTYNUMBER='" + Calledpartynumber + "'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    PhoneExtensionResponse response = new PhoneExtensionResponse();
                    response.ObjectId = item["OBJECTID"].ToString();
                    response.Calledpartyname = item["CALLEDPARTYNAME"].ToString();
                    response.Calledpartytype = item["CALLEDPARTYTYPE"].ToString();
                    response.Calledpartynumber = item["CALLEDPARTYNUMBER"].ToString();
                    response.Calledpartynumbertype = item["CALLEDPARTYNUMBERTYPE"].ToString();
                    responses.Add(response);
                }
            }
            return responses;
        }
        /// <summary>
        /// 查询电话信息  按电话号码查询
        /// </summary>
        /// <param name="Calledpartynumber"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SelectCalledpartynumber(string Calledpartynumber)
        {

            rtn_data rtn = new rtn_data { code = 1 };
            var phoneExtensions = InquiryCalledpartynumber(Calledpartynumber);
            rtn.data = phoneExtensions;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 新增功能：添加电话
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CallCenterInsert(PhoneExtensionRequest p)
        {
            p.Status = 1;
            p.ObjectId = Guid.NewGuid().ToString();
            p.Createdtime = DateTime.Now.ToString();

            rtn_data rtn = new rtn_data { code = 1 };
            var officePhone = PhoneExtensionRequest(p);
            rtn.data = officePhone;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加电话
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int PhoneExtensionRequest(PhoneExtensionRequest p)
        {
            string sql = @"insert into c_application_phoneextension(OBJECTID,APPLICATIONNUMBER,MIANAPPLICANTNAME,CALLEDPARTYNAME,CALLEDPARTYTYPE,CALLEDPARTYNUMBER,CALLEDPARTYNUMBERTYPE,CREATEDTIME,STATUS)
                         values('" + p.ObjectId + "','" + p.Applicationnumber + "','" + p.Mianapplicantname + "','" + p.Calledpartyname + "','" + p.Calledpartytype + "','" + p.Calledpartynumber + "','" + p.Calledpartynumbertype + "',to_date('" + p.Createdtime + "','yyyy/mm/dd HH24:MI:SS'),'" + p.Status + "')";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除  修改状态
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public int Modifystate(string ObjectId)
        {
            string sql = @"update c_application_phoneextension set STATUS=0 where OBJECTID='" + ObjectId + "'";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 添加电话：删除 修改状态为0 无效
        /// </summary>
        /// <param name="Objectid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CallCenterUpdate(string ObjectId)
        {

            rtn_data rtn = new rtn_data { code = 1 };
            var officePhone = Modifystate(ObjectId);
            rtn.data = officePhone;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询录音列表
        /// </summary>
        /// <param name="Applicationnumber"></param>
        /// <param name="Calledpartyname"></param>
        /// <param name="Calledpartytype"></param>
        /// <param name="Calledpartynumber"></param>
        /// <returns></returns>
        public List<RecordingRequest> GetCallRecords(string Applicationnumber, string Calledpartyname, string Calledpartytype, string Calledpartynumber)
        {
            //select * from c_callrecords where APPLICATIONNUMBER='Br-A117400000' and CALLEDPARTYNAME='2' and CALLEDPARTYTYPE='1' and CALLEDPARTYNUMBER='1' order by CREATEDTIME desc
            List<RecordingRequest> list = new List<RecordingRequest>();
            string sql = "select * from c_callrecords   where APPLICATIONNUMBER='" + Applicationnumber + "' {0} order by CREATEDTIME desc";
            var whereSQL = string.Empty;
            if (!string.IsNullOrEmpty(Calledpartyname))
            {
                whereSQL += " and CALLEDPARTYNAME like '%" + Calledpartyname + "%'";
            }
            if (!string.IsNullOrEmpty(Calledpartytype))
            {
                whereSQL += " and CALLEDPARTYTYPE like '%" + Calledpartytype + "%'";
            }
            if (!string.IsNullOrEmpty(Calledpartynumber))
            {
                whereSQL += "and CALLEDPARTYNUMBER like '%" + Calledpartynumber + "%' ";
            }

            sql = string.Format(sql, whereSQL);

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    RecordingRequest Recording = new RecordingRequest();
                    Recording.Calledpartyname = item["CALLEDPARTYNAME"] + string.Empty;
                    Recording.Calledpartytype = item["CALLEDPARTYTYPE"] + string.Empty;
                    Recording.Calledpartynumber = item["CALLEDPARTYNUMBER"] + string.Empty;
                    Recording.Calledpartynumbertype = item["CALLEDPARTYNUMBERTYPE"] + string.Empty;
                    Recording.Createdtime = Convert.ToDateTime(item["RECORDCREATEDTIME"] + string.Empty).ToString("yyyy-MM-dd HH:mm:ss");
                    Recording.Recordfilename = item["RECORDFILENAME"] + string.Empty;
                    list.Add(Recording);
                }
            }
            return list;
        }


        /// <summary>
        /// 信审审核页面
        /// 查询录音信息 通过贷款申请编号
        /// </summary>
        /// <param name="Applicationnumber">贷款申请编号</param>
        /// <param name="Calledpartyname">被叫人姓名</param>
        /// <param name="Calledpartytype">被叫人角色类型</param>
        /// <param name="Calledpartynumber">被叫人电话</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult XSSHQueryRecords(string Applicationnumber, string Calledpartyname, string Calledpartytype, string Calledpartynumber)
        {

            rtn_data rtn = new rtn_data { code = 1 };
            var phoneExtensions = GetCallRecords(Applicationnumber, Calledpartyname, Calledpartytype, Calledpartynumber);
            rtn.data = phoneExtensions;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 电话弹屏

        /// <summary>
        /// 保存通话记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveRecord(CallRecordRequest request)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            var objectId = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(request.QuestionTypeStr))
                {
                    request.QuestionType = JsonConvert.DeserializeObject<List<QuestionType>>(request.QuestionTypeStr);
                }

                InsertCallRecord(request, ref objectId);

                if (request.QuestionType != null && request.QuestionType.Any())
                {
                    InsertCallQuestionType(objectId, request.UniqueId, request.QuestionType);
                }
                rtn.data = objectId;
            }
            catch (Exception ex)
            {
                rtn.code = 0;
                rtn.message = ex.ToString();
            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存通话记录
        /// </summary>
        public int InsertCallRecord(CallRecordRequest request, ref string objectId)
        {

            if (request.CalledNumber.StartsWith("0") && request.CalledNumberType == "手机 Mobile Phone")
            {
                request.CalledNumber = request.CalledNumber.Substring(1);
            }

            //if (request.CalledNumberType == "座机")
            //{
            //    request.CalledNumber = "0" + request.CalledNumber;
            //}

            var questiontypecount = request.QuestionType != null && request.QuestionType.Any() ? request.QuestionType.Count : 0;


            var sql = @"insert into c_callrecords(objectid,applicationnumber,contractno,mianapplicantname,calledpartyname,calledpartytype,calledpartynumber,calledpartynumbertype,calledpartyidtype,calledpartyidnumber,callername,callerposition,remark,callduration,recordfilename,calltype,uniqueid,callercode,questiontypecount,recordcreatedtime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}',{18},to_date('{19}','yyyy/mm/dd HH24:MI:SS')) ";

            objectId = Guid.NewGuid().ToString();

            sql = string.Format(sql, objectId, request.ApplicationNumber, request.ContractNo, request.MainApplicantName, request.CalledName, request.CalledType, request.CalledNumber, request.CalledNumberType, request.CalledIdType, request.CalledIdNumber, request.CallerName, request.CallerPosition, request.Remark, request.CallDuration, request.RecordFileName.ToUpper(), request.CallType, request.UniqueId, request.CallerCode, questiontypecount, request.RecordCreatedTime);

            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 保存通话咨询类型
        /// </summary>
        /// <param name="uniqueid"></param>
        /// <param name="QuestionType"></param>
        public void InsertCallQuestionType(string objectId, string uniqueid, List<QuestionType> QuestionType)
        {
            foreach (var type in QuestionType)
            {
                var sql = @"insert into c_callquestiontype (objectid,uniqueid,qtype1id,qtype1name,qtype2id,qtype2name,parentid) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                sql = string.Format(sql, Guid.NewGuid().ToString(), uniqueid, type.QType1Id, type.QType1Name, type.QType2Id, type.QType2Name, objectId);
                OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            }
        }


        /// <summary>
        /// 查询人员合同信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult QueryPersonContractInfo(string PhoneNumber)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            try
            {
                var callIns = GetPersonContractInfo(PhoneNumber);
                rtn.data = callIns;
            }
            catch (Exception ex)
            {
                rtn.code = 0;
                AppUtility.Engine.LogWriter.Write($"查询人员合同信息错误-->PhoneNumber：{PhoneNumber}-->>{ex.ToString()}");
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据手机号码查询
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public List<CallInResponse> GetPersonContractInfo(string PhoneNumber)
        {
            var phonesql = @"select  APPLICATION_NUMBER    
                                ,PHONE_NUMBER         
                                ,EXTERNAL_CONTRACT_NBR
                                ,NAME                 
                                ,ROLE_ID              
                                ,Id_Card_Typ          
                                ,ID_CARD_NBR          
                                ,REQUEST_STATUS_CDE   
                                from IN_CMS.V_CALL_CONTRACT_INFO where phone_number='{0}'";

            phonesql = string.Format(phonesql, PhoneNumber);

            var mainContractSql = @"select  APPLICATION_NUMBER    
                                ,PHONE_NUMBER         
                                ,EXTERNAL_CONTRACT_NBR
                                ,NAME                 
                                ,ROLE_ID              
                                ,Id_Card_Typ          
                                ,ID_CARD_NBR          
                                ,REQUEST_STATUS_CDE   
                                from IN_CMS.V_CALL_CONTRACT_INFO where EXTERNAL_CONTRACT_NBR='{0}' and ROLE_ID='B'";


            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", phonesql);

            var callIns = new List<CallInResponse>();

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    CallInResponse callIn = new CallInResponse();
                    callIn.ApplicationNumber = item["APPLICATION_NUMBER"] + string.Empty;
                    callIn.PhoneNumber = item["PHONE_NUMBER"] + string.Empty;
                    callIn.ContractNo = item["EXTERNAL_CONTRACT_NBR"] + string.Empty;
                    callIn.Name = item["NAME"] + string.Empty;
                    var roleId = item["ROLE_ID"] + string.Empty;

                    switch (roleId)
                    {
                        case "B":
                            callIn.PersonType = "主借人";
                            break;
                        case "G":
                            callIn.PersonType = "担保人";
                            break;
                        case "C":
                            callIn.PersonType = "共借人";
                            break;
                        default:
                            callIn.PersonType = "其他";
                            break;
                    }

                    if (roleId != "B")
                    {
                        mainContractSql = string.Format(mainContractSql, callIn.ContractNo);
                        DataTable mainPersonDt = CommonFunction.ExecuteDataTableSql("CAPDB", mainContractSql);
                        if (CommonFunction.hasData(mainPersonDt))
                        {
                            callIn.MainApplicantName = mainPersonDt.Rows[0]["Name"] + string.Empty;
                        }
                    }
                    else
                    {
                        callIn.MainApplicantName = callIn.Name;
                    }

                    var idCardType = item["Id_Card_Typ"] + string.Empty;

                    switch (idCardType)
                    {
                        case "00001":
                            callIn.IdCardType = "身份证";
                            break;
                        case "00002":
                            callIn.IdCardType = "户口本";
                            break;
                        case "00003":
                            callIn.IdCardType = "护照";
                            break;
                        default:
                            callIn.IdCardType = "其他";
                            break;
                    }

                    callIn.IdCardNumber = item["ID_CARD_NBR"] + string.Empty;

                    var statusCode = item["REQUEST_STATUS_CDE"] + string.Empty;

                    if (!string.IsNullOrEmpty(statusCode))
                    {
                        switch (statusCode)
                        {
                            case "20":
                                callIn.ContractStatus = "普通";
                                break;
                            case "21":
                                callIn.ContractStatus = "逾期";
                                break;
                            case "22":
                                callIn.ContractStatus = "高逾期";
                                break;
                            case "25":
                                callIn.ContractStatus = "结清";
                                break;
                            default:
                                break;
                        }
                    }
                    callIns.Add(callIn);
                }
            }

            return callIns;
        }

        #endregion

        #region 录音记录页面

        /// <summary>
        /// 录音记录页面
        /// 数据中心-查询录音记录
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QueryRecords(QueryCallRecordsRquest request)
        {
            var totalNumber = 0;

            var dt = GetRecords(request, ref totalNumber);

            var data = BuildRecordingModel(dt);

            //获取咨询类型
            var qtypes = GetQuestionTypes(data.Select(p => p.UniqueId).ToList());

            data.ForEach(p =>
            {
                var types = qtypes.Where(qtype => qtype.ParentId == p.ObjectId).ToList();

                if (types == null && !types.Any())
                {
                    return;
                }
                p.QuestionType = string.Join(",", types.Select(t => t.QType2Name));
                p.QTpye1Ids = types.GroupBy(t => t.QType1Id).Select(t => t.Key).ToList();
                p.QTpye2Ids = types.GroupBy(t => t.QType2Id).Select(t => t.Key).ToList();
            });

            if (!string.IsNullOrEmpty(request.QuestionType))
            {
                data = data.Where(p => p.QuestionType.Contains(request.QuestionType)).ToList();
            }

            GridViewModel<QueryCallRecordsResponse> result = new GridViewModel<QueryCallRecordsResponse>(totalNumber, data);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 录音查询返回Model
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<QueryCallRecordsResponse> BuildRecordingModel(DataTable dt)
        {
            var list = new List<QueryCallRecordsResponse>();

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var response = new QueryCallRecordsResponse();
                    response.ObjectId = row["OBJECTID"] + string.Empty;
                    response.CalledName = row["CALLEDPARTYNAME"] + string.Empty;//被叫人姓名
                    response.CalledType = row["CALLEDPARTYTYPE"] + string.Empty;//被叫人角色类型
                    response.CalledIdType = row["CALLEDPARTYIDTYPE"] + string.Empty;//证件类型
                    response.CalledIdNumber = row["CALLEDPARTYIDNUMBER"] + string.Empty;//证件号码
                    response.ContractNo = row["CONTRACTNO"] + string.Empty;//合同号
                    response.CalledNumber = row["CALLEDPARTYNUMBER"] + string.Empty;//被叫人电话
                    response.CalledNumberType = row["CALLEDPARTYNUMBERTYPE"] + string.Empty;//电话类型
                    //response.QuestionType = row["QUESTIONTYPE"] + string.Empty;//咨询类型
                    response.CallerName = row["CALLERNAME"] + string.Empty;//主叫人姓名
                    response.CallerPosition = row["CALLERPOSITION"] + string.Empty;//职务
                    response.Remark = row["REMARK"] + string.Empty;
                    response.RecordFileName = row["RECORDFILENAME"] + string.Empty;//录音文件名
                    response.UniqueId = row["UniqueId"] + string.Empty;//录音文件名
                    response.CallType = Convert.ToInt32(row["CALLTYPE"] + string.Empty);
                    var callDuration = row["CALLDURATION"] + string.Empty;
                    response.CallDuration = string.IsNullOrEmpty(callDuration) ? 0 : Convert.ToInt32(callDuration);
                    response.RowNumber = Convert.ToInt32(row["rownum"] + string.Empty);
                    response.CreateTime = Convert.ToDateTime(row["RECORDCREATEDTIME"] + string.Empty).ToString("yyyy-MM-dd HH:mm:ss");
                    list.Add(response);
                }
            }
            return list;
        }



        /// <summary>
        /// 录音记录页面
        /// 录音记录查询 SQL
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataTable GetRecords(QueryCallRecordsRquest request, ref int totalNumber)
        {
            var countSql = @"select count(1) from c_callrecords ";

            string sql = @"select * from  c_callrecords ";

            var whereSql = " where CALLTYPE=" + request.CallType + "";

            var parentSql = @"select * from C_CALLPOWER where usercode='{0}' and state=1";

            DataTable parentDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(parentSql, request.CallerCode));

            var codeWhereSql = "'" + request.CallerCode + "',";

            if (CommonFunction.hasData(parentDt))
            {
                var parentId = parentDt.Rows[0]["parentid"] + string.Empty;

                if (string.IsNullOrEmpty(parentId))
                {
                    var parentObjectId = parentDt.Rows[0]["objectid"] + string.Empty;

                    DataTable chilrdDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format("select * from C_CALLPOWER where parentid='{0}' and state=1", parentObjectId));
                    if (CommonFunction.hasData(chilrdDt))
                    {

                        foreach (DataRow row in chilrdDt.Rows)
                        {
                            codeWhereSql += "'" + row["usercode"] + string.Empty + "',";
                        }
                    }
                }
            }
            codeWhereSql = codeWhereSql.Substring(0, codeWhereSql.Length - 1);

            whereSql += string.Format(" and CALLERCODE in ({0})", codeWhereSql);

            if (!string.IsNullOrEmpty(request.CalledName))
            {
                whereSql += " and CALLEDPARTYNAME='" + request.CalledName + "'"; //用户姓名
            }
            if (!string.IsNullOrEmpty(request.CalledType))
            {
                whereSql += " and CALLEDPARTYTYPE='" + request.CalledType + "'"; //角色类型
            }
            if (!string.IsNullOrEmpty(request.CalledIdType))
            {
                whereSql += " and CALLEDPARTYIDTYPE='" + request.CalledIdType + "'"; //证件类型
            }
            if (!string.IsNullOrEmpty(request.CalledIdNumber))
            {
                whereSql += " and CALLEDPARTYIDNUMBER='" + request.CalledIdNumber + "'"; //证件号码
            }
            if (!string.IsNullOrEmpty(request.ContractNo))
            {
                whereSql += " and CONTRACTNO='" + request.ContractNo + "'"; //合同号
            }
            if (!string.IsNullOrEmpty(request.CalledNumber))
            {
                whereSql += " and CALLEDPARTYNUMBER='" + request.CalledNumber + "'"; //被叫人电话
            }
            if (!string.IsNullOrEmpty(request.CalledNumberType))
            {
                whereSql += " and CALLEDPARTYNUMBERTYPE='" + request.CalledNumberType + "'"; //电话类型
            }
            //if (!string.IsNullOrEmpty(request.QuestionType))
            //{
            //    whereSql += " and QUESTIONTYPE='" + request.QuestionType + "'"; //咨询类型
            //}
            if (!string.IsNullOrEmpty(request.CallerName))
            {
                whereSql += " and CALLERNAME='" + request.CallerName + "'"; //主叫人
            }
            if (!string.IsNullOrEmpty(request.CallerPosition))
            {
                whereSql += " and CALLERPOSITION='" + request.CallerPosition + "'"; //职务
            }

            if (!string.IsNullOrEmpty(request.StartDate))
            {
                whereSql += " and RECORDCREATEDTIME >= to_date('" + request.StartDate + "','yyyy-MM-dd')"; //
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                whereSql += " and RECORDCREATEDTIME <= to_date('" + request.EndDate + "','yyyy-MM-dd')"; //
            }

            sql = string.Format("{0} {1} {2}", sql, whereSql, " order by createdtime desc");
            countSql = string.Format("{0} {1}", countSql, whereSql);

            if (request.StartIndex > 0 && request.EndIndex > 0)
            {
                //sql += " where c.rn >={1} and c.rn<={2}";
                var pageSql = @" SELECT rownum,c.*  FROM (SELECT ROWNUM r, a.* FROM (select * from c_callrecords {0} order by createdtime desc) a WHERE ROWNUM <= {1} ) c  WHERE r >= {2} ";

                sql = string.Format(pageSql, whereSql, request.EndIndex, request.StartIndex);
            }


            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var num = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(countSql);

            totalNumber = Convert.ToInt32(num);

            return dt;
        }

        public List<QuestionType> GetQuestionTypes(List<string> uniqueIds)
        {
            var qtypes = new List<QuestionType>();

            var sql = @"select * from c_callquestiontype";

            if (uniqueIds != null && !uniqueIds.Any())
            {
                sql += string.Format(" where uniqueid in('{0}')", string.Join("','", uniqueIds));
            }

            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    qtypes.Add(new QuestionType { ObjectId = row["ObjectId"] + string.Empty, Uniqueid = row["Uniqueid"] + string.Empty, QType1Id = row["QType1Id"] + string.Empty, QType1Name = row["QType1Name"] + string.Empty, QType2Id = row["QType2Id"] + string.Empty, QType2Name = row["QType2Name"] + string.Empty, ParentId = row["ParentId"] + string.Empty });
                }
            }
            return qtypes;

        }

        /// <summary>
        /// 更新通话记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateRecord(CallRecordRequest request)
        {
            rtn_data rtn = new rtn_data { code = 1 };

            if (string.IsNullOrEmpty(request.ObjectId))
            {
                rtn.code = 0;
                rtn.message = "更新参数错误";
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(request.QuestionTypeStr))
            {
                request.QuestionType = JsonConvert.DeserializeObject<List<QuestionType>>(request.QuestionTypeStr);
            }

            try
            {
                UpdateCallRecordInfo(request);

                if (request.QuestionType != null && request.QuestionType.Any())
                {
                    UpdateCallQuestionType(request.ObjectId, request.UniqueId, request.QuestionType);
                }
            }
            catch (Exception ex)
            {
                rtn.code = 0;
                rtn.message = ex.ToString();
            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑录音内容
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool UpdateCallRecordInfo(CallRecordRequest request)
        {
            if (string.IsNullOrEmpty(request.ObjectId))
            {
                return false;
            }

            var sql = @"update c_callrecords set {0} where objectId='{1}'";

            var updateSql = string.Empty;

            if (!string.IsNullOrEmpty(request.CalledName))
            {
                updateSql += "calledpartyname='" + request.CalledName + "',";
            }

            if (!string.IsNullOrEmpty(request.CalledType))
            {
                updateSql += "calledpartytype='" + request.CalledType + "',";
            }

            if (!string.IsNullOrEmpty(request.CalledIdType))
            {
                updateSql += "calledpartyidtype='" + request.CalledIdType + "',";
            }

            if (!string.IsNullOrEmpty(request.CalledIdNumber))
            {
                updateSql += "calledpartyidnumber='" + request.CalledIdNumber + "',";
            }

            if (!string.IsNullOrEmpty(request.ContractNo))
            {
                updateSql += "contractno='" + request.ContractNo + "',";
            }

            if (!string.IsNullOrEmpty(request.CalledNumber))
            {
                updateSql += "calledpartynumber='" + request.CalledNumber + "',";
            }

            if (!string.IsNullOrEmpty(request.CalledNumberType))
            {
                updateSql += "calledpartynumbertype='" + request.CalledNumberType + "',";
            }

            if (!string.IsNullOrEmpty(request.CallerName))
            {
                updateSql += "callername='" + request.CallerName + "',";
            }

            if (!string.IsNullOrEmpty(request.CallerPosition))
            {
                updateSql += "callerposition='" + request.CallerPosition + "',";
            }

            if (!string.IsNullOrEmpty(request.Remark))
            {
                updateSql += "remark='" + request.Remark + "',";
            }

            if (request.CallDuration > 0)
            {
                updateSql += "callduration=" + request.CallDuration + ",";
            }

            var questiontypecount = request.QuestionType != null && request.QuestionType.Any() ? request.QuestionType.Count : 0;

            updateSql += "questiontypecount=" + questiontypecount + ",";

            if (string.IsNullOrEmpty(updateSql))
            {
                return false;
            }

            updateSql = updateSql.Substring(0, updateSql.Length - 1);

            sql = string.Format(sql, updateSql, request.ObjectId);

            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql) > 0 ? true : false;
        }

        /// <summary>
        /// 更新咨询类型
        /// </summary>
        /// <param name="types"></param>
        public void UpdateCallQuestionType(string parentid, string uniqueid, List<QuestionType> types)
        {
            var cmd = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateTransaction();

            try
            {

                cmd.ExecuteNonQuery("delete from c_callquestiontype where parentid ='" + parentid + "'");

                foreach (var type in types)
                {
                    var sql = @"insert into c_callquestiontype (objectid,uniqueid,qtype1id,qtype1name,qtype2id,qtype2name,parentid) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";
                    sql = string.Format(sql, Guid.NewGuid().ToString(), uniqueid, type.QType1Id, type.QType1Name, type.QType2Id, type.QType2Name, parentid);
                    cmd.ExecuteNonQuery(sql);
                }

                cmd.Commit();
            }
            catch (Exception ex)
            {
                cmd.Rollback();
            }
        }

        #endregion
    }

    #region Class

    public class PhoneExtensionRequest
    {
        public string ObjectId { get; set; }
        public string Applicationnumber { get; set; }
        public string Mianapplicantname { get; set; }
        public string Calledpartyname { get; set; }
        public string Calledpartytype { get; set; }
        public string Calledpartynumber { get; set; }
        public string Calledpartynumbertype { get; set; }
        public string Createdtime { get; set; }
        public int Status { get; set; }

    }

    public class PhoneExtensionResponse
    {
        public string ObjectId { get; set; }
        public string Calledpartyname { get; set; }
        public string Calledpartytype { get; set; }
        public string Calledpartynumber { get; set; }
        public string Calledpartynumbertype { get; set; }
    }

    public class RecordingRequest
    {
        public string Applicationnumber { get; set; }
        public string Calledpartyname { get; set; }
        public string Calledpartytype { get; set; }
        public string Calledpartynumber { get; set; }
        public string Calledpartynumbertype { get; set; }
        public string Createdtime { get; set; }
        public string Recordfilename { get; set; }

    }
    #endregion


}