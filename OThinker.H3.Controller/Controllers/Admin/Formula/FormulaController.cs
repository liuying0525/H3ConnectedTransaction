using Newtonsoft.Json;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance.Keywords;
using OThinker.H3.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Admin.Formula
{
    public class FormulaController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 验证公式
        /// </summary>
        /// <param name="Formula"></param>
        /// <param name="SchemaCode">流程模板编码</param>
        /// <param name="RuleCode"></param>
        /// <returns></returns>
        public JsonResult Validate(string Formula, string SchemaCode, string RuleCode)
        {
            return ExecuteFunctionRun(() =>
            {
                //校验结果
                ActionResult result = new ActionResult(true, "");

                Formula = Server.HtmlDecode(Formula);
                //错误项
                string[] errorItems = null;
                //错误信息
                List<string> Errors = new List<string>();

                if (!string.IsNullOrEmpty(Formula))
                {
                    // 所有数据项的名称
                    Dictionary<string, string> formulaItemNames = new Dictionary<string, string>();
                    string formulaId = Guid.NewGuid().ToString();
                    formulaItemNames.Add(formulaId, Formula);

                    //所有项名称:数据项、流程关键字
                    List<string> allItemNames = new List<string>();

                    //流程关键字
                    string[] words = OThinker.H3.Instance.Keywords.ParserFactory.GetKeywords();
                    allItemNames.AddRange(words);

                    //业务模型数据项
                    //string SchemaCode = CurrentParams[ConstantString.Param_SchemaCode];
                    if (!string.IsNullOrEmpty(SchemaCode))
                    {
                        DataModel.BizObjectSchema Schema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                        //如果一个流程包含有多个流程模板的时候，需要重新计算一下shcema
                        if (Schema == null)
                        {
                            WorkflowTemplate.WorkflowClause clause = this.Engine.WorkflowManager.GetClause(SchemaCode);
                            if (clause != null)
                            {
                                Schema = this.Engine.BizObjectManager.GetDraftSchema(clause.BizSchemaCode);
                            }
                        }
                        if (Schema != null)
                        {
                            if (Schema.IsQuotePacket)
                            {
                                Schema = this.Engine.BizObjectManager.GetDraftSchema(Schema.BindPacket);
                            }
                            foreach (DataModel.PropertySchema item in Schema.Properties)
                            {
                                allItemNames.Add(item.Name);
                            }
                        }
                    }

                    //string RuleCode = CurrentContext.Request[ConstantString.Param_RuleCode];
                    // 业务规则数据项
                    if (!string.IsNullOrEmpty(RuleCode))
                    {
                        OThinker.H3.BizBus.BizRule.BizRuleTable rule = this.Engine.BizBus.GetBizRule(RuleCode);
                        if (rule != null)
                        {
                            if (rule.DataElements != null)
                            {
                                foreach (OThinker.H3.BizBus.BizRule.BizRuleDataElement RuleElement in rule.DataElements)
                                {
                                    allItemNames.Add(RuleElement.ElementName);
                                }
                            }
                        }
                    }

                    Function[] fs = FunctionFactory.Create(this.Engine.Organization, this.Engine.BizBus);
                    Dictionary<string, Function> fDic = new Dictionary<string, Function>();
                    if (fs != null)
                    {
                        foreach (OThinker.H3.Math.Function f in fs)
                        {
                            fDic.Add(f.FunctionName, f);
                        }
                    }

                    result.Success = FormulaParser.Validate(Formula, fDic, allItemNames, ref Errors);

                    result.Message = string.Join(";", Errors);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取公式树
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="RuleCode"></param>
        /// <returns></returns>
        public JsonResult GetFomulaTreeData(string SchemaCode, string RuleCode)
        {
            return ExecuteFunctionRun(() =>
            {
                List<FormulaTreeNode> treeDataList = GetFormulaTree(SchemaCode, RuleCode);
                return Json(treeDataList, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取组织对应的名称
        /// </summary>
        /// <param name="UnitKeys"></param>
        /// <returns></returns>
        public JsonResult GetUnitNames(string UnitKeys)
        {
            return ExecuteFunctionRun(() =>
            {
                try
                {
                    string[] UserKeys = (string[])JsonConvert.DeserializeObject(UnitKeys, typeof(string[]));
                    if (UserKeys != null && UserKeys.Length > 0)
                    {
                        OThinker.Organization.IOrganization _Organization = this.Engine.Organization;
                        List<object> lstUnitNames = new List<object>();

                        foreach (string Key in UserKeys)
                        {
                            OThinker.Organization.Unit Unit = _Organization.GetUnit(Key);
                            if (Unit == null)
                            {
                                Unit = _Organization.GetUnit(Key);
                                if (Unit == null) { Unit = _Organization.GetUserByCode(Key); }
                            }

                            if (Unit != null)
                            {
                                lstUnitNames.Add(new
                                {
                                    Key = Key,
                                    Name = Unit.Name
                                });
                            }
                        }
                        return Json(lstUnitNames, JsonRequestBehavior.AllowGet);
                    }
                }
                catch { }

                return null;
            });
        }

        /// <summary>
        /// 获取提示信息
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="RuleCode"></param>
        /// <returns></returns>
        public JsonResult GetFormulaTips(string SchemaCode, string RuleCode)
        {
            return ExecuteFunctionRun(() =>
            {
                FormulaTipViewModel model = new FormulaTipViewModel();

                #region 参与者函数

                List<object> lstParticipantFunctions = new List<object>();
                foreach (OThinker.H3.Math.Function Function in OThinker.H3.Math.FunctionFactory.Create(this.Engine.Organization, this.Engine.BizBus))
                {
                    if (Function is OThinker.H3.Math.Function)
                    {
                        //ERROR:这两个函数名称不是Public所以不能访问,下行代码写死了函数名称
                        //以下两个函数不显示
                        if (Function.FunctionName == "OrgCodeToID" || Function.FunctionName == "OrgIDToCode")
                        {
                            continue;
                        }

                        OThinker.H3.Math.Parameter returns = Function.GetHelper().Return;
                        //构造脚本
                        lstParticipantFunctions.Add(new
                        {
                            FunctionName = Function.FunctionName,
                            Helper = ((OThinker.H3.Math.Function)Function).GetHelper(),
                            ReturnType = returns == null ? null : returns.LogicTypes
                        });


                    }
                }
                model.ParticipantFunctions = lstParticipantFunctions;

                #endregion

                #region 参数类型

                // <Key,{Name,DisplayName}>
                Dictionary<string, object> LogicTypeDictionary = new Dictionary<string, object>();
                foreach (Data.DataLogicType LogicType in Enum.GetValues(typeof(Data.DataLogicType)))
                {
                    LogicTypeDictionary.Add(((int)LogicType).ToString(), new { Name = LogicType.ToString(), DisplayName = Data.DataLogicTypeConvertor.ToLogicTypeName(LogicType) });
                }
                model.LogicTypes = LogicTypeDictionary;
                #endregion

                #region 规则环境

                if (!string.IsNullOrWhiteSpace(RuleCode))
                {
                    OThinker.H3.BizBus.BizRule.BizRuleTable rule = this.Engine.BizBus.GetBizRule(RuleCode);
                    List<object> lstRuleElement = new List<object>();
                    if (rule != null)
                    {

                        if (rule.DataElements != null)
                        {
                            foreach (OThinker.H3.BizBus.BizRule.BizRuleDataElement RuleElement in rule.DataElements)
                            {
                                //词汇表
                                lstRuleElement.Add(new
                                {
                                    Name = RuleElement.ElementName,
                                    DisplayName = RuleElement.DisplayName,
                                    LogicType = RuleElement.LogicType
                                });
                            }
                        }
                    }

                    model.RuleElement = lstRuleElement;
                }

                #endregion

                #region 流程环境

                if (!string.IsNullOrEmpty(SchemaCode))
                {
                    ////系统数据
                    List<FormulaTreeNode> listSystemDatas = GetSystemDataItemNode("");

                    ////获取所有节点的文本集合,供智能感知
                    List<string> lstNodeStrings = new List<string>();
                    AddTreeString(listSystemDatas, lstNodeStrings);

                    model.InstanceVariables = lstNodeStrings;

                }

                #endregion

                #region 数据模型环境
                if (!string.IsNullOrEmpty(SchemaCode))
                {
                    DataModel.BizObjectSchema BizSchema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);

                    if (BizSchema != null)
                    {
                        if (BizSchema != null && BizSchema.Properties != null)
                        {
                            List<object> lstDataItems = new List<object>();
                            foreach (DataModel.PropertySchema Property in BizSchema.Properties)
                            {
                                //不显示保留数据项
                                if (!DataModel.BizObjectSchema.IsReservedProperty(Property.Name))
                                {
                                    lstDataItems.Add(new
                                    {
                                        Name = Property.Name,
                                        DisplayName = Property.DisplayName,
                                        LogicType = Property.LogicType
                                    });
                                    ;
                                }
                            }

                            model.DataItems = lstDataItems;
                        }
                    }
                }

                #endregion

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 创建公式编辑器左边树
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected List<FormulaTreeNode> GetFormulaTree(string SchemaCode, string RuleCode)
        {
            List<FormulaTreeNode> treeNodeList = new List<FormulaTreeNode>();

            #region 输入
            string InputObjectID = Guid.NewGuid().ToString();//构造一个ObjectID
            FormulaTreeNode InputNode = new FormulaTreeNode()
            {
                ObjectID = InputObjectID,
                Text = "FormulaEditor.FormulaEditor_Input",//显示
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/in.png",//图标
                IsLeaf = true,
                LoadDataUrl = "",//加载数据，组织结构时使用
                FormulaType = FormulaType.Input.ToString(),//判断类型
                Value = "",//存储值，点击时传递该值
                ParentID = ""
            };
            treeNodeList.Add(InputNode);
            #endregion

            #region 常量

            string BlockObjectID = Guid.NewGuid().ToString();//构造一个ObjectID
            FormulaTreeNode BlockNode = new FormulaTreeNode()
            {
                ObjectID = BlockObjectID,
                Text = "FormulaEditor.FormulaEditor_Constant",//显示
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png",//图标
                IsLeaf = false,
                LoadDataUrl = "",//加载数据，组织结构时使用
                FormulaType = FormulaType.Input.ToString(),//判断类型
                Value = "",//存储值，点击时传递该值
                ParentID = ""
            };

            treeNodeList.Add(BlockNode);

            List<string> BlockValus = new List<string>();
            BlockValus.Add("True");
            BlockValus.Add("False");
            BlockValus.Add("null");

            foreach (string block in BlockValus)
            {
                FormulaTreeNode constNode = new FormulaTreeNode()
                {
                    ObjectID = Guid.NewGuid().ToString(),
                    Text = block,//显示
                    Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/const.png",//图标
                    IsLeaf = true,
                    LoadDataUrl = "",//加载数据，组织结构时使用
                    FormulaType = FormulaType.Block.ToString(),//判断类型
                    Value = block,//存储值，点击时传递该值
                    ParentID = BlockObjectID
                };

                treeNodeList.Add(constNode);
            }

            #endregion

            #region 函数

            var FunctionRootID = Guid.NewGuid().ToString();
            FormulaTreeNode FunctionParentNode = new FormulaTreeNode()
            {
                ObjectID = FunctionRootID,
                Text = "FormulaEditor.FormulaEditor_Funciton",
                Value = "",
                ParentID = "",
                LoadDataUrl = "",
                IsLeaf = false,
                FormulaType = FormulaType.Function.ToString(),
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png" //图标
            };
            treeNodeList.Add(FunctionParentNode);

            #endregion

            #region 参与者函数

            List<object> lstParticipantFunctions = new List<object>();
            foreach (OThinker.H3.Math.Function Function in OThinker.H3.Math.FunctionFactory.Create(this.Engine.Organization, this.Engine.BizBus))
            {
                if (Function is OThinker.H3.Math.Function)
                {
                    //ERROR:这两个函数名称不是Public所以不能访问,下行代码写死了函数名称
                    //以下两个函数不显示
                    if (Function.FunctionName == "OrgCodeToID" || Function.FunctionName == "OrgIDToCode")
                    {
                        continue;
                    }

                    OThinker.H3.Math.Parameter returns = Function.GetHelper().Return;
                    //构造脚本
                    lstParticipantFunctions.Add(new
                    {
                        FunctionName = Function.FunctionName,
                        Helper = ((OThinker.H3.Math.Function)Function).GetHelper(),
                        ReturnType = returns == null ? null : returns.LogicTypes
                    });

                    //构造树节点
                    FormulaTreeNode FunctionNode = new FormulaTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Text = Function.FunctionName,
                        Value = Function.FunctionName,
                        ParentID = FunctionRootID,
                        LoadDataUrl = "",
                        IsLeaf = true,
                        FormulaType = FormulaType.ParticipantFunction.ToString(),
                        Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/pf.png" //图标
                    };
                    treeNodeList.Add(FunctionNode);
                }
            }

            #endregion

            #region 参数类型

            // <Key,{Name,DisplayName}>
            Dictionary<string, object> LogicTypeDictionary = new Dictionary<string, object>();
            foreach (Data.DataLogicType LogicType in Enum.GetValues(typeof(Data.DataLogicType)))
            {
                LogicTypeDictionary.Add(((int)LogicType).ToString(), new { Name = LogicType.ToString(), DisplayName = Data.DataLogicTypeConvertor.ToLogicTypeName(LogicType) });
            }
            //TODO
            //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "LogicTypes", "var LogicTypes=" + this.JSSerializer.Serialize(LogicTypeDictionary) + ";", true);

            #endregion

            #region 规则环境

            if (!string.IsNullOrWhiteSpace(RuleCode))
            {
                OThinker.H3.BizBus.BizRule.BizRuleTable rule = this.Engine.BizBus.GetBizRule(RuleCode);
                if (rule != null)
                {
                    string RuleRootID = Guid.NewGuid().ToString();
                    bool isleaf = rule.DataElements == null;
                    FormulaTreeNode RuleRootNode = new FormulaTreeNode()
                    {
                        ObjectID = RuleRootID,
                        Text = "FormulaEditor.FormulaEditor_Vocabulary",
                        Value = "",
                        ParentID = "",
                        LoadDataUrl = "",
                        IsLeaf = isleaf,
                        FormulaType = FormulaType.RuleElement.ToString(),
                        Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png" //图标
                    };

                    treeNodeList.Add(RuleRootNode);

                    if (rule.DataElements != null)
                    {
                        foreach (OThinker.H3.BizBus.BizRule.BizRuleDataElement RuleElement in rule.DataElements)
                        {
                            //词汇表
                            FormulaTreeNode RuleElementNode = new FormulaTreeNode()
                            {
                                ObjectID = Guid.NewGuid().ToString(),
                                Text = RuleElement.DisplayName + "[" + RuleElement.ElementName + "]",
                                Value = RuleElement.ElementName,
                                ParentID = RuleRootID,
                                LoadDataUrl = "",
                                IsLeaf = true,
                                FormulaType = FormulaType.RuleElement.ToString(),
                                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/di.png" //图标
                            };

                            treeNodeList.Add(RuleElementNode);
                        }
                    }
                }
            }

            #endregion

            #region 流程环境

            if (!string.IsNullOrEmpty(SchemaCode))
            {
                //系统数据
                List<FormulaTreeNode> listSystemDatas = GetSystemDataItemNode("");

                treeNodeList.AddRange(listSystemDatas);
            }

            #endregion

            #region 数据模型环境
            if (!string.IsNullOrEmpty(SchemaCode))
            {
                DataModel.BizObjectSchema BizSchema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                //如果 BizShceMa为空，把SchemaCode 作为流程编码查询对应的SchemaCode然后再查询
                if (BizSchema == null)
                {
                    WorkflowTemplate.WorkflowClause clause = this.Engine.WorkflowManager.GetClause(SchemaCode);
                    {
                        if (clause != null)
                        {
                            BizSchema = this.Engine.BizObjectManager.GetDraftSchema(clause.BizSchemaCode);
                        }
                    }
                }


                if (BizSchema != null)
                {
                    if (BizSchema.IsQuotePacket)
                    {
                        BizSchema = this.Engine.BizObjectManager.GetDraftSchema(BizSchema.BindPacket);
                    }
                    string SchemaRootID = Guid.NewGuid().ToString();
                    FormulaTreeNode SchemaRootNode = new FormulaTreeNode()
                    {
                        ObjectID = SchemaRootID,
                        Text = "FormulaEditor.FormulaEditor_BusinessPorperty",
                        Value = "",
                        ParentID = "",
                        LoadDataUrl = "",
                        IsLeaf = false,
                        FormulaType = FormulaType.BizObjectSchema.ToString(),
                        Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png" //图标
                    };



                    if (BizSchema != null && BizSchema.Properties != null)
                    {
                        List<object> lstDataItems = new List<object>();
                        foreach (DataModel.PropertySchema Property in BizSchema.Properties)
                        {
                            //不显示保留数据项
                            if (!DataModel.BizObjectSchema.IsReservedProperty(Property.Name))
                            {
                                lstDataItems.Add(new
                                {
                                    Name = Property.Name,
                                    DisplayName = Property.DisplayName,
                                    LogicType = Property.LogicType
                                });
                                TreeNode DataItemNode = new TreeNode(Property.FullName, Property.Name);
                                DataItemNode.NavigateUrl = "javascript:FormulaSettings.InsertVariable('" + Property.Name + "')";
                                DataItemNode.ImageUrl = "../../WFRes/_Content/designer/image/formula/di.png";
                                FormulaTreeNode SchemaNode = new FormulaTreeNode()
                                {
                                    ObjectID = Guid.NewGuid().ToString(),
                                    Text = Property.DisplayName + "[" + Property.Name + "]",
                                    Value = Property.Name,
                                    ParentID = SchemaRootID,
                                    LoadDataUrl = "",
                                    IsLeaf = true,
                                    FormulaType = FormulaType.BizObjectSchema.ToString(),
                                    Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/di.png" //图标
                                };

                                treeNodeList.Add(SchemaNode);
                            }
                        }

                        if (lstDataItems.Count == 0) { SchemaRootNode.IsLeaf = true; }
                        treeNodeList.Add(SchemaRootNode);

                        //ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "FunctionNames", "var DataItems=" + this.JSSerializer.Serialize(lstDataItems) + ";", true);
                    }
                }
            }

            #endregion

            #region 符号

            string OperatorRootID = Guid.NewGuid().ToString();
            FormulaTreeNode OperatorRootNode = new FormulaTreeNode()
            {
                ObjectID = OperatorRootID,
                Text = "FormulaEditor.FormulaEditor_Symbol",
                Value = "",
                ParentID = "",
                LoadDataUrl = "",
                IsLeaf = false,
                FormulaType = FormulaType.Operator.ToString(),
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png" //图标
            };

            treeNodeList.Add(OperatorRootNode);

            //加减乘除
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Plus) + " +", Value = "+", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Minus) + " -", Value = "-", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Mul) + " *", Value = "*", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Div) + " /", Value = "/", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Set) + " =", Value = "=", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });

            //大小等于
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Gr) + " >", Value = ">", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.GrEq) + " >=", Value = ">=", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });

            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Ls) + " <", Value = "<", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.LsEq) + " <=", Value = "<=", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Eq) + " ==", Value = "==", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.NtEq) + " !=", Value = "!=", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });

            //与或非
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.And) + " &&", Value = "&&", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Or) + " ||", Value = "||", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.Not) + " !", Value = "!", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });

            //左/右括号
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.LeftPar) + " (", Value = "(", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });
            treeNodeList.Add(new FormulaTreeNode() { ObjectID = Guid.NewGuid().ToString(), ParentID = OperatorRootID, IsLeaf = true, Text = Enum.GetName(typeof(OThinker.H3.Math.Operator), OThinker.H3.Math.Operator.RightPar) + " )", Value = ")", Icon = this.PortalRoot + "/WFRes/_Content/designer//image/formula/op.png", FormulaType = FormulaType.Operator.ToString() });

            #endregion

            #region 组织架构

            var OrgRootID = this.Engine.Organization.RootUnit.ObjectID;
            FormulaTreeNode OrgRootNode = new FormulaTreeNode()
            {
                ObjectID = OrgRootID,
                Text = this.Engine.Organization.RootUnit.Name,
                Value = OrgRootID,
                ParentID = "",
                LoadDataUrl = this.PortalRoot + "/Formula/LoadTreeData?unitID=" + OrgRootID,
                IsLeaf = false,
                FormulaType = FormulaType.Organization.ToString(),
                Icon = this.PortalRoot + "/WFRes/_Content/designer/image/formula/folder_16.png" //图标
            };

            treeNodeList.Add(OrgRootNode);

            ////选中事件，前台处理
            //if (!string.IsNullOrEmpty(Company.Code))
            //    UserTree.NavigateUrl = "javascript:FormulaSettings.InsertUser(" + JSSerializer.Serialize(UserTree.Text) + "," + JSSerializer.Serialize(Company.Code) + ")";
            //else
            //    UserTree.NavigateUrl = "javascript:FormulaSettings.InsertUser(" + JSSerializer.Serialize(UserTree.Text) + ",'" + JSSerializer.Serialize(Company.ObjectID) + "')";

            #endregion

            return treeNodeList;
        }


        #region 创建组织机构的树节点

        /// <summary>
        /// 获取组织机构子节点
        /// </summary>
        /// <param name="unitID"></param>
        /// <param name="UnitType"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public JsonResult LoadTreeData(string unitID, OThinker.Organization.UnitType UnitType = OThinker.Organization.UnitType.Unspecified, bool Recursive = false)
        {
            return ExecuteFunctionRun(() =>
            {
                object treeObj = LoadOrgChildNode(unitID, UnitType, Recursive);
                return Json(treeObj, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 创建组织结构孩子节点
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        private List<FormulaTreeNode> LoadOrgChildNode(
            string unitID,
            OThinker.Organization.UnitType UnitType = OThinker.Organization.UnitType.Unspecified,
            bool Recursive = false)
        {

            List<OThinker.Organization.Unit> stateFilteredChildren = this.Engine.Organization.GetChildUnits(
                unitID,
                UnitType,
                Recursive,
                 OThinker.Organization.State.Active);

            // 按照可见类型进行过滤
            // 按照OrganizationUnit, Group, User的顺序进行排列
            ArrayList unitList = SortOrgList(stateFilteredChildren.ToArray());

            List<FormulaTreeNode> treeNode = new List<FormulaTreeNode>();
            foreach (OThinker.Organization.Unit child in unitList)
            {
                if (treeNode.Any(p => p.ObjectID == child.ObjectID)) continue;

                switch (child.UnitType)
                {
                    case OThinker.Organization.UnitType.OrganizationUnit:
                        treeNode.Add(BuildOrgChildNode(unitID, child, "fa icon-zuzhitubiao"));
                        break;
                    case OThinker.Organization.UnitType.Group:
                        treeNode.Add(BuildOrgChildNode(unitID, child, "fa fa-users"));
                        break;
                    case OThinker.Organization.UnitType.User:
                        string iconName = "fa fa-user";
                        treeNode.Add(BuildOrgChildNode(unitID, child, iconName));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return treeNode;
        }

        /// <summary>
        /// 创建组织机构的树节点
        /// </summary>
        /// <param name="unitID"></param>
        /// <param name="child"></param>
        /// <param name="imgName"></param>
        /// <returns></returns>
        private FormulaTreeNode BuildOrgChildNode(string unitID, OThinker.Organization.Unit child, string imgName)
        {
            //组织机构全部使用OBJECTID 作参数
            string Code = child.ObjectID;
            //if (child.UnitType == OThinker.Organization.UnitType.User) { Code = ((OThinker.Organization.User)child).Code; }

            FormulaTreeNode node = new FormulaTreeNode()
            {
                Text = "_" + child.Name,//在资源文件解析的时候，如果以_开头，则返回_之后的字符串，否则会根据资源文件里面的字符串替换，组织结构名称会找不到
                ObjectID = child.ObjectID,
                Value = Code,
                IsLeaf = child.UnitType == OThinker.Organization.UnitType.User,
                Icon = imgName,
                ParentID = unitID,
                FormulaType = FormulaType.Organization.ToString()
            };
            if (!node.IsLeaf)
            {
                node.LoadDataUrl = this.PortalRoot + "/Formula/LoadTreeData?unitID=" + child.ObjectID;
            }
            return node;
        }
        #endregion

        #region 排序组织列表
        /// <summary>
        /// 排序组织列表
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private ArrayList SortOrgList(OThinker.Organization.Unit[] children)
        {
            ArrayList unitList = new ArrayList();
            ArrayList keyList = new ArrayList();
            foreach (OThinker.Organization.Unit child in children)
            {
                //编制已取消
                //if (child.UnitType == OThinker.Organization.UnitType.Staff) continue;
                string key = null;
                switch (child.UnitType)
                {
                    case OThinker.Organization.UnitType.OrganizationUnit:
                        key = "c" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    case OThinker.Organization.UnitType.Group:
                        key = "e" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    case OThinker.Organization.UnitType.User:
                        key = "f" + string.Format("{0:D8}", child.SortKey) + child.Name;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                keyList.Add(key);
                unitList.Add(child);
            }
            unitList = OThinker.Data.Sorter.Sort(keyList, unitList);
            return unitList;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取所有节点的文本集合
        /// </summary>
        /// <param name="TreeNode"></param>
        /// <param name="lstNodeStrings"></param>
        void AddTreeString(List<FormulaTreeNode> listSystemDatas, List<string> lstNodeStrings)
        {
            if (lstNodeStrings == null)
                lstNodeStrings = new List<string>();

            if (listSystemDatas != null)
            {
                foreach (FormulaTreeNode node in listSystemDatas)
                {
                    if (!lstNodeStrings.Contains(node.Text))
                    { lstNodeStrings.Add(node.Text); }
                }
            }
        }

        #region 从KeyWordNode拷贝到TreeNode ------------
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
                FormulaType = FormulaType.FlowSystemVariables.ToString(),
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

        #endregion
    }


    /// <summary>
    /// 公式编辑左边树类型
    /// </summary>
    public enum FormulaType
    {
        /// <summary>
        /// 输入
        /// </summary>
        Input,
        /// <summary>
        /// 常量
        /// </summary>
        Block,
        /// <summary>
        /// 一般函数
        /// </summary>
        Function,
        /// <summary>
        /// 参与者函数
        /// </summary>
        ParticipantFunction,
        /// <summary>
        /// 参数类型
        /// </summary>
        LogicType,
        /// <summary>
        /// 规则词汇表
        /// </summary>
        RuleElement,
        /// <summary>
        /// 流程系统数据
        /// </summary>
        FlowSystemVariables,
        /// <summary>
        /// 数据模型
        /// </summary>
        BizObjectSchema,
        /// <summary>
        /// 符号
        /// </summary>
        Operator,
        /// <summary>
        /// 组织结构
        /// </summary>
        Organization
    }
}
