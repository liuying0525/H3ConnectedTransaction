using OThinker.H3.Controllers;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.OracleClient;
using OThinker.H3;
/// <summary>
/// DataRisk 的摘要说明
/// </summary>

namespace DongZheng.H3.WebApi.Common.Portal
{
    public class DataRisk
    {
        public DataRisk()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
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

        public static string postDataRisk(string instanceid)
        {
            string rtn = "";
            string post_url = CommonFunction.getConfigurationAppSettingsKeyValue("dz_datarisk_url");
            string back_url = CommonFunction.getConfigurationAppSettingsKeyValue("dz_datarisk_back_url");
            string param = "{\"para\":{\"app_id\":\"" + instanceid + "_sqr\",\"url\":\"" + back_url + "\"}}";
            CommonFunction.WriteH3Log("大数据库风控模型请求开始，param：" + param);
            if (string.IsNullOrEmpty(post_url) || string.IsNullOrEmpty(back_url))
            {
                CommonFunction.WriteH3Log("大数据库风控模型请求或回调url为空！param：" + param);
            }
            else
            {
                rtn = CommonFunction.PostHttp(post_url, param, 5000);
                CommonFunction.WriteH3Log("大数据库风控模型请求结束：" + rtn);
            }
            return rtn;

        }
        public static void DataRiskBack(string stream)
        {
            CommonFunction.WriteH3Log("大数据风控回调开始，参数：" + stream);
            try
            {
                bool rtn = false;
                JavaScriptSerializer js = new JavaScriptSerializer();
                var result = js.Deserialize<drResult>(stream);
                List<DataItemParam> keyValues = new List<DataItemParam>();
                keyValues.Add(new DataItemParam()
                {
                    ItemName = "sqrdsjmx",
                    ItemValue = result.data.score
                });
                string instanceid = result.app_id.Split('_')[0];
                WorkFlowFunction wf = new WorkFlowFunction();
                string SchemaCode = "APPLICATION";// wf.getSchemaCodeByInstanceID(instanceid);
                wf.SetItemValues(SchemaCode, wf.getBizobjectByInstanceid(instanceid), keyValues, wf.GetUserIDByCode("rsfk"));
                try
                {
                    if (float.Parse(result.data.score) > 0.75)
                    {
                        string workItemId = WorkFlowFunction.getRSWorkItemIDByInstanceid(instanceid);
                        if (!CommonFunction.hasData(workItemId))
                        {
                            CommonFunction.WriteH3Log("大数据风控回调结束，当前节点不在风控，不结束流程，参数：" + stream);
                            return;
                        }
                        string Application_Number = "";
                        string UserCode = "";
                        string sql = @" select a.user_name,a.application_number 
                                from I_application a join OT_instancecontext b on a.objectid = b.bizobjectid 
                                where b.objectid = '{0}'";
                        sql = string.Format(sql, instanceid);
                        DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
                        if (CommonFunction.hasData(dt))
                        {
                            Application_Number = dt.Rows[0]["application_number"].ToString();
                            UserCode = dt.Rows[0]["user_name"].ToString();
                        }
                        //拒绝，回写CAp
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("InstanceID", instanceid + string.Empty);
                        dic.Add("Application_Number", Application_Number + string.Empty);
                        dic.Add("StatusCode", "拒绝" + string.Empty);
                        dic.Add("Approval_UserCode", UserCode + string.Empty);
                        dic.Add("Approval_Comment", "东正大数据风控模型拒绝" + string.Empty);
                        CommonFunction.ExecuteBizBus("CAPServiceNew", "ProposalApproval", dic);
                        //结束流程
                        wf.finishedInstance(instanceid);
                        CommonFunction.WriteH3Log("大数据风控回调处理结束，拒绝处理成功，" + stream);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    CommonFunction.WriteH3Log("大数据风控回调处理异常，参数：" + stream + "，错误信息：" + ex.Message);
                }
                rtn = wf.SubmitWorkItemByRongShu(instanceid, "", wf.GetUserIDByCode("rsfk"));
                CommonFunction.WriteH3Log("大数据风控回调处理结束，提交至信审完成，" + stream);
                //return rtn.ToString();
            }
            catch (Exception ex)
            {
                CommonFunction.WriteH3Log("大数据风控回调处理异常，参数：" + stream + "，错误信息：" + ex.Message);
            }
        }
        #region Class
        public partial class drResult
        {
            public string app_id { get; set; }
            public data data { get; set; }
            public string code { get; set; }
            public string message { get; set; }
        }
        public partial class data
        {
            public string score { get; set; }
        }

        #endregion

    }
}
