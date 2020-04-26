using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using Renci.SshNet;
namespace postJXSFKBL
{
    class Program
    {
        static void Main(string[] args)
        {

            //string INKPIDATA_URL = System.Configuration.ConfigurationManager.AppSettings["INKPIDATA"] + string.Empty;
            //string INSUBDATA_URL = System.Configuration.ConfigurationManager.AppSettings["INSUBDATA"] + string.Empty;
            //string REFUSEDATA_URL = System.Configuration.ConfigurationManager.AppSettings["REFUSEDATA"] + string.Empty;
            //postJXSFKBL.DzService.DZBIZService service = new DzService.DZBIZService();
            //service.postDZJR("INKPIDATA", INKPIDATA_URL);//经销商放款统计
            //System.Threading.Thread.Sleep(5000);
            //service.postDZJR("INSUBDATA", INSUBDATA_URL);//经销商放款明细
            //System.Threading.Thread.Sleep(5000);
            //service.postDZJR("REFUSEDATA", REFUSEDATA_URL);//客户拒批明细

            string ls_userid = "00260135";
            string ls_password = "7580417";
            postJXSFKBL.jtLDAP a = new jtLDAP();
            a.LDAP_URL = "LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com";
            a.LDAP_USERID = "cn=" + ls_userid + ",dc=zhengtongauto,dc=com";
            a.LDAP_PASSWORD = ls_password;
            try
            {
                a.GetEntry();
                string r = a.f_getPersonMes(ls_userid, "description");//返回的用户信息中，包含了岗位和在职状态，GWZT=（0：待聘、1：试用、2：正式、3：离职）
                //richTextBox1.Text = "密码正确";
            }
            catch (Exception ex)
            {
                // 密码错误 :ex.Message.ToString();
            } 
        }
    }
}
