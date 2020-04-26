using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ProductCenter
{
    public class ProductCarTypeQueryRequest: PagerInfo
    {
        public string ProductName { get; set; }
        public string CarType { get; set; }
    }

    public class ProductCarTypeQueryViewModel : ViewModelBase
    {
        public int RowIndex { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string AssetMake { get; set; }

        public string AssetMakeCode { get; set; }

        public string AssetBrand { get; set; }

        public string AssetBrandCode { get; set; }

        public string AssetModel { get; set; }

        public string AssetModelCode { get; set; }

        public string Price { get; set; }
    }
}