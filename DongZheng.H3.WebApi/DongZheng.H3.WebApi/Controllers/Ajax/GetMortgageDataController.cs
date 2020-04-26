using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class GetMortgageDataController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public class Motigage
        {
            public string Province { get; set; }        //所在省份      STATE_NAME
            public string City { get; set; }            //所在城市      CITY_NAME
            public string ApplyType { get; set; }      //申请类型      APPLICATION_TYPE_NAME
            public string JKName { get; set; }        //借款人姓名
            public string GJName { get; set; }          //共借人姓名
            public string DBRName { get; set; }          //担保人姓名
            public string JKCardID { get; set; }         //证件号码
            public string GJCardID { get; set; }          //共借人证件号码
            public string DBRCardID { get; set; }          //担保人证件号码
            public string Assets { get; set; }            //资产状况         CONDITION_NAME
            public string Factory { get; set; }           //制作商           ASSET_MAKE_DSC
            public string CarType { get; set; }           //车型              POWER_PARAMETER 
            public string MotorType { get; set; }          //发动机号码     ENGINE_NUMBER
            public string FrameNumber { get; set; }          //车架号       VIN_NUMBER
            public string ProductClass { get; set; }          //产品组      FP_GROUP_NAME
            public string ProductType { get; set; }          //产品类型     FINANCIAL_PRODUCT_NAME
            public string Isself { get; set; }
            public string Localdo { get; set; }
            public string SelectShop { get; set; }
            public string SFMoney { get; set; }            //首付金额       CASH_DEPOSIT
            public string DKMoney { get; set; }           //贷款金额        AMOUNT_FINANCED
            public string FJMoney { get; set; }           //附加费          ACCESSORY_AMT
            public string LicenceNo { get; set; }         //牌照号 
            public string DyName { get; set; }           //抵押员
            public string SpName { get; set; }            //上牌员　
            public string OBJECTID { get; set; }           //ObjectID
            public string DealerName { get; set; }        //经销商名称
            public string NewCarPrice { get; set; }          //新车指导价      NEW_PRICE
            public string AssetsPrice { get; set; }          //资产价格        NEW_PRICE
            public string PdfPath { get; set; }
            public string ApplyNo { get; set; }
            public List<GJPeople> GJPeople;
        }
        public class GJPeople
        {
            public string People;
            public string PeopleType;
            public string ID;
            public string IdType;
        }
        public void Index()
        {
            var context = HttpContext;
            string applyNo = context.Request["ApplyNo"] ?? "Br-A087422000";
            Motigage mt = GetPrintPDFURL(applyNo);
            JavaScriptSerializer json = new JavaScriptSerializer();
            context.Response.Write(json.Serialize(mt));
        }

        public Motigage GetPrintPDFURL(string applyNo)
        {
            string tmpPath = "";// System.Web.HttpContext.Current.Server.MapPath("").Replace("ajax", "Filetemplate") + "\\营业执照\\";
            string sql = "SELECT * FROM I_MORTGAGE WHERE APPLYNO = '" + applyNo + "'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            Motigage mt = new Motigage();
            if (dt.Rows.Count > 0)
            {
                mt.Province = dt.Rows[0]["PROVINCE"].ToString();
                mt.City = dt.Rows[0]["CITY"].ToString();
                mt.ApplyType = dt.Rows[0]["APPLYTYPE"].ToString();
                mt.JKName = dt.Rows[0]["JKNAME"].ToString();
                mt.GJName = dt.Rows[0]["GJNAME"].ToString();
                mt.DBRName = dt.Rows[0]["DBRNAME"].ToString();
                mt.JKCardID = dt.Rows[0]["JKCARDID"].ToString();
                mt.GJCardID = dt.Rows[0]["GJCARDID"].ToString();
                mt.DBRCardID = dt.Rows[0]["DBRCARDID"].ToString();
                mt.Assets = dt.Rows[0]["ASSETS"].ToString();
                mt.Factory = dt.Rows[0]["FACTORY"].ToString();
                mt.CarType = dt.Rows[0]["CARTYPE"].ToString();
                mt.NewCarPrice = dt.Rows[0]["NEWCARPRICE"].ToString();
                mt.AssetsPrice = dt.Rows[0]["ASSETSPRICE"].ToString();
                mt.MotorType = dt.Rows[0]["MOTORTYPE"].ToString();
                mt.FrameNumber = dt.Rows[0]["FRAMENUMBER"].ToString();
                mt.ProductClass = dt.Rows[0]["PRODUCTCLASS"].ToString();
                mt.ProductType = dt.Rows[0]["PRODUCTTYPE"].ToString();
                mt.DKMoney = dt.Rows[0]["DKMONEY"].ToString();
                mt.SFMoney = dt.Rows[0]["SFMONEY"].ToString();
                mt.FJMoney = dt.Rows[0]["FJMONEY"].ToString();
                mt.LicenceNo = dt.Rows[0]["LICENCENO"].ToString();
                mt.DyName = dt.Rows[0]["DYNAME"].ToString();
                mt.SpName = dt.Rows[0]["SPNAME"].ToString();
                mt.OBJECTID = dt.Rows[0]["OBJECTID"].ToString();
                mt.DealerName = dt.Rows[0]["经销商名称"].ToString();
                mt.Isself = dt.Rows[0]["ISSELF"].ToString();
                mt.Localdo = dt.Rows[0]["LOCALDO"].ToString();
                mt.SelectShop = dt.Rows[0]["SELECTSHOP"].ToString();
                mt.ApplyNo = dt.Rows[0]["APPLYNO"].ToString();
                List<GJPeople> GJlist = new List<GJPeople>();
                string gjsql = "SELECT * FROM I_GJPEOPLE WHERE PARENTOBJECTID = '" + dt.Rows[0]["OBJECTID"] + "'";
                DataTable gjdt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(gjsql);
                if (gjdt.Rows.Count > 0)
                {
                    for (int i = 0; i < gjdt.Rows.Count; i++)
                    {
                        GJPeople gj = new GJPeople();
                        gj.People = gjdt.Rows[i]["PEOPLE"].ToString();
                        gj.PeopleType = gjdt.Rows[i]["PEOPLETYPE"].ToString();
                        gj.IdType = gjdt.Rows[i]["IDTYPE"].ToString();
                        gj.ID = gjdt.Rows[i]["ID"].ToString();
                        GJlist.Add(gj);
                    }
                }
                mt.GJPeople = GJlist;
            }
            switch (mt.DealerName)
            {
                case "包头市宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "包头市宝泽汽车销售服务有限公司高新区分公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "包头市路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "包头市世通瑞晟汽车销售服务有限公司 01":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "包头众锐汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "北京百旺沃瑞汽车销售服务有限公司":
                    break;
                case "北京宝泽行汽车销售服务有限公司":
                    break;
                case "北京德万隆经贸有限公司":
                    break;
                case "北京嘉瑞雅汽车销售服务有限公司":
                    break;
                case "北京名尊奥翔汽车销售有限公司":
                    break;
                case "北京正通宝泽行汽车销售有限公司":
                    break;
                case "北京正通鼎沃汽车销售服务有限公司":
                    break;
                case "北京中汽南方华北汽车服务有限公司":
                    break;
                case "北京中汽南方中关汽车销售有限公司":
                    break;
                case "郴州瑞宝汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150039.pdf";
                    break;
                case "成都劲翔汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150057.pdf";
                    break;
                case "成都祺宝汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150057.pdf";
                    break;
                case "东莞奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150011.pdf";
                    break;
                case "东莞捷运行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150011.pdf";
                    break;
                case "东莞寮步中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150011.pdf";
                    break;
                case "东莞正通凯迪汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150011.pdf";
                    break;
                case "东莞中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150011.pdf";
                    break;
                case "佛山奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150026.pdf";
                    break;
                case "佛山鼎宝行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150024.pdf";
                    break;
                case "佛山正通众锐汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150024.pdf";
                    break;
                case "福建中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150032.pdf";
                    break;
                case "福州鼎沃汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150032.pdf";
                    break;
                case "抚州市东信汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "赣州宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150064.pdf";
                    break;
                case "广东中汽南方汽车销售服务有限公司（VOLVO）":
                    tmpPath = tmpPath + "00000002201808150021.pdf";
                    break;
                case "广东中汽南方汽车销售服务有限公司（路虎）":
                    tmpPath = tmpPath + "00000002201808150021.pdf";
                    break;
                case "广西瑞合行汽车服务有限公司":
                    tmpPath = tmpPath + "00000002201808150028.pdf";
                    break;
                case "广州宝泰行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150021.pdf";
                    break;
                case "广州宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150021.pdf";
                    break;
                case "广州宝泽汽车销售服务有限公司MINI":
                    tmpPath = tmpPath + "00000002201808150021.pdf";
                    break;
                case "广州市恒悦行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150022.pdf";
                    break;
                case "贵州瑞合行汽车服务有限公司":
                    tmpPath = tmpPath + "00000002201808150038.pdf";
                    break;
                case "海南中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150048.pdf";
                    break;
                case "合肥速融汽车贸易有限公司":
                    tmpPath = tmpPath + "00000002201808150009.pdf";
                    break;
                case "河南必诚汽车服务有限公司":
                    tmpPath = tmpPath + "00000002201808150012.pdf";
                    break;
                case "河南菲卡汽车销售有限公司 02":
                    tmpPath = tmpPath + "00000002201808150012.pdf";
                    break;
                case "河南省锦堂盛汽车有限公司":
                    tmpPath = tmpPath + "00000002201808150012.pdf";
                    break;
                case "衡阳路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150039.pdf";
                    break;
                case "呼和浩特市捷运行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "呼和浩特市祺宝汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "呼和浩特市英菲汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "湖北奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湖北博诚汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湖北鼎杰汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湖北捷瑞汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湖北欣瑞汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湖南融晟行汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150049.pdf";
                    break;
                case "湖南中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150045.pdf";
                    break;
                case "湖南中汽南方星沙汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150015.pdf";
                    break;
                case "吉林坔水汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150019.pdf";
                    break;
                case "济南天瑞汽车咨询服务有限公司":
                    tmpPath = tmpPath + "00000002201808150016.pdf";
                    break;
                case "江西德奥汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150060.pdf";
                    break;
                case "江西正通泽田汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "揭阳鼎杰汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150047.pdf";
                    break;
                case "揭阳路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150047.pdf";
                    break;
                case "荆门宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150054.pdf";
                    break;
                case "廊坊市路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150023.pdf";
                    break;
                case "连云港市耀辉汽车贸易有限公司 01":
                    tmpPath = tmpPath + "00000002201808150034.pdf";
                    break;
                case "连云港众可汽车销售有限公司 02":
                    tmpPath = tmpPath + "00000002201808150034.pdf";
                    break;
                case "南昌宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "内蒙古鼎杰汽车贸易有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "内蒙古鼎泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "内蒙古津远进口汽车销售服务有限公司 02":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "内蒙古鑫垚陆之虎汽贸有限公司 02":
                    tmpPath = tmpPath + "00000002201808150053.pdf";
                    break;
                case "青岛奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150055.pdf";
                    break;
                case "青岛华成汽车服务有限公司":
                    tmpPath = tmpPath + "00000002201808150055.pdf";
                    break;
                case "清远南方丰田汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150059.pdf";
                    break;
                case "清远南方合众汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150059.pdf";
                    break;
                case "汕头市宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150066.pdf";
                    break;
                case "汕头市宏祥物资有限公司":
                    tmpPath = tmpPath + "00000002201808150066.pdf";
                    break;
                case "汕头市路杰汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150066.pdf";
                    break;
                case "上海奥汇汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海锋之行汽车金融信息服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海举骋汽车贸易有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海陆达汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海陆狮汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海绅瑞汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海绅协汽车贸易有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海绅协绅起汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海绅协绅通汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上海绅亚汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150029.pdf";
                    break;
                case "上饶市宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "上饶市路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "深圳宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳鼎沃汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市南方腾龙汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市南方腾田汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市南方腾星汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市南方英菲尼迪汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市彦信金融服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市彦信汽车信息咨询服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市中汽南方华沃汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市中汽南方机电设备有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "深圳市中汽南方汽车维修有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "嵊州奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150017.pdf";
                    break;
                case "十堰绅协汽车贸易有限公司":
                    tmpPath = tmpPath + "00000002201808150050.pdf";
                    break;
                case "天津汽车工业销售深圳南方有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "天津中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150014.pdf";
                    break;
                case "威海路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150027.pdf";
                    break;
                case "武汉宝泽行汽车维修服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉宝泽汽车销售服务有限公司江岸分公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉开泰汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉路泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉正邦兴达资产管理咨询有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "武汉正通悦驰汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "湘潭宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150050.pdf";
                    break;
                case "襄阳宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150010.pdf";
                    break;
                case "徐州千瑞汽车销售有限公司 02":
                    tmpPath = tmpPath + "00000002201808150034.pdf";
                    break;
                case "宜昌宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150040.pdf";
                    break;
                case "宜春宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150041.pdf";
                    break;
                case "云南瑞合福德汽车服务有限公司":
                    tmpPath = tmpPath + "00000002201808150038.pdf";
                    break;
                case "湛江正通凯迪汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150047.pdf";
                    break;
                case "长沙瑞宝汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150045.pdf";
                    break;
                case "长沙瑞宝汽车销售服务有限公司韶山路分公司":
                    tmpPath = tmpPath + "00000002201808150045.pdf";
                    break;
                case "郑州奥泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150025.pdf";
                    break;
                case "郑州鼎沃汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150036.pdf";
                    break;
                case "中山中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150068.pdf";
                    break;
                case "珠海宝泽汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150067.pdf";
                    break;
                case "珠海宝泽汽车销售服务有限公司MINI":
                    tmpPath = tmpPath + "00000002201808150067.pdf";
                    break;
                case "珠海中汽南方捷路汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150049.pdf";
                    break;
                case "珠海中汽南方汽车销售服务有限公司":
                    tmpPath = tmpPath + "00000002201808150067.pdf";
                    break;
                default:
                    break;
            }
            mt.PdfPath = tmpPath;
            return mt;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}