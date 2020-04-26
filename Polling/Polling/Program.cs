using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Polling
{
    class Program
    {
        public static string OrgSyncAddress263 = System.Configuration.ConfigurationManager.AppSettings["OrgSyncAddress263"] + string.Empty;     

        public static string OrgSyncAddress263_135 = System.Configuration.ConfigurationManager.AppSettings["OrgSyncAddress263_135"] + string.Empty;
       
        static void Main(string[] args)
        {
            Console.Write("开始");
            //function();
            //function75();
           function135();

            //functionPRDEverytime();
            //functionPRDEveryOne();
            Console.Write("结束");
        }

        public static void function()
        {
            PollingWebService.PollingWebService PW = new PollingWebService.PollingWebService();
            PW.DistributorPolling();
            PW.CJHPolling();
        }

        public static void function75()
        {
            PollingWebService75.PollingWebService PW = new PollingWebService75.PollingWebService();
            PW.DistributorPolling();
            PW.CJHPolling();
        }

        public static void function135()
        {
            PollingWebService135.PollingWebService PW = new PollingWebService135.PollingWebService();
            PW.Timeout = 300000;
            // 5分钟一次
            PW.DistributorPolling();//贷款流程状态获取、还款流程状态获取、还款异常消息
            PW.SendMessageCWFKPolling();// 贷款放款通知财务部


            //一天一次
            PW.CreditPolling(); // 授信报表
            PW.CJHPolling();// 车架号待修改
            PW.BillWflx();// 账单每月1日 提醒
            PW.BillCgfxr();// 账单过期第一日提醒
            PW.SendMessagePolling();//还款10天到期提醒
            PW.dealerInfoSUMMARY();//经销商汇总信息


            //同步用户
            //HttpHelper hh = new HttpHelper();
            //var result263 = hh.PostWebRequest(OrgSyncAddress263_135, ""); // H3端内容用户
            //PW.CAPPolling(); // 批售经销商用户
        
           
          
        }

        public static void functionPRDEverytime()
        {
            WebReferencePRD.PollingWebService PW = new WebReferencePRD.PollingWebService();
            PW.Timeout = 300000;
            PW.DistributorPolling();
           
           
        }
        public static void functionPRDEveryOne()
        {
            // 授信报告
            WebReferencePRD.PollingWebService PW = new WebReferencePRD.PollingWebService();           
            PW.CreditPolling();

            //同步用户
            HttpHelper hh = new HttpHelper();
            var result263 = hh.PostWebRequest(OrgSyncAddress263, "");

            PW.CAPPolling();
           

        }


    }
}
