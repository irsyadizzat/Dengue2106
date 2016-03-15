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

namespace Dengue.Controllers
{
    public class DengueClustersController : Controller
    {
        private DengueClusterGateway DengueClustergateway = new DengueClusterGateway();
        private DengueContext db = new DengueContext();

        // GET: DengueClusters
        public ActionResult Index()
        {
            //IEnumerable<DengueCluster> data = DengueClustergateway.SelectAll();
            //foreach (DengueCluster dc in data)
            //{
            //    DengueClustergateway.Delete(dc.DCluster_ID);
            //}
            //List<string> tempstring = new List<string>();
            //List<string> locations = new List<string>();
            //List<string> cases = new List<string>();
            //List<string> hyperlink = new List<string>();
            //List<string> coordinates = new List<string>();

            //WebClient web = new WebClient();
            //String html = web.DownloadString("file:///C:/Users/IzzatLaptop/Desktop/XML%20File/dengue-clusters.kml");
            //MatchCollection m1 = Regex.Matches(html, @"<td>\s*(.+?)\s*</td>", RegexOptions.Singleline);
            //MatchCollection m2 = Regex.Matches(html, @"href=\s*(.+?)\s*>", RegexOptions.Singleline);
            //MatchCollection m3 = Regex.Matches(html, @"<coordinates>\s*(.+?)\s*</coordinates>", RegexOptions.Singleline);

            ////locations and cases
            //foreach (Match m in m1)
            //{

            //    string test = m.Groups[1].Value;
            //    tempstring.Add(test);
            //}

            ////hyperlink
            //foreach (Match m in m2)
            //{

            //    string test = m.Groups[1].Value;
            //    hyperlink.Add(test);
            //}

            ////coordinates
            //foreach (Match m in m3)
            //{

            //    string test = m.Groups[1].Value;
            //    coordinates.Add(test);
            //}

            ////cases
            //for (int i = 6; i < tempstring.Count; i += 15)
            //{
            //    string newthingy = tempstring[i];
            //    cases.Add(newthingy);
            //}

            ////locations
            //for (int i = 4; i < tempstring.Count; i += 15)
            //{
            //    string newthingy = tempstring[i];
            //    locations.Add(newthingy);
            //}

            //DengueCluster dengueCluster = new DengueCluster();

            //for (int j = 0; j < locations.Count; j++)
            //{
            //    dengueCluster.Description = locations[j];
            //    dengueCluster.No_of_Cases = Int32.Parse(cases[j]);
            //    dengueCluster.Hyperlink = hyperlink[j];
            //    dengueCluster.Coordinates = coordinates[j];
            //    dengueCluster.Alert_Level = "0";
            //    dengueCluster.Upload_Date = DateTime.Now.Date.ToShortDateString();
            //    DengueClustergateway.Insert(dengueCluster);
            //    db.SaveChanges();
            //}


            return View(DengueClustergateway.SelectAll());
        }

        // GET: DengueClusters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCluster dengueCluster = DengueClustergateway.SelectById(id);
            if (dengueCluster == null)
            {
                return HttpNotFound();
            }
            return View(dengueCluster);
        }

        // GET: DengueClusters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DengueClusters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description,No_of_Cases,Hyperlink,Coordinates,Alert_Level,Upload_Date")] DengueCluster dengueCluster)
        {
            if (ModelState.IsValid)
            {
                DengueClustergateway.Insert(dengueCluster);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dengueCluster);
        }

        // GET: DengueClusters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCluster dengueCluster = DengueClustergateway.SelectById(id);
            if (dengueCluster == null)
            {
                return HttpNotFound();
            }
            return View(dengueCluster);
        }

        // POST: DengueClusters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Description,No_of_Cases,Hyperlink,Coordinates,Alert_Level,Upload_Date")] DengueCluster dengueCluster)
        {
            if (ModelState.IsValid)
            {
                DengueClustergateway.Update(dengueCluster);
                return RedirectToAction("Index");
            }
            return View(dengueCluster);
        }

        // GET: DengueClusters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCluster dengueCluster = DengueClustergateway.SelectById(id);
            if (dengueCluster == null)
            {
                return HttpNotFound();
            }
            return View(dengueCluster);
        }

        // POST: DengueClusters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DengueCluster dengueCluster = DengueClustergateway.SelectById(id);
            DengueClustergateway.Delete(id);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: DengueClusters/Create
        [HttpPost]
        public ActionResult Upload()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
