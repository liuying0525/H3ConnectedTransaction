<%@ WebService Language="C#" Class="Organization" %>

using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;

/// <summary>
/// Summary description for Organization
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Organization : System.Web.Services.WebService
{
    public Organization()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    [WebMethod]
    public string GetManager(string ID)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.GetManager(ID);
    }

    [WebMethod]
    public string GetName(string UserID)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.GetName(UserID);
    }

    [WebMethod]
    public string GetFullName(string UnitID)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.GetFullName(UnitID);
    }

    [WebMethod]
    public string GetParent(string ID)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.GetParent(ID);
    }



    [WebMethod]
    public bool IsAncestor(string ChildID, string AncestorID)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.IsAncestor(ChildID, AncestorID);
    }

    [WebMethod]
    public OThinker.Organization.HandleResult AddUser(string Modifier, OThinker.Organization.User User)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(Modifier, User);
    }

    [WebMethod]
    public OThinker.Organization.HandleResult AddGroup(string Modifier, OThinker.Organization.Group Group)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(Modifier, Group);
    }

    [WebMethod]
    public OThinker.Organization.HandleResult AddOrgUnit(string Modifier, OThinker.Organization.OrganizationUnit OrgUnit)
    {
        return OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(Modifier, OrgUnit);
    }
/// <summary>
    /// 组织架构添加
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public string AddMyUnit(string uiteid,string dep)
    {
        string time=DateTime.Now.ToString()+":begin";
        //var unit = new OThinker.Organization.OrganizationUnit();
        //unit.ObjectID = Guid.NewGuid().ToString();
        //unit.Name = "内网经销商";//1d80ab5b-5b10-416a-ad4d-f3f6e329ddc9  外网经销商：
        //unit.ParentID = OThinker.H3.Controllers.AppUtility.Engine.Organization.Company.UnitID;
        //--信审  22e018b7-1bf3-48c1-8575-932a44605063   运营  a9a1b321-4d6b-47cb-820f-0e7cfe01e17e
        //string uiteid = "a9a1b321-4d6b-47cb-820f-0e7cfe01e17e";
        string sql = "   select * from H3.A_USER where dep='"+dep+"'";
        DataTable dt = new DataTable();
        dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        string rcount="   finished:"+dt.Rows.Count;
        for(int i=0;i<dt.Rows.Count;i++)
        {
            var parentou = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetUnit(uiteid);
            var unitUser = new OThinker.Organization.User();
            unitUser.ObjectID = Guid.NewGuid().ToString();
            unitUser.Code = dt.Rows[i]["code"].ToString();
            unitUser.Name =  dt.Rows[i]["name"].ToString();
            unitUser.ParentID = parentou == null ? OThinker.H3.Controllers.AppUtility.Engine.Organization.Company.UnitID : parentou.ObjectID;
            //unitUser.EmployeeNumber = employeenumber;
            unitUser.Mobile=  dt.Rows[i]["phone"].ToString();
            //unitUser.Email = "18363847@qq.com";
            unitUser.CreatedTime = DateTime.Now;
            // 写入服务器 
            var result = OThinker.Organization.HandleResult.SUCCESS;
            result = OThinker.H3.Controllers.AppUtility.Engine.Organization.AddUnit(null, unitUser);
            if (result != OThinker.Organization.HandleResult.SUCCESS)
            {
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(unitUser.Name);

            }
        }

        return time+rcount;
    }
}

