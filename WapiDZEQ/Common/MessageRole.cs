using OThinker.H3;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace WapiDZEQ.Common
{
    public class MessageRole : OThinker.H3.Controllers.MvcPage
    {
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set
            {
                _Engine = value;
            }
        }

        /// <summary>
        /// 根据角色编码获取角色id
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns></returns>
        public List<RoleUserViewModel> GetRoleUser(string roleCode)
        {
            var sql1 = string.Format("select Objectid from Ot_Orgpost where code='{0}'",roleCode);
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql1);
            string roleid = dt.Rows[0]["Objectid"] + string.Empty;
            //根据roleid获取对应的账号

            return GetRoleUserList(roleid);
        }
        /// <summary>
        /// 角色用户关系列表
        /// </summary>
        /// <returns></returns>
        public List<RoleUserViewModel> GetRoleUserList(string roleId)
        {
            //return ExecuteFunctionRun(() =>
            //{
                string userCode = "";
                string userName = "";
                OThinker.Organization.OrgPost post = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(roleId) as OThinker.Organization.OrgPost;

                List<RoleUserViewModel> lstRoleUsers = new List<RoleUserViewModel>();

                if (post != null && post.ChildList != null && post.ChildList.Length > 0)
                {
                    foreach (OThinker.Organization.OrgStaff staff in post.ChildList)
                    {
                        string userid = staff.UserID;
                        OThinker.Organization.User _user = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(userid) as OThinker.Organization.User;
                        if (_user == null) { continue; }
                        string _userCode = _user.Code;
                        string _userName = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(userid).Name;
                        string OrgName = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetName(_user.ParentID);
                        _userName = _userName + "(" + OrgName + ")";

                        //范围组织名称
                        string[] scopeids = staff.OUScope;
                        Dictionary<string, string> dicNames = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetNames(scopeids);
                        string scopeNames = "";
                        foreach (string key in dicNames.Keys) { scopeNames += dicNames[key] + ";"; }

                        lstRoleUsers.Add(new RoleUserViewModel
                        {
                            ObjectID = staff.ObjectID,
                            RoleID = roleId,
                            UserID = staff.UserID,
                            UserCode = _userCode,
                            UserName = _userName,
                            Sortkey = staff.ParentIndex,//staff 没有对应属性
                            ManagerScope = scopeNames,
                            ManagerScopeIds = string.Join(";", staff.OUScope),
                            Description = staff.Description

                        });
                    }
                }

                //筛选
                if (userCode != null)
                {
                    lstRoleUsers = lstRoleUsers.Where(r => r.UserCode.ToLower().Contains(userCode.ToLower())).ToList();
                }

                if (userCode != null && lstRoleUsers != null)
                {
                    lstRoleUsers = lstRoleUsers.Where(r => r.UserName.Contains(userName)).ToList();
                }
                if (lstRoleUsers.Count > 0)
                {
                    lstRoleUsers = lstRoleUsers.OrderBy(r => r.Sortkey).ToList();
                }

                return lstRoleUsers;
            //});
        }

        public List<UserCode> GetParticipants(string ActivityCode, string InstanceID)
        {

           
           
                if (!string.IsNullOrEmpty(ActivityCode) && !string.IsNullOrEmpty(InstanceID))
                {
                    //流程
                    OThinker.H3.Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                    if (InstanceContext != null)
                    {
                        //流程模板
                        OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate Template = this.Engine.WorkflowManager.GetPublishedTemplate(
                            InstanceContext.WorkflowCode,
                            InstanceContext.WorkflowVersion);
                        if (Template != null)
                        {
                            //活动信息
                            ClientActivityBase Activity = Template.GetActivityByCode(ActivityCode) as ClientActivityBase;

                            OThinker.H3.Instance.InstanceData InstanceData = new OThinker.H3.Instance.InstanceData(this.Engine, InstanceID, null);

                            string[] ParticipantIDs = Activity.ParseParticipants(InstanceData, this.Engine.Organization);
                            OThinker.Organization.Unit[] ParticipantUsers = this.Engine.Organization.GetUnits(ParticipantIDs).ToArray();

                            if (ParticipantUsers != null)
                            {
                                List<UserCode> ParticipantUserNames = new List<UserCode>();
                                foreach (OThinker.Organization.Unit u in ParticipantUsers)
                                {
                                    if (u != null && u.UnitType == OThinker.Organization.UnitType.User)
                                    {
                                        OThinker.Organization.User user = (OThinker.Organization.User)u;
                                        ParticipantUserNames.Add(new UserCode
                                        {
                                            Name = user.Name,
                                            Code = user.Code,
                                            ObjectID = user.ObjectID
                                        });
                                        //ParticipantUserNames.Add(u.Name + "[" + u.Code + "]");
                                    }
                                }
                                return ParticipantUserNames;
                            }
                        }
                    }
                }

               return null;
           
        }

        public List<string> GetParticipants(string unitId, string jobCode, string categoryCode)
        {
            return Engine.Organization.GetUsersByJobCode(unitId, jobCode, categoryCode);
          
        }

    }


    public class UserCode
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ObjectID { get; set; }
    }
}