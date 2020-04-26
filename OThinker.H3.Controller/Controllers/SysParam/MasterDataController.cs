using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 数据字典控制器
    /// </summary>
    [Authorize]
    public class MasterDataController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.ProcessModel_BizMasterData_Code; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MasterDataController() { }

        /// <summary>
        /// 获取页面初始化数据
        /// </summary>
        /// <returns>页面初始化数据</returns>
        [HttpGet]
        public JsonResult GetMastePageData()
        {
            return ExecuteFunctionRun(() =>
            {
                var result = new
                {
                    category = GetCategoryList()
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取列表显示数据
        /// </summary>
        /// <param name="id">分类ID</param>
        /// <returns>带分页的数据字典数据</returns>
        [HttpPost]
        public JsonResult GetMasterDataList(string id, PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.H3.Data.EnumerableMetadata[] data = this.Engine.MetadataRepository.GetByCategory(id);
                int total = data == null ? 0 : data.Length;
                // TODO:注意data可能为null对象
                List<MasterDataViewModel> list = new List<MasterDataViewModel>();
                if (data != null)
                    list = data.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).Select(s => new MasterDataViewModel
                     {
                         ObjectID = s.ObjectID,
                         Category = s.Category,
                         Code = s.Code,
                         DisplayText = s.EnumValue,
                         IsDefault = s.IsDefault,
                         SortKey = s.SortKey
                     }).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取一个数据字典
        /// </summary>
        /// <param name="id">数据字典id</param>
        /// <returns>数据字典模型</returns>
        [HttpGet]
        public JsonResult GetMasterData(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                //返回全局变量实例
                var item = this.Engine.MetadataRepository.GetById(id);// .GlobalDataManager.GetItem(ItemName);
                MasterDataViewModel model = null;
                if (item != null)
                {
                    model = new MasterDataViewModel()
                    {
                        ObjectID = item.ObjectID,
                        Category = item.Category,
                        Code = item.Code,
                        DisplayText = item.EnumValue,
                        IsDefault = item.IsDefault,
                        SortKey = item.SortKey
                    };
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存数据字典
        /// </summary>
        /// <param name="model">数据字典模型</param>
        /// <returns>1:失败;0:成功</returns>
        [HttpPost]
        public JsonResult SaveMasterData(MasterDataViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                OThinker.H3.Data.EnumerableMetadata data = null;
                bool success = false;
                string msg = string.Empty;
                if (!string.IsNullOrEmpty(model.ObjectID))
                {
                    data = this.Engine.MetadataRepository.GetById(model.ObjectID);
                    data.SortKey = model.SortKey;
                    data.EnumValue = model.DisplayText;
                    data.IsDefault = model.IsDefault;
                    success = this.Engine.MetadataRepository.Update(data);
                }
                else
                {
                    //判断对象是否已经存在
                    if (this.Engine.MetadataRepository.GetByCode(model.Category, model.Code) != null)
                    {
                        success = false;
                        msg = "msgGlobalString.CodeDuplicate";
                    }
                    else
                    {
                        data = new OThinker.H3.Data.EnumerableMetadata()
                        {
                            Category = model.Category,
                            Code = model.Code,
                            EnumValue = model.DisplayText,
                            SortKey = model.SortKey,
                            IsDefault = model.IsDefault
                        };
                        success = this.Engine.MetadataRepository.Add(data);
                    }
                }
                result.Success = success;
                if (!success)
                {
                    result.Message = (msg == string.Empty ? "msgGlobalString.SaveFailed" : msg);
                }
                else
                {
                    var category = GetCategoryList();
                    result.Extend = new
                    {
                        data = category,
                        initValue = model.Category,
                        initText = model.Category
                    };
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除数据字典
        /// </summary>
        /// <param name="ids">数据字典IDS</param>
        /// <returns>失败:错误消息;成功:空对象</returns>
        [HttpPost]
        public JsonResult DelMasterData(string ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ids = ids.TrimEnd(',');
                string[] idArr = ids.Split(',');
                try
                {
                    foreach (string id in idArr)
                    {
                        this.Engine.MetadataRepository.Remove(id);
                    }
                    result.Success = true;
                    var category = GetCategoryList();
                    result.Extend = new
                    {
                        data = category
                    };
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 更改默认项状态
        /// </summary>
        /// <param name="id">默认项ID</param>
        /// <param name="isDefault">默认项状态</param>
        /// <returns>0:成功;1:失败</returns>
        [HttpPost]
        public JsonResult ChangeMasterDataDefault(string id, bool isDefault)
        {

            return ExecuteFunctionRun(() =>
            {
                bool success = this.Engine.MetadataRepository.SetDefaultItem(id, isDefault);
                object result = new { };
                if (success)
                {
                    result = new { stat = 0 };
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 导入数据字典xls
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadMasterDataFile()
        {
            return ExecuteFunctionRun(() =>
            {
                var files = Request.Files;
                var length = files.Count;
                bool success = false;
                List<ImportError> msg = new List<ImportError>();
                foreach (string file in Request.Files)
                {
                    string fileName = Request.Files[file].FileName;
                    //文件名为空
                    if (string.IsNullOrEmpty(fileName))
                    {
                        msg.Add(new ImportError
                        {
                            Error = "msgGlobalString.Import_NotUploaded",
                            Seq = 0
                        });
                        break;
                    }
                    string fileType = Path.GetExtension(Common.TrimHtml(Path.GetFileName(fileName))).ToLowerInvariant();
                    //文件格式错误
                    if (!fileType.Replace(".", "").Equals("xls"))
                    {
                        msg.Add(new ImportError
                        {
                            Error = "msgGlobalString.Import_InvilidateFile",
                            Seq = 0
                        });
                        break;
                    }
                    if (Request.Files[file].FileName == "") { }
                    try
                    {
                        Stream s = Request.Files[file].InputStream;
                        success = SaveUplodStream(s, out msg);
                    }
                    catch (Exception e)
                    {
                        msg.Add(new ImportError
                        {
                            Error = e.Message,
                            Seq = 0
                        });
                    }
                };
                ActionResult result = new ActionResult(success, string.Empty, msg);
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 将数据字典文件流保存到数据库
        /// </summary>
        /// <param name="inputStream">数据字典文件流</param>
        /// <param name="msg">错误消息</param>
        /// <returns>true:成功;false:失败</returns> 
        private bool SaveUplodStream(Stream inputStream, out List<ImportError> msg)
        {

            DataTable dt = Common.ExcelToDataTable(inputStream);
            List<Data.EnumerableMetadata> masterDataList = new List<Data.EnumerableMetadata>();
            int seq = 0;
            msg = new List<ImportError>();
            foreach (DataRow dr in dt.Rows)
            {
                seq++;
                string category = dr["类别"].ToString().Trim();
                string value = dr["文本"].ToString().Trim();
                string code = dr["编码"].ToString().Trim();
                bool isDefault = dr["是否默认"].ToString().Trim() == "是" || dr["是否默认"].ToString().Trim() == "1" || dr["是否默认"].ToString().Trim().ToLowerInvariant() == "true";
                int sortKey;
                if (!Int32.TryParse(dr["排序号"].ToString().Trim(), out sortKey))
                {
                    //this.ShowWarningMessage(string.Format(this.PortalResource.GetString("ImportMasterData_SortKeyMssg"), seq));
                    msg.Add(new ImportError
                    {
                        Error = "msgGlobalString.Import_SortKeyMsg",
                        Seq = seq
                    });
                    return false;
                }

                if (masterDataList.Any(p => p.Code == code)
                    || this.Engine.MetadataRepository.GetByCode(category, code) != null)
                {
                    msg.Add(new ImportError
                    {
                        Error = "msgGlobalString.Import_CodeRepeat",
                        Seq = seq
                    });
                    return false;
                }

                masterDataList.Add(new Data.EnumerableMetadata()
                {
                    Category = category,
                    Code = code,
                    EnumValue = value,
                    SortKey = sortKey,
                    IsDefault = isDefault
                });
            }

            seq = 0;
            foreach (Data.EnumerableMetadata data in masterDataList)
            {
                seq++;
                //if (!this.Engine.MetadataRepository.Save(data))
                if (!this.Engine.MetadataRepository.Add(data))
                {
                    msg.Add(new ImportError
                    {
                        Seq = seq,
                        Error = "msgGlobalString.Import_SaveFailed"
                    });
                }
            }
            return msg.Count > 0 ? false : true;
        }

        /// <summary>
        /// 获取数据字典分类
        /// </summary>
        /// <returns>数据字典分类</returns>
        private object GetCategoryList()
        {
            Dictionary<string, string> table = this.Engine.MetadataRepository.GetCategoryTable();
            // 分类列表数据
            var category = table.Select(s => new
            {
                text = s.Value,
                id = s.Key
            });
            return category;
        }
    }
}
