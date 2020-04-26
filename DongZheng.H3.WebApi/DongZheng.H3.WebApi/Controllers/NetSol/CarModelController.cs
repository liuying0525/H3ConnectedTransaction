using DongZheng.H3.WebApi.Models.ApiResponse;
using Newtonsoft.Json;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.NetSol
{
    [ValidateInput(false)]
    [Xss]
    public class CarModelController :CustomController
    {
        public const string che300_fromVersion = "Che300_FromVersion";

        public static string che300_token = System.Configuration.ConfigurationManager.AppSettings["Che300_API_Token"] + string.Empty;
        public static string che300_api_url = System.Configuration.ConfigurationManager.AppSettings["Che300_API_URL"] + string.Empty;
        public static string che300_getmodelinfo_url = System.Configuration.ConfigurationManager.AppSettings["Che300_GetModelInfo"] + string.Empty;
        public static string che300_getmodelparas = System.Configuration.ConfigurationManager.AppSettings["Che300_GetModelParas"] + string.Empty;

        public static string getCarBrandList = System.Configuration.ConfigurationManager.AppSettings["Che300_GetCarBrandList"] + string.Empty;
        public static string getCarSeriesList = System.Configuration.ConfigurationManager.AppSettings["Che300_GetCarSeriesList"] + string.Empty;
        public static string getCarModelList = System.Configuration.ConfigurationManager.AppSettings["Che300_GetCarModelList"] + string.Empty;
        //空版本号
        public static string emptyVersionID = "";
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

        // GET: CarModel
        public string Index()
        {
            return "CarModel Controller";
        }

        //增量同步，同步过一次之后，不会再进行同步
        public JsonResult CarModelSync(string usercode, string username)
        {
            string fromVersion = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetCustomSetting(che300_fromVersion);
            string msg = "";
            bool success = CarModelSync_(usercode, username, fromVersion, true, ref msg);
            return Json(new { Success = success, Msg = msg }, JsonRequestBehavior.AllowGet);
        }
        //指定che300的版本号进行同步
        public JsonResult CarModelSyncByVersion(string usercode, string username, string version)
        {
            string msg = "";
            bool success = CarModelSync_(usercode, username, version, false, ref msg);
            return Json(new { Success = success, Msg = msg }, JsonRequestBehavior.AllowGet);
        }
        //获取同步结果
        public JsonResult GetChe300SyncResult(PagerInfo pagerInfo)
        {
            string sql_num = "select count(1) from Che300_SyncResult";
            string number = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_num) + string.Empty;
            string sql_data = @"select * from (select ROW_NUMBER() OVER ( order by m.sync_date desc ) AS RowNumber_,m.* from Che300_SyncResult m ) a where RowNumber_ between {0} and {1}";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(sql_data, pagerInfo.StartIndex, pagerInfo.EndIndex));
            List<object> result = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                object o = new
                {
                    current_version = row["current_version"] + string.Empty,
                    latest_version = row["latest_version"] + string.Empty,
                    sync_date = row["sync_date"] + string.Empty,
                    sync_user_name = row["sync_user_name"] + string.Empty,
                    sync_user_id = row["sync_user_id"] + string.Empty,
                    sync_result = row["sync_result"] + string.Empty,
                    sync_result_code = row["sync_result_code"] + string.Empty,
                    rent_sync_result_code = row["rent_sync_result_code"] + string.Empty,
                    che300_data_code = row["che300_data_code"] + string.Empty
                };
                result.Add(o);
            }
            return Json(new { Success = true, Rows = result, Total = number, iTotalDisplayRecords = number, iTotalRecords = number, sEcho = pagerInfo.sEcho }, JsonRequestBehavior.AllowGet);
        }
        //获取需要人工调整的车型
        public JsonResult GetModel_BrandOrSeries_Change(PagerInfo pagerInfo)
        {
            List<Model> models = new List<Model>();
            string sql_num = "select count(1) from t_model_upd";
            string number = ExecuteScalar(sql_num) + string.Empty;
            string sql_data = @"select * from (select ROW_NUMBER() OVER ( order by createdtime desc, series_group_name,series_name,model_name ) AS RowNumber_,m.* from t_model_upd m) a where RowNumber_ between {0} and {1}";
            DataTable dt = ExecuteDataTable(string.Format(sql_data, pagerInfo.StartIndex, pagerInfo.EndIndex));
            foreach (DataRow row in dt.Rows)
            {
                Model m = new Model();
                m.body_type = row["body_type"] + string.Empty;
                m.brand_id = row["brand_id"] + string.Empty;
                m.brand_name = row["brand_name"] + string.Empty;
                m.car_struct = row["car_struct"] + string.Empty;
                m.discharge_standard = row["discharge_standard"] + string.Empty;
                m.door_number = row["door_number"] + string.Empty;
                m.drive_name = row["drive_type"] + string.Empty;//注意
                m.engine_power = row["engine_power"] + string.Empty;
                //m.gear_name = row["gear_name"] + string.Empty;没有这个字段
                m.gear_type = row["gear_type"] + string.Empty;
                m.is_green = row["is_green"] + string.Empty;
                m.is_parallel = row["is_parallel"] + string.Empty;
                m.liter = row["liter"] + string.Empty;
                //m.maker_type = row[""] + string.Empty;没有这个字段
                m.market_date = row["market_date"] + string.Empty;
                //m.max_reg_year = row[""] + string.Empty;
                //m.min_reg_year = row[""] + string.Empty;
                m.model_id = row["model_id"] + string.Empty;
                m.model_name = row["model_name"] + string.Empty;
                m.model_year = row["year"] + string.Empty;//注意
                //m.oper_type = row[""] + string.Empty;
                m.price = row["price"] + string.Empty;
                m.seat_number = row["seat_number"] + string.Empty;
                m.series_group_name = row["series_group_name"] + string.Empty;
                m.series_id = row["series_id"] + string.Empty;
                m.series_name = row["series_name"] + string.Empty;
                m.version_id = row["version_id"] + string.Empty;
                m.version_time = row["version_time"] + string.Empty;
                //以下字段为标识,记录字段
                m.upd_flag = row["upd_flag"] + string.Empty;
                m.upd_comment = row["upd_comment"] + string.Empty;

                m.upd_type = row["upd_type"] + string.Empty;
                m.upd_time = row["upd_time"] + string.Empty;
                m.upd_result = row["upd_result"] + string.Empty;
                m.upd_usercode = row["upd_usercode"] + string.Empty;
                m.upd_username = row["upd_username"] + string.Empty;
                models.Add(m);
            }
            return Json(new { Success = true, Rows = models, Total = number, iTotalDisplayRecords = number, iTotalRecords = number, sEcho = pagerInfo.sEcho }, JsonRequestBehavior.AllowGet);
        }





        //获取品牌列表
        public JsonResult GetBrandList()
        {
            string url = getCarBrandList + "?token=" + che300_token;
            string result_str = HttpHelper.get(url);
            List<Brand> brands = JsonConvert.DeserializeObject<List<Brand>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_str)["brand_list"].ToString()); //所有Brand
            if (brands == null || brands.Count() == 0)
            {
                return Json(new { Success = false, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Data = brands }, JsonRequestBehavior.AllowGet);
        }
        //获取车系列表
        public JsonResult GetSeriesList(string brand_id)
        {
            string getSeriesListUrl = getCarSeriesList + "?token=" + che300_token + "&brandId=" + brand_id;
            string result_series = HttpHelper.get(getSeriesListUrl);
            List<Series> series = JsonConvert.DeserializeObject<List<Series>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_series)["series_list"].ToString());//所有车系
            if (series == null || series.Count() == 0)
            {
                return Json(new { Success = false, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Data = series }, JsonRequestBehavior.AllowGet);
        }

        //获取车型列表
        public JsonResult GetModelList(string series_id)
        {
            string getCarModelListUrl = getCarModelList + "?token=" + che300_token + "&seriesId=" + series_id;
            string result_models = HttpHelper.get(getCarModelListUrl);
            List<Model> models = JsonConvert.DeserializeObject<List<Model>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_models)["model_list"].ToString());
            if (models == null || models.Count() == 0)
            {
                return Json(new { Success = false, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Data = models }, JsonRequestBehavior.AllowGet);
        }

        //车型批量同步
        public JsonResult SyncModelBatch(List<string> model_ids)
        {
            //检查Upd表中的操作是否都完成
            string sql_check = "select count(1) from t_Model_Upd where upd_type is null or upd_type=2";
            int num_upd = Convert.ToInt32(ExecuteScalar(sql_check) + string.Empty);
            if (num_upd > 0)
            {
                var errmsg = "还有需要人工处理的车型未处理，请先处理完再进行同步";
                return Json(new { Success = false, Data = "", Msg = errmsg }, JsonRequestBehavior.AllowGet);
            }
            List<object> result = new List<object>();
            foreach (var model_id in model_ids)
            {
                var model = GetModelInfo(model_id);
                var old_model = GetT_ModelInfo(model_id);
                string ope_state = "0";//车型数据操作是否成功:0失败，1成功，2人工
                bool cms_state = false;
                string cms_msg = "";
                bool rent_state = false;
                string rent_msg = "";
                if (old_model == null)//没有取到数据
                {
                    if (AddModel(model, DateTime.Now, emptyVersionID) > 0)
                        ope_state = "1";
                }
                else
                {
                    UpdResult upd = GetUpdFlag(model, old_model.series_group_id, old_model.series_group_name, old_model.series_id, old_model.series_name, old_model.model_name);

                    if (upd.UpdFlag == 999)//没有修改车系组，车系,车型,直接Update t_model
                    {
                        if (UpdateCarModel(model, old_model.price) > 0)
                        {
                            ope_state = "1";
                        }
                    }
                    else
                    {
                        if (AddModelUpd(model, DateTime.Now, emptyVersionID, upd.UpdFlag.ToString(), upd.UpdMessage) > 0)
                        {
                            ope_state = "2";
                        }
                    }
                }
                if (ope_state == "1")
                {
                    var cms_result = SyncOneModel(model_id);
                    var rent_result = new UpdResult();
                    //平行进口的车型不同步到直租
                    if (model.is_parallel == "1")
                    {
                        //同步之前也是平行进口
                        if (old_model.is_parallel == "1")
                        {
                            rent_result.UpdFlag = 1;
                            rent_result.UpdMessage = "平行进口车型,不同步到直租";
                        }
                        else//同步之前是非平行进口,需要下架;
                        {
                            //直租的车型下架
                            if (rent_asset_del("3", model.model_id))
                            {
                                rent_result.UpdFlag = 1;
                                rent_result.UpdMessage = "下架成功[由非平行进口调整为平行进口车型]";
                            }
                            else
                            {
                                rent_result.UpdFlag = -5000;
                                rent_result.UpdMessage = "下架失败[由非平行进口调整为平行进口车型]";
                            }
                        }
                    }
                    else
                    {
                        rent_result = SyncOneModel_Rent(model_id);
                        //品牌或者车系没有,重新获取品牌和车系,写入到t_brand及t_series表,然后再重新调用一次
                        if (rent_result.UpdFlag == -199)
                        {
                            string url = getCarBrandList + "?token=" + che300_token;
                            string result_str = HttpHelper.get(url);
                            List<Brand> brands = CarConvert.ConvertToBrand(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_str)["brand_list"].ToString()); //所有Brand

                            string getSeriesListUrl = getCarSeriesList + "?token=" + che300_token + "&brandId=" + model.brand_id;
                            string result_series = HttpHelper.get(getSeriesListUrl);

                            Brand brand = brands.FirstOrDefault(t => t.brand_id == model.brand_id);
                            List<Series> series = CarConvert.ConvertToSeries(brand, JsonConvert.DeserializeObject<Dictionary<string, object>>(result_series)["series_list"].ToString());//所有车系
                            Series s = series.FirstOrDefault(t => t.series_id == model.series_id);

                            AddBrand(brand, DateTime.Now, "");
                            AddSeries(s, DateTime.Now, "");
                            rent_result = SyncOneModel_Rent(model_id);
                        }
                    }
                    cms_state = (cms_result.UpdFlag == 1 || cms_result.UpdFlag == -20011);
                    cms_msg = cms_result.UpdMessage;
                    rent_state = rent_result.UpdFlag == 1;
                    rent_msg = rent_result.UpdMessage;
                }

                result.Add(new
                {
                    Model_Id = model_id,
                    Ope_State = ope_state,
                    CMS_State = cms_state,
                    CMS_Msg = cms_msg,
                    Rent_State = rent_state,
                    Rent_Msg = rent_msg
                });
            }
            return Json(new { Success = true, Data = result, Msg = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Test1(string id)
        {
            var txt = GetModelInfo(id);
            return Json(txt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Test2(string id)
        {
            var txt = GetT_ModelInfo(id);
            return Json(txt, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Test3(string id)
        {
            var txt = GetT_Model_UpdInfo(id);
            return Json(txt, JsonRequestBehavior.AllowGet);
        }

        #region 数据初始化
        public JsonResult SyncAllChe300Data_Clear()
        {
            List<object> res = new List<object>();
            string url = getCarBrandList + "?token=" + che300_token;
            AppUtility.Engine.LogWriter.Write("Che300 api url:" + url);
            string result_str = HttpHelper.get(url);
            //记录车300接口返回结果：
            AppUtility.Engine.LogWriter.Write("Che300接口返回结果(GetBrandList)：" + result_str);
            List<Brand> brands =CarConvert.ConvertToBrand(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_str)["brand_list"].ToString()); //所有Brand
            if (brands == null || brands.Count() == 0)
            {
                return Json("GetBrandList结果为空", JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var brand in brands)
                {
                    string getSeriesListUrl = getCarSeriesList + "?token=" + che300_token + "&brandId=" + brand.brand_id;
                    AppUtility.Engine.LogWriter.Write("Che300 api url:" + getSeriesListUrl);
                    string result_series = HttpHelper.get(getSeriesListUrl);
                    //记录车300接口返回结果：
                    AppUtility.Engine.LogWriter.Write("Che300接口返回结果(getCarSeriesList)：" + result_series);
                    List<Series> series =CarConvert.ConvertToSeries(brand,JsonConvert.DeserializeObject<Dictionary<string, object>>(result_series)["series_list"].ToString());//所有车系
                    AddBrand(brand, DateTime.Now, emptyVersionID);
                    foreach (var s in series)
                    {
                        string getCarModelListUrl = getCarModelList + "?token=" + che300_token + "&seriesId=" + s.series_id;
                        AppUtility.Engine.LogWriter.Write("Che300 api url:" + getCarModelListUrl);
                        string result_models = HttpHelper.get(getCarModelListUrl);
                        //记录车300接口返回结果：
                        AppUtility.Engine.LogWriter.Write("Che300接口返回结果(getCarModelLis)：" + result_models);
                        List<Model> models = JsonConvert.DeserializeObject<List<Model>>(JsonConvert.DeserializeObject<Dictionary<string, object>>(result_models)["model_list"].ToString());
                        AddSeries(s, DateTime.Now, emptyVersionID);
                        foreach (var model_new in models)
                        {
                            Model model = GetModelInfo(model_new.model_id);
                            #region 新增
                            AddModel(model, DateTime.Now, emptyVersionID);
                            #endregion
                            res.Add(new { Model_ID = model.model_id, Model_Name = model.model_name });
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            AppUtility.Engine.LogWriter.Write("同步所有数据的结果" + Newtonsoft.Json.JsonConvert.SerializeObject(res));
            return Json(new { Success = true, Number = res.Count() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncModelParameters()
        {
            int num = 0;
            //string sql = "select * from T_MODEL_ID_new";            
            //DataTable dt = ExecuteDataTable(sql);
            //foreach (DataRow row in dt.Rows)
            //{
            //    if (Add_Model_Parameters(row["model_id"] + string.Empty) > 0)
            //    {
            //        //删除；
            //        ExecuteNonQuery("delete from T_MODEL_ID_new where model_id='" + row["model_id"] + string.Empty + "'");
            //        num++;
            //    }
            //}
            return Json(new { Success = true, Number = num }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncModelParameters_One(string model_id)
        {
            int num = 0;
            num = Add_Model_Parameters(model_id);
            return Json(new { Success = true, Number = num }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        //点击操作按钮时执行的事件;
        public JsonResult UpdateUpdState(string type, string model_id, string upd_flag, string usercode, string username)
        {
            var success = false;
            var msg = "";
            //3:不做处理
            #region 不做处理
            if (type == "ignore")
            {
                //更新状态及结果
                success = SyncCancle(model_id, usercode, username);
                if (!success)
                {
                    msg = "处理失败";
                }
            }
            #endregion

            #region 删除:-1
            else if (upd_flag == "-1")
            {
                //1.check是否下架
                //2.删除t_model表
                //3.更新状态及结果
                string model_name = "";// GetT_ModelInfo(model_id).model_name;
                //if (CheckIsActivate(model_name))//未下架
                //{
                //    success = false;
                //    msg = "车型[" + model_name + "]未下架";
                //}
                //else
                string sql = "select model_name from t_model_upd where  upd_flag=-1 and model_id='{0}'";
                model_name = ExecuteScalar(string.Format(sql, model_id)) + string.Empty;
                if (SetModelInactivate(model_name))
                {
                    if (rent_asset_del("3", model_id))
                    {
                        string sql_Del = @"delete from t_model where model_id='{0}'";
                        sql_Del = string.Format(sql_Del, model_id);
                        ExecuteNonQuery(sql_Del);
                        Upd_T_Model_Upd_State(UpdateType.Success, "已删除", model_id, usercode, username);
                        success = true;
                        msg = "";
                    }
                    else
                    {
                        success = false;
                        msg = "直租车型[" + model_id + "]下架失败，请联系管理员";
                        Upd_T_Model_Upd_State(UpdateType.RentFail, msg, model_id, usercode, username);
                    }
                }
                else
                {
                    success = false;
                    msg = "在CMS中未找到车型[" + model_name + "],下架失败";
                }
            }
            #endregion

            #region 有调整：车系组或者车系有调整，包含1
            else if (upd_flag.IndexOf("1") > -1)
            {
                //1.check是否下架
                //2.同步
                //3.更新状态upd_type及结果upd_result
                var t_model_info = GetT_ModelInfo(model_id);
                string model_name = t_model_info.model_name;



                //if (CheckIsActivate(model_name))//未下架
                //{
                //    success = false;
                //    msg = "车型[" + model_name + "]未下架";
                //}
                //先下架车型
                if (SetModelInactivate(model_name))
                {
                    var cur_model = GetT_Model_UpdInfo(model_id);

                    //判断名称是否有修改*************,有修改直接修改名称(车系组,车系)
                    var upd_result = UpdateNameWhenAdjust(cur_model);
                    if (upd_result.UpdFlag != 1)
                    {
                        success = false;
                        msg = upd_result.UpdMessage;
                        return Json(new { Success = success, Msg = msg }, JsonRequestBehavior.AllowGet);
                    }
                    //1.根据t_model_upd表中的数据更新T_model表
                    if (UpdateCarModel(cur_model,t_model_info.price) > 0)
                    {
                        //2.调用单个车型同步的接口
                        var syncresult = SyncOneModel(model_id);
                        if (syncresult.UpdFlag == 1)
                        {
                            //直租不同步平行进口
                            if (cur_model.is_parallel != "1")
                            {
                                #region 直租系统同步结果；
                                var syncresult_rent = SyncOneModel_Rent(model_id);
                                if (syncresult_rent.UpdFlag == 1)
                                {
                                    //3.更新状态
                                    Upd_T_Model_Upd_State(UpdateType.Success, "同步成功", model_id, usercode, username);
                                    success = true;
                                    msg = "同步成功";
                                }
                                else
                                {
                                    msg = "直租车型[" + model_id + "]更新失败，请联系管理员";
                                    success = false;
                                    Upd_T_Model_Upd_State(UpdateType.RentFail, msg, model_id, usercode, username);
                                }
                                #endregion
                            }
                            else
                            {
                                #region 1.平行进口车型不同步到直租 2.非平行进口调整为平行进口需要下架
                                if (t_model_info.is_parallel == "1")
                                {
                                    //3.更新状态
                                    Upd_T_Model_Upd_State(UpdateType.Success, "同步成功", model_id, usercode, username);
                                    success = true;
                                    msg = "平行进口车型不同步到直租";
                                }
                                else
                                {
                                    //直租的车型下架
                                    if (rent_asset_del("3", cur_model.model_id))
                                    {
                                        Upd_T_Model_Upd_State(UpdateType.Success, "同步成功", model_id, usercode, username);
                                        success = true;
                                        msg = "下架成功[由非平行进口调整为平行进口车型]";
                                    }
                                    else
                                    {
                                        success = false;
                                        msg = "下架失败[由非平行进口调整为平行进口车型]";
                                        Upd_T_Model_Upd_State(UpdateType.RentFail, "平行进口车型下架失败", model_id, usercode, username);
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            success = false;
                            msg = syncresult.UpdFlag + ":" + syncresult.UpdMessage;
                        }
                    }
                    else
                    {
                        success = false;
                        msg = "更新T_Model表中记录失败";
                    }
                }
                else
                {
                    success = false;
                    msg = "在CMS中未找到车型[" + model_name + "],下架失败";
                }
            }
            #endregion

            #region 修改名称
            else if (upd_flag.IndexOf("1") == -1 && upd_flag.IndexOf("2") > -1)
            {
                //1.修改名称
                //2.更新状态及结果
                var result = SyncName(model_id, upd_flag, usercode, username);
                if (result.UpdFlag == 1)
                {
                    var cur_model = GetT_Model_UpdInfo(model_id);
                    //平行进口,直租不同步
                    if (cur_model.is_parallel != "1")
                    {
                        var result_rent = SyncOneModel_Rent(model_id);
                        if (result_rent.UpdFlag == 1)
                        {
                            success = true;
                            msg = result.UpdMessage;
                            Upd_T_Model_Upd_State(UpdateType.Success, "修改名称成功", model_id, usercode, username);
                        }
                        else
                        {
                            success = false;
                            msg = "直租车型[" + model_id + "]更新失败，请联系管理员";
                            Upd_T_Model_Upd_State(UpdateType.RentFail, msg, model_id, usercode, username);
                        }
                    }
                    else
                    {
                        success = true;
                        msg = result.UpdMessage;
                        Upd_T_Model_Upd_State(UpdateType.Success, "修改名称成功", model_id, usercode, username);
                    }
                }
            }
            #endregion

            return Json(new { Success = success, Msg = msg }, JsonRequestBehavior.AllowGet);
        }

        //获取Che300的接口返回的数据
        public JsonResult GetChe300SyncData(string id)
        {
            string r = GetChe300Data(id);
            return Json(r, JsonRequestBehavior.AllowGet);
        }
        //获取本次同步结果的详细记录
        public JsonResult GetSyncDetail(string seq_id)
        {
            List<object> objs = new List<object>();
            string sql = "select * from asset_info_his where imp_seq=" + seq_id + " order by imp_time";
            DataTable dt = ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                objs.Add(new
                {
                    ASSET_MODEL_DSC = row["ASSET_MODEL_DSC"] + string.Empty,
                    MIOCN = row["MIOCN"] + string.Empty,
                    IMP_TIME = row["IMP_TIME"] + string.Empty
                });
            }
            return Json(new { Success = true, Data = objs }, JsonRequestBehavior.AllowGet);
        }

        //获取本次同步结果的详细记录
        public JsonResult GetSyncDetail_Rent(string seq_id)
        {
            List<object> objs = new List<object>();
            string sql = "select * from V_QUERY_RENT_LOG where apch=" + seq_id + " order by tzxsj";
            DataTable dt = ExecuteDataTable(sql);
            if(dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    objs.Add(new
                    {
                        Status = row["ACWXX"] + string.Empty,
                        Brand_Id = row["Brand_Id"] + string.Empty,
                        Brand_Name = row["Brand_Name"] + string.Empty,
                        Series_Id = row["Series_Id"] + string.Empty,
                        Series_Name = row["Series_Name"] + string.Empty,
                        Model_Id = row["Model_Id"] + string.Empty,
                        Model_Name = row["Model_Name"] + string.Empty,
                        SyncTime = Convert.ToDateTime(row["tzxsj"] + string.Empty).ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
           
            return Json(new { Success = true, Data = objs }, JsonRequestBehavior.AllowGet);
        }

        #region 基础方法
        /// <summary>
        /// 车型同步的方法
        /// </summary>
        /// <param name="usercode">操作人，日志用</param>
        /// <param name="username">操作人的名称，日志用</param>
        /// <param name="fromVersion">同步时的版本号</param>
        /// <param name="needDefaultVersion">是否需要使用默认版本号（配置文件中）</param>
        /// <param name="errmsg">错误信息</param>
        /// <returns></returns>
        public bool CarModelSync_(string usercode, string username, string fromVersion, bool needDefaultVersion, ref string errmsg)
        {
            //检查Upd表中的操作是否都完成
            string sql_check = "select count(1) from t_Model_Upd where upd_type is null or upd_type=2";
            int num_upd = Convert.ToInt32(ExecuteScalar(sql_check) + string.Empty);
            if (num_upd > 0)
            {
                errmsg = "还有需要人工处理的车型未处理，请先处理完再进行同步";
                return false;
            }

            AppUtility.Engine.LogWriter.Write("车型同步：UserCode-->" + usercode + "，UserName-->" + username + "，fromVersion-->" + fromVersion + "，needDefaultVersion-->" + needDefaultVersion);

            if (string.IsNullOrEmpty(fromVersion) && needDefaultVersion)
            {
                string defaultVersion = System.Configuration.ConfigurationManager.AppSettings["Che300_API_FromVersion"] + string.Empty;
                AppUtility.Engine.LogWriter.Write("车型同步：使用默认版本号-->" + defaultVersion);
                fromVersion = defaultVersion;
            }

            if (string.IsNullOrEmpty(fromVersion))
            {
                errmsg = "车300接口参数fromVersion为空";
                return false;
            }

            string url = che300_api_url + "&token=" + che300_token + "&fromVersion=" + fromVersion;
            AppUtility.Engine.LogWriter.Write("Che300 api url:" + url);
            string result_str = HttpHelper.get(url);
            //记录车300接口返回结果：
            AppUtility.Engine.LogWriter.Write("Che300接口返回结果：" + result_str);
            che300_result result = JsonConvert.DeserializeObject<che300_result>(result_str);
            AppUtility.Engine.LogWriter.Write("Che300返回结果反序列化结束...");
            if (result.status == 0)
            {
                errmsg = result.error_msg;
                return false;
            }
            result.data.id = Guid.NewGuid().ToString();
            SaveChe300Data(result.data.id, result_str);
            var v = ImportModel(result, usercode, username);
            errmsg = v.ToString();
            return true;
        }
        /// <summary>
        /// 导入数据到中间库
        /// </summary>
        /// <param name="result">结果集</param>
        /// <param name="usercode">用户</param>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public ImportResult ImportModel(che300_result result, string usercode, string username)
        {
            AppUtility.Engine.LogWriter.Write("开始导入Che300数据..." + result.data.id);
            var brand_I = result.data.brand.Where(b => b.oper_type == "新增").ToList().Count();
            var brank_U = result.data.brand.Where(b => b.oper_type == "更新").ToList().Count();
            var brank_D = result.data.brand.Where(b => b.oper_type == "删除").ToList().Count();

            var series_I = result.data.series.Where(s => s.oper_type == "新增").ToList().Count();
            var series_U = result.data.series.Where(s => s.oper_type == "更新").ToList().Count();
            var series_D = result.data.series.Where(s => s.oper_type == "删除").ToList().Count();

            var model_I = result.data.model.Where(m => m.oper_type == "新增").ToList().Count();
            var model_U = result.data.model.Where(m => m.oper_type == "更新").ToList().Count();
            var model_D = result.data.model.Where(m => m.oper_type == "删除").ToList().Count();

            string apiresult = "数据解析：品牌-->新增" + brand_I + ",更新" + brank_U + ",删除" + brank_D +
                "；车系-->新增" + series_I + ",更新" + series_U + ",删除" + series_D +
                "；车型-->新增" + model_I + ",更新" + model_U + ",删除" + model_D;
            AppUtility.Engine.LogWriter.Write(apiresult);
            ImportResult ir = new ImportResult();
            var V = result.data.version;
            SyncVersion version = V[0];

            //gear_type字段对应gear_name这个字段
            //drive_type字段对应drive_name这个字段
            AppUtility.Engine.LogWriter.Write("品牌导入...");
            #region 1.品牌导入
            foreach (var v in result.data.brand)
            {
                if (v.oper_type == "新增")
                {
                    ir.Brand_Insert_Num += AddBrand(v, version.BrandTimestamp, version.InitialVersion);
                }
                else if (v.oper_type == "更新")
                {
                    ir.Brand_Update_Num += UpdateBrand(v, version.BrandTimestamp, version.InitialVersion);
                }
                else if (v.oper_type == "删除")
                {
                    ir.Brand_Delete_Num += DeleteBrand(v);
                }
            }
            #endregion
            AppUtility.Engine.LogWriter.Write("车系导入...");
            #region 2.车系导入
            foreach (var v in result.data.series)
            {
                if (v.oper_type == "新增")
                {
                    ir.Series_Insert_Num += AddSeries(v, version.SeriesTimestamp, version.InitialVersion);
                }
                else if (v.oper_type == "更新")
                {
                    var n = UpdateSeries(v, version.SeriesTimestamp, version.InitialVersion);
                    if (n == 0)
                    {
                        ir.Series_Insert_Num += AddSeries(v, version.SeriesTimestamp, version.InitialVersion);
                    }
                    else
                    {
                        ir.Series_Update_Num += n;
                    }
                }
                else if (v.oper_type == "删除")
                {
                    ir.Series_Delete_Num += DeleteSeries(v);
                }
            }
            #endregion
            AppUtility.Engine.LogWriter.Write("车型导入...");
            #region 3.车型导入
            foreach (var v in result.data.model)
            {
                v.version_id = version.InitialVersion;
                v.version_time = version.ModelTimestamp.ToString("yyyy-MM-dd HH:mm:ss");
                if (v.is_parallel == "1")
                {
                    v.series_group_name += "(平行进口)";
                    v.series_group_id += "A";
                }
                #region 新增
                if (v.oper_type == "新增")
                {
                    ir.Model_Insert_Num += AddModel(v, version.ModelTimestamp, version.InitialVersion);
                    //添加参数；
                    Add_Model_Parameters(v.model_id);
                }
                #endregion

                #region 更新
                else if (v.oper_type == "更新")
                {
                    var old_model = GetT_ModelInfo(v.model_id);
                    if (old_model == null )//没有取到数据
                    {
                        #region 新增
                        ir.Model_Insert_Num += AddModel(v, version.ModelTimestamp, version.InitialVersion);
                        #endregion
                    }
                    else
                    {
                        UpdResult upd = GetUpdFlag(v, old_model.series_group_id, old_model.series_group_name, old_model.series_id, old_model.series_name, old_model.model_name);

                        if (upd.UpdFlag == 999)//没有修改车系组，车系,车型,直接Update t_model
                        {
                            #region 修改
                            if (UpdateCarModel(v, old_model.price) > 0)
                            {
                                ir.Model_Update_Num++;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 只插入到t_model_upd表，不做同步,后通过界面通过;
                            ir.Brand_Series_Update_Num += AddModelUpd(v, version.ModelTimestamp, version.InitialVersion, upd.UpdFlag.ToString(), upd.UpdMessage);
                            #endregion
                        }
                    }
                }
                #endregion

                #region 删除
                else if (v.oper_type == "删除")
                {
                    ir.Brand_Series_Update_Num += DeleteModel(v,version.ModelTimestamp,version.InitialVersion);
                }
                #endregion
            }
            #endregion
            AppUtility.Engine.LogWriter.Write("执行存储过程...");
            #region 4.执行存储过程
            //写到历史表中
            sp_to_history("ADD_ASSET_TO_HIS", version.ModelTimestamp);
            sp_to_history("ADD_ASSET_TO_HIS_RENT", version.ModelTimestamp);
            //写到业务系统中
            var cms_result = sp_to_system("ADD_ASSET_TO_CMS", version.ModelTimestamp);
            var rent_result = sp_to_system("ADD_ASSET_TO_RENT", version.ModelTimestamp);
            #endregion
            AppUtility.Engine.LogWriter.Write("车型导入：写入同步结果...");
            #region 5.写入同步结果
            string sql_Insert_Log = @"insert into Che300_SyncResult (current_version,latest_version,Sync_Date,Sync_User_Name,Sync_User_Id,sync_result,sync_result_code,rent_sync_result_code,che300_data_code) 
values('{0}','{1}',sysdate,'{2}','{3}','{4}','{5}','{6}','{7}')";
            string importmsg = ir.ToString();
            sql_Insert_Log = string.Format(sql_Insert_Log, version.InitialVersion, version.CurrentVersion, username, usercode, importmsg,
                (cms_result.UpdFlag == 1 ? cms_result.UpdMessage : cms_result.UpdFlag.ToString()),
                (rent_result.UpdFlag == 1 ? rent_result.UpdMessage : rent_result.UpdFlag.ToString()),
                result.data.id);
            AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql_Insert_Log);
            #endregion
            AppUtility.Engine.LogWriter.Write("车型导入：保存最新版本号-->" + version.CurrentVersion);
            AppUtility.Engine.SettingManager.SetCustomSetting(che300_fromVersion, version.CurrentVersion);
            AppUtility.Engine.LogWriter.Write("车型导入结束，导入结果：" + ir.ToString());
            return ir;
        }

        /// <summary>
        /// 从t_model表到历史表中（CMS，直租）
        /// </summary>
        /// <param name="sp_name">存储过程名称</param>
        /// <param name="dt_stamp">时间</param>
        public void sp_to_history(string sp_name, DateTime dt_stamp)
        {
            OracleParameter[] para = new OracleParameter[]
            {
                new OracleParameter("p_from_time", OracleType.DateTime, 80),
             new OracleParameter("p_to_time", OracleType.DateTime, 80)
            };

            para[0].Direction = ParameterDirection.Input;
            para[0].Value = dt_stamp.AddDays(-1);
            para[1].Direction = ParameterDirection.Input;
            para[1].Value = dt_stamp.AddDays(+1);
            var result1 = RunProcedure(sp_name, para);
            AppUtility.Engine.LogWriter.Write("执行存储过程" + sp_name + "结束...");
        }

        /// <summary>
        /// 从历史表中到业务系统中（CMS，直租）
        /// </summary>
        /// <param name="sp_name">存储过程名称</param>
        /// <param name="dt_stamp">时间</param>
        /// <returns></returns>
        public UpdResult sp_to_system(string sp_name, DateTime dt_stamp)
        {
            OracleParameter[] para2 = new OracleParameter[]
          {
                new OracleParameter("p_from_time", OracleType.DateTime, 80)
                ,new OracleParameter("p_to_time", OracleType.DateTime, 80)
                ,new OracleParameter("o_result",OracleType.Number)
                ,new OracleParameter("o_serial",OracleType.Number)
          };

            para2[0].Direction = ParameterDirection.Input;
            para2[0].Value = dt_stamp.AddDays(-1);
            para2[1].Direction = ParameterDirection.Input;
            para2[1].Value = dt_stamp.AddDays(+1);
            para2[2].Direction = ParameterDirection.Output;
            para2[3].Direction = ParameterDirection.Output;
            var result2 = RunProcedure(sp_name, para2);
            AppUtility.Engine.LogWriter.Write("执行存储过程" + sp_name + "结束，返回值o_result-->" + result2[2].Value + string.Empty + ",o_serial-->" + result2[3].Value + string.Empty);
            return new UpdResult() { UpdFlag = Convert.ToInt32(result2[2].Value), UpdMessage = result2[3].Value + string.Empty };
        }

        //车型同步不做处理:3
        public bool SyncCancle(string model_id, string usercode, string username)
        {
            return Upd_T_Model_Upd_State(UpdateType.Ignore, "不做处理", model_id, usercode, username);
        }
        //修改名称:12,32,42
        public UpdResult SyncName(string model_id, string upd_flag, string usercode, string username)
        {
            string msg = "";
            string sp_name = "";
            string old_name = "";
            string new_name = "";
            double result = 0;

            Model old_model = GetT_ModelInfo(model_id);
            //获取T_model_upd表中的记录;
            Model cur_model = GetT_Model_UpdInfo(model_id);

            var chars = upd_flag.ToCharArray();
            for (int n = 0; n < chars.Length; n++)
            {
                if (chars[n] == '2')
                {
                    if (n == 0)
                    {
                        sp_name = "rename_asset_make";
                        old_name = old_model.series_group_name;
                        new_name = cur_model.series_group_name;
                        var re_result = Rename(old_model, sp_name, old_name, new_name);
                        if (re_result.UpdFlag != 1)
                        {
                            return re_result;
                        }
                    }
                    else if (n == 1)
                    {
                        sp_name = "rename_asset_brand";
                        old_name = old_model.series_name;
                        new_name = cur_model.series_name;
                        var re_result = Rename(old_model, sp_name, old_name, new_name);
                        if (re_result.UpdFlag != 1)
                        {
                            return re_result;
                        }
                    }
                    else if (n == 2)
                    {
                        sp_name = "rename_asset_model";
                        old_name = old_model.model_name;
                        new_name = cur_model.model_name;
                        var re_result = Rename(old_model, sp_name, old_name, new_name);
                        if (re_result.UpdFlag != 1)
                        {
                            return re_result;
                        }
                    }
                }
            }
            UpdateCarModel(cur_model, old_model.price);
            return new UpdResult { UpdFlag = 1, UpdMessage = "" };
        }

        public UpdResult Rename(Model model, string sp_name, string old_name, string new_name)
        {
            var need_invoke_sp = true;
            switch (sp_name)
            {
                case "rename_asset_make":
                    if (ExecuteScalar("select count(1) from ASSET_MAKE_CODE@To_Cms M WHERE  M.ASSET_MAKE_DSC='" + new_name + "' and M.activate_ind='T'") + string.Empty == "1")
                    {
                        need_invoke_sp = false;
                    }
                    break;
                case "rename_asset_brand":
                    if (ExecuteScalar("select count(1) from ASSET_Brand_CODE@To_Cms M WHERE  M.ASSET_Brand_DSC='" + new_name + "' and M.activate_ind='T'") + string.Empty == "1")
                    {
                        need_invoke_sp = false;
                    }
                    break;
            }
            if (!need_invoke_sp)
            {
                return new UpdResult { UpdFlag = 1, UpdMessage = "" };
            }

            UpdResult ur = new UpdResult();
            OracleParameter[] para1 = new OracleParameter[]
            {
                new OracleParameter("p_old_name", OracleType.VarChar, 200),
                new OracleParameter("p_new_name", OracleType.VarChar, 200),
                new OracleParameter("o_result", OracleType.Number)
            };
            double result = 0;
            string msg = "";
            para1[0].Direction = ParameterDirection.Input;
            para1[0].Value = old_name;
            para1[1].Direction = ParameterDirection.Input;
            para1[1].Value = new_name;
            para1[2].Direction = ParameterDirection.Output;
            var r = RunProcedure(sp_name, para1);
            result = Convert.ToDouble(r[2].Value + string.Empty);
            ur.UpdFlag = result;
            AppUtility.Engine.LogWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(ur));
            if (result == 1)
            {
                string sql_rename = "";
                if (sp_name== "rename_asset_make")
                {
                    sql_rename = "update t_model set series_group_name='" + new_name + "' where series_group_id='" + model.series_group_id + "' and series_group_name='" + old_name + "'";
                }
                else if(sp_name== "rename_asset_brand")
                {
                    sql_rename = "update t_model set series_name='" + new_name + "' where series_id='" + model.series_id + "' and series_name='" + old_name + "'";
                }
                AppUtility.Engine.LogWriter.Write("重命名[" + sp_name + "]：series_group_id-->" + model.series_group_id + ",series_id-->" + model.series_id + ",old_name-->" + old_name + ",new_name-->" + new_name + ",数量-->" + ExecuteNonQuery(sql_rename));
                ur.UpdMessage = msg + "修改成功:" + old_name + "-->" + new_name;
            }
            else
            {
                ur.UpdMessage = msg + "修改失败:" + ur.UpdFlag;
            }
            return ur;
        }

        //check车型是否在架;
        public bool CheckIsActivate(string model_name)
        {
            string sql = "select count(1) from ASSET_MODEL_CODE@To_Cms A where A.Asset_Model_Dsc ='{0}' and a.activate_ind='T'";
            int n = Convert.ToInt32(ExecuteScalar(string.Format(sql, model_name)));
            if (n > 0)
                return true;
            else
                return false;
        }

        //车型下架
        public bool SetModelInactivate(string model_name)
        {
            string sql = "update ASSET_MODEL_CODE@To_Cms A set a.activate_ind='F' where A.Asset_Model_Dsc ='{0}' and a.activate_ind='T'";
            int n = ExecuteNonQuery(string.Format(sql, model_name));
            if (n > 0)
                return true;
            else
                return false;
        }

        //更新T_Model_Upd的状态
        public bool Upd_T_Model_Upd_State(UpdateType upd_type, string msg, string model_id, string usercode, string username)
        {
            string sql = "update t_model_upd set upd_type={0},upd_result=upd_result||'{1}',upd_time=sysdate,upd_usercode='{3}',upd_username='{4}' where model_id='{2}' and upd_type is null";
            return ExecuteNonQuery(string.Format(sql, (int)upd_type, msg, model_id, usercode, username)) > 0;
        }

        /// <summary>
        /// 单个车型同步
        /// </summary>
        /// <param name="model_id">T_Model表中的model_id</param>
        /// <returns></returns>
        public UpdResult SyncOneModel(string model_id)
        {
            UpdResult ur = new UpdResult();
            OracleParameter[] para1 = new OracleParameter[]
            {
                new OracleParameter("p_model_id", OracleType.Number),
                new OracleParameter("o_result", OracleType.Number)
            };
            para1[0].Direction = ParameterDirection.Input;
            para1[0].Value = model_id;
            para1[1].Direction = ParameterDirection.Output;
            var r = RunProcedure("ADD_ASSET_TO_CMS_ONE", para1);
            string msg = "";
            ur.UpdFlag = Convert.ToDouble(r[1].Value + string.Empty);
            if (ur.UpdFlag == 1)
            {
                msg = "同步成功";
            }
            else
            {
                if (ur.UpdFlag == -20010)
                {
                    msg = "无车型数据";
                }
                else if (ur.UpdFlag == -20011)
                {
                    msg = "此车型在CMS中已激活";
                }
                else
                {
                    msg = "同步失败[20099]";
                }
            }
            ur.UpdMessage = msg;
            return ur;
        }

        public UpdResult SyncOneModel_Rent(string model_id)
        {
            UpdResult ur = new UpdResult();
            string sql = "select SQ_IMP_RENT.Nextval from dual";
            string num = ExecuteScalar(sql) + string.Empty;
            OracleParameter[] para1 = new OracleParameter[]
            {
                new OracleParameter("p_model_id", OracleType.Number),
                new OracleParameter("o_result", OracleType.Number),
                new OracleParameter("o_serial",OracleType.Number)
            };
            para1[0].Direction = ParameterDirection.Input;
            para1[0].Value = model_id;
            para1[1].Direction = ParameterDirection.Output;
            para1[2].Direction = ParameterDirection.Input;
            para1[2].Value = num;
            var r = RunProcedure("ADD_ASSET_TO_RENT_ONE", para1);
            ur.UpdFlag = Convert.ToDouble(r[1].Value + string.Empty);
            if (ur.UpdFlag != 1)
            {
                ur.UpdMessage = "更新失败[" + ur.UpdFlag + "]";
            }
            else
            {
                ur.UpdMessage = "更新成功";
            }
            return ur;
        }

        /// <summary>
        /// 保存车300接口返回的数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SaveChe300Data(string id, string data)
        {
            AppUtility.Engine.LogWriter.Write("保存Che300数据..." + id);
            int i = 0;
            string sql = "insert into che300_data(id,che300_data,create_date) values('" + id + "',:content,sysdate)";
            try
            {
                string connectionCode = "Engine";
                var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                OracleConnection connection = new OracleConnection(dbObject.DbConnectionString);
                connection.Open();
                OracleCommand Cmd = new OracleCommand(sql, connection);
                OracleParameter Temp = new OracleParameter("content", OracleType.Clob);
                Temp.Direction = ParameterDirection.Input;
                Temp.Value = data;
                Cmd.Parameters.Add(Temp);
                i = Cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("保存Che300数据异常：" + ex.ToString());
            }
            return i > 0;
        }

        public string GetChe300Data(string id)
        {
            string sql = "select che300_data from che300_data where id='{0}'";
            return AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(string.Format(sql, id)) + string.Empty;
        }

        public int Add_Model_Parameters(string model_id)
        {
            string url = che300_getmodelparas + "?modelId=" + model_id + "&token=" + che300_token;
            AppUtility.Engine.LogWriter.Write("Che300 api getparameters:" + url);
            string result_str = HttpHelper.get(url);
            //记录车300接口返回结果：
            AppUtility.Engine.LogWriter.Write("Che300接口返回结果：" + result_str);
            int num = 0;
            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(result_str);
            if (result["status"] + string.Empty == "1")
            {
                var paras = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(result["paramters"] + string.Empty);
                foreach (KeyValuePair<string, Dictionary<string, string>> kv in paras)
                {
                    foreach (KeyValuePair<string, string> kv_paras in kv.Value)
                    {
                        num += InsertModelParameters(model_id, kv.Key, kv_paras.Key, kv_paras.Value);
                    }
                }
            }
            AppUtility.Engine.LogWriter.Write("Add Parameters,Model ID-->" + model_id + ",总数量：" + num);
            return num;
        }

        public int InsertModelParameters(string model_id, string para_group_name, string para_name, string para_value)
        {
            if (string.IsNullOrEmpty(para_value))
                para_value = "-";
            string sql = "insert into t_model_param(model_id,parameter_group,parameter_name,parameter_value) values('{0}','{1}','{2}','{3}')";
            sql = string.Format(sql, model_id, para_group_name, para_name, para_value);
            return ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 获取当前Upd的类型,是否有修改
        /// </summary>
        /// <param name="curModel"></param>
        /// <param name="series_group_id"></param>
        /// <param name="series_group_name"></param>
        /// <param name="series_id"></param>
        /// <param name="series_name"></param>
        /// <param name="model_name"></param>
        /// <returns></returns>
        public UpdResult GetUpdFlag(Model curModel, string series_group_id, string series_group_name, string series_id, string series_name, string model_name)
        {
            int result = 0;
            string message = "";
            bool isSeries_group_id_change = false;
            bool isSeries_group_name_change = false;
            bool isSeries_id_change = false;
            bool isSeries_name_change = false;
            bool isModel_name_change = false;

            #region 5个字段是否发生了修改
            //如果车系组发生修改
            if (curModel.series_group_id != series_group_id)
            {
                isSeries_group_id_change = true;
                message += "series_group：" + series_group_name + "[" + series_group_id + "]" + "-->"
                    + curModel.series_group_name + "[" + curModel.series_group_id + "]" + "\r\n";
            }
            else if (curModel.series_group_name != series_group_name)
            {
                isSeries_group_name_change = true;
                message += "series_group_name：" + series_group_name + "-->" + curModel.series_group_name + "\r\n";
            }
            //如果车型ID修改:调整车系;名称修改则是修改名称
            if (curModel.series_id != series_id)
            {
                isSeries_id_change = true;
                isSeries_name_change = true;
                message += "series：" + series_name + "[" + series_id + "]-->" + curModel.series_name + "[" + curModel.series_id + "]\r\n";
            }
            else
            {
                if (curModel.series_name != series_name)
                {
                    isSeries_name_change = true;
                    message += "series：" + series_name + "-->" + curModel.series_name + "\r\n";
                }
            }
            //如果车型名称修改了.在CMS修改车型名称;
            if (curModel.model_name != model_name)
            {
                isModel_name_change = true;
                message += "model_name：" + model_name + "-->" + curModel.model_name + "\r\n";
            }
            #endregion

            string flag = "";
            #region 给upd_flag赋值
            //1调整,2修改,9未修改
            #region 车系组
            if (isSeries_group_id_change)
            {
                flag += "1";
            }
            else
            {
                if (isSeries_group_name_change)
                {
                    flag += "2";
                }
                else
                {
                    flag += "9";
                }
            }
            #endregion
            #region 车系
            if (isSeries_id_change)
            {
                flag += "1";
            }
            else
            {
                if (isSeries_name_change)
                {
                    flag += "2";
                }
                else
                {
                    flag += "9";
                }
            }
            #endregion
            #region 车型
            if (isModel_name_change)
            {
                flag += "2";
            }
            else
            {
                flag += "9";
            }
            #endregion
            result = Convert.ToInt32(flag);
            #endregion
            return new UpdResult { UpdFlag = result, UpdMessage = message };
        }

        public Model GetModelInfo(string Model_Id)
        {
            string errmsg = "";
            string url = che300_getmodelinfo_url + "?token=" + che300_token + "&modelId=" + Model_Id;
            AppUtility.Engine.LogWriter.Write("Che300 getmodelinfo url:" + url);
            string result_str = HttpHelper.get(url);
            //记录车300接口返回结果：
            AppUtility.Engine.LogWriter.Write("Che300接口getmodelinfo返回结果：" + result_str);


            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(result_str);
            if (result["status"].ToString() == "1")
            {
                Dictionary<string, string> m = JsonConvert.DeserializeObject<Dictionary<string, string>>(result["model"] + string.Empty);
                Model modelinfo = JsonConvert.DeserializeObject<Model>(result["model"] + string.Empty);
                try
                {
                    //modelinfo.body_type = m["body_type"] + string.Empty;
                    //modelinfo.brand_id = m["brand_id"] + string.Empty;
                    //modelinfo.brand_name = m["brand_name"] + string.Empty;
                    //modelinfo.car_struct = m["car_struct"] + string.Empty;
                    modelinfo.discharge_standard = m["discharge"] + string.Empty;
                    //modelinfo.door_number = m["door_number"] + string.Empty;
                    //modelinfo.drive_name = m["drive_name"] + string.Empty;
                    //modelinfo.engine_power = m["engine_power"] + string.Empty;
                    //modelinfo.gear_name = m["gear_name"] + string.Empty;
                    modelinfo.gear_type = m["gear"] + string.Empty;
                    //modelinfo.is_green = m["is_green"] + string.Empty;
                    //modelinfo.is_parallel = m["is_parallel"] + string.Empty;
                    modelinfo.liter = m["liter"] + m["liter_unit"] + string.Empty;
                    //modelinfo.maker_type = m["maker_type"] + string.Empty;
                    //modelinfo.market_date = m["market_date"] + string.Empty;
                    //modelinfo.max_reg_year = m["max_reg_year"] + string.Empty;
                    //modelinfo.min_reg_year = m["min_reg_year"] + string.Empty;
                    modelinfo.model_id = m["id"] + string.Empty;
                    modelinfo.model_name = m["name"] + string.Empty;
                    modelinfo.model_year = m["year"] + string.Empty;
                    //modelinfo.price = m["price"] + string.Empty;
                    //modelinfo.seat_number = m["seat_number"] + string.Empty;
                    //modelinfo.series_group_id = m["series_group_id"] + string.Empty;
                    //modelinfo.series_group_name = m["series_group_name"] + string.Empty;
                    //modelinfo.series_id = m["series_id"] + string.Empty;
                    //modelinfo.series_name = m["series_name"] + string.Empty;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (modelinfo.is_parallel == "1")
                {
                    modelinfo.series_group_name += "(平行进口)";
                    modelinfo.series_group_id += "A";
                }
                return modelinfo;
            }
            return null;
        }

        //获取当前model表中的车型名称
        public Model GetT_ModelInfo(string model_id)
        {
            string sql = "select * from t_model where model_id='" + model_id + "'";
            DataTable dt = ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Type type = typeof(Model);
                object obj = Activator.CreateInstance(type, null);
                PropertyInfo[] fields = obj.GetType().GetProperties();
                foreach (PropertyInfo t in fields)
                {
                    if (t.CanWrite)
                    {
                        try
                        {
                            if (t.Name == "model_year")
                            {
                                t.SetValue(obj, row["year"] + string.Empty, null);
                            }
                            else if (t.Name == "gear_name")
                            {
                                t.SetValue(obj, row["gear_type"] + string.Empty, null);
                            }
                            else if (t.Name == "drive_name")
                            {
                                t.SetValue(obj, row["drive_type"] + string.Empty, null);
                            }
                            else
                            {
                                t.SetValue(obj, row[t.Name] + string.Empty, null);//给对象赋值
                            }
                        }
                        catch
                        {
                            t.SetValue(obj, "", null);//给对象赋值
                        }
                    }
                }
                return (Model)obj;
            }
            return null;
        }

        //获取当前model表中的车型名称
        public Model GetT_Model_UpdInfo(string model_id)
        {
            string sql = "select * from t_model_upd where model_id='" + model_id + "' and upd_type is null";
            DataTable dt = ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Type type = typeof(Model);
                object obj = Activator.CreateInstance(type, null);
                PropertyInfo[] fields = obj.GetType().GetProperties();
                foreach (PropertyInfo t in fields)
                {
                    if (t.CanWrite)
                    {
                        try
                        {
                            if (t.Name == "model_year")
                            {
                                t.SetValue(obj, row["year"] + string.Empty, null);
                            }
                            else if (t.Name == "gear_name")
                            {
                                t.SetValue(obj, row["gear_type"] + string.Empty, null);
                            }
                            else if (t.Name == "drive_name")
                            {
                                t.SetValue(obj, row["drive_type"] + string.Empty, null);
                            }
                            else
                            {
                                t.SetValue(obj, row[t.Name] + string.Empty, null);//给对象赋值
                            }
                        }
                        catch
                        {
                            t.SetValue(obj, "", null);//给对象赋值
                        }
                    }
                }
                return (Model)obj;
            }
            return null;
        }

        public bool rent_asset_del(string type, string id)
        {
            OracleParameter[] para = new OracleParameter[]
           {
                new OracleParameter("p_del_type", OracleType.Number),
             new OracleParameter("p_id", OracleType.Number),
             new OracleParameter("o_result", OracleType.VarChar,200)
           };

            para[0].Direction = ParameterDirection.Input;
            para[0].Value = type;
            para[1].Direction = ParameterDirection.Input;
            para[1].Value = id;
            para[2].Direction = ParameterDirection.Output;
            var result1 = RunProcedure("DEL_RENT_ASSET", para);
            AppUtility.Engine.LogWriter.Write("执行存储过程DEL_RENT_ASSET结束，参数Type-->" + type + "，P_id-->" + id + "，返回值-->" + result1[2].Value);
            if (result1[2].Value + string.Empty == "1")
            {
                return true;
            }
            return false;
        }

        public UpdResult UpdateNameWhenAdjust(Model cur_model)
        {
            UpdResult ur = new UpdResult() { UpdFlag = 1, UpdMessage = "未修改名称" };
            string sql_series_group_name = "select distinct series_group_name from t_model where series_group_id='" + cur_model.series_group_id + "'";
            string old_series_group_name = ExecuteScalar(sql_series_group_name) + string.Empty;
            string sql_series_name = "select distinct series_name from t_model where series_id='" + cur_model.series_id + "'";
            string old_series_name = ExecuteScalar(sql_series_name) + string.Empty;
            if (old_series_group_name != "" && cur_model.series_group_name != old_series_group_name)
            {
                ur = Rename(cur_model, "rename_asset_make", old_series_group_name, cur_model.series_group_name);
                if (ur.UpdFlag != 1)
                    return ur;
            }
            if (old_series_name != "" && cur_model.series_name != old_series_name)
            {
                ur = Rename(cur_model, "rename_asset_brand", old_series_name, cur_model.series_name);
                if (ur.UpdFlag != 1)
                    return ur;
            }
            return ur;
        }

        #region 数据库操作
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                string connectionCode = "ImportModal";
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteNonQuery(sql);
                }
            }
            catch
            {
                return 0;
            }
            return 0;
        }

        public object ExecuteScalar(string sql)
        {
            try
            {
                string connectionCode = "ImportModal";
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteScalar(sql);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public DataTable ExecuteDataTable(string sql)
        {
            try
            {
                string connectionCode = "ImportModal";
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteDataTable(sql);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public OracleParameterCollection RunProcedure(string StoredProcedureName, OracleParameter[] Parameters)
        {
            string connectionCode = "ImportModal";
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            OracleConnection conn = new OracleConnection(dbObject.DbConnectionString);
            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedureName;
                cmd.Parameters.AddRange(Parameters);
                cmd.ExecuteNonQuery();
                conn.Close();
                return cmd.Parameters;
            }
            catch (Exception ex)
            {
                conn.Close();
                AppUtility.Engine.LogWriter.Write("RunProcedure执行存储过程方法异常：" + ex.ToString());
                return null;
            }

        }
        #endregion

        #region 品牌 增、删、改
        public int AddBrand(Brand v, DateTime versionTime, string versionId)
        {
            string sql_Insert = "insert into t_brand(brand_id,brand_name,brand_initial,version_time,version_id) values('{0}','{1}','{2}',to_date('{3}','yyyy-MM-dd hh24:mi:ss'),'{4}')";
            sql_Insert = string.Format(sql_Insert, v.brand_id, v.brand_name, v.brand_initial, versionTime.ToString("yyyy-MM-dd HH:mm:ss"), versionId);
            return ExecuteNonQuery(sql_Insert);
        }
        public int UpdateBrand(Brand v, DateTime versionTime, string versionId)
        {
            string sql_Update = "update t_brand set brand_name='{1}',brand_initial='{2}',version_time=to_date('{3}','yyyy-MM-dd hh24:mi:ss'),version_id='{4}' where brand_id='{0}'";
            sql_Update = string.Format(sql_Update, v.brand_id, v.brand_name, v.brand_initial, versionTime.ToString("yyyy-MM-dd HH:mm:ss"), versionId);
            return ExecuteNonQuery(sql_Update);
        }
        public int DeleteBrand(Brand v)
        {
            //1.先插入到t_brand_del表
            //2.删除原表t_brand表数据
            string sql_insert_to_del = @"insert into t_brand_del( brand_id,brand_name,brand_initial,version_time,version_id,upd_flag,del_time)
select brand_id,brand_name,brand_initial,version_time,version_id,0 upd_flag,sysdate from t_brand where brand_id='{0}'";
            if (ExecuteNonQuery(string.Format(sql_insert_to_del, v.brand_id)) > 0)
            {
                string sql_Del = "delete from t_brand where brand_id='{0}'";
                sql_Del = string.Format(sql_Del, v.brand_id);
                return ExecuteNonQuery(sql_Del);
            }
            return 0;
        }
        #endregion

        #region 车系 增、删、改
        public int AddSeries(Series v, DateTime versionTime, string versionId)
        {
            string sql_Insert = @"insert into t_series(brand_id,brand_name,series_group_name,series_id,series_name,level_name,maker_type,version_time,version_id) 
                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',to_date('{7}','yyyy-MM-dd hh24:mi:ss'),'{8}')";
            sql_Insert = string.Format(sql_Insert, v.brand_id, v.brand_name, v.series_group_name, v.series_id, v.series_name, v.level_name, v.maker_type, versionTime.ToString("yyyy-MM-dd HH:mm:ss"), versionId);
            return ExecuteNonQuery(sql_Insert);
        }
        public int UpdateSeries(Series v, DateTime versionTime, string versionId)
        {
            string sql_Update = "update t_series set brand_id='{0}', brand_name='{1}',series_group_name='{2}',series_name='{4}',level_name='{5}',maker_type='{6}',version_time=to_date('{7}','yyyy-MM-dd hh24:mi:ss'),version_id='{8}' where series_id='{3}'";
            sql_Update = string.Format(sql_Update, v.brand_id, v.brand_name, v.series_group_name, v.series_id, v.series_name, v.level_name, v.maker_type, versionTime.ToString("yyyy-MM-dd HH:mm:ss"), versionId);
            return ExecuteNonQuery(sql_Update);
        }
        public int DeleteSeries(Series v)
        {
            //1.先插入到t_series_del表
            //2.删除原表t_series表数据
            string sql_insert_to_del = @"insert into t_series_del(brand_id,brand_name,series_group_name, series_id,series_name,level_name,maker_type,version_time,version_id,upd_flag,del_time)
select brand_id,brand_name,series_group_name,series_id,series_name,level_name,maker_type,version_time,version_id,0 upd_flag,sysdate from t_series where series_id='{0}'";
            if (ExecuteNonQuery(string.Format(sql_insert_to_del, v.series_id)) > 0)
            {
                string sql_Del = "delete from t_series where series_id='{0}'";
                sql_Del = string.Format(sql_Del, v.series_id);
                return ExecuteNonQuery(sql_Del);
            }
            return 0;
        }
        #endregion

        #region 车型 增、删、改
        public int AddModel(Model v, DateTime versionTime, string versionId)
        {
            string sql_Insert = @"
insert into t_model(brand_id,brand_name,series_group_name,series_id,series_name,model_id,model_name,price,year,liter,engine_power,
gear_type,discharge_standard,market_date,door_number,seat_number,drive_type,car_struct,body_type,is_parallel,is_green,version_time,version_id,min_reg_year,max_reg_year,series_group_id)
values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21},'{22}','{23}','{24}','{25}')
";
            sql_Insert = string.Format(sql_Insert, v.brand_id, v.brand_name, v.series_group_name
                , v.series_id, v.series_name, v.model_id, v.model_name, v.price, v.model_year, v.liter,
                v.engine_power, v.gear_name, v.discharge_standard, v.market_date == "" ? "null" : "to_date('" + v.market_date + "','yyyy-mm-dd hh24:mi:ss')", v.door_number, v.seat_number,
                v.drive_name, v.car_struct, v.body_type, v.is_parallel, v.is_green, "to_date('" + versionTime.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')", versionId, v.min_reg_year, v.max_reg_year, v.series_group_id);
            return ExecuteNonQuery(sql_Insert);
        }

        public int UpdateCarModel(Model model,string old_price)
        {
            string sql_Upd = @"
                            update t_model set brand_id='{0}',brand_name='{1}',series_group_name='{2}',series_id='{3}',series_name='{4}',model_id='{5}',
                            model_name='{6}',price='{7}',year='{8}',liter='{9}',engine_power='{10}',gear_type='{11}',discharge_standard='{12}',market_date={13},
                            door_number='{14}',seat_number='{15}',drive_type='{16}',car_struct='{17}',body_type='{18}',is_parallel='{19}',is_green='{20}',version_time={21},version_id='{22}',min_reg_year='{23}',max_reg_year='{24}',series_group_id='{25}'
                            where model_id='{5}'";
            sql_Upd = string.Format(sql_Upd, model.brand_id, model.brand_name, model.series_group_name
                , model.series_id, model.series_name, model.model_id, model.model_name, model.price, model.model_year, model.liter,
                model.engine_power, model.gear_name, model.discharge_standard, model.market_date == "" ? "null" : "to_date('" + model.market_date + "','yyyy-mm-dd hh24:mi:ss')", model.door_number, model.seat_number,
                model.drive_name, model.car_struct, model.body_type, model.is_parallel, model.is_green, string.IsNullOrEmpty(model.version_time) ? "sysdate" : "to_date('" + model.version_time + "','yyyy-mm-dd hh24:mi:ss')", model.version_id, model.min_reg_year, model.max_reg_year, model.series_group_id);

            if (model.price != old_price)
            {
                UpdateCMSModelPrice(model.model_name, model.price);
            }
            return ExecuteNonQuery(sql_Upd);
        }

        //更新CMS系统中车型的价格
        public UpdResult UpdateCMSModelPrice(string model_name,string new_price)
        {
            string sql_get_cde = "select asset_model_cde from ASSET_MODEL_CODE@To_Cms M where m.asset_model_dsc='" + model_name + "' and m.activate_ind='T'";
            string asset_code = ExecuteScalar(sql_get_cde) + string.Empty;
            OracleParameter[] para2 = new OracleParameter[]
            {
                new OracleParameter("p_asset_model_cde", OracleType.VarChar, 200)
                ,new OracleParameter("p_new_price", OracleType.Number,10)
                ,new OracleParameter("o_result",OracleType.Number)
            };

            para2[0].Direction = ParameterDirection.Input;
            para2[0].Value = asset_code;
            para2[1].Direction = ParameterDirection.Input;
            para2[1].Value = Convert.ToDouble(new_price) * 10000;
            para2[2].Direction = ParameterDirection.Output;
            var result2 = RunProcedure("chang_asset_model_price", para2);
            AppUtility.Engine.LogWriter.Write("执行存储过程chang_asset_model_price结束，返回值o_result-->" + result2[2].Value + string.Empty);
            double result = Convert.ToInt32(result2[2].Value);
            string msg = result == 1 ? "修改价格成功" : "[" + result + "]修改价格失败";
            return new UpdResult() { UpdFlag = result, UpdMessage = msg };
        }

        public int AddModelUpd(Model v, DateTime versionTime, string versionId, string UpdFlag, string UpdMessage)
        {
            string sql_Insert = @"
insert into t_model_upd(brand_id,brand_name,series_group_name,series_id,series_name,model_id,model_name,price,year,liter,engine_power,
gear_type,discharge_standard,market_date,door_number,seat_number,drive_type,car_struct,body_type,is_parallel,is_green,version_time,upd_flag,version_id,min_reg_year,max_reg_year,upd_comment,series_group_id,createdtime)
values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21},{22},'{23}','{24}','{25}','{26}','{27}',sysdate)
";
            sql_Insert = string.Format(sql_Insert, v.brand_id, v.brand_name, v.series_group_name
                , v.series_id, v.series_name, v.model_id, v.model_name, v.price, v.model_year, v.liter,
                v.engine_power, v.gear_name, v.discharge_standard, v.market_date == "" ? "null" : "to_date('" + v.market_date + "','yyyy-mm-dd hh24:mi:ss')", v.door_number, v.seat_number,
                v.drive_name, v.car_struct, v.body_type, v.is_parallel, v.is_green, "to_date('" + versionTime.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')",
                UpdFlag, versionId, v.min_reg_year, v.max_reg_year, UpdMessage, v.series_group_id);
            return ExecuteNonQuery(sql_Insert);
        }

        public int DeleteModel(Model v, DateTime versionTime, string versionId)
        {
            string sql_Get = "select * from t_model where model_id='" + v.model_id + "'";
            DataTable dt = ExecuteDataTable(sql_Get);
            if (dt == null || dt.Rows.Count == 0)//没有取到数据
            {
                return 0;
            }
            return AddModelUpd(v, versionTime, versionId, "-1", "车300已删除");
        }
        #endregion
        #endregion
    }
}