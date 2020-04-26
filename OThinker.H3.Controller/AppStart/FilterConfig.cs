using OThinker.H3.Controllers.AppStart;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionLogAttribute());
        }
    }
}