
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
        public const string UserCode = "98.059";
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
        public JsonResult GetLoanList(PagerInfo pagerInfo, string FrameNo, string Models, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            //loanLsit loanLsit = new loanLsit();
            DataSet ds = GetH3LoanList(pagerInfo, FrameNo, Models, Manufacturer, FrameNoNature, address, StartTime, EndTime);
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
            string sql = @"select    DEALER_CODE, UNIT_NO, MAKE, ASSET_DESC, VIN_NO, COLOR, BP_ADDRESS, to_char
                            (STOCK_DATE,'yyyy-mm-dd') AS STOCK_DATE, LOAN_STATUS,
                             to_char(MATURITY_DATE,'yyyy-mm-dd') AS MATURITY_DATE, REQUESTED_AMT 
                            from IN_WFS.v_asset_base_info v where v.vin_no ='" + vin_no + "'";
            string sql2 = @"select 
                            BILL_NO,to_char(CALC_END_DATE,'yyyy-mm-dd') AS CALC_END_DATE, TRANSACTION_AMT, VIN_NO, ASSET_CODE,
                        MAKE, ASSET_DESC  from IN_WFS.V_ASSET_FPM_INFO v where v.vin_no ='" + vin_no + "'";
            string sql3 = "select PREVIOUS_VALUE, NEW_VALUE, to_char(EVENT_TIME,'yyyy-mm-dd') AS EVENT_TIME from IN_WFS.V_VIN_UPDATE_HIS v where v.new_value ='" + vin_no + "'";
            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql2);
            DataTable dt3 = ExecuteDataTableSql(dataCode, sql3);
            return Json(new { CLJBXX = ToJson(dt), LXXQ = ToJson(dt2), CJHBG = ToJson(dt3) }, JsonRequestBehavior.AllowGet);
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

            return Json(new { result = ToJson(GethomePageData(UserCode).Tables[0]) }, JsonRequestBehavior.AllowGet);
            // return {}GethomePageData
        }

        public DataSet GethomePageData(string UserCode)
        {
            UserCode = "98.152";
            string sql = @"select      
            CREDIT_LIMIT_SYNC SXED,
            UTILIZED_LIMIT_SYNC  JZJRYYED,
            AVAILABLE_LIMIT_SYNC KYSXED, 
            TOTAL_UNITS ZKSL,
            IN_PROCESS_UNITS,
            VEHICLES_MATURING_IN_2_WEEKS KDQCL,
            DISPLAY_FEE_ACCRUED_MTD DYLX, 
            INVOICES_UNPAID WFFP, 
            SETTLED_TODAY 今天结算,
            DISPLAY_FEE_PAID_YTD DNYFLX,
            INVOICES_OVERDUE 逾期账单 ,  
            AVERAGE_UTILIZED_MTD 本月平均使用额, 
            CURR_DATE, 
            TEMP_LIMIT 原始融资额度,
            to_char(EXPIRY_DATE,'yyyy-mm-dd') SXDQR ,  
            UPDATE_DATE, 
            UAF_AMOUNT WFPJE from IN_WFS.V_ZSUMMARY_DLR v where v.dealer_code = " + UserCode;
            DataSet ds = new DataSet();
            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            ds.Tables.Add(dt);
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
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@"SELECT rownum rn,  aa.*  from （select 
                         DEALER_CODE JXS,
                         DIST_INVOICE_NO AS DKBH ,
                         UNIT_NO CLBH,
                         ASSET_DESC CX,    
                         to_char(STOCK_DATE,'yyyy-mm-dd') DKSQSJ , 
                         COLOR YS,
                         PREVIOUS_VALUE CJHLS,
                         NEW_VALUE CJHYJ1,
                         VIN_NO CJHYJ2,
                         EVENT_TIME XGSJ,
                         REQUESTED_AMT DKJE,
                         LOAN_STATUS SQZT, 
                         MAKE ZZS,
                         ADDRESS DZ,  
                         ORDER_NO,
                         VIN_UPDATE,
                         PAID_AMT
                         from IN_WFS.V_LOAN_STOCK_LIST v 
                         left join IN_WFS.V_VIN_UPDATE_HIS v2 on v2.new_value = v.vin_no
                         where v.dealer_code = '{0}' ) aa where   1=1", UserCode);
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
                if (state == "处理中")
                {
                    sql += "AND SQZT   in ('05','10','40') and PAID_AMT <= 0 ";
                }
                if (state == "已拒绝")
                {
                    sql += "AND SQZT   in ('50','30') ";
                }
                if (state == "已放款")
                {
                    sql += "AND SQZT  ='40' and PAID_AMT > 0 ";
                }
                if (state == "已售出")
                {
                    sql += "AND SQZT ='60' ";
                }
                // sql += string.Format("AND SQZT   like '%{0}%' ", state);
            }
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                sql += string.Format("AND ZZS like '%{0}%' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature))
            {
                if (FrameNoNature == "临时")
                {
                    sql += " and ORDER_NO = 'Y' and VIN_UPDATE = 'N'  ";
                }
                if (FrameNoNature == "永久")
                {
                    sql += "and ORDER_NO = 'N' and VIN_UPDATE <> 'N'  ";
                }
                // sql += string.Format("AND CJHXZ like '%{0}%' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(address))
            {
                sql += string.Format("AND DZ ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                sql += string.Format("AND DKSQSJ>to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                sql += string.Format("AND DKSQSJ<to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", endDate);

            }
            #endregion
            DataSet ds = new DataSet();
            //总行数
            DataTable count = ExecuteDataTableSql(dataCode, sql);

            //条件下的总金额
            DataTable sum = ExecuteDataTableSql(dataCode, "select sum(DKJE)  from (" + sql + ")");
            #region 排序
            if (pagerInfo.iSortCol_0 != 0)
            {
                string OrderBy = "ORDER BY ";
                if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " DKBH " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " CLBH " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " CX " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " DKSQSJ " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " YS " + pagerInfo.sSortDir_0.ToUpper();
                }
                //else if (pagerInfo.iSortCol_0 == 6)
                //{
                //    OrderBy += " CJHXZ " + pagerInfo.sSortDir_0.ToUpper();
                //}
                else if (pagerInfo.iSortCol_0 == 7)
                {
                    OrderBy += " DKJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 8)
                {
                    OrderBy += " SQZT " + pagerInfo.sSortDir_0.ToUpper();
                }

                sql += OrderBy;
            }
            #endregion
            string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = ExecuteDataTableSql(dataCode, sqlstr);
            ds.Tables.Add(count);
            ds.Tables.Add(sum);
            ds.Tables.Add(dt);
            return ds;
        }





        public DataSet GetH3LoanList(PagerInfo pagerInfo, string FrameNo, string Models, string Manufacturer, string FrameNoNature, string address, string startDate, string endDate)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@" select  rownum rn,C.objectid,PARENTOBJECTID,i.objectid as instanceid ,JXS ,DKBH , 
            CLBH ,CX ,to_char(DKSQSJ,'yyyy-mm-dd') DKSQSJ , YS , CJHXZ, CJH, DKJE,  SQZT , ZZS ,DZ 
         from I_CLXX C inner join ot_instancecontext i on   i.BIZOBJECTID= c.PARENTOBJECTID where  jxs='{0}' and SQZT='待提交' ", UserID);
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
                sql += string.Format("AND DZ  like ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                sql += string.Format("AND DKSQSJ>to_date('{0}','yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                sql += string.Format("AND DKSQSJ<to_date('{0}','yyyy-mm-dd')\r\n", endDate);

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
                string OrderBy = "ORDER BY ";

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
            #endregion
            string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlstr);
            ds.Tables.Add(count);
            ds.Tables.Add(sum);
            ds.Tables.Add(dt);
            return ds;
        }




        public DataSet GetCondition()
        {
            string sql = string.Format(@"select distinct MAKE from IN_WFS.V_DEALER_MAKE v where v.dealer_code  = '{0}'", UserCode);// --制造商
            string sql1 = string.Format(@" select COLOR,COLOR_CODE from IN_WFS.V_DEALER_MAKE_COLOR v where v.dealer_code = '{0}'", UserCode); //--颜色
            string sql2 = string.Format(@" select distinct ADDRESS from IN_WFS.V_LOAN_STOCK_LIST v where v.dealer_code = '{0}'", UserCode); //--地址";

            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            DataTable dt2 = ExecuteDataTableSql(dataCode, sql1);
            DataTable dt3 = ExecuteDataTableSql(dataCode, sql2);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);
            ds.Tables.Add(dt3);
            return ds;


        }


        public JsonResult ReturnCondition()
        {
            DataSet ds = GetCondition();
            return Json(new { Manufacturer = ToJson(ds.Tables[0]), Colour = ToJson(ds.Tables[1]), Address = ToJson(ds.Tables[2]) }, JsonRequestBehavior.AllowGet);
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
            dd.ExportReportCurrency(dt, sheetName + "贷款列表导出" + date);
            return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
        }

        public DataTable ExportH3LoanList(string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@" select  DKBH 贷款编号, 
            CLBH 车辆编号,CX 车型 ,to_char(DKSQSJ,'yyyy-mm-dd') 申请时间 , YS 颜色, CJHXZ 车架号性质, CJH 车架号, DKJE 贷款金额,  SQZT 申请状态  from I_CLXX  where  jxs='{0}' ", UserID);
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
            if (!string.IsNullOrEmpty(address) && address != "undefined")
            {
                sql += string.Format("AND DZ  like ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(StartTime) && StartTime != "undefined")
            {
                sql += string.Format("AND DKSQSJ>to_date('{0}','yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime) && EndTime != "undefined")
            {
                sql += string.Format("AND DKSQSJ<to_date('{0}','yyyy-mm-dd')\r\n", EndTime);

            }
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            #endregion
            return dt;

        }


        public DataTable ExportWFSLoanList(string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime)
        {
            string sql = string.Format(@"select 
                                            DIST_INVOICE_NO   贷款编号,
                                            UNIT_NO  车辆编号 ,
                                            ASSET_DESC  车型  ,
                                            to_char(STOCK_DATE,'yyyy-mm-dd')  贷款申请时间,
                                            COLOR 颜色 ,
                                 case when ORDER_NO = 'Y' and VIN_UPDATE = 'N' then '临时' else  '永久' end 车架号性质,
                                            VIN_NO 车架号 ,
                                            REQUESTED_AMT  贷款金额	,
                    case when (LOAN_STATUS = '05'or LOAN_STATUS = '10'or  LOAN_STATUS = '40') and PAID_AMT <= 0  then '处理中' 
                         when LOAN_STATUS = '50'or LOAN_STATUS = '30'  then '已拒绝' 
                         when LOAN_STATUS = '40' and PAID_AMT > 0  then '已放款'
                         when LOAN_STATUS = '60'  then '已放款'  end 申请状态
                         from  IN_WFS.V_LOAN_STOCK_LIST v 
                         left join IN_WFS.V_VIN_UPDATE_HIS v2 on v2.new_value = v.vin_no
                         where v.dealer_code = '{0}'", UserCode);
            #region 查询条件

            if (!string.IsNullOrEmpty(FrameNo))
            {
                sql += string.Format("AND VIN_NO like '%{0}%' ", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models))
            {
                sql += string.Format("AND ASSET_DESC like '%{0}%' ", Models);
            }
            if (!string.IsNullOrEmpty(state))
            {
                if (!string.IsNullOrEmpty(state))
                {
                    if (state == "处理中")
                    {
                        sql += "AND LOAN_STATUS   in ('05','10','40') and PAID_AMT <= 0 ";
                    }
                    if (state == "已拒绝")
                    {
                        sql += "AND LOAN_STATUS   in ('50','30') ";
                    }
                    if (state == "已放款")
                    {
                        sql += "AND LOAN_STATUS  ='40' and PAID_AMT > 0 ";
                    }
                    if (state == "已售出")
                    {
                        sql += "AND LOAN_STATUS ='60' ";
                    }
                }
            }
            if (!string.IsNullOrEmpty(Manufacturer) && Manufacturer != "undefined")
            {
                sql += string.Format("AND MAKE like '%{0}%' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature) && FrameNoNature != "undefined")
            {
                if (FrameNoNature == "临时")
                {
                    sql += " and ORDER_NO = 'Y' and VIN_UPDATE = 'N'  ";
                }
                if (FrameNoNature == "永久")
                {
                    sql += "and ORDER_NO = 'N' and VIN_UPDATE <> 'N'  ";
                }
                // sql += string.Format("AND CJHXZ like '%{0}%' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(address) && address != "undefined")
            {
                sql += string.Format("AND ADDRESS ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format("AND STOCK_DATE>to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format("AND STOCK_DATE<to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);

            }
            DataTable dt = ExecuteDataTableSql(dataCode, sql);
            #endregion
            return dt;
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