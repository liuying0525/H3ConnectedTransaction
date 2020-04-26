using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using OThinker.H3.Portal;
using OThinker.Organization;


public partial class ContractEdit : OThinker.H3.WorkSheet.SheetPage
{
    public string CNTDBConnPool
    {
        get { return System.Configuration.ConfigurationManager.AppSettings["CNTConnPool"]; }
    }

    public void Alert(string message)
    {
        Page.ClientScript.RegisterStartupScript(GetType(), "", "alert('"+ message+"');", true);
    }

    
    /// <summary>
    /// 页面标题
    /// </summary>
    public string PageTitle
    {
        get
        {
            return Request["title"] ?? string.Empty;
        }
    }

    /// <summary>
    /// 正文数据项
    /// </summary>
    public string DataField {
        get
        {
            return Request["datafield"] ?? string.Empty;
        }
    }

    /// <summary>
    /// PDF数据项
    /// </summary>
    public string PDFDataField
    {

        get
        {

            if (Request["pdfdatafield"] == null)
                return "PDF正文";

            return Request["pdfdatafield"];
        }
    }

    /// <summary>
    /// 文件保存名称
    /// </summary>
    public string FileName
    {
        get
        {
            if (Request["fileName"] == null) {
                return string.Empty;
            }
            return Request["fileName"];
        }
    }
   

    protected void Page_Load(object sender, EventArgs e)
    {
        var fileTitle = FileName;
        //hidAttachmentID.Value = SheetOffice1.PdfID;

        #region 不同的流程，文件保存名称不相同，需要写逻辑获取文档保存名称
       
        #endregion

        hdDocTitle.Value = fileTitle;//保存的文件名称
        if (string.IsNullOrEmpty(hdDocTitle.Value))
        {
            hdDocTitle.Value = Guid.NewGuid().ToString();
        }
        lblTitle.Text = PageTitle;

        //对应数据项名称，每一个流程对应的数据项不同，从URL中传递
        //SheetOffice1.DataField = DataField;
        //SheetOffice1.PDFDataField = PDFDataField;

        var contractTypeId = Enviroment.InstanceData["ContractTypeId"].Value;
        var sql = @"select t.* from CNT_ContractTemplate t where t.ContractId=" + contractTypeId;
        //var dt = MMSHFunction.ExecuteTableSql(CNTDBConnPool, sql);
        //if (dt != null && dt.Rows.Count > 0)
        //{
        //    ddl_TempName.DataTextField = "ContractTemplate";
        //    ddl_TempName.DataValueField = "ID";
        //    ddl_TempName.DataSource = dt;
        //    ddl_TempName.DataBind();
        //}
        ddl_TempName.Items.Insert(0, new ListItem("<--请选择合同模版-->","-1"));
    }

    /// <summary>
    /// 获取合同类型所属的部门名称
    /// </summary>
    /// <returns></returns>
    public string GetContactDeptName()
    {
        var deptName = string.Empty;
        var contractId = Enviroment.InstanceData["ContractTypeId"].Value;
        var contractIds = new List<string> {"301","302","303","304","305"};//柴油机销售合同
        if (contractIds.Contains(contractId.ToString()))
        {
            var signDept = Enviroment.InstanceData["SignDept"] == null ? string.Empty : Enviroment.InstanceData["SignDept"].Value.ToString();
            if (signDept == "中车戚墅堰机车车辆工艺研究所有限公司") return string.Empty;
        }
//        var strSql = string.Format(@"select t.*,a.[Name] AS DeptName from CNT_ContractType t 
//            left join X_Dept a on a.Id=t.DeptId where ContractId={0}", contractId);
//        var dt = MMSHFunction.ExecuteTableSql(CNTDBConnPool, strSql);
        //if (dt != null && dt.Rows.Count > 0)
        //{
        //    deptName = dt.Rows[0]["DeptName"].ToString();
        //}
        return deptName;
    }

    /// <summary>
    /// 获取合同流程可将合同正文另存的名单
    /// </summary>
    /// <returns></returns>
    //public string GetOutCooperateSaveAs()
    //{
    //    var members = string.Empty;
    //   // var users = GetGroupMembersByGroupCode("G_OutCooperate_SaveAs");
    //    if (users != null && users.Length > 0)
    //    {
    //        foreach (var user in users)
    //        {
    //            members += user + ";";
    //        }
    //        members = members.TrimEnd(';');
    //    }
    //    return members;
    //}

    /// <summary>
    /// 根据组编码获取指定组的成员
    /// </summary>
    /// <param name="groupCode">组编码</param>
    /// <returns></returns>
    //protected string[] GetGroupMembersByGroupCode(string groupCode)
    //{
    //    //var groupId = string.Empty;
    //    //var g = Enviroment.Engine.Organization.GetUnitByCode(groupCode) as Group;
    //    //if (g != null)
    //    //{
    //    //    groupId = g.ObjectID;
    //    //}
    //    //var strs = new string[1];
    //    //strs[0] = groupId;
    //    //return Enviroment.Engine.Organization.GetMembers(strs);
    //}
}
