using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Portal
{
    /// <summary>
    /// WorkPcmFunction 的摘要说明
    /// </summary>
    public class WorkPcmFunction
    {
        public WorkPcmFunction()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public string IsExistHKNLInfo(string appno, string sfz)
        {
            string rtn = "";
            string sql = "select * from Dz_Hknl_Result where appno= '" + appno + "' and zjr='" + sfz + "'";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rtn = "{\"Result\":\"Success\",\"C_IncomOfMonth\":" + dt.Rows[0]["ZJR_YSRGZ"] + ",\"C_DabtsOfMonth\":" + dt.Rows[0]["ZJR_YYHZW"] + ",\"C_AssetValuation\":" + dt.Rows[0]["ZJR_KHZCGZ"] + ",\"C_RepayLoan\":" + dt.Rows[0]["ZJR_KHYHKNL"] + "}";
            }
            return rtn;
        }
        public string GetCustomerInfo(string certno, string dzcarloan)
        {
            decimal C_IncomOfMonth = 0;         //月收入估值 
            decimal C_DabtsOfMonth = 0;         //月应还债务
            decimal C_AssetValuation = 0;       //客户资产估值
            decimal C_RepayLoan = 0;            //客户月还贷能力
            decimal dCreditAmt = 0;             //信用卡额度
            decimal dHousingLoan = 0;           //房贷
            decimal dCreditRepayOfMoth = 0;     //信用卡月还款额
            decimal dCreditLoanOfMoth = 0;      //贷款月应还款额
            decimal dLastAmtOfMonth = 0;        //剩余月均应还本金
            decimal dDZLoan = 0;                //东正车贷月应还款额
            decimal dAssetAmt = 0;              //资产估值
            string strCertno = certno;
            dDZLoan = Convert.ToDecimal(dzcarloan);

            string strReportID = GetReoprtID(strCertno);
            string date = DateTime.Now.ToString("yyyyMMdd");
            CommonFunction.AddLog("PCM_LOG", date, "*****************客户身份证件:[" + strCertno + "]  主键ID:[" + strReportID + "]日志:" + DateTime.Now.ToString() + "***********\r\n");
            CommonFunction.AddLog("PCM_LOG", date, "东正车贷月应还款额:" + dDZLoan.ToString() + "\r\n");
            if (!string.IsNullOrEmpty(strReportID))
            {
                CommonFunction.AddLog("PCM_LOG", date, "信用卡信息:\r\n");
                //获取信用卡信息
                DataTable dtCreditinfo = GetCreditInfo(strCertno);
                if (dtCreditinfo.Rows.Count > 0)
                {
                    DataRow[] row = null;
                    //优先级--1：近6个月使用且未逾期的账户；取最大值。
                    CommonFunction.AddLog("PCM_LOG", date, "优先级--1：近6个月使用且未逾期的账户；取最大值\r\n");
                    row = dtCreditinfo.Select("last1='N' AND last2='N' AND  last3='N' AND  last4='N' AND  last5='N' AND  last6='N'");
                    if (row.Length > 0)
                    {
                        foreach (DataRow item in row)
                        {
                            decimal temp = 0;
                            if (item["信用卡共享额度"] != DBNull.Value) { temp = Convert.ToDecimal(item["信用卡共享额度"]); }
                            if (dCreditAmt < temp) { dCreditAmt = temp; }
                            CommonFunction.AddLog("PCM_LOG", date, "贷记卡信息ID:[" + item["贷记卡信息ID"].ToString() + "]  还款状态:[" + item["还款状态"].ToString() + "]   信用卡共享额度[" + item["信用卡共享额度"].ToString() + "]\r\n");

                        }
                    }
                    else
                    {
                        //优先级--2： 近6个月使用且偶有逾期的账户；取平均值（非零账户）。
                        CommonFunction.AddLog("PCM_LOG", date, "优先级--2： 近6个月使用且偶有逾期的账户；取平均值（非零账户)\r\n");
                        row = dtCreditinfo.Select("(LAST1='N') AND (LAST2='1' OR LAST2='N') AND (LAST3='1' OR LAST3='N') AND (LAST4='1' OR LAST4='N') AND (LAST5='1' OR LAST5='N') AND (LAST6='1' OR LAST6='N')");
                        if (row.Length > 0)
                        {
                            for (int i = 0; i < row.Length; i++)
                            {
                                dCreditAmt += Convert.ToDecimal(row[i]["信用卡共享额度"]);
                                CommonFunction.AddLog("PCM_LOG", date, "贷记卡信息ID:[" + row[i]["贷记卡信息ID"].ToString() + "]  还款状态:[" + row[i]["还款状态"].ToString() + "]   信用卡共享额度[" + row[i]["信用卡共享额度"].ToString() + "]\r\n");
                            }
                            dCreditAmt = dCreditAmt / (row.Length);

                        }
                        else
                        {
                            //优先级--3：开卡至少有6个月的账户；取平均值（非零账户）。
                            CommonFunction.AddLog("PCM_LOG", date, "优先级--3：开卡至少有6个月的账户；取平均值（非零账）\r\n");
                            row = dtCreditinfo.Select(@"(LAST1='N' OR LAST1='*' OR LAST1='#') 
                                                       AND (LAST2='N' OR LAST2='*' OR LAST2='#') 
                                                       AND (LAST3='N' OR LAST3='*' OR LAST3='#') 
                                                       AND (LAST4='N' OR LAST4='*' OR LAST4='#') 
                                                       AND (LAST5='N' OR LAST5='*' OR LAST5='#') 
                                                       AND (LAST6='N' OR LAST6='*' OR LAST6='#')");
                            if (row.Length > 0)
                            {
                                for (int i = 0; i < row.Length; i++)
                                {
                                    dCreditAmt += Convert.ToDecimal(row[i]["信用卡共享额度"]);
                                    CommonFunction.AddLog("PCM_LOG", date, "贷记卡信息ID:[" + row[i]["贷记卡信息ID"].ToString() + "]  还款状态:[" + row[i]["还款状态"].ToString() + "]   信用卡共享额度[" + row[i]["信用卡共享额度"].ToString() + "]\r\n");
                                }
                                dCreditAmt = dCreditAmt / (row.Length);
                            }
                            else
                            {
                                //先级--4：全部账户；取平均值（非零账户）。
                                CommonFunction.AddLog("PCM_LOG", date, "先级--4：全部账户；取平均值（非零账户）\r\n");
                                foreach (DataRow item in dtCreditinfo.Rows)
                                {
                                    dCreditAmt += Convert.ToDecimal(item["信用卡共享额度"]);
                                    CommonFunction.AddLog("PCM_LOG", date, "贷记卡信息ID:[" + item["贷记卡信息ID"].ToString() + "]  还款状态:[" + item["还款状态"].ToString() + "]   信用卡共享额度[" + item["信用卡共享额度"].ToString() + "]\r\n");
                                }
                                dCreditAmt = dCreditAmt / (dtCreditinfo.Rows.Count);
                            }
                        }
                    }
                }
                CommonFunction.AddLog("PCM_LOG", date, "信用卡额度:" + dCreditAmt + "\r\n");
                //获取其他指标信息
                DataTable dtOtherInfo = GetOtherInfo(strReportID);
                if (dtOtherInfo.Rows.Count > 0)
                {
                    if (dtOtherInfo.Rows[0]["房贷"] != DBNull.Value) { dHousingLoan = Convert.ToDecimal(dtOtherInfo.Rows[0]["房贷"]); }
                    if (dtOtherInfo.Rows[0]["信用卡月还款总和"] != DBNull.Value) { dCreditRepayOfMoth = Convert.ToDecimal(dtOtherInfo.Rows[0]["信用卡月还款总和"]); }
                    if (dtOtherInfo.Rows[0]["月度应还债务总和"] != DBNull.Value) { dCreditLoanOfMoth = Convert.ToDecimal(dtOtherInfo.Rows[0]["月度应还债务总和"]); }
                    if (dtOtherInfo.Rows[0]["剩余月均应还本金"] != DBNull.Value) { dLastAmtOfMonth = Convert.ToDecimal(dtOtherInfo.Rows[0]["剩余月均应还本金"]); }
                    if (dtOtherInfo.Rows[0]["客户资产估值"] != DBNull.Value) { dAssetAmt = Convert.ToDecimal(dtOtherInfo.Rows[0]["客户资产估值"]); }
                }
                CommonFunction.AddLog("PCM_LOG", date, "房贷:" + dHousingLoan + "   信用卡月还款总和:" + dCreditRepayOfMoth + "  月度应还债务总和:" + dCreditLoanOfMoth + "  剩余月均应还本金:" + dLastAmtOfMonth + "   客户资产估值:" + dAssetAmt + "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "贷款月应还款额=非（个人经营性贷款、个人助学贷款和农户贷款）的月度应还债务总和，与剩余月均应还本金之间取较小值。\r\n");
                //贷款月应还款额：非（个人经营性贷款、个人助学贷款和农户贷款）的月度应还债务总和，与剩余月均应还本金之间取较小值。
                dCreditLoanOfMoth = dCreditLoanOfMoth > dLastAmtOfMonth ? dLastAmtOfMonth : dCreditLoanOfMoth;

                //客户月收入估值=月收入估值 = MAX (信用卡额度/1.5, 房贷×5)。

                C_IncomOfMonth = (dCreditAmt / (decimal)1.5) > (dHousingLoan * 5) ? decimal.Round(dCreditAmt / (decimal)1.5, 2) : decimal.Round(dHousingLoan * 5, 2);

                //月应还债务 = 信用卡月应还款额 + 贷款月应还款额 + 东正车贷月应还款额。

                C_DabtsOfMonth = decimal.Round(dCreditRepayOfMoth + dCreditLoanOfMoth + dDZLoan, 2);

                //客户资产估值：
                C_AssetValuation = decimal.Round(dAssetAmt, 2);
                // 客户月还贷能力指标 = 月收入估算 + 客户资产估值 + 5000 - 月应还债务
                C_RepayLoan = decimal.Round(C_IncomOfMonth + C_AssetValuation + 5000 - C_DabtsOfMonth, 2);


                CommonFunction.AddLog("PCM_LOG", date, "客户月收入估值= MAX (信用卡额度/1.5, 房贷×5)\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "客户月收入估值:" + C_IncomOfMonth.ToString() + "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "月应还债务 = 信用卡月应还款额 + 贷款月应还款额 + 东正车贷月应还款额\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "月应还债务:" + C_DabtsOfMonth.ToString() + "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "客户资产估值:" + C_AssetValuation.ToString() + "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "客户月还贷能力指标 = 月收入估算 + 客户资产估值 + 5000 - 月应还债务\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "客户月还贷能力指标:" + C_RepayLoan.ToString() + "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "************************结束*********************************\r\n");
                CommonFunction.AddLog("PCM_LOG", date, "\r\n");
            }
            string rtn = "{\"Result\":\"Success\",\"C_IncomOfMonth\":" + C_IncomOfMonth + ",\"C_DabtsOfMonth\":" + C_DabtsOfMonth + ",\"C_AssetValuation\":" + C_AssetValuation + ",\"C_RepayLoan\":" + C_RepayLoan + "}";
            return rtn;
        }
        /// <summary>
        /// 根据身份证件号获取主键ID
        /// </summary>
        /// <param name="context">身份证件号</param>
        /// <returns></returns>
        public string GetReoprtID(string strCertno)
        {
            string rtn = "";
            string strSqlBase = string.Format(@"select distinct(report_id) from R_NR_QueryReq  where certno='{0}'", strCertno);
            object obj = new WorkFlowFunction().ExecuteScalar("rsmysql", strSqlBase.ToLower());
            if (obj != null)
            {
                rtn = obj.ToString();
            }
            return rtn;
        }
        /// <summary>
        /// 根据身份证件号获取信用卡信息
        /// </summary>
        /// <param name="context">身份证件号</param>
        /// <returns></returns>
        public DataTable GetCreditInfo(string strCertno)
        {
            string rtn = "";
            //根据证件号码得到信用卡信息
            //string strSql = string.Format(@"select  a.*,b.loancard_id 贷记卡信息ID,d.latest24state 还款状态,c.ShareCreditLimitAmount 信用卡共享额度,
            //                                substr(d.latest24state,-1,1) last1,substr(d.latest24state,-3,1) last2,substr(d.latest24state,-5,1) last3,
            //                                substr(d.latest24state,-7,1) last4,substr(d.latest24state,-9,1) last5,substr(d.latest24state,-11,1) last6
            //                                from
            //                                (select distinct(certno),report_id from R_NR_QueryReq  where certno='{0}') a
            //                                inner join R_NR_Loancard b on a.report_id=b.report_id
            //                                inner join r_nr_awardcreditinfo c on a.report_id=c.report_id and b.loancard_id=c.super_id
            //                                inner join R_NR_Latest24MonthPaymentState d on a.report_id=d.report_id and b.loancard_id=d.super_id
            //                                where ShareCreditLimitAmount>0", strCertno);

            string strSql = string.Format(@"select  a.*,b.loancard_id 贷记卡信息id,d.latest24state 还款状态,c.sharecreditlimitamount 信用卡共享额度,
                                    substr(d.latest24state,-1,1) last1,substr(d.latest24state,-3,1) last2,substr(d.latest24state,-5,1) last3,
                                    substr(d.latest24state,-7,1) last4,substr(d.latest24state,-9,1) last5,substr(d.latest24state,-11,1) last6
                                    from
                                    (select distinct(certno),report_id from r_nr_queryreq  where certno='{0}') a
                                    join r_nr_loancard b on a.report_id=b.report_id
                                    join r_nr_awardcreditinfo c on  b.loancard_id=c.super_id and a.report_id=c.report_id 
                                    join (select d.* from  r_nr_latest24monthpaymentstate d where d.report_id in (select distinct report_id from r_nr_queryreq  where certno='{0}') ) d
                                    on b.loancard_id=d.super_id
                                    where c.sharecreditlimitamount>0", strCertno);
            DataTable dt = new WorkFlowFunction().ExecuteDataTableSql("rsmysql", strSql.ToLower());

            return dt;

        }
        /// <summary>
        /// 获取其他指标信息
        /// </summary>
        /// <param name="strReportID"></param>
        /// <returns></returns>
        public DataTable GetOtherInfo(string strReportID)
        {
            string rtn = "";
            //获取其他信息
            string strSql = string.Format(@"select 
                                        (   
                                            select sum(b.ScheduledPaymentAmount) 房贷  from R_NR_ContractInfo a
                                            inner join R_NR_CurrAccountInfo b on a.loan_id=b.loan_id and a.report_id=b.report_id 
                                            where a.report_id='{0}'
                                            and (type='个人商用房（包括商住两用）贷款' or type='个人住房贷款' or  type='个人住房公积金贷款')
                                            AND (CLASS5STATE='正常' OR CLASS5STATE='关注')  and ScheduledPaymentAmount>0
                                        ) 房贷
                                        ,
                                        (
                                            select sum(ScheduledPaymentAmount) 信用卡月还款总和  from R_NR_RepayInfo 
                                            where report_id='{0}' and ScheduledPaymentAmount>0
                                            
                                        )信用卡月还款总和
                                        ,
                                        (
                                            select sum(b.ScheduledPaymentAmount) 贷款月应还款额  from R_NR_ContractInfo a
                                            inner join R_NR_CurrAccountInfo b on a.loan_id=b.loan_id and a.report_id=b.report_id 
                                            where a.report_id='{0}'
                                            and (type!='个人经营性贷款' and type!='个人助学贷款' and type!='农户贷款' ) 
                                            and a.PaymentRating='按月归还'
                                            and ScheduledPaymentAmount>0
                                        ) 月度应还债务总和
                                        ,
                                        (
                                            select sum(Balance/RemainPaymentCyc)剩余月均应还本金  from R_NR_ContractInfo a
                                            inner join R_NR_CurrAccountInfo b on a.loan_id=b.loan_id and a.report_id=b.report_id 
                                            where a.report_id='{0}'
                                            and (type!='个人经营性贷款' and type!='个人助学贷款' and type!='农户贷款' ) 
                                            and a.PaymentRating='按月归还'
                                            and ScheduledPaymentAmount>0
                                        ) 剩余月均应还本金
                                        ,
                                        (
                                            select  sum(CreditLimitAmount) 客户资产估值 from R_NR_Loan a
                                            inner join R_NR_ContractInfo b on a.report_id =b.report_id and a.loan_id=b.loan_id
                                            where a.report_id='{0}'
                                            and (type='个人住房贷款' or type='个人住房公积金贷款' or type='个人商用房（包括商住两用）贷款') 
                                            and a.state='结清' and CreditLimitAmount>0
                                        ) 客户资产估值", strReportID);
            DataTable dt = new WorkFlowFunction().ExecuteDataTableSql("rsmysql", strSql.ToLower());

            return dt;

        }
    }


}