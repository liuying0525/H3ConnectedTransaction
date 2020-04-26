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
    public class BizRuleDecisionMatrixColumnController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        /// <summary>
        /// 保存决策表列信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save( BizRuleDecisionMatrixItemViewModel model)
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
                    result.Success=false;
                    result.Message = "BizRule.DeletedOrNotExists";
                    return Json(result);
                }

                BizRuleDecisionMatrix DecisionMatrix = Rule.GetDecisionMatrix(model.MatrixCode);
                if (DecisionMatrix == null)
                {
                    //业务对象模式不存在，或者已经被删除
                   result.Success=false;
                   result.Message = "BizRule.DeletedOrNotExists";
                   return Json(result);
                }

                if (!result.Success) return Json(result);
                if (string.IsNullOrEmpty(model.EffectiveCondition) && ! model.IsDefault)
                {
                   // this.ShowWarningMessage(this.PortalResource.GetString("EditBizRuleTableColumn_Mssg1"));
                    result.Success = false;
                    result.Message = "BizRule.SelectDefault";
                    return Json(result);
                }
                if (IsCreate)
                {
                    OThinker.H3.BizBus.BizRule.Header column = new H3.BizBus.BizRule.Header();
                    column.DisplayName = model.DisplayName;
                    column.Description = model.Description;
                    column.Value = model.EffectiveCondition;
                    column.IsDefault = model.IsDefault;
                    column.SortKey = model.SortKey;
                    DecisionMatrix.AddColumn(GetPrentIndexes(model.ParentStrIndexs).ToArray(), column);
                }
                else
                {
                   Header Column = null; // 当前列

                   Header ParentColumn = null;// 直接父级列
                   List<int> ParentColumnIndexes = GetPrentIndexes(model.ParentStrIndexs);

                   if (ParentColumnIndexes.Count > 0)
                   {//如果存在父节点，则逐步找到最后一个节点，也就是直接上级
                       foreach (int index in ParentColumnIndexes)
                       {
                           if (ParentColumn == null)
                               ParentColumn = DecisionMatrix.Columns[index];
                           else
                               ParentColumn = ParentColumn.Children[index];
                       }
                   }

                   if (model.Index > -1)
                   {
                       //如果存在当前列，则看有无直接上级
                       if (ParentColumn == null)
                           Column = DecisionMatrix.Columns[model.Index];
                       else
                           Column = ParentColumn.Children[model.Index];
                   }

                    Column.DisplayName = model.DisplayName;
                    Column.Value = model.EffectiveCondition;
                    Column.Description = model.Description;
                    Column.IsDefault = model.IsDefault;
                    Column.SortKey = model.SortKey;

                    //DecisionMatrix.Resort();
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
        /// 加载列信息
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <param name="columnIndex"></param>
        /// <param name="parentIndexes"></param>
        /// <returns></returns>
        public JsonResult Load(string ruleCode,string matrixCode,int columnIndex,string parentIndexes)
        {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult();
                BizRuleTable Rule = Engine.BizBus.GetBizRule(ruleCode);
                BizRuleDecisionMatrix DecisionMatrix = null;
                if (Rule != null)
                {
                   DecisionMatrix= Rule.GetDecisionMatrix(matrixCode);
                }

                Header Column = null; // 当前列
                Header ParentColumn = null;// 直接父级列
                List<int> ParentColumnIndexes = GetPrentIndexes(parentIndexes);

                if (ParentColumnIndexes.Count > 0)
                {//如果存在父节点，则逐步找到最后一个节点，也就是直接上级
                    foreach (int index in ParentColumnIndexes)
                    {
                        if (ParentColumn == null)
                            ParentColumn = DecisionMatrix.Columns[index];
                        else
                            ParentColumn = ParentColumn.Children[index];
                    }
                }

                //如果存在当前列，则看有无直接上级
                if (ParentColumn == null)
                    Column = DecisionMatrix.Columns[columnIndex];
                else
                    Column = ParentColumn.Children[columnIndex];

                BizRuleDecisionMatrixItemViewModel model = new BizRuleDecisionMatrixItemViewModel();
                model.Index = columnIndex;
                if (Column != null)
                {
                    model.ObjectID = Column.GetHashCode().ToString();//做判断是否为新增使用，不为空表示为更新
                    model.DisplayName = Column.DisplayName;
                    model.Description = Column.Description;
                    model.EffectiveCondition = Column.Value;
                    model.IsDefault = Column.IsDefault;
                    model.SortKey = Column.SortKey;
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
        private ActionResult ValidateData(BizRuleDecisionMatrixItemViewModel model )
        {
            ActionResult result =new ActionResult(true,"");
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
        private ActionResult ValidateAcl(string ruleCode,string matrixCode)
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
