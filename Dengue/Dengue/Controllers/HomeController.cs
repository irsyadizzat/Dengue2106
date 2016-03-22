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
            setWeatherDropdown();

            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            //Weathergateway.getWeatherData();
            //DengueCHgateway.uploadDengueCH();
            //BHgateway.uploadBreedingHabitat();
            //BHgateway.getDate();

            //ViewBag.dengueHistory = DengueCHgateway.SelectAll();
            //IEnumerable<DengueCaseHistory> dengueHistory = DengueCHgateway.SelectAll();
           

            return View();
        }
        public ActionResult DrawDengueHistoryChart()
        {
            IEnumerable<DengueCaseHistory> dengueHistory = DengueCHgateway.SelectAll();
            int count = dengueHistory.Count();
            int k = 0;
            string[] x = new string[count];

            int[] y = new int[count];

            foreach (DengueCaseHistory DCH in dengueHistory)
            {
                x[k] = DCH.Epi_Week.ToString();
                y[k] = DCH.No_of_Cases;
                k++;
            }

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 270;

            Title t = new Title();
            t.Text = "Dengue Cases History Chart (as of 01/01/2016)";

            chart.Titles.Add(t);

            ChartArea ca = new ChartArea();
            ca.AxisX.Title = "Week";
            ca.AxisY.Title = "Number of Cases";
            chart.ChartAreas.Add(ca);

            Series dataS = new Series("Data");

            dataS.ChartType = SeriesChartType.Column;

            for (int i = 0; i < x.Length; i++)
            {
                dataS.Points.AddXY(x[i], y[i]);
                dataS.Label = "#VALY";
            }

            chart.Series.Add(dataS);
            // chart.Series["Data"].Points[chartRegion]["Exploded"] = "True";
            
            // chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            // chart.Legends["Location"].Docking = Docking.Top;



            chart.SaveImage(Server.MapPath("~/Content/DengueClusterRegionChart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/DengueClusterRegionChart"), "jpeg");
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

            setWeatherDropdown();

            return View(dengueClusterAll);
        }

        // GET: BreedingHabitat
        public ActionResult bHCase(string search, string sortCases)
        {
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();


            IEnumerable<BreedingHabitat> bH = BHgateway.SelectAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToUpper();
                bH = bH.Where(d => d.Location.ToUpper().Contains(search));
            }
            else
            {
                ViewBag.PriceSortParm = String.IsNullOrEmpty(sortCases) ? "case_desc" : "";
                switch (sortCases)
                {

                    case "case_desc":
                        bH = BHgateway.Order("case_desc", bH);
                        break;
                    default:
                        bH = BHgateway.Order("default", bH);

                        break;
                }
            }

            setWeatherDropdown();

            return View(bH);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.contactedUs = "false";
            setWeatherDropdown();

            ViewBag.Message = "Your contact page.";
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(string name, string contactNo, string email, string description)
        {
            ViewBag.contactedUs = "true";
            setWeatherDropdown();
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
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            //0 = street name, 1 = zone, 2 = forecast
            string[] passedWeatherInfo = weather.Split(';');
            int noOfLocationInZone = 0;
            Boolean jurongAdded = false;

            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();

            foreach (Weather w in weatherAll)
            {
                if (w.Locations == "JURONG EAST/WEST" || w.Locations == "JURONG INDUSTRIAL ESTATE" || w.Locations == "JURONG ISLAND")
                {
                    if (jurongAdded == false)
                    {
                        weatherLocationList.Add(new SelectListItem() { Text = "JURONG", Value = "JURONG;" + w.Zone + ";" + w.Forecast });
                        jurongAdded = true;
                    }
                }
                else if (w.Locations == "CITY")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "ORCHARD", Value = "ORCHARD;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations == "MACRITCHIE RESERVOIR")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "MACRITCHIE", Value = "MACRITCHIE;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations != "PULAU TEKONG" || w.Locations != "PULAU UBIN")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
                }

                if (weather != null) {
                    if (w.Zone == passedWeatherInfo[1]) {
                        noOfLocationInZone++;
                        ViewBag.noOfLocationInZone = noOfLocationInZone;
                    }
                }
            }
            ViewBag.Weather = weatherLocationList;

            if (weather == null)
            {
                ViewBag.selectedEvaluateArea = false;
            }
            else
            {
                //logic to get risk level
                int dcLocationScore = 0, bhLocationScore = 0, 
                    dcRegionScore = 0, bhRegionScore = 0, 
                    weatherScore = 0, totalScore = 0;
                double sgBH = BHgateway.getNoCases();
                double sgDC = DengueClustergateway.getNoCases();
                double regionBH = BHgateway.getCasesRegion(passedWeatherInfo[1]);
                double regionDC = DengueClustergateway.getCasesRegion(passedWeatherInfo[1]);
                double locationBH = BHgateway.getCasesLocation(passedWeatherInfo[0]);
                double locationDC = DengueClustergateway.getCasesLocation(passedWeatherInfo[0]);

                dcLocationScore = computeLocationScore(locationDC, regionDC, noOfLocationInZone);
                bhLocationScore = computeLocationScore(locationBH, regionBH, noOfLocationInZone);
                dcRegionScore = computeRegionScore(regionDC, sgDC);
                bhRegionScore = computeRegionScore(regionBH, sgBH);
                weatherScore = computeWeatherScore(passedWeatherInfo[2]);

                System.Diagnostics.Debug.WriteLine("dcLocationScore: " + dcLocationScore);
                System.Diagnostics.Debug.WriteLine("bhLocationScore: " + bhLocationScore);
                System.Diagnostics.Debug.WriteLine("dcScore: "+ dcRegionScore);
                System.Diagnostics.Debug.WriteLine("bhScore: " + bhRegionScore);
                System.Diagnostics.Debug.WriteLine("weatherScore: " + weatherScore);

                totalScore = dcLocationScore + bhLocationScore + dcRegionScore + bhRegionScore + weatherScore;

                if (totalScore >= 8)
                {
                    ViewBag.riskLevel = "HIGH";
                }
                else if (totalScore > 4)
                {
                    ViewBag.riskLevel = "MEDIUM";
                }
                else if (totalScore <= 4) {
                    ViewBag.riskLevel = "LOW";
                }

                //For info in whole singapore
                ViewBag.dengueClusterCases = sgDC;
                ViewBag.breedingCases = sgBH;
                //for info in region
                ViewBag.breedingRegion = regionBH;
                ViewBag.dengueRegion = regionDC;
                //for info in area
                ViewBag.dengueLocation = locationDC;
                ViewBag.breedingLocation = locationBH;

                //set street name
                ViewBag.street = passedWeatherInfo[0];
                //set region name and region number
                convertShortToLongZone(passedWeatherInfo[1]);

                //to check if a value is chosen
                ViewBag.selectedEvaluateArea = true;
            }
            System.Diagnostics.Debug.WriteLine(weather);

            return View();
        }

        public int computeWeatherScore(string weatherAbb) {
            int score = 0;

            weatherAbb = weatherAbb.Trim();

            if (weatherAbb == "Fair (Day)" || weatherAbb == "Fair (Night)" || weatherAbb == "Hazy")
            {
                score = 1;
            }
            else if (weatherAbb == "Partly Cloudy" || weatherAbb == "Cloudy" || weatherAbb == "Windy")
            {
                System.Diagnostics.Debug.WriteLine("hello");
                score = 2;
            }
            else if (weatherAbb == "Rain" || weatherAbb == "Passing Showers" || weatherAbb == "Showers" || weatherAbb == "Thunder Showers") {
                score = 3;
            }

            return score;
        }

        public int computeRegionScore(double baseValue, double comparedValue) {
            int score = 0;

            double average = 0;

            average = comparedValue / 5;

            if (baseValue > average) {
                score = 1;
            }

            return score;
        }

        public int computeLocationScore(double baseValue, double comparedValue, double noOfLocationInZone) {
            int score = 0;

            double average = 0, qtrAmt = 0;

            System.Diagnostics.Debug.WriteLine("baseValue: " + baseValue);
            System.Diagnostics.Debug.WriteLine("comparedValue: " + comparedValue);
            System.Diagnostics.Debug.WriteLine("noOfLocationInZone: " + noOfLocationInZone);

            //get average and lower quartile
            average = comparedValue / noOfLocationInZone;
            qtrAmt = 0.25 * average;

            if (baseValue >= average)
            {
                score = 3;
            }
            else if (baseValue >= qtrAmt)
            {
                score = 2;
            }
            else if (baseValue < qtrAmt)
            {
                score = 1;
            }

            return score;
        }

        public void convertShortToLongZone(string zone) {

            if (zone == "C")
            {
                ViewBag.region = "Central";
                ViewBag.regionNumber = 4;
            }
            else if (zone == "N")
            {
                ViewBag.region = "North";
                ViewBag.regionNumber = 0;
            }
            else if (zone == "S")
            {
                ViewBag.region = "South";
                ViewBag.regionNumber = 1;
            }
            else if (zone == "E")
            {
                ViewBag.region = "East";
                ViewBag.regionNumber = 2;
            }
            else if (zone == "W")
            {
                ViewBag.region = "West";
                ViewBag.regionNumber = 3;
            }

            return;
        }

        public string convertLongToShortZone(int regionNumber)
        {
            string zone = "";

            if (regionNumber == 0)
            {
                zone = "North";
            }
            else if (regionNumber == 1)
            {
                zone = "South";
            }
            else if (regionNumber == 2)
            {
                zone = "East";
            }
            else if (regionNumber == 3)
            {
                zone = "West";
            }
            else if (regionNumber == 4)
            {
                zone = "Central";
            }

            return zone;
        }

        public ActionResult DrawRegionChart(int chartRegion, int noOfLocationInZone, string street, string type)
        {
            string zone = convertLongToShortZone(chartRegion);

            string[] x = new string[2] { street, "Other Areas in "+zone };
            int[] y;

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 455;

            Title t = new Title();

            int noOfCaseLocation, noOfRegionLocation, result;

            if (type == "DengueCluster")
            {
                noOfCaseLocation = DengueClustergateway.getCasesLocation(street);
                noOfRegionLocation = DengueClustergateway.getCasesRegion(zone.Substring(0, 1));
                result = noOfRegionLocation - noOfCaseLocation;

                y = new int[2] {
                    result,
                    noOfCaseLocation
                };
                t.Text = "Dengue Cluster Chart (In Region)";
            }
            else {

                noOfCaseLocation = BHgateway.getCasesLocation(street);
                noOfRegionLocation = BHgateway.getCasesRegion(zone.Substring(0, 1));
                result = noOfRegionLocation - noOfCaseLocation;

                y = new int[2] {
                    result,
                    noOfCaseLocation
                };
                t.Text = "Breeding Habitat Chart (In Region)";
            }            

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
            chart.Series["Data"].Points[0]["Exploded"] = "True";

            //chart.Legends.Add(new Legend("Location"));
            //chart.Series["Data"].Legend = "Location";
            //chart.Legends["Location"].Docking = Docking.Top;

            chart.SaveImage(Server.MapPath("~/Content/"+ type +"Chart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/" + type + "Chart"), "jpeg");
        }

        public ActionResult DrawOverallChart(int chartRegion, string type)
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y;

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = 455;

            Title t = new Title();

            if (type == "DengueCluster")
            {
                y = new int[5] {
                    DengueClustergateway.getCasesRegion("N"),
                    DengueClustergateway.getCasesRegion("S"),
                    DengueClustergateway.getCasesRegion("E"),
                    DengueClustergateway.getCasesRegion("W"),
                    DengueClustergateway.getCasesRegion("C")
                };
                t.Text = "Dengue Cluster Chart (In Singapore)";
            }
            else
            {
                y = new int[5] {
                    BHgateway.getCasesRegion("N"),
                    BHgateway.getCasesRegion("S"),
                    BHgateway.getCasesRegion("E"),
                    BHgateway.getCasesRegion("W"),
                    BHgateway.getCasesRegion("C")
                };
                t.Text = "Breeding Habitat Chart (In Singapore)";
            }
            
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

            chart.SaveImage(Server.MapPath("~/Content/Overall"+ type + "Chart"), ChartImageFormat.Jpeg);
            // Return the contents of the Stream to the client
            return base.File(Server.MapPath("~/Content/Overall" + type + "Chart"), "jpeg");
        }

        public void setWeatherDropdown() {
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();

            Boolean jurongAdded = false;

            foreach (Weather w in weatherAll)
            {
                if (w.Locations == "JURONG EAST/WEST" || w.Locations == "JURONG INDUSTRIAL ESTATE" || w.Locations == "JURONG ISLAND")
                {
                    if (jurongAdded == false)
                    {
                        weatherLocationList.Add(new SelectListItem() { Text = "JURONG", Value = "JURONG;" + w.Zone + ";" + w.Forecast });
                        jurongAdded = true;
                    }
                }
                else if(w.Locations == "HOLLAND VILLAGE")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "HOLLAND VILLAGE", Value = "HOLLAND;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations == "CITY")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "ORCHARD", Value = "ORCHARD;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations == "MACRITCHIE RESERVOIR") {
                    weatherLocationList.Add(new SelectListItem() { Text = "MACRITCHIE", Value = "MACRITCHIE;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations != "PULAU TEKONG" && w.Locations != "PULAU UBIN" && w.Locations != "PEIRCE RESERVOIR" && w.Locations != "SOUTHERN ISLAND")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
                }
            }

            ViewBag.Weather = weatherLocationList;

            return;
        }

    }
}