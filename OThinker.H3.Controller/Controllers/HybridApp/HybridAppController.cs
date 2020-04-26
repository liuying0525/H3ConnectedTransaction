using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.HybridApp;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.HybridApp
{
    /// <summary>
    /// 移动应用菜单控制器
    /// </summary>
    public class HybridAppController : ControllerBase
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_HybridApp_Code; }
        }

        /// <summary>
        /// 设置是否显示轮播图
        /// </summary>
        /// <param name="isDispaly">是否显示</param>
        /// <returns>null</returns>
        [HttpGet]
        public JsonResult SetSlideShowDisplay(bool isDisplay)
        {
            return ExecuteFunctionRun(() =>
            {
                if (this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_SlideShowDisplay, isDisplay.ToString()))
                    return Json(new ActionResult(true, "msgGlobalString.SetSuccess"), JsonRequestBehavior.AllowGet);
                else return Json(new ActionResult(false, "msgGlobalString.SetFailed"), JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取移动应用信息
        /// </summary>
        /// <param name="appCode">编码</param>
        /// <returns>移动应用信息</returns>
        [HttpGet]
        public JsonResult GetHybridApp(string userId, string appCode)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = this.Engine.Organization.GetAllAdministrators()[0].ObjectID;
                //userId = this.UserValidator.UserID; 
            }
            ActionResult result = new ActionResult(false);
            //是否显示
            bool slideShowDisplay = false;
            Boolean.TryParse(this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SlideShowDisplay), out slideShowDisplay);

            result.Success = true;
            result.Extend = new
            {
                SlideShows = this.GetDisplaySlideShows(userId),
                AppFunctionNodes = this.GetDisplayFunctionNodes(userId),
                SlideShowDisplay = slideShowDisplay,
                Badge = new
                {
                    unfinishedworkitem = GetUserUnfinishedWorkItemCount(userId) > 99 ? "99+" : GetUserUnfinishedWorkItemCount(userId).ToString(),
                    unreadworkitem = GetUserUnReadWorkItemCount(userId) > 99 ? "99+" : GetUserUnReadWorkItemCount(userId).ToString()
                }
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取轮播图列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>轮播图列表</returns>
        [HttpPost]
        public JsonResult GetSlideShowsList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                var SlideShowsList = this.GetSlideShows(this.UserValidator.UserID);
                int total = SlideShowsList.Count();
                var list = SlideShowsList.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取移动应用节点列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>轮播图列表</returns>
        [HttpPost]
        public JsonResult GetAppFunctionNodesList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                var AppFunctionNodes = this.GetFunctionNodes();
                int total = AppFunctionNodes.Count();
                var list = AppFunctionNodes.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToList();
                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取一个轮播图信息
        /// </summary>
        /// <param name="code">轮播图编码</param>
        /// <param name="parentId">父ID</param>
        /// <returns>轮播图</returns>
        [HttpGet]
        public JsonResult GetSlideShow(string userId, string code, string parentCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                var parentNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(parentCode);
                var slideShow = this.Engine.HybridAppManager.GetSlideShow(code);
                if (string.IsNullOrEmpty(code))
                {
                    result.Extend = new SlideShowViewModel();
                }
                else if (null == parentNode)
                {
                    result.Message = "HybridApp.ParentNodeNotExist";
                    result.Success = false;
                }
                else if (null == slideShow)
                {
                    result.Message = "HybridApp.SlideNotExits";
                    result.Success = false;
                }
                else
                {
                    SlideShowViewModel model = new SlideShowViewModel()
                    {
                        ObjectID = slideShow.ObjectID,
                        ParentCode = slideShow.ParentCode,
                        Code = slideShow.SlideCode,
                        Description = slideShow.Description,
                        IsDisplay = slideShow.IsDisplay,
                        Url = slideShow.Url,
                        SortKey = slideShow.SortKey,
                        Image = GetImageUrl(userId, SlideShow.TableName, slideShow)
                    };
                    result.Extend = model;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 添加轮播图
        /// </summary>
        /// <returns>是否添加成功</returns>
        [HttpPost]
        public JsonResult SaveSlideShow(SlideShowViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                var parentNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(model.ParentCode);
                ActionResult result = SlideShowValidate(model);
                // 检查文件大小
                if (files.Count > 0 && files[0].ContentLength > 1024 * 1024)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.ImageMaxLength";
                    return Json(result, "text/html");
                }
                else if (null == parentNode)
                {
                    result.Message = "HybridApp.ParentNodeNotExist";
                    result.Success = false;
                    return Json(result, "text/html");
                }
                else if (result.Success == false)
                    return Json(result, JsonRequestBehavior.AllowGet);

                SlideShow slideShow;
                //新建
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    string guid = Guid.NewGuid().ToString();
                    this.SaveImage(files, guid, parentNode.Code);
                    slideShow = new SlideShow()
                    {
                        SlideCode = model.Code,
                        SortKey = model.SortKey,
                        IsDisplay = model.IsDisplay,
                        Description = model.Description,
                        Url = model.Url,
                        ObjectID = guid,
                        ParentCode = model.ParentCode,
                    };

                    if (!this.Engine.HybridAppManager.AddSlideShow(slideShow))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.SaveFailed";

                    }
                }
                else
                {
                    //编辑
                    slideShow = this.Engine.HybridAppManager.GetSlideShow(model.Code);
                    slideShow.IsDisplay = model.IsDisplay;
                    slideShow.SortKey = model.SortKey;
                    slideShow.Url = model.Url;
                    slideShow.Description = model.Description;
                    if (this.Engine.HybridAppManager.UpdateSlideShow(slideShow))
                    {
                        this.SaveImage(files, slideShow.ObjectID, parentNode.ObjectID);
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.SaveFailed";
                    }
                }

                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除轮播图
        /// </summary>
        /// <param name="code">轮播图编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelSlideShow(string code)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                SlideShow slideShow = this.Engine.HybridAppManager.GetSlideShow(code);
                if (null == slideShow)
                {
                    result.Message = "HybridApp.SlideNotExits";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //删除轮播图,再删除轮播图的附件图片
                if (this.Engine.HybridAppManager.RemoveSlideShow(slideShow.SlideCode))
                {
                    this.Engine.BizObjectManager.RemoveAttachment(this.UserValidator.UserID, SlideShow.TableName, slideShow.ParentObjectID, slideShow.ObjectID);
                    result.Success = true;
                    result.Message = "msgGlobalString.DeleteSucced";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.Message = "msgGlobalString.DeleteFailed";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            });
        }

        /// <summary>
        /// 获取应用节点信息
        /// </summary>
        /// <param name="code">应用节点编码</param>
        /// <returns>应用节点信息</returns>
        [HttpGet]
        public JsonResult GetAppFunctionNode(string code)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                var appNode = this.Engine.HybridAppManager.GetAppFunctionNode(code);
                AppFunctionNodeViewModel model = null;
                var openMethods = GetOpenMethods();
                if (null != appNode)
                {
                    model = new AppFunctionNodeViewModel()
                    {
                        IconUrl = appNode.IconUrl,
                        SortKey = appNode.SortKey,
                        Url = appNode.Url,
                        DisplayName = appNode.DisplayName,
                        IsDisplay = appNode.IsDisplay,
                        Description = appNode.Description,
                        Code = appNode.AppFunctionNodeCode,
                        ParentID = appNode.ParentCode,
                        ObjectID = appNode.ObjectID,
                        AppOpenMethod = appNode.AppOpenMethod.ToString()
                    };
                }
                else
                {
                    model = new AppFunctionNodeViewModel()
                    {
                        AppOpenMethod = openMethods.FirstOrDefault().Text,
                        IsDisplay = true
                    };
                }
                result.Extend = new
                {
                    AppFunctionNode = model,
                    OpenMethods = openMethods
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 保存应用节点
        /// </summary>
        /// <param name="model">应用节点模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveAppFunctionNode(AppFunctionNodeViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = FunctionNodeValidate(model);
                if (result.Success == false)
                    return Json(result, JsonRequestBehavior.AllowGet);

                //新建
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    AppFunctionNode appNode = new AppFunctionNode()
                    {
                        ParentCode = model.ParentID,
                        Description = model.Description,
                        IconUrl = model.IconUrl,
                        DisplayName = model.DisplayName,
                        AppFunctionNodeCode = model.Code,
                        Url = model.Url,
                        AppOpenMethod = (AppOpenMethod)Enum.Parse(typeof(AppOpenMethod), model.AppOpenMethod),
                        SortKey = model.SortKey,
                        IsDisplay = model.IsDisplay
                    };

                    if (!this.Engine.HybridAppManager.AddAppFunctionNode(appNode))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.SaveFailed";

                    }

                }
                else
                {
                    //编辑
                    AppFunctionNode appNode = this.Engine.HybridAppManager.GetAppFunctionNode(model.Code);
                    appNode.Description = model.Description;
                    appNode.IconUrl = model.IconUrl;
                    appNode.DisplayName = model.DisplayName;
                    appNode.Url = model.Url;
                    appNode.SortKey = model.SortKey;
                    appNode.AppOpenMethod = (AppOpenMethod)Enum.Parse(typeof(AppOpenMethod), model.AppOpenMethod);
                    appNode.IsDisplay = model.IsDisplay;
                    if (!this.Engine.HybridAppManager.UpdateAppFunctionNode(appNode))
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.SaveFailed";

                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 验证应用节点字段信息
        /// </summary>
        /// <param name="model">应用节点模型</param>
        /// <returns>是否验证成功</returns>
        private ActionResult FunctionNodeValidate(AppFunctionNodeViewModel model)
        {
            ActionResult result = new ActionResult(true);
            if (string.IsNullOrEmpty(model.Code))
            {
                result.Success = false;
                result.Message = "HybridApp.CodeNotNull";
            }
            else if (string.IsNullOrEmpty(model.DisplayName))
            {
                result.Success = false;
                result.Message = "HybridApp.DisplayNameNotNull";
            }
            return result;
        }
        /// <summary>
        /// 删除应用节点
        /// </summary>
        /// <param name="code">应用节点编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelAppFunctionNode(string code)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                AppFunctionNode appFunctionNode = this.Engine.HybridAppManager.GetAppFunctionNode(code);
                if (null == appFunctionNode)
                {
                    result.Message = "HybridApp.FunctionNodeNotExits";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (this.Engine.HybridAppManager.RemoveAppFunctionNode(appFunctionNode.AppFunctionNodeCode))
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.DeleteSucced";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.Message = "msgGlobalString.DeleteFailed";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            });
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="files">图片文件</param>
        /// <param name="slideShowId">轮播图ID</param>
        /// <param name="parentNodeId">父节点ID</param>
        /// <returns>图片地址</returns>
        private void SaveImage(HttpFileCollectionBase files, string slideShowId, string parentNodeId)
        {
            //保存用户头像图片
            if (files.Count > 0)
            {
                string filePath = files[0].FileName;
                if (string.IsNullOrEmpty(filePath)) { return; }

                string fileName = TrimHtml(Path.GetFileName(filePath)).ToLowerInvariant();

                //已经存在的图片
                AttachmentHeader header = this.Engine.BizObjectManager.GetAttachmentHeader(SlideShow.TableName, parentNodeId, slideShowId);
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
                        ObjectID = slideShowId,
                        BizObjectId = parentNodeId,
                        BizObjectSchemaCode = SlideShow.TableName,
                        FileName = fileName,
                        Content = GetBytesFromStream(files[0].InputStream),
                        ContentLength = (int)files[0].ContentLength,
                        CreatedBy = this.UserValidator.UserID,
                        ModifiedBy = this.UserValidator.UserID
                    };
                    this.Engine.BizObjectManager.AddAttachment(attach);
                }
            }
        }

        /// <summary>
        /// 轮播图数据验证
        /// </summary>
        /// <param name="model">轮播图模型</param>
        /// <returns>验证结果</returns>
        private ActionResult SlideShowValidate(SlideShowViewModel model)
        {
            ActionResult result = new ActionResult(true);
            if (string.IsNullOrEmpty(model.Code))
            {
                result.Success = false;
                result.Message = "HybridApp.CodeNotNull";
            }
            return result;
        }

        /// <summary>
        /// 获取图片地址
        /// </summary>
        /// <param name="bizObjectSchemaCode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetImageUrl(string userId, string bizObjectSchemaCode, SlideShow model)
        {
            string imagePath = "";
            AttachmentHeader header = this.Engine.BizObjectManager.GetAttachmentHeader(SlideShow.TableName, model.ParentCode, model.ObjectID);
            if (header != null)
            {
                imagePath = this.PortalRoot + "/ReadAttachment/Read?SchemaCode=" + header.BizObjectSchemaCode + "&BizObjectID=" + header.BizObjectId + "&AttachmentID=" + header.AttachmentID + "&UserID=" + userId + "&IsMobile=true&OpenMethod=0";
                try
                {
                    string UserID = string.Empty;
                    string BizObjectSchemaCode = string.Empty;
                    string BizObjectID = string.Empty;
                    string AttachmentID = string.Empty;
                    string ImageUrl = "http://127.0.0.1:8010" + imagePath.Replace(" ", "");
                    Uri uri = new Uri(ImageUrl);
                    string queryString = uri.Query.Replace("?", "");
                    if (!string.IsNullOrEmpty(queryString))
                    {
                        string[] pars = queryString.Split('&');
                        for (int j = 0; j < pars.Count(); j++)
                        {
                            string[] p_v = pars[j].Split('=');
                            if (p_v[0] == "UserID")
                                UserID = p_v[1];
                            else if (p_v[0] == "SchemaCode")
                                BizObjectSchemaCode = p_v[1];
                            else if (p_v[0] == "BizObjectID")
                                BizObjectID = p_v[1];
                            else if (p_v[0] == "AttachmentID")
                                AttachmentID = p_v[1];
                        }
                    }
                    Attachment attachment = this.Engine.BizObjectManager.GetAttachment(
                                               UserID,
                                               BizObjectSchemaCode,
                                               BizObjectID,
                                               AttachmentID);
                    if (attachment == null || attachment.Content == null || attachment.Content.Length == 0)
                    { }
                    else
                    {
                        //保存文件到服务器
                        string fileName = attachment.ObjectID + Path.GetExtension(attachment.FileName) + ".jpg";
                        string imgUrl = this.PortalRoot + "/TempImages/AppSlides/" + Engine.EngineConfig.Code + "//" + fileName;
                        string savePath = System.AppDomain.CurrentDomain.BaseDirectory + "TempImages//AppSlides//" + Engine.EngineConfig.Code + "//" + fileName;
                        // 获取图片路径
                        if (System.IO.File.Exists(savePath))
                        {
                            FileInfo file = new FileInfo(savePath);
                            if (file.LastWriteTime > attachment.ModifiedTime)
                            {
                                imagePath = imgUrl;
                            }
                            else
                            {
                                try { file.Delete(); }
                                catch { }
                            }
                        }
                        try
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                            using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    bw.Write(attachment.Content);
                                    bw.Close();
                                }
                            }
                            imagePath = imgUrl;
                        }
                        catch { }
                    }
                }
                catch { }
            }
            return imagePath;
        }

        /// <summary>
        /// 将图片输出到临时目录并显示
        /// </summary>
        /// <param name="bizObjectSchemaCode"></param>
        /// <param name="model"></param>
        /// <param name="tempImages"></param>
        /// <returns></returns>
        private string GetImage(string userId, string bizObjectSchemaCode, SlideShow model, string tempImages)
        {

            // 以下是获取用户图片
            string fileName = Path.Combine("SlideShow//" + Engine.EngineConfig.Code + "//" + model.ParentCode, model.ObjectID + ".jpg");
            string savePath = Path.Combine(tempImages, fileName);

            // 获取图片路径
            if (System.IO.File.Exists(savePath))
            {
                FileInfo file = new FileInfo(savePath);
                if (file.LastWriteTime > model.ModifiedTime)
                {
                    return fileName;
                }
                else
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                    }
                }
            }

            // 生成图片
            Attachment image = null;
            try
            {
                image = Engine.BizObjectManager.GetAttachment(userId,
                       SlideShow.TableName,
                        model.ParentCode,
                        model.ObjectID);

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
        /// <summary>
        /// 获取轮播图
        /// </summary>
        /// <returns>轮播图列表</returns>
        private List<SlideShowViewModel> GetSlideShows(string userId)
        {
            var slideShows = this.Engine.HybridAppManager.GetAllSlidesShow();
            List<SlideShowViewModel> list = slideShows.Select(s => new SlideShowViewModel
            {
                Code = s.SlideCode,
                Description = s.Description,
                IsDisplay = s.IsDisplay,
                SortKey = s.SortKey,
                ParentCode = s.ParentCode,
                Url = s.Url,
                Image = GetImageUrl(userId, SlideShow.TableName, s)
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取应用节点
        /// </summary>
        /// <returns>应用节点列表</returns>
        private List<AppFunctionNodeViewModel> GetFunctionNodes()
        {
            var functionNodes = this.Engine.HybridAppManager.GetAllAppFunctionNode();
            List<AppFunctionNodeViewModel> list = functionNodes.Select(s => new AppFunctionNodeViewModel
            {
                Code = s.AppFunctionNodeCode,
                AppOpenMethod = s.AppOpenMethod.ToString(),
                DisplayName = s.DisplayName,
                Description = s.Description,
                SortKey = s.SortKey,
                IconUrl = s.IconUrl,
                Url = s.Url,
                IsDisplay = s.IsDisplay
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取需要显示的轮播图
        /// </summary>
        /// <returns>轮播图列表</returns>
        public List<SlideShowViewModel> GetDisplaySlideShows(string userId)
        {
            List<SlideShowViewModel> list = GetSlideShows(userId).Where(s => s.IsDisplay == true).ToList();
            return list;
        }


        //待办任务数量
        public int GetUserUnfinishedWorkItemCount(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return 0; }
            int recordCounts;
            // 构造查询用户帐号的条件
            string[] conditions = Query.GetWorkItemConditions(
                userId,
                true,
                H3.WorkItem.WorkItemState.Unfinished,
                H3.WorkItem.WorkItemType.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified);

            // 获取总记录数，计算页码         
            recordCounts = this.Engine.Query.CountWorkItem(conditions);
            return recordCounts;
        }

        //待阅读任务数量
        public int GetUserUnReadWorkItemCount(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return 0; }
            int recordCounts;
            // 构造查询用户帐号的条件
            string[] conditions = this.Engine.Query.GetWorkItemConditions(
                userId,
                H3.WorkItem.WorkItem.PropertyName_ReceiveTime,
                DateTime.MinValue,
                DateTime.MaxValue,
                H3.WorkItem.WorkItemState.Unfinished,
                "",
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                H3.Instance.InstanceState.Unspecified,
                "",
                true,
                true);

            // 获取总记录数     
            recordCounts = this.Engine.PortalQuery.CountWorkItem(conditions, OThinker.H3.WorkItem.CirculateItem.TableName);
            return recordCounts;
        }

        /// <summary>
        /// 获取应用的打开方式
        /// </summary>
        /// <returns></returns>
        private List<Item> GetOpenMethods()
        {
            List<Item> list = new List<Item>();
            var appOpenMethods = Enum.GetNames(typeof(AppOpenMethod));
            if (appOpenMethods == null) return null;
            list = appOpenMethods.Select(s => new Item()
            {
                Text = s,
                Value = ((int)Enum.Parse(typeof(AppOpenMethod), s)).ToString()
            }).ToList();
            return list;
        }

        /// <summary>
        /// 返回APP，只显示IsDisplay=true,并按排序值排序
        /// 如果个数不满4的倍数，需要填充空的数据
        /// </summary>
        /// <returns></returns>
        public List<AppFunctionNodeViewModel> GetDisplayFunctionNodes(string userId)
        {
            List<AppFunctionNodeViewModel> list = GetFunctionNodes().Where(f => f.IsDisplay == true).OrderBy(f => f.SortKey).ToList();
            foreach (AppFunctionNodeViewModel model in list)
            {
                if (model.Url.ToLower().Contains("unfinishedworkitem"))
                {
                    model.EdgeNum = 0;//GetUserUnfinishedWorkItemCount(userId);
                    model.EdgeCode = "unfinishedworkitem";
                }
                else if (model.Url.ToLower().Contains("unreadworkitem"))
                {
                    model.EdgeNum = 0;// GetUserUnReadWorkItemCount(userId);
                    model.EdgeCode = "unreadworkitem";
                }
                else
                {
                    //edgeNUm==0时不显示
                    model.EdgeNum = 0;
                }
            }

            //返回list的数量需要是4的整数倍，不够增加空值
            int yushu = list.Count % 3;
            if (yushu > 0)
            {
                int left = 3 - yushu;
                for (int i = 0; i < left; i++)
                {
                    list.Add(new AppFunctionNodeViewModel()
                    {
                        Code = "",
                        DisplayName = "",
                        IsDisplay = true,
                        EdgeNum = 0,
                        Url = "",
                        IconUrl = ""
                    });
                }
            }
            int arrLength = (list.Count / 20) + 1;
            return list;
        }
    }
}
