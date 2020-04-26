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
    public partial class XSSH : OThinker.H3.Controllers.MvcPage
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
            OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + "：驳回" + this.MvcPost.Command);
            
            // 保存后，后台执行事件
            base.SaveDataFields(MvcPost, result);
        }
    }
}
