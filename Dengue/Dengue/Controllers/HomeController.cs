using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Dengue.DAL;
using Dengue.Models;
using System.Net.Mail;
//using System.Web.Helpers;
using System.Web.UI.DataVisualization.Charting;

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
            System.Diagnostics.Debug.WriteLine("WOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();
            foreach (Weather w in weatherAll)
            {
                weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
            }
            ViewBag.Weather = weatherLocationList;

            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            //Weathergateway.getWeatherData();
            //DengueCHgateway.uploadDengueCH();
            //BHgateway.uploadBreedingHabitat();
            //BHgateway.getDate();
            List<string> longitude = DengueClustergateway.getLongitude();
            List<string> latitude = DengueClustergateway.getLatitude();
            //ViewData["noDengueCase"] = denguecases;
            List<string> hlongitude = BHgateway.getLongitude();
            List<string> hlatitude = BHgateway.getLatitude();

            ViewBag.Longitude = longitude;
            ViewBag.Latitude = latitude;

            ViewBag.hLongitude = hlongitude;
            ViewBag.hLatitude = hlatitude;
            //return View(DengueClustergateway.SelectAll());

            return View();
        }

        [HttpPost]
        public ActionResult StoreRegion(String[] passedRegionArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            //DengueClustergateway.uploadDengueCluster(passedRegionArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult StoreHabitat(String[] passedHabitatArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            //BHgateway.uploadBreedingHabitat(passedHabitatArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult StoreRegionLocation(String[] passedHabitatArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            //BHgateway.uploadBreedingHabitat(passedHabitatArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult StoreHabitatLocation(String[] passedHabitatArray)
        {
            System.Diagnostics.Debug.WriteLine("into store region method");
            //System.Diagnostics.Debug.WriteLine(passedHabitatArray[0]);
            //logic to store to database
            //BHgateway.uploadBreedingHabitat(passedHabitatArray);
            System.Diagnostics.Debug.WriteLine("finish store region method");
            return RedirectToAction("index", "home");
        }

        // GET: DengueClusters
        public ActionResult Case(string search, string sortCases)
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

            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();
            foreach (Weather w in weatherAll)
            {
                weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
            }
            ViewBag.Weather = weatherLocationList;

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
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();
            foreach (Weather w in weatherAll)
            {
                weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
            }
            ViewBag.Weather = weatherLocationList;

            ViewBag.Message = "Your contact page.";
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(string name, string contactNo, string email, string description)
        {
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();
            foreach (Weather w in weatherAll)
            {
                weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
            }
            ViewBag.Weather = weatherLocationList;
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(email);
                mail.To.Add("dengue@ttgy.sg");
                mail.Subject = "Dengue Contact Us - New Case";
                mail.Body = "Name: " + name + "<br><br>E-mail Address: " + email + "<br><br>Contact Number: " + contactNo + "<br><br>Description: <br><br>" + description;
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

        public ActionResult EvaluateArea(string weather)
        {
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();
            foreach (Weather w in weatherAll)
            {
                weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
            }
            ViewBag.Weather = weatherLocationList;

            if (weather == null)
            {
                ViewBag.selectedEvaluateArea = false;
            }
            else
            {
                //0 = street name, 1 = zone, 2 = forecast
                string[] passedWeatherInfo = weather.Split(';');

                ViewBag.street = passedWeatherInfo[0];
                if (passedWeatherInfo[1] == "C")
                {
                    ViewBag.region = "Central";
                }
                else if (passedWeatherInfo[1] == "N")
                {
                    ViewBag.region = "North";
                }
                else if (passedWeatherInfo[1] == "S")
                {
                    ViewBag.region = "South";
                }
                else if (passedWeatherInfo[1] == "E")
                {
                    ViewBag.region = "East";
                }
                else if (passedWeatherInfo[1] == "W")
                {
                    ViewBag.region = "West";
                }

                ViewBag.selectedEvaluateArea = true;
            }
            System.Diagnostics.Debug.WriteLine(weather);




            return View();
        }

        public ActionResult DrawDengueClusterRegionChart()
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y = new int[5] { 50, 50, 50, 50, 50 };

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 455;

            Title t = new Title();
            t.Text = "Dengue Cluster Chart (By Region)";

            chart.Titles.Add(t);

            ChartArea ca = new ChartArea();
            chart.ChartAreas.Add(ca);

            Series dataS = new Series("Data");

            dataS.ChartType = SeriesChartType.Pie;
            dataS["PieLabelStyle"] = "Outside";
            dataS["PieLineColor"] = "Black";

            for (int i = 0; i < x.Length; i++)
            {
                dataS.Points.AddXY(x[i], y[i]);
                dataS.Label = "#VALX\n   #VALY";
            }

            chart.Series.Add(dataS);
            chart.Series["Data"].Points[2]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/DengueClusterRegionChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/DengueClusterRegionChart"), "jpeg");
        }

        public ActionResult DrawBreedingHabitatRegionChart()
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y = new int[5] { 50, 50, 50, 50, 50 };

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 455;

            Title t = new Title();
            t.Text = "Breeding Habitat Chart (By Region)";

            chart.Titles.Add(t);

            ChartArea ca = new ChartArea();
            chart.ChartAreas.Add(ca);

            Series dataS = new Series("Data");

            dataS.ChartType = SeriesChartType.Pie;
            dataS["PieLabelStyle"] = "Outside";
            dataS["PieLineColor"] = "Black";

            for (int i = 0; i < x.Length; i++)
            {
                dataS.Points.AddXY(x[i], y[i]);
                dataS.Label = "#VALX\n   #VALY";
            }

            chart.Series.Add(dataS);
            chart.Series["Data"].Points[2]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/BreedingHabitatRegionChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/BreedingHabitatRegionChart"), "jpeg");
        }

        public ActionResult DrawOverallChart()
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y = new int[5] { 50, 50, 50, 50, 50 };

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 455;

            Title t = new Title();
            t.Text = "Overall Chart (Breeding Habitat + Dengue Cluster) (By Region)";

            chart.Titles.Add(t);

            ChartArea ca = new ChartArea();
            chart.ChartAreas.Add(ca);

            Series dataS = new Series("Data");

            dataS.ChartType = SeriesChartType.Pie;
            dataS["PieLabelStyle"] = "Outside";
            dataS["PieLineColor"] = "Black";

            for (int i = 0; i < x.Length; i++)
            {
                dataS.Points.AddXY(x[i], y[i]);
                dataS.Label = "#VALX\n   #VALY";
            }

            chart.Series.Add(dataS);
            chart.Series["Data"].Points[2]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/OverallChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/OverallChart"), "jpeg");
        }

    }
}