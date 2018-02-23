using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BeerShopTicket.Controllers
{
    public class WebExperienceProfilesController : Controller
    {
        // GET: WebExperienceProfiles
        public ActionResult Index()
        {
            var apiContext = GetAPIContext();
            var list = WebProfile.GetList(apiContext);

            if (!list.Any())
            {
                SeedWebProfiles(apiContext);
                list = WebProfile.GetList(apiContext);
            }

            return View(list);
        }

        private void SeedWebProfiles(APIContext apicontext)
        {
            var digitalGoods = new WebProfile()
            {
                 name = "digitalGodsNinja",
                 input_fields = new InputFields()
                 {
                     no_shipping = 1
                 }
            };
            WebProfile.Create(apicontext, digitalGoods);
        }

        private APIContext GetAPIContext()
        {
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        } 
    }
}