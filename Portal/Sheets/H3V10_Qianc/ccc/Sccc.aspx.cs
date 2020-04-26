using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OThinker.H3.Controllers;
using OThinker.H3.DataModel;

namespace OThinker.H3.Portal.Sheets.H3V10_Qianc
{
    public partial class Sccc : OThinker.H3.Controllers.MvcPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }
       
        public override void SaveDataFields(MvcPostValue MvcPost, MvcResult result)
        {
           
            MvcPost.BizObject.DataItems.SetValue("aaaaaa", "21");
           
            base.SaveDataFields(MvcPost, result);
        }
    }
}
