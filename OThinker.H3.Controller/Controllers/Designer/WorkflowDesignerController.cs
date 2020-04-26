using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.Controllers.Admin.Formula;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance.Keywords;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Designer
{
    public class WorkflowDesignerController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        #region 设置可选的通知方式

        /// <summary>
        /// 设置可选的通知方式
        /// </summary>
        public JsonResult SetNotifyTypes()
        {
            //通知方式
            List<object> listObj = new List<object>();
            foreach (string name in Enum.GetNames(typeof(OThinker.H3.Notification.NotifyType)))
            {

                if (name != OThinker.H3.Notification.NotifyType.Unspecified.ToString())
                {
                    listObj.Add(new { Text = name, Value = (int)Enum.Parse(typeof(OThinker.H3.Notification.NotifyType), name) + string.Empty });
                }
            }

            return Json(listObj);
        }

        #endregion

        #region 设置可选的活动模板

        /// <summary>
        /// 设置可选的活动模板
        /// </summary>
        public JsonResult SetActivityTemplates()
        {
            //活动模板
            Activity[] ActivityConfigs = this.Engine.WorkflowConfigManager.GetAllActivityConfigs();
            foreach (Activity activity in ActivityConfigs)
            {
                string name = "Activity_" + activity.ActivityType.ToString();//前台需要实现多语言 //this.PortalResource.GetString("Activity_" + activity.ActivityType.ToString());
                //ERROR:实现多语言时,在这里调用获取当前语言对应的显示名称
                activity.DisplayName = string.IsNullOrEmpty(name) ? activity.ActivityType.ToString() : name;
                activity.CustomCode = activity.GenerateCode(Activity.DefaultNameSpace, "{ClassName}");
            }
            //输出到前端
            return Json(ActivityConfigs);

            //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "ActivityTemplateConfigs", "var ActivityTemplateConfigs=" + ActivityTemplateString + ";", true);
        }

        #endregion

        #region 流程编辑模式
        /// <summary>
        /// 业务对象模型是否可编辑
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public bool IsSchemaEditable(string SchemaCode)
        {
            if (!string.IsNullOrEmpty(SchemaCode))
            {
                FunctionNode functionNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(SchemaCode);
                if (functionNode != null)
                {
                    if (functionNode.IsLocked != true || (functionNode.IsLocked == true && string.Compare(functionNode.LockedBy, this.UserValidator.UserID, true) == 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获取流程编辑模式
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="WorkflowVersion"></param>
        /// <returns></returns>
        public JsonResult GetWorkflowMode(string SchemaCode, int WorkflowVersion)
        {
            return ExecuteFunctionRun(() =>
            {

                var workflowCode = (IsSchemaEditable(SchemaCode) && WorkflowVersion == WorkflowDocument.NullWorkflowVersion ? "1" : "2");

                return Json(workflowCode);
            });
        }

        #endregion


        /// <summary>
        /// 输出流程模板包、流程模板
        /// </summary>
        public JsonResult RegisterActivityTemplateConfigs(string WorkflowCode, int WorkflowVersion, string InstanceID)
        {
            return ExecuteFunctionRun(() =>
            {
                WorkflowCode = Server.UrlDecode(WorkflowCode); //解决WorkflowCode为汉字的情景
                ActivityTemplateConfigsViewModel model = new ActivityTemplateConfigsViewModel();
                if ((WorkflowCode == null || WorkflowCode == "") && WorkflowVersion == -1)
                {
                    Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                    if (InstanceContext != null)
                    {
                        WorkflowCode = InstanceContext.WorkflowCode;
                        WorkflowVersion = InstanceContext.WorkflowVersion;
                        model.InstanceContext = InstanceContext;
                        model.ExceptionActivities = this.ExceptionActivities(InstanceID);
                    }
                }

                #region 是否锁定
                model.IsControlUsable = BizWorkflowPackageLockByID(WorkflowCode);
                #endregion

                #region 流程模板

                WorkflowDocument WorkflowTemplate = null;
                if (WorkflowVersion == WorkflowDocument.NullWorkflowVersion)
                {
                    WorkflowTemplate = this.Engine.WorkflowManager.GetDraftTemplate(WorkflowCode);

                }
                else
                    WorkflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplate(WorkflowCode, WorkflowVersion);

                #endregion

                #region 数据模型

                //数据项
                List<string> lstDataItems = new List<string>();
                //所有属性
                List<object> lstProperties = new List<object>();
                //输出到前台的数据项
                List<object> lstFrontDataItems = new List<object>();
                //允许设置权限的数据项
                List<string> lstPermissionDataItems = new List<string>();

                //参与者数据项
                List<string> lstUserDataItems = new List<string>();
                //逻辑型数据项
                List<string> lstBoolDataItems = new List<string>();

                //业务方法名称列表 
                List<string> lstBizMethodNames = new List<string>();
                List<object> lstAllDataItems = new List<object>();//系统数据项，+流程数据项，流程设计选择时使用

                WorkflowClause Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                if (Clause != null && Clause.BizSchemaCode != null)
                {
                    //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "ClauseName", "var ClauseName=" + JSSerializer.Serialize(Clause.DisplayName + string.Empty) + ";", true);
                    model.ClauseName = Clause.WorkflowName + string.Empty;
                    //流程模板浏览页面地址
                    // ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "WorkflowViewUrl", "var WorkflowViewUrl=" + JSSerializer.Serialize(GetPortalRoot(this.Page) + ConstantString.PagePath_WorkflowDesigner) + ";", true);
                    model.WorkflowViewUrl = PortalRoot + ConstantString.PagePath_WorkflowDesigner;
                    DataModel.BizObjectSchema BizObject = this.Engine.BizObjectManager.GetDraftSchema(Clause.BizSchemaCode);
                    if (BizObject != null)
                    {
                        //流程包
                        object Package = new
                        {
                            SchemaCode = BizObject.SchemaCode
                        };
                        //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "Package", "var Package=" + JSSerializer.Serialize(Package) + ";", true);
                        model.Package = Package;

                        #region 页标签

                        if (Clause != null)
                        {
                            string TabName = string.IsNullOrEmpty(Clause.WorkflowName) ? Clause.WorkflowCode : Clause.WorkflowName;
                            if (WorkflowVersion != WorkflowDocument.NullWorkflowVersion)
                            {
                                TabName += "[" + WorkflowVersion + "]";
                            }
                            else
                            {
                                TabName += "[设计]";
                            }
                            //SetTabHeader(TabName);//前台执行
                            model.TabName = TabName;

                        }

                        #endregion

                        List<object> lstBizMethods = new List<object>();
                        //可选的业务方法
                        if (BizObject.Methods != null)
                        {
                            foreach (var Method in BizObject.Methods)
                            {
                                if (!DataModel.BizObjectSchema.IsDefaultMethod(Method.MethodName) && Method.MethodType == DataModel.MethodType.Normal)
                                {
                                    lstBizMethodNames.Add(Method.MethodName);
                                    lstBizMethods.Add(new { Text = Method.FullName, Value = Method.MethodName });
                                }
                            }
                        }
                        model.BizMethods = lstBizMethods;
                        //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "BizMethods", "var BizMethods=" + JSSerializer.Serialize(lstBizMethods.ToArray()) + ";", true);

                        #region  数据项、参与者、逻辑型数据项
                        //数据项、参与者、逻辑型数据项
                        if (BizObject.Properties != null)
                        {
                            List<Object> listNotifyCondition = new List<object>();
                            List<Object> listApprovalDataItem = new List<object>();
                            List<Object> listCommentDataItem = new List<object>();

                            listNotifyCondition.Add(new { Text = string.Empty, Value = string.Empty });
                            listApprovalDataItem.Add(new { Text = string.Empty, Value = string.Empty });
                            listCommentDataItem.Add(new { Text = string.Empty, Value = string.Empty });

                            foreach (DataModel.PropertySchema Property in BizObject.Properties)
                            {
                                //不显示保留数据项
                                if (!DataModel.BizObjectSchema.IsReservedProperty(Property.Name))
                                {
                                    lstDataItems.Add(Property.Name);
                                    lstProperties.Add(new
                                    {
                                        Text = Property.DisplayName,
                                        Value = Property.Name
                                    });

                                    lstFrontDataItems.Add(new
                                    {
                                        Text = Property.DisplayName,
                                        Value = Property.Name,
                                        //标记子表标题行
                                        ItemType = Property.LogicType == Data.DataLogicType.BizObjectArray ? "SubTableHeader" : string.Empty
                                    });
                                    lstPermissionDataItems.Add(Property.Name);



                                    //通知条件 TODO
                                    // sltNotifyCondition.Items.Add(new ListItem(Property.DisplayName, Property.Name));
                                    listNotifyCondition.Add(new { Text = Property.DisplayName, Value = Property.Name });

                                    //子表
                                    if ((Property.LogicType == Data.DataLogicType.BizObject || Property.LogicType == Data.DataLogicType.BizObjectArray)
                                        && Property.ChildSchema != null && Property.ChildSchema.Properties != null)
                                    {
                                        foreach (DataModel.PropertySchema ChildProperty in Property.ChildSchema.Properties)
                                        {
                                            if (!DataModel.BizObjectSchema.IsReservedProperty(ChildProperty.Name))
                                            {
                                                lstFrontDataItems.Add(new
                                                {
                                                    Text = Property.DisplayName + "." + ChildProperty.DisplayName,
                                                    Value = Property.Name + "." + ChildProperty.Name
                                                });
                                                lstPermissionDataItems.Add(Property.Name + "." + ChildProperty.Name);
                                            }
                                        }
                                    }
                                    //参与者
                                    else if (Property.LogicType == Data.DataLogicType.SingleParticipant
                                            || Property.LogicType == Data.DataLogicType.MultiParticipant)
                                        lstUserDataItems.Add(Property.Name);
                                    //逻辑型
                                    else if (Property.LogicType == Data.DataLogicType.Bool)
                                    {
                                        lstBoolDataItems.Add(Property.Name);
                                        //TODO
                                        listApprovalDataItem.Add(new { Text = Property.DisplayName, Value = Property.Name });

                                    }
                                    //文本
                                    else if (Property.LogicType == Data.DataLogicType.String)
                                    {

                                        listCommentDataItem.Add(new { Text = Property.DisplayName, Value = Property.Name });
                                    }

                                    //else if (Property.LogicType == Data.DataLogicType.TimeSpan)
                                    //    sltAllowedTime.Items.Add(new ListItem(Property.DisplayName, Property.Name));
                                }
                            }
                            model.NotifyCondition = listNotifyCondition;
                            model.ApprovalDataItem = listApprovalDataItem;
                            model.CommentDataItem = listCommentDataItem;
                        }

                        #endregion

                        #region 关联对象权限

                        if (BizObject.Associations != null)
                        {
                            foreach (DataModel.BizObjectAssociation association in BizObject.Associations)
                            {
                                DataModel.BizObjectSchema AssociatedSchema = this.Engine.BizObjectManager.GetPublishedSchema(association.AssociatedSchemaCode);
                                if (AssociatedSchema != null && AssociatedSchema.Properties != null)
                                {
                                    lstFrontDataItems.Add(new
                                    {
                                        Text = association.DisplayName,
                                        Value = association.Name,
                                        //标记子表标题行
                                        ItemType = "SubTableHeader"
                                    });
                                    lstPermissionDataItems.Add(association.Name);

                                    foreach (DataModel.PropertySchema AssociatedChildProperty in AssociatedSchema.Properties)
                                    {
                                        if (!DataModel.BizObjectSchema.IsReservedProperty(AssociatedChildProperty.Name))
                                        {
                                            lstFrontDataItems.Add(new
                                            {
                                                Text = association.DisplayName + "." + AssociatedChildProperty.DisplayName,
                                                Value = association.Name + "." + AssociatedChildProperty.Name
                                            });
                                            lstPermissionDataItems.Add(association.Name + "." + AssociatedChildProperty.Name);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 可选表单
                        //TODO
                        // sltSheetCodes.Items.Clear();
                        List<object> listSheetCodes = new List<object>();
                        Sheet.BizSheet[] Sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(Clause.BizSchemaCode);
                        if (Sheets != null)
                        {
                            foreach (Sheet.BizSheet Sheet in Sheets)
                            {

                                listSheetCodes.Add(new { Text = Sheet.DisplayName + "[" + Sheet.SheetCode + "]", Value = Sheet.SheetCode });
                            }
                        }
                        model.SheetCodes = listSheetCodes;
                        #endregion
                    }
                }
                model.DataItems = lstFrontDataItems;

                #endregion

                //设置可选的数据项 TODO 前台处理
                //SetDataItems(lstDataItems);

                #region 前置条件, Text前台需要解析多语言包
                // TODO
                List<object> listEntryCondition = new List<object>();
                listEntryCondition.Add(new { Text = "Designer.Designer_AnyOne", Value = (int)EntryConditionType.Any + string.Empty, Indext = 0 });
                listEntryCondition.Add(new { Text = "Designer.Designer_All", Value = (int)EntryConditionType.All + string.Empty, Index = 1 });
                model.EntryCondition = listEntryCondition;

                #endregion

                #region 同步异步

                List<object> listSync = new List<object>();
                listSync.Add(new { Text = "Designer.Designer_Sync", Value = "true", Index = 0 });
                listSync.Add(new { Text = "Designer.Designer_Async", Value = "false", Index = 1 });
                model.SyncOrASync = listSync;
                #endregion

                #region 参与者策略
                //TODO
                List<object> listParAbNormalPolicy = GetParAbnormalPolicy();
                model.ParAbnormalPolicy = listParAbNormalPolicy;


                #endregion

                #region 发起者参与者策略
                List<object> listOriginatorParAbnormalPolicy = GetOriginatorParAbnormalPolicy();
                model.OriginatorParAbnormalPolicy = listOriginatorParAbnormalPolicy;
                #endregion

                #region 参与方式:单人/多人
                //TODO
                List<object> listParticipantMode = new List<object>();
                listParticipantMode.Add(new { Text = "Designer.Designer_Single", Value = (int)ActivityParticipateType.SingleParticipant + string.Empty, Index = 0 });
                listParticipantMode.Add(new { Text = "Designer.Designer_Mulit", Value = (int)ActivityParticipateType.MultiParticipants + string.Empty, Index = 1 });
                model.ParticipantMode = listParticipantMode;
                #endregion

                #region 可选参与方式:并行/串行
                //TODO
                List<object> listParticipateMethod = new List<object>();
                listParticipateMethod.Add(new { Text = "Designer.Designer_Parallel", Value = (int)ParticipateMethod.Parallel + string.Empty, Index = 0 });
                listParticipateMethod.Add(new { Text = "Designer.Designer_Serial", Value = (int)ParticipateMethod.Serial + string.Empty, Index = 1 });
                model.ParticipateMethod = listParticipateMethod;

                #endregion

                #region 提交时检查
                //TODO
                List<object> listSubmittingValidation = new List<object>();
                listSubmittingValidation.Add(new { Text = "Designer.Designer_NotCheck", Value = (int)SubmittingValidationType.None + string.Empty, Index = 0 });
                listSubmittingValidation.Add(new { Text = "Designer.Designer_CheckParticipant", Value = (int)SubmittingValidationType.CheckNextParIsNull + string.Empty, Index = 1 });
                model.SubmittingValidation = listSubmittingValidation;

                #endregion

                #region 子流程数据映射类型

                List<object> lstMapTypes = new List<object>();
                foreach (string WorkflowDataMapInOutType in Enum.GetNames(typeof(OThinker.H3.WorkflowTemplate.WorkflowDataMap.InOutType)))
                {
                    //不需要使用InOutAppend
                    if (WorkflowDataMapInOutType != OThinker.H3.WorkflowTemplate.WorkflowDataMap.InOutType.InOutAppend.ToString())
                    {
                        lstMapTypes.Add(new
                        {
                            Text = WorkflowDataMapInOutType,
                            Value = (int)Enum.Parse(typeof(OThinker.H3.WorkflowTemplate.WorkflowDataMap.InOutType), WorkflowDataMapInOutType) + string.Empty
                        });
                    }
                }
                model.MapTypes = lstMapTypes;

                #endregion

                #region 表单锁
                //TODO

                List<object> listLockLevel = new List<object>();
                listLockLevel.Add(new { Text = "Designer.Designer_PopupManyPeople", Value = (int)LockLevel.Warning + string.Empty });
                listLockLevel.Add(new { Text = "Designer.Designer_ProhibitOneTime", Value = (int)LockLevel.Mono + string.Empty });
                listLockLevel.Add(new { Text = "Designer.Designer_CancelExclusive", Value = (int)LockLevel.CancelOthers + string.Empty });
                model.LockLevel = listLockLevel;

                #endregion

                #region 锁策略
                //TODO
                List<object> listLockPlicy = new List<object>();
                listLockPlicy.Add(new { Text = "Designer.Designer_NotLock", Value = (int)LockPolicy.None + string.Empty });
                listLockPlicy.Add(new { Text = "Designer.Designer_LockOpen", Value = (int)LockPolicy.Open + string.Empty });
                listLockPlicy.Add(new { Text = "Designer.Designer_LockRequest", Value = (int)LockPolicy.Request + string.Empty });
                model.LockPolicy = listLockPlicy;


                #endregion

                #region 超时策略
                //TODO
                List<object> listOvertimePolicy = new List<object>();
                listOvertimePolicy.Add(new { Text = "Designer.Designer_ExecuteStrategy", Value = (int)OvertimePolicy.None + string.Empty });
                listOvertimePolicy.Add(new { Text = "Designer.Designer_Approval", Value = (int)OvertimePolicy.Approve + string.Empty });
                //超时审批拒绝，暂时关闭
                //listOvertimePolicy.Add(new { Text = "Designer.Designer_ApprovalVote", Value = (int)OvertimePolicy.Disapprove + string.Empty });
                listOvertimePolicy.Add(new { Text = "Designer.Designer_AutoComplete", Value = (int)OvertimePolicy.Finish + string.Empty });
                listOvertimePolicy.Add(new { Text = "Designer.Designer_RemindStrategy1", Value = (int)OvertimePolicy.Remind1 + string.Empty });
                listOvertimePolicy.Add(new { Text = "Designer.Designer_RemindStrategy2", Value = (int)OvertimePolicy.Remind2 + string.Empty });
                //超时策略3,4在消息设置中去除，关闭
                //listOvertimePolicy.Add(new { Text = "Designer.Designer_RemindStrategy3", Value = (int)OvertimePolicy.Remind3 + string.Empty });
                //listOvertimePolicy.Add(new { Text = "Designer.Designer_RemindStrategy4", Value = (int)OvertimePolicy.Remind4 + string.Empty });
                model.OvertimePolicy = listOvertimePolicy;


                #endregion

                #region 数据项映射类型

                List<object> lstDataDisposalTypes = new List<object>();
                foreach (string DisposalTypeString in Enum.GetNames(typeof(DataDisposalType)))
                {
                    lstDataDisposalTypes.Add(new
                    {
                        Text = DisposalTypeString,
                        Value = (int)Enum.Parse(typeof(DataDisposalType), DisposalTypeString) + string.Empty
                    });
                }
                model.DataDisposalTypes = lstDataDisposalTypes;

                #endregion

                #region 初始化活动的数据权限

                if (WorkflowVersion == WorkflowDocument.NullWorkflowVersion && WorkflowTemplate != null && WorkflowTemplate.Activities != null)
                {//流程模板编码\显示名称
                    Dictionary<string, string> dicWorkflowNames = new Dictionary<string, string>();
                    //整理数据项权限
                    foreach (Activity Activity in WorkflowTemplate.Activities)
                    {
                        if (Activity is ParticipativeActivity)
                        {
                            
                            ParticipativeActivity ParticipativeActivity = (ParticipativeActivity)Activity;

                            List<string> lstValidPermissionItemNames = new List<string>();
                            List<DataItemPermission> lstValidDataItemPermissions = new List<DataItemPermission>();
                            if (ParticipativeActivity.DataPermissions == null)
                                ParticipativeActivity.DataPermissions = lstValidDataItemPermissions.ToArray();

                            if (ParticipativeActivity.DataPermissions != null)
                            {
                                foreach (DataItemPermission DataItemPermission in ParticipativeActivity.DataPermissions)
                                {
                                    if (lstPermissionDataItems.Contains(DataItemPermission.ItemName))
                                    {
                                        lstValidDataItemPermissions.Add(DataItemPermission);
                                        lstValidPermissionItemNames.Add(DataItemPermission.ItemName);
                                    }
                                }
                            }

                            foreach (string ItemName in lstPermissionDataItems)
                            {
                                if (!lstValidPermissionItemNames.Contains(ItemName))
                                {
                                    lstValidDataItemPermissions.Add(new DataItemPermission()
                                    {
                                        ItemName = ItemName,
                                        Visible = true,
                                        MobileVisible = true
                                    });
                                    lstValidPermissionItemNames.Add(ItemName);
                                }
                            }

                            ParticipativeActivity.DataPermissions = lstValidDataItemPermissions.ToArray();
                        }
                        //子流程:获取流程模板显示名称
                        else if (Activity is SubInstanceActivity)
                        {
                            string _SubWorkflowCode = ((SubInstanceActivity)Activity).WorkflowCode;
                            if (!string.IsNullOrEmpty(_SubWorkflowCode) && !dicWorkflowNames.ContainsKey(_SubWorkflowCode.ToLower()))
                            {
                                WorkflowClause _SubClause = this.Engine.WorkflowManager.GetClause(_SubWorkflowCode);
                                dicWorkflowNames.Add(_SubWorkflowCode.ToLower(), _SubClause == null ? _SubWorkflowCode : _SubClause.WorkflowName);
                            }
                        }
                    }
                    //流程模板编码\显示名称
                    model.WorkflowNames = dicWorkflowNames;

                }

                #endregion
                model.WorkflowTemplate = WorkflowTemplate;

                #region 数据项

                model.DataItemsSelect = GetALLDataItems(WorkflowCode);
                #endregion

                return Json(model, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 获取所有数据项
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <returns></returns>
        public List<FormulaTreeNode> GetALLDataItems(string SchemaCode)
        {

            List<FormulaTreeNode> listNodes = new List<FormulaTreeNode>();
            //根节点
            var RootNodeID = Guid.NewGuid().ToString();
            FormulaTreeNode AllDataNode = new FormulaTreeNode()
            {
                ObjectID = RootNodeID,
                Text = "Designer.Designer_AllDataItem",
                Value = "",
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png",
                ParentID = ""
            };

            listNodes.Add(AllDataNode);

            //系统数据项
            listNodes.AddRange(GetSystemDataItemNode(RootNodeID));

            //流程数据项
            listNodes.AddRange(GetDataItems(SchemaCode, RootNodeID));

            return listNodes;


        }


        #region 系统数据项-----------
        /// <summary>
        /// 获取流程系统数据项树节点
        /// </summary>
        /// <returns></returns>
        protected List<FormulaTreeNode> GetSystemDataItemNode(string ParentNodeID)
        {
            List<FormulaTreeNode> listNodes = new List<FormulaTreeNode>();
            KeyWordNode KeyWordNode = OThinker.H3.Instance.Keywords.ParserFactory.GetDataTreeNode();

            var FomulaRootID = Guid.NewGuid().ToString();

            FormulaTreeNode FormulaNode = new FormulaTreeNode()
            {
                ObjectID = FomulaRootID,
                Text = KeyWordNode.Text,
                Value = KeyWordNode.Text,
                IsLeaf = false,
                ParentID = ParentNodeID,
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png"
            };

            listNodes.Add(FormulaNode);
            listNodes.AddRange(CopyTreeNode(KeyWordNode, FomulaRootID));

            return listNodes;
        }

        /// <summary>
        /// 从KeyWordNode拷贝到TreeNode
        /// </summary>
        /// <param name="fromParentNode"></param>
        /// <param name="toParentNode"></param>
        List<FormulaTreeNode> CopyTreeNode(KeyWordNode fromParentNode, string ParentNodeID)
        {
            List<FormulaTreeNode> listNodes = new List<FormulaTreeNode>();
            if (fromParentNode != null)
            {
                if (fromParentNode.ChildNodes.Count == 0)
                {
                    //toParentNode.ChildNodes.Add(new TreeNode(fromParentNode.Text, fromParentNode.Text));
                }
                else
                {
                    foreach (KeyWordNode fromChild in fromParentNode.ChildNodes)
                    {
                        var NodeObjectID = Guid.NewGuid().ToString();

                        FormulaTreeNode FormulaNode = new FormulaTreeNode()
                        {
                            ObjectID = NodeObjectID,
                            Text = fromChild.Text,
                            Value = fromChild.Text,
                            ParentID = ParentNodeID,
                            FormulaType = FormulaType.FlowSystemVariables.ToString(),
                            IsLeaf = (fromChild.ChildNodes.Count == 0)
                        };
                        listNodes.Add(FormulaNode);
                        if (fromChild.ChildNodes.Count > 0)
                        {
                            FormulaNode.Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png";
                            listNodes.AddRange(CopyTreeNode(fromChild, NodeObjectID));
                        }
                        //toParentNode.ChildNodes.Add(toChild);

                    }
                }
            }

            return listNodes;
        }


        #endregion

        #region 流程数据项
        List<FormulaTreeNode> GetDataItems(string SchemaCode, string ParentID)
        {
            var RootNodeID = Guid.NewGuid().ToString();
            FormulaTreeNode flowDataNode = new FormulaTreeNode()
            {
                ObjectID = RootNodeID,
                Text = "Designer.Designer_InstanceDataItem",
                Value = "",
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png",
                ParentID = ParentID
            };
            List<FormulaTreeNode> lstDataItems = new List<FormulaTreeNode>();
            lstDataItems.Add(flowDataNode);

            WorkflowClause clause = this.Engine.WorkflowManager.GetClause(SchemaCode);
            if (clause != null) { SchemaCode = clause.BizSchemaCode; }
            if (!string.IsNullOrEmpty(SchemaCode))
            {
                DataModel.BizObjectSchema BizSchema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);

                if (BizSchema != null)
                {
                    if (BizSchema != null && BizSchema.Properties != null)
                    {

                        foreach (DataModel.PropertySchema Property in BizSchema.Properties)
                        {
                            //不显示保留数据项
                            if (!DataModel.BizObjectSchema.IsReservedProperty(Property.Name))
                            {
                                lstDataItems.Add(new FormulaTreeNode()
                                {
                                    ObjectID = Guid.NewGuid().ToString(),
                                    IsLeaf = true,
                                    ParentID = RootNodeID,
                                    Text = Property.DisplayName,
                                    Value = Property.Name
                                });
                                ;
                            }
                        }


                    }
                }
            }

            return lstDataItems;
        }

        #endregion

        #region 绑定参与者策略

        /// <summary>
        /// 获取可选参与者策略
        /// </summary>
        /// <param name="DropDownList"></param>
        List<object> GetParAbnormalPolicy()
        {
            List<object> lstObj = new List<object>();
            lstObj.Add(new { Text = "Designer.Designer_NotHandle", Value = (int)ParAbnormalPolicy.Default + string.Empty });
            lstObj.Add(new { Text = "Designer.Designer_LastResult", Value = (int)ParAbnormalPolicy.CopyLastApproval + string.Empty });
            lstObj.Add(new { Text = "Designer.Designer_Approval", Value = (int)ParAbnormalPolicy.Approve + string.Empty });

            return lstObj;
        }

        #endregion

        #region 绑定发起者参与者策略

        /// <summary>
        /// 获取发起者参与策略
        /// </summary>
        /// <returns></returns>
        List<object> GetOriginatorParAbnormalPolicy()
        {
            List<object> lstObj = new List<object>();
            lstObj.Add(new { Text = "Designer.Designer_NotHandle", Value = (int)ParAbnormalPolicy.Default + string.Empty });
            lstObj.Add(new { Text = "Designer.Designer_Approval", Value = (int)ParAbnormalPolicy.Approve + string.Empty });
            return lstObj;
        }
        #endregion

        /// <summary>
        /// 异常节点
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        private List<string> ExceptionActivities(string InstanceId)
        {
            //异常活动
            List<string> lstExceptionActivities = new List<string>();
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
            Exceptions.ExceptionLog[] ExceptionLogs = this.Engine.ExceptionManager.GetExceptionsByInstance(InstanceId);
            if (ExceptionLogs != null)
            {

                foreach (Exceptions.ExceptionLog ExceptionLog in ExceptionLogs)
                {
                    if (ExceptionLog.State == Exceptions.ExceptionState.Unfixed)
                    {
                        InstanceContext.State = Instance.InstanceState.Exceptional;
                        if (ExceptionLog.SourceType == Instance.RuntimeObjectType.Activity)
                        {
                            lstExceptionActivities.Add(ExceptionLog.SourceName);
                        }
                    }
                }
            }
            return lstExceptionActivities;
        }
    }
}
