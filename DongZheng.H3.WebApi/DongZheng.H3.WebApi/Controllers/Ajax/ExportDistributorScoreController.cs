using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class ExportDistributorScoreController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string xlsName = "经销商评分_" + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            ExportExcel();
            string xls = context.Server.MapPath(AppConfig.PortalRoot + "/TempImages/经销商评分.xls");
            FileInfo file = new FileInfo(xls);
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("content-disposition", "attachment; filename=" + "经销商评分_" + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".xls");
            context.Response.AddHeader("Content-Length", file.Length.ToString());
            context.Response.WriteFile(file.FullName);
        }

        protected string ExportExcel()
        {
            Dictionary<string, string> heads = new Dictionary<string, string>();
            heads.Add("经销商名称", "经销商名称");
            heads.Add("内网/外网", "内网/外网");
            heads.Add("性质", "性质");
            heads.Add("准入_Score", "准入_Score");
            heads.Add("货后_Score", "货后_Score");
            heads.Add("综合_Score", "综合_Score");
            heads.Add("LEVEL", "LEVEL");
            heads.Add("年", "年");
            heads.Add("月", "月");
            heads.Add("日", "日");
            string SqlStr = @"SELECT JXSMC,NWWW,KIND,ZRSCORE,DHSCORE,ZHSCORE,JXS_LEVEL,YEAR,MONTH,DAY FROM C_JXSMC";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(SqlStr);
            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("序号", i);
                data.Add("经销商名称", dt.Rows[i]["JXSMC"]);
                data.Add("内网/外网", dt.Rows[i]["NWWW"]);
                data.Add("性质", dt.Rows[i]["KIND"]);
                data.Add("准入_Score", dt.Rows[i]["ZRSCORE"]);
                data.Add("货后_Score", dt.Rows[i]["DHSCORE"]);
                data.Add("综合_Score", dt.Rows[i]["ZHSCORE"]);
                data.Add("LEVEL", dt.Rows[i]["JXS_LEVEL"]);
                data.Add("年", dt.Rows[i]["YEAR"]);
                data.Add("月", dt.Rows[i]["MONTH"]);
                data.Add("日", dt.Rows[i]["DAY"]);
                datas.Add(data);
            }
            return OThinker.H3.Controllers.ExportExcel.ExportExecl("经销商评分", heads, datas);
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}