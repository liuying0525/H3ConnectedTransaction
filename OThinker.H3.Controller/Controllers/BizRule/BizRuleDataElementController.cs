
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using OThinker.H3.Controllers.ViewModels;

namespace OThinker.H3.Controllers.Controllers.BizRule
{
    public class BizRuleDataElementController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        #region 参数类型
        /// <summary>
        /// 获取数据类型列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLogicTypeList()
        {
            return ExecuteFunctionRun(() =>
            {
                // 获得支持的数据类型
                OThinker.H3.Data.DataLogicType[] types = OThinker.H3.Data.DataLogicTypeConvertor.GetBizRuleSupportLogicTypes();
                List<object> lists = new List<object>();
                foreach (OThinker.H3.Data.DataLogicType type in types)
                {
                    string typeName = OThinker.H3.Data.DataLogicTypeConvertor.ToLogicTypeName(type);
                    lists.Add(new { Text = typeName, Value = type.ToString() });
                }
                return Json(lists, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取参数类型列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetParamTypeList()
        {
            return ExecuteFunctionRun(() =>
            {
                List<object> lists = new List<object>();
                //// 参数类型
                string[] paramTypes = Enum.GetNames(typeof(H3.BizBus.BizRule.InOutType));
                foreach (string paramType in paramTypes)
                {
                    lists.Add(new { Text = paramType, Value = paramType });
                }
                return Json(lists, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion


        /// <summary>
        /// 保存业务规则词汇
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Save(BizRuleGlossaryViewModel model)
        {
            Data.DataLogicType logicType = (Data.DataLogicType)Enum.Parse(typeof(Data.DataLogicType), model.LogicType);
            string defaultValue = string.IsNullOrEmpty(model.DefaultValue)?"":model.DefaultValue;
            bool IsCreate = string.IsNullOrEmpty(model.ObjectID);

            ActionResult result = ValidateData(logicType,model);
            if (!result.Success) { return Json(result); }
            OThinker.H3.BizBus.BizRule.BizRuleDataElement Element;
            OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(model.RuleCode);
            

            if (IsCreate)
            {
                // 数据项必须以字母开始，不让创建到数据库表字段时报错
                // System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[A-Za-z][A-Za-z0-9_]*$");
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(model.ElementName).Success)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg4";
                    return Json(result);
                }

                //检测是否重名
                if (Rule.GetDataElement(model.ElementName) != null)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.CodeDuplicate";
                    return Json(result);
                }

                Element = new OThinker.H3.BizBus.BizRule.BizRuleDataElement();
                Element.ElementName = model.ElementName;
                Element.DisplayName = model.DisplayName;
                Element.Description = model.Description;
                Element.LogicType = logicType;
                Element.ParamType = (H3.BizBus.BizRule.InOutType)Enum.Parse(typeof(H3.BizBus.BizRule.InOutType), model.ParamType);
                Element.DefaultValue = defaultValue;

                if (!Rule.AddDataElement(Element))
                {
 
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result);
                }
            }
            else
            {
                Element = Rule.GetDataElement(model.ElementName);
                Element.DisplayName = model.DisplayName;
                Element.Description = model.Description;
                Element.LogicType = logicType;
                Element.ParamType = (H3.BizBus.BizRule.InOutType)Enum.Parse(typeof(H3.BizBus.BizRule.InOutType), model.ParamType);
                Element.DefaultValue = defaultValue;
            }

            if (!Engine.BizBus.UpdateBizRule(Rule))
            {
                //ShowWarningMessage(PortalResource.GetString("EditBizRuleTableDataElement_SaveFailed"));
                result.Success = false;
                result.Message = "msgGlobalString.SaveFailed";
                return Json(result);
            }

            result.Success = true;
            result.Message = "msgGlobalString.SaveSucced";
            return Json(result);
        }


        /// <summary>
        /// 移除业务规则词汇
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public JsonResult RemoveElement(string ruleCode,string elementName)
        {
            return ExecuteFunctionRun(() => {

                ActionResult result = new ActionResult(true, "");
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                Rule.RemoveDataElement(elementName);
                result.Success = this.Engine.BizBus.UpdateBizRule(Rule);
                if (!result.Success)
                {
                    //删除失败
                    result.Message = "msgGlobalString.DeleteFailed";
                    //this.ShowWarningMessage(this.PortalResource.GetString("EditBizRuleTableDataElement_RemoveFailed"));
                }
                else
                {
                    result.Message = "msgGlobalString.DeleteSucced";
                    //this.ReLoadCurrentTabPage();
                }

                return Json(result);
            });
        }

        /// <summary>
        /// 获取业务规则词汇
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public JsonResult GetRuleElement(string ruleCode, string elementName)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                OThinker.H3.BizBus.BizRule.BizRuleDataElement Element;
                BizRuleGlossaryViewModel rElement = new BizRuleGlossaryViewModel();
                if (Rule != null)
                {
                    Element = Rule.GetDataElement(elementName);
                    
                    rElement.ElementName = Element.ElementName;
                    rElement.RuleCode = ruleCode;
                    rElement.DisplayName = Element.DisplayName;
                    rElement.Description = Element.Description;
                    rElement.LogicType = Element.LogicType.ToString();
                    rElement.ParamType = Element.ParamType.ToString();
                    rElement.DefaultValue = Element.DefaultValue==null?"":Element.DefaultValue.ToString();
                    rElement.ObjectID = Element.GetHashCode().ToString();//判定是否为新增使用，不为空即可
                
                }
               
                return Json(rElement,JsonRequestBehavior.AllowGet);
                    
            });
        }

        #region 校验数据
        private ActionResult ValidateData(Data.DataLogicType logicType, BizRuleGlossaryViewModel model)
        {
            System.Type realType = OThinker.H3.Data.DataLogicTypeConvertor.ToRealType(logicType);
            ActionResult result = new ActionResult(true,"");
            if (string.IsNullOrEmpty(model.ElementName) || string.IsNullOrEmpty(model.ElementName))
            {
                result.Success = false;
                result.Message = "BizRule.InvalidName";
               
                return result;
            }

            // 验证默认值是否合法
            if (!string.IsNullOrEmpty(model.DefaultValue))
            {
                object DefaultValue = model.DefaultValue;
                if (realType.FullName == typeof(string[]).FullName) { 
                    DefaultValue = model.DefaultValue.Split(';');
                    result.Message = DefaultValue.ToString();
                    return result;
                }

                if (!OThinker.Data.Convertor.Check(realType, model.DefaultValue))
                {
                    result.Success = false;
                    result.Message = "BizRule.InvalidDefaultValue";
                    return result;
                }
            }
            return result;
        }
        #endregion
    }
}
