using System.Web;
using System.Web.Http;


namespace Sample_api_users_FF.Controllers
{
    public class DeviceController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Index()
        {

            var ua = Request.Headers.UserAgent.ToString();
            var isM = Utils.IsMobile(ua);

            return Ok(new
            {
                ua = ua,
                IsMobile = isM
            });
        }

    }
}
