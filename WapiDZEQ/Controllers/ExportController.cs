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
    public class ExportController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportController()
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

        /// <summary>
        /// 贷款导入模版下载
        /// </summary>
        /// <param name="userCode"></param>
        public void ExportToExcelByTemplate(string MAKE)
        {
            var UserID = this.UserValidator.UserID;
            Organization.Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(UserID);
           string userCode = ((OThinker.Organization.User)u).Appellation;
            string msg = string.Empty;
            try
            {
                DistributorController DC = new DistributorController();

                #region 取数据可卖车型、颜色
                string sql = "select distinct trim(ASSET_CODE） ASSET_CODE ,trim(ASSET_DESC） ASSET_DESC from IN_WFS.V_DEALER_MAKE_MODEL_NAME_CODE v where v.dealer_code =" + userCode + " and MAKE ='" + MAKE + "'";
                DataTable CXTable = DC.ExecuteDataTableSql("Wholesale", sql);
                string sql2 = "select trim(COLOR) COLOR,trim(COLOR_CODE) COLOR_CODE from IN_WFS.V_DEALER_MAKE_COLOR v where v.dealer_code = " + userCode + " and MAKE ='" + MAKE + "'";
                DataTable YSTable = DC.ExecuteDataTableSql("Wholesale", sql2);
                #endregion
                #region 打开Excel表格模板，并初始化到NPOI对象中
                IWorkbook wk = null;
                string filePath = Server.MapPath(@"~/Sheets/Wholesale/Distributor/申请贷款模板.xlsx");
                if (!System.IO.File.Exists(filePath))
                {

                    msg = "模板不存在！";
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(msg);
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

                ISheet sheet = wk.GetSheetAt(1);
                ISheet sheetRef = wk.GetSheet("Sheet2");
                ISheet sheetRef2 = wk.GetSheet("Sheet3");
               

                ICell cell1 = null;//数据行中的第一列
                ICell cell22 = null;//数据行中的第二列

                int CXCount = CXTable.Rows.Count;
                int YSCount = YSTable.Rows.Count;

                for (int i = 0; i < CXCount; i++)
                {
                    var row = sheetRef.CreateRow(i + 1);

                    cell1 = row.CreateCell(0);
                    cell1.SetCellValue(CXTable.Rows[i]["ASSET_DESC"] + string.Empty);
                    //if (i < YSCount)
                    //{
                    //    cell2 = row.CreateCell(1);
                    //    cell2.SetCellValue(YSTable.Rows[i]["COLOR"] + string.Empty);
                    //}
                }
                for (int i = 0; i < YSCount; i++)
                {

                    var row = sheetRef2.CreateRow(i + 1);

                  
                    cell22 = row.CreateCell(0);
                    cell22.SetCellValue(YSTable.Rows[i]["COLOR"] + string.Empty);
                    
                }
                #endregion

                #region 表格导出
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode(MAKE+"申请贷款模版", System.Text.Encoding.UTF8)));
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                wk = null;
                #endregion
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(msg);
            }
        }
        /// <summary>
        /// 贷款车辆-报告导出
        /// </summary>
        /// <returns></returns>
        public void ExportLoan(string dkbh, string jxs,string jxscode)
        {
            string sheetName = string.Empty;
           // string jxsid = GetUserCode(jxs).Trim();
            string sql = string.Format(@" select to_char(rownum) 序号,DKBH 贷款编号,CX 车型,CJHXZ 车架号性质,CJH 车架号,YS 颜色 ,DKJE 贷款金额 from I_CLXX where DKBH= '{0}' and jxs = '{1}'", dkbh, jxs);
            //   union all
            //   select  '合计' 合计 ,N' ' 车辆编号,N' ' 车型 ,N' ' 车架号性质,N' ' 车架号,N' ' 颜色, sum(DKJE) 贷款金额 from I_CLXX where DKBH= '{0}' group by DKJE
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            string date = DateTime.Now.ToString("yyyyMMdd");
            CurrencyClass dd = new CurrencyClass();
            dd.ExportReportCurrency(dt, sheetName + jxscode + "贷款车辆_" + date);
            //return Json(new { Count = "" }, JsonRequestBehavior.AllowGet);
        }

        public  string GetUserCode(string APPELLATION)
        {
            string sql = string.Format("select objectid  from OT_User where APPELLATION='{0}'", APPELLATION);
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
        }
        /// <summary>
        /// 导出授信额度报告
        /// </summary>
        public void ExportSXED()
        {
            DistributorController dc = new  DistributorController();
            string sheetName = string.Empty;
            string sql = string.Format(@"select v.Dealer_Code 经销商编码 ,v.dealer_name 经销商名称 ,v.Make 制造商,to_char(d.expiry_date,'yyyy-mm-dd') 授信到期日,v.availed_limit 贷款余额, v.balance_limit 可用授信额度
                                        from IN_WFS.v_dealer_make_availed_limit2 v join IN_WFS.V_DEALER_EXPIRY_DATE d on v.Dealer_Code=d.Dealer_Code ");
            DataTable dt = dc.ExecuteDataTableSql("Wholesale", sql);
            
            string date = DateTime.Now.ToString("yyyyMMdd");
            CurrencyClass dd = new CurrencyClass();
            dd.ExportReportCurrency(dt, sheetName + "信审部-授信额度报告导出_" + date);
        }

    }
}