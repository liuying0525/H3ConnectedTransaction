<%@ WebService Language="C#" Class="OThinker.H3.Portal.CAPService" %>
using System;
using System.Web.Services;
using System.Data;
using OThinker.H3.Data;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 

namespace OThinker.H3.Portal
{
    /// <summary>
    /// 流程实例操作相关接口
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释 。 
    // [System.Web.Script.Services.ScriptService]
    public class CAPService : System.Web.Services.WebService
    {

        /// <summary>
        /// 
        /// </summary>
        public CAPService()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        #region CAP
        [WebMethod(Description = "ProposalApproval")]
        public string ProposalApproval(string InstanceID, string Application_Number, string StatusCode, string Approval_UserCode, string Approval_Comment)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：回写CAP开始");
            CAP_Fuc cap = new CAP_Fuc();
            string cap_state = cap.GetCAPStateCode(Application_Number);
            if (cap_state == "01")
            {
                ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
                ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
                value.applicationNo = Application_Number;
                value.Comment = "驳回";
                ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
                transResult = service.SendBackFI(value);
                this.Engine.LogWriter.Write(Application_Number + "：打回CAP草稿状态结束(01)：'" + transResult.message + "'");

                cap.SetStateTo06(Application_Number, "核准后需要取消");
            }


            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "APP"; break;
                case "拒绝": state = "DEC"; break;
                case "有条件核准": state = "APC"; break;
                case "取消": state = "CAN"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，未回写CAP");
                return "";
            }
            return Approval(InstanceID, Application_Number, Approval_Comment, Approval_UserCode, state);
        }
        [WebMethod(Description = "ProposalApprovalByFK")]
        public string ProposalApprovalByFK(string InstanceID, string applicationNumber, string ApprovalUser)
        {
            string state = "APP";
            string ApprovalComments = "风控自动通过";
            return Approval(InstanceID, applicationNumber, ApprovalComments, ApprovalUser, state);
        }

        [WebMethod(Description = "Approval")]
        public string Approval(string InstanceID, string Application_Number, string Approval_Comment, string Approval_UserCode, string Approval_StatusCode)
        {
            this.Engine.LogWriter.Write("开始回写CAP：InstanceID-->" + InstanceID + ",Application_Number-->" +
                Application_Number + ",Approval_Comment-->" + Approval_Comment + ",Approval_UserCode-->" +
                Approval_UserCode + ",Approval_StatusCode-->" + Approval_StatusCode);
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = Approval_StatusCode;
            value.applicationNumber = Application_Number;
            value.ApprovalComments = Approval_Comment;
            value.ApprovalUser = Approval_UserCode;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write("回写CAP结束:InstanceID-->" + InstanceID + ",Application_Number-->" +
                Application_Number + ",ReturnCode-->" + transResult.returnCode + ",Message-->" + transResult.message);
            return transResult.returnCode + ":" + transResult.message;
        }
        [WebMethod(Description = "SendBackFI")]
        public string SendBackFI(string SchemaCode, string applicationNo, string InstanceID, string ActivityCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：驳回BackFI：" + SchemaCode + "," + applicationNo + "," + InstanceID + "," + ActivityCode);
            if (string.IsNullOrEmpty(applicationNo))
            {
                return "applicationNo为空，不需要驳回到Cap";
            }
            if (applicationNo == "-1")
            {
                return "单号为-1，不需要驳回到Cap";
            }
            //取当前节点的所有任务
            string tokenid = "";
            string sqltoken = "select InstanceId,ActivityCode,TokenId from OT_WorkItem where InstanceId='" + InstanceID + "' and ActivityCode='" + ActivityCode + "'";
            DataTable dtworkitem = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqltoken);
            if (dtworkitem != null && dtworkitem.Rows.Count > 0)
            {
                tokenid = dtworkitem.Rows[0]["TokenId"].ToString();
            }
            this.Engine.LogWriter.Write(applicationNo + "：tokenid为" + tokenid);
            if (tokenid == "" || tokenid == "1")
            {
                return "";
            }
            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态开始");
            string ApprovalComments = "";
            string sql = "select c.text from OT_instancecontext a inner join I_" + SchemaCode + " b on A.BIZOBJECTID=B.OBJECTID inner join OT_Comment c on c.instanceid=a.objectid where a.state=2 and C.DATAFIELD='CSYJ' and b.application_number='" + applicationNo + "' order by a.createdtime desc";
            ApprovalComments = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = ApprovalComments != "" ? ApprovalComments : "驳回";
            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态意见：" + value.Comment);
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            this.Engine.LogWriter.Write(applicationNo + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "FKSQBack")]
        public string FKSQBack(string SchemaCode, string applicationNo, string InstanceID, string ActivityCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：FI输入放款信息驳回Back：" + SchemaCode + "," + applicationNo + "," + InstanceID + "," + ActivityCode);

            if (ActivityCode != "Activity48")
            {
                return "";
            }
            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态开始");
            string ApprovalComments = "FI输入放款信息驳回";
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = ApprovalComments != "" ? ApprovalComments : "驳回";
            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态意见：" + value.Comment);
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            this.Engine.LogWriter.Write(applicationNo + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "FKSQBackToNew")]
        public string FKSQBackToNew(string applicationNo)
        {
            CAP_Fuc cap = new CAP_Fuc();
            var result = cap.SetStateTo06(applicationNo, "FI输入放款信息驳回后转新的");
            return result.ToString();
        }
        [WebMethod(Description = "YYFKSQBack")]
        public string YYFKSQBack(string applicationNo, string comment)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：" + comment + "：" + applicationNo);

            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态开始");
            string ApprovalComments = comment;
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = ApprovalComments != "" ? ApprovalComments : "驳回";
            this.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态意见：" + value.Comment);
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            this.Engine.LogWriter.Write(applicationNo + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            CAP_Fuc cap = new CAP_Fuc();
            cap.SetStateTo06(applicationNo, comment + "后转新的");
            return transResult.message;
        }
        [WebMethod(Description = "UpdateDDBank")]
        public string UpdateDDBank(string applicationNumber, string AccountName, string AccountNumber, string BankName, string BranchName, string EngineNo, string VinNo)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "更新银行信息开始：" + applicationNumber + "," + AccountName + "," + AccountNumber + "," + BankName + "," + BranchName + "," + EngineNo + "," + VinNo);
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.DDBankReq value = new ReturnToCAP.DDBankReq();
            value.AccountName = AccountName;
            value.AccountNumber = AccountNumber;
            value.BankName = BankName;
            value.BranchName = BranchName;
            value.EngineNo = EngineNo;
            value.VinNo = VinNo;
            value.applicationNumber = applicationNumber;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.UpdateDDBank(value);
            this.Engine.LogWriter.Write("更新银行信息结束：'" + transResult.message + "'");
            return transResult.message;
        }

        [WebMethod(Description = "UpdateDDBank，后台取数")]
        public string UpdateDDBankNew(string applicationNumber, string InsID)
        {
            string AccountName, AccountNumber, BankName, BranchName, EngineNo, VinNo;
            string sql = @"
select det.Bankname,det.Branchname,det.Accoutname,det.AccoutNumber,det.ENGINE,det.VIN_NUMBER from I_VEHICLE_DETAIL det
left join Ot_Instancecontext con on con.bizobjectid=det.parentobjectid
where con.objectid='{0}'
";
            sql = string.Format(sql, InsID);
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count == 0)
                return "更新银行信息，InstanceID错误或不存在,InstanceID-->" + InsID;
            DataRow row = dt.Rows[0];
            AccountName = row["Accoutname"] + string.Empty;
            AccountNumber = row["AccoutNumber"] + string.Empty;
            BankName = row["BankName"] + string.Empty;
            BranchName = row["Branchname"] + string.Empty;
            EngineNo = row["ENGINE"] + string.Empty;
            VinNo = row["VIN_NUMBER"] + string.Empty;
            return UpdateDDBank(applicationNumber, AccountName, AccountNumber, BankName, BranchName, EngineNo, VinNo);
        }
        [WebMethod(Description = "CMSComplied")]
        public string CMSComplied(string instanceid, string applicationNumber, string StatusCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSComplied开始");
            var sql = "select * from ot_workitemfinished  where instanceid='" + instanceid + "' and ActivityCode='Activity17' order by tokenid desc";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ACTIONNAME"] + string.Empty == "Reject")
                {
                    this.Engine.LogWriter.Write("运营初审驳回操作无需CMSComplied回写" + applicationNumber + "，结束");
                    return "";
                }
            }
            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "12"; break;
                case "拒绝": state = "14"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，CMSComplied回写失败");
                return "";
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSComplied 参数：" + applicationNumber + "," + StatusCode + "," + state);
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.CMSCompliedReq value = new ReturnToCAP.CMSCompliedReq();
            value.applicationNumber = applicationNumber;
            value.StatusCode = state;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.CMSComplied(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：CMSComplied结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝")
            {
                WorkFlowFunction wf = new WorkFlowFunction();
                DataTable data = wf.getInstanceData("APPLICATION", instanceid);
                var userCode = data.Rows[0]["USER_NAME"] + string.Empty;
                ProposalApproval(instanceid, applicationNumber, "拒绝", userCode, "运营初审拒绝");
                //结束当前流程
                new WorkFlowFunction().finishedInstance(instanceid);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：审核拒绝，自动结束流程！");
            }
            return transResult.message;
        }
        [WebMethod(Description = "CMSSettled")]
        public string CMSSettled(string instanceid, string applicationNumber, string StatusCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSSettled开始");
            var sql = "select * from ot_workitemfinished  where instanceid='" + instanceid + "' and ActivityCode='Activity18' order by tokenid desc";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["ACTIONNAME"] + string.Empty == "Reject")
                {
                    this.Engine.LogWriter.Write("运营终审驳回操作无需CMSSettled回写，结束");
                    return "";
                }
            }
            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "17"; break;
                case "拒绝": state = "05"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，CMSSettled回写失败");
                return "";
            }
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.CMSSettledReq value = new ReturnToCAP.CMSSettledReq();
            value.applicationNumber = applicationNumber;
            value.StatusCode = state;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.CMSSettled(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：CMSSettled结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝")
            {
                WorkFlowFunction wf = new WorkFlowFunction();
                DataTable data = wf.getInstanceData("APPLICATION", instanceid);
                var userCode = data.Rows[0]["USER_NAME"] + string.Empty;
                ProposalApproval(instanceid, applicationNumber, "拒绝", userCode, "运营终审拒绝");
                //结束当前流程
                new WorkFlowFunction().finishedInstance(instanceid);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：审核拒绝，自动结束流程！");
            }
            return transResult.message;
        }
        [WebMethod(Description = "CMSCollectionData")]
        public string CMSCollectionData(string contractNbr)
        {
            string value = "";
            this.Engine.LogWriter.Write("CMSCollectionData开始");
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            value = service.CMSCollectionData(contractNbr);
            this.Engine.LogWriter.Write("CMSCollectionData结束：'" + value + "'");
            return value;
        }


        #endregion
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
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
            set
            {
                _Engine = value;
            }
        }
    }
}