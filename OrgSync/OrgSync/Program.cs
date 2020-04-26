using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgSync
{
    class Program
    {
        public static string OrgSyncAddress = System.Configuration.ConfigurationManager.AppSettings["OrgSyncAddress"] + string.Empty;
        static void Main(string[] args)
        {
            HttpHelper hh = new HttpHelper();
            var result = hh.PostWebRequest(OrgSyncAddress, "");
        }
    }
}
