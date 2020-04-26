using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using OThinker.H3.Sheet;
using OThinker.Data;
using OThinker.H3.Instance;

namespace OThinker.H3.Controllers.Controllers.WorkItemDetail
{
    public class WorkItemDetailController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }
        /// <summary>
        /// 工作任务的URL
        /// </summary>
        private string url = string.Empty;
        
        public JsonResult WorkItemDetail(string paramString)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                Dictionary<string, string> dicParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString);

                string strMode = string.Empty, WorkItemID = string.Empty, strIsMobile = string.Empty,
                    state = string.Empty;
                Dictionary<string, string> dicOtherParams = new Dictionary<string, string>();
                //读取URL参数
                foreach (string key in dicParams.Keys)
                {
                    if (key == Param_Mode) { strMode = dicParams[key]; continue; }
                    if (key == Param_WorkItemID) { WorkItemID = dicParams[key]; continue; }
                    if (key == Param_IsMobile) { strIsMobile = dicParams[key]; continue; }
                    if (key == "state") { state = dicParams["state"]; continue; }
                    dicOtherParams.Add(key, dicParams[key]);
                }
                //Work、View。。。
                SheetMode Mode = this.GetSheetMode(strMode, WorkItemID);
                //WorkItem
                WorkItem.WorkItem CurrentWorkItem = this.GetCurrentWorkItem(WorkItemID);
                //获取工作任务的表单对象
                BizSheet WorkItemSheet = this.GetWorkItemSheet(CurrentWorkItem);
                bool IsMobile = this.IsMobile(strIsMobile, state);

                // 解析Url地址
                if (Mode == SheetMode.Work)
                {
                    url = this.GetWorkSheetUrl(CurrentWorkItem, WorkItemSheet, IsMobile);
                }
                else
                {
                    url = this.GetViewSheetUrl(CurrentWorkItem, WorkItemSheet, Mode, IsMobile);
                }
                // 将其中的数据参数做转换
                InstanceData InstanceData = this.GetInstanceData(CurrentWorkItem);
                if (url.Contains(OThinker.H3.Math.Variant.VariablePrefix.ToString()))
                {
                    url = InstanceData.ParseText(url);
                }
                // 处理缓存
                DateTime t = DateTime.Now;
                DateTime.TryParse(InstanceData.BizObject[OThinker.Organization.Unit.PropertyName_ModifiedTime] + string.Empty, out t);
                url += "&T=" + t.ToString("HHmmss") + WorkItemID.Substring(0, 8);
                if (Mode == SheetMode.Print)
                {
                    url += "Print";
                }
                result.Success = true;
                result.Message = url;
                return Json(result);
            });
        }
        public InstanceData GetInstanceData(WorkItem.WorkItem CurrentWorkItem)
        {
            InstanceData InstanceData = new InstanceData(this.Engine, CurrentWorkItem.InstanceId, this.UserValidator.UserID);
            return InstanceData;
        }

        #region 获取待办任务的链接地址
        /// <summary>
        /// 获取待办任务的链接地址
        /// </summary>
        /// <param name="WorkItem"></param>
        /// <param name="Sheet"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public string GetWorkSheetUrl(
            WorkItem.WorkItem WorkItem,
            BizSheet Sheet,
            bool IsMobile)
        {
            if (WorkItem == null)
            {
                return null;
            }
            string baseUrl = string.Empty;
            baseUrl = GetSheetBaseUrl(SheetMode.Work, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode.Work + "&";
            if (WorkItem != null)
            {
                baseUrl += SheetEnviroment.Param_WorkItemID + "=" + WorkItem.WorkItemID + "&";
            }
            return baseUrl;
        }

        public string GetViewSheetUrl(
            WorkItem.WorkItem WorkItem,
            BizSheet Sheet,
            SheetMode SheetMode,
            bool IsMobile)
        {
            string baseUrl = GetSheetBaseUrl(SheetMode.View, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode + "&";
            baseUrl += SheetEnviroment.Param_WorkItemID + "=" + WorkItem.WorkItemID + "&";
            return baseUrl;
        }

        public string GetSheetBaseUrl(SheetMode SheetMode, bool IsMobile, BizSheet Sheet)
        {
            string baseUrl = null;
            switch (Sheet.SheetType)
            {
                case SheetType.None:
                    baseUrl = null;
                    break;
                case SheetType.DefaultSheet:
                    baseUrl = this.GetDefaultSheetUrl(Sheet, IsMobile);
                    break;
                case SheetType.CustomSheet:
                    string sheetUrl = GetWorkItemSheet(IsMobile, SheetMode, Sheet.MobileSheetAddress, Sheet.SheetAddress, Sheet.PrintSheetAddress);
                    if (sheetUrl.LastIndexOf("?") == -1)
                    {
                        // url的形式应该是http://.../page1.aspx
                        baseUrl = sheetUrl + "?";
                    }
                    else if (sheetUrl.LastIndexOf("?") == sheetUrl.Length - 1
                        || sheetUrl.LastIndexOf("&") == sheetUrl.Length - 1)
                    {
                        // url的形式应该是http://.../page1.aspx?
                        // url的形式应该是http://.../page1.aspx?param1=value1&
                        baseUrl = sheetUrl;
                    }
                    else
                    {
                        // url的形式应该是http://.../page1.aspx?param1=value1
                        baseUrl = sheetUrl + "&";
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return baseUrl;
        }

        /// <summary>
        /// 获取任务打开的表单URL
        /// </summary>
        /// <param name="IsMobile"></param>
        /// <param name="SheetMode"></param>
        /// <param name="MobileSheetUrl"></param>
        /// <param name="PCSheetUrl"></param>
        /// <param name="PrintSheetUrl"></param>
        /// <returns></returns>
        public string GetWorkItemSheet(bool IsMobile, SheetMode SheetMode, string MobileSheetUrl, string PCSheetUrl, string PrintSheetUrl)
        {
            if (SheetMode == SheetMode.Print) return PrintSheetUrl;
            if (IsMobile)
            {
                if (!string.IsNullOrEmpty(MobileSheetUrl))
                {
                    return MobileSheetUrl + "?" + Param_IsMobile + "=" + true;
                }
                else
                {
                    return PCSheetUrl + "?" + Param_IsMobile + "=" + true;
                }
            }
            else
            {
                return PCSheetUrl + "?";
            }
        }

        /// <summary>
        /// 获取默认表单URL
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        private string GetDefaultSheetUrl(BizSheet Sheet, bool isMobile)
        {
            return GetDefaultSheetUrl(Sheet) + (isMobile ? "IsMobile=" + isMobile + "&" : "");
        }
        /// <summary>
        /// 获取默认表单URL
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        private string GetDefaultSheetUrl(BizSheet Sheet)
        {
            string SheetUrl = string.Empty;
            if (Sheet.EnabledCode)
            {
                SheetUrl = System.IO.Path.Combine(this.PortalRoot, GetDesignerSheetAddress(Sheet));
            }
            else
            {
                if (Sheet.IsMVC)
                {
                    SheetUrl = System.IO.Path.Combine(this.PortalRoot, "MvcDefaultSheet.aspx").Replace('\\', '/');
                }
                else
                {
                    SheetUrl = System.IO.Path.Combine(this.PortalRoot, "DefaultSheet.aspx").Replace('\\', '/');
                }
            }
            SheetUrl += "?";// +Param_SheetCode + "=" + Sheet.SheetCode + "&";
            return SheetUrl;
        }
        /// <summary>
        /// 获取设计后的表单URL地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public string GetDesignerSheetAddress(BizSheet Sheet)
        {
            // 表单文件名称
            string sheetAddress = Sheet.SheetAddress == string.Empty ? Sheet.SheetCode + ".aspx" : Sheet.SheetAddress;
            string sheetDirectory = "Sheets/" + AppUtility.Engine.EngineConfig.Code + "/" + Sheet.BizObjectSchemaCode;
            // 表单存储路径
            sheetAddress = Path.Combine(sheetDirectory, sheetAddress);
            // 获取完整的表单路径
            string sheetAddressFullPath = Path.Combine(Server.MapPath("."), sheetAddress);

            bool created = false;

            if (!System.IO.File.Exists(sheetAddressFullPath))
            {
                if (!Directory.Exists(Path.Combine(Server.MapPath("."), sheetDirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("."), sheetDirectory));
                }
                created = true;
            }
            else
            {
                FileInfo f = new FileInfo(sheetAddressFullPath);
                if (f.LastWriteTime < Sheet.LastModifiedTime)
                {
                    created = true;
                }
            }

            if (created)
            {
                // 生成文件
                string cs = Sheet.CodeContent;
                string aspx = string.Empty;
                if (cs == string.Empty) cs = GetCSharpCode(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode);
                if (Sheet.IsMVC)
                {
                    aspx = GetMvcSheetAspx(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode, Sheet.RuntimeContent, Sheet.Javascript);
                }
                //else
                //{
                //    aspx = GetSheetAspx(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode, Sheet.RuntimeContent, Sheet.Javascript);
                //}

                using (StreamWriter sw = new StreamWriter(sheetAddressFullPath, false, Encoding.UTF8))
                {
                    sw.Write(aspx);
                }
                using (StreamWriter sw = new StreamWriter(sheetAddressFullPath + ".cs", false, Encoding.UTF8))
                {
                    sw.Write(cs);
                }
            }

            return sheetAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="SheetCode"></param>
        /// <param name="SheetAspx"></param>
        /// <param name="Script"></param>
        /// <returns></returns>
        public string GetMvcSheetAspx(string EngineCode, string SheetCode, string SheetAspx, string Script)
        {
            string aspx = "<%@ Page Language=\"C#\" AutoEventWireup=\"true\" CodeFile=\"" + SheetCode + ".aspx.cs\"";
            aspx += " Inherits=\"OThinker.H3.Portal.Sheets." + EngineCode + "." + SheetCode + "\" EnableEventValidation=\"false\" MasterPageFile=\"~/MvcSheet.master\" %>\n";
            aspx += "<%@ OutputCache Duration=\"999999\" VaryByParam=\"T\" VaryByCustom=\"browser\" %>\n";
            aspx += "<asp:Content ID=\"head\" ContentPlaceHolderID=\"headContent\" runat=\"Server\">\n";
            //aspx += "\t<script type=\"text/javascript\">\n";
            //aspx += Script;
            //aspx += "\n</script>\n";
            aspx += "</asp:Content>\n";
            aspx += "<asp:Content ID=\"menu\" ContentPlaceHolderID=\"cphMenu\" runat=\"Server\">\n";
            aspx += "</asp:Content>\n";
            aspx += "<asp:Content ID=\"master\" ContentPlaceHolderID=\"masterContent\" runat=\"Server\">\n";
            aspx += SheetAspx;
            aspx += "</asp:Content>";
            // aspx = string.Format(aspx, SheetCode, EngineCode, SheetAspx);
            return aspx;
        }
        /// <summary>
        /// 获取C#代码
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        public string GetCSharpCode(string EngineCode, string SheetCode)
        {
            // 保存为 cs 文件
            string cs = "using System;\r\n";
            cs += "using System.Collections;\r\n";
            cs += "using System.Configuration;\r\n";
            cs += "using System.Data;\r\n";
            cs += "using System.Web;\r\n";
            cs += "using System.Web.Security;\r\n";
            cs += "using System.Web.UI;\r\n";
            cs += "using System.Web.UI.HtmlControls;\r\n";
            cs += "using System.Web.UI.WebControls;\r\n";
            cs += "using System.Web.UI.WebControls.WebParts;\r\n";
            cs += "\r\n";
            cs += "namespace OThinker.H3.Portal.Sheets." + EngineCode + "\r\n";
            cs += "{\r\n";
            cs += "    public partial class " + SheetCode + " : OThinker.H3.WorkSheet.SheetPage\r\n";
            cs += "    {\r\n";
            cs += "        protected void Page_Load(object sender, EventArgs e)\r\n";
            cs += "        {\r\n";
            cs += "        }\r\n";
            cs += "\r\n";
            cs += "        /// <summary>\r\n";
            cs += "        /// 加载引擎数据到表单\r\n";
            cs += "        /// </summary>\r\n";
            cs += "        public override void LoadDataFields()\r\n";
            cs += "        {\r\n";
            cs += "            base.LoadDataFields();\r\n";
            cs += "        }\r\n";
            cs += "\r\n";
            cs += "        /// <summary>\r\n";
            cs += "        /// 保存表单数据到引擎中\r\n";
            cs += "        /// </summary>\r\n";
            cs += "        /// <param name=\"Args\"></param>\r\n";
            cs += "        public override void SaveDataFields(OThinker.H3.WorkSheet.SheetSubmitEventArgs Args)\r\n";
            cs += "        {\r\n";
            cs += "            base.SaveDataFields(Args);\r\n";
            cs += "        }\r\n";
            cs += "    }\r\n";
            cs += "}\r\n";

            return cs;
        }
        #endregion

        public bool IsMobile(string strIsMobile, string state)
        {
            bool result = false;
            OThinker.Data.BoolMatchValue IsMobile = OThinker.Data.BoolMatchValue.Unspecified;
            if (string.IsNullOrEmpty(strIsMobile) || !bool.TryParse(strIsMobile, out result))
            {
                IsMobile = OThinker.Data.BoolMatchValue.False;
            }
            if (!result)
            {
                result = (state.ToLower() == "mobile" || state.ToLower().EndsWith(this.Engine.EngineConfig.Code.ToLower()));
            }
            IsMobile = result ? BoolMatchValue.True : BoolMatchValue.False;
            return IsMobile == OThinker.Data.BoolMatchValue.True;
        }


        public BizSheet GetWorkItemSheet(WorkItem.WorkItem CurrentWorkItem)
        {
            BizSheet workItemSheet = this.Engine.BizSheetManager.GetBizSheetByCode(CurrentWorkItem.SheetCode);
            return workItemSheet;
        }

        /// <summary>
        /// 获取当前任务表单打开模式
        /// </summary>
        public SheetMode GetSheetMode(string strMode, string WorkItemID)
        {
            SheetMode SheetMode = SheetMode.Unspecified;
            OThinker.H3.WorkItem.WorkItem CurrentWorkItem = this.GetCurrentWorkItem(WorkItemID);

            if (strMode.Trim() != string.Empty)
            {
                SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), strMode);
            }
            else if (CurrentWorkItem.State == WorkItem.WorkItemState.Finished || CurrentWorkItem.State == WorkItem.WorkItemState.Canceled)
            {
                SheetMode = SheetMode.View;
            }
            else
            {
                SheetMode = SheetMode.Work;
            }
            if (SheetMode == SheetMode.Work && (CurrentWorkItem.State == WorkItem.WorkItemState.Finished || CurrentWorkItem.State == WorkItem.WorkItemState.Canceled))
            {
                SheetMode = SheetMode.View;
            }
            return SheetMode;
        }

        /// <summary>
        /// 获取当前工作任务
        /// </summary>
        public OThinker.H3.WorkItem.WorkItem GetCurrentWorkItem(string WorkItemID)
        {
            OThinker.H3.WorkItem.WorkItem CurrentWorkItem = null;
            CurrentWorkItem = this.Engine.WorkItemManager.GetWorkItem(WorkItemID);
            return CurrentWorkItem;
        }
    }
}
