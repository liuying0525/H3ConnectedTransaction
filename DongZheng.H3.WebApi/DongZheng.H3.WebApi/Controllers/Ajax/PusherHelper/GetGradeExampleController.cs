using Newtonsoft.Json;
using ScoreCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class GetGradeExampleController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";
        private string _dealerId = "";
        private string _dealerName = "";

        public void Index()
        {
            var context = HttpContext;
            _dealerId = context.Request["DealerId"];
            _dealerName = context.Request["DealerName"];
            ScoreManage sm = ScoreManage.LoadGradeObject("loadfullscorestruct", "C_Grade", new StructNames(false), LoadDealerInfo, System.Configuration.ConfigurationManager.AppSettings["BPMEngine"]);
            context.Response.Write(JsonConvert.SerializeObject(sm.GetGrade()));
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