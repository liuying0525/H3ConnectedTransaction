using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.Util;
using DongZheng.H3.WebApi.Models.ProductCenter;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.ProductCenter
{
    [ValidateInput(false)]
    [Xss]
    public class ProductCenterController : CustomController
    {
        public override string FunctionCode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 产品查询数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProductQuery(ProductQueryRequest request)
        {
            var totalNumber = 0;

            var dt = GetProductQuery(request,ref totalNumber);

            var data = BuildProductQueryModel(dt);

            GridViewModel<ProductQueryViewModel> result = new GridViewModel<ProductQueryViewModel>(totalNumber, data, request.sEcho);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产品查询导出excel
        /// </summary>
        /// <param name="ProductName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public System.Web.Mvc.ActionResult ProductQueryExport(string ProductName,string StartTime,string EndTime)
        {
            var totalNumber = 0;

            var dt = GetProductQuery(new ProductQueryRequest { ProductName = ProductName, StartTime = StartTime, EndTime = EndTime }, ref totalNumber);

            if (dt.Rows.Count > 10000)
            {
                rtn_data rtn = new rtn_data();
                return Json(new rtn_data { code = 0, message = "无法导出超过1万条数据" }, JsonRequestBehavior.AllowGet);
            }

            var headers = new List<string>() { "序号", "产品ID", "产品名称", "产品组", "产品组ID", "产品有效期始", "产品有效期至", "更新日期", "客户类型", "最小融资额", "最大融资额", "最小租期", "最大租期", "最大融资比例", "尾款", "最小残值比例", "最大残值比例", "最小贷款期数", "最大贷款期数", "残值百分比", "应用残值", "客户利率", "实际利率", "补贴率", "能否编辑残值", "提前终止罚金比例", "提前结清","还款方式","补贴经销商","补贴制造商" };

            var book = NPOIHelper.Export(dt,string.Empty, headers);

            NpoiMemoryStream ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;

            return File(ms, "application/vnd.ms-excel", "产品"+DateTime.Now.ToString("yyyyMMddHHmmss")+".xlsx");
        }

        /// <summary>
        /// 产品/车型查询数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProductCarTypeQuery(ProductCarTypeQueryRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductName) && string.IsNullOrEmpty(request.CarType))
            {
                return Json(new GridViewModel<ProductCarTypeQueryViewModel>(0, new List<ProductCarTypeQueryViewModel>(), request.sEcho), JsonRequestBehavior.AllowGet);
            }

            var totalNumber = 0;

            var dt = GetProductCarTypeQuery(request, ref totalNumber);

            var data = BuildProductCarTypeQueryModel(dt);

            GridViewModel<ProductCarTypeQueryViewModel> result = new GridViewModel<ProductCarTypeQueryViewModel>(totalNumber, data, request.sEcho);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产品/车型查询导出excel
        /// </summary>
        /// <param name="ProductName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public System.Web.Mvc.ActionResult ProductCarTypeQueryExport(string ProductName, string CarType)
        {
            var totalNumber = 0;

            var dt = GetProductCarTypeQuery(new ProductCarTypeQueryRequest { ProductName = ProductName, CarType = CarType }, ref totalNumber);

            if (dt.Rows.Count > 10000)
            {
                rtn_data rtn = new rtn_data();
                return Json(new rtn_data { code = 0, message = "无法导出超过1万条数据" }, JsonRequestBehavior.AllowGet);
            }

            var headers = new List<string>() { "序号", "产品名称", "产品ID", "制造商", "制造商ID", "品牌", "品牌ID", "车型", "车型ID","价格" };

            var book = NPOIHelper.Export(dt, string.Empty, headers);

            NpoiMemoryStream ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;

            return File(ms, "application/vnd.ms-excel", "产品-车型" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        /// <summary>
        /// 产品/经销商查询数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProductDealerQuery(ProductDealerQueryRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductName) && string.IsNullOrEmpty(request.DealerName))
            {
                return Json(new GridViewModel<ProductDealerQueryViewModel>(0, new List<ProductDealerQueryViewModel>(), request.sEcho), JsonRequestBehavior.AllowGet);
            }

            var totalNumber = 0;

            var dt = GetProductDealerQuery(request, ref totalNumber);

            var data = BuildProductDealerQueryModel(dt);

            GridViewModel<ProductDealerQueryViewModel> result = new GridViewModel<ProductDealerQueryViewModel>(totalNumber, data, request.sEcho);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 产品/经销商查询导出excel
        /// </summary>
        /// <param name="ProductName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpGet]
        public System.Web.Mvc.ActionResult ProductDealerQueryExport(string ProductName, string DealerName)
        {
            var totalNumber = 0;

            var dt = GetProductDealerQuery(new ProductDealerQueryRequest { ProductName = ProductName, DealerName = DealerName }, ref totalNumber);

            if (dt.Rows.Count > 10000)
            {
                rtn_data rtn = new rtn_data();
                return Json(new rtn_data { code = 0, message = "无法导出超过1万条数据" }, JsonRequestBehavior.AllowGet);
            }

            var headers = new List<string>() { "序号", "产品组", "产品组ID", "产品名称", "产品ID", "公司名称", "公司ID" };

            var book = NPOIHelper.Export(dt, string.Empty, headers);

            NpoiMemoryStream ms = new NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;

            return File(ms, "application/vnd.ms-excel", "产品-经销商" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        /// <summary>
        /// 所有产品查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult AllProductsQuery()
        {
            var dt = GetProductAllQuery();

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 经销商查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult AllDealersQuery(string dealerName)
        {
            var dt = GetDealersAllQuery(dealerName);

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加关联
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddRelation(ProductDealerRelationRequest request)
        {
            rtn_data rtn = new rtn_data();

            try
            {
                request.Dealers.ForEach(dealer =>
                {
                    request.Products.ForEach(product =>
                    {
                        ProductDealerAddRelation(dealer.DealerName, product.ProductName);
                    });
                });
                rtn.code = 1;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("添加关联错误：" + ex.ToString());
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 解除关联
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveRelation(ProductDealerRelationRequest request)
        {
            rtn_data rtn = new rtn_data();

            try
            {
                request.Dealers.ForEach(dealer =>
                {
                    request.Products.ForEach(product =>
                    {
                        ProductDealerRemoveRelation(dealer.DealerName, product.ProductName);
                    });
                });
                rtn.code = 1;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("解除关联错误：" + ex.ToString());
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        #region build Model

        /// <summary>
        /// 产品查询返回Model
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<ProductQueryViewModel> BuildProductQueryModel(DataTable dt)
        {
            var list = new List<ProductQueryViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProductQueryViewModel
                {
                    RowIndex = Convert.ToInt32(row["rn"].ToString()),
                    ProductId = row["financial_product_id"].ToString(),
                    ProductName = row["financial_product_nme"].ToString(),
                    GroupId = row["FP_GROUP_ID"].ToString(),
                    GroupName = row["FP_GROUP_NME"].ToString(),
                    StartTime = Convert.ToDateTime(row["VALID_FROM_DTE"].ToString()).ToString("yyyy-MM-dd"),
                    EndTime = Convert.ToDateTime(row["VALID_TO_DTE"].ToString()).ToString("yyyy-MM-dd"),
                    ModifyTime = row["Execution_Dte"].ToString(),
                    FLEET_COMP_INDIV_IND = row["FLEET_COMP_INDIV_IND"].ToString(),
                    MinFinancingAmount = row["MINIMUM_FINANCING_AMT"].ToString(),
                    MaxFinancingAmount = row["MAXIMUM_FINANCING_AMT"].ToString(),
                    MinLeaseTrm = row["MINIMUM_LEASE_TRM"].ToString(),
                    MaxLeaseTrm = row["MAXIMUN_LEASE_TRM"].ToString(),
                    MaxFinancingPCT = row["MAXIMUM_FINANCING_PCT"].ToString(),
                    FutureValueType = row["Future_Value_Typ"].ToString(),
                    MinimumRvPct = row["minimum_rv_pct"].ToString(),
                    MaximumRvPct = row["maximum_rv_pct"].ToString(),
                    MinimumTermMm = row["minimum_term_mm"].ToString(),
                    MaximumTermMm = row["maximum_term_mm"].ToString(),
                    RvPCT = row["RV_PCT"].ToString(),
                    RvGuaranteeAmt = row["Rv_Guarantee_Amt"].ToString(),
                    CustomerRate = string.Format("{0}%", row["CUSTOMER_RTE"].ToString()),
                    ActualRate = string.Format("{0}%", row["ACTUAL_RTE"].ToString()),
                    SubsidyRate = string.Format("{0}%", row["SUBSIDY_RTE"].ToString()),
                    RvEditableInd = row["rv_editable_ind"].ToString(),
                    EndNonactrebatePct = row["et_nonactrebate_pct"].ToString(),
                    EndPenaltyInd = row["et_penalty_ind"].ToString()
                });
            }

            return list;
        }

        /// <summary>
        /// 产品/车型查询返回Model
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<ProductCarTypeQueryViewModel> BuildProductCarTypeQueryModel(DataTable dt)
        {
            var list = new List<ProductCarTypeQueryViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProductCarTypeQueryViewModel
                {
                    RowIndex = Convert.ToInt32(row["rn"].ToString()),
                    ProductId = row["Financial_Product_Id"].ToString(),
                    ProductName = row["Financial_Product_Nme"].ToString(),
                    AssetMake = row["ASSET_MAKE_DSC"].ToString(),
                    AssetMakeCode = row["ASSET_MAKE_CDE"].ToString(),
                    AssetBrand = row["ASSET_BRAND_DSC"].ToString(),
                    AssetBrandCode = row["Asset_Brand_Cde"].ToString(),
                    AssetModel = row["ASSET_MODEL_DSC"].ToString(),
                    AssetModelCode = row["Asset_Model_Cde"].ToString(),
                    Price = Convert.ToDecimal(row["LIST_PRICE_AMT"].ToString()).ToString("C")
                });
            }

            return list;
        }


        /// <summary>
        /// 产品/经销商查询返回Model
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<ProductDealerQueryViewModel> BuildProductDealerQueryModel(DataTable dt)
        {
            var list = new List<ProductDealerQueryViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ProductDealerQueryViewModel
                {
                    RowIndex = Convert.ToInt32(row["rn"].ToString()),
                    ProductId = row["Financial_Product_Id"].ToString(),
                    ProductName = row["Financial_Product_Nme"].ToString(),
                    GroupId= row["fp_group_id"].ToString(),
                    GroupName= row["fp_group_nme"].ToString(),
                    CompanyId= row["business_partner_id"].ToString(),
                    CompanyName= row["COMPANY_NME"].ToString()
                });
            }

            return list;
        }
        #endregion


        #region sql 

        /// <summary>
        /// 产品查询SQL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        public DataTable GetProductQuery(ProductQueryRequest request, ref int totalNumber)
        {
            var countSQl = @"select count(1) from IN_CMS.V_FP_PARAMETER_MAIN_ALL v where 1=1 {0}";

            var sql = @"with cte as(
                            select rownum rn,v.*
                            from IN_CMS.V_FP_PARAMETER_MAIN_ALL v where 1=1 {0}
                        ) 
                        select  c.rn
                                ,c.financial_product_id
                                ,c.financial_product_nme
                                ,c.FP_GROUP_NME --产品组
                                ,c.FP_GROUP_ID  --产品组ID
                                ,c.VALID_FROM_DTE --产品有效期始
                                ,c.VALID_TO_DTE --产品有效期至
                                ,c.Execution_Dte --更新日期
                                ,c.FLEET_COMP_INDIV_IND
                                ,c.MINIMUM_FINANCING_AMT --最小融资额
                                ,c.MAXIMUM_FINANCING_AMT --最大融资额
                                ,c.MINIMUM_LEASE_TRM --最小租期
                                ,c.MAXIMUN_LEASE_TRM --最大租期
                                ,c.MAXIMUM_FINANCING_PCT --最大融资比例
                                ,c.Future_Value_Typ --尾款 R 表示支持尾款
                                ,c.minimum_rv_pct
                                ,c.maximum_rv_pct
                                ,c.minimum_term_mm
                                ,c.maximum_term_mm
                                ,c.RV_PCT --残值百分比
                                ,c.Rv_Guarantee_Amt
                                ,c.CUSTOMER_RTE --客户利率
                                ,c.ACTUAL_RTE --实际利率
                                ,c.SUBSIDY_RTE --补贴率
                                ,c.rv_editable_ind
                                ,c.et_nonactrebate_pct --提前终止罚金比例
                                ,c.et_penalty_ind 
                                ,c.REPAYMENT_METHOD 
                                ,c.DEALER_SUBSIDY_PCT
                                ,c.MANUFACTURER_SUBSIDY_PCT
                                from cte c ";

            var conditions = string.Empty;

            if (!string.IsNullOrEmpty(request.ProductName))
            {
                conditions += string.Format(" And v.financial_product_nme like '%{0}%' ", request.ProductName);
            }
            if (!string.IsNullOrEmpty(request.StartTime))//接收时间的开始时间范围
            {
                conditions += string.Format(" AND  v.VALID_FROM_DTE>=to_date('{0}','yyyy-mm-dd')\r\n", request.StartTime);
            }
            if (!string.IsNullOrEmpty(request.EndTime))//接收时间的结束时间范围
            {
                conditions += string.Format(" AND v.VALID_TO_DTE<=to_date('{0}','yyyy-mm-dd')\r\n", request.EndTime);
            }
            if (request.StartIndex > 0 && request.EndIndex > 0)
            {
                sql += " where c.rn >={1} and c.rn<={2}";
                sql = string.Format(sql, conditions, request.StartIndex, request.EndIndex);
            }
            else
            {
                sql = string.Format(sql, conditions);
            }

            countSQl = string.Format(countSQl, conditions);

            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);
            var num = CommonFunction.ExecuteScalar("CAPDB", countSQl);
            totalNumber = Convert.ToInt32(num);
            return dt;
        }

        /// <summary>
        /// 所有产品查询SQL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        public DataTable GetProductAllQuery()
        {
            var sql = @"select distinct v.financial_product_id as ProductId,v.financial_product_nme as ProductName  from IN_CMS.V_FP_PARAMETER_MAIN_ALL v ";

            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);
            
            return dt;
        }

        /// <summary>
        /// 产品/车型查询查询SQL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        public DataTable GetProductCarTypeQuery(ProductCarTypeQueryRequest request, ref int totalNumber)
        {
            var countSQl = @"select count(1) from IN_CMS.V_FP_ASSET v where 1=1 {0}";

            var sql = @"with cte as(
                            select rownum rn,v.*
                            from IN_CMS.V_FP_ASSET v where 1=1 {0}
                        ) 
                        select * from cte c ";

            var conditions = string.Empty;

            if (!string.IsNullOrEmpty(request.ProductName))
            {
                conditions += string.Format(" And v.Financial_Product_Nme like '%{0}%' ", request.ProductName);
            }
            if (!string.IsNullOrEmpty(request.CarType))
            {
                conditions += string.Format(" AND  v.ASSET_MODEL_DSC like '%{0}%' ", request.CarType);
            }

            if (request.StartIndex > 0 && request.EndIndex > 0)
            {
                sql += " where c.rn >={1} and c.rn<={2}";
                sql = string.Format(sql, conditions, request.StartIndex, request.EndIndex);
            }
            else
            {
                sql = string.Format(sql, conditions);
            }


            countSQl = string.Format(countSQl, conditions);

            sql = string.Format(sql, conditions, request.StartIndex, request.EndIndex);

            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);
            var num = CommonFunction.ExecuteScalar("CAPDB", countSQl);
            totalNumber = Convert.ToInt32(num);
            return dt;
        }


        /// <summary>
        /// 产品/经销商查询查询SQL
        /// </summary>
        /// <param name="request"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        public DataTable GetProductDealerQuery(ProductDealerQueryRequest request, ref int totalNumber)
        {
            var countSQl = @"select count(1) from IN_CMS.V_FP_DEALER v where 1=1 {0}";

            var sql = @"with cte as(
                            select rownum rn,v.*
                            from IN_CMS.V_FP_DEALER v where 1=1 {0}
                        ) 
                        select * from cte c ";

            var conditions = string.Empty;

            if (!string.IsNullOrEmpty(request.ProductName))
            {
                conditions += string.Format(" And v.FINANCIAL_PRODUCT_NME like '%{0}%' ", request.ProductName);
            }
            if (!string.IsNullOrEmpty(request.DealerName))
            {
                conditions += string.Format(" AND  v.COMPANY_NME like '%{0}%' ", request.DealerName);
            }

            if (request.StartIndex > 0 && request.EndIndex > 0)
            {
                sql += " where c.rn >={1} and c.rn<={2}";
                sql = string.Format(sql, conditions, request.StartIndex, request.EndIndex);
            }
            else
            {
                sql = string.Format(sql, conditions);
            }

            countSQl = string.Format(countSQl, conditions);

            sql = string.Format(sql, conditions, request.StartIndex, request.EndIndex);

            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);
            var num = CommonFunction.ExecuteScalar("CAPDB", countSQl);
            totalNumber = Convert.ToInt32(num);
            return dt;
        }


        /// <summary>
        /// 查询所有经销商
        /// </summary>
        /// <param name="dealerName"></param>
        /// <returns></returns>
        public DataTable GetDealersAllQuery(string dealerName)
        {
            //var sql = @"select u.code,u.appellation,u.name from h3.ot_user u join h3.ot_organizationunit o on u.parentid=o.objectid where u.state =1 and  (o.name like '%内网经销商%' or o.name like '%外网经销商%') ";

            var sql = @"select business_partner_id as BusinessPartnerId,business_partner_nme as BusinessPartnerName from IN_CMS.v_dealer_info ";

            if (!string.IsNullOrEmpty(dealerName))
            {
                //sql += string.Format(" and u.appellation like '%{0}%' ", dealerName);
                sql += string.Format(" where business_partner_nme like '%{0}%' ", dealerName);
            }
            //var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            DataTable dt = CommonFunction.ExecuteDataTableSql("CAPDB", sql);
            return dt;
        }


        public void ProductDealerAddRelation(string dealerName, string productName)
        {
            OracleParameter[] para1 = new OracleParameter[]
            {
                new OracleParameter("dealer_name", OracleType.VarChar, 200),
                new OracleParameter("fp_name", OracleType.VarChar, 200),
                new OracleParameter("o_result_code", OracleType.Number),
            };

            para1[0].Direction = ParameterDirection.Input;
            para1[1].Direction = ParameterDirection.Input;
            para1[2].Direction = ParameterDirection.Output;
            para1[0].Value = dealerName;
            para1[1].Value = productName;

            var result = CommonFunction.ExecuteProcedure("CAPDB", "ZRELATE_DEALER_FP", para1);

            AppUtility.Engine.LogWriter.Write("添加关联返回：" + (result[2].Value + string.Empty) + "dealerName=" + dealerName + "productName=" + productName);
        }

        public void ProductDealerRemoveRelation(string dealerName, string productName)
        {
            OracleParameter[] para1 = new OracleParameter[]
            {
                new OracleParameter("dealer_name", OracleType.VarChar, 200),
                new OracleParameter("fp_name", OracleType.VarChar, 200),
                new OracleParameter("o_result_code", OracleType.Number),
            };

            para1[0].Direction = ParameterDirection.Input;
            para1[1].Direction = ParameterDirection.Input;
            para1[2].Direction = ParameterDirection.Output;
            para1[0].Value = dealerName;
            para1[1].Value = productName;

            var result = CommonFunction.ExecuteProcedure("CAPDB", "ZDELETE_DEALER_FP", para1);

            AppUtility.Engine.LogWriter.Write("解除关联返回：" + (result[2].Value + string.Empty) + "dealerName=" + dealerName + "productName=" + productName);
        }

        #endregion

    }

}