using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BizRule
{
    public class BizRuleController:ControllerBase
    {
        public BizRuleController() { }
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        /// <summary>
        /// 获取当前注册授权类型
        /// </summary>
        public OThinker.H3.Configs.LicenseType LicenseType
        {
            get
            {
                return (OThinker.H3.Configs.LicenseType)UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType];
            }
        }
        /// <summary>
        /// 获取业务规则是否允许使用
        /// </summary>
        public bool BizRuleAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizRule] + string.Empty);
            }
        }

        /// <summary>
        /// 保存规则
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveBizRule(BizRuleViewModel model)
        {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult();
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = null;
                bool IsCreate = string.IsNullOrEmpty(model.ObjectID);
                if (!BizRuleAuthorized)
                {
                    result.Success = false;
                    result.Message="BizRule.NotAuthorized";
                    return Json(result);
                }
                if (IsCreate)
                {
                    if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Code.Trim()))
                    {
                        //ShowWarningMessage(PortalResource.GetString("EditBizRuleTable_InvalidCode"));
                        result.Success = false;
                        result.Message = "msgGlobalString.InvalidCode";
                        return Json(result);
                    }

                    //业务规则必须以字母开始，不让创建到数据库表字段时报错
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                    if (!regex.Match(model.Code).Success)
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg4";
                        return Json(result);
                    }

                    Rule = new OThinker.H3.BizBus.BizRule.BizRuleTable(model.Code.Trim());
                    Rule.DisplayName = model.DisplayName;
                    Rule.Description = model.Description;


                    if (Engine.BizBus.GetBizRule(Rule.Code) != null)
                    {
                        //ShowWarningMessage(PortalResource.GetString("EditBizRuleTable_CodeExisted"));
                        result.Success = false;
                        result.Message = "msgGlobalString.CodeExisted";
                        return Json(result);
                    }

                    if (Engine.BizBus.AddBizRule(Rule, model.ParentCode))
                    {
                        result.Success = true;
                        //ReLoadParentTreeNode(ParentID);
                    }
                }
                else
                {
                    Rule = this.Engine.BizBus.GetBizRule(model.Code);
                    Rule.DisplayName = model.DisplayName;
                    Rule.Description = model.Description;
                    result.Success = this.Engine.BizBus.UpdateBizRule(Rule);
                }

                if (result.Success)
                {
                    result.Message = "msgGlobalString.SaveSucced";
                    return Json(result);
                }
                else
                {
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result);
                }
            });
        }

        /// <summary>
        /// 删除规则 ，有引用时不能删除
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <returns></returns>
        public JsonResult DeleteBizRule(string ruleCode)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.H3.BizBus.BizService.BizService Service = this.Engine.BizBus.GetBizService(ruleCode);
                ActionResult result = new ActionResult();
                if (Service.UsedCount > 0)
                {
                    //this.ShowSuccessMessage(this.PortalResource.GetString("EditBizRuleTable_BeDeleted"));
                    result.Success = false;
                    result.Message = "msgGlobalString.NotDeleted";
                    return Json(result);
                }

                if (Service.Methods != null)
                {
                    foreach (BizServiceMethod m in Service.Methods)
                    {
                        if (m.UsedCount > 0)
                        {
                            //this.ShowSuccessMessage(this.PortalResource.GetString("EditBizRuleTable_BeDeleted"));
                            result.Success = false;
                            result.Message = "msgGlobalString.NotDeleted";
                            return Json(result);
                        }
                    }
                }

                if (!this.Engine.BizBus.RemoveBizRule(ruleCode))
                {
                    //删除失败
                    //this.ShowWarningMessage(this.PortalResource.GetString("EditBizRuleTable_RemoveFailed"));
                    result.Success = false;
                    result.Message = "msgGlobalString.DeleteSucced";
                    return Json(result);
                }
                result.Success = true;
                return Json(result);
            });
            //this.ReLoadParentTreeNode(this.ParentID);
            //this.CloseCurrentTabPage();
        }

        /// <summary>
        /// 获取业务规则信息
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <returns></returns>
        public JsonResult GetBizRule(string ruleCode)
        {
            return ExecuteFunctionRun(() => {

                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(ruleCode);

                BizRuleViewModel model = new BizRuleViewModel();
                if (Rule != null)
                {
                    model.ObjectID = "-";//随便赋值一个值，用于判断是否为新增
                    model.Code = Rule.Code;
                    model.DisplayName = Rule.DisplayName;
                    model.Description = Rule.Description;
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 检验规则
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <returns></returns>
        public JsonResult Validate(string ruleCode)
        {
            return ExecuteFunctionRun(() => {
                ValidationResult r = this.Engine.BizBus.ValidateBizRule(ruleCode);
                ActionResult result = new ActionResult();
                result.Success = r.Valid;
                if (result.Success)
                {
                    //检验成功
                    // this.ShowSuccessMessage(this.PortalResource.GetString("EditBizRuleTable_Msg1"));
                    result.Message = "msgGlobalString.ValidateSuccess";
                }
                else
                {
                    result.Message = "msgGlobalString.ValidateFailed ";
                    result.Extend = r.ToString();
                    // this.ShowWarningMessage(r.ToString().Replace(System.Environment.NewLine, "<BR>").Replace(";", "<BR>"));
                }

                return Json(result,JsonRequestBehavior.AllowGet);
            });
           
        }


        /// <summary>
        /// 获取词汇表信息
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <returns></returns>
        public JsonResult GetGlossary(string ruleCode)
        {
            return ExecuteFunctionRun(() => {

                var pageindex = Request["PageIndex"];

                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                List<BizRuleGlossaryViewModel> lists = new List<BizRuleGlossaryViewModel>();
                if (Rule.DataElements != null)
                {
                    foreach (OThinker.H3.BizBus.BizRule.BizRuleDataElement item in Rule.DataElements)
                    {
                        BizRuleGlossaryViewModel model = new BizRuleGlossaryViewModel();

                        model.ElementName = item.ElementName;
                        model.DisplayName = item.DisplayName;
                        model.LogicType = item.LogicType.ToString();
                   
                        model.ParamType = item.ParamType.ToString();

                        model.DefaultValue = item.DefaultValue == null ? "" : item.DefaultValue.ToString() ;
                        
                        lists.Add(model);
                    }
                }

                //返回符合liegerUIGrid格式的分页数据
                var griddata = CreateLigerUIGridData(lists.ToArray());

                return Json(griddata,JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 删除词汇信息 
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public JsonResult DeleteGlossary(string ruleCode,string elementNames)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                string[] elements = elementNames.Split(';');
                foreach (string elementName in elements)
                {
                    Rule.RemoveDataElement(elementName);
                }
                result.Success = this.Engine.BizBus.UpdateBizRule(Rule);

                if (!result.Success)
                {
                    result.Message = "msgGlobalString.DeleteFailed";
                }

                return Json(result);
            });
        }
    }


}
