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


            return View();
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

                //logic to get risk level
                int dcScore = 0, bhScore = 0, weatherScore = 0, totalScore = 0;
                double dcAverage = 0, bhAverage = 0;
                double sgBH = BHgateway.getNoCases();
                double sgDC = DengueClustergateway.getNoCases();
                double regionBH = BHgateway.getCasesRegion(passedWeatherInfo[1]);
                double regionDC = DengueClustergateway.getCasesRegion(passedWeatherInfo[1]);

                //get average percentage
                dcAverage = ((sgDC / 5) / sgDC) * 100;
                bhAverage = ((sgBH / 5) / sgBH) * 100;

                System.Diagnostics.Debug.WriteLine(dcAverage);
                System.Diagnostics.Debug.WriteLine(bhAverage);

                double bhPercent = (regionBH / sgBH) * 100;
                double dcPercent = (regionDC / sgDC) * 100;

                System.Diagnostics.Debug.WriteLine(bhPercent);
                System.Diagnostics.Debug.WriteLine(dcPercent);

                if (bhPercent >= 66)
                {
                    bhScore = 3;
                }
                else if (bhPercent >= 33) {
                    bhScore = 2;
                }
                else if (bhPercent >= 0){
                    bhScore = 1;
                }

                if (dcPercent >= 66)
                {
                    dcScore = 3;
                }
                else if (dcPercent >= 33)
                {
                    dcScore = 2;
                }
                else if (dcPercent >= 0)
                {
                    dcScore = 1;
                }

                //TODO: logic to get weather score

                totalScore = dcScore + bhScore + weatherScore;

                if (totalScore > 6)
                {
                    ViewBag.riskLevel = "HIGH";
                }
                else if (totalScore > 3)
                {
                    ViewBag.riskLevel = "MEDIUM";
                }
                else if (totalScore > 0) {
                    ViewBag.riskLevel = "LOW";
                }

                //For info in whole singapore
                ViewBag.dengueClusterCases = sgDC;
                ViewBag.breedingCases = sgBH;
                //for info in region
                ViewBag.breedingRegion = regionBH;
                ViewBag.dengueRegion = regionDC;
                //for info in area
                ViewBag.dengueLocation = DengueClustergateway.getCasesLocation(passedWeatherInfo[0]);
                ViewBag.breedingLocation = BHgateway.getCasesLocation(passedWeatherInfo[0]);

                ViewBag.street = passedWeatherInfo[0];
                
                if (passedWeatherInfo[1] == "C")
                {
                    ViewBag.region = "Central";
                    ViewBag.regionNumber = 4;
                }
                else if (passedWeatherInfo[1] == "N")
                {
                    ViewBag.region = "North";
                    ViewBag.regionNumber = 0;
                }
                else if (passedWeatherInfo[1] == "S")
                {
                    ViewBag.region = "South";
                    ViewBag.regionNumber = 1;
                }
                else if (passedWeatherInfo[1] == "E")
                {
                    ViewBag.region = "East";
                    ViewBag.regionNumber = 2;
                }
                else if (passedWeatherInfo[1] == "W")
                {
                    ViewBag.region = "West";
                    ViewBag.regionNumber = 3;
                }

                ViewBag.selectedEvaluateArea = true;
            }
            System.Diagnostics.Debug.WriteLine(weather);




            return View();
        }

        public ActionResult DrawDengueClusterRegionChart(int chartRegion)
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
           
            int[] y = new int[5] {
                DengueClustergateway.getCasesRegion("N"),
                DengueClustergateway.getCasesRegion("S"),
                DengueClustergateway.getCasesRegion("E"),
                DengueClustergateway.getCasesRegion("W"),
                DengueClustergateway.getCasesRegion("C")
            };

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
            chart.Series["Data"].Points[chartRegion]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/DengueClusterRegionChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/DengueClusterRegionChart"), "jpeg");
        }

        public ActionResult DrawBreedingHabitatRegionChart(int chartRegion)
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y = new int[5] {
                BHgateway.getCasesRegion("N"),
                BHgateway.getCasesRegion("S"),
                BHgateway.getCasesRegion("E"),
                BHgateway.getCasesRegion("W"),
                BHgateway.getCasesRegion("C")
            };

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
            chart.Series["Data"].Points[chartRegion]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/BreedingHabitatRegionChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/BreedingHabitatRegionChart"), "jpeg");
        }

        public ActionResult DrawOverallChart(int chartRegion)
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y = new int[5] {
                DengueClustergateway.getCasesRegion("N") + BHgateway.getCasesRegion("N"),
                DengueClustergateway.getCasesRegion("S") + BHgateway.getCasesRegion("S"),
                DengueClustergateway.getCasesRegion("E") + BHgateway.getCasesRegion("E"),
                DengueClustergateway.getCasesRegion("W") + BHgateway.getCasesRegion("W"),
                DengueClustergateway.getCasesRegion("C") + BHgateway.getCasesRegion("C")
            };

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
            chart.Series["Data"].Points[chartRegion]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/OverallChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/OverallChart"), "jpeg");
        }

    }
}