using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    
    public class AdapterController : ControllerBase
    {
        public override string FunctionCode
        {
            get
            {
                return Acl.FunctionNode.BizBus_ListBizAdapter_Code;
            }
        }

        public AdapterController() { }

        /// <summary>
        /// 上传适配器文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult UploadAdapterAssembly(AdapterViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                ActionResult result = new ActionResult(true, "msgGlobalString.ImportSucced");

                if (files.Count > 0)
                {
                    string fileName = files[0].FileName;
                    if (Path.GetExtension(fileName).ToLower() != ".dll")
                    {
                        result.Success = false;
                        result.Message = "Adapter.Msg0";
                        return Json(result);
                    }

                    this.Engine.BizBus.RegisterAssembly(
                        files[0].FileName,
                        GetBytesFromStream(files[0].InputStream),
                        true);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载适配器列表
        /// </summary>
        /// <param name="adapterType">适配器类型(0:业务服务,1:业务规则)</param>
        /// <returns></returns>
        public JsonResult LoadBizAdapterList(OThinker.H3.BizBus.BizAdapters.AdapterType adapterType = OThinker.H3.BizBus.BizAdapters.AdapterType.BizService,bool isGrid = true)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.H3.BizBus.BizAdapters.BizAdapterPropertyAttribute[] adapters =
                    this.Engine.BizBus.GetBizAdapterAttributes(adapterType);

                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
                Dictionary<string, object> data;

                List<OThinker.H3.BizBus.BizAdapters.BizAdapterPropertyAttribute> lstSortAdapters = adapters.ToList().OrderBy(x => x.SortKey).ToList();
                foreach (OThinker.H3.BizBus.BizAdapters.BizAdapterPropertyAttribute a in lstSortAdapters)
                {
                    data = new Dictionary<string, object>();
                    data.Add(OThinker.H3.BizBus.BizAdapters.BizAdapterPropertyAttribute.PropertyName_DisplayName, a.DisplayName);
                    data.Add("Description", a.Description);
                    data.Add("Code", a.Code);
                    dataList.Add(data);
                }
                var griddata = CreateLigerUIGridData(dataList.ToArray());
                if (isGrid)
                {
                    return Json(griddata);
                }
                else {
                    List<Item> httpMethods = GetHttpMethods();
                    ActionResult result = new ActionResult()
                    {
                        Success = true,
                        Extend = new
                        {
                            GridData = griddata,
                            HttpMethods = httpMethods
                        }

                    };
                    return Json(result);
                }
            });
        }

        /// <summary>
        /// 获取HttpMethods的类型
        /// </summary>
        /// <returns>HttpMethod类型</returns>
        private List<Item> GetHttpMethods()
        {
            List<Item> list = new List<Item>();

            string[] httpMethods = Enum.GetNames(typeof(BizServiceHttpMethod));

            foreach (var s in httpMethods)
            {
                list.Add(new Item(s, s));
            }
            return list;

        }
    }
}
