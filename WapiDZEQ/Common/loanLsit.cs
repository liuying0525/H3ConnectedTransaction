using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WebapiDZEQ.Common
{
    public class loanLsit : ControllerBase
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

        public DataTable Getuserdata()
        {
            string sql = "select name,code,objectid from  OT_user";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

        }


        public DataSet GetLoanList(PagerInfo pagerInfo, string FrameNo, string Models, string state, string Manufacturer, string FrameNoNature, string address, string startDate, string endDate)
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format("select PARENTOBJECTID ,JXS ,DKBH , CLBH , CX ,DKSQSJ , YS , CJHXZ, CJHLS, CJHYJ, DKJE,  SQZT , ZZS ,DZ  from I_CLXX where  jxs='{0}' ", UserID);
            if (!string.IsNullOrEmpty(FrameNo))
            {
                sql += string.Format("AND ( CJHLS like '%{0}%' OR CJHYJ like '%{0}%' )", FrameNo);
            }
            if (!string.IsNullOrEmpty(Models))
            {
                sql += string.Format("AND CX like '%{0}%' ", Models);
            }
            if (!string.IsNullOrEmpty(state))
            {
                sql += string.Format("AND SQZT   like '%{0}%' ", state);
            }
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                sql += string.Format("AND ZZS like '%{0}%' ", Manufacturer);
            }
            if (!string.IsNullOrEmpty(FrameNoNature))
            {
                sql += string.Format("AND CJHXZ like '%{0}%' ", FrameNoNature);
            }
            if (!string.IsNullOrEmpty(address))
            {
                sql += string.Format("AND DZ  like ='{0}' ", address);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                sql += string.Format("AND DKSQSJ>to_date('{0}','yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                sql += string.Format("AND DKSQSJ<to_date('{0}','yyyy-mm-dd')\r\n", endDate);

            }
            DataSet ds = new DataSet();
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn > " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlstr);
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;
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