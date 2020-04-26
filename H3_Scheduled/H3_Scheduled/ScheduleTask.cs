using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H3_Scheduled
{
   public class ScheduleTask : BaseScheduleTask
    {
        protected override void DealBusiness()
        {
            DateTime now = DateTime.Now;
            //throw new NotImplementedException();
        }

        public virtual void Run()
        {
            DealBusiness();
        }
    }
}
