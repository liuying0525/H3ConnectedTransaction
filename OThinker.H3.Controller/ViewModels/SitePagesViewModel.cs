using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class SitePagesViewModel : ViewModelBase
    {
        public int RowNumber { get; set; }
        public string Title { get; set; }
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public object CreatedBy { get; set; }
        public object CreatedName { get; set; }
        public object CreatedTime { get; set; }
        public object LastModifiedTime { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
    }
}
