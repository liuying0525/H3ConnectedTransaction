using DongZheng.H3.WebApi.Models.ApiResponse;
using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.MySetting;
using Newtonsoft.Json;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Mvc;
using System.Linq;
using OThinker.H3.Instance;

namespace DongZheng.H3.WebApi.Controllers.Che300
{
    [Xss]
    public class ModelQueryController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public const string QUERY_ID = "queryid";

        [HttpPost]
        public JsonResult QueryModelInfo(string InstanceId)
        {
            #region 校验参数
            if (string.IsNullOrEmpty(InstanceId))
            {
                ResponseData<string> rspData = new ResponseData<string>();
                rspData.Code = "-1";
                rspData.Message = "InstanceId为空";
                rspData.Success = false;
                return Json(rspData);
            }
            InstanceContext context = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            if (context == null)
            {
                ResponseData<string> rspData = new ResponseData<string>();
                rspData.Code = "-1";
                rspData.Message = "InstanceId错误";
                rspData.Success = false;
                return Json(rspData);
            }
            if (context.BizObjectSchemaCode != "APPLICATION")
            {
                ResponseData<string> rspData = new ResponseData<string>();
                rspData.Code = "-1";
                rspData.Message = "非APPLICATION流程";
                rspData.Success = false;
                return Json(rspData);
            }

            #endregion

            #region 查询Vin码
            string sql_getvin = $"select VIN_NUMBER from i_application where objectid='{context.BizObjectId}'";
            string Vin = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_getvin) + string.Empty;
            if (string.IsNullOrEmpty(Vin))
            {
                ResponseData<string> rspData = new ResponseData<string>();
                rspData.Code = "-1";
                rspData.Message = "Vin为空";
                rspData.Success = false;
                return Json(rspData);
            }
            #endregion

            Vin = Vin.ToUpper();
            int validDays = 30;
            int.TryParse(ConfigurationManager.AppSettings["Che300_ModelValidDays"] + string.Empty, out validDays);

            var queryid = WorkflowSetting.GetCustomSettingValue(InstanceId, QUERY_ID);

            //是否有过查询记录；
            if (string.IsNullOrEmpty(queryid))
            {
                if (context.State == InstanceState.Finished)
                {
                    ResponseData<List<Model2>> rspData = new ResponseData<List<Model2>>();
                    rspData.Code = "1";
                    rspData.Message = "ok";
                    rspData.Success = true;
                    rspData.Data = new List<Model2>();
                    return Json(rspData);
                }
                //历史是否有过查询过的记录；
                string sql = $"select * from c_che300_model_by_vin where vinnumber='{Vin}' and querytime >to_date('{DateTime.Now.AddDays(-validDays).ToString()}','yyyy-MM-dd HH24:mi:ss') order by querytime desc";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                //有记录
                if (dt != null && dt.Rows.Count > 0)
                {
                    queryid = dt.Rows[0]["queryid"] + string.Empty;
                    List<Model2> models = GetModelInfoByQueryId(queryid);

                    ResponseData<List<Model2>> rspData = new ResponseData<List<Model2>>();
                    rspData.Code = "1";
                    rspData.Message = "ok";
                    rspData.Success = true;
                    rspData.Data = models;
                    return Json(rspData);
                }
                else//没有记录
                {
                    string methodUrl = ConfigurationManager.AppSettings["Che300_IdentifyModelByVIN"] + string.Empty;
                    string token = ConfigurationManager.AppSettings["Che300_API_Token"] + string.Empty;
                    var result = HttpHelper.GetWebRequest($"{methodUrl}?vin={Vin}&token={token}");
                    if (string.IsNullOrEmpty(result))
                    {
                        ResponseData<string> rspData = new ResponseData<string>();
                        rspData.Code = "-1";
                        rspData.Message = "查询超时，请刷新页面重试";
                        rspData.Success = false;
                        return Json(rspData);
                    }
                    try
                    {
                        IdentifyModelByVINResult identifyModelByVINResult = JsonConvert.DeserializeObject<IdentifyModelByVINResult>(result);
                        if (identifyModelByVINResult.status == "1")
                        {
                            #region Save Data to DB
                            queryid = Guid.NewGuid().ToString();

                            var modelInfos = identifyModelByVINResult.modelInfo.OrderByDescending(a => a.model_price).ToList();

                            modelInfos.ForEach(a =>
                            {
                                a.model_price = a.model_price * 10000;
                            });
                            SaveModelToDB(Vin, queryid, modelInfos);
                            WorkflowSetting.SetCustomSettingValue(InstanceId, "APPLICATION", QUERY_ID, queryid);
                            #endregion

                            ResponseData<List<Model2>> rspData = new ResponseData<List<Model2>>();
                            rspData.Code = "1";
                            rspData.Message = "ok";
                            rspData.Success = true;
                            rspData.Data = modelInfos;
                            return Json(rspData);
                        }
                        else
                        {
                            ResponseData<string> rspData = new ResponseData<string>();
                            rspData.Code = "-1";
                            rspData.Message = $"[ErrorCode:{identifyModelByVINResult.status}] " + identifyModelByVINResult.error_msg;
                            rspData.Success = false;
                            return Json(rspData);
                        }
                    }
                    catch (Exception ex)
                    {
                        ResponseData<string> rspData = new ResponseData<string>();
                        rspData.Code = "-1";
                        rspData.Message = ex.Message;
                        rspData.Success = false;
                        return Json(rspData);
                    }
                }
            }
            else//有查询记录,直接返回查询记录;
            {
                ResponseData<List<Model2>> rspData = new ResponseData<List<Model2>>();
                rspData.Code = "1";
                rspData.Message = "ok";
                rspData.Success = true;
                rspData.Data = GetModelInfoByQueryId(queryid);
                return Json(rspData);
            }
        }


        #region Methon
        protected List<Model2> GetModelInfoByQueryId(string QueryId)
        {
            string sql = $"select * from c_che300_model_by_vin where queryid='{QueryId}'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                List<Model2> models = Converter.ToList<Model2>(dt);
                return models.OrderByDescending(a => a.model_price).ToList();
            }
            else
            {
                return null;
            }
        }

        protected bool SaveModelToDB(string Vin, string QueryId, List<Model2> models)
        {
            foreach (var model in models)
            {
                SaveModelToDB(Vin, QueryId, model);
            }
            return true;
        }

        protected bool SaveModelToDB(string Vin, string QueryId, Model2 model)
        {
            string sql_insert = $"insert into c_che300_model_by_vin(objectid,queryid,vinnumber,querytime,brand_id,brand_name,series_group_name,series_id,series_name,model_id,model_name, model_price, model_year, min_reg_year, max_reg_year,model_gear,model_emission_standard,model_liter,color,ext_model_id)values('{Guid.NewGuid().ToString()}','{QueryId}', '{Vin}', to_date('{DateTime.Now.ToString()}', 'yyyy-MM-dd HH24:mi:ss'), {model.brand_id}, '{model.brand_name}','{model.series_group_name}',{model.series_id},'{model.series_name}',{model.model_id},'{model.model_name}',{model.model_price},{model.model_year},{model.min_reg_year},{model.max_reg_year},'{model.model_gear}','{model.model_emission_standard}','{model.model_liter}','{model.color}',{model.ext_model_id})";
            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_insert) > 0;
        }
        #endregion
    }
}