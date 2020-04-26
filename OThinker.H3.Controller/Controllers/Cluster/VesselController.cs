using OThinker.H3.Controllers.ViewModels.Cluster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Cluster
{
    public class VesselController : ClusterController
    {

        /// <summary>
        /// 获取集群列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetVesselList()
        {
            return ExecuteFunctionRun(() =>
            {
                System.Data.DataTable table = this.Connection.GetVesselConfigTable();
                List<VesselViewModel> list = new List<VesselViewModel>();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        int port = int.Parse(row[OThinker.Clusterware.VesselConfig.PropertyName_Port] + string.Empty);
                        int loadWeight = int.Parse(row[OThinker.Clusterware.VesselConfig.PropertyName_LoadWeight] + string.Empty);
                        int masterOrder = int.Parse(row[OThinker.Clusterware.VesselConfig.PropertyName_MasterOrder] + string.Empty);
                        string currentState = (row[OThinker.Clusterware.VesselConfig.ColumnName_CurrentState] + string.Empty == "1") ? "运行状态" : "挂起状态";
                        VesselViewModel model = new VesselViewModel()
                        {
                            Code = row[OThinker.Clusterware.VesselConfig.PropertyName_Code] + string.Empty,
                            Address = row[OThinker.Clusterware.VesselConfig.PropertyName_Address] + string.Empty,
                            Port = port,
                            LoadWeight = loadWeight,
                            Order = masterOrder,
                            CurrentState = currentState
                        };
                        list.Add(model);
                    }
                }
                return Json(CreateLigerUIGridData(list.OrderBy(s=>s.Order).ToArray()), JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除服务器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DeleteVessel(string codes)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] codeArray = codes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                ActionResult result = new ActionResult(false);
                if (codeArray != null)
                {
                    foreach (string code in codeArray)
                    {
                        this.Connection.RemoveVesselConfig(code);
                    }
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        [HttpPost]
        public JsonResult SaveVessel(VesselViewModel model) {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult(true);
                if (string.IsNullOrEmpty(model.Code)) {
                    result.Success = false;
                    result.Message = "服务编码不可为空!";
                }
                else if (string.IsNullOrEmpty(model.Address))
                {
                    result.Success = false;
                    result.Message = "服务器IP不可为空!";
                }
                if(!result.Success){
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                // TODO:待判定是否重复
                OThinker.Clusterware.VesselConfig config = new Clusterware.VesselConfig()
                {
                    Address = model.Address,
                    Code =model.Code,
                    Description = model.Description,
                    LoadWeight = model.LoadWeight,
                    MasterOrder = model.Order,
                    ObjectUri = Configs.ProductInfo.EngineUri,
                    Port = model.Port
                };
                if (this.Connection.AddVesselConfig(config))
                {
                    result.Success = true;
                }
                else
                {
                    result.Message = "添加失败";
                }
                return Json(result,JsonRequestBehavior.AllowGet);
            });
        }
    }
}
