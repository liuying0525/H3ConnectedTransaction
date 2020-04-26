
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
using OThinker.H3.Portal;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class LoanApprovalUdCWController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LoanApprovalUdCWController()
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
        /// <summary>
        /// 查询列表
        /// </summary>
        public JsonResult GetLoanApprovalUnCW(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            DataSet ds = LoanApprovalUnCWInfo(pagerInfo, JXS, StartTime, EndTime);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 查询语句
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="JXS"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public DataSet LoanApprovalUnCWInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            var DepartmentName = this.UserValidator.DepartmentName;
            //var activity = string.Empty;
            //if (DepartmentName == "市场部")
            //{
            //    activity = "Activity2";
            //}
            //if (DepartmentName == "财务部")
            //{
            //    activity = "Activity10";
            //}
            string sql = string.Format(@"Select * from (select 
                w.OBJECTID  工作项ID,
                d.sqbh 贷款申请编号,
                d.jxs 经销商,
                to_char(w.ReceiveTime,'yyyy-mm-dd hh24:mi:ss') 接收时间,
                to_char(w.ReceiveTime,'yyyy-mm-dd') 接收时间_天,
                d.SQCLTS 申请车辆台数,
                d.SQDKJE 贷款申请金额,
                d.BZJYE 保证金余额,
                (to_number( replace(d.SQDKJE,',','')) +  NVL( v.availed_limit,0))* (b.bondproportion*0.01) 应需保证金余额 ,
                W.instanceid,
                d.objectid 贷款表ID
                from I_DealerLoan  D
                inner join  ot_instancecontext i on i.bizobjectid=d.objectid 
                inner join ot_workitem w on w.instanceid=i.objectid
                left join i_bond b on b.distributorcode = d.jxscode 
                left join in_wfs.v_dealer_balance_limit@to_auth_wfs v on trim(v.dealer_code)=trim(d.jxscode) and trim(v.make)=trim(d.zzs)
                where Participant='{0}' and  w.WORKFLOWCODE='DealerLoan' and ACTIVITYCODE = 'Activity10' and  b.finalstate='已生效' ) where 1=1", UserID);
            //w.objectid O_objectid, 
            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and 经销商 like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format("AND 接收时间_天 >= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format("AND 接收时间_天 <= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY 接收时间 ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = "ORDER BY ";
                if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " 接收时间 " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1 ";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            #endregion
            //string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// 确认通过
        /// </summary>
        /// <param name="workItemId">工作项ID</param>
        /// <param name="yxbzjye">应需保证金余额</param>
        /// <param name="xbzjye">现保证金余额</param>
        /// <returns></returns>
        public JsonResult PassConfirm(string workItemId, string yxbzjye, double xbzjye, string object_id)
        {
            var UserID = this.UserValidator.UserID;
            //var succ = false;
            //string sql = string.Format("update I_DEALERLOAN set BZJYE='{0}' where OBJECTID='{1}'", xbzjye, object_id);
            //int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            //if (count > 0)
            //{
            //    succ = true;
            //}
            var UserName = this.UserValidator.UserName;
            bool state = false;
            state = new OThinker.H3.Portal.WorkItemServer().ReturnItem(UserID, workItemId, "Activity11", "DealerLoan", xbzjye, "信审初审处理中", System.DateTime.Now, UserName, "TG",0);
            return Json(new { Succ = state }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 驳回到市场
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult RejectToMarketing(double xbzjye, double yzzfbzj, string workItemId, string object_id)
        {
            var UserID = this.UserValidator.UserID;
            //var succ = false;
            //string sql = string.Format("update I_DEALERLOAN set BZJYE='{0}' where OBJECTID='{1}'", xbzjye, object_id);
            //int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            //if (count > 0)
            //{
            //    succ = true;
            //}


            var UserName = this.UserValidator.UserName;
            bool state = false;
            state = new OThinker.H3.Portal.WorkItemServer().ReturnItem(UserID, workItemId, "Activity3", "DealerLoan", xbzjye, "市场处理中", System.DateTime.Now, UserName, "BH", yzzfbzj);
            return Json(new { Succ = state }, JsonRequestBehavior.AllowGet);
        }
    }
}