using OThinker.H3.Controllers;
using ScoreCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class AddYWDataController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        private string _dealerId = "";
        private string _dealerName = "";

        public void Index()
        {
            var context = HttpContext;
            string Method = context.Request["method"] + string.Empty;
            string pId = context.Request["pId"] ?? "";
            string dealerId = context.Request["dealerId"] ?? "";
            string crmId = context.Request["crmId"] ?? "";
            if (string.IsNullOrEmpty(Method))
            {
                System.IO.Stream s = context.Request.InputStream;
                Newtonsoft.Json.Linq.JObject ret;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    ret = Newtonsoft.Json.Linq.JObject.Parse(sr.ReadToEnd());
                }
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_YWData WHERE DEALERID = '" + ret["DealerId"] + "'");
                string sql = dt == null || dt.Rows.Count == 0 ?//string.IsNullOrEmpty(ret["pId"].ToString()) ?
                        string.Format("INSERT INTO C_YWData(OBJECTID,DEALERID,DEALERNAME,\"欺诈风险\",\"投诉数\",\"拖车能力\",\"处置能力\",\"归档率\",CRMDEALERID)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", Guid.NewGuid().ToString(), ret["DealerId"], ret["DealerName"], ret["Risk"], ret["Complain"], ret["Tow"], ret["Punish"], ret["Archiving"], ret["CrmId"]) :
                        string.Format("UPDATE C_YWData SET \"欺诈风险\"='{0}',\"投诉数\"='{1}',\"拖车能力\"='{2}',\"处置能力\"='{3}',\"归档率\"='{4}',\"修改时间\"={5} WHERE DEALERID='{6}'", ret["Risk"], ret["Complain"], ret["Tow"], ret["Punish"], ret["Archiving"], "SYSDATE", ret["DealerId"]);
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                context.Response.Write(i > 0 ? "Success" : "Failed");
                //return i > 0 ? "Success" : "Failed";
            }
            else
            {
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                string sql = "SELECT * FROM DZ_V_H3_BUSINESSDATACOLLECT WHERE DEALERID='" + dealerId + "'";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    ret.Add("DEALERNAME", dt.Rows[0]["DEALERNAME"].ToString());
                    ret.Add("RISK", dt.Rows[0]["RISK"].ToString());
                    ret.Add("TOW", dt.Rows[0]["TOW"].ToString());
                    ret.Add("PUNISH", dt.Rows[0]["PUNISH"].ToString());
                    ret.Add("ARCHIVING", dt.Rows[0]["ARCHIVING"].ToString());
                    ret.Add("COMPLAIN", dt.Rows[0]["COMPLAIN"].ToString());
                    context.Response.Write(ret);
                }
            }
        }

        public Newtonsoft.Json.Linq.JArray GetDealer()
        {
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
            DataTable dt = new DataTable();
            string sql = "Select DISTINCT * FROM (SELECT \"DealerName\" as \"经销商编号\",\"DealerName\" as \"经销商名称\" FROM DEALERACCEDE UNION ALL SELECT cast(\"经销商名称\" as nvarchar2(1000)) as 经销商编号, cast(\"经销商名称\" as nvarchar2(1000)) as 经销商名称 FROM in_cms.MV_H3_DEALER_OD_INFO_CN)";
            dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                ret.Add("key", dt.Rows[i]["经销商编号"].ToString());
                ret.Add("value", dt.Rows[i]["经销商名称"].ToString());
                data.Add(ret);
            }
            return data;
        }

        private string GetGrade()
        {
            ScoreManage sm = ScoreManage.LoadGradeObject("loadfullscorestruct", "C_Grade", new StructNames(false), LoadDealerInfo, System.Configuration.ConfigurationManager.AppSettings["BPMEngine"]);
            List<Dealer> result = sm.GetGrade(true);
            if (result == null || result.Count <= 0 || result[0].ResultList.Count <= 0) return "";
            else return result[0].ResultList[0].ResultGrade;
        }

        public List<Dealer> LoadDealerInfo(List<Dealer> dealers)
        {
            dealers = new List<Dealer>();
            _dealerId = string.IsNullOrWhiteSpace(_dealerId) ? "" : _dealerId.Replace("'", "");
            _dealerName = string.IsNullOrWhiteSpace(_dealerName) ? "" : _dealerName.Replace("'", "");
            Dealer dealer = new Dealer();
            if (!string.IsNullOrWhiteSpace(_dealerId))
            {
                dealer.DealerId = _dealerId;
                DataTable table = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DealerAccede WHERE \"CrmDealerId\"='" + _dealerId + "'");
                foreach (DataRow row in table.Rows)
                {
                    dealer.DealerAccount = row["DealerAccount"].ToString();
                    dealer.DealerName = row["DealerName"].ToString();
                }
            }
            if (string.IsNullOrWhiteSpace(dealer.DealerName))
            {
                dealer.DealerName = _dealerName;
                DataTable table = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DealerAccede WHERE \"DealerName\"='" + _dealerName + "'");
                foreach (DataRow row in table.Rows)
                {
                    dealer.DealerAccount = row["DealerAccount"].ToString();
                    dealer.DealerId = row["CrmDealerId"].ToString();
                }
            }

            DataTable data = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT DISTINCT * FROM DealerData WHERE \"CrmDealerId\"='" + dealer.DealerId + "'");
            foreach (DataRow row in data.Rows)
            {
                dealer.DealerKind = row["DealerKind"].ToString();
                dealer.DealerType = row["DealerType"].ToString();
                dealer.DealerId = row["CrmDealerId"].ToString();
                DealerData tmp = new DealerData()
                {
                    ObjectId = row["ObjectID"].ToString(),
                    DealerDataType = row["Type"].ToString().ToLower() == "single" ? DealerDataType.Single : DealerDataType.Array,
                    Variable = row["Name"].ToString(),
                    Values = row["Value"] == null || row["Value"] == DBNull.Value ? "" : row["Value"]
                };
                if (tmp.DealerDataType == DealerDataType.Array)
                {
                    List<string> values = new List<string>();
                    DataTable valueTable = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DealerDataArray WHERE \"DealerDataObjectID\"='" + tmp.ObjectId + "'");
                    foreach (DataRow valueItem in valueTable.Rows)
                    {
                        values.Add(valueItem["Value"].ToString());
                    }
                    tmp.Values = values;
                }
                dealer.DataList.Add(tmp);
            }
            data = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM in_cms.MV_H3_DEALER_OD_INFO_CN WHERE \"经销商名称\"='" + dealer.DealerName + "'");
            foreach (DataRow row in data.Rows)
            {
                foreach (DataColumn column in data.Columns)
                {
                    if (column.ColumnName == "经销商名称") continue;
                    dealer.DataList.Add(new DealerData() { ObjectId = Guid.NewGuid().ToString(), DealerDataType = DealerDataType.Single, Variable = column.ColumnName, Values = row[column].ToString() });
                }
            }
            dealers.Add(dealer);
            return dealers;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}