using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using OThinker.Data;
using System.Web;
using OThinker.H3.Apps;
using OThinker.Organization;
using OThinker.H3.Data;
using System.IO;
using OThinker.H3.Acl;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 用户权限验证器
    /// </summary>
    [System.Serializable]
    public class UserValidator
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Engine">流程引擎</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="TempImages">图片存储临时目录</param>
        /// <param name="PortalSettings">门户设置</param>
        public UserValidator(
            IEngine Engine,
            string UserID,
            string TempImages,
            Dictionary<string, object> PortalSettings)
        {
            this._Engine = Engine;
            this._UserID = UserID;
            OThinker.Organization.Unit unit = this.Organization.GetUnit(this._UserID);
            if (unit == null || unit.UnitType != OThinker.Organization.UnitType.User)
            {
                throw new ArgumentOutOfRangeException("UserID \"" + UserID + "\" is invalid.");
            }
            this._User = (OThinker.Organization.User)unit;
            this._WorkingCalendar = this.Engine.WorkingCalendarManager.GetOrgCalendar(this._UserID);
            this.TempImages = TempImages;

            Dictionary<string, object> dic1 = PortalSettings;
            if (dic1 != null)
            {
                Dictionary<string, object> dic2 = new Dictionary<string, object>();
                foreach (string key in dic1.Keys)
                {
                    dic2.Add(key, SheetUtility.GetGlobalString(dic1[key]));
                }
                this._PortalSettings = dic2;
            }
        }

        private string TempImages = string.Empty;

        private Dictionary<string, object> _PortalSettings;
        public Dictionary<string, object> PortalSettings
        {
            get
            {
                if (this._PortalSettings == null)
                {
                    Dictionary<string, object> dic1 = this.Engine.SettingManager.GetSheetSettings();

                    Dictionary<string, object> dic2 = new Dictionary<string, object>();
                    foreach (string key in dic1.Keys)
                    {
                        dic2.Add(key, SheetUtility.GetGlobalString(dic1[key]));
                    }

                    this._PortalSettings = dic2;
                }
                return this._PortalSettings;
            }
        }

        private string _ImagePath = string.Empty;
        /// <summary>
        /// 获取用户图片路径
        /// </summary>
        public string ImagePath
        {
            get
            {
                if (this._ImagePath == string.Empty)
                {
                    _ImagePath = GetUserImagePath(this.Engine, this.User, this.TempImages);
                }
                return _ImagePath;
            }
            set
            {
                this._ImagePath = value;
            }
        }

        /// <summary>
        /// 读取用户图片
        /// </summary>
        /// <param name="Engine">引擎实例对象</param>
        /// <param name="User">用户对象</param>
        /// <param name="TempImages">临时图片存储目录</param>
        /// <returns></returns>
        public static string GetUserImagePath(IEngine Engine, OThinker.Organization.User User, string TempImages)
        {
            // 用户不存在则返回空
            if (User == null) return string.Empty;
            // 以下是获取用户图片
            string fileName = Path.Combine("face//" + Engine.EngineConfig.Code + "//" + User.ParentID, User.ObjectID + ".jpg");
            string savePath = Path.Combine(TempImages, fileName);

            // 获取图片路径
            if (File.Exists(savePath))
            {
                FileInfo file = new FileInfo(savePath);
                if (file.LastWriteTime > User.ModifiedTime)
                {
                    return fileName;
                }
                else
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                    }
                }
            }

            // 生成图片
            Attachment userImage = null;
            try
            {
                userImage = Engine.BizObjectManager.GetAttachment(User.UnitID,
                        OThinker.Organization.User.TableName,
                        User.UnitID,
                        User.UnitID);

                if (userImage != null && userImage.Content != null && userImage.Content.Length > 0)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(userImage.Content);
                            bw.Close();
                            return fileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 这里如果直接输出异常，那么用户不能登录
                Engine.LogWriter.Write("加载用户图片出现异常,UserValidator:" + ex.ToString());
            }

            return string.Empty;
        }

        #region 用户的基本信息 -------------------
        private string _UserID;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get
            {
                return this._UserID;
            }
        }

        private OThinker.Organization.User _User;
        /// <summary>
        /// 用户对象
        /// </summary>
        public OThinker.Organization.User User
        {
            get
            {
                return this._User;
            }
            set
            {
                this._User = value;
            }
        }

        private string _CompanyId = string.Empty;
        /// <summary>
        /// 公司的ID
        /// </summary>
        public string CompanyId
        {
            get
            {
                if (this._CompanyId == string.Empty)
                {
                    this._CompanyId = this.Organization.RootUnit.ObjectID;
                }
                return this._CompanyId;
            }
        }

        private OThinker.H3.Calendar.WorkingCalendar _WorkingCalendar;
        /// <summary>
        /// 使用的工作日历
        /// </summary>
        public OThinker.H3.Calendar.WorkingCalendar WorkingCalendar
        {
            get
            {
                return this._WorkingCalendar;
            }
        }

        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserCode
        {
            get
            {
                return this.User.Code;
            }
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get
            {
                return this.User.Name;
            }
        }

        /// <summary>
        /// 用户的全名
        /// </summary>
        public string UserFullName
        {
            get
            {
                return this.User.FullName;
            }
        }

        private List<string> _RecursiveMemberOfs = null;
        /// <summary>
        /// 获取当前用户的上级组织(包含自己)
        /// </summary>
        public string[] RecursiveMemberOfs
        {
            get
            {
                if (this._RecursiveMemberOfs == null)
                {
                    //从小到大排序
                    this._RecursiveMemberOfs = this.Organization.GetParents(this.UserID,
                        OThinker.Organization.UnitType.Unspecified, true, OThinker.Organization.State.Active);

                    this._RecursiveMemberOfs.Insert(0, this.UserID);//插入到第一个位置，作为从小到大排序
                }
                return this._RecursiveMemberOfs.ToArray();
            }
        }

        /// <summary>
        /// 获得某个组织的隶属于
        /// </summary>
        /// <param name="UnitID">组织ID</param>
        /// <param name="UnitType">隶属于的对象的类型范围</param>
        /// <param name="Recursive">是否递归获得隶属于对象</param>
        /// <param name="IncludeSelf">是否包含自己</param>
        /// <returns>组织的隶属于</returns>
        public string[] GetMemberOfs(string UnitID, OThinker.Organization.UnitType UnitType, bool Recursive, bool IncludeSelf)
        {
            if (UnitID == null)
            {
                return null;
            }
            List<string> list = this.Organization.GetParents(UnitID, UnitType, Recursive, State.Active);
            List<string> listCopy = new List<string>();

            //添加所在组的上级组织
            if (list != null && list.Count > 0)
            {
                listCopy.AddRange(list);

                foreach (string pid in listCopy)
                {
                    OThinker.Organization.Unit u = this.Engine.Organization.GetUnit(pid);
                    if (u is Organization.Group)
                    {
                        List<string> listp = this.Organization.GetParents(pid, UnitType.OrganizationUnit, Recursive, State.Active);
                        if (listp != null && listp.Count > 0)
                        {
                            foreach (string p in listp)
                            {
                                if (!list.Contains(p))
                                {
                                    list.Add(p);
                                }
                            }
                        }
                    }
                }
            }

            if (IncludeSelf)
            {
                //插入到第一个位置，作为从小到大排序
                list.Insert(0, UnitID);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获得某个组织的隶属于
        /// </summary>
        /// <param name="UnitID">组织ID</param>
        /// <param name="UnitType">隶属于的对象的类型范围</param>
        /// <param name="Recursive">是否递归获得隶属于对象</param>
        /// <param name="IncludeSelf">是否包含自己</param>
        /// <returns>组织的隶属于</returns>
        public List<string> GetMemberOfList(string UnitID, OThinker.Organization.UnitType UnitType, bool Recursive, bool IncludeSelf)
        {
            string[] memberOfs = this.GetMemberOfs(UnitID, UnitType, Recursive, IncludeSelf);
            if (memberOfs == null)
            {
                return new List<string>();
            }
            else
            {
                return new List<string>(memberOfs);
            }
        }

        private string[] _MemberOfs;
        /// <summary>
        /// 自己的隶属于
        /// </summary>
        public string[] MemberOfs
        {
            get
            {
                if (this._MemberOfs == null)
                {
                    this._MemberOfs = this.Organization.GetParents(this.UserID,
                        UnitType.Unspecified,
                        false,
                        OThinker.Organization.State.Active).ToArray();
                }
                return this._MemberOfs;
            }
        }

        private string[] _Jobs = null;
        /// <summary>
        /// 获取用户所属的角色
        /// </summary>
        public string[] Jobs
        {
            get
            {
                if (this._Jobs == null)
                {
                    this._Jobs = this.Organization.GetParents(this.UserID, UnitType.Post, true, State.Active).ToArray();
                    if (this._Jobs == null)
                    {
                        this._Jobs = new string[0];
                    }
                }
                return this._Jobs;
            }
        }

        /// <summary>
        /// 获取邮件地址
        /// </summary>
        public string Email
        {
            get
            {
                return this.User.Email;
            }
        }

        /// <summary>
        /// 获取直接经理
        /// </summary>
        public string ManagerID
        {
            get
            {
                return this.User.ManagerID;
            }
        }

        /// <summary>
        /// 获取所属OU
        /// </summary>
        public string Department
        {
            get
            {
                return this.User.ParentID;
            }
        }

        private string _DepartmentName = null;
        /// <summary>
        /// 获取所属机构的名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                if (this._DepartmentName == null)
                {
                    this._DepartmentName = this.Organization.GetName(this.Department);
                }
                return this._DepartmentName;
            }
        }

        #endregion

        #region 服务

        private IEngine _Engine;
        /// <summary>
        /// Engine对象
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return AppUtility.Engine;
                }
                return this._Engine;
            }
        }

        /// <summary>
        /// 组织结构对象
        /// </summary>
        public OThinker.Organization.IOrganization Organization
        {
            get
            {
                return this.Engine.Organization;
            }
        }

        /// <summary>
        /// 系统权限管理器
        /// </summary>
        public ISystemAclManager SystemAclManager
        {
            get
            {
                return this.Engine.SystemAclManager;
            }
        }

        /// <summary>
        /// 系统权限管理器
        /// </summary>
        public ISystemOrgAclManager SystemOrgAclManager
        {
            get
            {
                return this.Engine.SystemOrgAclManager;
            }
        }

        /// <summary>
        /// 流程模板权限管理器
        /// </summary>
        public IWorkflowAclManager WorkflowAclManager
        {
            get
            {
                return this.Engine.WorkflowAclManager;
            }
        }

        /// <summary>
        /// 报表权限管理器
        /// </summary>
        public IFunctionAclManager FunctionAclManager
        {
            get
            {
                return this.Engine.FunctionAclManager;
            }
        }

        #endregion

        #region 获得本地时间

        /// <summary>
        /// 将服务器上的时间转换为本地时间
        /// </summary>
        /// <param name="ServerTime">服务器时间</param>
        /// <returns>本地时间</returns>
        public DateTime GetLocalTime(DateTime ServerTime)
        {
            if (this.WorkingCalendar == null)
            {
                return ServerTime;
            }
            else
            {
                return ServerTime.Add(this.WorkingCalendar.TimeZone.Subtract(this.WorkingCalendar.ServerTimeZone));
            }
        }

        /// <summary>
        /// 将系统级时间转换为用户的本地时间
        /// </summary>
        /// <param name="ServerTime">服务器系统时间</param>
        /// <param name="IsHighPrecision">是否高精度显示，如果是高精度显示，格式则是日期 + 时间，否则则只有日期</param>
        /// <returns>本地时间的字符串</returns>
        public string GetLocalTimeText(DateTime ServerTime, bool IsHighPrecision)
        {
            DateTime localTime = this.GetLocalTime(ServerTime);
            // 最大或者最小值时，返回空
            if (localTime == DateTime.MinValue
                || localTime == DateTime.MaxValue
                || localTime.Year == 1753
                || localTime.Year == 9999)
            {
                return "-";
            }

            if (IsHighPrecision)
            {
                return localTime.ToShortDateString() + " " + localTime.ToShortTimeString();
            }
            else
            {
                return localTime.ToShortDateString();
            }
        }

        /// <summary>
        /// 将系统级时间转换为用户的本地时间
        /// </summary>
        /// <param name="ServerTimeText">服务器系统时间</param>
        /// <param name="IsHighPrecision">是否高精度显示，如果是高精度显示，格式则是日期 + 时间，否则则只有日期</param>
        /// <returns>本地时间的字符串</returns>
        public string GetLocalTimeText(string ServerTimeText, bool IsHighPrecision)
        {
            return this.GetLocalTimeText(DateTime.Parse(ServerTimeText), IsHighPrecision);
        }

        #endregion

        #region 是否用于管理员权限

        /// <summary>
        /// 用于缓存验证，以免每次都去访问数据库
        /// </summary>
        private BoolMatchValue _ValidateAdministrator = BoolMatchValue.Unspecified;
        /// <summary>
        /// 验证当前用户是否具有管理员的权限
        /// </summary>
        /// <returns>如果用户具有权限则返回true，否则返回false</returns>
        public virtual bool ValidateAdministrator()
        {
            // 首先检查是否是第一次获取该信息
            if (this._ValidateAdministrator == BoolMatchValue.Unspecified)
            {
                // 检查当前用户是否管理员
                if (this.Organization.IsAdministrator(this.UserID))
                {
                    this._ValidateAdministrator = BoolMatchValue.True;
                }
                else
                {
                    if (this.SystemAclManager.Check(this.RecursiveMemberOfs, AclType.Admin))
                    {
                        // 检查该用户所在的组或者组织单元是否拥有该权限
                        this._ValidateAdministrator = BoolMatchValue.True;
                    }
                    else
                    {
                        this._ValidateAdministrator = BoolMatchValue.False;
                    }
                }
            }
            return this._ValidateAdministrator == BoolMatchValue.True;
        }

        #endregion

        // ERROR, 下面的权限接口需要简化，等开发平台做完之后，再核对一遍
        #region 组织结构管理权限

        private Dictionary<string, List<SystemOrgAcl>> _SystemOrgAcTable = null;
        /// <summary>
        /// (Acl.OrgScope, Acl)记录集合。如果有缓存的话，则从缓存里面读取，否则的话，从服务器上读取。
        /// </summary>
        private Dictionary<string, List<SystemOrgAcl>> SystemOrgAclTable
        {
            get
            {
                if (this._SystemOrgAcTable == null)
                {
                    SystemOrgAcl[] acls = this.SystemOrgAclManager.GetUserAcls(this.RecursiveMemberOfs);
                    this._SystemOrgAcTable = new Dictionary<string, List<SystemOrgAcl>>();
                    if (acls != null)
                    {
                        foreach (SystemOrgAcl acl in acls)
                        {
                            List<SystemOrgAcl> list = null;
                            if (this._SystemOrgAcTable.ContainsKey(acl.OrgScope))
                            {
                                list = this._SystemOrgAcTable[acl.OrgScope];
                            }
                            else
                            {
                                list = new List<SystemOrgAcl>();
                                this._SystemOrgAcTable.Add(acl.OrgScope, list);
                            }
                            list.Add(acl);
                        }
                    }
                }
                return this._SystemOrgAcTable;
            }
        }

        /// <summary>
        /// 验证是否有组织权限
        /// </summary>
        /// <param name="UnitID">将ID做转换，如果是Null，那么转换成CompanyID</param>
        /// <param name="Type">权限类型</param>
        /// <returns>如果用户具有权限则返回true，否则返回false</returns>
        private bool ValidateSystemOrgAcl(string UnitID, AclType Type)
        {
            // 将ID做转换，如果是Null，那么转换成CompanyID
            string id = UnitID;
            if (id == null)
            {
                id = this.CompanyId;
            }

            if (this.ValidateAdministrator())
            {
                return true;
            }
            else
            {
                //一次性获取所有需要校验权限组织的所有隶属组织，主要其中一个有权限，就有权限，无法实现拒绝权限
                // 获得所有的隶属于，包括自己
                List<string> memberOfList = this.GetMemberOfList(id, OThinker.Organization.UnitType.Unspecified, true, true);
                foreach (string memberOf in memberOfList)
                {
                    if (this.SystemOrgAclTable.ContainsKey(memberOf))
                    {
                        List<SystemOrgAcl> acls = this.SystemOrgAclTable[memberOf];
                        foreach (SystemOrgAcl acl in acls)
                        {
                            if (acl.Administrator || acl.Check(Type))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 是否能够编辑组织结构
        /// </summary>
        /// <param name="UnitID">要编辑的组织结构的ID</param>
        /// <returns>如果用户具有权限则返回true，否则返回false</returns>
        public bool ValidateOrgEdit(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.EditOrg);
        }

        /// <summary>
        /// 是否能够查看组织机构数据
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public bool ValidateOrgView(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.View);
        }

        /// <summary>
        /// 是否能够管理组织机构的数据:包含BO、WF
        /// </summary>
        /// <param name="UnitID">要管理组织机构的ID</param>
        /// <returns>如果用户具有权限则返回true，否则返回false</returns>
        public bool ValidateOrgAdmin(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.Admin);
        }

        /// <summary>
        /// 验证是否具有所有组织的管理权限
        /// </summary>
        /// <param name="UnitIds">如果为Null或者长度为0的,那么返回用户是否具有整个公司的管理权限</param>
        /// <returns>如果具备权限，则返回true，否则返回false</returns>
        public bool ValidateOrgsAdmin(string[] UnitIds)
        {
            if (UnitIds == null || UnitIds.Length == 0)
            {
                return this.ValidateOrgAdmin(this.CompanyId);
            }
            foreach (string unitId in UnitIds)
            {
                if (!this.ValidateOrgAdmin(unitId))
                {
                    return false;
                }
            }
            return true;
        }

        #region Obsolete接口
        /// <summary>
        /// 验证当前用户是否能够分析指定组织结构的流程数据的权限
        /// </summary>
        /// <param name="UnitId">组织结构范围</param>
        /// <returns>如果有权限则返回true；否则返回false</returns>
        [Obsolete("该接口失效，请调用 ValidateOrgAdmin 接口")]
        public bool ValidateOrgAnalyzeData(string UnitId)
        {
            return this.ValidateOrgAdmin(UnitId);
        }

        /// <summary>
        /// 验证当前用户是否能够分析指定组织结构的流程数据的权限
        /// </summary>
        /// <param name="UnitIds">如果为Null或者长度为0的,那么返回用户是否具有整个公司的管理权限</param>
        /// <returns>如果有权限则返回true；否则返回false</returns>
        [Obsolete("该接口失效，请调用 ValidateOrgsAdmin 接口")]
        public bool ValidateOrgsAnalyzeData(string[] UnitIds)
        {
            return this.ValidateOrgsAdmin(UnitIds);
        }

        /// <summary>
        /// 验证当前用户是否能够分析指定组织结构的流程性能的权限
        /// </summary>
        /// <param name="UnitId">组织结构范围</param>
        /// <returns>如果有权限则返回true；否则返回false</returns>
        [Obsolete("该接口失效，请调用 ValidateOrgAdmin 接口")]
        public bool ValidateOrgAnalyzePerformance(string UnitId)
        {
            return this.ValidateOrgAdmin(UnitId);
        }

        /// <summary>
        /// 验证当前用户是否能够分析指定组织结构的流程数据的权限
        /// </summary>
        /// <param name="UnitIds">如果为Null或者长度为0的,那么返回用户是否具有整个公司的管理权限</param>
        /// <returns>如果有权限则返回true；否则返回false</returns>
        [Obsolete("该接口失效，请调用 ValidateOrgsAdmin 接口")]
        public bool ValidateOrgsAnalyzePerformance(string[] UnitIds)
        {
            return this.ValidateOrgsAdmin(UnitIds);
        }

        /// <summary>
        /// 验证当前用户是否能够查看指定组织结构的流程的权限
        /// </summary>
        /// <param name="UnitId">组织结构范围</param>
        /// <returns>如果有权限则返回true；否则返回false</returns>
        [Obsolete("该接口失效，请调用 ValidateView 接口")]
        public bool ValidateOrgViewInstance(string UnitId)
        {
            return this.ValidateOrgView(UnitId);
        }
        #endregion

        /// <summary>
        ///  获取拥有指定权限的组织机构ID集合
        /// </summary>
        /// <param name="AclType"></param>
        /// <returns></returns>
        private string[] GetOwnOrgs(AclType AclType)
        {
            if (this.ValidateAdministrator())
            {
                // 是管理员，那么返回整个公司
                return new string[] { this.CompanyId };
            }
            List<string> orgs = new List<string>();
            foreach (string OrgScope in this.SystemOrgAclTable.Keys)
            {
                List<SystemOrgAcl> acls = this.SystemOrgAclTable[OrgScope];
                foreach (SystemOrgAcl acl in acls)
                {
                    if (acl.Administrator || acl.Check(AclType))
                    {
                        orgs.Add(OrgScope);
                    }
                }
            }

            //检查是否有包含整个公司
            if (orgs.Contains(this.CompanyId))
            {
                orgs.Clear();
                orgs.Add(this.CompanyId);
            }
            return orgs.ToArray();
        }

        private string[] _AdminOrgs = null;
        /// <summary>
        /// 获得我能管理数据的组织
        /// </summary>
        public string[] AdminOrgs
        {
            get
            {
                if (_AdminOrgs == null)
                {
                    _AdminOrgs = this.GetOwnOrgs(AclType.Admin);
                }

                return _AdminOrgs;
            }
        }

        private string[] _ViewableOrgs = null;
        /// <summary>
        /// 获得我能查看数据的组织，包括用户可查看的和可管理的两部分
        /// </summary>
        public string[] ViewableOrgs
        {
            get
            {
                if (_ViewableOrgs == null)
                {
                    string[] viewableOrgs = this.GetOwnOrgs(AclType.View);
                    string[] adminOrgs = this.AdminOrgs;
                    string[] orgs = OThinker.Data.ArrayConvertor<string>.Add(viewableOrgs, adminOrgs);
                    _ViewableOrgs = OThinker.Data.ArrayConvertor<string>.RemoveDuplicate(orgs);
                }
                return _ViewableOrgs;
            }
        }

        private string[] _EditableOrgs = null;

        /// <summary>
        /// 获得我能编辑的组织，包括用户可编辑的和可管理的两部分
        /// </summary>
        public string[] EditableOrgs
        {
            get
            {
                if (_EditableOrgs == null)
                {
                    string[] viewableOrgs = this.GetOwnOrgs(AclType.EditOrg);
                    string[] adminOrgs = this.AdminOrgs;
                    string[] orgs = OThinker.Data.ArrayConvertor<string>.Add(viewableOrgs, adminOrgs);
                    _EditableOrgs = OThinker.Data.ArrayConvertor<string>.RemoveDuplicate(orgs);
                }
                return _EditableOrgs;
            }
        }
        #endregion

        #region 业务对象权限

        /// <summary>
        /// BO权限缓存：<SchemaCode+\+FolderId,BizObjectAcl[]>
        /// </summary>
        private Dictionary<string, BizObjectAcl[]> SchemaFolderBOAclTable = new Dictionary<string, BizObjectAcl[]>();

        /// <summary>
        /// 获取BO权限列表：如果缓存中有的话，从缓存中取，缓存中没有的话，从服务器上读取
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public BizObjectAcl[] GetBizObjectAcls(string SchemaCode, string FolderId)
        {
            if (string.IsNullOrEmpty(SchemaCode))
            {
                return null;
            }

            string key = (SchemaCode + "\\" + FolderId).ToLower();
            if (this.SchemaFolderBOAclTable.ContainsKey(key))
            {
                return this.SchemaFolderBOAclTable[key];
            }

            // 从服务器上获取
            BizObjectAcl[] acls = this.Engine.BizObjectManager.GetBizObjectUserAcls(SchemaCode, FolderId, this.RecursiveMemberOfs);
            this.SchemaFolderBOAclTable.Add(key, acls);

            return acls;
        }

        /// <summary>
        /// 根据组织范围类型，获取BO权限列表
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="FolderId"></param>
        /// <param name="BOOwners"></param>
        /// <param name="AclType"></param>
        /// <param name="ViewName"></param>
        /// <returns></returns>
        private bool ValidateBizObjectAcl(string SchemaCode, string FolderId, string[] BOOwners, AclType AclType, out string ViewName)
        {
            ViewName = null;

            //Error:这里会有问题，如果管理员的话，ViewName就取不到值了
            if (this.ValidateAdministrator()) return true;

            BizObjectAcl[] acls = this.GetBizObjectAcls(SchemaCode, FolderId);
            if (acls == null || acls.Length == 0)
            {
                return false;
            }
            foreach (BizObjectAcl acl in acls)
            {
                if (AclType == AclType.Add)
                {
                    //如果是新增的话，不需要校验组织唯独，直接校验是否有新增权限
                    if (acl.Check(AclType) || acl.Administrator)
                    {
                        ViewName = acl.ViewName;
                        return true;
                    }
                }
                //非新增权限的话，的校验组织维度
                else if (IsSuitableAcl(acl, BOOwners))
                {
                    if (acl.Check(AclType) || acl.Administrator)
                    {
                        ViewName = acl.ViewName;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 是否合适的组织
        /// </summary>
        /// <param name="Acl"></param>
        /// <param name="BOOwners"></param>
        /// <returns></returns>
        private bool IsSuitableAcl(BizObjectAcl Acl, string[] BOOwners)
        {
            switch (Acl.OrgScopeType)
            {
                case OrgScopeType.Self:
                    foreach (string id in BOOwners)
                    {
                        if (string.Compare(id, this._UserID, true) == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                case OrgScopeType.Specific:
                    foreach (string id in BOOwners)
                    {
                        if (string.Compare(id, Acl.OrgScope, true) == 0)
                        {
                            return true;
                        }
                    }
                    return false;
                case OrgScopeType.All:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 校验权限
        /// </summary>
        /// <param name="SchemaCode">数据模型编码</param>
        /// <param name="FolderId">文档库ID</param>
        /// <param name="OwnerId">拥有者</param>
        /// <param name="AclType">权限类型</param>
        /// <returns></returns>
        public bool ValidateBizObjectAcl(string SchemaCode, string FolderId, string OwnerId, AclType AclType)
        {
            string ViewName = string.Empty;
            if (OwnerId == this.User.UnitID && AclType == H3.Acl.AclType.View) return true;
            List<string> memberOfList = this.GetMemberOfList(OwnerId, OThinker.Organization.UnitType.Unspecified, true, true);

            return this.ValidateBizObjectAcl(SchemaCode, FolderId, memberOfList.ToArray(), AclType, out ViewName);
        }

        /// <summary>
        /// 校验是否有管理权限
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="FolderId"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        public bool ValidateBizObjectAdmin(string SchemaCode, string FolderId, string OwnerId)
        {
            return this.ValidateBizObjectAcl(SchemaCode, FolderId, OwnerId, AclType.Admin);
        }

        /// <summary>
        /// 校验是否有新增权限
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="FolderId"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        public bool ValidateBizObjectAdd(string SchemaCode, string FolderId, string OwnerId)
        {
            return this.ValidateBizObjectAcl(SchemaCode, FolderId, OwnerId, AclType.Add);
        }

        /// <summary>
        /// 校验是否有查看权限
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="FolderId"></param>
        /// <param name="OwnerId"></param>
        /// <returns></returns>
        public bool ValidateBizObjectView(string SchemaCode, string FolderId, string OwnerId)
        {
            return this.ValidateBizObjectAcl(SchemaCode, FolderId, OwnerId, AclType.View);
        }

        #endregion

        #region 流程权限

        #region 发起流程的权限

        /// <summary>
        /// 发起流程的数据缓存，<WorkflowFullName, bool>
        /// </summary>
        private Dictionary<string, bool> CreateInstanceValidationTable = new Dictionary<string, bool>();
        /// <summary>
        /// 是否拥有创建流程的权限
        /// </summary>
        /// <param name="WorkflowCode">流程模板编码</param>
        /// <returns>如果用户具有权限则返回true，否则返回false</returns>
        public bool ValidateCreateInstance(string WorkflowCode)
        {
            if (!this.CreateInstanceValidationTable.Keys.Contains(WorkflowCode))
            {

                bool validation = this.ValidateAdministrator()      // 是否是系统管理员权限
                                || this.WorkflowAclManager.Check(this.RecursiveMemberOfs, WorkflowCode, AclType.Add);// 所在的组是否拥有流程发起权限
                this.CreateInstanceValidationTable.Add(WorkflowCode, validation);
            }
            return this.CreateInstanceValidationTable[WorkflowCode];
        }

        #endregion

        // ERROR, 这部分代码没有看，因为将来会采用BizObjectSchema + Org来确定权限

        #region 公开的流程权限校验接口
        /// <summary>
        /// 校验流程实例的查看权限
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="Originator"></param>
        /// <returns></returns>
        public bool ValidateWFInsView(string WorkflowCode, string Originator)
        {
            return this.ValidateWFIns(WorkflowCode, Originator, AclType.View);
        }

        /// <summary>
        /// 校验流程实例的管理权限
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="Originator"></param>
        /// <returns></returns>
        public bool ValidateWFInsAdmin(string WorkflowCode, string Originator)
        {
            return this.ValidateWFIns(WorkflowCode, Originator, AclType.Admin);
        }

        #endregion

        private Dictionary<string, string> WorkflowSchemaCode = new Dictionary<string, string>();
        /// <summary>
        /// 流程权限校验私有方法
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <returns></returns>
        private string GetSchemaCodeByWorkflowCode(string WorkflowCode)
        {
            if (!WorkflowSchemaCode.ContainsKey(WorkflowCode))
            {
                OThinker.H3.WorkflowTemplate.WorkflowClause WorkflowClause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                if (WorkflowClause != null)
                {
                    WorkflowSchemaCode.Add(WorkflowCode, WorkflowClause.BizSchemaCode);
                }
                else
                {
                    WorkflowSchemaCode.Add(WorkflowCode, "");
                }
            }
            return WorkflowSchemaCode[WorkflowCode];
        }

        /// <summary>
        /// 带缓存的权限验证(流程权限)
        /// </summary>
        /// <param name="WorkflowCode">流程模板编码</param>
        /// <param name="Originator">流程实例的发起人，这里指的发起人的ID</param>
        /// <param name="AclType"></param>
        /// <returns></returns>
        public bool ValidateWFIns(string WorkflowCode, string Originator, AclType AclType)
        {
            // 检查参数
            if (Originator == null)
            {
                return false;
            }
            if (AclType == H3.Acl.AclType.View && this.User.UnitID == Originator) return true;
            //虚拟用户身份发起
            User OriginatorUser = this.Engine.Organization.GetUnit(this.User.UnitID) as User;
            List<User> virtualUsers = OriginatorUser.VirtualUsers;
            foreach (User user in virtualUsers)
            {
                if (AclType == H3.Acl.AclType.View && user.UnitID == Originator) return true;
            }
            if (this.ValidateAdministrator()) return true;
            string SchemaCode = this.GetSchemaCodeByWorkflowCode(WorkflowCode);
            if (string.IsNullOrWhiteSpace(SchemaCode)) return false;

            //校验组织权限
            bool result = this.ValidateSystemOrgAcl(Originator, AclType);
            if (!result)
            {
                //没有组织权限的时候，判断bo权限
                result = this.ValidateBizObjectAcl(SchemaCode, "", Originator, AclType);
            }
            return result;
        }

        ///// <summary>
        ///// 审计流程模板实例的权限
        ///// </summary>
        ///// <param name="WorkflowCode">流程模板编码</param>
        ///// <param name="Originators">发起人，如果为Null或者长度为0的,那么返回用户是否具有整个公司的分析权限</param>
        ///// <returns>只有对所有发起人都有分析权限才返回true，否则返回false</returns>
        //public bool ValidateWFAnalyzePerformance(string WorkflowCode, string[] Originators)
        //{
        //    if (Originators == null || Originators.Length == 0)
        //    {
        //        return this.ValidateWFInsAdmin(WorkflowCode, this.CompanyId);
        //    }
        //    foreach (string originator in Originators)
        //    {
        //        if (!this.ValidateWFInsAdmin(WorkflowCode, originator))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        #endregion

        #region 验证功能权限

        #region 功能权限Code

        private Dictionary<string, FunctionAcl> _FunctionAclTable = null;
        /// <summary>
        /// 用户功能权限缓存<UserID_功能Code, 相关的权限列表>
        /// </summary>
        private Dictionary<string, FunctionAcl> FunctionAclTable
        {
            get
            {
                if (this._FunctionAclTable == null)
                {
                    // 加载到内存中
                    FunctionAcl[] acls = this.FunctionAclManager.GetUserAcls(this.RecursiveMemberOfs);
                    this._FunctionAclTable = new Dictionary<string, FunctionAcl>();
                    if (acls != null)
                    {
                        foreach (FunctionAcl acl in acls)
                        {
                            this._FunctionAclTable.Add(acl.UserID + acl.FunctionCode, acl);
                        }
                    }
                }
                return this._FunctionAclTable;
            }
        }
        #endregion

        #region 功能校验接口

        /// <summary>
        /// 获得当前用户某个功能相关的所有权限记录，隶属组织里最近的权限记录
        /// </summary>
        /// <param name="UnitId"></param>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private FunctionAcl GetFunctionAcl(string UnitId, string FunctionCode)
        {
            if (string.IsNullOrWhiteSpace(FunctionCode) || string.IsNullOrWhiteSpace(UnitId))
            {
                return null;
            }
            //标示
            string key = UnitId + FunctionCode;
            if (!this.FunctionAclTable.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return this.FunctionAclTable[key];
            }
        }

        /// <summary>
        /// 获取当前用户隶属组织里，最近的一个模块权限配置
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <param name="WithinCache"></param>
        /// <returns></returns>
        private FunctionAcl GetLastFunctionAcl(string FunctionCode, bool WithinCache)
        {
            FunctionAcl acl = null;
            string[] members = this.RecursiveMemberOfs;
            foreach (string UserId in members)
            {
                //先从缓存中取
                acl = this.GetFunctionAcl(UserId, FunctionCode);
                if (acl != null)
                {
                    break;
                }
            }

            //不存在缓存中，且不从缓存读取
            if (acl == null && !WithinCache)
            {
                FunctionAcl[] Acls = this.Engine.FunctionAclManager.GetFunctionAclByCode(FunctionCode);
                if (Acls != null)
                {
                    foreach (FunctionAcl Acl in Acls)
                    {
                        if (members.Contains(Acl.UserID))
                        {
                            acl = Acl;
                            this._FunctionAclTable.Add(Acl.UserID + FunctionCode, Acl);
                            break;
                        }
                    }
                }
            }

            return acl;
        }

        private FunctionNodeType[] NeedCheckParentAclType = { FunctionNodeType.BizFolder, FunctionNodeType.BizObject, FunctionNodeType.BizWFFolder, FunctionNodeType.BizWorkflowPackage };

        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="FunctionCode">功能编辑，不允许为Null，否则返回False</param>
        /// <param name="AclType">权限类型</param>
        /// <returns>返回是否具有指定的权限</returns>
        private bool ValidateFunctionAcl(string FunctionCode, AclType AclType, bool WithinCache = true)
        {
            if (this.ValidateAdministrator())
            {
                return true;
            }

            // 检查报表ID
            if (string.IsNullOrWhiteSpace(FunctionCode))
            {
                return false;
            }

            // 获得最近的权限权限
            FunctionAcl acl = this.GetLastFunctionAcl(FunctionCode, WithinCache);
            // 检查是否存在权限记录
            if (acl == null)
            {
                //如果是设置过后台登陆，默认AdminRoot 有权限
                if (FunctionCode == FunctionNode.AdminRootCode && this.User.IsConsoleUser)
                {
                    return true;
                }

                //如果本身没有权限配置的话，指定的类型需要校验父级目录权限
                FunctionNode node;
                if (this.AllFunctionNodes == null)
                {
                    return false;
                }
                if (!this.AllFunctionNodes.Keys.Contains(FunctionCode))
                {
                    if (WithinCache)
                    {
                        return false;//从缓存中读取的话，读不到到就返回false
                    }
                    //不从缓存中读取，则需要从服务器上读一次
                    node = this.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
                    if (node == null)
                    {
                        return false;
                    }
                    this._AllFunctionNodes.Add(FunctionCode, node);
                }
                else
                {
                    node = this.AllFunctionNodes[FunctionCode];
                }
                if (string.IsNullOrWhiteSpace(node.ParentCode))
                {
                    return false;
                }
                if (!NeedCheckParentAclType.Contains(node.NodeType))
                {
                    return false;
                }
                //如果父级是流程模型菜单，就说明是顶级了
                if (node.ParentCode == FunctionNode.Category_ProcessModel_Code)
                {
                    return false;
                }
                //校验父级目录
                return this.ValidateFunctionAcl(node.ParentCode, AclType, WithinCache);
            }

            // 验证权限
            return acl.Check(AclType.Admin) || acl.Check(AclType);
        }

        /// <summary>
        /// 验证对于报表是否有运行权限
        /// </summary>
        /// <param name="FunctionCode">功能ID，不允许为Null，否则返回False</param>
        /// <returns>返回是否具有运行的权限</returns>
        public bool ValidateFunctionRun(string FunctionCode)
        {
            return this.ValidateFunctionAcl(FunctionCode, AclType.Run);
        }
        #endregion

        /// <summary>
        /// 所有功能<FunctionCode,FunctionNode>
        /// </summary>
        private Dictionary<string, FunctionNode> _AllFunctionNodes = null;

        private Dictionary<string, FunctionNode> AllFunctionNodes
        {
            get
            {
                if (this._AllFunctionNodes == null)
                {
                    // 加载所有功能列表
                    FunctionNode[] functions = this.Engine.FunctionAclManager.GetFunctionNodes();
                    if (functions != null)
                    {
                        this._AllFunctionNodes = new Dictionary<string, FunctionNode>();
                        foreach (FunctionNode f in functions)
                        {
                            if (!this._AllFunctionNodes.ContainsKey(f.Code))
                                this._AllFunctionNodes.Add(f.Code, f);
                        }
                    }
                }
                return this._AllFunctionNodes;
            }
        }

        /// <summary>
        /// 所有可以运行的功能
        /// </summary>
        private List<FunctionNode> _RunnableFunctions = null;

        /// <summary>
        /// 可运行功能的所有父级功能编码，如果需要继承父级功能权限是，才会记录：<FunctionCode,List<ParentCode>>
        /// </summary>
        private Dictionary<string, List<string>> _FunctionParentCodes = null;

        /// <summary>
        /// 所有可以运行的方法
        /// </summary>
        public FunctionNode[] RunnableFunctions
        {
            get
            {
                this.LoadRunnableFunctions();
                return this._RunnableFunctions.ToArray();
            }
        }

        /// <summary>
        /// 加载有权限的功能到缓存中
        /// </summary>
        private void LoadRunnableFunctions()
        {
            if (this._RunnableFunctions != null) return;
            this._RunnableFunctions = new List<FunctionNode>();
            this._FunctionParentCodes = new Dictionary<string, List<string>>();
            // 加载所有功能列表
            if (this.AllFunctionNodes == null || this.AllFunctionNodes.Values.Count == 0)
            {
                return;
            }
            FunctionNode[] functions = this.AllFunctionNodes.Values.ToArray();

            //需要校验的节点类型
            FunctionNodeType[] NodeTypes = {
                                               FunctionNodeType.Function,
                                               FunctionNodeType.Apps,
                                               FunctionNodeType.HybridApp,
                                               FunctionNodeType.AppNavigation,
                                               FunctionNodeType.AppMenu,
                                               FunctionNodeType.Organization,
                                               FunctionNodeType.BizObject,
                                               FunctionNodeType.ServiceFolder,
                                               FunctionNodeType.RuleFolder,
                                               FunctionNodeType.BPAReportSource,
                                               FunctionNodeType.ReportTemplateFolder,
                                               //FunctionNodeType.BizWFFolder,//流程目录这个是后台设计权限，修改得及时更新目录，如果放在缓存里不大合适
                                               FunctionNodeType.ProcessModel,
                                                FunctionNodeType.Report, //qiancheng
                                                FunctionNodeType.ReportFolder,
                                                FunctionNodeType.ReportFolderPage
                                           };

            //循环校验权限
            foreach (FunctionNode function in functions)
            {
                if (!NodeTypes.Contains(function.NodeType)) continue;
                //有权限，记录下来
                if (this.ValidateFunctionRun(function.Code))
                {
                    this._RunnableFunctions.Add(function);
                }
            }
            // 排序
            this._RunnableFunctions.Sort();
        }

        /// <summary>
        /// 获得子菜单功能,缓存中查找
        /// </summary>
        /// <param name="ParentCode"></param>
        /// <returns></returns>
        public FunctionNode[] GetFunctionsByParentCode(string ParentCode)
        {
            this.LoadRunnableFunctions();
            List<FunctionNode> childFunctoinNodes = this._RunnableFunctions.FindAll(f => f.ParentCode == ParentCode
                                                    || (string.IsNullOrWhiteSpace(ParentCode) && string.IsNullOrWhiteSpace(f.ParentCode)));
            if (childFunctoinNodes != null)
            {
                return childFunctoinNodes.ToArray();
            }
            return new FunctionNode[0] { };
        }

        /// <summary>
        /// 根据功能代码获得功能定义,如果没权限返回null，缓存中查找
        /// </summary>
        /// <param name="FunctionCode">功能代码</param>
        /// <returns>功能定义</returns>
        public FunctionNode GetFunctionNode(string FunctionCode)
        {
            if (this.RunnableFunctions != null)
            {
                foreach (FunctionNode fun in this.RunnableFunctions)
                {
                    if (fun.Code == FunctionCode)
                    {
                        return fun;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 实时获取子菜单功能
        /// </summary>
        /// <param name="ParentCode">父节点编码</param>
        /// <param name="WithinCache">是否使用缓存</param>
        /// <returns></returns>
        public FunctionNode[] GetFunctionsByParentCode(string ParentCode, bool WithinCache)
        {
            if (WithinCache) return this.GetFunctionsByParentCode(ParentCode);
            //不使用缓存
            List<FunctionNode> FunctionList = new List<FunctionNode>();
            FunctionNode[] FunctionNodes = this.FunctionAclManager.GetFunctionNodesByParentCode(ParentCode);
            if (FunctionNodes == null) return null;

            foreach (FunctionNode Node in FunctionNodes)
            {
                if (this.ValidateFunctionAcl(Node.Code, AclType.Run, WithinCache))
                {
                    FunctionList.Add(Node);
                }
            }

            return FunctionList.Count == 0 ? null : FunctionList.ToArray();
        }

        /// <summary>
        /// 实时根据代码获取功能定义，如果没有权限返回null
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        public FunctionNode GetFunctionNode(string FunctionCode, bool WithinCache)
        {
            if (WithinCache) return this.GetFunctionNode(FunctionCode);
            //不使用缓存
            FunctionNode Node = this.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            if (Node == null) return null;
            if (this.ValidateFunctionRun(Node.Code)) return Node;
            return null;
        }

        #endregion


        private List<User> lstUsers;
        /// <summary>
        /// 获取关联的用户集合
        /// </summary>
        public List<User> RelationUsers
        {
            get
            {
                if (lstUsers != null)
                {
                    return lstUsers;
                }
                else
                {
                    lstUsers = new List<User>();
                    // 获取所有被关联的用户集合
                    List<Unit> lstUnit = this.Engine.Organization.GetAllUnits(UnitType.User).Where(x => ((User)x).RelationUserID == this.User.ObjectID).ToList();
                    if (lstUnit != null && lstUnit.Count > 0)
                    {
                        foreach (Unit u in lstUnit)
                        {
                            lstUsers.Add((User)u);
                        }
                    }
                }
                return lstUsers;
            }
        }

        #region 业务规则的权限

        private BizRuleAcl[] _BizRuleAcls = null;
        private Dictionary<string, List<BizRuleAcl>> BizRuleAclTable = new Dictionary<string, List<BizRuleAcl>>();
        /// <summary>
        /// 当前用户拥有的所有规则权限
        /// </summary>
        public BizRuleAcl[] BizRuleAcls
        {
            get
            {
                if (this._BizRuleAcls == null)
                {
                    this._BizRuleAcls = this.Engine.BizRuleAclManager.GetUserAcls(this.RecursiveMemberOfs);

                    if (this._BizRuleAcls == null)
                    {
                        this._BizRuleAcls = new BizRuleAcl[0];
                    }

                    foreach (BizRuleAcl acl in this._BizRuleAcls)
                    {
                        string objectKey = acl.ObjectKey.ToLower();
                        if (!this.BizRuleAclTable.ContainsKey(objectKey))
                        {
                            this.BizRuleAclTable.Add(objectKey, new List<BizRuleAcl>());
                        }
                        this.BizRuleAclTable[objectKey].Add(acl);
                    }
                }
                return this._BizRuleAcls;
            }
        }

        /// <summary>
        /// 是否存在对于某个业务规则的管理权限
        /// </summary>
        /// <param name="RuleCode">业务规则编码</param>
        /// <param name="DecisionMatrixCode">决策表编码</param>
        /// <returns>如果拥有管理权限，则返回true，否则返回false</returns>
        public bool ValidateRuleAdmin(string RuleCode, string DecisionMatrixCode)
        {
            return this._ValidateRuleAcl(RuleCode, DecisionMatrixCode, AclType.Admin);
        }

        /// <summary>
        /// 是否存在对于某个业务规则的查看权限
        /// </summary>
        /// <param name="RuleCode">业务规则编码</param>
        /// <param name="DecisionMatrixCode">决策表编码</param>
        /// <returns>如果拥有管理权限，则返回true，否则返回false</returns>
        public bool ValidateRuleView(string RuleCode, string DecisionMatrixCode)
        {
            return this._ValidateRuleAcl(RuleCode, DecisionMatrixCode, AclType.View);
        }

        /// <summary>
        /// 是否存在对于某个业务规则的权限
        /// </summary>
        /// <param name="RuleCode">业务规则编码</param>
        /// <param name="DecisionMatrixCode">决策表编码</param>
        /// <param name="Type">权限类型</param>
        /// <returns>如果拥有管理权限，则返回true，否则返回false</returns>
        private bool _ValidateRuleAcl(string RuleCode, string DecisionMatrixCode, AclType Type)
        {
            if (this.ValidateAdministrator())
            {
                return true;
            }
            else if (this.BizRuleAcls != null)
            {
                string objectKey = BizRuleAcl.GetObjectKey(RuleCode, DecisionMatrixCode).ToLower();
                if (!this.BizRuleAclTable.ContainsKey(objectKey))
                {
                    return false;
                }
                BizRuleAcl[] acls = this.BizRuleAclTable[objectKey].ToArray();

                foreach (BizRuleAcl acl in acls)
                {
                    if (acl.Check(AclType.Admin) || acl.Check(Type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
