using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class STQHK : OThinker.H3.Controllers.MvcPage
    {
        public string wxapp_img_url = System.Configuration.ConfigurationManager.AppSettings["wxapp_img_url"] + string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
