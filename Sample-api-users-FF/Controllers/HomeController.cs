using System.Web.Mvc;

namespace Sample_api_users_FF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var bc = Request.Browser;
            var ua = Request.Headers["User-Agent"].ToString();
            var isM = Utils.IsMobile(ua);
            
            ViewBag.IsMobileDevice = bc.IsMobileDevice;
            ViewBag.isM = isM;
            ViewBag.ua = ua;

            return View();
        }
    }
}
