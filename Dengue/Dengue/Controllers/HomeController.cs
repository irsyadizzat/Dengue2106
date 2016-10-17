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
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Dengue.Controllers
{
    public class HomeController : Controller
    {
        ResultModel rM = new ResultModel();
        private DengueClusterGateway DengueClustergateway = new DengueClusterGateway();
        private WeatherGateway Weathergateway = new WeatherGateway();
        private DengueContext db = new DengueContext();
        private DengueCHGateway DengueCHgateway = new DengueCHGateway();
        private BHGateway BHgateway = new BHGateway();


        // GET: DengueClusters
        public ActionResult Index()
        {
            //ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            //Weathergateway.getWeatherData();
            //DengueCHgateway.uploadDengueCH();
            //BHgateway.uploadBreedingHabitat();
            //BHgateway.getDate();

            //ViewBag.dengueHistory = DengueCHgateway.SelectAll();
            //IEnumerable<DengueCaseHistory> dengueHistory = DengueCHgateway.SelectAll();   

            return View();
        }

        public ActionResult ContactUs()
        {
            ViewBag.contactedUs = "false";

            ViewBag.Message = "Your contact page.";
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(string name, string contactNo, string email, string description)
        {
            ViewBag.contactedUs = "true";
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

        public ActionResult GenerateStatistics() {

            List<SelectListItem> topicList = new List<SelectListItem>();
            topicList.Add(new SelectListItem() { Text = "Dengue History", Value = "Dengue History"});
            topicList.Add(new SelectListItem() { Text = "Dengue Clusters", Value = "Dengue Clusters" });
            topicList.Add(new SelectListItem() { Text = "Breeding Habitat", Value = "Breeding Habitat" });
            ViewBag.DataTopic = topicList;

            return View();
        }

        [HttpPost]
        public ActionResult GenerateStatistics(string DataTopic)
        {           

            double mean = 0, variance = 0, sd = 0;
            double count = 0;
            double total = 0;
            StatisticsModel sM = new StatisticsModel();
            List<double> data = new List<double> {};

            if (DataTopic == "Dengue History")
            {
                IEnumerable<DengueCaseHistory> dengueHistory = DengueCHgateway.SelectAll();
                count = dengueHistory.Count();
                foreach (DengueCaseHistory DCH in dengueHistory)
                {
                    total += DCH.No_of_Cases;
                    data.Add(DCH.No_of_Cases);
                }

                mean = Math.Round(data.Mean(),2);
                variance = Math.Round(data.Variance(),2);
                sd = Math.Round(data.StandardDeviation(),2);
            }
            else if (DataTopic == "Dengue Clusters")
            {
                IEnumerable<DengueCluster> dengueCluster = DengueClustergateway.SelectAll();
                count = dengueCluster.Count();

                foreach (DengueCluster DC in dengueCluster)
                {
                    total += DC.No_of_Cases;
                    data.Add(DC.No_of_Cases);
                }

                mean = Math.Round(data.Mean(), 2);
                variance = Math.Round(data.Variance(), 2);
                sd = Math.Round(data.StandardDeviation(), 2);
            }
            else
            {
                IEnumerable<BreedingHabitat> breedingHabitat = BHgateway.SelectAll();
                count = breedingHabitat.Count();

                foreach (BreedingHabitat bh in breedingHabitat)
                {
                    total += bh.No_of_Cases;
                    data.Add(bh.No_of_Cases);
                }

                mean = Math.Round(data.Mean(), 2);
                variance = Math.Round(data.Variance(), 2);
                sd = Math.Round(data.StandardDeviation(), 2);
            }

            sM.topic = DataTopic;
            sM.total = total;
            sM.count = count;
            sM.mean = mean;
            sM.variance = variance;
            sM.sd = sd;

            return View(sM);
        }

        public ActionResult DrawLineChart(int height)
        {
            IEnumerable<DengueCaseHistory> dengueHistory = DengueCHgateway.SelectAll();
            int count = dengueHistory.Count();
            int k = 0;
            string[] x = new string[count];

            int[] y = new int[count];

            foreach (DengueCaseHistory DCH in dengueHistory)
            {
                x[k] = "W"+DCH.Epi_Week.ToString();
                y[k] = DCH.No_of_Cases;
                k++;
            }

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = height;
            chart.Titles.Add("Dengue Case History (as of 01/01/2016)");

            ChartArea ca = new ChartArea();
            ca.AxisX.Title = "Week";
            ca.AxisY.Title = "No. of Dengue Cases";
            chart.ChartAreas.Add(ca);

            Series dataS = new Series("Data");

            dataS.ChartType = SeriesChartType.Line;

            for (int i = 0; i < x.Length; i++)
            {
                dataS.Points.AddXY(x[i], y[i]);
                dataS.Label = "#VALY";
            }

            chart.Series.Add(dataS);
            chart.Series["Data"].IsValueShownAsLabel = true;

            chart.SaveImage(Server.MapPath("~/Content/LineChart"), ChartImageFormat.Jpeg);
            return base.File(Server.MapPath("~/Content/LineChart"), "jpeg");
        }

        public ActionResult EvaluateArea(string weather)
        {
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();
            string[] passedWeatherInfo;
            //0 = street name, 1 = zone, 2 = forecast

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
                    passedWeatherInfo = weather.Split(';');
                    if (w.Zone == passedWeatherInfo[1]) {
                        noOfLocationInZone++;
                        rM.noOfLocationInZone = noOfLocationInZone;
                       // ViewBag.noOfLocationInZone = noOfLocationInZone;
                    }
                }
            }
            ViewBag.Weather = weatherLocationList;

            if (weather == null)
            {
                rM.selectedEvaluateArea = false;
               // ViewBag.selectedEvaluateArea = false;
            }
            else
            {
                passedWeatherInfo = weather.Split(';');
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
                    rM.risklevel = "HIGH";
                  //  ViewBag.riskLevel = "HIGH";
                }
                else if (totalScore > 4)
                {
                    rM.risklevel = "MEDIUM";
                   // ViewBag.riskLevel = "MEDIUM";
                }
                else if (totalScore <= 4) {
                    rM.risklevel = "LOW";
                  //  ViewBag.riskLevel = "LOW";
                }

                //For info in whole singapore
                rM.dengueClusterCases = sgDC;
                rM.breedingCases = sgBH;
                //ViewBag.dengueClusterCases = sgDC;
                //ViewBag.breedingCases = sgBH;

                //for info in region
                rM.breedingRegion = regionBH;
                rM.dengueRegion = regionDC;
                // ViewBag.breedingRegion = regionBH;
                // ViewBag.dengueRegion = regionDC;

                //for info in area
                rM.dengueLocation = locationDC;
                rM.breedingLocation = locationBH;
               // ViewBag.dengueLocation = locationDC;
              //  ViewBag.breedingLocation = locationBH;

                //set street name
                rM.street = passedWeatherInfo[0];
               // ViewBag.street = passedWeatherInfo[0];
                //set region name and region number
                convertShortToLongZone(passedWeatherInfo[1]);

                //to check if a value is chosen
                rM.selectedEvaluateArea = true;
                rM.areaWeather = passedWeatherInfo[2];
               // ViewBag.selectedEvaluateArea = true;
               // ViewBag.areaWeather = passedWeatherInfo[2];
            }
            System.Diagnostics.Debug.WriteLine(weather);

            return View(rM);
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
                rM.region = "Central";
                rM.regionNumber = 4;
               // ViewBag.region = "Central";
              //  ViewBag.regionNumber = 4;
            }
            else if (zone == "N")
            {
                rM.region = "North";
                rM.regionNumber = 0;
                //ViewBag.region = "North";
                //ViewBag.regionNumber = 0;
            }
            else if (zone == "S")
            {
                rM.region = "South";
                rM.regionNumber = 1;
                //ViewBag.region = "South";
                //ViewBag.regionNumber = 1;
            }
            else if (zone == "E")
            {
                rM.region = "East";
                rM.regionNumber = 2;
                //ViewBag.region = "East";
                //ViewBag.regionNumber = 2;
            }
            else if (zone == "W")
            {
                rM.region = "West";
                rM.regionNumber = 3;
                //ViewBag.region = "West";
                //ViewBag.regionNumber = 3;
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

            string[] x = new string[2] { "Other Areas in " + zone, street  };
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



        public ActionResult DrawOverallChart(int chartRegion, string type, int height)
        {
            string[] x = new string[5] { "North", "South", "East", "West", "Central" };
            int[] y;

            Chart chart = new Chart();
            chart.Width = 360;
            chart.Height = height;

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
            if (chartRegion != 99)
            {
                chart.Series["Data"].Points[chartRegion]["Exploded"] = "True";
            }

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

        public ActionResult WeatherPartial() {
            ViewBag.DayForecast = weatherForecast();
            return PartialView();
        }
        public ActionResult WeekCasesPartial()
        {
            ViewBag.WeekCases = DengueCHgateway.getWeekNoCases();
            return PartialView();
        }
        public ActionResult TotalCasesPartial()
        {
            ViewBag.TotalCases = DengueCHgateway.getTotalNoCases();
            return PartialView();
        }
        public ActionResult EvaluateDropdown()
        {
            setWeatherDropdown();
            return PartialView();
        }
        public string weatherForecast()
        {
            String url = "http://www.nea.gov.sg/api/WebAPI/?dataset=24hrs_forecast&keyref=781CF461BB6606AD4AF8F309C0CCE9941C98AB71D91D487D";
            HttpWebRequest urlRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse urlResponse = (HttpWebResponse)urlRequest.GetResponse();
            Stream urlReceiveStream = urlResponse.GetResponseStream();
            StreamReader urlReadStream = new StreamReader(urlReceiveStream, Encoding.UTF8);
            XDocument urlXDoc = XDocument.Load(urlReadStream);

            return (string)urlXDoc.XPathSelectElement("/channel/main/forecast");
        }
    }

    public static class ListExtension
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }
    }
}