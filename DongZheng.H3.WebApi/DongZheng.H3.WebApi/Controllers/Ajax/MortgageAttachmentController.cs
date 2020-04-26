using OThinker.H3.Controllers;
using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class MortgageAttachmentController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string BIZobjectId = context.Request["BizObjectId"];
            string instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + BIZobjectId + "'") + string.Empty;
            OThinker.H3.Instance.InstanceData instanceData = new OThinker.H3.Instance.InstanceData(AppUtility.Engine, instanceid, null);
            string contractNum = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT 合同号 FROM I_MORTGAGE WHERE OBJECTID = '" + BIZobjectId + "'") + string.Empty;

            CopyAttachmentFile(BIZobjectId, contractNum);
        }

        /// <summary>
        /// </summary>
        /// </summary>
        /// <param name="BizObjectId">BIZObjectID</param>
        /// <param name="contractNum">合同号</param>
        public void CopyAttachmentFile(string BizObjectId, string contractNum)
        {
            string attachmentId = "";
            string sql = "select objectid from Ot_Attachment where bizobjectid='" + BizObjectId + "' order by createdtime desc";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    attachmentId = dt.Rows[i]["objectID"].ToString(); ;
                    Attachment attachment = new Attachment();
                    attachment = OThinker.H3.Controllers.AppUtility.Engine.BizObjectManager.GetAttachment("", "", BizObjectId, attachmentId);
                    byte[] Content = attachment.Content;
                    if (System.IO.Directory.Exists(@"D:\H3 BPM\Portal\\TemplateFile\\" + contractNum + "") == false)
                        System.IO.Directory.CreateDirectory(@"D:\H3 BPM\Portal\\TemplateFile\\" + contractNum + "");
                    System.IO.File.WriteAllBytes(@"D:\H3 BPM\Portal\\TemplateFile\\" + contractNum + "\\" + attachment.FileName, Content);
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}