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
    public class CopyScoreRuleController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            try
            {
                string ruleId = context.Request["RuleId"];
                int count = 0;
                if (!int.TryParse(context.Request["Count"], out count))
                    count = 0;
                if (context.Request["RemoveOther"] != null && context.Request["RemoveOther"].ToLower() == "true")
                {
                    OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("DELETE FROM C_RULEPROPERTY WHERE ObjectId<>'{0}'", ruleId));
                }
                DataTable table = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_RULEPROPERTY WHERE ObjectId='" + ruleId + "'");
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < count; i++)
                    {
                        string gId = Guid.NewGuid().ToString();
                        OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("INSERT INTO C_RULEPROPERTY VALUES('{0}','{1}','{2}','{3}')", gId, row["InternetType"], row["ClassName"], row["State"]));
                        DataTable sTable = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_RULENAME WHERE PID='" + row["ObjectId"] + "'");
                        foreach (DataRow sRow in sTable.Rows)
                        {
                            string sId = Guid.NewGuid().ToString();
                            OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("INSERT INTO C_RULENAME VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", sId, sRow["BClassName"], sRow["ClassName"], gId, sRow["State"], sRow["Rate"]));
                            DataTable dTable = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_RULEDETAILS WHERE PID='" + sRow["ObjectId"] + "'");
                            string expression = "";
                            foreach (DataRow dRow in dTable.Rows)
                            {
                                string dId = Guid.NewGuid().ToString();
                                expression += "{" + dId + "}+";
                                OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("INSERT INTO C_RULEDETAILS VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", dId, dRow["RuleName"], dRow["DetaileName"], dRow["Rate"], sId, dRow["State"]));
                                DataTable oTable = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_SCORE WHERE PID='" + dRow["ObjectId"] + "'");
                                foreach (DataRow oRow in oTable.Rows)
                                {
                                    OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("INSERT INTO C_SCORE VALUES('{0}','{1}','{2}','{3}','{4}')", Guid.NewGuid().ToString(), oRow["Score"], oRow["Memo"], dId, oRow["State"]));
                                }
                            }
                            OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(string.Format("INSERT INTO C_RULEEXPRESSION VALUES('{0}','{1}','{2}','{3}')", Guid.NewGuid().ToString(), sId, expression.TrimEnd('+'), 1));
                        }
                    }
                }
                context.Response.Write(1);
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());
            }
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