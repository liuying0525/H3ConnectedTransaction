
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
using OThinker.Organization;
using System.Data.OracleClient;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class DistributorController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DistributorController()
        {
        }

        public string Test()
        {
            return "SUCCESS";
        }

        public const string dataCode = "Wholesale";


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
        public JsonResult DeleteLoan(string ObjId)
        {
            string sql = "delete from  i_Clxx where objectid='" + ObjId + "'";
            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// h3数据
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="FrameNo"></param>
        /// <param name="Models"></param>
        /// <param name="state"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="FrameNoNature"></param>
        /// <param name="address"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetLoanList(PagerInfo pagerInfo, string FrameNo, string Models, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime, string SQZT)
        {
            //loanLsit loanLsit = new loanLsit();
            DataSet ds = GetH3LoanList(pagerInfo, FrameNo, Models, Manufacturer, FrameNoNature, address, StartTime, EndTime, SQZT);
            int _RowCount = ds.Tables[0].Rows.Count;
            string TotalAmount = ds.Tables[1].Rows[0][0] + string.Empty;
            // TotalAmount = String.Format("{0:N}", TotalAmount);
            if (!string.IsNullOrEmpty(TotalAmount)) TotalAmount = decimal.Parse(TotalAmount).ToString("#,##0.00").Replace(".00", "");
            string _CarStatistics = "        车辆       " + _RowCount + "       贷款金额        " + TotalAmount;
            return Json(new { CarStatistics = _CarStatistics, RowCount = _RowCount, RowData = ToJson(ds.Tables[2]) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// wfs数据
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="FrameNo"></param>
        /// <param name="Models"></param>
        /// <param name="state"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="FrameNoNature"></param>
        /// <param name="address"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetWFSLoanList(PagerInfo pagerInfo, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            //loanLsit loanLsit = new loanLsit();
            DataSet ds = GetWFSLoan(pagerInfo, FrameNo, Models, state, Manufacturer, FrameNoNature, address, StartTime, EndTime);
            int _RowCount = ds.Tables[0].Rows.Count;
            string TotalAmount = ds.Tables[1].Rows[0][0] + string.Empty;
            // TotalAmount = String.Format("{0:N}", TotalAmount);
            if (!string.IsNullOrEmpty(TotalAmount)) TotalAmount = decimal.Parse(TotalAmount).ToString("#,##0.00").Replace(".00", "");
            string _CarStatistics = "        车辆       " + _RowCount + "       贷款金额        " + TotalAmount;
            return Json(new { CarStatistics = _CarStatistics, RowCount = _RowCount, RowData = ToJson(ds.Tables[2]) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getdetailed(string vin_no)
        {
            vin_no = vin_no.ToUpper();
            string sql = @"select V.DEALER_CODE
,V.UNIT_NO
,V.MAKE
,V.ASSET_DESC
,V.VIN_NO
,V.COLOR
,V.ADDRESS
,to_char(V.STOCK_DATE,'yyyy-mm-dd') AS STOCK_DATE
, case when V.LOAN_STATUS ='05' or V.LOAN_STATUS ='10' then '处理中'
       when V.LOAN_STATUS = '40' and V.PAID_AMT>0 then '已放款'
       when V.LOAN_STATUS = '40' and V.PAID_AMT<=0 then '处理中'
       when V.LOAN_STATUS = '50' or V.LOAN_STATUS ='30' then '已拒绝'  
       when V.LOAN_STATUS = '60'  then '已售出' 
         end LOAN_STATUS
,case when b.expiry_date <V.MATURITY_DATE then to_char(b.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end  MATURITY_DATE
,TO_CHAR(V.REQUESTED_AMT,'FM999,999,999,999,990.00') REQUESTED_AMT
,V.DIST_INVOICE_NO
,b.HKBH TRANSACTION_NO
,b.HKSPRP TRANSACTION_DATE
,b.HKZT  status_code
from IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS  v 
join (select a.* from (
select distinct V.UNIT_NO 
,V.VIN_NO 
,V.REQUESTED_AMT DKJE
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then '未到期' 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then '已到期'
        when c.states = '已拒绝' then '已拒绝'
         else '处理中' end  HKZT
,V.dealer_code 
,HKBH
,to_char(c.HKSPRP,'yyyy-mm-dd HH24:mi:ss') HKSPRP
,d.expiry_date
from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME,HKBH from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME,HKBH from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where UPPER(v.VIN_NO) = '{0}'
union
select V.UNIT_NO 
,V.VIN_NO 
,V.APPROVED_AMT DKJE
,case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then '已付清' 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then '已到期'  
       else '处理中' end HKZT
,V.dealer_code 
,HKBH
,nvl(to_char(C.HKSPRP,'yyyy-mm-dd HH24:mi:ss'),to_char(v.SETTLEMENT_DATE,'yyyy-mm-dd HH24:mi:ss')） HKSPRP
,d.expiry_date
from IN_WFS.V_SALES_WFS_DT@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
join IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v2 on v2.dealer_code = v.dealer_code and v2.UNIT_NO = v.UNIT_NO and v2.VIN_NO = v.VIN_NO
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME,HKBH from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME,HKBH from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where UPPER(a.cjh) ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where UPPER(v.VIN_NO) = '{0}' and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO )) a)b on trim(v.dealer_code)=trim(b.dealer_code) and UPPER(v.VIN_NO)=UPPER(b.VIN_NO) and trim(v.UNIT_NO) = trim(b.UNIT_NO)";
            sql = string.Format(sql, vin_no);
            string sql3 = "select PREVIOUS_VALUE, NEW_VALUE, to_char(EVENT_TIME,'yyyy-mm-dd HH24:mi:ss') AS EVENT_TIME from IN_WFS.V_VIN_UPDATE_HIS v where UPPer(v.new_value) ='" + vin_no + "'";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql); //ExecuteDataTableSql(dataCode, sql);

            // DataTable dt2 = ExecuteDataTableSql(dataCode, sql2);
            DataTable dt3 = ExecuteDataTableSql(dataCode, sql3);
            return Json(new { CLJBXX = ToJson(dt), CJHBG = ToJson(dt3) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetloanDelList(string[] param)
        {
            string obj = string.Empty;
            for (int i = 0; i < param.Length; i++)
            {
                obj += "'" + param[i] + "',";
            }
            obj = obj.Substring(0, obj.Length - 1);

            string sql = string.Format("select rownum as xh ,cx, YS,cjhxz,cjh,DKJE from I_clxx where objectid in(" + obj + ")");
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            return Json(new { result = ToJson(dt) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Delloan(string[] param)
        {
            string obj = string.Empty;
            for (int i = 0; i < param.Length; i++)
            {
                obj += "'" + param[i] + "',";
            }
            obj = obj.Substring(0, obj.Length - 1);
            string sql = string.Format("delete from I_clxx where objectid in(" + obj + ")");
            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            return Json(new { result = count }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult HomePage(string UserCode)
        {
            int count1 = 0;
            int count2 = 0;

            UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string flag = CheckTime(UserCode);
            if (flag=="1")
             SUMMARY();
            
            return Json(new { result = ToJson(GethomePageData(UserCode, ref count1, ref count2).Tables[0]),Count1 = count1,Count2 = count2 }, JsonRequestBehavior.AllowGet);
            // return {}GethomePageData
        }

        public string CheckTime(string UserCode)
        {
            string sql = "select  case when UPD_TIME+ 2/(24*60)<=sysdate then '1' else '0' end flag   from IN_WFS.V_SUMMARY_UPD_TIME v where v.dealer_code=" + UserCode;

           DataTable dt =   ExecuteDataTableSql(dataCode, sql);

            if(dt!=null&&dt.Rows!=null&&dt.Rows.Count>0)
            {
                return dt.Rows[0][0].ToString();
            }

            return "1";
        }

        //当年已付利息详细
        public JsonResult GetInterestBill(string param)
        {
            DataSet ds = InterestBill(param);
            int _RowCount = ds.Tables[0].Rows.Count;
            string TotalAmount = ds.Tables[2].Rows[0][0] + string.Empty;
            DataTable dt = ds.Tables[1];

            return Json(new { RowCount = _RowCount, RowData = ToJson(dt), Total = TotalAmount }, JsonRequestBehavior.AllowGet);

        }
        //授信信息明细
        public JsonResult GetCreditInformation( string wfpje,string dates)
        {
            DataSet ds = CreditInformation(wfpje,dates);
            int _RowCount = ds.Tables[0].Rows.Count;
            //string TotalAmount = ds.Tables[1].Rows[0][0] + string.Empty;
            DataTable dt = ds.Tables[0];
           string HZRZTotal = ds.Tables[1].Rows[0][0] + string.Empty;
           string YSXTotal = ds.Tables[1].Rows[0][1] + string.Empty;
           string KSYTotal = ds.Tables[1].Rows[0][2] + string.Empty;
           return Json(new { RowCount = _RowCount, RowData = ToJson(dt), HZRZTotal = HZRZTotal, YSXTotal = YSXTotal, KSYTotal = KSYTotal }, JsonRequestBehavior.AllowGet);

        }

        //未分配详细
        public JsonResult GetUnassigned()
        {
            DataSet ds = Unassigned();
            int _RowCount = ds.Tables[0].Rows.Count;
            string TotalAmount = ds.Tables[1].Rows[0][0] + string.Empty;
            return Json(new { RowCount = _RowCount, RowData = ToJson(ds.Tables[0]), Total = TotalAmount }, JsonRequestBehavior.AllowGet);

        }


        public DataSet GethomePageData(string UserCode,ref int count1,ref int count2)
        {
            UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = @"select      
            CREDIT_LIMIT_SYNC SXED,
            UTILIZED_LIMIT_SYNC  JZJRYYED,
            AVAILABLE_LIMIT_SYNC KYSXED, 
            TOTAL_UNITS ZKSL,
            IN_PROCESS_UNITS,
            VEHICLES_MATURING_IN_2_WEEKS KDQCL,
            DISPLAY_FEE_ACCRUED_MTD DYLX, 
            INVOICES_UNPAID, FPM_UNPAID , 
            SETTLED_TODAY 今天结算,
            DISPLAY_FEE_PAID_YTD DNYFLX,
            INVOICES_OVERDUE 逾期账单 ,  
            AVERAGE_UTILIZED_MTD 本月平均使用额, 
            to_char(CURR_DATE,'yyyy-mm-dd') CURR_DATE , 
            TEMP_LIMIT 原始融资额度,
            to_char(EXPIRY_DATE,'yyyy-mm-dd') SXDQR ,  
            UPDATE_DATE, 
            UAF_AMOUNT WFPJE,
            ASSET_OVERDUE,
            VIN_NO_UPDATED_NUM from IN_WFS.V_ZSUMMARY_DLR v where v.dealer_code = " + UserCode;


            string dpcl1 = @"select count(1) c from IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where  (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)<to_char(d.CURR_DATE,'yyyy-mm-dd') 
and c.states is null and v.dealer_code = {0}";

            string dpcl2 = @"select count(1) c from 
IN_WFS.V_SALES_WFS_DT@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
join IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v2 on v2.dealer_code = v.dealer_code and v2.UNIT_NO = v.UNIT_NO and v2.VIN_NO = v.VIN_NO
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where v.dealer_code = {0} and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO )
and  V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I' and (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null and v.PAID_AMT >0";

            dpcl1 = string.Format(dpcl1, UserCode);
            dpcl2 = string.Format(dpcl2, UserCode);

            var dpclCount = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpcl1);
            var dpclCount2 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpcl2);

             count1 = Convert.ToInt32(dpclCount) + Convert.ToInt32(dpclCount2);


             string dpcl11 = @"select count(distinct V.UNIT_NO)
from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where v.dealer_code = {0} and  (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)<to_char(ADD_MONTHS(d.CURR_DATE,1),'yyyy-mm-dd') 
and (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)>=to_char(d.CURR_DATE,'yyyy-mm-dd')
and  (c.states = '已拒绝' or c.states is null)";

             string dpc12 = @"select count(V.UNIT_NO)
from IN_WFS.V_SALES_WFS_DT@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
join IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v2 on v2.dealer_code = v.dealer_code and v2.UNIT_NO = v.UNIT_NO and v2.VIN_NO = v.VIN_NO
Left join (select a.* from (select states,a.HKSPRP,cjh,b.MODIFIEDTIME from  I_HK_List a
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}'
union
select a.states,a.HKSPRP,a.cjh,b.MODIFIEDTIME from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a 
join (
select MaX(a.MODIFIEDTIME) MODIFIEDTIME,cjh from  (
select states,b.MODIFIEDTIME,cjh from  I_HK_List a 
join i_repaymentdistributor b on a.parentobjectid = b.objectid where trim(a.JXSBM) ='{0}' 
union
select a.states,b.MODIFIEDTIME,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a  
group by cjh) b on a.cjh = b.cjh and a.MODIFIEDTIME = b.MODIFIEDTIME ) c on c.cjh = V.VIN_NO 
where v.PAID_AMT >0 and v.dealer_code = {0} and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO )
and (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)<to_char(ADD_MONTHS(d.CURR_DATE,1),'yyyy-mm-dd') 
and (case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end)>=to_char(d.CURR_DATE,'yyyy-mm-dd')
and V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I' and  c.states is null and v.PAID_AMT >0";

       
            dpcl11 = string.Format(dpcl11, UserCode);
            dpc12 = string.Format(dpc12, UserCode);
         

            var dpclCount11 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpcl11);
            var dpclCount12 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpc12);


            count2 = Convert.ToInt32(dpclCount11) + Convert.ToInt32(dpclCount12);





            DataSet ds = new DataSet();
            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            ds.Tables.Add(dt);
            return ds;
        }

        //当年已付利息详细
        public DataSet InterestBill(string year)
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = string.Format(@"SELECT rownum rn,  aa.*  from (select  AGENCY_CODE 
                    ,BILL_NO FKBH
                    ,to_char(BILL_DATE,'yyyy-mm')  YF
                    ,to_char(BILL_DATE,'yyyy-mm-DD')  RQ
                    ,to_char(BILL_DATE,'yyyy')  year
                    ,ASSET_NUM  FXCLS  
                      ,to_char(BILL_DUE_DATE,'yyyy-mm-DD')  FXDQR ,TO_CHAR("
                      + " V.\"Invoice_Amount\",'FM999,999,999,999,990.00') LXJE " +
                      @"from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%' 
                     and  STATUS_CODE ='CL' ORDER BY BILL_DATE desc) aa where  1=1", UserCode);
            //利息总计
            string Invoice_Amount = "\"Invoice_Amount\" ";
            string sql2 = string.Format(@"select  TO_CHAR(sum(" + Invoice_Amount + "),'FM999,999,999,999,990.00') Invoice_Amount from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%' and  STATUS_CODE ='CL'  ", UserCode);
            if (!string.IsNullOrEmpty(year))
            {
                sql += string.Format(" AND year = '{0}' ", year);
                sql2 += string.Format(" AND to_char(BILL_DATE,'yyyy') = '{0}' ", year);
            }

            DataSet ds = new DataSet();
            //总行数
            DataTable total = ExecuteDataTableSql(dataCode, sql);

            //string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            //int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            //sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();

            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            //利息总计
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql2);
            ds.Tables.Add(total);
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            return ds;

        }

        //未分配
        public DataSet Unassigned()
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = string.Format(@"SELECT  RECEIPT_REF SJBH, to_char(RECEIPT_DATE,'yyyy-mm-dd') RQ, RECEIPT_AMT FKJE , SETTLED_AMT YJSJE,
TO_CHAR(UAF_AMT,'FM999,999,999,999,990.00') WFPJE 

FROM IN_WFS.V_UAF_AMT_DT v where v.Agency_Code='{0}'", UserCode);

            string sql2 = string.Format(@"SELECT  sum(UAF_AMT) FROM IN_WFS.V_UAF_AMT_DT v where v.Agency_Code='{0}'", UserCode);
            DataSet ds = new DataSet();
            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql2);
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            return ds;

        }
        /// <summary>
        /// 授信信息明细
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public DataSet CreditInformation(string wfpje,string dates)
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = string.Format(@"select make, TO_CHAR(USED_LIMIT ,'FM999,999,999,999,990.00') USED_LIMIT, committed_amt, TO_CHAR(CREDIT_LIMIT_SYNC,'FM999,999,999,999,990.00') CREDIT_LIMIT_SYNC, TO_CHAR(MAKE_AVAILED_LIMIT,'FM999,999,999,999,990.00') MAKE_AVAILED_LIMIT from IN_WFS.V_DEALER_MAKE_AVAILED_LIMIT v where v.dealer_code=") + UserCode;

            string sql2 = string.Format(@"select  TO_CHAR(sum(CREDIT_LIMIT_SYNC),'FM999,999,999,999,990.00') CREDIT_LIMIT_SYNC ,TO_CHAR(sum(USED_LIMIT) ,'FM999,999,999,999,990.00') USED_LIMIT,TO_CHAR(sum(MAKE_AVAILED_LIMIT),'FM999,999,999,999,990.00') MAKE_AVAILED_LIMIT from
 IN_WFS.V_DEALER_MAKE_AVAILED_LIMIT v where v.dealer_code=") + UserCode;
            DataSet ds = new DataSet();
            DataTable dt = ExecuteDataTableSql(dataCode, sql); 
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql2); 
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            return ds;
        }


        /// <summary>
        /// WFS数据
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="FrameNo"></param>
        /// <param name="Models"></param>
        /// <param name="state"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="FrameNoNature"></param>
        /// <param name="address"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns> 
        public DataSet GetWFSLoan(PagerInfo pagerInfo, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string startDate, string endDate)
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            #region sql
            //string sql = string.Format(@"SELECT rownum rn,  aa.*  from （select 
            //             DEALER_CODE JXS,
            //             DIST_INVOICE_NO AS DKBH ,
            //             UNIT_NO CLBH,
            //             ASSET_DESC CX,    
            //             to_char(STOCK_DATE,'yyyy-mm-dd') DKSQSJ , 
            //             replace(to_char(STOCK_DATE,'yyyy-mm-dd'),'-','') px,
            //             COLOR YS,
            //             CASE  
            //             WHEN ORDER_NO = 'Y' and VIN_UPDATE = 'N' THEN '临时'
            //             WHEN ORDER_NO = 'N' and VIN_UPDATE <> 'N'  THEN '永久' END CJHXZ,
            //             PREVIOUS_VALUE CJHLS,
            //             NEW_VALUE CJHYJ1,
            //             VIN_NO CJHYJ2,
            //             to_char(EVENT_TIME,'yyyy-mm-dd') XGSJ,
            //             REQUESTED_AMT DKJE,
            //             LOAN_STATUS SQZT, 
            //             MAKE ZZS,
            //             ADDRESS DZ, 
            //             PAID_AMT
            //             from IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v 
            //             left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v2 on v2.new_value = v.vin_no
            //             where v.dealer_code = '{0}' ) aa where   1=1", UserCode);
            #endregion

            #region sql
            string sql = string.Format(@"SELECT distinct * from (
                                select  to_char(DKBH) DKBH,  
                                to_char(CLBH) CLBH ,
                                to_char( CX) CX ,
                                to_char(DKSQSJ,'yyyy-mm-dd hh24:mi:ss')    DKSQSJ , 
                                replace(to_char(DKSQSJ,'yyyy-mm-dd'),'-','') px,
                                to_char(YS)   YS ,
                                to_char(CJHXZ)  CJHXZ,
                                to_char('') CJHLS, 
                                to_char('') CJHYJ1  ,
                                to_char(CJH)  CJHYJ2, 
                                to_char('') XGSJ, 
                                to_char(DKJE) DKJE, 
                                to_char(SQZT)  SQZT , 
                                to_char(ZZS) ZZS ,
                                to_char(DZ) DZ ,
                                to_char('0')  PAID_AMT  
                                from I_CLXX A where  jxs='{0}' and  ( SQZT='处理中' or SQZT='已拒绝')
                                and not exists (
                                select * from  IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS where  dealer_code = '{1}' and VIN_NO = to_char(A.CJH)
                                )
                                union all  select distinct
                                to_char(DIST_INVOICE_NO)  
                                DKBH,to_char(UNIT_NO) CLBH,
                                to_char(ASSET_DESC) CX,
                                nvl(to_char(c.DKSQSJ,'yyyy-mm-dd hh24:mi:ss'),to_char(STOCK_DATE,'yyyy-mm-dd hh24:mi:ss'))  DKSQSJ ,
                                replace(nvl(to_char(c.DKSQSJ,'yyyy-mm-dd'),to_char(STOCK_DATE,'yyyy-mm-dd')),'-','') px,
                                to_char(COLOR) YS,CASE  
                                WHEN ORDER_NO = 'Y' and VIN_UPDATE = 'N' THEN '临时'
                                WHEN ORDER_NO = 'N' and VIN_UPDATE <> 'N'  THEN '永久' END CJHXZ, 
                                to_char(PREVIOUS_VALUE) CJHLS,
                                to_char(NEW_VALUE) CJHYJ1, 
                                to_char(VIN_NO) CJHYJ2, 
                                to_char(EVENT_TIME,'yyyy-mm-dd') XGSJ,
                                to_char(REQUESTED_AMT) DKJE,
                                case when V.LOAN_STATUS ='05' or V.LOAN_STATUS ='10' then '处理中'
                                when V.LOAN_STATUS = '40' and V.PAID_AMT>0 then '已放款'
                                when V.LOAN_STATUS = '40' and V.PAID_AMT<=0 then '处理中'
                                when V.LOAN_STATUS = '50' or V.LOAN_STATUS ='30' then '已拒绝'  
                                when V.LOAN_STATUS = '60'  then '已售出'  end  SQZT,
                                trim(MAKE) ZZS,
                                trim(ADDRESS) DZ,
                                to_char(PAID_AMT)  PAID_AMT
                                from IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v  
                                left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v2 on v2.new_value = v.vin_no and  trim(v.DIST_INVOICE_NO) = trim(v2.TRANSACTION_NO)
                                left join i_clxx c
                                on v.vin_no = c.cjh 
                                where v.dealer_code = '{1}' ) where 1=1 ", UserID, UserCode);//to_char(STOCK_DATE,'yyyy-mm-dd hh24:mi:ss') DKSQSJ,replace(to_char(STOCK_DATE,'yyyy-mm-dd'),'-','') px,

            #endregion


            #region 查询条件

            if (!string.IsNullOrEmpty(FrameNo))
            {
                sql += string.Format("AND ( CJHYJ1 like '%{0}%' OR CJHYJ2 like '%{0}%' OR CJHLS like '%{0}%' )", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models))
            {
                sql += string.Format("AND CX like '%{0}%' ", Models);
            }
            if (!string.IsNullOrEmpty(state))
            {
                //if (state == "已拒绝")
                //{
                //    sql += "AND SQZT   in ('50','30') ";
                //}
                //if (state == "已放款")
                //{
                //    sql += "AND SQZT  ='40' and PAID_AMT > 0 ";
                //}
                //if (state == "已售出")
                //{
                //    sql += "AND SQZT ='60' ";
                //}
                sql += string.Format("AND SQZT   like '%{0}%' ", state);
            }
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                sql += string.Format("AND ZZS = '{0}' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature))
            {
                sql += string.Format("AND CJHXZ = '{0}' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(address) && address != "undefined")
            {
                if (state != "处理中" && state != "")
                    sql += string.Format(" AND DZ ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                sql += string.Format("AND substr(DKSQSJ,0,10)>=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                sql += string.Format("AND substr(DKSQSJ,0,10)<=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", endDate);

            }
            #endregion
            DataSet ds = new DataSet();
            //总行数
            //DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            //条件下的总金额
            DataTable sum = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select sum(DKJE)  from (" + sql + ")");
            #region 排序
            string OrderBy = "  ORDER BY DKSQSJ DESC , CLBH desc ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " CLBH " + pagerInfo.sSortDir_0.ToUpper();
                }
                if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " DKSQSJ " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;
            #endregion
            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            //string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            //DataTable dt = ExecuteDataTableSql(dataCode, sqlstr);
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            ds.Tables.Add(count);
            ds.Tables.Add(sum);
            ds.Tables.Add(dt);
            return ds;
        }





        public DataSet GetH3LoanList(PagerInfo pagerInfo, string FrameNo, string Models, string Manufacturer, string FrameNoNature, string address, string startDate, string endDate, string SQZT)
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = string.Format(@" select C.objectid,PARENTOBJECTID,i.objectid as instanceid ,JXS ,DKBH , 
            CLBH ,CX ,to_char(DKSQSJ,'yyyy-mm-dd hh24:mi:ss') DKSQSJ , YS , CJHXZ, CJH, DKJE,  SQZT , ZZS ,DZ 
         from I_CLXX C inner join ot_instancecontext i on   i.BIZOBJECTID= c.PARENTOBJECTID where  jxs='{0}' and (SQZT='待提交' or SQZT='处理中')", UserID);
            #region 查询条件

            if (!string.IsNullOrEmpty(FrameNo))
            {
                sql += string.Format("AND ( CJH like '%{0}%' )", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models))
            {
                sql += string.Format("AND CX like '%{0}%' ", Models);
            }
            //if (!string.IsNullOrEmpty(state))
            //{
            //    sql += string.Format("AND SQZT   like '%{0}%' ", state);
            //}
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                sql += string.Format("AND ZZS like '%{0}%' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature))
            {
                sql += string.Format("AND CJHXZ like '%{0}%' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(address))
            {
                sql += string.Format("AND DZ ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                sql += string.Format("AND substr(DKSQSJ,0,10)>=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                sql += string.Format("AND substr(DKSQSJ,0,10)<=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", endDate);

            }
            if (!string.IsNullOrEmpty(SQZT))
            {
                sql += string.Format("AND SQZT  ='{0}' ", SQZT);
            }
            #endregion
            DataSet ds = new DataSet();
            //总行数
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            //条件下的总金额
            DataTable sum = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select sum(dkje)  from (" + sql + ")");
            #region 排序
            if (pagerInfo.iSortCol_0 != 0)
            {
                string OrderBy = " ORDER BY ";

                if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " CX " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " YS " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " CJHXZ " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " CJH " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " DKJE " + pagerInfo.sSortDir_0.ToUpper();
                }

                sql += OrderBy;
            }
            sql += " order by DKSQSJ desc ,CLBH desc ";
            #endregion
            string sqlstr = "select a.*,rn from (select rownum rn, a1.* from ( " + sql + ") a1) a where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlstr);
            ds.Tables.Add(count);
            ds.Tables.Add(sum);
            ds.Tables.Add(dt);
            return ds;
        }

        //贷款列表下拉框
        public JsonResult ReturnCondition()
        {
            DataSet ds = GetCondition();
            return Json(new { Manufacturer = ToJson(ds.Tables[0]), Colour = ToJson(ds.Tables[1]), Address = ToJson(ds.Tables[2]) }, JsonRequestBehavior.AllowGet);
        }


        //贷款列表下拉框
        public DataSet GetCondition()
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);
            string sql = string.Format(@"select trim(MAKE) MAKE from IN_WFS.V_DEALER_MAKE v where v.make in (
select v2.make from IN_WFS.V_DEALER_MAKE_MODEL_RATE v2 where v2.dealer_code='{0}')
and v.dealer_code ='{0}'", UserCode);// --制造商
            string sql1 = string.Format(@" select trim(COLOR) COLOR,trim(COLOR_CODE) COLOR_CODE from IN_WFS.V_DEALER_MAKE_COLOR v where v.dealer_code = '{0}'", UserCode); //--颜色
            string sql2 = string.Format(@" select distinct trim(ADDRESS) ADDRESS from IN_WFS.V_LOAN_STOCK_LIST v where v.dealer_code = '{0}'", UserCode); //--地址";

            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql1);
            DataTable dt3 = ExecuteDataTableSql(dataCode, sql2);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            ds.Tables.Add(dt3);
            return ds;


        }






        public JsonResult ExportLoan(string type, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            string sheetName = string.Empty;
            DataTable dt = new DataTable();
            if (type == "H3")
            {
                dt = ExportH3LoanList(FrameNo, Models, state, Manufacturer, FrameNoNature, address, StartTime, EndTime);
                sheetName = "待提交";
            }
            else
            {
                dt = ExportWFSLoanList(FrameNo, Models, state, Manufacturer, FrameNoNature, address, StartTime, EndTime);
            }

            string date = DateTime.Now.ToString("yyyyMMdd");
            CurrencyClass dd = new CurrencyClass();
            dd.ExportReportCurrency(dt, sheetName + "贷款列表_" + date);
            return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
        }

        public DataTable ExportH3LoanList(string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@" select CX 车型 ,YS 颜色, CJHXZ 车架号性质,CJH 车架号,DKJE 贷款金额,
                    to_char(DKSQSJ,'yyyy-mm-dd hh24:mi:ss') 贷款申请时间 ,   
                    SQZT 申请状态  from I_CLXX  where  jxs='{0}' and  SQZT='{1}' ", UserID, "待提交");//,to_char(DKSQSJ,'yyyy-mm-dd') 申请时间 
            #region 查询条件
            if (!string.IsNullOrEmpty(FrameNo) && FrameNo != "undefined")
            {
                sql += string.Format("AND ( CJH like '%{0}%')", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models) && Models != "undefined")
            {
                sql += string.Format("AND CX like '%{0}%' ", Models);
            }
            if (!string.IsNullOrEmpty(state) && state != "undefined")
            {
                sql += string.Format("AND SQZT   like '%{0}%' ", state);
            }
            if (!string.IsNullOrEmpty(Manufacturer) && Manufacturer != "undefined")
            {
                sql += string.Format("AND ZZS like '%{0}%' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature) && FrameNoNature != "undefined")
            {
                sql += string.Format("AND CJHXZ like '%{0}%' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(StartTime) && StartTime != "undefined")
            {
                sql += string.Format("AND to_char(DKSQSJ,'yyyy-mm-dd')>=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime) && EndTime != "undefined")
            {
                sql += string.Format("AND to_char(DKSQSJ,'yyyy-mm-dd')<=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);

            }
            if (!string.IsNullOrEmpty(address) && address != "undefined")
            {
                sql += string.Format("AND DZ ='{0}' ", address);
            }
            sql += " order by 贷款申请时间 desc ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            #endregion
            return dt;

        }


        public DataTable ExportWFSLoanList(string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            string UserCode = string.Empty;
            string UserID = string.Empty;
            GetUser(out UserID, out UserCode);

            #region old


            //string sql = string.Format(@"select aa.* from (select 
            //            DIST_INVOICE_NO   贷款编号,
            //            UNIT_NO  车辆编号 ,
            //            ASSET_DESC  车型  ,
            //            to_char(STOCK_DATE,'yyyy-mm-dd')  贷款申请时间,
            //            COLOR 颜色 ,
            //            case when ORDER_NO = 'Y' and VIN_UPDATE = 'N' then '临时' else  '永久' end 车架号性质,
            //            VIN_NO 车架号 ,
            //            REQUESTED_AMT  贷款金额  ,
            //            case when (LOAN_STATUS = '05'or LOAN_STATUS = '10'or  LOAN_STATUS = '40')
            //            and PAID_AMT <= 0 then '处理中' 
            //            when LOAN_STATUS = '50'or LOAN_STATUS = '30'  then '已拒绝' 
            //            when LOAN_STATUS = '40' and PAID_AMT > 0  then '已放款'
            //            when LOAN_STATUS = '60'  then '已售出'  end 申请状态,
            //            MAKE 制造商,
            //            ADDRESS 地址
            //            from  IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v 
            //            left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v2 on v2.new_value = v.vin_no
            //            where v.dealer_code ='{0}') aa where 1=1 ", UserCode);


            //if (!string.IsNullOrEmpty(state))
            //{
            //    if (state == "处理中")
            //    {
            //        #region 处理中sql

            //        //sql = string.Format(@"SELECT  aa.*  from (select  to_char(DKBH) 贷款编号, 
            //        //    to_char(CLBH) 车辆编号 ,to_char( CX) 车型 ,
            //        //    to_char(DKSQSJ,'yyyy-mm-dd')    贷款申请时间 , 
            //        //    to_char(YS)   颜色 ,
            //        //    to_char(CJHXZ)  车架号性质,
            //        //    to_char(CJH)  车架号,  
            //        //    to_char(DKJE) 贷款金额, 
            //        //    to_char(SQZT)  申请状态,
            //        //    to_char(ZZS) 制造商,
            //        //    to_char(DZ) 地址 
            //        //    from I_CLXX  where  jxs='{0}' and SQZT='处理中'
            //        //    union all 
            //        //    select
            //        //    to_char(DIST_INVOICE_NO)  贷款编号,
            //        //    to_char(UNIT_NO) 车辆编号,
            //        //    to_char(ASSET_DESC) 车型,
            //        //    to_char(STOCK_DATE,'yyyy-mm-dd') 贷款申请时间, 
            //        //    to_char(COLOR) 颜色,
            //        //    CASE  
            //        //    WHEN ORDER_NO = 'Y' and VIN_UPDATE = 'N' THEN '临时'
            //        //    WHEN ORDER_NO = 'N' and VIN_UPDATE <> 'N' THEN '永久' END 车架号性质
            //        //    ,to_char(VIN_NO) 车架号,to_char(REQUESTED_AMT) 贷款金额, 
            //        //    CASE
            //        //    when LOAN_STATUS = '50'or LOAN_STATUS = '30'  then '已拒绝' 
            //        //    when LOAN_STATUS = '40' and PAID_AMT > 0  then '已放款'
            //        //    when LOAN_STATUS = '60'  then '已售出'  end 申请状态,
            //        //    MAKE 制造商,
            //        //    ADDRESS 地址
            //        //    from IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v    
            //        //    left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v2 on v2.new_value = v.vin_no 
            //        //    where v.dealer_code = '{1}' AND
            //        //    (LOAN_STATUS  in ('05','10','40') and PAID_AMT <= 0 )  ) aa where   1=1", UserID, UserCode);

            //        #endregion
            //    }
            //}
            #endregion

            #region  new
            string sql = @"SELECT A.贷款编号, A.车辆编号,A.车型,A.贷款申请时间,A.颜色 ,A.车架号性质,A.车架号,A.贷款金额,A.申请状态 from (SELECT aa.贷款编号, aa.车辆编号,aa.车型,aa.贷款申请时间,aa.颜色 ,aa.车架号性质,aa.车架号,aa.车架号1,aa.车架号2,aa.贷款金额,aa.申请状态 from (select distinct to_char(DKBH) 贷款编号, 
                to_char(CLBH) 车辆编号 ,to_char( CX) 车型 ,
                to_char(DKSQSJ,'yyyy-mm-dd hh24:mi:ss')    贷款申请时间 , 
                to_char(YS)   颜色 ,
                to_char(CJHXZ)  车架号性质,
                to_char(CJH)  车架号, 
                ''车架号1,
                ''车架号2,
                to_char(DKJE) 贷款金额, 
                to_char(SQZT)  申请状态,
                to_char(ZZS) 制造商,
                to_char(DZ) 地址 
                from I_CLXX A where  jxs='{0}' and ( SQZT='处理中' or SQZT='已拒绝')
                and not exists (
                select * from  IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS where  dealer_code = '{1}' and VIN_NO = to_char(A.CJH)
                )
                union all 
                select distinct
                to_char(DIST_INVOICE_NO)  贷款编号,
                to_char(UNIT_NO) 车辆编号,
                to_char(ASSET_DESC) 车型,
                to_char(STOCK_DATE,'yyyy-mm-dd hh24:mi:ss') 贷款申请时间, 
                to_char(COLOR) 颜色,
                CASE  
                WHEN ORDER_NO = 'Y' and VIN_UPDATE = 'N' THEN '临时'
                WHEN ORDER_NO = 'N' and VIN_UPDATE <> 'N' THEN '永久' END 车架号性质
                ,to_char(VIN_NO) 车架号
                ,to_char(PREVIOUS_VALUE) 车架号1
                ,to_char(NEW_VALUE) 车架号2
                ,to_char(REQUESTED_AMT) 贷款金额,
                case when V.LOAN_STATUS ='05' or V.LOAN_STATUS ='10' then '处理中'
                when V.LOAN_STATUS = '40' and V.PAID_AMT>0 then '已放款'
                when V.LOAN_STATUS = '40' and V.PAID_AMT<=0 then '处理中'
                when V.LOAN_STATUS = '50' or V.LOAN_STATUS ='30' then '已拒绝'  
                when V.LOAN_STATUS = '60'  then '已售出'  end  申请状态,
                MAKE 制造商,
                ADDRESS 地址
                from IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v    
                left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v2 on v2.new_value = v.vin_no and  trim(v.DIST_INVOICE_NO)  = trim(v2.TRANSACTION_NO)
                where v.dealer_code = '{1}' ) aa where   1=1 {2})A";
            #endregion


            #region 查询条件
            string con = "";

            if (!string.IsNullOrEmpty(FrameNo) && FrameNo != "undefined")
            {
                con = con+string.Format(" AND  (车架号 like '%{0}%' or 车架号1 like '%{0}%' or 车架号2 like '%{0}%' )", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models) && Models != "undefined")
            {
                con = con + string.Format(" AND 车型 like '%{0}%' ", Models);
            }
            if (!string.IsNullOrEmpty(state) && state != "undefined")
            {
                con = con + string.Format(" AND 申请状态   = '{0}' ", state);
            }

            if (!string.IsNullOrEmpty(FrameNoNature) && FrameNoNature != "undefined")
            {
                con = con + string.Format(" AND 车架号性质 = '{0}' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(StartTime) && StartTime != "undefined")
            {
                con = con + string.Format(" AND substr(贷款申请时间,0,10)>=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime) && EndTime != "undefined")
            {
                con = con + string.Format(" AND substr(贷款申请时间,0,10)<=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);

            }
            if (!string.IsNullOrEmpty(address) && address != "undefined")
            {
                if (state != "处理中" && state != "")
                    con = con + string.Format(" AND 地址 = '{0}' ", address);
            }
            if (!string.IsNullOrEmpty(Manufacturer) && Manufacturer != "undefined")
            {
                con = con + string.Format(" AND 制造商 LIKE '%{0}%' ", Manufacturer);
            }
            #endregion

            sql = string.Format(sql, UserID, UserCode, con);

            sql += " ORDER BY A.贷款申请时间 DESC ,车辆编号 desc ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            return dt;
        }



        public void GetUser(out string UserID, out string UserCode)
        {
            UserID = this.UserValidator.UserID;
            Organization.Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(UserID);
            UserCode = ((OThinker.Organization.User)u).Appellation;

        }

        /// <summary>
        /// 汇总数据接口 
        /// </summary>
        public void SUMMARY()
        {
            string UserID, UserCode;
            GetUser(out UserID, out UserCode);

            //此连接在web.config里
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["jxsDZPORT"];
            //执行存储过程

            OracleConnection conn = new OracleConnection(connectionString);
            string storedProcedureName = "IN_WFS.ZSUMMARY_DLR";
            OracleParameter[] parameters = { new OracleParameter("p_company_code", OracleType.VarChar, 80),
                new OracleParameter("p_dealer_code", OracleType.VarChar, 80)};
            parameters[0].Value = "08615";
            parameters[1].Value = UserCode;
            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                cmd.ExecuteNonQuery();
                //string ret = cmd.Parameters["o_trans_no"].Value + string.Empty;
                //result = cmd.Parameters["o_result"].Value + string.Empty;
                //err_code = cmd.Parameters["o_err_code"].Value + string.Empty;

                conn.Close();
                //return ret;

            }
            catch (Exception ex)
            {
                //result = string.Empty;
                //err_code = string.Empty;
                conn.Close();
                //return "";

            }

        }


        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc]);
                }
                list.Add(result);
            }

            return SerializeToJson(list);
        }

        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="recursionLimit">序列化对象的深度，默认为100</param>
        /// <returns>Json字符串</returns>
        public static string SerializeToJson(object obj)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();

            return serialize.Serialize(obj);
        }


        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }
    }
}