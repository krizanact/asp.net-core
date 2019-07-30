using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediatorium.Model.Model.Building;
using Mediatorium.Service.AppService;
using Mediatorium.Model.Core;
using AutoMapper;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Mediatorium.Web.Controllers
{
    public class BuildingController : Controller
    {
        private readonly IBuildingService _buildingService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public BuildingController(IBuildingService buildingService, IUserService userService,IHostingEnvironment hostingEnvironment)
        {
            _buildingService = buildingService;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {           
            if (User.Identity.IsAuthenticated)
            {
                // If user is authenticated store list of his buildign in ViewBag.All
                ViewBag.All = await List();              
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public async Task<string> Create(BuildingInput model)
        {
            
            
            if (await _buildingService.CheckIfThisBuildingIsUnique(model.Street, model.HouseNumber, model.Place))
            {
                return JsonConvert.SerializeObject(new { success = false });
            }

            else
            {
                // Upload image file into uploads folder
                string filePath = null;
                string uniqueFileName = null;
                if (model.customFile != null)
                {
                    uniqueFileName = GetUniqueFileName(model.customFile.FileName);
                    var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    filePath = Path.Combine(uploads, uniqueFileName);
                    model.customFile.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                var building = new Building()
                {
                    BuildingName = model.BuildingName,
                    Street = model.Street,
                    HouseNumber = model.HouseNumber,
                    ZipCode = model.ZipCode,
                    Place = model.Place,
                    Photo = uniqueFileName,
                    BuildingTypeId = model.BuildingTypeId
                };

                await _buildingService.CreateBuilding(building, User.Identity.Name);

                return JsonConvert.SerializeObject(new { success = true });

            }    
        }

        [HttpPost]
        public async Task <IActionResult> Search(string name)
        {
            // Calling this method so list of building stays in quick panel even after post request
            ViewBag.All = await List();

            // Calling all buildings from database
            var allBuildings = await _buildingService.GetAllBuildings();
            // Declare list result so we can save there our wanted results
            List<Building> result = new List<Building>() { };

            foreach (var item in allBuildings)
            {                 
                // Search address from database by street name and its house number
                if (!String.IsNullOrEmpty(name) &&
                    (item.Street.Replace(" ", "").ToLower() + item.HouseNumber).Contains(name.Replace(" ", "").Split(",")[0].ToLower()))
                {
                    result.Add(new Building { BuildingName = item.BuildingName, Photo = item.Photo, Street = item.Street, HouseNumber = item.HouseNumber, Place = item.Place });
                }
            }
            // Save our list in ViewBag so we can retreve this in View
            ViewBag.AllBuildings = result;          
            return View("Index");
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult NewAdv()
        {
            return View();
        }

        public IActionResult OpenAdv()
        {
            return View();
        }

        #region Private methods
        /// <summary>
        /// This method returns list of buildings which logged user has made
        /// </summary>
        /// <returns></returns>
        private async Task<List<Building>> List()
        {
            // Get all buildings from database
            var allBuildings = await _buildingService.GetAllBuildings();
            // Get currently logged user ID
            var user = await _userService.GetUser(User.Identity.Name);
            // Declare list to save our wanted list
            List<Building> list = new List<Building>() { };

            // Check if logged user has made any building so he can see list of his buildings in quick panel
            for (int i = 0; i < allBuildings.Count(); i++)
            {
                if (allBuildings[i].UserBuilding.Where(x => x.UserId.Equals(user.Id)).Any())
                {
                    list.Add(allBuildings[i]);
                }
            }

            return list;
        }
        #endregion

        #region uploadPicture
        public string UploadImage(IFormFile image)
        {
            // Upload image file into uploads folder
            var uniqueFileName = GetUniqueFileName(image.FileName);
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, uniqueFileName);
            image.CopyTo(new FileStream(filePath, FileMode.Create));

            // Set session
            return JsonConvert.SerializeObject(new { fileName = uniqueFileName });
        }

        /// <summary>
        /// Generate unique name for every file that is uploaded
        /// </summary>
        /// <returns>The unique file name.</returns>
        /// <param name="fileName">File name.</param>
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
        #endregion
    }   
}


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
