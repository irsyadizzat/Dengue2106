using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dengue.DAL;
using Dengue.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace Dengue.Controllers
{
    public class HomeController : Controller
    {
        private DengueClusterGateway DengueClustergateway = new DengueClusterGateway();
        private WeatherGateway Weathergateway = new WeatherGateway();
        private DengueContext db = new DengueContext();
        private DengueCHGateway DengueCHgateway = new DengueCHGateway();
        private BHGateway BHgateway = new BHGateway();

        IEnumerable<DengueCluster> dengueClusterAll;
        IEnumerable<DengueCaseHistory> dengueCHAll;
       

        // GET: DengueClusters
        public ActionResult Index()
        {

            DengueClustergateway.uploadDengueCluster();
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            Weathergateway.getWeatherData();
            DengueCHgateway.uploadDengueCH();
            BHgateway.uploadBreedingHabitat();
            BHgateway.getDate();
            //ViewData["noDengueCase"] = denguecases;

            //return View(DengueClustergateway.SelectAll());

            return View();
        }

        // GET: DengueClusters
        public ActionResult Case(string search,string sortCases)
        {
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            // ViewData["noDengueCase"] = denguecases;

            dengueClusterAll = DengueClustergateway.SelectAll();
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToUpper();
                dengueClusterAll = dengueClusterAll.Where(d => d.Description.ToUpper().Contains(search));
            }
            else
            {
                ViewBag.PriceSortParm = String.IsNullOrEmpty(sortCases) ? "case_desc" : "";
                switch (sortCases)
                {

                    case "case_desc":
                        dengueClusterAll = DengueClustergateway.Order("case_desc", dengueClusterAll);
                        break;
                    default:
                        dengueClusterAll = DengueClustergateway.Order("default", dengueClusterAll);

                        break;
                }
            }
            List<string> weatherLocation = new List<string>();
            
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();

            foreach (Weather weather in weatherAll)
            {
                weatherLocation.Add(weather.Locations);
            }

            SelectList list = new SelectList(weatherLocation);
            ViewBag.Weather = list;

            return View(dengueClusterAll);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.Message = "Your contact page.";
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(string name,string contactNo,string email,string description)
        {
            string test = name;
            return View();
        }
    }
}