using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Instance
{
    [Xss]
    [SqlInject]
    public class InstanceExController : CustomController
    {
        public override string FunctionCode => "";

        /// <summary>
        /// wangxg 19.10 重写QueryInstance
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="searchKey"></param>
        /// <param name="workflowCode"></param>
        /// <param name="unitID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="instaceState"></param>
        /// <returns></returns>
        public JsonResult QueryInstance(PagerInfo pagerInfo, string searchKey, string workflowCode, string unitID, DateTime? startTime, DateTime? endTime, int instaceState)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<InstanceViewModel> griddata = new List<InstanceViewModel>();
                GridViewModel<InstanceViewModel> result = new GridViewModel<InstanceViewModel>(0, griddata, pagerInfo.sEcho);

                #region 查询条件的初始化和整理
                if (string.IsNullOrEmpty(unitID))
                {
                    string rootOrgSql = "SELECT OBJECTID FROM H3.OT_ORGANIZATIONUNIT WHERE ISROOTUNIT=1";
                    unitID = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(rootOrgSql).ToString();
                    //unitID = UserValidator.UserID;
                }
                if (string.IsNullOrEmpty(workflowCode))
                {
                    //获取所有有权限发起的流程模板
                    DataTable dtworkflows = Engine.PortalQuery.QueryWorkflow(this.UserValidator.RecursiveMemberOfs, this.UserValidator.ValidateAdministrator());
                    //根据可以发起的流程模板编码，倒推获取所有的节点集合
                    List<string> aclWorkflowCodes = new List<string>();
                    foreach (DataRow row in dtworkflows.Rows)
                    {
                        if (aclWorkflowCodes.Contains(row[WorkItem.PropertyName_WorkflowCode])) continue;
                        aclWorkflowCodes.Add(row[WorkItem.PropertyName_WorkflowCode] + string.Empty);
                    }

                    string sql = $@"SELECT A.SCHEMACODE, C.NAME ORGSCOPE, B.NAME USR, B.CODE USR_CODE, A.ADMINISTRATOR, A.CREATEBIZOBJECT, A.VIEWDATA
FROM H3.OT_BIZOBJECTACL A 
LEFT JOIN H3.OT_USER B ON A.USERID=B.OBJECTID
LEFT JOIN H3.OT_ORGANIZATIONUNIT C ON A.ORGSCOPE = C.OBJECTID

LEFT JOIN H3.OT_GROUP Q ON A.USERID=Q.OBJECTID
LEFT JOIN H3.OT_GROUPCHILD W ON Q.OBJECTID = W.PARENTOBJECTID
LEFT JOIN H3.OT_USER Y ON W.CHILDID = Y.OBJECTID

LEFT JOIN H3.OT_ORGANIZATIONUNIT X ON A.USERID=X.OBJECTID
WHERE 1=1 AND (A.ADMINISTRATOR=1 OR A.CREATEBIZOBJECT=1 OR A.VIEWDATA=1 ) 
AND (B.CODE='{UserValidator.UserCode}' 
    OR Y.CODE = '{UserValidator.UserCode}' 
    OR EXISTS --UUID是当前用户所属组织
        (
            -- 联合组织和分组及用户
            SELECT DISTINCT R.CODE, P.CODE
            FROM H3.OT_ORGANIZATIONUNIT T
            LEFT JOIN H3.OT_GROUP M ON T.OBJECTID = M.PARENTID
            LEFT JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
            LEFT JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
            LEFT JOIN H3.OT_USER R ON R.PARENTID = T.OBJECTID
            WHERE  R.CODE='{UserValidator.UserCode}' OR P.CODE='{UserValidator.UserCode}' 
            START WITH T.OBJECTID = A.USERID -- UUID是组织节点
            CONNECT BY PRIOR T.OBJECTID = T.PARENTID
        )
    )";
                    DataTable schemas = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (schemas != null && schemas.Rows.Count > 0)
                    {
                        foreach (DataRow row in schemas.Rows)
                        {
                            if (aclWorkflowCodes.Contains(row["SCHEMACODE"])) continue;
                            aclWorkflowCodes.Add(row["SCHEMACODE"] + string.Empty);
                        }
                    }
                    workflowCode = string.Join(",", aclWorkflowCodes);
                }

                string startTimeFilter = "";
                if (startTime != null)
                {
                    startTimeFilter = $" A.CREATEDTIME >= to_date('{startTime.Value.ToString("yyyy-MM-dd 00:00:00")}','yyyy-mm-dd hh24:mi:ss') AND ";
                    //startTime = DateTime.Now.AddDays(-30);
                }
                string endTimeFilter = "";
                if (endTime != null)
                {
                    endTimeFilter = $" A.CREATEDTIME <= to_date('{endTime.Value.ToString("yyyy-MM-dd 23:59:59")}','yyyy-mm-dd hh24:mi:ss') AND ";
                    //endTime = DateTime.Now;
                }
                string stat = "(1)";
                switch (instaceState)
                {
                    case 0: stat = "(0,1,2)"; break;//进行中
                    case 1: stat = "(4)"; break;//结束
                    case 2: stat = "(5)"; break;//取消
                }

                workflowCode = "'" + workflowCode.Replace("'", "").Replace("--", "").Replace(",", "','") + "'";
                unitID = unitID.Replace("'", "").Replace("--", "");

                string searchFilter = "";
                if (!string.IsNullOrEmpty(searchKey))
                {
                    searchKey = searchKey.Replace("'", "").Replace("--", "").Trim();
                    searchFilter = $" INSTANCENAME LIKE '%{searchKey}%' AND ";
                }
                #endregion

                string countSql = @" SELECT COUNT(1) ";
                #region selectSql
                string selectSql = @" SELECT DISTINCT A.OBJECTID InstanceID
        , A.PRIORITY Priority
        , INSTANCENAME InstanceName
        , A.WORKFLOWCODE WorkflowCode
        , C.WORKFLOWNAME WorkflowName
        , A.ORIGINATOR Originator
        , A.ORIGINATORNAME OriginatorName
        , A.CREATEDTIME CreatedTime
        , B.PARENTID ORGUNIT
        --, D.DISPLAYNAME ApproverLink
        --, D.PARTICIPANTNAME Approver
        , A.STATE InstanceState
        , A.PLANFINISHTIME PlanFinishTime
        , A.FINISHTIME FinishedTime
        , A.EXCEPTIONAL Exceptional ";
                #endregion

                #region adminSql
                string adminSql = $@"FROM H3.OT_INSTANCECONTEXT A 
        JOIN H3.OT_USER B ON A.ORIGINATOR = B.OBJECTID
        JOIN H3.OT_WORKFLOWCLAUSE C ON A.WORKFLOWCODE = C.WORKFLOWCODE
        --LEFT JOIN  H3.OT_WORKITEM D ON A.OBJECTID = D.INSTANCEID
    WHERE {searchFilter} {startTimeFilter} {endTimeFilter} 
    A.STATE in {stat} AND
    A.WORKFLOWCODE IN ({workflowCode}) AND 
    (A.ORIGINATOR = '{unitID}' OR B.PARENTID IN
    (        
        SELECT  OBJECTID --, NAME
        FROM H3.OT_ORGANIZATIONUNIT
        START WITH OBJECTID = '{unitID}' -- 前端提交的 只查看此组织范围
        CONNECT BY PRIOR OBJECTID = PARENTID 
    ))";
                #endregion

                #region commSql
                string commSql = $@"
    FROM H3.OT_INSTANCECONTEXT A 
        JOIN H3.OT_USER B ON A.ORIGINATOR = B.OBJECTID
        JOIN H3.OT_WORKFLOWCLAUSE C ON A.WORKFLOWCODE = C.WORKFLOWCODE
        --LEFT JOIN  H3.OT_WORKITEM D ON A.OBJECTID = D.INSTANCEID
    WHERE {searchFilter} {startTimeFilter} {endTimeFilter} 
    A.STATE in {stat} 
    AND A.WORKFLOWCODE IN ({workflowCode})  
    AND (A.ORIGINATOR = '{unitID}' OR B.PARENTID IN
    (        
        SELECT  OBJECTID --, NAME
        FROM H3.OT_ORGANIZATIONUNIT
        START WITH OBJECTID = '{unitID}' -- 前端提交的 只查看此组织范围
        CONNECT BY PRIOR OBJECTID = PARENTID 
    ))
    AND (B.CODE ='{UserValidator.UserCode}'
    OR  (EXISTS -- 用户是'个人'且拥有全组织的流程权限
        (
            SELECT 1
            FROM H3.OT_BIZOBJECTACL Z JOIN H3.OT_USER X ON Z.USERID=X.OBJECTID
            WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                AND Z.ORGSCOPETYPE = 2 AND X.CODE ='{UserValidator.UserCode}' -- 模型权限 全组织
        )
    OR EXISTS -- 用户是'分组'且拥有全组织的流程权限
        (
            SELECT 1 FROM H3.OT_BIZOBJECTACL Z 
                JOIN H3.OT_GROUP M ON Z.USERID=M.OBJECTID
                JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
                JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
            WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                AND Z.ORGSCOPETYPE = 2 AND P.CODE = '{UserValidator.UserCode}' -- 模型权限 全组织
        )
    OR EXISTS -- 用户是'组织'且拥有全组织的流程权限
        (
            SELECT 1 FROM H3.OT_BIZOBJECTACL Z JOIN H3.OT_ORGANIZATIONUNIT X ON Z.USERID=X.OBJECTID --权限给的是组织，不是人
            WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                AND Z.ORGSCOPETYPE = 2 AND EXISTS --UUID是当前用户所属组织
                        (
                            -- 联合组织和分组及用户
                            SELECT DISTINCT R.CODE, P.CODE
                            FROM H3.OT_ORGANIZATIONUNIT T
                            LEFT JOIN H3.OT_GROUP M ON T.OBJECTID = M.PARENTID
                            LEFT JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
                            LEFT JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
                            LEFT JOIN H3.OT_USER R ON R.PARENTID = T.OBJECTID
                            WHERE  R.CODE='{UserValidator.UserCode}' OR P.CODE='{UserValidator.UserCode}' 
                            START WITH T.OBJECTID = Z.USERID -- UUID是组织节点
                            CONNECT BY PRIOR T.OBJECTID = T.PARENTID
                        )
        )
    OR EXISTS -- 用户是'个人'且拥有部分组织的流程权限
        (
            SELECT  OBJECTID --, NAME
            FROM H3.OT_ORGANIZATIONUNIT
            WHERE B.PARENTID IN OBJECTID
            START WITH OBJECTID IN 
            (
                SELECT ORGSCOPE
                FROM H3.OT_BIZOBJECTACL Z JOIN H3.OT_USER X ON Z.USERID=X.OBJECTID
                WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                    AND Z.ORGSCOPETYPE = 0 AND X.CODE ='{UserValidator.UserCode}' -- 模型权限 特定范围
            )
            CONNECT BY PRIOR OBJECTID = PARENTID 
        )
    OR EXISTS -- 用户是'分组'且拥有部分组织的流程权限
        (
            SELECT  OBJECTID --, NAME
            FROM H3.OT_ORGANIZATIONUNIT
            WHERE B.PARENTID IN OBJECTID
            START WITH OBJECTID IN 
            (
                SELECT ORGSCOPE
                FROM H3.OT_BIZOBJECTACL Z 
                    JOIN H3.OT_GROUP M ON Z.USERID=M.OBJECTID
                    JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
                    JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
                WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                    AND Z.ORGSCOPETYPE = 0 AND P.CODE = '{UserValidator.UserCode}' -- 模型权限 特定范围
            )
            CONNECT BY PRIOR OBJECTID = PARENTID 
        )
    OR EXISTS -- 用户是'组织'且拥有部分组织的流程权限
        (
            SELECT  OBJECTID --, NAME
            FROM H3.OT_ORGANIZATIONUNIT
            WHERE B.PARENTID IN OBJECTID
            START WITH OBJECTID IN 
            (
                SELECT ORGSCOPE
                FROM H3.OT_BIZOBJECTACL Z JOIN H3.OT_ORGANIZATIONUNIT X ON Z.USERID=X.OBJECTID --权限给的是组织，不是人
                WHERE SCHEMACODE = A.WORKFLOWCODE AND (Z.ADMINISTRATOR=1 OR Z.CREATEBIZOBJECT=1 OR Z.VIEWDATA=1 ) 
                    AND Z.ORGSCOPETYPE = 0 AND EXISTS --UUID是当前用户所属组织
                            (
                                -- 联合组织和分组及用户
                                SELECT DISTINCT R.CODE, P.CODE
                                FROM H3.OT_ORGANIZATIONUNIT T
                                LEFT JOIN H3.OT_GROUP M ON T.OBJECTID = M.PARENTID
                                LEFT JOIN H3.OT_GROUPCHILD N ON M.OBJECTID = N.PARENTOBJECTID
                                LEFT JOIN H3.OT_USER P ON N.CHILDID = P.OBJECTID
                                LEFT JOIN H3.OT_USER R ON R.PARENTID = T.OBJECTID
                                WHERE  R.CODE='{UserValidator.UserCode}' OR P.CODE='{UserValidator.UserCode}' 
                                START WITH T.OBJECTID = Z.USERID -- UUID是组织节点
                                CONNECT BY PRIOR T.OBJECTID = T.PARENTID
                            )
            )
            CONNECT BY PRIOR OBJECTID = PARENTID 
        )
    ))";
                #endregion
                int total = Convert.ToInt32(Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(countSql + (UserValidator.ValidateAdministrator() ? adminSql : commSql)).ToString()); // 记录总数

                #region pageSql
                string pageSql = $@"SELECT * FROM (
SELECT ROWNUM AS ROWNO, AA.* FROM (
    {selectSql + (UserValidator.ValidateAdministrator() ? adminSql : commSql)}
    ORDER BY A.CREATEDTIME DESC
) AA 
WHERE ROWNUM <= {pagerInfo.EndIndex}
) BB 
WHERE ROWNO >= {pagerInfo.StartIndex}";
                #endregion
                DataTable dt = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(pageSql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        #region 构造行 inst
                        InstanceViewModel inst = new InstanceViewModel()
                        {
                            InstanceID = row["InstanceID"] + string.Empty,
                            InstanceName = row["InstanceName"] + string.Empty,
                            InstanceState = row["InstanceState"] + string.Empty,
                            Approver = string.Empty,
                            ApproverLink = string.Empty,
                            CreatedTime = row["CreatedTime"] + string.Empty,
                            Originator = row["Originator"] + string.Empty,
                            OriginatorName = row["OriginatorName"] + string.Empty,
                            WorkflowCode = row["WorkflowCode"] + string.Empty,
                            FinishedTime = row["FinishedTime"] + string.Empty,
                            PlanFinishTime = row["PlanFinishTime"] + string.Empty,
                            WorkflowName = row["WorkflowName"] + string.Empty,
                            Priority = row["Priority"] + string.Empty,
                            Exceptional = Convert.ToInt32(row["Exceptional"] + string.Empty) == 1 ? true : false
                        };
                        string approvSql = $"SELECT DISPLAYNAME, PARTICIPANTNAME FROM H3.OT_WORKITEM D WHERE D.INSTANCEID='{inst.InstanceID}'";
                        DataTable approvDt = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(approvSql);
                        if(approvDt != null && approvDt.Rows.Count > 0)
                        {
                            inst.ApproverLink = approvDt.Rows[0]["DISPLAYNAME"] + string.Empty;
                            List<string> appro = new List<string>();
                            for (int i = 0; i < approvDt.Rows.Count; i++)
                            {
                                appro.Add(approvDt.Rows[i]["PARTICIPANTNAME"] + string.Empty);
                            }
                            inst.Approver = string.Join(",", appro);
                        }
                        #endregion
                        griddata.Add(inst);
                    }
                }
                result = new GridViewModel<InstanceViewModel>(total, griddata, pagerInfo.sEcho);
                result.ExtendProperty = workflowCode;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

    }
}