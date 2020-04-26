
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;
using System.Web.Script.Serialization;
using WapiDZEQ;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class MarketingController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MarketingController()
        {
        }


        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        public JsonResult GetMarketDept(PagerInfo pagerInfo, string JXS, string YQZT, string StartTime, string EndTime)
        {
            DataSet ds = MarketDept(pagerInfo, JXS, YQZT, StartTime, EndTime);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);

        }

        public DataSet MarketDept(PagerInfo pagerInfo, string JXS, string YQZT, string StartTime, string EndTime)
        {
            var UserID = this.UserValidator.UserID;
            var DepartmentName = this.UserValidator.DepartmentName;
            var activity = string.Empty;
            if (DepartmentName == "市场部")
            {
                activity = "Activity2";
            }
            if (DepartmentName == "财务部")
            {
                activity = "Activity10";
            }
            string sql = string.Format(@"select * from (select 
                d.sqbh 申请编号,
                d.jxs 经销商,
                to_char(w.ReceiveTime,'yyyy-mm-dd hh24:mi:ss') 接收时间,
                to_char(w.ReceiveTime,'yyyy-mm-dd') 接收时间_天,
                d.SQCLTS 申请车辆台数,
                d.SQDKJE 贷款申请金额,
                v.INVOICES_OVERDUE 逾期账单,
                --d.DKYE 贷款余额,
                v2.AVAILED_LIMIT 贷款余额,
                W.instanceid,
                w.objectid
                from I_DealerLoan  D
                inner join  ot_instancecontext i on i.bizobjectid=d.objectid 
                inner join ot_workitem w on w.instanceid=i.objectid
                join IN_WFS.V_ZSUMMARY_DLR@TO_AUTH_WFS v on trim(v.dealer_code) = d.jxscode
                left join IN_WFS.v_dealer_balance_limit@TO_AUTH_WFS v2 on trim(v2.dealer_code) = d.jxscode and trim(d.ZZS) = trim(v2.Make)
                where Participant='{0}' and  w.WORKFLOWCODE='DealerLoan' ) where 1=1 ", UserID);//SELECT rownum rn,  aa.*  from （ ) aa where   1=1
            //DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            #region 查询条件
            if (!string.IsNullOrEmpty(JXS))
            {
                sql += string.Format(" and  经销商 like '%{0}%'", JXS);

            }
            if (!string.IsNullOrEmpty(YQZT))
            {
                if (YQZT == "有逾期")
                {
                    sql += string.Format(" and  逾期账单 >0");
                }
                else
                {
                    sql += string.Format(" and  逾期账单 <=0");
                }

            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                sql += string.Format("AND 接收时间_天 >= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", StartTime);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                sql += string.Format("AND 接收时间_天 <= to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", EndTime);
            }
            #endregion

            #region 排序
            string OrderBy = " ORDER BY 接收时间 ";
            if (pagerInfo.iSortCol_0 != 0)
            {
                OrderBy = " ORDER BY ";
                if (pagerInfo.iSortCol_0 == 2)
                {
                    OrderBy += " 接收时间 " + pagerInfo.sSortDir_0.ToUpper();
                }
            }
            sql += OrderBy;

            var sql1 = "select a1.* from (SELECT rownum rn,  aa.*  from ( " + sql + " ) aa ) a1 where 1=1 ";
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
           
            #endregion
            //string sqlstr = "select a1.* from ( " + sql + ") a1 where 1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sql1 += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            return ds;

        }

        public void ExportToExcelByTemplate(string userCode)
        {
            string msg = string.Empty;
            try
            {
                #region 取数据可卖车型、颜色
                string sql = "select distinct trim(ASSET_CODE） ASSET_CODE ,trim(ASSET_DESC） ASSET_DESC from IN_WFS.V_DEALER_MAKE_MODEL_NAME_CODE v where v.dealer_code =" + userCode;
                DataTable CXTable = ExecuteDataTableSql("Wholesale", sql);
                string sql2 = "select trim(COLOR) COLOR,trim(COLOR_CODE) COLOR_CODE from IN_WFS.V_DEALER_MAKE_COLOR v where v.dealer_code = " + userCode;
                DataTable YSTable = ExecuteDataTableSql("Wholesale", sql2);
                #endregion
                #region 打开Excel表格模板，并初始化到NPOI对象中
                IWorkbook wk = null;
                string filePath = Server.MapPath(@"~/Sheets/Wholesale/Distributor/申请贷款模版.xlsx");
                if (!System.IO.File.Exists(filePath))
                {
                    msg = "模板不存在！";
                    return;
                }
                string extension = System.IO.Path.GetExtension(filePath);
                FileStream fs = System.IO.File.OpenRead(filePath);
                if (extension.Equals(".xls"))
                {
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                }
                else
                {
                    //把xlsx文件中的数据写入wk中
                    wk = new XSSFWorkbook(fs);
                }
                fs.Close();
                #endregion

                #region 数据处理
                //1.读取Excel表格中的第二张Sheet表
                ISheet sheet = wk.GetSheetAt(1);
                ISheet sheetRef = wk.GetSheet("Sheet2");
                // IRow row = null;//数据行
                //var rowHeader = sheet.CreateRow(0);
                ICell cell1 = null;//数据行中的第一列
                ICell cell2 = null;//数据行中的第二列
                //2.添加Excel数据行。处理表格的边框，没有数据的数据行就没有内外边框。
                //获取数据行数(班级数量)
                int CXCount = CXTable.Rows.Count;
                int YSCount = YSTable.Rows.Count;

                for (int i = 0; i < CXCount; i++)
                {
                    var row = sheetRef.CreateRow(i + 1);

                    cell1 = row.CreateCell(0);
                    cell1.SetCellValue(CXTable.Rows[i]["ASSET_DESC"] + string.Empty);
                    if (i < YSCount)
                    {
                        cell2 = row.CreateCell(1);
                        cell2.SetCellValue(YSTable.Rows[i]["COLOR"] + string.Empty);
                    }
                }
                #endregion

                #region 表格导出
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode("申请贷款模版", System.Text.Encoding.UTF8)));
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                wk = null;
                #endregion
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                //LogWrite("导出课程表失败", ex.ToString(), CurrentOperator.Name, ResourceID);
                //Windows.MessageBox(Page, "导出课程表失败", MessageType.Normal);
            }
        }


        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }

    }
}