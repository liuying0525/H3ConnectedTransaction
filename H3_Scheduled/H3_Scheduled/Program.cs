using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using OThinker.H3;
using OThinker.H3.Controllers;

namespace H3_Scheduled
{
    class Program
    {
        public static System.Threading.Thread schedulerThread = null;
        static void Main(string[] args)
        {
            UpdateMortgage.GetInstance().StartThread();
            //UpdateMortgage updatemort = new H3_Scheduled.UpdateMortgage();
            //updatemort.UpdateMortgageData();
        }
    }
}
