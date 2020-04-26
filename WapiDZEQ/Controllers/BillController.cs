using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WapiDZEQ;



/// <summary>
/// RepaymentController 的摘要说明
/// </summary>
[Authorize]
public class BillController : OThinker.H3.Controllers.ControllerBase
{
    public BillController()
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

    public const string dataCode = "Wholesale";

    public System.Web.Mvc.JsonResult GetBillList(PagerInfo pagerInfo, string startYM, string endYM, string state)
    {

        DataSet ds = GetBillData(pagerInfo, startYM, endYM, state);
        int _RowCount = (ds.Tables[0].Rows[0][0] + string.Empty) != "" ? Int32.Parse(ds.Tables[0].Rows[0][0] + string.Empty) : 0;
        string totalAmt = ds.Tables[0].Rows[0][1] + string.Empty;

        // TotalAmount = String.Format("{0:N}", TotalAmount);
        if (!string.IsNullOrEmpty(totalAmt)) totalAmt = decimal.Parse(totalAmt).ToString("#,##0.00").Replace(".00", "");

        string _CarStatistics = "        利息金额       " + totalAmt;
        return Json(new { CarStatistics = _CarStatistics, RowCount = _RowCount, RowData = ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
    }

    public DataSet GetBillData(PagerInfo pagerInfo, string startYM, string endYM, string state)
    {
        var UserID = this.UserValidator.UserID;

        string sql = @" select * from  (select rownum rn ,a.* from (select  BILL_NO FKBH
,to_char(BILL_DATE,'yyyy-mm') MM
,case when to_char(BILL_DATE,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(BILL_DATE,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,to_char(BILL_DUE_DATE,'yyyy-mm-dd') FXDQR
,V.Asset_Num FXCLS" + ",V.\"Invoice_Amount\" LXJE "
 +
@",Case when v.STATUS_CODE = 'CL' then '已付'  else '未付' end  LXZDZT
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'   {1} {4}
) a ) A where rn>={2} and rn <={3}";


        #region 查询条件

        string conditions = "";

        if (!string.IsNullOrEmpty(startYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') >= '{0}' ", startYM);

        }
        if (!string.IsNullOrEmpty(endYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') <= '{0}' ", endYM);

        }
        if (!string.IsNullOrEmpty(state))
        {
            if (state == "已付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "CL");

            }
            if (state == "未付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "OP");

            }

        }


        #endregion

        #region 排序
        string OrderBy = "ORDER BY FXDQR desc ";
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY ";
            //if (pagerInfo.iSortCol_0 == 2)
            //{
            //    OrderBy += " JXJZRQ " + pagerInfo.sSortDir_0.ToUpper();
            //}
            //else 
            if (pagerInfo.iSortCol_0 == 2)
            {
                OrderBy += " FXDQR " + pagerInfo.sSortDir_0.ToUpper();
            }
        }

        #endregion

        string UserCode = this.UserValidator.User.Appellation;

        sql = string.Format(sql, UserCode, conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy);

        string sqlCount = @"select count(1) counts
,sum(LXJE) totalAmt
from  (select  BILL_NO FKBH
,to_char(BILL_DATE,'yyyy-mm') MM
,case when to_char(BILL_DATE,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(BILL_DATE,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,to_char(BILL_DUE_DATE,'yyyy-mm-dd') FXDQR
,V.Asset_Num FXCLS " + ",V.\"Invoice_Amount\" LXJE "
 +
@",Case when v.STATUS_CODE = 'CL' then '已付'  else '未付' end  LXZDZT
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'   {1}
) a ";

        sqlCount = string.Format(sqlCount, UserCode, conditions);

        DataSet ds = new DataSet();
        //总行数
        DataTable total = ExecuteDataTableSql(dataCode, sqlCount);


        DataTable dt = ExecuteDataTableSql(dataCode, sql);

        ds.Tables.Add(total);
        ds.Tables.Add(dt);
        return ds;
    }


    public System.Web.Mvc.JsonResult GetBillDTLList(PagerInfo pagerInfo, string cjh, string cx, string fkbh)
    {
        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(cjh + "  ;" + fkbh);
        DataSet ds = GetBillDTLData(pagerInfo, cjh, cx, fkbh);
        int _RowCount = (ds.Tables[0].Rows[0][0] + string.Empty) != "" ? Int32.Parse(ds.Tables[0].Rows[0][0] + string.Empty) : 0;
        string JXJZRQ = "";
        string FXCLS = "";
        string LXJE = "";
        string LXZDZT = "";
        if (ds.Tables[2].Rows.Count > 0)
        {
            JXJZRQ = ds.Tables[2].Rows[0][0] + string.Empty;
            FXCLS = ds.Tables[2].Rows[0][1] + string.Empty;
            LXJE = ds.Tables[2].Rows[0][2] + string.Empty;
            LXZDZT = ds.Tables[2].Rows[0][3] + string.Empty;
        }
        // TotalAmount = String.Format("{0:N}", TotalAmount);
        if (!string.IsNullOrEmpty(LXJE)) LXJE = decimal.Parse(LXJE).ToString("#,##0.00").Replace(".00", "");
        string _CarStatistics = "";
        if (fkbh != "null")
        {

            _CarStatistics = "        付款编号       " + fkbh + "         付款通知书时间       " + JXJZRQ + "         车辆总计      " + FXCLS + "        利息账单状态      " + LXZDZT
               + "        利息金额      " + LXJE;
        }
        else
        {
            _CarStatistics = "车辆总计      " + FXCLS + "        利息账单状态      " + LXZDZT
              + "        利息金额      " + LXJE;
        }
        return Json(new { CarStatistics = _CarStatistics, RowCount = _RowCount, RowData = ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
    }
    

    public DataSet GetBillDTLData(PagerInfo pagerInfo, string cjh, string cx, string fkbh)
    {

        string sql = @" select * from ( select rownum rn ,A.* from (select distinct  V2.UNIT_NO CLBH
,V2.ASSET_DESC CX
,V2.vin_no CJH
--,to_char(V2.CALC_END_DATE,'yyyy-mm-dd') JXJZRQ
<<<<<<< .mine
,case when V2.TRANSACTION_TYPE ='DSP' then  to_char(V3.LL)  else null end CPLR
,V2.REQUESTED_AMT DKJE
||||||| .r1089
,case when V2.TRANSACTION_TYPE ='DSP' then  V3.LL  else null end CPLR
,V.REQUESTED_AMT DKJE
=======
,case when V2.TRANSACTION_TYPE ='DSP' then  V3.LL  else null end CPLR
,V2.REQUESTED_AMT DKJE
>>>>>>> .r1485
,V2.TRANSACTION_AMT LXJE
,CASE When V2.TRANSACTION_TYPE ='DSP'then '利息' else '罚息' end  Type
from  IN_WFS.V_ASSET_FPM_INFO V2
join (select  wm_concat( CONCAT(CONCAT(CONCAT( CONCAT(CONCAT
           ( DAY_FROM,'-') ,DAY_TO),'天  '), INTEREST_RATE),'%')) LL 
          ,asset_code
           ,make
           ,dealer_code
           from( select distinct v.day_from,v.day_to,v.interest_rate,v.asset_code,v.make,dealer_code from IN_WFS.V_DEALER_MAKE_MODEL_RATE v
           where v.dealer_code = '{0}'  order by  day_to)
           group by make,asset_code,dealer_code) V3 on V2.ASSET_CODE = V3.ASSET_CODE and V2.MAKE = V3.MAKE and v2.dealer_code = V3.dealer_code
where trim(V2.dealer_code) = '{0}' and V2.BILL_NO ='{1}' {2} {3})A ) A where rn>={4} and rn <={5}";

       
        #region 查询条件

        string conditions = "";
        if (!string.IsNullOrEmpty(cjh))
        {
            conditions += string.Format("AND V2.vin_no = '{0}' ", cjh);
        }
        if (!string.IsNullOrEmpty(cx))
        {
            conditions += string.Format("AND V2.ASSET_DESC like '%{0}%' ", cx);
        }
        


        #endregion

        #region 排序
        string OrderBy = "ORDER BY CLBH";
        if (pagerInfo.iSortCol_0 == 0)
        {
            OrderBy = " ORDER BY ";
            OrderBy += " CLBH " + pagerInfo.sSortDir_0.ToUpper();
        }
        #endregion

        string UserCode = this.UserValidator.User.Appellation;

        sql = string.Format(sql, UserCode, fkbh, conditions, OrderBy, pagerInfo.StartIndex, pagerInfo.EndIndex);





        string sqlCount = @"select  count(distinct v2.UNIT_NO) c
from  IN_WFS.V_ASSET_FPM_INFO V2 
where  V2.BILL_NO ='{1}' and  trim(V2.dealer_code) = '{0}' {2}";

        sqlCount = string.Format(sqlCount, UserCode, fkbh, conditions);




        string sqlHD = @" select  case when to_char(BILL_DATE,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(BILL_DATE,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,V.Asset_Num FXCLS" + ",V.\"Invoice_Amount\" LXJE "
 + @",Case when v.STATUS_CODE = 'CL' then '已付'  else '未付' end  LXZDZT
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'  and  BILL_NO ='{1}'";

        sqlHD = string.Format(sqlHD, UserCode, fkbh);



        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("账单SQL" + "  ;" + sql);

        DataSet ds = new DataSet();
        //总行数
        DataTable total = ExecuteDataTableSql(dataCode, sqlCount);


        DataTable dt = ExecuteDataTableSql(dataCode, sql);
        DataTable dtHD = ExecuteDataTableSql(dataCode, sqlHD);

        ds.Tables.Add(total);
        ds.Tables.Add(dt);
        ds.Tables.Add(dtHD);

        return ds;
    }


    public System.Web.Mvc.JsonResult GetBillDTL(string billNo)
    {
        return this.ExecuteFunctionRun(() =>
        {

            DataSet ds = GetBillDTdt(billNo);

            return Json(new { BillList = ToJson(ds.Tables[0]), BillHD = ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        });
    }

    private DataSet GetBillDTdt(string billNo)
    {
        string sql = @"select distinct
V2.UNIT_NO CLBH
,V2.ASSET_DESC CX
,V2.vin_no CJH
,to_char(V2.CALC_END_DATE,'yyyy-mm-dd') JXJZRQ
<<<<<<< .mine
,to_char(V3.LL) CPLR
,V2.REQUESTED_AMT DKJE
||||||| .r1089
,V3.LL CPLR
,V.REQUESTED_AMT DKJE
=======
,V3.LL CPLR
,V2.REQUESTED_AMT DKJE
>>>>>>> .r1485
,V2.TRANSACTION_AMT LXJE
,CASE When V2.TRANSACTION_TYPE ='DSP'then '利息' else '罚息' end  TYPE
from  IN_WFS.V_ASSET_FPM_INFO V2 
join (select  wm_concat( CONCAT(CONCAT(CONCAT( CONCAT(CONCAT
            ( DAY_FROM,'-') ,DAY_TO),'天  '), INTEREST_RATE),'%')) LL 
            ,asset_code
            ,make
            ,dealer_code
            from( select distinct v.day_from,v.day_to,v.interest_rate,v.asset_code,v.make,dealer_code from IN_WFS.V_DEALER_MAKE_MODEL_RATE v
            order by  day_to)
            group by make,asset_code,dealer_code) V3 on V2.ASSET_CODE = V3.ASSET_CODE and V2.MAKE = V3.MAKE and v2.dealer_code = V3.dealer_code
where  V2.BILL_NO ='{0}'";

        string UserCode = this.UserValidator.User.Appellation;

        sql = string.Format(sql, billNo);

        string sqlHD = @"select  BILL_NO FKBH
,V.Asset_Num FXCLS"
+ ",V.\"Invoice_Amount\" LXJE "
 + @", '未付'   LXZDZT
from  IN_WFS.V_ALL_BILL v where v.BILL_NO ='{0}'";

        sqlHD = string.Format(sqlHD, billNo);
        DataSet ds = new DataSet();

        DataTable dt = ExecuteDataTableSql(dataCode, sql);
        DataTable dt2 = ExecuteDataTableSql(dataCode, sqlHD);
        ds.Tables.Add(dt);
        ds.Tables.Add(dt2);

        return ds;
    }


    public JsonResult Export(string startYM, string endYM, string state)
    {
        string sql = @"select  BILL_NO 付款编号
,to_char(BILL_DATE,'yyyy-mm') 月份
,case when to_char(BILL_DATE,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(BILL_DATE,'MONTH')),'yyyy-mm-dd') end 计息截止日期
,to_char(BILL_DUE_DATE,'yyyy-mm-dd') 付款到期日
,V.Asset_Num  付息车辆数 " + ",V.\"Invoice_Amount\" 利息金额 "
+
@",Case when v.STATUS_CODE = 'CL' then '已付'  else '未付' end  利息账单状态
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'   {1}
 ";

        #region 查询条件

        string conditions = "";

        if (!string.IsNullOrEmpty(startYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') >= '{0}' ", startYM);

        }
        if (!string.IsNullOrEmpty(endYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') <= '{0}' ", endYM);

        }
        if (!string.IsNullOrEmpty(state))
        {
            if (state == "已付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "CL");

            }
            if (state == "未付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "OP");

            }

        }

        string UserCode = this.UserValidator.User.Appellation;
        sql = string.Format(sql, UserCode, conditions);
        DataTable dt = ExecuteDataTableSql(dataCode, sql);
        CurrencyClass dd = new CurrencyClass();
        dd.ExportReportCurrency(dt, "账单列表" + DateTime.Now.ToString("yyyyMMdd"));
        return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);

        #endregion


    }

    public JsonResult ExportDTL(PagerInfo pagerInfo, string fkbh, string cjh, string cx)
    {
        string sql = @" select distinct
V2.UNIT_NO 车辆编号
,V2.ASSET_DESC 车型
,V2.vin_no 车架号
,to_char(V2.CALC_END_DATE,'yyyy-mm-dd') 计息截至日期
<<<<<<< .mine
,case when V2.TRANSACTION_TYPE ='DSP' then  to_char(V3.LL)  else null end 产品利率
,V2.REQUESTED_AMT 贷款金额
||||||| .r1089
,case when V2.TRANSACTION_TYPE ='DSP' then  V3.LL  else null end 产品利率
,V.REQUESTED_AMT 贷款金额
=======
,case when V2.TRANSACTION_TYPE ='DSP' then  V3.LL  else null end 产品利率
,V2.REQUESTED_AMT 贷款金额
>>>>>>> .r1485
,V2.TRANSACTION_AMT 利息金额
,CASE When V2.TRANSACTION_TYPE ='DSP'then '利息' else '罚息' end  交易类型
from  IN_WFS.V_ASSET_FPM_INFO V2
join (select  wm_concat( CONCAT(CONCAT(CONCAT( CONCAT(CONCAT
            ( DAY_FROM,'-') ,DAY_TO),'天  '), INTEREST_RATE),'%')) LL 
            ,asset_code
            ,make
            ,dealer_code
            from( select distinct v.day_from,v.day_to,v.interest_rate,v.asset_code,v.make,dealer_code from IN_WFS.V_DEALER_MAKE_MODEL_RATE v
            where v.dealer_code = '{0}'  order by  day_to)
            group by make,asset_code,dealer_code) V3 on trim(V2.ASSET_CODE) = trim(V3.ASSET_CODE) and  trim(V2.MAKE) = trim(V3.MAKE) and trim(v2.dealer_code) = trim(V3.dealer_code)
where trim(V2.dealer_code) = '{0}' and V2.BILL_NO ='{1}' {2} {3}";

        string conditions = "";
        if (!string.IsNullOrEmpty(cjh))
        {
            conditions += string.Format("AND V2.vin_no = '{0}' ", cjh);
        }
        if (!string.IsNullOrEmpty(cx))
        {
            conditions += string.Format("AND V2.ASSET_DESC like '%{0}%' ", cx);
        }


        #region 排序
        string OrderBy = " ORDER BY V2.UNIT_NO ";
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY ";
            if (pagerInfo.iSortCol_0 == 0)
            {
                OrderBy += " V2.UNIT_NO  " + pagerInfo.sSortDir_0.ToUpper();
            }
        }

        #endregion
        string UserCode = this.UserValidator.User.Appellation;

        sql = string.Format(sql, UserCode, fkbh, conditions, OrderBy);

        DataTable dt = ExecuteDataTableSql(dataCode, sql);

        CurrencyClass dd = new CurrencyClass();
        dd.ExportReportCurrency(dt, "账单明细列表" + DateTime.Now.ToString("yyyyMMdd"));
        return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);




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

    public int ExecuteCountSql(string connectionCode, string sql)
    {
        var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
        if (dbObject != null)
        {
            OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
            var command = factory.CreateCommand();
            var count = command.ExecuteScalar(sql);
            return Convert.ToInt32(count);
        }
        return 0;


    }

    public void ExecuteDataTableSP(string connectionCode, string sql, OThinker.Data.Database.Parameter[] par)
    {

        var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
        if (dbObject != null)
        {
            OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
            var command = factory.CreateCommand();
            command.ExecuteProcedure(sql, par);
        }

    }

    public string GetColumnsValue(DataRow row, string columns)
    {
        return row.Table.Columns.Contains(columns) ? row[columns] + string.Empty : "";
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

}