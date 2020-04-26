<%@ WebService Language="C#" Class="OThinker.H3.UpdateRiskMain.UpdateRiskMain" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Data;
using OThinker.H3.Controllers;
using OThinker.H3.WorkItem;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using Aspose.Words;
using System.IO;
using System.Drawing;
using Aspose.Words.Drawing;

namespace OThinker.H3.UpdateRiskMain
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UpdateRiskMain : System.Web.Services.WebService
    {
        [WebMethod]
        public void UpdateRiskMainPublic(string instanceId, string bizObjectId, string childObjectId, string parentInstanceID)
        {
            if (string.IsNullOrWhiteSpace(childObjectId) || string.IsNullOrWhiteSpace(parentInstanceID)) return;

            OThinker.H3.Instance.InstanceData InstanceData = new InstanceData(OThinker.H3.Controllers.AppUtility.Engine,
            parentInstanceID, null);
            OThinker.H3.DataModel.BizObject[] childPoints = InstanceData.BizObject["RiskPointDecision"] as OThinker.H3.DataModel.BizObject[];
            OThinker.H3.DataModel.BizObject childPoint = null;

            foreach (DataModel.BizObject item in childPoints)
            {
                if(item.GetValue("ObjectID").ToString() == childObjectId)
                {

                    string sqlbiz = string.Format(@"select A.objectid  from ot_workitem a LEFT JOIN OT_INSTANCECONTEXT B ON A.INSTANCEID=B.OBJECTID where B.BIZOBJECTID='{0}'",bizObjectId);
                    string WorkItemID = AppUtility.Engine.Query.CommandFactory.CreateCommand().ExecuteScalar(sqlbiz).ToString();
                    item.SetValue("ChildInstanceId", WorkItemID);
                }
            }
            InstanceData.BizObject.Update(InstanceData);
        }
    }
}

