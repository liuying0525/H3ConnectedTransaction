using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ProductCenter
{
    public class ProductDealerRelationRequest
    {
        public List<AddRelationProduct> Products { get; set; }

        public List<AddRelationDealer> Dealers { get; set; }

    }

    public class AddRelationProduct
    {
        public string ProductId { get; set; }

        public string ProductName { get; set; }
    }

    public class AddRelationDealer
    {
        public string DealerName { get; set; }

        public string DealerCode { get; set; }
    }
}