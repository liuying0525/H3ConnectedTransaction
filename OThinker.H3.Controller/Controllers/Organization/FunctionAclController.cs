using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{

    /// <summary>
    /// 功能权限编辑控制器
    /// </summary>
    [Authorize]
    public class FunctionAclController:ControllerBase
    {

         /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_Data_Code; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FunctionAclController() { }

        /// <summary>
        /// 获取组织，群名称
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public JsonResult GetUnitInfo(string unitID)
        {
            return ExecuteFunctionRun(() =>
            {
                var unit= new object();
                OThinker.Organization.Unit u = this.Engine.Organization.GetUnit(unitID);
                if (u != null) {
                    unit = new
                    {
                        UnitID = u.ObjectID,
                        UnitName = u.Name
                    };
                }

                return Json(unit, JsonRequestBehavior.AllowGet);
            });

            
        }

        /// <summary>
        /// 获取用户对应的权限列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFunctionData(string unitID)
        {
            //string UserID = this.UserValidator.UserID;
            List<Dictionary<string, object>> aclTable = this.Engine.FunctionAclManager.GetAclTable(unitID);
            List<Dictionary<string, object>> dataList = GetChilren(FunctionNode.AdminRootCode, aclTable);
            var jsonData = new { Rows = dataList, Total = dataList.Count };
            return Json(jsonData);
        }

        /// <summary>
        /// 保存权限设置
        /// </summary>
        /// <param name="addSelectedIds"></param>
        /// <param name="delSelectedIds"></param>
        /// <param name="unitID">绑定权限的组织单元，用户，角色ID</param>
        /// <returns></returns>
        public JsonResult SaveAcl(string addSelectedIds, string delSelectedIds,string unitID)
        {
            ActionResult result =new ActionResult(true,"msgGlobalString.SaveSucced");
            try
            {
                string[] addFunIds = addSelectedIds.Split(';');
                string[] delFunIds = delSelectedIds.Split(';');

                foreach (string id in addFunIds)
                {
                    if (string.IsNullOrWhiteSpace(id))
                        continue;
                    Acl.FunctionAcl acl = new OThinker.H3.Acl.FunctionAcl();
                    acl.CreatedBy = this.UserValidator.UserID;
                    acl.CreatedTime = System.DateTime.Now;
                    acl.FunctionCode = id;
                    acl.Run = true;
                    acl.UserID = unitID;
                    this.Engine.FunctionAclManager.Add(acl);
                }

                foreach (string id in delFunIds)
                {
                    if (string.IsNullOrWhiteSpace(id))
                        continue;
                    this.Engine.FunctionAclManager.Delete(id);
                }
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = "保存失败," + ex.Message;
            }

            //客户端实现
            //AddSelectIds.Value = ";";
            //DelSelectIds.Value = ";";

            return Json(result);
        }

        /// <summary>
        /// 递归获取当前树节点的子节点
        /// </summary>
        /// <param name="ParentCode"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> GetChilren(string ParentCode, List<Dictionary<string, object>> table)
        {
            List<Dictionary<string, object>> childNodes = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> row in table)
            {
                if (ParentCode == FunctionNode.Category_Apps_Code)
                {// 处理应用中心的显示
                    if (Convert.ToInt16(row[OThinker.H3.Acl.FunctionNode.PropertyName_NodeType]) == (int)FunctionNodeType.AppNavigation)
                    {
                        row.Add("children", GetChilren(row[OThinker.H3.Acl.FunctionNode.PropertyName_Code].ToString(), table));
                        childNodes.Add(row);
                    }
                }
                else if (Convert.ToString(row[OThinker.H3.Acl.FunctionNode.PropertyName_ParentCode]) == ParentCode)
                {
                    row.Add("children", GetChilren(row[OThinker.H3.Acl.FunctionNode.PropertyName_Code].ToString(), table));
                    childNodes.Add(row);
                }
                else if (ParentCode == FunctionNode.Category_ProcessModel_Code) //主数据
                {

                }
            }
            return childNodes.Count == 0 ? null : childNodes.OrderBy(p => p[OThinker.H3.Acl.FunctionNode.PropertyName_SortKey]).ToList();
        }

    }
}
