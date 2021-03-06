﻿using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 

namespace OThinker.H3.Portal
{
    public partial class RSRiskManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var sr = new StreamReader(Request.InputStream);
                var stream = sr.ReadToEnd();
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("融数风控回调接收成功！参数：" + stream);
                JavaScriptSerializer js = new JavaScriptSerializer();
                var rsresult = js.Deserialize<RSResult>(stream);
                RSRiskControllerResult(rsresult);
                this.Context.Response.Write("ok");
            }
            catch (Exception ex)
            {
                this.Context.Response.Write("error");
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("融数风控回调执行结果：" + ex.ToString());
            }
        }
        public void RSRiskControllerResult(RSResult entity)
        {
            bool result = false;
            if (entity != null && entity.reqID != "")
            {
                WorkFlowFunction wf = new WorkFlowFunction();
                //00 - 成功
                //01 - 正在处理中
                //02 - 错误
                //03 - 失败
                //04 - 超时转人工

                List<DataItemParam> keyValues = new List<DataItemParam>();
                string fkResult = "";
                if (entity.code == "00")
                {
                    //localreject: 部署在东正本地规则拒绝；
                    //cloudaccept: 部署在云端规则通过；
                    //cloudreject: 部署在云端规则拒绝；
                    //cloudmanual: 部署在云端规则返回转人工
                    fkResult = entity.data.action;
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "fkResult",
                        ItemValue = fkResult
                    });
                    //Automatic_approval：0人工（默认值），1自动审批，-1拒绝
                    if (fkResult == "cloudaccept")
                    {
                        //在融数通过的情况下，根据东正的业务判断是否需要走自动审批
                        #region
                        int auto = 0;
                        auto = wf.AutomaticApprovalByRongShu(entity.reqID);
                        keyValues.Add(new DataItemParam()
                        {
                            ItemName = "Automatic_approval",
                            ItemValue = auto
                        });
                        #endregion
                    }
                    else
                    {
                        keyValues.Add(new DataItemParam()
                        {
                            ItemName = "Automatic_approval",
                            ItemValue = 0
                        });
                    }
                    string SchemaCode = wf.getSchemaCodeByInstanceID(entity.reqID);
                    wf.SetItemValues(SchemaCode, wf.getBizobjectByInstanceid(entity.reqID), keyValues, wf.GetUserIDByCode("rsfk"));
                    result = wf.SubmitWorkItemByRongShu(entity.reqID, "", wf.GetUserIDByCode("rsfk"));
                }
                else if (entity.code == "02")
                {
                    //localreject: 部署在东正本地规则拒绝；
                    //cloudaccept: 部署在云端规则通过；
                    //cloudreject: 部署在云端规则拒绝；
                    //cloudmanual: 部署在云端规则返回转人工
                    //manual：1--》 人工查询外部数据源
                    //manual：2--》 超时重提进件
                    fkResult = entity.data.action;
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "fkResult",
                        ItemValue = "error"
                    });
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 0
                    });
                    string SchemaCode = wf.getSchemaCodeByInstanceID(entity.reqID);
                    wf.SetItemValues(SchemaCode, wf.getBizobjectByInstanceid(entity.reqID), keyValues, wf.GetUserIDByCode("rsfk"));
                    result = wf.SubmitWorkItemByRongShu(entity.reqID, "", wf.GetUserIDByCode("rsfk"));
                }
                else if (entity.code == "03")
                {
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("融数风控回调执行结果：处理超时，重新提交");
                    //重新提交请求
                    //string SchemaCode = wf.getSchemaCodeByInstanceID(entity.reqID);
                    //wf.postHttp(SchemaCode, entity.reqID);
                }
                else if (entity.code == "04")//超时转人工
                {
                    OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("融数风控回调执行结果：处理超时转人工");
                    //localreject: 部署在东正本地规则拒绝；
                    //cloudaccept: 部署在云端规则通过；
                    //cloudreject: 部署在云端规则拒绝；
                    //cloudmanual: 部署在云端规则返回转人工
                    fkResult = entity.data.action;
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "fkResult",
                        ItemValue = "overtime"
                    });
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 0
                    });
                    string SchemaCode = wf.getSchemaCodeByInstanceID(entity.reqID);
                    wf.SetItemValues(SchemaCode, wf.getBizobjectByInstanceid(entity.reqID), keyValues, wf.GetUserIDByCode("rsfk"));
                    result = wf.SubmitWorkItemByRongShu(entity.reqID, "", wf.GetUserIDByCode("rsfk"));
                }

            }
            else
            {
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("融数风控回调执行结果：回传参数为空！");
            }
        }
    }

}


