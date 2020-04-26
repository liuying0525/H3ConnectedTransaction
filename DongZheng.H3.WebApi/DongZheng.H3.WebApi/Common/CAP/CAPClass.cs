using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.CAP
{
    public class CAPClass
    {
        public static string AdministratorID = OThinker.Organization.User.AdministratorID;
        //public static string CapConnectionCode = System.Configuration.ConfigurationManager.AppSettings[""] + string.Empty;
        public static string SetCAPUser(bool isAll)
        {
            string sqlOUid = "select OBJECTID,Name  from  OT_OrganizationUnit where Name like '%网经销商'";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlOUid);

            string nwjxsOUID = dt.Select("Name='内网经销商'")[0]["OBJECTID"].ToString();
            string wwjxsOUID = dt.Select("Name='外网经销商'")[0]["OBJECTID"].ToString();
            string twjxsOUID = dt.Select("Name='退网经销商'")[0]["OBJECTID"].ToString();

            //SetSyncDateTime("CAP", AdministratorID);
            var msg = "";
            SyncResult syn = new SyncResult();
            //string sql = "select  [NO] 用户编码,[NAME] 用户名, [Phone] 手机号,FlagState 状态,   DeptType 部门 from captabletest";


            string sql = @"SELECT 
BSU.WORKFLOW_LOGIN_NME 登录用户, 
BSU.WORKFLOWLOGIN_ACTIVE_IND 在网状态,
BM2.BUSINESS_PARTNER_NME 经销商名称,
BM.BUSINESS_PARTNER_NME 登录用户名,
BP.PHONE_NBR 电话号码,
BP.EXECUTION_DTE 录入时间,
BSU.LAST_PASSWORDEXPIRY_DTE 账号上一次密码重置时间
FROM BP_SYS_USER BSU 
JOIN BP_MAIN BM ON BSU.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID 
JOIN BP_RELATIONSHIP BRE ON BRE.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID AND BRE.RELATIONSHIP_CDE='00112'
LEFT JOIN BP_MAIN BM2 ON BM2.BUSINESS_PARTNER_ID=BRE.BP_PRIMARY_ID
LEFT JOIN BP_PHONE BP ON BP.BUSINESS_PARTNER_ID=BM.BUSINESS_PARTNER_ID";

            DataTable CAPData = DBHelper.ExecuteDataTableSql(ConfigurationManager.AppSettings["capCode"] + string.Empty, sql);
            for (int i = 0; i < CAPData.Rows.Count; i++)
            {
                var code = CAPData.Rows[i]["登录用户"] + string.Empty;
                string userObj = GetUserObjectID(code);
                Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(userObj);
                User olduser = u == null ? null : (User)u;
                string ParentID = string.Empty;
                string FlagState = CAPData.Rows[i]["在网状态"] + string.Empty;
                string DeptType = CAPData.Rows[i]["登录用户"] + string.Empty;
                var _state = State.Active;
                var _ServiceState = UserServiceState.InService;

                if (FlagState == "F")
                {
                    ParentID = twjxsOUID;//退网经销商
                    _state = State.Inactive;//状态
                    _ServiceState = UserServiceState.Dismissed;
                }
                else
                {
                    _state = State.Active;
                    _ServiceState = UserServiceState.InService;
                    if (code.Substring(0, 2).Contains("98") || code.Substring(0, 4).Contains("8000"))
                    {
                        ParentID = nwjxsOUID;//内网经销商
                    }
                    else
                    {
                        ParentID = wwjxsOUID;//外网经销商
                    }
                }
                if (olduser == null)
                {
                    User us = new User();
                    us.Code = code;
                    if (!string.IsNullOrEmpty(CAPData.Rows[i]["录入时间"] + string.Empty))
                    {
                        us.CreatedTime = (DateTime)CAPData.Rows[i]["录入时间"];
                    }
                    us.Name = CAPData.Rows[i]["登录用户名"] + string.Empty;
                    us.Appellation = CAPData.Rows[i]["经销商名称"] + string.Empty;
                    us.Mobile = CAPData.Rows[i]["电话号码"] + string.Empty;
                    us.State = State.Active;//状态
                    us.ParentID = ParentID;
                    us.ModifiedTime = DateTime.Now;//修改时间
                    us.State = _state;//状态
                    us.ServiceState = _ServiceState;//在职状态
                    us.CreatedTime = DateTime.Now;
                    us.ObjectID = Guid.NewGuid().ToString();
                    if (!AddUnit(us, "CAP新增"))
                    {
                        msg = "更新失败!请联系管理员查看日志";
                        break;
                    }
                    syn.create_User_Success_Num++;

                }
                else
                {
                    if (isAll)
                    {
                        olduser.Code = code;
                        if (!string.IsNullOrEmpty(CAPData.Rows[i]["录入时间"] + string.Empty))
                        {
                            olduser.CreatedTime = (DateTime)CAPData.Rows[i]["录入时间"];
                        }
                        olduser.Name = CAPData.Rows[i]["登录用户名"] + string.Empty;
                        olduser.Appellation = CAPData.Rows[i]["经销商名称"] + string.Empty;
                        //如果有手机号码则不同步；
                        if (string.IsNullOrEmpty(olduser.Mobile))
                        {
                            olduser.Mobile = CAPData.Rows[i]["电话号码"] + string.Empty;
                        }
                        olduser.State = State.Active;//状态
                        olduser.ParentID = ParentID;
                        olduser.ModifiedTime = DateTime.Now;//修改时间
                        olduser.State = _state;//状态
                        olduser.ServiceState = _ServiceState;//在职状态
                        olduser.CreatedTime = DateTime.Now;
                        if (!UpdateUnit(olduser, "CAP更新"))
                        {
                            msg = "更新失败!请联系管理员查看日志";
                            break;
                        }
                        syn.update_User_Success_Num++;
                    }

                }

            }
            OThinker.H3.Controllers.AppUtility.Engine.Organization.Reload();
            //return Newtonsoft.Json.JsonConvert.SerializeObject(msg+syn.ToString());
            return msg + syn.ToString();

        }

        /// <summary>
        /// 插入组织架构
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool AddUnit(OThinker.Organization.Unit unit, string Msg)
        {
            bool isTrue = true;
            string errMsg = "";

            OThinker.Organization.HandleResult result = OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(AdministratorID, unit);
            if (result != OThinker.Organization.HandleResult.SUCCESS)
            {
                errMsg = "添加" + unit.UnitType.ToString() + "（" + Msg + "）失败：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;

                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);
                isTrue = false;
            }
            else
            {
                errMsg += "添加" + unit.UnitType.ToString() + "（" + Msg + "）成功：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);

            }


            return isTrue;
        }

        /// <summary>
        /// 修改组织架构
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool UpdateUnit(OThinker.Organization.Unit unit, string Msg)
        {
            string errMsg = "";
            bool isTrue = true;
            OThinker.Organization.HandleResult result = OThinker.H3.Controllers.AppUtility.Engine.Organization.UpdateUnit(AdministratorID, unit);
            if (result != OThinker.Organization.HandleResult.SUCCESS)
            {
                errMsg = "更新" + unit.UnitType.ToString() + "（" + Msg + "）失败：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;

                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);
                isTrue = false;
            }
            else
            {
                errMsg += "更新" + unit.UnitType.ToString() + "（" + Msg + "）成功：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);

            }
            return isTrue;
        }
        
        //根据员工号获取用户ID
        public static string GetUserObjectID(string code)
        {
            string sql = string.Format("select ObjectID from OT_User where code='{0}'", code);
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }
    }
}