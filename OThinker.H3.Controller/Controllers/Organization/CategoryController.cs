using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    /// <summary>
    /// 定义组织类型控制器
    /// </summary>
    [Authorize]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Category_Code; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CategoryController() { }

        /// <summary>
        /// 获取定义类型页面初始化数据,返回所有定义的组织类型
        /// </summary>
        /// <returns>返回所有定义的组织类型</returns>
        [HttpPost]
        public JsonResult GetCategoryList()
        {
            return ExecuteFunctionRun(() =>
            {
                List<OThinker.Organization.OrgCategory> listCategory = this.Engine.Organization.GetAllOrgCategories();
                if (listCategory == null)
                {
                    return Json(new { Rows = new List<OThinker.Organization.OrgCategory>(), Total = 0 });
                }
                //构造Ligui GridData对象
                var gridData = CreateLigerUIGridData(listCategory.ToArray());
                return Json(gridData, JsonRequestBehavior.DenyGet);
            });
        }

        /// <summary>
        /// 根据组织类型ID获取组织类型ViewModel
        /// </summary>
        /// <param name="id">组织类型ObjectID</param>
        /// <returns>组织类型ViewModel</returns>
        [HttpGet]
        public JsonResult GetCategoryDetailByID(string id)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.Organization.OrgCategory categorySource = this.Engine.Organization.GetCategoryByCode(id);
                if (categorySource == null)
                {
                    return Json(new CategoryViewModel(), JsonRequestBehavior.AllowGet);
                }

                //分类列表数据
                var category = new
                {
                    ObjectID = categorySource.ObjectID,
                    Code = categorySource.Code,
                    DisplayName = categorySource.DisplayName,
                    Description = categorySource.Description
                };
               
                return Json(category, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取绑定了指定组织类型的组织
        /// </summary>
        /// <param name="categoryCode">组织类型编码</param>
        /// <returns>返回绑定了指定组织类型的组织</returns>
        public JsonResult GetOrgListByCategoryCode(string categoryCode)
        {
            return ExecuteFunctionRun(() =>
            {
                List<OThinker.Organization.Unit> listUnit = this.Engine.Organization.GetCategoryReferences(categoryCode);
                OrgCategory category = this.Engine.Organization.GetCategoryByCode(categoryCode);
                if (category == null) { return null; }

                if (listUnit == null || listUnit.Count == 0)
                {
                     //没有数据时直接返回空对象
                    return Json(new Dictionary<string,string>());
                }

                List<string> ids = new List<string>();
                foreach (Unit unit in listUnit)
                {
                    ids.Add(unit.ObjectID);
                }
                Dictionary<string, string> fullNames = this.Engine.Organization.GetFullNames(ids.ToArray());

                List<object> objs = new List<object>();
                foreach (string key in fullNames.Keys)
                {
                    objs.Add(new { 
                        ObjectID =key,
                        OrgFullPath=fullNames[key],
                        Category=category.DisplayName
                    });

                }

                var gridData = CreateLigerUIGridData(objs.ToArray());

                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 保存或更新组织类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns>返回是否成功，失败返回错误信息</returns>
        [HttpPost]
        public JsonResult SaveCategory(CategoryViewModel model)
        {
            OThinker.Organization.OrgCategory data = null;
            bool result = false;
            string message = string.Empty;
            ActionResult actionResult = new ActionResult(true, "");
            return ExecuteFunctionRun(() =>
            {

                actionResult = VlidateCode(model.Code);//验证编码合法性
                if (!actionResult.Success) { return Json(actionResult, JsonRequestBehavior.AllowGet); }

                if (!string.IsNullOrEmpty(model.ObjectID))
                {
                    data = this.Engine.Organization.GetCategoryByCode(model.Code);
                    if (data != null)
                    {
                        data.DisplayName = model.DisplayName;
                        data.Description = model.Description;
                        result = this.Engine.Organization.UpdateOrgCategory(data);
                    }
                    else
                    {
                        message = "EditCategory.UpdatingCategoryIDNull";
                    }
                }
                else
                {
                    // 判断对象编码是否已经存在
                    if (this.Engine.Organization.GetCategoryByCode(model.Code) != null)
                    {
                        message = "msgGlobalString.CodeDuplicate";
                    }
                    else
                    {
                        data = new OThinker.Organization.OrgCategory()
                        {
                            Code = model.Code,
                            DisplayName = model.DisplayName,
                            Description = model.Description
                        };
                        result = this.Engine.Organization.AddOrgCategory(data);
                    }
                }

                actionResult = new ActionResult(result, result ? "msgGlobalString.SaveSucced" : message);
                return Json(actionResult, JsonRequestBehavior.DenyGet);
            });
        }

        /// <summary>
        /// 删除组织类型
        /// </summary>
        /// <param name="ids">组织类型ID组合</param>
        /// <returns>删除是否成功</returns>
        [HttpGet]
        public JsonResult DelCategory(string ids)
        {
            bool result = true;
            string message = string.Empty;

            return ExecuteFunctionRun(() =>
            {
                string[] idArr = ids.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string id in idArr)
                {
                    OrgCategory c = this.Engine.Organization.GetCategoryByObjectID(id);
                    if (this.Engine.Organization.GetCategoryReferences(c.Code).Count > 0)
                    {
                        result = false;
                        message += "组织类型：" + c.DisplayName +"已经绑定组织，请先解除绑定再删除;";
                        continue;
                    }

                    if (!this.Engine.Organization.RemoveOrgCategory(id))
                    {
                        result = false;
                        message += "EditCategory.DelFailed,EditCategory.ObjectID=" + id;
                    }
                }

                //这里更改主要是考虑作为服务方法，如果执行出现错误，那么服务端应该尽量将关键信息传递给前端知道
                ActionResult actionResult = new ActionResult(result,
                    result ? "msgGlobalString.SaveSucced" : "msgGlobalString.SaveFailed");

                actionResult.Extend = message;

                return Json(actionResult, JsonRequestBehavior.AllowGet);
            });
        }

        // End Class
    }
}
