using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace DongZheng.H3.WebApi.Common._263
{
    public class YXTClass
    {

        public static string AdministratorID = OThinker.Organization.User.AdministratorID;
        public static string ExceptUserCodes = ConfigurationManager.AppSettings["ExceptUserCodes"] + string.Empty;
        public static string ExceptOrgIDs = ConfigurationManager.AppSettings["ExceptOrgIDs"] + string.Empty;
        public static string NotSyncParentIDUserCodes = ConfigurationManager.AppSettings["NotSyncParentIDUserCodes"] + string.Empty;

        /// <summary>
        /// 263
        /// </summary>
        /// <param name="isAll">是否完全同步</param>
        /// <param name="ExecutionTime">执行时间</param>
        /// <returns></returns>
        public static string SetYXTUser(bool isAll,DateTime ExecutionTime)
        {

            var msg = "";
            string domain = "dongzhengafc.com";
            string account = "dongzhengafc.com";
            string sign1 = "cf6436ec710286f08292d48e36034012";
            string sign2 = "d7e7089f86df015c74fdadcc244ce4fb";

            xmapi.XmapiImplProxyService api = new xmapi.XmapiImplProxyService();
            var ou = api.getDepartment("chenghuashan", domain, account, sign2); //全部ou
            var user = api.getDomainUserlist_New(domain, account, sign1); //全部用户
            var userDismissed = api.getDomainUserlistByStatus(domain, 1, 0, account, sign1); //禁止用户
            if (ou == null || user == null || userDismissed == null)
            {
                return "263接口取数出错！";
            }
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("ou开始");
            DataTable dt = XmlToTablebyOU(ou);
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("ou结束");
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("user开始");
            DataTable dtUser = XmlToTablebyUser(user);
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("user结束");
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("dis开始");
            DataTable dtUserDismissed = XmlToTablebyUser(userDismissed);
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("dis结束");
            SyncResult syn = new SyncResult();
            if (dt.Rows.Count > 0)
            {
                if (!YXTOU(dt, "0", isAll, syn))
                {
                    msg = "OU更新失败!请联系管理员查看日志";

                }
                else
                {
                    var dateuser = DateTime.Now;
                    TimeSpan ts = dateuser.Subtract(DateTime.Now);
                    string timespan = "相差:" + ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时"
                                        + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("ou结束修改" + timespan);
                    if (!YXTUser(dtUser, isAll, false, syn, ExecutionTime))
                    {

                        msg = "263USER更新失败!请联系管理员查看日志";

                    }
                    else
                    {
                        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("user结束修改");
                        if (!YXTUser(dtUserDismissed, isAll, true, syn, ExecutionTime))
                        {
                            msg = "263禁止用户更新失败!请联系管理员查看日志";
                        }
                        OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("dis结束修改");
                    }
                }
            }
            //H3组织架构推送CRM
            Dictionary<string, object> dic = new Dictionary<string, object>();            dic.Add("dep", "市场销售部");            try            {                BizService.ExecuteBizNonQuery("CRMService", "postUser", dic);            }            catch (Exception ex)            {                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("CRM市场部组织架构推送异常：message：" + ex.Message);            }
            return msg + syn.ToString();

        }

        /// <summary>
        /// 263插入ou逻辑
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="YXTdepId">父级id</param>
        /// <param name="isAll">是否完全同步</param>
        /// <returns>bool</returns>
        public static bool YXTOU(DataTable dt, string YXTdepId, bool isAll, SyncResult syn)
        {
            bool isTrue = true;

            try
            {
                DataRow[] OUInfo = dt.Select("depId='" + YXTdepId + "'");
                for (int i = 0; i < OUInfo.Length; i++)
                {
                    string NewGuid = Guid.NewGuid().ToString();
                    string h3OUid = GetOUObjectiID(OUInfo[i]["departmentId"] + string.Empty);//获取对应H3OU的GUID
                    string H3OUParentID = GetOUObjectiID(YXTdepId);//获取对应H3OU的父GUID
                    OrganizationUnit unit = new OrganizationUnit();
                    Unit H3Unit = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(h3OUid);//获取H3中对应的组织
                    if (h3OUid != "" && H3Unit != null)//之前同步过，进行更新
                    {
                        unit = (OrganizationUnit)H3Unit;
                    }

                    unit.Name = OUInfo[i]["departmentName"] + string.Empty;
                    if (H3OUParentID != "")
                    {
                        unit.ParentID = H3OUParentID;
                    }
                    else
                    {
                        unit.ParentID = "18f923a7-5a5e-426d-94ae-a55ad1a4b240";
                    }
                    unit.Description = OUInfo[i]["departmentName"] + string.Empty;
                    unit.SortKey = int.Parse(OUInfo[i]["top"] + string.Empty);
                    if (h3OUid != "")//之前同步过，
                    {
                        if (isAll)
                        {
                            if (H3Unit == null)//之前同步过，后面删除后再次同步，使用之前的ObjectID
                            {
                                unit.ObjectID = h3OUid;
                                unit.CreatedTime = DateTime.Now;
                                isTrue = AddUnit(unit, "新增OU", syn);

                            }
                            else//同步过，进行更新
                            {
                                unit.ModifiedTime = DateTime.Now;
                                isTrue = UpdateUnit(unit, "更新OU", syn);
                            }
                        }

                    }
                    else//之前没有同步过，curUnit一定为null，不考虑人为删除对应的关系表
                    {
                        unit.ObjectID = NewGuid;
                        unit.CreatedTime = DateTime.Now;
                        isTrue = AddUnit(unit, "新增OU", syn);
                        InsertToCorresponding(OUInfo[i]["departmentId"] + string.Empty, OUInfo[i]["departmentName"] + string.Empty, NewGuid, YXTdepId);
                    }
                    YXTOU(dt, OUInfo[i]["departmentId"] + string.Empty, isAll, syn);

                }


            }
            catch (Exception e)
            {
                isTrue = false;
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(e.ToString());
            }

            return isTrue;

        }

       /// <summary>
       /// 263插入
       /// </summary>
       /// <param name="dt"></param>
       /// <param name="isAll"></param>
       /// <param name="isDismissed">是否在执行禁用</param>
       /// <param name="syn"></param>
       /// <param name="ExecutionTime">执行时间</param>
       /// <returns></returns>
        public static bool YXTUser(DataTable dt, bool isAll, bool isDismissed, SyncResult syn,DateTime ExecutionTime)
        {
            bool isTrue = true;
            DateTime date = DateTime.Now;
            var exceptUsercodes = ExceptUserCodes.Replace("'", "").Split(',');
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string code = dt.Rows[i]["userid"] + string.Empty;
                if (exceptUsercodes.Contains(code))//如果当前账户存在于不同步名单中，则不进行同步；
                {
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("用户-->" + code + "在不同步配置中，跳过同步！");
                    continue;
                }
                string name = dt.Rows[i]["name"] + string.Empty;
                string departmentid = dt.Rows[i]["departmentid"] + string.Empty;
                string h3OUid = GetOUObjectiID(departmentid);//获取对应H3OU的GUID
                string office = dt.Rows[i]["office"] + string.Empty;
                string mobile = dt.Rows[i]["mobile"] + string.Empty;
                mobile = mobile.Replace("+86-", "");
                string phone = dt.Rows[i]["phone"] + string.Empty;
                string fax = dt.Rows[i]["fax"] + string.Empty;
                string ServiceState = string.Empty;
                string userObj = GetUserObjectID(code);
                Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(userObj);
                User olduser = u == null ? null : (User)u;

                if (h3OUid != "")
                {
                    if (olduser == null)
                    {
                        User us = new User();
                        us.Code = code;
                        us.CreatedTime = DateTime.Now;
                        us.Name = name;
                        us.Mobile = mobile;
                        us.OfficePhone = phone;
                        us.Email = code;
                        us.FacsimileTelephoneNumber = fax;
                        us.State = State.Active;//状态
                        us.ParentID = h3OUid;
                        us.ModifiedTime = DateTime.Now;//修改时间
                        us.EntryDate = DateTime.Now;//新用户 增加入职日期；
                        us.Appellation = office;
                        if (isDismissed)
                        {
                            us.State = State.Inactive;//状态
                            us.ServiceState = UserServiceState.Dismissed;
                        }
                        else
                        {
                            us.State = State.Active;//状态
                            us.ServiceState = UserServiceState.InService;//在职状态
                        }

                        us.CreatedTime = DateTime.Now;
                        us.ObjectID = Guid.NewGuid().ToString();
                        if (!AddUnit(us, "263新增", syn))
                        {
                            isTrue = false;

                            break;
                        }

                    }
                    else
                    {
                        if (isAll)
                        {
                            olduser.Code = code;
                            olduser.CreatedTime = DateTime.Now;
                            olduser.Name = name;
                            //如果有手机号码则不同步；
                            if (string.IsNullOrEmpty(olduser.Mobile))
                            {
                                olduser.Mobile = mobile;
                            }
                            olduser.OfficePhone = phone;
                            olduser.Email = code;
                            olduser.FacsimileTelephoneNumber = fax;
                            //判断是否需要同步ParentID
                            if (!NotSyncParentIDUserCodes.Split(';').Contains(code))
                                olduser.ParentID = h3OUid;
                            olduser.ModifiedTime = DateTime.Now;//修改时间
                            olduser.Appellation = office;
                            if (isDismissed)
                            {
                                //如果是在职状态，而且此时要执行禁用（即离职），则加上离职日期
                                if (olduser.ServiceState == UserServiceState.InService)
                                {
                                    olduser.DepartureDate = DateTime.Now;
                                }
                                olduser.State = State.Inactive;//状态
                                olduser.ServiceState = UserServiceState.Dismissed;
                                
                            }
                            else
                            {
                                //默认就是激活，在职状态，不需要次都修改状态，这样会导致离职日期错误
                                //olduser.State = State.Active;//状态
                                //olduser.ServiceState = UserServiceState.InService;//在职状态
                            }
                            olduser.CreatedTime = DateTime.Now;
                            if (!UpdateUnit(olduser, "263更新", syn))
                            {
                                isTrue = false;
                                break;
                            }
                        }
                    }
                }

            }
            //执行禁用
            if (isAll && isDismissed)
            {
                UpdateState(ExecutionTime, syn);
            }

            OThinker.H3.Controllers.AppUtility.Engine.Organization.Reload();
            return isTrue;
        }




        public static void UpdateState(DateTime date, SyncResult syn)
        {
            string sql = string.Format(@"select objectid from ot_user where ServiceState<>'2'  and State= 1
                                    and   parentid not in ({0}) 
                                    and  code not in({1})  and  ModifiedTime< to_date('{2}',
                        'yyyy-mm-dd hh24:mi:ss')", ExceptOrgIDs, ExceptUserCodes, date);
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("禁用sql：" + sql);
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Unit ur = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(dt.Rows[i]["objectid"] + string.Empty);
                User old = ur == null ? null : (User)ur;
                if (old != null)
                {
                    //如果是在职状态，且此时Cap中没有这个用户，执行禁用，加上离职时间
                    if (old.ServiceState == UserServiceState.InService)
                    {
                        old.DepartureDate = DateTime.Now;
                    }
                    old.ServiceState = UserServiceState.Dismissed;
                    old.State = State.Inactive;
                    UpdateUnit(old, "263禁用", syn);
                }

            }


        }


        /// <summary>
        /// 插入对应表
        /// </summary>
        /// <param name="YXTdepartmentId"></param>
        /// <param name="departmentName"></param>
        /// <param name="depId"></param>
        public static void InsertToCorresponding(string YXTdepartmentId, string departmentName, string H3Deptid, string depId)
        {
            string sql = string.Format(@"INSERT INTO a_Corresponding(YXTDepartmentId,YXTDeptID,H3DepID
           ,DeptName)VALUES ('{0}','{1}','{2}','{3}')", YXTdepartmentId, depId, H3Deptid, departmentName);
            OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 获取h3部门对应id
        /// </summary>
        /// <param name="YXTDepartmentId"></param>
        /// <returns></returns>
        public static string GetOUObjectiID(string YXTDepartmentId)
        {
            string sql = string.Format("SELECT  H3DepID FROM a_Corresponding WHERE YXTDepartmentId='{0}'", YXTDepartmentId);
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }


        //根据员工号获取用户ID
        public static string GetUserObjectID(string code)
        {
            string sql = string.Format("select ObjectID from OT_User where code='{0}'", code);
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }

        /// <summary>
        /// Userxml转table
        /// </summary>
        /// <param name="xmlsUser"></param>
        /// <returns>dt</returns>
        public static DataTable XmlToTablebyUser(string xmlsUser)
        {
            System.Data.DataTable dt = new DataTable("user");
            var dc0 = new System.Data.DataColumn("userid");
            var dc1 = new System.Data.DataColumn("name");
            var dc2 = new System.Data.DataColumn("departmentid");
            var dc3 = new System.Data.DataColumn("office");
            var dc4 = new System.Data.DataColumn("mobile");
            var dc5 = new System.Data.DataColumn("phone");
            var dc6 = new System.Data.DataColumn("fax");

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);

            if (xmlsUser == null)
            {
                return dt;
            }

            StringReader Reader = new StringReader(xmlsUser);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Reader);
            XmlNode rootNode = xmlDoc.SelectSingleNode("userlist");


            foreach (XmlNode rootNode2 in rootNode.ChildNodes)
            {

                DataRow dr = dt.NewRow();
                foreach (XmlNode xxNode in rootNode2.ChildNodes)
                {

                    switch (xxNode.Name)
                    {
                        case "userid":
                            dr[0] = xxNode.InnerText;
                            break;
                        case "name":
                            dr[1] = xxNode.InnerText;
                            break;
                        case "departmentid":
                            dr[2] = xxNode.InnerText;
                            break;
                        case "office":
                            dr[3] = xxNode.InnerText;
                            break;
                        case "mobile":
                            dr[4] = xxNode.InnerText;
                            break;
                        case "phone":
                            dr[5] = xxNode.InnerText;
                            break;
                        case "fax":
                            dr[6] = xxNode.InnerText;
                            break;
                        default:
                            //    dr[1] = xxNode.InnerText;
                            break;
                    }
                }
                dt.Rows.Add(dr.ItemArray);
            }

            return dt;
        }

        /// <summary>
        /// OUXML转table
        /// </summary>
        /// <param name="xmlOU"></param>
        /// <returns>dt</returns>

        public static DataTable XmlToTablebyOU(string xmlOU)
        {
            System.Data.DataTable dt = new DataTable("OU");
            var dc0 = new System.Data.DataColumn("departmentId");
            var dc1 = new System.Data.DataColumn("domainId");
            var dc2 = new System.Data.DataColumn("departmentName");
            var dc3 = new System.Data.DataColumn("description");
            var dc4 = new System.Data.DataColumn("depId");
            var dc5 = new System.Data.DataColumn("rank");
            var dc6 = new System.Data.DataColumn("type");
            var dc7 = new System.Data.DataColumn("top");
            var dc8 = new System.Data.DataColumn("resv1");
            var dc9 = new System.Data.DataColumn("resv2");

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            dt.Columns.Add(dc9);
            if (xmlOU == null)
            {
                return dt;
            }

            StringReader Reader = new StringReader(xmlOU);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Reader);
            XmlNode rootNode = xmlDoc.SelectSingleNode("departmentList");


            foreach (XmlNode rootNode2 in rootNode.ChildNodes)
            {

                DataRow dr = dt.NewRow();
                foreach (XmlNode xxNode in rootNode2.ChildNodes)
                {

                    switch (xxNode.Name)
                    {
                        case "departmentId":
                            dr[0] = xxNode.InnerText;
                            break;
                        case "domainId":
                            dr[1] = xxNode.InnerText;
                            break;
                        case "departmentName":
                            dr[2] = xxNode.InnerText;
                            break;
                        case "description":
                            dr[3] = xxNode.InnerText;
                            break;
                        case "depId":
                            dr[4] = xxNode.InnerText;
                            break;
                        case "rank":
                            dr[5] = xxNode.InnerText;
                            break;
                        case "type":
                            dr[6] = xxNode.InnerText;
                            break;
                        case "top":
                            dr[7] = xxNode.InnerText;
                            break;
                        case "resv1":
                            dr[8] = xxNode.InnerText;
                            break;
                        case "resv2":
                            dr[9] = xxNode.InnerText;
                            break;
                        default:
                            //    dr[1] = xxNode.InnerText;
                            break;
                    }
                }
                dt.Rows.Add(dr.ItemArray);
            }

            return dt;
        }


        /// <summary>
        /// 插入组织架构
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool AddUnit(OThinker.Organization.Unit unit, string Msg, SyncResult syn)
        {
            bool isTrue = true;
            string errMsg = "";

            OThinker.Organization.HandleResult result = OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(AdministratorID, unit);
            if (result != OThinker.Organization.HandleResult.SUCCESS)
            {
                errMsg = "添加" + unit.UnitType.ToString() + "（" + Msg + "）失败：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;

                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);
                if (unit.UnitType.ToString() == "OrganizationUnit")
                {
                    syn.create_OU_Fail_Num++;
                }
                if (unit.UnitType.ToString() == "User")
                {
                    syn.create_User_Fail_Num++;
                }

                isTrue = false;
            }
            else
            {
                errMsg += "添加" + unit.UnitType.ToString() + "（" + Msg + "）成功：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);

                if (unit.UnitType.ToString() == "OrganizationUnit")
                {
                    syn.create_OU_Success_Num++;
                }
                if (unit.UnitType.ToString() == "User")
                {
                    syn.create_User_Success_Num++;
                }
            }


            return isTrue;
        }

        /// <summary>
        /// 修改组织架构
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static bool UpdateUnit(OThinker.Organization.Unit unit, string Msg, SyncResult syn)
        {
            string errMsg = "";
            bool isTrue = true;
            OThinker.Organization.HandleResult result = OThinker.H3.Controllers.AppUtility.Engine.Organization.UpdateUnit(AdministratorID, unit);
            if (result != OThinker.Organization.HandleResult.SUCCESS)
            {
                errMsg = "更新" + unit.UnitType.ToString() + "（" + Msg + "）失败：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;

                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);
                if (unit.UnitType.ToString() == "OrganizationUnit")
                {
                    syn.update_OU_Fail_Num++;
                }
                if (unit.UnitType.ToString() == "User")
                {
                    syn.update_User_Success_Num++;
                }
                isTrue = false;
            }
            else
            {
                errMsg += "更新" + unit.UnitType.ToString() + "（" + Msg + "）成功：" + result.ToString() + "," + unit.Name + ",ObjectID：" + unit.ObjectID + ",ParentID：" + unit.ParentID;
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(errMsg);
                if (unit.UnitType.ToString() == "OrganizationUnit")
                {
                    syn.update_OU_Success_Num++;
                }
                if (unit.UnitType.ToString() == "User")
                {
                    syn.update_User_Success_Num++;
                }

            }
            return isTrue;
        }

    }
}