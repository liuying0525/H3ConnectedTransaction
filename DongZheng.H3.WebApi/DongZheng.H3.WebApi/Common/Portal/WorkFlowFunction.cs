using OThinker.H3.Controllers;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using OThinker.H3;
/// <summary>
/// WorkFlowFunction 的摘要说明
/// </summary>
/// 
namespace DongZheng.H3.WebApi.Common.Portal
{
    public class WorkFlowFunction
    {
        public WorkFlowFunction()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set
            {
                _Engine = value;
            }
        }

        public static string getWorkItemIDByInstanceid(string instanceid)
        {
            string rtn = "";
            string sql = "select OBJECTID from H3.OT_WORKITEM  where INSTANCEID='" + instanceid + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }
        public static string getRSWorkItemIDByInstanceid(string instanceid)
        {
            string rtn = "";
            string sql = "select objectid from H3.OT_WORKITEM where displayname like '%风控%' and instanceid='" + instanceid + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }
        public static string getWorkItemIDByInstanceidAndActivityCode(string instanceid, string ActivityCode)
        {
            string rtn = "";
            string sql = "select objectid from H3.OT_WORKITEM where activitycode='" + ActivityCode + "' and instanceid ='" + instanceid + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }
        public static bool isExistApplyNo(string ApplyNo)
        {
            bool rtn = false;
            string sql = "select objectid from H3.I_XSSP where applyno ='" + ApplyNo + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = true;
            }
            return rtn;
        }
        public static int insertLYInfo(string instanceid, string userid, string ly)
        {
            string sql = "insert into H3.I_LY(objectid,parentobjectid,parentpropertyname,parentindex,lyxx,lyry,lysj) values ('" + Guid.NewGuid() + "',    '" + instanceid + "',    'LY',    '0', '" + ly + "','" + userid + "','" + DateTime.Now.ToString() + "')";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }
        public static DataTable getLYInfo(string instanceid)
        {
            string sql = "select a.*,b.name as username from H3.I_LY a,OT_User b where A.LYRY=B.OBJECTID and ";
            sql += "a.Parentobjectid = '" + instanceid + "' order by to_date(a.lysj, 'yyyy-mm-dd hh24:mi:ss') desc";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        }
        public DataTable getJXSFQBL()
        {
            string sql = "SELECT '东正金融' ZBMC,";
            sql += "        贷款编码 GSDM,";
            sql += "        TO_CHAR (SYSDATE - 1, 'YYYY-MM-DD') RQ,";
            sql += "        数量 SL,";
            sql += "        金额 JE";
            sql += "   FROM (  SELECT CONCAT ('`', SUBSTR (TRIM (合同激活日期), 1, 7))";
            sql += "                     合同激活月份,";
            sql += "                  贷款编码,";
            sql += "                  COUNT (申请号) 数量,";
            sql += "                  SUM (贷款本金) 金额";
            sql += "             FROM (SELECT C.PROPOSAL_NBR 申请号,";
            sql += "                          zc.GSDM 贷款编码,";
            sql += "                          C.EXTERNAL_CONTRACT_NBR 合同号,";
            sql += "                          AT.NAME 客户名称,";
            sql += "                          CASE AT.APPLICANT_TYPE ";
            sql += "                             WHEN 'I' THEN '个人'";
            sql += "                             WHEN 'C' THEN '公司'";
            sql += "                             ELSE '其他'         ";
            sql += "                          END                    ";
            sql += "                             客户类型,           ";
            sql += "                          CASE                   ";
            sql += "                             WHEN A.USER_NAME IN ('9821503', '9821504')  ";
            sql += "                             THEN                                        ";
            sql += "                                '天津汽车工业销售深圳南方有限公司'       ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652701000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652701'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('上海锋之行-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652702000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652702'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('深圳彦信-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652703000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652703'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('云南瑞合福德-',                 ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652704000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652704'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('上海举骋-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652705000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652705'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('贵州瑞合行-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652706000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652706'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('抚州东信-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652707000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652707'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('济南鹏信-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652708000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652708'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('武汉正邦兴达-',                 ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652709000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652709'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('河南必诚-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652710000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652710'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('厦门大顺-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652711000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652711'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('湖南融晟行-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652712000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652712'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('广西瑞合行-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652713000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652713'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('上海森那美-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652714000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652714'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('成都劲翔-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652715000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652715'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('海南广汇鑫-',                   ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652716000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652716'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('吉林坔水-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             WHEN     BM.PMS_DEALER_NBR <> '68.652717000'";
            sql += "                                  AND SUBSTR (BM.PMS_DEALER_NBR, 1, 9) = ";
            sql += "                                         '68.652717'                     ";
            sql += "                             THEN                                        ";
            sql += "                                CONCAT ('合肥速融-',                     ";
            sql += "                                        BM.BUSINESS_PARTNER_NME)         ";
            sql += "                             ELSE                                        ";
            sql += "                                BM.BUSINESS_PARTNER_NME                  ";
            sql += "                          END                                            ";
            sql += "                             经销商名称,                                 ";
            sql += "                          CASE SUBSTR (BM.PMS_DEALER_NBR, 1, 3)          ";
            sql += "                             WHEN '68.' THEN '外网'                      ";
            sql += "                             WHEN '98.' THEN '内网'                      ";
            sql += "                             ELSE '不详'                                 ";
            sql += "                          END                                            ";
            sql += "                             渠道,                                       ";
            sql += "                          FA.FINANCED_AMT 贷款本金,                      ";
            sql += "                          FA.CUSTOMER_RTE 客户利率,                      ";
            sql += "                          FA.ACTUAL_RTE 实际利率,                        ";
            sql += "                          CASE                                           ";
            sql += "                             WHEN A.APPLICATION_NUMBER = 'Br-A000407000' ";
            sql += "                             THEN                                        ";
            sql += "                                '2015-06-18'                             ";
            sql += "                             ELSE                                        ";
            sql += "                                CONCAT (                                 ";
            sql += "                                   '   ',                                ";
            sql += "                                   TO_CHAR (C.CONTRACT_ACTIVATION_DTE,   ";
            sql += "                                            'yyyy/mm/dd'))               ";
            sql += "                          END                                            ";
            sql += "                             合同激活日期,                               ";
            sql += "                          FA.MANUFACTURER_SUBSIDY_AMT 厂家贴息额,        ";
            sql += "                          FA.DEALER_SUBSIDY_AMT 经销商贴息额             ";
            sql += "                     FROM CONTRACT C                                     ";
            sql += "                          JOIN APPLICATION A                             ";
            sql += "                             ON     C.PROPOSAL_NBR = A.APPLICATION_NUMBER";
            sql += "                                AND C.EXTERNAL_CONTRACT_NBR IS NOT NULL";
            sql += "                                AND C.LIVE_STS <> 'D'";
            sql += "                          JOIN APPLICANT_TYPE AT";
            sql += "                             ON     C.PROPOSAL_NBR = AT.APPLICATION_NUMBER";
            sql += "                                AND AT.MAIN_APPLICANT = 'Y'";
            sql += "                          JOIN BP_MAIN BM";
            sql += "                             ON BM.BUSINESS_PARTNER_ID = A.BUSINESS_PARTNER_ID";
            sql += "                          JOIN FINANCIAL_AGREEMENT FA";
            sql += "                             ON C.CONTRACT_ID = FA.CONTRACT_ID";
            sql += "                          JOIN ZTCOMPANY ZC";
            sql += "                             ON TRIM (ZC.JXSMC) = TRIM (BM.BUSINESS_PARTNER_NME)";
            sql += "                    WHERE     TO_CHAR (C.CONTRACT_ACTIVATION_DTE, 'YYYYMM') >";
            sql += "                                 TO_CHAR (";
            sql += "                                    (SELECT ADD_MONTHS (SYSDATE, -1) FROM DUAL),";
            sql += "                                    'YYYYMM')";
            sql += "                          AND SUBSTR (BM.PMS_DEALER_NBR, 1, 3) = '98.')";
            sql += "         GROUP BY CONCAT ('`', SUBSTR (TRIM (合同激活日期), 1, 7)),";
            sql += "                  贷款编码)";

            return ExecuteDataTableSql("cap", sql);
        }
        public DataTable getJXSFQBLMX()
        {
            string sql = "SELECT ";
            sql += "          '车辆金额' ZBMC,";
            sql += "          ZC.GSDM  ,";
            sql += "       FA.FINANCED_AMT JE,";
            sql += "       CASE";
            sql += "          WHEN A.APPLICATION_NUMBER = 'Br-A000407000'";
            sql += "          THEN";
            sql += "             '2015-06-18'";
            sql += "          ELSE";
            sql += "             CONCAT ('   ',";
            sql += "                     TO_CHAR (C.CONTRACT_ACTIVATION_DTE, 'yyyy/mm/dd'))";
            sql += "       END";
            sql += "          RQ,";
            sql += "          VD.VIN_NUMBER  DPH,";
            sql += "           '' BZ";
            sql += "  FROM CONTRACT C";
            sql += "       JOIN APPLICATION A";
            sql += "          ON     C.PROPOSAL_NBR = A.APPLICATION_NUMBER";
            sql += "             AND C.EXTERNAL_CONTRACT_NBR IS NOT NULL";
            sql += "             AND C.LIVE_STS <> 'D'";
            sql += "       JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = A.BUSINESS_PARTNER_ID";
            sql += "       JOIN ZTCOMPANY ZC ON ZC.JXSMC=BM.BUSINESS_PARTNER_NME";
            sql += "       JOIN FINANCIAL_AGREEMENT FA ON C.CONTRACT_ID = FA.CONTRACT_ID";
            sql += "       JOIN VEHICLE_DETAIL VD ON VD.APPLICATION_NUMBER=C.PROPOSAL_NBR";
            sql += "       WHERE TO_CHAR(C.CONTRACT_ACTIVATION_DTE,'YYYYMM')>TO_CHAR((SELECT ADD_MONTHS(SYSDATE,-1) FROM DUAL ),'YYYYMM')";
            sql += "       AND SUBSTR (BM.PMS_DEALER_NBR, 1, 3)='98.'";

            return ExecuteDataTableSql("cap", sql);
        }
        public DataTable getDZJR_REFUSEDATA()
        {
            string StartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "SELECT '拒批数量' ZBMC,GSDM,COUNT(APPLICATIONNO) SL,RKSRQ,RJSRQ";
            sql += "  FROM ";
            sql += "  ((SELECT ZC.GSDM,IR.DEALERNAME, IR.ZSSHZT,IR.APPLICATIONNO ,IR.YYZSSHZT,'" + StartDate + "' RKSRQ,'" + EndDate + "' RJSRQ";
            sql += "  FROM I_RETAILAPP IR";
            sql += "  JOIN ZTCOMPANY ZC ON ZC.JXSMC=IR.DEALERNAME";
            sql += "  JOIN OT_INSTANCECONTEXT OI ON IR.OBJECTID = OI.BIZOBJECTID";
            sql += "  JOIN ";
            sql += "  (SELECT INSTANCEID,";
            sql += "                                                 FINISHERNAME,";
            sql += "                                                 FINISHTIME,";
            sql += "                                                 ROW_NUMBER ()";
            sql += "                                                 OVER (";
            sql += "                                                    PARTITION BY ACTIVITYCODE,";
            sql += "                                                                 INSTANCEID";
            sql += "                                                    ORDER BY FINISHTIME DESC)";
            sql += "                                                    RN";
            sql += "                                            FROM OT_WORKITEMFINISHED";
            sql += "                                           WHERE     WORKFLOWCODE IN ('RetailApp',";
            sql += "                                                                      'CompanyApp')";
            sql += "                                                 AND FINISHERNAME IS NOT NULL";
            sql += "                                                 AND ACTIVITYCODE = 'Activity13'";
            sql += "                                                 AND FINISHERNAME <> '系统管理员')";
            sql += "                                         OOW ON OOW.INSTANCEID = OI.OBJECTID AND RN=1";
            sql += "  WHERE IR.ZSSHZT='拒绝'  AND IR.OWNERPARENTID='28a0ba01-4f58-4097-96cb-77c2b09e8253' AND";
            sql += "  TO_CHAR(OOW.FINISHTIME,'YYYY-MM-DD')>='" + StartDate + "' AND  TO_CHAR(OOW.FINISHTIME,'YYYY-MM-DD')<'" + EndDate + "') ";
            sql += "  UNION ALL";
            sql += "  (SELECT ZC.GSDM,IR.DEALERNAME, IR.ZSSHZT,IR.APPLICATIONNO ,IR.YYZSSHZT,'" + StartDate + "' RKSRQ,'" + EndDate + "' RJSRQ";
            sql += "  FROM I_RETAILAPP IR";
            sql += "  JOIN ZTCOMPANY ZC ON ZC.JXSMC=IR.DEALERNAME";
            sql += "  JOIN OT_INSTANCECONTEXT OI ON IR.OBJECTID = OI.BIZOBJECTID";
            sql += "  JOIN ";
            sql += "  (SELECT INSTANCEID,";
            sql += "                                                 FINISHERNAME,";
            sql += "                                                 FINISHTIME,";
            sql += "                                                 ROW_NUMBER ()";
            sql += "                                                 OVER (";
            sql += "                                                    PARTITION BY ACTIVITYCODE,";
            sql += "                                                                 INSTANCEID";
            sql += "                                                    ORDER BY FINISHTIME DESC)";
            sql += "                                                    RN";
            sql += "                                            FROM OT_WORKITEMFINISHED";
            sql += "                                           WHERE     WORKFLOWCODE IN ('RetailApp',";
            sql += "                                                                      'CompanyApp')";
            sql += "                                                 AND FINISHERNAME IS NOT NULL";
            sql += "                                                 AND ACTIVITYCODE = 'Activity18'";
            sql += "                                                 AND FINISHERNAME <> '系统管理员')";
            sql += "                                         OOW ON OOW.INSTANCEID = OI.OBJECTID AND RN=1";
            sql += "  WHERE IR.YYZSSHZT='拒绝'  AND IR.OWNERPARENTID='28a0ba01-4f58-4097-96cb-77c2b09e8253' AND ";
            sql += "  TO_CHAR(OOW.FINISHTIME,'YYYY-MM-DD')>='" + StartDate + "' AND  TO_CHAR(OOW.FINISHTIME,'YYYY-MM-DD')<'" + EndDate + "') )";
            sql += "  GROUP BY GSDM,RKSRQ,RJSRQ";

            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        }
        public int CaculateWeekDay(int y, int m, int d)
        {
            if (m == 1 || m == 2)
            {
                m += 12;
                y--;
            }
            int week = (d + 2 * m + 3 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
            return week + 1;
        }
        public DataTable getCAPckje(string applicantcardid, string applicationnumber, string cap)
        {
            string sql = "select round(ce.net_financed_amt),ce.exp_application_number,ce.application_status_cde";
            sql += " ,ce.exp_applicant_card_id,ce.exp_applicant_name,ce.is_exposed_ind,fpg.fp_group_nme,ce.application_status_dte,ce.no_of_terms";
            sql += " ,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc";
            sql += " from CAP_EXPOSURE ce";
            sql += " join contract_detail cd on cd.application_number=ce.application_number";
            sql += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id";
            sql += " join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde";
            sql += " where 1=1 ";
            sql += " and ce.exp_applicant_card_id='" + applicantcardid + "'";
            //sql += " and ce.is_exposed_ind='T'";
            sql += " and ce.application_number='" + applicationnumber + "'";

            return ExecuteDataTableSql(cap, sql);
        }
        public DataTable getCMSckje(string applicantcardid, string applicationnumber, string cap)
        {
            string sql = "select  round(cme.principle_outstanding_amt), cme.exp_application_number,cme.contract_number,rs.request_status_dsc";
            sql += " ,cme.contract_status_cde,cme.identification_code,cme.exp_applicant_card_id,cme.exp_applicant_name";
            sql += " ,BM.BUSINESS_PARTNER_NME,fpg.fp_group_nme,cme.contract_dte,round(cme.interest_rate,2),round(cme.net_financed_amt)";
            sql += " ,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc,cme.overdue_30_days,cme.overdue_60_days,cme.overdue_90_days";
            sql += " ,cme.overdue_above_90_days,cme.overdue_above_120_days,cme.no_of_terms,cme.no_of_terms_paid,cme.is_exposed_ind";
            //sql+=" --,cme.*";
            sql += "  from CMS_EXPOSURE cme ";
            sql += "  join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql += "  JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number";
            sql += "  JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID";
            sql += "  join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id ";
            sql += "  join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde";
            sql += "  join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde";
            sql += "  join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde";
            sql += " where 1=1";
            sql += " and cme.application_number='" + applicationnumber + "'";
            //sql+=" --and IS_EXPOSED_IND = 'T'";
            sql += " and cme.exp_applicant_card_id='" + applicantcardid + "'";
            return ExecuteDataTableSql(cap, sql);
        }
        public DataTable getGJRCAPck(string applicantcardid, string applicationnumber, string cap)
        {
            string sql = "select round(ce.net_financed_amt),ce.exp_application_number,ce.application_status_cde,ce.application_status_dte,ce.identification_code";
            sql += " ,ce.exp_applicant_card_id,ce.exp_applicant_name,fpg.fp_group_nme,ce.no_of_terms";
            sql += " ,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc";
            sql += " from CAP_EXPOSURE ce";
            sql += " join contract_detail cd on cd.application_number=ce.application_number";
            sql += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id";
            sql += " join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde";
            sql += " where 1=1";
            sql += " and ce.application_number='" + applicationnumber + "'";
            sql += " and ce.identification_code in (2,3)";
            return ExecuteDataTableSql(cap, sql);
        }


        public DataTable getGJRCMSck(string applicantcardid, string applicationnumber, string cap)
        {
            string sql = "select  round(cme.principle_outstanding_amt), cme.exp_application_number,cme.contract_number,rs.request_status_dsc";
            sql += " ,cme.contract_status_cde,cme.identification_code,cme.exp_applicant_card_id,cme.exp_applicant_name";
            sql += " ,BM.BUSINESS_PARTNER_NME,fpg.fp_group_nme,cme.contract_dte,round(cme.interest_rate,2),round(cme.net_financed_amt)";
            sql += " ,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc,cme.overdue_30_days,cme.overdue_60_days,cme.overdue_90_days";
            sql += " ,cme.overdue_above_90_days,cme.overdue_above_120_days,cme.no_of_terms,cme.no_of_terms_paid";
            sql += " from CMS_EXPOSURE cme";
            sql += " join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql += " JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number";
            sql += " JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID";
            sql += " join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id";
            sql += " join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde";
            sql += " where 1=1";
            sql += " and cme.application_number='" + applicationnumber + "'";
            sql += " and cme.identification_code in (2,3)";
            return ExecuteDataTableSql(cap, sql);
        }
        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = this.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }
        public object ExecuteScalar(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteScalar(sql);
            }
            return null;
        }
        public string getinstenceid(string applicationno)
        {
            string rtn = "";
            string sql = " select objectid from H3.OT_instancecontext where bizobjectid in (select objectid from H3.I_RETAILAPP where applicationno='" + applicationno + "')";
            var rtninstanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
            if (rtninstanceid != null)
            {
                rtn = rtninstanceid.ToString();
            }
            else
            {
                rtn = "nofind";
            }
            return rtn;
        }

        public string getCinstenceid(string applicationno)
        {
            string rtn = "";
            string sql = " select objectid,'C' type from H3.OT_instancecontext where bizobjectid in ( select objectid from H3.I_CompanyApp where applicationno='" + applicationno + "') ";
            sql += " union all ";
            sql += " select objectid,'HC' type  from H3.OT_instancecontext where bizobjectid in (select objectid from H3.I_HCompanyApp where applicationno='" + applicationno + "')";
            sql += " union all ";
            sql += " select objectid,'R' type  from H3.OT_instancecontext where bizobjectid in (select objectid from H3.I_RetailApp where applicationno='" + applicationno + "') ";
            sql += " union all ";
            sql += " select objectid,'HR' type  from H3.OT_instancecontext where bizobjectid in (select objectid from H3.I_HRetailApp where applicationno='" + applicationno + "')";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rtn = dt.Rows[0][0].ToString() + ";" + dt.Rows[0][1].ToString();
            }
            else
            {
                rtn = "nofind";
            }
            return rtn;
        }
        /// <summary>
        /// 个人贷款申请
        /// </summary>
        /// <param name="bizObject"></param>
        /// <param name="xd"></param>
        /// <param name="workFlowCode"></param>
        /// <param name="userObjectid"></param>
        /// <returns></returns>
        public BizObject setIndvidualBizObject(BizObject bizObject, XmlDocument xd, string workFlowCode, string userObjectid, string subTable)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "解析CAP XML开始");
            XmlNode xn;
            CAPInfo cap = new CAPInfo();
            #region 数据赋值
            #region 节点appAsset赋值
            xn = xd.SelectSingleNode("//appAsset//data");
            appAsset appAsset = XmlUtil.Deserialize(typeof(appAsset), "<appAsset>" + xn.InnerXml + "</appAsset>") as appAsset;
            System.Reflection.PropertyInfo[] properties = appAsset.GetType().GetProperties();
            //foreach (System.Reflection.PropertyInfo property in properties)
            //{
            //    try
            //    {
            //        bizObject[property.Name] = property.GetValue(appAsset).ToString();
            //    }
            //    catch{ }
            //}
            bizObject["applicationNo"] = appAsset.applicationNo;
            bizObject["assetConditionName"] = appAsset.assetConditionName;
            bizObject["assetMakeName"] = appAsset.assetMakeName;
            bizObject["assetPrice"] = appAsset.assetPrice;
            bizObject["brandName"] = appAsset.brandName;
            bizObject["vecomments"] = appAsset.comments;
            bizObject["cylinder"] = appAsset.cylinder;
            bizObject["deliveryDate"] = appAsset.deliveryDate;
            bizObject["engineCC"] = appAsset.engineCC;
            bizObject["engineNo"] = appAsset.engineNo;
            bizObject["guranteeOption"] = appAsset.guranteeOption == "00001" ? "是" : "否";
            bizObject["manufactureYear"] = appAsset.manufactureYear;
            bizObject["miocn"] = appAsset.miocn;
            bizObject["modelName"] = appAsset.modelName;
            bizObject["odometer"] = appAsset.odometer;
            bizObject["purposeName"] = appAsset.purposeName;
            bizObject["registrationNo"] = appAsset.registrationNo;
            bizObject["releaseDate"] = appAsset.releaseDate;
            bizObject["releaseMonth"] = appAsset.releaseMonth;
            bizObject["releaseYear"] = appAsset.releaseYear;
            bizObject["series"] = appAsset.series;
            bizObject["style"] = appAsset.style;
            bizObject["transmissionName"] = appAsset.transmissionName;
            bizObject["vehicleAge"] = appAsset.vehicleAge;
            bizObject["vehicleBody"] = appAsset.vehicleBody;
            bizObject["vehiclecolorName"] = appAsset.vehiclecolorName;
            bizObject["vehicleSubtypeName"] = appAsset.vehicleSubtypeName;
            bizObject["vineditind"] = appAsset.vineditind == "T" ? "是" : "否";
            bizObject["vinNo"] = appAsset.vinNo;
            bizObject["wheelWidth"] = appAsset.wheelWidth;

            #endregion
            #region 节点appFinanTerms赋值
            try
            {
                xn = xd.SelectSingleNode("//appFinanTerms//data");
                appFinanTerms appFinanTerms = XmlUtil.Deserialize(typeof(appFinanTerms), "<appFinanTerms>" + xn.InnerXml + "</appFinanTerms>") as appFinanTerms;

                //bizObject["accessoriesInd"] = appFinanTerms.accessoriesInd.Contains("T") ? "是" : "否";
                bizObject["actualRate"] = appFinanTerms.actualRate;
                bizObject["applicationNo"] = appFinanTerms.applicationNo;
                bizObject["balloonRate"] = appFinanTerms.balloonRate;
                bizObject["deferredTerm"] = appFinanTerms.deferredTerm;
                bizObject["downpaymentamount"] = appFinanTerms.downpaymentamount;
                bizObject["downpaymentrate"] = appFinanTerms.downpaymentrate;
                // bizObject["Equals"] = appFinanTerms.Equals;
                bizObject["financedamount"] = appFinanTerms.financedamount;
                bizObject["financedamountrate"] = appFinanTerms.financedamountrate;
                bizObject["interestRate"] = appFinanTerms.interestRate;
                bizObject["otherfees"] = appFinanTerms.otherfees;
                bizObject["paymentFrequencyName"] = appFinanTerms.paymentFrequencyName;
                bizObject["productGroupName"] = appFinanTerms.productGroupName;
                bizObject["productTypeName"] = appFinanTerms.productTypeName;
                bizObject["rentalmodeName"] = appFinanTerms.rentalmodeName;
                bizObject["salesprice"] = appFinanTerms.salesprice;
                bizObject["subsidyAmount"] = appFinanTerms.subsidyAmount;
                bizObject["subsidyRate"] = appFinanTerms.subsidyRate;
                bizObject["termMonth"] = appFinanTerms.termMonth;
                bizObject["totalintamount"] = appFinanTerms.totalintamount;
                bizObject["vehicleprice"] = appFinanTerms.vehicleprice;
                bizObject["totalaccessoryamount"] = appFinanTerms.totalaccessoryamount;
            }
            catch { }
            #endregion
            #region 节点appRental还款计划赋值
            try
            {
                XmlNodeList appRentallst = xd.SelectSingleNode("//appRental").ChildNodes;
                foreach (XmlNode appRentalnode in appRentallst)
                {
                    appRental appRental = XmlUtil.Deserialize(typeof(appRental), "<appRental>" + appRentalnode.InnerXml + "</appRental>") as appRental;
                    cap.appRental.Add(appRental);
                }
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workFlowCode);
                // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）

                BizObjectSchema childSchema = schema.GetProperty(subTable).ChildSchema; // 获取子业务对象编码
                var lstChildBO = new List<BizObject>();
                /*
                这里类似以上BizObject的方法给 childBO 赋值
                例如：childBO["短文本数据项"] = "值";
                */
                foreach (var child in cap.appRental)
                {
                    BizObject childBO = new BizObject(this.Engine, childSchema, userObjectid);
                    childBO["startTerm"] = child.startTerm;
                    childBO["endTerm"] = child.endTerm;
                    childBO["rentalAmount"] = child.rentalAmount;
                    childBO["grossRentalAmount"] = child.grossRentalAmount;
                    bizObject["Bccdygje"] = child.grossRentalAmount;
                    lstChildBO.Add(childBO);
                }
                //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                bizObject[subTable] = lstChildBO.ToArray();
            }

            catch { }
            #endregion
            #region 节点assetAccessories附加费
            try
            {
                XmlNodeList assetAccessorieslst = xd.SelectSingleNode("//assetAccessories").ChildNodes;
                foreach (XmlNode assetAccessoriesnode in assetAccessorieslst)
                {
                    assetAccessories assetAccessories = XmlUtil.Deserialize(typeof(assetAccessories), "<assetAccessories>" + assetAccessoriesnode.InnerXml + "</assetAccessories>") as assetAccessories;
                    cap.assetAccessories.Add(assetAccessories);
                }
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workFlowCode);
                // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）

                BizObjectSchema childSchema = schema.GetProperty("RentalAssetAccessoriesTable").ChildSchema; // 获取子业务对象编码
                var lstChildBO = new List<BizObject>();
                /*
                这里类似以上BizObject的方法给 childBO 赋值
                例如：childBO["短文本数据项"] = "值";
                */
                foreach (var child in cap.assetAccessories)
                {
                    BizObject childBO = new BizObject(this.Engine, childSchema, userObjectid);
                    childBO["additionalCode"] = child.additionalCode;
                    childBO["additionalName1"] = child.additionalName;
                    childBO["additionalprice"] = child.price;
                    childBO["mainType"] = child.mainType;
                    lstChildBO.Add(childBO);
                }
                //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                bizObject["RentalAssetAccessoriesTable"] = lstChildBO.ToArray();

            }

            catch { }
            #endregion
            #region 节点appInfo赋值
            try
            {

                xn = xd.SelectSingleNode("//appInfo//data");
                appInfo appInfo = XmlUtil.Deserialize(typeof(appInfo), "<appInfo>" + xn.InnerXml + "</appInfo>") as appInfo;

                bizObject["APPCREATETIME"] = appInfo.APPCREATETIME;
                bizObject["applicationNo"] = appInfo.applicationNo;
                //bizObject["appStatus"] = appInfo.appStatus;
                bizObject["appTypeName"] = appInfo.appTypeName;
                //bizObject["comments"] = appInfo.comments;
                bizObject["companyName"] = appInfo.companyName;
                bizObject["ContactNumber"] = appInfo.ContactNumber;
                bizObject["dealercityName"] = appInfo.dealercityName;
                bizObject["dealerName"] = appInfo.dealerName;
                bizObject["distributorName"] = appInfo.distributorName;
                //bizObject["financialadvisorId"] = appInfo.financialadvisorId;
                bizObject["financialadvisorName"] = appInfo.financialadvisorName;
                bizObject["provinceName"] = appInfo.provinceName;
                bizObject["refCANumber"] = appInfo.refCANumber;
                bizObject["salesPersonName"] = appInfo.salesPersonName;
                bizObject["showRoomName"] = appInfo.showRoomName;
                bizObject["userName"] = appInfo.userName;
                bizObject["vehicleTypeName"] = appInfo.vehicleTypeName;

            }

            catch { }
            #endregion
            XmlNodeList xnlst = xd.SelectSingleNode("//applicantInfo").ChildNodes;
            string identification_code = "";
            string strRole = "";
            foreach (XmlNode child in xnlst)
            {
                applicantInfo applicantInfo = XmlUtil.Deserialize(typeof(applicantInfo), "<applicantInfo>" + child.InnerXml + "</applicantInfo>") as applicantInfo;
                //主借人
                if (applicantInfo.APPLICANTROLE == "B")
                {
                    strRole += "B";
                    identification_code = applicantInfo.IDENTIFICATION_CODE;
                    #region 基本信息 节点applicantInfo赋值
                    try
                    {
                        bizObject["actualsalary"] = applicantInfo.actualsalary;
                        //bizObject["ageInMonth"] = applicantInfo.ageInMonth;
                        //bizObject["ageInYear"] = applicantInfo.ageInYear;
                        //bizObject["ageInYYMM"] = applicantInfo.ageInYYMM;
                        //  bizObject["APPLICANTROLE"] = applicantInfo.APPLICANTROLE;
                        //  bizObject["APPLICATION_NUMBER"] = applicantInfo.APPLICATION_NUMBER;
                        // bizObject["bankstatementind"] = applicantInfo.bankstatementind;
                        // bizObject["blacklistcontractind"] = applicantInfo.blacklistcontractind;
                        // bizObject["blacklistdurationmonthNo"] = applicantInfo.blacklistdurationmonthNo;
                        bizObject["blacklistind"] = applicantInfo.blacklistind == "T" ? "是" : "否";
                        bizObject["blacklistnorecordind"] = applicantInfo.blacklistnorecordind == "T" ? "是" : "否";
                        bizObject["childrenflag"] = applicantInfo.childrenflag == "Y" ? "是" : "否";
                        bizObject["citizenshipName"] = applicantInfo.citizenshipName;
                        bizObject["consent"] = applicantInfo.consent == "Y" ? "是" : "否";
                        //bizObject["customerid"] = applicantInfo.customerid;
                        bizObject["ThaiFirstName"] = applicantInfo.ThaiFirstName;
                        bizObject["dateofbirth"] = applicantInfo.dateofbirth.Split(' ')[0];
                        bizObject["desginationName"] = applicantInfo.desginationName;
                        bizObject["drivinglicensestatusName"] = applicantInfo.drivinglicensestatusName;
                        bizObject["educationName"] = applicantInfo.educationName;
                        bizObject["emailaddress"] = applicantInfo.emailaddress;
                        bizObject["EmployerType"] = applicantInfo.employeetypeName;
                        bizObject["engfirstname"] = applicantInfo.engfirstname;
                        bizObject["englastname"] = applicantInfo.englastname;
                        //   bizObject["engmiddlename"] = applicantInfo.engmiddlename;
                        // bizObject["familyavgincome"] = applicantInfo.familyavgincome;
                        bizObject["formername"] = applicantInfo.formername;
                        bizObject["genderName"] = applicantInfo.genderName;
                        bizObject["houseownername"] = applicantInfo.houseownername;
                        bizObject["hokouName"] = applicantInfo.hukouName;
                        bizObject["idcardexpirydate"] = applicantInfo.idcardexpirydate;
                        bizObject["idcardissuedate"] = applicantInfo.idcardissuedate;
                        bizObject["IdCardNo"] = applicantInfo.idcardnumber; //111
                        bizObject["IdCarTypeName"] = applicantInfo.idcardtypeName;//222
                        bizObject["IndustrySubTypeName"] = applicantInfo.IndustrySubTypeName;
                        bizObject["industrytypeName"] = applicantInfo.industrytypeName;
                        bizObject["issuingAuthority"] = applicantInfo.issuingAuthority;
                        bizObject["jobgroupName"] = applicantInfo.jobgroupName;
                        //   bizObject["lettercertifyind"] = applicantInfo.lettercertifyind;
                        //  bizObject["licenseexpirydate"] = applicantInfo.licenseexpirydate;
                        bizObject["LicenseNo"] = applicantInfo.licenseNumber;
                        bizObject["lienee"] = applicantInfo.lienee == "T" ? "是" : "否";
                        bizObject["maritalstatusname"] = applicantInfo.maritalstatusname;
                        bizObject["nationName"] = applicantInfo.nationName;
                        bizObject["nooffamilymembers"] = applicantInfo.nooffamilymembers;
                        bizObject["numberofdependents"] = applicantInfo.numberofdependents;
                        bizObject["occupationName"] = applicantInfo.occupationName;
                        //   bizObject["occupationType"] = applicantInfo.occupationType;
                        bizObject["relationShipName"] = applicantInfo.relationShipName;
                        bizObject["salaryrangeName"] = applicantInfo.salaryrangeName;
                        // bizObject["salayslipind"] = applicantInfo.salayslipind;
                        bizObject["staffind"] = applicantInfo.staffind == "T" ? "是" : "否";
                        bizObject["subOccupationName"] = applicantInfo.subOccupationName;
                        //bizObject["thailastname"] = applicantInfo.thailastname;
                        // bizObject["thaimiddlename"] = applicantInfo.thaimiddlename;
                        bizObject["thaititlename"] = applicantInfo.thaititlename;
                        //  bizObject["timeasCustomer"] = applicantInfo.timeasCustomer;
                        bizObject["titlename"] = applicantInfo.titlename;
                        //    bizObject["tradingas"] = applicantInfo.tradingas;
                        bizObject["actualsalary"] = applicantInfo.actualsalary;
                        bizObject["vipind"] = applicantInfo.vipind == "T" ? "是" : "否";
                        bizObject["EmploymentTypeName"] = applicantInfo.employeetypeName;
                    }
                    catch { }
                    #endregion

                    #region 工作信息 节点employment赋值
                    try
                    {
                        XmlNodeList employmentlst = xd.SelectSingleNode("//employment").ChildNodes;
                        foreach (XmlNode employmentnode in employmentlst)
                        {
                            if (employmentnode.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                employment employment = XmlUtil.Deserialize(typeof(employment), "<employment>" + employmentnode.InnerXml + "</employment>") as employment;
                                bizObject["businesstypeName"] = employment.businesstypeName;
                                bizObject["companyCityName"] = employment.cityName;
                                bizObject["companyComments"] = employment.comments;
                                bizObject["companyAddress"] = employment.companyAddress;
                                bizObject["ZjrcompanyName"] = employment.companyName;
                                bizObject["employertype"] = employment.employertype;
                                bizObject["Fax"] = employment.Fax;
                                bizObject["jobdescription"] = employment.jobdescription;
                                //  bizObject["lineId"] = employment.lineId;
                                bizObject["noofemployees"] = employment.noofemployees;
                                bizObject["phonenumber"] = employment.phonenumber;
                                bizObject["positionName"] = employment.positionName;
                                bizObject["companypostCode"] = employment.postCode;
                                bizObject["comapnyProvinceName"] = employment.provinceName;
                                bizObject["timeinyear"] = employment.timeinyear;
                                bizObject["timeinmonth"] = employment.timeinmonth;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region 地址 节点addressList，phoneList赋值
                    try
                    {
                        XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                        foreach (XmlNode address in addresslst)
                        {
                            if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                bizObject["AddressId"] = addressList.AddressId;
                                bizObject["addressstatusName"] = addressList.addressstatusName;
                                bizObject["addresstypeName"] = addressList.addresstypeName;
                                bizObject["birthpalaceprovince"] = addressList.birthpalaceprovince;
                                bizObject["hukoucityName"] = addressList.cityName;
                                bizObject["countryName"] = addressList.countryName;
                                bizObject["currentLivingAddress"] = addressList.currentLivingAddress;
                                bizObject["defaultmailingaddress"] = addressList.defaultmailingaddress == "Y" ? "是" : "否";
                                bizObject["homevalue"] = addressList.homevalue;
                                bizObject["hukouaddress"] = addressList.hukouaddress == "Y" ? "是" : "否";
                                bizObject["livingsince"] = addressList.livingsince;
                                bizObject["nativedistrict"] = addressList.nativedistrict;
                                bizObject["postcode"] = addressList.postcode;
                                bizObject["propertytypeName"] = addressList.propertytypeName;
                                bizObject["hukouprovinceName"] = addressList.provinceName;
                                bizObject["residencetypeName"] = addressList.residencetypeName;
                                //-少建 bizObject["stayinmonth"] = addressList.stayinmonth;
                                bizObject["stayinyear"] = addressList.stayinyear;
                                break;
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                        foreach (XmlNode phone in phonelst)
                        {
                            if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                //  bizObject["areaCode"] = phoneList.areaCode;
                                bizObject["countrycodeName"] = phoneList.countrycodeName;
                                bizObject["extension"] = phoneList.extension;
                                bizObject["phoneNo"] = phoneList.phoneNo;
                                bizObject["phonetypeName"] = phoneList.phonetypeName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region 联系人 referenceList赋值
                    try
                    {
                        XmlNodeList referencelst = xd.SelectSingleNode("//referenceList").ChildNodes;
                        foreach (XmlNode reference in referencelst)
                        {
                            if (reference.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                referenceList referenceList = XmlUtil.Deserialize(typeof(referenceList), "<referenceList>" + reference.InnerXml + "</referenceList>") as referenceList;

                                bizObject["name1"] = referenceList.name;
                                bizObject["address"] = referenceList.address;
                                bizObject["lxcityName"] = referenceList.cityName;
                                bizObject["lxhokouName"] = referenceList.hokouName;
                                bizObject["lxmobile"] = referenceList.mobile;
                                bizObject["lxphoneNo"] = referenceList.phoneNo;

                                bizObject["lxpostCode"] = referenceList.postCode;
                                bizObject["lxprovinceCode"] = referenceList.provinceName;
                                bizObject["relationshipName"] = referenceList.relationshipName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
                //共借人
                else if (applicantInfo.APPLICANTROLE == "C")
                {
                    strRole += "C";
                    identification_code = applicantInfo.IDENTIFICATION_CODE;
                    #region 基本信息 节点applicantInfo赋值
                    try
                    {
                        bizObject["Gjactualsalary"] = applicantInfo.actualsalary;
                        //  bizObject["GjageInMonth"] = applicantInfo.ageInMonth;
                        //  bizObject["GjageInYear"] = applicantInfo.ageInYear;
                        // bizObject["GjageInYYMM"] = applicantInfo.ageInYYMM;
                        bizObject["Gjblacklistind"] = applicantInfo.blacklistind == "T" ? "是" : "否";
                        bizObject["Gjblacklistnorecordind"] = applicantInfo.blacklistnorecordind == "T" ? "是" : "否";
                        bizObject["Gjchildrenflag"] = applicantInfo.childrenflag == "Y" ? "是" : "否";
                        bizObject["GjcitizenshipName"] = applicantInfo.citizenshipName;
                        bizObject["Gjconsent"] = applicantInfo.consent == "Y" ? "是" : "否";
                        //bizObject["Gjcustomerid"] = applicantInfo.customerid;
                        bizObject["GjThaiFirstName"] = applicantInfo.ThaiFirstName;
                        bizObject["Gjdateofbirth"] = applicantInfo.dateofbirth.Split(' ')[0];
                        bizObject["GjdesginationName"] = applicantInfo.desginationName;
                        bizObject["GjdrivinglicensestatusName"] = applicantInfo.drivinglicensestatusName;
                        bizObject["GjeducationName"] = applicantInfo.educationName;
                        bizObject["Gjemailaddress"] = applicantInfo.emailaddress;
                        bizObject["GjEmployerType"] = applicantInfo.employeetypeName;
                        bizObject["Gjengfirstname"] = applicantInfo.engfirstname;
                        bizObject["Gjenglastname"] = applicantInfo.englastname;

                        //bizObject["Gjengmiddlename"] = applicantInfo.engmiddlename;
                        // bizObject["Gjfamilyavgincome"] = applicantInfo.familyavgincome;
                        bizObject["Gjformername"] = applicantInfo.formername;
                        bizObject["GjgenderName"] = applicantInfo.genderName;
                        bizObject["Gjhouseownername"] = applicantInfo.houseownername;
                        bizObject["GjhokouName"] = applicantInfo.hukouName;
                        bizObject["Gjidcardexpirydate"] = applicantInfo.idcardexpirydate;
                        bizObject["Gjidcardissuedate"] = applicantInfo.idcardissuedate;
                        bizObject["GjIdCardNo"] = applicantInfo.idcardnumber; //111
                        bizObject["GjIdCarTypeName"] = applicantInfo.idcardtypeName;//222
                        bizObject["GjIndustrySubTypeName"] = applicantInfo.IndustrySubTypeName;
                        bizObject["GjindustrytypeName"] = applicantInfo.industrytypeName;
                        bizObject["GjissuingAuthority"] = applicantInfo.issuingAuthority;
                        bizObject["GjjobgroupName"] = applicantInfo.jobgroupName;
                        bizObject["GjLicenseNo"] = applicantInfo.licenseNumber;
                        bizObject["Gjlienee"] = applicantInfo.lienee == "T" ? "是" : "否";
                        bizObject["Gjmaritalstatusname"] = applicantInfo.maritalstatusname;
                        bizObject["GjnationName"] = applicantInfo.nationName;
                        bizObject["Gjnooffamilymembers"] = applicantInfo.nooffamilymembers;
                        bizObject["Gjnumberofdependents"] = applicantInfo.numberofdependents;
                        bizObject["GjoccupationName"] = applicantInfo.occupationName;
                        bizObject["GjrelationShipName"] = applicantInfo.relationShipName;
                        bizObject["GjsalaryrangeName"] = applicantInfo.salaryrangeName;
                        //  bizObject["Gjsalayslipind"] = applicantInfo.salayslipind;
                        bizObject["Gjstaffind"] = applicantInfo.staffind == "T" ? "是" : "否";
                        bizObject["GjsubOccupationName"] = applicantInfo.subOccupationName;
                        bizObject["Gjthaititlename"] = applicantInfo.thaititlename;
                        bizObject["Gjtitlename"] = applicantInfo.titlename;
                        bizObject["Gjspouse"] = applicantInfo.spouse == "T" ? "是" : "否";
                        bizObject["Gjvipind"] = applicantInfo.vipind == "T" ? "是" : "否";
                        bizObject["GjEmploymentTypeName"] = applicantInfo.employeetypeName;
                    }
                    catch { }
                    #endregion

                    #region 是否面签 节点applicantType赋值
                    try
                    {
                        XmlNodeList applicantTypelst = xd.SelectSingleNode("//applicantType").ChildNodes;
                        foreach (XmlNode applicantTypenode in applicantTypelst)
                        {
                            if (applicantTypenode.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                applicantType applicantType = XmlUtil.Deserialize(typeof(applicantType), "<applicantType>" + applicantTypenode.InnerXml + "</applicantType>") as applicantType;

                                bizObject["IS_INACTIVE_IND"] = applicantType.IS_INACTIVE_IND.Contains("T") ? "是" : "否";
                                break;
                            }
                        }
                    }
                    catch
                    {
                        bizObject["IS_INACTIVE_IND"] = "否";
                    }
                    #endregion

                    #region 工作信息 节点employment赋值
                    try
                    {
                        XmlNodeList employmentlst = xd.SelectSingleNode("//employment").ChildNodes;
                        foreach (XmlNode employmentnode in employmentlst)
                        {
                            if (employmentnode.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                employment employment = XmlUtil.Deserialize(typeof(employment), "<employment>" + employmentnode.InnerXml + "</employment>") as employment;

                                bizObject["GjbusinesstypeName"] = employment.businesstypeName;
                                bizObject["GjCityName"] = employment.cityName;
                                bizObject["GjcompanyComments"] = employment.comments;
                                bizObject["GjcompanyAddress"] = employment.companyAddress;
                                bizObject["GjcompanyName"] = employment.companyName;
                                bizObject["Gjemployertype"] = employment.employertype;
                                bizObject["GjFax"] = employment.Fax;
                                bizObject["Gjjobdescription"] = employment.jobdescription;
                                //bizObject["GjlineId"] = employment.lineId;
                                bizObject["Gjnoofemployees"] = employment.noofemployees;
                                bizObject["Gjphonenumber"] = employment.phonenumber;
                                bizObject["GjpositionName"] = employment.positionName;
                                bizObject["GjcompanypostCode"] = employment.postCode;
                                bizObject["GjProvinceName"] = employment.provinceName;
                                bizObject["Gjtimeinyear"] = employment.timeinyear;
                                bizObject["Gjtimeinmonth"] = employment.timeinmonth;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region 联系信息 节点addressList，phoneList赋值
                    try
                    {
                        XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                        foreach (XmlNode address in addresslst)
                        {
                            if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                bizObject["GjAddressId"] = addressList.AddressId;
                                bizObject["GjaddressstatusName"] = addressList.addressstatusName;
                                bizObject["GjaddresstypeName"] = addressList.addresstypeName;
                                bizObject["Gjbirthpalaceprovince"] = addressList.birthpalaceprovince;
                                bizObject["GjhukoucityName"] = addressList.cityName;
                                bizObject["GjcountryName"] = addressList.countryName;
                                bizObject["GjcurrentLivingAddress"] = addressList.currentLivingAddress;
                                bizObject["Gjdefaultmailingaddress"] = addressList.defaultmailingaddress == "Y" ? "是" : "否";
                                bizObject["Gjhomevalue"] = addressList.homevalue;
                                bizObject["Gjhukouaddress"] = addressList.hukouaddress == "Y" ? "是" : "否";
                                bizObject["Gjlivingsince"] = addressList.livingsince;
                                bizObject["Gjnativedistrict"] = addressList.nativedistrict;
                                bizObject["Gjpostcode"] = addressList.postcode;
                                bizObject["GjpropertytypeName"] = addressList.propertytypeName;
                                bizObject["GjhukouprovinceName"] = addressList.provinceName;
                                bizObject["GjresidencetypeName"] = addressList.residencetypeName;
                                bizObject["Gjstayinmonth"] = addressList.stayinmonth;
                                bizObject["Gjstayinyear"] = addressList.stayinyear;
                                break;
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                        foreach (XmlNode phone in phonelst)
                        {
                            if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                bizObject["GjareaCode"] = phoneList.areaCode;
                                bizObject["GjcountrycodeName"] = phoneList.countrycodeName;
                                bizObject["Gjextension"] = phoneList.extension;
                                bizObject["GjphoneNo"] = phoneList.phoneNo;
                                bizObject["GjphonetypeName"] = phoneList.phonetypeName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
                //担保人
                else if (applicantInfo.APPLICANTROLE == "G")
                {
                    strRole += "G";
                    identification_code = applicantInfo.IDENTIFICATION_CODE;
                    #region 基本信息 节点applicantInfo赋值
                    try
                    {
                        bizObject["Dbactualsalary"] = applicantInfo.actualsalary;
                        //   bizObject["DbageInMonth"] = applicantInfo.ageInMonth;
                        //bizObject["DbageInYear"] = applicantInfo.ageInYear;
                        //bizObject["DbageInYYMM"] = applicantInfo.ageInYYMM;
                        bizObject["Dbblacklistind"] = applicantInfo.blacklistind;
                        bizObject["Dbblacklistnorecordind"] = applicantInfo.blacklistnorecordind == "T" ? "是" : "否";
                        bizObject["Dbchildrenflag"] = applicantInfo.childrenflag == "Y" ? "是" : "否";
                        bizObject["DbcitizenshipName"] = applicantInfo.citizenshipName;
                        bizObject["Dbconsent"] = applicantInfo.consent == "Y" ? "是" : "否";
                        //    bizObject["Dbcustomerid"] = applicantInfo.customerid;
                        bizObject["DbThaiFirstName"] = applicantInfo.ThaiFirstName;
                        bizObject["Dbdateofbirth"] = applicantInfo.dateofbirth.Split(' ')[0];
                        bizObject["DbdesginationName"] = applicantInfo.desginationName;
                        bizObject["DbdrivinglicensestatusName"] = applicantInfo.drivinglicensestatusName;
                        bizObject["DbeducationName"] = applicantInfo.educationName;
                        bizObject["Dbemailaddress"] = applicantInfo.emailaddress;
                        bizObject["DbEmployerType"] = applicantInfo.employeetypeName;
                        bizObject["Dbengfirstname"] = applicantInfo.engfirstname;
                        bizObject["Dbenglastname"] = applicantInfo.englastname;
                        //  bizObject["Dbengmiddlename"] = applicantInfo.engmiddlename;
                        //    bizObject["Dbfamilyavgincome"] = applicantInfo.familyavgincome;
                        bizObject["Dbformername"] = applicantInfo.formername;
                        bizObject["DbgenderName"] = applicantInfo.genderName;
                        bizObject["Dbhouseownername"] = applicantInfo.houseownername;
                        bizObject["DbhokouName"] = applicantInfo.hukouName;
                        bizObject["Dbidcardexpirydate"] = applicantInfo.idcardexpirydate;
                        bizObject["Dbidcardissuedate"] = applicantInfo.idcardissuedate;
                        bizObject["DbIdCardNo"] = applicantInfo.idcardnumber; //111
                        bizObject["DbIdCarTypeName"] = applicantInfo.idcardtypeName;//222
                        bizObject["DbIndustrySubTypeName"] = applicantInfo.IndustrySubTypeName;
                        bizObject["DbindustrytypeName"] = applicantInfo.industrytypeName;
                        bizObject["DbissuingAuthority"] = applicantInfo.issuingAuthority;
                        bizObject["DbjobgroupName"] = applicantInfo.jobgroupName;
                        bizObject["DbLicenseNo"] = applicantInfo.licenseNumber;
                        bizObject["Dblienee"] = applicantInfo.lienee;
                        bizObject["Dbmaritalstatusname"] = applicantInfo.maritalstatusname;
                        bizObject["DbNationCode"] = applicantInfo.nationName;
                        bizObject["Dbnooffamilymembers"] = applicantInfo.nooffamilymembers;
                        bizObject["Dbnumberofdependents"] = applicantInfo.numberofdependents;
                        bizObject["DboccupationName"] = applicantInfo.occupationName;
                        bizObject["DbrelationShipName"] = applicantInfo.relationShipName;
                        bizObject["DbsalaryrangeName"] = applicantInfo.salaryrangeName;
                        //  bizObject["Dbsalayslipind"] = applicantInfo.salayslipind;
                        bizObject["Dbstaffind"] = applicantInfo.staffind;
                        bizObject["DbsubOccupationName"] = applicantInfo.subOccupationName;
                        bizObject["Dbthaititlename"] = applicantInfo.thaititlename;
                        bizObject["Dbtitlename"] = applicantInfo.titlename;
                        bizObject["Dbactualsalary"] = applicantInfo.actualsalary;
                        bizObject["Dbvipind"] = applicantInfo.vipind == "T" ? "是" : "否";
                        bizObject["DbEmploymentTypeName"] = applicantInfo.employeetypeName;
                    }
                    catch { }
                    #endregion

                    #region 工作信息 节点employment赋值
                    try
                    {
                        XmlNodeList employmentlst = xd.SelectSingleNode("//employment").ChildNodes;
                        foreach (XmlNode employmentnode in employmentlst)
                        {
                            if (employmentnode.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                employment employment = XmlUtil.Deserialize(typeof(employment), "<employment>" + employmentnode.InnerXml + "</employment>") as employment;

                                bizObject["DbbusinesstypeName"] = employment.businesstypeName;
                                bizObject["DbcompanyCityName"] = employment.cityName;
                                bizObject["DbcompanyProvinceName"] = employment.provinceName;
                                bizObject["DbComments"] = employment.comments;
                                bizObject["DbcompanyAddress"] = employment.companyAddress;
                                bizObject["DbcompanyName"] = employment.companyName;
                                bizObject["Dbemployertype"] = employment.employertype;
                                bizObject["DbFax"] = employment.Fax;
                                bizObject["Dbjobdescription"] = employment.jobdescription;
                                // bizObject["DblineId"] = employment.lineId;
                                bizObject["Dbnoofemployees"] = employment.noofemployees;
                                bizObject["Dbphonenumber"] = employment.phonenumber;
                                bizObject["DbpositionName"] = employment.positionName;
                                bizObject["DbcompanypostCode"] = employment.postCode;
                                bizObject["DbcompanyProvinceName"] = employment.provinceName;
                                bizObject["Dbtimeinyear"] = employment.timeinyear;
                                bizObject["Dbtimeinmonth"] = employment.timeinmonth;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region 联系信息 节点addressList，phoneList赋值
                    try
                    {
                        XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                        foreach (XmlNode address in addresslst)
                        {
                            if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                bizObject["DbAddressId"] = addressList.AddressId;
                                bizObject["DbaddressstatusName"] = addressList.addressstatusName;
                                bizObject["DbaddresstypeName"] = addressList.addresstypeName;
                                bizObject["Dbbirthpalaceprovince"] = addressList.birthpalaceprovince;
                                bizObject["DbhukoucityName"] = addressList.cityName;
                                bizObject["DbcountryName"] = addressList.countryName;
                                bizObject["DbcurrentLivingAddress"] = addressList.currentLivingAddress;
                                bizObject["Dbdefaultmailingaddress"] = addressList.defaultmailingaddress;
                                bizObject["Dbhomevalue"] = addressList.homevalue;
                                bizObject["Dbhukouaddress"] = addressList.hukouaddress;
                                bizObject["Dblivingsince"] = addressList.livingsince;
                                bizObject["Dbnativedistrict"] = addressList.nativedistrict;
                                bizObject["Dbpostcode"] = addressList.postcode;
                                bizObject["DbpropertytypeName"] = addressList.propertytypeName;
                                bizObject["DbhukouprovinceName"] = addressList.provinceName;
                                bizObject["DbresidencetypeName"] = addressList.residencetypeName;
                                bizObject["Dbstayinmonth"] = addressList.stayinmonth;
                                bizObject["Dbstayinyear"] = addressList.stayinyear;
                                break;
                            }
                        }


                        XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                        foreach (XmlNode phone in phonelst)
                        {
                            if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                bizObject["DbareaCode"] = phoneList.areaCode;
                                bizObject["DbcountrycodeName"] = phoneList.countrycodeName;
                                bizObject["Dbextension"] = phoneList.extension;
                                bizObject["DbphoneNo"] = phoneList.phoneNo;
                                bizObject["DbphonetypeName"] = phoneList.phonetypeName;
                                break;
                            }

                        }
                    }
                    catch { }

                    #endregion
                }
            }
            #region 标记共借人为公司
            try
            {
                XmlNodeList companyInfolst = xd.SelectSingleNode("//companyInfo").ChildNodes;
                foreach (XmlNode child in companyInfolst)
                {
                    companyInfo companyInfo = XmlUtil.Deserialize(typeof(companyInfo), "<companyInfo>" + child.InnerXml + "</companyInfo>") as companyInfo;
                    //主借机构
                    if (companyInfo.COMPANYROLE == "CB")
                    {
                        #region 基本信息 节点companyInfo赋值
                        try
                        {
                            //共借人为公司
                            bizObject["applicant_type"] = "company";
                            break;
                        }
                        catch { }
                        #endregion
                    }
                }
            }
            catch { }

            #endregion
            #region 清除担保人资料
            if (!strRole.Contains("G") && bizObject["DbThaiFirstName"] != null && bizObject["DbThaiFirstName"].ToString().Trim() != "")
            {
                //清除担保人资料
                try
                {
                    bizObject["Dbactualsalary"] = null;
                    //   bizObject["DbageInMonth"] =null;
                    //bizObject["DbageInYear"] =null;
                    //bizObject["DbageInYYMM"] =null;
                    bizObject["Dbblacklistind"] = null;
                    bizObject["Dbblacklistnorecordind"] = null;
                    bizObject["Dbchildrenflag"] = null;
                    bizObject["DbcitizenshipName"] = null;
                    bizObject["Dbconsent"] = null;
                    //    bizObject["Dbcustomerid"] =null;
                    bizObject["DbThaiFirstName"] = null;
                    bizObject["Dbdateofbirth"] = null;
                    bizObject["DbdesginationName"] = null;
                    bizObject["DbdrivinglicensestatusName"] = null;
                    bizObject["DbeducationName"] = null;
                    bizObject["Dbemailaddress"] = null;
                    bizObject["DbEmployerType"] = null;
                    bizObject["Dbengfirstname"] = null;
                    bizObject["Dbenglastname"] = null;
                    //  bizObject["Dbengmiddlename"] =null;
                    //    bizObject["Dbfamilyavgincome"] =null;
                    bizObject["Dbformername"] = null;
                    bizObject["DbgenderName"] = null;
                    bizObject["Dbhouseownername"] = null;
                    bizObject["DbhokouName"] = null;
                    bizObject["Dbidcardexpirydate"] = null;
                    bizObject["Dbidcardissuedate"] = null;
                    bizObject["DbIdCardNo"] = null;
                    bizObject["DbIdCarTypeName"] = null;
                    bizObject["DbIndustrySubTypeName"] = null;
                    bizObject["DbindustrytypeName"] = null;
                    bizObject["DbissuingAuthority"] = null;
                    bizObject["DbjobgroupName"] = null;
                    bizObject["DbLicenseNo"] = null;
                    bizObject["Dblienee"] = null;
                    bizObject["Dbmaritalstatusname"] = null;
                    bizObject["DbNationCode"] = null;
                    bizObject["Dbnooffamilymembers"] = null;
                    bizObject["Dbnumberofdependents"] = null;
                    bizObject["DboccupationName"] = null;
                    bizObject["DbrelationShipName"] = null;
                    bizObject["DbsalaryrangeName"] = null;
                    //  bizObject["Dbsalayslipind"] =null;
                    bizObject["Dbstaffind"] = null;
                    bizObject["DbsubOccupationName"] = null;
                    bizObject["Dbthaititlename"] = null;
                    bizObject["Dbtitlename"] = null;
                    bizObject["Dbactualsalary"] = null;
                    bizObject["Dbvipind"] = null;
                    bizObject["DbEmploymentTypeName"] = null;


                    bizObject["DbbusinesstypeName"] = null;
                    bizObject["DbcompanyCityName"] = null;
                    bizObject["DbcompanyProvinceName"] = null;
                    bizObject["DbComments"] = null;
                    bizObject["DbcompanyAddress"] = null;
                    bizObject["DbcompanyName"] = null;
                    bizObject["Dbemployertype"] = null;
                    bizObject["DbFax"] = null;
                    bizObject["Dbjobdescription"] = null;
                    // bizObject["DblineId"] =null;  
                    bizObject["Dbnoofemployees"] = null;
                    bizObject["Dbphonenumber"] = null;
                    bizObject["DbpositionName"] = null;
                    bizObject["DbcompanypostCode"] = null;
                    bizObject["DbcompanyProvinceName"] = null;
                    bizObject["Dbtimeinyear"] = null;
                    bizObject["Dbtimeinmonth"] = null;


                    bizObject["DbAddressId"] = null;
                    bizObject["DbaddressstatusName"] = null;
                    bizObject["DbaddresstypeName"] = null;
                    bizObject["Dbbirthpalaceprovince"] = null;
                    bizObject["DbhukoucityName"] = null;
                    bizObject["DbcountryName"] = null;
                    bizObject["DbcurrentLivingAddress"] = null;
                    bizObject["Dbdefaultmailingaddress"] = null;
                    bizObject["Dbhomevalue"] = null;
                    bizObject["Dbhukouaddress"] = null;
                    bizObject["Dblivingsince"] = null;
                    bizObject["Dbnativedistrict"] = null;
                    bizObject["Dbpostcode"] = null;
                    bizObject["DbpropertytypeName"] = null;
                    bizObject["DbhukouprovinceName"] = null;
                    bizObject["DbresidencetypeName"] = null;
                    bizObject["Dbstayinmonth"] = null;
                    bizObject["Dbstayinyear"] = null;


                    bizObject["DbareaCode"] = null;
                    bizObject["DbcountrycodeName"] = null;
                    bizObject["Dbextension"] = null;
                    bizObject["DbphoneNo"] = null;
                    bizObject["DbphonetypeName"] = null;
                }
                catch { }
            }
            #endregion
            #region 清除共借人资料
            if (!strRole.Contains("C") && bizObject["GjThaiFirstName"] != null && bizObject["GjThaiFirstName"].ToString().Trim() != "")
            {
                //清除共借人资料
                try
                {
                    bizObject["Gjactualsalary"] = null;
                    //  bizObject["GjageInMonth"] =null;
                    //  bizObject["GjageInYear"] =null;
                    // bizObject["GjageInYYMM"] =null;
                    bizObject["Gjblacklistind"] = null;
                    bizObject["Gjblacklistnorecordind"] = null;
                    bizObject["Gjchildrenflag"] = null;
                    bizObject["GjcitizenshipName"] = null;
                    bizObject["Gjconsent"] = null;
                    //bizObject["Gjcustomerid"] =null;
                    bizObject["GjThaiFirstName"] = null;
                    bizObject["Gjdateofbirth"] = null;
                    bizObject["GjdesginationName"] = null;
                    bizObject["GjdrivinglicensestatusName"] = null;
                    bizObject["GjeducationName"] = null;
                    bizObject["Gjemailaddress"] = null;
                    bizObject["GjEmployerType"] = null;
                    bizObject["Gjengfirstname"] = null;
                    bizObject["Gjenglastname"] = null;

                    //bizObject["Gjengmiddlename"] =null;
                    // bizObject["Gjfamilyavgincome"] =null;
                    bizObject["Gjformername"] = null;
                    bizObject["GjgenderName"] = null;
                    bizObject["Gjhouseownername"] = null;
                    bizObject["GjhokouName"] = null;
                    bizObject["Gjidcardexpirydate"] = null;
                    bizObject["Gjidcardissuedate"] = null;
                    bizObject["GjIdCardNo"] = null;
                    bizObject["GjIdCarTypeName"] = null;
                    bizObject["GjIndustrySubTypeName"] = null;
                    bizObject["GjindustrytypeName"] = null;
                    bizObject["GjissuingAuthority"] = null;
                    bizObject["GjjobgroupName"] = null;
                    bizObject["GjLicenseNo"] = null;
                    bizObject["Gjlienee"] = null;
                    bizObject["Gjmaritalstatusname"] = null;
                    bizObject["GjnationName"] = null;
                    bizObject["Gjnooffamilymembers"] = null;
                    bizObject["Gjnumberofdependents"] = null;
                    bizObject["GjoccupationName"] = null;
                    bizObject["GjrelationShipName"] = null;
                    bizObject["GjsalaryrangeName"] = null;
                    //  bizObject["Gjsalayslipind"] =null;
                    bizObject["Gjstaffind"] = null;
                    bizObject["GjsubOccupationName"] = null;
                    bizObject["Gjthaititlename"] = null;
                    bizObject["Gjtitlename"] = null;
                    bizObject["Gjspouse"] = null;
                    bizObject["Gjvipind"] = null;
                    bizObject["GjEmploymentTypeName"] = null;



                    bizObject["IS_INACTIVE_IND"] = null;


                    bizObject["GjbusinesstypeName"] = null;
                    bizObject["GjCityName"] = null;
                    bizObject["GjcompanyComments"] = null;
                    bizObject["GjcompanyAddress"] = null;
                    bizObject["GjcompanyName"] = null;
                    bizObject["Gjemployertype"] = null;
                    bizObject["GjFax"] = null;
                    bizObject["Gjjobdescription"] = null;
                    //bizObject["GjlineId"] =null;
                    bizObject["Gjnoofemployees"] = null;
                    bizObject["Gjphonenumber"] = null;
                    bizObject["GjpositionName"] = null;
                    bizObject["GjcompanypostCode"] = null;
                    bizObject["GjProvinceName"] = null;
                    bizObject["Gjtimeinyear"] = null;
                    bizObject["Gjtimeinmonth"] = null;


                    bizObject["GjAddressId"] = null;
                    bizObject["GjaddressstatusName"] = null;
                    bizObject["GjaddresstypeName"] = null;
                    bizObject["Gjbirthpalaceprovince"] = null;
                    bizObject["GjhukoucityName"] = null;
                    bizObject["GjcountryName"] = null;
                    bizObject["GjcurrentLivingAddress"] = null;
                    bizObject["Gjdefaultmailingaddress"] = null;
                    bizObject["Gjhomevalue"] = null;
                    bizObject["Gjhukouaddress"] = null;
                    bizObject["Gjlivingsince"] = null;
                    bizObject["Gjnativedistrict"] = null;
                    bizObject["Gjpostcode"] = null;
                    bizObject["GjpropertytypeName"] = null;
                    bizObject["GjhukouprovinceName"] = null;
                    bizObject["GjresidencetypeName"] = null;
                    bizObject["Gjstayinmonth"] = null;
                    bizObject["Gjstayinyear"] = null;

                    bizObject["GjareaCode"] = null;
                    bizObject["GjcountrycodeName"] = null;
                    bizObject["Gjextension"] = null;
                    bizObject["GjphoneNo"] = null;
                    bizObject["GjphonetypeName"] = null;
                }
                catch { }
            }
            #endregion
            #endregion
            return bizObject;
        }
        /// <summary>
        /// 机构贷款申请
        /// </summary>
        /// <param name="bizObject"></param>
        /// <param name="xd"></param>
        /// <param name="workFlowCode"></param>
        /// <param name="userObjectid"></param>
        /// <returns></returns>
        public BizObject setCompanyBizObject(BizObject bizObject, XmlDocument xd, string workFlowCode, string userObjectid, string subTable)
        {
            XmlNode xn;
            CAPInfo cap = new CAPInfo();
            #region 数据赋值
            #region 节点appAsset赋值
            xn = xd.SelectSingleNode("//appAsset//data");
            appAsset appAsset = XmlUtil.Deserialize(typeof(appAsset), "<appAsset>" + xn.InnerXml + "</appAsset>") as appAsset;
            System.Reflection.PropertyInfo[] properties = appAsset.GetType().GetProperties();
            bizObject["applicationNo"] = appAsset.applicationNo;
            bizObject["assetConditionName"] = appAsset.assetConditionName;
            bizObject["assetMakeName"] = appAsset.assetMakeName;
            bizObject["assetPrice"] = appAsset.assetPrice;
            bizObject["brandName"] = appAsset.brandName;
            bizObject["vecomments"] = appAsset.comments;
            bizObject["cylinder"] = appAsset.cylinder;
            bizObject["deliveryDate"] = appAsset.deliveryDate;
            bizObject["engineCC"] = appAsset.engineCC;
            bizObject["engineNo"] = appAsset.engineNo;
            bizObject["guranteeOption"] = appAsset.guranteeOption == "00001" ? "是" : "否";
            bizObject["manufactureYear"] = appAsset.manufactureYear;
            bizObject["miocn"] = appAsset.miocn;
            bizObject["modelName"] = appAsset.modelName;
            bizObject["odometer"] = appAsset.odometer;
            bizObject["purposeName"] = appAsset.purposeName;
            bizObject["registrationNo"] = appAsset.registrationNo;
            bizObject["releaseDate"] = appAsset.releaseDate;
            bizObject["releaseMonth"] = appAsset.releaseMonth;
            bizObject["releaseYear"] = appAsset.releaseYear;
            bizObject["series"] = appAsset.series;
            bizObject["style"] = appAsset.style;
            bizObject["transmissionName"] = appAsset.transmissionName;
            bizObject["vehicleAge"] = appAsset.vehicleAge;
            bizObject["vehicleBody"] = appAsset.vehicleBody;
            bizObject["vehiclecolorName"] = appAsset.vehiclecolorName;
            bizObject["vehicleSubtypeName"] = appAsset.vehicleSubtypeName;
            bizObject["vineditind"] = appAsset.vineditind == "T" ? "是" : "否";
            bizObject["vinNo"] = appAsset.vinNo;
            bizObject["wheelWidth"] = appAsset.wheelWidth;
            #endregion
            #region 节点appFinanTerms赋值
            try
            {
                xn = xd.SelectSingleNode("//appFinanTerms//data");
                appFinanTerms appFinanTerms = XmlUtil.Deserialize(typeof(appFinanTerms), "<appFinanTerms>" + xn.InnerXml + "</appFinanTerms>") as appFinanTerms;

                //bizObject["accessoriesInd"] = appFinanTerms.accessoriesInd.Contains("T") ? "是" : "否";
                bizObject["actualRate"] = appFinanTerms.actualRate;
                bizObject["applicationNo"] = appFinanTerms.applicationNo;
                bizObject["balloonRate"] = appFinanTerms.balloonRate;
                bizObject["deferredTerm"] = appFinanTerms.deferredTerm;
                bizObject["downpaymentamount"] = appFinanTerms.downpaymentamount;
                bizObject["downpaymentrate"] = appFinanTerms.downpaymentrate;
                // bizObject["Equals"] = appFinanTerms.Equals;
                bizObject["financedamount"] = appFinanTerms.financedamount;
                bizObject["financedamountrate"] = appFinanTerms.financedamountrate;
                bizObject["interestRate"] = appFinanTerms.interestRate;
                bizObject["otherfees"] = appFinanTerms.otherfees;
                bizObject["paymentFrequencyName"] = appFinanTerms.paymentFrequencyName;
                bizObject["productGroupName"] = appFinanTerms.productGroupName;
                bizObject["productTypeName"] = appFinanTerms.productTypeName;
                bizObject["rentalmodeName"] = appFinanTerms.rentalmodeName;
                bizObject["salesprice"] = appFinanTerms.salesprice;
                bizObject["subsidyAmount"] = appFinanTerms.subsidyAmount;
                bizObject["subsidyRate"] = appFinanTerms.subsidyRate;
                bizObject["termMonth"] = appFinanTerms.termMonth;
                bizObject["totalintamount"] = appFinanTerms.totalintamount;
                bizObject["vehicleprice"] = appFinanTerms.vehicleprice;
                bizObject["totalaccessoryamount"] = appFinanTerms.totalaccessoryamount;

            }
            catch { }
            #endregion
            #region 节点appRental还款计划赋值
            try
            {
                XmlNodeList appRentallst = xd.SelectSingleNode("//appRental").ChildNodes;
                foreach (XmlNode appRentalnode in appRentallst)
                {
                    appRental appRental = XmlUtil.Deserialize(typeof(appRental), "<appRental>" + appRentalnode.InnerXml + "</appRental>") as appRental;
                    cap.appRental.Add(appRental);
                }
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workFlowCode);
                // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）  "CompanyDetailtable"
                BizObjectSchema childSchema = schema.GetProperty(subTable).ChildSchema; // 获取子业务对象编码
                var lstChildBO = new List<BizObject>();
                /*
                这里类似以上BizObject的方法给 childBO 赋值
                例如：childBO["短文本数据项"] = "值";
                */
                foreach (var child in cap.appRental)
                {
                    BizObject childBO = new BizObject(this.Engine, childSchema, userObjectid);
                    childBO["startTerm"] = child.startTerm;
                    childBO["endTerm"] = child.endTerm;
                    childBO["rentalAmount"] = child.rentalAmount;
                    childBO["grossRentalAmount"] = child.grossRentalAmount;
                    bizObject["Bccdygje"] = child.grossRentalAmount;
                    lstChildBO.Add(childBO);
                }
                //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                bizObject[subTable] = lstChildBO.ToArray();
            }
            catch { }
            #endregion
            #region 节点assetAccessories附加费
            try
            {
                XmlNodeList assetAccessorieslst = xd.SelectSingleNode("//assetAccessories").ChildNodes;
                foreach (XmlNode assetAccessoriesnode in assetAccessorieslst)
                {
                    assetAccessories assetAccessories = XmlUtil.Deserialize(typeof(assetAccessories), "<assetAccessories>" + assetAccessoriesnode.InnerXml + "</assetAccessories>") as assetAccessories;
                    cap.assetAccessories.Add(assetAccessories);
                }
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workFlowCode);
                // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）

                BizObjectSchema childSchema = schema.GetProperty("CompanyAssetAccessoriesTable").ChildSchema; // 获取子业务对象编码
                var lstChildBO = new List<BizObject>();
                /*
                这里类似以上BizObject的方法给 childBO 赋值
                例如：childBO["短文本数据项"] = "值";
                */
                foreach (var child in cap.assetAccessories)
                {
                    BizObject childBO = new BizObject(this.Engine, childSchema, userObjectid);
                    childBO["additionalCode"] = child.additionalCode;
                    childBO["additionalName1"] = child.additionalName;
                    childBO["additionalprice"] = child.price;
                    childBO["mainType"] = child.mainType;
                    lstChildBO.Add(childBO);
                }
                //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                bizObject["CompanyAssetAccessoriesTable"] = lstChildBO.ToArray();

            }

            catch { }
            #endregion
            #endregion
            #region 节点appInfo赋值
            try
            {
                xn = xd.SelectSingleNode("//appInfo//data");
                appInfo appInfo = XmlUtil.Deserialize(typeof(appInfo), "<appInfo>" + xn.InnerXml + "</appInfo>") as appInfo;

                bizObject["APPCREATETIME"] = appInfo.APPCREATETIME;
                bizObject["applicationNo"] = appInfo.applicationNo;
                //bizObject["appStatus"] = appInfo.appStatus;
                bizObject["appTypeName"] = appInfo.appTypeName;
                //bizObject["comments"] = appInfo.comments;
                bizObject["companyName"] = appInfo.companyName;
                bizObject["ContactNumber"] = appInfo.ContactNumber;
                bizObject["dealercityName"] = appInfo.dealercityName;
                bizObject["dealerName"] = appInfo.dealerName;
                bizObject["distributorName"] = appInfo.distributorName;
                //bizObject["financialadvisorId"] = appInfo.financialadvisorId;
                bizObject["financialadvisorName"] = appInfo.financialadvisorName;
                bizObject["provinceName"] = appInfo.provinceName;
                bizObject["refCANumber"] = appInfo.refCANumber;
                bizObject["salesPersonName"] = appInfo.salesPersonName;
                bizObject["showRoomName"] = appInfo.showRoomName;
                bizObject["userName"] = appInfo.userName;
                bizObject["vehicleTypeName"] = appInfo.vehicleTypeName;
            }
            catch { }
            #endregion
            #region 机构代 companyInfo赋值
            XmlNodeList companyInfolst = xd.SelectSingleNode("//companyInfo").ChildNodes;
            string identification_code = "";
            foreach (XmlNode child in companyInfolst)
            {
                companyInfo companyInfo = XmlUtil.Deserialize(typeof(companyInfo), "<companyInfo>" + child.InnerXml + "</companyInfo>") as companyInfo;
                //主借机构
                if (companyInfo.COMPANYROLE == "B")
                {
                    identification_code = companyInfo.IDENTIFICATION_CODE;
                    #region 基本信息 节点companyInfo赋值
                    try
                    {
                        bizObject["businesstypeName"] = companyInfo.businesstypeName;
                        bizObject["applicationNo"] = companyInfo.applicationNo;
                        bizObject["capitalReamount"] = companyInfo.capitalReamount;
                        bizObject["dealcomments"] = companyInfo.comments;
                        bizObject["companyNamech"] = companyInfo.companyName;
                        //  bizObject["COMPANYROLE"] = companyInfo.COMPANYROLE;
                        bizObject["compaynameeng"] = companyInfo.compaynameeng;
                        bizObject["emailAddress"] = companyInfo.emailAddress;
                        bizObject["establishedin"] = companyInfo.establishedin;

                        bizObject["BlacklistInd"] = companyInfo.externalCreditRecord == "T" ? "是" : "否";
                        bizObject["industrytypeName"] = companyInfo.industrytypeName;
                        bizObject["lienee"] = companyInfo.lienee == "T" ? "是" : "否";
                        bizObject["loanCard"] = companyInfo.loanCard;
                        bizObject["loancardPassword"] = companyInfo.loancardPassword;
                        bizObject["networthamt"] = companyInfo.networthamt;
                        //bizObject["noRecord"] = companyInfo.noRecord;
                        bizObject["organizationCode"] = companyInfo.organizationCode;
                        bizObject["parentCompany"] = companyInfo.parentCompany;
                        bizObject["registrationNo"] = companyInfo.registrationNo;
                        bizObject["representativeDesignation"] = companyInfo.representativeDesignation;
                        bizObject["representativeidNo"] = companyInfo.representativeidNo;
                        bizObject["representativeName"] = companyInfo.representativeName;
                        // bizObject["staffNumber"] = companyInfo.staffNumber;
                        bizObject["subsidaryCompany"] = companyInfo.subsidaryCompany;
                        bizObject["taxid"] = companyInfo.taxid;
                        bizObject["trustName"] = companyInfo.trustName;
                        bizObject["years"] = companyInfo.years;
                    }
                    catch { }
                    #endregion

                    #region 地址 节点addressList，phoneList赋值
                    try
                    {
                        XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                        foreach (XmlNode address in addresslst)
                        {
                            if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                bizObject["AddressId"] = addressList.registeredAddress;
                                bizObject["addressstatusName"] = addressList.addressstatusName;
                                bizObject["addresstypeName"] = addressList.addresstypeName;
                                bizObject["birthpalaceprovince"] = addressList.birthpalaceprovince;
                                bizObject["hukoucityName"] = addressList.cityName;
                                bizObject["countryName"] = addressList.countryName;
                                bizObject["currentLivingAddress"] = addressList.currentLivingAddress;
                                bizObject["defaultmailingaddress"] = addressList.defaultmailingaddress == "T" ? "是" : "否";
                                bizObject["homevalue"] = addressList.homevalue;
                                bizObject["hukouaddress"] = addressList.hukouaddress == "Y" ? "是" : "否";
                                bizObject["livingsince"] = addressList.livingsince;
                                bizObject["nativedistrict"] = addressList.nativedistrict;
                                bizObject["postcode"] = addressList.postcode;
                                bizObject["propertytypeName"] = addressList.propertytypeName;
                                bizObject["hukouprovinceName"] = addressList.provinceName;
                                bizObject["residencetypeName"] = addressList.residencetypeName;
                                //-少建 bizObject["stayinmonth"] = addressList.stayinmonth;
                                bizObject["stayinyear"] = addressList.stayinyear;
                                break;
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                        foreach (XmlNode phone in phonelst)
                        {
                            if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                //  bizObject["areaCode"] = phoneList.areaCode;
                                bizObject["countrycodeName"] = phoneList.countrycodeName;
                                bizObject["extension"] = phoneList.extension;
                                bizObject["phoneNo"] = phoneList.phoneNo;
                                bizObject["phonetypeName"] = phoneList.phonetypeName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion

                    #region 联系人 referenceList赋值
                    try
                    {
                        XmlNodeList referencelst = xd.SelectSingleNode("//referenceList").ChildNodes;
                        foreach (XmlNode reference in referencelst)
                        {
                            if (reference.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                referenceList referenceList = XmlUtil.Deserialize(typeof(referenceList), "<referenceList>" + reference.InnerXml + "</referenceList>") as referenceList;

                                bizObject["name1"] = referenceList.name;
                                bizObject["address"] = referenceList.address;
                                bizObject["lxcityName"] = referenceList.cityName;
                                bizObject["lxhokouName"] = referenceList.hokouName;
                                bizObject["lxmobile"] = referenceList.mobile;
                                bizObject["lxphoneNo"] = referenceList.phoneNo;

                                bizObject["lxpostCode"] = referenceList.postCode;
                                bizObject["lxprovinceCode"] = referenceList.provinceName;
                                bizObject["relationshipName"] = referenceList.relationshipName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
                //担保机构
                if (companyInfo.COMPANYROLE == "G")
                {
                    identification_code = companyInfo.IDENTIFICATION_CODE;
                    #region 基本信息 节点companyInfo赋值
                    try
                    {
                        bizObject["DbCbusinesstypeName"] = companyInfo.businesstypeName;
                        bizObject["DbCcapitalReamount"] = companyInfo.capitalReamount;
                        bizObject["DbCcompanyNamech"] = companyInfo.companyName;
                        bizObject["DbCcompaynameeng"] = companyInfo.compaynameeng;
                        bizObject["DbCemailAddress"] = companyInfo.emailAddress;
                        bizObject["DbCestablishedin"] = companyInfo.establishedin;
                        //bizObject["DbCBlacklistInd"] = companyInfo.externalCreditRecord == "T" ? "是" : "否";
                        bizObject["DbCindustrytypeName"] = companyInfo.industrytypeName;
                        bizObject["DbClienee"] = companyInfo.lienee == "T" ? "是" : "否";
                        bizObject["DbCloanCard"] = companyInfo.loanCard;
                        bizObject["DbCloancardPassword"] = companyInfo.loancardPassword;
                        bizObject["DbCnetworthamt"] = companyInfo.networthamt;
                        bizObject["DbCorganizationCode"] = companyInfo.organizationCode;
                        bizObject["DbCparentCompany"] = companyInfo.parentCompany;
                        bizObject["DbCregistrationNo"] = companyInfo.registrationNo;
                        bizObject["DbCrepresentativeDesignation"] = companyInfo.representativeDesignation;
                        bizObject["DbCrepresentativeidNo"] = companyInfo.representativeidNo;
                        bizObject["DbCrepresentativeName"] = companyInfo.representativeName;
                        bizObject["DbCsubsidaryCompany"] = companyInfo.subsidaryCompany;
                        bizObject["DbCtaxid"] = companyInfo.taxid;
                        bizObject["DbCtrustName"] = companyInfo.trustName;
                        bizObject["DbCyears"] = companyInfo.years;
                    }
                    catch { }
                    #endregion

                    #region 地址 节点addressList，phoneList赋值
                    try
                    {
                        XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                        foreach (XmlNode address in addresslst)
                        {
                            if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                bizObject["DbCAddressId"] = addressList.registeredAddress;
                                bizObject["DbCaddressstatusName"] = addressList.addressstatusName;
                                bizObject["DbCaddresstypeName"] = addressList.addresstypeName;
                                //bizObject["DbCbirthpalaceprovince"] = addressList.birthpalaceprovince;
                                //bizObject["DbChukoucityName"] = addressList.cityName;
                                bizObject["DbCcountryName"] = addressList.countryName;
                                bizObject["DbCcurrentLivingAddress"] = addressList.currentLivingAddress;
                                bizObject["DbCdefaultmailingaddress"] = addressList.defaultmailingaddress == "T" ? "是" : "否";
                                bizObject["DbChomevalue"] = addressList.homevalue;
                                bizObject["DbChukouaddress"] = addressList.hukouaddress == "Y" ? "是" : "否";
                                bizObject["DbClivingsince"] = addressList.livingsince;
                                //bizObject["DbCnativedistrict"] = addressList.nativedistrict;
                                bizObject["DbCpostcode"] = addressList.postcode;
                                bizObject["DbCpropertytypeName"] = addressList.propertytypeName;
                                //bizObject["DbChukouprovinceName"] = addressList.provinceName;
                                bizObject["DbCresidencetypeName"] = addressList.residencetypeName;
                                bizObject["DbCstayinyear"] = addressList.stayinyear;
                                break;
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                        foreach (XmlNode phone in phonelst)
                        {
                            if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                            {
                                phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                //  bizObject["areaCode"] = phoneList.areaCode;
                                bizObject["DbCcountrycodeName"] = phoneList.countrycodeName;
                                bizObject["DbCextension"] = phoneList.extension;
                                bizObject["DbCphoneNo"] = phoneList.phoneNo;
                                bizObject["DbCphonetypeName"] = phoneList.phonetypeName;
                                break;
                            }
                        }
                    }
                    catch { }
                    #endregion
                }
            }
            #endregion
            #region 担保人 applicantInfo赋值
            try
            {
                XmlNodeList xnlst = xd.SelectSingleNode("//applicantInfo").ChildNodes;
                foreach (XmlNode child in xnlst)
                {
                    applicantInfo applicantInfo = XmlUtil.Deserialize(typeof(applicantInfo), "<applicantInfo>" + child.InnerXml + "</applicantInfo>") as applicantInfo;

                    if (applicantInfo.APPLICANTROLE == "G")
                    {
                        identification_code = applicantInfo.IDENTIFICATION_CODE;
                        #region 基本信息 节点applicantInfo赋值
                        try
                        {
                            bizObject["Dbactualsalary"] = applicantInfo.actualsalary;
                            //bizObject["DbageInMonth"] = applicantInfo.ageInMonth;
                            //bizObject["DbageInYear"] = applicantInfo.ageInYear;
                            //bizObject["DbageInYYMM"] = applicantInfo.ageInYYMM;
                            bizObject["Dbblacklistind"] = applicantInfo.blacklistind == "T" ? "是" : "否";
                            bizObject["Dbblacklistnorecordind"] = applicantInfo.blacklistnorecordind;
                            bizObject["Dbchildrenflag"] = applicantInfo.childrenflag;
                            bizObject["DbcitizenshipName"] = applicantInfo.citizenshipName;
                            bizObject["Dbconsent"] = applicantInfo.consent;
                            //    bizObject["Dbcustomerid"] = applicantInfo.customerid;
                            bizObject["DbThaiFirstName"] = applicantInfo.ThaiFirstName;
                            bizObject["Dbdateofbirth"] = applicantInfo.dateofbirth.Split(' ')[0];
                            bizObject["DbdesginationName"] = applicantInfo.desginationName;
                            bizObject["DbdrivinglicensestatusName"] = applicantInfo.drivinglicensestatusName;
                            bizObject["DbeducationName"] = applicantInfo.educationName;
                            bizObject["Dbemailaddress"] = applicantInfo.emailaddress;
                            bizObject["DbEmployerType"] = applicantInfo.employeetypeName;
                            bizObject["Dbengfirstname"] = applicantInfo.engfirstname;
                            bizObject["Dbenglastname"] = applicantInfo.englastname;
                            //  bizObject["Dbengmiddlename"] = applicantInfo.engmiddlename;
                            //    bizObject["Dbfamilyavgincome"] = applicantInfo.familyavgincome;
                            bizObject["Dbformername"] = applicantInfo.formername;
                            bizObject["DbgenderName"] = applicantInfo.genderName;
                            bizObject["Dbhouseownername"] = applicantInfo.houseownername;
                            bizObject["DbhokouName"] = applicantInfo.hukouName;
                            bizObject["Dbidcardexpirydate"] = applicantInfo.idcardexpirydate;
                            bizObject["Dbidcardissuedate"] = applicantInfo.idcardissuedate;
                            bizObject["DbIdCardNo"] = applicantInfo.idcardnumber; //111
                            bizObject["DbIdCarTypeName"] = applicantInfo.idcardtypeName;//222
                            bizObject["DbIndustrySubTypeName"] = applicantInfo.IndustrySubTypeName;
                            bizObject["DbindustrytypeName"] = applicantInfo.industrytypeName;
                            bizObject["DbissuingAuthority"] = applicantInfo.issuingAuthority;
                            bizObject["DbjobgroupName"] = applicantInfo.jobgroupName;
                            bizObject["DbLicenseNo"] = applicantInfo.licenseNumber;
                            bizObject["Dblienee"] = applicantInfo.lienee == "T" ? "是" : "否";
                            bizObject["Dbmaritalstatusname"] = applicantInfo.maritalstatusname;
                            bizObject["DbNationCode"] = applicantInfo.nationName;
                            bizObject["Dbnooffamilymembers"] = applicantInfo.nooffamilymembers;
                            bizObject["Dbnumberofdependents"] = applicantInfo.numberofdependents;
                            bizObject["DboccupationName"] = applicantInfo.occupationName;
                            bizObject["DbrelationShipName"] = applicantInfo.relationShipName;
                            bizObject["DbsalaryrangeName"] = applicantInfo.salaryrangeName;
                            //  bizObject["Dbsalayslipind"] = applicantInfo.salayslipind;
                            bizObject["Dbstaffind"] = applicantInfo.staffind == "T" ? "是" : "否";
                            bizObject["DbsubOccupationName"] = applicantInfo.subOccupationName;
                            bizObject["Dbthaititlename"] = applicantInfo.thaititlename;
                            bizObject["Dbtitlename"] = applicantInfo.titlename;
                            bizObject["Dbactualsalary"] = applicantInfo.actualsalary;
                            bizObject["Dbvipind"] = applicantInfo.vipind;
                        }
                        catch { }
                        #endregion

                        #region 工作信息 节点employment赋值
                        try
                        {
                            XmlNodeList employmentlst = xd.SelectSingleNode("//employment").ChildNodes;
                            foreach (XmlNode employmentnode in employmentlst)
                            {
                                if (employmentnode.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                                {
                                    employment employment = XmlUtil.Deserialize(typeof(employment), "<employment>" + employmentnode.InnerXml + "</employment>") as employment;

                                    bizObject["DbbusinesstypeName"] = employment.businesstypeName;
                                    bizObject["DbcompanyCityName"] = employment.cityName;
                                    bizObject["DbcompanyProvinceName"] = employment.provinceName;
                                    bizObject["DbComments"] = employment.comments;
                                    bizObject["DbcompanyAddress"] = employment.companyAddress;
                                    bizObject["DbcompanyName"] = employment.companyName;
                                    bizObject["Dbemployertype"] = employment.employertype;
                                    bizObject["DbFax"] = employment.Fax;
                                    bizObject["Dbjobdescription"] = employment.jobdescription;
                                    // bizObject["DblineId"] = employment.lineId;
                                    bizObject["Dbnoofemployees"] = employment.noofemployees;
                                    bizObject["Dbphonenumber"] = employment.phonenumber;
                                    bizObject["DbpositionName"] = employment.positionName;
                                    bizObject["DbcompanypostCode"] = employment.postCode;
                                    bizObject["DbcompanyProvinceName"] = employment.provinceName;
                                    bizObject["Dbtimeinyear"] = employment.timeinyear;
                                    bizObject["Dbtimeinmonth"] = employment.timeinmonth;
                                    break;
                                }
                            }
                        }
                        catch { }
                        #endregion

                        #region 联系信息 节点addressList，phoneList赋值
                        try
                        {
                            XmlNodeList addresslst = xd.SelectSingleNode("//addressList").ChildNodes;
                            foreach (XmlNode address in addresslst)
                            {
                                if (address.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                                {
                                    addressList addressList = XmlUtil.Deserialize(typeof(addressList), "<addressList>" + address.InnerXml + "</addressList>") as addressList;

                                    bizObject["DbAddressId"] = addressList.AddressId;
                                    bizObject["DbaddressstatusName"] = addressList.addressstatusName;
                                    bizObject["DbaddresstypeName"] = addressList.addresstypeName;
                                    bizObject["Dbbirthpalaceprovince"] = addressList.birthpalaceprovince;
                                    bizObject["DbhukoucityName"] = addressList.cityName;
                                    bizObject["DbcountryName"] = addressList.countryName;
                                    bizObject["DbcurrentLivingAddress"] = addressList.currentLivingAddress;
                                    bizObject["Dbdefaultmailingaddress"] = addressList.defaultmailingaddress == "Y" ? "是" : "否";
                                    bizObject["Dbhomevalue"] = addressList.homevalue;
                                    bizObject["Dbhukouaddress"] = addressList.hukouaddress == "Y" ? "是" : "否";
                                    bizObject["Dblivingsince"] = addressList.livingsince;
                                    bizObject["Dbnativedistrict"] = addressList.nativedistrict;
                                    bizObject["Dbpostcode"] = addressList.postcode;
                                    bizObject["DbpropertytypeName"] = addressList.propertytypeName;
                                    bizObject["DbhukouprovinceName"] = addressList.provinceName;
                                    bizObject["DbresidencetypeName"] = addressList.residencetypeName;
                                    bizObject["Dbstayinmonth"] = addressList.stayinmonth;
                                    bizObject["Dbstayinyear"] = addressList.stayinyear;
                                    break;
                                }
                            }
                        }
                        catch { }
                        try
                        {
                            XmlNodeList phonelst = xd.SelectSingleNode("//phoneList").ChildNodes;
                            foreach (XmlNode phone in phonelst)
                            {
                                if (phone.SelectSingleNode("IDENTIFICATION_CODE").InnerText == identification_code)
                                {
                                    phoneList phoneList = XmlUtil.Deserialize(typeof(phoneList), "<phoneList>" + phone.InnerXml + "</phoneList>") as phoneList;
                                    bizObject["DbareaCode"] = phoneList.areaCode;
                                    bizObject["DbcountrycodeName"] = phoneList.countrycodeName;
                                    bizObject["Dbextension"] = phoneList.extension;
                                    bizObject["DbphoneNo"] = phoneList.phoneNo;
                                    bizObject["DbphonetypeName"] = phoneList.phonetypeName;
                                    break;
                                }

                            }
                        }
                        catch { }
                        #endregion

                        break;
                    }
                }
            }
            catch { }
            #endregion
            return bizObject;
        }
        /// <summary>
        /// 获取影像工作流资料
        /// </summary>
        /// <param name="ApplyNo"></param>
        /// <returns></returns>
        public static DataTable getdkInstanceData(string WorkFlowCode, string applicationno)
        {
            DataTable rtn = new DataTable();
            string sql = "select b.objectid,a.objectid instanceid,b.applicationno from OT_Instancecontext a inner join H3.I_" + WorkFlowCode + " b on a.bizobjectid = b.objectid where A.STATE = '2' and b.applicationno ='" + applicationno + "' order by a.createdtime desc";
            rtn = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return rtn;
        }


        public DataTable getInstanceData(string SchemaCode, string instanceid)
        {
            DataTable rtn = new DataTable();
            //string sql = "select a.* from I_" + SchemaCode + " a, OT_instancecontext b where A.OBJECTID= b.bizobjectid and b.objectid = '" + instanceid + "'";
            string sql = " select a.*,c.codename from I_" + SchemaCode + " a";
            sql += " join OT_instancecontext b on A.OBJECTID = b.bizobjectid";
            sql += " left join area c on a.dycs = c.codeid";
            sql += " where b.objectid = '" + instanceid + "'";
            return rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        }
        public DataTable getInstanceData(string instanceid)
        {
            DataTable rtn = new DataTable();
            string SchemaCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("select workflowcode from H3.OT_instancecontext where objectid = '" + instanceid + "'").ToString();
            string sql = "select a.*,'" + SchemaCode + "' as SchemaCode from H3.I_" + SchemaCode + " a, H3.OT_instancecontext b where A.OBJECTID= b.bizobjectid and b.objectid = '" + instanceid + "'";
            return rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        }
        public string getRejectWorkItem(string objectids)
        {
            // 处理单引号 wangxg 19.8
            var idsArr = objectids.Split(',');
            for (int i = 0; i < idsArr.Length; i++)
            {
                idsArr[i] = $"'{idsArr[i]}'";
            }

            string rtn = "";
            string sql = " select objectid from H3.OT_WORKITEM where instanceid in ( select distinct instanceid from  H3.OT_WORKITEMFINISHED ";
            sql += " where ACTIONNAME='Reject' and workflowcode in ('CompanyApp','RetailApp','APPLICATION') AND activitycode in ('Activity17','Activity14')";
            sql += " and instanceid in (select instanceid from H3.OT_WORKITEM where";
            //sql += " objectid  in (" + objectids + "))) and objectid in (" + objectids + ")";
            sql += " objectid  in (" + string.Join(",", idsArr) + "))) and objectid in (" + string.Join(",", idsArr) + ")";// wangxg 19.8
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null)
                rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }
        /// <summary>
        /// 获取省市县
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string getssx(string usercode, string parentid)
        {
            string rtn = "";
            string sql = "";
            if (parentid == "100000")
            {
                sql += "select distinct  a.codeid,a.codename from area a inner join areaAuthority b on a.codeid=b.codeid  ";
                sql += "where 1=1";
                sql += "and b.userid='" + usercode + "' ";
            }
            else
            {
                sql += "select a.codeid,a.codename from area a where 1=1 ";
            }
            sql += "and a. parentid='" + parentid + "'";
            sql += "order by a.codeid";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null)
                rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }
        /// <summary>
        /// 根据维护人员获取抵押城市
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string getdycs(string usercode)
        {
            string rtn = "";
            string sql = "";
            sql += "select * from (";
            sql += "select  rank() over(order by shenid,shiid,xian) rowno,shen,shenid,shi,shiid,xian,xianid from (";
            sql += " select distinct a.codename shen , a.codeid shenid ,b.codename shi, b.codeid shiid ,case when c.sfdy='1' then   c.codename else '' end  xian ,";
            sql += " case when c.sfdy='1' then   c.codeid else '' end  xianid from area a left join area b on a.codeid=b.parentid left join area c on b.codeid=c.parentid";
            sql += " where a.codeid in (select distinct a.codeid from area a inner join areaAuthority b on a.codeid=b.codeid  where 1=1 and b.userid='" + usercode + "' )";
            sql += " and   b.sfdy='1' ) a order by shenid,shiid,xianid";
            sql += "  ) a order by a.rowno";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null)
                rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }
        /// <summary>
        /// 根据维护人员获取抵押城市
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string searchdycs(string ids, string usercode)
        {
            string rtn = "";
            string sql = "";
            string sqlCondition = "";
            string[] arrayID = ids.Split(',');
            if (arrayID[2] != "")//查询县
            {
                sqlCondition += " and (c.codeid ='" + arrayID[2] + "')";
            }
            else if (arrayID[1] != "")//查询市
            {
                sqlCondition += " and (b.codeid ='" + arrayID[1] + "' or c.parentid='" + arrayID[1] + "')";
            }
            else if (arrayID[0] != "")//查询省
            {
                sqlCondition += " and (a.codeid='" + arrayID[0] + "' or b.parentid='" + arrayID[0] + "' or  c.parentid in (select codeid from area where parentid='" + arrayID[0] + "'and sfdy='1')) ";
            }
            sql += "select  rank() over(order by shenid,shiid,xian) rowno,shen,shenid,shi,shiid,xian,xianid from (";
            sql += " select distinct a.codename shen , a.codeid shenid ,b.codename shi, b.codeid shiid ,case when c.sfdy='1' then   c.codename else '' end  xian ,";
            sql += " case when c.sfdy='1' then   c.codeid else '' end  xianid from area a left join area b on a.codeid=b.parentid left join area c on b.codeid=c.parentid";
            sql += " where a.codeid in (select distinct a.codeid from area a inner join areaAuthority b on a.codeid=b.codeid  where 1=1  and a.parentid =100000  and b.userid='" + usercode + "' )";
            sql += " and   b.sfdy='1' " + sqlCondition + " ) a order by shenid,shiid,xian";

            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null)
                rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }
/// <summary>
        /// 抵押城市下载
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string downloaddycs()
        {
            string rtn = "";
            string sql = @"SELECT 
        shen 省,
        shi 市,
        xian 县
FROM 
    (SELECT DISTINCT a.codename shen ,a.codeid shenid ,b.codename shi,b.codeid shiid ,
        case
        WHEN c.sfdy='1' THEN
        c.codename
        ELSE ''
        END xian ,
        CASE
        WHEN c.sfdy='1' THEN
        c.codeid
        ELSE ''
        END xianid
    FROM area a
    LEFT JOIN area b
        ON a.codeid=b.parentid
    LEFT JOIN area c
        ON b.codeid=c.parentid
    WHERE a.codeid IN 
        (SELECT DISTINCT a.codeid
        FROM area a
        INNER JOIN areaAuthority b
            ON a.codeid=b.codeid
        WHERE a.parentid =100000 
                )
                AND b.sfdy='1' ) a 
    ORDER BY  shenid,shiid,xian desc";

            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null)
                rtn = WorkFlowFunction.DataToJSON(dt);
            return rtn;
        }
        /// <summary>
        /// 根据维护人员获取抵押城市
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string deldycs(string ids, string usercode)
        {
            string rtn = "";
            string sql = "update area set sfdy='',dyr='" + usercode + "', dysj='" + DateTime.Now.ToString() + "' where 1=1 ";
            string[] arrayID = ids.Split(',');
            if (arrayID[2] != "" && arrayID[2] != "undefined")//删除县
            {
                sql += "and codeid='" + arrayID[2] + "'";
            }
            else//删除市
            {
                //先判断市下面有没有县，如果有不能删除
                string csql = "select  * from area where sfdy='1' and parentid='" + arrayID[1] + "'";
                if (CommonFunction.hasData(this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(csql)))
                {
                    return "下级还有数据不能直接删除！";
                }
                sql += "and codeid='" + arrayID[1] + "'";

            }
            rtn = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql).ToString();

            return rtn;
        }
       
        /// <summary>
        /// 更新抵押城市
        /// </summary>
        /// <param name="usercode"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public string updatedycs(string usercode, string shen, string shi, string xian)
        {
            string rtn = "";
            string sql = "";
            string condition = "'" + shen + "','" + shi + "'";
            if (xian != "")
            {
                condition += ",'" + xian + "'";
            }
            sql += " update area set sfdy='1',dyr='" + usercode + "',dysj='" + DateTime.Now.ToString() + "' where codeid in (" + condition + ")";
            this.Engine.LogWriter.Write("修改抵押城市sql：" + sql);
            rtn = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql).ToString();

            return rtn;
        }
        /// <summary>
        /// 取消流程实例
        /// </summary>
        /// <param name="ApplyNo"></param>
        /// <returns></returns>
        public string cancelInstanceData(string instanceid)
        {
            OThinker.H3.Controllers.InstanceDetailController controller = new OThinker.H3.Controllers.InstanceDetailController();
            var result = controller.CancelInstance(instanceid);
            return result.ToString();
        }
        /// <summary>
        /// 更新流程实例数据
        /// </summary>
        /// <param name="ApplyNo"></param>
        /// <returns></returns>
        public string updateInstanceData(string instanceid, string workflowCode)
        {
            string rtn = "";
            var user = this.Engine.Organization.GetUserByCode("administrator" + string.Empty);
            InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(instanceid);
            if (ic == null)
            {
                return "";
            }

            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workflowCode);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(
                this.Engine.Organization,
                this.Engine.MetadataRepository,
                this.Engine.BizObjectManager,
                null,
                schema,
                user.ObjectID,
                user.ParentID);

            bo.ObjectID = ic.BizObjectId;
            bo.Load();//装载流程数据；
            //bo = setIndvidualBizObject(bo, new XmlDocument(), "", "");
            bo.Update();
            #region 结束当前任务
            string sql = "SELECT ObjectID FROM OT_WorkItem WHERE InstanceId='{0}' ORDER BY TokenId desc";
            sql = string.Format(sql, instanceid);
            string workItemId = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            if (workItemId != "")
            {
                // 获取工作项
                OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);

                // 结束工作项
                this.Engine.WorkItemManager.FinishWorkItem(
                    item.ObjectID,
                    user.UnitID,
                    OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                    null,
                    OThinker.Data.BoolMatchValue.True,
                    string.Empty,
                    null,
                    OThinker.H3.WorkItem.ActionEventType.Forward,
                    11);

                // 需要通知实例事件管理器结束事件
                AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                        MessageEmergencyType.Normal,
                        item.InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.True,
                        false,
                        OThinker.Data.BoolMatchValue.True,
                        true,
                        null);
                this.Engine.InstanceManager.SendMessage(endMessage);
            }
            #endregion

            return rtn;
        }
        private System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        /// <summary>
        /// 获取JOSN序列化对象
        /// </summary>
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (jsSerializer == null)
                {
                    jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return jsSerializer;
            }
        }
        /// <summary>
        /// 设置数据项的值
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="property"></param>
        /// <param name="itemValue"></param>
        private void SetItemValue(OThinker.H3.DataModel.BizObject bo, OThinker.H3.DataModel.PropertySchema property, object itemValue)
        {
            if (property.LogicType == DataLogicType.DateTime)
            {
                DateTime d = DateTime.Now;
                DateTime.TryParse(itemValue + string.Empty, out d);
                bo[property.Name] = d;
            }
            else if (property.LogicType == DataLogicType.Bool)
            {
                bool b = false;
                bool.TryParse(itemValue + string.Empty, out b);
                bo[property.Name] = b;
            }
            else if (property.LogicType == DataLogicType.TimeSpan)
            {// 时间段
                TimeSpan t = new TimeSpan(0, 0, 0);
                TimeSpan.TryParse(itemValue + string.Empty, out t);
                bo[property.Name] = t;
            }
            else if (property.LogicType == DataLogicType.SingleParticipant)
            {// 参与者单人:传入用户登录名
                if (itemValue != null)
                {
                    try
                    {
                        var user = this.Engine.Organization.GetUserByCode(itemValue + string.Empty);
                        bo[property.Name] = user == null ? "" : user.ObjectID;
                    }
                    catch (Exception ex)
                    {
                        bo[property.Name] = null;
                        this.Engine.LogWriter.Write("字段：" + property.Name + "赋值异常-->" + ex.ToString());
                    }
                }
            }
            else if (property.LogicType == DataLogicType.MultiParticipant)
            {// 参与者多人:传入用户登录名
                if (itemValue != null)
                {
                    try
                    {
                        List<string> retUserIDs = new List<string>();
                        List<string> userCodes = JSSerializer.Deserialize<List<string>>(JSSerializer.Serialize(itemValue));
                        if (userCodes == null)
                            bo[property.Name] = null;
                        else
                        {
                            foreach (string code in userCodes)
                            {
                                var user = this.Engine.Organization.GetUserByCode(code + string.Empty);
                                //this.Engine.Organization.getuser
                                if (user != null)
                                    retUserIDs.Add(user.ObjectID);
                            }
                            bo[property.Name] = retUserIDs.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        bo[property.Name] = null;
                        this.Engine.LogWriter.Write("字段：" + property.Name + "赋值异常-->" + ex.ToString());
                    }
                }
            }
            else if (property.LogicType == DataLogicType.BizObjectArray)
            {// 子表
                if (property.ChildSchema == null) return;

                List<OThinker.H3.DataModel.BizObject> details = new List<OThinker.H3.DataModel.BizObject>();
                List<Dictionary<string, object>> gridView = JSSerializer.Deserialize<List<Dictionary<string, object>>>(JSSerializer.Serialize(itemValue));
                if (gridView == null) return;

                foreach (Dictionary<string, object> row in gridView)
                {
                    OThinker.H3.DataModel.BizObject subBo = new OThinker.H3.DataModel.BizObject(this.Engine, property.ChildSchema, bo.OwnerId);
                    foreach (KeyValuePair<string, object> item in row)
                    {
                        OThinker.H3.DataModel.PropertySchema childProperty = property.ChildSchema.GetProperty(item.Key);
                        if (childProperty == null) continue;
                        this.SetItemValue(subBo, childProperty, item.Value);
                    }
                    details.Add(subBo);
                }
                bo[property.Name] = details.ToArray();
            }
            else if (property.LogicType == DataLogicType.Attachment)
            {// 附件
                //List<List<DataItemParam>> attachments = JSSerializer.Deserialize<List<List<DataItemParam>>>(JSSerializer.Serialize(itemValue));
                List<AttachmentParamNew> attachments = JSSerializer.Deserialize<List<AttachmentParamNew>>(JSSerializer.Serialize(itemValue));
                //先清理掉当前流程实例的所有附件（在更新的情况下）
                this.Engine.BizObjectManager.RemoveAttachments(bo.Schema.SchemaCode, bo.ObjectID, property.Name);
                if (attachments != null)
                {
                    foreach (AttachmentParamNew attachment in attachments)
                    {
                        OThinker.H3.Data.Attachment attach = new Attachment()
                        {
                            FileName = attachment.FileName,
                            BizObjectId = bo.ObjectID,
                            DataField = property.Name,
                            Downloadable = true,
                            DownloadUrl = attachment.DownloadUrl,
                            Content = attachment.Content,
                            BizObjectSchemaCode = bo.Schema.SchemaCode,
                            ContentLength = attachment.Content == null ? attachment.ContentLength : attachment.Content.Length,
                            ContentType = attachment.ContentType
                        };
                        this.Engine.BizObjectManager.AddAttachment(attach);
                    }
                }
            }
            //if (property.LogicType == DataLogicType.String
            //                   || property.LogicType == DataLogicType.ShortString
            //                   || property.LogicType == DataLogicType.Int
            //                   || property.LogicType == DataLogicType.Long
            //                   || property.LogicType == DataLogicType.Decimal
            //                   || property.LogicType == DataLogicType.Html
            //    || property.LogicType == DataLogicType.Double)
            else
            {// 短文本、长文本、整数、数值、HTML
                bo[property.Name] = itemValue;
            }
        }

        public Authentication authentication = new Authentication("administrator", "000000");
        public OThinker.H3.Controllers.UserValidator UserValidator = null;
        public string GetUserIDByCode(string UserCode)
        {
            var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(this.Engine, UserCode);
            return CurrentUserValidator.UserID;
        }
        public string getBizobjectByInstanceid(string Instanceid)
        {
            string sql = "select bizobjectid from H3.OT_instancecontext where objectid='" + Instanceid + "'";
            return this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }

        /// <summary>
        /// 验证当前用户是否正确
        /// </summary>
        /// <returns></returns>
        public void ValidateSoapHeader()
        {
            if (authentication == null)
            {
                throw new Exception("请输入身份认证信息!");
            }
            UserValidator = OThinker.H3.Controllers.UserValidatorFactory.Validate(authentication.UserCode, authentication.Password);
            if (UserValidator == null)
            {
                throw new Exception("帐号或密码不正确!");
            }
            this.Engine = UserValidator.Engine;
            // this.Engine = OThinker.H3.WorkSheet.AppUtility.Engine;
        }
        /// <summary>
        /// 获取用户的待办任务总数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        public int GetUserUnfinishedWorkItemCount(string UserCode)
        {
            int rtn = 0;
            string UserID = GetUserIDByCode(UserCode);
            string sql = " SELECT COUNT(1) FROM OT_WorkItem WHERE (OT_WorkItem.Participant='" + UserID + "' OR OT_WorkItem.Delegant='" + UserID + "') ";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = int.Parse(dt.Rows[0][0].ToString());
            }
            return rtn;
        }

        /// <summary>
        /// 获取用户的已办任务总数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int GetUserFinishedWorkItemCount(string UserCode)
        {
            int recordCounts;
            string UserID = GetUserIDByCode(UserCode);
            // 构造查询用户帐号的条件
            string[] conditions = Query.GetWorkItemConditions(
                UserID,
                true,
                OThinker.H3.WorkItem.WorkItemState.Finished,
                OThinker.H3.WorkItem.WorkItemType.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified);
            // 获取总记录数，计算页码
            recordCounts = this.Engine.Query.CountWorkItem(conditions);
            return recordCounts;
        }
        public string GetUserFinishedWorkItemCountBySql(string UserCode)
        {
            var rtn = "0";
            string UserID = GetUserIDByCode(UserCode);
            string sql = "SELECT COUNT(1) FROM OT_WorkItemFinished WHERE(OT_WorkItemFinished.Participant= '" + UserID + "' OR OT_WorkItemFinished.Delegant= '" + UserID + "') AND OT_WorkItemFinished.State = 2";
            rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
            return rtn;
        }
        /// <summary>
        /// 根据sql获取用户的待阅任务总数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string GetUserUnReadWorkItemCountBySql(string UserCode)
        {
            var rtn = "0";
            string UserID = GetUserIDByCode(UserCode);
            string sql = "SELECT COUNT(1) FROM OT_CirculateItem WHERE(OT_CirculateItem.Participant = '" + UserID + "' OR OT_CirculateItem.Delegant = '" + UserID + "') AND OT_CirculateItem.State != 2 AND OT_CirculateItem.State != 3";
            rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
            return rtn;
        }
        /// <summary>
        /// 获取用户的待阅任务总数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int GetUserUnReadWorkItemCount(string UserCode)
        {
            int recordCounts;
            string UserID = GetUserIDByCode(UserCode);
            // 构造查询用户帐号的条件
            string[] conditions = Query.GetWorkItemConditions(
                UserID,
                true,
                OThinker.H3.WorkItem.WorkItemState.Unfinished,
                OThinker.H3.WorkItem.WorkItemType.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified);

            // 获取总记录数     
            recordCounts = this.Engine.PortalQuery.CountWorkItem(conditions, OThinker.H3.WorkItem.CirculateItem.TableName);
            return recordCounts;
        }

        /// <summary>
        /// 获取用户的已阅任务总数
        /// </summary>
        /// <returns></returns>
        public int GetUserReadedWorkItemCount(string UserCode)
        {
            string UserID = GetUserIDByCode(UserCode);
            int recordCounts;
            // 构造查询用户帐号的条件
            string[] conditions = Query.GetWorkItemConditions(
                UserID,
                true,
                OThinker.H3.WorkItem.WorkItemState.Finished,
                OThinker.H3.WorkItem.WorkItemType.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified);

            // 获取总记录数       
            recordCounts = this.Engine.PortalQuery.CountWorkItem(conditions, OThinker.H3.WorkItem.CirculateItemFinished.TableName);
            return recordCounts;
        }

        // <summary>
        /// 查询用户的已办
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="startTime">开始时间（可控）</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="endIndex">结束索引</param>
        /// <param name="workflowCode">流程编码</param>
        /// <param name="instanceName">流程实例名称</param>
        /// <returns></returns>
        public List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> GetFinishWorkItems(string userCode, DateTime? startTime, DateTime? endTime, int startIndex, int endIndex, string workflowCode, string instanceName)
        {
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> listItems = new List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel>();
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode);
            if (user == null) return listItems;

            string[] conditions = Engine.PortalQuery.GetWorkItemConditions(user.ObjectID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    OThinker.H3.WorkItem.WorkItemState.Finished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    true,
                    OThinker.H3.WorkItem.WorkItemFinished.TableName);
            string OrderBy = "ORDER BY " +
                 OThinker.H3.WorkItem.WorkItemFinished.TableName + "." + OThinker.H3.WorkItem.WorkItemFinished.PropertyName_ReceiveTime + " DESC";
            DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, startIndex, endIndex, OrderBy, OThinker.H3.WorkItem.WorkItemFinished.TableName);
            //int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItemFinished.TableName); // 记录总数
            string[] columns = new string[] { OThinker.H3.WorkItem.WorkItemFinished.PropertyName_OrgUnit };

            OThinker.H3.Controllers.WorkItemController controller = new OThinker.H3.Controllers.WorkItemController();
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> griddata = controller.Getgriddata(dtWorkitem, columns);

            return griddata;
        }

        // <summary>
        /// 查询用户的待办
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="startTime">开始时间（可控）</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="endIndex">结束索引</param>
        /// <param name="workflowCode">流程编码</param>
        /// <param name="instanceName">流程实例名称</param>
        /// <returns></returns>
        public List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> GetUnFinishWorkItems(string userCode, DateTime? startTime, DateTime? endTime, int startIndex, int endIndex, string workflowCode, string instanceName)
        {
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> listItems = new List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel>();
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode);
            if (user == null) return listItems;

            string[] conditions = Engine.PortalQuery.GetWorkItemConditions(user.ObjectID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    OThinker.H3.WorkItem.WorkItemState.Unfinished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    true,
                    OThinker.H3.WorkItem.WorkItem.TableName);
            string OrderBy = "ORDER BY " +
                 OThinker.H3.WorkItem.WorkItem.TableName + "." + OThinker.H3.WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
            DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, startIndex, endIndex, OrderBy, OThinker.H3.WorkItem.WorkItem.TableName);
            //int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItemFinished.TableName); // 记录总数
            string[] columns = new string[] { OThinker.H3.WorkItem.WorkItem.PropertyName_OrgUnit };

            OThinker.H3.Controllers.WorkItemController controller = new OThinker.H3.Controllers.WorkItemController();
            List<OThinker.H3.Controllers.ViewModels.WorkItemViewModel> griddata = controller.Getgriddata(dtWorkitem, columns);

            return griddata;
        }
        /// <summary>
        /// 根据sql获取用户的已阅任务总数
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public string GetUserReadWorkItemCountBySql(string UserCode)
        {
            var rtn = "0";
            string UserID = GetUserIDByCode(UserCode);
            string sql = "SELECT COUNT(1) FROM OT_CirculateItemFinished WHERE(OT_CirculateItemFinished.Participant = '" + UserID + "' OR OT_CirculateItemFinished.Delegant = '" + UserID + "') AND OT_CirculateItemFinished.State = 2";
            rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
            return rtn;
        }

        /// <summary>
        /// 查询用户的已阅任务
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="startTime">开始时间（可控）</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="endIndex">结束索引</param>
        /// <param name="workflowCode">流程编码</param>
        /// <param name="instanceName">流程实例名称</param>
        /// <returns></returns>
        public List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> GetReadWorkItems(string userCode, DateTime? startTime, DateTime? endTime, int startIndex, int endIndex, string workflowCode, string instanceName)
        {
            List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> listItems = new List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel>();
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode);
            if (user == null) return listItems;
            string[] conditions = Engine.PortalQuery.GetWorkItemConditions(user.ObjectID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    OThinker.H3.WorkItem.WorkItemState.Finished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    false,
                    OThinker.H3.WorkItem.CirculateItemFinished.TableName);
            DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, startIndex, endIndex, string.Empty, OThinker.H3.WorkItem.CirculateItemFinished.TableName);

            int total = Engine.PortalQuery.CountWorkItem(conditions, OThinker.H3.WorkItem.CirculateItemFinished.TableName); // 记录总数
            string[] columns = new string[] { OThinker.H3.WorkItem.CirculateItemFinished.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };

            OThinker.H3.Controllers.CirculateItemController controller = new OThinker.H3.Controllers.CirculateItemController();
            listItems = controller.Getgriddata(dtWorkitem, columns, false);
            return listItems;

        }


        /// <summary>
        /// 查询用户的待阅任务
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="startTime">开始时间（可控）</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="startIndex">开始索引</param>
        /// <param name="endIndex">结束索引</param>
        /// <param name="workflowCode">流程编码</param>
        /// <param name="instanceName">流程实例名称</param>
        /// <returns></returns>
        public List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> GetUnReadWorkItems(string userCode, DateTime? startTime, DateTime? endTime, int startIndex, int endIndex, string workflowCode, string instanceName)
        {
            List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel> listItems = new List<OThinker.H3.Controllers.ViewModels.CirculateItemViewModel>();
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode);
            if (user == null) return listItems;
            string[] conditions = Engine.PortalQuery.GetWorkItemConditions(user.ObjectID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    OThinker.H3.WorkItem.WorkItemState.Unfinished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    false,
                    OThinker.H3.WorkItem.CirculateItem.TableName);
            DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, startIndex, endIndex, string.Empty, OThinker.H3.WorkItem.CirculateItem.TableName);

            //int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.CirculateItem.TableName); // 记录总数
            string[] columns = new string[] { OThinker.H3.WorkItem.CirculateItem.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };

            OThinker.H3.Controllers.CirculateItemController controller = new OThinker.H3.Controllers.CirculateItemController();
            listItems = controller.Getgriddata(dtWorkitem, columns, false);
            return listItems;

        }

        /// <summary>
        /// 调用融数接口
        /// </summary>
        /// <param name="instanceid">流程实例id</param>
        /// <returns></returns>
        public string postHttp(string SchemaCode, string instanceid)
        {
            if (instanceid == null || instanceid == "")
            {
                this.Engine.LogWriter.Write("instanceid：" + instanceid);
                return "";
            }
            //融数地址
            String url = ConfigurationManager.AppSettings["rsurl"] + string.Empty;
            //回调Url
            string cbURL = ConfigurationManager.AppSettings["cbURL"] + string.Empty;
            if (url == null || url == "" || cbURL == null || cbURL == "")
            {
                this.Engine.LogWriter.Write("调用失败：调用融数url或融数回调url不能为空！");
                return "调用失败：调用url不能为空！";
            }
            DataTable dt = getInstanceData(SchemaCode, instanceid);
            //通信识别ID
            string reqID = instanceid;

            //创建时间
            string time = DateTime.Now.ToString();
            //简化贷，标贷
            string lx = dt.Rows[0]["productGroupName"].ToString();
            if (lx.Contains("简化") || lx.Contains("车秒") || lx.Contains("高首付"))
            {
                lx = "1";
            }
            else if (lx.Contains("机构贷"))
            {
                lx = "3";
            }
            else
            {
                lx = "2";
            }
            //string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["applicationno"].ToString() + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            if (lx != "3")
            {
                param += "\"cust_name\":\"" + dt.Rows[0]["ThaiFirstName"].ToString().Trim() + "\",";
                param += "\"cert_tpye\":\"" + dt.Rows[0]["IdCarTypeName"].ToString().Trim() + "\",";
                param += "\"cert_no\":\"" + dt.Rows[0]["IdCardNo"].ToString().Trim() + "\",";
                param += "\"cert_end_date\":\"" + dt.Rows[0]["IdCardExpiryDate"].ToString().Trim() + "\",";
                param += "\"mobile\":\"" + dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
                param += "\"gender\":\"" + dt.Rows[0]["genderName"].ToString().Trim() + "\",";
                param += "\"birthday_year\":\"\",";
                param += "\"birthday_month\":\"\",";
                param += "\"cust_birthday\":\"" + dt.Rows[0]["DateOfBirth"].ToString().Trim() + "\",";
                param += "\"registered_address_province\":\"" + dt.Rows[0]["hukouprovinceName"].ToString().Trim() + "\",";
                //户籍地址
                param += "\"registered_address_city\":\"" + dt.Rows[0]["hukoucityName"].ToString().Trim() + "\",";
                //新增居住地
                param += "\"residence_address_city\":\"" + dt.Rows[0]["currentLivingAddress"].ToString().Trim() + "\",";
                //新增抵押城市
                param += "\"plege_address_city\":\"" + dt.Rows[0]["codename"].ToString().Trim() + "\",";
                param += "\"reside_address_province\":\"" + dt.Rows[0]["provinceName"].ToString().Trim() + "\",";
                param += "\"nationality\":\"" + dt.Rows[0]["CitizenshipName"].ToString().Trim() + "\",";
                param += "\"nation\":\"" + dt.Rows[0]["NationName"].ToString().Trim() + "\",";
                param += "\"education_level\":\"" + dt.Rows[0]["EducationName"].ToString().Trim() + "\",";
                param += "\"marital_status\":\"" + dt.Rows[0]["MaritalStatusName"].ToString().Trim() + "\",";
                param += "\"marital_status_check\":\"\",";
                param += "\"house_status\":\"" + dt.Rows[0]["propertytypeName"].ToString().Trim() + "\",";
                param += "\"employer\":\"" + dt.Rows[0]["ZjrCompanyName"].ToString().Trim() + "\",";
                param += "\"employer_telphone\":\"" + dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
                param += "\"employer_address_province\":\"" + dt.Rows[0]["comapnyProvinceName"].ToString().Trim() + "\",";
                param += "\"employer_address_city\":\"" + dt.Rows[0]["companyCityName"].ToString().Trim() + "\",";
                param += "\"occupation\":\"" + dt.Rows[0]["positionName"].ToString().Trim() + "\",";
                param += "\"working_years\":\"" + dt.Rows[0]["timeinyear"].ToString().Trim() + "\",";
                param += "\"income_month\":\"" + dt.Rows[0]["ActualSalary"].ToString().Trim() + "\",";
                param += "\"mate_name\":\"" + dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
                param += "\"mate_cert_type\":\"" + dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
                param += "\"mate_cert_no\":\"" + dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
                param += "\"mate_mobile\":\"" + dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";
                param += "\"partner_name\":\"" + dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
                param += "\"partner_cert_type\":\"" + dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
                param += "\"partner_cert_no\":\"" + dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
                param += "\"partner_moblie\":\"" + dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";

                //判断内外网客户
                //param += "\"customer_source\":\"" + (dt.Rows[0]["userName"].ToString().StartsWith("98") ? "内网" : "外网") + "\",";
                string nww = "";
                if (dt.Rows[0]["userName"].ToString().StartsWith("98") || dt.Rows[0]["userName"].ToString().StartsWith("80000"))
                {
                    nww = "内网";
                }
                else
                {
                    nww = "外网";
                }
                param += "\"customer_source\":\"" + nww + "\",";
                param += "\"business_type\":\"" + dt.Rows[0]["assetConditionName"].ToString().Trim() + "\",";
                param += "\"fours_number\":\"" + dt.Rows[0]["userName"].ToString().Trim() + "\",";
                param += "\"dealer_name\":\"" + dt.Rows[0]["dealerName"].ToString().Trim() + "\",";
                param += "\"vehicle_model\":\"" + dt.Rows[0]["brandName"].ToString().Trim() + "\",";
                param += "\"vehicle_brand\":\"" + dt.Rows[0]["assetMakeName"].ToString().Trim() + "\",";
                param += "\"vehicle_purpose\":\"" + dt.Rows[0]["purposeName"].ToString().Trim() + "\",";
                param += "\"vehicle_price\":\"" + dt.Rows[0]["vehicleprice"].ToString().Trim() + "\",";
                param += "\"application_amount\":\"" + dt.Rows[0]["financedamount"].ToString().Trim() + "\",";
                param += "\"application_term\":\"" + dt.Rows[0]["termMonth"].ToString().Trim() + "\",";
                param += "\"product_group\":\"" + dt.Rows[0]["productGroupName"].ToString().Trim() + "\",";
                param += "\"product_type\":\"" + dt.Rows[0]["productTypeName"].ToString().Trim() + "\",";
                param += "\"first_payments_ratio\":\"" + dt.Rows[0]["downpaymentrate"].ToString().Trim() + "\",";
                param += "\"first_payments_amount\":\"" + dt.Rows[0]["downpaymentamount"].ToString().Trim() + "\",";
                param += "\"vehicle_identification_value\":\"" + dt.Rows[0]["assetPrice"].ToString().Trim() + "\",";
                param += "\"if_partner\":\"\",";
                param += "\"if_partner_relatives\":\"" + dt.Rows[0]["GjrelationShipName"].ToString().Trim() + "\",";

                //客户现有银行号 [Caccountnum]
                param += "\"bank_no\":\"" + dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";
                //判断共借人是否是配偶
                param += "\"if_partner_mate\":\"" + (dt.Rows[0]["GjrelationShipName"].ToString().Contains("配偶") ? "是" : "否") + "\",";
                //是否隐藏配偶
                param += "\"if_hidden_partner\":\"" + dt.Rows[0]["IS_INACTIVE_IND"].ToString().Trim() + "\",";
                param += "\"if_hidden_partner2\":\"" + dt.Rows[0]["Gjspouse"].ToString().Trim() + "\",";

                //判断申请人是否有存量的简化贷
                //param += "\"if_stock_simplified_loan\":\"" + GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString()) + "\",";
                param += "\"if_stock_simplified_loan\":\"" + GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString(), dt.Rows[0]["applicationNo"].ToString()) + "\",";
                //判断是否包括附加费贷款（是/否）
                param += "\"if_extra_charge_loan\":\"" + (dt.Rows[0]["accessoriesInd"].ToString().Trim()) + "\",";
                param += "\"if_rereq_complete\":\"是\",";
                //获取敞口值：申请人+配偶+本次贷款金额
                param += "\"exposure_value\":\"" + indvidualHistoryFinancedAmount(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim(), dt.Rows[0]["financedamount"].ToString().Trim()) + "\",";
                param += "\"if_quality_dealer_code\":\"" + yzjxs(dt.Rows[0]["userName"].ToString()).Trim() + "\",";
                param += "\"if_mortgage\":\"" + dt.Rows[0]["lienee"].ToString().Trim() + "\",";
                param += "\"vehicle_manufacture_date\":\"" + dt.Rows[0]["releaseDate"].ToString().Trim() + "\",";
                param += "\"vehicle_kilometers\":\"" + dt.Rows[0]["odometer"].ToString().Trim() + "\",";
                param += "\"if_data_complete\":\"是\",";
                param += "\"cym\":\"" + dt.Rows[0]["FormerName"].ToString().Trim() + "\",";
                param += "\"csdssx\":\"" + dt.Rows[0]["birthpalaceprovince"].ToString().Trim() + "\",";
                param += "\"jgssx\":\"" + dt.Rows[0]["nativedistrict"].ToString().Trim() + "\",";
                param += "\"ssssxq\":\"" + dt.Rows[0]["hukouprovinceName"].ToString().Trim() + dt.Rows[0]["hukoucityName"].ToString().Trim() + "\",";
                param += "\"zz\":\"" + dt.Rows[0]["currentLivingAddress"].ToString().Trim() + "\",";
                param += "\"partner_cym\":\"" + dt.Rows[0]["GjFormerName"].ToString().Trim() + "\",";
                param += "\"partner_xb\":\"" + dt.Rows[0]["GjgenderName"].ToString().Trim() + "\",";
                param += "\"partner_mz\":\"" + dt.Rows[0]["GjNationName"].ToString().Trim() + "\",";
                param += "\"partner_csrq\":\"" + dt.Rows[0]["GjDateOfBirth"].ToString().Trim() + "\",";
                param += "\"partner_whcd\":\"" + dt.Rows[0]["GjEducationName"].ToString().Trim() + "\",";
                param += "\"partner_hyzk\":\"" + dt.Rows[0]["GjMaritalStatusName"].ToString().Trim() + "\",";
                param += "\"partner_fwcs\":\"" + dt.Rows[0]["GjcompanyName"].ToString().Trim() + "\",";
                param += "\"partner_csdssx\":\"" + dt.Rows[0]["Gjbirthpalaceprovince"].ToString().Trim() + "\",";
                param += "\"partner_jgssx\":\"" + dt.Rows[0]["Gjnativedistrict"].ToString().Trim() + "\",";
                param += "\"partner_ssssxq\":\"" + dt.Rows[0]["GjhukouprovinceName"].ToString().Trim() + dt.Rows[0]["GjhukoucityName"].ToString().Trim() + "\",";
                param += "\"partner_zz\":\"" + dt.Rows[0]["GjcurrentLivingAddress"].ToString().Trim() + "\",";
            }
            param += "\"if_guarantee\":\"\",";
            param += "\"guarantee_name\":\"" + dt.Rows[0]["DbThaiFirstName"].ToString().Trim() + "\",";
            param += "\"guarantee_cert_type\":\"" + dt.Rows[0]["DbIdCarTypeName"].ToString().Trim() + "\",";
            param += "\"guarantee_cert_no\":\"" + dt.Rows[0]["DbIdCardNo"].ToString().Trim() + "\",";
            param += "\"guarantee_mobile\":\"" + dt.Rows[0]["DbphoneNo"].ToString().Trim() + "\",";
            param += "\"guarantee_cym\":\"" + dt.Rows[0]["DbFormerName"].ToString().Trim() + "\",";
            param += "\"guarantee_xb\":\"" + dt.Rows[0]["DbgenderName"].ToString().Trim() + "\",";
            if (lx != "3")
            {
                param += "\"guarantee_mz\":\"" + dt.Rows[0]["DbNationName"].ToString().Trim() + "\",";
            }
            else
            {
                param += "\"guarantee_mz\":\"" + dt.Rows[0]["DbNationCode"].ToString().Trim() + "\",";
            }
            param += "\"guarantee_csrq\":\"" + dt.Rows[0]["DbDateOfBirth"].ToString().Trim() + "\",";
            param += "\"guarantee_whcd\":\"" + dt.Rows[0]["DbEducationName"].ToString().Trim() + "\",";
            param += "\"guarantee_hyzk\":\"" + dt.Rows[0]["DbMaritalStatusName"].ToString().Trim() + "\",";
            param += "\"guarantee_fwcs\":\"" + dt.Rows[0]["DbcompanyName"].ToString().Trim() + "\",";
            param += "\"guarantee_csdssx\":\"" + dt.Rows[0]["Dbbirthpalaceprovince"].ToString().Trim() + "\",";
            param += "\"guarantee_jgssx\":\"" + dt.Rows[0]["Dbnativedistrict"].ToString().Trim() + "\",";
            param += "\"guarantee_ssssxq\":\"" + dt.Rows[0]["DbhukouprovinceName"].ToString().Trim() + dt.Rows[0]["DbhukoucityName"].ToString().Trim() + "\",";
            param += "\"guarantee_zz\":\"" + dt.Rows[0]["DbcurrentLivingAddress"].ToString().Trim() + "\",";
            //新增经销商城市
            param += "\"dealer_address_city\":\"" + dt.Rows[0]["dealercityName"].ToString().Trim() + "\"";
            param += "}}}";
            this.Engine.LogWriter.Write("调用融数风控系统开始！");
            this.Engine.LogWriter.Write("param参数值：" + param);
            //推送数据给融数
            var result = "";
            try
            {
                result = PostMoths(url, param);
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("融数风控调用出错" + ex.Message.ToString());
            }
            this.Engine.LogWriter.Write(result.ToString());
            return result.ToString();

        }

        public string postHttpByManual(string SchemaCode, string instanceid, string manual, string url)
        {
            DataTable dt = getInstanceData(SchemaCode, instanceid);
            //通信识别ID
            string reqID = instanceid;

            //创建时间
            string time = DateTime.Now.ToString();
            //简化贷，标贷
            string lx = dt.Rows[0]["productGroupName"].ToString();
            if (lx.Contains("简化") || lx.Contains("车秒") || lx.Contains("高首付"))
            {
                lx = "1";
            }
            else if (lx.Contains("机构贷"))
            {
                lx = "3";
            }
            else
            {
                lx = "2";
            }
            string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["applicationno"].ToString() + "\",\"cbURL\":\"\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"manual\":\"" + manual + "\",\"param\":{";
            //string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["applicationno"].ToString() + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            if (lx != "3")
            {
                param += "\"cust_name\":\"" + dt.Rows[0]["ThaiFirstName"].ToString().Trim() + "\",";
                param += "\"cert_tpye\":\"" + dt.Rows[0]["IdCarTypeName"].ToString().Trim() + "\",";
                param += "\"cert_no\":\"" + dt.Rows[0]["IdCardNo"].ToString().Trim() + "\",";
                param += "\"cert_end_date\":\"" + dt.Rows[0]["IdCardExpiryDate"].ToString().Trim() + "\",";
                param += "\"mobile\":\"" + dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
                param += "\"gender\":\"" + dt.Rows[0]["genderName"].ToString().Trim() + "\",";
                param += "\"birthday_year\":\"\",";
                param += "\"birthday_month\":\"\",";
                param += "\"cust_birthday\":\"" + dt.Rows[0]["DateOfBirth"].ToString().Trim() + "\",";
                param += "\"registered_address_province\":\"" + dt.Rows[0]["hukouprovinceName"].ToString().Trim() + "\",";
                //户籍地址
                param += "\"registered_address_city\":\"" + dt.Rows[0]["hukoucityName"].ToString().Trim() + "\",";
                //新增居住地
                param += "\"residence_address_city\":\"" + dt.Rows[0]["currentLivingAddress"].ToString().Trim() + "\",";
                //新增抵押城市
                param += "\"plege_address_city\":\"" + dt.Rows[0]["codename"].ToString().Trim() + "\",";
                param += "\"reside_address_province\":\"" + dt.Rows[0]["provinceName"].ToString().Trim() + "\",";
                param += "\"nationality\":\"" + dt.Rows[0]["CitizenshipName"].ToString().Trim() + "\",";
                param += "\"nation\":\"" + dt.Rows[0]["NationName"].ToString().Trim() + "\",";
                param += "\"education_level\":\"" + dt.Rows[0]["EducationName"].ToString().Trim() + "\",";
                param += "\"marital_status\":\"" + dt.Rows[0]["MaritalStatusName"].ToString().Trim() + "\",";
                param += "\"marital_status_check\":\"\",";
                param += "\"house_status\":\"" + dt.Rows[0]["propertytypeName"].ToString().Trim() + "\",";
                param += "\"employer\":\"" + dt.Rows[0]["ZjrCompanyName"].ToString().Trim() + "\",";
                param += "\"employer_telphone\":\"" + dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
                param += "\"employer_address_province\":\"" + dt.Rows[0]["comapnyProvinceName"].ToString().Trim() + "\",";
                param += "\"employer_address_city\":\"" + dt.Rows[0]["companyCityName"].ToString().Trim() + "\",";
                param += "\"occupation\":\"" + dt.Rows[0]["positionName"].ToString().Trim() + "\",";
                param += "\"working_years\":\"" + dt.Rows[0]["timeinyear"].ToString().Trim() + "\",";
                param += "\"income_month\":\"" + dt.Rows[0]["ActualSalary"].ToString().Trim() + "\",";
                param += "\"mate_name\":\"" + dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
                param += "\"mate_cert_type\":\"" + dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
                param += "\"mate_cert_no\":\"" + dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
                param += "\"mate_mobile\":\"" + dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";
                param += "\"partner_name\":\"" + dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
                param += "\"partner_cert_type\":\"" + dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
                param += "\"partner_cert_no\":\"" + dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
                param += "\"partner_moblie\":\"" + dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";

                //判断内外网客户
                //param += "\"customer_source\":\"" + (dt.Rows[0]["userName"].ToString().StartsWith("98") ? "内网" : "外网") + "\",";
                string nww = "";
                if (dt.Rows[0]["userName"].ToString().StartsWith("98") || dt.Rows[0]["userName"].ToString().StartsWith("80000"))
                {
                    nww = "内网";
                }
                else
                {
                    nww = "外网";
                }
                param += "\"customer_source\":\"" + nww + "\",";
                param += "\"business_type\":\"" + dt.Rows[0]["assetConditionName"].ToString().Trim() + "\",";
                param += "\"fours_number\":\"" + dt.Rows[0]["userName"].ToString().Trim() + "\",";
                param += "\"dealer_name\":\"" + dt.Rows[0]["dealerName"].ToString().Trim() + "\",";
                param += "\"vehicle_model\":\"" + dt.Rows[0]["brandName"].ToString().Trim() + "\",";
                param += "\"vehicle_brand\":\"" + dt.Rows[0]["assetMakeName"].ToString().Trim() + "\",";
                param += "\"vehicle_purpose\":\"" + dt.Rows[0]["purposeName"].ToString().Trim() + "\",";
                param += "\"vehicle_price\":\"" + dt.Rows[0]["vehicleprice"].ToString().Trim() + "\",";
                param += "\"application_amount\":\"" + dt.Rows[0]["financedamount"].ToString().Trim() + "\",";
                param += "\"application_term\":\"" + dt.Rows[0]["termMonth"].ToString().Trim() + "\",";
                param += "\"product_group\":\"" + dt.Rows[0]["productGroupName"].ToString().Trim() + "\",";
                param += "\"product_type\":\"" + dt.Rows[0]["productTypeName"].ToString().Trim() + "\",";
                param += "\"first_payments_ratio\":\"" + dt.Rows[0]["downpaymentrate"].ToString().Trim() + "\",";
                param += "\"first_payments_amount\":\"" + dt.Rows[0]["downpaymentamount"].ToString().Trim() + "\",";
                param += "\"vehicle_identification_value\":\"" + dt.Rows[0]["assetPrice"].ToString().Trim() + "\",";
                param += "\"if_partner\":\"\",";
                param += "\"if_partner_relatives\":\"" + dt.Rows[0]["GjrelationShipName"].ToString().Trim() + "\",";

                //客户现有银行号 [Caccountnum]
                param += "\"bank_no\":\"" + dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";
                //判断共借人是否是配偶
                param += "\"if_partner_mate\":\"" + (dt.Rows[0]["GjrelationShipName"].ToString().Contains("配偶") ? "是" : "否") + "\",";
                //是否隐藏配偶
                param += "\"if_hidden_partner\":\"" + dt.Rows[0]["IS_INACTIVE_IND"].ToString().Trim() + "\",";

                ////判断申请人是否有存量的简化贷
                //param += "\"if_stock_simplified_loan\":\"" + GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString().Trim()) + "\",";
                param += "\"if_stock_simplified_loan\":\"" + GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim()) + "\",";
                //判断是否包括附加费贷款（是/否）
                param += "\"if_extra_charge_loan\":\"" + (dt.Rows[0]["accessoriesInd"].ToString().Trim()) + "\",";
                param += "\"if_rereq_complete\":\"是\",";
                //获取敞口值：申请人+配偶+本次贷款金额
                param += "\"exposure_value\":\"" + indvidualHistoryFinancedAmount(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim(), dt.Rows[0]["financedamount"].ToString().Trim()) + "\",";
                param += "\"if_quality_dealer_code\":\"" + yzjxs(dt.Rows[0]["userName"].ToString().Trim()) + "\",";
                param += "\"if_mortgage\":\"" + dt.Rows[0]["lienee"].ToString().Trim() + "\",";
                param += "\"vehicle_manufacture_date\":\"" + dt.Rows[0]["releaseDate"].ToString().Trim() + "\",";
                param += "\"vehicle_kilometers\":\"" + dt.Rows[0]["odometer"].ToString().Trim() + "\",";
                param += "\"if_data_complete\":\"是\",";
                param += "\"cym\":\"" + dt.Rows[0]["FormerName"].ToString().Trim() + "\",";
                param += "\"csdssx\":\"" + dt.Rows[0]["birthpalaceprovince"].ToString().Trim() + "\",";
                param += "\"jgssx\":\"" + dt.Rows[0]["nativedistrict"].ToString().Trim() + "\",";
                param += "\"ssssxq\":\"" + dt.Rows[0]["hukouprovinceName"].ToString().Trim() + dt.Rows[0]["hukoucityName"].ToString().Trim() + "\",";
                param += "\"zz\":\"" + dt.Rows[0]["currentLivingAddress"].ToString().Trim() + "\",";
                param += "\"partner_cym\":\"" + dt.Rows[0]["GjFormerName"].ToString().Trim() + "\",";
                param += "\"partner_xb\":\"" + dt.Rows[0]["GjgenderName"].ToString().Trim() + "\",";
                param += "\"partner_mz\":\"" + dt.Rows[0]["GjNationName"].ToString().Trim() + "\",";
                param += "\"partner_csrq\":\"" + dt.Rows[0]["GjDateOfBirth"].ToString().Trim() + "\",";
                param += "\"partner_whcd\":\"" + dt.Rows[0]["GjEducationName"].ToString().Trim() + "\",";
                param += "\"partner_hyzk\":\"" + dt.Rows[0]["GjMaritalStatusName"].ToString().Trim() + "\",";
                param += "\"partner_fwcs\":\"" + dt.Rows[0]["GjcompanyName"].ToString().Trim() + "\",";
                param += "\"partner_csdssx\":\"" + dt.Rows[0]["Gjbirthpalaceprovince"].ToString().Trim() + "\",";
                param += "\"partner_jgssx\":\"" + dt.Rows[0]["Gjnativedistrict"].ToString().Trim() + "\",";
                param += "\"partner_ssssxq\":\"" + dt.Rows[0]["GjhukouprovinceName"].ToString().Trim() + dt.Rows[0]["GjhukoucityName"].ToString().Trim() + "\",";
                param += "\"partner_zz\":\"" + dt.Rows[0]["GjcurrentLivingAddress"].ToString().Trim() + "\",";
            }
            param += "\"if_guarantee\":\"\",";
            param += "\"guarantee_name\":\"" + dt.Rows[0]["DbThaiFirstName"].ToString().Trim() + "\",";
            param += "\"guarantee_cert_type\":\"" + dt.Rows[0]["DbIdCarTypeName"].ToString().Trim() + "\",";
            param += "\"guarantee_cert_no\":\"" + dt.Rows[0]["DbIdCardNo"].ToString().Trim() + "\",";
            param += "\"guarantee_mobile\":\"" + dt.Rows[0]["DbphoneNo"].ToString().Trim() + "\",";
            param += "\"guarantee_cym\":\"" + dt.Rows[0]["DbFormerName"].ToString().Trim() + "\",";
            param += "\"guarantee_xb\":\"" + dt.Rows[0]["DbgenderName"].ToString().Trim() + "\",";
            if ((lx != "3"))
            {
                param += "\"guarantee_mz\":\"" + dt.Rows[0]["DbNationName"].ToString().Trim() + "\",";

            }
            else
            {
                param += "\"guarantee_mz\":\"" + dt.Rows[0]["DbNationCode"].ToString().Trim() + "\",";
            }
            param += "\"guarantee_csrq\":\"" + dt.Rows[0]["DbDateOfBirth"].ToString().Trim() + "\",";
            param += "\"guarantee_whcd\":\"" + dt.Rows[0]["DbEducationName"].ToString().Trim() + "\",";
            param += "\"guarantee_hyzk\":\"" + dt.Rows[0]["DbMaritalStatusName"].ToString().Trim() + "\",";
            param += "\"guarantee_fwcs\":\"" + dt.Rows[0]["DbcompanyName"].ToString().Trim() + "\",";
            param += "\"guarantee_csdssx\":\"" + dt.Rows[0]["Dbbirthpalaceprovince"].ToString().Trim() + "\",";
            param += "\"guarantee_jgssx\":\"" + dt.Rows[0]["Dbnativedistrict"].ToString().Trim() + "\",";
            param += "\"guarantee_ssssxq\":\"" + dt.Rows[0]["DbhukouprovinceName"].ToString().Trim() + dt.Rows[0]["DbhukoucityName"].ToString().Trim() + "\",";
            param += "\"guarantee_zz\":\"" + dt.Rows[0]["DbcurrentLivingAddress"].ToString().Trim() + "\",";
            //新增经销商城市
            param += "\"dealer_address_city\":\"" + dt.Rows[0]["dealercityName"].ToString().Trim() + "\"";
            param += "}}}";
            this.Engine.LogWriter.Write("调用融数风控系统开始！");
            this.Engine.LogWriter.Write("param参数值：" + param);
            //推送数据给融数
            var result = "";
            try
            {
                result = PostMoths(url, param);
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("融数风控调用出错" + ex.Message.ToString());
            }
            this.Engine.LogWriter.Write(result.ToString());
            return result.ToString();

        }
        public string getSchemaCodeByInstanceID(string instanceid)
        {
            string SchemaCode = "";
            string sql = "select BIZOBJECTSCHEMACODE from H3.OT_instancecontext WHERE OBJECTID='" + instanceid + "'";
            SchemaCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            return SchemaCode;
        }
        /// <summary>
        /// 是否有存量的简化贷
        /// </summary>
        /// <param name="IdCardNo"></param>
        /// <returns></returns>
        public string GetJHDBySQR(string IdCardNo, string applicationno)
        {
            string sql1 = "";
            //CAP_EXPOSURE根据身份证号和申请号查敞口信息 简化贷
            sql1 += "  select sum(CT) total ,exp_applicant_card_id from ( ";
            //sql1 += " select round(ce.net_financed_amt) ct,to_char(ce.exp_application_number) exp_application_number,to_char(ce.application_status_cde) application_status_cde,to_char(ce.exp_applicant_card_id) exp_applicant_card_id,";
            //sql1 += " to_char(ce.exp_applicant_name) exp_applicant_name,to_char(fpg.fp_group_nme) fp_group_nme";
            //sql1 += " from CAP_EXPOSURE ce";
            //sql1 += " join contract_detail cd on cd.application_number=ce.application_number";
            //sql1 += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id";
            //sql1 += " where 1=1 ";
            //sql1 += " and ce.is_exposed_ind='T'";
            //sql1 += " and ce.exp_applicant_card_id='" + IdCardNo + "'";//'44520219800129241X'";
            //sql1 += " and ce.application_number='" + applicationno + "'";
            //sql1 += " and fpg.fp_group_id in (20,7,8,10,11,12,13,14,15,16,17,18)";
            //sql1 += " union all";
            //CMS_EXPOSURE根据身份证号和申请号查敞口信息 简化贷
            sql1 += " select  round(cme.principle_outstanding_amt) ct,to_char(cme.application_number) exp_application_number ,to_char(cme.identification_code) application_status_cde,to_char(cme.exp_applicant_card_id) exp_applicant_card_id,";
            sql1 += " to_char(cme.exp_applicant_name) exp_applicant_name,to_char(fpg.fp_group_nme) fp_group_nme";
            sql1 += " from CMS_EXPOSURE cme ";
            sql1 += " JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number";
            sql1 += " join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id ";
            sql1 += " where 1=1";
            sql1 += " and cme.application_number='" + applicationno + "'";
            sql1 += " and IS_EXPOSED_IND = 'T'";
            sql1 += " and cme.exp_applicant_card_id='" + IdCardNo + "'";//'620102196707275317'";
            sql1 += " and fpg.fp_group_id in (20,7,8,10,11,12,13,14,15,16,17,18)";
            sql1 += " ) group by exp_applicant_card_id";
            DataTable dt = ExecuteDataTableSql("cap", sql1);
            string rtn = "";
            if (CommonFunction.hasData(dt) && dt.Rows[0][0].ToString() != "0")
            {
                rtn = "是";
            }
            else
            {
                rtn = "否";
            }
            return rtn;
        }
        /// <summary>
        /// 判断是否是优质经销商
        /// </summary>
        /// <param name="jxscode"></param>
        /// <returns></returns>
        public string yzjxs(string jxscode)
        {
            int rtn = 0;
            string sql = string.Format("select count(1) from I_YZJXS jxs left join ot_user us on jxs.jxsname = us.appellation where us.code = '{0}'", jxscode);
            rtn = Convert.ToInt32(this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
            return rtn == 0 ? "否" : "是";
        }

        /// <summary>
        /// 个人贷款：获取根据申请人（包含配偶）敞口信息
        /// </summary>
        /// <param name="sqr"></param>
        /// <returns></returns>
        public string indvidualHistoryFinancedAmount(string IdCardNo, string applicationno, string currentV)
        {
            string rtn = "0";
            //string sql = "select sum(financedamount)+(" + currentV + ") from H3.I_RETAILAPP where IdCardNo ='" + IdCardNo + "' or IdCardNo= (select GjIdCardNo from H3.I_RETAILAPP where applicationno = '" + applicationno + "')";
            string sql = "select sum(counter) from (select NVL(sum(financedamount),0)+(" + currentV + ") counter  from H3.I_RETAILAPP where  (applicationno != '" + applicationno + "' and IdCardNo ='" + IdCardNo + "') "
                        + " or IdCardNo = (select GjIdCardNo from H3.I_RETAILAPP where rownum = 1 and applicationno = '" + applicationno + "')"
                        + " union all"
                        + " select NVL(sum(financedamount),0) counter from H3.I_HRETAILAPP where IdCardNo = '" + IdCardNo + "'"
                        + " or IdCardNo = (select GjIdCardNo from H3.I_HRETAILAPP where rownum = 1 and applicationno = '" + applicationno + "'))";
            rtn = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
            return rtn;
        }
        /// <summary>
        /// 获取个人贷款申请人和配偶的贷款申请额度
        /// </summary>
        /// <param name="sqr"></param>
        /// <returns></returns>
        public string getIndvidualApplicationHistory(string IdCardNo, string applicationno)
        {
            //string sql = "select applicationNo,ThaiFirstName,financedamount from H3.I_RETAILAPP where IdCardNo ='" + IdCardNo + "' or IdCardNo = (select GjIdCardNo from H3.I_RETAILAPP where applicationno = '" + applicationno + "')";
            string sql = "select a.*,b.objectid instanceid from (select 'RetailApp' applicationType,objectid,applicationNo,ThaiFirstName,financedamount from H3.I_RETAILAPP where  (applicationno != '" + applicationno + "' and IdCardNo ='" + IdCardNo + "') "
                        + " or IdCardNo = (select GjIdCardNo from H3.I_RETAILAPP where rownum = 1 and applicationno = '" + applicationno + "')"
                        + " union all"
                        + " select 'HRetailApp'applicationType,objectid,applicationNo,ThaiFirstName,financedamount from H3.I_HRETAILAPP where IdCardNo = '" + IdCardNo + "'"
                        + " or IdCardNo = (select GjIdCardNo from H3.I_HRETAILAPP where rownum = 1 and applicationno = '" + applicationno + "')) a,H3.OT_instancecontext b where a.objectid = b.bizobjectid";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return DataToJSON(dt);
        }
        public static string DataToJSON(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + EncodeStr(dt.Columns[j].ColumnName.ToString()) + "\":" + "\"" + (EncodeStr(dt.Rows[i][j].ToString())) + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + EncodeStr(dt.Columns[j].ColumnName.ToString()) + "\":" + "\"" + (EncodeStr(dt.Rows[i][j].ToString())) + "\"");
                        }
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("}");
                    }
                    else
                    {
                        JsonString.Append("},");
                    }
                }
                return "[" + JsonString.ToString() + "]";
            }
            else
            {
                return "{ }";
            }
        }
        public static string EncodeStr(string str)
        {
            str = str.Replace("\"", "");
            str = str.Replace("\\", "，");
            str = str.Replace("	", "");
            return str;
        }

        /// 零售信审审批和放款审批分单逻辑
        /// </summary>
        /// <param name="dep">部门</param>
        /// <param name="post">岗位</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public string BizAutoAssign(string instanceID, string activeCode, string cyz, string amount)
        {
            if (!(cyz == null || cyz == ""))
            {
                string lzsql = " select a.state,a.servicestate,b.name from OT_User a inner join Ot_Organizationunit b";
                lzsql += " on a.parentid=b.objectid  where (a.state=0 or a.servicestate=2) and  a.objectid='" + cyz + "'";
                DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(lzsql);
                if (dt == null || dt.Rows.Count < 1)
                {
                    return cyz;
                }
            }
            if (instanceID == null || instanceID == "")
            {
                this.Engine.LogWriter.Write("instanceid为空！");
                return "";
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceID:" + instanceID + "amount:" + amount);
            string userID = "", dep = "", post = "", defualtuser = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";
            string sql = " ";
            switch (activeCode)
            {
                //信贷初审 信贷终审  运营初审 运营终审
                case "信审初审": dep = "信贷审批部"; post = "初审"; break;
                case "信审终审": dep = "信贷审批部"; post = "终审"; break;
                case "运营初审": dep = "运营部"; post = "初审"; break;
                case "运营终审": dep = "运营部"; post = "终审"; break;
                case "运营预审": dep = "运营部"; post = "预审"; break;
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " instanceID：" + instanceID);
            if (dep.Contains("信贷"))
            {
                defualtuser = "9357bc42-e8c0-47d7-a985-6810b452e95f";//曲亮
                if (decimal.Parse(amount) > 900000 && post == "终审")//信审终审大于90万
                {
                    string[] arrayUser = getGroupUserByCondition("信审终审", "自动分单");
                    if (arrayUser != null && arrayUser.Length > 0)
                    {
                        userID = arrayUser[0];
                    }
                    else
                    {
                        userID = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";//发给管理员
                        this.Engine.LogWriter.Write("自动分单结果，instanceid：" + instanceID + "金额大于90万，未找到信审终审的人员");
                    }

                    return userID;
                }
                string type = "";
                DataTable dtInstanceData = getInstanceData(instanceID);
                if (dtInstanceData.Rows[0]["userName"].ToString().StartsWith("98") || dtInstanceData.Rows[0]["userName"].ToString().StartsWith("80000"))
                {
                    type = "内";
                }
                else
                {
                    type = "外";
                }
                sql += " select d.yh,d.xm,d.gw, case when sum(d.shxs) is null then 0 else sum(d.shxs) end counter from (";
                sql += " select  d.yh,d.xm,d.gw,d.shxs from ( ";
                sql += " SELECT ow1.PARTICIPANT,TO_CHAR (TRIM (OW1.PARTICIPANTNAME)) username,      case when  OW1.ACTIVITYCODE='Activity13' then '终审'         when  OW1.ACTIVITYCODE='Activity14' then '初审' end gw,           OW1.INSTANCEID                             FROM                               (SELECT OW.instanceid,ow.PARTICIPANT,                                        OW.PARTICIPANTNAME,                                       OW.ACTIVITYCODE,                                          OW.DISPLAYNAME,                                          OW.RECEIVETIME,                                          OW.FINISHTIME,                                           OW.ACTIONNAME,                              ROW_NUMBER ()                                           OVER (PARTITION BY OW.ACTIVITYCODE,OW.instanceid                                               ORDER BY OW.RECEIVETIME DESC) RN                             FROM                           ( (SELECT instanceid,PARTICIPANT,                                         PARTICIPANTNAME,                                           ACTIVITYCODE,                                          DISPLAYNAME,                                           RECEIVETIME,                                           FINISHTIME,                                          ACTIONNAME                                      FROM OT_Workitemfinished                                    WHERE     workflowCode IN ('RetailApp','CompanyApp','APPLICATION')                                         AND PARTICIPANTNAME <> '系统管理员'                                        AND TO_CHAR (RECEIVETIME, 'YYYYMMDD') >                                               TO_CHAR (SYSDATE-1, 'YYYYMMDD')                                        AND ACTIVITYCODE in ( 'Activity14'，'Activity13')                                        )                         UNION ALL                      (SELECT instanceid,PARTICIPANT,                                          PARTICIPANTNAME,                                         ACTIVITYCODE,                                            DISPLAYNAME,                                           RECEIVETIME,                                           FINISHTIME,                                            ACTIONNAME                                      FROM OT_Workitem                                   WHERE     workflowCode IN ('RetailApp','CompanyApp','APPLICATION')                                        AND PARTICIPANTNAME <> '系统管理员'                                        AND TO_CHAR (RECEIVETIME, 'YYYYMMDD') >                                                TO_CHAR (SYSDATE-1, 'YYYYMMDD')                                         AND ACTIVITYCODE in ( 'Activity14'，'Activity13')                                         ) ) OW) OW1";
                sql += "  WHERE OW1.RN = 1 ";
                sql += "  ) a right join I_xsyyshryxx d  on   d.yh=a.participant ";
                sql += " inner join  Ot_User e on e.objectid=d.yh";
                sql += "  where 1=1  ";
                sql += " and e.state=1 and e.servicestate=0  ";
                sql += " and d.nww like '%" + type + "%'";
                sql += " and d.zt='上班'";
                sql += " and d.gw='" + post + "'";
                sql += " and d.bm='" + dep + "'";
                sql += " and d.shedend >= " + amount + "";
                sql += " and d.shedstart <  " + amount + "";
                sql += " ) d ";
                sql += " group by d.yh,d.xm,d.gw ";
                sql += " order by counter asc";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "信贷 sql：" + sql);
            }
            else if (dep.Contains("运营"))
            {
                defualtuser = "eeefda92-05a5-4d3d-9fc4-3cf4643ccdf7";//陈炜
                sql += " select OBJECTID,NAME,sum(COUNTER) countor from (";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "'  and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEM d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp','APPLICATION') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " union all";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "' and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEMFINISHED d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp','APPLICATION') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " ) group by NAME,OBJECTID ORDER BY countor ASC ";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 运管sql：" + sql);
            }
            try
            {
                DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    userID = dt.Rows[0][0].ToString();
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 自动分单结果：" + userID);
                }
                else
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 未找到相关审核人员，自动分单将转给管理员：" + defualtuser);
                }
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 自动分单sql报错：" + ex.ToString());
            }
            return userID.Trim() != "" ? userID : defualtuser;
        }
        /// <summary>
        /// 根据组名称和上级部门名称获取组成员
        /// </summary>
        /// <param name="groupname"></param>
        /// <param name="unitname"></param>
        /// <returns></returns>
        public string[] getGroupUserByCondition(string groupname, string unitname)
        {
            string[] rtn = null;
            string sql = "select a.childid from Ot_Groupchild a inner join Ot_Group b on a.parentobjectid=b.objectid";
            sql += " inner join Ot_Organizationunit c on b.parentid=c.objectid";
            sql += " where b.name='" + groupname + "' and c.name='" + unitname + "'";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.AsEnumerable().Select(d => d.Field<string>("childid")).ToArray();
            }
            return rtn;
        }
        public string BizAutoAssignHistory(string instanceID, string activeCode, string cyz, string amount)
        {
            if (instanceID == null || instanceID == "")
            {
                this.Engine.LogWriter.Write("instanceid：" + instanceID);
                return "";
            }
            if (!(cyz == null || cyz == ""))
            {
                return cyz;
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceID:" + instanceID + "amount:" + amount);
            string userID = "", dep = "", post = "", defualtuser = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";
            string sql = " ";
            switch (activeCode)
            {
                //信贷初审 信贷终审  运营初审 运营终审
                case "信审初审": dep = "信贷审批部"; post = "初审"; break;
                case "信审终审": dep = "信贷审批部"; post = "终审"; break;
                case "运营初审": dep = "运营部"; post = "初审"; break;
                case "运营终审": dep = "运营部"; post = "终审"; break;
                case "运营预审": dep = "运营部"; post = "预审"; break;
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " instanceID：" + instanceID);
            if (dep.Contains("信贷"))
            {
                defualtuser = "6d081132-b4d9-4454-9cfd-fe11e79704da";//周磊
                string beginSql = "";
                string endSql = "";
                string type = "";
                DataTable dtInstanceData = getInstanceData(instanceID);
                //type = dtInstanceData.Rows[0]["userName"].ToString().StartsWith("98") ? "内" : "外";
                if (dtInstanceData.Rows[0]["userName"].ToString().StartsWith("98") || dtInstanceData.Rows[0]["userName"].ToString().StartsWith("80000"))
                {
                    type = "内";
                }
                else
                {
                    type = "外";
                }
                if (post == "终审")
                {
                    beginSql += "  select c.name,c.objectid,1 RATIO  from(";
                }
                else
                {
                    beginSql += "  select c.name,c.objectid,case  when e.financedamount < 500000 then 1  when e.financedamount >= 500000 then";
                    beginSql += "  1.5 else 0  end RATIO  from(";
                }
                //beginSql += "  select c.name,c.objectid,case  when e.financedamount < 500000 then 1  when e.financedamount >= 500000 then";
                //beginSql += "  1.5 else 0  end RATIO  from(";
                beginSql += "   select B.NAME, B.OBJECTID from H3.I_XSYYSHRYXX a";
                beginSql += "   inner join";
                beginSql += "   H3.OT_User b on A.zh = B.CODE";
                beginSql += "   where a.bm = '" + dep + "' and a.gw = '" + post + "'  and a.zt='上班'  AND A.nww  like '%" + type + "%'";
                if (post == "终审" && decimal.Parse(amount) < 500000)
                {
                    //beginSql += "   and a.shed > " + amount + " and b.code!='leonzhou'";
                    beginSql += "   and a.shed > " + amount + " and b.objectid!='" + defualtuser + "'";
                }
                else
                {
                    beginSql += "   and a.shed > " + amount + "";
                }
                beginSql += "    and b.objectid not in (select PARTICIPANT from";
                beginSql += "     H3.OT_WORKITEMFINISHED where instanceid = '" + instanceID + "')";
                beginSql += "     ) c ";
                //endSql += "     left join H3.OT_WORKITEM d on C.OBJECTID = D.PARTICIPANT and d.workflowcode = 'RetailApp'";
                endSql += "     and d.receivetime > to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00', 'yyyy-mm-dd hh24:mi:ss')";
                endSql += "     left join H3.I_" + dtInstanceData.Rows[0]["SchemaCode"] + " e on E.INSTANCEID = D.INSTANCEID ";
                sql += " select a.objectid,a.name,sum(a.ratio) counter from ( ";
                sql = sql + beginSql + " left join H3.OT_WORKITEM d on C.OBJECTID = D.PARTICIPANT and d.workflowcode = 'RetailApp' " + endSql;
                sql = sql + " union all " + beginSql + " left join H3.OT_WORKITEMFINISHED d on C.OBJECTID = D.PARTICIPANT and d.workflowcode = 'RetailApp' " + endSql;
                sql = sql + " union all " + beginSql + " left join H3.OT_WORKITEM d on C.OBJECTID = D.PARTICIPANT and d.workflowcode = 'COMPANYAPP' " + endSql;
                sql = sql + " union all " + beginSql + " left join H3.OT_WORKITEMFINISHED d on C.OBJECTID = D.PARTICIPANT and d.workflowcode = 'COMPANYAPP' " + endSql;
                sql += ") a group by a.objectid,a.name order by counter asc";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "信贷 sql：" + sql);
            }
            else if (dep.Contains("运营"))
            {
                defualtuser = "eeefda92-05a5-4d3d-9fc4-3cf4643ccdf7";//陈炜
                sql += " select OBJECTID,NAME,sum(COUNTER) countor from (";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "'  and a.zt='上班'   and b.objectid not in (";
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEM d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " union all";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "' and a.zt='上班'  and b.objectid not in (";
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEMFINISHED d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " ) group by NAME,OBJECTID ORDER BY countor ASC ";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 运管sql：" + sql);
            }
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                userID = dt.Rows[0][0].ToString();
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 自动分单结果：" + userID);
            }
            return userID.Trim() != "" ? userID : defualtuser;
        }
        public string BizAssign(string objectids)
        {
            string rtn = "false";
            string cyz = "", workflowcode = "";
            string sql = " select a.objectid,a.PARTICIPANT,a.workflowcode,a.activitycode,a.displayname,b.bizobjectid from h3.ot_workitem a,h3.ot_instancecontext b where a.instanceid=b.objectid and a.objectid in (" + objectids + ")";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["workflowcode"].ToString().Contains("RetailApp"))
                    {
                        workflowcode = "RetailApp";//一期个人贷款
                    }
                    else if (row["workflowcode"].ToString().Contains("CompanyApp"))
                    {
                        workflowcode = "CompanyApp";//一期机构贷款
                    }
                    else if (row["workflowcode"].ToString().Contains("APPLICATION"))
                    {
                        workflowcode = "APPLICATION";//二期贷款
                    }
                    else
                    {
                        this.Engine.LogWriter.Write(DateTime.Now.ToString() + "手动分单失败，流程编码错误：" + row["workflowcode"].ToString());
                        continue;
                    }
                    if (row["displayname"].ToString().Contains("信审（初审）"))
                    {
                        cyz = "cyzXSCS";//信审初审
                    }
                    else if (row["displayname"].ToString().Contains("信审（终审）"))
                    {
                        cyz = "cyzXSZS";//信审终审
                    }
                    else if (row["displayname"].ToString().Contains("运营人员（初审）"))
                    {
                        cyz = "cyzYYCS";//运营初审
                    }
                    else if (row["displayname"].ToString().Contains("运营人员（终审）"))
                    {
                        cyz = "cyzYYZS";//运营终审
                    }
                    else
                    {
                        this.Engine.LogWriter.Write(DateTime.Now.ToString() + "任务" + row["objectid"].ToString() + "手动分单失败，节点显示映射出错：" + row["displayname"].ToString());
                        continue;
                    }

                    //sql = " update  h3.I_" + workflowcode + " set " + cyz + "='" + row["PARTICIPANT"].ToString() + "'  where objectid ='" + row["bizobjectid"].ToString() + "'";
                    //this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                    List<DataItemParam> keyValues = new List<DataItemParam>();
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = cyz,
                        ItemValue = row["PARTICIPANT"].ToString()
                    });
                    SetItemValues(workflowcode, row["bizobjectid"].ToString(), keyValues, "18f923a7-5a5e-426d-94ae-a55ad1a4b239");
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + "任务" + row["objectid"].ToString() + "手动分单成功：" + row["displayname"].ToString());
                }
            }
            rtn = "true";
            return rtn;
        }

        public string PostMoths(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            request.Timeout = 5000;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            //if (response.StatusCode == HttpStatusCode.OK)
            //{

            //}
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

        /// <summary>
        /// 设置批量流程数据项的值
        /// </summary>
        /// <param name="bizObjectSchemaCode"></param>
        /// <param name="bizObjectId"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public bool SetItemValues(string bizObjectSchemaCode, string bizObjectId, List<DataItemParam> keyValues, string userid)
        {
            if (keyValues == null || keyValues.Count == 0) return false;
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (DataItemParam param in keyValues)
            {
                values.Add(param.ItemName, param.ItemValue);
            }
            return this.Engine.BizObjectManager.SetPropertyValues(bizObjectSchemaCode, bizObjectId, userid, values);
        }
        /// <summary>
        /// 提交(已阅)工作任务
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public bool SubmitWorkItem(string workItemId, string commentText)
        {
            // ValidateSoapHeader();
            // 获取操作的用户

            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            SubmitItem(workItemId, OThinker.Data.BoolMatchValue.True, commentText, this.Engine.Organization.GetUserByCode("administrator").ObjectID);//UserValidator.UserID);
            return true;
        }
        /// <summary>
        /// 融数自动审批通过判断
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public int AutomaticApprovalByRongShu(string instanceid)
        {
            int rtn = 0;
            //二手车不走自动审批
            InstanceData data = new InstanceData(this.Engine, instanceid, "");
            if (data["assetConditionName"].Value != null && data["assetConditionName"].Value.ToString().Contains("二手车"))
            {
                rtn = 0;
                return rtn;
            }
            //公牌贷的时候不走自动审批
            if (data["applicant_type"].Value != null && data["applicant_type"].Value.ToString().Trim() != "")
            {
                rtn = 0;
                return rtn;
            }
            //只要驳回过的流程，不走自动审批
            string sql = string.Format("select count(1) from Ot_Workitemfinished where instanceid='{0}' and actioneventtype=3", instanceid);
            int num = Convert.ToInt32(OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
            if (num == 0)
            {
                rtn = 1;
            }
            else
            {
                rtn = 0;
                return rtn;
            }
            string workItemId = getRSWorkItemIDByInstanceid(instanceid);
            if (CommonFunction.hasData(workItemId))
            {
                #region 个人收入负债测算
                try
                {
                    this.Engine.LogWriter.Write("客户收入负债测算开始");
                    double dzcarloan = 0;
                    double qs = double.Parse(data["termMonth"].Value.ToString());
                    double financedamount = double.Parse(data["financedamount"].Value.ToString());
                    double myhk = financedamount / qs;
                    WorkPcmFunction wpf = new WorkPcmFunction();

                    if (data["GjThaiFirstName"].Value == null || data["GjThaiFirstName"].Value.ToString() == "")
                    {
                        dzcarloan = myhk * 0.7;
                    }
                    else
                    {
                        dzcarloan = myhk;
                    }
                    string stream = wpf.GetCustomerInfo(data["IdCardNo"].Value.ToString(), dzcarloan.ToString());
                    this.Engine.LogWriter.Write("客户收入负债测算回传值：" + stream);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var gcresult = js.Deserialize<hknl_result>(stream);
                    if (gcresult != null && gcresult.Result == "Success")
                    {
                        this.Engine.LogWriter.Write("客户收入负债测算：每期还款额度为" + myhk + "，客户还款能力为" + gcresult.C_RepayLoan);
                        //数据保存
                        double srfzc = double.Parse(gcresult.C_RepayLoan) - myhk;
                        hknl_model md = new hknl_model();
                        md.APPNO = data["applicationno"].Value.ToString();
                        md.ZJR = data["IdCardNo"].Value.ToString();
                        md.ZJR_KHYHKNL = gcresult.C_RepayLoan;
                        md.ZJR_KHZCGZ = gcresult.C_AssetValuation;
                        md.ZJR_YSRGZ = gcresult.C_IncomOfMonth;
                        md.ZJR_YYHZW = gcresult.C_DabtsOfMonth;
                        md.QS = qs.ToString();
                        md.DKJE = financedamount.ToString();
                        md.SRFZC = srfzc.ToString("f2");
                        inserthknl(md);
                        if (srfzc < 0)
                        {
                            //客户收入负债差小于0
                            rtn = 0;
                            return rtn;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Engine.LogWriter.Write("客户收入负债测算发生异常：" + ex.Message);
                }
                #endregion
            }
            return rtn;

        }
        /// <summary>
        /// 融数风控提交工作任务
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public bool SubmitWorkItemByRongShu(string instanceid, string commentText, string userid)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 融数风控节点提交开始，流程实例ID：" + instanceid);
            string workItemId = getRSWorkItemIDByInstanceid(instanceid);
            if (!CommonFunction.hasData(workItemId))
            {
                //日志记录
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 融数风控节点提交失败，未查找到workItemId，流程实例ID：" + instanceid);
                return false;
            }
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            SubmitItem(workItemId, OThinker.Data.BoolMatchValue.True, commentText, userid);//UserValidator.UserID);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 融数风控节点提交成功，流程实例ID：" + instanceid);
            return true;
        }
        /// <summary>
        /// 通过实例ID提交工作任务
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public bool SubmitWorkItemByInstanceID(string instanceid, string commentText)
        {
            string workItemId = getWorkItemIDByInstanceid(instanceid);
            if (!CommonFunction.hasData(workItemId))
            {
                //日志记录
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid提交任务失败，未找到任务ID，流程实例ID：" + instanceid);
                return false;
            }
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            SubmitItem(workItemId, OThinker.Data.BoolMatchValue.True, commentText, this.Engine.Organization.GetUserByCode("administrator").ObjectID);
            return true;
        }

        /// <summary>
        /// 提交工作项
        /// </summary>
        /// <param name="instanceid">流程实例id</param>
        /// <param name="UserCode">提交人</param>
        public void SubmitItem(string instanceid, string UserCode)
        {
            #region 结束当前任务
            string sql = "SELECT ObjectID FROM OT_WorkItem WHERE InstanceId='{0}' ORDER BY TokenId desc";
            sql = string.Format(sql, instanceid);
            string workItemId = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            var user = this.Engine.Organization.GetUserByCode(UserCode + string.Empty);
            if (workItemId != "")
            {
                // 获取工作项
                OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);

                // 结束工作项
                this.Engine.WorkItemManager.FinishWorkItem(
                    item.ObjectID,
                    user.UnitID,
                    OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                    null,
                    OThinker.Data.BoolMatchValue.True,
                    string.Empty,
                    null,
                    OThinker.H3.WorkItem.ActionEventType.Forward,
                    11);

                // 需要通知实例事件管理器结束事件
                AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                        MessageEmergencyType.Normal,
                        item.InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.True,
                        false,
                        OThinker.Data.BoolMatchValue.True,
                        true,
                        null);
                this.Engine.InstanceManager.SendMessage(endMessage);
            }
            #endregion
        }
        /// <summary>
        /// 提交工作项
        /// </summary>
        /// <param name="workItemId">工作项ID</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        /// <param name="userId">处理人</param>
        private void SubmitItem(string workItemId, OThinker.Data.BoolMatchValue approval, string commentText, string userId)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext instance = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // 添加意见
            this.AppendComment(instance, item, OThinker.Data.BoolMatchValue.Unspecified, commentText);

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                userId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                null,
                approval,
                commentText,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                (int)OThinker.H3.Controllers.SheetButtonType.Submit);
            // 需要通知实例事件管理器结束事件
            OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    approval,
                    false,
                    approval,
                    true,
                    null);
            this.Engine.InstanceManager.SendMessage(endMessage);
        }
        /// <summary>
        /// 给工作项添加审批意见
        /// </summary>
        /// <param name="item">工作项</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        private void AppendComment(OThinker.H3.Instance.InstanceContext Instance, OThinker.H3.WorkItem.WorkItem item, OThinker.Data.BoolMatchValue approval, string commentText)
        {
            // 添加一个审批意见
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetPublishedTemplate(
                item.WorkflowCode,
                item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(item.WorkflowCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                OThinker.H3.Data.Comment comment = new OThinker.H3.Data.Comment();
                comment.Activity = item.ActivityCode;
                comment.Approval = approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = item.InstanceId;
                comment.BizObjectId = Instance.BizObjectId;
                comment.BizObjectSchemaCode = Instance.BizObjectSchemaCode;
                comment.OUName = this.Engine.Organization.GetName(this.Engine.Organization.GetParent(item.Participant));
                comment.Text = commentText;
                comment.TokenId = item.TokenId;
                comment.UserID = item.Participant;

                // 设置用户的默认签章
                OThinker.Organization.Signature[] signs = this.Engine.Organization.GetSignaturesByUnit(item.Participant);
                if (signs != null && signs.Length > 0)
                {
                    foreach (OThinker.Organization.Signature sign in signs)
                    {
                        if (sign.IsDefault)
                        {
                            comment.SignatureId = sign.ObjectID;
                            break;
                        }
                    }
                }
                this.Engine.BizObjectManager.AddComment(comment);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public bool finishedInstance(string instanceId)
        {
            try
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程开始，流程实例ID：" + instanceId);
                OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(instanceId);
                if (context == null) return false;

                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetDefaultWorkflow(context.WorkflowCode);

                OThinker.H3.Messages.ActivateActivityMessage activateMessage = new OThinker.H3.Messages.ActivateActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.High,
                        instanceId,
                        workflow.EndActivityCode,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null,
                        null,
                        false,
                        OThinker.H3.WorkItem.ActionEventType.Adjust
                    );
                this.Engine.InstanceManager.SendMessage(activateMessage);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程成功，流程实例ID：" + instanceId);
                return true;
            }
            catch
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程失败，流程实例ID：" + instanceId);
                return false;
            }
        }
        public bool ActiveToken(string instanceId, string activityCode, string[] participants)
        {
            try
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 激活流程节点开始，流程实例ID：" + instanceId);
                // 准备触发后面Activity的消息
                OThinker.H3.Messages.ActivateActivityMessage activateMessage
                    = new OThinker.H3.Messages.ActivateActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        instanceId,
                        activityCode,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        participants,
                        null,
                        false,
                        OThinker.H3.WorkItem.ActionEventType.Adjust);
                this.Engine.InstanceManager.SendMessage(activateMessage);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 激活流程节点结束，流程实例ID：" + instanceId);
                return true;
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 激活流程节点异常，流程实例ID：" + instanceId + "，异常信息：" + ex.Message);
                return false;
            }
        }
        public string ReturnWorkItemByInstanceid(string instanceid, string userId, string commentText)
        {
            bool rtn = false;
            try
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点开始，流程实例ID：" + instanceid);
                //string userId = "ac845887-3bde-4580-ae0e-fe74149465e2";//融数风控
                string workItemId = getWorkItemIDByInstanceid(instanceid);
                OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
                //手工节点不允许驳回
                if (item != null && item.ItemType == OThinker.H3.WorkItem.WorkItemType.Fill) return rtn.ToString();
                rtn = ReturnItem(userId, workItemId, commentText);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点结束，流程实例ID：" + instanceid);
                return rtn.ToString();

            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点异常，流程实例ID：" + instanceid + "，异常信息：" + ex.Message);
                return rtn.ToString();
            }
        }
        private bool ReturnItem(string userId, string workItemId, string commentText)
        {
            OThinker.Organization.User user = this.Engine.Organization.GetUnit(userId) as OThinker.Organization.User;
            if (user == null) return false;
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // ToKen
            OThinker.H3.Instance.IToken Token = context.GetToken(item.TokenId);
            if (Token.PreTokens == null) return false;
            int PreToken = int.Parse(Token.PreTokens[0].ToString());
            OThinker.H3.Instance.IToken PreToken1 = context.GetToken(PreToken);
            string activityName = PreToken1.Activity;
            // 添加意见
            this.AppendComment(context, item, OThinker.Data.BoolMatchValue.False, commentText);
            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                  item.ObjectID,
                  user.ObjectID,
                  OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                  null,
                  OThinker.Data.BoolMatchValue.False,
                  commentText,
                  null,
                  OThinker.H3.WorkItem.ActionEventType.Backward,
                  (int)OThinker.H3.Controllers.SheetButtonType.Return);
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                activityName,
                OThinker.H3.Instance.Token.UnspecifiedID,
                null,
                new int[] { item.TokenId },
                false,
                OThinker.H3.WorkItem.ActionEventType.Backward);

            // 通知该Activity已经完成
            OThinker.H3.Messages.AsyncEndMessage endMessage =
                new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.False,
                    true,
                    OThinker.Data.BoolMatchValue.False,
                    false,
                    activateMessage);
            this.Engine.InstanceManager.SendMessage(endMessage);
            return true;
        }
        /// <summary>
        /// 添加组织架构
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        public string AddMyUnit(string BizObjectID, string userid)
        {
            string bm = "", unitid = "";
            string sql = "select * from H3.I_RYXXBIMPORT where parentobjectid='" + BizObjectID + "'";
            DataTable dt = new DataTable();
            dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            int count = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bm = dt.Rows[i]["bm"].ToString().Trim();
                if (bm.Trim() == "")
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 组织架构导入部门不能为空！");
                    break;
                }
                var result = OThinker.Organization.HandleResult.SUCCESS;
                string orgSql = "select objectid from H3.OT_ORGANIZATIONUNIT where name='" + bm + "'";
                object obj = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(orgSql);
                unitid = obj == null ? "" : obj.ToString();
                if (!CommonFunction.hasData(unitid))
                {
                    OThinker.Organization.OrganizationUnit OrgUnit = new OThinker.Organization.OrganizationUnit();
                    OrgUnit.ParentID = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("select objectid from H3.OT_ORGANIZATIONUNIT where isrootunit = 1").ToString();
                    OrgUnit.Name = bm;
                    OrgUnit.ObjectID = Guid.NewGuid().ToString();
                    OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(userid, OrgUnit);
                    unitid = OrgUnit.ObjectID;
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 组织架构导入部门：" + bm);
                }
                var parentou = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(unitid);
                OThinker.Organization.User userselect = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUserByCode(dt.Rows[i]["zh"].ToString().Trim());
                if (userselect != null && userselect.Code != "")
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " ：员工" + dt.Rows[i]["xm"].ToString().Trim() + "已经存在，导入失败");
                    continue;
                }
                var unitUser = new OThinker.Organization.User();
                unitUser.ObjectID = Guid.NewGuid().ToString();
                unitUser.Code = dt.Rows[i]["zh"].ToString().Trim();
                unitUser.Name = dt.Rows[i]["xm"].ToString().Trim();
                unitUser.ParentID = parentou == null ? OThinker.H3.Controllers.AppUtility.Engine.Organization.Company.UnitID : parentou.ObjectID;
                //unitUser.EmployeeNumber = employeenumber;
                unitUser.Mobile = dt.Rows[i]["dh"].ToString().Trim();
                //unitUser.Email = "18363847@qq.com";
                unitUser.CreatedTime = DateTime.Now;
                // 写入服务器 
                result = OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(null, unitUser);
                count += 1;
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 组织架构导入员工：" + unitUser.Name + "导入成功");
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 组织架构导入员工共计：" + count + "人");
            return count.ToString();
        }
       public bool GetIsFinished(string InstanceId)
        {
            InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
            return ic.IsFinished;
        }
       public DataTable gethknl()
       {
           string rtn = "";
           //string sql = "select * from DZ_HKNL where BZ1 is not null ";
           string sql = "select a.* from dz_hknl a where a.bz1 not in (select appno from dz_hknl_result where appno is not null) and a.bz1 is not null ";
           DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
           return dt;
       }
       public int inserthknl(string APPNO, string ZJR, string GJR, string DKJE, string QS, string ZJR_YSRGZ, string ZJR_YYHZW, string ZJR_KHZCGZ, string ZJR_KHYHKNL, string GJR_YSRGZ, string GJR_YYHZW, string GJR_KHZCGZ, string GJR_KHYHKNL)
       {
           int rtn = 0;
           string sql = " INSERT INTO DZ_HKNL_RESULT(APPNO,ZJR,GJR,DKJE,QS,ZJR_YSRGZ,ZJR_YYHZW,ZJR_KHZCGZ,ZJR_KHYHKNL,GJR_YSRGZ,GJR_YYHZW,GJR_KHZCGZ,GJR_KHYHKNL) ";
           sql += " VALUES ";
           sql += " ('" + APPNO + "','" + ZJR + "','" + GJR + "'," + DKJE + "," + QS + "," + ZJR_YSRGZ + "," + ZJR_YYHZW + "," + ZJR_KHZCGZ + "," + ZJR_KHYHKNL + "," + GJR_YSRGZ + "," + GJR_YYHZW + "," + GJR_KHZCGZ + "," + GJR_KHYHKNL + ") ";
           rtn = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
           return rtn;
       }
       public int inserthknl(hknl_model model)
        {
            int rtn = 0;
            string sql = " INSERT INTO DZ_HKNL_RESULT(APPNO,ZJR,GJR,DKJE,QS,ZJR_YSRGZ,ZJR_YYHZW,ZJR_KHZCGZ,ZJR_KHYHKNL,GJR_YSRGZ,GJR_YYHZW,GJR_KHZCGZ,GJR_KHYHKNL,SRFZC,CJSJ) ";
            sql += " VALUES ";
            sql += " ('" + model.APPNO + "','" + model.ZJR + "','" + model.GJR + "'," + model.DKJE + "," + model.QS + "," + model.ZJR_YSRGZ + "," + model.ZJR_YYHZW + "," + model.ZJR_KHZCGZ + "," + model.ZJR_KHYHKNL + "," + model.GJR_YSRGZ + "," + model.GJR_YYHZW +"," + model.GJR_KHZCGZ + "," + model.GJR_KHYHKNL + "," + model.SRFZC + ",SYSDATE) ";
this.Engine.LogWriter.Write("客户收入负债测算数据保存sql：" + sql);
            rtn = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            return rtn;
        }

       /// <summary>
       /// 获取内外网经销商
       /// </summary>
       /// <param name="nww"></param>
       /// <returns></returns>
       public DataTable getjxs(string nww)
       {
           string sql = "select distinct appellation,parentid from Ot_User where  appellation is not null  ";
           if (nww == "内网")
           {
               sql += " and parentid ='28a0ba01-4f58-4097-96cb-77c2b09e8253'";
           }
           if (nww == "外网")
           {
               sql += " and parentid ='9b8eb0fb-31c2-4577-bea7-9e453812f863'";
           }
           sql += " order by parentid";
           DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
           return dt;
       }
       /// <summary>
       /// 获取经销商额度管理明细
       /// </summary>
       /// <param name="nww"></param>
       /// <param name="jxs"></param>
       /// <returns></returns>
       public DataTable getJxsEdglList(string nww, string jxs)
       {
           string sql = "";
           sql += " select b.nww,b.jxs,b.csfked,NVL(a.ysyed,0) ysyed,(b.csfked-NVL(a.ysyed,0)) kyed,b.objectid  from (";
           sql += " select a.jxsmc,sum(sybj) ysyed  from (";
           sql += " SELECT   BM.BUSINESS_PARTNER_NME jxsmc,";
           sql += " ATY.NAME kh,";
           sql += " A.APPLICATION_NUMBER sqbh,";
           sql += " A.APPLICATION_DATE sqsj,";
           sql += " CD.AMOUNT_FINANCED dkje, ";
           sql += " FN_CMS_PRINCIPAL_AMT(C.CONTRACT_ID,CD.AMOUNT_FINANCED) sybj, ";
           sql += " C.EXTERNAL_CONTRACT_NBR hth, ";
           sql += " RSC.REQUEST_STATUS_DSC htzt,";
           sql += " RD.EXPIRY_DTE dysj,";
           sql += " RD.START_DTE sqspsj ,";
           //sql += " nvl(ir.zsshzt || ic.zsshzt,'xx') h3xszs,nvl(ir.yycsshzt || ic.yycsshzt,'xx') h3yycs,nvl(ir.yyzsshzt || ic.yyzsshztt,'xx') h3yyzs ";
           sql += " nvl(ir.zsshzt || ic.zsshzt || ia.zsshzt,'xx') h3xszs,nvl(ir.yycsshzt || ic.yycsshzt || ia.yycsshzt,'xx') h3yycs,nvl(ir.yyzsshzt || ic.yyzsshztt || ia.yyzsshzt,'xx') h3yyzs ";
           sql += " FROM APPLICATION@TO_CMS A";
           sql += " inner JOIN BP_MAIN @TO_CMS BM ON BM.BUSINESS_PARTNER_ID = A.BUSINESS_PARTNER_ID";
           sql += " LEFT JOIN CONTRACT @TO_CMS C";
           sql += "    ON C.PROPOSAL_NBR = A.APPLICATION_NUMBER";
           sql += "       AND C.EXTERNAL_CONTRACT_NBR IS NOT NULL";
           sql += " LEFT JOIN REQUEST_STATUS_CODE @TO_CMS RSC      ";
           sql += "    ON C.REQUEST_STATUS_CDE = RSC.REQUEST_STATUS_CDE";
           sql += " LEFT JOIN ";
           sql += " (SELECT CONTRACT_REFERENCE_ID,";
           sql += "         START_DTE,";
           sql += "         EXPIRY_DTE, ";
           sql += "         PDC_RETURN_DTE,";
           sql += "         FOLLOWUP_COMMENTS";
           sql += "    FROM (SELECT CONTRACT_REFERENCE_ID,";
           sql += "                 START_DTE,";
           sql += "                 EXPIRY_DTE,";
           sql += "                 PDC_RETURN_DTE,";
           sql += "                 FOLLOWUP_COMMENTS,";
           sql += "                 ROW_NUMBER ()";
           sql += "                 OVER (PARTITION BY CONTRACT_REFERENCE_ID";
           sql += "                       ORDER BY CONTRACT_ID DESC)";
           sql += "                     RN";
           sql += "             FROM REQUEST_DETAIL @TO_CMS";
           sql += "            WHERE BUSINESS_PARTNER_NME IS NOT NULL)";
           sql += "    WHERE RN = 1) RD";
           sql += "     ON C.CONTRACT_ID = RD.CONTRACT_REFERENCE_ID";
           sql += "  LEFT JOIN APPLICANT_TYPE @TO_CMS ATY";
           sql += "      ON     A.APPLICATION_NUMBER = ATY.APPLICATION_NUMBER";
           sql += "        AND ATY.MAIN_APPLICANT = 'Y'";
           sql += "  LEFT JOIN CONTRACT_DETAIL @TO_CMS CD ON CD.APPLICATION_NUMBER = A.APPLICATION_NUMBER";
           sql += " left join financial_product@TO_CMS fp on fp.financial_product_id = C.financial_product_id";
           sql += " left join  i_Retailapp ir on ir.applicationno=A.application_number ";
           sql += " left join  i_companyapp ic on ic.applicationno=A.application_number ";
           sql += " left join  i_application ia on ia.application_number=A.application_number ";
           sql += " WHERE   A.APPLICATION_NUMBER not in (select applicationno from (";
           sql += " select  b.applicationno";
           sql += " from Ot_Instancecontext ins";
           sql += " inner join Ot_Workitem item on ins.objectid= item.instanceid and item.displayname !='运营人员（终审）'";
           sql += " inner join i_Retailapp b on  ins.bizobjectid = b.objectid";
           sql += " where ins.state!=4 and b.applicationno is not null";
           sql += " union all";
           sql += " select c.applicationno";
           sql += " from Ot_Instancecontext ins ";
           sql += " inner join Ot_Workitem item on ins.objectid= item.instanceid and item.displayname !='运营人员（终审）'";
           sql += " inner join  i_companyapp c on ins.bizobjectid =  c.objectid";
           sql += " where ins.state!=4 and c.applicationno is not null ";
           sql += "  union all ";
           sql += " select d.application_number ";
           sql += " from Ot_Instancecontext ins ";
           sql += " inner join Ot_Workitem item on ins.objectid = item.instanceid and item.displayname != '运营人员（终审）' ";
           sql += " inner join  i_application d on ins.bizobjectid = d.objectid ";
           sql += " where ins.state != 4 and d.application_number is not null)";
           sql += " ) ";
           sql += " and  A.APPLICATION_NUMBER not in ( select APPLICATION_NUMBER from  APPLICATION@TO_CMS where APPLICATION_NUMBER not in ";
           sql += " ( select PROPOSAL_NBR from contract@TO_CMS where PROPOSAL_NBR is not null ) and APPLICATION_DATE <  TO_DATE('2018/1/11 00:00:00','yyyy-mm-dd hh24:mi:ss') ";
           sql += " and STATUS_CODE ='01')";
           sql += " and (fp.FINANCIAL_PRODUCT_NME not like '%中信联贷%'  or fp.FINANCIAL_PRODUCT_NME is null)";
           sql += " and  A.STATUS_CODE IN (01,55,03)  ";
           sql += " and (RSC.REQUEST_STATUS_DSC !='Payout'  or RSC.REQUEST_STATUS_DSC is null )";
           //sql += " and (C.LIVE_STS='L'  or C.LIVE_STS is null )";
           //sql += " AND (C.LIVE_STS='L' or (C.LIVE_STS='D' and A.APPLICATION_DATE > TO_DATE('2018/1/11 00:00:00','yyyy-mm-dd hh24:mi:ss')))";
           sql += " AND (C.LIVE_STS='L' or ((C.LIVE_STS='D' or C.LIVE_STS is null )  and A.APPLICATION_DATE > TO_DATE('2018/1/11 00:00:00','yyyy-mm-dd hh24:mi:ss')))";
           sql += " and  RD.EXPIRY_DTE is null  ";

           sql += " ) a  where a.h3xszs <> '取消' and  a.h3xszs <> '拒绝' and a.h3yycs <> '拒绝' and  a.h3yyzs <> '拒绝' group by a.jxsmc order by jxsmc ) a right join I_jxsedgl b on a.jxsmc = b.jxs";
           sql += " where 1=1 ";
           if (string.IsNullOrEmpty(nww) && string.IsNullOrEmpty(jxs))
           {
               return new DataTable();
           }
           if (!string.IsNullOrEmpty(nww))
           {
               sql += " and b.nww like '%" + nww + "%' ";
           }
           if (!string.IsNullOrEmpty(jxs))
           {
               sql += " and b.jxs like '%" + jxs + "%' ";
           }
           DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
           return dt;
       }

       /// <summary>
       /// 获取经销商额度管理明细
       /// </summary>
       /// <param name="jxs"></param>
       /// <returns></returns>
       public DataTable getJxsedmx(string jxs)
       {
           string sql = "";
           sql += " select a.*  from (";
           sql += " SELECT   BM.BUSINESS_PARTNER_NME jxsmc,";
           sql += " ATY.NAME kh,";
           sql += " A.APPLICATION_NUMBER sqbh,";
           sql += " A.APPLICATION_DATE sqsj,";
           sql += " CD.AMOUNT_FINANCED dkje, ";
           sql += " FN_CMS_PRINCIPAL_AMT(C.CONTRACT_ID,CD.AMOUNT_FINANCED) sybj, ";
           sql += " C.EXTERNAL_CONTRACT_NBR hth, ";
           sql += " RSC.REQUEST_STATUS_DSC htzt,";
           sql += " to_char(C.CONTRACT_ACTIVATION_DTE,'yyyy-MM-dd') fksj,";
           sql += " floor(to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + "','yyyy-mm-dd') - C.CONTRACT_ACTIVATION_DTE) fkts,";
           sql += " RD.EXPIRY_DTE dysj,";
           sql += " RD.START_DTE sqspsj ,";
           //sql += " nvl(ir.zsshzt || ic.zsshzt,'xx') h3xszs,nvl(ir.yycsshzt || ic.yycsshzt,'xx') h3yycs,nvl(ir.yyzsshzt || ic.yyzsshztt,'xx') h3yyzs ";
           sql += " nvl(ir.zsshzt || ic.zsshzt || ia.zsshzt,'xx') h3xszs,nvl(ir.yycsshzt || ic.yycsshzt || ia.yycsshzt,'xx') h3yycs,nvl(ir.yyzsshzt || ic.yyzsshztt || ia.yyzsshzt,'xx') h3yyzs ";
           sql += " FROM APPLICATION@TO_CMS A";
           sql += " inner JOIN BP_MAIN @TO_CMS BM ON BM.BUSINESS_PARTNER_ID = A.BUSINESS_PARTNER_ID and  BM.BUSINESS_PARTNER_NME = '" + jxs + "'";
           sql += " LEFT JOIN CONTRACT @TO_CMS C";
           sql += "    ON C.PROPOSAL_NBR = A.APPLICATION_NUMBER";
           sql += "       AND C.EXTERNAL_CONTRACT_NBR IS NOT NULL";
           sql += " LEFT JOIN REQUEST_STATUS_CODE @TO_CMS RSC      ";
           sql += "    ON C.REQUEST_STATUS_CDE = RSC.REQUEST_STATUS_CDE";
           sql += " LEFT JOIN ";
           sql += " (SELECT CONTRACT_REFERENCE_ID,";
           sql += "         START_DTE,";
           sql += "         EXPIRY_DTE, ";
           sql += "         PDC_RETURN_DTE,";
           sql += "         FOLLOWUP_COMMENTS";
           sql += "    FROM (SELECT CONTRACT_REFERENCE_ID,";
           sql += "                 START_DTE,";
           sql += "                 EXPIRY_DTE,";
           sql += "                 PDC_RETURN_DTE,";
           sql += "                 FOLLOWUP_COMMENTS,";
           sql += "                 ROW_NUMBER ()";
           sql += "                 OVER (PARTITION BY CONTRACT_REFERENCE_ID";
           sql += "                       ORDER BY CONTRACT_ID DESC)";
           sql += "                     RN";
           sql += "             FROM REQUEST_DETAIL @TO_CMS";
           sql += "            WHERE BUSINESS_PARTNER_NME IS NOT NULL)";
           sql += "    WHERE RN = 1) RD";
           sql += "     ON C.CONTRACT_ID = RD.CONTRACT_REFERENCE_ID";
           sql += "  LEFT JOIN APPLICANT_TYPE @TO_CMS ATY";
           sql += "      ON     A.APPLICATION_NUMBER = ATY.APPLICATION_NUMBER";
           sql += "        AND ATY.MAIN_APPLICANT = 'Y'";
           sql += "  LEFT JOIN CONTRACT_DETAIL @TO_CMS CD ON CD.APPLICATION_NUMBER = A.APPLICATION_NUMBER";
           sql += " left join financial_product@TO_CMS fp on fp.financial_product_id = C.financial_product_id";
           sql += " left join  i_Retailapp ir on ir.applicationno=A.application_number ";
           sql += " left join  i_companyapp ic on ic.applicationno=A.application_number ";
           sql += " left join  i_application ia on ia.application_number=A.application_number ";
           sql += " WHERE   A.APPLICATION_NUMBER not in (select applicationno from (";
           sql += " select  b.applicationno";
           sql += " from Ot_Instancecontext ins";
           sql += " inner join Ot_Workitem item on ins.objectid= item.instanceid and item.displayname !='运营人员（终审）'";
           sql += " inner join i_Retailapp b on  ins.bizobjectid = b.objectid";
           sql += " where ins.state!=4 and b.applicationno is not null";
           sql += " union all";
           sql += " select c.applicationno";
           sql += " from Ot_Instancecontext ins ";
           sql += " inner join Ot_Workitem item on ins.objectid= item.instanceid and item.displayname !='运营人员（终审）'";
           sql += " inner join  i_companyapp c on ins.bizobjectid =  c.objectid";
           sql += " where ins.state!=4 and c.applicationno is not null ";
           sql += "  union all ";
           sql += " select d.application_number ";
           sql += " from Ot_Instancecontext ins ";
           sql += " inner join Ot_Workitem item on ins.objectid = item.instanceid and item.displayname != '运营人员（终审）' ";
           sql += " inner join  i_application d on ins.bizobjectid = d.objectid ";
           sql += " where ins.state != 4 and d.application_number is not null)";
           sql += " ) ";
           sql += " and  A.APPLICATION_NUMBER not in ( select APPLICATION_NUMBER from  APPLICATION@TO_CMS where APPLICATION_NUMBER not in ";
           sql += " ( select PROPOSAL_NBR from contract@TO_CMS where PROPOSAL_NBR is not null ) and APPLICATION_DATE <  TO_DATE('2018/1/11 00:00:00','yyyy-mm-dd hh24:mi:ss') ";
           sql += " and STATUS_CODE ='01')";
           sql += " and (fp.FINANCIAL_PRODUCT_NME not like '%中信联贷%'  or fp.FINANCIAL_PRODUCT_NME is null)";
           sql += " and  A.STATUS_CODE IN (01,55,03)  ";
           sql += " and (RSC.REQUEST_STATUS_DSC !='Payout'  or RSC.REQUEST_STATUS_DSC is null )";
           sql += " AND (C.LIVE_STS='L' or ((C.LIVE_STS='D' or C.LIVE_STS is null )  and A.APPLICATION_DATE > TO_DATE('2018/1/11 00:00:00','yyyy-mm-dd hh24:mi:ss')))";
           sql += " and  RD.EXPIRY_DTE is null  ";
           sql += " ) a where a.h3xszs <> '取消' and  a.h3xszs <> '拒绝' and a.h3yycs <> '拒绝' and  a.h3yyzs <> '拒绝' ";
           DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
           return dt;
       }

       /// <summary>
       /// 申请单是否做了抵押
       /// </summary>
       /// <param name="APPLICATION_NUMBER">申请号</param>
       /// <returns></returns>
       public int Is_Mortgaged(string application_number)
       {
           string sql = "";
           sql += " select a.* from REQUEST_DETAIL @TO_CMS a ";
           sql += " join contract@TO_CMS b on a.CONTRACT_REFERENCE_ID = b.CONTRACT_ID ";
           sql += " join  APPLICATION@TO_CMS c on b.PROPOSAL_NBR =c.APPLICATION_NUMBER ";
           sql += " where expiry_dte is not null and c.APPLICATION_NUMBER='" + application_number + "'";
           DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
           if (CommonFunction.hasData(dt))
           {
               return 1;
           }
           else
           {
               return 0;
           }
       }
    }

    public class Employee
    {
        public Employee() { }
        public Employee(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 身份验证类
    /// </summary>
    public class Authentication : System.Web.Services.Protocols.SoapHeader
    {
        public Authentication() { }
        public Authentication(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 流程服务返回消息类
    /// </summary>
    //public class BPMServiceResult
    //{
    //    /// <summary>
    //    /// 消息类构造函数
    //    /// </summary>
    //    /// <param name="success"></param>
    //    /// <param name="instanceId"></param>
    //    /// <param name="workItemId"></param>
    //    /// <param name="message"></param>
    //    public BPMServiceResult(bool success, string instanceId, string workItemId, string message, string WorkItemUrl)
    //    {
    //        this.Success = success;
    //        this.InstanceID = instanceId;
    //        this.Message = message;
    //        this.WorkItemID = workItemId;
    //        this.WorkItemUrl = WorkItemUrl;
    //    }

    //    /// <summary>
    //    /// 消息类构造函数
    //    /// </summary>
    //    /// <param name="success"></param>
    //    /// <param name="message"></param>
    //    public BPMServiceResult(bool success, string message)
    //        : this(success, string.Empty, string.Empty, message, string.Empty)
    //    {

    //    }

    //    public BPMServiceResult() { }

    //    private bool success = false;
    //    /// <summary>
    //    /// 获取或设置流程启动是否成功
    //    /// </summary>
    //    public bool Success
    //    {
    //        get { return success; }
    //        set { success = value; }
    //    }
    //    private string instanceId = string.Empty;
    //    /// <summary>
    //    /// 获取或设置启动的流程实例ID
    //    /// </summary>
    //    public string InstanceID
    //    {
    //        get { return instanceId; }
    //        set { this.instanceId = value; }
    //    }
    //    private string message = string.Empty;
    //    /// <summary>
    //    /// 获取或设置系统返回消息
    //    /// </summary>
    //    public string Message
    //    {
    //        get { return message; }
    //        set { this.message = value; }
    //    }
    //    private string workItemId = string.Empty;
    //    /// <summary>
    //    /// 获取或设置第一个节点的ItemID
    //    /// </summary>
    //    public string WorkItemID
    //    {
    //        get { return workItemId; }
    //        set { this.workItemId = value; }
    //    }
    //    private string workItemUrl = string.Empty;
    //    /// <summary>
    //    /// 获取或设置第一个节点的url
    //    /// </summary>
    //    public string WorkItemUrl
    //    {
    //        get { return workItemUrl; }
    //        set { this.workItemUrl = value; }
    //    }
    //}

    /// <summary>
    /// 提交任务后返回对象
    /// </summary>
    [Serializable]
    public class ReturnWorkItemInfo
    {
        public ReturnWorkItemInfo() { }
        private bool isSuccess = false;
        /// <summary>
        /// 是否提交成功
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
            set { this.isSuccess = value; }
        }
        private string workItemUrl = string.Empty;
        /// <summary>
        /// 当前表单地址
        /// </summary>
        public string WorkItemUrl
        {
            get { return workItemUrl; }
            set { this.workItemUrl = value; }
        }
    }

    /// <summary>
    /// 数据项参数
    /// </summary>
    [Serializable]
    public class DataItemParam
    {
        private string itemName = string.Empty;
        /// <summary>
        /// 获取或设置数据项名称
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { this.itemName = value; }
        }

        private object itemValue = string.Empty;
        /// <summary>
        /// 获取或设置数据项的值
        /// </summary>
        public object ItemValue
        {
            get { return itemValue; }
            set { this.itemValue = value; }
        }
    }
}