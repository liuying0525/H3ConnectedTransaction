using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class WorkflowTemplateSettingController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; ; }
        }
        /// 流程模板默认图标地址
        /// </summary>
        string DefautIconImagePath
        {
            get
            {
                return  this.PortalRoot+"/WFRes/image/Workflow_DefaultIcon.png";
            }
        }

        /// <summary>
        /// 流程图标文件夹
        /// </summary>
        string WorkflowIconDirectory
        {
            get
            {
                return "TempImages/Workflow";
            }
        }

        string _WorkflowIconRootPath;
        /// <summary>
        /// 流程模板图标文件根路径
        /// </summary>
        string WorkflowIconRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(this._WorkflowIconRootPath))
                {
                    this._WorkflowIconRootPath = AppDomain.CurrentDomain.BaseDirectory + WorkflowIconDirectory;
                }
                return this._WorkflowIconRootPath;
            }
        }
        public JsonResult Load(string WorkflowCode)
        {
            return ExecuteFunctionRun(() => {
                WorkflowClauseViewModel model = new WorkflowClauseViewModel();

                WorkflowTemplate.WorkflowClause SelectedClause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                var WorkflowVersions = LoadVersion(WorkflowCode);

                // 流水号的重置策略
                var SeqnoResetTypes = LoadSeqNoResetTypes();
                //工作日历
                var WorkCalendar = LoadWorkCalendar();

                int defaultVersion = this.Engine.WorkflowManager.GetWorkflowDefaultVersion(WorkflowCode);
                model.WorkflowCode = WorkflowCode;
                model.WorkflowVersion = defaultVersion.ToString();
                model.IsControlUsable = BizWorkflowPackageLockByID(WorkflowCode);
                model.SeqNoResetType = Instance.SequenceNoResetType.None.ToString();//默认值
                var returnValue=new{

                    IsControlUsable=true,//是否可编辑
                    WorkflowVersions=WorkflowVersions, //流程版本
                    SeqNoResetTypes=SeqnoResetTypes,//流水号重置策略
                    CalendarList=WorkCalendar,//工作日历
                    WorkflowClause=model //运行参数
                };

                //还没保存过运行参数
                if (SelectedClause == null)
                {
                    return Json(returnValue, JsonRequestBehavior.AllowGet);
                }

                //code、名称、排序 都读取节点信息，同步保存到策略上
                model.SequenceCode = SelectedClause.SequenceCode??"";
                model.DisplayName = SelectedClause.WorkflowName;
                model.SortKey = SelectedClause.SortKey;
                // 工作日历
                model.WorkCalendar = SelectedClause.CalendarId;
                // 异常管理员
                model.ExceptionManager = SelectedClause.ExceptionManager;
               
                //流程号重置方式
                Instance.SequenceNoResetType sequenceNoResetType = SelectedClause.SeqNoResetType;
                model.SeqNoResetType = sequenceNoResetType.ToString();
                // 获得选中的工作流模板的状态
                WorkflowTemplate.WorkflowState state = SelectedClause.State;
                model.IsActive = state == OThinker.H3.WorkflowTemplate.WorkflowState.Active;
                // 流程模板的移动办公设置
                model.MobileStart = SelectedClause.MobileStart;
                // 加载图标
                model.ImageUrl = string.IsNullOrEmpty(SelectedClause.IconFileName) ? this.DefautIconImagePath : (this.PortalRoot+"/" + this.WorkflowIconDirectory + "/" + SelectedClause.IconFileName);

                return Json(returnValue, JsonRequestBehavior.AllowGet);
            });
        }

       
        public JsonResult Save(WorkflowClauseViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                System.Web.HttpFileCollectionBase files = HttpContext.Request.Files;
                WorkflowTemplate.WorkflowClause SelectedClause = this.Engine.WorkflowManager.GetClause(model.WorkflowCode);
                // 检查输入
                int sortKey = model.SortKey;

                // 更新模板
                SelectedClause.WorkflowName = model.DisplayName;
                SelectedClause.SortKey = sortKey;

                // 移动办公设置
                SelectedClause.MobileStart = model.MobileStart;

                // 流水号编码
                SelectedClause.SequenceCode = model.SequenceCode??"";
                // 流水号重置方式
                SelectedClause.SeqNoResetType = (Instance.SequenceNoResetType)Enum.Parse(typeof(Instance.SequenceNoResetType), model.SeqNoResetType);

                SelectedClause.State = model.IsActive ? WorkflowTemplate.WorkflowState.Active : OThinker.H3.WorkflowTemplate.WorkflowState.Inactive;

                SelectedClause.CalendarId = model.WorkCalendar;
                //异常管理员 
                SelectedClause.ExceptionManager = model.ExceptionManager;

                //旧图标地址
                string oldImagePath = SelectedClause.IconFileName;
                // 设置图标
                if (files.Count>0 && files[0].FileName.Length>0)
                {
                    var fileIcon = files[0];
                    if (!System.IO.Directory.Exists(WorkflowIconRootPath))
                    {
                        System.IO.Directory.CreateDirectory(WorkflowIconRootPath);
                    }

                    string fileExt = System.IO.Path.GetExtension(fileIcon.FileName);
                    if (!(fileExt == ".png" || fileExt == ".jpg" || fileExt == ".gif" || fileExt == ".bmp"))
                    {
                        result.Message = "WorkflowSetting.WorkflowTemplateSetting_ImageFormat";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    //记录旧图标
                    oldImagePath = SelectedClause.IconFileName;
                    SelectedClause.IconFileName = Guid.NewGuid().ToString().ToLower() + fileExt;

                    fileIcon.SaveAs(WorkflowIconRootPath + "/" + SelectedClause.IconFileName);

                }

                //设置默认版本号
                int defaultVersion = SelectedClause.DefaultVersion;
                int.TryParse(model.WorkflowVersion,out defaultVersion);
                SelectedClause.DefaultVersion = defaultVersion;
                
                long result2 = this.Engine.WorkflowManager.UpdateClause(SelectedClause);
                if (result2 != ErrorCode.SUCCESS)
                {
                    result.Message = "msgGlobalString.UpdateFailed";
                    return Json(result,"text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //删除旧图标
                    try
                    {
                        if (oldImagePath != SelectedClause.IconFileName)
                        {
                            System.IO.File.Delete(WorkflowIconRootPath + "/" + oldImagePath);
                        }
                    }
                    catch { }

                    // 通知结果
                    result.Message = "msgGlobalString.SaveSucced";
                    result.Success = true;
                    return Json(result, "text/html", JsonRequestBehavior.AllowGet);
                }
            });
        }

        public JsonResult GetWorkflowAcl(string WorkflowCode)
        {
            return ExecuteFunctionRun(() => {
                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
                Dictionary<string, object> data;
                System.Data.DataTable WorkflowAclTable = this.Engine.Query.QueryWorkflowAcl(WorkflowCode);
                foreach (System.Data.DataRow row in WorkflowAclTable.Rows)
                {
                    data = new Dictionary<string, object>();
                    data.Add("ObjectID", row["ObjectID"]);
                    data.Add("UserID", GetFullName(row["UserID"].ToString()));
                    data.Add("CreateInstance", row["CreateInstance"]);
                    dataList.Add(data);
                }
                var GridData = CreateLigerUIGridData(dataList.ToArray());
                return Json(GridData,JsonRequestBehavior.AllowGet);
            });
        }

        private string GetFullName(string unitId)
        {
            string UserAlias=string.Empty;
            // 编辑模式
            OThinker.Organization.Unit u = this.Engine.Organization.GetUnit(unitId);
            if (u is OThinker.Organization.User)
            {
                UserAlias = this.Engine.Organization.GetFullName(u.ParentID) + "/" + u.Name;
            }
            else
            {
                UserAlias = this.Engine.Organization.GetFullName(unitId);
            }

            return UserAlias;
        }

        public JsonResult DeleteWorkflowAcl(string AclID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                if (!string.IsNullOrEmpty(AclID))
                    this.Engine.WorkflowAclManager.Delete(AclID);

                return Json(result);
            });
        }

        #region
        /// <summary>
        /// 加载流程版本
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <returns></returns>
        List<object> LoadVersion(string WorkflowCode)
        {
            int[] versions = this.Engine.WorkflowManager.GetWorkflowVersions(WorkflowCode);
            List<object> lstObj = new List<object>();

            if (versions == null || versions.Length == 0)
            {
                return lstObj;
            }
            else
            {
                foreach (int version in versions)
                {
                    lstObj.Add(new { Text = version, Value = version });
                }
                //


                return lstObj;
            }
        }

        /// <summary>
        /// 流水号重置策略
        /// </summary>
        /// <returns></returns>
        List<object> LoadSeqNoResetTypes()
        {
            List<object> lstObj = new List<object>();
            // 流水号的重置策略
            string[] seqNoResetTypes = Enum.GetNames(typeof(OThinker.H3.Instance.SequenceNoResetType));
            foreach (string name in seqNoResetTypes)
            {
                lstObj.Add(new { Text = "WorkflowSetting.WorkflowTemplateSetting_" + name, Value = name });
            }

            return lstObj;
        }

        List<object> LoadWorkCalendar()
        {

            List<object> lstObj = new List<object>();
            Calendar.WorkingCalendar[] calendars = this.Engine.WorkingCalendarManager.GetCalendarList();
            if (calendars != null)
            {
                foreach (Calendar.WorkingCalendar calendar in calendars)
                {
                    lstObj.Add(new { Text = calendar.DisplayName, Value = calendar.ObjectID });
                }
            }

            return lstObj;
        }

        #endregion

    }
}
