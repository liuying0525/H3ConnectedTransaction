<%@ WebService Language="C#" Class="OThinker.H3.Portal.ForExternalWebService" %>
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
namespace OThinker.H3.Portal
{
    /// <summary>
    /// 流程实例操作相关接口
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
    // [System.Web.Script.Services.ScriptService]
    public class ForExternalWebService : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        public ForExternalWebService()
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
        [WebMethod(Description = "ProposalApproval")]
        public DataSet getApplicationInfoForSFTP(string date)
        {
            DataSet ds = new DataSet();
            string sql = @"select a.申请人,a.电话,a.证件号,a.制造商,a.品牌 from (
             select  objectid,ThaiFirstName 申请人,phoneNo 电话,IdCardNo 证件号,assetMakeName 制造商,brandName 品牌 from i_Retailapp
             union all
             select  objectid,companyNamech 申请人,phoneNo 电话,organizationCode 证件号,assetMakeName 制造商,brandName 品牌
             from i_companyapp 
             union all
             select ia.objectid,ia.application_name 申请人,iapf.PHONE_NUMBER 电话,iad.ID_CARD_NBR 证件号,ivd.ASSET_MAKE_DSC 制造商,ivd.ASSET_BRAND_DSC 品牌 
             from I_application ia join I_APPLICANT_TYPE iat on ia.objectid = iat.parentobjectid and ia.application_name = iat.name1 
             left join I_APPLICANT_DETAIL iad on  ia.objectid = iad.parentobjectid and iat.IDENTIFICATION_CODE1 = iad.IDENTIFICATION_CODE2 
             left join I_VEHICLE_DETAIL ivd on  ia.objectid = ivd.parentobjectid  and iat.IDENTIFICATION_CODE1 = ivd.IDENTIFICATION_CODE7 
             left join I_APPLICANT_PHONE_FAX iapf on  ia.objectid = iapf.parentobjectid  and iat.IDENTIFICATION_CODE1 = iapf.IDENTIFICATION_CODE5 
             ) a inner join Ot_Instancecontext b on a.objectid=b.bizobjectid
             join OT_WorkItemFinished owif on owif.instanceid=b.objectid and owif.workflowcode in ('RetailApp','CompanyApp','APPLICATION') and owif.tokenid =1
             and trunc(owif.finishtime) =  to_date('{0}','yyyy-mm-dd') 
             where  b.state not in (4,5) order by owif.finishtime";
            sql = string.Format(sql, date);
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql); ;
            ds.Tables.Add(dt);
            return ds;
        }
} 
}