
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;
using System.Web.Script.Serialization;
using WapiDZEQ;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class WholesaleWorkItemController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WholesaleWorkItemController()
        {
        }


        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        public JsonResult GetWorkItemList(PagerInfo pagerInfo, string distributorName, string startDate, string endDate)
        {
            int total = 0;
            DataTable dtWorkitem = QueryWorkItemListData(pagerInfo, distributorName, startDate, endDate, ref total);


            return Json(new { RowCount = total, RowData = DistributorController.ToJson(dtWorkitem) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetWorkItemFinalsList(PagerInfo pagerInfo, string distributorName, string startDate, string endDate,string state)
        {
            int total = 0;
            DataTable dtWorkitem = QueryWorkItemFinalsListData(pagerInfo, distributorName, startDate, endDate,state, ref total);


            return Json(new { RowCount = total, RowData = DistributorController.ToJson(dtWorkitem) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetOPRWorkItemList(PagerInfo pagerInfo, string distributorName, string startDate, string endDate)
        {
            int total = 0;
            DataTable dtWorkitem = QueryOPRWorkItemListData(pagerInfo, distributorName, startDate, endDate, ref total);


            return Json(new { RowCount = total, RowData = DistributorController.ToJson(dtWorkitem) }, JsonRequestBehavior.AllowGet);

        }



        private DataTable QueryWorkItemListData(PagerInfo pagerInfo, string distributorName, string startDate, string endDate, ref int total)
        {
            string userId = this.UserValidator.UserID;
            string sql = @" select * from (select rownum rn , A.* from (select a.totaldzje
,a.totalhkje
,a.totallxje
,a.totaldzje-(a.totalhkje+a.totallxje) yqsje
,to_char(Max(b.cjjedzsj),'yyyy-mm-dd HH24:mi') hkdzsj 
,f.objectid workid
,e.objectid instanceid 
,a.jxs,to_char(f.receivetime,'yyyy-mm-dd HH24:mi') receivetime
,a.JXSH3ID
from I_Repayment a
Left join I_DZJEList b on a.objectid = b.parentobjectid
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitem f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment'  and ACTIVITYCODE = 'Market' and f.participant = '{0}' {3}
group by a.totaldzje,a.totalhkje,a.totallxje,f.objectid,e.objectid,a.jxs,f.receivetime,a.JXSH3ID  order by f.receivetime desc ) A  {4}) A where A.rn >= {1} and A.rn <={2} ";

            string sqlCount = @"select count(1) from ( select 
a.jxs,to_char(f.receivetime,'yyyy-mm-dd') receivetime
from I_Repayment a
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitem f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment' and ACTIVITYCODE = 'Market' and f.participant = '{0}' {1}
 ) A";

            string conditions = "";
            if (!string.IsNullOrEmpty(distributorName))
            {
                conditions = conditions + string.Format(@" AND a.jxs like '%{0}%'", distributorName);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.receivetime,'yyyy-mm-dd HH24:mi') >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.receivetime,'yyyy-mm-dd') <= '{0}'", endDate);
            }
            

            #region 排序

            string OrderBy = " order by  A.receivetime asc  ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " A.receivetime " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " A.hkdzsj " + pagerInfo.sSortDir_0.ToUpper();
                }
               

            }
            #endregion

            sql = string.Format(sql, userId, pagerInfo.StartIndex, pagerInfo.EndIndex, conditions,OrderBy);
            sqlCount = string.Format(sqlCount, userId, conditions);
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlCount);
            if (count !=null)
                total = Convert.ToInt32(count);

            return dt;
        }


        private DataTable QueryWorkItemFinalsListData(PagerInfo pagerInfo, string distributorName, string startDate, string endDate,string state, ref int total)
        {
            string userId = this.UserValidator.UserID;
            string sql = @" select * from (select rownum rn ,A.* from (select  A.* from (select distinct a.JXS
,a.TotalDZJE
,a.XZQSBJ QSBJ
,a.QSLX
,a.WFPBJ TotalWFPJE
,to_char(f.FINISHTIME,'yyyy-mm-dd HH24:mi') FINISHTIME
,a.OperationStates
,f.objectid workid
,e.objectid instanceid
,a.SCBCLR 
,a.SCBCLRID
,f.ACTIVITYCODE
,a.JXSH3ID
from I_Repayment a
Left join I_DZJEList b on a.objectid = b.parentobjectid
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitemfinished f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment' and (ACTIVITYCODE = 'Operate'or ACTIVITYCODE = 'Market') and f.participant = '{0}' {3} 
 ) A {4} ) A)A where A.rn >= {1} and A.rn <={2} ";

            string sqlCount = @"select count(1) from ( select 
a.jxs,to_char(f.FINISHTIME,'yyyy-mm-dd') FINISHTIME,a.OperationStates
from I_Repayment a
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitemfinished f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment' and (ACTIVITYCODE = 'Operate'or ACTIVITYCODE = 'Market') and f.participant = '{0}' {1}
 ) A";

            string conditions = "";
            if (!string.IsNullOrEmpty(distributorName))
            {
                conditions = conditions + string.Format(@" AND a.jxs like '%{0}%'", distributorName);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.FINISHTIME,'yyyy-mm-dd HH24:mi') >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.FINISHTIME,'yyyy-mm-dd') <= '{0}'", endDate);
            }
            if (!string.IsNullOrEmpty(state))
            {
                conditions = conditions + string.Format(@" AND a.OperationStates = '{0}'", state);
            }

            string OrderBy = " order by A.FINISHTIME desc  ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " A.FINISHTIME " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " A.TotalWFPJE " + pagerInfo.sSortDir_0.ToUpper();
                }
              
            }

            sql = string.Format(sql, userId, pagerInfo.StartIndex, pagerInfo.EndIndex, conditions,OrderBy);
            sqlCount = string.Format(sqlCount, userId, conditions);

            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlCount);
            if (count != null)
                total = Convert.ToInt32(count);

            return dt;
        }

        private DataTable QueryOPRWorkItemListData(PagerInfo pagerInfo, string distributorName, string startDate, string endDate,  ref int total)
        {
            string userId = this.UserValidator.UserID;
            string sql = @" select * from (select rownum rn , A.* from (select distinct a.JXS
,a.TotalDZJE
,a.XZQSBJ QSBJ
,a.QSLX
,a. WFPBJ TotalWFPJE
,to_char(f.receivetime,'yyyy-mm-dd HH24:mi') receivetime
,a.OperationStates
,f.objectid workid
,e.objectid instanceid
,a.SCBCLR 
,a.SCBCLRID
,a.JXSH3ID
from I_Repayment a
Left join I_DZJEList b on a.objectid = b.parentobjectid
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitem f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment' and ACTIVITYCODE = 'Operate' and f.participant = '{0}' {3}
 ) A {4} ) A where A.rn >= {1} and A.rn <={2} ";

            string sqlCount = @"select count(1) from ( select 
a.jxs,to_char(f.receivetime,'yyyy-mm-dd') receivetime,a.OperationStates
from I_Repayment a
join Ot_Instancecontext e on e.bizobjectid = a.objectid
join Ot_Workitem f on f.instanceid = e.objectid
where e.workflowcode = 'Repayment'  and ACTIVITYCODE = 'Operate' and f.participant = '{0}' {1}
 ) A";

            string conditions = "";
            if (!string.IsNullOrEmpty(distributorName))
            {
                conditions = conditions + string.Format(@" AND a.jxs like '%{0}%'", distributorName);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.receivetime,'yyyy-mm-dd HH24:mi') >= '{0}'", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                conditions = conditions + string.Format(@" AND to_char(f.receivetime,'yyyy-mm-dd') <= '{0}'", endDate);
            }

            string OrderBy = " order by A.receivetime asc  ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " A.receivetime " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " A.TotalWFPJE " + pagerInfo.sSortDir_0.ToUpper();
                }

            }

            sql = string.Format(sql, userId, pagerInfo.StartIndex, pagerInfo.EndIndex, conditions,OrderBy);
            sqlCount = string.Format(sqlCount, userId, conditions);

            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlCount);
            if (count != null)
                total = Convert.ToInt32(count);

            return dt;
        }


        public JsonResult GetWorkCount()
        {
            string UserID = this.UserValidator.UserID;
            return this.ExecuteFunctionRun(() =>
            {
                OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true);
                Dictionary<string, int> Extend = new Dictionary<string, int>();
               

                //= @"select count(1) counts from I_Bond where BondState='终审中'";
                string sqlDK = @"select count(1) from Ot_Workitem where WORKFLOWCODE = 'DealerLoan' and PARTICIPANT='{0}'";
                string sqlHK = @"select count(1) from Ot_Workitem where WORKFLOWCODE = 'Repayment' and PARTICIPANT='{0}'";
                string sqlSX = @"select count(1) from Ot_Circulateitem where WORKFLOWCODE = 'CreditExtension' and PARTICIPANT='{0}'";
                string sqlDKCW = @"select count(1) from Ot_Workitem where WORKFLOWCODE = 'DealerLoan' and ACTIVITYCODE='Activity10' and PARTICIPANT='{0}'";

                var DKCount = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sqlDK, UserID));
                var HKCount = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sqlHK, UserID));
                var SXCount = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sqlSX, UserID));
                var DKCWCount = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sqlDKCW, UserID));

               
                int DKCount1 = Convert.ToInt32(DKCount);
                int HKCount1 = Convert.ToInt32(HKCount);
                int SXCount1 = Convert.ToInt32(SXCount);
                int DKCWCount1 = Convert.ToInt32(DKCWCount);


                Extend["DKCount"] = DKCount1;
                Extend["HKCount"] = HKCount1;
                Extend["SXCount"] = SXCount1;
                Extend["DKCWCount"] = DKCWCount1;
                result.Extend = Extend;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
      


    }
}