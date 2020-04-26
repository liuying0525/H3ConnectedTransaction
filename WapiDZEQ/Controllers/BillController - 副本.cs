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
    public const string UserCode = "98.150"; //98.158
    public const string dataCode = "Wholesale";

    public System.Web.Mvc.JsonResult GetBillList(PagerInfo pagerInfo, string startYM, string endYM,string state)
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
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'   {1}
union all
select '' FKBH
,to_char(v.systemdate,'yyyy-mm') MM
,case when to_char(v.systemdate,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(v.systemdate,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,to_char(add_months(sysdate,1),'yyyy-mm-dd') FXDQR
,count(*) FXCLS
,sum(v.incurred_mi) LXJE
,'未出账单' LXZDZT
from IN_WFS.V_ZDSP_STOCK_LIST_D v 
where v.dealer_code = {0} {4}
group by v.DEALER_CODE ,v.systemdate) a ) A where rn>={2} and rn <={3}";


        #region 查询条件

        string conditions = "";
        string Unconditions = "";
        if (!string.IsNullOrEmpty(startYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') >= '{0}' ", startYM);
            Unconditions += string.Format(" AND to_char(v.systemdate,'yyyy-mm') >='{0}'", startYM);
        }
        if (!string.IsNullOrEmpty(endYM))
        {
            conditions += string.Format(" AND to_char(BILL_DATE,'yyyy-mm') <= '{0}' ", endYM);
            Unconditions += string.Format(" AND to_char(v.systemdate,'yyyy-mm') <='{0}'", endYM);
        }
        if (!string.IsNullOrEmpty(state))
        {
            if (state == "已付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "CL");
                Unconditions += "AND 1<>1 ";
            }
            if (state == "未付")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "OP");
                Unconditions += "AND 1<>1 ";
            }
            if (state == "未出账单")
            {
                conditions += string.Format(" AND v.STATUS_CODE = '{0}' ", "UNOUT");
            }
        }
      

        #endregion

        sql = string.Format(sql, UserCode, conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, Unconditions);

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
union all
select '' FKBH
,to_char(v.systemdate,'yyyy-mm') MM
,case when to_char(v.systemdate,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(v.systemdate,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,to_char(add_months(sysdate,1),'yyyy-mm-dd') FXDQR
,count(*) FXCLS
,sum(v.incurred_mi) LXJE
,'未出账单' LXZDZT
from IN_WFS.V_ZDSP_STOCK_LIST_D v 
where v.dealer_code = {0} {2}
group by v.DEALER_CODE ,v.systemdate) a ";

        sqlCount = string.Format(sqlCount, UserCode, conditions, Unconditions);

        DataSet ds = new DataSet();
        //总行数
        DataTable total = ExecuteDataTableSql(dataCode, sqlCount);


        DataTable dt = ExecuteDataTableSql(dataCode, sql);
        
        ds.Tables.Add(total);
        ds.Tables.Add(dt);
        return ds;
    }


    public System.Web.Mvc.JsonResult GetBillDTLList(PagerInfo pagerInfo, string cjh, string cx,string fkbh)
    {
        
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
        if (fkbh!="null")
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

        string sql = @" select * from  (select  rownum rn
,V.UNIT_NO CLBH
,V.ASSET_DESC CX
,V.vin_no CJH
,to_char(V2.CALC_END_DATE,'yyyy-mm-dd') JXJZRQ
,V3.LL CPLR
,V.REQUESTED_AMT DKJE
,V2.TRANSACTION_AMT LXJE
from ( select distinct UNIT_NO,ASSET_DESC,vin_no,REQUESTED_AMT,dealer_code from IN_WFS.V_LOAN_STOCK_LIST_ALL where dealer_code = '{0}' ) V
join IN_WFS.V_ASSET_FPM_INFO V2 on V.vin_NO = V2.vin_no
join (select  wm_concat( CONCAT(CONCAT(CONCAT( CONCAT(CONCAT
            ( DAY_FROM,'-') ,DAY_TO),'天  '), INTEREST_RATE),'%')) LL 
            ,asset_code
            ,make
            ,dealer_code
            from( select distinct v.day_from,v.day_to,v.interest_rate,v.asset_code,v.make,dealer_code from IN_WFS.V_DEALER_MAKE_MODEL_RATE v
            where v.dealer_code = '{0}'  order by  day_to)
            group by make,asset_code,dealer_code) V3 on V2.ASSET_CODE = V3.ASSET_CODE and V2.MAKE = V3.MAKE and v.dealer_code = V3.dealer_code
where V.dealer_code = '{0}' and V2.BILL_NO ='{1}' {2} ) A where rn>={3} and rn <={4}";

        #region 查询条件

        string conditions = "";
        if (!string.IsNullOrEmpty(cjh))
        {
            conditions += string.Format("AND V.vin_no = '{0}' ", cjh);
        }
        if (!string.IsNullOrEmpty(cx))
        {
            conditions += string.Format("AND V.ASSET_DESC like '%{0}%' ", cx);
        }
        if (fkbh == "null")
        {
            conditions = "";

            if (!string.IsNullOrEmpty(cjh))
            {
                conditions += string.Format("AND V.CHASSIS_COMM_NO = '{0}' ", cjh);
            }
            if (!string.IsNullOrEmpty(cx))
            {
                conditions += string.Format("AND V.ASSET_DESC like '%{0}%' ", cx);
            }
        }


        #endregion
        sql = string.Format(sql, UserCode, fkbh, conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);

        if (fkbh=="null")
        {
            sql = @" select * from  (select rownum rn
,V.UNIT_NO CLBH
,V.ASSET_DESC CX
,V.CHASSIS_COMM_NO CJH
,to_char(V.systemdate,'yyyy-mm-dd') JXJZRQ
,V3.LL CPLR
,V.PAID_AMT DKJE
,V.incurred_mi LXJE
from  IN_WFS.V_ZDSP_STOCK_LIST_D V
join (select  wm_concat( CONCAT(CONCAT(CONCAT( CONCAT(CONCAT
            ( DAY_FROM,'-') ,DAY_TO),'天  '), INTEREST_RATE),'%')) LL 
            ,dealer_code
            ,ASSET_DESC
            from( select distinct v.day_from,v.day_to,v.interest_rate,v.asset_code,v.make,dealer_code,ASSET_DESC from IN_WFS.V_DEALER_MAKE_MODEL_RATE v
            where v.dealer_code = '{0}'  order by  day_to)
            group by dealer_code,ASSET_DESC) V3 on V.ASSET_DESC = V3.ASSET_DESC  and v.dealer_code = V3.dealer_code
where v.dealer_code = {0} {1} ) A where rn>={2} and rn <={3}";
        }

        sql = string.Format(sql, UserCode,  conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);

        

        string sqlCount = @"select count(1) count
from ( select distinct UNIT_NO,ASSET_DESC,vin_no,REQUESTED_AMT,dealer_code from IN_WFS.V_LOAN_STOCK_LIST_ALL where dealer_code = '{0}' ) V
join IN_WFS.V_ASSET_FPM_INFO V2 on V.vin_NO = V2.vin_no where V2.BILL_NO = '{1}'  {2}";

        sqlCount = string.Format(sqlCount, UserCode, fkbh, conditions);

        if (fkbh == "null")
        {
            sqlCount = @"select count(1) count
from  IN_WFS.V_ZDSP_STOCK_LIST_D V 
where v.dealer_code = {0}  {1}";
        }
        sqlCount = string.Format(sqlCount, UserCode, conditions);
        


        string sqlHD = @" select  case when to_char(BILL_DATE,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(BILL_DATE,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,V.Asset_Num FXCLS"+ ",V.\"Invoice_Amount\" LXJE "
 +@",Case when v.STATUS_CODE = 'CL' then '已付'  else '未付' end  LXZDZT
from  IN_WFS.V_ALL_BILL v where v.Agency_Code='{0}' and v.BILL_NO like 'FPM%'  and  BILL_NO ='{1}'";

        sqlHD = string.Format(sqlHD, UserCode, fkbh);

        if (fkbh=="null")
        {
            sqlHD = @"select case when to_char(v.systemdate,'yyyy-mm')=to_char(sysdate,'yyyy-mm') then to_char(sysdate-1,'yyyy-mm-dd')
  else to_char(last_day(trunc(v.systemdate,'MONTH')),'yyyy-mm-dd') end JXJZRQ
,count(*) FXCLS
,sum(v.incurred_mi) LXJE
,'未出账单' LXZDZT
from IN_WFS.V_ZDSP_STOCK_LIST_D v 
where v.dealer_code = {0} 
group by v.DEALER_CODE ,v.systemdate";

            sqlHD = string.Format(sqlHD, UserCode);
        }

        

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