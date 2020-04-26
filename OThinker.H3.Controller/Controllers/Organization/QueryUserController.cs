using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Organization
{
    [Authorize]
    public class QueryUserController:ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Organization_QueryOrg_Code; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryUserController() { }


        public JsonResult GetUserList(string keyWordStr, OThinker.Organization.State state)
        {
            return ExecuteFunctionRun(() =>
            {
                List<object> userList = new List<object>();
                int total = 0;
                if (!string.IsNullOrEmpty(keyWordStr))
                {
                    List<OThinker.Organization.User> listUsers = this.Engine.Organization.QueryUser(keyWordStr, state,100);
                    bool isAdmin = this.UserValidator.ValidateOrgAdmin(this.UserValidator.UserID);

                    if (listUsers != null)
                    {
                        total = listUsers.Count;

                        int startIndex = 0;
                        int endIndex = total;
                        OThinker.Organization.User row;
                        for (int i = startIndex; i < endIndex; i++)
                        {
                            row = listUsers[i];
                            OThinker.Organization.PrivacyLevel PrivacyLevel = (OThinker.Organization.PrivacyLevel)Enum.Parse(typeof(OThinker.Organization.PrivacyLevel), Convert.ToString(row.PrivacyLevel));

                            if (!isAdmin)
                            {
                                //管理员和自己
                                if (PrivacyLevel == OThinker.Organization.PrivacyLevel.Confidential) continue;
                                //仅本部门的成员
                                if (PrivacyLevel == OThinker.Organization.PrivacyLevel.PublicToDept &&
                                         this.UserValidator.User.ParentID != row.ParentID) continue;

                                //仅本部门的成员和上级部门成功
                                if (PrivacyLevel == OThinker.Organization.PrivacyLevel.PublicToDepts)
                                {
                                    if (this.UserValidator.User.ParentID != row.ParentID &&
                                        this.UserValidator.User.ParentID != this.Engine.Organization.GetParent(row.ParentID)) continue;
                                }
                            }

                            userList.Add(new
                            {
                                ObjectID = row.ObjectID,
                                Code = row.Code,
                                Name = row.Name,
                                Email = row.Email,
                                Mobile = row.Mobile,
                                OfficePhone = row.OfficePhone,
                                Description = row.Description,
                                OrgName = this.Engine.Organization.GetUnit(row.ParentID).Name
                            });
                        }
                    }
                }
                var griddata = CreateLigerUIGridData(userList.ToArray());
                return Json(griddata);
            });
        }
    }
}
