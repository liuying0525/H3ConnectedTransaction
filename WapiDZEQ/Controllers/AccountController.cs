using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
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
public class AccountController : OThinker.H3.Controllers.ControllerBase
{
    public AccountController()
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

    public System.Web.Mvc.JsonResult GetAccountList(PagerInfo pagerInfo, string distributorCode, string distributorName)
    {
        int total = 0;
        DataTable dt = GetAccountData(pagerInfo, distributorCode, distributorName, ref total);


        return Json(new { RowCount = total, RowData = ToJson(dt) }, JsonRequestBehavior.AllowGet);
    }

    public System.Web.Mvc.JsonResult GetAccountDTLList(PagerInfo pagerInfo, string JXSBM)
    {
        int total = 0;
        decimal totalAmt = 0;
        string CarStatistics = "";
        string jxs = "";

        DataTable dt = GetAccountDTLData(pagerInfo, JXSBM, ref total, ref totalAmt, ref jxs);
        CarStatistics = totalAmt.ToString("#,##0.00");


        return Json(new { RowCount = total, CarStatistics = CarStatistics, jxs = jxs, RowData = ToJson(dt) }, JsonRequestBehavior.AllowGet);
    }

    public DataTable GetAccountData(PagerInfo pagerInfo, string distributorCode, string distributorName, ref int total)
    {
        string UserID = this.UserValidator.UserID;

        string sql = @" select * from (select rownum rn,B.* from (select distinct v.dealer_code jxsbm ,v.dealer_name jxs  
,a.YDZJE ,a.ZHCZSJ
from IN_WFS.V_DEALER_INFO@TO_AUTH_WFS v
Left join
( select  a.jxsbm,sum(b.DZJE)YDZJE, to_char(Max(CJJEDZSJ),'yyyy-mm-dd HH24:mi') ZHCZSJ 
from I_Repayment  a 
Left join I_DZJEList b on a.objectid = b.parentobjectid
where 1=1 {0}
group by a.jxsbm) a on trim(a.jxsbm) = trim(v.dealer_code)
where 1=1 {3} {4} ) B ) T where rn>={1} and rn <={2}";


        #region 查询条件

        string conditions = "";
        string conditionsD = "";

        if (!string.IsNullOrEmpty(distributorCode))
        {
            conditions += string.Format(" and a.jxsbm = '{0}'", distributorCode);
            conditionsD += string.Format(" and v.dealer_code = '{0}'", distributorCode);
        }
        if (!string.IsNullOrEmpty(distributorName))
        {
            conditionsD += string.Format(" and v.dealer_name like '%{0}%'", distributorName);
        }



        #endregion

        #region 排序

        string OrderBy = " ORDER BY jxsbm asc ";
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY  ";
            if (pagerInfo.iSortCol_0 == 4)
            {
                OrderBy += " a.ZHCZSJ " + pagerInfo.sSortDir_0.ToUpper();
            }
            if (pagerInfo.iSortCol_0 == 1)
            {
                OrderBy = " ORDER BY  ";
                OrderBy += "jxsbm " + pagerInfo.sSortDir_0.ToUpper();
            }
        }
        #endregion

        sql = string.Format(sql, conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, conditionsD, OrderBy);

        string sqlCount = @" select count(distinct v.dealer_code) from IN_WFS.V_DEALER_INFO@TO_AUTH_WFS v  where 1=1 {0} ";

        sqlCount = string.Format(sqlCount, conditionsD);

        var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlCount);
        total = Convert.ToInt32(count);


        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


        return dt;
    }

    public DataTable GetAccountDTLData(PagerInfo pagerInfo, string JXSBM, ref int total, ref decimal totalAmt, ref string jxs)
    {
        string UserID = this.UserValidator.UserID;

        string sql = @" select * from  (select  rownum rn,A.* from (select DZJEBH ,a.jxs,a.jxsbm,b.DZJE, to_char(CJJEDZSJ,'yyyy-mm-dd HH24:mi') CJJEDZSJ,to_char(JEDZSJ,'yyyy-mm-dd')JEDZSJ,CWDZBZ
from I_Repayment  a 
join I_DZJEList b on a.objectid = b.parentobjectid
where a.jxsbm = '{0}'{3} ) A ) where rn>={1} and rn <={2}";

        #region 排序

        string OrderBy = " ORDER BY  CJJEDZSJ desc ";
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY  ";
            if (pagerInfo.iSortCol_0 == 3)
            {
                OrderBy += " CJJEDZSJ " + pagerInfo.sSortDir_0.ToUpper();
            }
        }

        #endregion


        sql = string.Format(sql, JXSBM, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy);

        string sqlTotal = @"select count(1) count,sum(DZJE) totalAmt,b.jxs from I_DZJEList a
join I_Repayment b on b.objectid = a.parentobjectid  where b.jxsbm = '{0}'
group by b.jxsbm,b.jxs";

        sqlTotal = string.Format(sqlTotal, JXSBM);

        DataTable dtTotal = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlTotal);
        if (dtTotal.Rows.Count > 0)
        {
            total = Convert.ToInt32(dtTotal.Rows[0][0].ToString());
            totalAmt = Convert.ToDecimal(dtTotal.Rows[0][1] == null ? "0" : dtTotal.Rows[0][1].ToString());
            jxs = dtTotal.Rows[0][2].ToString();
        }
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


        return dt;
    }
    /// <summary>
    /// 导出
    /// </summary>
    /// <param name="startYM"></param>
    /// <param name="endYM"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public JsonResult Export(PagerInfo pagerInfo, string JXSBM)
    {
        string sql = @"select DZJEBH 
,a.jxs 经销商
,a.jxsbm 到账金额编号
,b.DZJE 到账金额
, to_char(CJJEDZSJ,'yyyy-mm-dd HH24:mi') 创建到账金额时间
,to_char(JEDZSJ,'yyyy-mm-dd') 金额到账时间
,CWDZBZ 到账金额备注
from I_Repayment  a 
join I_DZJEList b on a.objectid = b.parentobjectid
where a.JXSBM = '{0}' {1}";

        #region 排序

        string OrderBy = " ORDER BY  CJJEDZSJ desc ";
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY  ";
            if (pagerInfo.iSortCol_0 == 3)
            {
                OrderBy += " CJJEDZSJ " + pagerInfo.sSortDir_0.ToUpper();
            }
        }

        #endregion

        string UserCode = this.UserValidator.User.Appellation;
        sql = string.Format(sql, JXSBM, OrderBy);
        DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        CurrencyClass dd = new CurrencyClass();
        dd.ExportReportCurrency(dt, "到账金额明细表" + DateTime.Now.ToString("yyyyMMdd"));
        return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);

    }



    public System.Web.Mvc.JsonResult CreateAccount(string JXSBH, string DZJE, string JEDZSJ, string CWDZBZ, string JXS)
    {
        JXSBH = JXSBH.Trim();



        string sqlBH = @"select MAX(DZJEBH) DZJEBH from I_DZJEList a join I_Repayment b on a.parentobjectid = b.objectid where b.JXSBM = '" + JXSBH + "'";
        string sqlWorkItem = @"select a.objectid,
a.INSTANCEID,c.objectid bizId ,c.TotalHKJE,c.TotalLXJE,c.TotalDZJE from OT_WorkItem a 
join  Ot_Instancecontext b on a.INSTANCEID = b.Objectid
join I_Repayment c on c.objectid = b.BIZOBJECTID
where  a.workflowcode = 'Repayment' and  a.ACTIVITYCODE= 'Market' and c.JXSBM = '" + JXSBH + "'";

        string sqlRepayment = string.Format(@"select h.* from (select CLBH,CX,CJH,DKDQR,FKFS,DKJE,HKSPRP,STATES,SPBH,LSCJH,CJHXGSJ from  I_HK_List where trim(jxsbm) ='{0}') h 
join (
select MaX(HKSPRP) HKSPRP,cjh from  (
select states,HKSPRP,cjh from  I_HK_List where trim(jxsbm) ='{0}'
union
select a.states,a.HKSPRP,a.cjh from I_HKList a
join i_Repayment b on a.parentobjectid = b.objectid where b.JXSBM ='{0}') a 
group by cjh) b on h.cjh = b.cjh and h.HKSPRP = b.HKSPRP
and NOT EXISTS(select 1  from I_HKList a
                       join I_Repayment b on a.parentobjectid = b.objectid where trim(b.jxsbm) = '{0}' and a.cjh = h.cjh and a.HKSPRP =h.HKSPRP )", JXSBH);

        string sqlBill = string.Format(@"select  BILL_NO FKBH
,to_char(BILL_DATE,'yyyy-mm') MM
,to_char(BILL_DATE,'yyyy-mm-dd') JXJZRQ
,to_char(BILL_DUE_DATE,'yyyy-mm-dd') FXDQR
,V.Asset_Num FXCLS," + "V.\"Invoice_Amount\" LXJE"
+ @" from  IN_WFS.V_ALL_BILL@TO_AUTH_WFS v where   v.BILL_NO like 'FPM%' and v.STATUS_CODE = 'OP' and v.Agency_Code='{0}' and  NOT EXISTS(select 1  from I_YFLX a
                       join I_Repayment b on a.parentobjectid = b.objectid where trim(b.jxsbm) = '{0}'  and ((SCBYJ = '清算' and YYBYJ ='清算') or SCBYJ is null) and v.BILL_NO = a.FKBH )", JXSBH);



        var DZJEBH = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlBH);

        int nub = 0;
        if (DZJEBH != null && DZJEBH.ToString() != "")
        {
            string[] strs = DZJEBH.ToString().Split('/');
            if (strs.Length == 3)
            {
                string str = strs[2];
                if (strs[2].Substring(0, 1) == "0")
                {
                    nub = Convert.ToInt32(strs[2].Substring(1, 1));
                }
                else
                    nub = Convert.ToInt32(strs[2]);
            }
        }
        string DZJEBHNew = "";
        nub = nub + 1;



        if (nub < 10)
            DZJEBHNew = "DR" + JXSBH + "/" + DateTime.Now.ToString("yyyyMMdd").Substring(2, 6) + "/0" + nub;
        else
            DZJEBHNew = "DR" + JXSBH + "/" + DateTime.Now.ToString("yyyyMMdd").Substring(2, 6) + "/" + nub;

        DataTable dtWorkItem = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlWorkItem);
        DataTable dtRepayment = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlRepayment);
        DataTable dtBill = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlBill);

        WorkItemServer Ws = new WorkItemServer();


        string WorkitemID = "";
        string InstanceID = "";
        if (dtWorkItem.Rows.Count > 0)
        {
            List<string> listStr = new List<string>();
            listStr.Add(DZJEBHNew);
            listStr.Add(DZJE);
            listStr.Add(JEDZSJ);
            listStr.Add(CWDZBZ);
            WorkitemID = dtWorkItem.Rows[0][0].ToString();
            InstanceID = dtWorkItem.Rows[0][1].ToString();
            string BizId = dtWorkItem.Rows[0][2].ToString();

            decimal totalHKJE = dtWorkItem.Rows[0][3] == null ? 0 : Convert.ToDecimal(dtWorkItem.Rows[0][3].ToString());
            decimal totalLXJE = dtWorkItem.Rows[0][4] == null ? 0 : Convert.ToDecimal(dtWorkItem.Rows[0][4].ToString());
            decimal totalDZJE = dtWorkItem.Rows[0][5] == null ? 0 : Convert.ToDecimal(dtWorkItem.Rows[0][5].ToString());




            if (dtRepayment.Rows.Count > 0)
            {
                decimal amt = 0;
                SetInstanceData(InstanceID, WorkitemID, "Repayment", "HKList", BizId, dtRepayment, ref  amt);
                totalHKJE = totalHKJE + amt;

            }
            if (dtBill.Rows.Count > 0)
            {
                decimal amt = 0;
                SetInstanceData(InstanceID, WorkitemID, "Repayment", "YFLX", BizId, dtBill, ref amt);
                totalLXJE = totalLXJE + amt;
            }

            totalDZJE = totalDZJE + Convert.ToDecimal(DZJE);
            listStr.Add(totalHKJE + "");
            listStr.Add(totalLXJE + "");
            listStr.Add(totalDZJE + "");

            SetInstanceData(InstanceID, WorkitemID, "Repayment", "DZJEList", BizId, listStr);

        }
        else
        {
            List<DataItemParam> paramValues = new List<DataItemParam>();

            decimal totalHKJE = 0;

            if (dtRepayment.Rows.Count > 0)
            {

                foreach (DataRow item in dtRepayment.Rows)
                {

                    totalHKJE = totalHKJE + Convert.ToDecimal(item["DKJE"].ToString());

                }

            }
            decimal totalLXJE = 0;
            if (dtBill.Rows.Count > 0)
            {

                foreach (DataRow item in dtBill.Rows)
                {

                    totalLXJE = totalLXJE + Convert.ToDecimal(item["LXJE"].ToString());
                }
            }
            decimal totalDZJE = Convert.ToDecimal(DZJE);



            if (dtRepayment.Rows.Count > 0)
            {
                string SCBYJ = "";
                if (totalDZJE == totalHKJE + totalLXJE)
                {
                    SCBYJ = "清算";
                }
                foreach (DataRow item in dtRepayment.Rows)
                {
                    DataItemParam dp3 = new DataItemParam();
                    dp3.ItemName = "HKList";
                    dp3.ItemValue = "[{\"ItemName\":\"" + "CX" + "\",\"ItemValue\":\"" + item["CX"].ToString() + "\"},{\"ItemName\":\"" + "CJH" + "\",\"ItemValue\":\"" + item["CJH"].ToString() + "\"},{\"ItemName\":\"" + "DKDQR" + "\",\"ItemValue\":\"" + item["DKDQR"].ToString() + "\"},{\"ItemName\":\"" + "FKFS" + "\",\"ItemValue\":\"" + item["FKFS"].ToString() + "\"},{\"ItemName\":\"" + "DKJE" + "\",\"ItemValue\":\"" + item["DKJE"].ToString() + "\"},{\"ItemName\":\"" + "HKSPRP" + "\",\"ItemValue\":\"" + item["HKSPRP"].ToString() + "\"},{\"ItemName\":\"" + "STATES" + "\",\"ItemValue\":\"" + item["STATES"].ToString() + "\"},{\"ItemName\":\"" + "SPBH" + "\",\"ItemValue\":\"" + item["SPBH"].ToString() + "\"},{\"ItemName\":\"" + "LSCJH" + "\",\"ItemValue\":\"" + item["LSCJH"].ToString() + "\"},{\"ItemName\":\"" + "CJHXGSJ" + "\",\"ItemValue\":\"" + item["CJHXGSJ"].ToString() + "\"},{\"ItemName\":\"" + "CLBH" + "\",\"ItemValue\":\"" + item["CLBH"].ToString() + "\"},{\"ItemName\":\"" + "SCBYJ" + "\",\"ItemValue\":\"" + SCBYJ + "\"}]";

                    paramValues.Add(dp3);
                }

            }

            if (dtBill.Rows.Count > 0)
            {
                string SCBYJ = "";
                if (totalDZJE == totalHKJE + totalLXJE)
                {
                    SCBYJ = "清算";
                }
                foreach (DataRow item in dtBill.Rows)
                {
                    DataItemParam dp3 = new DataItemParam();
                    dp3.ItemName = "YFLX";
                    dp3.ItemValue = "[{\"ItemName\":\"" + "FKBH" + "\",\"ItemValue\":\"" + item["FKBH"].ToString() + "\"},{\"ItemName\":\"" + "YM" + "\",\"ItemValue\":\"" + item["MM"].ToString() + "\"},{\"ItemName\":\"" + "JXJZRQ" + "\",\"ItemValue\":\"" + item["JXJZRQ"].ToString() + "\"},{\"ItemName\":\"" + "FXDQR" + "\",\"ItemValue\":\"" + item["FXDQR"].ToString() + "\"},{\"ItemName\":\"" + "LXJE" + "\",\"ItemValue\":\"" + item["LXJE"].ToString() + "\"},{\"ItemName\":\"" + "FXCLS" + "\",\"ItemValue\":\"" + item["FXCLS"].ToString() + "\"},{\"ItemName\":\"" + "SCBYJ" + "\",\"ItemValue\":\"" + SCBYJ + "\"}]";

                    paramValues.Add(dp3);
                }
            }

            string sqlixs = @"select Objectid from OT_User where APPELLATION = '" + JXSBH + "' and STATE =1 ";
            var jxsObjectID = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlixs);

            DataItemParam dp0 = new DataItemParam();
            dp0.ItemName = "JXSH3ID";
            dp0.ItemValue = jxsObjectID;


            DataItemParam dp1 = new DataItemParam();
            dp1.ItemName = "JXSBM";
            dp1.ItemValue = JXSBH;
            DataItemParam dp2 = new DataItemParam();
            dp2.ItemName = "JXS";
            dp2.ItemValue = JXS;



            DataItemParam dp4 = new DataItemParam();
            dp4.ItemName = "DZJEList";
            dp4.ItemValue = "[{\"ItemName\":\"" + "DZJEBH" + "\",\"ItemValue\":\"" + DZJEBHNew + "\"},{\"ItemName\":\"" + "DZJE" + "\",\"ItemValue\":\"" + DZJE + "\"},{\"ItemName\":\"" + "JEDZSJ" + "\",\"ItemValue\":\"" + JEDZSJ + "\"},{\"ItemName\":\"" + "CWDZBZ" + "\",\"ItemValue\":\"" + CWDZBZ + "\"},{\"ItemName\":\"" + "CJJEDZSJ" + "\",\"ItemValue\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}]"; ;


            DataItemParam dp5 = new DataItemParam();
            dp5.ItemName = "TotalHKJE";
            dp5.ItemValue = totalHKJE;
            DataItemParam dp6 = new DataItemParam();
            dp6.ItemName = "TotalLXJE";
            dp6.ItemValue = totalLXJE;
            DataItemParam dp7 = new DataItemParam();
            dp7.ItemName = "TotalDZJE";
            dp7.ItemValue = totalDZJE;
            DataItemParam dp8 = new DataItemParam();
            //DataItemParam dp9 = new DataItemParam();
            //DataItemParam dp10 = new DataItemParam();
            //DataItemParam dp11 = new DataItemParam();
            //DataItemParam dp12 = new DataItemParam();
            if (totalHKJE + totalLXJE == totalDZJE)
            {
                dp8.ItemName = "OperationStates";
                dp8.ItemValue = "运营部处理中";

                ////dp9.ItemName = "XZQSBJ";
                ////dp9.ItemValue = totalDZJE;

                ////dp10.ItemName = "SFBJ";
                ////dp10.ItemValue = totalDZJE;

                ////dp11.ItemName = "SSLX";
                ////dp11.ItemValue = totalLXJE;

                ////dp12.ItemName = "QSLX";
                ////dp12.ItemValue = totalLXJE;

            }
            else
            {
                dp8.ItemName = "OperationStates";
                dp8.ItemValue = "市场部处理中";
            }


            paramValues.Add(dp0);
            paramValues.Add(dp1);
            paramValues.Add(dp2);
            paramValues.Add(dp4);
            paramValues.Add(dp5);
            paramValues.Add(dp6);
            paramValues.Add(dp7);
            paramValues.Add(dp8);
            //paramValues.Add(dp9);
            //paramValues.Add(dp10);
            //paramValues.Add(dp11);
            //paramValues.Add(dp12);

            Ws.startWorkflow("Repayment", this.UserValidator.UserCode, true, paramValues);
        }




        var result = new
        {
            Success = true

        };

        return Json(result, JsonRequestBehavior.AllowGet);

    }

    public bool SetInstanceData(string InstanceID, string WorkitemID, string bizObjectSchemaCode, string childSchemaCode, string detObjectID, List<string> strList)
    {
        BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(bizObjectSchemaCode);
        BizObject bo = new BizObject(Engine, schema, "");

        //BizObjectID 
        bo.ObjectID = detObjectID;
        bo.Load(); // Load 是装载数据

        bo["TotalHKJE"] = strList[4];
        bo["TotalLXJE"] = strList[5];
        bo["TotalDZJE"] = strList[6];
        //bo["XZQSBJ"] = strList[4];
        //bo["SFBJ"] = strList[4];
        //bo["SSLX"] = strList[5];
        //bo["QSLX"] = strList[5];

        //设置主表字段的值
        if (!string.IsNullOrEmpty(childSchemaCode))
        {
            int i = 0;
            if (bo[childSchemaCode]!=null)
            foreach (var v in (OThinker.H3.DataModel.BizObject[])bo[childSchemaCode])
            {
                i++;
            }
            BizObjectSchema childSchema = schema.GetProperty(childSchemaCode).ChildSchema;
            BizObject[] bizObjects = new BizObject[i + 1];
            int j = 0;
            if (bo[childSchemaCode] != null)
            foreach (var v in (OThinker.H3.DataModel.BizObject[])bo[childSchemaCode])
            {
                bizObjects[j] = new BizObject(this.Engine, childSchema, this.UserValidator.UserID);
                bizObjects[j] = v;
                j++;
            }



            // 第一行
            bizObjects[i] = new BizObject(this.Engine, childSchema, this.UserValidator.UserID);
            bizObjects[i]["DZJEBH"] = strList[0];
            bizObjects[i]["DZJE"] = strList[1];
            bizObjects[i]["JEDZSJ"] = strList[2];
            bizObjects[i]["CWDZBZ"] = strList[3];
            bizObjects[i]["CJJEDZSJ"] = DateTime.Now;



            bo[childSchemaCode] = bizObjects;
        }

        return bo.Update();
    }

    public bool SetInstanceData(string InstanceID, string WorkitemID, string bizObjectSchemaCode, string childSchemaCode, string detObjectID, DataTable dt, ref decimal amt)
    {
        BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(bizObjectSchemaCode);
        BizObject bo = new BizObject(Engine, schema, "");

        //BizObjectID 
        bo.ObjectID = detObjectID;
        bo.Load(); // Load 是装载数据
        //设置主表字段的值
        int i = 0;
        if (bo[childSchemaCode] != null)
            foreach (var v in (OThinker.H3.DataModel.BizObject[])bo[childSchemaCode])
            {
                i++;
            }
        BizObjectSchema childSchema = schema.GetProperty(childSchemaCode).ChildSchema;
        BizObject[] bizObjects = new BizObject[i + dt.Rows.Count];
        int j = 0;
        if (bo[childSchemaCode] != null)
            foreach (var v in (OThinker.H3.DataModel.BizObject[])bo[childSchemaCode])
            {
                bizObjects[j] = new BizObject(this.Engine, childSchema, this.UserValidator.UserID);
                bizObjects[j] = v;
                j++;
            }



        foreach (DataRow dr in dt.Rows)
        {
            bizObjects[i] = new BizObject(this.Engine, childSchema, this.UserValidator.UserID);
            foreach (string item in childSchema.GetPropertyNames())
            {
                if (dt.Columns.Contains(item))
                {
                    if ("DKJE" == item)
                        amt = amt + Convert.ToDecimal(dr["DKJE"].ToString());
                    if ("LXJE" == item)
                        amt = amt + Convert.ToDecimal(dr["LXJE"].ToString());
                    bizObjects[i][item] = dr[item].ToString();
                }

            }
            i++;
        }

        bo[childSchemaCode] = bizObjects;

        return bo.Update();
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