using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 
using OThinker.H3.Controllers;
namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class FKSQ_Mobile : TemplatePrintClass
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
            // 保存后，后台执行事件
            base.SaveDataFields(MvcPost, result);
        }
    }
}
