using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 全局变量控制器
    /// </summary>
    [Authorize]
    public class GlobalDataController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysParam_GlobalData_Code;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GlobalDataController() { }

        /// <summary>
        /// 获取全局变量列表
        /// </summary>
        /// <param name="pagerInfo">页码信息</param>
        /// <returns>全局变量分页数据</returns>
        [HttpPost]
        public JsonResult GetGlobalDataList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                //数据量不多,一次性从服务器返回全部进行处理
                Data.PrimitiveMetadata[] items = this.Engine.MetadataRepository.GetAllPrimitiveItems();
                int total = items == null ? 0 : items.Length;
                List<GlobalDataViewModel> list = items.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).Select(s => new GlobalDataViewModel
                {
                    ObjectID = s.ObjectID,
                    ItemName = s.ItemName,
                    ItemValue = s.ItemValue,
                    Description = s.Description
                }).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存全局变量
        /// </summary>
        /// <param name="model">全局变量数据</param>
        /// <returns>成功:true;失败:false</returns>
        [HttpPost]
        public JsonResult SaveGlobalData(GlobalDataViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                //全局变量增加编码规范，不许输入特殊字符，目的是同步java导入流程包时，有特殊字符系统会报错的问题 add qiancheng
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[\u4E00-\u9FA5a-zA-Z0-9_@\\\\.]*$");
                //^[\u4E00-\u9FA5a-zA-Z0-9_@\\\\.]*$
                if (!regex.Match(model.ItemName).Success)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg4";
                    return Json(result, "text/html");
                }
                //TODO 增加新方法 CreatePrimitiveItem(GlobalDataViewModel Model)
                if (string.IsNullOrEmpty(model.ItemName))
                {
                    //保存 Bugger:失败成功都返回0
                    result.Success = this.Engine.MetadataRepository.CreatePrimitiveItem(model.ItemName, model.ItemValue, model.Description);//.GlobalDataManager.CreateItem(itemName, itemDesc, itemValue);                
                }
                else
                {
                    //更新
                    result.Success = this.Engine.MetadataRepository.SetPrimitiveItemValue(model.ItemName, model.ItemValue, model.Description);//.GlobalDataManager.SetItemValue(itemName, itemValue);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取一个全局变量
        /// </summary>
        /// <param name="itemName">全局变量名</param>
        /// <returns>全局变量显示模型</returns>
        [HttpGet]
        public JsonResult GetGlobalData(string itemName)
        {
            return ExecuteFunctionRun(() =>
            {
                //返回全局变量实例
                var item = this.Engine.MetadataRepository.GetPrimitiveItem(itemName);// .GlobalDataManager.GetItem(ItemName);
                GlobalDataViewModel model = null;
                if (item != null)
                {
                    model = new GlobalDataViewModel()
                    {
                        ObjectID = item.ObjectID,
                        ItemName = item.ItemName,
                        ItemValue = item.ItemValue,
                        Description = item.Description
                    };
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除全局变量
        /// </summary>
        /// <param name="ids">全局变量名称</param>
        /// <returns>失败:错误消息;成功:空对象</returns>
        [HttpPost]
        public JsonResult DelGlobalData(string ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ids = ids.TrimEnd(',');
                string[] idArr = ids.Split(',');
                ActionResult result = null;
                try
                {
                    foreach (string id in idArr)
                    {
                        this.Engine.MetadataRepository.RemovePrimitiveItem(id);
                    }
                    result = new ActionResult(true);
                }
                catch (Exception e)
                {
                    result = new ActionResult(false, e.Message);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


    }
}
