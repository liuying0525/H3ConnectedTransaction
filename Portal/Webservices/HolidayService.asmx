<%@ WebService Language="C#" Class="OThinker.H3.Portal.HolidayService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Data;
using OThinker.H3.Controllers;

namespace OThinker.H3.Portal
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class HolidayService : System.Web.Services.WebService
    {
        /// <summary>
        /// 添加假期工时，目录只支持加班及调休
        /// </summary>
        /// <param name="userCode">H3用户登录名</param>
        /// <param name="holidayType">固定值：加班、调休</param>
        /// <param name="hours">工时，数值类型</param>
        /// <param name="instanceid">流程实例ID</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [WebMethod(Description = "添加假期工时，目录只支持加班及调休")]
        public Result AddHoliday(string userCode, string holidayType, string hours, string instanceid, string remark)
        {
            Result result = new Result();
            //判断参数是否为空
            if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(holidayType) || string.IsNullOrEmpty(hours) ||
                    string.IsNullOrEmpty(instanceid))
            {
                return new Result { Success = false, Msg = "一个或多个参数为空" };
            }
            //判断用户名是否存在
            Organization.User us = AppUtility.Engine.Organization.GetUserByCode(userCode);
            if (us == null)
            {
                return new Result { Success = false, Msg = "userCode参数值错误，不存在此用户-->" + userCode };
            }
            //判断hours是否是数值
            double h;
            if (!double.TryParse(hours, out h))
            {
                return new Result { Success = false, Msg = "hours参数值错误，此参数必须为数值类型-->" + hours };
            }
            //判断类型是否正确
            if (holidayType != "加班" && holidayType != "调休")
            {
                return new Result { Success = false, Msg = "不支持此类型-->" + holidayType + "，目前只支持“加班”及“调休”" };
            }
            //判断InstanceID是否正确，在H3中走了审批流程
            Instance.InstanceContext ic = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
            if (ic == null)
            {
                return new Result { Success = false, Msg = "instanceid参数值错误，此参数必须为H3中对应的流程实例ID-->" + instanceid };
            }
            string sql = "select id from C_Emp_Holidays where usercode='" + userCode + "' and holidaytype='加班' and state=1";
            DataTable dtHolidaysInfo = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (holidayType == "加班")
            {
                //c_emp_holidays_detail-->type
                //1为加班的详细信息
                //2为调休的详细信息


                //c_emp_holidays-->state
                //1为激活状态
                //0为禁用或者删除状态
                int number = dtHolidaysInfo.Rows.Count;
                if (number == 0)//没有数据，插入一条
                {
                    string sqlInsert = @"
begin
insert into C_Emp_Holidays(id,usercode,holidaytype,totalhours,usedhours,state,createdtime,year,remark) values('{0}','{1}','{2}',{3},{4},{5},sysdate,'{6}','{7}');
insert into c_emp_holidays_detail(id,pid,instanceid,hours,type,createdtime,remark) values('{8}','{0}','{9}',{3},1,sysdate,'{7}');
end;
";
                    sqlInsert = string.Format(sqlInsert, Guid.NewGuid().ToString(), userCode, holidayType, hours, 0, 1, DateTime.Now.Year, remark, Guid.NewGuid().ToString(), instanceid);
                    int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlInsert);

                    result.Success = true;
                    result.Msg = "";
                }
                else//有数据，数据增加
                {
                    string sqlUpdate = @"
begin
update C_Emp_Holidays set totalhours=totalhours+{0} where usercode='{1}' and holidaytype='加班' and state=1;
insert into c_emp_holidays_detail(id,pid,instanceid,hours,type,createdtime,remark) values('{2}','{3}','{4}',{0},1,sysdate,'{5}');
end;
";
                    sqlUpdate = string.Format(sqlUpdate, hours, userCode, Guid.NewGuid().ToString(), dtHolidaysInfo.Rows[0][0], instanceid, remark);
                    //执行多行，没有返回，n为-1
                    int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlUpdate);
                    result.Success = true;
                    result.Msg = "";
                }
            }
            else if (holidayType == "调休")
            {
                string sqlUpdate = @"
begin
update C_Emp_Holidays set usedhours=usedhours+{0} where usercode='{1}' and holidaytype='加班' and state=1;
insert into c_emp_holidays_detail(id,pid,instanceid,hours,type,createdtime,remark) values('{2}','{3}','{4}',{0},1,sysdate,'{5}');
end;
";
                sqlUpdate = string.Format(sqlUpdate, hours, userCode, Guid.NewGuid().ToString(), dtHolidaysInfo.Rows[0][0], instanceid, remark);
                int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlUpdate);

                result.Success = true;
                result.Msg = "";

            }
            return result;
        }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
    }
}

