using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Notification;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 常规参数控制器
    /// </summary>
    [Authorize]
    public class CommonParamController : ControllerBase
    {

        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_CommonParam_Code; }
        }

        /// <summary>
        /// 获取页面初始化参数
        /// </summary>
        /// <returns>页面初始化参数</returns>
        [HttpGet]
        public JsonResult GetCommonParam()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                FileServer fileServer = this.Engine.FilePolicyManager.GetDefaultFileServer();
                //系统设置模型
                CommonParamViewModel model = new CommonParamViewModel()
                {
                    Password = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UserInitialPassword),
                    AgencyIdentity = CustomSetting.GetAgencyIdentityType(Engine.SettingManager).ToString(),
                    ExceptionManager = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ExceptionManager),
                    SequenceNoModal = CustomSetting.GetSequenceNoModal(Engine.SettingManager).ToString(),
                    SequenceNoDateFormat = CustomSetting.GetSequenceNoDateFormat(Engine.SettingManager).ToString(),
                    SequenceNoOrder = CustomSetting.GetSequenceNoOrder(Engine.SettingManager).ToString(),
                    SequenceNoLength = CustomSetting.GetSequenceNoLength(Engine.SettingManager).ToString(),
                    SequenceNoResetType = CustomSetting.GetGlobalSeqResetType(this.Engine.SettingManager).ToString(),
                    NextInstanceID = GetNextInstanceID(),
                    OvertimeCheckInterval = CustomSetting.GetOvertimeCheckInterval(this.Engine.SettingManager).ToString(),
                    RemoteHubAddress = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemoteStubAddress),
                    WikiUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WikiUrl),
                    StorageMethod = ((int)fileServer.StorageType).ToString(),
                    ClientMethod = ((int)fileServer.DownloadType).ToString(),
                    SQLPool = fileServer.DbCode,
                    FTPServer = fileServer.ServerHost,
                    FTPServerPort = fileServer.ServerPort.ToString() ?? "21",
                    FTPAcount = fileServer.Account,
                    FTPPassWord = fileServer.Password,
                    FTPUrl = fileServer.DownloadUrl,
                    BasePath = fileServer.BasePath,
                    CorpID = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatCorpID),
                    Secret = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatSecret),
                    MessageSecret = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatMessageSecret),
                    WeChatProID = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatAgentId),
                    WeChatMsgUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_PortalUrl),
                    AppKey = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_JPushAppKey),
                    MasterSecret = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_JPushMasterSecret),
                    AppName = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_JPushAppName),
                    DDCorpID = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDCorpID),
                    DDMsgUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDUrl),
                    DDPcMsgUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDPcUrl),
                    DDProID = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAgentId),
                    DDSecret = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDSecret)

                };
                //流水号的日期格式
                List<Item> sequenceNoDateFormats = InitSequenceNoDateFormats();
                //流水号的模式
                List<Item> sequenceNoModals = InitSequenceNoModals();
                //委托身份
                List<Item> agencyIdentitys = InitAgencyIdentitys();
                //流水号的顺序
                List<Item> sequenceNoOrders = InitSequenceNoOrders();
                //流水号的长度
                List<Item> sequenceNoLengths = InitSequenceNoLengths();
                //流水号的重置策略
                List<Item> sequenceNoResetTypes = InitRestTypes();
                //用户选择的方式
                List<Item> clientMethods = InitClientMethods();
                //存储策略
                List<Item> storageMethods = InitStorageMethods();
                //数据库连接池
                List<Item> sqlPools = InitSqlPools();
                result.Extend = new
                {
                    CommonParam = model,
                    SequenceNoDateFormats = sequenceNoDateFormats,
                    SequenceNoModals = sequenceNoModals,
                    AgencyIdentitys = agencyIdentitys,
                    SequenceNoOrders = sequenceNoOrders,
                    SequenceNoLengths = sequenceNoLengths,
                    SequenceNoResetTypes = sequenceNoResetTypes,
                    StorageMethods = storageMethods,
                    ClientMethods = clientMethods,
                    SQLPools = sqlPools
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取钉钉应用信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDingAppInfo()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                string attachmentId = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDMediaId);

                string imagePath = "";
                imagePath = GetImage(DingTalkAdapter.Param_DingTalkCode, attachmentId, Server.MapPath(this.PortalRoot + @"/tempImages/")).Replace("//", "/");
                if (imagePath != "")
                {
                    imagePath = this.PortalRoot + "/tempImages/" + imagePath + "?T=" + DateTime.Now.ToString("yyyyMMddhhmmss");
                }
                result.Extend = new DingAppViewModel
                {
                    AppName = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAppName) ?? "",
                    AppDesc = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAppDesc) ?? "",
                    HomeUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAppHomeUrl) ?? "",
                    PcUrl = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAppPcUrl) ?? "",
                    OmpLink = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDOmpLink) ?? "",
                    AgentId = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAppAgentId) ?? "",
                    ImageUrl = imagePath ?? ""

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 重置引擎连接
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ResetEngine(string connectionString)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                AppConfig.ResetEngine(connectionString);
                result.Message = connectionString;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存钉钉微应用信息并发布
        /// </summary>
        /// <param name="model">微应用信息</param>
        /// <returns>发布结果</returns>
        [HttpPost]
        public JsonResult PublishDingApp(DingAppViewModel model)
        {
            ActionResult result;
            //数据验证
            result = ValideDingApp(model);
            System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
            if (files.Count > 0 && files[0].ContentLength > 1024 * 1024)
            {
                result.Success = false;
                result.Message = "msgGlobalString.ImageMaxLength";
                return Json(result, "text/html");
            }
            //获取钉钉微应用头像附件ID
            string attchmentId = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDMediaId);
            if (files.Count > 0 && !string.IsNullOrEmpty(files[0].FileName))
            {
                //ID为空则新建
                attchmentId = attchmentId ?? Guid.NewGuid().ToString();
                SaveImage(files, attchmentId, attchmentId);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDMediaId, attchmentId);
            }
            if (string.IsNullOrEmpty(attchmentId))
            {
                result.Success = false;
                result.Message = "msgGlobalString.ImgNotEmpty";
                return Json(result, "text/html");
            }
            //更新附件中的DingTalkMediaID
            this.Engine.DingTalkAdapter.GetAttachmentDingTalkViewUrl(this.UserValidator.UserID, DingTalkAdapter.Param_DingTalkCode, attchmentId, attchmentId);
            Attachment attachment = this.Engine.BizObjectManager.GetAttachment(this.UserValidator.UserID, DingTalkAdapter.Param_DingTalkCode, attchmentId, attchmentId);
            if (string.IsNullOrEmpty(attachment.DingTalkMediaID))
            {
                result.Success = false;
                result.Message = "msgGlobalString.ImgNotEmpty";
                return Json(result, "text/html");
            }
            //钉钉微应用设置
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAppName, model.AppName);
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAppDesc, model.AppDesc);
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAppHomeUrl, model.HomeUrl);
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAppPcUrl, model.PcUrl);
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDOmpLink, model.OmpLink);
            DingTalkMicroApp dingApp = this.Engine.DingTalkAdapter.CreateMicroApp(attachment.DingTalkMediaID, model.AppName, model.AppDesc, model.HomeUrl, model.PcUrl, model.OmpLink);
            if (dingApp.errcode != "0")
            {
                result.Success = false;
                result.Message = "msgGlobalString.PublishFailed";
            }
            else
            {
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAppAgentId, dingApp.agentid);
            }
            result.Extend = dingApp;
            return Json(result, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证钉钉微应用数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult ValideDingApp(DingAppViewModel model)
        {
            ActionResult result = new ActionResult(true);
            return result;
        }

        /// <summary>
        /// ftp验证
        /// </summary>
        /// <param name="model">Ftp信息模型</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult FtpValidate(CommonParamViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Success = FtpValidateResult(model);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// FTP信息验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool FtpValidateResult(CommonParamViewModel model)
        {
            FileServer fileServer = this.Engine.FilePolicyManager.GetDefaultFileServer();
            // 检查数据库连接
            OThinker.H3.DataModel.FileStorageType storageType = (OThinker.H3.DataModel.FileStorageType)int.Parse(model.StorageMethod);
            OThinker.H3.DataModel.FileDownloadType downloadType = (OThinker.H3.DataModel.FileDownloadType)int.Parse(model.ClientMethod);
            fileServer.StorageType = storageType;
            string password = model.FTPPassWord;
            if (!string.IsNullOrEmpty(password))
            {
                fileServer.Password = password;
            }

            if (storageType == FileStorageType.FTP)
            {
                FtpWebRequest ftpRequest;
                try
                {
                    if (!string.IsNullOrEmpty(model.FTPServerPort))
                        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + model.FTPServer.Trim() + ":" + model.FTPServerPort.Trim() + "/"));
                    else
                        ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + model.FTPServer.Trim() + "/"));

                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    ftpRequest.UseBinary = true;
                    ftpRequest.Credentials = new NetworkCredential(model.FTPAcount, fileServer.Password);
                    FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                }
                catch
                {
                    return false;

                }
            }
            return true;
        }

        /// <summary>
        /// 流程的顺序号
        /// </summary>
        /// <returns></returns>
        private string GetNextInstanceID()
        {
            string nextId = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NextInstanceSeqID);
            int seqNo = 0;
            if (!int.TryParse(nextId, out seqNo))
            {
                seqNo = 1;
            }
            return seqNo.ToString();
        }

        /// <summary>
        /// 初始化数据库池
        /// </summary>
        /// <returns></returns>
        private List<Item> InitSqlPools()
        {
            List<Item> list = new List<Item>();
            H3.Settings.BizDbConnectionConfig[] configs = this.Engine.SettingManager.GetBizDbConnectionConfigList();
            if (configs != null)
            {
                foreach (H3.Settings.BizDbConnectionConfig config in configs)
                {
                    config.DisplayName = string.IsNullOrEmpty(config.DisplayName) ? config.DbCode : config.DisplayName;
                    list.Add(new Item() { Text = config.DisplayName, Value = config.DbCode });
                }
            }
            return list;
        }

        /// <summary>
        /// 初始化存储策略
        /// </summary>
        /// <returns></returns>
        private List<Item> InitStorageMethods()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item() { Text = "CommonParam.StorageMethods.DataBase", Value = "0" });
            list.Add(new Item() { Text = "CommonParam.StorageMethods.Ftp", Value = "2" });
            return list;
        }

        /// <summary>
        /// 初始化客户打开方式
        /// </summary>
        /// <returns></returns>
        private List<Item> InitClientMethods()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item() { Text = "CommonParam.ClientMethods.Server", Value = "0" });
            list.Add(new Item() { Text = "CommonParam.ClientMethods.Direct", Value = "1" });
            return list;
        }

        /// <summary>
        /// 流水号的日期格式
        /// </summary>
        /// <returns>日期格式列表</returns>
        private List<Item> InitSequenceNoDateFormats()
        {
            List<Item> list = new List<Item>();
            string[] sequenceNoDateNames = Enum.GetNames(typeof(Instance.SequenceNoDateFormat));
            foreach (string name in sequenceNoDateNames)
            {
                list.Add(new Item() { Text = name, Value = name });
            }
            return list;
        }

        /// <summary>
        /// 流水号的模式
        /// </summary>
        /// <returns></returns>
        private List<Item> InitSequenceNoModals()
        {
            // 流水号的模式
            List<Item> list = new List<Item>();
            string[] sequenceNoModalNames = Enum.GetNames(typeof(Instance.SequenceNoModal));
            foreach (string name in sequenceNoModalNames)
            {
                list.Add(new Item() { Text = "CommonParam.SequenceNoModals." + name, Value = name });
            }
            return list;
        }

        /// <summary>
        /// 初始化委托身份
        /// </summary>
        /// <returns>委托身份选项列表</returns>
        private List<Item> InitAgencyIdentitys()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item() { Text = "CommonParam.AgencyIdentitys.Agent", Value = "Agent" });
            list.Add(new Item() { Text = "CommonParam.AgencyIdentitys.Source", Value = "Source" });
            return list;
        }

        /// <summary>
        /// 初始化流水号排序
        /// </summary>
        /// <returns>流水号排序选项列表</returns>
        private List<Item> InitSequenceNoOrders()
        {
            List<Item> list = new List<Item>();
            string[] sequenceNoOrders = Enum.GetNames(typeof(Instance.SequenceNoOrder));
            foreach (string name in sequenceNoOrders)
            {
                string text = "CommonParam.SequenceNoOrders." + name;
                list.Add(new Item { Text = text, Value = name });
            }
            return list;
        }

        /// <summary>
        /// 初始化流水号长度
        /// </summary>
        /// <returns>流水号长度选项列表</returns>
        private List<Item> InitSequenceNoLengths()
        {
            List<Item> list = new List<Item>();
            // 流水号的长度
            for (int count = 4; count < 15; count++)
            {
                list.Add(new Item { Text = count.ToString(), Value = count.ToString() });
            }
            return list;

        }
        /// <summary>
        /// 初始化流水号重置模式
        /// </summary>
        /// <returns>流水号重置选项列表</returns>
        private List<Item> InitRestTypes()
        {
            List<Item> list = new List<Item>();
            string[] seqNoResetTypes = Enum.GetNames(typeof(OThinker.H3.Instance.SequenceNoResetType));
            foreach (string name in seqNoResetTypes)
            {
                string text = "DataSettings." + name;
                list.Add(new Item { Text = text, Value = name });
            }
            return list;
        }

        /// <summary>
        /// 保存系统常规参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveCommonParam(CommonParamViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (OThinker.H3.DataModel.FileStorageType.FTP == (OThinker.H3.DataModel.FileStorageType)int.Parse(model.StorageMethod))
                {
                    if (string.IsNullOrEmpty(model.FTPServer))
                    {
                        result.Success = false;
                        result.Message = "CommonParam.FTPInfoRequired";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (!FtpValidateResult(model) && false)
                        {
                            result.Success = false;
                            result.Message = "msgGlobalString.FtpValidateFailed";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                if (model.ClientMethod == "1" && string.IsNullOrEmpty(model.FTPUrl))
                {
                    result.Success = false;
                    result.Message = "CommonParam.FTPUrlIsEmpty";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                bool saveStorage = SaveStorageInfo(model);
                //新用户初始密码设置
                if (!string.IsNullOrWhiteSpace(model.Password))
                    Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_UserInitialPassword, model.Password.Trim());
                // 流水号的方式
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoModal, model.SequenceNoModal);
                // 流水号的日期格式
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoDateFormat, model.SequenceNoDateFormat);
                // 流水号的长度
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoLength, model.SequenceNoLength);
                // 流水号的顺序
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoOrder, model.SequenceNoOrder);
                // 流水号的重置策略
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_GlobalSeqNoResetType, model.SequenceNoResetType);

                // 异常管理员
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ExceptionManager, model.ExceptionManager);

                // 委托身份,默认为AgencyIdentityType.Source
                AgencyIdentityType agencyIdentity = AgencyIdentityType.Source;
                CustomSetting.SetAgencyIdentityType(Engine.SettingManager, agencyIdentity);

                // 任务超时检查的时间间隔
                CustomSetting.SetOvertimeCheckInterval(Engine.SettingManager, System.TimeSpan.Parse(model.OvertimeCheckInterval));
                //内网集成服务器的地址
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemoteStubAddress, model.RemoteHubAddress);

                //帮助站点url
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WikiUrl,
                   string.IsNullOrEmpty(model.WikiUrl) ? "" : model.WikiUrl.Trim());

                //极光推送设置
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_JPushAppKey, model.AppKey);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_JPushMasterSecret, model.MasterSecret);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_JPushAppName, model.AppName);

                //微信设置
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatCorpID, model.CorpID);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatSecret, model.Secret);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatMessageSecret, model.MessageSecret);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatAgentId, model.WeChatProID);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_PortalUrl, model.WeChatMsgUrl);

                //钉钉设置
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDCorpID, model.DDCorpID);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDSecret, model.DDSecret);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAgentId, model.DDProID);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDUrl, model.DDMsgUrl);
                Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDPcUrl, model.DDPcMsgUrl);

                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存存储策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true:成功;false:失败</returns>
        private bool SaveStorageInfo(CommonParamViewModel model)
        {

            FileServer fileServer = this.Engine.FilePolicyManager.GetDefaultFileServer();
            OThinker.H3.DataModel.FileDownloadType downloadType = (OThinker.H3.DataModel.FileDownloadType)int.Parse(model.ClientMethod);
            OThinker.H3.DataModel.FileStorageType storageType = (OThinker.H3.DataModel.FileStorageType)int.Parse(model.StorageMethod);
            fileServer.DbCode = model.SQLPool;
            fileServer.Account = model.FTPAcount;
            fileServer.ServerHost = model.FTPServer;
            fileServer.ServerPort = string.IsNullOrEmpty(model.FTPServerPort) ? 21 : int.Parse(model.FTPServerPort);
            fileServer.Password = model.FTPPassWord;
            fileServer.DownloadUrl = model.FTPUrl;
            fileServer.DownloadType = downloadType;
            fileServer.StorageType = storageType;
            fileServer.BasePath = model.BasePath ?? "";
            return this.Engine.FilePolicyManager.UpdateFileServer(fileServer);
        }

        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="files">文件</param>
        /// <param name="bizObjectId">业务对象ID</param>
        /// <param name="attachmentId">附件ID</param>
        private void SaveImage(HttpFileCollectionBase files, string bizObjectId, string attachmentId)
        {
            //保存用户头像图片
            if (files.Count > 0)
            {
                string filePath = files[0].FileName;
                string fileName = TrimHtml(Path.GetFileName(filePath)).ToLowerInvariant();

                //已经存在的图片
                AttachmentHeader header = this.Engine.BizObjectManager.GetAttachmentHeader(DingTalkAdapter.Param_DingTalkCode, attachmentId, bizObjectId);
                if (header != null)
                {
                    try
                    {
                        this.Engine.BizObjectManager.UpdateAttachment(header.BizObjectSchemaCode,
                            header.BizObjectId,
                            header.AttachmentID, this.UserValidator.UserID, header.FileName, header.ContentType, GetBytesFromStream(files[0].InputStream), header.FileFlag);
                    }
                    catch { }
                }
                else
                {
                    // 图片采用附件上传方式
                    Attachment attach = new Attachment()
                    {
                        ObjectID = bizObjectId,
                        BizObjectId = attachmentId,
                        BizObjectSchemaCode = DingTalkAdapter.Param_DingTalkCode,
                        FileName = fileName,
                        Content = GetBytesFromStream(files[0].InputStream),
                        ContentLength = (int)files[0].ContentLength,
                        CreatedBy = this.UserValidator.UserID,
                        ModifiedBy = this.UserValidator.UserID,
                        ContentType = files[0].ContentType
                    };
                    this.Engine.BizObjectManager.AddAttachment(attach);
                }
            }
        }


        /// <summary>
        /// 将图片输出到临时目录并显示
        /// </summary>
        /// <param name="bizObjectSchemaCode"></param>
        /// <param name="model"></param>
        /// <param name="tempImages"></param>
        /// <returns></returns>
        private string GetImage(string bizObjectSchemaCode, string attchentId, string tempImages)
        {
            // 以下是获取图片
            string fileName = Path.Combine("DingTalkImage//" + Engine.EngineConfig.Code + "//", attchentId + ".jpg");
            string savePath = Path.Combine(tempImages, fileName);
            // 生成图片
            // 获取图片路径
            if (System.IO.File.Exists(savePath))
            {
                FileInfo file = new FileInfo(savePath);
                try
                {
                    file.Delete();
                }
                catch
                {
                }
            }
            Attachment image = null;
            try
            {
                image = Engine.BizObjectManager.GetAttachment(UserValidator.UserID,
                       bizObjectSchemaCode,
                        attchentId,
                        attchentId);

                if (image != null && image.Content != null && image.Content.Length > 0)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(image.Content);
                            bw.Close();
                            return fileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 这里如果直接输出异常，那么用户不能登录
                Engine.LogWriter.Write("加载用户图片出现异常,UserValidator:" + ex.ToString());
            }

            return string.Empty;
        }
    }
}
