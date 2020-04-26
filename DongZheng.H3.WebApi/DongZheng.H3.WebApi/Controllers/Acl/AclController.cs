using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Acl
{
    [Xss]
    public class AclController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public string Index()
        {
            return "Acl service";
        }

        //获取用户的权限
        public JsonResult GetAcls(string user_code)
        {
            string acls = "";
            if (user_code == "Administrator")
            {
                string sql = "select code from OT_ENUMERABLEMETADATA where Category='权限组' order by sortkey";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    acls += row[0] + ";";
                }
                if (acls != "")
                {
                    acls = acls.Substring(0, acls.Length - 1);
                }
            }
            else
            {
                string sql = "select acls from i_acl_manage where user_code='" + user_code + "'";
                acls = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                if (acls != "" && acls.LastIndexOf(';') == acls.Length - 1)
                {
                    acls = acls.Substring(0, acls.Length - 1);
                }
            }
            return Json(acls.Split(';'), JsonRequestBehavior.AllowGet);
        }
    }
}