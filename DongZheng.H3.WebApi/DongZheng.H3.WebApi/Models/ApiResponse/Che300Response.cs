using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ApiResponse
{
    public class IdentifyModelByVINResult
    {
        public string status { get; set; }
        public string error_msg { get; set; }
        public List<Model2> modelInfo { get; set; }
    }

    /// <summary>
    /// che300返回的结果
    /// </summary>
    public class che300_result
    {
        public int status { get; set; }
        public Data data { get; set; }
        public string error_msg { get; set; }
    }

    public class che300_ModelInfo
    {
        public int status { get; set; }
        public Model model { get; set; }
    }

    /// <summary>
    /// 品牌
    /// </summary>
    public class Brand
    {
        public string brand_id { get; set; }
        public string brand_name { get; set; }
        public string brand_initial { get; set; }
        public string oper_type { get; set; }
    }
    /// <summary>
    /// 车系
    /// </summary>
    public class Series
    {
        public string brand_id { get; set; }
        public string brand_name { get; set; }
        public string series_id { get; set; }
        public string series_name { get; set; }
        public string series_group_name { get; set; }
        public string level_name { get; set; }
        public string maker_type { get; set; }
        public string oper_type { get; set; }
    }
    /// <summary>
    /// 车型
    /// </summary>
    public class Model
    {
        public string brand_id { get; set; }
        public string brand_name { get; set; }
        public string series_id { get; set; }
        public string series_name { get; set; }
        public string series_group_id { get; set; }
        public string series_group_name { get; set; }
        public string model_id { get; set; }
        public string model_name { get; set; }
        public string price { get; set; }
        public string liter { get; set; }
        public string gear_type { get; set; }
        public string model_year { get; set; }
        public string maker_type { get; set; }
        public string discharge_standard { get; set; }
        public string seat_number { get; set; }
        public string min_reg_year { get; set; }
        public string max_reg_year { get; set; }
        public string gear_name { get; set; }
        public string engine_power { get; set; }
        public string market_date { get; set; }
        public string door_number { get; set; }
        public string drive_name { get; set; }
        public string car_struct { get; set; }
        public string body_type { get; set; }
        public string is_parallel { get; set; }
        public string is_green { get; set; }
        public string oper_type { get; set; }
        //自定义字段
        public string version_id { get; set; }
        public string version_time { get; set; }
        public string upd_flag { get; set; }
        public string upd_flag_dsc
        {
            get
            {
                string msg = "";
                switch (upd_flag)
                {
                    case "-1": msg = "车300已删除"; break;
                    case "0": msg = "历史数据"; break;
                    case "3": msg = "不做处理"; break;
                    case "199": msg = "品牌调整"; break;
                    case "299": msg = "品牌名称修改"; break;
                    case "119": msg = "品牌调整<br/>车系调整"; break;
                    case "219": msg = "品牌名称修改<br/>车系调整"; break;
                    case "112": msg = "品牌调整<br/>车系调整<br/>车型名称修改"; break;
                    case "212": msg = "品牌名称修改<br/>车系调整<br/>车型名称修改"; break;
                    case "129": msg = "品牌调整<br/>车系名称修改"; break;
                    case "229": msg = "品牌名称修改<br/>车系名称修改"; break;
                    case "122": msg = "品牌调整<br/>车系名称修改<br/>车型名称修改"; break;
                    case "222": msg = "品牌名称修改<br/>车系名称修改<br/>车型名称修改"; break;
                    case "192": msg = "品牌调整<br/>车型名称修改"; break;
                    case "292": msg = "品牌名称修改<br/>车型名称修改"; break;
                    case "919": msg = "车系调整"; break;
                    case "912": msg = "车系调整<br/>车型名称修改"; break;
                    case "929": msg = "车系名称修改"; break;
                    case "922": msg = "车系名称修改<br/>车型名称修改"; break;
                    case "992": msg = "车型名称修改"; break;
                    case "999": msg = "无修改"; break;

                }
                return msg;
            }
        }
        public string upd_type { get; set; }
        public string upd_time { get; set; }
        public string upd_usercode { get; set; }
        public string upd_username { get; set; }
        public string upd_comment { get; set; }
        public string upd_result { get; set; }
    }

    public class Model2
    {
        public int brand_id { get; set; }
        public string brand_name { get; set; }
        public string series_group_name { get; set; }
        public int series_id { get; set; }
        public string series_name { get; set; }
        public int model_id { get; set; }
        public string model_name { get; set; }
        public int model_year { get; set; }
        public decimal model_price { get; set; }
        public string model_gear { get; set; }
        public string model_liter { get; set; }
        public string model_emission_standard { get; set; }
        public int min_reg_year { get; set; }
        public int max_reg_year { get; set; }
        public int ext_model_id { get; set; }
        public string color { get; set; }
    }

    /// <summary>
    /// SyncVersion
    /// </summary>
    public class SyncVersion
    {
        public DateTime ModelTimestamp { get; set; }
        public DateTime SeriesTimestamp { get; set; }
        public DateTime BrandTimestamp { get; set; }
        public string CurrentVersion { get; set; }
        public string InitialVersion { get; set; }
    }
    /// <summary>
    /// che300返回的Data
    /// </summary>
    public class Data
    {
        public string id { get; set; }
        public List<Brand> brand { get; set; }
        public List<Series> series { get; set; }
        public List<Model> model { get; set; }
        public List<SyncVersion> version { get; set; }
    }

    public class BrandList
    {
        public string status { get; set; }
        public List<Brand> brand_list { get; set; }

    }

    public class UpdResult
    {
        public double UpdFlag { get; set; }
        public string UpdMessage { get; set; }
    }

    public class ImportResult
    {
        public int Brand_Insert_Num { get; set; }
        public int Brand_Update_Num { get; set; }
        public int Brand_Delete_Num { get; set; }

        public int Series_Insert_Num { get; set; }
        public int Series_Update_Num { get; set; }
        public int Series_Delete_Num { get; set; }

        public int Model_Insert_Num { get; set; }
        public int Model_Update_Num { get; set; }
        public int Model_Delete_Num { get; set; }

        public int Brand_Series_Update_Num { get; set; }

        public override string ToString()
        {
            string msg = "";
            if (Brand_Insert_Num > 0)
                msg += "新增品牌：" + Brand_Insert_Num + "个，";
            if (Brand_Update_Num > 0)
                msg += "更新品牌：" + Brand_Update_Num + "个，";
            if (Brand_Delete_Num > 0)
                msg += "删除品牌：" + Brand_Delete_Num + "个，";

            if (Series_Insert_Num > 0)
                msg += "新增车系：" + Series_Insert_Num + "个，";
            if (Series_Update_Num > 0)
                msg += "更新车系：" + Series_Update_Num + "个，";
            if (Series_Delete_Num > 0)
                msg += "删除车系：" + Series_Delete_Num + "个，";

            if (Model_Insert_Num > 0)
                msg += "新增车型：" + Model_Insert_Num + "个，";
            if (Model_Update_Num > 0)
                msg += "更新车型：" + Model_Update_Num + "个，";
            if (Model_Delete_Num > 0)
                msg += "删除车型：" + Model_Delete_Num + "个，";

            if (Brand_Series_Update_Num > 0)
                msg += "需要手工调整的车型：" + Brand_Series_Update_Num + "个,";
            if (msg == "")
            {
                msg = "无任何修改";
            }
            else
            {
                msg = msg.Substring(0, msg.Length - 1);
            }
            return msg;
        }
    }

    #region Convert
    public class CarConvert
    {
        public static List<Series> ConvertToSeries(Brand brand, string source)
        {
            List<Series> series = new List<Series>();
            var listSource = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(source);
            foreach (var li in listSource)
            {
                Series s = new Series();
                s.brand_id = brand.brand_id;
                s.brand_name = brand.brand_name;
                s.level_name = "";
                s.maker_type = li["maker_type"] + string.Empty;
                s.series_group_name = li["series_group_name"] + string.Empty;
                s.series_id = li["series_id"] + string.Empty;
                s.series_name = li["series_name"] + string.Empty;
                series.Add(s);
            }
            return series;
        }

        public static List<Brand> ConvertToBrand(string source)
        {
            List<Brand> brands = new List<Brand>();
            var listSource = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(source);
            foreach (var li in listSource)
            {
                Brand s = new Brand();
                s.brand_id = li["brand_id"] + string.Empty;
                s.brand_name = li["brand_name"] + string.Empty;
                s.brand_initial = li["initial"] + string.Empty;
                brands.Add(s);
            }
            return brands;
        }
    }
    #endregion

    #region Enum
    public enum UpdateType
    {
        Success = 0,
        Ignore = 3,
        RentFail = 2
    }
    public enum ModelType
    {
        Make,
        Brand,
        Model
    }
    #endregion

}