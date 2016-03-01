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

namespace Dengue.Controllers
{
    public class DengueCaseHistoriesController : Controller
    {
        private DengueContext db = new DengueContext();

        // GET: DengueCaseHistories
        public ActionResult Index()
        {
            return View(db.DengueCaseHistory.ToList());
        }

        // GET: DengueCaseHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCaseHistory dengueCaseHistory = db.DengueCaseHistory.Find(id);
            if (dengueCaseHistory == null)
            {
                return HttpNotFound();
            }
            return View(dengueCaseHistory);
        }

        // GET: DengueCaseHistories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DengueCaseHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DCH_ID,No_of_Cases,Epi_Week")] DengueCaseHistory dengueCaseHistory)
        {
            if (ModelState.IsValid)
            {
                db.DengueCaseHistory.Add(dengueCaseHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dengueCaseHistory);
        }

        // GET: DengueCaseHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCaseHistory dengueCaseHistory = db.DengueCaseHistory.Find(id);
            if (dengueCaseHistory == null)
            {
                return HttpNotFound();
            }
            return View(dengueCaseHistory);
        }

        // POST: DengueCaseHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DCH_ID,No_of_Cases,Epi_Week")] DengueCaseHistory dengueCaseHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dengueCaseHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dengueCaseHistory);
        }

        // GET: DengueCaseHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DengueCaseHistory dengueCaseHistory = db.DengueCaseHistory.Find(id);
            if (dengueCaseHistory == null)
            {
                return HttpNotFound();
            }
            return View(dengueCaseHistory);
        }

        // POST: DengueCaseHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DengueCaseHistory dengueCaseHistory = db.DengueCaseHistory.Find(id);
            db.DengueCaseHistory.Remove(dengueCaseHistory);
            db.SaveChanges();
            return RedirectToAction("Index");
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
