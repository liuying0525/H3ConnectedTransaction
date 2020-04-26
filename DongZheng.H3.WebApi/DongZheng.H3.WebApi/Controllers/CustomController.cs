using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    /// <summary>
    /// 菜单类验证越权 ErrCode = 802 permission denied
    /// wangxg 19.7.3
    /// 需要验证越权的控制器都继承此类，不需要验证越权就继承 OThinker.H3.Controllers.ControllerBase
    /// </summary>
    public abstract class CustomController : OThinker.H3.Controllers.ControllerBase
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // 越权验证开关
            var auth = System.Configuration.ConfigurationManager.AppSettings.Get("Auth");
            if (auth != null && (auth.ToLower().Equals("0") || auth.ToLower().Equals("false")))
                return;

            ContentResult Content = new ContentResult();

            List<string> funs = new List<string>();// 根据分号拆分后转小写后的所有可访问的接口列表
            var funDes = GetFunctionApps();// 获取当前用户能访问的所有菜单权限
            GetFunDes(funDes, funs);// 转换算法
            string action = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName
                + "/" + filterContext.ActionDescriptor.ActionName;// 当前请求要访问的接口拼接串
            action = action.ToLower();// 转小写后与权限集合对比

            if (!funs.Contains(action))
            {
                Content.Content = "权限不足";
                filterContext.Result = Content;
                filterContext.HttpContext.Response.StatusCode = 802;
                filterContext.HttpContext.Response.StatusDescription = "permission denied";
                return;
            }
            
        }

        /// <summary>
        /// 菜单所属顶层node code
        /// </summary>
        private string _TopAppCode;
        public string TopAppCode
        {
            get
            {
                if (string.IsNullOrEmpty(_TopAppCode)) return "";
                return _TopAppCode;
            }
            set
            {
                _TopAppCode = value;
            }
        }

        /// <summary>
        /// 获取当前用户所有菜单权限
        /// </summary>
        /// <param name="funDes"></param>
        /// <param name="funs"></param>
        private void GetFunDes(List<FunctionViewModel> funDes, List<string> funs)
        {
            if (funs == null || funDes == null) return;
            foreach(var item in funDes)
            {
                var des = item.Description;
                if(des != null)
                {
                    var desArr = des.Split(';');
                    foreach (var tem in desArr)
                    {
                        var fun = tem.Trim();
                        if (!string.IsNullOrEmpty(fun))
                            funs.Add(fun.ToLower());
                    }
                }
                if (item.Children != null && item.Children.Count > 0)
                    GetFunDes(item.Children, funs);
            }
        }
        
        /// <summary>
        /// 获取应用App的集合
        /// </summary>
        /// <returns></returns>
        private List<FunctionViewModel> GetFunctionApps()
        {
            //门户可见、固定到首页
            List<FunctionViewModel> functions = new List<FunctionViewModel>();
            if (this.UserValidator != null)
            {
                OThinker.H3.Apps.AppNavigation[] AllApps = this.Engine.AppNavigationManager.GetAllApps();
                Dictionary<string, OThinker.H3.Apps.AppNavigation> appNavigation = new Dictionary<string, OThinker.H3.Apps.AppNavigation>();
                foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
                {
                    foreach (OThinker.H3.Apps.AppNavigation app in AllApps)
                    {
                        if (functionNode.Code == app.AppCode)
                        {
                            if (!appNavigation.ContainsKey(functionNode.Code))
                            {
                                appNavigation.Add(functionNode.Code, app);
                                break;
                            }
                        }
                    }
                }


                foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
                {
                    if (functionNode.NodeType == OThinker.H3.Acl.FunctionNodeType.AppNavigation)
                    {
                        if (!appNavigation.ContainsKey(functionNode.Code)) continue;
                        OThinker.H3.Apps.AppNavigation appNav = appNavigation[functionNode.Code];
                        if (appNav.VisibleOnPortal == OThinker.Data.BoolMatchValue.True)
                        {
                            TopAppCode = functionNode.Code;
                            FunctionViewModel parentViewModel = new FunctionViewModel()
                            {
                                Code = functionNode.Code,
                                TopAppCode = this.TopAppCode,
                                DisplayName = functionNode.DisplayName,
                                IconUrl = functionNode.IconUrl,
                                IconCss = functionNode.IconCss,
                                SortKey = functionNode.SortKey,
                                Url = functionNode.Url,
                                Children = new List<FunctionViewModel>(),
                                DockOnHomePage = (appNav != null && appNav.DockOnHomePage == OThinker.Data.BoolMatchValue.True),
                                Description = functionNode.Description,// 19.7.3 wangxg
                            };
                            functions.Add(parentViewModel);
                            // 递归获取子节点
                            this.GetChildFunctions(functionNode, parentViewModel);
                        }
                    }
                }
                
            }
            return functions;
        }

        /// <summary>
        /// 递归获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="parentViewModel"></param>
        private void GetChildFunctions(OThinker.H3.Acl.FunctionNode parentNode, FunctionViewModel parentViewModel)
        {
            foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
            {
                if (functionNode.ParentCode == parentNode.Code)
                {
                    FunctionViewModel viewModel = new FunctionViewModel()
                    {
                        Code = functionNode.Code,
                        DisplayName = functionNode.DisplayName,
                        TopAppCode = this.TopAppCode,
                        IconCss = functionNode.IconCss,
                        SortKey = functionNode.SortKey,
                        Url = functionNode.Url,
                        Children = new List<FunctionViewModel>(),
                        Description = functionNode.Description // 19.7.3 wangxg
                    };
                    parentViewModel.Children.Add(viewModel);

                    // 递归获取子节点
                    this.GetChildFunctions(functionNode, viewModel);
                }
            }
        }
        
    }
}