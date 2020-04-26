<%@ WebService Language="C#" Class="OThinker.H3.Portal.CRMService" %>
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
    public class CRMService : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        public CRMService()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        [WebMethod(Description = "postUser")]
        public void postUser(string dep)
        {
            CRMFunction crmf = new CRMFunction();
            crmf.postUser(dep);
        }
        [WebMethod(Description = "ProposalApproval")]
        public void postOrder(string bizobjectid,string appno)
        {
            CRMFunction crmf = new CRMFunction();
            crmf.postOrder(bizobjectid, appno);
        }
        [WebMethod(Description = "ProposalApproval")]
        public void postUnFinishedWorkItemInfo(string SchemaCode, string workitemID)
        {
            CRMFunction crmf = new CRMFunction();
            crmf.postUnFinishedWorkItemInfo(SchemaCode,workitemID);
        }
        [WebMethod(Description = "ProposalApproval")]
        public void postFinishedWorkItemInfo(string SchemaCode, string activity, string workitemID)
        {
            CRMFunction crmf = new CRMFunction();
            crmf.postFinishedWorkItemInfo(SchemaCode,activity,workitemID);
        }
	[WebMethod(Description = "getObjectidByAppNo")]
        public string getObjectidByAppNo(string AppNo)
        {
            string objectid = "";
            try
            {
                string sql = "select objectid from I_APPLICATION where APPLICATION_NUMBER ='{0}'";
                sql = string.Format(sql, AppNo);
                objectid = CommonFunction.ExecuteDataTableSql(sql).Rows[0][0].ToString();
		CommonFunction.WriteH3Log("CRM：bizobjectid获取-"+objectid+"，"+AppNo);
            }
            catch (Exception ex) { CommonFunction.WriteH3Log("CRM：bizobjectid获取失败-"+AppNo); }
            return objectid;
        }
	[WebMethod(Description = "postOrderByAdmin")]
        public string postOrderByAdmin(string bizobjectid,string appno)
        {
            CRMFunction crmf = new CRMFunction();
            return crmf.postOrderByAdmin(bizobjectid, appno);
        }
    }
}
