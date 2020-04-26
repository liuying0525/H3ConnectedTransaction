<%@ WebService Language="C#" Class="OThinker.H3.Portal.RetailBusinessService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Data;
using OThinker.H3.Controllers;
using System.Data.OracleClient;
using Newtonsoft.Json;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 

namespace OThinker.H3.Portal
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RetailBusinessService : System.Web.Services.WebService
    {
        [WebMethod(Description = "获取产品类型")]
        public List<CAP_Fuc.FinancialType> GetFinancialType(string asset_condition, string type, string bp_id, string group_id, string model_cde)
        {
            return CAP_Fuc.GetFinancialType(asset_condition, bp_id, group_id, model_cde, type);
        }

        [WebMethod(Description = "H3数据写入到CAP中")]
        public CAP_Fuc.ToCapResult WriteToCAP(string biz_objectid, string plan_id, string application_no)
        {
            //增加判断，生产上发现驳回后，传过来的单号为空
            //现在是如果单号为空，再去表中查询一次，如果还是为空，说明是新发起
            //2018-10-08  14:32
            if (string.IsNullOrEmpty(application_no))
            {
                string sql = "select APPLICATION_NUMBER from I_APPLICATION where objectid='" + biz_objectid + "'";
                string app_no = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                if (!string.IsNullOrEmpty(app_no))
                {
                    AppUtility.Engine.LogWriter.Write("H3回写到CAP单号为空，但实际是-->" + app_no);
                    application_no = app_no;
                }
            }
            return CAP_Fuc.WriteDataToCAP(biz_objectid, plan_id, application_no);
        }
        
        /// <summary>
        /// 跳过附件上传节点
        /// </summary>
        /// <param name="BizObjectId"></param>
        [WebMethod(Description ="")]
        public void IgnoreFJSC(string BizObjectId)
        {
            CAP_Fuc.ExternalSysIgnore(BizObjectId);
        }

        /// <summary>
        /// 更新附件的BizObjectID
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        [WebMethod(Description = "更新附件的BizObjectID")]
        public void UpdateAttachmentBizObjectid(string InstanceID,string DataField)
        {
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceID);
            string sql = "update Ot_Attachment set bizobjectid='{1}' where bizobjectid='{0}' and datafield in ({2})";
            sql = string.Format(sql, InstanceID, context.BizObjectId, DataField);
            int num = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            AppUtility.Engine.LogWriter.Write("更新附件的BizObjectID，SQL-->" + sql + "，受影响行数-->" + num);
        }
    }
}

