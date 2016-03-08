using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dengue.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace Dengue.DAL
{
    public class WeatherGateway : DataGateway<Weather>
    {
        public void getWeatherData()
        {
            DataGateway<Weather> Weathergateway = new DataGateway<Weather>();
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
                //Stream receiveStream = we.Response.GetResponseStream();
                //StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                //// print the error received from Server
                //Console.WriteLine("Error Encountered - ");
                //System.Diagnostics.Debug.WriteLine("Error Encountered - ");
                //Console.WriteLine(readStream.ReadToEnd());
            }
            catch (NullReferenceException)
            {

            }

            return;
        }

    }
}