using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Repositories;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        //2 liste za spremanje ID i Cijene
        public static IEnumerable<long> idArray = new List<long>();
        public static IEnumerable<string> priceArray = new List<string>();

        
         public ActionResult Values()
          {

              return View();
          }

          [HttpPost]
          public ActionResult Values(Input id)
          {
            var results = new DataRepository();
            //poziv funkcije iz DataRepository
            results.InitializeData().GetAwaiter().GetResult();
            //pozovi u listu idArray svaki ucitani ID iz DataRepository._stdInfo
            //pozovi u listu priceArray svaki ucitani price iz DataRepository._stdInfo

            idArray = DataRepository._stdInfo.Select(c => c.id);
            priceArray = DataRepository._stdInfo.Select(c => c.price);
            int counter = 0;

            var input = id.inputId;  //spremiti u variajblu input unesenu vrijednost

            

            for (var i = 0; i < idArray.Count(); i++)
            {


                if(input == idArray.ElementAt(i))
                {
                    counter++;
                    return Content("Uneseni ID postoji i cijena proizvoda je " + priceArray.ElementAt(i) + " $");
                    
                }
               
                
           

            }

            if (counter == 0)
            {
                return Content("Prema pretragama ID-a proizvod ne postoji! ");
            }

            return View();
              
          }


        
        






    }
    }
