using Newtonsoft.Json;
using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace OThinker.H3.Controllers.Controllers.Sheets
{
	public class WorkItemSheetsController : ControllerBase
	{
		public override string FunctionCode
		{
			get { return ""; }
		}

		public JsonResult WorkItemSheets(string paramString)
		{
			ActionResult result = new ActionResult(false, "");

			Dictionary<string, string> dicParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString);

			bool isMobile = false;
			string LoginName = string.Empty;
			string LoginPassword = string.Empty;
			string MobileToken = string.Empty;
			string WechatCode = string.Empty;
			string EngineCode = string.Empty;
			foreach (string key in dicParams.Keys)
			{
				if (key == Param_WorkItemID) { WorkItemID = dicParams[key]; continue; }
				if (key == Param_Mode) { SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), dicParams[key]); continue; }
				if (key == Param_IsMobile)
				{
					bool.TryParse(dicParams[key], out isMobile);
					IsMobile = isMobile;
					continue;
				}
				if (key.ToLower() == "loginname") { LoginName = dicParams[key]; }
				if (key.ToLower() == "loginpassword") { LoginPassword = dicParams[key]; }
				if (key.ToLower() == "mobiletoken") { MobileToken = dicParams[key]; }
				if (key.ToLower() == "code") { WechatCode = dicParams[key]; }
				if (key.ToLower() == "state") { EngineCode = dicParams[key]; }
			}
			//TODO:微信不需要做单点登录
			////实现微信单点登录
			//if (!string.IsNullOrEmpty(WechatCode) && !string.IsNullOrEmpty(EngineCode)
			//    && System.Web.HttpContext.Current.Session[Sessions.GetUserValidator()] != null)
			//{
			//    IsMobile = true;
			//    UserValidatorFactory.LoginAsWeChat(EngineCode, WechatCode);
			//}

			//APP打开表单验证
			if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(MobileToken) && this.UserValidator == null)
			{
				if (!SSOopenSheet(LoginName, MobileToken))
				{
					result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
			if (this.UserValidator == null && !string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(LoginPassword))
			{// 实现登录验证
				OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(LoginName);
				if (user.ValidatePassword(LoginPassword))
				{
					Session[Sessions.GetUserValidator()] = UserValidatorFactory.GetUserValidator(this.Engine, user.Code);
				}
			}

			if (this.UserValidator == null)
			{
				result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
				return Json(result, JsonRequestBehavior.AllowGet);
			}

			// 解析Url地址
			if (SheetMode == SheetMode.Work)
			{
				if (CurrentWorkItem != null)
				{
					url = this.GetWorkSheetUrl(
					CurrentWorkItem,
					WorkItemSheet,
					IsMobile);
				}
				else
				{
					url = this.GetViewCirculateItemSheetUrl(
							CurrentCirculateItem,
							WorkItemSheet,
							SheetMode,
							IsMobile);
				}
			}
			else
			{
				if (CurrentWorkItem != null)
				{
					url = this.GetViewSheetUrl(
						   CurrentWorkItem,
						   WorkItemSheet,
						   SheetMode,
						   IsMobile);
				}
				else
				{
					url = this.GetViewCirculateItemSheetUrl(
						CurrentCirculateItem,
						WorkItemSheet,
						SheetMode,
						IsMobile);
				}
			}
			// 将其中的数据参数做转换
			if (url.Contains(OThinker.H3.Math.Variant.VariablePrefix.ToString()))
			{
				url = InstanceData.ParseText(url);
			}
			// 处理缓存
			DateTime t = DateTime.Now;
			url += "&T=" + t.ToString("HHmmss") + WorkItemID.Substring(0, 8);
			if (SheetMode == SheetMode.Print)
			{
				url += "Print";
			}
			result.Success = true;
			result.Message = url;
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		//方法重写，区分推送和表单直接打开所存在的差异
		public JsonResult WorkItemSheetsNew(string paramString)
		{
			ActionResult result = new ActionResult(false, "");

			Dictionary<string, string> dicParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString);
			bool isMobile = false;
			string LoginName = string.Empty;
			string MobileToken = string.Empty;
			string WechatCode = string.Empty;
			string EngineCode = string.Empty;
			foreach (string key in dicParams.Keys)
			{
				if (key == Param_WorkItemID) { WorkItemID = dicParams[key]; continue; }
				if (key == Param_Mode) { SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), dicParams[key]); continue; }
				if (key == Param_IsMobile)
				{
					bool.TryParse(dicParams[key], out isMobile);
					IsMobile = isMobile;
					continue;
				}
				if (key.ToLower() == "loginname") { LoginName = dicParams[key]; }
				if (key.ToLower() == "mobiletoken") { MobileToken = dicParams[key]; }
				if (key.ToLower() == "code") { WechatCode = dicParams[key]; }
				if (key.ToLower() == "state") { EngineCode = dicParams[key]; }
			} 

			//APP打开表单验证
			if (!string.IsNullOrEmpty(LoginName) && !string.IsNullOrEmpty(MobileToken) && this.UserValidator == null)
			{
				if (!SSOopenSheet(LoginName, MobileToken))
				{
					result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
					return Json(result, JsonRequestBehavior.AllowGet);
				}
			}
			else if (this.UserValidator == null)
			{
				result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
             
            Instance.InstanceContext instanceContext = null;

            instanceContext = this.Engine.InstanceManager.GetInstanceContext(CurrentWorkItem.InstanceId);
            bool customAuthorized = SheetUtility.ValidateAuthorization(this.UserValidator,
                SheetDataType.Workflow,
                false,
                CurrentWorkItem.SheetCode,
                null,
                SheetMode.View,
                CurrentWorkItem.WorkflowCode,
                null,
                null,
                instanceContext);
            if (customAuthorized)
            {
                //验证关联关系流程权限
                MvcViewContext sheet = new MvcViewContext()
                {
                    Message = Configs.Global.ResourceManager.GetString("MvcController_Perission"),
                    Close = true
                };
                result.Success = false;
                result.Message = sheet.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

			// 解析Url地址
			if (SheetMode == SheetMode.Work)
			{
				if (CurrentWorkItem != null)
				{
					url = this.GetWorkSheetUrl(
					CurrentWorkItem,
					WorkItemSheet,
					IsMobile);
				}
				else
				{
					url = this.GetViewCirculateItemSheetUrl(
							CurrentCirculateItem,
							WorkItemSheet,
							SheetMode,
							IsMobile);
				}
			}
			else
			{
				if (CurrentWorkItem != null)
				{
					url = this.GetViewSheetUrl(
						   CurrentWorkItem,
						   WorkItemSheet,
						   SheetMode,
						   IsMobile);
				}
				else
				{
					url = this.GetViewCirculateItemSheetUrl(
						CurrentCirculateItem,
						WorkItemSheet,
						SheetMode,
						IsMobile);
				}
			}
			// 将其中的数据参数做转换
			if (url.Contains(OThinker.H3.Math.Variant.VariablePrefix.ToString()))
			{
				url = InstanceData.ParseText(url);
			}
			// 处理缓存
			DateTime t = DateTime.Now;
			url += "&T=" + t.ToString("HHmmss") + WorkItemID.Substring(0, 8);
			if (SheetMode == SheetMode.Print)
			{
				url += "Print";
			}
			result.Success = true;
			result.Message = url;
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 工作任务的URL
		/// </summary>
		private string url = string.Empty;

		private bool _IsMobile = false;
		private bool IsMobile
		{
			get
			{
				return this._IsMobile;
			}
			set
			{
				this._IsMobile = value;
			}
		}
		private string _WorkItemID = string.Empty;
		/// <summary>
		/// 获取当前任务ID
		/// </summary>
		private string WorkItemID
		{
			get
			{
				return this._WorkItemID;
			}
			set
			{
				this._WorkItemID = value;
			}
		}
		private Sheet.BizSheet workItemSheet = null;
		/// <summary>
		/// 获取工作任务的表单对象
		/// </summary>
		public Sheet.BizSheet WorkItemSheet
		{
			get
			{
				if (this.workItemSheet == null)
				{
					if (CurrentWorkItem != null)
					{
						this.workItemSheet = this.Engine.BizSheetManager.GetBizSheetByCode(CurrentWorkItem.SheetCode);
					}
					else
					{
						this.workItemSheet = this.Engine.BizSheetManager.GetBizSheetByCode(CurrentCirculateItem.SheetCode);
					}
                    if (this.workItemSheet == null)
                    {
                        string schemaCode = string.Empty;
                        if (this.CurrentWorkItem != null) schemaCode = this.Engine.InstanceManager.GetInstanceContext(this.CurrentWorkItem.InstanceId).BizObjectSchemaCode;
                        else if (this.CurrentCirculateItem != null) schemaCode = this.Engine.InstanceManager.GetInstanceContext(this.CurrentCirculateItem.InstanceId).BizObjectSchemaCode;
                        BizSheet[] sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(schemaCode);
                        if (sheets != null && sheets.Length > 0) this.workItemSheet = sheets[0];
                    }
				}
				return this.workItemSheet;
			}
		}
		private Instance.InstanceData _InstanceData = null;
		/// <summary>
		/// 获取当前任务流程数据
		/// </summary>
		private H3.Instance.InstanceData InstanceData
		{
			get
			{
				if (this._InstanceData == null)
				{
					this._InstanceData = new Instance.InstanceData(this.Engine, CurrentWorkItem.InstanceId, this.UserValidator.UserID);
				}
				return this._InstanceData;
			}
		}
		private WorkItem.WorkItem _CurrentWorkItem = null;
		/// <summary>
		/// 获取当前工作任务
		/// </summary>
		private WorkItem.WorkItem CurrentWorkItem
		{
			get
			{
				if (this._CurrentWorkItem == null)
				{
					this._CurrentWorkItem = this.Engine.WorkItemManager.GetWorkItem(WorkItemID);
				}
				return this._CurrentWorkItem;
			}
		}

		private WorkItem.CirculateItem _CurrentCirculateItem = null;
		/// <summary>
		/// 获取当前工作任务
		/// </summary>
		private WorkItem.CirculateItem CurrentCirculateItem
		{
			get
			{
				if (this._CurrentCirculateItem == null)
				{
					this._CurrentCirculateItem = this.Engine.WorkItemManager.GetCirculateItem(WorkItemID);
				}
				return this._CurrentCirculateItem;
			}
		}

		private SheetMode _SheetMode = SheetMode.Unspecified;
		/// <summary>
		/// 获取当前任务表单打开模式
		/// </summary>
		private SheetMode SheetMode
		{
			get
			{
				if (this._SheetMode == SheetMode.Unspecified)
				{
					if (CurrentCirculateItem != null)
					{
						if (CurrentCirculateItem.State == WorkItem.WorkItemState.Finished)
						{
							this._SheetMode = SheetMode.View;
						}
						else
						{
							this._SheetMode = SheetMode.Work;
						}
					}
					else
					{
						if (CurrentWorkItem.State == WorkItem.WorkItemState.Finished || CurrentWorkItem.State == WorkItem.WorkItemState.Canceled)
						{
							this._SheetMode = SheetMode.View;
						}
						else
						{
							this._SheetMode = SheetMode.Work;
						}

						if (this._SheetMode == SheetMode.Work && (CurrentWorkItem.State == WorkItem.WorkItemState.Finished || CurrentWorkItem.State == WorkItem.WorkItemState.Canceled))
						{
							this._SheetMode = SheetMode.View;
						}
					}
				}
				return this._SheetMode;
			}
			set
			{
				this._SheetMode = value;
			}
		}
	}
}
