using DZ.External.WebApi.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DZ.External.WebApi.Controllers
{
    public class CRMAPIController : OThinker.H3.Controllers.ControllerBase
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
        /// <summary>
        /// 金融产品，车辆，经销商 关联（集团）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="Financial_Product_ID"></param>
        /// <returns></returns>
        public ContentResult getItemInfo(string link_no)
        {
            string result = "";
            try
            {
                string sql = @"select * from ( 
                select ow.objectid third_system_no,
                        ins.objectid link_no,
                        12 deal_kind,
                        ow.displayname extend3,
                        case  when  ow.displayname ='信审（初审）' then '' else to_char(oc.text) end audit_remark,
                        case  when  ow.actionname = 'Submit' and ow.displayname ='信审（初审）' then   to_char(ir.csshzt)
                              when  ow.actionname = 'Submit' and ow.displayname ='信审（终审）' then  to_char(ir.zsshzt)
                              when  ow.actionname = 'Submit' and ow.displayname ='运营人员（初审）' then  to_char(ir.yycsshzt)
                              when  ow.actionname = 'Submit' and ow.displayname ='运营人员（终审）' then  to_char(ir.yyzsshzt)
                              when  ow.actionname = 'Submit' or ow.actionname is null then  '提交'
                              when  ow.actionname = 'Reject' then '驳回' 
                              else  to_char(ow.actionname)  end  extend2,
                        to_char(ow.finishtime,'yyyy-MM-dd HH24:mi:ss')  audit_date,
                        ou.name || '(' || ou.code || ')' extend4 ,
                        ou.code extend6,
                        'admin' sub_account_id ,
                        ow.receivetime extend46,
                        ow.displayname extend1,
                        0 delete_flag,
                        ROUND(TO_NUMBER(ow.finishtime - ow.receivetime) * 24*60) extend5
                        from OT_WorkItemFinished ow join OT_User ou on ou.objectid = ow.participant
                        join Ot_Instancecontext ins on ins.objectid = ow.instanceid
                        join i_Application ir on ir.objectid = ins.bizobjectid
                        left join OT_Comment oc on ow.activitycode = oc.activity and ins.objectid = oc.instanceid and oc.tokenid = ow.tokenid
                        where ins.objectid = '{0}'
           union all             
                        select ow.objectid third_system_no,
                        ins.objectid link_no,
                        12 deal_kind,
                        ow.displayname extend3,
                        '' audit_remark,
                        '处理中' extend2,
                        '' audit_date,
                        ou.name || '(' || ou.code || ')' extend4 ,
                        ou.code extend6,
                        'admin' sub_account_id ,
                        ow.receivetime extend46,
                        ow.displayname extend1,
                        0 delete_flag,
                        0 extend5
                        from OT_WorkItem ow join OT_User ou on ou.objectid = ow.participant
                        join Ot_Instancecontext ins on ins.objectid = ow.instanceid
                        join i_Application ir on ir.objectid = ins.bizobjectid
                        where ins.objectid = '{0}'
                        
                        ) a order by a.extend46 ";
                sql = string.Format(sql, link_no);
                result = ExecuteDataTableSql(sql);
                return new ContentResult { Content = result, ContentType = "application/json" };
            }
            catch (Exception ex)
            {
                return rtnExError(ex);
            }
            
        }

        public static string ExecuteDataTableSql(string sql)
        {
            string rtn = "";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss" };
                //rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter);
                rtn = DataTable2Json(dt);
                rtn = "{ \"errcode\": \"0\", \"errmsg\": \"\", \"data\":[" + rtn + "]}";
            }
            else
            {
                rtn = "{ \"errcode\": \"0\", \"errmsg\": \"数据为空\", \"data\":[]}";
            }
            return rtn;
        }
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
}