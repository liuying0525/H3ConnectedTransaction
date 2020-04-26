using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;
using System.Web.Script.Serialization;
using OThinker.H3.Controllers;
using OThinker.H3.Portal;
using System.Web.Mvc;
using WapiDZEQ;



namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class JXSInformationController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JXSInformationController()
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
        public const string dataCode = "Wholesale";
        public JsonResult JXSInformation(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            DataSet ds = GETJXSInformation(pagerInfo, JXS, JXSCODE);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JXSHKInfo(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            DataSet ds = GETJXSHKInfo(pagerInfo, JXS, JXSCODE);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        }

        public DataSet GETJXSHKInfo(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            var UserID = this.UserValidator.UserID;

            string sql = string.Format(@"select jxs ,jxsbm ,to_char(EXPIRY_DATE,'yyyy-mm-dd') EXPIRY_DATE,BONDPROPORTION,a.TOTALDZJE, a.TOTALLXJE,a.TOTALHKJE,a.wfpbj2,to_char(b.jedzsj,'yyyy-mm-dd') JEDZSJ,to_char(b.cjjedzsj,'yyyy-mm-dd hh24:mi:ss') CJJEDZSJ ,c.sequenceno 
from  i_Repayment a
join i_Dzjelist b  on a.objectid = b.parentobjectid
join Ot_Instancecontext c on c.bizobjectid = a.objectid
join IN_WFS.V_ZSUMMARY_DLR@TO_AUTH_WFS v on trim(v.dealer_code)=trim(a.jxsbm)
left join i_bond j on trim(a.jxsbm)=trim(j.distributorcode) and FINALSTATE='已生效' where c.state=4 ");

            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and a.jxs like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(JXSCODE))
            {
                sql += string.Format(" and a.jxsbm like '%{0}%'", JXSCODE);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY a.jxsbm";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                 if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " a.jxsbm " + pagerInfo.sSortDir_0.ToUpper();
                }
                 else if (pagerInfo.iSortCol_0 == 2)
                 {
                     OrderBy += " EXPIRY_DATE " + pagerInfo.sSortDir_0.ToUpper();
                 }
                 else if (pagerInfo.iSortCol_0 == 3)
                 {
                     OrderBy += " BONDPROPORTION " + pagerInfo.sSortDir_0.ToUpper();
                 }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " TOTALLXJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " TOTALDZJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " TOTALHKJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 7)
                {
                    OrderBy += " wfpbj2 " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 8)
                {
                    OrderBy += " jedzsj " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 9)
                {
                    OrderBy += " cjjedzsj  " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;
            #endregion

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1 ";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;

        }



        public DataSet GETJXSInformation(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format(@"select distinct trim(i.DEALER_NAME) JXS,
i.DEALER_CODE JXSCODE,
to_char(EXPIRY_DATE,'yyyy-mm-dd') SXDQR,
CREDIT_LIMIT_SYNC KZRZED,
AVAILABLE_LIMIT_SYNC KYSXED,
TOTAL_UNITS ZKSL,
UTILIZED_LIMIT_SYNC JZJRKYED,
FPM_UNPAID WFLXJE,
VIN_NO_UPDATED_NUM DXGCJH,
ASSET_OVERDUE YDQCL,
VEHICLES_MATURING_IN_2_WEEKS JYYDQCL ,
BONDPROPORTION BZJBL
from IN_WFS.V_ZSUMMARY_DLR@TO_AUTH_WFS v
inner join IN_WFS.V_DEALER_INFO@TO_AUTH_WFS i on v.dealer_code=i.dealer_code
left join i_bond j on trim(v.dealer_code)=j.distributorcode and FINALSTATE='已生效'
where 1=1");
            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and i.DEALER_NAME like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(JXSCODE))
            {
                sql += string.Format(" and i.DEALER_CODE like '%{0}%'", JXSCODE);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY i.DEALER_CODE";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 7)
                {
                    OrderBy += " FPM_UNPAID " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " to_char(EXPIRY_DATE,'yyyy-mm-dd') " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " i.DEALER_CODE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " CREDIT_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " UTILIZED_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " AVAILABLE_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " BONDPROPORTION " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 8)
                {
                    OrderBy += " TOTAL_UNITS " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 9)
                {
                    OrderBy += " VIN_NO_UPDATED_NUM " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 10)
                {
                    OrderBy += " ASSET_OVERDUE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 11)
                {
                    OrderBy += " VEHICLES_MATURING_IN_2_WEEKS " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;
            #endregion
            //string sql1 = "select rownum rn,a.* from (" + sql + ") a ";
            //string sqlstr = "select a1.* from ( " + sql + ") a1 where 1=1 ";

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1 ";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int count1 = 0;
                int count2 = 0;
                



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

                dpcl1 = string.Format(dpcl1, dt.Rows[i]["JXSCODE"].ToString());
                dpcl2 = string.Format(dpcl2, dt.Rows[i]["JXSCODE"].ToString());

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


                dpcl11 = string.Format(dpcl11, dt.Rows[i]["JXSCODE"].ToString());
                dpc12 = string.Format(dpc12, dt.Rows[i]["JXSCODE"].ToString());


                var dpclCount11 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpcl11);
                var dpclCount12 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpc12);


                count2 = Convert.ToInt32(dpclCount11) + Convert.ToInt32(dpclCount12);

                dt.Rows[i]["YDQCL"] = count1;
                dt.Rows[i]["JYYDQCL"] = count2;

            }

            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;

        }


        public JsonResult Export(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            string sql = @"select distinct trim(i.DEALER_NAME) 经销商名称,
i.DEALER_CODE 经销商编号,
to_char(EXPIRY_DATE,'yyyy-mm-dd') 授信到期日,
CREDIT_LIMIT_SYNC 核准融资额度,
UTILIZED_LIMIT_SYNC 截至今日可用额度,
AVAILABLE_LIMIT_SYNC 目前可用授信额度,
BONDPROPORTION 保证金比例,
FPM_UNPAID 未付利息金额,
TOTAL_UNITS 已放款车辆总计,
VIN_NO_UPDATED_NUM 车架号待修改,
ASSET_OVERDUE 已到期车辆,
VEHICLES_MATURING_IN_2_WEEKS 近1月即将到期车辆
from IN_WFS.V_ZSUMMARY_DLR@TO_AUTH_WFS v
inner join IN_WFS.V_DEALER_INFO@TO_AUTH_WFS i on v.dealer_code=i.dealer_code
left join i_bond j on trim(v.dealer_code)=j.distributorcode and FINALSTATE='已生效'
where 1=1 {0} {1}";
            #region 查询条件
            string conditions = "";
            if (!string.IsNullOrEmpty(JXS))
            {
                conditions += string.Format(" and i.DEALER_NAME like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(JXSCODE))
            {
                conditions += string.Format(" and i.DEALER_CODE like '%{0}%'", JXSCODE);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY i.DEALER_CODE";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " FPM_UNPAID " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " to_char(EXPIRY_DATE,'yyyy-mm-dd') " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " i.DEALER_CODE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " CREDIT_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " UTILIZED_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " AVAILABLE_LIMIT_SYNC " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " BONDPROPORTION " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 8)
                {
                    OrderBy += " TOTAL_UNITS " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 9)
                {
                    OrderBy += " VIN_NO_UPDATED_NUM " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 10)
                {
                    OrderBy += " ASSET_OVERDUE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 11)
                {
                    OrderBy += " VEHICLES_MATURING_IN_2_WEEKS " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            #endregion

            string UserCode = this.UserValidator.User.Appellation;
            sql = string.Format(sql, conditions, OrderBy);
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int count1 = 0;
                int count2 = 0;




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

                dpcl1 = string.Format(dpcl1, dt.Rows[i]["经销商编号"].ToString());
                dpcl2 = string.Format(dpcl2, dt.Rows[i]["经销商编号"].ToString());

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


                dpcl11 = string.Format(dpcl11, dt.Rows[i]["经销商编号"].ToString());
                dpc12 = string.Format(dpc12, dt.Rows[i]["经销商编号"].ToString());


                var dpclCount11 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpcl11);
                var dpclCount12 = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(dpc12);


                count2 = Convert.ToInt32(dpclCount11) + Convert.ToInt32(dpclCount12);

                dt.Rows[i]["已到期车辆"] = count1;
                dt.Rows[i]["近1月即将到期车辆"] = count2;

            }



            CurrencyClass dd = new CurrencyClass();
            dd.ExportReportCurrency(dt, "经销商汇总信息" + DateTime.Now.ToString("yyyyMMdd"));
            return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportHK(PagerInfo pagerInfo, string JXS, string JXSCODE)
        {
            string sql = string.Format(@"select jxs 经销商名称 ,jxsbm 经销商编码 ,to_char(EXPIRY_DATE,'yyyy-mm-dd') 授信到期日,BONDPROPORTION 保证金比例 ,a.TOTALDZJE 到账总金额, a.TOTALLXJE 还款利息金额,a.TOTALHKJE 还款本金,a.wfpbj2 未分配金额,to_char(jedzsj,'yyyy-mm-dd') 入账日期,to_char(cjjedzsj,'yyyy-mm-dd hh24:mi:ss') 创建入账时间,c.sequenceno 编号
from  i_Repayment a
join i_Dzjelist b  on a.objectid = b.parentobjectid
join Ot_Instancecontext c on c.bizobjectid = a.objectid
join IN_WFS.V_ZSUMMARY_DLR@TO_AUTH_WFS v on trim(v.dealer_code)=trim(a.jxsbm)
left join i_bond j on trim(a.jxsbm)=trim(j.distributorcode) and FINALSTATE='已生效' where c.state=4");

            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and a.jxs like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(JXSCODE))
            {
                sql += string.Format(" and a.jxsbm like '%{0}%'", JXSCODE);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY a.jxsbm";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 1)
                {
                    OrderBy += " a.jxsbm " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " EXPIRY_DATE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 3)
                {
                    OrderBy += " TOTALLXJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 4)
                {
                    OrderBy += " TOTALDZJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 5)
                {
                    OrderBy += " TOTALHKJE " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 6)
                {
                    OrderBy += " wfpbj2 " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 7)
                {
                    OrderBy += " jedzsj " + pagerInfo.sSortDir_0.ToUpper();
                }
                else if (pagerInfo.iSortCol_0 == 8)
                {
                    OrderBy += " cjjedzsj  " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;
            #endregion

            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            CurrencyClass dd = new CurrencyClass();
            dd.ExportReportCurrency(dt, "经销商还款信息" + DateTime.Now.ToString("yyyyMMdd"));
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
}