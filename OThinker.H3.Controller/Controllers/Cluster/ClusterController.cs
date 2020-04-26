using OThinker.Clusterware;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Cluster
{
    /// <summary>
    /// 集群管理父控制器
    /// </summary>
    public class ClusterController : Controller
    {
        #region 获取主引擎的配置信息 ---------------------
        private OThinker.Data.Database.DatabaseType dbType = OThinker.Data.Database.DatabaseType.Unspecified;
        /// <summary>
        /// 获取或设置集群服务地址
        /// </summary>
        protected string ServerAddress
        {
            get
            {
                return Session["ServerAddress"] + string.Empty;
            }
            set
            {
                Session["ServerAddress"] = value;
            }
        }

        /// <summary>
        /// 获取或设置服务器的端口号
        /// </summary>
        protected int Port
        {
            get
            {
                int port = 0;
                int.TryParse(Session["Port"] + string.Empty, out port);
                return port;
            }
            set
            {
                Session["Port"] = value;
            }
        }

        /// <summary>
        /// 获取或设置用户登录名称
        /// </summary>
        protected string UserName
        {
            get
            {
                return Session["UserName"] + string.Empty;
            }
            set
            {
                Session["UserName"] = value;
            }
        }

        /// <summary>
        /// 获取或设置用户登录密码
        /// </summary>
        protected string Password
        {
            get
            {
                return Session["Password"] + string.Empty;
            }
            set
            {
                Session["Password"] = value;
            }
        }

        #endregion

        private string _ClusterRoot = null;

        /// <summary>
        /// 获取站点名称
        /// </summary>
        public string ClusterRoot
        {
            get
            {
                if (_ClusterRoot == null)
                {
                    _ClusterRoot = ConfigurationManager.AppSettings["ClusterRoot"] + string.Empty;
                    if (this._ClusterRoot == string.Empty) _ClusterRoot = "/Cluster";
                    if (this._ClusterRoot == "/") _ClusterRoot = "";//为“/”时会把localhost和端口号丢失
                }
                return this._ClusterRoot;
            }
        }

        /// <summary>
        /// 获取集群服务器的连接
        /// </summary>
        protected OThinker.Clusterware.MasterConnection Connection
        {
            get
            {
                if (this.Session["ClusterConnection"] == null)
                {
                    if (Server == null)
                    {
                        LoginOut();
                    }
                    ConnectionSetting setting = new OThinker.Clusterware.ConnectionSetting()
                    {
                        Address = ServerAddress,
                        ObjectUri = OThinker.H3.Configs.ProductInfo.EngineUri,
                        Port = Port
                    };

                    OThinker.Clusterware.MasterConnection connection = new Clusterware.MasterConnection(
                        setting,
                        UserName,
                        Password);
                    if (connection.Connect())
                    {
                        this.Session["ClusterConnection"] = connection;
                    }
                    else
                    {
                        LoginOut();
                    }
                }
                return this.Session["ClusterConnection"] as OThinker.Clusterware.MasterConnection;
            }
            set
            {
                this.Session["ClusterConnection"] = value;
            }
        }


        public JsonResult ExecuteFunctionRun(Func<JsonResult> func)
        {
            if (Connection == null)
            {
                ActionResult result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
                result.Extend = this.ClusterRoot;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return func();
            }
        }

        /// <summary>
        /// 登录集群系统
        /// </summary>
        /// <param name="clusterUser">登录信息</param>
        /// <returns>是否成功</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult Login(CluserUserViewModel clusterUser)
        {

            this.UserName = clusterUser.UserName;
            this.Password = clusterUser.Password;
            this.ServerAddress = clusterUser.ServerAddress;
            this.Port = clusterUser.Port;
            ActionResult result = new ActionResult(false);
            if (this.Connection != null)
            {
                result.Success = true;
                result.Extend = this.ClusterRoot;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetIndexData()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                result.Extend = new
                {
                    ClusterRoot = this.ClusterRoot,
                    UserName = this.UserName
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        [HttpGet]
        public RedirectResult LoginOut()
        {
            this.Session.Remove("ClusterConnection");
            this.Session.Abandon();
            return new RedirectResult("~/Login.html");
        }

        /// <summary>
        /// 转向登录页面
        /// </summary>
        protected RedirectResult RedirectLogin()
        {
            return new RedirectResult("~/Login.html");
        }

        /// <summary>
        /// 获取每页显示的数据量
        /// </summary>
        public virtual int Pagesize
        {
            get
            {
                { return Convert.ToInt32(Request.Params["pageSize"] ?? "20"); }
            }
        }

        /// <summary>
        /// 获取当前页码
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return Convert.ToInt32(Request.Params["pageIndex"] ?? "1");
            }
        }

        #region 创建LigerUI表格的数据，已序列化
        /// <summary>
        /// 创建LigerUI表格的数据，已序列化
        /// </summary>
        /// <param name="rowOjb"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public object CreateLigerUIGridData(object[] rowOjb)
        {
            int totalCount = rowOjb == null ? 0 : rowOjb.Length;
            int startIndex = 0;
            int endIndex = totalCount;

            List<Object> objList = new List<object>();

            if (this.Pagesize > 0)
            {
                startIndex = (CurrentPageIndex - 1) * Pagesize < 0 ? 0 : (CurrentPageIndex - 1) * Pagesize;
                endIndex = CurrentPageIndex * Pagesize >= totalCount ? totalCount : CurrentPageIndex * Pagesize;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                objList.Add((rowOjb as object[])[i]);
            }

            var jsonObj = new { Rows = objList, Total = totalCount };
            return jsonObj;
        }
        #endregion

    }
}
