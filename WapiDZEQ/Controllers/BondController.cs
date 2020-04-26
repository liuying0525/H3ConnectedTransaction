using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



/// <summary>
/// BondListController 的摘要说明
/// </summary>
[Authorize]
public class BondController : OThinker.H3.Controllers.ControllerBase
{
	public BondController()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
       
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
  

    [HttpPost]
    public System.Web.Mvc.JsonResult UpdateBondApproval(string param)
    {
        WorkItemServer Ws = new WorkItemServer();
        string[] paramS = param.Split(',');
       // DataTable dt = QueryBondApproval(paramS);

      //  List<BondViewModel> paramList = this.Getgriddata(dt);//this.GetgriddataEdit(dtWorkitem, param);

       // List<BondViewModel> paramList = param;
        //System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        //json.MaxJsonLength = int.MaxValue;
        //paramList = json.Deserialize<List<BondViewModel>>(param);  

      
        return this.ExecuteFunctionRun(() =>
        {
            OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true);
            var code = this.UserValidator.UserID;
            foreach (string item in paramS)
            {
                string[] str =  item.Split(';');
                if (string.IsNullOrEmpty(str[0]) || str[0] == "null") continue;
                List<DataItemParam> paramValues = new List<DataItemParam>();
                //paramValues.Add(new DataItemParam()
                //{
                //    ItemName = "Distributorcode",
                //    ItemValue = item.Distributorcode
                //});
                //paramValues.Add(new DataItemParam()
                //{
                //    ItemName = "Distributorname",
                //    ItemValue = item.Distributorname
                //});
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "BondProportion",
                    ItemValue = str[0]
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "OperationTime",
                    ItemValue = DateTime.Now.ToLongDateString()
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "FinalState",
                    ItemValue = "已生效"
                });
               
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "BondState",
                    ItemValue = "已生效"
                });


                Ws.ForwardWorkItem(paramValues, code, "Bond", str[2], str[3]);
                string sql = @"update I_Bond
set BondState = '已失效',FinalState = '已失效'
where objectid !='{0}' and Distributorcode ='{1}'";

                sql = string.Format(sql, str[4], str[5]);

                OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);

            }

            return Json(result, "text/html", JsonRequestBehavior.AllowGet);
        }, string.Empty);
    }

    public System.Web.Mvc.JsonResult GetApprovalBond(string[] param)
    {

        return this.ExecuteFunctionRun(() =>
        {

            DataTable dt = QueryBondApproval(param);

            List<BondViewModel> bondViewModelList = this.Getgriddata(dt);//this.GetgriddataEdit(dtWorkitem, param);          


            var result = new
            {
                Success = bondViewModelList != null,
                Bond = bondViewModelList,

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    public System.Web.Mvc.JsonResult GetBondShow(string param)
    {

        return this.ExecuteFunctionRun(() =>
        {
            string[] ps = param.Split(';');
            string distributorCode = ps[0];
            string distributorName = ps[1];
            DataTable dt = QueryBondShow(param);
            List<BondViewModel> bondViewModelList = this.Getgriddata(dt);
            var result = new
            {
                
                DistributorName = distributorName,
                BondViewModelList = bondViewModelList,
               

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    public System.Web.Mvc.JsonResult GetBondEdit(string[] param)
    {
      
        return this.ExecuteFunctionRun(() =>
        {
          
                DataTable dt = QueryBondEdit(param);
             
                List<BondViewModel> bondViewModelList = this.Getgriddata(dt);//this.GetgriddataEdit(dtWorkitem, param);          

            
            var result = new
            {
                Success = bondViewModelList != null,
                Bond = bondViewModelList,
                
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    public System.Web.Mvc.JsonResult GetBondFinalEdit(string[] param)
    {

        return this.ExecuteFunctionRun(() =>
        {

            DataTable dt = QueryBondFinalEdit(param);

            List<BondViewModel> bondViewModelList = this.Getgriddata(dt);//this.GetgriddataEdit(dtWorkitem, param);          


            var result = new
            {
                Success = bondViewModelList != null,
                Bond = bondViewModelList,

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    public System.Web.Mvc.JsonResult GetBondList(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string state,string distributorCode ,string Status)
    {
        return this.ExecuteFunctionRun(() =>
        {
            int total = 0;
            DataTable dtWorkitem = QueryBondList(pagerInfo, keyWord, startDate, endDate, state, distributorCode, Status, ref total);

           // string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };
            List<BondViewModel> griddata = this.Getgriddata(dtWorkitem);
            GridViewModel<BondViewModel> result = new GridViewModel<BondViewModel>(total, griddata, pagerInfo.sEcho);
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    public System.Web.Mvc.JsonResult GetFinalBondList(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string state, string distributorCode, string Status)
    {
        return this.ExecuteFunctionRun(() =>
        {
            int total = 0;
            DataTable dtWorkitem = QueryFinalBondList(pagerInfo, keyWord, startDate, endDate, state,distributorCode, Status, ref total);

            // string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };
            List<BondViewModel> griddata = this.Getgriddata(dtWorkitem);
            GridViewModel<BondViewModel> result = new GridViewModel<BondViewModel>(total, griddata, pagerInfo.sEcho);
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }


    [HttpPost]
    public System.Web.Mvc.JsonResult UpdateBondEdit(string param)
        {
            WorkItemServer Ws = new WorkItemServer();
            
            string[] WorkItems = param.Split(',');
           
                
            
            return this.ExecuteFunctionRun(() =>
            {
                OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true);
                var code = this.UserValidator.UserCode;
                foreach (string item in WorkItems)
                {
                    string[] pv = item.Split(';');
                    if (string.IsNullOrEmpty(pv[2].Trim()) || pv[2].Trim() == "null") continue;
                    List<DataItemParam> paramValues = new List<DataItemParam>();
                    paramValues.Add(new DataItemParam()
                    {
                        ItemName = "Distributorcode",
                        ItemValue = pv[0].Trim()
                    });
                    paramValues.Add(new DataItemParam()
                    {
                        ItemName = "Distributorname",
                        ItemValue = pv[1].Trim()
                    });
                    paramValues.Add(new DataItemParam()
                    {
                        ItemName = "BondProportion",
                        ItemValue = Convert.ToDecimal(pv[2].Trim())
                    });
                    paramValues.Add(new DataItemParam()
                    {
                        ItemName = "OperationTime",
                        ItemValue = DateTime.Now.ToLongDateString()
                    });

                    string sql = "select objectid from Ot_User where APPELLATION = '" + pv[0].Trim() + "' and STATE =1 ";
                    var JXSH3ID = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
                    if (JXSH3ID == null || JXSH3ID.ToString() == "")
                    {
                        sql = "select objectid from Ot_User where APPELLATION = '" + pv[0].Trim() + "'";
                        JXSH3ID = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
                    }
                    paramValues.Add(new DataItemParam()
                    {
                        ItemName = "JXSH3ID",
                        ItemValue = JXSH3ID
                    });


                    if (pv[3].Trim() == "待创建" || pv[3].Trim() == "已生效")
                    {
                        paramValues.Add(new DataItemParam()
                        {
                            ItemName = "BondState",
                            ItemValue = "终审中"
                        });
                        if (pv[3].Trim() == "待创建")
                        {
                            paramValues.Add(new DataItemParam()
                            {
                                ItemName = "FinalState",
                                ItemValue = "创建待审核"
                            });
                        }
                        if (pv[3].Trim() == "已生效")
                        {
                            paramValues.Add(new DataItemParam()
                            {
                                ItemName = "FinalState",
                                ItemValue = "修改待审核"
                            });
                        }
                        Ws.startWorkflow("Bond", code, true, paramValues);
                    }

                }

                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

    [HttpPost]
    public System.Web.Mvc.JsonResult UpdateBondFinalEdit(string page)
    {
        WorkItemServer Ws = new WorkItemServer();

        string[] WorkItems = page.Split(',');
        //string[] WorkItems = param;

        return this.ExecuteFunctionRun(() =>
        {
            OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true);
            var code = this.UserValidator.UserCode;
            foreach (string item in WorkItems)
            {
                if (string.IsNullOrEmpty(item)) continue;
               
                string[] pv = item.Split(';');
                if (string.IsNullOrEmpty(pv[2].Trim()) || pv[2].Trim()=="null") continue;
                string sql = @"update I_Bond
set BondState = '已失效',FinalState = '已失效'
where objectid ='{0}' and Distributorcode ='{1}'";

                sql = string.Format(sql, pv[3].Trim(), pv[0].Trim());

                OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);

                string H3IDSql = string.Format(@"select objectid from Ot_User where APPELLATION = '{0}'  and STATE =1 ", pv[0].Trim());

                var H3ID = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(H3IDSql);

                if (H3ID == null || H3ID.ToString() == "")
                {
                    sql = "select objectid from Ot_User where APPELLATION = '" + pv[0].Trim() + "'";
                    H3ID = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
                }

                List<DataItemParam> paramValues = new List<DataItemParam>();
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "Distributorcode",
                    ItemValue = pv[0].Trim()
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "Distributorname",
                    ItemValue = pv[1].Trim()
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "BondProportion",
                    ItemValue = Convert.ToDecimal(pv[2].Trim())
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "OperationTime",
                    ItemValue = DateTime.Now.ToLongDateString()
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "BondState",
                    ItemValue = "已生效"
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "FinalState",
                    ItemValue = "已生效"
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "ISYN",
                    ItemValue = "是"
                });
                paramValues.Add(new DataItemParam()
                {
                    ItemName = "JXSH3ID",
                    ItemValue = H3ID
                });

                Ws.startWorkflow("Bond", code, true, paramValues);
                
            }

            return Json(result, "text/html", JsonRequestBehavior.AllowGet);
        }, string.Empty);
    }


    public JsonResult GetBondCount()
    {
        string UserID = this.UserValidator.UserID;
        return this.ExecuteFunctionRun(() =>
        {
            OThinker.H3.Controllers.ActionResult result = new OThinker.H3.Controllers.ActionResult(true);
            Dictionary<string, int> Extend = new Dictionary<string, int>();
            string sql1 = @"select count(distinct Distributorcode) counts
from I_Bond";
            string sql2 = @"select count(distinct DEALER_CODE)count from IN_WFS.V_DEALER_INFO v";

            var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql1);
            int num1 = Convert.ToInt32(count);
            int num2 = ExecuteCountSql("Wholesale", sql2);

             //= @"select count(1) counts from I_Bond where BondState='终审中'";
            string sql3 = @"SELECT count(1)
 from i_bond A
JOIN (select Max(b.Modifiedtime)  OperationTime
             ,b.distributorcode
        from i_bond  b  
        group by b.distributorcode
      ) B ON A.Distributorcode = B.Distributorcode and B.OperationTime = A.Modifiedtime
join Ot_Instancecontext c on c.bizobjectid = A.OBJECTID 
                      join (select a.objectid workItemcode ,a.instanceid,a.starttime
                               from (select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{0}'
                               union 
                               select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{0}') a
                               join ( 
                               select Max(starttime)receivetime,objectid
                               from (
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{0}'
                               union 
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{0}')
                               group by objectid) b on a.instanceid = b.objectid and b.receivetime = a.starttime
                               
                               ) d on d.instanceid = c.objectid
WHERE 1=1 AND FinalState in('创建待审核','修改待审核')";

            var count3 = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sql3,UserID));

            int total1 = num2 - num1;
            int total2 = Convert.ToInt32(count3);
    

            Extend["BondCount"] = total1;
            Extend["BondFinalCount"] = total2;
            result.Extend = Extend;
            return Json(result, JsonRequestBehavior.AllowGet);
        });
    }

    private DataTable QueryFinalBondList(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string state,string distributorCode,string Status, ref int totalNumber)
    {

        string Conditions = "";//其它查询条件
        string sql = "";
        string UserID = this.UserValidator.UserID;

        #region 其它查询条件
        if (!string.IsNullOrEmpty(startDate))//接收时间的开始时间范围
        {
            Conditions += string.Format("AND A.OperationTime>=to_date('{0}','yyyy-mm-dd')\r\n", startDate);
        }
        if (!string.IsNullOrEmpty(endDate))//接收时间的结束时间范围
        {
            Conditions += string.Format("AND A.OperationTime<=to_date('{0}','yyyy-mm-dd')\r\n", endDate);
        }
        if (!string.IsNullOrEmpty(state))
        {
            Conditions += string.Format("AND FinalState in('{0}')", state);
        }

        if (!string.IsNullOrEmpty(keyWord))
        {
            Conditions += string.Format("AND A.Distributorname LIKE N'%{0}%' \r\n", keyWord);
        }
        if (!string.IsNullOrEmpty(distributorCode))
        {
            Conditions += string.Format("AND A.Distributorcode LIKE N'%{0}%' \r\n", distributorCode);
        }
        if (!string.IsNullOrEmpty(Status))
        {
            Conditions += string.Format("AND e.Status ='{0}' \r\n", Status);
        }
        #endregion

        #region 排序

        string OrderBy = " order by ORDER_FINALSTATE, OperationTime desc ";//order by A.BondState desc, A.Modifiedtime, reDateTime asc
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = "ORDER BY ";
            if (pagerInfo.iSortCol_0 == 4)
            {
                OrderBy += " A.Modifiedtime " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 5)
            {
                OrderBy += " a.FinalState " + pagerInfo.sSortDir_0.ToUpper();
            }

        }
        #endregion
        string Conditions1 = Conditions;
        Conditions = Conditions + OrderBy;

        #region 获取待办任务列表

        sql = @" SELECT  *
FROM    (
  SELECT    rownum RowNumber_,P.* from( select
                     A.Objectid, A.Distributorcode,A.Distributorname 
                     --,A.BondProportion
                     ,case when A.BondState='终审中' then (select BondProportion from I_Bond i where  BondState = '已生效' and i.Distributorcode =A.Distributorcode ) else A.BondProportion end BondProportion
                     ,A.Modifiedtime OperationTime,A.BondState 
                     ,c.objectid instanceid,d.workItemCode,a.FinalState
                     , d.RECEIVETIME reDateTime
                     ,case when FinalState = '创建待审核' or FinalState='修改待审核' then 1 else 2 end ORDER_FINALSTATE
                     from i_bond A
                     JOIN (select Max(b.Modifiedtime) as OperationTime
                                ,b.distributorcode
                                from i_bond  b  
                                group by b.distributorcode
                      ) B ON A.Distributorcode = B.Distributorcode and A.Modifiedtime = b.OperationTime
                      join Ot_Instancecontext c on c.bizobjectid = A.OBJECTID 
                      join (select a.objectid workItemcode ,a.instanceid,a.starttime,a.RECEIVETIME
                               from (select a.objectid instanceid,b.starttime,b.objectid,b.RECEIVETIME
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{3}'
                               union 
                               select a.objectid instanceid,b.starttime,b.objectid,b.RECEIVETIME
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{3}') a
                               join ( 
                               select Max(starttime)receivetime,objectid
                               from (
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{3}'
                               union 
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{3}')
                               group by objectid) b on a.instanceid = b.objectid and b.receivetime = a.starttime
                               
                               ) d on d.instanceid = c.objectid
           WHERE 1=1 {0} ) P ) T
WHERE   RowNumber_ >= {1}
        AND RowNumber_ <= {2}       
       
";

        sql = string.Format(sql, Conditions, pagerInfo.StartIndex, pagerInfo.EndIndex,UserID);
        #endregion

        #region 获取待办任务总数
        string sqlGetNumber = @"
		 SELECT count(1)
 from i_bond A
JOIN (select Max(b.Modifiedtime)  OperationTime
             ,b.distributorcode
        from i_bond  b  
        group by b.distributorcode
      ) B ON A.Distributorcode = B.Distributorcode and B.OperationTime = A.Modifiedtime
join Ot_Instancecontext c on c.bizobjectid = A.OBJECTID 
                      join (select a.objectid workItemcode ,a.instanceid,a.starttime
                               from (select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{1}'
                               union 
                               select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{1}') a
                               join ( 
                               select Max(starttime)receivetime,objectid
                               from (
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2' and b.PARTICIPANT = '{1}'
                               union 
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4' and b.PARTICIPANT = '{1}')
                               group by objectid) b on a.instanceid = b.objectid and b.receivetime = a.starttime
                               
                               ) d on d.instanceid = c.objectid
WHERE 1=1 {0} 
";

        sqlGetNumber = string.Format(sqlGetNumber, Conditions1, UserID);
        #endregion

        

        DataTable dt = new DataTable();

        var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetNumber);
        totalNumber = Convert.ToInt32(count);
        dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
    

        return dt;
    }


    private DataTable QueryBondList(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string state, string distributorCode,string Status, ref int totalNumber)
    {
    
        string Conditions = "";//其它查询条件
       
        string sql = "";
       
        

        #region 其它查询条件
        if (!string.IsNullOrEmpty(startDate))//接收时间的开始时间范围
        {
            Conditions += string.Format("AND to_char(W.OperationTime,'yyyy-mm-dd')>=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", startDate);
        }
        if (!string.IsNullOrEmpty(endDate))//接收时间的结束时间范围
        {
            Conditions += string.Format("AND to_char(W.OperationTime,'yyyy-mm-dd')<=to_char(to_date('{0}','yyyy-mm-dd'),'yyyy-mm-dd')\r\n", endDate);
        }
        if (!string.IsNullOrEmpty(state) )
        {
            Conditions += string.Format("AND W.BondState in('{0}')", state);
        }
      
        if (!string.IsNullOrEmpty(keyWord))
        {
            Conditions += string.Format("AND W.Distributorname LIKE N'%{0}%' \r\n", keyWord);
            
        }
        if (!string.IsNullOrEmpty(distributorCode))
        {
            Conditions += string.Format("AND W.Distributorcode LIKE N'%{0}%' \r\n", distributorCode);
            
        }
        if (!string.IsNullOrEmpty(Status))
        {
            Conditions += string.Format("AND W.Status ='{0}' \r\n", Status);
        }
        #endregion

  

        #region 获取待办任务总数
        string sqlGetNumber = @"
		 select count(1) from(
SELECT distinct e.Dealer_Code Distributorcode,e.BP_NAME Distributorname 
,A.Modifiedtime OperationTime
                     , NVL(BondState, '待创建') BondState
 from i_bond A
JOIN (select Max(b.Modifiedtime)  OperationTime
             ,b.distributorcode
        from i_bond  b  
        group by b.distributorcode
      ) B ON A.Distributorcode = B.Distributorcode and B.OperationTime = A.Modifiedtime
 right join IN_WFS.V_DEALER_INFO@TO_AUTH_WFS e on trim(e.Dealer_Code) = trim(a.DistributorCode)) W
WHERE 1=1 {0} 
";
        

        sqlGetNumber = string.Format(sqlGetNumber, Conditions); 
      
        #endregion

        #region 排序

        string OrderBy = " order by  ORDER_DCK  ,OperationTime desc , Distributorcode ";//, reDateTime asc, Distributorcode
        if (pagerInfo.iSortCol_0 != 0)
        {
            OrderBy = " ORDER BY ";
            if (pagerInfo.iSortCol_0 == 4)
            {
                OrderBy += " W.reDateTime " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 5)
            {
                OrderBy += " W.OperationTime " + pagerInfo.sSortDir_0.ToUpper();
            }

        }
        #endregion

        
        

        DataTable dt = new DataTable();
        DataTable dtdis = new DataTable();
        int num = 0;
     
            sql = @" SELECT  *
FROM    ( select  rownum  RowNumber_,V.* from (
select W.* from (
SELECT  distinct  A.Objectid, e.Dealer_Code Distributorcode,e.BP_NAME Distributorname 
                     ,case when A.BondState='终审中' 
                            then (select BondProportion from I_Bond i where  BondState = '已生效' and i.Distributorcode =A.Distributorcode ) 
                            else A.BondProportion 
                        end BondProportion
                     ,A.Modifiedtime OperationTime
                     , NVL(BondState, '待创建') BondState
                     ,c.objectid instanceid,a.FinalState
                     , LMT_START_DATE  reDateTime
                     , case when NVL(BondState, '待创建')='待创建' then  1 else 2 end ORDER_DCK
                     from i_bond A
                     JOIN (select Max(b.Modifiedtime) as OperationTime
                                ,b.distributorcode
                                from i_bond  b  
                                group by b.distributorcode
                      ) B ON A.Distributorcode = B.Distributorcode and A.Modifiedtime = b.OperationTime
                      join Ot_Instancecontext c on c.bizobjectid = A.OBJECTID 
                      join (select a.objectid workItemcode ,a.instanceid, a.starttime
                               from (select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2'
                               union 
                               select a.objectid instanceid,b.starttime,b.objectid
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4') a
                               join ( 
                               select Max(starttime)receivetime,objectid
                               from (
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitem b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '2'
                               union 
                               select a.objectid,b.starttime
                               from Ot_Instancecontext a
                               join Ot_Workitemfinished b on a.objectid = b.instanceid
                               where a.workflowcode = 'Bond' and a.state = '4')
                               group by objectid) b on a.instanceid = b.objectid and b.receivetime = a.starttime
                               
                               ) d on d.instanceid = c.objectid
                     right join IN_WFS.V_DEALER_INFO@to_auth_wfs e on trim(e.Dealer_Code) = trim(a.DistributorCode)) W where 1=1 {0} {3}) V 
                ) T
WHERE   RowNumber_ >= {1}
        AND RowNumber_ <= {2}       
       
";
           
            sql = string.Format(sql, Conditions, pagerInfo.StartIndex, pagerInfo.EndIndex,OrderBy);
            var count = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetNumber);
            num = Convert.ToInt32(count);
            dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            totalNumber = num;
         


            return dt;
    }

    private DataTable QueryBondApproval(string[] param)
    {
        string bondId = "";

        if (param == null || param.Count() < 0) return null;
        foreach (var item in param)
        {
            bondId += "'" + item.Split(';')[0].Trim() + "',";
           
        }
        bondId = bondId.Trim(',').Trim();
      
        string UserID = this.UserValidator.UserID;

        string sql = @"
select b.DistributorCode
, b.DistributorName
,b.BondState
,b.BondProportion
,b.BondProportion NewBondProportion
,b.FinalState
,a.originatorname
,c.objectid  WorkItemCode
,a.objectid  InstanceId
,b.objectid
,c.ActivityCode
, case when d.finalstate='创建待审核'then null else d.bondproportion end proportion
from Ot_Instancecontext a
join I_Bond b on a.bizobjectid = b.objectid
join Ot_Workitem c on c.InstanceId = a.objectid
Left join I_Bond d on b.DistributorCode = d.DistributorCode
where (d.finalstate = '已生效' or d.finalstate='创建待审核') and  b.objectid in ({0})  and c.PARTICIPANT = '{1}'
";

        sql = string.Format(sql, bondId, UserID);
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


        return dt;
    }


    /// <summary>
    /// 获取返回到前端的BondViewModel
    /// </summary>
    /// <param name="dtWorkItem"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public List<BondViewModel> Getgriddata(DataTable dt)
    {

        List<BondViewModel> griddata = new List<BondViewModel>();
        if (dt == null) return griddata;
        foreach (DataRow row in dt.Rows)
        {
            BondViewModel bv= new BondViewModel();
            if (dt.Columns.Contains("RowNumber_"))
            bv.RowNumber_ = Convert.ToInt32(this.GetColumnsValue(row, "RowNumber_"));
            if (dt.Columns.Contains("Distributorcode"))
            bv.Distributorcode = this.GetColumnsValue(row, "Distributorcode");
            if (dt.Columns.Contains("Distributorname"))
            bv.Distributorname = this.GetColumnsValue(row, "Distributorname");
            if (dt.Columns.Contains("BondProportion"))
                if(row["BondProportion"].ToString()== "")
                bv. BondProportion =  null;
                else 
                bv. BondProportion = Convert.ToDecimal(this.GetColumnsValue(row, "BondProportion"));
            if (dt.Columns.Contains("OperationTime"))
                if (row["OperationTime"].ToString() == "")
                    bv.OperationTime = null; 
                else
                    bv.OperationTime = this.GetValueFromDate(row["OperationTime"], "yyyy-MM-dd HH:mm");
            if (dt.Columns.Contains("BondState"))
            bv.BondState = this.GetColumnsValue(row, "BondState");
            if (dt.Columns.Contains("WorkItemCode"))
            bv.WorkItemCode = this.GetColumnsValue(row, "WorkItemCode");
            if (dt.Columns.Contains("InstanceId"))
                bv.InstanceId = this.GetColumnsValue(row, "InstanceId");
            if (dt.Columns.Contains("ObjectId"))
            bv.ObjectId = this.GetColumnsValue(row, "ObjectId");
            if (dt.Columns.Contains("FinalState"))
                bv.FinalState = this.GetColumnsValue(row, "FinalState");
            if (dt.Columns.Contains("Originatorname"))
                bv.Originatorname = this.GetColumnsValue(row, "Originatorname");

            if (dt.Columns.Contains("ActivityCode"))
                bv.ActivityCode = this.GetColumnsValue(row, "ActivityCode");
            if (dt.Columns.Contains("NewBondProportion"))
                if (row["NewBondProportion"].ToString() == "")
                    bv.NewBondProportion = null;
                    else
                    bv.NewBondProportion = Convert.ToDecimal(this.GetColumnsValue(row, "NewBondProportion"));
            if (dt.Columns.Contains("Proportion"))
                if (row["Proportion"].ToString() == "")
                    bv.Proportion = null;
                else
                    bv.Proportion = Convert.ToDecimal(this.GetColumnsValue(row, "Proportion"));
            if (dt.Columns.Contains("reDateTime"))
                if (row["reDateTime"].ToString() == "")
                    bv.reDateTime = null;
                else
                    bv.reDateTime = this.GetValueFromDate(row["reDateTime"], "yyyy-MM-dd HH:mm");
            if (dt.Columns.Contains("Status"))
                bv.Status = this.GetColumnsValue(row, "Status");

            griddata.Add(bv);
        }
        return griddata;
    }
    //ObjectId+";"Distributorcode + ";" WorkItemCode + ";" InstanceId 
    private DataTable QueryBondEdit(string[] param)
    {
        string bondId = "";
        string Distributorcode = "";
        if (param==null ||param.Count() < 0) return null;
        foreach (var item in param)
        {
            bondId += "'"+item.Split(';')[0].Trim() + "',";
            Distributorcode = Distributorcode+"'"+item.Split(';')[1].Trim() + "',";
        }
        bondId = bondId.Trim(',').Trim();
        Distributorcode = Distributorcode.Trim(',').Trim();

        
        string sql = @"
select DistributorCode, DistributorName,BondState,BondProportion,null NewBondProportion,FinalState
from I_Bond where objectid in ({0})
";
       
        sql = string.Format(sql, bondId);
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

        string sqlDis = @"select distinct Dealer_Code DistributorCode,BP_NAME DistributorName,'待创建' BondState,null BondProportion ,null NewBondProportion,null FinalState from IN_WFS.V_DEALER_INFO v where v.dealer_code in ({0}) ";
        sqlDis = string.Format(sqlDis, Distributorcode);
        DataTable dtDis = ExecuteDataTableSql("Wholesale", sqlDis);

        foreach (DataRow bond in dt.Rows)
        {
            foreach (DataRow dis in dtDis.Rows)
            {
                if (this.GetColumnsValue(bond, "DistributorCode").Trim() == this.GetColumnsValue(dis, "DistributorCode").Trim())
                {

                    dis["BondProportion"] = bond["BondProportion"];

                    dis["BondState"] = bond["BondState"];
                    dis["FinalState"] = bond["FinalState"];

                    break;
                }

            }

        }

        return dtDis;
    }

    private DataTable QueryBondFinalEdit(string[] param)
    {
        string bondId = "";
        string Distributorcode = "";
        if (param == null || param.Count() < 0) return null;
        foreach (var item in param)
        {
            bondId += "'" + item.Split(';')[0].Trim() + "',";
            Distributorcode = Distributorcode + "'" + item.Split(';')[1].Trim() + "',";
        }
        bondId = bondId.Trim(',').Trim();
        Distributorcode = Distributorcode.Trim(',').Trim();


        string sql = @"
select ObjectId, DistributorCode, DistributorName,BondState,BondProportion,null NewBondProportion,FinalState
from I_Bond where objectid in ({0}) and FinalState = N'已生效'
";

        sql = string.Format(sql, bondId);
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

        return dt;
    }

    public BondViewModel GetgriddataEdit(DataTable dtWorkItem,string keyWord)
    {
        string[] param = keyWord.Split(';');
        string objectId = param[0];
        string distributorCode = param[1];
        string workItemCode = param[2];
        string instanceId = param[3];
        string distributorName = param[4];

        BondViewModel bv = new BondViewModel();
        if (dtWorkItem.Rows.Count > 0)
        {
            foreach (DataRow row in dtWorkItem.Rows)
            {

                bv.Distributorcode = this.GetColumnsValue(row, "DistributorCode");
                bv.Distributorname = this.GetColumnsValue(row, "DistributorName");
                if (this.GetColumnsValue(row, "BondProportion") == "")
                    bv.BondProportion = null;
                else
                    bv.BondProportion = Convert.ToDecimal(this.GetColumnsValue(row, "BondProportion"));
                bv.BondState = this.GetColumnsValue(row, "BondState");                
                bv.WorkItemCode = workItemCode;
                bv.InstanceId = instanceId;

            }
        }
        else
        {
            bv.Distributorcode = distributorCode;
            bv.Distributorname = distributorName;           
            bv.BondProportion = null;
            bv.BondState = "待创建";
            bv.WorkItemCode = workItemCode;
            bv.InstanceId = instanceId;

        }
        return bv;
    }

    private DataTable QueryBondShow(string param)
    {
        string[] ps = param.Split(';');
        string distributorCode = ps[0].Trim();
        string distributorName = ps[1].Trim();

        string sql = @"
select DistributorCode, DistributorName
, BondState
, BondProportion
,case when BondState='终审中' then NULL 
 else Modifiedtime end OperationTime
from I_Bond where DistributorCode = '{0}' order by Modifiedtime
";
        sql = string.Format(sql, distributorCode);
        DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        return dt;

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

    public int ExecuteCountSql(string connectionCode, string sql)
    {
        var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
        if (dbObject != null)
        {
            OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
            var command = factory.CreateCommand();
            var count = command.ExecuteScalar(sql);
            return Convert.ToInt32(count);
        }
        return 0;
    }

    public string GetColumnsValue(DataRow row, string columns)
    {
        return row.Table.Columns.Contains(columns) ? row[columns] + string.Empty : "";
    }

    public class page
    {
        public string name { get; set; }
        public string code { get; set; }
    } 
   
}