using System;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.Organization;
using System.Text.RegularExpressions;
using OThinker.H3.Controllers;

namespace WapiDZEQ.Controllers
{
    [Authorize]
    public class CAPController : OThinker.H3.Controllers.ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CAPController()
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

        public static string AdministratorID = OThinker.Organization.User.AdministratorID;
        public static int AddCount = 0;
        public static int UpdCount = 0;
        public JsonResult SetCAPUser()
        { //c5d80f0e-1fb3-424c-bc92-313101af25c4    135    6b06027b-bbf5-4a26-8949-f3ed6721faf3  77PRD
            string OUId = "6b06027b-bbf5-4a26-8949-f3ed6721faf3";
            var msg = "";
            SyncResult syn = new SyncResult();
            //SyncResult syn = new SyncResult();
            string sql = @"select t.dealer_code DEALERCODE,
                           t.dealer_name DEALERNAME,
                           t.user_code USERCODE
                           ,t.STATUS
                           from IN_WFS.V_DEALER_INFO t";
            DataTable dt = new DataTable();
            DistributorController dc = new DistributorController();
            dt = dc.ExecuteDataTableSql("Wholesale", sql);
            //dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var dealerCode = dt.Rows[i]["DEALERCODE"].ToString().Trim();
                var dealerName = dt.Rows[i]["DEALERNAME"].ToString().Trim();
                var userCode = dt.Rows[i]["USERCODE"].ToString().Trim();
                var status = dt.Rows[i]["STATUS"].ToString().Trim();

                var unitUser = new User();
                unitUser.ParentID = OUId;
                unitUser.Appellation = dealerCode;
                unitUser.Code = userCode;
                unitUser.Name = dealerName;
                unitUser.ServiceState = UserServiceState.InService;
                if(status=="Y"){
                    unitUser.State = OThinker.Organization.State.Active;
                }
                else if (status == "N")
                {
                    unitUser.State = OThinker.Organization.State.Inactive;
                }

                var aa = userCode.Substring(0, 2);
                var bb = userCode.Substring(2, 2);

                const string pattern = "^[0-9]*$";
                Regex rx = new Regex(pattern);
                bool isnum = rx.IsMatch(aa);
                if (isnum)
                {
                    if (aa.ToString() == "98")
                    {
                        unitUser.PostalCode = "内网";
                    }
                    else if (aa.ToString() == "80" || aa.ToString() == "61")
                    {
                        unitUser.PostalCode = "外网";
                    }
                    else
                    {
                        unitUser.PostalCode = "";
                    }
                }
                else
                {
                    if (bb.ToString() == "98")
                    {
                        unitUser.PostalCode = "内网";
                    }
                    else if (bb.ToString() == "80" || bb.ToString() == "61")
                    {
                        unitUser.PostalCode = "外网";
                    }
                    else
                    {
                        unitUser.PostalCode = "";
                    }
                }

                string userObj = GetUserObjectID(userCode);
                Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(userObj);
                User olduser = u == null ? null : (User)u;

                if (olduser == null)
                {
                    if (!AddUnit(unitUser, "CAP新增"))
                    {
                        syn.ErrMsg ="更新失败!请联系管理员查看日志";
                        syn.Success = false;
                        break;
                    }
                }
                else
                {
                    olduser.Appellation = unitUser.Appellation;
                    olduser.Code = unitUser.Code;
                    olduser.Name = unitUser.Name;
                    olduser.State = unitUser.State;
                    olduser.PostalCode = unitUser.PostalCode;

                    if (!UpdateUnit(olduser, "CAP更新"))
                    {
                        syn.ErrMsg = "更新失败!请联系管理员查看日志";
                        syn.Success = false;
                        break;
                    }
                }
            }
            
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 导入共计：" + AddCount + "人。\n" + " 更新共计：" + UpdCount + "人");
            //OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 更新共计：" + UpdCount + "人"); 
            OThinker.H3.Controllers.AppUtility.Engine.Organization.Reload();
            syn.ErrMsg = "更新成功";
            syn.Success = true;
            return Json(new { Success = syn.Success, Message = syn.ErrMsg },JsonRequestBehavior.AllowGet);

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
                AddCount += 1;
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
                UpdCount += 1;
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

        public class SyncResult
        {
            public bool Success { get; set; }
            public string ErrMsg { get; set; }

            public int create_OU_Success_Num { get; set; }
            public int create_OU_Fail_Num { get; set; }
            public int update_OU_Success_Num { get; set; }
            public int update_OU_Fail_Num { get; set; }

            public int create_User_Success_Num { get; set; }
            public int create_User_Fail_Num { get; set; }
            public int update_User_Success_Num { get; set; }
            public int update_User_Fail_Num { get; set; }

            public int delete_User_Success_Num { get; set; }
            public int delete_User_Fail_Num { get; set; }

            public override string ToString()
            {
                string msg = "";
                if (create_OU_Success_Num != 0 || create_OU_Fail_Num != 0)
                    msg += "OU创建：成功" + create_OU_Success_Num + "个，失败" + create_OU_Fail_Num + "个；\n";

                if (update_OU_Success_Num != 0 || update_OU_Fail_Num != 0)
                    msg += "OU更新：成功" + update_OU_Success_Num + "个，失败" + update_OU_Fail_Num + "个；\n";

                if (create_User_Success_Num != 0 || create_User_Fail_Num != 0)
                    msg += "User创建：成功" + create_User_Success_Num + "个，失败" + create_User_Fail_Num + "个；\n";

                if (update_User_Success_Num != 0 || update_User_Fail_Num != 0)
                    msg += "User更新：成功" + update_User_Success_Num + "个，失败" + update_User_Fail_Num + "个；\n";

                if (delete_User_Success_Num != 0 || delete_User_Fail_Num != 0)
                    msg += "User刪除：成功" + delete_User_Success_Num + "个，失败" + delete_User_Fail_Num + "个；\n";

                if (msg == "")
                    msg = "没有任何修改";
                return msg;
            }
        }

    }
}