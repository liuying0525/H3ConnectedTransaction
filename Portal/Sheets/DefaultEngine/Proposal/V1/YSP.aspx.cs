using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OThinker.H3.Controllers;
using System.Collections.Generic;
using System.Data.OracleClient;

namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class YSP : OThinker.H3.Controllers.MvcPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void SaveDataFields(MvcPostValue MvcPost, MvcResult result)
        {
            //wangxg 19.8
            string val = "";
            foreach (var item in MvcPost.BizObject.DataItems)
            {
                val += item.Value.V;
            }

            string msg = "";
            bool isInject = new DongZheng.H3.WebApi.Controllers.XssAttribute().IsContainXSSCharacter(val, out msg);
            if (isInject)
            {
                result.Successful = false;
                result.Errors.Add("检测到SQL敏感字符");
                return;
            }
            isInject = new DongZheng.H3.WebApi.Controllers.SqlInjectAttribute().IsSqlInjectCharacter(val, out msg);
            if (isInject)
            {
                result.Successful = false;
                result.Errors.Add("检测到XSS敏感字符");
                return;
            }
            // 保存后，后台执行事件
            base.SaveDataFields(MvcPost, result);
        }


    }
}
