using BeerShopAppSimple.Entities;
using BeerShopTicket.Models;
using BeerShopTicket.Models.Home;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BeerShopTicket.Controllers
{
    public class HomeController : Controller
    {
        private DBContext _db = new DBContext();

        //get
        [HttpGet]
        public ActionResult Index()
        {
            var model = new IndexViewModel()
            {
                Events = _db.Events.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Purchase(int? id)
        {
            //mudar id, logic for test
            var ev = _db.Events.FirstOrDefault(m => m.id == 1);

            var model = new PurchaseViewModel()
            {
                Title = ev.Title,
                ImageUrl = ev.ImageUrl,
                EventDate = ev.EventDate,
                Price = ev.Price
            };

            Session["id"] = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Purchase(PurchaseViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                var id = Convert.ToInt16(Session["id"]);
                var ev = _db.Events.FirstOrDefault(m => m.id == id);

                var ticket = new Ticket()
                {
                    FirstName = pvm.FName,
                    LastName = pvm.LName,
                    Email = pvm.Email,
                    EventDate = ev.EventDate
                };
                _db.Tickets.Add(ticket);
                _db.SaveChanges();

                var apicontext = GetAPIContext();

                var payment = new Payment
                {
                     experience_profile_id = "XP-SNYK-8WBV-46QD-5RWK",
                     intent = "sale",
                     payer = new Payer
                     {
                         payment_method = "paypal"
                     },
                     transactions = new List<Transaction>
                     {
                         new Transaction
                         {
                             description = $"Evento(único) para {ev.Title} agendado para {ev.EventDate:dddd, dd MMMM yyyy}",
                             amount = new Amount
                             {
                                 currency = "BRL",
                                 total = ev.Price.ToString()
                             },
                             item_list = new ItemList()
                             {
                                 items = new List<Item>()
                                 {
                                     new Item()
                                     {
                                         description = $"Evento (único) para {ev.Title} na data {ev.EventDate:dddd, dd MMMM yyyy}",
                                         currency = "BRL",
                                         quantity = "1",
                                         price = ev.Price.ToString()
                                     }
                                 }
                             }
                         }
                     },
                     redirect_urls = new RedirectUrls
                     {
                         return_url = Url.Action("Return","Home", null,Request.Url.Scheme),
                         cancel_url = Url.Action("Cancel","Home",null,Request.Url.Scheme)
                     }
                };

                //envia o pagamento para o paypal
                var createPayment = payment.Create(apicontext);
                //salva a referencia do pagamento
                ticket.PayPalReference = createPayment.id;
                _db.SaveChanges();
                //procura a URL de aprovação para enviar para o user
                var approvalUrl = createPayment.links.FirstOrDefault(m => m.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));

                //envia a page de aprovação para o user
                return Redirect(approvalUrl.href);

            }

            return View();

        }

        public ActionResult Return(string payerId, string paymentId)
        {
            //busca o ticket existente
            var ticket = _db.Tickets.FirstOrDefault(m => m.PayPalReference == paymentId);
            //popula variavel com credenciais da api
            var apiContext = GetAPIContext();
            //config o pagador para o pagamento
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            //identifica o pagamento para executar
            var payment = new Payment()
            {
                id = paymentId
            };
            //execute o pagamento
            var executePayment = payment.Execute(apiContext, paymentExecution);

            return RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult Cancel()
        {
            return View();
        }

        private APIContext GetAPIContext()
        {
            //autenticacao paypal 
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }


    }
}