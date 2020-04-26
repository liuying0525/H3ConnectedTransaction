<%@ WebHandler Language="C#" Class="DelProducts" %>

using System;
using System.Web;
using System.IO;
using System.Data;
using System.Collections.Generic;
using OThinker.H3;
using System.Text;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using System.Web.SessionState;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;

public class DelProducts : MvcPage, IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        System.IO.Stream s = context.Request.GetBufferedInputStream();
        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
        {
            string sql = "";
            Newtonsoft.Json.Linq.JArray ret = Newtonsoft.Json.Linq.JArray.Parse(sr.ReadToEnd());
            foreach (Newtonsoft.Json.Linq.JObject item in ret)
            {
                sql += (sql == "" ? " WHERE " : " OR ") + "(ProductID='" + item["ProductID"] + "' AND Dealer='" + item["Dealer"] + "')";
            }
            if (!string.IsNullOrWhiteSpace(sql))
                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM C_Products" + sql);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}