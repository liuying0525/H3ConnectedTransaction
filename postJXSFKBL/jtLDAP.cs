using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Collections; 

namespace postJXSFKBL
{
    class jtLDAP
    {
        private string is_url = "LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com";//测试地址:LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com
        private string is_userid = "cn=Manager,dc=zhengtongauto,dc=com";//用户代码 cn=Manager,dc=zhengtongauto,dc=com
        private string is_password="secret";//用户密码 secret

        public string LDAP_URL
        {
            set { is_url = value; }
        }

        public string LDAP_USERID
        {
            set { is_userid = value; }
        }

        public string LDAP_PASSWORD
        {
            set { is_password = value; }
        }



        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="dm">用户代码</param>
        /// <param name="ret">返回值</param>
        /// <returns></returns>
        public string f_getPersonMes(string dm, ref string ret)
        {
            try
            {
                DirectoryEntry entry = GetEntry();

                DirectorySearcher searcher = new DirectorySearcher(entry);
                entry.Close();

                searcher.Filter = "(cn=" + dm + ")";

                SearchResult r = searcher.FindOne();

                DirectoryEntry entry2 = null;
                entry2 = r.GetDirectoryEntry();
                string description = entry2.Properties["description"][0].ToString();
                string sn = entry2.Properties["sn"][0].ToString();
                string telephoneNumber = entry2.Properties["telephoneNumber"][0].ToString();

                ret = "{\r\n";
                ret += "\"cn\":\"" + dm + "\",\r\n";
                ret += "\"sn\":\"" + sn + "\",\r\n";
                ret += "\"telephoneNumber\":\"" + telephoneNumber + "\",\r\n";
                ret += "\"description\":\"" + description + "\"\r\n";
                ret += "}";
                entry2.Close();
                return "";
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="dm">用户代码</param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public string f_getPersonMes(string dm, string PropertyName)
        {
            try
            {
                DirectoryEntry entry = GetEntry();

                DirectorySearcher searcher = new DirectorySearcher(entry);
                entry.Close();

                searcher.Filter = "(cn=" + dm + ")";

                SearchResult r = searcher.FindOne();

                DirectoryEntry entry2 = null;
                entry2 = r.GetDirectoryEntry();


                string ret = "";
                if (entry2.Properties.Contains(PropertyName))
                {
                    ret = entry2.Properties[PropertyName][0].ToString();
                }
                else
                {
                    if (PropertyName == "dn")
                    {
                        ret = entry2.Path.ToString();
                        int i = ret.LastIndexOf("/");
                        ret = ret.Substring(i + 1);
                    }
                }



                entry2.Close();
                return ret;
            }
            catch (System.Exception ex)
            {
                return "";
            }


        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="password"></param>
        /// <returns>空代表密码正确,其他代表密码不正确</returns>
        public string f_VerifyPassword(string dm, string password)
        {
            try
            {
                DirectoryEntry entry = GetEntry();
                return "";
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }


        }

        /// <summary>
        /// 连接服务
        /// </summary>
        /// <returns></returns>
        public DirectoryEntry GetEntry()
        {
            DirectoryEntry entry = new DirectoryEntry(is_url, is_userid, is_password, AuthenticationTypes.None);
            try
            {
                object native = entry.NativeObject;
                return entry;
            }
            catch (System.Exception ex)
            {
                throw new Exception("连接失败." + ex.Message);
            }
            return null;
        }
    }
}



