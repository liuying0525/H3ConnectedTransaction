using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Novacode;
using System.IO;
using Aspose.Words;

namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class NTKO_TESTEdit : OThinker.H3.Controllers.MvcPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }


        //public void ChangeToPdf()
        //{
        //    string path = System.AppDomain.CurrentDomain.BaseDirectory;
        //    //path += "Sheets\\模版\\个人零售贷款合同_final.doc";
        //    path += "Sheets\\模版\\个人零售贷款合同.doc";
        //    //using (DocX document = DocX.Load(path))
        //    //{
        //    //    document.Bookmarks["xb"].SetText("1");
        //    //    document.Bookmarks["xm"].SetText("ddfsd123");
        //    //    string filename = path + "Sheets\\模版\\个人零售贷款合同_final.doc";
        //    //    document.Save();//SaveFormat.Pdf
        //    //}

        //    var stream = new MemoryStream(FileBytes(path));
        //    var document = new Document(stream);
        //    var builder = new DocumentBuilder(document);
        //    //builder.StartBookmark("asset_brand");
        //    //builder.Write("asd");
        //    //builder.EndBookmark("asset_brand");
        //    document.Range.Bookmarks["asset_brand"].Text = "dasdasd";
        //    string filePath = Server.MapPath("~/TempImages/") + Path.GetFileNameWithoutExtension("个人零售贷款合同") + "_final.doc";
        //    //保存成PDF
        //    builder.Document.Save(filePath, SaveFormat.Doc);

        //}



        public void ChangeToPdf()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            //path += "Sheets\\模版\\个人零售贷款合同_final.doc";
            path += "Sheets\\模版\\个人零售贷款合同.doc";
            //using (DocX document = DocX.Load(path))
            //{
            //    document.Bookmarks["xb"].SetText("1");
            //    document.Bookmarks["xm"].SetText("ddfsd123");
            //    string filename = path + "Sheets\\模版\\个人零售贷款合同_final.doc";
            //    document.Save();//SaveFormat.Pdf
            //}

            var stream = new MemoryStream(FileBytes(path));
            var document = new Document(stream);
            var builder = new DocumentBuilder(document); 
            document.Range.Bookmarks["asset_brand"].Text = "asset_brand";
            document.Range.Bookmarks["asset_usage"].Text = "asset_usage";
            document.Range.Bookmarks["application_nbr"].Text = "application_nbr";
            document.Range.Bookmarks["guarantor_name"].Text = "guarantor_name";
            document.Range.Bookmarks["guarantor_id_nbr"].Text = "guarantor_id_nbr";
            document.Range.Bookmarks["g_post_code"].Text = "g_post_code";
            document.Range.Bookmarks["guarantor_address"].Text = "guarantor_address";
            document.Range.Bookmarks["borrower_name"].Text = "borrower_name";
            document.Range.Bookmarks["borrower_id_nbr"].Text = "borrower_id_nbr";
            document.Range.Bookmarks["coborrower_name"].Text = "coborrower_name";
            document.Range.Bookmarks["coborrower_id_nbr"].Text = "coborrower_id_nbr";
            document.Range.Bookmarks["borrower_address"].Text = "borrower_address";
            document.Range.Bookmarks["b_postcode1"].Text = "b_postcode1";
            document.Range.Bookmarks["b_mobile"].Text = "b_mobile";
            document.Range.Bookmarks["lienee_name"].Text = "lienee_name";
            document.Range.Bookmarks["dealer_name"].Text = "dealer_name";
            document.Range.Bookmarks["dealer_code"].Text = "dealer_code";
            document.Range.Bookmarks["dealer_address"].Text = "dealer_address";
            document.Range.Bookmarks["dealer_phone"].Text = "dealer_phone";
            document.Range.Bookmarks["dealer_postcode"].Text = "dealer_postcode";
            document.Range.Bookmarks["asset_price"].Text = "asset_price";
            document.Range.Bookmarks["accessories_ind"].Text = "accessories_ind";
            document.Range.Bookmarks["downpayment_amt"].Text = "downpayment_amt";
            document.Range.Bookmarks["brand"].Text = "brand";
            document.Range.Bookmarks["asset_brand"].Text = "asset_brand";
            document.Range.Bookmarks["asset_model"].Text = "asset_model";
            document.Range.Bookmarks["vin_number"].Text = "vin_number";
            document.Range.Bookmarks["vehicle_condition"].Text = "vehicle_condition";
            document.Range.Bookmarks["asset_color"].Text = "asset_color";
            document.Range.Bookmarks["manufacturer_year"].Text = "manufacturer_year";
            document.Range.Bookmarks["asset_usage"].Text = "asset_usage";
            string GUID = System.Guid.NewGuid().ToString();
            string filePath = Server.MapPath("~/TempImages/") + Path.GetFileNameWithoutExtension("个人零售贷款合同") + GUID+".doc";

            builder.Document.Save(filePath, SaveFormat.Doc);
            // UploadAttachment(filePath);SetReadOnly
        }


        public static byte[] FileBytes(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new byte[0];
            }

            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];

            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }
    }
}
