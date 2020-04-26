using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OThinker.H3.Controllers;

namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class HSRetailApp : OThinker.H3.Controllers.MvcPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 保存表单数据到引擎中
        /// </summary>
        /// <param name="Args"></param>
        public override void SaveDataFields(MvcPostValue MvcPost, MvcResult result)
        {
            // 以下函数可改变数据项的值
            //MvcPost.BizObject.DataItems.SetValue("CJH", MvcPost.InstanceId);
            if (this.MvcPost.Command == "Submit" || this.MvcPost.Command == "Save"  || this.MvcPost.Command == "Reject")
            {
                this.ActionContext.InstanceData["instanceid"].Value = MvcPost.InstanceId;
            }

            // 保存后，后台执行事件
            base.SaveDataFields(MvcPost, result);
        }
    }
}
