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
using System.Net.Mail;

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
            //System.Diagnostics.Debug.WriteLine("WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            //     Weathergateway.getWeatherData();
            //   DengueCHgateway.uploadDengueCH();
            //  List<string> hlongitude = BHgateway.getlocation();
            //    BHgateway.uploadBreedingHabitat();

            //    DengueClustergateway.uploadDengueCluster();


            return View();
        }

        [HttpPost]
        public ActionResult StoreRegion(String[][] passedRegionArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");

            System.Diagnostics.Debug.WriteLine(passedRegionArray.GetLength(0));
            System.Diagnostics.Debug.WriteLine(passedRegionArray.GetLength(1));
            //logic to store to database
            //DengueClustergateway.uploadDengueCluster(passedRegionArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index","home");
        }

        [HttpPost]
        public ActionResult StoreHabitat(String[][] passedHabitatArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
           // BHgateway.uploadBreedingHabitat(passedHabitatArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult StoreRegionLocation(String[] passedRegionLocationNameArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult StoreHabitatLocation(String[] passedHabitatLocationNameArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
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
        public ActionResult ContactUs(string name,string contactNo,string email,string description)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(email);
                mail.To.Add("dengue@ttgy.sg");
                mail.Subject = "Dengue Contact Us - New Case";
                mail.Body = "Name: " + name + "<br><br>E-mail Address: "+ email +"<br><br>Contact Number: " + contactNo + "<br><br>Description: <br><br>" + description;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("dengue@ttgy.sg", "sexybeast123!");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    ViewBag.Data = "sent";
                }
            }
            return View();
        }
    }
}