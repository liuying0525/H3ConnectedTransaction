using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.DTO
{
    public class PbocResultDto
    {
        /// <summary>
        /// 人员角色	
        /// </summary>
        public string personnelCategory { get; set; } = string.Empty;

        public string PersonnelCategoryName
        {
            get
            {
                var category = "";
                switch (personnelCategory)
                {
                    case "A":
                        category = "申请人";
                        break;
                    case "B":
                        category = "共同申请人";
                        break;
                    case "C":
                        category = "担保人";
                        break;
                    default:
                        break;

                }

                return category;
            }
        }

        public string name { get; set; }

        public string idCard { get; set; }

        public string pbocReportUrl { get; set; }
    }
}