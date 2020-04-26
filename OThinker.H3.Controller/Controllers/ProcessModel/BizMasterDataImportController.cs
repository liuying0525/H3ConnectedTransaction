using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 导入主数据控制器
    /// </summary>
    [Authorize]
    public class BizMasterDataImportController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.ProcessModel_BizMasterData_Code; }
        }

        //private string masterDataCode = string.Empty;
        //private string masterDataName = string.Empty;
        private DataModel.BizObjectSchema bizObjectSchema;
        private DataModel.BizListenerPolicy bizListenerPolicy;
        private List<DataModel.ScheduleInvoker> scheduleInvokerList;
        private List<DataModel.BizQuery> bizQueryList;


        public JsonResult Import(BizMasterDataImportViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);

                string newMasterDataCode = model.MasterDataCode;

                try
                {
                    ReadXmlFile(model);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result);
                }


                model.MasterDataCode = newMasterDataCode;

                if (bizObjectSchema == null)
                {
                    result.Success = false;
                    result.Message = "EditMasterData.MasterData_Mssg5";
                    return Json(result);
                }


                if (string.IsNullOrEmpty(model.MasterDataCode))
                {
                    result.Success = false;
                    result.Message = "EditMasterData.MasterData_Mssg6";
                    return Json(result);
                }

                bool isSuccess = false;
                string errorMsg = "msgGlobalString.ImportFail";
                BizObjectSchema record = this.Engine.BizObjectManager.GetDraftSchema(model.MasterDataCode);
                if (record == null)
                {
                    //数据模型
                    bizObjectSchema.SchemaCode = model.MasterDataCode;
                    isSuccess = this.Engine.BizObjectManager.AddDraftSchema(bizObjectSchema);
                    //监听实例
                    this.Engine.BizObjectManager.SetListenerPolicy(model.MasterDataCode, bizListenerPolicy);
                    //添加成功，这里还是需要写入FuntionNode节点的，为了跟流程包里的数据模型节点区别
                    FunctionNode node = new FunctionNode(bizObjectSchema.SchemaCode, bizObjectSchema.DisplayName
                        , "", "", FunctionNodeType.BizObject, model.ParentCode, 0);
                    this.Engine.FunctionAclManager.AddFunctionNode(node);
                }
                else
                {
                    bool isCover = model.IsCover;
                    if (isCover)
                    {
                        //数据模型
                        isSuccess = this.Engine.BizObjectManager.UpdateDraftSchema(bizObjectSchema);
                        //监听实例
                        this.Engine.BizObjectManager.SetListenerPolicy(model.MasterDataCode, bizListenerPolicy);
                        //更新FuntionNode
                        FunctionNode funcNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(model.MasterDataCode);
                        funcNode.DisplayName = bizObjectSchema.DisplayName;
                        this.Engine.FunctionAclManager.UpdateFunctionNode(funcNode);
                    }
                    else { errorMsg = "EditMasterData.MasterData_Mssg8"; }
                }

                //定时作业
                if (scheduleInvokerList != null && scheduleInvokerList.Count > 0)
                {
                    //先删除
                    ScheduleInvoker[] oldScheduleInvokers = this.Engine.BizObjectManager.GetScheduleInvokerList(model.MasterDataCode);
                    if (oldScheduleInvokers != null)
                    {
                        foreach (var scheduleInvoker in oldScheduleInvokers)
                        {
                            this.Engine.BizObjectManager.RemoveScheduleInvoker(scheduleInvoker.ObjectID);
                        }
                    }
                    //再新增
                    foreach (var scheduleInvoker in scheduleInvokerList)
                    {
                        scheduleInvoker.ObjectID = Guid.NewGuid().ToString();
                        scheduleInvoker.SchemaCode = model.MasterDataCode;
                        scheduleInvoker.Serialized = false;
                        this.Engine.BizObjectManager.AddScheduleInvoker(scheduleInvoker);
                    }
                }
                //查询列表
                if (bizQueryList != null && bizQueryList.Count > 0)
                {
                    //先删除
                    BizQuery[] oldBizQueries = this.Engine.BizObjectManager.GetBizQueries(model.MasterDataCode);
                    if (oldBizQueries != null)
                    {
                        foreach (var bizQuery in oldBizQueries)
                        {
                            this.Engine.BizObjectManager.RemoveBizQuery(bizQuery);
                        }
                    }
                    //后新增
                    foreach (var bizQuery in bizQueryList)
                    {
                        bizQuery.ObjectID = Guid.NewGuid().ToString();
                        bizQuery.SchemaCode = model.MasterDataCode;
                        bizQuery.Serialized = false;
                        if (bizQuery.Columns != null)
                        {
                            foreach (BizQueryColumn col in bizQuery.Columns)
                            {
                                col.Serialized = false;
                                col.ObjectID = Guid.NewGuid().ToString();
                            }
                        }
                        if (bizQuery.BizActions != null)
                        {
                            foreach (BizQueryAction col in bizQuery.BizActions)
                            {
                                col.Serialized = false;
                                col.ObjectID = Guid.NewGuid().ToString();

                            }
                        }
                        if (bizQuery.QueryItems != null)
                        {
                            foreach (BizQueryItem col in bizQuery.QueryItems)
                            {
                                col.Serialized = false;
                                col.ObjectID = Guid.NewGuid().ToString();
                            }
                        }
                        this.Engine.BizObjectManager.AddBizQuery(bizQuery);
                    }
                }

                if (isSuccess)
                {
                    //从缓存中移除文件内容
                    this.Session[model.XmlSessionName] = null;
                    result.Message = "msgGlobalString.ImportSucceed";
                }
                else
                {
                    result.Message = errorMsg;
                    result.Success = false;
                }

                return Json(result);
            });

        }

        /// <summary>
        /// 上传主数据文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                BizMasterDataImportViewModel model = new BizMasterDataImportViewModel();
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;//传输的文件

                if (files == null || files.Count == 0 || string.IsNullOrEmpty(files[0].FileName))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
                var file = files[0];

                string fileType = Path.GetExtension(TrimHtml(Path.GetFileName(file.FileName))).ToLowerInvariant();
                if (!fileType.Replace(".", "").Equals("xml"))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.FileMustIsXML";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                string xmlStr = string.Empty;
                using (StreamReader sr = new StreamReader(file.InputStream))
                {
                    xmlStr = sr.ReadToEnd();
                }
                string newName = Guid.NewGuid().ToString() + fileType;
                model.XmlSessionName = newName;
                //将文件内容存放在缓存中
                this.Session[newName] = xmlStr;

                //验证是否为伪造的xml文件
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(this.Session[model.XmlSessionName].ToString());
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                try
                {
                    ReadXmlFile(model);
                }
                catch
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }


                //界面控制
                result.Success = true;
                result.Message = "";
                result.Extend = model;
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);

            });

        }

        private void ReadXmlFile(BizMasterDataImportViewModel model)
        {
            //从服务器加载
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(this.Session[model.XmlSessionName].ToString());
            //根节点
            XmlElement bizMasterData = xmlDoc.DocumentElement;

            //数据模型
            XmlNodeList bizObjectSchemaNodes = bizMasterData.GetElementsByTagName("BizObjectSchema");
            if (bizObjectSchemaNodes != null)
            {
                bizObjectSchema = Convertor.XmlToObject(typeof(BizObjectSchema), bizObjectSchemaNodes[0].OuterXml) as BizObjectSchema;
            }

            //主数据编码、名称
            if (bizObjectSchema != null)
            {
                model.MasterDataCode = bizObjectSchema.SchemaCode;
                model.MasterDataName = bizObjectSchema.DisplayName;
            }

            //监听实例
            XmlNodeList bizListenerPolicyNodes = bizMasterData.GetElementsByTagName("BizListenerPolicy");
            if (bizListenerPolicyNodes != null && bizListenerPolicyNodes.Count > 0)
            {
                bizListenerPolicy = Convertor.XmlToObject(typeof(BizListenerPolicy), bizListenerPolicyNodes[0].OuterXml) as BizListenerPolicy;
            }

            //定时作业
            XmlNodeList scheduleInvokerNodes = bizMasterData.GetElementsByTagName("ScheduleInvokers");
            if (scheduleInvokerNodes != null && scheduleInvokerNodes.Count > 0)
            {
                scheduleInvokerList = new List<ScheduleInvoker>();
                foreach (XmlNode scheduleInvoker in scheduleInvokerNodes[0].ChildNodes)
                {
                    scheduleInvokerList.Add(Convertor.XmlToObject(typeof(ScheduleInvoker), scheduleInvoker.OuterXml) as ScheduleInvoker);
                }
            }

            //查询列表
            XmlNodeList bizQueryNodes = bizMasterData.GetElementsByTagName("BizQueries");
            if (bizQueryNodes != null && bizQueryNodes.Count > 0)
            {
                bizQueryList = new List<BizQuery>();
                foreach (XmlNode query in bizQueryNodes[0].ChildNodes)
                {
                    bizQueryList.Add(Convertor.XmlToObject(typeof(BizQuery), query.OuterXml) as BizQuery);
                }
            }
        }
    }
}
