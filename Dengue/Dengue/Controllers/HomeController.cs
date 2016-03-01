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

        IEnumerable<DengueCluster> dengueClusterAll;
        IEnumerable<DengueCaseHistory> dengueCHAll;

        int denguecases = 0;
        int dengueCaseHistory = 0;

        public void getNoCases()
        {
            IEnumerable<DengueCluster> numbercases = DengueClustergateway.SelectAll();
            foreach (DengueCluster dc in numbercases)
            {
                denguecases += dc.No_of_Cases;
            }
            ViewData["noDengueCase"] = denguecases;
        }
        public void Upload()
        {
            IEnumerable<DengueCluster> data = DengueClustergateway.SelectAll();
            foreach (DengueCluster dc in data)
            {
                DengueClustergateway.Delete(dc.DCluster_ID);
            }
            List<string> tempstring = new List<string>();
            List<string> locations = new List<string>();
            List<string> cases = new List<string>();
            List<string> hyperlink = new List<string>();
            List<string> coordinates = new List<string>();
            List<string> dengueCaseHistory = new List<string>();

            //Dengue Cluster
            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/e7536645-6126-4358-b959-a02b22c6c473/resource/c1d04c0e-3926-40bc-8e97-2dfbb1c51c3a/download/DENGUECLUSTER.kml");
            MatchCollection m1 = Regex.Matches(html, @"<td>\s*(.+?)\s*</td>", RegexOptions.Singleline);
            MatchCollection m2 = Regex.Matches(html, @"href=\s*(.+?)\s*>", RegexOptions.Singleline);
            MatchCollection m3 = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);

            //locations and cases
            foreach (Match m in m1)
            {

                string test = m.Groups[1].Value;
                tempstring.Add(test);
            }

            //hyperlink
            foreach (Match m in m2)
            {

                string test = m.Groups[1].Value;
                System.Diagnostics.Debug.WriteLine("before: " + test);
                test = test.Substring(1, test.Length - 2);
                System.Diagnostics.Debug.WriteLine("after: " + test);
                hyperlink.Add(test);
            }

            //coordinates
            foreach (Match m in m3)
            {

                string test = m.Groups[1].Value;
                coordinates.Add(test);
            }

            //cases
            for (int i = 6; i < tempstring.Count; i += 15)
            {
                string newthingy = tempstring[i];
                cases.Add(newthingy);
            }

            //locations
            for (int i = 4; i < tempstring.Count; i += 15)
            {
                string newthingy = tempstring[i];
                locations.Add(newthingy);
            }

            DengueCluster dengueCluster = new DengueCluster();

            for (int j = 0; j < locations.Count; j++)
            {
                dengueCluster.Description = locations[j];
                dengueCluster.No_of_Cases = Int32.Parse(cases[j]);
                dengueCluster.Hyperlink = hyperlink[j];
                dengueCluster.Coordinates = coordinates[j];
                dengueCluster.Alert_Level = "0";
                dengueCluster.Upload_Date = DateTime.Now.Date.ToShortDateString();
                DengueClustergateway.Insert(dengueCluster);
                db.SaveChanges();
            }

            //Dengue Case History
            //dengueCHAll = DengueCHgateway.SelectAll();
            //foreach (DengueCaseHistory dch in dengueCHAll)
            //{
            //    DengueCHgateway.Delete(dch.DCH_ID);
            //}

            //String html2 = web.DownloadString("C:/Users/IzzatLaptop/Desktop/XMLFile/weekly-infectious-bulletin-cases.csv");
            //MatchCollection m4 = Regex.Matches(html2, @"Dengue Fever,\s*(.+?)\s*2016", RegexOptions.Singleline);

            //DengueCaseHistory dengueCH = new DengueCaseHistory();
            //foreach (Match match in m4)
            //{

            //    string test = match.Groups[1].Value;
            //    dengueCaseHistory.Add(test);



            //}
            //for (int k = 1; k < dengueCaseHistory.Count; k++)
            //{
            //    dengueCH.No_of_Cases = Int32.Parse(dengueCaseHistory[k]);
            //    dengueCH.Epi_Week = k;
            //    DengueCHgateway.Insert(dengueCH);
            //    db.SaveChanges();
            //}

        }

        public void getWeatherData()
        {
            IEnumerable<Weather> weatherdata = Weathergateway.SelectAll();
            foreach (Weather wt in weatherdata)
            {
                Weathergateway.Delete(wt.Weather_Id);
            }
            // Step 1: Construct URL
            String nowcastUrl = "http://www.nea.gov.sg/api/WebAPI?dataset=nowcast&keyref=781CF461BB6606AD4AF8F309C0CCE99464076C2CB94375BA";
            String twelvecastUrl = "http://www.nea.gov.sg/api/WebAPI?dataset=12hrs_forecast&keyref=781CF461BB6606AD4AF8F309C0CCE99464076C2CB94375BA";
            // Step 2: Call API Url
            HttpWebRequest nowcastRequest = (HttpWebRequest)WebRequest.Create(nowcastUrl);
            HttpWebRequest twelvecastRequest = (HttpWebRequest)WebRequest.Create(twelvecastUrl);
            try
            {
                HttpWebResponse nowcastResponse = (HttpWebResponse)nowcastRequest.GetResponse();
                HttpWebResponse twelevecastResponse = (HttpWebResponse)twelvecastRequest.GetResponse();

                Stream nowcastReceiveStream = nowcastResponse.GetResponseStream();
                Stream twelevecastReceiveStream = twelevecastResponse.GetResponseStream();

                StreamReader nowcastReadStream = new StreamReader(nowcastReceiveStream, Encoding.UTF8);
                StreamReader twelvecastReadStream = new StreamReader(twelevecastReceiveStream, Encoding.UTF8);

                XDocument nowcastXDoc = XDocument.Load(nowcastReadStream);
                XDocument twelvecastXDoc = XDocument.Load(twelvecastReadStream);

                String twelveForecastEast = (string)twelvecastXDoc.XPathSelectElement("/channel/item/wxeast");
                String twelveForecastWest = (string)twelvecastXDoc.XPathSelectElement("/channel/item/wxwest");
                String twelveForecastNorth = (string)twelvecastXDoc.XPathSelectElement("/channel/item/wxnorth");
                String twelveForecastSouth = (string)twelvecastXDoc.XPathSelectElement("/channel/item/wxsouth");
                String twelveForecastCentral = (string)twelvecastXDoc.XPathSelectElement("/channel/item/wxcentral");

                var areas = nowcastXDoc.XPathSelectElements("/channel/item/weatherForecast/area");
              String issueDate = (string)nowcastXDoc.XPathSelectElement("/channel/item/issue_datentime");

                Weather weather = new Weather();

                foreach (var node in areas)
                {
                    
                    weather.Locations = node.Attribute("name").Value;
                    weather.Forecast = node.Attribute("forecast").Value;
                    weather.Zone = node.Attribute("zone").Value;
                    weather.Issue_Date = issueDate;
                    //if (node.Attribute("zone").Value == "E")
                    //{
                    //    weather.TwelveHrForceast = twelveForecastEast;
                    //}
                    //else if (node.Attribute("zone").Value == "W")
                    //{
                    //    weather.TwelveHrForceast = twelveForecastWest;
                    //}
                    //else if (node.Attribute("zone").Value == "N")
                    //{
                    //    weather.TwelveHrForceast = twelveForecastNorth;
                    //}
                    //else if (node.Attribute("zone").Value == "S")
                    //{
                    //    weather.TwelveHrForceast = twelveForecastSouth;
                    //}
                    //else if (node.Attribute("zone").Value == "C")
                    //{
                    //    weather.TwelveHrForceast = twelveForecastCentral;
                    //}

                    Weathergateway.Insert(weather);
                    db.SaveChanges();
                    //System.Diagnostics.Debug.WriteLine(weather.Locations);
                    //System.Diagnostics.Debug.WriteLine(weather.Forecast);
                    //System.Diagnostics.Debug.WriteLine(weather.Zone);
                    //System.Diagnostics.Debug.WriteLine(weather.Issue_Date);
                }

                //System.Diagnostics.Debug.WriteLine("Response stream received.");
                //System.Diagnostics.Debug.WriteLine(readStream.ReadToEnd());
            }
            catch (WebException we)
            {
                // Step 2b: If response status != 200
                Stream receiveStream = we.Response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                // print the error received from Server
                Console.WriteLine("Error Encountered - ");
                System.Diagnostics.Debug.WriteLine("Error Encountered - ");
                Console.WriteLine(readStream.ReadToEnd());
            }
            catch (NullReferenceException)
            {

            }

            return;
        }

        // GET: DengueClusters
        public ActionResult Index()
        {

            Upload();
            getNoCases();
            getWeatherData();
            
            //ViewData["noDengueCase"] = denguecases;

            //return View(DengueClustergateway.SelectAll());

            return View();
        }

        // GET: DengueClusters
        public ActionResult Case(string search,string sortCases)
        {
            getNoCases();
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
            getNoCases();
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