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

        public void uploadDengueCluster()
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

            //String html2 = web.DownloadString("https://data.gov.sg/dataset/e51da589-b2d7-486b-adfc-4505d47e1206/resource/ef7e44f1-9b14-4680-a60a-37d2c9dda390/download");
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


    }


}