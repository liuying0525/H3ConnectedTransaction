using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.Apps;
using OThinker.H3.BizBus.BizRule;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Calendar;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Settings;
using OThinker.H3.WorkItem;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OThinker.H3.Controllers.AppCode.Admin
{
    public class DeploymentProcess
    {
        ControllerBase _controller = null;

        private HttpContextBase Context;

        private HttpRequestBase Request;

        private HttpResponseBase Response;

        private HttpSessionStateBase Session;
        #region 构造函数
        public DeploymentProcess(ControllerBase controller, string LoginName, string Password)
        {
            _controller = controller;
            this.Context = controller.HttpContext;
            this.Request = controller.Request;
            this.Response = controller.Response;
            this.Session = controller.Session;

            this.LoginName = LoginName;
            this.Password = Password;
        }
        #endregion

        #region 变量
        private string _Code;
        /// <summary>
        /// 编码
        /// </summary>
        public string Code
        {
            get
            {
                return this._Code;
            }
        }

        private FunctionNodeType _NodeType;
        /// <summary>
        /// 类型
        /// </summary>
        private FunctionNodeType NodeType
        {
            get
            {
                return this._NodeType;
            }
        }

        private string LoginName;
        private string Password;

        public Clusterware.ConnectionResult ConnResult;
        /// <summary>
        /// 部署服务器引擎
        /// </summary>
        public IEngine DeloymenEngine
        {
            get
            {
                if (this.Session["_DeloymentEngine"] == null)
                {
                    string Host = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentServerHost);
                    string Port = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentServerPort);
                    string EngineCode = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentEngineCode);
                    //string LoginName = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentLoginName);
                    //string Password = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentPassword);

                    string ConnString = string.Format("Servers={0}:{1};User={2};Password={3};Engine={4}", Host, Port, LoginName, Password, EngineCode);

                    OThinker.H3.Connection conn = new Connection();
                    ConnResult = conn.Open(ConnString);
                    if (ConnResult == Clusterware.ConnectionResult.Success)
                        this.Session["_DeloymentEngine"] = conn.Engine;
                    //this._DeloymentEngine = conn.Engine;
                }
                return (IEngine)this.Session["_DeloymentEngine"];//this._DeloymentEngine;
            }
        }

        #endregion

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public object Connection()
        {
            return ConnectDeploymentEngine();
        }

        /// <summary>
        /// 断开
        /// </summary>
        public void Dispose()
        {
            this.Session.Remove("_DeloymentEngine");
        }

        /// <summary>
        /// 一键部署
        /// </summary>
        /// <param name="NotyType"></param>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        public object OneButtonDeployment(FunctionNodeType NodeType, string FunctionCode)
        {
            //System.Threading.Thread.Sleep(1000);
            this._NodeType = NodeType;
            this._Code = FunctionCode;

            //测试连接部署引擎
            HandlerResult result = ConnectDeploymentEngine();
            if (result.State)
            {
                switch (this.NodeType)
                {
                    case FunctionNodeType.ServiceFolder://业务服务目录
                    case FunctionNodeType.RuleFolder://业务规则目录
                    case FunctionNodeType.BizFolder://主数据目录
                    case FunctionNodeType.BizWFFolder://流程目录
                    case FunctionNodeType.Report://报表模型qiancheng
                    case FunctionNodeType.ReportFolder://报表页签
                    case FunctionNodeType.ReportTemplateFolder://报表目录
                        result = DeploymentFunctionNode(this.Code);
                        break;
                    case FunctionNodeType.ReportFolderPage://报表目录
                        result = DeploymentReport(this.Code);
                        break;
                    case FunctionNodeType.BizObject:
                        result = DeploymentBizObject(this.Code);
                        break;
                    case FunctionNodeType.BizWorkflowPackage://流程包
                        result = DeploymentPackage(this.Code);
                        break;
                    case FunctionNodeType.BizService:
                    case FunctionNodeType.BizRule:
                        result = DeploymentBizService(this.Code);
                        break;
                    case FunctionNodeType.AppNavigation: //应用程序
                        result = DeploymentApp(this.Code);
                        break;
                    case FunctionNodeType.BPAReportSource:
                        result = DeploymentReportSource(this.Code);
                        break;
                    case FunctionNodeType.Function:
                        result = DeploymentFunction(this.Code);
                        break;
                    case FunctionNodeType.BPAReportTemplate:
                        result = DeploymentReportTemplate(this.Code);
                        break;
                    default:
                        return new HandlerResult("DeploymentProcess.Msg0", "", this.NodeType.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private HandlerResult DeploymentFunction(string FunctionCode)
        {
            HandlerResult result = new HandlerResult();
            switch (FunctionCode)
            {
                case FunctionNode.Category_Organization_Code://组织模型
                    result = DeploymentOrg();
                    break;
                case FunctionNode.SysParam_CommonParam_Code:
                    result = DeploymentCommonParam();
                    break;
                case FunctionNode.SysParam_SystemAcl_Code:
                    result = DeploymentSysAdministrator();
                    break;
                case FunctionNode.SysParam_SystemOrgAcl_Code:
                    result = DeploymentSystemOrgAcl();
                    break;
                case FunctionNode.SysParam_SettingsDisplay_Code:
                    result = DeploymentSettingsDisplay();
                    break;
                case FunctionNode.SysParam_MessageSetting_Code:
                    result = DeploymentMessageSetting();
                    break;
                case FunctionNode.SysParam_WorkingCalendar_Code:
                    result = DeploymentWorkingCalendar();
                    break;
                case FunctionNode.SysParam_DataDictionary_Code:
                    result = DeploymentDataDictionary();
                    break;
                case FunctionNode.SysParam_GlobalData_Code:
                    result = DeploymentGlobalData();
                    break;
                case FunctionNode.SysParam_MobileDevice_Code:
                    result = DeploymentMobileDevice();
                    break;
                case FunctionNode.SysParam_QueryAgent_Code:
                    result = DeploymentAgent();
                    break;
                case FunctionNode.SysParam_ServerMonitor_Code:
                    break;
                case FunctionNode.BizBus_EditBizDbConfig_Code:
                    result = DeploymentBizDbConfig();
                    break;
                case FunctionNode.BizBus_EditBizAccount_Code:
                    result = DeploymentBizAccount();
                    break;
                case FunctionNode.SysParam_StatusSetting_Code:
                    result = DeploymentStatusSetting();
                    break;
            }
            return result;
        }

        #region 测试连接部署引擎
        /// <summary>
        /// 测试连接部署引擎
        /// </summary>
        private HandlerResult ConnectDeploymentEngine()
        {
            if (this.DeloymenEngine != null)
            {
                return new HandlerResult();
            }

            if (ConnResult == Clusterware.ConnectionResult.Success)
            {
                return new HandlerResult();
            }
            else
            {
                return new HandlerResult("DeploymentProcess.Msg1", "", ConnResult.ToString());
            }
        }
        #endregion

        #region 部署FunctionNode,目录类
        /// <summary>
        /// 部署FunctionNode
        /// </summary>
        /// <param name="FunctionCode"></param>
        private HandlerResult DeploymentFunctionNode(string FunctionCode)
        {
            FunctionNode Node = _controller.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            if (Node == null || Node.NodeType != this.NodeType)
            {
                return new HandlerResult("DeploymentProcess.Msg2", FunctionCode);
            }

            FunctionNode TargetNode = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            long result = ErrorCode.SUCCESS;
            if (TargetNode == null)
            {
                //由于engine新增数据为多线程异步操作，所以不能保障在子节点同步时，父节点已新增完成
                //此处的判断只是做提示使用，没有功能性作用，未避免错误提示。注释这段代码
                //if (!string.IsNullOrWhiteSpace(Node.ParentCode))
                //{
                //    if (this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(Node.ParentCode) == null)
                //    {
                //        return new HandlerResult("DeploymentProcess.Msg3", new List<string>() { Node.DisplayName, Node.ParentCode, "" });
                //    }
                //}
                Node.Serialized = false;
                Node.ObjectID = Guid.NewGuid().ToString();
                result = this.DeloymenEngine.FunctionAclManager.AddFunctionNode(Node);
                if (result != ErrorCode.SUCCESS)
                {
                    return new HandlerResult("DeploymentProcess.Msg4", Node.DisplayName, result.ToString());
                }
            }
            else if (TargetNode.NodeType != this.NodeType)
            {
                return new HandlerResult("DeploymentProcess.Msg5", new List<string>() { FunctionCode, TargetNode.DisplayName, TargetNode.NodeType.ToString() });
            }
            else
            {
                TargetNode.DisplayName = Node.DisplayName;
                TargetNode.IconUrl = Node.IconUrl;
                TargetNode.IsSystem = Node.IsSystem;
                TargetNode.ParentCode = Node.ParentCode;
                TargetNode.SortKey = Node.SortKey;
                TargetNode.Url = Node.Url;

                result = this.DeloymenEngine.FunctionAclManager.UpdateFunctionNode(TargetNode);
                if (result != ErrorCode.SUCCESS)
                {
                    return new HandlerResult("DeploymentProcess.Msg4", Node.DisplayName, result.ToString());
                }
            }

            return new HandlerResult();
        }
        #endregion

        #region 部署流程包
        /// <summary>
        /// 部署流程包
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private HandlerResult DeploymentPackage(string FunctionCode)
        {
            FunctionNode PackageNode = _controller.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            if (PackageNode == null || PackageNode.NodeType != this.NodeType)
            {
                return new HandlerResult("DeploymentProcess.Msg6");
            }

            // 创建父节点
            this.CreateDirectory(PackageNode.ParentCode);
            //if (!IsExistFunctionNode(PackageNode.ParentCode))
            //{
            //    return new HandlerResult("DeploymentProcess.Msg7");
            //}
            DataModel.BizObjectSchema Schema = _controller.Engine.BizObjectManager.GetDraftSchema(FunctionCode);
            DataModel.BizListenerPolicy BizListenerPolicy = _controller.Engine.BizObjectManager.GetListenerPolicy(FunctionCode);
            DataModel.ScheduleInvoker[] ScheduleInvokers = _controller.Engine.BizObjectManager.GetScheduleInvokerList(FunctionCode);
            DataModel.BizQuery[] BizQuerys = _controller.Engine.BizObjectManager.GetBizQueries(FunctionCode);
            Sheet.BizSheet[] Sheets = _controller.Engine.BizSheetManager.GetBizSheetBySchemaCode(FunctionCode);
            WorkflowTemplate.WorkflowClause[] WorkflowClauses = _controller.Engine.WorkflowManager.GetClausesBySchemaCode(FunctionCode);
            List<WorkflowTemplate.DraftWorkflowTemplate> WorkflowTemplates = new List<WorkflowTemplate.DraftWorkflowTemplate>();
            Dictionary<string, string> WorkflowNames = new Dictionary<string, string>();
            if (WorkflowClauses != null)
            {
                foreach (WorkflowTemplate.WorkflowClause Clauses in WorkflowClauses)
                {
                    WorkflowTemplate.DraftWorkflowTemplate t = _controller.Engine.WorkflowManager.GetDraftTemplate(Clauses.WorkflowCode);
                    if (t != null)
                    {
                        WorkflowTemplates.Add(t);
                        WorkflowNames.Add(Clauses.WorkflowCode, Clauses.WorkflowName);
                    }
                }
            }

            FunctionNode TargetNode = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);

            if (TargetNode != null && TargetNode.NodeType != this.NodeType)
            {
                return new HandlerResult("DeploymentProcess.Msg5", new List<string> { FunctionCode, TargetNode.DisplayName, TargetNode.NodeType.ToString() });
            }

            //查找部署服务器上的数据，如果找到，则用更新的方式（修改ObjectID根目标一致）,如果找不到则用新增的方式（修改Serialized和ObjectID)
            RepairQuerys(FunctionCode, BizQuerys);

            //定时作业
            if (ScheduleInvokers != null)
            {
                foreach (var scheduleInvoker in ScheduleInvokers)
                {
                    scheduleInvoker.ObjectID = Guid.NewGuid().ToString();
                    scheduleInvoker.Serialized = false;
                }
            }

            //表单
            if (Sheets != null)
            {
                foreach (Sheet.BizSheet sheet in Sheets)
                {
                    Sheet.BizSheet serverSheet = this.DeloymenEngine.BizSheetManager.GetBizSheetByCode(sheet.SheetCode);
                    if (serverSheet == null)
                    {
                        sheet.Serialized = false;
                        sheet.ObjectID = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        sheet.ObjectID = serverSheet.ObjectID;
                    }
                }
            }
            Unit Administrator = this.DeloymenEngine.Organization.GetUserByCode(this.LoginName);

            string resultStr = string.Empty;
            bool result = this.DeloymenEngine.AppPackageManager.ImportAppPackage(
                    Administrator.ObjectID,
                    PackageNode.ParentCode,
                    PackageNode.Code,
                    PackageNode.DisplayName,
                    Schema,
                    BizListenerPolicy,
                    BizQuerys == null ? new List<DataModel.BizQuery>() : BizQuerys.ToList(),
                    ScheduleInvokers == null ? new List<DataModel.ScheduleInvoker>() : ScheduleInvokers.ToList(),
                    Sheets == null ? new List<Sheet.BizSheet>() : Sheets.ToList(),
                    WorkflowTemplates,
                    WorkflowNames,
                    true,
                    out resultStr);

            if (!result)
            {
                return new HandlerResult(resultStr);
            }

            //流程包排序号
            TargetNode = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(PackageNode.Code);
            TargetNode.SortKey = PackageNode.SortKey;
            this.DeloymenEngine.FunctionAclManager.UpdateFunctionNode(TargetNode);


            foreach (WorkflowTemplate.WorkflowClause Clause in WorkflowClauses)
            {
                WorkflowTemplate.WorkflowClause targetClause = this.DeloymenEngine.WorkflowManager.GetClause(Clause.WorkflowCode);
                targetClause.CalendarId = Clause.CalendarId;
                targetClause.ExceptionManager = Clause.ExceptionManager;
                //ERROR:将图标转存到目标服务器
                targetClause.IconFileName = null;//Clause.IconFileName;
                targetClause.MobileStart = Clause.MobileStart;
                targetClause.SequenceCode = Clause.SequenceCode;
                targetClause.SeqNoResetType = Clause.SeqNoResetType;
                targetClause.SortKey = Clause.SortKey;
                targetClause.State = Clause.State;
                this.DeloymenEngine.WorkflowManager.UpdateClause(targetClause);
            }
            string msg = "";
            if (!this.DeloymenEngine.BizObjectManager.PublishSchema(FunctionCode, out msg))
            {
                return new HandlerResult(msg);
            }

            foreach (string workflowCode in WorkflowNames.Keys)
            {
                this.DeloymenEngine.WorkflowManager.RegisterWorkflow(LoginName, workflowCode, true);
            }
            //处理权限
            foreach (var item in WorkflowClauses)
            {
                var Acls = _controller.Engine.WorkflowAclManager.GetWorkflowAcls(item.WorkflowCode);
                this.DeloymenEngine.WorkflowAclManager.RemoveByWorkflow(item.WorkflowCode);
                foreach (var Acl in Acls)
                {
                    if (this.DeloymenEngine.WorkflowAclManager.GetWorkflowAcl(Acl.AclID) == null)
                    {
                        Acl.Serialized = false;
                        this.DeloymenEngine.WorkflowAclManager.Add(Acl);
                    }
                    else
                    {
                        this.DeloymenEngine.WorkflowAclManager.Update(new WorkflowAcl[] { Acl });
                    }
                }
            }
            return new HandlerResult();
        }

        /// <summary>
        /// 修复数据
        /// </summary>
        /// <param name="FuncitonCode"></param>
        /// <param name="BizQuerys"></param>
        private void RepairQuerys(string FuncitonCode, DataModel.BizQuery[] BizQuerys)
        {
            if (BizQuerys != null)
            {
                foreach (DataModel.BizQuery q in BizQuerys)
                {
                    q.ObjectID = Guid.NewGuid().ToString();
                    q.Serialized = false;

                    if (q.Columns != null)
                    {
                        foreach (BizQueryColumn c in q.Columns)
                        {
                            c.Serialized = false;
                        }
                    }

                    if (q.BizActions != null)
                    {
                        foreach (BizQueryAction a in q.BizActions)
                        {
                            a.Serialized = false;
                        }
                    }

                    if (q.QueryItems != null)
                    {
                        foreach (BizQueryItem i in q.QueryItems)
                        {
                            i.Serialized = false;
                        }
                    }
                }

                DataModel.BizQuery[] BizQuerys1 = this.DeloymenEngine.BizObjectManager.GetBizQueries(FuncitonCode);
                if (BizQuerys1 != null)
                {
                    foreach (DataModel.BizQuery q1 in BizQuerys1)
                    {
                        DataModel.BizQuery q = BizQuerys.Where(p => p.QueryCode == q1.QueryCode).SingleOrDefault();
                        if (q != null)
                        {
                            q.ObjectID = q1.ObjectID;
                            q.Serialized = true;
                            //设置属性为脏
                            q.SetPropertyDirty(BizQuery.PropertyName_DisplayName);
                            q.SetPropertyDirty(BizQuery.PropertyName_ListMethod);
                            q.SetPropertyDirty(BizQuery.PropertyName_ListDefault);
                            q.SetPropertyDirty(BizQuery.PropertyName_Columns);
                            q.SetPropertyDirty(BizQuery.PropertyName_QueryItems);
                            q.SetPropertyDirty(BizQuery.PropertyName_BizQueryActions);

                            if (q.Columns != null)
                            {
                                foreach (BizQueryColumn c in q.Columns)
                                {
                                    c.Serialized = true;
                                }
                            }

                            if (q.BizActions != null)
                            {
                                foreach (BizQueryAction a in q.BizActions)
                                {
                                    a.Serialized = true;
                                }
                            }

                            if (q.QueryItems != null)
                            {
                                foreach (BizQueryItem i in q.QueryItems)
                                {
                                    i.Serialized = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 判断code是否存在目标的Functonnode里面
        /// <summary>
        /// 判断code是否存在目标的Functonnode里面
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private bool IsExistFunctionNode(string FunctionCode)
        {
            return this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode) != null;
        }

        /// <summary>
        /// 构建流程目录
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private void CreateDirectory(string FunctionCode)
        {
            if (this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode) != null)
            {// 已经存在
                return;
            }
            // 需要创建当前目录
            FunctionNode functionNode = AppUtility.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            // 先创建上级目录
            this.CreateDirectory(functionNode.ParentCode);
            // 再创建当级目录
            this.DeloymenEngine.FunctionAclManager.AddFunctionNode(new FunctionNode()
            {
                Code = functionNode.Code,
                ParentCode = functionNode.ParentCode,
                DisplayName = functionNode.DisplayName,
                IconType = functionNode.IconType,
                NodeType = functionNode.NodeType,
                IsSystem = functionNode.IsSystem,
                Url = functionNode.Url,
                OpenNewWindow = functionNode.OpenNewWindow,
                SortKey = functionNode.SortKey,
                State = functionNode.State
            });
        }
        #endregion

        #region 部署业务服务
        /// <summary>
        /// 部署业务服务
        /// </summary>
        /// <param name="BizServiceCode"></param>
        /// <returns></returns>
        private HandlerResult DeploymentBizService(string BizServiceCode)
        {
            BizService Service = _controller.Engine.BizBus.GetBizService(BizServiceCode);
            if (Service == null)
            {
                return new HandlerResult("DeploymentProcess.Msg8");
            }

            if (!string.IsNullOrWhiteSpace(Service.FolderCode))
            {
                //如果存在目录的话，判断目录是否已经存在部署服务器
                CreateDirectory(Service.FolderCode);
                //if (!IsExistFunctionNode(Service.FolderCode))
                //{
                //    return new HandlerResult("DeploymentProcess.Msg9");
                //}
            }

            if (this.NodeType == FunctionNodeType.BizRule)
            {
                return DeploymentBizRule(BizServiceCode, Service.FolderCode);
            }
            else
            {
                BizService TargetService = this.DeloymenEngine.BizBus.GetBizService(BizServiceCode);
                if (TargetService != null)
                {
                    TargetService.AccountCategory = Service.AccountCategory;
                    TargetService.AllowCustomMethods = Service.AllowCustomMethods;
                    TargetService.BizAdapterCode = Service.BizAdapterCode;
                    TargetService.Description = Service.Description;
                    TargetService.DisplayName = Service.DisplayName;
                    TargetService.EnableAccountMapping = Service.EnableAccountMapping;
                    TargetService.Methods = Service.Methods;
                    TargetService.VersionNo = Service.VersionNo;

                    if (!this.DeloymenEngine.BizBus.UpdateBizService(TargetService, true).Valid)
                    {
                        return new HandlerResult("DeploymentProcess.Msg10");
                    }
                }
                else
                {
                    Service.ObjectID = Guid.NewGuid().ToString();
                    Service.Serialized = false;

                    if (Service.Settings != null)
                    {
                        foreach (BizServiceSetting setting in Service.Settings)
                        {
                            setting.ObjectID = Guid.NewGuid().ToString();
                            setting.Serialized = false;
                        }
                    }

                    if (Service.Methods != null)
                    {
                        foreach (BizServiceMethod method in Service.Methods)
                        {
                            method.ObjectID = Guid.NewGuid().ToString();
                            method.Serialized = false;
                        }
                    }

                    ValidationResult result = this.DeloymenEngine.BizBus.AddBizService(Service, true);

                    if (!result.Valid)
                    {
                        return new HandlerResult("DeploymentProcess.Msg11", "", string.Join(";", result.Warnings.ToArray()));
                    }
                }

                return new HandlerResult();
            }
        }
        #endregion

        #region 部署业务规则
        private HandlerResult DeploymentBizRule(string RuleCode, string FolderCode)
        {
            BizRuleTable RuleTale = _controller.Engine.BizBus.GetBizRule(RuleCode);
            if (RuleTale == null)
            {
                return new HandlerResult("DeploymentProcess.Msg12");
            }

            BizRuleTable TargetTable = this.DeloymenEngine.BizBus.GetBizRule(RuleCode);
            if (TargetTable == null)
            {
                if (!this.DeloymenEngine.BizBus.AddBizRule(RuleTale, FolderCode))
                {
                    return new HandlerResult("DeploymentProcess.Msg13");
                }
            }
            else
            {
                if (!this.DeloymenEngine.BizBus.UpdateBizRule(RuleTale))
                {
                    return new HandlerResult("DeploymentProcess.Msg10");
                }
            }

            return new HandlerResult();
        }
        #endregion

        #region 部署主数据
        private HandlerResult DeploymentBizObject(string SchemaCode)
        {
            FunctionNode SchemaNode = _controller.Engine.FunctionAclManager.GetFunctionNodeByCode(SchemaCode);
            BizObjectSchema Schema = _controller.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
            if (SchemaNode == null)
            {
                return new HandlerResult("DeploymentProcess.Msg14");
            }

            if (!string.IsNullOrWhiteSpace(SchemaNode.ParentCode))
            {
                CreateDirectory(SchemaNode.ParentCode);
                //if (!IsExistFunctionNode(SchemaNode.ParentCode))
                //{
                //    return new HandlerResult("DeploymentProcess.Msg15");
                //}
            }

            FunctionNode TargetNode = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(SchemaCode);
            BizObjectSchema TargetSchema = this.DeloymenEngine.BizObjectManager.GetDraftSchema(SchemaCode);
            if (TargetNode != null && TargetNode.NodeType != this.NodeType)
            {
                return new HandlerResult("DeploymentProcess.Msg5", new List<string>() { SchemaCode, TargetNode.DisplayName, TargetNode.NodeType.ToString() });
            }


            if (TargetNode == null)
            {
                if (TargetSchema != null)
                {
                    return new HandlerResult("DeploymentProcess.Msg16", SchemaCode);
                }
                SchemaNode.ObjectID = Guid.NewGuid().ToString();
                SchemaNode.Serialized = false;
                long result = this.DeloymenEngine.FunctionAclManager.AddFunctionNode(SchemaNode);
                if (result != ErrorCode.SUCCESS)
                {
                    return new HandlerResult("DeploymentProcess.Msg17", "", result.ToString());
                }

                if (!this.DeloymenEngine.BizObjectManager.AddDraftSchema(Schema))
                {
                    return new HandlerResult("DeploymentProcess.Msg18");
                }
            }
            else
            {
                TargetNode.DisplayName = SchemaNode.DisplayName;
                TargetNode.IconUrl = SchemaNode.IconUrl;
                TargetNode.IsSystem = SchemaNode.IsSystem;
                TargetNode.ParentCode = SchemaNode.ParentCode;
                TargetNode.SortKey = SchemaNode.SortKey;
                TargetNode.Url = SchemaNode.Url;
                long result = this.DeloymenEngine.FunctionAclManager.UpdateFunctionNode(TargetNode);
                if (result != ErrorCode.SUCCESS)
                {
                    return new HandlerResult("DeploymentProcess.Msg19", "", result.ToString());
                }

                if (TargetSchema != null)
                {
                    if (!this.DeloymenEngine.BizObjectManager.UpdateDraftSchema(Schema))
                    {
                        return new HandlerResult("DeploymentProcess.Msg20");
                    }
                }
                else
                {
                    if (!this.DeloymenEngine.BizObjectManager.AddDraftSchema(Schema))
                    {
                        return new HandlerResult("DeploymentProcess.Msg18");
                    }
                }
            }

            //查询列表
            BizQuery[] BizQuerys = _controller.Engine.BizObjectManager.GetBizQueries(SchemaCode);
            if (BizQuerys != null && BizQuerys.Length != 0)
            {
                RepairQuerys(SchemaCode, BizQuerys);
                foreach (BizQuery BizQuery in BizQuerys)
                {
                    if (BizQuery.Serialized == false)
                    {
                        this.DeloymenEngine.BizObjectManager.AddBizQuery(BizQuery);
                    }
                    else
                    {
                        this.DeloymenEngine.BizObjectManager.UpdateBizQuery(BizQuery);
                    }
                }
            }

            //权限
            DeploymentBizObjectAcl(SchemaCode);

            string msg = "";
            if (!this.DeloymenEngine.BizObjectManager.PublishSchema(SchemaCode, out msg))
            {
                return new HandlerResult(msg);
            }
            return new HandlerResult();
        }

        private void DeploymentBizObjectAcl(string SchemaCode)
        {
            BizObjectAcl[] bizObjectAcls = _controller.Engine.BizObjectManager.GetBizObjectAcls(SchemaCode, null);
            if (bizObjectAcls != null)
            {
                BizObjectAcl[] targetBizObjectAcls = this.DeloymenEngine.BizObjectManager.GetBizObjectAcls(SchemaCode, null);
                foreach (BizObjectAcl bizObjectAcl in bizObjectAcls)
                {
                    //当部署环境中不存在UserID，不部署该权限
                    if (this.DeloymenEngine.Organization.GetUnit(bizObjectAcl.UserID) == null)
                    {
                        continue;
                    }

                    BizObjectAcl targetBizObjectAcl = null;
                    if (targetBizObjectAcls != null && targetBizObjectAcls.Length > 0)
                    {
                        targetBizObjectAcl = targetBizObjectAcls.FirstOrDefault(i => i.UserID == bizObjectAcl.UserID
                            && i.OrgScopeType == bizObjectAcl.OrgScopeType
                            && i.OrgScope == bizObjectAcl.OrgScope);
                    }
                    if (targetBizObjectAcl == null)
                    {
                        bizObjectAcl.Serialized = false;
                        this.DeloymenEngine.BizObjectManager.AddBizObjectAcl(bizObjectAcl);
                    }
                    else
                    {
                        targetBizObjectAcl.CreateBizObject = bizObjectAcl.CreateBizObject;
                        targetBizObjectAcl.ViewData = bizObjectAcl.ViewData;
                        targetBizObjectAcl.Administrator = bizObjectAcl.Administrator;
                        this.DeloymenEngine.BizObjectManager.UpdateBizObjectAcl(targetBizObjectAcl);
                    }
                }
            }
        }
        #endregion

        #region 部署应用
        private HandlerResult DeploymentApp(string appCode)
        {
            AppNavigation app = _controller.Engine.AppNavigationManager.GetApp(appCode);
            if (app == null)
            {
                return new HandlerResult("DeploymentProcess.Msg21");
            }

            AppNavigation targetApp = this.DeloymenEngine.AppNavigationManager.GetApp(appCode);
            if (targetApp == null)
            {
                app.Serialized = false;
                if (!this.DeloymenEngine.AppNavigationManager.AddApp(app))
                {
                    return new HandlerResult("DeploymentProcess.Msg13");
                }
            }
            else
            {
                targetApp.DisplayName = app.DisplayName;
                targetApp.WeChatID = app.WeChatID;
                targetApp.IconUrl = app.IconUrl;
                targetApp.Description = app.Description;
                targetApp.SortKey = app.SortKey;
                targetApp.VisibleOnPortal = app.VisibleOnPortal;
                targetApp.DockOnHomePage = app.DockOnHomePage;
                targetApp.VisibleOnMobile = app.VisibleOnMobile;
                targetApp.ModifiedTime = DateTime.Now;
                if (!this.DeloymenEngine.AppNavigationManager.UpdateApp(targetApp))
                {
                    return new HandlerResult("DeploymentProcess.Msg10");
                }
            }

            //部署权限
            DeploymentAppMenuAcl(appCode);

            //部署子菜单
            FunctionNode[] level1Menus = _controller.Engine.FunctionAclManager.GetFunctionNodesByParentCode(appCode);
            if (level1Menus != null)
            {
                foreach (FunctionNode level1Menu in level1Menus)
                {
                    DeploymentAppMenu(level1Menu);
                }
            }

            return new HandlerResult();
        }

        private void DeploymentAppMenu(FunctionNode node)
        {
            FunctionNode targetMenu = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(node.Code);
            if (targetMenu == null)
            {
                node.Serialized = false;
                this.DeloymenEngine.FunctionAclManager.AddFunctionNode(node);
            }
            else
            {
                targetMenu.ParentCode = node.ParentCode;
                targetMenu.IsSystem = node.IsSystem;
                targetMenu.DisplayName = node.DisplayName;
                targetMenu.LockedBy = node.LockedBy;
                targetMenu.Description = node.Description;
                targetMenu.SortKey = node.SortKey;
                targetMenu.NodeType = node.NodeType;
                targetMenu.OpenNewWindow = node.OpenNewWindow;
                targetMenu.IconType = node.IconType;
                targetMenu.IconCss = node.IconCss;
                targetMenu.IconUrl = node.IconUrl;
                targetMenu.Url = node.Url;
                this.DeloymenEngine.FunctionAclManager.UpdateFunctionNode(targetMenu);
            }
            //部署子菜单权限
            DeploymentAppMenuAcl(node.Code);
            //递归部署子菜单
            FunctionNode[] childNodes = _controller.Engine.FunctionAclManager.GetFunctionNodesByParentCode(node.Code);
            if (childNodes != null)
            {
                foreach (FunctionNode childNode in childNodes)
                {
                    DeploymentAppMenu(childNode);
                }
            }
        }

        private void DeploymentAppMenuAcl(string code)
        {
            FunctionAcl[] menuAcls = _controller.Engine.FunctionAclManager.GetFunctionAclByCode(code);
            if (menuAcls != null)
            {
                FunctionAcl[] targetMenuAcls = this.DeloymenEngine.FunctionAclManager.GetFunctionAclByCode(code);
                foreach (FunctionAcl menuAcl in menuAcls)
                {
                    //当部署环境中不存在UserID，不部署该权限
                    if (this.DeloymenEngine.Organization.GetUnit(menuAcl.UserID) == null)
                    {
                        continue;
                    }

                    FunctionAcl targerMenuAcl = null;
                    if (targetMenuAcls != null && targetMenuAcls.Length > 0)
                    {
                        targerMenuAcl = targetMenuAcls.FirstOrDefault(i => i.UserID == menuAcl.UserID);
                    }
                    if (targerMenuAcl == null)
                    {
                        menuAcl.Serialized = false;
                        this.DeloymenEngine.FunctionAclManager.Add(menuAcl);
                    }
                    else
                    {
                        targerMenuAcl.Run = menuAcl.Run;
                        targerMenuAcl.ModifiedTime = DateTime.Now;
                        this.DeloymenEngine.FunctionAclManager.Update(targerMenuAcl);
                    }
                }
            }
        }

        #endregion

        #region 部署组织模型
        //对象在本地和部署环境上的Code和ObjectID都相同时，才会执行更新操作
        //Code相同，ObjectID不同，提示ID_INVALIDATE异常
        private HandlerResult DeploymentOrg()
        {
            //StringBuilder info = new StringBuilder();
            List<HandlerResult> info = new List<HandlerResult>();
            DeploymentCategory(info);
            DeploymentJob(info);
            DeploymentCompany(info);
            DeploymentSynchronzation(info);

            OrganizationUnit company = _controller.Engine.Organization.RootUnit;
            Unit[] units = _controller.Engine.Organization.GetChildUnits(company.ObjectID, UnitType.OrganizationUnit | UnitType.User | UnitType.Group, true, State.Active).ToArray();
            Unit[] targetUnits = this.DeloymenEngine.Organization.GetChildUnits(company.ObjectID, UnitType.OrganizationUnit | UnitType.User | UnitType.Group, true, State.Active).ToArray();
            if (units != null)
            {
                //记录经理等字段赋空的Unit
                Dictionary<string, string> unitManagerDic = new Dictionary<string, string>();
                Dictionary<string, string> unitViceManagerDic = new Dictionary<string, string>();
                Dictionary<string, string> unitSecretaryDic = new Dictionary<string, string>();
                Dictionary<string, string> unitSupervisorDic = new Dictionary<string, string>();

                Dictionary<string, string[]> unitChildDic = new Dictionary<string, string[]>();

                foreach (Unit unit in units)
                {

                    #region Unit的经理等在部署环境不存在时，为避免部署失败，先将经理等字段赋空，部署完成后再进行更新
                    if (!string.IsNullOrEmpty(unit.ManagerID) && targetUnits.Count(i => i.ObjectID == unit.ManagerID) == 0)
                    {
                        unitManagerDic.Add(unit.ObjectID, unit.ManagerID);
                        unit.ManagerID = Unit.NullID;
                    }
                    if (unit is User)
                    {
                        User user = unit as User;
                        if (!string.IsNullOrEmpty(user.SecretaryID) && targetUnits.Count(i => i.ObjectID == user.SecretaryID) == 0)
                        {
                            unitSecretaryDic.Add(user.Code, user.SecretaryID);

                        }
                        //if (!string.IsNullOrEmpty(user.SupervisorID) && targetUnits.Count(i => i.ObjectID == user.SupervisorID) == 0)
                        //{
                        //    unitSupervisorDic.Add(unit.Code, unit.SupervisorID);
                        //    unit.SupervisorID = Unit.NullID;
                        //}
                    }
                    GroupBase groupBase = unit as GroupBase;
                    if (groupBase != null && groupBase.Children != null && groupBase.Children.Length > 0)
                    {
                        unitChildDic.Add(groupBase.ObjectID, groupBase.Children);
                        groupBase.Children = null;
                    }
                    #endregion

                    switch (unit.UnitType)
                    {
                        case UnitType.OrganizationUnit:
                            DeploymentOrgUnit(targetUnits, unit, info);
                            break;
                        case UnitType.Group:
                            DeploymentGroup(targetUnits, unit, info);
                            break;
                        case UnitType.User:
                            DeploymentUser(targetUnits, unit, info);
                            break;
                        default: break;
                    }
                    DeploymentUnitAcl(unit.UnitID);
                }

                #region 部署完成后更新经理等字段
                List<string> needUpdateCodes = new List<string>();
                needUpdateCodes.AddRange(unitManagerDic.Keys);
                needUpdateCodes.AddRange(unitViceManagerDic.Keys);
                needUpdateCodes.AddRange(unitSecretaryDic.Keys);
                needUpdateCodes.AddRange(unitSupervisorDic.Keys);

                needUpdateCodes.AddRange(unitChildDic.Keys);
                Unit[] needUpdateUnits = this.DeloymenEngine.Organization.GetUnits(needUpdateCodes.ToArray()).ToArray();
                foreach (string code in unitManagerDic.Keys)
                {
                    Unit needUpdateUnit = needUpdateUnits.FirstOrDefault(i => i.ObjectID == code);
                    if (needUpdateUnit != null)
                    {
                        needUpdateUnit.ManagerID = unitManagerDic[code];
                        HandleResult re111 = this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, needUpdateUnit);
                    }
                }
                foreach (string code in unitSecretaryDic.Keys)
                {
                    Unit needUpdateUnit = needUpdateUnits.FirstOrDefault(i => i.ObjectID == code);
                    if (needUpdateUnit is User)
                    {
                        User user = needUpdateUnit as User;
                        if (user != null)
                        {
                            user.SecretaryID = unitSecretaryDic[code];
                            this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, user);
                        }
                    }

                }
                foreach (string code in unitChildDic.Keys)
                {
                    GroupBase needUpdateGroupBase = needUpdateUnits.FirstOrDefault(i => i.ObjectID == code) as GroupBase;
                    if (needUpdateGroupBase != null)
                    {
                        needUpdateGroupBase.Children = unitChildDic[code];
                        this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, needUpdateGroupBase);
                    }
                }
                #endregion
            }

            HandlerResult re = new HandlerResult();
            if (info.Count > 0)
            {
                re.Info = info;
            }
            return re;
        }
        /// <summary>
        /// 部署用户
        /// </summary>
        /// <param name="targetUnits"></param>
        /// <param name="unit"></param>
        private void DeploymentUser(Unit[] targetUnits, Unit unit, List<HandlerResult> info)
        {
            User user = unit as User;
            User targetUser = null;
            if (targetUnits != null && targetUnits.Length > 0)
            {
                targetUser = targetUnits.FirstOrDefault(i => i.ObjectID == user.ObjectID) as User;
            }
            HandleResult result;
            if (targetUser == null)
            {
                user.Serialized = false;
                //新增赋初始密码
                string pwd = this.DeloymenEngine.SettingManager.GetCustomSetting(CustomSetting.Setting_UserInitialPassword);
                if (!string.IsNullOrEmpty(pwd))
                {
                    user.Password = pwd;
                }
                else
                {
                    user.Password = OThinker.Organization.User.DefaultPassword;
                }
                result = this.DeloymenEngine.Organization.AddUnit(_controller.UserValidator.UserID, user);
            }
            else
            {
                if (user.ObjectID != targetUser.ObjectID)
                {
                    HandlerResult handerResult = new HandlerResult("DeployProcess.User", new List<string>() { user.Name, user.Code, HandleResult.ID_INVALIDATE.ToString() });
                    info.Add(handerResult);
                    return;
                }
                targetUser.Description = user.Description;
                targetUser.Email = user.Email;
                targetUser.ManagerID = user.ManagerID;
                targetUser.SecretaryID = user.SecretaryID;
                targetUser.Mobile = user.Mobile;
                targetUser.WeChatAccount = user.WeChatAccount;
                targetUser.DingTalkAccount = user.DingTalkAccount;
                targetUser.OfficePhone = user.OfficePhone;
                targetUser.FacsimileTelephoneNumber = user.FacsimileTelephoneNumber;
                targetUser.Name = user.Name;
                targetUser.Appellation = user.Appellation;
                targetUser.EmployeeNumber = user.EmployeeNumber;
                targetUser.Gender = user.Gender;
                targetUser.IsConsoleUser = user.IsConsoleUser;
                //targetUser.ImageName = user.ImageName;
                targetUser.EmployeeRank = user.EmployeeRank;
                targetUser.State = user.State;
                targetUser.PrivacyLevel = user.PrivacyLevel;
                targetUser.SortKey = user.SortKey;
                //targetUser.CategoryID = user.CategoryID;
                //targetUser.CountryName = user.CountryName;
                //targetUser.Province = user.Province;
                //targetUser.City = user.City;
                //targetUser.Street = user.Street;
                targetUser.PostalCode = user.PostalCode;
                targetUser.Birthday = user.Birthday;
                targetUser.EntryDate = user.EntryDate;
                targetUser.DepartureDate = user.DepartureDate;
                targetUser.ServiceState = user.ServiceState;
                targetUser.IDNumber = user.IDNumber;
                //targetUser.Notification = user.Notification;
                targetUser.ParentID = user.ParentID;
                result = this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, targetUser);
            }
            if (result != HandleResult.SUCCESS)
            {
                HandlerResult handerResult = new HandlerResult("DeployProcess.User", new List<string>() { user.Name, user.ObjectID, result.ToString() });
                info.Add(handerResult);
            }
            //部署头像
            Attachment pic = _controller.Engine.BizObjectManager.GetAttachment(user.ObjectID, Organization.User.TableName, user.ObjectID, user.ObjectID);
            if (pic != null)
            {
                Attachment targetPic = this.DeloymenEngine.BizObjectManager.GetAttachment(user.ObjectID, Organization.User.TableName, user.ObjectID, user.ObjectID);
                if (targetPic == null)
                {
                    pic.Serialized = false;
                    this.DeloymenEngine.BizObjectManager.AddAttachment(pic);
                }
                else
                {
                    this.DeloymenEngine.BizObjectManager.UpdateAttachment(pic.BizObjectSchemaCode, pic.BizObjectId, pic.AttachmentID, _controller.UserValidator.UserID, pic.FileName, pic.ContentType, pic.Content, pic.FileFlag);
                }
            }
            //部署签章
            var Signatures = _controller.Engine.Organization.GetSignaturesByUnit(user.ObjectID);
            if (Signatures != null)
            {
                foreach (var item in Signatures)
                {
                    item.Serialized = false;
                    if (this.DeloymenEngine.Organization.GetSignature(item.ObjectID) != null)
                    {
                        this.DeloymenEngine.Organization.RemoveSignature(item.ObjectID);
                    }
                    this.DeloymenEngine.Organization.AddSignature(item);
                }
            }
            //同步角色
            var userRoleList = _controller.Engine.Organization.GetParents(user.ObjectID, UnitType.Post, true, State.Active);
            foreach (var userRole in userRoleList)
            {
                OrgPost post = _controller.Engine.Organization.GetUnit(userRole) as OrgPost;
                if (post != null)
                {
                    this.DeloymenEngine.Organization.UpdateUnit(user.ObjectID, post);
                }
            }
        }
        /// <summary>
        /// 部署组
        /// </summary>
        /// <param name="targetUnits"></param>
        /// <param name="unit"></param>
        private void DeploymentGroup(Unit[] targetUnits, Unit unit, List<HandlerResult> info)
        {
            Group group = unit as Group;
            Group targetGroup = null;
            if (targetUnits != null && targetUnits.Length > 0)
            {
                targetGroup = targetUnits.FirstOrDefault(i => i.ObjectID == group.ObjectID) as Group;
            }

            HandleResult result;
            if (targetGroup == null)
            {
                group.Serialized = false;
                result = this.DeloymenEngine.Organization.AddUnit(_controller.UserValidator.UserID, group);
            }
            else
            {
                if (group.ObjectID != targetGroup.ObjectID)
                {
                    HandlerResult handerResult = new HandlerResult("DeployProcess.Group", new List<string>() { group.Name, group.ObjectID, HandleResult.ID_INVALIDATE.ToString() });
                    info.Add(handerResult);
                    return;
                }
                targetGroup.Name = group.Name;
                targetGroup.Description = group.Description;
                //targetGroup.AllowChildGroup = group.AllowChildGroup;
                targetGroup.ManagerID = group.ManagerID;
                targetGroup.ParentID = group.ParentID;
                targetGroup.Children = group.Children;
                targetGroup.Visibility = group.Visibility;
                targetGroup.SortKey = group.SortKey;
                targetGroup.CalendarID = group.CalendarID;
                result = this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, targetGroup);
            }
            if (result != HandleResult.SUCCESS)
            {
                HandlerResult handerResult = new HandlerResult("DeployProcess.Group", new List<string>() { group.Name, group.ObjectID, result.ToString() });
                info.Add(handerResult);
            }
        }
        /// <summary>
        /// 部署OU
        /// </summary>
        /// <param name="targetUnits"></param>
        /// <param name="unit"></param>
        private void DeploymentOrgUnit(Unit[] targetUnits, Unit unit, List<HandlerResult> info)
        {
            OrganizationUnit orgUnit = unit as OrganizationUnit;
            OrganizationUnit targetOrgUnit = null;
            if (targetUnits != null && targetUnits.Length > 0)
            {
                targetOrgUnit = targetUnits.FirstOrDefault(i => i.ObjectID == orgUnit.ObjectID) as OrganizationUnit;
            }

            HandleResult result;
            if (targetOrgUnit == null)
            {
                orgUnit.Serialized = false;
                result = this.DeloymenEngine.Organization.AddUnit(_controller.UserValidator.UserID, orgUnit);
            }
            else
            {
                if (orgUnit.ObjectID != targetOrgUnit.ObjectID)
                {
                    HandlerResult handerResult = new HandlerResult("DeployProcess.OU", new List<string>() { orgUnit.Name, orgUnit.ObjectID, HandleResult.ID_INVALIDATE.ToString() });
                    info.Add(handerResult);
                    return;
                }
                targetOrgUnit.Name = orgUnit.Name;
                targetOrgUnit.Description = orgUnit.Description;
                targetOrgUnit.ManagerID = orgUnit.ManagerID;
                targetOrgUnit.ParentID = orgUnit.ParentID;
                targetOrgUnit.CalendarID = orgUnit.CalendarID;
                //targetOrgUnit.CanAddPost = orgUnit.CanAddPost;
                //targetOrgUnit.CanAddOrgUnit = orgUnit.CanAddOrgUnit;
                //targetOrgUnit.CanAddGroup = orgUnit.CanAddGroup;
                targetOrgUnit.Visibility = orgUnit.Visibility;
                targetOrgUnit.SortKey = orgUnit.SortKey;
                targetOrgUnit.WorkflowCode = orgUnit.WorkflowCode;
                targetOrgUnit.CategoryCode = orgUnit.CategoryCode;
                result = this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, targetOrgUnit);
            }
            if (result != HandleResult.SUCCESS)
            {
                HandlerResult handerResult = new HandlerResult("DeployProcess.OU", new List<string>() { orgUnit.Name, orgUnit.ObjectID, result.ToString() });
            }
        }
        /// <summary>
        /// 部署公司信息
        /// </summary>
        private void DeploymentCompany(List<HandlerResult> info)
        {
            OrganizationUnit company = _controller.Engine.Organization.Company;
            OrganizationUnit targetCompany = this.DeloymenEngine.Organization.Company;
            if (company.ObjectID != targetCompany.ObjectID)
            {
                HandlerResult result = new HandlerResult("DeploymentProcess.Company", new List<string>() { company.Name, company.ObjectID, HandleResult.ID_INVALIDATE.ToString() });
                info.Add(result);
                return;
            }
            targetCompany.Name = company.Name;
            targetCompany.Description = company.Description;
            targetCompany.ManagerID = company.ManagerID;
            //targetCompany.CalendarId = company.CalendarId;

            this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, targetCompany);

            DeploymentUnitAcl(company.UnitID);
        }

        /// <summary>
        /// 部署组织类型
        /// </summary>
        private void DeploymentCategory(List<HandlerResult> info)
        {
            OrgCategory[] categories = _controller.Engine.Organization.GetAllOrgCategories().ToArray();
            OrgCategory[] targetCategories = this.DeloymenEngine.Organization.GetAllOrgCategories().ToArray();
            if (categories != null)
            {
                foreach (OrgCategory category in categories)
                {
                    OrgCategory targetCategory = null;
                    if (targetCategories != null && targetCategories.Length > 0)
                    {
                        targetCategory = targetCategories.FirstOrDefault(i => i.Code == category.Code);
                    }
                    if (targetCategory == null)
                    {
                        category.Serialized = false;
                        this.DeloymenEngine.Organization.AddOrgCategory(category);
                    }
                    else
                    {
                        if (category.ObjectID != targetCategory.ObjectID)
                        {
                            HandlerResult result = new HandlerResult("DeploymentProcess.Category", new List<string>() { category.DisplayName, category.Code, HandleResult.ID_INVALIDATE.ToString() });
                            info.Add(result);
                            continue;
                        }
                        targetCategory.Code = category.Code;
                        targetCategory.DisplayName = category.DisplayName;
                        targetCategory.Description = category.Description;
                        targetCategory.ObjectID = category.ObjectID;

                        this.DeloymenEngine.Organization.UpdateOrgCategory(targetCategory);
                    }
                }
            }
        }
        /// <summary>
        /// 部署组织同步
        /// </summary>
        /// <param name="info"></param>
        private void DeploymentSynchronzation(List<HandlerResult> info)
        {
            try
            {
                List<string> settingList = new List<string>{
                    OThinker.H3.Settings.CustomSetting.Setting_ADPathes,
                    H3.Settings.CustomSetting.Setting_ADUser,
                    H3.Settings.CustomSetting.Setting_ADPassword,
                    H3.Settings.CustomSetting.Setting_ADSyncTimes,
                    H3.Settings.CustomSetting.Setting_OrgReloadTimes
                };
                DeploymentCustomerSetting(settingList);
                var enableADValidation = OThinker.H3.Settings.CustomSetting.GetEnableADValidation(_controller.Engine.SettingManager);
                OThinker.H3.Settings.CustomSetting.SetEnableADValidation(this.DeloymenEngine.SettingManager, enableADValidation);
            }
            catch (Exception ex)
            {
                info.Add(new HandlerResult("DeploymentProcess.Synchronzation", new List<string>() { ex.Message}));
            }
        }

        private void DeploymentCustomerSetting(List<string> settingNames)
        {
            if (settingNames != null)
            {
                string settingValue = string.Empty;
                foreach (string name in settingNames)
                {
                    settingValue = _controller.Engine.SettingManager.GetCustomSetting(name);
                    if(!string.IsNullOrEmpty(settingValue))
                    {
                        this.DeloymenEngine.SettingManager.SetCustomSetting(name, settingValue);
                    }
                }
            }
        }
        /// <summary>
        /// 部署职务
        /// </summary>
        private void DeploymentJob(List<HandlerResult> info)
        {

            List<Unit> jobs = _controller.Engine.Organization.GetAllUnits(UnitType.Post);
            List<Unit> targetJobs = this.DeloymenEngine.Organization.GetAllUnits(UnitType.Post);
            if (jobs != null)
            {
                foreach (Unit job in jobs)
                {
                    Unit targetJob = null;
                    if (targetJobs != null && targetJobs.Count > 0)
                    {
                        targetJob = targetJobs.FirstOrDefault(i => i.Name == job.Name);
                    }
                    if (targetJob == null)
                    {
                        job.Serialized = false;
                        this.DeloymenEngine.Organization.AddUnit(_controller.UserValidator.UserID, job);
                    }
                    else
                    {
                        if (job.ObjectID != targetJob.ObjectID)
                        {
                            HandlerResult result = new HandlerResult("DeploymentProcess.OrgJob", new List<string>() { job.Name, job.ObjectID, HandleResult.ID_INVALIDATE.ToString() });
                            info.Add(result);
                            continue;
                        }
                        targetJob.SourceID = job.SourceID;
                        targetJob.Name = job.Name;
                        targetJob.Description = job.Description;
                        targetJob.ParentID = job.ParentID;
                        this.DeloymenEngine.Organization.UpdateUnit(_controller.UserValidator.UserID, targetJob);
                    }
                }
            }
        }
        /// <summary>
        /// 部署组织权限
        /// </summary>
        /// <param name="unitId"></param>
        private void DeploymentUnitAcl(string unitId)
        {
            FunctionAcl[] unitAcls = _controller.Engine.FunctionAclManager.GetUserAcls(new string[] { unitId });
            if (unitAcls != null)
            {
                FunctionAcl[] targetUnitAcls = this.DeloymenEngine.FunctionAclManager.GetUserAcls(new string[] { unitId });
                foreach (FunctionAcl unitAcl in unitAcls)
                {
                    FunctionAcl targetUnitAcl = null;
                    if (targetUnitAcls != null && targetUnitAcls.Length > 0)
                    {
                        targetUnitAcl = targetUnitAcls.FirstOrDefault(i => i.FunctionCode == unitAcl.FunctionCode);
                    }
                    if (targetUnitAcl == null)
                    {
                        unitAcl.Serialized = false;
                        this.DeloymenEngine.FunctionAclManager.Add(unitAcl);
                    }
                    else
                    {
                        targetUnitAcl.Run = unitAcl.Run;
                        targetUnitAcl.Administrator = unitAcl.Administrator;
                        targetUnitAcl.ModifiedTime = DateTime.Now;
                        this.DeloymenEngine.FunctionAclManager.Update(targetUnitAcl);
                    }
                }
            }

        }
        #endregion

        #region 部署系统参数

        private HandlerResult DeploymentCommonParam()
        {
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_UserInitialPassword,
                _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UserInitialPassword));
            // 根地址
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_PortalRoot
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_PortalRoot));
            // 流水号
            // 流水号的方式
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoModal
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SequenceNoModal));
            // 流水号的日期格式
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoDateFormat
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SequenceNoDateFormat));
            // 流水号的长度
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoLength
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SequenceNoLength));
            // 流水号的顺序
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SequenceNoOrder
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SequenceNoOrder));
            // 流水号的重置策略
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_GlobalSeqNoResetType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_GlobalSeqNoResetType));
            // 异常管理员
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ExceptionManager
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ExceptionManager));
            // 委托身份
            CustomSetting.SetAgencyIdentityType(this.DeloymenEngine.SettingManager
                , CustomSetting.GetAgencyIdentityType(_controller.Engine.SettingManager));
            // 任务超时检查的时间间隔
            CustomSetting.SetOvertimeCheckInterval(this.DeloymenEngine.SettingManager
                , CustomSetting.GetOvertimeCheckInterval(_controller.Engine.SettingManager));

            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WikiUrl
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WikiUrl));
            return new HandlerResult();
        }

        private HandlerResult DeploymentSysAdministrator()
        {
            DataTable acls = _controller.Engine.Query.QuerySystemAcl();
            if (acls != null && acls.Rows.Count > 0)
            {
                DataTable targetAcls = this.DeloymenEngine.Query.QuerySystemAcl();
                foreach (DataRow acl in acls.Rows)
                {
                    DataRow targetAcl = null;
                    if (targetAcls != null && targetAcls.Rows.Count > 0)
                    {
                        foreach (DataRow row in targetAcls.Rows)
                        {
                            if (row[SystemAcl.PropertyName_UserID] == acl[SystemAcl.PropertyName_UserID])
                            {
                                targetAcl = row;
                                break;
                            }
                        }
                    }
                    SystemAcl aclReocrd = new SystemAcl();
                    aclReocrd.UserID = acl[SystemAcl.PropertyName_UserID].ToString();
                    object isAdmin = acl[SystemAcl.PropertyName_Administrator];
                    aclReocrd.Administrator = (isAdmin != null && isAdmin.ToString() == "1") ? true : false;
                    aclReocrd.CreatedBy = acl[SystemAcl.PropertyName_CreatedBy].ToString();
                    DateTime createdTime;
                    if (DateTime.TryParse(acl[SystemAcl.PropertyName_CreatedTime].ToString(), out createdTime))
                    {
                        aclReocrd.CreatedTime = createdTime;
                    }
                    aclReocrd.ModifiedBy = acl[SystemAcl.PropertyName_ModifiedBy].ToString();
                    DateTime modifiedTime;
                    if (DateTime.TryParse(acl[SystemAcl.PropertyName_ModifiedTime].ToString(), out modifiedTime))
                    {
                        aclReocrd.ModifiedTime = modifiedTime;
                    }
                    if (targetAcl == null)
                    {
                        aclReocrd.Serialized = false;
                        this.DeloymenEngine.SystemAclManager.Add(aclReocrd);
                    }
                    else
                    {
                        aclReocrd.ObjectID = targetAcl[SystemAcl.PropertyName_ObjectID].ToString();
                        aclReocrd.Serialized = true;
                        this.DeloymenEngine.SystemAclManager.Update(new SystemAcl[] { aclReocrd });
                    }
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentSystemOrgAcl()
        {
            List<SystemOrgAcl> acls = _controller.Engine.SystemOrgAclManager.GetAllAcls();
            if (acls != null)
            {
                List<SystemOrgAcl> targetAcls = this.DeloymenEngine.SystemOrgAclManager.GetAllAcls();
                foreach (SystemOrgAcl acl in acls)
                {
                    SystemOrgAcl targetAcl = null;
                    if (targetAcls != null && targetAcls.Count > 0)
                    {
                        targetAcl = targetAcls.FirstOrDefault(i => i.UserID == acl.UserID && i.OrgScope == acl.OrgScope);
                    }
                    if (targetAcl == null)
                    {
                        if (this.DeloymenEngine.Organization.GetUnit(acl.OrgScope) != null
                            && this.DeloymenEngine.Organization.GetUnit(acl.UserID) != null)
                        {
                            acl.Serialized = false;
                            this.DeloymenEngine.SystemOrgAclManager.Add(acl);
                        }
                    }
                    else
                    {
                        targetAcl.EditOrg = acl.EditOrg;
                        targetAcl.ViewOrgData = acl.ViewOrgData;
                        targetAcl.Administrator = acl.Administrator;
                        targetAcl.ModifiedTime = DateTime.Now;
                        targetAcl.ModifiedBy = _controller.UserValidator.UserID;
                        this.DeloymenEngine.SystemOrgAclManager.Update(new SystemOrgAcl[] { targetAcl });
                    }
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentSettingsDisplay()
        {
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_UnfinishedWorkItemRefreshInterval
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UnfinishedWorkItemRefreshInterval));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WorkItemColorFilterVisibility
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WorkItemColorFilterVisibility));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WorkItemTypeFilterVisibility
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WorkItemTypeFilterVisibility));
            // 待办任务列表
            CustomSetting.SetUnfinishedWorkItemDisplayType(this.DeloymenEngine.SettingManager
                , CustomSetting.GetUnfinishedWorkItemDisplayType(_controller.Engine.SettingManager));
            // 操作按钮的名称
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionAdjustActivity
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionAdjustActivity));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCancelInstance
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCancelInstance));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionFinishInstance
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionFinishInstance));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCirculate
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCirculate));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionConsult
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionConsult));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionForward
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionForward));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionInstanceState
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionInstanceState));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionJump
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionJump));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionPrint
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionPrint));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionRetrieve
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionRetrieve));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionReturn
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionReturn));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionRollback
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionRollback));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionSave
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionSave));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionSubmit
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionSubmit));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCancelWorkItem
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCancelWorkItem));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionAssist
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionAssist));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionLockInstance
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionLockInstance));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionUnlockInstance
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionUnlockInstance));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionViewed
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionViewed));
            // 操作按钮的加粗
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionAdjustActivity_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionAdjustActivity_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCancelInstance_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCancelInstance_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionFinishInstance_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionFinishInstance_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCirculate_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCirculate_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionConsult_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionConsult_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionForward_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionForward_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionInstanceState_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionInstanceState_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionJump_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionJump_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionPrint_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionPrint_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionRetrieve_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionRetrieve_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionReturn_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionReturn_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionRollback_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionRollback_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionSave_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionSave_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionSubmit_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionSubmit_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionCancelWorkItem_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionCancelWorkItem_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionAssist_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionAssist_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionLockInstance_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionLockInstance_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionUnlockInstance_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionUnlockInstance_Bold));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ActionViewed_Bold
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ActionViewed_Bold));
            // 签章图片的宽度
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SigImageWidth
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SigImageWidth));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SigImageHeight
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SigImageHeight));
            return new HandlerResult();
        }

        private HandlerResult DeploymentMessageSetting()
        {
            // 微信
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatCorpID
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatCorpID));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatSecret
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatSecret));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatAgentId
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatAgentId));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_PortalUrl
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_PortalUrl));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatPush
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_WeChatPush));
            this.DeloymenEngine.WeChatAdapter.Reload();

            //钉钉
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDAgentId
               , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAgentId));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDSecret
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDSecret));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDCorpID
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDCorpID));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDPush
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDPush));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDUrl
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDUrl));
            this.DeloymenEngine.DingTalkAdapter.Reload();

            //通知内容
            //新任务到达时提醒，取代原先的Email only方式
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewTaskArrivedNotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewTaskArrivedNotifyType));
            // 提醒
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_NotifyWorkItem
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NotifyWorkItem));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_NotifyUrgency
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NotifyUrgency));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_UrgencyNotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UrgencyNotifyType));
            // 发送任务提醒的标题
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationSMSTitle
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationSMSTitle));
            // 短信审批成功和失败后的提示
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SMSApprovalSuccessNotification
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SMSApprovalSuccessNotification));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_SMSApprovalFailedNotification
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_SMSApprovalFailedNotification));

            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationTitle
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationTitle));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_UrgencyNotificationTitle
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UrgencyNotificationTitle));

            // 超时提醒策略1
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1NotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1NotifyType));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1Title
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1Title));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1Content
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1Content));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1PostInterval
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1PostInterval));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1PostAction
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1PostAction));

            // 超时提醒策略2
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2NotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2NotifyType));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2Title
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2Title));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2Content
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2Content));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2PostInterval
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2PostInterval));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2PostAction
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2PostAction));

            // 超时提醒策略3
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy3NotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy3NotifyType));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy3Title
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy3Title));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy3Content
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy3Content));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy3PostInterval
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy3PostInterval));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy3PostAction
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy3PostAction));

            // 超时提醒策略4
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy4NotifyType
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy4NotifyType));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy4Title
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy4Title));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy4Content
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy4Content));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy4PostInterval
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy4PostInterval));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy4PostAction
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy4PostAction));

            // 定期发送邮件通知
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderMethod
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ReccurentReminderMethod));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderPattern
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ReccurentReminderPattern));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RecurrentReminderDayOfWeek
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RecurrentReminderDayOfWeek));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderWorkingCalendar
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ReccurentReminderWorkingCalendar));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailNotificationContent
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailNotificationContent));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailUrgencyContent
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailUrgencyContent));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindTime
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindTime));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailRemindContent
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailRemindContent));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailRemindTitle
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailRemindTitle));

            return new HandlerResult();
        }

        private HandlerResult DeploymentWorkingCalendar()
        {
            WorkingCalendar[] calendars = _controller.Engine.WorkingCalendarManager.GetCalendarList();
            if (calendars != null && calendars.Length > 0)
            {
                foreach (WorkingCalendar calendar in calendars)
                {
                    calendar.Serialized = false;
                    Dictionary<string, bool> defaultWorkingDays = new Dictionary<string, bool>();
                    defaultWorkingDays.Add(DayOfWeek.Monday.ToString(), true);
                    defaultWorkingDays.Add(DayOfWeek.Tuesday.ToString(), true);
                    defaultWorkingDays.Add(DayOfWeek.Wednesday.ToString(), true);
                    defaultWorkingDays.Add(DayOfWeek.Thursday.ToString(), true);
                    defaultWorkingDays.Add(DayOfWeek.Friday.ToString(), true);
                    defaultWorkingDays.Add(DayOfWeek.Saturday.ToString(), false);
                    defaultWorkingDays.Add(DayOfWeek.Sunday.ToString(), false);
                    this.DeloymenEngine.WorkingCalendarManager.AddCalendar(calendar);
                    WorkingDay[] workingDays = _controller.Engine.WorkingCalendarManager.GetWorkingDays(calendar.ObjectID);
                    WorkingDay[] targetWorkingDays = this.DeloymenEngine.WorkingCalendarManager.GetWorkingDays(calendar.ObjectID);
                    if (workingDays != null)
                    {
                        foreach (WorkingDay workingDay in workingDays)
                        {
                            WorkingDay targetWorkingDay = null;
                            if (targetWorkingDays != null && targetWorkingDays.Length > 0)
                            {
                                targetWorkingDay = targetWorkingDays.FirstOrDefault(i => i.CurrentDate == workingDay.CurrentDate);
                            }
                            if (targetWorkingDay != null)
                            {
                                targetWorkingDay.IsWorkingDay = workingDay.IsWorkingDay;
                                targetWorkingDay.IsExceptional = workingDay.IsExceptional;
                                targetWorkingDay.Description = workingDay.Description;
                                targetWorkingDay.WorkingTimeSpans = workingDay.WorkingTimeSpans;
                                this.DeloymenEngine.WorkingCalendarManager.UpdateWorkingDay(targetWorkingDay);
                            }
                        }
                    }
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentDataDictionary()
        {
            EnumerableMetadata[] dataDics = _controller.Engine.MetadataRepository.GetByCategory(string.Empty);
            if (dataDics != null)
            {
                EnumerableMetadata[] targetDataDics = this.DeloymenEngine.MetadataRepository.GetByCategory(string.Empty);
                foreach (EnumerableMetadata dataDic in dataDics)
                {
                    EnumerableMetadata targetDataDic = null;
                    if (targetDataDics != null && targetDataDics.Length > 0)
                    {
                        //targetDataDic = targetDataDics.FirstOrDefault(i => i.Category == dataDic.Category && i.Code == dataDic.Code);
                        targetDataDic = targetDataDics.FirstOrDefault(i => i.ObjectID==dataDic.ObjectID);
                    }
                    if (targetDataDic == null)
                    {
                        dataDic.Serialized = false;
                        this.DeloymenEngine.MetadataRepository.Add(dataDic);
                    }
                    else
                    {
                        targetDataDic.EnumValue = dataDic.EnumValue;
                        targetDataDic.SortKey = dataDic.SortKey;
                        targetDataDic.IsDefault = dataDic.IsDefault;
                        this.DeloymenEngine.MetadataRepository.Update(targetDataDic);
                    }
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentGlobalData()
        {
            PrimitiveMetadata[] globalDatas = _controller.Engine.MetadataRepository.GetAllPrimitiveItems();
            if (globalDatas != null)
            {
                PrimitiveMetadata[] targetGlobalDatas = this.DeloymenEngine.MetadataRepository.GetAllPrimitiveItems();
                foreach (PrimitiveMetadata globalData in globalDatas)
                {
                    PrimitiveMetadata targetGlobalData = null;
                    if (targetGlobalDatas != null && targetGlobalDatas.Length > 0)
                    {
                        targetGlobalData = targetGlobalDatas.FirstOrDefault(i => i.ItemName == globalData.ItemName);
                    }
                    if (targetGlobalData == null)
                    {
                        globalData.Serialized = false;
                        this.DeloymenEngine.MetadataRepository.CreatePrimitiveItem(globalData.ItemName, globalData.Description, globalData.ItemValue);
                    }
                    else
                    {
                        this.DeloymenEngine.MetadataRepository.SetPrimitiveItemValue(targetGlobalData.ItemName, globalData.ItemValue, globalData.Description);
                    }
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentMobileDevice()
        {
            string sidString = _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DisabledMobileSID);
            if (!string.IsNullOrEmpty(sidString))
            {
                List<string> sidList = sidString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                string targetSIDString = this.DeloymenEngine.SettingManager.GetCustomSetting(CustomSetting.Setting_DisabledMobileSID);
                if (!string.IsNullOrEmpty(targetSIDString))
                {
                    string[] targetSIDs = targetSIDString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string targetSID in targetSIDs)
                    {
                        if (!sidList.Contains(targetSID))
                        {
                            sidList.Add(targetSID);
                        }
                    }
                }
                if (!this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_DisabledMobileSID, string.Join(",", sidList.ToArray())))
                {
                    return new HandlerResult("DeploymentProcess.Msg22");
                }
            }
            return new HandlerResult();
        }

        private HandlerResult DeploymentAgent()
        {
            DataTable agents = _controller.Engine.Query.QueryAgent();
            if (agents != null && agents.Rows.Count > 0)
            {
                DataTable targetAgents = this.DeloymenEngine.Query.QueryAgent();
                foreach (DataRow agent in agents.Rows)
                {
                    DataRow targetAgent = null;
                    if (targetAgents != null && targetAgents.Rows.Count > 0)
                    {
                        foreach (DataRow row in targetAgents.Rows)
                        {
                            if (row[Agency.PropertyName_UserID].ToString() == agent[Agency.PropertyName_UserID].ToString()
                                && row[Agency.PropertyName_WorkflowCode].ToString() == agent[Agency.PropertyName_WorkflowCode].ToString()
                                && row[Agency.PropertyName_Originator].ToString() == agent[Agency.PropertyName_Originator].ToString())
                            {
                                targetAgent = row;
                                break;
                            }
                        }
                    }

                    Agency agencyRecord = new Agency();
                    agencyRecord.UserID = agent[Agency.PropertyName_UserID].ToString();
                    agencyRecord.AgentID = agent[Agency.PropertyName_AgentID].ToString();
                    agencyRecord.WorkflowCode = agent[Agency.PropertyName_WorkflowCode].ToString();
                    agencyRecord.Originator = agent[Agency.PropertyName_Originator].ToString();
                    DateTime startTime;
                    if (DateTime.TryParse(agent[Agency.PropertyName_StartTime].ToString(), out startTime))
                    {
                        agencyRecord.StartTime = startTime;
                    }
                    DateTime endTime;
                    if (DateTime.TryParse(agent[Agency.PropertyName_EndTime].ToString(), out endTime))
                    {
                        agencyRecord.EndTime = endTime;
                    }
                    AgencyType agencyType;
                    if (Enum.TryParse(agent[Agency.PropertyName_AgencyType].ToString(), out agencyType))
                    {
                        agencyRecord.AgencyType = agencyType;
                    }
                    if (targetAgent == null)
                    {
                        agencyRecord.Serialized = false;
                        this.DeloymenEngine.AgencyManager.Add(agencyRecord);
                    }
                    else
                    {
                        agencyRecord.ObjectID = targetAgent[Agency.PropertyName_ObjectID].ToString();
                        agencyRecord.Serialized = true;
                        this.DeloymenEngine.AgencyManager.Update(agencyRecord);
                    }
                }
            }

            return new HandlerResult();
        }
        #endregion

        #region 部署报表数据源
        private HandlerResult DeploymentReportSource(string SourceCode)
        {
            ReportSource ReportSource = _controller.Engine.Analyzer.GetReportSourceByCode(SourceCode);
            if (ReportSource == null)
            {
                return new HandlerResult("DeploymentProcess.Msg23");
            }

            ReportSource TargetReportSource = this.DeloymenEngine.Analyzer.GetReportSourceByCode(SourceCode);
            if (TargetReportSource == null)
            {
                ReportSource.ObjectID = Guid.NewGuid().ToString();
                ReportSource.Serialized = false;

                if (ReportSource.Columns != null)
                {
                    foreach (ReportSourceColumn column in ReportSource.Columns)
                    {
                        column.ObjectID = Guid.NewGuid().ToString();
                        column.ParentObjectID = ReportSource.ObjectID;
                        column.Serialized = false;
                    }
                }
                if (!this.DeloymenEngine.Analyzer.AddReportSource(ReportSource))
                {
                    return new HandlerResult("DeploymentProcess.Msg24");
                }
            }
            else
            {
                TargetReportSource.DisplayName = ReportSource.DisplayName;
                List<ReportSourceColumn> Columns = new List<ReportSourceColumn>();
                if (ReportSource.Columns != null)
                {
                    foreach (ReportSourceColumn column in ReportSource.Columns)
                    {
                        column.ObjectID = Guid.NewGuid().ToString();
                        column.ParentObjectID = TargetReportSource.ObjectID;
                        column.Serialized = false;
                        Columns.Add(column);
                    }
                }

                TargetReportSource.Columns = Columns.ToArray();
                TargetReportSource.DbCode = ReportSource.DbCode;
                TargetReportSource.Description = ReportSource.Description;
                TargetReportSource.SchemaCode = ReportSource.SchemaCode;
                TargetReportSource.TableNameOrCommandText = ReportSource.TableNameOrCommandText;
                TargetReportSource.ReportSourceType = ReportSource.ReportSourceType;
                TargetReportSource.DataSourceType = ReportSource.DataSourceType;
                if (!this.DeloymenEngine.Analyzer.UpdateReportSource(TargetReportSource))
                {
                    return new HandlerResult("DeploymentProcess.Msg25");
                }
            }
            return new HandlerResult();
        }
        #endregion

        #region 部署报表模板
        private HandlerResult DeploymentReportTemplate(string Code)
        {
            ReportTemplate Template = _controller.Engine.Analyzer.GetReportTemplateByCode(Code);
            if (Template == null)
            {
                return new HandlerResult("DeploymentProcess.Msg26");
            }

            FunctionNode FolderNode = this.DeloymenEngine.FunctionAclManager.GetFunctionNodeByCode(Template.FolderCode);
            if (FolderNode == null || FolderNode.NodeType != FunctionNodeType.ReportTemplateFolder)
            {
                return new HandlerResult("DeploymentProcess.Msg27");
            }

            ReportTemplate TargetTemplate = this.DeloymenEngine.Analyzer.GetReportTemplateByCode(Code);
            if (TargetTemplate == null)
            {
                Template.ObjectID = Guid.NewGuid().ToString();
                Template.Serialized = false;
                if (Template.Columns != null)
                {
                    foreach (ReportTemplateColumn Column in Template.Columns)
                    {
                        Column.ObjectID = Guid.NewGuid().ToString();
                        Column.Serialized = false;
                    }
                }

                if (Template.Parameters != null)
                {
                    foreach (ReportTemplateParameter Parameter in Template.Parameters)
                    {
                        Parameter.ObjectID = Guid.NewGuid().ToString();
                        Parameter.Serialized = false;
                    }
                }

                if (Template.RowDrillParam != null)
                {
                    foreach (DrillParam Param in Template.RowDrillParam)
                    {
                        Param.ObjectID = Guid.NewGuid().ToString();
                        Param.Serialized = false;
                    }
                }

                if (Template.ColumnDrillParam != null)
                {
                    Template.ColumnDrillParam.ObjectID = Guid.NewGuid().ToString();
                    Template.ColumnDrillParam.Serialized = false;
                }
                long result = this.DeloymenEngine.Analyzer.AddReportTemplate(Template);
                if (result != ErrorCode.SUCCESS)
                {
                    return new HandlerResult("DeploymentProcess.Msg28", "", result.ToString());
                }
            }
            else
            {
                TargetTemplate.AxisUnit = Template.AxisUnit;
                TargetTemplate.Charts = Template.Charts;

                List<ReportTemplateColumn> Columns = new List<ReportTemplateColumn>();
                if (Template.Columns != null)
                {
                    foreach (ReportTemplateColumn Column in Template.Columns)
                    {
                        Column.ObjectID = Guid.NewGuid().ToString();
                        Column.Serialized = false;
                        Column.ParentObjectID = TargetTemplate.ObjectID;
                        Columns.Add(Column);
                    }
                }
                TargetTemplate.Columns = Columns.ToArray();

                TargetTemplate.ColumnTitle = Template.ColumnTitle;
                TargetTemplate.DefaultChart = Template.DefaultChart;
                TargetTemplate.DisplayName = Template.DisplayName;
                TargetTemplate.DrillCode = Template.DrillCode;

                List<ReportTemplateParameter> Parameters = new List<ReportTemplateParameter>();
                if (Template.Parameters != null)
                {
                    foreach (ReportTemplateParameter p in Template.Parameters)
                    {
                        p.ObjectID = Guid.NewGuid().ToString();
                        p.ParentObjectID = TargetTemplate.ObjectID;
                        p.Serialized = false;
                        Parameters.Add(p);
                    }
                }
                TargetTemplate.Parameters = Parameters.ToArray();

                if (Template.ColumnDrillParam != null)
                {
                    TargetTemplate.ColumnDrillParam = Template.ColumnDrillParam;
                    TargetTemplate.ColumnDrillParam.ObjectID = Guid.NewGuid().ToString();
                    TargetTemplate.ColumnDrillParam.ParentObjectID = TargetTemplate.ParentObjectID;
                    TargetTemplate.ColumnDrillParam.Serialized = false;
                }
                else
                {
                    TargetTemplate.ColumnDrillParam = null;
                }

                List<DrillParam> DParam = new List<DrillParam>();
                if (Template.RowDrillParam != null)
                {
                    foreach (DrillParam d in Template.RowDrillParam)
                    {
                        d.ObjectID = Guid.NewGuid().ToString();
                        d.ParentObjectID = TargetTemplate.ObjectID;
                        d.Serialized = false;
                        DParam.Add(d);
                    }
                }
                TargetTemplate.RowDrillParam = DParam.ToArray();

                TargetTemplate.ReportType = Template.ReportType;
                TargetTemplate.RowTitle = Template.RowTitle;
                TargetTemplate.SourceCode = Template.SourceCode;

                if (!this.DeloymenEngine.Analyzer.UpdateReportTemplate(TargetTemplate))
                {
                    return new HandlerResult("DeploymentProcess.Msg13");
                }
            }

            return new HandlerResult();
        }
        #endregion

        private HandlerResult DeploymentReport(string Code)
        {
            var _result=new HandlerResult();
            _result=DeploymentFunctionNode(Code);
            if(!_result.State)
            {
                return _result;
            }
            try
            {
                var reportTemplates = _controller.Engine.ReportManager.GetReportPage(Code);
                var targetReportTemplate=this.DeloymenEngine.ReportManager.GetReportPage(Code);
                if (targetReportTemplate != null && !string.IsNullOrEmpty(targetReportTemplate.Code))
                {
                    this.DeloymenEngine.ReportManager.UpdateReportPage(reportTemplates);
                }
                else
                {
                    SetSerialized(reportTemplates, false);
                    this.DeloymenEngine.ReportManager.AddReportPage(reportTemplates);
                }
                //foreach (var template in reportTemplates)
                //{
                //    if (targetReportTemplates.FirstOrDefault(o => o.Code == template.Code) != null)
                //    {
                //        this.DeloymenEngine.ReportManager.UpdateReportPage(template);
                //    }
                //    else
                //    {
                //        //var template=new 
                //        this.DeloymenEngine.ReportManager.AddReportPage(template);
                //    }
                //}
            }
            catch(Exception ex)
            {
                _result.State = false;
                _result.ErrorMsg = ex.Message;
            }
            return _result;
        }

        private void SetSerialized(OThinker.Reporting.ReportPage reportPage, bool value)
        {
            reportPage.Serialized = value;
            if (reportPage.ReportWidgets != null)
            {
                foreach (OThinker.Reporting.ReportWidget ReportWidget in reportPage.ReportWidgets)
                {
                    ReportWidget.Serialized = value;

                    SetReportWidgetColumn(ReportWidget.Categories, value);
                    SetReportWidgetColumn(ReportWidget.Columns, value);
                    SetReportWidgetColumn(ReportWidget.FilterColumns, value);
                    SetReportWidgetColumn(ReportWidget.Series, value);
                    SetReportWidgetColumn(ReportWidget.SortColumns, value);
                    if (ReportWidget.SourceFilters != null)
                    {
                        foreach (OThinker.Reporting.ReportFilter ReportFilter in ReportWidget.SourceFilters)
                        {
                            ReportFilter.Serialized = value;
                        }
                    }
                    if (ReportWidget.ReportWidgetSimpleBoard != null)
                    {
                        foreach (OThinker.Reporting.ReportWidgetSimpleBoard ReportWidgetSimpleBoard in ReportWidget.ReportWidgetSimpleBoard)
                        {
                            ReportWidgetSimpleBoard.Serialized = value;
                            SetReportWidgetColumn(ReportWidgetSimpleBoard.Columns, value);
                        }
                    }
                }
            }
            if (reportPage.Filters != null)
            {
                foreach (OThinker.Reporting.ReportFilter ReportFilter in reportPage.Filters)
                {
                    ReportFilter.Serialized = value;
                }
            }
            if (reportPage.ReportSources != null)
            {
                foreach (OThinker.Reporting.ReportSource ReportSource in reportPage.ReportSources)
                {
                    ReportSource.Serialized = value;
                    SetReportWidgetColumn(ReportSource.FunctionColumns, value);
                    SetReportWidgetColumn(ReportSource.SqlColumns, value);
                    SetReportWidgetColumn(ReportSource.SQLWhereColumns, value);
                    if (ReportSource.ReportSourceAssociations != null)
                    {
                        foreach (OThinker.Reporting.ReportSourceAssociation ReportSourceAssociation in ReportSource.ReportSourceAssociations)
                        {
                            ReportSourceAssociation.Serialized = value;
                        }
                    }
                }
            }
            //Type t = obj.GetType();
            //System.Reflection.PropertyInfo[] properties = t.GetProperties();
            //foreach(var property in properties)
            //{
            //    if (property.GetCustomAttributes(typeof(OThinker.Data.Database.Serialization.SerializablePropertyAttribute), true).Length > 0)
            //    {
            //        if (property.PropertyType.IsArray)
            //        {
            //            var list = property.GetValue(obj, null);
            //            foreach (object o in (list as IEnumerable<object>))
            //            {
            //                SetSerialized(o, value);
            //            }
            //        }
            //        else if (property.GetType().IsSubclassOf(typeof(OThinker.Data.Database.Serialization.SerializableObject)))
            //        {
            //            SetSerialized(property.GetValue(obj, null), value);
            //        }
            //        else
            //        {
            //        }
            //    }
            //    if (string.Equals(property.Name, "Serialized", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        property.SetValue(obj, value);
            //    }
            //}
        }

        private void SetReportWidgetColumn(OThinker.Reporting.ReportWidgetColumn[] ReportWidgetColumns,bool value)
        {
            if (ReportWidgetColumns != null)
            {
                foreach (OThinker.Reporting.ReportWidgetColumn ReportWidgetColumn in ReportWidgetColumns)
                {
                    ReportWidgetColumn.Serialized = value;
                }
            }
        }

        #region 部署连接池
        /// <summary>
        /// 部署连接池
        /// </summary>
        /// <returns></returns>
        private HandlerResult DeploymentBizDbConfig()
        {
            BizDbConnectionConfig[] DbConfigs = _controller.Engine.SettingManager.GetBizDbConnectionConfigList();
            if (DbConfigs == null)
            {
                return new HandlerResult("DeploymentProcess.Msg29");
            }

            foreach (BizDbConnectionConfig DbConfig in DbConfigs)
            {
                BizDbConnectionConfig TargetConfig = this.DeloymenEngine.SettingManager.GetBizDbConnectionConfig(DbConfig.DbCode);
                if (TargetConfig == null)
                {
                    BizDbConnectionConfig curreentDbConfig = _controller.Engine.SettingManager.GetBizDbConnectionConfig(DbConfig.DbCode);
                    curreentDbConfig.ObjectID = Guid.NewGuid().ToString();
                    curreentDbConfig.Serialized = false;
                    this.DeloymenEngine.SettingManager.AddBizDbConnectionConfig(curreentDbConfig);
                }
            }

            return new HandlerResult();
        }

        #endregion

        #region 部署账户映射
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private HandlerResult DeploymentBizAccount()
        {
            BizAccountCategory[] categories = _controller.Engine.BizBus.GetAccountCategories();
            if (categories == null)
            {
                return new HandlerResult("没有账号映射数据!");
            }

            foreach (BizAccountCategory category in categories)
            {
                BizAccountCategory targetCategory = this.DeloymenEngine.BizBus.GetAccountCategory(category.CategoryCode);
                if (targetCategory == null)
                {
                    targetCategory = new BizAccountCategory();
                }

                targetCategory.CategoryCode = category.CategoryCode;
                targetCategory.DisplayName = category.DisplayName;
                targetCategory.Description = category.Description;
                targetCategory.AuthenticationType = category.AuthenticationType;

                if (targetCategory.Serialized)
                {
                    this.DeloymenEngine.BizBus.UpdateAccountCategory(targetCategory);
                }
                else
                {
                    this.DeloymenEngine.BizBus.AddAccountCategory(targetCategory);
                }

                DeploymentAccountMappings(targetCategory.CategoryCode);
            }

            return new HandlerResult();
        }

        private void DeploymentAccountMappings(string CategoryCode)
        {
            OThinker.H3.BizBus.BizService.BizAccountMapping[] mappings = _controller.Engine.BizBus.GetAccountMappings(CategoryCode);
            OThinker.H3.BizBus.BizService.BizAccountMapping[] targetMappings = this.DeloymenEngine.BizBus.GetAccountMappings(CategoryCode);

            if (targetMappings != null)
            {
                foreach (BizAccountMapping mapping in targetMappings)
                    this.DeloymenEngine.BizBus.RemoveAccountMapping(mapping.ObjectID);
            }

            if (mappings != null)
            {
                foreach (BizAccountMapping mapping in mappings)
                {
                    mapping.ObjectID = Guid.NewGuid().ToString();
                    mapping.Serialized = false;
                    this.DeloymenEngine.BizBus.AddAccountMapping(mapping);
                }
            }
        }
        #endregion

        #region 部署状态设置
        private HandlerResult DeploymentStatusSetting()
        {
            // 状态
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateApproved
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateApproved));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateCanceled
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateCanceled));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateDisapproved
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateDisapproved));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateExceptional
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateExceptional));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateFinished
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateFinished));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateFinishing
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateFinishing));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateInitiated
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateInitiated));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateNotCanceled
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateNotCanceled));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateRunning
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateRunning));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateStarting
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateStarting));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateSuspended
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateSuspended));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateUnfinished
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateUnfinished));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateUnspecified
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateUnspecified));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateWaiting
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateWaiting));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_StateWorking
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_StateWorking));

            // 流程优先级
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_InstancePriorityHigh
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_InstancePriorityHigh));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_InstancePriorityNormal
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_InstancePriorityNormal));
            this.DeloymenEngine.SettingManager.SetCustomSetting(CustomSetting.Setting_InstancePriorityLow
                , _controller.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_InstancePriorityLow));
            return new HandlerResult();
        }
        #endregion

        /// <summary>
        /// 返回结果类
        /// </summary>
        public class HandlerResult
        {
            /// <summary>
            /// 状态结果
            /// </summary>
            public bool State;
            public string ErrorMsg;
            public string ErrorCode;
            public string ExtendMsg;
            public List<string> ErrorMsgs;
            public object Info = string.Empty;

            public HandlerResult()
            {
                this.State = true;
            }

            public HandlerResult(string ErrorMsg)
            {
                this.State = false;
                this.ErrorMsg = ErrorMsg;
            }

            public HandlerResult(string ErrorCode, string ErrorMsg)
            {
                this.ErrorMsg = ErrorMsg;
                this.ErrorCode = ErrorCode;
            }
            public HandlerResult(string ErrorCode, string ErrorMsg, string ExtendMsg)
            {
                this.State = false;
                this.ErrorMsg = ErrorMsg;
                this.ErrorCode = ErrorCode;
                this.ExtendMsg = ExtendMsg;
            }

            public HandlerResult(string ErrorCode, List<string> ErrorMsgs)
            {
                this.State = false;
                this.ErrorCode = ErrorCode;
                this.ErrorMsgs = ErrorMsgs;
            }
        }
    }
}
