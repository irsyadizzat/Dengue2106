using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dengue.Models;
using System.Data.Entity;
using System.Net;
using System.Text.RegularExpressions;

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

        public void uploadDengueCluster(String[][] passedRegionArray)
        {
     //       string webDate = getDate();

            IEnumerable<DengueCluster> data = DengueClustergateway.SelectAll();

       //     var e = data.First();
        //    string databaseDate = e.Upload_Date;

            List<string> tempstring = new List<string>();
            List<string> locations = new List<string>();
            List<string> cases = new List<string>();
            List<string> hyperlink = new List<string>();
            List<string> coordinates = new List<string>();


        //    if (!webDate.Equals(databaseDate))
         //   {
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
                    dengueCluster.No_of_Cases = Int32.Parse(cases[j]);
                    dengueCluster.Hyperlink = hyperlink[j];
                    dengueCluster.Coordinates = coordinates[j];
                    dengueCluster.Alert_Level = "0";
                    dengueCluster.Upload_Date = "test";
                    for (int row = 0; row < passedRegionArray.GetLength(0); row++)
                    {
                        for (int col = 0; col == passedRegionArray.GetLength(1); col++)
                        {
                            dengueCluster.zone = passedRegionArray[row][col];
                            dengueCluster.location = passedRegionArray[row][col + 1];
                           
                        }
                    }
                
                    DengueClustergateway.Insert(dengueCluster);
                    db.SaveChanges();
                }
           // }


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

    }


}