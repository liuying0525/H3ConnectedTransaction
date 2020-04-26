using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using DongZheng.H3.WebApi.Common;
using OThinker.Data;
using OThinker.Organization;
using OThinker.H3.Instance;
using System.IO;
using System.Text;
using OThinker.H3.Messages;
using OThinker.H3;

namespace DongZheng.H3.WebApi.Controllers.ProcessCenter
{
    [ValidateInput(false)]
    [Xss]
    public class AutoAllocationOrderController : OThinker.H3.Controllers.ControllerBase
    {
        AllocationOrder allocationOrder = new AllocationOrder();
        /// <summary>
        /// 日志文件名称
        /// </summary>
        const string LogFileName = "AllocationOrder";
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        //public JsonResult GetOrder()
        //{
        //    rtn_data rtn = new rtn_data();
        //    AllocationOrder.AllocationOrderRule rule = allocationOrder.UserRule(this.UserValidator.UserID);
        //    if (rule != null)
        //    {
        //        //现有订单数量
        //        int num = allocationOrder.GetProposalOrderNumber(this.UserValidator.UserID);
        //        allocationOrder.AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), this.UserValidator.UserName + "[" + this.UserValidator.UserCode + "]" + "现有订单数量：" + num);
        //        if (num < rule.LimitNumber)
        //        {
        //            //条件 
        //            string condtions = "";

        //            #region 渠道类型
        //            if (rule.ChannelType == "内网")
        //            {
        //                condtions += " and app.createdbyparentid ='28a0ba01-4f58-4097-96cb-77c2b09e8253'";
        //            }
        //            else if (rule.ChannelType == "外网")
        //            {
        //                condtions += " and app.createdbyparentid ='9b8eb0fb-31c2-4577-bea7-9e453812f863'";
        //            }
        //            #endregion

        //            #region 资产类型
        //            if (!string.IsNullOrEmpty(rule.AssetCondition))
        //            {
        //                condtions += " and vehicle.condition='" + rule.AssetCondition + "'";
        //            }
        //            #endregion

        //            #region 审核额度
        //            condtions += " and contract.amount_financed>=" + rule.LoanAmountFrom;
        //            condtions += " and contract.amount_financed<=" + rule.LoanAmountTo;
        //            #endregion

        //            #region SQL
        //            string sql = allocationOrder.GetQueryTaskSql(condtions);
        //            #endregion

        //            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        //            if (dt.Rows.Count > 0)
        //            {
        //                DataRow row = dt.Rows[0];
        //                allocationOrder.AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), string.Format("任务[{0}]分配给{1}[{2}]", row["objectid"], rule.UserName, rule.UserCode));
        //                //插入到分单历史表中；
        //                allocationOrder.SaveToOrderHis(rule, row["instanceid"] + string.Empty, row["objectid"] + string.Empty, row["application_number"] + string.Empty, row["amount_financed"] + string.Empty, row["createdbyparentid"] + string.Empty, num);
        //                //分单；
        //                rtn = allocationOrder.SendOrder(rule, row["bizobjectschemacode"] + string.Empty, row["bizobjectid"] + string.Empty, row["participant"] + string.Empty, row["objectid"] + string.Empty, row["amount_financed"] + string.Empty);
        //            }
        //            else
        //            {
        //                rtn.code = -1;
        //                rtn.message = "暂时没有可分配的申请，请稍后再试";
        //            }
        //        }
        //        else
        //        {
        //            rtn.code = -1;
        //            rtn.message = string.Format("主动获单数量上限为{0}，当前为{1}", rule.LimitNumber, num);
        //        }
        //    }
        //    else
        //    {
        //        rtn.code = -1;
        //        rtn.message = "您没有配置分单规则，请联系管理员添加";
        //    }
        //    return Json(rtn);
        //}

        //public JsonResult GetAllocationAcl()
        //{
        //    bool hasacl = false;
        //    AllocationOrder.AllocationOrderRule rule = allocationOrder.UserRule(this.UserValidator.UserID);
        //    if (rule != null)
        //    {
        //        hasacl = true;
        //    }
        //    return Json(hasacl);
        //}

        public JsonResult Test(string id)
        {
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(id);
            AllocationOrder a = new AllocationOrder();
            var r = a.NeedAllocationOrder(context);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryFKState(string workflowCode, string[] workitemIds)
        {
            rtn_data rtn = new rtn_data();
            if (workitemIds == null || workitemIds.Length == 0 || string.IsNullOrEmpty(workflowCode))
            {
                rtn.code = -1;
                rtn.message = "参数为空";
            }
            try
            {
                string sql = @"
select wi.objectid,app.allocation_available from i_{0} app 
left join ot_instancecontext con on app.objectid=con.bizobjectid
left join ot_workitem wi on con.objectid=wi.instanceid where 1=1 {1}";
                string conditions = "";

                foreach (var id in workitemIds)
                {
                    conditions += "'" + id + "',";
                }
                conditions = " and wi.objectid in (" + conditions.Substring(0, conditions.Length - 1) + ")";
                sql = string.Format(sql, workflowCode, conditions);
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                Dictionary<string, bool> data = new Dictionary<string, bool>();
                foreach (DataRow r in dt.Rows)
                {
                    data.Add(r["objectid"] + string.Empty, r["allocation_available"] + string.Empty == "1" ? true : false);
                }
                rtn.code = 1;
                rtn.data = data;
            }
            catch (Exception ex)
            {
                rtn.code = -1;
                rtn.message = "异常";
            }
            return Json(rtn);
        }

        //public JsonResult GetOrderNumber()
        //{
        //    rtn_data rtn = new rtn_data();
        //    try
        //    {
        //        AllocationOrder.AllocationOrderRule rule = allocationOrder.UserRule(this.UserValidator.UserID);
        //        if (rule != null)
        //        {
        //            //条件 
        //            string condtions = "";

        //            #region 渠道类型
        //            if (rule.ChannelType == "内网")
        //            {
        //                condtions += " and app.createdbyparentid ='28a0ba01-4f58-4097-96cb-77c2b09e8253'";
        //            }
        //            else if (rule.ChannelType == "外网")
        //            {
        //                condtions += " and app.createdbyparentid ='9b8eb0fb-31c2-4577-bea7-9e453812f863'";
        //            }
        //            #endregion

        //            #region 资产类型
        //            if (!string.IsNullOrEmpty(rule.AssetCondition))
        //            {
        //                condtions += " and vehicle.condition='" + rule.AssetCondition + "'";
        //            }
        //            #endregion

        //            #region 审核额度
        //            condtions += " and contract.amount_financed>=" + rule.LoanAmountFrom;
        //            condtions += " and contract.amount_financed<=" + rule.LoanAmountTo;
        //            #endregion

        //            #region SQL
        //            string sql = allocationOrder.GetQueryTaskSql(condtions);
        //            #endregion

        //            int filterNumber = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql).Rows.Count;

        //            int totalNumber = allocationOrder.GetTaskTotalNumber();

        //            rtn.code = 1;
        //            rtn.data = "满足[" + filterNumber + "]/总[" + totalNumber + "]";
        //        }
        //        else
        //        {
        //            rtn.code = -1;
        //            rtn.message = "无分单规则";
        //        }
        //        return Json(rtn);
        //    }
        //    catch (Exception ex)
        //    {
        //        rtn.code = -1;
        //        rtn.message = "查询异常";
        //        return Json(rtn);
        //    }
        //}

        public JsonResult TransferUserByRG(string worlitemids, string replaceuserid)
        {
            string[] workItems = worlitemids.Split(';');
            // 转移工作项
            int successCount = 0;
            if (workItems != null)
            {
                foreach (string itemId in workItems)
                {
                    if (string.IsNullOrEmpty(itemId))
                        continue;
                    var item = this.Engine.WorkItemManager.GetWorkItem(itemId);
                    if (item.DisplayName == "东正风控评估")
                    {
                        var context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);

                        //流程数据
                        string sql = "select app.application_number,det.amount_financed,app.allocation_available from i_application{0} app left join i_contract_detail{0} det on app.objectid = det.parentobjectid where app.objectid = '{1}'";
                        sql = string.Format(sql, context.BizObjectSchemaCode.ToLower().Replace("application", ""), context.BizObjectId);
                        DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                        //允许分单
                        if (dt.Rows[0]["allocation_available"] + string.Empty == "1")
                        {
                            //获取当前规则
                            AllocationOrder.AllocationOrderRule rule = allocationOrder.UserRule(replaceuserid);
                            if (rule != null)
                            {
                                allocationOrder.AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), string.Format("任务[{0}]分配给{1}[{2}]-->人工分单", itemId, rule.UserName, rule.UserCode));

                                //现有订单数量
                                int num = allocationOrder.GetProposalOrderNumber(replaceuserid);

                                //插入到分单历史表中；
                                allocationOrder.SaveToOrderHis(rule, item.InstanceId, item.ObjectID, dt.Rows[0]["application_number"] + string.Empty, dt.Rows[0]["amount_financed"] + string.Empty, context.OrgUnit, num);
                                //分单；
                                allocationOrder.SendOrder(rule, context.BizObjectSchemaCode, context.BizObjectId, item.Participant, item.ObjectID, dt.Rows[0]["amount_financed"] + string.Empty);
                            }
                        }
                    }
                    else
                    {
                        long result1 = this.Engine.WorkItemManager.Transfer(itemId, replaceuserid);
                        if (result1 == ErrorCode.SUCCESS)
                        {
                            this.Engine.LogWriter.Write(string.Format("TransferUserTransferUserByRG itemId:{0},replaceuserid:{1},by:{2}", itemId, replaceuserid, this.UserValidator.UserName + "[" + this.UserValidator.UserCode + "]"));
                            successCount++;
                        }
                    }
                }
            }

            OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true, "TransferUser.TransferSucess");//TODO  信息提示
            return Json(result);
        }
    }

    public class AllocationOrder
    {
        /// <summary>
        /// 日志文件名称
        /// </summary>
        const string LogFileName = "AllocationOrder";
        public string[] WorkflowCode = (System.Configuration.ConfigurationManager.AppSettings["allocationWorkflows"] + string.Empty).Split(';');

        #region Method

        /// <summary>
        /// 获取当前人员的零售业务订单数量
        /// </summary>
        /// <returns></returns>
        public int GetProposalOrderNumber(string UserID)
        {
            string condition = "";
            foreach (var c in WorkflowCode)
            {
                if (condition == "")
                    condition += "workflowcode='" + c + "'";
                else
                    condition += " or workflowcode='" + c + "'";
            }
            condition = " and (" + condition + ")";
            string sql = "select count(1) from ot_workitem where (PARTICIPANT='{0}' or delegant='{0}') {1}";
            sql = string.Format(sql, UserID, condition);
            var n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
            return Convert.ToInt32(n);
        }

        public AllocationOrderRule UserRule(string UserId)
        {
            string sqlGetRules = string.Format("select * from i_CreditGetOrderRules where FIRSTCREDITPERSON='{0}'", UserId);
            DataTable dtRules = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlGetRules);
            if (dtRules.Rows.Count > 0)
            {
                //规则
                AllocationOrderRule rule = Converter.ToObject<AllocationOrderRule>(dtRules.Rows[0]);
                return rule;
            }
            return null;
        }

        /// <summary>
        /// 用户是否有效
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public bool UserAvailable(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), string.Format("UserAvailable参数为空-->{0}", UserId));
                return false;
            }
            var user = (User)AppUtility.Engine.Organization.GetUnit(UserId);
            if (user != null)
            {
                if (user.State == State.Inactive || user.ServiceState == UserServiceState.Dismissed)
                {
                    AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), string.Format("{0}[{1}][{2}]用户禁用或者离职，需要重新分单", user.Name, user.Code, user.ObjectID));
                    return false;
                }
                return true;
            }
            else
            {
                AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), string.Format("{0}用户不存在，需要重新分单", UserId));
                return false;
            }
        }

        /// <summary>
        /// 是否需要分单
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool NeedAllocationOrder(InstanceContext context)
        {
            string sql = "select * from i_" + context.BizObjectSchemaCode + " where objectid='" + context.BizObjectId + "'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string cyz = dt.Rows[0]["cyzxscs"] + string.Empty;
                #region 参与者为空
                if (string.IsNullOrEmpty(cyz))
                {
                    //1.查询有没有贷款历史；
                    string appType = dt.Rows[0]["application_type_code"] + string.Empty;
                    string sqlGetHis = "";
                    #region 查询历史申请订单  SQL
                    if (appType == "00001")
                    {
                        string sqlGetIDCardNo = @"select det.id_card_nbr from i_application{0} app
left join i_applicant_detail{0} det on app.objectid=det.parentobjectid and det.identification_code2=1
where app.objectid='{1}' order by app.createdtime desc";
                        sqlGetIDCardNo = string.Format(sqlGetIDCardNo, context.BizObjectSchemaCode.Replace("APPLICATION", ""), context.BizObjectId);
                        string idcardno = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetIDCardNo) + string.Empty;

                        sqlGetHis = GetQueryHisApplicationSql(idcardno, context.BizObjectId, appType);
                    }
                    else
                    {
                        string sqlGetOrgNo = @"select det.ORGANIZATION_CDE from i_application app
left join i_company_detail det on app.objectid=det.parentobjectid and det.identification_code3=1
where app.objectid='{0}' order by app.createdtime desc";
                        sqlGetOrgNo = string.Format(sqlGetOrgNo, context.BizObjectId);
                        string orgNo = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetOrgNo) + string.Empty;

                        sqlGetHis = GetQueryHisApplicationSql(orgNo, context.BizObjectId, appType);
                    }
                    #endregion
                    DataTable dtHistory = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlGetHis);
                    if (dtHistory.Rows.Count > 0)
                    {
                        string cyzxscs = "";
                        foreach (DataRow r in dtHistory.Rows)
                        {
                            cyzxscs = r["cyzxscs"] + string.Empty;
                            if (!string.IsNullOrEmpty(cyzxscs))
                                break;
                        }
                        AddLog(LogFileName, DateTime.Now.ToString("yyyy-MM-dd"), "历史参与者为：" + cyzxscs + "，实例ID为-->" + context.InstanceId);
                        //用户可用，采用历史分单规则，不需要重新分单
                        if (UserAvailable(cyzxscs))
                        {
                            #region 给参与者赋值
                            AllocationOrderRule rule = UserRule(cyzxscs);
                            string cyzxszs = "";
                            if (rule != null)
                            {
                                cyzxszs = rule.FinalCreditPerson;
                                //增加判断贷款金额
                                double dk_amount;
                                if (double.TryParse(dt.Rows[0]["financedamount"] + string.Empty, out dk_amount))
                                {
                                    //贷款金额大于终审的审核额度
                                    if (dk_amount > rule.FinalLoanAmountTo)
                                    {
                                        cyzxszs = GetSpecialAudit(dk_amount);
                                    }
                                }
                            }
                            Dictionary<string, object> dicPara = new Dictionary<string, object>();
                            dicPara.Add("cyzxscs", cyzxscs);
                            dicPara.Add("cyzxszs", cyzxszs);
                            AppUtility.Engine.BizObjectManager.SetPropertyValues(context.BizObjectSchemaCode, context.BizObjectId, "", dicPara);
                            #endregion
                            return false;
                        }
                    }
                    return true;
                }
                #endregion
                #region 参与者不为空
                else
                {
                    //用户可用，不用分单
                    if (UserAvailable(cyz))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                #endregion
            }
            return true;
        }

        /// <summary>
        /// 分单
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="bizschemacode"></param>
        /// <param name="bizobjectid"></param>
        /// <param name="participant"></param>
        /// <param name="workitemid"></param>
        /// <returns></returns>
        public rtn_data SendOrder(AllocationOrderRule rule, string bizschemacode, string bizobjectid, string participant, string workitemid,string loanAmount)
        {
            rtn_data rtn = new rtn_data();
            #region 分单
            string zsPerson = rule.FinalCreditPerson;
            double amount;
            if (double.TryParse(loanAmount, out amount))
            {
                //贷款金额大于终审的审核额度
                if (amount > rule.FinalLoanAmountTo)
                {
                    zsPerson = GetSpecialAudit(amount);
                }
            }
            
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("cyzXSCS", rule.FirstCreditPerson);
            values.Add("cyzXSZS", zsPerson);
            values.Add("allocation_available", 0);//不可再分单；
            var setResult = AppUtility.Engine.BizObjectManager.SetPropertyValues(bizschemacode, bizobjectid, participant, values);
            if (setResult)
            {
                BPM.SubmitItem(workitemid, BoolMatchValue.True, "", participant);
                rtn.code = 1;
                rtn.message = "分单成功";
            }
            else
            {
                rtn.code = -1;
                rtn.message = "分单失败";
            }
            #endregion
            return rtn;
        }

        /// <summary>
        /// 获取特殊的终审人员，满意条件的取第一条；
        /// </summary>
        /// <param name="amount">根据金额</param>
        /// <returns></returns>
        public string GetSpecialAudit(double amount)
        {
            string sqlGetRules = string.Format("select * from i_CreditGetOrderRules where SpecialPerson=1 and FinalLoanAmountFrom<{0} and FinalLoanAmountTo>{0}", amount);
            DataTable dtRules = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlGetRules);
            if (dtRules.Rows.Count > 0)
            {
                return dtRules.Rows[0]["FinalCreditPerson"] + string.Empty;
            }
            return "";
        }

        /// <summary>
        /// 保存到订单记录表
        /// </summary>
        /// <param name="rule">规则</param>
        /// <param name="instanceid">流程id</param>
        /// <param name="workitemid">任务id</param>
        /// <param name="applicationNumber">申请单号</param>
        /// <param name="loanAmount">贷款金额</param>
        /// <param name="originatorUnit">发起人部门id</param>
        /// <param name="currentNum">当前待办中的申请数量（零售）</param>
        /// <returns></returns>
        public bool SaveToOrderHis(AllocationOrderRule rule, string instanceid,string workitemid,string applicationNumber,string loanAmount,string originatorUnit, int currentNum)
        {
            int channel_type = originatorUnit == "28a0ba01-4f58-4097-96cb-77c2b09e8253" ? 0 : 1;
            string sqlInsert = @"insert into C_Auto_Allocation_Order_Detail(Allocation_Date,instanceid,workitemid,application_number,channel_type,
asset_condition,loan_amount,usercode,username,final_usercode,final_username,app_num,rule_channel_type,rule_asset_condition,rule_amount_from,rule_amount_to,rule_limit_num) 
values(sysdate,'{0}','{1}','{2}',{3},'{4}',{5},'{6}','{7}','{8}','{9}',{10},'{11}','{12}',{13},{14},{15})";
            sqlInsert = string.Format(sqlInsert, instanceid, workitemid, applicationNumber, channel_type,
                rule.AssetCondition,loanAmount , rule.UserCode, rule.UserName, rule.FinalUserCode, rule.FinalUserName,
                currentNum, rule.ChannelType, rule.AssetCondition, rule.LoanAmountFrom, rule.LoanAmountTo, rule.LimitNumber);
            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlInsert) > 0;
        }

        /// <summary>
        /// 获取查询零售业务的sql
        /// </summary>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public string GetQueryTaskSql(string Condition)
        {
            string sqlTemplate = @"
select wi.objectid,con.bizobjectschemacode,con.workflowcode,con.bizobjectid,wi.participant,wi.instanceid,app.application_number,app.createdbyparentid,wi.receivetime,contract.amount_financed
from ot_workitem wi
left join ot_instancecontext con on wi.instanceid=con.objectid
left join i_application{1} app on con.bizobjectid=app.objectid
left join i_vehicle_detail{1} vehicle on vehicle.parentobjectid=app.objectid
left join i_contract_detail{1} contract on contract.parentobjectid=app.objectid
left join C_Auto_Allocation_Order_Detail det on wi.objectid=det.workitemid
where wi.workflowcode='APPLICATION{1}' and wi.activitycode='Activity12' and det.instanceid is null
and app.automatic_approval=0 and app.fk_result='转人工' and app.allocation_available=1
{0}
";
            string sqlbody = "";
            foreach (var c in WorkflowCode)
            {
                if (sqlbody == "")
                    sqlbody += string.Format(sqlTemplate, Condition, c.Replace("APPLICATION", ""));
                else
                    sqlbody += "\r\nUnion all\r\n" + string.Format(sqlTemplate, Condition, c.Replace("APPLICATION", ""));
            }
            string sql = "select * from (" + sqlbody + ") a order by a.receivetime";
            return sql;
        }

        /// <summary>
        /// 获取任务池中任务总数
        /// </summary>
        /// <returns></returns>
        public int GetTaskTotalNumber()
        {
            string sql = GetQueryTaskSql("");
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return dt.Rows.Count;
        }

        /// <summary>
        /// 获取查询历史订单的SQL
        /// </summary>
        /// <param name="id_card_nbr"></param>
        /// <param name="bizobjectid"></param>
        /// <param name="application_type_code"></param>
        /// <returns></returns>
        public string GetQueryHisApplicationSql(string id_card_nbr,string bizobjectid,string application_type_code)
        {
            if (application_type_code == "00001")
            {
                string sqlTemp = @"select app.cyzxscs,app.createdtime from i_application{0} app
left join i_applicant_detail{0} det on app.objectid=det.parentobjectid and det.identification_code2=1
where det.id_card_nbr='{1}' and app.objectid<>'{2}'";

                string sqlBody = "";
                foreach (var c in WorkflowCode)
                {
                    if (sqlBody == "")
                    {
                        sqlBody += string.Format(sqlTemp, c.Replace("APPLICATION", ""), id_card_nbr, bizobjectid);
                    }
                    else
                    {
                        sqlBody += "\r\nUnion all\r\n" + string.Format(sqlTemp, c.Replace("APPLICATION", ""), id_card_nbr, bizobjectid);
                    }
                }
                string sql = "select * from (" + sqlBody + ") a order by a.createdtime desc";
                return sql;
            }
            else if (application_type_code == "00002")
            {
                string sqlTemp = @"select app.cyzxscs,app.createdtime from i_application app
left join i_company_detail det on app.objectid=det.parentobjectid and det.identification_code3=1
where det.ORGANIZATION_CDE='{0}' and app.objectid<>'{1}' order by app.createdtime desc";
                string sql = string.Format(sqlTemp, id_card_nbr, bizobjectid);
                return sql;
            }
            return "";
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="LogDire"></param>
        /// <param name="logName"></param>
        /// <param name="logInfo"></param>
        public void AddLog(string folder, string logName, string logInfo)
        {
            try
            {
                string LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\" + folder + "\\";
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

                    File.WriteAllText(path, DateTime.Now.ToString() + ":" + logInfo + "\r\n", Encoding.Default);
                }
                else
                {
                    File.AppendAllText(path, DateTime.Now.ToString() + ":" + logInfo + "\r\n", Encoding.Default);
                }
            }
            catch
            {

            }
        }

        #endregion

        #region Class
        /// <summary>
        /// 分单规则
        /// </summary>
        public class AllocationOrderRule
        {
            /// <summary>
            /// 初审人员
            /// </summary>
            public string FirstCreditPerson { get; set; }
            /// <summary>
            /// 初审姓名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 初审Code
            /// </summary>
            public string UserCode { get; set; }
            /// <summary>
            /// 初审部门
            /// </summary>
            public string Department { get; set; }
            /// <summary>
            /// 渠道类型
            /// </summary>
            public string ChannelType { get; set; }

            /// <summary>
            /// 资产类型
            /// </summary>
            public string AssetCondition { get; set; }
            /// <summary>
            /// 审核起始额度
            /// </summary>
            public double LoanAmountFrom { get; set; }
            /// <summary>
            /// 审核终止额度
            /// </summary>
            public double LoanAmountTo { get; set; }
            /// <summary>
            /// 主动获单上限
            /// </summary>
            public int LimitNumber { get; set; }
            /// <summary>
            /// 终审人员
            /// </summary>
            public string FinalCreditPerson { get; set; }

            /// <summary>
            /// 终审姓名
            /// </summary>
            public string FinalUserName { get; set; }
            /// <summary>
            /// 终审账号
            /// </summary>
            public string FinalUserCode { get; set; }
            /// <summary>
            /// 终审部门
            /// </summary>
            public string FinalDepartment { get; set; }
            /// <summary>
            /// 终审审核起始额度
            /// </summary>
            public double FinalLoanAmountFrom { get; set; }
            /// <summary>
            /// 终审审核终止额度
            /// </summary>
            public double FinalLoanAmountTo { get; set; }
        }

        #endregion
    }

}