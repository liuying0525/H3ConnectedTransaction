using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class AddProductsController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            System.IO.Stream s = context.Request.InputStream;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
            {
                string sql = "";
                Newtonsoft.Json.Linq.JObject ret = Newtonsoft.Json.Linq.JObject.Parse(sr.ReadToEnd());
                foreach (Newtonsoft.Json.Linq.JObject token in ret["Products"])
                {
                    sql = @"
MERGE INTO C_PRODUCTS P1
USING (SELECT '" + token["ProductId"] + @"' AS ProductId, '" + ret["Dealer"] + @"' AS Dealer FROM dual) P2
ON (P1.ProductId=P2.ProductId AND P1.Dealer=P2.Dealer)
WHEN MATCHED THEN
UPDATE SET P1.ProductDescription='" + token["Description"] + @"', P1.ProductAlias='" + token["Alias"] + @"'
WHEN NOT MATCHED THEN
INSERT (ObjectID,Dealer,ProductId,ProductName,ProductAlias,ProductDescription) 
VALUES('" + Guid.NewGuid().ToString() + "',P2.Dealer, P2.ProductId,'" + token["Name"] + "','" + token["Alias"] + "','" + token["Description"] + "')";
                    AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                }
                context.Response.Write("Success");
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