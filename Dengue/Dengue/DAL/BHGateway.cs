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
    public class BHGateway : DataGateway<BreedingHabitat>
    {
        private DataGateway<BreedingHabitat> BHgateway = new DataGateway<BreedingHabitat>();
        List<string> coordinates = new List<string>();
        List<string> longitude = new List<string>();
        List<string> latitude = new List<string>();
        List<string> cases = new List<string>();

        public void uploadBreedingHabitat(String[] passedHabitatArray)
        {
       //     string webDate = getDate();

            IEnumerable<BreedingHabitat> data = BHgateway.SelectAll();

         //   var e = data.First();
         //   string databaseDate = e.Upload_Date;




          //  if (!webDate.Equals(databaseDate))
          //  {
                //Remove data before inserting
                foreach (BreedingHabitat BH in data)
                {
                BHgateway.Delete(BH.BH_ID);
                }

            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/9f05bb14-a9df-4596-b8a2-cdf821b788c6/download");

            MatchCollection cCentral = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            MatchCollection nCentral = Regex.Matches(html, @"<name>Aedes Mosquito Breeding Habitats :\s*(.+?)\s*</name>", RegexOptions.Singleline);

            String html2 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/ed953998-4ee2-4a2a-bb18-3e61fc014afd/download");

            MatchCollection cNorthEast = Regex.Matches(html2, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            MatchCollection nNorthEast = Regex.Matches(html2, @"<name>Aedes Mosquito Breeding Habitats :\s*(.+?)\s*</name>", RegexOptions.Singleline);


            String html3 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/3c03a902-6d95-47e6-9dc1-5378ac8425c5/download");

            MatchCollection cNorthWest = Regex.Matches(html3, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            MatchCollection nNorthWest = Regex.Matches(html3, @"<name>Aedes Mosquito Breeding Habitats :\s*(.+?)\s*</name>", RegexOptions.Singleline);

            String html4 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/f24472c9-6703-4863-b58a-114722e418c3/download");

            MatchCollection cSouthEast = Regex.Matches(html4, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            MatchCollection nSouthEast = Regex.Matches(html4, @"<name>Aedes Mosquito Breeding Habitats :\s*(.+?)\s*</name>", RegexOptions.Singleline);

            String html5 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/7a7881b4-ea5e-46b1-aa4e-a25b8a0cefda/download");

            MatchCollection cSouthWest = Regex.Matches(html5, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            MatchCollection nSouthWest = Regex.Matches(html5, @"<name>Aedes Mosquito Breeding Habitats :\s*(.+?)\s*</name>", RegexOptions.Singleline);
            //coordinates
            foreach (Match m in cCentral)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }


            //breeding habitat cases
            foreach (Match m in nCentral)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }

            //coordinates
            foreach (Match m in cNorthEast)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }

            //breeding habitat cases
            foreach (Match m in nNorthEast)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }


            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }

            //breeding habitat cases
            foreach (Match m in nNorthWest)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }

            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }

            //breeding habitat cases
            foreach (Match m in nNorthWest)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }


            foreach (Match m in cSouthEast)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }

            //breeding habitat cases
            foreach (Match m in nSouthEast)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }


            foreach (Match m in cSouthWest)
            {

                string temp = m.Groups[1].Value;
                coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                longitude.Add(temp2[0]);
                latitude.Add(temp2[1]);
            }

            //breeding habitat cases
            foreach (Match m in nSouthWest)
            {

                string temp = m.Groups[1].Value;
                cases.Add(temp);
            }

            BreedingHabitat bH = new BreedingHabitat();

                for (int j = 0; j < coordinates.Count; j++)
                {
                    bH.Reporter_Name = "Izzat";
                    bH.Contact_No = "96938353";
                    bH.Email = "test.com";
                    bH.Location = "somewhere";
                    bH.Longitude = longitude[j];
                    bH.Latitude = latitude[j];
                    bH.Details = "AEDES!";
                    bH.No_of_Cases = Int32.Parse(cases[j]);               
                    bH.Reported_Date = "unknown";
                    bH.Upload_Date = "test";
                    bH.zone = passedHabitatArray[j];
                    BHgateway.Insert(bH);
                    db.SaveChanges();
                }
         //   }


        }

        public List<string> getLongitude()
        {
            List<string> getLongitude = new List<string>();


            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/9f05bb14-a9df-4596-b8a2-cdf821b788c6/download");

            MatchCollection cCentral = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);


            String html2 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/ed953998-4ee2-4a2a-bb18-3e61fc014afd/download");

            MatchCollection cNorthEast = Regex.Matches(html2, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            


            String html3 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/3c03a902-6d95-47e6-9dc1-5378ac8425c5/download");

            MatchCollection cNorthWest = Regex.Matches(html3, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            

            String html4 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/f24472c9-6703-4863-b58a-114722e418c3/download");

            MatchCollection cSouthEast = Regex.Matches(html4, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
           

            String html5 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/7a7881b4-ea5e-46b1-aa4e-a25b8a0cefda/download");

            MatchCollection cSouthWest = Regex.Matches(html5, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
    

            foreach (Match m in cCentral)
            {

                string temp = m.Groups[1].Value;
               // coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
              //  latitude.Add(temp2[1]);
            }


            foreach (Match m in cNorthEast)
            {

                string temp = m.Groups[1].Value;
              //  coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
              //  latitude.Add(temp2[1]);
            }



            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
             //   coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
              //  latitude.Add(temp2[1]);
            }


            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
              //  coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
               // latitude.Add(temp2[1]);
            }



            foreach (Match m in cSouthEast)
            {

                string temp = m.Groups[1].Value;
               // coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
               // latitude.Add(temp2[1]);
            }



            foreach (Match m in cSouthWest)
            {

                string temp = m.Groups[1].Value;
               // coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                getLongitude.Add(temp2[0]);
               // latitude.Add(temp2[1]);
            }

            return getLongitude;
        }

        public List<string> getLatitude()
        {
            List<string> getLatitude = new List<string>();

            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/9f05bb14-a9df-4596-b8a2-cdf821b788c6/download");

            MatchCollection cCentral = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
          

            String html2 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/ed953998-4ee2-4a2a-bb18-3e61fc014afd/download");

            MatchCollection cNorthEast = Regex.Matches(html2, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
           


            String html3 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/3c03a902-6d95-47e6-9dc1-5378ac8425c5/download");

            MatchCollection cNorthWest = Regex.Matches(html3, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
            

            String html4 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/f24472c9-6703-4863-b58a-114722e418c3/download");

            MatchCollection cSouthEast = Regex.Matches(html4, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
           

            String html5 = web.DownloadString("https://data.gov.sg/dataset/bbef5ace-4ac5-494a-a385-07c5acff0ae4/resource/7a7881b4-ea5e-46b1-aa4e-a25b8a0cefda/download");

            MatchCollection cSouthWest = Regex.Matches(html5, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);
         

            foreach (Match m in cCentral)
            {

                string temp = m.Groups[1].Value;
             //   coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
            //    longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }


            foreach (Match m in cNorthEast)
            {

                string temp = m.Groups[1].Value;
             //   coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                //  longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }



            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
             //   coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                //   longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }


            foreach (Match m in cNorthWest)
            {

                string temp = m.Groups[1].Value;
              //  coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                //  longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }



            foreach (Match m in cSouthEast)
            {

                string temp = m.Groups[1].Value;
              //  coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
                //  longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }



            foreach (Match m in cSouthWest)
            {

                string temp = m.Groups[1].Value;
              //  coordinates.Add(temp);
                string[] temp2 = temp.Split(new string[] { "," }, StringSplitOptions.None);
              //  longitude.Add(temp2[0]);
                getLatitude.Add(temp2[1]);
            }

            return getLatitude;
        }

        public string getDate()
        {
            int i = 0;
            string webDate = "new";


            //Dengue Cluster
            WebClient web = new WebClient();
            String html = web.DownloadString("https://data.gov.sg/dataset/dengue-mosquito-breeding-habitats");
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
    }
}