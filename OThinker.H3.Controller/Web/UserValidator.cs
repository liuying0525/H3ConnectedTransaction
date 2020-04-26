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
    /// �û�Ȩ����֤��
    /// </summary>
    [System.Serializable]
    public class UserValidator
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="Engine">��������</param>
        /// <param name="UserID">�û�ID</param>
        /// <param name="TempImages">ͼƬ�洢��ʱĿ¼</param>
        /// <param name="PortalSettings">�Ż�����</param>
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
        /// ��ȡ�û�ͼƬ·��
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
        /// ��ȡ�û�ͼƬ
        /// </summary>
        /// <param name="Engine">����ʵ������</param>
        /// <param name="User">�û�����</param>
        /// <param name="TempImages">��ʱͼƬ�洢Ŀ¼</param>
        /// <returns></returns>
        public static string GetUserImagePath(IEngine Engine, OThinker.Organization.User User, string TempImages)
        {
            // �û��������򷵻ؿ�
            if (User == null) return string.Empty;
            // �����ǻ�ȡ�û�ͼƬ
            string fileName = Path.Combine("face//" + Engine.EngineConfig.Code + "//" + User.ParentID, User.ObjectID + ".jpg");
            string savePath = Path.Combine(TempImages, fileName);

            // ��ȡͼƬ·��
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

            // ����ͼƬ
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
                // �������ֱ������쳣����ô�û����ܵ�¼
                Engine.LogWriter.Write("�����û�ͼƬ�����쳣,UserValidator:" + ex.ToString());
            }

            return string.Empty;
        }

        #region �û��Ļ�����Ϣ -------------------
        private string _UserID;
        /// <summary>
        /// �û�ID
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
        /// �û�����
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
        /// ��˾��ID
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
        /// ʹ�õĹ�������
        /// </summary>
        public OThinker.H3.Calendar.WorkingCalendar WorkingCalendar
        {
            get
            {
                return this._WorkingCalendar;
            }
        }

        /// <summary>
        /// �û���¼��
        /// </summary>
        public string UserCode
        {
            get
            {
                return this.User.Code;
            }
        }

        /// <summary>
        /// �û�����
        /// </summary>
        public string UserName
        {
            get
            {
                return this.User.Name;
            }
        }

        /// <summary>
        /// �û���ȫ��
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
        /// ��ȡ��ǰ�û����ϼ���֯(�����Լ�)
        /// </summary>
        public string[] RecursiveMemberOfs
        {
            get
            {
                if (this._RecursiveMemberOfs == null)
                {
                    //��С��������
                    this._RecursiveMemberOfs = this.Organization.GetParents(this.UserID,
                        OThinker.Organization.UnitType.Unspecified, true, OThinker.Organization.State.Active);

                    this._RecursiveMemberOfs.Insert(0, this.UserID);//���뵽��һ��λ�ã���Ϊ��С��������
                }
                return this._RecursiveMemberOfs.ToArray();
            }
        }

        /// <summary>
        /// ���ĳ����֯��������
        /// </summary>
        /// <param name="UnitID">��֯ID</param>
        /// <param name="UnitType">�����ڵĶ�������ͷ�Χ</param>
        /// <param name="Recursive">�Ƿ�ݹ��������ڶ���</param>
        /// <param name="IncludeSelf">�Ƿ�����Լ�</param>
        /// <returns>��֯��������</returns>
        public string[] GetMemberOfs(string UnitID, OThinker.Organization.UnitType UnitType, bool Recursive, bool IncludeSelf)
        {
            if (UnitID == null)
            {
                return null;
            }
            List<string> list = this.Organization.GetParents(UnitID, UnitType, Recursive, State.Active);
            List<string> listCopy = new List<string>();

            //�����������ϼ���֯
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
                //���뵽��һ��λ�ã���Ϊ��С��������
                list.Insert(0, UnitID);
            }
            return list.ToArray();
        }

        /// <summary>
        /// ���ĳ����֯��������
        /// </summary>
        /// <param name="UnitID">��֯ID</param>
        /// <param name="UnitType">�����ڵĶ�������ͷ�Χ</param>
        /// <param name="Recursive">�Ƿ�ݹ��������ڶ���</param>
        /// <param name="IncludeSelf">�Ƿ�����Լ�</param>
        /// <returns>��֯��������</returns>
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
        /// �Լ���������
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
        /// ��ȡ�û������Ľ�ɫ
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
        /// ��ȡ�ʼ���ַ
        /// </summary>
        public string Email
        {
            get
            {
                return this.User.Email;
            }
        }

        /// <summary>
        /// ��ȡֱ�Ӿ���
        /// </summary>
        public string ManagerID
        {
            get
            {
                return this.User.ManagerID;
            }
        }

        /// <summary>
        /// ��ȡ����OU
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
        /// ��ȡ��������������
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

        #region ����

        private IEngine _Engine;
        /// <summary>
        /// Engine����
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
        /// ��֯�ṹ����
        /// </summary>
        public OThinker.Organization.IOrganization Organization
        {
            get
            {
                return this.Engine.Organization;
            }
        }

        /// <summary>
        /// ϵͳȨ�޹�����
        /// </summary>
        public ISystemAclManager SystemAclManager
        {
            get
            {
                return this.Engine.SystemAclManager;
            }
        }

        /// <summary>
        /// ϵͳȨ�޹�����
        /// </summary>
        public ISystemOrgAclManager SystemOrgAclManager
        {
            get
            {
                return this.Engine.SystemOrgAclManager;
            }
        }

        /// <summary>
        /// ����ģ��Ȩ�޹�����
        /// </summary>
        public IWorkflowAclManager WorkflowAclManager
        {
            get
            {
                return this.Engine.WorkflowAclManager;
            }
        }

        /// <summary>
        /// ����Ȩ�޹�����
        /// </summary>
        public IFunctionAclManager FunctionAclManager
        {
            get
            {
                return this.Engine.FunctionAclManager;
            }
        }

        #endregion

        #region ��ñ���ʱ��

        /// <summary>
        /// ���������ϵ�ʱ��ת��Ϊ����ʱ��
        /// </summary>
        /// <param name="ServerTime">������ʱ��</param>
        /// <returns>����ʱ��</returns>
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
        /// ��ϵͳ��ʱ��ת��Ϊ�û��ı���ʱ��
        /// </summary>
        /// <param name="ServerTime">������ϵͳʱ��</param>
        /// <param name="IsHighPrecision">�Ƿ�߾�����ʾ������Ǹ߾�����ʾ����ʽ�������� + ʱ�䣬������ֻ������</param>
        /// <returns>����ʱ����ַ���</returns>
        public string GetLocalTimeText(DateTime ServerTime, bool IsHighPrecision)
        {
            DateTime localTime = this.GetLocalTime(ServerTime);
            // ��������Сֵʱ�����ؿ�
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
        /// ��ϵͳ��ʱ��ת��Ϊ�û��ı���ʱ��
        /// </summary>
        /// <param name="ServerTimeText">������ϵͳʱ��</param>
        /// <param name="IsHighPrecision">�Ƿ�߾�����ʾ������Ǹ߾�����ʾ����ʽ�������� + ʱ�䣬������ֻ������</param>
        /// <returns>����ʱ����ַ���</returns>
        public string GetLocalTimeText(string ServerTimeText, bool IsHighPrecision)
        {
            return this.GetLocalTimeText(DateTime.Parse(ServerTimeText), IsHighPrecision);
        }

        #endregion

        #region �Ƿ����ڹ���ԱȨ��

        /// <summary>
        /// ���ڻ�����֤������ÿ�ζ�ȥ�������ݿ�
        /// </summary>
        private BoolMatchValue _ValidateAdministrator = BoolMatchValue.Unspecified;
        /// <summary>
        /// ��֤��ǰ�û��Ƿ���й���Ա��Ȩ��
        /// </summary>
        /// <returns>����û�����Ȩ���򷵻�true�����򷵻�false</returns>
        public virtual bool ValidateAdministrator()
        {
            // ���ȼ���Ƿ��ǵ�һ�λ�ȡ����Ϣ
            if (this._ValidateAdministrator == BoolMatchValue.Unspecified)
            {
                // ��鵱ǰ�û��Ƿ����Ա
                if (this.Organization.IsAdministrator(this.UserID))
                {
                    this._ValidateAdministrator = BoolMatchValue.True;
                }
                else
                {
                    if (this.SystemAclManager.Check(this.RecursiveMemberOfs, AclType.Admin))
                    {
                        // �����û����ڵ��������֯��Ԫ�Ƿ�ӵ�и�Ȩ��
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

        // ERROR, �����Ȩ�޽ӿ���Ҫ�򻯣��ȿ���ƽ̨����֮���ٺ˶�һ��
        #region ��֯�ṹ����Ȩ��

        private Dictionary<string, List<SystemOrgAcl>> _SystemOrgAcTable = null;
        /// <summary>
        /// (Acl.OrgScope, Acl)��¼���ϡ�����л���Ļ�����ӻ��������ȡ������Ļ����ӷ������϶�ȡ��
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
        /// ��֤�Ƿ�����֯Ȩ��
        /// </summary>
        /// <param name="UnitID">��ID��ת���������Null����ôת����CompanyID</param>
        /// <param name="Type">Ȩ������</param>
        /// <returns>����û�����Ȩ���򷵻�true�����򷵻�false</returns>
        private bool ValidateSystemOrgAcl(string UnitID, AclType Type)
        {
            // ��ID��ת���������Null����ôת����CompanyID
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
                //һ���Ի�ȡ������ҪУ��Ȩ����֯������������֯����Ҫ����һ����Ȩ�ޣ�����Ȩ�ޣ��޷�ʵ�־ܾ�Ȩ��
                // ������е������ڣ������Լ�
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
        /// �Ƿ��ܹ��༭��֯�ṹ
        /// </summary>
        /// <param name="UnitID">Ҫ�༭����֯�ṹ��ID</param>
        /// <returns>����û�����Ȩ���򷵻�true�����򷵻�false</returns>
        public bool ValidateOrgEdit(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.EditOrg);
        }

        /// <summary>
        /// �Ƿ��ܹ��鿴��֯��������
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public bool ValidateOrgView(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.View);
        }

        /// <summary>
        /// �Ƿ��ܹ�������֯����������:����BO��WF
        /// </summary>
        /// <param name="UnitID">Ҫ������֯������ID</param>
        /// <returns>����û�����Ȩ���򷵻�true�����򷵻�false</returns>
        public bool ValidateOrgAdmin(string UnitID)
        {
            return this.ValidateSystemOrgAcl(UnitID, AclType.Admin);
        }

        /// <summary>
        /// ��֤�Ƿ����������֯�Ĺ���Ȩ��
        /// </summary>
        /// <param name="UnitIds">���ΪNull���߳���Ϊ0��,��ô�����û��Ƿ����������˾�Ĺ���Ȩ��</param>
        /// <returns>����߱�Ȩ�ޣ��򷵻�true�����򷵻�false</returns>
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

        #region Obsolete�ӿ�
        /// <summary>
        /// ��֤��ǰ�û��Ƿ��ܹ�����ָ����֯�ṹ���������ݵ�Ȩ��
        /// </summary>
        /// <param name="UnitId">��֯�ṹ��Χ</param>
        /// <returns>�����Ȩ���򷵻�true�����򷵻�false</returns>
        [Obsolete("�ýӿ�ʧЧ������� ValidateOrgAdmin �ӿ�")]
        public bool ValidateOrgAnalyzeData(string UnitId)
        {
            return this.ValidateOrgAdmin(UnitId);
        }

        /// <summary>
        /// ��֤��ǰ�û��Ƿ��ܹ�����ָ����֯�ṹ���������ݵ�Ȩ��
        /// </summary>
        /// <param name="UnitIds">���ΪNull���߳���Ϊ0��,��ô�����û��Ƿ����������˾�Ĺ���Ȩ��</param>
        /// <returns>�����Ȩ���򷵻�true�����򷵻�false</returns>
        [Obsolete("�ýӿ�ʧЧ������� ValidateOrgsAdmin �ӿ�")]
        public bool ValidateOrgsAnalyzeData(string[] UnitIds)
        {
            return this.ValidateOrgsAdmin(UnitIds);
        }

        /// <summary>
        /// ��֤��ǰ�û��Ƿ��ܹ�����ָ����֯�ṹ���������ܵ�Ȩ��
        /// </summary>
        /// <param name="UnitId">��֯�ṹ��Χ</param>
        /// <returns>�����Ȩ���򷵻�true�����򷵻�false</returns>
        [Obsolete("�ýӿ�ʧЧ������� ValidateOrgAdmin �ӿ�")]
        public bool ValidateOrgAnalyzePerformance(string UnitId)
        {
            return this.ValidateOrgAdmin(UnitId);
        }

        /// <summary>
        /// ��֤��ǰ�û��Ƿ��ܹ�����ָ����֯�ṹ���������ݵ�Ȩ��
        /// </summary>
        /// <param name="UnitIds">���ΪNull���߳���Ϊ0��,��ô�����û��Ƿ����������˾�Ĺ���Ȩ��</param>
        /// <returns>�����Ȩ���򷵻�true�����򷵻�false</returns>
        [Obsolete("�ýӿ�ʧЧ������� ValidateOrgsAdmin �ӿ�")]
        public bool ValidateOrgsAnalyzePerformance(string[] UnitIds)
        {
            return this.ValidateOrgsAdmin(UnitIds);
        }

        /// <summary>
        /// ��֤��ǰ�û��Ƿ��ܹ��鿴ָ����֯�ṹ�����̵�Ȩ��
        /// </summary>
        /// <param name="UnitId">��֯�ṹ��Χ</param>
        /// <returns>�����Ȩ���򷵻�true�����򷵻�false</returns>
        [Obsolete("�ýӿ�ʧЧ������� ValidateView �ӿ�")]
        public bool ValidateOrgViewInstance(string UnitId)
        {
            return this.ValidateOrgView(UnitId);
        }
        #endregion

        /// <summary>
        ///  ��ȡӵ��ָ��Ȩ�޵���֯����ID����
        /// </summary>
        /// <param name="AclType"></param>
        /// <returns></returns>
        private string[] GetOwnOrgs(AclType AclType)
        {
            if (this.ValidateAdministrator())
            {
                // �ǹ���Ա����ô����������˾
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

            //����Ƿ��а���������˾
            if (orgs.Contains(this.CompanyId))
            {
                orgs.Clear();
                orgs.Add(this.CompanyId);
            }
            return orgs.ToArray();
        }

        private string[] _AdminOrgs = null;
        /// <summary>
        /// ������ܹ������ݵ���֯
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
        /// ������ܲ鿴���ݵ���֯�������û��ɲ鿴�ĺͿɹ����������
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
        /// ������ܱ༭����֯�������û��ɱ༭�ĺͿɹ����������
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

        #region ҵ�����Ȩ��

        /// <summary>
        /// BOȨ�޻��棺<SchemaCode+\+FolderId,BizObjectAcl[]>
        /// </summary>
        private Dictionary<string, BizObjectAcl[]> SchemaFolderBOAclTable = new Dictionary<string, BizObjectAcl[]>();

        /// <summary>
        /// ��ȡBOȨ���б�����������еĻ����ӻ�����ȡ��������û�еĻ����ӷ������϶�ȡ
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

            // �ӷ������ϻ�ȡ
            BizObjectAcl[] acls = this.Engine.BizObjectManager.GetBizObjectUserAcls(SchemaCode, FolderId, this.RecursiveMemberOfs);
            this.SchemaFolderBOAclTable.Add(key, acls);

            return acls;
        }

        /// <summary>
        /// ������֯��Χ���ͣ���ȡBOȨ���б�
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

            //Error:����������⣬�������Ա�Ļ���ViewName��ȡ����ֵ��
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
                    //����������Ļ�������ҪУ����֯Ψ����ֱ��У���Ƿ�������Ȩ��
                    if (acl.Check(AclType) || acl.Administrator)
                    {
                        ViewName = acl.ViewName;
                        return true;
                    }
                }
                //������Ȩ�޵Ļ�����У����֯ά��
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
        /// �Ƿ���ʵ���֯
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
        /// У��Ȩ��
        /// </summary>
        /// <param name="SchemaCode">����ģ�ͱ���</param>
        /// <param name="FolderId">�ĵ���ID</param>
        /// <param name="OwnerId">ӵ����</param>
        /// <param name="AclType">Ȩ������</param>
        /// <returns></returns>
        public bool ValidateBizObjectAcl(string SchemaCode, string FolderId, string OwnerId, AclType AclType)
        {
            string ViewName = string.Empty;
            if (OwnerId == this.User.UnitID && AclType == H3.Acl.AclType.View) return true;
            List<string> memberOfList = this.GetMemberOfList(OwnerId, OThinker.Organization.UnitType.Unspecified, true, true);

            return this.ValidateBizObjectAcl(SchemaCode, FolderId, memberOfList.ToArray(), AclType, out ViewName);
        }

        /// <summary>
        /// У���Ƿ��й���Ȩ��
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
        /// У���Ƿ�������Ȩ��
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
        /// У���Ƿ��в鿴Ȩ��
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

        #region ����Ȩ��

        #region �������̵�Ȩ��

        /// <summary>
        /// �������̵����ݻ��棬<WorkflowFullName, bool>
        /// </summary>
        private Dictionary<string, bool> CreateInstanceValidationTable = new Dictionary<string, bool>();
        /// <summary>
        /// �Ƿ�ӵ�д������̵�Ȩ��
        /// </summary>
        /// <param name="WorkflowCode">����ģ�����</param>
        /// <returns>����û�����Ȩ���򷵻�true�����򷵻�false</returns>
        public bool ValidateCreateInstance(string WorkflowCode)
        {
            if (!this.CreateInstanceValidationTable.Keys.Contains(WorkflowCode))
            {

                bool validation = this.ValidateAdministrator()      // �Ƿ���ϵͳ����ԱȨ��
                                || this.WorkflowAclManager.Check(this.RecursiveMemberOfs, WorkflowCode, AclType.Add);// ���ڵ����Ƿ�ӵ�����̷���Ȩ��
                this.CreateInstanceValidationTable.Add(WorkflowCode, validation);
            }
            return this.CreateInstanceValidationTable[WorkflowCode];
        }

        #endregion

        // ERROR, �ⲿ�ִ���û�п�����Ϊ���������BizObjectSchema + Org��ȷ��Ȩ��

        #region ����������Ȩ��У��ӿ�
        /// <summary>
        /// У������ʵ���Ĳ鿴Ȩ��
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="Originator"></param>
        /// <returns></returns>
        public bool ValidateWFInsView(string WorkflowCode, string Originator)
        {
            return this.ValidateWFIns(WorkflowCode, Originator, AclType.View);
        }

        /// <summary>
        /// У������ʵ���Ĺ���Ȩ��
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
        /// ����Ȩ��У��˽�з���
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
        /// �������Ȩ����֤(����Ȩ��)
        /// </summary>
        /// <param name="WorkflowCode">����ģ�����</param>
        /// <param name="Originator">����ʵ���ķ����ˣ�����ָ�ķ����˵�ID</param>
        /// <param name="AclType"></param>
        /// <returns></returns>
        public bool ValidateWFIns(string WorkflowCode, string Originator, AclType AclType)
        {
            // ������
            if (Originator == null)
            {
                return false;
            }
            if (AclType == H3.Acl.AclType.View && this.User.UnitID == Originator) return true;
            //�����û���ݷ���
            User OriginatorUser = this.Engine.Organization.GetUnit(this.User.UnitID) as User;
            List<User> virtualUsers = OriginatorUser.VirtualUsers;
            foreach (User user in virtualUsers)
            {
                if (AclType == H3.Acl.AclType.View && user.UnitID == Originator) return true;
            }
            if (this.ValidateAdministrator()) return true;
            string SchemaCode = this.GetSchemaCodeByWorkflowCode(WorkflowCode);
            if (string.IsNullOrWhiteSpace(SchemaCode)) return false;

            //У����֯Ȩ��
            bool result = this.ValidateSystemOrgAcl(Originator, AclType);
            if (!result)
            {
                //û����֯Ȩ�޵�ʱ���ж�boȨ��
                result = this.ValidateBizObjectAcl(SchemaCode, "", Originator, AclType);
            }
            return result;
        }

        ///// <summary>
        ///// �������ģ��ʵ����Ȩ��
        ///// </summary>
        ///// <param name="WorkflowCode">����ģ�����</param>
        ///// <param name="Originators">�����ˣ����ΪNull���߳���Ϊ0��,��ô�����û��Ƿ����������˾�ķ���Ȩ��</param>
        ///// <returns>ֻ�ж����з����˶��з���Ȩ�޲ŷ���true�����򷵻�false</returns>
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

        #region ��֤����Ȩ��

        #region ����Ȩ��Code

        private Dictionary<string, FunctionAcl> _FunctionAclTable = null;
        /// <summary>
        /// �û�����Ȩ�޻���<UserID_����Code, ��ص�Ȩ���б�>
        /// </summary>
        private Dictionary<string, FunctionAcl> FunctionAclTable
        {
            get
            {
                if (this._FunctionAclTable == null)
                {
                    // ���ص��ڴ���
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

        #region ����У��ӿ�

        /// <summary>
        /// ��õ�ǰ�û�ĳ��������ص�����Ȩ�޼�¼��������֯�������Ȩ�޼�¼
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
            //��ʾ
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
        /// ��ȡ��ǰ�û�������֯������һ��ģ��Ȩ������
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
                //�ȴӻ�����ȡ
                acl = this.GetFunctionAcl(UserId, FunctionCode);
                if (acl != null)
                {
                    break;
                }
            }

            //�����ڻ����У��Ҳ��ӻ����ȡ
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
        /// ��֤Ȩ��
        /// </summary>
        /// <param name="FunctionCode">���ܱ༭��������ΪNull�����򷵻�False</param>
        /// <param name="AclType">Ȩ������</param>
        /// <returns>�����Ƿ����ָ����Ȩ��</returns>
        private bool ValidateFunctionAcl(string FunctionCode, AclType AclType, bool WithinCache = true)
        {
            if (this.ValidateAdministrator())
            {
                return true;
            }

            // ��鱨��ID
            if (string.IsNullOrWhiteSpace(FunctionCode))
            {
                return false;
            }

            // ��������Ȩ��Ȩ��
            FunctionAcl acl = this.GetLastFunctionAcl(FunctionCode, WithinCache);
            // ����Ƿ����Ȩ�޼�¼
            if (acl == null)
            {
                //��������ù���̨��½��Ĭ��AdminRoot ��Ȩ��
                if (FunctionCode == FunctionNode.AdminRootCode && this.User.IsConsoleUser)
                {
                    return true;
                }

                //�������û��Ȩ�����õĻ���ָ����������ҪУ�鸸��Ŀ¼Ȩ��
                FunctionNode node;
                if (this.AllFunctionNodes == null)
                {
                    return false;
                }
                if (!this.AllFunctionNodes.Keys.Contains(FunctionCode))
                {
                    if (WithinCache)
                    {
                        return false;//�ӻ����ж�ȡ�Ļ������������ͷ���false
                    }
                    //���ӻ����ж�ȡ������Ҫ�ӷ������϶�һ��
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
                //�������������ģ�Ͳ˵�����˵���Ƕ�����
                if (node.ParentCode == FunctionNode.Category_ProcessModel_Code)
                {
                    return false;
                }
                //У�鸸��Ŀ¼
                return this.ValidateFunctionAcl(node.ParentCode, AclType, WithinCache);
            }

            // ��֤Ȩ��
            return acl.Check(AclType.Admin) || acl.Check(AclType);
        }

        /// <summary>
        /// ��֤���ڱ����Ƿ�������Ȩ��
        /// </summary>
        /// <param name="FunctionCode">����ID��������ΪNull�����򷵻�False</param>
        /// <returns>�����Ƿ�������е�Ȩ��</returns>
        public bool ValidateFunctionRun(string FunctionCode)
        {
            return this.ValidateFunctionAcl(FunctionCode, AclType.Run);
        }
        #endregion

        /// <summary>
        /// ���й���<FunctionCode,FunctionNode>
        /// </summary>
        private Dictionary<string, FunctionNode> _AllFunctionNodes = null;

        private Dictionary<string, FunctionNode> AllFunctionNodes
        {
            get
            {
                if (this._AllFunctionNodes == null)
                {
                    // �������й����б�
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
        /// ���п������еĹ���
        /// </summary>
        private List<FunctionNode> _RunnableFunctions = null;

        /// <summary>
        /// �����й��ܵ����и������ܱ��룬�����Ҫ�̳и�������Ȩ���ǣ��Ż��¼��<FunctionCode,List<ParentCode>>
        /// </summary>
        private Dictionary<string, List<string>> _FunctionParentCodes = null;

        /// <summary>
        /// ���п������еķ���
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
        /// ������Ȩ�޵Ĺ��ܵ�������
        /// </summary>
        private void LoadRunnableFunctions()
        {
            if (this._RunnableFunctions != null) return;
            this._RunnableFunctions = new List<FunctionNode>();
            this._FunctionParentCodes = new Dictionary<string, List<string>>();
            // �������й����б�
            if (this.AllFunctionNodes == null || this.AllFunctionNodes.Values.Count == 0)
            {
                return;
            }
            FunctionNode[] functions = this.AllFunctionNodes.Values.ToArray();

            //��ҪУ��Ľڵ�����
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
                                               //FunctionNodeType.BizWFFolder,//����Ŀ¼����Ǻ�̨���Ȩ�ޣ��޸ĵü�ʱ����Ŀ¼��������ڻ����ﲻ�����
                                               FunctionNodeType.ProcessModel,
                                                FunctionNodeType.Report, //qiancheng
                                                FunctionNodeType.ReportFolder,
                                                FunctionNodeType.ReportFolderPage
                                           };

            //ѭ��У��Ȩ��
            foreach (FunctionNode function in functions)
            {
                if (!NodeTypes.Contains(function.NodeType)) continue;
                //��Ȩ�ޣ���¼����
                if (this.ValidateFunctionRun(function.Code))
                {
                    this._RunnableFunctions.Add(function);
                }
            }
            // ����
            this._RunnableFunctions.Sort();
        }

        /// <summary>
        /// ����Ӳ˵�����,�����в���
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
        /// ���ݹ��ܴ����ù��ܶ���,���ûȨ�޷���null�������в���
        /// </summary>
        /// <param name="FunctionCode">���ܴ���</param>
        /// <returns>���ܶ���</returns>
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
        /// ʵʱ��ȡ�Ӳ˵�����
        /// </summary>
        /// <param name="ParentCode">���ڵ����</param>
        /// <param name="WithinCache">�Ƿ�ʹ�û���</param>
        /// <returns></returns>
        public FunctionNode[] GetFunctionsByParentCode(string ParentCode, bool WithinCache)
        {
            if (WithinCache) return this.GetFunctionsByParentCode(ParentCode);
            //��ʹ�û���
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
        /// ʵʱ���ݴ����ȡ���ܶ��壬���û��Ȩ�޷���null
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        public FunctionNode GetFunctionNode(string FunctionCode, bool WithinCache)
        {
            if (WithinCache) return this.GetFunctionNode(FunctionCode);
            //��ʹ�û���
            FunctionNode Node = this.FunctionAclManager.GetFunctionNodeByCode(FunctionCode);
            if (Node == null) return null;
            if (this.ValidateFunctionRun(Node.Code)) return Node;
            return null;
        }

        #endregion


        private List<User> lstUsers;
        /// <summary>
        /// ��ȡ�������û�����
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
                    // ��ȡ���б��������û�����
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

        #region ҵ������Ȩ��

        private BizRuleAcl[] _BizRuleAcls = null;
        private Dictionary<string, List<BizRuleAcl>> BizRuleAclTable = new Dictionary<string, List<BizRuleAcl>>();
        /// <summary>
        /// ��ǰ�û�ӵ�е����й���Ȩ��
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
        /// �Ƿ���ڶ���ĳ��ҵ�����Ĺ���Ȩ��
        /// </summary>
        /// <param name="RuleCode">ҵ��������</param>
        /// <param name="DecisionMatrixCode">���߱����</param>
        /// <returns>���ӵ�й���Ȩ�ޣ��򷵻�true�����򷵻�false</returns>
        public bool ValidateRuleAdmin(string RuleCode, string DecisionMatrixCode)
        {
            return this._ValidateRuleAcl(RuleCode, DecisionMatrixCode, AclType.Admin);
        }

        /// <summary>
        /// �Ƿ���ڶ���ĳ��ҵ�����Ĳ鿴Ȩ��
        /// </summary>
        /// <param name="RuleCode">ҵ��������</param>
        /// <param name="DecisionMatrixCode">���߱����</param>
        /// <returns>���ӵ�й���Ȩ�ޣ��򷵻�true�����򷵻�false</returns>
        public bool ValidateRuleView(string RuleCode, string DecisionMatrixCode)
        {
            return this._ValidateRuleAcl(RuleCode, DecisionMatrixCode, AclType.View);
        }

        /// <summary>
        /// �Ƿ���ڶ���ĳ��ҵ������Ȩ��
        /// </summary>
        /// <param name="RuleCode">ҵ��������</param>
        /// <param name="DecisionMatrixCode">���߱����</param>
        /// <param name="Type">Ȩ������</param>
        /// <returns>���ӵ�й���Ȩ�ޣ��򷵻�true�����򷵻�false</returns>
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
