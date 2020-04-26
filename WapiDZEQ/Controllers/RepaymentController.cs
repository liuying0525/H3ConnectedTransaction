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
using OThinker.H3.Portal;


/// <summary>
/// RepaymentController 的摘要说明
/// </summary>
[Authorize]
public class RepaymentController : OThinker.H3.Controllers.ControllerBase
{
    public RepaymentController()
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

    public System.Web.Mvc.JsonResult GetRepaymentList(PagerInfo pagerInfo, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string StartTime, string EndTime, string repaymentStartTime, string repaymentEndTime, string CLBH)
    {
        DataSet ds = GetRepaymentDataSet(pagerInfo, FrameNo, Models, state, Manufacturer, FrameNoNature, address, StartTime, EndTime, repaymentStartTime, repaymentEndTime, CLBH);
        int _RowCount = (ds.Tables[0].Rows[0][0] + string.Empty) != "" ? Int32.Parse(ds.Tables[0].Rows[0][0] + string.Empty) : 0;
        string requestAmt = ds.Tables[0].Rows[0][1] + string.Empty;
        // string FPM = (ds.Tables[0].Rows[0][2] + string.Empty)!=""?ds.Tables[0].Rows[0][2] + string.Empty:"0";
        // TotalAmount = String.Format("{0:N}", TotalAmount);
        if (!string.IsNullOrEmpty(requestAmt)) requestAmt = decimal.Parse(requestAmt).ToString("#,##0.00").Replace(".00", "");
        // if (!string.IsNullOrEmpty(FPM)) FPM = decimal.Parse(FPM).ToString("#,##0.00").Replace(".00", "");
        string _CarStatistics = "        车辆       " + _RowCount + "       贷款金额        " + requestAmt;
        return Json(new { CarStatistics = _CarStatistics, RowCount = _RowCount, RowData = ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
    }

    public DataSet GetRepaymentDataSet(PagerInfo pagerInfo, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string startDate, string endDate, string repaymentStartTime, string repaymentEndTime, string CLBH)
    {
        var UserID = this.UserValidator.UserID;
        string UserCode = this.UserValidator.User.Appellation;

        string sql = @" select * from (select  rownum rn,c.* from ( select a.* from (
select distinct V.UNIT_NO CLBH
,V.MODEL CX
,case when (V.ORDER_NO = 'Y' and V.VIN_UPDATE = 'N') then '临时' else '永久' end CJHXZ
,V.VIN_NO CJH
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end DKDQR
--,V.Total_FPM LXZJ
,V.REQUESTED_AMT DKJE
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then '未到期' 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then '已到期'
        when c.states = '已拒绝' then '已拒绝'
      
         else '处理中' end  HKZT
,V.dealer_code JXSID
,null HKBH
,to_char(c.HKSPRP,'yyyy-mm-dd HH24:mi:ss') HKSPRP
,V.MAKE
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then 1 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then 2
        when c.states = '已拒绝' then 3
      
         else 4 end  HKZT_ORDER
--,(select to_char(vv.TRANSACTION_DATE,'yyyy-mm-dd') from IN_WFS.V_PV_INFO@TO_AUTH_WFS vv where trim(vv.agency_code)=trim(v.dealer_code) and trim(vv.UNIT_NO)=trim(v.UNIT_NO)) FKSJ
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
where v.dealer_code = {0} {5}
union
select V.UNIT_NO CLBH
,V.ASSET_DESC CX
, '永久' CJHXZ
,V.VIN_NO CJH
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end DKDQR
,V.APPROVED_AMT DKJE
, case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then '已付清' 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then '已到期'
       else '处理中' end  HKZT
,V.dealer_code JXSID
,null HKBH
,nvl(to_char(C.HKSPRP,'yyyy-mm-dd HH24:mi:ss'),to_char(v.SETTLEMENT_DATE,'yyyy-mm-dd HH24:mi:ss')） HKSPRP
,V.MAKE
, case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then 5 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then 1  
       else 4 end  HKZT_ORDER
--,(select to_char(vv.TRANSACTION_DATE,'yyyy-mm-dd') from IN_WFS.V_PV_INFO@TO_AUTH_WFS vv where trim(vv.agency_code)=trim(v.dealer_code) and trim(vv.UNIT_NO)=trim(v.UNIT_NO)) FKSJ
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
where v.PAID_AMT >0 and v.dealer_code = {0} and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO ){6})
 a  where 1=1 {1} {4})  C) where rn>={2} and rn <={3}";//,to_char(c.MODIFIEDTIME,'yyyy-mm-dd HH24:mi:ss') MODIFIEDTIME

        #region 查询条件

        string conditions = "";

        if (!string.IsNullOrEmpty(FrameNo))
        {
            conditions += string.Format("AND a.CJH like '%{0}%' ", FrameNo);

        }
        if (!string.IsNullOrEmpty(Models))
        {
            conditions += string.Format("AND a.CX like '%{0}%'  ", Models);

        }
        if (!string.IsNullOrEmpty(state))
        {

            conditions += string.Format("AND a.HKZT = '{0}'  ", state);

        }
        if (!string.IsNullOrEmpty(Manufacturer))
        {
            conditions += string.Format("AND a.MAKE like '%{0}%' ", Manufacturer);

        }
        if (!string.IsNullOrEmpty(FrameNoNature))
        {
            conditions += string.Format("AND a.CJHXZ = '{0}' ", FrameNoNature);

        }
        //if (!string.IsNullOrEmpty(address))
        //{
        //    conditions += string.Format("AND trim(V.ADDRESS) ='{0}' ", address);
        //    conditions2 += string.Format("AND trim(V.ADDRESS) ='{0}' ", address);
        //}
        if (!string.IsNullOrEmpty(startDate))
        {
            conditions += string.Format("AND DKDQR>='{0}'\r\n", startDate);

        }
        if (!string.IsNullOrEmpty(endDate))
        {
            conditions += string.Format("AND DKDQR<='{0}'\r\n", endDate);


        }
        if (!string.IsNullOrEmpty(repaymentStartTime))
        {

            conditions += string.Format("AND substr(HKSPRP,0,10)>='{0}'\r\n", repaymentStartTime);
        }
        if (!string.IsNullOrEmpty(repaymentEndTime))
        {

            conditions += string.Format("AND substr(HKSPRP,0,10)<='{0}'\r\n", repaymentEndTime);

        }
        if (!string.IsNullOrEmpty(CLBH))
        {
            conditions += string.Format("AND a.CLBH like '%{0}%' ", CLBH);

        }

        #endregion

        #region 排序

        string OrderBy = " ORDER BY HKZT_ORDER asc, a.HKSPRP desc ,a.CLBH asc ";//a.HKSPRP desc , CLBH 
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY ";
            if (pagerInfo.iSortCol_0 == 5)
            {
                OrderBy += " a.DKDQR " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 6)
            {
                OrderBy += " a.HKSPRP " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 1)
            {
                OrderBy += " CLBH " + pagerInfo.sSortDir_0.ToUpper();
            }
           

        }

        #endregion

        //默认选择
        string hkzt = "";
        string hkzt1 = "";
        if (state == "已到期")
        {
            hkzt = " and to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null  and to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd') and c.states is null ";
            hkzt1 = " and  V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd') ";
        }
        sql = string.Format(sql, UserCode, conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, hkzt, hkzt1);


        string sqlCount = @"select count(1),sum(DKJE)requestAmt from (
select distinct V.UNIT_NO CLBH
,V.MODEL CX
,case when (V.ORDER_NO = 'Y' and V.VIN_UPDATE = 'N') then '临时' else '永久' end CJHXZ
,V.VIN_NO CJH
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end DKDQR
--,V.Total_FPM LXZJ
,V.REQUESTED_AMT DKJE
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then '未到期' 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then '已到期'
        when c.states = '已拒绝' then '已拒绝'
         else '处理中' end  HKZT
,V.dealer_code JXSID
,null HKBH
,to_char(c.HKSPRP,'yyyy-mm-dd HH24:mi:ss') HKSPRP
,V.MAKE
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
where v.dealer_code = {0} {2}
union
select V.UNIT_NO CLBH
,V.ASSET_DESC CX
, '永久' CJHXZ
,V.VIN_NO CJH
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end DKDQR
,V.APPROVED_AMT DKJE
, case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then '已付清' 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then '已到期'  
       else '处理中' end  HKZT
,V.dealer_code JXSID
,null HKBH
,nvl(to_char(C.HKSPRP,'yyyy-mm-dd HH24:mi:ss'),to_char(v.SETTLEMENT_DATE,'yyyy-mm-dd HH24:mi:ss')） HKSPRP
,V.MAKE
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
where v.dealer_code = {0}and  v.PAID_AMT >0 and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO )  {3}) a where 1=1 {1}";
        sqlCount = string.Format(sqlCount, UserCode, conditions, hkzt, hkzt1);



        DataSet ds = new DataSet();
        //总行数
        DataTable total = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlCount);




        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        ds.Tables.Add(total);
        ds.Tables.Add(dt);
        return ds;
    }


    public JsonResult GetCJHEdit(string param)
    {
        string UserCode = this.UserValidator.User.Appellation;
        string sql = string.Format(@"select distinct UNIT_NO CLBH,ASSET_DESC CX,VIN_NO CJH,null NewCJX from IN_WFS.V_LOAN_STOCK_LIST_ALL v where v.dealer_code = '{0}'
and v.Loan_Status IN ( '40')
and VIN_NO in ('{1}')", UserCode, param);

        if (string.IsNullOrEmpty(param))
        {
            sql = string.Format(@"select distinct UNIT_NO CLBH,ASSET_DESC CX,VIN_NO CJH,null NewCJX from IN_WFS.V_LOAN_STOCK_LIST_ALL v where v.dealer_code = '{0}'
and v.Loan_Status IN ( '40')
and ORDER_NO = 'Y' and VIN_UPDATE = 'N'", UserCode);
        }

        return this.ExecuteFunctionRun(() =>
       {

           DataTable dt = ExecuteDataTableSql(dataCode, sql);

           List<RepaymentViewModel> repaymentList = Getgriddata(dt);
           var result = new
           {
               Success = repaymentList != null,
               Repayment = repaymentList,
               Count = repaymentList.Count,

           };
           return Json(result, JsonRequestBehavior.AllowGet);
       });

    }


    [HttpGet]
    public System.Web.Mvc.JsonResult UpdateCJH(string[] param)
    {
        DataTable dt = null;
        string Batchstr = "";
        int Batch = 0;
        string batchNub = @"select  Max(distinct batch) batch from I_FrameNumberMiddleTable where substr(batch,0,8)='" + DateTime.Now.ToString("yyyyMMdd") + "'";
        var batchVar = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(batchNub);
        Batch = Convert.ToInt32(batchVar.ToString().Length > 8 ? batchVar.ToString().Substring(8, batchVar.ToString().Length - 8) : "0") + 1;

        if (batchVar.ToString().Length > 8 && batchVar.ToString().Substring(0, 8) == DateTime.Now.ToString("yyyyMMdd"))
            Batchstr = DateTime.Now.ToString("yyyyMMdd") + Batch;
        else
            Batchstr = DateTime.Now.ToString("yyyyMMdd") + 1;

        string[] strPar = param;//param.Split(',');
        string message = "";
        OThinker.Data.Database.Parameter[] pars = new OThinker.Data.Database.Parameter[2];
        for (int i = 0; i < strPar.Length; i++)
        {
            string sqlUser = string.Format(@"select distinct trim(Unit_No) Unit_No,asset_desc,trim(a.dealer_code) dealer_code,trim(b.dealer_name) dealer_name from IN_WFS.V_LOAN_STOCK_LIST a
join IN_WFS.V_DEALER_INFO b on a.dealer_code = b.dealer_code
 where Vin_No='{0}'", strPar[i].Split(';')[0]);
            dt = ExecuteDataTableSql("Wholesale",sqlUser);
            try
            {

                pars[0] = new OThinker.Data.Database.Parameter("v_vin_no_old", DbType.String, strPar[i].Split(';')[0]);
                pars[1] = new OThinker.Data.Database.Parameter("v_vin_no_new", DbType.String, strPar[i].Split(';')[1]);
                ExecuteDataTableSP("Wholesale", "IN_WFS.ZUPDATEVIN", pars);
            }
            catch (Exception e)
            {

                message = message + strPar[i].Split(';')[0] + "; ";
                return Json(new { Success = message=="", Message =e.Message  }, JsonRequestBehavior.AllowGet);
            }




            string objectid = Guid.NewGuid().ToString();

            string VehicleNumber = dt.Rows[0]["Unit_No"].ToString();
            string CarModel = dt.Rows[0]["asset_desc"].ToString();
            //string TemporaryFrameNumber = (pars[0]).ToString();
            //string PermanentFrameNumber = (pars[1]).ToString();
            string TemporaryFrameNumber = strPar[i].Split(';')[0];
            string PermanentFrameNumber = strPar[i].Split(';')[1];
            string sql = string.Format(@"INSERT INTO I_FrameNumberMiddleTable(objectid,Batch,VehicleNumber,CarModel,TemporaryFrameNumber,PermanentFrameNumber)
VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", objectid, Batchstr, VehicleNumber, CarModel, TemporaryFrameNumber, PermanentFrameNumber);

            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);


        }
        try
        {

            MessageClass ms = new MessageClass();

            string[] workerArr = { "YYB01" };
            //发送消息给运营
            string msstrc = dt.Rows[0]["dealer_name"].ToString() + "(" + dt.Rows[0]["dealer_code"].ToString() + ")有<font color=\"red\">" + strPar.Length + "</font>辆车临时车架号已改为永久车架号";
            for (int j = 0; j < workerArr.Length; j++)
            {
                string arr = workerArr[j];
                List<RoleUserViewModel> roleuser = new WapiDZEQ.Common.MessageRole().GetRoleUser(arr);
                for (int k = 0; k < roleuser.Count; k++)
                {
                    string userId = roleuser[k].UserID;
                    string USERCODE = roleuser[k].UserCode;
                    ms.InsertMSG(userId, USERCODE, msstrc, true, 4, Batchstr);
                }
            }


            var result = new
            {
                Success = message == "",
                Message = message

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception)
        {

            return Json(new { Success = message == "", Message = "" }, JsonRequestBehavior.AllowGet);
        } 
    }

    public int CfuCJH(string CJH)
    {
        string sql = " select count(*) from I_clxx where  CJH ='" + CJH + "'";
        int count = Convert.ToInt32(OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
        return count;
    }

    public JsonResult CheckCJH(string cjh)
    {
        string sql = string.Format(@"select count(1) count from IN_WFS.V_LOAN_STOCK_LIST where VIN_NO = '{0}' ", cjh);


        return this.ExecuteFunctionRun(() =>
        {

            int count = ExecuteCountSql(dataCode, sql);
            var result = new
            {
                Success = count != 0,
                Count = count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        });

    }


    public JsonResult ExportCJH()
    {
        string UserCode = this.UserValidator.User.Appellation;
        string sql = @"select V.UNIT_NO 车辆编号,V.ASSET_DESC 车型,V.VIN_NO 临时车架号,'' 永久车架号 from IN_WFS.V_LOAN_STOCK_LIST V
where V.ORDER_NO = 'Y' and V.VIN_UPDATE = 'N' and dealer_code = '" + UserCode + "' and PAID_AMT>0 ";

        DataTable dt = ExecuteDataTableSql(dataCode, sql);

        CurrencyClass dd = new CurrencyClass();
        dd.ExportExcel(dt, "需更新车架号" + DateTime.Now.ToString("yyyyMMdd"));
        return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
    }

    public JsonResult Export(string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string startTime, string endTime, string address, string repaymentStartTime, string repaymentEndTime, string CLBH)
    {
        string UserCode = this.UserValidator.User.Appellation;
        string sql = @" select 车辆编号,车型,车架号性质,车架号,贷款到期日,贷款金额,还款状态,还款申请日期,制造商 from ( select a.* from (
select distinct V.UNIT_NO 车辆编号
,V.MODEL 车型
,case when (V.ORDER_NO = 'Y' and V.VIN_UPDATE = 'N') then '临时' else '永久' end 车架号性质
,V.VIN_NO 车架号
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end 贷款到期日
--,V.Total_FPM LXZJ
,V.REQUESTED_AMT 贷款金额
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then '未到期' 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then '已到期'
        when c.states = '已拒绝' then '已拒绝'
         else '处理中' end   还款状态

,to_char(c.HKSPRP,'yyyy-mm-dd HH24:mi:ss') 还款申请日期
,V.MAKE 制造商
,case when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null ) and (to_char(d.expiry_date,'yyyy-mm-dd')>=to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) then 1 
       when (to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null) or (to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')and c.states is null)then 2
        when c.states = '已拒绝' then 3
      
         else 4 end  HKZT_ORDER
--,(select to_char(vv.TRANSACTION_DATE,'yyyy-mm-dd') from IN_WFS.V_PV_INFO@TO_AUTH_WFS vv where trim(vv.agency_code)=trim(v.dealer_code) and trim(vv.UNIT_NO)=trim(v.UNIT_NO)) 贷款起始日
from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
Left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v1 on v1.new_value =  V.VIN_NO
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
where v.dealer_code = {0} 
union
select V.UNIT_NO 车辆编号
,V.ASSET_DESC 车型
, '永久' 车架号性质
,V.VIN_NO 车架号
,case when d.expiry_date <V.MATURITY_DATE then to_char(d.expiry_date,'yyyy-mm-dd')else  to_char(V.MATURITY_DATE,'yyyy-mm-dd') end 贷款到期日
,V.APPROVED_AMT 贷款金额
, case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then '已付清' 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then '已到期'  
       else '处理中' end  还款状态

,nvl(to_char(C.HKSPRP,'yyyy-mm-dd HH24:mi:ss'),to_char(v.SETTLEMENT_DATE,'yyyy-mm-dd HH24:mi:ss')） 还款申请日期
,V.MAKE 制造商
, case when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is not null)   then 5 
       when (V.LOAN_STATUS='60'and SETTLEMENT_DATE is null and v2.STOCK_STATUS = 'I'and c.states is null and （to_char(V.MATURITY_DATE,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')  or to_char(d.expiry_date,'yyyy-mm-dd')<to_char(d.CURR_DATE,'yyyy-mm-dd')))  then 1  
       else 4 end  HKZT_ORDER
--,(select to_char(vv.TRANSACTION_DATE,'yyyy-mm-dd') from IN_WFS.V_PV_INFO@TO_AUTH_WFS vv where trim(vv.agency_code)=trim(v.dealer_code) and trim(vv.UNIT_NO)=trim(v.UNIT_NO)) 贷款起始日
from IN_WFS.V_SALES_WFS_DT@TO_AUTH_WFS v
join IN_WFS.V_DEALER_EXPIRY_DATE@TO_AUTH_WFS d on d.Make = v.Make and d.dealer_code = v.dealer_code
join IN_WFS.V_LOAN_STOCK_LIST@TO_AUTH_WFS v2 on v2.dealer_code = v.dealer_code and v2.UNIT_NO = v.UNIT_NO and v2.VIN_NO = v.VIN_NO
Left join IN_WFS.V_VIN_UPDATE_HIS@TO_AUTH_WFS v1 on v1.new_value =  V.VIN_NO
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
where v.dealer_code = {0} and  v.PAID_AMT >0 and Not EXISTS (select 1 from  IN_WFS.V_STOCK_DC_HIS@TO_AUTH_WFS v1 where v1.VIN_NO=v.VIN_NO )) a where 1=1 {1}  ORDER BY HKZT_ORDER asc, a.还款申请日期 desc ,a.车辆编号 asc ) q ";//a.还款申请日期 desc , a.车辆编号

        #region 查询条件

        string conditions = "";

        if (!string.IsNullOrEmpty(FrameNo))
        {
            conditions += string.Format("AND ( a.车架号 like '%{0}%' or a.车架号 like '%{0}%')  ", FrameNo);

        }
        if (!string.IsNullOrEmpty(Models))
        {
            conditions += string.Format("AND a.车型 like '%{0}%'  ", Models);

        }
        if (!string.IsNullOrEmpty(state))
        {

            conditions += string.Format("AND a.还款状态 = '{0}'  ", state);

        }
        if (!string.IsNullOrEmpty(Manufacturer))
        {
            conditions += string.Format("AND a.制造商 like '%{0}%' ", Manufacturer);

        }
        if (!string.IsNullOrEmpty(FrameNoNature))
        {
            conditions += string.Format("AND a.车架号性质 = '{0}' ", FrameNoNature);

        }
        //if (!string.IsNullOrEmpty(address))
        //{
        //    conditions += string.Format("AND trim(V.ADDRESS) ='{0}' ", address);
        //    conditions2 += string.Format("AND trim(V.ADDRESS) ='{0}' ", address);
        //}
        if (!string.IsNullOrEmpty(startTime))
        {
            conditions += string.Format("AND 贷款到期日>='{0}'\r\n", startTime);

        }
        if (!string.IsNullOrEmpty(endTime))
        {
            conditions += string.Format("AND 贷款到期日<='{0}'\r\n", endTime);


        }
        if (!string.IsNullOrEmpty(repaymentStartTime))
        {

            conditions += string.Format("AND substr(还款申请日期,0,10)>='{0}'\r\n", repaymentStartTime);
        }
        if (!string.IsNullOrEmpty(repaymentEndTime))
        {

            conditions += string.Format("AND substr(还款申请日期,0,10)<='{0}'\r\n", repaymentEndTime);

        }
        if (!string.IsNullOrEmpty(CLBH))
        {
            conditions += string.Format("AND a.车辆编号 like '%{0}%' ", CLBH);

        }

        #endregion
        sql = string.Format(sql, UserCode, conditions);
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

        CurrencyClass dd = new CurrencyClass();
        dd.ExportReportCurrency(dt, "还款列表" + DateTime.Now.ToString("yyyyMMdd"));
        return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
    }

    public void ImportCJHFile()
    {
        //OThinker.H3.Controllers.ImportDZController im = new OThinker.H3.Controllers.ImportDZController();
        // DataTable dt = im.ImportFile();

    }



    /// <summary>
    /// 获取返回到前端的BondViewModel
    /// </summary>
    /// <param name="dtWorkItem"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public List<RepaymentViewModel> Getgriddata(DataTable dt)
    {

        List<RepaymentViewModel> griddata = new List<RepaymentViewModel>();
        if (dt == null) return griddata;
        foreach (DataRow row in dt.Rows)
        {
            RepaymentViewModel bv = new RepaymentViewModel();
            if (dt.Columns.Contains("HKBH"))
                bv.HKBH = this.GetColumnsValue(row, "HKBH");
            if (dt.Columns.Contains("CX"))
                bv.CX = this.GetColumnsValue(row, "CX");
            if (dt.Columns.Contains("CJH"))
                bv.CJH = this.GetColumnsValue(row, "CJH");
            if (dt.Columns.Contains("FKFS"))
                bv.FKFS = this.GetColumnsValue(row, "FKFS");
            if (dt.Columns.Contains("CLBH"))
                bv.CLBH = this.GetColumnsValue(row, "CLBH");
            if (dt.Columns.Contains("CJHXZ"))
                bv.CJHXZ = this.GetColumnsValue(row, "CJHXZ");
            if (dt.Columns.Contains("HKZT"))
                bv.HKZT = this.GetColumnsValue(row, "HKZT");
            if (dt.Columns.Contains("DKJE"))
                bv.DKJE = this.GetColumnsValue(row, "DKJE") == "" ? 0 : Convert.ToDecimal(this.GetColumnsValue(row, "DKJE"));

            griddata.Add(bv);
        }
        return griddata;
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