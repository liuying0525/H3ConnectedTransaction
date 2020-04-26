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

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    public class MessageController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageController()
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

        public JsonResult GetMessageList(PagerInfo pagerInfo, string State)
        {
            DataSet ds = MessageList(pagerInfo, State);

            DistributorController dc = new DistributorController();
            DistributorController.ToJson(ds.Tables[0]);
            int _RowCount = ds.Tables[0].Rows.Count;
            return Json(new { RowCount = _RowCount, RowData = DistributorController.ToJson(ds.Tables[1]), _Count = ds.Tables[2].Rows.Count }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateStateAll()
        {
            var UserID = this.UserValidator.UserID;
            string sql = string.Format("update  I_Messagelist set  MESSAGESTATE='1',ENDTIME= to_date('{1}','yyyy-mm-dd HH24:mi:ss') where    USERID='{0}' and  MESSAGESTATE<>'1'", UserID, DateTime.Now);
            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateState(string OBJECTID)
        { 
            string sql = string.Format("update  I_Messagelist set  MESSAGESTATE='1',ENDTIME= to_date('{1}','yyyy-mm-dd HH24:mi:ss') where    OBJECTID='{0}' and  MESSAGESTATE<>'1'", OBJECTID, DateTime.Now);
            int count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);
        }

        public DataSet MessageList(PagerInfo pagerInfo, string State)
        {
            var UserID = this.UserValidator.UserID;
            Organization.Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(UserID);
            string UserCode = ((OThinker.Organization.User)u).Appellation;
            string sql = string.Format(@"SELECT rownum rn,  aa.*  from （
                            select 
                            OBJECTID,USERID, USERCODE, MESSAGE,
                            to_char(RECEIVETIME,'yyyy-mm-dd hh24:mi:ss') RECEIVETIME , 
                            ENDTIME,
                            MESSAGESTATE,
                            MessageType,
                            QueryKey 
                            from  I_Messagelist
                            WHERE USERID='{0}' ORDER BY RECEIVETIME desc ) aa where   1=1", UserID);// or USERCODE='{1}', UserCode

            #region 查询条件
            //总条数
            DataTable count = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (!string.IsNullOrEmpty(State))
            {
                sql += string.Format(" and  MESSAGESTATE ='{0}'", State);

            }
            #endregion
            // 未读总计
            string sqlUN = string.Format(@"select MESSAGESTATE num  from  I_Messagelist  WHERE USERID='{0}' and MESSAGESTATE=0", UserID, UserCode);

            DataTable CountUN = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlUN);
            string sqlstr = "select a1.* from ( " + sql + ") a1 where   1=1 ";
            int startindex = pagerInfo.StartIndex == 1 ? 0 : pagerInfo.StartIndex;
            sqlstr += "and rn <=" + pagerInfo.EndIndex.ToString() + " and rn >= " + startindex.ToString();
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlstr);
            DataSet ds = new DataSet();
            ds.Tables.Add(count);
            ds.Tables.Add(dt);
            ds.Tables.Add(CountUN);
            return ds;

        }



        public JsonResult GetUnreadCount()
        {
            var UserID = this.UserValidator.UserID;
            string sql = "select count(MESSAGESTATE) from  I_Messagelist  WHERE USERID='" + UserID + "' and MESSAGESTATE=0";
            int count = int.Parse(OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString());

            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult Getloanlist(string[] param)
        {

            string QUERYKEY = param[0];
            string MessageType = param[1];
            var UserID = this.UserValidator.UserID;
            Organization.Unit u = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(UserID);
            string UserCode = ((OThinker.Organization.User)u).Appellation;
            
            string sql = string.Empty;
            if (MessageType == "1")
            {
                #region 贷款申请状态
                sql = string.Format(@" select distinct UNIT_NO CLBH , CX ,  CJHXZ, CJH,
                           TO_CHAR(DKJE ,'FM999,999,999,999,990.00') DKJE,
                           case when V.LOAN_STATUS ='05' or V.LOAN_STATUS ='10' then '处理中'
                                when V.LOAN_STATUS = '40' and V.PAID_AMT>0 then '已放款'
                                when V.LOAN_STATUS = '40' and V.PAID_AMT<=0 then '处理中'
                                when V.LOAN_STATUS = '50' or V.LOAN_STATUS ='30' then '已拒绝'  
                                when V.LOAN_STATUS = '60'  then '已售出'  end  SQZT
                            from I_CLXX C join IN_WFS.V_LOAN_STOCK_LIST@To_Auth_Wfs v on trim(c.CJH) = trim(v.VIN_NO) where DKBH='{0}'", QUERYKEY);
                #endregion
            }
            else if (MessageType == "3")
            {
                #region 临时车架号提醒
//                sql = string.Format(@"select   UNIT_NO CLBH ,ASSET_DESC CX,VIN_NO CJH from  IN_WFS.V_LOAN_STOCK_LIST v 
//                    where   ORDER_NO = 'Y' and VIN_UPDATE = 'N' and  V.LOAN_STATUS = '40' and V.PAID_AMT>0
//                           and STOCK_DATE <(to_date('{0}','yyyy-MM-dd') - 5）  and v.dealer_code='{1}'",
//                           DateTime.Now.ToShortDateString(), UserCode);

                sql = string.Format(@"select distinct UNIT_NO CLBH ,MODEL CX,VIN_NO CJH  from  
IN_WFS.V_STOCK_DC_HIS v where   ORDER_NO = 'Y' and VIN_UPDATE = 'N' 
                        and LOAN_GIVEN_DATE <=(to_date('{0}','yyyy-MM-dd') - 5)
                         and v.dealer_code='{1}'", DateTime.Now.ToShortDateString(), QUERYKEY);

                #endregion
            }
            else if (MessageType == "4")
            {
                #region 车架号修改
                sql = string.Format(@"select  BATCH, VEHICLENUMBER CLBH, CARMODEL CX , TEMPORARYFRAMENUMBER LSCJH,  PERMANENTFRAMENUMBER YJCJH  from  I_FrameNumberMiddleTable where BATCH='{0}'", QUERYKEY);

                #endregion

            }
            DataTable dt = new DataTable();
            DistributorController DC = new DistributorController();
            if (MessageType == "4" || MessageType == "1")
                dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            else
                dt = DC.ExecuteDataTableSql("Wholesale", sql);

            return Json(new { RowData = DistributorController.ToJson(dt) }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetWORKITEMID(string QUERYKEY)
        {
            var UserID = this.UserValidator.UserID;
            #region 清算异常
            string sql = string.Format(@" select objectid WORKITEMID from  Ot_Workitemfinished where instanceid='{0}' and PARTICIPANT='{1}'", QUERYKEY, UserID);

            #endregion
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            return Json(new { RowData = DistributorController.ToJson(dt) }, JsonRequestBehavior.AllowGet);


        }


    }

}