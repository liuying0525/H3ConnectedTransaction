<%@ WebService Language="C#" Class="OThinker.H3.Portal.BizServiceByAdministrator" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OThinker.H3.DataModel;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using OThinker.Organization;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 
namespace OThinker.H3.Portal
{
    /// <summary>
    /// 流程实例操作相关接口
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
    // [System.Web.Script.Services.ScriptService]
    public class BizServiceByAdministrator : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        public BizServiceByAdministrator()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        
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

        [WebMethod(Description = "人工驳回CAP草稿状态")]
        public string SendBackFI(string applicationNo, string WorkFlowCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "人工打回CAP草稿状态开始");
            DataTable dt = WorkFlowFunction.getdkInstanceData(WorkFlowCode, applicationNo);
            string wid = WorkFlowFunction.getWorkItemIDByInstanceidAndActivityCode(dt.Rows[0]["instanceid"].ToString(), "Activity2");
            if (wid == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceid：" + dt.Rows[0]["instanceid"].ToString() + "，cap草稿状态提交异常，当前流程不在初始节点");
                return DateTime.Now.ToString() + "instanceid：" + dt.Rows[0]["instanceid"].ToString() + "，cap草稿状态提交异常，当前流程不在初始节点";
            }
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = "人工提交";
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "人工驳回CAP草稿状态")]
        public string SubmitWorkItemByInstanceID(string instanceid)
        {
            bool rtn = false;
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "提交结束");
            rtn = new WorkFlowFunction().SubmitWorkItemByInstanceID(instanceid, "提交");
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：提交结束'");
            return rtn.ToString();
        }
        [WebMethod(Description = "ProposalApproval")]
        public string ProposalApproval(string SchemaCode, string applicationNumber, string StatusCode, string ApprovalUser)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态'" + StatusCode + "'回写CAP开始：" + SchemaCode + "," + applicationNumber + "," + ApprovalUser);
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
            string ApprovalComments = "风控自动通过";
            //string sql = "select * from (select  text from OT_Comment where instanceid='" + instanceid + "'  order by CreatedTime desc) where rownum=1";
            //string sql = "select c.text,a.objectid from OT_instancecontext a inner join I_" + SchemaCode + " b on A.BIZOBJECTID=B.OBJECTID inner join OT_Comment c on c.instanceid=a.objectid where a.state=2 and C.DATAFIELD='CSYJ' and b.applicationno='" + applicationNumber + "' order by a.createdtime desc";
            //DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            //ApprovalComments = dt.Rows[0][0].ToString();
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = state;
            value.applicationNumber = applicationNumber;
            value.ApprovalComments = ApprovalComments;
            value.ApprovalUser = ApprovalUser;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态回写CAP结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝" || StatusCode == "取消")
            {
                //结束当前流程
                //new WorkFlowFunction().finishedInstance(dt.Rows[0][1].ToString());
            }
            return transResult.message;
        }
        [WebMethod(Description = "ProposalApprovalByFK")]
        public string ProposalApprovalByFK(string SchemaCode, string applicationNumber, string ApprovalUser)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态'APP'回写CAP开始：" + SchemaCode + "," + applicationNumber + "," + ApprovalUser);
            string state = "APP";

            string ApprovalComments = "风控自动通过";
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = state;
            value.applicationNumber = applicationNumber;
            value.ApprovalComments = ApprovalComments;
            value.ApprovalUser = ApprovalUser;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：风控自动通过状态回写CAP结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "ProposalApprovalFinnishedInstance")]
        public string ProposalApprovalFinnishedInstance(string SchemaCode, string applicationNumber, string StatusCode, string ApprovalUser)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态'" + StatusCode + "'回写CAP开始：" + SchemaCode + "," + applicationNumber + "," + ApprovalUser);
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
            string ApprovalComments = "";
            //string sql = "select * from (select  text from OT_Comment where instanceid='" + instanceid + "'  order by CreatedTime desc) where rownum=1";
            string sql = "select c.text,a.objectid from OT_instancecontext a inner join I_" + SchemaCode + " b on A.BIZOBJECTID=B.OBJECTID inner join OT_Comment c on c.instanceid=a.objectid where  C.DATAFIELD='ZSYJj' and b.applicationno='" + applicationNumber + "' order by a.createdtime desc";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            ApprovalComments = dt.Rows[0][0].ToString();
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = state;
            value.applicationNumber = applicationNumber;
            value.ApprovalComments = ApprovalComments;
            value.ApprovalUser = ApprovalUser;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态回写CAP结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝" || StatusCode == "取消")
            {
                //结束当前流程
                new WorkFlowFunction().finishedInstance(dt.Rows[0][1].ToString());
            }
            return transResult.message;
        }
    }

    public class Employee
    {
        public Employee() { }
        public Employee(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 身份验证类
    /// </summary>
    public class Authentication : System.Web.Services.Protocols.SoapHeader
    {
        public Authentication() { }
        public Authentication(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }


    /// <summary>
    /// 提交任务后返回对象
    /// </summary>
    [Serializable]
    public class ReturnWorkItemInfo
    {
        public ReturnWorkItemInfo() { }
        private bool isSuccess = false;
        /// <summary>
        /// 是否提交成功
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
            set { this.isSuccess = value; }
        }
        private string workItemUrl = string.Empty;
        /// <summary>
        /// 当前表单地址
        /// </summary>
        public string WorkItemUrl
        {
            get { return workItemUrl; }
            set { this.workItemUrl = value; }
        }
    }
}