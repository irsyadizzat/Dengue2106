﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dengue.Models;
using System.Data.Entity;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace Dengue.DAL
{
    public class DengueClusterGateway : DataGateway<DengueCluster>
    {
        private DataGateway<DengueCluster> DengueClustergateway = new DataGateway<DengueCluster>();
        public int getNoCases()
        {
            int denguecases = 0;
            IEnumerable<DengueCluster> numbercases = DengueClustergateway.SelectAll();
            foreach (DengueCluster dc in numbercases)
            {
                denguecases += dc.No_of_Cases;
            }
            return denguecases;
        }

        public int getCasesRegion(string region)
        {
            int denguecases = 0;

            IEnumerable<DengueCluster> numbercases = DengueClustergateway.SelectAll();
            region = region.ToUpper();

            numbercases = numbercases.Where(d => d.zone.ToUpper().Contains(region));

            foreach (DengueCluster dc in numbercases)
            {
                denguecases += dc.No_of_Cases;
            }
            return denguecases;
        }

        public int getCasesLocation(string location)
        {
            int denguecases = 0;

            IEnumerable<DengueCluster> numbercases = DengueClustergateway.SelectAll();
            location = location.ToUpper();

            numbercases = numbercases.Where(d => d.location.ToUpper().Contains(location));

            foreach (DengueCluster dc in numbercases)
            {
                denguecases += dc.No_of_Cases;
            }
            return denguecases;
        }

        internal IEnumerable<DengueCluster> Order(string sortOrder, IEnumerable<DengueCluster> DengueCluster)
        {
            DengueCluster = from t in SelectAll()
                    select t;
            switch (sortOrder)
            {

                case "case_desc":

                    DengueCluster = DengueCluster.OrderByDescending(d => d.No_of_Cases);

                    break;
                case "default":
                    DengueCluster = DengueCluster.OrderBy(d => d.No_of_Cases);

                    break;
            }
            return DengueCluster.ToList();

        }

        internal IEnumerable<DengueCluster> searchLocation(string location)
        {

            IEnumerable<DengueCluster> DengueCluster = DengueClustergateway.SelectAll();
            location = location.ToUpper();
            DengueCluster = DengueCluster.Where(d => d.location.ToUpper().Contains(location));

            return DengueCluster.ToList();

        }


        internal IEnumerable<DengueCluster> searchRegion(string region)
        {

            IEnumerable<DengueCluster> DengueCluster = DengueClustergateway.SelectAll();
            region = region.ToUpper();
            DengueCluster = DengueCluster.Where(d => d.zone.ToUpper().Contains(region));

            return DengueCluster.ToList();

        }
        public void uploadDengueCluster()
        {
                   string webDate = getDate();

            IEnumerable<DengueCluster> data = DengueClustergateway.SelectAll();

                 var e = data.First();
                string databaseDate = e.Upload_Date;

            List<string> tempstring = new List<string>();
            List<string> locations = new List<string>();
            List<string> cases = new List<string>();
            List<string> hyperlink = new List<string>();
            List<string> coordinates = new List<string>();

            List<string> realLocation = getlocation();
            List<string> region = coordinatesToRegion();
                if (!webDate.Equals(databaseDate))
              {
            //Remove data before inserting
            foreach (DengueCluster dc in data)
            {
                DengueClustergateway.Delete(dc.DCluster_ID);
            }

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
             //   dengueCluster.location = "temp";
                dengueCluster.No_of_Cases = Int32.Parse(cases[j]);
                dengueCluster.Hyperlink = hyperlink[j];
                dengueCluster.location = realLocation[j];
                dengueCluster.zone = region[j];
                dengueCluster.Coordinates = coordinates[j];
                dengueCluster.Alert_Level = "0";
                dengueCluster.Upload_Date = webDate;
                DengueClustergateway.Insert(dengueCluster);
                db.SaveChanges();
            }
             }


        }
        public string getDate()
        {
            int i = 0;
            string webDate = "new";


            //Dengue Cluster
            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/dengue-clusters");
            MatchCollection m1 = Regex.Matches(html, @"<td\s*(.+?)\s*</td>", RegexOptions.Singleline);


            //locations and cases
            foreach (Match m in m1)
            {

                if (i == 2)
                {
                    webDate = m.Groups[1].Value;
                    webDate = webDate.Remove(0, 24);
                    webDate = webDate.Remove(13, 7);
                }

                i++;
            }
            

            return webDate;

        }

        public List<string> getLongitude()
        {
            List<string> longitude = new List<string>();

            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/e7536645-6126-4358-b959-a02b22c6c473/resource/c1d04c0e-3926-40bc-8e97-2dfbb1c51c3a/download/DENGUECLUSTER.kml");

            MatchCollection m3 = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);

            //coordinates
            foreach (Match m in m3)
            {

                string test = m.Groups[1].Value;
                string[] test2 = test.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(test2[0]);
            }
            return longitude;
        }

        public List<string> getLatitude()
        {

            List<string> latitude = new List<string>();
            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/e7536645-6126-4358-b959-a02b22c6c473/resource/c1d04c0e-3926-40bc-8e97-2dfbb1c51c3a/download/DENGUECLUSTER.kml");

            MatchCollection m3 = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);

            //coordinates
            foreach (Match m in m3)
            {

                string test = m.Groups[1].Value;
                string[] test2 = test.Split(new string[] { "," }, StringSplitOptions.None);
                latitude.Add(test2[1]);
            }
            return latitude;
        }

        public List<string> getlocation()
        {
            List<string> longitude = getLongitude();
            List<string> latitude = getLatitude();

            List<string> location = new List<string>();
            for(int i = 0; i < longitude.Count(); i++)
            {
                String url = "https://maps.googleapis.com/maps/api/geocode/xml?latlng=" + latitude[i] + "," + longitude[i];
                HttpWebRequest urlRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse urlResponse = (HttpWebResponse)urlRequest.GetResponse();
                Stream urlReceiveStream = urlResponse.GetResponseStream();
                StreamReader urlReadStream = new StreamReader(urlReceiveStream, Encoding.UTF8);
                XDocument urlXDoc = XDocument.Load(urlReadStream);

                location.Add((string)urlXDoc.XPathSelectElement("/GeocodeResponse/result/formatted_address"));

            }
            



            return location;
        }

        public List<string> coordinatesToRegion()
        {

            List<string> longitude = getLongitude();
            List<string> latitude = getLatitude();
            List<string> region = new List<string>();

            for (int i = 0; i < longitude.Count(); i++)
            {

           
            //right side of sg
            if (double.Parse(longitude[i]) > 103.895000)
            {
                region.Add("E");
            }
            //left side of sg
            else if (double.Parse(longitude[i]) < 103.770000)
            {
               region.Add("W");
             
            }
            //top part of sg
            else if (double.Parse(latitude[i]) > 1.390000)
            {
               region.Add("N");
            }
            //bottom part of sg
            else if (double.Parse(latitude[i]) < 1.315)
            {
                region.Add("S");
            }
            //central of sg
            else
            {
                region.Add("C");
            }

            }
            return region;
        }
    }



}