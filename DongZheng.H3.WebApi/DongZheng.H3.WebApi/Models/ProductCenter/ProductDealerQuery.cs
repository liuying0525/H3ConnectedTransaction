using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ProductCenter
{
    public class ProductDealerQueryRequest: PagerInfo
    {
        public string ProductName { get; set; }
        public string DealerName { get; set; }
    }

    public class ProductDealerQueryViewModel : ViewModelBase
    {
        public int RowIndex { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// 产品组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string GroupId { get; set; }

        public string CompanyName { get; set; }

        public string CompanyId { get; set; }
    }
}