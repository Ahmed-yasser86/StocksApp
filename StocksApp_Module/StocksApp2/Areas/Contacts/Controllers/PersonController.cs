using Microsoft.AspNetCore.Mvc;

namespace StocksApp2.ContactComponent.Controllers
{

    [Area("Contacts")]

    public class PersonController : Controller
    {


        [Route("Person/index")]
        [Route("/")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
