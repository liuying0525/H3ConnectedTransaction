using OThinker.H3.Acl;
using OThinker.H3.DataModel;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Reporting
{
    public class ReportSqlSourseController : ControllerBase
    {

        //该页面没有受权限控制，不需要注册节点编码
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 更改数据来源类型
        /// </summary>
        /// <param name="sorceType">来源类型</param>
        /// <returns>数据库列表项 或 业务系统列表</returns>
        [HttpPost]
        public JsonResult SorceTypeChange()
        {
            string sorceType = this.Request["Parameter"];
            return ExecuteFunctionRun(() =>
            {
                List<ListItem> items = new List<ListItem>();
                
                        BizDbConnectionConfig[] configs = this.Engine.SettingManager.GetBizDbConnectionConfigList();
                        foreach (BizDbConnectionConfig config in configs)
                        {
                            items.Add(new ListItem(config.DisplayName, config.DbCode));
                        }
                       
               
                return Json(items, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取数据表数组
        /// </summary>
        /// <param name="dbCode">业务数据库编号</param>
        /// <returns>所有表的名称</returns>
        [HttpPost]
        public JsonResult LoadTables()
        {
            string dbCode = this.Request["Parameter"];
            return ExecuteFunctionRun(() =>
            {

                    if (dbCode == null || dbCode == "")
                    {
                        dbCode = "Engine";//默认读取H3引擎链接池
                    }
              
                    string[] tableNames = this.Engine.SettingManager.GetBizDbTableNames(dbCode);
                    return Json(tableNames, JsonRequestBehavior.AllowGet);
              
            });
        }

        /// <summary>
        /// 获得业务数据库里的所有视图名称
        /// </summary>
        /// <param name="dbCode">业务数据库编号</param>
        /// <returns>所有视图的名称</returns>
        [HttpPost]
        public JsonResult LoadViews()
        {
            string dbCode = this.Request["Parameter"];
            return ExecuteFunctionRun(() =>
            {
                    if (dbCode == null || dbCode == "")
                    {
                        dbCode = "Engine";//默认读取H3引擎链接池
                    }
                    string[] viewNames = this.Engine.SettingManager.GetBizDbViewNames(dbCode);
                    return Json(viewNames, JsonRequestBehavior.AllowGet);
  
            });
        }

    }

}
