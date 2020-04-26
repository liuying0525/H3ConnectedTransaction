using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OThinker.H3.Controllers;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.OracleClient;
using Newtonsoft.Json.Converters;
using System.Web.Http;
using System.Net;
using System.IO;


namespace DZ.External.WebApi.Common
{
    public class webapi
    {
        #region 小程序接口
        /// <summary>
        /// 业务数据库连接池
        /// </summary>
        protected const string DZWeChatAPIConn = "DZWeChatAPI";
        protected const string DZZhiZuAPIConn = "zhizu";
        protected const string DZCMSAPIConn = "cap";
        protected const string DZCmsMiddleConn = "CAPDB";
        /// <summary>
        /// --金融产品参数限制条件
        /// </summary>
        /// <returns></returns>

        public static string V_FP_PARAMETER_MAIN()
        {
            string result = "";
            //string sql = "select * from IN_CMS.V_FP_PARAMETER_MAIN v";
            string sql = "select FINANCIAL_PRODUCT_ID,FINANCIAL_PRODUCT_NME,FP_GROUP_NME,FP_GROUP_ID,VALID_FROM_DTE,VALID_TO_DTE,EXECUTION_DTE,MINIMUM_FINANCING_AMT,MAXIMUM_FINANCING_AMT,MINIMUM_LEASE_TRM,MAXIMUN_LEASE_TRM,MAXIMUM_FINANCING_PCT,FUTURE_VALUE_TYP,MINIMUM_RV_PCT,MAXIMUM_RV_PCT,MINIMUM_TERM_MM,MAXIMUM_TERM_MM,RV_PCT,CUSTOMER_RTE,ACTUAL_RTE from IN_CMS.V_FP_PARAMETER_MAIN v";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 产品利率
        /// </summary>
        /// <param name="financial_product_id"></param>
        /// <returns></returns>

        public static string v_fp_rate(string financial_product_id)
        {
            string result = "";
            if (string.IsNullOrEmpty(financial_product_id))
            {
                return errEmpty(financial_product_id);
            }
            string sql = "select * from IN_CMS.v_fp_rate v where v.financial_product_id=" + financial_product_id;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 产品与车辆关联
        /// </summary>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public static string V_FP_ASSET(string Financial_Product_ID)
        {
            string result = "";
            if (string.IsNullOrEmpty(Financial_Product_ID))
            {
                return errEmpty(Financial_Product_ID);
            }
            string sql = "select * from IN_CMS.V_FP_ASSET v where v.Financial_Product_ID =" + Financial_Product_ID;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 产品与经销商关联
        /// </summary>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public static string V_FP_DEALER(string Financial_Product_ID)
        {
            string result = "";
            if (string.IsNullOrEmpty(Financial_Product_ID))
            {
                return errEmpty(Financial_Product_ID);
            }
            string sql = "select * from IN_CMS.V_FP_DEALER v where v.Financial_Product_ID =" + Financial_Product_ID;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 车型查询车价
        /// </summary>
        /// <param name="asset_model_cde"></param>
        /// <returns></returns>
        public static string V_ASSET_INFO(string asset_model_cde)
        {
            string result = "";
            if (string.IsNullOrEmpty(asset_model_cde))
            {
                return errEmpty(asset_model_cde);
            }
            string sql = "select distinct * from IN_CMS.V_ASSET_INFO v where v.asset_model_cde=" + asset_model_cde;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 订单申请编号查询
        /// </summary>
        /// <param name="phone_number"></param>
        /// <returns></returns>
        public static string v_app_phone_number(string phone_number)
        {
            string result = "";
            if (string.IsNullOrEmpty(phone_number))
            {
                return errEmpty(phone_number);
            }
            string sql = "select * from v_app_phone_number v where v.phone_number='" + phone_number + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="ID_CARD_NBR"></param>
        /// <returns></returns>
        public static string V_APPLICATION_STATUS(string ID_CARD_NBR)
        {
            string result = "";
            if (string.IsNullOrEmpty(ID_CARD_NBR))
            {
                return errEmpty(ID_CARD_NBR);
            }
            string sql = "select * from IN_CMS.V_APPLICATION_STATUS where ID_CARD_NBR='" + ID_CARD_NBR + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 还款计划表
        /// </summary>
        /// <param name="application_number"></param>
        /// <returns></returns>
        public static string V_USER_PLAN_DT(string proposal_nbr)
        {
            string result = "";
            if (string.IsNullOrEmpty(proposal_nbr))
            {
                return errEmpty(proposal_nbr);
            }
            string sql = "select * from V_USER_PLAN_DT v where v.proposal_nbr='" + proposal_nbr + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }

        /// <summary>
        /// 经销商与车型关联（通过经销商ID查询车型ID）
        /// </summary>
        /// <param name="business_partner_id"></param>
        /// <returns></returns>
        public static string V_ASSET_DEALER(string business_partner_id)
        {
            string result = "";
            if (string.IsNullOrEmpty(business_partner_id))
            {
                return errEmpty(business_partner_id);
            }
            string sql = "select * from IN_CMS.V_ASSET_DEALER v where v.business_partner_id='" + business_partner_id + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 门店信息查询
        /// </summary>
        /// <returns></returns>
        public static string v_dealer_info()
        {
            string result = "";
            string sql = "select * from IN_CMS.v_dealer_info ";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 金融产品，车辆，经销商 关联（小程序）
        /// </summary>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public static string V_FP_ASSET_DEALERForWechat(string Financial_Product_ID, string business_partner_id)
        {
            string result = "";
            if (string.IsNullOrEmpty(Financial_Product_ID) || string.IsNullOrEmpty(business_partner_id))
            {
                return errEmpty("");
            }
            string sql = "select * from IN_CMS.V_FP_ASSET_DEALER v where 1=1 ";
            sql += " and v.Financial_Product_ID=" + Financial_Product_ID;
            sql += " and business_partner_id=" + business_partner_id;

            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 根据门店车型查询产品
        /// </summary>
        /// <param name="BUSINESS_PARTNER_ID"></param>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public static string getProductByCompanyAndModel(string BUSINESS_PARTNER_ID, string ASSET_MODEL_CDE)
        {
            string result = "";
            if (string.IsNullOrEmpty(BUSINESS_PARTNER_ID) || string.IsNullOrEmpty(ASSET_MODEL_CDE))
            {
                return errEmpty("");
            }
            string sql = "select  distinct FINANCIAL_PRODUCT_ID,FINANCIAL_PRODUCT_NME from V_FP_ASSET_DEALER where BUSINESS_PARTNER_ID=" + BUSINESS_PARTNER_ID + "  and  ASSET_MODEL_CDE=" + ASSET_MODEL_CDE;

            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 根据车型查询产品
        /// </summary>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public static string getProductByModel(string ASSET_MODEL_CDE)
        {
            string result = "";
            if (string.IsNullOrEmpty(ASSET_MODEL_CDE))
            {
                return errEmpty("");
            }
            string sql = "select  distinct FINANCIAL_PRODUCT_ID,FINANCIAL_PRODUCT_NME from V_FP_ASSET_DEALER where  ASSET_MODEL_CDE=" + ASSET_MODEL_CDE;

            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }

        /// <summary>
        /// 根据车型产品查询门店
        /// </summary>
        /// <param name="FINANCIAL_PRODUCT_ID"></param>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public static string getCompanyByModelAndProduct(string FINANCIAL_PRODUCT_ID, string ASSET_MODEL_CDE)
        {
            string result = "";
            if (string.IsNullOrEmpty(FINANCIAL_PRODUCT_ID) || string.IsNullOrEmpty(ASSET_MODEL_CDE))
            {
                return errEmpty("");
            }
            string sql = "select  distinct BUSINESS_PARTNER_ID,COMPANY_NME from V_FP_ASSET_DEALER where FINANCIAL_PRODUCT_ID=" + FINANCIAL_PRODUCT_ID + " and  ASSET_MODEL_CDE=" + ASSET_MODEL_CDE;

            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 车型信息查询接口
        /// </summary>
        /// <param name="version_Stime">开始时间</param>
        /// <param name="version_Etime">结束时间</param>
        /// <returns></returns>
        public static string v_asset_info_detail(string version_Stime, string version_Etime)
        {
            string result = "";
            if (string.IsNullOrEmpty(version_Stime) || string.IsNullOrEmpty(version_Etime))
            {
                return errEmpty("");
            }
            string sql = "select * from IN_CMS.v_asset_info_detail t where t.version_time > to_date('" + version_Stime + "','yyyy-mm-dd hh24:mi:ss') and t.version_time < to_date('" + version_Etime + "','yyyy-mm-dd hh24:mi:ss')";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 订单明细信息查询
        /// </summary>
        /// <param name="application_number"></param>
        /// <returns></returns>
        public static string V_APPLICATION_DET(string application_number)
        {
            string result = "";
            if (string.IsNullOrEmpty(application_number))
            {
                return errEmpty("application_number");
            }
            string sql = "select * from V_APPLICATION_DET v where v.application_number='" + application_number + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 金融专员账户信息
        /// </summary>
        /// <returns></returns>
        public static string V_BUSINESS_PARTENTER_INFO()
        {
            string result = "";
            string sql = "select * from V_BUSINESS_PARTENTER_INFO v";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 用户验证
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string ValidateUser(string userCode, string password)
        {
            OThinker.H3.Controllers.UserValidator user = OThinker.H3.Controllers.UserValidatorFactory.Validate(userCode, password);
            if (user != null)
            {
                return "{ \"errcode\": \"0\", \"errmsg\": \"验证成功\", \"data\":[]}";
            }
            else
            {
                return "{ \"errcode\": \"1\", \"errmsg\": \"帐号或密码不正确\", \"data\":[]}";
            }

        }
        /// <summary>
        /// 等额本息月供计算器接口
        /// </summary>
        /// <param name="D"></param>
        /// <param name="W"></param>
        /// <param name="M"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static string V_PMT(string D, string W, string M, string R)
        {
            string result = "";
            if (string.IsNullOrEmpty(D) || string.IsNullOrEmpty(W) || string.IsNullOrEmpty(M) || string.IsNullOrEmpty(R))
            {
                return errEmpty("");
            }

            string sql = "select zpmt(" + D + "," + W + "," + M + "," + R + ",0) pmt from dual";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// --品牌LOGO图片URL
        /// </summary>
        /// <returns></returns>
        public static string v_asset_logo_info()
        {
            string result = "";
            string sql = "select * from v_asset_logo_info ";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// --车系图片URL
        /// </summary>
        /// <returns></returns>
        public static string v_asset_series_url_info()
        {
            string result = "";
            string sql = " select * from v_asset_series_url_info ";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }

        /// <summary>
        /// 产品信息(直租)
        /// </summary>
        /// <returns></returns>
        public static string V_PRODUCT_INFO()
        {
            string result = "";
            string sql = " select * from V_PRODUCT_INFO v";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 区域经销商与产品关系(直租)
        /// </summary>
        /// <returns></returns>
        public static string V_PRODUCT_DEALER(string azdywid)
        {
            string result = "";
            string sql = "select * from V_PRODUCT_DEALER v where azdywid='" + azdywid + "'";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 车辆与产品关系(直租)
        /// </summary>
        /// <returns></returns>
        public static string V_PRODUCT_SCARMODEL(string fld_modelid)
        {
            string result = "";
            string sql = " select * from V_PRODUCT_SCARMODEL v where fld_modelid in ('" + fld_modelid + "','000')";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 申请合同详细（直租）
        /// </summary>
        /// <param name="phone_number"></param>
        /// <returns></returns>
        public static string V_APPLICATION_DET_INFO(string phone_number)
        {
            string result = "";
            if (string.IsNullOrEmpty(phone_number))
            {
                return errEmpty(phone_number);
            }
            string sql = "select * from V_APPLICATION_DET_INFO v where phone_number='" + phone_number + "'";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 还款计划表（直租）
        /// </summary>
        /// <param name="AHTHM"></param>
        /// <returns></returns>
        public static string V_REPAY_PLAN(string AHTHM)
        {
            string result = "";
            if (string.IsNullOrEmpty(AHTHM))
            {
                return errEmpty(AHTHM);
            }
            string sql = "select * from V_REPAY_PLAN v where v.AHTHM='" + AHTHM + "'";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 门店信息（直租）
        /// </summary>
        /// <returns></returns>
        public static string V_RENT_DEALER_INFO()
        {
            string result = "";
            string sql = "select * from V_RENT_DEALER_INFO v";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 获取车型（直租）
        /// </summary>
        /// <returns></returns>
        public static string V_RENT_MODEL_INFO(string fld_trimid)
        {
            string result = "";
            if (string.IsNullOrEmpty(fld_trimid))
            {
                return errEmpty(fld_trimid);
            }
            string sql = "select * from V_RENT_MODEL_INFO v where v.fld_trimid=" + fld_trimid + "";
            result = ExecuteDataTableSql(DZZhiZuAPIConn, sql);
            return result;
        }

        /// <summary>
        /// 直租月供计算接口
        /// </summary>
        /// <param name="D">贷款额</param>
        /// <param name="W">尾款</param>
        /// <param name="M">期数</param>
        /// <param name="R">利率</param>
        /// <returns></returns>
        public static string ZPMTRENT(string D, string W, string M, string R)
        {
            string result = "";
            if (string.IsNullOrEmpty(D) || string.IsNullOrEmpty(W) || string.IsNullOrEmpty(M) || string.IsNullOrEmpty(R))
            {
                return errEmpty("");
            }
            string sql = "select zpmtrent(" + D + "," + W + "," + M + "," + R + ") v_pmt from dual ";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 提前还款查询
        /// </summary>
        /// <param name="P_ET_DTE"></param>
        /// <param name="P_EXT_CTR_NO"></param>
        /// <returns></returns>
        public static string v_query_et_amt(string P_AppDTE, string P_ET_DTE, string P_EXT_CTR_NO)
        {
            string result = "";
            if (string.IsNullOrEmpty(P_ET_DTE) || string.IsNullOrEmpty(P_EXT_CTR_NO))
            {
                return errEmpty("");
            }
            string sql = "";
            //sql = @"select * from v_query_et_amt WHERE EXTERNAL_CONTRACT_NBR = '{0}' and ET_DATE='{1}'";
            sql = @"  select v.EXTERNAL_CONTRACT_NBR--合同号
  ,v.ET_DTE
  ,v.ET_OS_PRIN_AMT--未偿本金
  ,v.INT_RCBL_AMT--利息应收
  ,et_nonactrebate_amt--提前终止费用
  ,v.ET_OD_RENTAL_AMT --逾期租金
  ,ET_OS_PRIN_AMT+int_rcbl_amt+et_nonactrebate_amt+ET_OD_RENTAL_AMT ET_AMT
  ,v.ET_PENALTY_AMT--提前罚款
  ,v.ODI_PENALTY_OS_AMT--未付逾期利息
  ,v.ACCRUED_OD_AMT   --应计逾期利息
  ,ET_OS_PRIN_AMT+int_rcbl_amt+et_nonactrebate_amt+et_penalty_amt+v.ET_OD_RENTAL_AMT+v.ODI_PENALTY_OS_AMT+v.ACCRUED_OD_AMT  ET_TOTAL_AMT--应收净额
 from DZ_VW_CALC_ET_DATA v where p_view_param.set_contract_nbr_param('{0}')='{0}' 
and p_view_param.set_ETDate_param(to_date('{1}','yyyy-mm-dd'))=to_date('{1}','yyyy-mm-dd')
and p_view_param.set_AppDate_param(to_date('{2}','yyyy-mm-dd'))=to_date('{2}','yyyy-mm-dd')";
            sql = string.Format(sql, P_EXT_CTR_NO, P_ET_DTE, P_AppDTE);
            result = ExecuteDataTableSql(DZCMSAPIConn, sql);
            return result;

        }
        /// <summary>
        /// 提前还款计算
        /// </summary>
        /// <param name="P_ET_DTE"></param>
        /// <param name="P_EXT_CTR_NO"></param>
        /// <returns></returns>
        public static string v_count_et_amt(string P_ET_DTE, string P_EXT_CTR_NO)
        {
            string result = "";
            if (string.IsNullOrEmpty(P_ET_DTE) || string.IsNullOrEmpty(P_EXT_CTR_NO))
            {
                return errEmpty("");
            }
            string sql = "";
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(DZWeChatAPIConn);
            string connectionString = dbObject.DbConnectionString;
            OracleConnection conn = new OracleConnection(connectionString);
            string storedProcedureName = "IN_CMS.calc_et_data";

            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                cmd.Parameters.Add("P_ET_DTE", OracleType.VarChar, 10).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("P_EXT_CTR_NO", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("o_result_code", OracleType.VarChar, 250).Direction = ParameterDirection.Output;//

                cmd.Parameters["P_ET_DTE"].Value = P_ET_DTE;// DateTime.Parse(P_ET_DTE).ToString("yyyy-MM-dd");
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(cmd.Parameters["P_ET_DTE"].Value.ToString());
                cmd.Parameters["P_EXT_CTR_NO"].Value = P_EXT_CTR_NO;
                cmd.ExecuteNonQuery();

                object o_result_code = cmd.Parameters["o_result_code"].Value;
                conn.Close();
                if (o_result_code.ToString() == "1")
                {
                    return "{ \"errcode\": \"0\", \"errmsg\": \"计算成功！\", \"data\":[]}";
                    //sql = @"select * from v_query_et_amt WHERE EXTERNAL_CONTRACT_NBR = '" + P_EXT_CTR_NO + "'";
                }
                else
                {
                    return "{ \"errcode\": \"1\", \"errmsg\": \"计算失败！失败代码：" + o_result_code.ToString() + "\", \"data\":[]}";
                }
            }

            catch (Exception ex)
            {
                return rtnExError(ex);
            }

        }
        /// <summary>
        /// --根据手机号查合同
        /// </summary>
        /// <returns></returns>
        public static string v_app_contract_info(string phone_number)
        {
            string result = "";
            if (string.IsNullOrEmpty(phone_number))
            {
                return errEmpty("");
            }
            string sql = "select * from v_app_contract_info v where v.phone_number='" + phone_number + "'";
            //OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("v_app_contract_info:"+sql);
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }

        /// <summary>
        ///   --根据年月返回这个月所有节假日''
        /// </summary>
        /// <param name="holiday_dte">201506</param>
        /// <returns></returns>

        public static string getHoliday(string holiday_dte)
        {
            string result = "";
            if (string.IsNullOrEmpty(holiday_dte))
            {
                return errEmpty("");
            }

            string sql = "select holiday_dte from  HOLIDAY_CODE@to_cms where to_char(holiday_dte,'yyyymm')='" + holiday_dte + "'";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        #endregion

        #region 集团接口
        /// <summary>
        /// 金融产品，车辆，经销商 关联（集团）
        /// </summary>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public static string V_FP_ASSET_DEALER(string ASSET_MODEL_CDE, string BUSINESS_PARTNER_ID)
        {
            string result = "";
            if (string.IsNullOrEmpty(BUSINESS_PARTNER_ID) || string.IsNullOrEmpty(ASSET_MODEL_CDE))
            {
                return errEmpty("");
            }


            string sql = "select distinct FP_GROUP_NME,FP_GROUP_ID,FINANCIAL_PRODUCT_NME,FINANCIAL_PRODUCT_ID from IN_CMS.V_FP_ASSET_DEALER v where v.BUSINESS_PARTNER_ID=" + BUSINESS_PARTNER_ID + " and ASSET_MODEL_CDE=" + ASSET_MODEL_CDE;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 查金融产品参数限制条件 future_value_typ=R 表示支持尾款（集团）
        /// </summary>
        /// <param name="financial_product_id"></param>
        /// <returns></returns>
        public static string V_FP_PARAMETER_MAIN(string financial_product_id)
        {
            string result = "";
            if (string.IsNullOrEmpty(financial_product_id))
            {
                return errEmpty(financial_product_id);
            }
            string sql = "select * from IN_CMS.V_FP_PARAMETER_MAIN v where v.financial_product_id=" + financial_product_id;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 根据时间范围查金融产品参数限制条件
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static string V_FP_PARAMETER_MAIN(string beginDate, string endDate)
        {
            string result = "";
            if (string.IsNullOrEmpty(beginDate) || string.IsNullOrEmpty(endDate))
            {
                return errEmpty("");
            }
            string sql = "select * from IN_CMS.V_FP_PARAMETER_MAIN v where v.Execution_Dte >= to_date('" + beginDate + "','yyyy-mm-dd') and v.Execution_Dte <= to_date('" + endDate + "','yyyy-mm-dd')";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 还款计划计算接口（集团）
        /// </summary>
        /// <param name="D">贷款额</param>
        /// <param name="W">尾款</param>
        /// <param name="M">贷款期数</param>
        /// <param name="R">利率</param>
        /// <param name="T">贷款类型</param>
        /// <param name="FP">产品编号</param>
        /// <returns></returns>
        public static string V_PLAN_DT(string D, string W, string M, string FP)
        {
            string result = "";
            string R = "", T = "";
            if (string.IsNullOrEmpty(D) || string.IsNullOrEmpty(W) || string.IsNullOrEmpty(M) || string.IsNullOrEmpty(FP))
            {
                return errEmpty("");
            }

            //string connectionString = System.Configuration.ConfigurationManager.AppSettings["jxsDZPORT"];
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(DZWeChatAPIConn);
            string connectionString = dbObject.DbConnectionString;
            OracleConnection conn = new OracleConnection(connectionString);
            string storedProcedureName = "IN_CMS.zgeneral_plan";

            try
            {
                //获取利率和贷款类型
                T = ExecuteScalar(DZWeChatAPIConn, "select zgettype('" + FP + "')  R from dual");
                //T = ExecuteScalar(DZWeChatAPIConn, "select distinct ACTUAL_RTE/100 T from IN_CMS.v_fp_rate v where v.financial_product_id='" + FP + "'");
                R = ExecuteScalar(DZWeChatAPIConn, "select distinct v.CUSTOMER_RTE/100  from IN_CMS.V_FP_PARAMETER_MAIN v where v.financial_product_id=" + FP + " and v.minimum_term_mm <" + M + " and v.maximum_term_mm>=" + M + "");

                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;

                cmd.Parameters.Add("D", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("W", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("M", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("R", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("T", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("FP", OracleType.VarChar, 250).Direction = ParameterDirection.Input;
                cmd.Parameters.Add("o_plan_id", OracleType.VarChar, 250).Direction = ParameterDirection.Output;//还款计划编号

                cmd.Parameters["D"].Value = D;
                cmd.Parameters["W"].Value = W;
                cmd.Parameters["M"].Value = M;
                cmd.Parameters["R"].Value = R;
                cmd.Parameters["T"].Value = T;
                cmd.Parameters["FP"].Value = FP;

                cmd.ExecuteNonQuery();

                string o_plan_id = cmd.Parameters["o_plan_id"].Value + string.Empty;
                conn.Close();

                string sql = "select * from IN_CMS.V_PLAN_DT v where v.plan_id=" + o_plan_id;
                result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
                return result;

            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
        }

        /// <summary>
        /// 批量查CAP申请状态（集团）
        /// </summary>
        /// <param name="application_numbers">申请号字串，逗号隔开</param>
        /// <returns></returns>
        public static string V_APPLICATION_STATUS_ByMoreNum(string application_numbers)
        {
            string result = "";
            if (string.IsNullOrEmpty(application_numbers))
            {
                return errEmpty(application_numbers);
            }
            string[] array = application_numbers.Split(',');
            string ss = "";
            foreach (string s in array)
            {
                if (s != "")
                {
                    ss += "'" + s + "',";
                }
            }
            ss = ss.Substring(0, ss.Length - 1);
            string sql = "select * from IN_CMS.V_APPLICATION_STATUS where application_number in (" + ss + ")";
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }

        /// <summary>
        /// 根据品牌ID查失效车型
        /// </summary>
        /// <param name="asset_make_cde"></param>
        /// <returns></returns>
        public static string v_inactive_asset_info(string asset_make_cde)
        {
            string result = "";
            if (string.IsNullOrEmpty(asset_make_cde))
            {
                return errEmpty(asset_make_cde);
            }
            string sql = "select * from v_inactive_asset_info v where v.asset_make_cde=" + asset_make_cde;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 集团销售订单信息推送
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string salesorder_post([FromBody] salesorder model)
        {
            string result = "";
            if (string.IsNullOrEmpty(model.order_no) || string.IsNullOrEmpty(model.phone) || string.IsNullOrEmpty(model.customer) || string.IsNullOrEmpty(model.sales) || string.IsNullOrEmpty(model.sales_phone))
            {
                AddLog("集团推送失败：参数为空");
                return errEmpty("");
            }
            string sql = "insert into jt_sales_order(order_no ,order_time ,customer ,phone ,idcard_no ,business ,business_id ,asset_model_dsc ,asset_model_cde ,retail_price_amt ,financial_product_nme ,financial_product_id ,sfbl ,dkje ,ygje ,ygqx ,wkje ,wkbl ,sales ,sales_phone)";
            sql += " values(";
            sql += " '" + model.order_no + "','" + model.order_time + "','" + model.customer + "','" + model.phone + "','" + model.idcard_no + "','" + model.business + "','" + model.business_id + "','" + model.asset_model_dsc + "','" + model.asset_model_cde + "','" + model.retail_price_amt + "','" + model.financial_product_nme + "','" + model.financial_product_id + "','" + model.sfbl + "','" + model.dkje + "','" + model.ygje + "','" + model.ygqx + "','" + model.wkje + "','" + model.wkbl + "','" + model.sales + "','" + model.sales_phone + "'";
            sql += ")";
            AddLog("集团推送sql：" + sql);
            int n = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            if (n > 0)
            {
                try
                {
                    AddLog("集团数据推送成功！");
                    result = "{ \"errcode\": \"0\", \"errmsg\": \"\"}";
                    //推送小程序
                    AddLog("推送小程序数据开始！");
                    string authid = System.Configuration.ConfigurationManager.AppSettings["authid"];
                    string authsecret = System.Configuration.ConfigurationManager.AppSettings["authsecret"];
                    string wechatTokenUrl = System.Configuration.ConfigurationManager.AppSettings["wechatTokenUrl"];
                    string url = wechatTokenUrl + "authid=" + authid + "&authsecret=" + authsecret;
                    //{"access_token":"87bmLAs0e7hf9M9NCLFyMXR0bYUxtBP81bkKGFe7iqrdC8tkM3GMSPQLrUXOYsSivMW6TV%2BiLnRyTpS8Wrf0nYdYGlag4PLu","code":"A00006","expires_in":7200};
                    AddLog("推送小程序数据参数token url：" + url);
                    string jsonText = HttpGet(url);
                    AddLog("推送小程序数据token返回参数：" + jsonText);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
                    string access_token = jo["access_token"].ToString();

                    string strURL = System.Configuration.ConfigurationManager.AppSettings["wechatCreateOrderUrl"];//"http://dongzheng.test.maiget.com/api/wxapp/reservationorder/fi/createorder";
                    AddLog("推送小程序post url参数：" + strURL);
                    var postData = "";
                    postData = "access_token=" + access_token;
                    postData += "&ordernumberext=" + model.order_no;
                    postData += "&reservationname=" + model.customer;
                    postData += "&reservationtelnum=" + model.phone;
                    postData += "&reservationidnumber=" + model.idcard_no;
                    postData += "&businessid=" + model.business_id;
                    postData += "&carmodelid=" + model.asset_model_cde;
                    postData += "&financialproductid=" + model.financial_product_id;
                    postData += "&downpaymentratio=" + model.sfbl;
                    postData += "&downpaymentpara=" + model.dkje;
                    postData += "&monthlysupply=" + model.ygje;
                    postData += "&term=" + model.ygqx;
                    postData += "&createtime=" + model.order_time;
                    postData += "&tailmoney=" + model.wkbl;
                    postData += "&finalpayment=" + model.wkje;
                    postData += "&salesmanname=" + model.sales;
                    postData += "&salesmancont=" + model.sales_phone;
                    AddLog("推送小程序postData：" + postData);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strURL);
                    req.Method = "POST";
                    req.Timeout = 30000;
                    req.AllowAutoRedirect = false;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.KeepAlive = true;

                    byte[] postBytes = Encoding.UTF8.GetBytes(postData);
                    req.ContentLength = postBytes.Length;
                    Stream postDataStream = req.GetRequestStream();
                    postDataStream.Write(postBytes, 0, postBytes.Length);
                    postDataStream.Close();

                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    System.IO.Stream s;
                    s = resp.GetResponseStream();
                    string StrDate = "";
                    string strValue = "";
                    StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                    while ((StrDate = Reader.ReadLine()) != null)
                    {
                        strValue += StrDate + "\r\n";
                    }
                    AddLog("推送小程序结果：" + strValue);
                }
                catch (Exception ex)
                {
                    AddLog("推送小程序结果：" + ex.ToString());
                }

            }
            else
            {
                result = "{ \"errcode\": \"1\", \"errmsg\": \"数据写入失败\"}";
                AddLog("集团数据推送失败！");
            }
            return result;
        }
        /// <summary>
        /// 根据门店车型查推荐产品 
        /// </summary>
        /// <param name="asset_make_cde"></param>
        /// <returns></returns>
        public static string v_recommand_fp(string business_partner_id, string asset_model_cde)
        {
            string result = "";
            if (string.IsNullOrEmpty(business_partner_id) || string.IsNullOrEmpty(asset_model_cde))
            {
                return errEmpty("");
            }
            string sql = "select * from v_recommand_fp v where v.business_partner_id=" + business_partner_id;// +" and v.asset_model_cde=" + asset_model_cde;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        /// <summary>
        /// 根据门店和产品ID查推荐产品详细信息
        /// </summary>
        /// <param name="business_partner_id"></param>
        /// <param name="financial_product_id"></param>
        /// <returns></returns>
        public static string v_recommand_fp_det(string business_partner_id, string financial_product_id)
        {
            string result = "";
            if (string.IsNullOrEmpty(business_partner_id) || string.IsNullOrEmpty(financial_product_id))
            {
                return errEmpty("");
            }
            string sql = "select * from v_recommand_fp_det vf where  vf.business_partner_id=" + business_partner_id + " and vf.financial_product_id=" + financial_product_id;
            result = ExecuteDataTableSql(DZWeChatAPIConn, sql);
            return result;
        }
        #endregion

        #region 红旗优惠券

        /// <summary>
        /// 红旗订单
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static string getHqContractInfo(string StartDate, string EndDate)
        {
            var result = string.Empty;

            var sql = @"select * from IN_CMS.V_HQ_CONTRACT_INFO ";

            var whereSql = " where 1=1 ";

            if (!string.IsNullOrEmpty(StartDate))
            {
                whereSql += string.Format(" AND SUBMISSION_DATE>=to_date('{0}','yyyy-mm-dd') ", StartDate);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                whereSql += string.Format(" AND SUBMISSION_DATE<=to_date('{0}','yyyy-mm-dd') ", EndDate);
            }

            result = ExecuteDataTableSql(DZCmsMiddleConn, sql + whereSql);

            return result;
        }


        #endregion

        #region 基础
        /// <summary>
        /// 返回json数据
        /// </summary>
        /// <param name="connectionCode"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ExecuteDataTableToJsonSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                DataTable dt = command.ExecuteDataTable(sql);
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
                                JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + (dt.Rows[i][j].ToString().Replace("\"", "\\\"")) + "\",");
                            }
                            else if (j == dt.Columns.Count - 1)
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + (dt.Rows[i][j].ToString().Replace("\"", "\\\"")) + "\"");
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
                    return "{ \"errcode\": \"0\", \"errmsg\": \"\", \"data\":[" + JsonString.ToString() + "]}";
                }
                else
                {
                    return "{ \"errcode\": \"0\", \"errmsg\": \"数据为空\", \"data\":[]}";
                }
            }
            else
            {
                return "{ \"errcode\": \"1\", \"errmsg\": \"未取到数据，connectionCode错误，请联系接口供应商\", \"data\":[]}";
            }

        }
        public static string ExecuteDataTableSql(string connectionCode, string sql)
        {
            string rtn = "";
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                DataTable dt = command.ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss" };
                    //JsonConvert.SerializeObject(list.Tables[0], Formatting.Indented, timeConverter);
                    rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                    rtn = "{ \"errcode\": \"0\", \"errmsg\": \"\", \"data\":" + rtn + "}";
                }
                else
                {
                    rtn = "{ \"errcode\": \"0\", \"errmsg\": \"数据为空\", \"data\":[]}";
                }
            }
            else
            {
                rtn = "{ \"errcode\": \"1\", \"errmsg\": \"未取到数据，connectionCode错误，请联系接口供应商\", \"data\":[]}";
            }

            return rtn;
        }
        public static string ExecuteScalar(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                var rtn = command.ExecuteScalar(sql);
                if (rtn == null)
                {
                    return "";
                }
                else
                {
                    return rtn.ToString();
                }
            }
            else
            {
                return "";
                //return "{ \"errcode\": \"1\", \"errmsg\": \"未取到数据，connectionCode错误，请联系接口供应商\", \"data\":[]}";
            }

        }
        public static void ExecuteNonQuery(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                var rtn = command.ExecuteNonQuery(sql);

            }
            else
            {
                //return "{ \"errcode\": \"1\", \"errmsg\": \"未取到数据，connectionCode错误，请联系接口供应商\", \"data\":[]}";
            }

        }
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "GET";
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        result = sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="folder">文件夹相对路径</param>
        /// <param name="logName">日志文件名称</param>
        /// <param name="logInfo">日志文件内容</param>
        public static string AddLog(string logInfo)
        {
            string logName = DateTime.Now.ToString("yyyyMMdd");
            string rtn = "true";
            try
            {
                string LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\";
                if (!Directory.Exists(LogDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(LogDire);
                }
                string path = LogDire + logName + ".txt";
                if (!File.Exists(path))
                {
                    // 不存在，创建
                    FileStream fs = File.Create(path);
                    fs.Close();

                    File.WriteAllText(path, DateTime.Now.ToString() + ":" + logInfo, Encoding.Default);
                }
                else
                {
                    File.AppendAllText(path, "\r\n" + DateTime.Now.ToString() + ":" + logInfo, Encoding.Default);
                }
            }
            catch
            {
                rtn = "fase";
            }
            return rtn;
        }
        /// <summary>
        /// 参数错误
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string errEmpty(string parameter)
        {
            return "{ \"errcode\": \"1\", \"errmsg\": \"参数" + parameter + "不能为空\", \"data\":[]}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string rtnExError(Exception ex)
        {
            return "{ \"errcode\": \"1\", \"errmsg\": \"" + ex.Message + "\", \"data\":[]}";

        }
        #endregion
    }
    //集团推送消息到H3，再由H3转推给小程序
    public class salesorder
    {
        public string order_no { get; set; }
        public string order_time { get; set; }
        public string customer { get; set; }
        public string phone { get; set; }
        public string idcard_no { get; set; }
        public string business { get; set; }
        public string business_id { get; set; }
        public string asset_model_dsc { get; set; }
        public string asset_model_cde { get; set; }
        public string retail_price_amt { get; set; }
        public string financial_product_nme { get; set; }
        public string financial_product_id { get; set; }
        public string sfbl { get; set; }
        public string dkje { get; set; }
        public string ygje { get; set; }
        public string ygqx { get; set; }
        public string wkje { get; set; }
        public string wkbl { get; set; }
        public string sales { get; set; }
        public string sales_phone { get; set; }
        public string attribute1 { get; set; }
        public string attribute2 { get; set; }
        public string attribute3 { get; set; }
    }

    public class ValidateUserRequest
    {
        public string access_token { get; set; }
        public string userCode { get; set; }
        public string password { get; set; }
    }
}