using DongZheng.H3.WebApi.Common.Portal;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class StartWorkFlowController : CustomController
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string ApplyNo = "";
            System.IO.Stream s = context.Request.InputStream;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
            {
                ApplyNo = sr.ReadToEnd();
            }
            OThinker.H3.Controllers.UserValidator User = context.Session[OThinker.H3.Controllers.Sessions.GetUserValidator()] as OThinker.H3.Controllers.UserValidator;
            AppUtility.Engine.LogWriter.Write("000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000：抵押中心：" + User.UserCode + "," + User.UserName);
            string ret = "";
            BPMServiceResult result = new BPMServiceResult();
            string strobjectid = "";
            if (!string.IsNullOrEmpty(ApplyNo))
                strobjectid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM I_Mortgage WHERE ApplyNo LIKE '%" + ApplyNo + "%'") + string.Empty;
            if (!string.IsNullOrEmpty(strobjectid))
            {
                result.Message = "已发起抵押流程，不能重复发起";
                result.Success = false;
            }
            else
            {
                string sql = "select STATE_NME,CITY_NME,APPLICATION_TYPE_NAME,APPLICATION_TYPE_CODE,CONDITION_NAME,ASSET_MAKE_DSC,POWER_PARAMETER,NEW_PRICE,ENGINE_NUMBER,VIN_NUMBER,FP_GROUP_NAME,FINANCIAL_PRODUCT_NAME,CASH_DEPOSIT,AMOUNT_FINANCED,ACCESSORY_AMT,BUSINESS_PARTNER_NME from dz_vw_h3_proposal_info where Application_Number ='" + ApplyNo + "'";
                System.Data.DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                string loanTime = "";
                string contractNum = "";
                string state = "";
                Motigage mt = new Motigage();
                List<DataItemParam> datalist = new List<DataItemParam>();
                string sqlht = "SELECT 合同号,合同激活日期,经销商名称 FROM in_cms.mv_dy_application_date_info@to_dy_cms WHERE 申请号 = '" + ApplyNo + "'";
                System.Data.DataTable dtht = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlht);
                if (dtht.Rows.Count > 0)
                {
                    loanTime = dtht.Rows[0]["合同激活日期"] + string.Empty;
                    contractNum = dtht.Rows[0]["合同号"] + string.Empty;
                    datalist.Add(new DataItemParam { ItemName = "经销商名称", ItemValue = dtht.Rows[0]["经销商名称"] + string.Empty });
                    AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------1：经销商名称：" + dtht.Rows[0]["经销商名称"]);
                }
                if (dt.Rows.Count > 0)
                {
                    AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------1：抵押中心：--------------------------------------------------------------------");
                    datalist.Add(new DataItemParam { ItemName = "province", ItemValue = dt.Rows[0]["STATE_NME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "city", ItemValue = dt.Rows[0]["CITY_NME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "applyType", ItemValue = dt.Rows[0]["APPLICATION_TYPE_NAME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "assets", ItemValue = dt.Rows[0]["CONDITION_NAME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "factory", ItemValue = dt.Rows[0]["ASSET_MAKE_DSC"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "carType", ItemValue = dt.Rows[0]["POWER_PARAMETER"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "newCarPrice", ItemValue = dt.Rows[0]["NEW_PRICE"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "assetsPrice", ItemValue = dt.Rows[0]["NEW_PRICE"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "motorType", ItemValue = dt.Rows[0]["ENGINE_NUMBER"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "frameNumber", ItemValue = dt.Rows[0]["VIN_NUMBER"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "productClass", ItemValue = dt.Rows[0]["FP_GROUP_NAME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "productType", ItemValue = dt.Rows[0]["FINANCIAL_PRODUCT_NAME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "SFMoney", ItemValue = dt.Rows[0]["CASH_DEPOSIT"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "DKMoney", ItemValue = dt.Rows[0]["AMOUNT_FINANCED"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "FJMoney", ItemValue = dt.Rows[0]["ACCESSORY_AMT"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "经销商名称", ItemValue = dt.Rows[0]["BUSINESS_PARTNER_NME"] + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "LoanTime", ItemValue = loanTime + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "ApplyNo", ItemValue = ApplyNo + string.Empty });
                    datalist.Add(new DataItemParam { ItemName = "合同号", ItemValue = contractNum + string.Empty });
                    bool mortflag = false;
                    if (Convert.ToDateTime(loanTime) > DateTime.Now) mortflag = true;
                    datalist.Add(new DataItemParam { ItemName = "XDHFflag", ItemValue = mortflag ? "是" : "否" + string.Empty });
                    state = dt.Rows[0]["APPLICATION_TYPE_CODE"] + string.Empty;
                    sql = "select FIRST_THI_NME,CASE WHEN TYPE='M' THEN '主贷人' WHEN TYPE='C' THEN '共借人' ELSE '担保人' END AS TYPE,ID_CARD_NME,ID_CARD_NBR FROM dz_vw_h3_proposal_app_type where Application_Number ='" + ApplyNo + "'";
                    System.Data.DataTable dtchild = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (dtchild.Rows.Count > 0)
                    {
                        List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                        for (int i = 0; i < dtchild.Rows.Count; i++)
                        {
                            AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------1：主贷人for：" + i.ToString() + dtchild.Rows[i]["FIRST_THI_NME"]);
                            if (dtchild.Rows[i]["TYPE"].ToString() == "主贷人" || dtchild.Rows[i]["TYPE"].ToString().IndexOf("主贷人", StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                datalist.Add(new DataItemParam { ItemName = "JKName", ItemValue = dtchild.Rows[i]["FIRST_THI_NME"] + string.Empty });
                                AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------2：主贷人：" + i.ToString() + dtchild.Rows[i]["FIRST_THI_NME"]);
                            }
                            chileList.Add(new List<DataItemParam>()
                            {
                                new DataItemParam{ ItemName = "People", ItemValue = dtchild.Rows[i]["FIRST_THI_NME"] + string.Empty },
                                new DataItemParam{ ItemName = "PeopleType", ItemValue =dtchild.Rows[i]["TYPE"] + string.Empty},
                                new DataItemParam{ ItemName = "IdType", ItemValue = dtchild.Rows[i]["ID_CARD_NME"] + string.Empty},
                                new DataItemParam{ ItemName = "ID", ItemValue = dtchild.Rows[i]["ID_CARD_NBR"] + string.Empty}
                            });
                        }
                        datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                    }
                }
                else
                {
                    string sql0 = "select provinceName,dealercityName,appTypeName,ThaiFirstName,IdCarTypeName,IdCardNo,GjThaiFirstName,GjIdCarTypeName,GjIdCardNo,DbThaiFirstName,DbIdCarTypeName,DbIdCardNo,assetConditionName,assetMakeName,brandName,assetPrice,engineNo,vinNo,productGroupName,productTypeName,	downpaymentamount,financedamount,totalaccessoryamount,DEALERNAME  from I_RetailApp where applicationNo ='" + ApplyNo + "'";
                    System.Data.DataTable dt0 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql0);
                    if (dt0.Rows.Count > 0)
                    {
                        AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------2：抵押中心：--------------------------------------------------------------------");
                        datalist.Add(new DataItemParam { ItemName = "province", ItemValue = dt0.Rows[0]["provinceName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "city", ItemValue = dt0.Rows[0]["dealercityName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "applyType", ItemValue = dt0.Rows[0]["appTypeName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "assets", ItemValue = dt0.Rows[0]["assetConditionName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "factory", ItemValue = dt0.Rows[0]["assetMakeName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "carType", ItemValue = dt0.Rows[0]["brandName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "newCarPrice", ItemValue = dt0.Rows[0]["assetPrice"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "assetsPrice", ItemValue = dt0.Rows[0]["assetPrice"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "motorType", ItemValue = dt0.Rows[0]["engineNo"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "frameNumber", ItemValue = dt0.Rows[0]["vinNo"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "productClass", ItemValue = dt0.Rows[0]["productGroupName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "productType", ItemValue = dt0.Rows[0]["productTypeName"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "SFMoney", ItemValue = dt0.Rows[0]["downpaymentamount"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "DKMoney", ItemValue = dt0.Rows[0]["financedamount"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "FJMoney", ItemValue = dt0.Rows[0]["totalaccessoryamount"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "经销商名称", ItemValue = dt0.Rows[0]["DEALERNAME"] + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "LoanTime", ItemValue = loanTime + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "ApplyNo", ItemValue = ApplyNo + string.Empty });
                        datalist.Add(new DataItemParam { ItemName = "合同号", ItemValue = contractNum + string.Empty });
                        //datalist.Add(new DataItemParam { ItemName = "name", ItemValue = ApplyNo + "-" + User.UserName });
                        bool mortflag = false;
                        if (Convert.ToDateTime(loanTime) > DateTime.Now) mortflag = true;
                        datalist.Add(new DataItemParam { ItemName = "XDHFflag", ItemValue = mortflag ? "是" : "否" + string.Empty });
                        List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                        if (!string.IsNullOrEmpty(dt0.Rows[0]["ThaiFirstName"] + string.Empty))
                        {
                            datalist.Add(new DataItemParam { ItemName = "JKName", ItemValue = dt0.Rows[0]["ThaiFirstName"] + string.Empty });
                            AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------3：主贷人：" + dt0.Rows[0]["ThaiFirstName"]);
                            chileList.Add(new List<DataItemParam>()
                            {
                                new DataItemParam{ ItemName = "People", ItemValue = dt0.Rows[0]["ThaiFirstName"] + string.Empty },
                                new DataItemParam{ ItemName = "PeopleType", ItemValue =  "主贷人"},
                                new DataItemParam{ ItemName = "IdType", ItemValue = dt0.Rows[0]["IdCarTypeName"] + string.Empty},
                                new DataItemParam{ ItemName = "ID", ItemValue = dt0.Rows[0]["IdCardNo"] + string.Empty}
                            });
                            datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                        }
                        if (!string.IsNullOrEmpty(dt0.Rows[0]["GjThaiFirstName"] + string.Empty))
                        {
                            chileList.Add(new List<DataItemParam>()
                            {
                                new DataItemParam{ ItemName = "People", ItemValue =  dt0.Rows[0]["GjThaiFirstName"] + string.Empty},
                                new DataItemParam{ ItemName = "PeopleType", ItemValue =   "共借人"},
                                new DataItemParam{ ItemName = "IdType", ItemValue =  dt0.Rows[0]["GjIdCarTypeName"] + string.Empty},
                                new DataItemParam{ ItemName = "ID", ItemValue = dt0.Rows[0]["GjIdCardNo"] + string.Empty}
                            });
                            datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                        }
                        if (!string.IsNullOrEmpty(dt0.Rows[0]["DbThaiFirstName"] + string.Empty))
                        {
                            chileList.Add(new List<DataItemParam>()
                            {
                                new DataItemParam{ ItemName = "People", ItemValue =  dt0.Rows[0]["DbThaiFirstName"] + string.Empty},
                                new DataItemParam{ ItemName = "PeopleType", ItemValue =   "担保人"},
                                new DataItemParam{ ItemName = "IdType", ItemValue =   dt0.Rows[0]["DbIdCarTypeName"] + string.Empty},
                                new DataItemParam{ ItemName = "ID", ItemValue = dt0.Rows[0]["DbIdCardNo"] + string.Empty}
                            });
                            datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                        }
                    }
                    else
                    {
                        string sql1 = "select provinceName,dealercityName,appTypeName,ZjrCompanyName,DbThaiFirstName,DbIdCarTypeName,DbIdCardNo,assetConditionName,assetMakeName,brandName,assetPrice,engineNo,vinNo,productGroupName,productTypeName,downpaymentamount,financedamount,totalaccessoryamount,DEALERNAME from I_CompanyApp where applicationNo ='" + ApplyNo + "'";
                        System.Data.DataTable dt1 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
                        if (dt1.Rows.Count > 0)
                        {
                            AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------3：抵押中心：--------------------------------------------------------------------");
                            datalist.Add(new DataItemParam { ItemName = "province", ItemValue = dt1.Rows[0]["provinceName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "city", ItemValue = dt1.Rows[0]["dealercityName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "applyType", ItemValue = dt1.Rows[0]["appTypeName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "assets", ItemValue = dt1.Rows[0]["assetConditionName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "factory", ItemValue = dt1.Rows[0]["assetMakeName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "carType", ItemValue = dt1.Rows[0]["brandName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "newCarPrice", ItemValue = dt1.Rows[0]["assetPrice"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "assetsPrice", ItemValue = dt1.Rows[0]["assetPrice"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "motorType", ItemValue = dt1.Rows[0]["engineNo"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "frameNumber", ItemValue = dt1.Rows[0]["vinNo"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "productClass", ItemValue = dt1.Rows[0]["productGroupName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "productType", ItemValue = dt1.Rows[0]["productTypeName"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "SFMoney", ItemValue = dt1.Rows[0]["downpaymentamount"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "DKMoney", ItemValue = dt1.Rows[0]["financedamount"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "FJMoney", ItemValue = dt1.Rows[0]["totalaccessoryamount"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "经销商名称", ItemValue = dt1.Rows[0]["DEALERNAME"] + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "LoanTime", ItemValue = loanTime + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "ApplyNo", ItemValue = ApplyNo + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "合同号", ItemValue = contractNum + string.Empty });
                            //datalist.Add(new DataItemParam { ItemName = "name", ItemValue = ApplyNo + "-" + User.UserName });
                            bool mortflag = false;
                            if (Convert.ToDateTime(loanTime) > DateTime.Now) mortflag = true;
                            datalist.Add(new DataItemParam { ItemName = "XDHFflag", ItemValue = mortflag ? "是" : "否" + string.Empty });

                            List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                            if (!string.IsNullOrEmpty(dt1.Rows[0]["ZjrCompanyName"] + string.Empty))
                            {
                                //foreach (var contact in dt1.Rows[0]["ZjrCompanyName"] + string.Empty)
                                //{
                                datalist.Add(new DataItemParam { ItemName = "JKName", ItemValue = dt1.Rows[0]["ZjrCompanyName"] + string.Empty });
                                AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------4：主贷人：" + dt1.Rows[0]["ZjrCompanyName"]);
                                chileList.Add(new List<DataItemParam>()
                                {
                                    new DataItemParam{ ItemName = "People", ItemValue = dt1.Rows[0]["ZjrCompanyName"] + string.Empty },
                                    new DataItemParam{ ItemName = "PeopleType", ItemValue = "主贷公司"},
                                    new DataItemParam{ ItemName = "IdType", ItemValue = ""},
                                    new DataItemParam{ ItemName = "ID", ItemValue = ""}
                                });
                                // }
                                datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                            }
                            if (!string.IsNullOrEmpty(dt1.Rows[0]["DbThaiFirstName"] + string.Empty))
                            {
                                //foreach (var contact in dt1.Rows[0]["DbThaiFirstName"] + string.Empty)
                                //{
                                chileList.Add(new List<DataItemParam>()
                                {
                                    new DataItemParam{ ItemName = "People", ItemValue = dt1.Rows[0]["DbThaiFirstName"] + string.Empty },
                                    new DataItemParam{ ItemName = "PeopleType", ItemValue = "担保人"},
                                    new DataItemParam{ ItemName = "IdType", ItemValue = dt1.Rows[0]["DbIdCarTypeName"] + string.Empty},
                                    new DataItemParam{ ItemName = "ID", ItemValue = dt1.Rows[0]["DbIdCardNo"] + string.Empty}
                                    });
                                //}
                                datalist.Add(new DataItemParam { ItemName = "GJPeople", ItemValue = chileList });
                            }
                        }
                        else
                        {
                            AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------5：主贷人：无");
                            datalist.Add(new DataItemParam { ItemName = "ApplyNo", ItemValue = ApplyNo + string.Empty });
                            datalist.Add(new DataItemParam { ItemName = "合同号", ItemValue = contractNum + string.Empty });
                        }
                    }
                }
                string str = JSSerializer.Serialize(datalist);
                List<DataItemParam> lis = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<DataItemParam>>(str);
                // HttpContext.Current.Server.Execute("BPMServicebpm.asmx",);
                result = startWorkflowBPM("Mortgage", User.UserCode, false, lis);
            }
            //BPMServiceResult result = startWorkflowByEntityTrinsJson("Mortgage", "liuwanying@dongzhengafc.com", false, str);// User.UserCode  "18f923a7-5a5e-426d-94ae-a55ad1a4b239"   "liuwanying@dongzhengafc.com"
            if (result.Success == true) ret = result.WorkItemUrl;
            else ret = "0";
            context.Response.Write(ret);
        }

        public class Motigage
        {
            public string province { get; set; }        //所在省份      STATE_NAME
            public string city { get; set; }            //所在城市      CITY_NAME
            public string applyType { get; set; }      //申请类型      APPLICATION_TYPE_NAME
            public string JKName { get; set; }        //借款人姓名
            public string GJName { get; set; }          //共借人姓名
            public string DBRName { get; set; }          //担保人姓名
            public string JKCardID { get; set; }         //证件号码
            public string GJCardID { get; set; }          //共借人证件号码
            public string DBRCardID { get; set; }          //担保人证件号码
            public string assets { get; set; }            //资产状况         CONDITION_NAME
            public string factory { get; set; }           //制作商           ASSET_MAKE_DSC
            public string carType { get; set; }           //车型              POWER_PARAMETER 
            public string newCarPrice { get; set; }          //新车指导价      NEW_PRICE
            public string assetsPrice { get; set; }          //资产价格        NEW_PRICE
            public string motorType { get; set; }          //发动机号码     ENGINE_NUMBER
            public string frameNumber { get; set; }          //车架号       VIN_NUMBER
            public string productClass { get; set; }          //产品组      FP_GROUP_NAME
            public string productType { get; set; }          //产品类型     FINANCIAL_PRODUCT_NAME
            public string SFMoney { get; set; }            //首付金额       CASH_DEPOSIT
            public string DKMoney { get; set; }           //贷款金额        AMOUNT_FINANCED
            public string FJMoney { get; set; }           //附加费          ACCESSORY_AMT
            public List<GJPeople> GJPeople;

        }
        public class GJPeople
        {
            public string People;
            public string PeopleType;
            public string ID;
            public string IdType;
        }
        /// <summary>
        /// 不带子表启动
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="userCode"></param>
        /// <param name="finishStart"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        private BPMServiceResult StartWorkflow1(string workflowCode, string userCode, bool finishStart, object Model)
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
                OThinker.Organization.User user = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
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
                var s = Model.GetType().GetProperties();

                foreach (System.Reflection.PropertyInfo p in Model.GetType().GetProperties())
                {
                    object Val = p.GetValue(Model, null);
                    if (bo.Schema.ContainsField(p.Name) && Val != null)
                    {
                        bo[p.Name] = Val;
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
                    user.UnitID,
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
                        finishStart,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.SendMessage(startInstanceMessage);
                string sql = string.Format("select sequenceno from ot_instancecontext where objectid='{0}'", InstanceId);
                DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.Query.QueryTable(sql);
                string url = "http://172.16.7.96:8010/Portal/index.html#/InstanceDetail/";
                url = url + InstanceId + "///";
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", url);
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// 获取最新的流程模板
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
        {
            // 获取最新版本号
            int workflowVersion = OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode);
            return GetWorkflowTemplate(workflowCode, workflowVersion);
        }
        /// <summary>
        /// 获取指定版本号的流程模板对象
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="workflowVersion">流程模板版本号</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            // 获取模板
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = OThinker.H3.Controllers.AppUtility.Engine.WorkflowManager.GetPublishedTemplateHeader(
                    workflowCode,
                    workflowVersion);
            return workflowTemplate;
        }











        /// <summary>
        /// 启动H3流程实例
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
                //string user =  GetUserIDByCode(userCode);  //"liuwanying@dongzhengafc.com";
                OThinker.Organization.User user = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
                if (user == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,用户{" + userCode + "}不存在。");
                    return result;
                }

                //if (EntityParamValues == "null")
                //{
                //    result = new BPMServiceResult(false, "流程启动失败,申请号不存在。");
                //    return result;
                //}

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
                                OThinker.H3.DataModel.BizObjectSchema childSchema = schema.GetProperty(jo.Key).ChildSchema;
                                OThinker.H3.DataModel.BizObject[] bizObjects = new OThinker.H3.DataModel.BizObject[list.Count];
                                int i = 0;
                                foreach (object objT in list)
                                {
                                    bizObjects[i] = new OThinker.H3.DataModel.BizObject(OThinker.H3.Controllers.AppUtility.Engine, childSchema, userCode);
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
                    userCode,
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
                        finishStart,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.SendMessage(startInstanceMessage);
                string sqlW = string.Format("select objectid  from ot_workitem where instanceid='{0}'", InstanceId);
                string strl = OThinker.H3.Controllers.AppUtility.Engine.Query.CommandFactory.CreateCommand().ExecuteScalar(sqlW).ToString();


                //   DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.Query.QueryTable(sql);
                // string url = "http://172.16.7.96:8010/Portal/index.html#/InstanceDetail/";



                string url = "http://localhost:8010/Portal/MvcDefaultSheet.aspx?SheetCode=SMortgage&Mode=Work&WorkItemID=" + strl;
                // url = url + InstanceId + "///";
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", url);
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex + string.Empty);
            }
            return result;
        }

        private BPMServiceResult startWorkflowBPM(
                string workflowCode,
                string userCode,
                bool finishStart,
          List<DataItemParam> paramValues)
        {
            //ValidateSoapHeader();
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
                OThinker.Organization.User user = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
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

                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in paramValues)
                    {
                        if (string.IsNullOrEmpty(param.ItemValue + string.Empty)) continue;
                        if (bo.Schema.ContainsField(param.ItemName))
                        {
                            if (!bo.Schema.Fields.Any(s => s.Name == param.ItemName)) continue;
                            OThinker.H3.DataModel.FieldSchema field = bo.Schema.Fields.Single(s => s.Name == param.ItemName);
                            if (field.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant)
                            {

                                //OThinker.Organization.Unit unit = null;
                                //if (param.ItemValue.ToString().Length == 36)
                                //{
                                //    unit = this.Engine.Organization.GetUnit(param.ItemValue.ToString());
                                //}
                                OThinker.Organization.Unit unit = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(param.ItemValue.ToString());
                                if (unit != null)
                                {
                                    bo.SetValue(field.Name, unit.ObjectID);
                                }
                                else
                                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(string.Format("{0}没有查询到{1}的用户信息", param.ItemName, param.ItemValue));
                            }
                            else if (field.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                            {
                                var listUserCode = new List<string>();
                                foreach (var itemUserCode in param.ItemValue.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    OThinker.Organization.Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(itemUserCode);
                                    if (u != null)
                                    {
                                        listUserCode.Add(u.ObjectID);
                                    }
                                    else
                                    {
                                        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(string.Format("{0}没有查询到{1}的用户信息", workflowCode, itemUserCode));
                                    }
                                }
                                bo.SetValue(field.Name, listUserCode.ToArray());
                            }
                            else
                            {
                                if (field.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                                {
                                    continue;
                                }
                                else if (field.LogicType == OThinker.H3.Data.DataLogicType.Comment)
                                {
                                    continue;
                                }
                                else
                                {
                                    bo[param.ItemName] = param.ItemValue;
                                }
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
                     user.UnitID,
                     null,
                     false,
                     null,//OThinker.H3.Instance.Token.
                     null,
                     OThinker.H3.Instance.Token.UnspecifiedID
                     );
                //审批意见
                DataItemParam CommentFieldItem = new DataItemParam();
                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候给子表赋值
                    foreach (DataItemParam param in paramValues)
                    {
                        if (bo.Schema.ContainsField(param.ItemName))
                        {
                            if (!bo.Schema.Fields.Any(s => s.Name == param.ItemName)) continue;
                            OThinker.H3.DataModel.FieldSchema field = bo.Schema.Fields.Single(s => s.Name == param.ItemName);
                            if (field.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                            {
                                string jsonDt = Newtonsoft.Json.JsonConvert.SerializeObject(param.ItemValue);
                                SetSublist(InstanceId, param.ItemName, jsonDt);
                            }
                            //这里是主表附件赋值
                            //if (field.LogicType == Data.DataLogicType.Attachment)
                            //{
                            //    //取出URL，调用方法下载附件
                            //    JArray jArray = (JArray)JsonConvert.DeserializeObject(param.ItemValue.ToString());
                            //    string Urlstr = jArray[0]["Url"].ToString();
                            //    var content = DownloadFromSharedPoint(Urlstr);
                            //    SaveAttachment(bo, InstanceId, param.ItemName, param.ItemValue.ToString(), content);
                            //}
                            if (field.LogicType == OThinker.H3.Data.DataLogicType.Comment)
                            {
                                CommentFieldItem.ItemName = param.ItemName + string.Empty;
                                CommentFieldItem.ItemValue = param.ItemValue + string.Empty;
                            }
                        }
                    }
                }

                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
                //OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        emergency,
                        InstanceId,
                        null,
                        paramTables,
                        OThinker.H3.Instance.PriorityType.Normal,
                        finishStart,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.SendMessage(startInstanceMessage);
                //result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");

                OThinker.H3.Instance.InstanceContext instance = OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);



                string sqlW = string.Format("select objectid  from ot_workitem where instanceid='{0}'", InstanceId);
                object ret = null;
                int i = 0;
                while ((ret == null || ret == DBNull.Value) && i < 10)
                {
                    ret = OThinker.H3.Controllers.AppUtility.Engine.Query.CommandFactory.CreateCommand().ExecuteScalar(sqlW);
                    Thread.Sleep(100);
                }
                string strl = ret.ToString();


                //   DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.Query.QueryTable(sql);
                // string url = "http://172.16.7.96:8010/Portal/index.html#/InstanceDetail/";


                string url = OThinker.H3.Controllers.AppConfig.PortalRoot + "/WorkItemSheets.html?WorkItemID=" + strl;
                //string url = "http://172.16.7.96:8010/Portal/MvcDefaultSheet.aspx?SheetCode=SMortgage&Mode=Work&WorkItemID=" + strl;
                // url = url + InstanceId + "///";
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", url);
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
            }
            return result;
        }

        private BPMServiceResult SetSublist(string finstanceid, string subTableName, string jsondt)
        {
            var instance = OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.GetInstanceContext(finstanceid);
            var bizobjectid = instance.BizObjectId;
            var data = new OThinker.H3.Instance.InstanceData(OThinker.H3.Controllers.AppUtility.Engine, finstanceid, instance.Originator);
            var Bo = data.BizObject;
            var childSchema = data.Schema.GetProperty(subTableName).ChildSchema;
            var dic1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<Dictionary<string, object>>>>(jsondt);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(OThinker.H3.Controllers.AppUtility.Engine, childSchema, instance.Originator);
            if (dic1 == null || dic1.Count == 0)
            {
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("子表明细,编号:" + finstanceid + "");
            }
            List<OThinker.H3.DataModel.BizObject> childBoList = new List<OThinker.H3.DataModel.BizObject>();
            BPMServiceResult res = new BPMServiceResult() { Success = false };
            try
            {
                foreach (var item in dic1)
                {
                    var childBo = new OThinker.H3.DataModel.BizObject(data.Engine, childSchema, data.InstanceContext.Originator);
                    foreach (var row in item)
                    {
                        var name = "";
                        var value = "";
                        foreach (var col in row)
                        {
                            if (col.Key == "ItemName") name = col.Value == null ? "" : col.Value.ToString();
                            if (col.Key == "ItemValue") value = col.Value == null ? "" : col.Value.ToString();

                            //string dataKeyChild = col.Key;
                            //if (col.Value == null) continue;
                            //string dataValueChild = col.Value.ToString();
                            //if (childBo.Schema.GetProperty(dataKeyChild) == null) continue;
                            //if (string.IsNullOrEmpty(dataValueChild)) continue;
                            //if (childBo.Schema.Fields.Any(s => s.Name == dataKeyChild))
                            //{
                            //    OThinker.H3.DataModel.FieldSchema field = childBo.Schema.Fields.Single(s => s.Name == dataKeyChild);
                            //    if (field.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant)
                            //    {
                            //        OThinker.Organization.Unit unit = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(dataValueChild.ToString());
                            //        if (unit != null)
                            //            childBo.SetValue(dataKeyChild, unit.ObjectID);
                            //        else
                            //            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("**.." + finstanceid + "没有查询到" + dataValueChild + "的用户信息");
                            //    }
                            //    //else if (field.LogicType == Data.DataLogicType.Attachment && dataValueChild.ToString()!="")
                            //    //{
                            //    //    //取出URL，调用方法下载附件
                            //    //    JArray jArray = (JArray)JsonConvert.DeserializeObject(dataValueChild.ToString());
                            //    //    string Urlstr = jArray[0]["Url"].ToString();
                            //    //    var FilesUrl = DownloadFromSharedPoint(Urlstr);
                            //    //    dataValueChild = Newtonsoft.Json.JsonConvert.SerializeObject(jArray);
                            //    //    //附件数据项code
                            //    //    var datazb = "" + subTableName + "." + field.Name.ToString() + "";
                            //    //    //这里是子表附件得赋值
                            //    //    SaveAttachments(datazb, dataValueChild, Bo, childBo.ObjectID, FilesUrl);
                            //    //}
                            //    else if (field.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                            //    {
                            //        var listUserCode = new List<string>();
                            //        foreach (var itemUserCode in dataValueChild.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            //        {
                            //            var u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(itemUserCode) as OThinker.Organization.User;
                            //            if (u != null)
                            //            {
                            //                listUserCode.Add(u.ObjectID);
                            //            }
                            //            else
                            //            {
                            //                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("**.." + finstanceid + "没有查询到" + itemUserCode + "的用户信息");
                            //            }
                            //        }
                            //        childBo.SetValue(dataKeyChild, listUserCode.ToArray());
                            //    }
                            //    else
                            //    {
                            //        childBo.SetValue(dataKeyChild, dataValueChild);
                            //    }
                            //}
                        }
                        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value))
                            childBo.SetValue(name, value);
                    }
                    childBoList.Add(childBo);
                }
                Bo[subTableName] = childBoList.ToArray();
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("字表明细:" + childBoList.Count + ",编号:" + finstanceid + "");
                foreach (var item in childBoList)
                {
                    foreach (var dir in item.ValueTable)
                    {
                        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("字表明细:" + dir.Key + ",value:" + dir.Value + ",编号:" + finstanceid + "");
                    }
                }
                Bo.ObjectID = bizobjectid;
                try
                {
                    res.Success = Bo.Update();   //经常更新失败---？？？
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("子表更新结果:" + res.Success + ",编号:" + finstanceid + "");
                    //0.1秒后再执行--因为 子表数据丢失而添加
                    ////加入子表附件
                    //foreach (var row in dic1)
                    //{
                    //    var childBos = new OThinker.H3.DataModel.BizObject(data.Engine, childSchema, data.InstanceContext.Originator);
                    //    foreach (var col in row)
                    //    {
                    //        string dataKeyChilds = col.Key;
                    //        string dataValueChild = col.Value.ToString();
                    //        if (childBos.Schema.GetProperty(dataKeyChilds) == null) continue;
                    //        DataModel.FieldSchema fields = childBos.Schema.Fields.Single(s => s.Name == dataKeyChilds);
                    //        if (fields.LogicType != Data.DataLogicType.Attachment) continue;
                    //        if (childBos.Schema.Fields.Any(s => s.Name == dataKeyChilds))
                    //        {
                    //            if (fields.LogicType == Data.DataLogicType.Attachment)
                    //            {
                    //                //取出URL，调用方法下载附件
                    //                JArray jArray = (JArray)JsonConvert.DeserializeObject(dataValueChild.ToString());
                    //                string Urlstr = jArray[0]["Url"].ToString();
                    //                var FilesUrl = DownloadFromSharedPoint(Urlstr);
                    //                jArray[0]["Url"] = FilesUrl;
                    //                dataValueChild = Newtonsoft.Json.JsonConvert.SerializeObject(jArray);
                    //                //附件数据项code
                    //                var datazb = "" + subTableName + "." + fields.Name.ToString() + "";
                    //                //这里是子表附件得赋值
                    //                SaveAttachments(datazb, dataValueChild, Bo, childBo.ObjectID);
                    //            }
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("子表更新失败:" + ex.Message + ",编号:" + finstanceid + "");
                    throw;
                }
                if (res.Success)
                {
                    res.Message = "OK";
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
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

        //public string GetUserIDByCode(string UserCode)
        //{
        //    var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, UserCode);
        //    return CurrentUserValidator.UserID;
        //}

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}