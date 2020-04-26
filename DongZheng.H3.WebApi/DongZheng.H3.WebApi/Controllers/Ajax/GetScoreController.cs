using OThinker.H3.Controllers;
using ScoreCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class GetScoreController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        private string _dealerId = "";
        private string _dealerName = "";
        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string score = "";
            string action = context.Request["action"] ?? "";
            _dealerName = context.Request["dealer"] ?? "";
            _dealerId = context.Request["crmDealerId"] ?? "";
            string type = context.Request["type"] ?? "";
            string dealerType = context.Request["dealerType"] ?? "";
            if (string.IsNullOrEmpty(_dealerName) || string.IsNullOrEmpty(type))
            {
                try
                {
                    string sql = "SELECT DISTRIBUTOR,TYPE,DISTRIBUTORTYPE FROM I_ALLOWIN WHERE CRMDEALERID='" + _dealerId + "'";
                    DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    type = dt.Rows[0]["TYPE"] + string.Empty;
                    dealerType = dt.Rows[0]["DISTRIBUTORTYPE"] + string.Empty;
                    _dealerName = dt.Rows[0]["DISTRIBUTOR"] + string.Empty;
                }
                catch (Exception ex) { }
            }
            ScoreManage sm = ScoreManage.LoadGradeObject("loadfullscorestruct", "C_Grade", new StructNames(false), LoadDealerInfo, System.Configuration.ConfigurationManager.AppSettings["BPMEngine"]);
            List<Dealer> scoreList = sm.GetGrade();
            foreach (var dealer in scoreList)
            {
                foreach (var list in dealer.ResultList)
                {
                    if (type == list.DealerType && dealerType == list.DealerIntranet)
                    {
                        if (action == "Prewaring" || action == "Relegation" || action == "RiskPoint2") score = list.TotalScore.ToString();
                        else
                        {
                            foreach (var item in list.Scores)
                            {
                                if (item.ScoreName == "入网评分" && (action == "AllowIn" || action == "RiskPoint1" || action == "UpdateEnterNet")) score = item.RealScore.ToString();
                            }
                        }
                    }
                }
            }
            if (action == "RiskPoint1" && !string.IsNullOrEmpty(_dealerName))
            {
                string sql =
                    "SELECT TOTAL,ICOUNT,CASE WHEN TOTAL IS NULL OR TOTAL = 0 THEN 0 ELSE ROUND(ICOUNT*100/TOTAL,2) END RATE FROM (" +
                    "SELECT DISTINCT IA.DISTRIBUTOR AS Dealer,IA.SystemScore,IA.TYPE,IA.DISTRIBUTORTYPE,IA.PROVINCE,IA.CITY,IA.CrmDealerId," +
                    "(SELECT COUNT(1) FROM in_cms.mv_dy_application_date_info@to_dy_cms cms WHERE cms.经销商名称 = IA.DISTRIBUTOR) TOTAL," +
                    "(SELECT COUNT(1) FROM in_cms.mv_dy_application_date_info@to_dy_cms cms WHERE cms.经销商名称 = IA.DISTRIBUTOR AND TO_DATE(cms.合同到期日,'yyyy-mm-dd')< SYSDATE) ICOUNT " +
                    "FROM I_ALLOWIN IA WHERE IA.DISTRIBUTOR = '" + _dealerName + "')";
                DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                score += "|" + table.Rows[0]["RATE"] + "%";
            }
            //string ms = JsonConvert.SerializeObject(sm.GetGrade());
            context.Response.Write(score);
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
            data = ScoreManage.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM in_cms.MV_H3_DEALER_OD_INFO_CN@to_dy_cms WHERE \"经销商名称\"='" + dealer.DealerName + "'");
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