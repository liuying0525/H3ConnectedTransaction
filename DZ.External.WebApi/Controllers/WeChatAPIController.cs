using DZ.External.WebApi.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DZ.External.WebApi.Controllers
{
    public class WeChatAPIController : OThinker.H3.Controllers.ControllerBase
    {
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

        #region 小程序接口
        /// <summary>
        /// 金融产品参数限制条件
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_FP_PARAMETER_MAIN(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_PARAMETER_MAIN();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 产品利率
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="financial_product_id"></param>
        /// <returns></returns>
        public ContentResult v_fp_rate(string access_token, string financial_product_id)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_fp_rate(financial_product_id);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 产品与车辆关联
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public ContentResult V_FP_ASSET(string access_token, string Financial_Product_ID)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_ASSET(Financial_Product_ID);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 产品与经销商关联
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public ContentResult V_FP_DEALER(string access_token, string Financial_Product_ID)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_DEALER(Financial_Product_ID);
            }
            catch (Exception ex)
            {
                rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 车型查询车价
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="asset_model_cde"></param>
        /// <returns></returns>
        public ContentResult V_ASSET_INFO(string access_token, string asset_model_cde)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_ASSET_INFO(asset_model_cde);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 订单申请编号查询
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="asset_model_cde"></param>
        /// <returns></returns>
        public ContentResult v_app_phone_number(string access_token, string phone_number)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_app_phone_number(phone_number);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="ID_CARD_NBR"></param>
        /// <returns></returns>
        public ContentResult V_APPLICATION_STATUS(string access_token, string ID_CARD_NBR)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_APPLICATION_STATUS(ID_CARD_NBR);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 还款计划表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="application_number"></param>
        /// <returns></returns>
        public ContentResult V_USER_PLAN_DT(string access_token, string application_number)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_USER_PLAN_DT(application_number);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 经销商与车型关联（通过经销商ID查询车型ID）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="business_partner_id"></param>
        /// <returns></returns>
        public ContentResult V_ASSET_DEALER(string access_token, string business_partner_id)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_ASSET_DEALER(business_partner_id);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 门店信息查询
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult v_dealer_info(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_dealer_info();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 金融产品，车辆，经销商 关联（小程序）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="Financial_Product_ID"></param>
        /// <param name="business_partner_id"></param>
        /// <returns></returns>
        public ContentResult V_FP_ASSET_DEALERForWechat(string access_token, string Financial_Product_ID, string business_partner_id)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_ASSET_DEALERForWechat(Financial_Product_ID, business_partner_id);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 根据门店车型查询产品
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="BUSINESS_PARTNER_ID"></param>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public ContentResult getProductByCompanyAndModel(string access_token, string BUSINESS_PARTNER_ID, string ASSET_MODEL_CDE)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.getProductByCompanyAndModel(BUSINESS_PARTNER_ID, ASSET_MODEL_CDE);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        /// <summary>
        /// 根据车型查询产品
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public ContentResult getProductByModel(string access_token, string ASSET_MODEL_CDE)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.getProductByModel(ASSET_MODEL_CDE);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        /// <summary>
        /// 根据车型产品查询门店
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="FINANCIAL_PRODUCT_ID"></param>
        /// <param name="ASSET_MODEL_CDE"></param>
        /// <returns></returns>
        public ContentResult getCompanyByModelAndProduct(string access_token, string FINANCIAL_PRODUCT_ID, string ASSET_MODEL_CDE)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.getCompanyByModelAndProduct(FINANCIAL_PRODUCT_ID, ASSET_MODEL_CDE);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        /// <summary>
        /// 车型信息查询接口
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="version_Stime"></param>
        /// <param name="version_Etime"></param>
        /// <returns></returns>
        public ContentResult v_asset_info_detail(string access_token, string version_Stime, string version_Etime)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_asset_info_detail(version_Stime, version_Etime);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 订单明细信息查询
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="application_number"></param>
        /// <returns></returns>
        public ContentResult V_APPLICATION_DET(string access_token, string application_number)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_APPLICATION_DET(application_number);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 金融专员账户信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_BUSINESS_PARTENTER_INFO(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_BUSINESS_PARTENTER_INFO();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        /// <summary>
        /// 用户验证
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ContentResult ValidateUser([FromBody]ValidateUserRequest request)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(request.access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.ValidateUser(request.userCode, request.password);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 等额本息月供计算器接口
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="D"></param>
        /// <param name="M"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public ContentResult V_PMT(string access_token, string D, string W, string M, string R)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_PMT(D, W, M, R);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// --品牌LOGO图片URL
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult v_asset_logo_info(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_asset_logo_info();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// --车系图片URL
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult v_asset_series_url_info(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_asset_series_url_info();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 产品信息(直租)
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_PRODUCT_INFO(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_PRODUCT_INFO();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 区域经销商与产品关系(直租)
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_PRODUCT_DEALER(string access_token, string azdywid)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_PRODUCT_DEALER(azdywid.Trim());
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 车辆与产品关系(直租)
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_PRODUCT_SCARMODEL(string access_token, string fld_modelid)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_PRODUCT_SCARMODEL(fld_modelid.Trim());
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 申请合同详细（直租）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="phone_number"></param>
        /// <returns></returns>
        public ContentResult V_APPLICATION_DET_INFO(string access_token, string phone_number)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_APPLICATION_DET_INFO(phone_number);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 还款计划表（直租）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="AHTHM"></param>
        /// <returns></returns>
        public ContentResult V_REPAY_PLAN(string access_token, string AHTHM)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_REPAY_PLAN(AHTHM);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 门店信息（直租）
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_RENT_DEALER_INFO(string access_token)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_RENT_DEALER_INFO();
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 获取车型（直租）
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ContentResult V_RENT_MODEL_INFO(string access_token, string fld_trimid)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_RENT_MODEL_INFO(fld_trimid);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="D"></param>
        /// <param name="W"></param>
        /// <param name="M"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public ContentResult ZPMTRENT(string access_token, string D, string W, string M, string R)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.ZPMTRENT(D, W, M, R);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        public ContentResult v_query_et_amt(string access_token, string P_AppDTE, string P_ET_DTE, string P_EXT_CTR_NO)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_query_et_amt(P_AppDTE, P_ET_DTE, P_EXT_CTR_NO);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        public ContentResult v_count_et_amt(string access_token, string P_ET_DTE, string P_EXT_CTR_NO)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_count_et_amt(P_ET_DTE, P_EXT_CTR_NO);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        public ContentResult v_app_contract_info(string access_token, string phone_number)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_app_contract_info(phone_number);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        public ContentResult getHoliday(string access_token, string holiday_dte)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.getHoliday(holiday_dte);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        #endregion

        #region 集团接口
        /// <summary>
        /// 金融产品，车辆，经销商 关联（集团）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public ContentResult V_FP_ASSET_DEALER(string access_token, string ASSET_MODEL_CDE, string BUSINESS_PARTNER_ID)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }

                result = webapi.V_FP_ASSET_DEALER(ASSET_MODEL_CDE, BUSINESS_PARTNER_ID);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 查金融产品参数限制条件 future_value_typ=R 表示支持尾款（集团）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="financial_product_id"></param>
        /// <returns></returns>
        public ContentResult V_FP_PARAMETER_MAINForJT(string access_token, string financial_product_id)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_PARAMETER_MAIN(financial_product_id);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 根据时间范围查金融产品参数限制条件
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public ContentResult V_FP_PARAMETER_MAINByDate(string access_token, string beginDate, string endDate)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_FP_PARAMETER_MAIN(beginDate, endDate);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 还款计划计算接口微信小程序，集团
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="D">贷款额</param>
        /// <param name="W">尾款</param>
        /// <param name="M">贷款期数</param>
        /// <param name="FP">产品编号</param>
        /// <returns></returns>
        public ContentResult V_PLAN_DT(string access_token, string D, string W, string M, string FP)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_PLAN_DT(D, W, M, FP);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        /// <summary>
        /// 批量查CAP申请状态（集团）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="application_numbers">申请号字串，逗号隔开</param>
        /// <returns></returns>
        public ContentResult V_APPLICATION_STATUS_ByMoreNum(string access_token, string application_numbers)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.V_APPLICATION_STATUS_ByMoreNum(application_numbers);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 根据品牌ID查失效车型
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="asset_make_cde"></param>
        /// <returns></returns>
        public ContentResult v_inactive_asset_info(string access_token, string asset_make_cde)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_inactive_asset_info(asset_make_cde);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 集团销售订单信息推送
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ContentResult salesorder_post(string access_token, [FromBody] salesorder model)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.salesorder_post(model);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 根据门店查推荐产品 
        /// </summary>
        /// <param name="asset_make_cde"></param>
        /// <returns></returns>
        public ContentResult v_recommand_fp(string access_token, string business_partner_id, string asset_model_cde)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_recommand_fp(business_partner_id, asset_model_cde);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        /// <summary>
        /// 根据门店和产品ID查推荐产品详细信息
        /// </summary>
        /// <param name="asset_make_cde"></param>
        /// <returns></returns>
        public ContentResult v_recommand_fp_det(string access_token, string business_partner_id, string financial_product_id)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }
                result = webapi.v_recommand_fp_det(business_partner_id, financial_product_id);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        #endregion

        #region 红旗优惠券项目

        /// <summary>
        /// 红旗优惠券合同信息
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public ContentResult HqCouponContractInfo(string access_token, string StartDate, string EndDate)
        {
            string result = "";
            try
            {
                Validate v = new Validate();
                if (v.ValidateToken(access_token) == false)
                {
                    return rtnTokenError();
                }

                result = webapi.getHqContractInfo(StartDate, EndDate);
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }

            return new ContentResult { Content = result, ContentType = "application/json" };
        }

        #endregion

        public ContentResult rtnTokenError()
        {
            string result = "{ \"errcode\": \"1\", \"errmsg\": \"token错误\", \"data\":[]}";
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
        public ContentResult rtnExError(Exception ex)
        {
            string result = "{ \"errcode\": \"1\", \"errmsg\": \"" + ex.Message + "\", \"data\":[]}";
            return new ContentResult { Content = result, ContentType = "application/json" };
        }
    }
}