using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Enums;

namespace StocksApp2.ContactComponent.Controllers
{

    [Area("Contacts")]

    public class PersonController : Controller
    {



        private readonly IPersonServices _personServices;
        private readonly ICountryServices _countryServices;
        public PersonController(IPersonServices personServices , ICountryServices countryServices)
        {
            _personServices = personServices;
            _countryServices = countryServices;
        }

     
        [Route("Person/Create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countryServices.Countries();
            ViewBag.Countries = countries;

            return View();
        }


        [Route("Person/Create")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest model)
        {
            List<CountryResponse> countries = _countryServices.Countries();
            ViewBag.Countries = countries;

            if (ModelState.IsValid)
            {
                _personServices.AddPerson(model);
                return RedirectToAction("Index","Person");
            }

            ViewBag.ModelErrors = ModelState.Values.SelectMany(v => v.Errors);
            return View(model);
        }

      

        [Route("Person/index")]
        [Route("/")]
        public IActionResult Index(string SearchBy,string SearchString,
            string SortedBy , sortedListOp  SortOption= sortedListOp.Ascending)
        {

            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(Person.email), "Email"},
                {nameof(Person.Gender),"Gender" },
                {nameof(Person.NewsLetter),"News Letter" },
                {nameof(Person.DateOfBirth),"Date Of Birth" },
                {nameof(Person.phone),"Phone" },
                {nameof(Person.CountryId),"Country" },
                {nameof(Person.Name),"Name" }
            };

            ViewBag.CurrentSearchBy = SearchBy;
            ViewBag.CurrentSearchString = SearchString;

            ViewBag.CurrentSortedBy =SortedBy;
            ViewBag.CurrentSortOption = SortOption.ToString();

            try
            {
                var persons = _personServices.SearchPersonsBy( SearchString, SearchBy);

                var SortedPersons = _personServices.getPersonsSorted(persons, SortedBy, SortOption);
                return View(SortedPersons);
            }
            catch (Exception ex)
            {

                return View("Error");
            }
        }
    }
}
