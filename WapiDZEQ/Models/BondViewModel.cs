
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
   
    public class BondViewModel : ViewModelBase
    {
        public int RowNumber_ { get; set; }


        public string Distributorcode { get; set; }

        public string Distributorname { get; set; }

 
        public Decimal? BondProportion { get; set; }


        public string OperationTime { get; set; }

        public string BondState { get; set; }

        public string WorkItemCode { get; set; }

        public string InstanceId { get; set; }

        public string ObjectId { get; set; }

        public Decimal? NewBondProportion { get; set; }


        public string FinalState { get; set; }


        public string Remark { get; set; }

        public string Originatorname { get; set; }

        public string ActivityCode { get; set; }

        public Decimal? Proportion { get; set; }

        public string reDateTime { get; set; }

        public string Status { get; set; }


    }
}
