using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 

namespace OThinker.H3.Portal
{
    public partial class DzDataRisk : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                DataRisk.DataRiskBack(stream); 
                this.Context.Response.Write("ok");
            }
            catch (Exception ex)
            {
                this.Context.Response.Write("error");
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("东正大数据回调异常：" + ex.ToString());
            }
        }
    }

}