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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

/// <summary>
/// WorkFlowFunction 的摘要说明
/// </summary>

namespace DongZheng.H3.WebApi.Common.Portal
{
    public class CRMFunction
    {
        public CRMFunction()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        #region CRM组织架构推送
        /// <summary>
        /// CRM组织架构推送
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="Instanceid"></param>
        public void postUser(string dep)
        {
            CommonFunction.WriteH3Log("CRM组织架构推送开始，部门：" + dep);
            string url = CommonFunction.getConfigurationAppSettingsKeyValue("crm_staff_url");
            string md5Key = CommonFunction.getConfigurationAppSettingsKeyValue("crm_md5_key");
            string sql = @"select b.name extend1,a.objectid third_system_no,a.code sub_account_id,
                           a.name staff_name,a.mobile mobile_phone,a.email comm_email,a.appellation extend2,
                           case when a.state=0 or a.servicestate=2 then 1 else 0 end delete_flag
                           from OT_User a join OT_OrganizationUnit b on a.parentid  = b.objectid
                           where b.name in ('{0}')
                           order by b.sortkey,a.sortkey ";
            sql = string.Format(sql, dep);
            string data = "";
            DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
            if (CommonFunction.hasData(dt))
            {
                data = DataTable2Json(dt);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM组织架构推送数据为空，部门：" + dep);
            }
            try
            {
                CommonFunction.WriteH3Log("CRM组织架构推送数据：" + data);
                data = "{\"sub_account_id\":\"admin\",\"list\":[" + data + "]}";
                string sign = CommonFunction.MD5Encrypt32(data + "&" + md5Key);
                string param = "{\"sign\":\"" + sign + "\",\"data_type\": 2,\"data\":" + data + "}";
                param = CommonFunction.get_uft8(param);
                string result = CommonFunction.PostHttp(url, param);
                CommonFunction.WriteH3Log("CRM推送订单结果：" + result);
            }
            catch (Exception ex)
            {
                CommonFunction.WriteH3Log("CRM组织架构推送异常，部门：" + dep + "，" + ex.Message);
            }
            CommonFunction.WriteH3Log("CRM组织架构推送结束，部门：" + dep);

        }
        #endregion

        #region 订单推送
        /// <summary>
        /// CRM订单推送
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="Instanceid"></param>
        public void postOrder(string bizobjectid, string appno)
        {
            CommonFunction.WriteH3Log("CRM订单推送开始：bizobjectid：" + bizobjectid);
            string url = CommonFunction.getConfigurationAppSettingsKeyValue("crm_order_url");
            string md5Key = CommonFunction.getConfigurationAppSettingsKeyValue("crm_md5_key");
            string data = getOrder(bizobjectid, appno);
            if (data != "")
            {
                try
                {
                    CommonFunction.WriteH3Log("CRM推送订单数据：" + data);
                    string sign = CommonFunction.MD5Encrypt32(data + "&" + md5Key);
                    string param = "{\"sign\":\"" + sign + "\",\"data_type\": 2,\"data\":" + data + "}";
                    param = CommonFunction.get_uft8(param);
                    string result = CommonFunction.PostHttp(url, param, 5000);
                    CommonFunction.WriteH3Log("CRM推送订单结果：" + result);
                    //推送第一个节点FI进件的已办任务
                    //string sql = @" select * from OT_workitemfinished 
                    //                where activitycode = 'Activity2' and instanceid = '{0}' order by finishtime desc";
                    //sql = string.Format(sql, Instanceid);
                    //DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
                    //if (CommonFunction.hasData(dt))
                    //{
                    //    string workitemID = dt.Rows[0][0].ToString();
                    //    postFinishedWorkItemInfo(SchemaCode, "Activity2", workitemID);
                    //}
                }
                catch (Exception ex)
                {
                    CommonFunction.WriteH3Log("CRM推送订单异常：" + bizobjectid + "，" + ex.Message);
                }
                CommonFunction.WriteH3Log("CRM订单推送结束：bizobjectid：" + bizobjectid);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM订单推送异常，未获取到数据：bizobjectid：" + bizobjectid);
            }

        }
 	public string postOrderByAdmin(string bizobjectid, string appno)
        {
            string result="";
            CommonFunction.WriteH3Log("CRM订单推送开始：bizobjectid：" + bizobjectid);
            string url = CommonFunction.getConfigurationAppSettingsKeyValue("crm_order_url");
            string md5Key = CommonFunction.getConfigurationAppSettingsKeyValue("crm_md5_key");
            string data = getOrder(bizobjectid, appno);
            if (data != "")
            {
                try
                {
                    CommonFunction.WriteH3Log("CRM推送订单数据：" + data);
                    string sign = CommonFunction.MD5Encrypt32(data + "&" + md5Key);
                    string param = "{\"sign\":\"" + sign + "\",\"data_type\": 2,\"data\":" + data + "}";
                    param = CommonFunction.get_uft8(param);
                    result = CommonFunction.PostHttp(url, param, 5000);
                    CommonFunction.WriteH3Log("CRM推送订单结果：" + result);
                }
                catch (Exception ex)
                {
                    CommonFunction.WriteH3Log("CRM推送订单异常：" + bizobjectid + "，" + ex.Message);
                    result = "CRM推送订单异常：" + bizobjectid + "，" + ex.Message;
                }
                CommonFunction.WriteH3Log("CRM订单推送结束：bizobjectid：" + bizobjectid);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM订单推送异常，未获取到数据：bizobjectid：" + bizobjectid);
                result = "CRM订单推送异常，未获取到数据：bizobjectid：" + bizobjectid;
            }
            return result;
        }
        public string getOrder(string bizobjectid, string appno)
        {
            string rtn = "";
            string sql = "";
            //SchemaCode = SchemaCode.ToUpper();
            //if (SchemaCode == "RETAILAPP")
            //{
            //    sql = @"select ir.applicationno contract_order_no,oi.objectid third_system_no,
            //            ir.ThaiFirstName extend1,ir.productGroupName extend10,ir.actualRate extend39,ir.termMonth extend26,
            //            ir.assetMakeName extend12,ir.brandName extend23,ir.modelName extend14,
            //            to_char(ir.financedamount) extend38,
            //            case  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%核准%' then '核准' 
            //                  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%拒绝%' then '拒绝' 
            //                  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%取消%' then '取消' 
            //                  else  '进行中' end extend25,
            //            'admin' sub_account_id,'0' delete_flag
            //            from I_RetailApp ir join  OT_instancecontext oi on oi.bizobjectid = ir.objectid
            //            where oi.objectid = '{0}'";
            //}
            //else if (SchemaCode == "COMPANYAPP")
            //{
            //    sql = @"select ir.applicationno contract_order_no,oi.objectid third_system_no,
            //            ir.companyNamech extend1,ir.productGroupName extend10,ir.actualRate extend39,ir.termMonth extend26,
            //            ir.assetMakeName extend12,ir.brandName extend23,ir.modelName extend14,
            //            ir.financedamount extend38,
            //            case  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%核准%' then '核准' 
            //                  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%拒绝%' then '拒绝' 
            //                  when to_char(ir. ZSSHZT || ir.YYCSSHZT || ir.YYZSSHZT ) like '%取消%' then '取消' 
            //                  else  '进行中' end extend25,
            //            'admin'  sub_account_id,'0' delete_flag
            //            from I_CompanyApp ir join  OT_instancecontext oi on oi.bizobjectid = ir.objectid
            //            where oi.objectid = '{0}' ";
            //}
            //else if (SchemaCode == "APPLICATION")
            //{
            //    sql = @"select ia.application_number contract_order_no,oi.objectid third_system_no,ia.user_name extend51,BM.BUSINESS_PARTNER_NME customer_name,
            //            ia.application_name extend1,icd.FP_GROUP_NAME extend52,icd.FINANCIAL_PRODUCT_NAME extend53,
            //            icd.ACTUAL_RTE extend39,icd.LEASE_TERM_IN_MONTH extend26,
            //            ivd.ASSET_MAKE_DSC extend12,ivd.ASSET_BRAND_DSC extend13,ivd.POWER_PARAMETER extend14,
            //            icd.AMOUNT_FINANCED extend38,
            //            case  when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%核准%' then '核准' 
            //                when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%拒绝%' then '拒绝' 
            //                when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%取消%' then '取消' 
            //                else  '进行中' end extend25,
            //            'admin' sub_account_id,'0' delete_flag
            //            from I_application ia join OT_instancecontext oi on oi.bizobjectid = ia.objectid                              
            //            join I_APPLICANT_TYPE iat on ia.objectid = iat.parentobjectid and ia.application_name = iat.name1 
            //            left join I_CONTRACT_DETAIL icd on ia.objectid = icd.parentobjectid
            //            left join I_APPLICANT_DETAIL iad on  ia.objectid = iad.parentobjectid and iat.IDENTIFICATION_CODE1 = iad.IDENTIFICATION_CODE2 
            //            left join I_VEHICLE_DETAIL ivd on  ia.objectid = ivd.parentobjectid  and iat.IDENTIFICATION_CODE1 = ivd.IDENTIFICATION_CODE7 
            //            left join I_APPLICANT_PHONE_FAX iapf on  ia.objectid = iapf.parentobjectid  and iat.IDENTIFICATION_CODE1 = iapf.IDENTIFICATION_CODE5 
            //            left join BP_MAIN@TO_CMS BM ON BM.BUSINESS_PARTNER_ID = ia.BUSINESS_PARTNER_ID
            //            where oi.bizobjectid = '{0}'";
            //}
            sql = @"select  distinct  '{0}' contract_order_no,oi.objectid third_system_no,ia.user_name extend51,BM.BUSINESS_PARTNER_NME customer_name,
                        ia.application_name extend1,icd.FP_GROUP_NAME extend52,icd.FINANCIAL_PRODUCT_NAME extend53,
                        icd.ACTUAL_RTE extend39,icd.LEASE_TERM_IN_MONTH extend26,
                        ivd.ASSET_MAKE_DSC extend12,ivd.ASSET_BRAND_DSC extend13,ivd.POWER_PARAMETER extend14,
                        icd.AMOUNT_FINANCED extend38,
                        case  when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%核准%' then '核准' 
                            when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%拒绝%' then '拒绝' 
                            when to_char(ia. ZSSHZT || ia.YYCSSHZT || ia.YYZSSHZT ) like '%取消%' then '取消' 
                            else  '进行中' end extend25,
                        'admin' sub_account_id,'0' delete_flag
                        from I_application ia join OT_instancecontext oi on oi.bizobjectid = ia.objectid                              
                        join I_APPLICANT_TYPE iat on ia.objectid = iat.parentobjectid and ia.application_name = iat.name1 
                        left join I_CONTRACT_DETAIL icd on ia.objectid = icd.parentobjectid
                        left join I_APPLICANT_DETAIL iad on  ia.objectid = iad.parentobjectid and iat.IDENTIFICATION_CODE1 = iad.IDENTIFICATION_CODE2 
                        left join I_VEHICLE_DETAIL ivd on  ia.objectid = ivd.parentobjectid  and iat.IDENTIFICATION_CODE1 = ivd.IDENTIFICATION_CODE7 
                        left join I_APPLICANT_PHONE_FAX iapf on  ia.objectid = iapf.parentobjectid  and iat.IDENTIFICATION_CODE1 = iapf.IDENTIFICATION_CODE5 
                        left join BP_MAIN@TO_CMS BM ON BM.BUSINESS_PARTNER_ID = ia.BUSINESS_PARTNER_ID
                        where oi.bizobjectid = '{1}'";
            sql = string.Format(sql, appno, bizobjectid);
            DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = DataTable2Json(dt);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM推送订单异常，订单数据为空，bizobjectid：" + bizobjectid);
            }
            return rtn;
        }
        #endregion

        #region 待办任务推送
        /// <summary>
        /// 待办任务推送
        /// </summary>
        /// <param name="workitemID"></param>
        public void postUnFinishedWorkItemInfo(string SchemaCode, string workitemID)
        {
            return ;
            string url = CommonFunction.getConfigurationAppSettingsKeyValue("crm_auditremark_url");
            string md5Key = CommonFunction.getConfigurationAppSettingsKeyValue("crm_md5_key");
            string data = getUnFinishedWorkItem(SchemaCode, workitemID);
            if (data != "")
            {
                try
                {
                    CommonFunction.WriteH3Log("CRM推送待办任务数据：" + data);
                    string sign = CommonFunction.MD5Encrypt32(data + "&" + md5Key);
                    string param = "{\"sign\":\"" + sign + "\",\"data_type\": 2,\"data\":" + data + "}";
                    param = CommonFunction.get_uft8(param);
                    string result = CommonFunction.PostHttp(url, param);
                    CommonFunction.WriteH3Log("CRM推送待办任务结果：" + result);
                }
                catch (Exception ex)
                {
                    CommonFunction.WriteH3Log("CRM推送待办任务异常，SchemaCode：" + SchemaCode + "，Instanceid：" + workitemID + "，" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 获取待办任务信息
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="workitemID"></param>
        /// <returns></returns>
        public string getUnFinishedWorkItem(string SchemaCode, string workitemID)
        {
            string rtn = "";
            string sql = "";
            sql = @"select ow.objectid third_system_no,
                        ins.objectid link_no,
                        12 deal_kind,
                        ow.displayname extend3,
                        '' audit_remark,
                        ow.state extend2,
                        '' audit_date,
                        ou.name || '(' || ou.code || ')' extend4 ,
                        ou.code extend6,
                        'admin' sub_account_id ,
                        ow.receivetime extend46,
                        ow.displayname extend1,
                        0 delete_flag
                        from OT_WorkItem ow join OT_User ou on ou.objectid = ow.participant
                        join Ot_Instancecontext ins on ins.objectid = ow.instanceid
                        join i_{0} ir on ir.objectid = ins.bizobjectid
                        where ow.objectid = '{1}'";
            sql = string.Format(sql, SchemaCode, workitemID);
            DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = DataTable2Json(dt);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM推送待办任务异常，数据为空，SchemaCode：" + SchemaCode + "，Instanceid：" + workitemID);
            }
            return rtn;
        }
        #endregion

        #region 已办任务推送
        /// <summary>
        /// 已办任务推送
        /// </summary>
        /// <param name="workitemID"></param>
        public void postFinishedWorkItemInfo(string SchemaCode, string activity, string workitemID)
        {
	    return ;
            string url = CommonFunction.getConfigurationAppSettingsKeyValue("crm_auditremark_url");
            string md5Key = CommonFunction.getConfigurationAppSettingsKeyValue("crm_md5_key");
            string data = getFinishedWorkItem(SchemaCode, activity, workitemID);
            if (data != "")
            {
                try
                {
                    CommonFunction.WriteH3Log("CRM推送已办任务数据：" + data);
                    string sign = CommonFunction.MD5Encrypt32(data + "&" + md5Key);
                    string param = "{\"sign\":\"" + sign + "\",\"data_type\": 2,\"data\":" + data + "}";
                    param = CommonFunction.get_uft8(param);
                    string result = CommonFunction.PostHttp(url, param);
                    CommonFunction.WriteH3Log("CRM推送已办任务结果：" + result);
                }
                catch (Exception ex)
                {
                    CommonFunction.WriteH3Log("CRM推送已办任务异常，SchemaCode：" + SchemaCode + "，Instanceid：" + workitemID + "，" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 获取已办任务信息
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="workitemID"></param>
        /// <returns></returns>
        public string getFinishedWorkItem(string SchemaCode, string activity, string workitemID)
        {
            string rtn = "";
            string sql = "";
            string zd = "";
            //判断是否是审核节点
            if (activity == "Activity14" || activity == "Activity13" ||
                activity == "Activity55" || activity == "Activity17" || activity == "Activity18")
            {
                switch (activity)
                {
                    case "Activity14": zd = "csshzt"; break;
                    case "Activity13": zd = "zsshzt"; break;
                    case "Activity55": zd = "yyysshzt"; break;
                    case "Activity17": zd = "yycsshzt"; break;
                    case "Activity18": zd = "yyzsshzt"; break;
                }
                sql = @"select * from (select ow.objectid third_system_no,
                        ins.objectid link_no,
                        12 deal_kind,
                        ow.displayname extend3,
                        oc.text audit_remark,
                        ir.{0} extend2,
                        ow.finishtime audit_date,
                        ou.name || '(' || ou.code || ')' extend4 ,
                        ou.code extend6,
                        'admin' sub_account_id ,
                        ow.receivetime extend46,
                        ow.displayname extend1,
                        0 delete_flag,
                        ROUND(TO_NUMBER(ow.finishtime - ow.receivetime) * 24*60) extend5
                        from OT_WorkItemFinished ow join OT_User ou on ou.objectid = ow.participant
                        join Ot_Instancecontext ins on ins.objectid = ow.instanceid
                        join i_{1} ir on ir.objectid = ins.bizobjectid
                        left join OT_Comment oc on ow.activitycode = oc.activity and ins.objectid = oc.instanceid
                        where ow.objectid = '{2}'
                        order by oc.modifiedtime desc) a where rownum =1";
                sql = string.Format(sql, zd, SchemaCode, workitemID);
            }
            else
            {
                sql = @"select ow.objectid third_system_no,
                        ins.objectid link_no,
                        12 deal_kind,
                        ow.displayname extend3,
                        '' audit_remark,
                        case  when  ow.actionname = 'Submit' or ow.actionname is null then  '提交' when  ow.actionname ='Reject' then '驳回' else '' end  extend2,
                        ow.finishtime audit_date,
                        ou.name || '(' || ou.code || ')' extend4 ,
                        ou.code extend6,
                        'admin' sub_account_id ,
                        ow.receivetime extend46,
                        ow.displayname extend1,
                        0 delete_flag,
                        ROUND(TO_NUMBER(ow.finishtime - ow.receivetime) * 24*60) extend5
                        from OT_WorkItemFinished ow join OT_User ou on ou.objectid = ow.participant
                        join Ot_Instancecontext ins on ins.objectid = ow.instanceid
                        join i_{0} ir on ir.objectid = ins.bizobjectid
                        where ow.objectid = '{1}'";
                sql = string.Format(sql, SchemaCode, workitemID);
            }
            CommonFunction.WriteH3Log("CRM推送已办任务异常，数据记录：activity：" + activity + "，workitemID：" + workitemID + "，zd：" + zd + "，sql：" + sql);
            DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = DataTable2Json(dt);
            }
            else
            {
                CommonFunction.WriteH3Log("CRM推送已办任务异常，数据为空，SchemaCode：" + SchemaCode + "，Instanceid：" + workitemID);
            }
            return rtn;
        }
        #endregion

        /// <summary>
        /// table数据转换json
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2Json(System.Data.DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName.ToLower());
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\"")); //对于特殊字符，还应该进行特别的处理。
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            return jsonBuilder.ToString();
        }
    }
    public class CRM_Order
    {
        private string bizobjectid;
        private string appno;
        public CRM_Order(string biz_objectid, string application_no)
        {
            bizobjectid = biz_objectid;
            appno = application_no;
        }
        public void PostOrder()
        {
            CRMFunction cRMFunction = new CRMFunction();
            cRMFunction.postOrder(bizobjectid, appno);
        }
    }
}