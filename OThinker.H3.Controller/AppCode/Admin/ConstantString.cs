using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.AppCode.Admin
{
    /// <summary>
    /// ConstantString 的摘要说明
    /// </summary>
    public static class ConstantString
    {
        #region 后台树相关常量定义
        /// <summary>
        /// 普通节点
        /// </summary>
        public const string CommonNnode = "CommonNnode";

        /// <summary>
        /// 加载组织结构树
        /// </summary>
        public const string LoadOrgTree = "OrgTree";

        /// <summary>
        /// 加载组织结构孩子树
        /// </summary>
        public const string LoadOrgChildNode = "LoadOrgChildNode";

        /// <summary>
        /// 加载流程类型下的流程
        /// </summary>
        public const string LoadProcessModel = "LoadProcessModel";

        /// <summary>
        /// 加载数据模型的菜单
        /// </summary>
        public const string LoadBizOjectChildren = "LoadBizOjectChildren";

        /// <summary>
        /// 加载业务服务的方法
        /// </summary>
        public const string LoadBizServiceChildren = "LoadBizServiceChildren";

        /// <summary>
        /// 加载业务规则的孩子：决策表
        /// </summary>
        public const string LoadBizRuleChildren = "LoadBizRuleChildren";

        /// <summary>
        /// 加载消息设置子节点
        /// </summary>
        public const string LoadMessageNode = "SysParam_MessageSetting";
        /// <summary>
        /// 加载表单
        /// </summary>
        public const string LoadMDSheetList = "MDSheetList";

        /// <summary>
        /// 加载业务方法
        /// </summary>
        public const string LoadBizMethodTree = "LoadBizMethodTree";
        #endregion

        #region 树节点的工具栏标示
        /// <summary>
        /// 报表模型工具栏
        /// </summary>
        public const string TB_ReportPage = "TB_ReportPage";
        /// <summary>
        /// 组织机构工具栏
        /// </summary>
        public const string TB_Company = "TB_Company";
        /// <summary>
        /// 流程模型工具栏
        /// </summary>
        public const string TB_ProcessModel = "TB_ProcessModel";
        /// <summary>
        /// 业务服务工具栏
        /// </summary>
        public const string TB_BizService = "TB_BizService";
        /// <summary>
        /// 主数据工具栏
        /// </summary>
        public const string TB_BizMasterData = "TB_BizMasterData";
        /// <summary>
        /// 业务规则
        /// </summary>
        public const string TB_BizRule = "TB_BizRule";
        /// <summary>
        /// 业务方法工具栏
        /// </summary>
        public const string TB_BizMethod = "TB_BizMethod";

        /// <summary>
        /// 报表数据源工具标示
        /// </summary>
        public const string TB_ReportSource = "TB_ReportSource";
        /// <summary>
        /// 报表模板工具栏
        /// </summary>
        public const string TB_ReportTemplate = "TB_ReportTemplate";
        #endregion

        /// <summary>
        /// 管理站点路径
        /// </summary>
        public const string ConsoleRoot = "Admin";

        #region 后台登陆相关常量
        /// <summary>
        /// 登录模式标示
        /// </summary>
        public const string LoginModalKey = "LoginModal";
        /// <summary>
        /// 登陆关闭类型：默认刷新界面
        /// </summary>
        public const string LoginIsRefresh = "LoginIsRefresh";

        /// <summary>
        /// 登录模式
        /// </summary>
        public enum LoginModal
        {
            /// <summary>
            /// 模态登录
            /// </summary>
            ModalShow,
            /// <summary>
            /// 非模态登录
            /// </summary>
            UnModalShow
        }
        #endregion

        #region 链接地址
        /// <summary>
        /// 编辑组织结构（公司的）页面地址
        /// </summary>
        public const string PagePath_EditCompany = "Organization/EditCompany.html";//"/Admin/Organization/EditCompany.html";
        /// <summary>
        /// 新增、编辑群组的页面地址
        /// </summary>
        public const string PagePath_EditSegment = "/Admin/Organization/EditSegment.html";
        /// <summary>
        /// 新增、编辑OU的页面地址
        /// </summary>
        public const string PagePath_EditOrgUnit = "Organization/EditOrgUnit.html";
        /// <summary>
        /// 新增、编辑组的页面地址
        /// </summary>
        public const string PagePath_EditGroup = "Organization/EditGroup.html";
        /// <summary>
        /// 新增、编辑用户的页面地址
        /// </summary>
        public const string PagePath_EditUser = "Organization/EditUser.html";
        /// <summary>
        /// 新增、编辑岗位的页面地址
        /// </summary>
        public const string PagePath_EditOrgPost = "/Admin/Organization/EditOrgPost.html";
        /// <summary>
        /// 编辑业务对象
        /// </summary>
        public const string PagePath_EditBizObjectSchema = "/Admin/ProcessModel/EditBizObjectSchema.html";
        /// <summary>
        /// 业务对象的查询列表
        /// </summary>
        public const string PagePath_EditBizObjectSchemaViewList = "/Admin/ProcessModel/EditBizObjectSchemaViewList.html";
        /// <summary>
        /// 数据模型 定义查询 地址
        /// </summary>
        public const string PagePath_ListBizQuery = "/Admin/ProcessModel/ListBizQuery.html";
        /// <summary>
        /// 业务方法的列表地址
        /// </summary>
        public const string PagePath_BizObjectSchemaMethodList = "/Admin/ProcessModel/BizObjectSchemaMethodList.html";
        /// <summary>
        /// 数据模型的监听实例 地址
        /// </summary>
        public const string PagePath_ListListener = "/Admin/ProcessModel/ListListener.html";
        /// <summary>
        /// 编辑监听实例
        /// </summary>
        public const string PagePath_EditListenerPolicy = "/Admin/ProcessModel/EditListenerPolicy.html";
        /// <summary>
        /// 数据模型 定时作业 地址
        /// </summary>
        public const string PagePath_ListScheduleInvoker = "/Admin/ProcessModel/ListScheduleInvoker.html";
        /// <summary>
        /// 业务服务地址
        /// </summary>
        public const string PagePath_EditBizService = "/Admin/ProcessModel/EditBizService.html";
        /// <summary>
        /// 业务服务的方法列表地址
        /// </summary>
        public const string PagePath_ListBizServiceMethod = "/Admin/ProcessModel/ListBizServiceMethod.html";
        /// <summary>
        /// 数据模型关联关系列表地址
        /// </summary>
        public const string PagePath_ListBizObjectSchemaAssociation = "/Admin/ProcessModel/ListBizObjectSchemaAssociation.html";
        /// <summary>
        /// 运行参数设置地址
        /// </summary>
        public const string PagePath_WorkflowTemplateSetting = "/Admin/ProcessModel/WorkflowTemplateSetting.html";
        /// <summary>
        /// 流程模板地址
        /// </summary>
        public const string PagePath_WorkflowDesigner = "/Admin/Designer/Designer.html";

        /// <summary>
        /// 流程目录新增、编辑界面
        /// </summary>
        public const string PagePath_AddProcessFolder = "/Admin/ProcessModel/AddProcessFolder.html";
        /// <summary>
        /// 流程包新增、编辑地址
        /// </summary>
        public const string PagePath_EditBizWorkflowPackage = "/Admin/ProcessModel/EditBizWorkflowPackage.html";
        /// <summary>
        /// 自定义表单编辑界面
        /// </summary>
        public const string PagePath_WorkSheetEdit = "/Admin/ProcessModel/WorkSheetEdit.html";
        /// <summary>
        /// 表单设计器
        /// </summary>
        public const string PagePath_MvcDesigner = "/Admin/MvcDesigner/MvcDesigner.html";
        /// <summary>
        /// 表单设计器
        /// </summary>
        public const string PagePath_SheetDesigner = "/Admin/SheetDesigner/SheetDesigner.html";
        /// <summary>
        /// 流程模板历史版本
        /// </summary>
        public const string PagePath_BizWorkflowHistory = "/Admin/ProcessModel/BizWorkflowHistory.html";
        /// <summary>
        /// 流程监控
        /// </summary>
        public const string PagePath_ListSimulation = "/Admin/ProcessModel/SimulationList.html";
        /// <summary>
        /// 业务方法编辑界面
        /// </summary>
        public const string PagePath_BizObjectSchemaMethod = "/Admin/ProcessModel/EditBizObjectSchemaMethod.html";
        /// <summary>
        /// 业务规则
        /// </summary>
        public const string PagePath_EditBizRuleTable = "/Admin/BizRule/EditBizRuleTable.html";
        /// <summary>
        /// 编辑决策表路径
        /// </summary>
        public const string PagePath_EditBizRuleDecisionMatrix = "/Admin/BizRule/EditBizRuleDecisionMatrix.html";
        /// <summary>
        /// 应用程序列表
        /// </summary>
        public const string PagePath_AppList = "/Admin/Apps/AppList.html";
        /// <summary>
        /// 应用程序菜单列表
        /// </summary>
        public const string PagePath_AppMenuList = "/Admin/Apps/AppMenuList.html";

        /// <summary>
        /// 明细汇总表编辑页面
        /// </summary>
        public const string PagePath_ReportTemplateSummaryEditUrl = "/Admin/BPA/ReportTemplate_Summary.html";
        /// <summary>
        /// 交叉分析表编辑页面
        /// </summary>
        public const string PagePath_ReportTemplateCrossEditUrl = "/Admin/BPA/ReportTemplate_Cross.html";
        #endregion

        #region 参数常量
        /// <summary>
        /// 编码参数标示
        /// </summary>
        public const string Param_Code = "Code";
        /// <summary>
        /// 报表数据源编码
        /// </summary>
        public const string Param_ReportSourceCode = "ReportSourceCode";
        /// <summary>
        /// 序号参数
        /// </summary>
        public const string Param_Index = "Index";
        /// <summary>
        /// 父级序号，用\分割
        /// </summary>
        public const string Param_ParentIndexes = "ParentIndexes";
        /// <summary>
        /// 编码参数标示
        /// </summary>
        public const string Param_ParentID = "ParentID";
        /// <summary>
        /// 流程模型编码参数标示
        /// </summary>
        public const string Param_SchemaCode = "SchemaCode";
        /// <summary>
        /// 流程模型原始编码参数标示
        /// </summary>
        public const string Param_OwnSchemaCode = "OwnSchemaCode";
        /// <summary>
        /// 业务规则编码
        /// </summary>
        public const string Param_RuleCode = "RuleCode";
        /// <summary>
        /// 属性名称参数标示
        /// </summary>
        public const string Param_Property = "Property";
        /// <summary>
        /// 父属性名称参数标示
        /// </summary>
        public const string Param_ParentProperty = "ParentProperty";
        /// <summary>
        /// 视图名称参数标示
        /// </summary>
        public const string Param_ViewName = "ViewName";
        /// <summary>
        /// 业务方法
        /// </summary>
        public const string Param_Method = "Method";
        /// <summary>
        /// 规则的方法类型
        /// </summary>
        public const string Param_MethodType = "MethodType";
        /// <summary>
        /// 映射序号
        /// </summary>
        public const string Param_MapIndex = "MapIndex";
        /// <summary>
        /// 映射的参数名称
        /// </summary>
        public const string Param_ParamName = "ParamName";

        /// <summary>
        /// 是否参数？True：是参数;False：是返回值
        /// </summary>
        public const string Param_IsParam = "IsParam";
        /// <summary>
        /// 操作模式
        ///     0：用于测试该BizObjectSchema，不带任何其他参数，默认值
        ///     1：加载模式，用于根据主键加载某个BizObject
        ///     2：创建模式，传入一组参数用于创建某个BizObject
        /// </summary>
        public const string Param_Mode = "Mode";
        /// <summary>
        /// 业务方法 定义查询
        /// </summary>
        public const string Param_QueryCode = "QueryCode";
        /// <summary>
        /// ID 参数
        /// </summary>
        public const string Param_ID = "ID";
        /// <summary>
        /// 业务服务编码参数
        /// </summary>
        public const string Param_ServiceCode = "ServiceCode";
        /// <summary>
        /// 关联名称参数
        /// </summary>
        public const string Param_AssociationName = "AssociationName";
        /// <summary>
        /// 流程目录code参数
        /// </summary>
        public const string Param_WorkflowFolderCode = "WorkflowFolderCode";
        /// <summary>
        /// 表单编码参数
        /// </summary>
        public const string Param_SheetCode = "SheetCode";
        /// <summary>
        /// 流程模板编码参数
        /// </summary>
        public const string Param_WorkflowCode = "WorkflowCode";
        /// <summary>
        /// 表单编码
        /// </summary>
        public const string Param_FormCode = "FormCode";
        /// <summary>
        /// 规则决策编码
        /// </summary>
        public const string Param_DecisionMatrixCode = "DecisionMatrixCode";
        /// <summary>
        /// 版本参数
        /// </summary>
        public const string Param_Version = "Version";
        /// <summary>
        /// 权限ID
        /// </summary>
        public const string Param_AclID = "AclID";

        /// <summary>
        /// 应用程序编码参数
        /// </summary>
        public const string Param_AppCode = "AppCode";

        /// <summary>
        /// 流程模拟ID
        /// </summary>
        public const string Param_SimulationID = "SimulationID";
        /// <summary>
        /// 流程模拟ID
        /// </summary>
        public const string Param_SimulationListID = "SimulationListID";

        /// <summary>
        /// 流程模拟运行批次ID
        /// </summary>
        public const string Param_SimulationTokenID = "SimulationTokenID";
        #endregion

        #region 自定义流程选择控件树节点构造常量
        public const string LoadWorkflowCode = "LoadWorkflowCode";
        public const string ProcessModel_WorkflowCode = "ProcessModel_WorkflowCode";
        public const string ProcessModel_WFFolder = "ProcessModel_WFFolder";
        #endregion
    }
}
