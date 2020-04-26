using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizMasterDataImportViewModel:ViewModelBase
    {
        public string XmlSessionName { get; set; }

        public string ParentID { get; set; }

        public string ParentCode { get; set; }

        public string MasterDataCode { get; set; }

        public string MasterDataName { get; set; }

        public string CodeInfo { get; set; }

        public bool IsCover { get; set; }
    }
}
