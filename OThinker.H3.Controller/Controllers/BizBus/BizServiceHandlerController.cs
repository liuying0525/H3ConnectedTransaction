using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    /// <summary>
    /// 业务服务导入导出控制器
    /// </summary>
    public class BizServiceHandlerController:ControllerBase
    {
        public BizServiceHandlerController() { }

        public override string FunctionCode
        {
            get { return FunctionNode.BizBus_BizService_Code; }
        }

        /// <summary>
        ///  // 根据Code判断服务是否存在
        /// 如果不存在，则调用 AddBizService 方法
        /// 如果存在
        ///    判断非覆盖模式，则提示已经存在，不可导入
        ///    如果覆盖模式
        ///          并且ObjectID不同，把当前导入对象的ObjectID改为服务器的ObjectID，执行更新；
        ///          如果ObjectID相同，那么直接执行更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult ImportService(BizServiceHandlerViewModel model)
        {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult();
                BizService bizService = null;
                var newCode = model.ServiceCode;
                try
                {
                    ReadXmlFile(ref model,out bizService);
                    //
                    model.ServiceCode = newCode;
                    
                }
                catch
                {
                    result.Success = false;
                    result.Message = "BizServiceHandler.ServiceImport_Mssg2";
                    return Json(result);
                }
             
                if (string.IsNullOrEmpty(model.ServiceCode))
                {
                    result.Success = false;
                    result.Message = "BizServiceHandler.ServiceImport_Mssg3";
                    return Json(result);
                }

                bool isSuccess = false;
                string errorMsg = "msgGlobalString.ImportFail";
                BizService record = this.Engine.BizBus.GetBizService(model.ServiceCode);
                bizService.FolderCode = model.ParentCode;
                if (record == null)
                {
                    bizService.ObjectID = Guid.NewGuid().ToString();
                    bizService.Serialized = false;
                    bizService.Code = model.ServiceCode;
                    bizService.Serialized = false;
                    bizService.UsedCount = 0; // 将被引用次数重置为0

                    if (bizService.Settings != null)
                    {
                        foreach (BizServiceSetting setting in bizService.Settings)
                        {
                            setting.ObjectID = Guid.NewGuid().ToString();
                            setting.Serialized = false;
                        }
                    }

                    if (bizService.Methods != null)
                    {
                        foreach (BizServiceMethod method in bizService.Methods)
                        {
                            method.ObjectID = Guid.NewGuid().ToString();
                            method.Serialized = false;
                        }
                    }
                    isSuccess = this.Engine.BizBus.AddBizService(bizService, true).Valid;
                }
                else
                {
                    bool isCover = false;
                    bool.TryParse(model.Cover, out isCover);
                    if (isCover)
                    {
                        bizService.ObjectID = record.ObjectID;
                        isSuccess = this.Engine.BizBus.UpdateBizService(bizService, true).Valid;
                    }
                    else { errorMsg = "BizServiceHandler.ServiceImport_Mssg4"; }
                }

                if (isSuccess)
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.ImportSucceed";

                    Session[model.XMLString] = null;//清空Session
                }
                else
                {
                    result.Success = false;
                    result.Message = errorMsg;
                }
                return Json(result);
            });
        }

        /// <summary>
        /// 上传业务服务文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload()
        {
            return ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;//传输的文件

                ActionResult result = new ActionResult();
                BizServiceHandlerViewModel model = new BizServiceHandlerViewModel();
                
                if (files == null || files.Count==0 || string.IsNullOrEmpty(files[0].FileName))
                {
                  
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
                string fileType = Path.GetExtension(TrimHtml(Path.GetFileName(files[0].FileName))).ToLowerInvariant();
                if (!fileType.Replace(".", "").Equals("xml"))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.FileMustIsXML";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                //将文件内容存放在缓存中
                string xmlStr = string.Empty;
                using (StreamReader sr = new StreamReader(files[0].InputStream))
                {
                    xmlStr = sr.ReadToEnd();
                }
                string newName = Guid.NewGuid().ToString() + fileType;
               
                model.FileName = newName;
                model.XMLString = Guid.NewGuid().ToString(); //SessionName
                Session[model.XMLString] = xmlStr;//存储XML字符串
               

                //验证是否为伪造的xml文件
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlStr);
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }

                BizService bizService=null;
                try
                {
                    ReadXmlFile(ref model, out bizService); //从XML中读取服务名称和编码
                }
                catch {
                    result.Success = false;
                    result.Message = "msgGlobalString.InvilidateFile";
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
               

                //界面控制，前台控制
                result.Success = true;
                result.Message = "";
                result.Extend = model;
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }

        public Object DownLoad(string serviceCode) {
            return ExecuteFileResultFunctionRun(() => {

                XmlDocument XmlDoc = new XmlDocument();

                XmlElement bizService = XmlDoc.CreateElement(FunctionNodeType.BizService.ToString());
                XmlDoc.AppendChild(bizService);

                BizService bizServiceRecord = this.Engine.BizBus.GetBizService(serviceCode);
                bizService.InnerXml += Convertor.ObjectToXml(bizServiceRecord);

                //导出文件
                string path = Server.MapPath("~/TempImages/");
                string fileName = serviceCode + ".xml";

                XmlDoc.Save(path + fileName);

                return File(path + fileName, "application/octect-stream",fileName);
            });
        }

        private void ReadXmlFile(ref BizServiceHandlerViewModel model,out BizService bizService)
        {
            //从服务器加载
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Session[model.XMLString].ToString());
            //根节点
            XmlElement root = xmlDoc.DocumentElement;

            //数据模型
            XmlNode Node = root.GetElementsByTagName("BizService")[0];
            bizService = (BizService)Convertor.XmlToObject(typeof(BizService), Node.OuterXml);

            //业务服务编码、名称
            model.ServiceCode = bizService.Code;
            model.ServiceName = bizService.DisplayName;
        }
    }
}
