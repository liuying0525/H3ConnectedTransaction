using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;

namespace DongZheng.H3.WebApi.Controllers.ProcessCenter
{
    [ValidateInput(false)]
    [Xss]
    public class LoanCommitmentController : CustomController
    {
        public override string FunctionCode => "Workspace_LoanCommitment";

        // GET: LoanCommitment
        public string Index()
        {
            return "放款承诺函";
        }

        public JsonResult GetLoanCommitment(PageInfo pi)
        {
            int num = 0;
            var rows = GetLoanCommitmentData(pi, ref num);
            return Json(new
            {
                Rows = rows,
                Total = num,
                iTotalDisplayRecords = num,
                iTotalRecords = num,
                sEcho = pi.sEcho
            }, JsonRequestBehavior.AllowGet);
        }

        public List<object> GetLoanCommitmentData(PageInfo pi,ref int Number)
        {
            string condition = "";
            if (!string.IsNullOrEmpty(pi.keyWord))
            {
                condition += "and con.instancename like '%" + pi.keyWord + "%'";
            }
            if (!string.IsNullOrEmpty(pi.Pick))
            {
                condition += "\r\nand app.IsPicked =" + pi.Pick;
            }
            if (!string.IsNullOrEmpty(pi.StartTime))
            {
                condition += "\r\nand cir.receivetime >to_date('" + pi.StartTime + "','yyyy-mm-dd')";
            }
            if (!string.IsNullOrEmpty(pi.EndTime))
            {
                condition += "\r\nand cir.receivetime <to_date('" + pi.EndTime + "','yyyy-mm-dd')";
            }

            string sqlList = @"
SELECT * FROM (
select ROW_NUMBER() OVER ( order by cir.receivetime ) AS RowNumber_,app.objectid,cir.objectid workitemid,con.objectid instanceid,con.instancename,cir.displayname,con.originator,con.originatorname,unit.name OriginatorOUName 
,cir.receivetime,con.workflowcode,cir.creator,cir.creatorname,app.SFDBT,app.IsPicked
from ( select objectid,SFDBT,IsPicked from  i_Retailapp union all select objectid,SFDBT,IsPicked from I_Application) app
left join Ot_Instancecontext con on app.objectid=con.bizobjectid
left join (
select * from Ot_Circulateitem where (Workflowcode='RetailApp' or Workflowcode='APPLICATION') and ActivityCode='Activity67'
union all
select * from Ot_Circulateitemfinished where (Workflowcode='RetailApp' or Workflowcode='APPLICATION') and ActivityCode='Activity67'
) cir on con.objectid=cir.instanceid
left join ot_organizationunit unit on con.orgunit=unit.Objectid
where app.SFDBT=1 and con.objectid is not null
" + condition+" )D WHERE RowNumber_ >= {0} AND RowNumber_ <= {1}";
            sqlList = string.Format(sqlList, pi.iDisplayStart + 1, pi.iDisplayStart + pi.iDisplayLength);

            string sqlNum= @"
select count(1)
from ( select objectid,SFDBT,IsPicked from  i_Retailapp union all select objectid,SFDBT,IsPicked from I_Application) app
left join Ot_Instancecontext con on app.objectid=con.bizobjectid
left join (
select * from Ot_Circulateitem where (Workflowcode='RetailApp' or Workflowcode='APPLICATION') and ActivityCode='Activity67'
union all
select * from Ot_Circulateitemfinished where (Workflowcode='RetailApp' or Workflowcode='APPLICATION') and ActivityCode='Activity67'
) cir on con.objectid=cir.instanceid
where app.SFDBT=1 and con.objectid is not null
" + condition ;
            //AppUtility.Engine.LogWriter.Write("sqlList-->" + sqlList);
            //AppUtility.Engine.LogWriter.Write("sqlNum-->" + sqlNum);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlList);

            Number = Convert.ToInt32(AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlNum));

            List<object> result = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new
                {
                    DisplayName = row["displayname"],
                    InstanceId = row["instanceid"],
                    InstanceName = row["instancename"],
                    ObjectID = row["objectid"],
                    Originator = row["originator"],
                    OriginatorName = row["originatorname"],
                    OriginatorOUName = row["OriginatorOUName"],
                    ReceiveTime = Convert.ToDateTime(row["receivetime"]).ToString("yyyy-MM-dd HH:mm"),
                    WorkflowCode = row["workflowcode"],
                    CirculateCreator = row["creator"] + string.Empty,
                    CirculateCreatorName = row["creatorname"] + string.Empty,
                    Picked = (row["IsPicked"] + string.Empty) == "1" ? true : false,
                    WorkItemID=row["workitemid"]
                });
            }
            return result;
        }

        public JsonResult SetPickedByBatch(List<ItemInfo> Items)
        {
            string errmsg = "";
            foreach (var item in Items)
            {
                if (string.IsNullOrEmpty(item.ObjectID) || string.IsNullOrEmpty(item.WorkflowCode))
                    continue;

                if (!AppUtility.Engine.BizObjectManager.SetPropertyValue(item.WorkflowCode, item.ObjectID, "", "IsPicked", true))
                {
                    errmsg += "ID-->" + item.ObjectID + ",WorkflowCode-->" + item.WorkflowCode + " Pick失败;";
                }
            }
            if (errmsg == "")
                return Json(new { Success = true, Msg = "" }, JsonRequestBehavior.AllowGet);
            else
            {
                AppUtility.Engine.LogWriter.Write("批量Pick异常失败:" + errmsg);
                return Json(new { Success = false, Msg = errmsg }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class PageInfo
    {
        public int sEcho { get; set; }
        public int iColumns { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public string keyWord { get; set; }
        public string Pick { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        //sEcho:1
        //iColumns:6
        //sColumns:,,,,,
        //iDisplayStart:0
        //iDisplayLength:10
        //mDataProp_0:ObjectID
        //mDataProp_1:InstanceName
        //mDataProp_2:DisplayName
        //mDataProp_3:ReceiveTime
        //mDataProp_4:OriginatorName
        //mDataProp_5:OriginatorOUName
        //keyWord:
        //Pick
        //StartTime
        //EndTime
    }

    public class ItemInfo
    {
        public string WorkflowCode { get; set; }
        public string ObjectID { get; set; }
    }
}