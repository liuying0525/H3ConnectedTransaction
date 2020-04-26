using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;
using System.Web.Script.Serialization;
using WapiDZEQ;

 
namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class LoanApprovaUDlController : ControllerBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public LoanApprovaUDlController()
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
        /// 查询
        /// </summary>
        public JsonResult GetLoanApprovalInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            DataSet ds = LoanApproval(pagerInfo, JXS, StartTime, EndTime);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 数据库查询
        /// </summary>
        public DataSet LoanApproval(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@"SELECT * from (select d.sqbh ,
d.jxs jxsz,
d.SQCLTS ,
d.SQDKJE ,
(to_number( replace(d.SQDKJE,',','')) +  NVL( v.availed_limit,0))* (b.bondproportion*0.01) BZJYE ,
d.CWCLR ,
d.CWSPYJDX , 
to_char(w.RECEIVETIME,'yyyy-mm-dd hh24:mi:ss') Receivetimez,
to_char(w.FINISHTIME,'yyyy-mm-dd hh24:mi:ss') Finishtimez,
w.instanceid,
w.objectid
from I_DealerLoan d
inner join  ot_instancecontext i on i.bizobjectid=d.objectid
inner join ot_workitemfinished w on w.instanceid=i.objectid 
left join i_bond b on b.distributorcode = d.jxscode 
left join in_wfs.v_dealer_balance_limit@to_auth_wfs v on trim(v.dealer_code)=trim(d.jxscode) and trim(v.make)=trim(d.zzs)
where Participant='{0}' and  w.WORKFLOWCODE='DealerLoan' and ACTIVITYCODE = 'Activity10' ) where 1=1 ", UserID);
            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and jxsz like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format("AND Receivetimez  >= '{0}'", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format("AND Finishtimez <= '{0}'", EndTime);
            }
            #endregion

            #region 排序
            string OrderBy = "ORDER BY Finishtimez desc ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " Receivetimez " + pagerInfo.sSortDir_0.ToUpper();
                }else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " Finishtimez " + pagerInfo.sSortDir_0.ToUpper();
                }
            }

            sql += OrderBy;
            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            #endregion
            //string sql1 = "select rownum rn,a.* from (" + sql + ") a ";
            //string sqlstr = "select a1.* from ( " + sql + ") a1 where 1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;

        }
    }
}