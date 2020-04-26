using OThinker.H3.Acl;
using OThinker.H3.Controllers.Controllers.Admin.Handler;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 参数映射控制器
    /// </summary>
    [Authorize]
    public class ServiceMethodParamMapController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        #region 参数
        protected OThinker.H3.DataModel.BizObjectSchema Schema;
        private string SelectedMethod;
        private int SelectedMapIndex;
        private bool IsParam = false;
        private string ParamName = null;
        private OThinker.H3.DataModel.ServiceMethodMap SelectedMap
        {
            get
            {
                if (this.SelectedMapIndex == -1)
                {
                    return null;
                }
                else
                {
                    return this.Schema.GetMethod(this.SelectedMethod).MethodMaps[this.SelectedMapIndex];
                }
            }
        }
        #endregion

        /// <summary>
        /// 解析变量
        /// </summary>
        /// <param name="mapIndex">参数行号</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="method">方法</param>
        /// <param name="methodType">绑定方法类型</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="isParam">是否参数</param>
        /// <returns>解析结果</returns>
        private ActionResult ParseParam(string mapIndex, string schemaCode, string method, string methodType, string paramName, bool isParam)
        {
            string code = HttpUtility.UrlDecode(schemaCode);
            ActionResult result = new ActionResult();
            if (string.IsNullOrEmpty(code))
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaParamMap_Msg0";
                return result;

            }
            else if (string.IsNullOrEmpty(mapIndex))
            {
                result.Success = false;
                result.Message = "msgGlobalString.SaveFirst";
                return result;
            }
            this.Schema = this.Engine.BizObjectManager.GetDraftSchema(code);
            if (this.Schema == null)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaParamMap_Msg0";
                //业务对象模式不存在，或者已经被删除
                return result;
            }

            //验权
            //this.ValidateBiztreeNodeModify(BizTreeNodeType.BizObject, code);

            this.SelectedMethod = HttpUtility.UrlDecode(method);

            string strIndex = HttpUtility.UrlDecode(mapIndex);
            if (string.IsNullOrEmpty(strIndex))
            {
                this.SelectedMapIndex = -1;
            }
            else
            {
                this.SelectedMapIndex = int.Parse(strIndex);
            }
            this.IsParam = isParam;
            this.ParamName = HttpUtility.UrlDecode(paramName);
            result.Success = true;
            return result;

        }

        /// <summary>
        /// 获取参数映射信息
        /// </summary>
        /// <param name="mapIndex">参数行号</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="method">方法</param>
        /// <param name="methodType">绑定方法类型</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="isParam">是否参数</param>
        /// <returns>参数映射信息</returns>
        [HttpGet]
        public JsonResult GetServiceMethodParamMap(string mapIndex, string schemaCode, string method, string methodType, string paramName, bool isParam)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(mapIndex, schemaCode, method, methodType, paramName, isParam);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                PropertyTreeHandler proTreeHandler = new PropertyTreeHandler(this);
                SchemaMethodParamMapViewModel model = GetSchemaMethodParamMapViewModel();
                model.MapIndex = mapIndex;
                model.SchemaCode = schemaCode;
                model.Method = method;
                result.Extend = new
                {
                    PropertyTree = proTreeHandler.GetPropertyTree(schemaCode),
                    ParamMap = model,
                    MapTypes = new List<Item>() {
                    new Item("Property","0"),
                    new Item("Const","1")
                    }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存参数映射信息
        /// </summary>
        /// <param name="model">参数映射信息</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public JsonResult SaveSErviceMethodParamMap(SchemaMethodParamMapViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.MapIndex, model.SchemaCode, model.Method, model.MethodType, model.ParamName, model.IsParam);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                H3.DataModel.DataMapType mapType = (H3.DataModel.DataMapType)int.Parse(model.MapType);
                string mapTo = null;
                switch (mapType)
                {
                    case H3.DataModel.DataMapType.Property:
                        if (model.PropertyName != null)
                        {
                            mapTo = model.PropertyName;// this.lstProperty.SelectedValue;
                            if (mapTo.IndexOf("[") > -1)
                            {
                                mapTo = mapTo.Substring(mapTo.IndexOf("[") + 1);
                            }
                            if (mapTo.IndexOf("]") > -1)
                            {
                                mapTo = mapTo.Substring(0, mapTo.IndexOf("]"));
                            }
                        }
                        break;
                    case H3.DataModel.DataMapType.Const:
                        mapTo = model.ConstValue;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (this.IsParam)
                {
                    this.SelectedMap.ParamMaps.SetMap(this.ParamName, mapType, mapTo);
                }
                else
                {
                    this.SelectedMap.ReturnMaps.SetMap(this.ParamName, mapType, mapTo);
                }

                // 保存
                if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    // 保存失败
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取参数映射信息
        /// </summary>
        /// <returns>参数映射信息</returns>
        private SchemaMethodParamMapViewModel GetSchemaMethodParamMapViewModel()
        {
            SchemaMethodParamMapViewModel model = new SchemaMethodParamMapViewModel();
            // 参数
            OThinker.H3.DataModel.DataMap map = null;
            if (this.IsParam)
            {
                map = this.SelectedMap.ParamMaps.GetMap(this.ParamName);
            }
            else
            {
                map = this.SelectedMap.ReturnMaps.GetMap(this.ParamName);
            }
            if (map == null)
            {
                return model;
            }

            model.ParamName = this.ParamName;
            model.IsParam = this.IsParam;
            model.MapType = ((int)map.MapType).ToString();
            switch (map.MapType)
            {
                case H3.DataModel.DataMapType.Property:
                    model.PropertyName = map.MapTo;
                    model.ConstValue = "";
                    break;
                case H3.DataModel.DataMapType.Const:
                    model.ConstValue = map.MapTo;
                    break;
                case H3.DataModel.DataMapType.None:
                    model.ConstValue = "";
                    break;
                default:
                    throw new NotImplementedException();
            }

            return model;
        }


    }
}
