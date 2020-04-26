
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WapiDZEQ.Controllers
{
    /// <summary>
    /// CreditExtensionController 的摘要说明
    /// </summary>
    [Authorize]
    public class CreditExtensionController : OThinker.H3.Controllers.ControllerBase
    {
        public CreditExtensionController()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
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
        #region 授信管理--待办
        /// <summary>
        /// 查询--待办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="JXS">经销商</param>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <returns></returns>
        public JsonResult GetCreditUdInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            DataSet ds = CreditUdInfo(pagerInfo, JXS, StartTime, EndTime);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询语句
        /// </summary>
        public DataSet CreditUdInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            //获取经销商code
            string codesql = string.Format(@"select    
            distinct C.JXSCODE JXSCODE
            from I_CREDITEXTENSION C
            inner join ot_instancecontext i on i.bizobjectid=c.objectid 
            inner join OT_WorkItem w on w.instanceid=i.objectid
            where w.WORKFLOWCODE='CreditExtension'and Participant='{0}' ", UserID);
            DataTable codedt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(codesql);
            for (int i = 0; i < codedt.Rows.Count; i++) {
                var UserCode = codedt.Rows[i]["JXSCODE"].ToString();
                string flag = new CurrencyClass().CheckTime(UserCode);
                if (flag == "1")
                    new CurrencyClass().SUMMARY(UserCode);
            }

            string sql = string.Format(@"select * from (select  distinct  
            w.OBJECTID w_obj,  
            c.OBJECTID c_obj,
            Z.CREDIT_LIMIT_SYNC z_sxed,
            v.availed_limit dkye , 
            Z.AVAILABLE_LIMIT_SYNC z_kysxed , 
            Z.dealer_code z_jxscode,
            to_char(C.SXDQR,'yyyy-mm-dd hh24:mm:ss') c_sxdqr,
            C.UserCode c_jxscode,
            C.JXS c_jxs
            from I_CREDITEXTENSION C
            left join IN_WFS.V_ZSUMMARY_DLR@to_auth_wfs Z
            on trim(Z.dealer_code)=trim(C.JXSCODE)
            join  in_wfs.v_dealer_balance_limit@to_auth_wfs v on trim(v.dealer_code)=trim(C.JXSCODE) 
            inner join  ot_instancecontext i on i.bizobjectid=c.objectid 
            inner join OT_WorkItem w on w.instanceid=i.objectid
            where w.WORKFLOWCODE='CreditExtension'and Participant='{0}' ) where 1=1 ", UserID);//and  , UserID, -- Z.UTILIZED_LIMIT_SYNC z_jzjryyed ,

            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and c_jxs ='{0}'", JXS);

            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format(" AND c_sxdqr >= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format(" AND c_sxdqr < to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY c_sxdqr desc ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " c_sxdqr " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            #endregion
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;
        }
        /// <summary>
        /// 设为已读--单个
        /// </summary>
        public JsonResult SetRead(string workItemId, string obj, double sxed, double dkye, double kysxed)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@"update i_CreditExtension 
                         set SXED={0},
                         DKJE={1},
                         KYSXED = {2},
                         CLSJ = '{3}'
                         where objectid ='{4}'", sxed, dkye, kysxed, System.DateTime.Now.ToString("yyyy-MM-dd"), obj);
            new OThinker.H3.Portal.WorkItemServer().ENDWorkItem(UserID,"",workItemId,"" );//i.READSTATE = '已读' ,
            var succ = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql) > 0 ? true : false;
            return Json(new { Succ = succ }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 设为已读--批量
        /// </summary>
        public JsonResult SetAllRead(string workitemId)
        {
            try
            {
                string datetime = System.DateTime.Now.ToString("yyyy-MM-dd");
                var UserID = this.UserValidator.UserID;
                var itemid = workitemId.Split('#');
                for (int i = 0; i < itemid.Length; i++)
                {
                    var item = itemid[i].Split(';');
                    new OThinker.H3.Portal.WorkItemServer().ENDWorkItem(UserID,"",item[0],"");
                    string sql = string.Format(@"update i_CreditExtension 
                         set SXED={0},
                         DKJE={1},
                         KYSXED = {2},
                         CLSJ = '{3}'
                         where objectid ='{4}'", Convert.ToDouble(item[2]), Convert.ToDouble(item[3]), Convert.ToDouble(item[4]), datetime, item[1]);
                    var succ = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql) > 0 ? true : false;
                }
                return Json(new { Succ = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Succ = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 授信管理--已办
        /// <summary>
        /// 查询列表--已办
        /// </summary>
        public JsonResult GetCreditDoInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            DataSet ds = CreditDoInfo(pagerInfo, JXS, StartTime, EndTime);

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
        public DataSet CreditDoInfo(PagerInfo pagerInfo, string JXS, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            // Z.CREDIT_LIMIT_SYNC z_sxed,
            //Z.UTILIZED_LIMIT_SYNC z_jzjryyed ,
            //Z.AVAILABLE_LIMIT_SYNC z_kysxed , 
            string sql = string.Format(@"select * from (select distinct   
            w.OBJECTID w_obj,  
            C.SXED z_sxed,
            C.DKJE z_jzjryyed ,
            C.KYSXED z_kysxed , 
            Z.dealer_code z_jxscode,
            to_char(C.SXDQR,'yyyy-mm-dd') c_sxdqr,
            C.UserCode c_jxscode,
            C.JXS c_jxs,
            to_char(w.FINISHTIME,'yyyy-mm-dd hh24:mi:ss') c_clsj 
            from I_CREDITEXTENSION C
            left join IN_WFS.V_ZSUMMARY_DLR@to_auth_wfs Z
            on Z.dealer_code=C.JXSCODE
            inner join  ot_instancecontext i on i.bizobjectid=c.objectid 
            inner join ot_Workitemfinished w on w.instanceid=i.objectid
            where w.WORKFLOWCODE='CreditExtension' and Participant='{0}' ) where 1=1 ", UserID);

            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and c_jxs ='{0}'", JXS);

            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format(" AND c_sxdqr >= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format(" AND c_sxdqr < to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY c_clsj desc ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " c_sxdqr " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " c_CLSJ " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            #endregion
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;
        }
        #endregion
    }
}