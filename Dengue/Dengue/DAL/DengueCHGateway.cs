using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dengue.Models;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Net;

namespace Dengue.DAL
{
    public class DengueCHGateway : DataGateway<DengueCaseHistory>
    {
        private DataGateway<DengueCaseHistory> DengueCHgateway = new DataGateway<DengueCaseHistory>();
        public void uploadDengueCH()
        {
            //Dengue Case History
            //IEnumerable<DengueCaseHistory> data = DengueCHgateway.SelectAll();

            //     var e = data.Last();
            //    int databaseDate = e.Epi_Week;

            List<string> dengueCaseHistory = new List<string>();
            IEnumerable<DengueCaseHistory> dengueCHAll = DengueCHgateway.SelectAll();
            foreach (DengueCaseHistory dch in dengueCHAll)
            {
                DengueCHgateway.Delete(dch.DCH_ID);
            }

            WebClient web = new WebClient();
            String html2 = web.DownloadString("https://data.gov.sg/dataset/e51da589-b2d7-486b-adfc-4505d47e1206/resource/ef7e44f1-9b14-4680-a60a-37d2c9dda390/download/weekly-infectious-bulletin-cases.csv");
            MatchCollection m4 = Regex.Matches(html2, @"Dengue Fever,\s*(.+?)\s*2016", RegexOptions.Singleline);

            DengueCaseHistory dengueCH = new DengueCaseHistory();
            foreach (Match match in m4)
            {

                string test = match.Groups[1].Value;
                dengueCaseHistory.Add(test);



            }
            for (int k = 1; k < dengueCaseHistory.Count; k++)
            {
                dengueCH.No_of_Cases = Int32.Parse(dengueCaseHistory[k]);
                dengueCH.Epi_Week = k;
                DengueCHgateway.Insert(dengueCH);
                db.SaveChanges();
            }
        }
        public int getTotalNoCases()
        {
            int denguecases = 0;
            IEnumerable<DengueCaseHistory> numbercases = DengueCHgateway.SelectAll();
            foreach (DengueCaseHistory dc in numbercases)
            {
                denguecases += dc.No_of_Cases;
            }
            return denguecases;
        }

        public int getWeekNoCases()
        {
            int denguecases = 0;
            IEnumerable<DengueCaseHistory> numbercases = DengueCHgateway.SelectAll();


            denguecases = numbercases.Last().No_of_Cases;

            return denguecases;
        }
    }
}