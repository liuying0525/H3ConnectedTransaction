using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.BizBus.BizRule;

namespace OThinker.H3.Controllers.Controllers.BizRule
{
    public class BizRuleDecisionMatrixController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        #region 决策表一般操作
        /// <summary>
        /// 添加决策表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult AddDecisionMatrix(BizRuleDecisionMatrixViewModel model)
        {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult(false,"");

                BizRuleTable Rule = Engine.BizBus.GetBizRule(model.RuleCode);//业务规则

                if (string.IsNullOrEmpty(model.Code))
                {
                    result.Message = "msgGlobalString.InvalidCode";
                    return Json(result);
                }

                if (Rule.GetDecisionMatrix(model.Code) != null)
                {
                    result.Message = "msgGlobalString.CodeExisted";
                    return Json(result);
                }

                //决策表必须以字母开始，不让创建到数据库表字段时报错
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(model.Code).Success)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg4";
                    return Json(result);
                }

                //决策表名称不能含有特殊字符
                if (!regex.Match(model.DisplayName).Success)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.NameInValid";
                    return Json(result);
                }

                OThinker.H3.BizBus.BizRule.BizRuleDecisionMatrix matrix = OThinker.H3.BizBus.BizRule.DecisionMatrixFactory.Create(
                    (DecisionMatrixType)Enum.Parse(typeof(DecisionMatrixType),
                    model.DecisionMatrixType),
                    model.Code);
                matrix.DisplayName = model.DisplayName;
                matrix.Description = model.Description;
                matrix.DecisionMatrixScope = (DecisionMatrixScope)Enum.Parse(typeof(DecisionMatrixScope), model.Scope);
                if (!Rule.AddDecisionMatrix(matrix))
                {
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result);
                }
                if (!this.Engine.BizBus.UpdateBizRule(Rule))
                {
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result);
                }
                result.Success = true;
                result.Message = "msgGlobalString.SaveSucced";
                return Json(result);
            });
        }

        /// <summary>
        /// 更新决策表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult EditDecisionMatrix(BizRuleDecisionMatrixViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false,"");

                BizRuleTable Rule = Engine.BizBus.GetBizRule(model.RuleCode);//业务规则
                

                if (string.IsNullOrEmpty(model.Code))
                {
                    result.Message = "msgGlobalString.InvalidCode";
                    return Json(result);
                }

                BizRuleDecisionMatrix matrix = Rule.GetDecisionMatrix(model.Code);//决策表

                //决策表名称不能含有特殊字符
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(model.DisplayName).Success)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.NameInValid";
                    return Json(result);
                }

                matrix.DisplayName = model.DisplayName;
                matrix.Description = model.Description;
                //matrix.MatrixType = (DecisionMatrixType)Enum.Parse(typeof(DecisionMatrixType), model.DecisionMatrixType);

                matrix.RowExecutionType = (ExecutionType)Enum.Parse(typeof(ExecutionType), model.RowExecutionType);
                matrix.ColumnExecutionType = (ExecutionType)Enum.Parse(typeof(ExecutionType), model.ColumnExecutionType);
                matrix.DecisionMatrixScope = (DecisionMatrixScope)Enum.Parse(typeof(DecisionMatrixScope), model.Scope);
                // 保存结果的字段
                switch (matrix.MatrixType)
                {
                    case DecisionMatrixType.Script:
                        break;
                    case DecisionMatrixType.SelectiveArray:
                        ((SelectiveArrayDecisionMatrix)matrix).ResultElementData = model.ResultField;
                        break;
                    case DecisionMatrixType.SortedArray:
                        ((SortedArrayDecisionMatrix)matrix).ResultElementData = model.ResultField;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                bool flag = this.Engine.BizBus.UpdateBizRule(Rule);
                if (flag)
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.SaveSucced";
                }
                else
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                }
                return Json(result);
            });
        }

        /// <summary>
        /// 移除决策表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult RemoveDecisionMatrix(string ruleCode, string matrixCode)
        {
            return ExecuteFunctionRun(() =>
            {
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);

                ActionResult result = new ActionResult(false, "msgGlobalString.DeleteFailed");
                if (!Rule.RemoveDecisionMatrix(DecisionMatrix.Code))
                {

                    return Json(result);
                }
                string code = Rule.Code;
                if (!Engine.BizBus.UpdateBizRule(Rule))
                {
                    //删除失败
                    return Json(result);
                }
               
                result.Success = true;
                result.Message = "msgGlobalString.DeleteSucced";
                return Json(result);
            });
        }

        /// <summary>
        /// 加载决策表信息
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        public JsonResult LoadDecisionMatrixInfo(string ruleCode, string matrixCode)
        {
            return ExecuteFunctionRun(() =>
            {
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);
                if (DecisionMatrix == null) { return null; }

                BizRuleDecisionMatrixViewModel model = new BizRuleDecisionMatrixViewModel();
                model.ObjectID = DecisionMatrix.GetHashCode().ToString();//判断是否新增使用，可随便赋值
                model.Code = DecisionMatrix.Code;
                model.RuleCode = ruleCode;
                model.RowExecutionType = DecisionMatrix.RowExecutionType.ToString();
                model.ColumnExecutionType = DecisionMatrix.ColumnExecutionType.ToString();
                model.DisplayName = DecisionMatrix.DisplayName;
                model.Description = DecisionMatrix.Description;
                model.DecisionMatrixType = DecisionMatrix.MatrixType.ToString();
                model.Scope = DecisionMatrix.DecisionMatrixScope.ToString();
                switch (DecisionMatrix.MatrixType)
                {
                    case DecisionMatrixType.Script:
                        model.ResultField = "";
                        break;
                    case DecisionMatrixType.SelectiveArray:
                       
                        model.ResultField = ((SelectiveArrayDecisionMatrix)DecisionMatrix).ResultElementData;
                        break;
                    case DecisionMatrixType.SortedArray:

                        model.ResultField = ((SortedArrayDecisionMatrix)DecisionMatrix).ResultElementData;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                return Json(model,JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region 决策表行列信息查询

        /// <summary>
        /// 获取决策表列数据
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        public JsonResult GetColumnData(string ruleCode, string matrixCode)
        {
            return ExecuteFunctionRun(() => {
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);

                List<BizRuleDecisionMatrixItemViewModel> dataList = new List<BizRuleDecisionMatrixItemViewModel>();
                if (DecisionMatrix.Columns != null)
                {
                    dataList = GetMatrixData(DecisionMatrix.Columns);
                }
                var gridData = CreateLigerUIGridData(dataList.ToArray());

                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取决策表行数据
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        public JsonResult GetRowData(string ruleCode, string matrixCode)
        {
            return ExecuteFunctionRun(() =>
            {
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);

                List<BizRuleDecisionMatrixItemViewModel> dataList = new List<BizRuleDecisionMatrixItemViewModel>();
                if (DecisionMatrix.Rows != null)
                {
                    dataList = GetMatrixData(DecisionMatrix.Rows);
                }
                var gridData = CreateLigerUIGridData(dataList.ToArray());

                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 删除决策表行或列
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <param name="hasRightToDelete"></param>
        /// <param name="deleteType"></param>
        /// <param name="hasRightToDelete"></param>
        /// <param name="parentIndexes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public JsonResult DeleteCell(string ruleCode, string matrixCode, bool hasRightToDelete,string deleteType,string parentIndexes,int index)
        {
            return ExecuteFunctionRun(() =>
            {
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);

                bool result = false;
                List<int> parentIndexs = GetPrentIndexes(parentIndexes);
                if (hasRightToDelete && !string.IsNullOrWhiteSpace(deleteType))
                {
                    if (deleteType == "Column")
                    {
                        DecisionMatrix.RemoveColumn(parentIndexs.ToArray<int>(), index);
                    }
                    else if (deleteType == "Row")
                    {
                        DecisionMatrix.RemoveRow(parentIndexs.ToArray(), index);
                    }
                    result = this.Engine.BizBus.UpdateBizRule(Rule);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        private List<int> GetPrentIndexes(string parentStrIndexes)
        {
            if (string.IsNullOrWhiteSpace(parentStrIndexes)) return new List<int>();
            string[] strIndexes = parentStrIndexes.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> indexList = new List<int>();
            foreach (string strI in strIndexes)
            {
                indexList.Add(int.Parse(strI));
            }
            return indexList;
        }


        private List<BizRuleDecisionMatrixItemViewModel> GetMatrixData(OThinker.H3.BizBus.BizRule.Header[] headers, string ParenStrIndexes = "")
        {
            List<BizRuleDecisionMatrixItemViewModel> dataList = new List<BizRuleDecisionMatrixItemViewModel>();
           
            for (int i = 0, j = headers.Length; i < j; i++)
            {
                OThinker.H3.BizBus.BizRule.Header header = headers[i];
               
                BizRuleDecisionMatrixItemViewModel model = new BizRuleDecisionMatrixItemViewModel();
                model.Index = i;
                model.DisplayName = header.DisplayName;
                model.EffectiveCondition = header.Value;
                model.Description = header.Description;
                model.SortKey = header.SortKey;
                model.ParentStrIndexs = ParenStrIndexes;

                if (header.Children != null && header.Children.Length > 0)
                {
                    List<BizRuleDecisionMatrixItemViewModel> listChildren = GetMatrixData(header.Children, ParenStrIndexes + "\\" + i);
                    if (model.children != null)
                    {
                        model.children.AddRange(listChildren);
                    }
                    else
                    {
                        model.children = new List<BizRuleDecisionMatrixItemViewModel>();
                        model.children.AddRange(listChildren);
                    }
                    //dataList.AddRange(listChildren);
                }
                dataList.Add(model);
            }
            return dataList;
        }

        #endregion

        #region 加载下拉列表数据

        public class OptionItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
        
        /// <summary>
        /// 执行方式
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadExecutionType()
        {
            return ExecuteFunctionRun(() =>
            {
                string[] names = Enum.GetNames(typeof(ExecutionType));
                List<OptionItem> list = new List<OptionItem>();
                foreach (string name in names)
                {
                    list.Add(new OptionItem() { Text = name, Value = name });
                }
                return Json(list,JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 决策表类型
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDecisionMatrix()
        {
            return ExecuteFunctionRun(() =>
            {
               
                string[] names = Enum.GetNames(typeof(DecisionMatrixType));
                List<OptionItem> list = new List<OptionItem>();
                foreach (string name in names)
                {
                    list.Add(new OptionItem() { Text = name, Value = name });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 决策表作用域
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDecisionMatrixScope()
        {
            return ExecuteFunctionRun(() =>
            {

                string[] names = Enum.GetNames(typeof(DecisionMatrixScope));
                List<OptionItem> list = new List<OptionItem>();
                foreach (string name in names)
                {
                    list.Add(new OptionItem() { Text = GetDecisionMatrixScopeName(name), Value = name });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            });
        }

        private string GetDecisionMatrixScopeName(string name)
        {
            if (name.ToLower() == "public") { return "共有"; }
            if (name.ToLower() == "private") { return "私有"; }
            return name;
        }

        /// <summary>
        /// 决策表返回数据项
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadResultFields(string ruleCode,string matrixCode)
        {
            return ExecuteFunctionRun(() =>
            {
                List<OptionItem> list = new List<OptionItem>();
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(matrixCode);
                // 保存结果的字段
                BizRuleDataElement[] dataElements = Rule.DataElements;
                if (dataElements != null)
                {
                    bool isSelectiveOrSorted = false;
                    if (DecisionMatrix != null)
                    {
                        isSelectiveOrSorted = DecisionMatrix.MatrixType == DecisionMatrixType.SelectiveArray ||
                         DecisionMatrix.MatrixType == DecisionMatrixType.SortedArray;
                    }
                    foreach (BizRuleDataElement dataElement in dataElements)
                    {
                        // 选择和排序规则的返回值只允许选择参与者
                        if (isSelectiveOrSorted)
                        {
                            if (dataElement.LogicType == Data.DataLogicType.MultiParticipant ||
                                dataElement.LogicType == Data.DataLogicType.SingleParticipant)
                            {
                                list.Add(new OptionItem() { Text = dataElement.DisplayName2, Value = dataElement.ElementName });
                            }
                        }
                        else
                        {
                            if (dataElement.RealType == typeof(string) || dataElement.RealType == typeof(string[]))
                            {
                                list.Add(new OptionItem() { Text = dataElement.DisplayName2, Value = dataElement.ElementName });
                            }
                        }
                    }
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            });
        }

        
        #endregion
    }
}
