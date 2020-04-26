using OThinker.H3.BizBus.BizRule;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace OThinker.H3.Controllers.Controllers.BizRule
{
    /// <summary>
    /// 决策表相关操作
    /// </summary>
   public class BizRuleDecisionMatrixRowController:ControllerBase
    {

        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        /// <summary>
        /// 保存决策表行信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(BizRuleDecisionMatrixItemViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                //校验数据
                result = ValidateData(model);

                bool IsCreate = string.IsNullOrEmpty(model.ObjectID);
                BizRuleTable Rule = Engine.BizBus.GetBizRule(model.RuleCode);
                if (Rule == null)
                {
                    //业务对象模式不存在，或者已经被删除
                    result.Success = false;
                    result.Message = "BizRule.DeletedOrNotExists";
                    return Json(result);
                }

                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(model.MatrixCode);
                if (DecisionMatrix == null)
                {
                    //业务对象模式不存在，或者已经被删除
                    result.Success = false;
                    result.Message = "BizRule.DeletedOrNotExists";
                    return Json(result);
                }

                if (!result.Success) return Json(result);
                if (string.IsNullOrEmpty(model.EffectiveCondition) && !model.IsDefault)
                {
                    // this.ShowWarningMessage(this.PortalResource.GetString("EditBizRuleTableColumn_Mssg1"));
                    result.Success = false;
                    result.Message = "BizRule.SelectDefault";
                    return Json(result);
                }
                if (IsCreate)
                {
                    OThinker.H3.BizBus.BizRule.Header row = new H3.BizBus.BizRule.Header();
                    row.DisplayName = model.DisplayName;
                    row.Description = model.Description;
                    row.Value = model.EffectiveCondition;
                    row.IsDefault = model.IsDefault;
                    row.SortKey = model.SortKey;
                    DecisionMatrix.AddRow(GetPrentIndexes(model.ParentStrIndexs).ToArray(), row);
                }
                else
                {
                    Header Row = null; // 当前列
                    Header ParentRow = null;// 直接父级列
                    List<int> ParentRowIndexes = GetPrentIndexes(model.ParentStrIndexs);

                    if (ParentRowIndexes.Count > 0)
                    {//如果存在父节点，则逐步找到最后一个节点，也就是直接上级
                        foreach (int index in ParentRowIndexes)
                        {
                            if (ParentRow == null)
                                ParentRow = DecisionMatrix.Rows[index];
                            else
                                ParentRow = ParentRow.Children[index];
                        }
                    }
                    //如果存在当前行，则看有无直接上级
                    if (ParentRow == null)
                        Row = DecisionMatrix.Rows[model.Index];
                    else
                        Row = ParentRow.Children[model.Index];

                    Row.DisplayName = model.DisplayName;
                    Row.Value = model.EffectiveCondition;
                    Row.Description = model.Description;
                    Row.IsDefault = model.IsDefault;
                    Row.SortKey = model.SortKey;
                }
                if (this.Engine.BizBus.UpdateBizRule(Rule))
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.SaveSucced";
                }
                else
                {
                    //this.ShowWarningMessage(this.PortalResource.GetString("EditBizRuleTableColumn_SaveFailed"));
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载行信息
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <param name="columnIndex"></param>
        /// <param name="parentIndexes"></param>
        /// <returns></returns>
        public JsonResult Load(string ruleCode, string matrixCode, int rowIndex, string parentIndexes)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult();
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = null;
                if (Rule != null)
                {
                   DecisionMatrix= Rule.GetDecisionMatrix(matrixCode);
                }

                Header Row = null; // 当前行

                Header ParentRow = null;// 直接父级行
                List<int> ParentRowIndexes = GetPrentIndexes(parentIndexes);

                if (ParentRowIndexes.Count > 0)
                {//如果存在父节点，则逐步找到最后一个节点，也就是直接上级
                    foreach (int index in ParentRowIndexes)
                    {
                        if (ParentRow == null)
                            ParentRow = DecisionMatrix.Rows[index];
                        else
                            ParentRow = ParentRow.Children[index];
                    }
                }

                //如果存在当前行，则看有无直接上级
                if (ParentRow == null)
                    Row = DecisionMatrix.Rows[rowIndex];
                else
                    Row = ParentRow.Children[rowIndex];

                BizRuleDecisionMatrixItemViewModel model = new BizRuleDecisionMatrixItemViewModel();
                model.Index = rowIndex;
                if (Row != null)
                {
                    model.ObjectID = Row.GetHashCode().ToString();//做判断是否为新增使用，不为空表示为更新
                    model.DisplayName = Row.DisplayName;
                    model.Description = Row.Description;
                    model.EffectiveCondition = Row.Value;
                    model.IsDefault = Row.IsDefault;
                    model.SortKey = Row.SortKey;
                    model.RuleCode = ruleCode;
                    model.MatrixCode = matrixCode;
                }
                return Json(model, JsonRequestBehavior.AllowGet);
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

        #region 校验函数
        private ActionResult ValidateData(BizRuleDecisionMatrixItemViewModel model)
        {
            ActionResult result = new ActionResult(true, "");
            if (string.IsNullOrWhiteSpace(model.DisplayName))
            {
                //this.ShowWarningMessage(this.PortalResource.GetString("EmptyName"));
                result.Success = true;
                result.Message = "BizRule.EmptyName";
            }

            return result;
        }
        #endregion

        #region 验权
        private ActionResult ValidateAcl(string ruleCode, string matrixCode)
        {
            ActionResult result = new ActionResult(true, "");
            if (!this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.Category_BizRule_Code))
            {
                if (!this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.Category_BizRule_Code) ||
                    !this.UserValidator.ValidateRuleView(ruleCode, matrixCode))
                {
                    //this.ShowErrorMessage(this.PortalResource.GetString("PermissionDenied"), true);
                    result.Success = false;
                    result.Message = "msgGlobalString.PermissionDenied";
                }
                if (!this.UserValidator.ValidateRuleAdmin(ruleCode, matrixCode))
                {
                    //this.btnSave.Enabled = false;
                }
            }
            return result;
        }
        #endregion
    }
}
