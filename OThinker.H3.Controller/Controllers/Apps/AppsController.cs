using OThinker.Data;
using OThinker.H3.Acl;
using OThinker.H3.Apps;
using OThinker.H3.Controllers.ViewModels;
using Newtonsoft.Json;


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OThinker.H3.Controllers.Controllers.Apps
{
    /// <summary>
    /// 应用中心控制器
    /// </summary>
    public class AppsController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_Apps_Code; }
        }

        #region Nodes
        Dictionary<string, FunctionNode> _FunctionDictionary;
        /// <summary>
        /// Code,FunctionNode
        /// </summary>
        Dictionary<string, FunctionNode> FunctionDictionary
        {
            get
            {
                if (this._FunctionDictionary == null)
                {
                    this._FunctionDictionary = new Dictionary<string, FunctionNode>();
                    foreach (FunctionNode f in AllFunctionNodes)
                    {
                        if (!_FunctionDictionary.ContainsKey(f.Code))
                            _FunctionDictionary.Add(f.Code, f);
                    }
                }
                return this._FunctionDictionary;
            }
        }

        Dictionary<string, List<string>> _DicCodeAndChildren;
        /// <summary>
        /// Code,ChildrenCodes
        /// </summary>
        Dictionary<string, List<string>> DicCodeAndChildren
        {
            get
            {
                if (_DicCodeAndChildren == null)
                {
                    _DicCodeAndChildren = new Dictionary<string, List<string>>();
                    foreach (FunctionNode f in AllFunctionNodes)
                    {
                        if (!string.IsNullOrEmpty(f.ParentCode))
                        {
                            if (!_DicCodeAndChildren.ContainsKey(f.ParentCode))
                            {
                                _DicCodeAndChildren.Add(f.ParentCode, new List<string>());
                            }
                            _DicCodeAndChildren[f.ParentCode].Add(f.Code);
                        }
                    }
                }
                return this._DicCodeAndChildren;
            }
        }

        FunctionNode[] _AllFunctionNodes;
        FunctionNode[] AllFunctionNodes
        {
            get
            {
                if (_AllFunctionNodes == null)
                {
                    _AllFunctionNodes = this.Engine.FunctionAclManager.GetFunctionNodes();
                    if (_AllFunctionNodes == null)
                        _AllFunctionNodes = new FunctionNode[] { };
                }
                return _AllFunctionNodes;
            }
        }
        #endregion

        private AppNavigation app = null;
        /// <summary>
        /// 保存应用
        /// </summary>
        /// <param name="model">应用中心模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveApps(AppsViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                AppNavigation App = null;
                if (!string.IsNullOrEmpty(model.ObjectID))
                {
                    App = this.Engine.AppNavigationManager.GetApp(model.AppCode);
                }
                else
                {
                    if (!this.AppsAuthorized)
                    {
                        result.Success = false;
                        result.Message = "Apps.NotAuthorized";
                    }
                    else if (this.Engine.AppNavigationManager.GetApp(model.AppCode) != null || this.Engine.FunctionAclManager.GetFunctionNodeByCode(model.AppCode) != null)
                    {
                        result.Success = false;
                        result.Message = "Apps.CodeExists";
                    }
                    else
                    {
                        result.Success = true;
                    }
                    if (result.Success) App = new AppNavigation();
                }
                if (App != null)
                {
                    App.AppCode = model.AppCode;
                    App.DisplayName = model.DisplayName;
                    App.WeChatID = model.WeChatID;
                    App.IconUrl = model.Image;
                    App.Description = model.Description;
                    App.SortKey = model.SortKey;
                    //App.Url = txtUrl.Text;
                    App.VisibleOnPortal = model.VisibleOnPortal ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    App.DockOnHomePage = model.DockOnHomePage ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    App.VisibleOnMobile = model.VisibleOnMobile ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    if (string.IsNullOrEmpty(model.ObjectID))
                    {
                        result.Success = this.Engine.AppNavigationManager.AddApp(App);
                        App = this.Engine.AppNavigationManager.GetApp(model.AppCode);

                    }
                    else
                    {
                        result.Success = this.Engine.AppNavigationManager.UpdateApp(App);
                    }
                    FunctionNode AppsNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionNode.Category_Apps_Code);
                    if (AppsNode != null)
                        result.Extend = new { AppCode = App.AppCode, ParentId = AppsNode.ObjectID };
                    else result.Extend = new { AppCode = App.AppCode };
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        [HttpPost]
        public JsonResult PublicToDingTalk(string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                if(files==null||files.Count==0)
                {
                    result.Success = false;
                    result.Message = "Apps.FileEmptyError";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if(files[0].ContentLength>100000)
                {
                    result.Success = false;
                    result.Message = "Apps.FileSizeError";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var appmode = this.Engine.AppNavigationManager.GetApp(appCode);
                if (appmode == null)
                {
                    result.Success = false;
                    result.Message = "Apps.NotSaved";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(appmode.DingTalkID))
                {
                    string mediaid = string.Empty;
                    if (files!=null)
                    {
                        mediaid = this.Engine.DingTalkAdapter.UpdateFile(files[0].FileName, files[0].ContentType, files[0].ContentLength, GetBytesFromStream(files[0].InputStream));
                    }
                    //string homeUrl = Request.ApplicationPath + "/Mobile/index.html?showmenu=false&targer=appCenter&state=DefaultEngine&loginfrom=dingtalk&params={appCode:'" + appmode.AppCode + "'}";
                    string homeUrl = DingTalkPulishURL(appmode.AppCode);
                    var appresult = this.Engine.DingTalkAdapter.CreateMicroApp(mediaid, appmode.DisplayName, string.IsNullOrEmpty(appmode.Description)?appmode.DisplayName:appmode.Description, homeUrl, appmode.Url, "");
                    if (!string.IsNullOrEmpty(appresult.agentid))
                    {
                        appmode.DingTalkID = appresult.agentid;
                        this.Engine.AppNavigationManager.UpdateApp(appmode);
                        //权限
                        //var roles = this.Engine.FunctionAclManager.GetFunctionAclByCode(appmode.AppCode);
                    }
                    result.Success = appresult.errcode.Equals("0");
                    result.Message = appresult.errmsg;
                }
                else
                {
                    result.Success = false;
                    result.Message = "apps.wasPubliced";
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 通过编码获得当前应用
        /// </summary>
        /// <param name="appCode">编码</param>
        /// <returns>当前应用</returns>
        [HttpGet]
        public JsonResult GetApps(string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                AppNavigation app = null;
                app = this.Engine.AppNavigationManager.GetApp(appCode);
                if (app == null)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.NullObjectException";
                }
                else
                {
                    AppsViewModel model = new AppsViewModel()
                    {
                        AppCode = app.AppCode,
                        Description = app.Description,
                        DisplayName = app.DisplayName,
                        DockOnHomePage = app.DockOnHomePage == OThinker.Data.BoolMatchValue.True,
                        Image = app.IconUrl,
                        ObjectID = app.ObjectID,
                        SortKey = app.SortKey,
                        VisibleOnPortal = app.VisibleOnPortal == OThinker.Data.BoolMatchValue.True,
                        VisibleOnMobile=app.VisibleOnMobile==OThinker.Data.BoolMatchValue.True,
                        WeChatID = app.WeChatID,
                        DingTalkID=app.DingTalkID
                    };
                    result.Success = true;
                    result.Extend = model;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }



        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="appCode"></param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelApp(string appCode)
        {

            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Success = false;
                AppNavigation app = this.Engine.AppNavigationManager.GetApp(appCode);
                if (app != null)
                {
                    this.Engine.AppNavigationManager.RemoveApp(appCode);
                    result.Success = true;
                    FunctionNode AppsNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionNode.Category_Apps_Code);
                    if (AppsNode != null)
                        result.Extend = AppsNode.ObjectID;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取功能节点列表
        /// </summary>
        /// <param name="appCode">应用编码</param>
        /// <returns>功能节点列表</returns>
        [HttpPost]
        public JsonResult GetAppMenuTree(string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<Dictionary<string, object>> NodeList = new List<Dictionary<string, object>>();
                NodeList.AddRange(GetChilren(appCode, 0));
                var gridObj = new { Rows = NodeList, Total = NodeList.Count };
                result.Extend = gridObj;
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除功能菜单
        /// </summary>
        /// <param name="ids">菜单ID</param>
        /// <param name="appCode">应用编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelAppMenuTree(string ids, string appCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                string[] IDs = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (IDs != null && IDs.Length > 0)
                {
                    foreach (string id in IDs)
                    {
                        this.Engine.FunctionAclManager.RemoveFunctionNode(id, true);
                    }
                }

                //更新App的ModifiedTime
                AppNavigation app = this.Engine.AppNavigationManager.GetApp(appCode);
                if (app != null)
                {
                    this.Engine.AppNavigationManager.UpdateApp(app);
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);

            });
        }

        [HttpGet]
        public void ExportApp(string code)
        {
            string appCode = code;
            if (string.IsNullOrEmpty(appCode))
            {
                return;
            }

            //应用程序
            AppNavigation app = this.Engine.AppNavigationManager.GetApp(appCode);

            //菜单
            List<FunctionNode> functionNodeList = new List<FunctionNode>();
            //一级菜单
            FunctionNode[] functionNodes = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(appCode);
            if (functionNodes != null)
            {
                foreach (FunctionNode functionNode in functionNodes)
                {
                    functionNodeList.Add(functionNode);
                    //二级菜单
                    FunctionNode[] leafNodes = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(functionNode.Code);
                    if (leafNodes != null)
                    {
                        foreach (FunctionNode leafNode in leafNodes)
                        {
                            functionNodeList.Add(leafNode);
                        }
                    }
                }
            }

            //构造XML
            XElement appDoc = new XElement(appCode);
            appDoc.Add(XElement.Parse(Convertor.ObjectToXml(app)));
            foreach (FunctionNode node in functionNodeList)
            {
                appDoc.Add(XElement.Parse(Convertor.ObjectToXml(node)));
            }

            //导出文件
            string path = Server.MapPath("~/TempImages/");
            string fileName = appCode + ".xml";
            appDoc.Save(path + fileName);
            this.Response.Clear();
            this.Response.ContentType = "text/xml";
            this.Response.AppendHeader("Content-Disposition", "attachment;filename=\"" + fileName + "\"");
            this.Response.TransmitFile(path + fileName);
            this.Response.End();
            System.IO.File.Delete(path + fileName);
        }

        [HttpPost]
        public JsonResult ImportApp(string appImportSettingsStr)
        {
            return ExecuteFunctionRun(() =>
            {
                AppImportSettingsViewModel model = JsonConvert.DeserializeObject<AppImportSettingsViewModel>(appImportSettingsStr);
                ActionResult result = new ActionResult();
                string xmlStr = Session[model.FileName] == null ? "" : Session[model.FileName].ToString();
                List<FunctionNode> oldFunctionList = null;
                if (!ReadXmlFile(xmlStr, out oldFunctionList))
                {
                    result.Success = false;
                }
                if (!Validate(model, oldFunctionList, out result))
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    AppNavigation oldApp = this.Engine.AppNavigationManager.GetApp(app.AppCode);
                    if (oldApp == null)
                    {
                        app.ObjectID = Guid.NewGuid().ToString();
                        app.Serialized = false;
                        this.Engine.AppNavigationManager.AddApp(app);
                    }
                    else
                    {
                        if (model.IsCover)
                        {
                            app.ObjectID = oldApp.ObjectID;
                            this.Engine.AppNavigationManager.UpdateApp(app);
                        }
                    }
                    //先删除菜单
                    FunctionNode[] oldNodes = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(app.AppCode);
                    if (oldNodes != null)
                    {
                        foreach (FunctionNode oldNode in oldNodes)
                        {
                            this.Engine.FunctionAclManager.RemoveFunctionNodeByCode(oldNode.Code, true);
                        }
                    }
                    //添加菜单
                    foreach (FunctionNode node in oldFunctionList)
                    {
                        this.Engine.FunctionAclManager.AddFunctionNode(node);
                    }
                    //从缓存中移除文件内容

                    Session.Remove(model.FileName);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "Apps.ImportFail";
                    result.Extend = ex.Message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        [HttpPost]
        public JsonResult UpLoadFile()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                var files = Request.Files;
                if (files == null)
                {
                    result.Success = false;
                    result.Message = "Apps.Msg7";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string fileName = files[0].FileName;
                string fileType = Path.GetExtension(Common.TrimHtml(Path.GetFileName(fileName))).ToLowerInvariant();
                if (!fileType.Replace(".", "").Equals("xml"))
                {
                    result.Success = false;
                    result.Message = "Apps.Msg8";
                    return Json(result, JsonRequestBehavior.AllowGet);

                }

                string xmlStr = string.Empty;
                using (StreamReader sr = new StreamReader(files[0].InputStream))
                {
                    xmlStr = sr.ReadToEnd();
                }
                string newName = Guid.NewGuid().ToString() + fileType;
                //将文件内容存放在缓存中
                Session[newName] = xmlStr;
                List<FunctionNode> functionNodeList;
                try
                {
                    bool readState = ReadXmlFile(xmlStr, out functionNodeList);
                    if (readState)
                    {
                        result.Success = true;
                        result.Extend = new
                        {
                            AppImportSettings = new AppImportSettingsViewModel
                            {
                                FileName = newName,
                                AppName = app.DisplayName,
                                AppCode = app.AppCode,
                                IsCover = true,
                                FunctionNodeList = functionNodeList
                            }
                        };
                    }
                }
                catch
                {
                    result.Success = false;
                    result.Message = "Apps.UploadFailed";
                }
                return Json(result, "text/html", JsonRequestBehavior.AllowGet);
            });

        }


        private bool Validate(AppImportSettingsViewModel model, List<FunctionNode> functionNodeList, out ActionResult result)
        {
            result = new ActionResult();
            //app
            string newAppCode = model.AppCode.Trim();
            string oldAppCode = app.AppCode;
            if (string.IsNullOrEmpty(newAppCode))
            {
                result.Success = false;
                result.Message = "Apps.Msg3";
                return false;
            }
            if (this.Engine.AppNavigationManager.GetApp(newAppCode) != null && model.IsCover == false)
            {
                result.Success = false;
                result.Message = "Apps.Msg4";
                return false;
            }
            app.AppCode = newAppCode;
            app.DisplayName = model.AppName.Trim();
            //更新app一级菜单的ParentCode
            functionNodeList.FindAll(i => i.ParentCode == oldAppCode).ForEach(i => i.ParentCode = newAppCode);

            FunctionNode[] levelOneNodes = this.Engine.FunctionAclManager.GetFunctionNodesByParentCode(newAppCode);
            //菜单
            if (model.FunctionNodeList.Count == functionNodeList.Count)
            {
                for (int j = 0; j < functionNodeList.Count; j++)
                {
                    string oldCode = functionNodeList[j].Code;
                    string newCode = model.FunctionNodeList[j].Code;
                    string newName = model.FunctionNodeList[j].DisplayName;
                    if (string.IsNullOrEmpty(newCode))
                    {
                        result.Success = false;
                        result.Message = "Apps.Msg5";
                        return false;
                    }
                    FunctionNode oldNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(newCode);
                    //菜单存在，且不属于当前app或者app的一级菜单
                    if (oldNode != null && !(oldNode.ParentCode == newAppCode ||
                        (levelOneNodes != null && levelOneNodes.Count(i => oldNode.ParentCode == i.Code) > 0)))
                    {
                        result.Success = false;
                        result.Message = "Apps.Msg6";
                        return false;
                    }
                    FunctionNode node = functionNodeList.First(i => i.Code == oldCode);
                    node.ObjectID = Guid.NewGuid().ToString();
                    node.Serialized = false;
                    node.Code = newCode;
                    node.DisplayName = newName;
                    //更新当前菜单的下级菜单的ParentCode
                    functionNodeList.FindAll(i => i.ParentCode == oldCode).ForEach(i => i.ParentCode = newCode);
                }
            }
            return true;
        }


        private bool ReadXmlFile(string xmlStr, out List<FunctionNode> list)
        {
            //根节点
            XElement xmlDoc = XElement.Parse(xmlStr);
            //应用程序
            XElement appDoc = xmlDoc.Element(FunctionNodeType.AppNavigation.ToString());
            app = Convertor.XmlToObject(typeof(AppNavigation), appDoc.ToString()) as AppNavigation;
            if (app == null)
            {
                list = null;
                return false;
            }
            //菜单
            IEnumerable<XElement> functionNodes = xmlDoc.Elements("FunctionNode");
            List<FunctionNode> functionNodeList = new List<FunctionNode>();
            foreach (XElement functionNodeDoc in functionNodes)
            {
                FunctionNode functionNode = Convertor.XmlToObject(typeof(FunctionNode), functionNodeDoc.ToString()) as FunctionNode;
                functionNodeList.Add(functionNode);
            }
            list = functionNodeList;
            return true;
        }


        private List<Dictionary<string, object>> GetChilren(string functionCode, int parentLevel)
        {
            List<Dictionary<string, object>> NodeList = new List<Dictionary<string, object>>();
            Dictionary<string, object> node;
            FunctionNode[] functionNodes = GetFunctionNodesByParentCode(functionCode);
            if (functionNodes != null)
            {
                foreach (FunctionNode functionNode in functionNodes)
                {
                    node = new Dictionary<string, object>();
                    node.Add(FunctionNode.PropertyName_ObjectID, functionNode.ObjectID);
                    node.Add(FunctionNode.PropertyName_Code, functionNode.Code);
                    node.Add(FunctionNode.PropertyName_DisplayName, functionNode.DisplayName);
                    node.Add(FunctionNode.PropertyName_Description, functionNode.Description);
                    node.Add(FunctionNode.PropertyName_SortKey, functionNode.SortKey);
                    node.Add(FunctionNode.PropertyName_IconUrl, functionNode.IconCss);
                    node.Add(FunctionNode.PropertyName_Url, functionNode.Url);
                    node.Add("Level", parentLevel + 1);

                    node.Add("children", GetChilren(functionNode.Code, parentLevel + 1));
                    NodeList.Add(node);
                }
            }
            return NodeList;
        }

        private FunctionNode[] GetFunctionNodesByParentCode(string ParentCode)
        {
            if (!string.IsNullOrEmpty(ParentCode) && this.DicCodeAndChildren.ContainsKey(ParentCode))
            {
                List<FunctionNode> lstFunctionNodes = new List<FunctionNode>();
                //排序
                List<int> lstSortKeys = new List<int>();
                foreach (string fCode in this.DicCodeAndChildren[ParentCode])
                {
                    if (this.FunctionDictionary.ContainsKey(fCode))
                    {
                        lstFunctionNodes.Add(this.FunctionDictionary[fCode]);
                        lstSortKeys.Add(this.FunctionDictionary[fCode].SortKey);
                    }
                }
                return OThinker.Data.Sorter<int, FunctionNode>.Sort(lstSortKeys.ToArray(), lstFunctionNodes.ToArray());
            }
            return new FunctionNode[] { };
        }
        /// <summary>
        /// 获取当前注册授权类型
        /// </summary>
        public OThinker.H3.Configs.LicenseType LicenseType
        {
            get
            {
                return (OThinker.H3.Configs.LicenseType)this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType];
            }
        }

        /// <summary>
        /// 获取应用中心是否允许使用
        /// </summary>
        public bool AppsAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Apps] + string.Empty);
            }
        }
        /// <summary>
        /// 获取钉钉
        /// </summary>
        /// <param name="appCode"></param>
        /// <returns></returns>
        private string DingTalkPulishURL(string appCode)
        {
            string path=Request.Url.OriginalString;
            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("request url is null");
            string host = path.Substring(0,path.IndexOf(Request.ApplicationPath))+Request.ApplicationPath;
            string url = host + "/Mobile/index.html?showmenu=false&target=appCenterItem&state=DefaultEngine&loginfrom=dingtalk&params={\"AppCode\":\"" + appCode + "\"}";
            return url;
        }
    }
}
