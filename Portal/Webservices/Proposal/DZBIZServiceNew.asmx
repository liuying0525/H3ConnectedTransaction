<%@ WebService Language="C#" Class="OThinker.H3.Portal.DZBIZServiceNew" %>
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
    public class DZBIZServiceNew : System.Web.Services.WebService
    {
        WorkFlowFunctionNew wfn = new WorkFlowFunctionNew();
        public DZBIZServiceNew()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        /// <summary>
        /// 零售信审审批和放款审批分单逻辑
        /// </summary>
        /// <param name="dep">部门</param>
        /// <param name="post">岗位</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        [WebMethod(Description = "零售信审审批和放款审批分单逻辑")]
        public string BizAutoAssign(string instanceID, string activeCode, string cyz, string amount)
        {
            string userID = "";

            userID = wfn.BizAutoAssign(instanceID, activeCode, cyz, amount);
            return userID;
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

        /// <summary>
        /// 调用融数接口
        /// </summary>
        /// <param name="instanceid">流程实例ID</param>
        /// <returns></returns>
        [WebMethod(Description = "调用融数接口")]
        public string postHttp(string SchemaCode, string instanceid)
        {
            WorkFlowFunctionNew wf = new WorkFlowFunctionNew();
            return wf.postHttp(SchemaCode, instanceid);
        }
        [WebMethod(Description = "零售贷款申请路由判断")]
        public WorkFlowFunctionNew.RouteResult ApplicationRouter(string instanceID)
        {
            return new WorkFlowFunctionNew().ApplicationRouter(instanceID);
        }
        
        [WebMethod(Description = "批量查询同盾报告")]
        public void BatchQueryTdReport(string instanceID)
        {
            new WorkFlowFunctionNew().BatchQueryTdReport(instanceID);
        }

        [WebMethod(Description = "结束流程的接口")]
        public void FinishInstance(string InsID)
        {
            //结束当前流程
            new WorkFlowFunction().finishedInstance(InsID);
        }
    }
}