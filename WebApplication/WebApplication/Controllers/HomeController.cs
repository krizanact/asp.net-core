using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        /*pozivom ove 2 funkcije dobijemo ispis cijele liste iz klase DataRepository
          * u slucaju da ne koristimo return  view-a i funkcije iznad*/


        public HomeController()
        {
            var results = new DataRepository();
            //call function from other class
            results.InitializeData().GetAwaiter().GetResult();


        }

        [HttpGet]
        public IEnumerable<Data> GetAll()
        {
            return DataRepository._stdInfo;

        }
    }
}
