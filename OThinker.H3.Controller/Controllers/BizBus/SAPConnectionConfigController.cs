using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    public class SAPConnectionConfigController : ControllerBase
    {
        public SAPConnectionConfigController() { }

        public override string FunctionCode
        {
            get
            {
                return Acl.FunctionNode.BizBus_EditSAPConnectionConfig_Code;
            }
        }

        private OThinker.H3.BizBus.SAPConnector.Client _SAPClient = null;
        private OThinker.H3.BizBus.SAPConnector.Client SAPClient
        {
            get
            {
                if (_SAPClient == null)
                {
                    _SAPClient = new OThinker.H3.BizBus.SAPConnector.Client("localhost");
                }
                return _SAPClient;
            }
        }

        //private OThinker.H3.BizBus.SAPConnector.Client SAPClient
        //{
        //    get
        //    {
        //        if (this.Cache[this.GetType().ToString() + "_localhost"] == null)
        //        {
        //            // 注：初次配置SAPConnector只能连接本机服务
        //            this.Cache[this.GetType().ToString() + "_localhost"] = new BizBus.SAPConnector.Client("localhost");
        //        }
        //        return (OThinker.H3.BizBus.SAPConnector.Client)this.Cache[this.GetType().ToString() + "_localhost"];
        //    }
        //}

        /// <summary>
        /// 获取连接列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSAPConnectionConfigList()
        {
            return ExecuteFunctionRun(() =>
            {
                List<SAPConnectionConfigViewModel> listModel = new List<SAPConnectionConfigViewModel>();
                OThinker.H3.BizBus.SAPConnector.SAPConnectionConfig[] SAPConnectionConfigs = SAPClient.Connector.ListConfigs();
                if (SAPConnectionConfigs != null)
                {
                    foreach (OThinker.H3.BizBus.SAPConnector.SAPConnectionConfig config in SAPConnectionConfigs)
                    {
                        config.LoginPassword = "******";

                        SAPConnectionConfigViewModel model = new SAPConnectionConfigViewModel();
                        model.Name = config.ConnectionName;
                        model.AppserverHost = config.AppServerHost;
                        model.Client = config.Client;
                        model.Language = config.Language;
                        model.LoginUser = config.LoginUser;
                        model.LoginPassword = config.LoginPassword;
                        model.MaxPoolSize = config.MaxPoolSize;
                        model.SystemNumber = config.SystemNumber;
                        model.ObjectID = config.ObjectID;//区分更新和新增

                        listModel.Add(model);
                    }
                }
                var griddata = CreateLigerUIGridData(listModel.ToArray());
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除连接
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteSAPConnectionConfig(string names)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "SAPConnectionConfig.DeleteSuccess");
                string[] NameArray = names.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (NameArray != null)
                {
                    SAPClient.Connector.RemoveConfigs(NameArray);
                }
                return Json(result);
            });
        }

        /// <summary>
        /// 设置默认
        /// </summary>
        /// <returns></returns>
        public JsonResult SetDefault(string name)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "SAPConnectionConfig.SetSuccess");//TODO
                if (!string.IsNullOrWhiteSpace(name))
                {
                    SAPClient.Connector.SetDefaultConfig(name);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取BAPI列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBAPIList()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                string message = "";
                List<object> list = GetChildBAPIList("", out message);
                if (message.Length > 0) { result.Success = false; result.Message = message; return Json(result, JsonRequestBehavior.AllowGet); }
                result.Extend = list;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 保存连接信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveConnectionConfig(SAPConnectionConfigViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult(true, "");
                if (model.MaxPoolSize == 0) { result.Message = "SAPConnectionConfig.Msg0"; result.Success = false; return Json(result); }//TODO 信息提示
                // 更新到服务器
                try
                {
                    OThinker.H3.BizBus.SAPConnector.SAPConnectionConfig config = SAPClient.Connector.GetConfig(model.Name);//.GetDefaultConfig();
                    if (config == null)
                    {
                        config = new OThinker.H3.BizBus.SAPConnector.SAPConnectionConfig();
                    }
                    else if (string.IsNullOrWhiteSpace(model.ObjectID))//如果是新增，提示已经存在
                    {
                        //this.ShowWarningMessage(this.PortalResource.GetString("EditSAPConnectionConfig_NameExist"));
                        result.Success = false;
                        result.Message = "SAPConnectionConfig.NameExist";//TODO
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    config.AppServerHost = model.AppserverHost;
                    config.Client = model.Client;
                    config.ConnectionName = model.Name;
                    config.Language = model.Language;
                    config.LoginPassword = model.LoginPassword;
                    config.LoginUser = model.LoginUser;
                    config.MaxPoolSize = model.MaxPoolSize;
                    config.SystemNumber = model.SystemNumber;

                    if (SAPClient.Connector.SaveConfig(config))
                    {
                        // ShowSuccessMessage(this.PortalResource.GetString("Msg_UpdateSuccess"));
                        result.Message = "SAPConnectionConfig.SaveSuccess";
                        //this.CloseCurrentDialog();
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "SAPConnectionConfig.SaveSuccess";
                        //ShowWarningMessage(this.PortalResource.GetString("EditSAPConnectionConfig_Msg1"));
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "SAPConnectionConfig.SaveFailed";
                    result.Extend = ex.Message;
                    //this.ShowWarningMessage(this.PortalResource.GetString("EditSAPConnectionConfig_Msg2"));
                    //return;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取连接信息
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public JsonResult GetConnectionConfig(string connectionName)
        {
            return ExecuteFunctionRun(() =>
            {
                SAPConnectionConfigViewModel model = new SAPConnectionConfigViewModel();
                OThinker.H3.BizBus.SAPConnector.SAPConnectionConfig config = SAPClient.Connector.GetConfig(connectionName);
                if (config != null)
                {
                    model.Name = config.ConnectionName;
                    model.AppserverHost = config.AppServerHost;
                    model.Client = config.Client;
                    model.Language = config.Language;
                    model.LoginUser = config.LoginUser;
                    model.LoginPassword = config.LoginPassword;
                    model.MaxPoolSize = config.MaxPoolSize;
                    model.SystemNumber = config.SystemNumber;
                    model.ObjectID = config.ObjectID;//区分更新和新增
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        public List<Object> GetChildBAPIList(string parentValue, out string message)
        {
            if (string.IsNullOrEmpty(parentValue)) { parentValue = ""; }//设置根节点

            List<Object> dataList = new List<object>();
            OThinker.H3.BizBus.SAPConnector.BAPINode[] children = null;
            // dataList.Add(new { id = "T1", pid = "0", text = "TQ",icon=this.PortalRoot+ "/WFRes/images/IB_Segment.gif" });
            try
            {
                children = SAPClient.Connector.GetComponents(parentValue);
            }
            catch (Exception ex)
            {
                message = "SAPConnectionConfig.ListBAPI_Msg0";
                this.Engine.LogWriter.Write(ex.ToString());
                return null;
            }

            foreach (OThinker.H3.BizBus.SAPConnector.BAPINode child in children)
            {
                string key = null;

                var imageUrl = "";
                switch (child.NodeType)
                {
                    case H3.BizBus.SAPConnector.BAPINodeType.Component:
                        imageUrl = this.PortalRoot + "/WFRes/images/IB_Segment.gif";
                        break;
                    case H3.BizBus.SAPConnector.BAPINodeType.Object:
                        imageUrl = this.PortalRoot + "/WFRes/images/IB_OU.gif";
                        break;
                    case H3.BizBus.SAPConnector.BAPINodeType.Method:
                        imageUrl = this.PortalRoot + "/WFRes/images/IB_Man.gif";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (child.Name != "")
                {
                    dataList.Add(new { id = child.Name, pid = parentValue, text = child.DisplayName, icon = imageUrl });
                }

                switch (child.NodeType)
                {
                    case H3.BizBus.SAPConnector.BAPINodeType.Component:
                        key = "a" + child.Name;
                        if (child.Name != "")
                            dataList.AddRange(GetChildBAPIList(child.Name, out message));
                        break;
                    case H3.BizBus.SAPConnector.BAPINodeType.Object:
                        key = "b" + child.Name;
                        if (child.Name != "")
                            dataList.AddRange(GetChildBAPIList(child.Name, out message));
                        break;
                    case H3.BizBus.SAPConnector.BAPINodeType.Method:
                        key = "c" + child.Name;
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }

            message = "";
            return dataList;
        }

    }
}
