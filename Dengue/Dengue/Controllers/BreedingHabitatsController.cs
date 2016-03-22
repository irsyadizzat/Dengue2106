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
    public class BreedingHabitatsController : Controller
    {
        private BHGateway BHgateway = new BHGateway();
        public ActionResult Index()
        {
            return View(BHgateway.SelectAll());
        }

        //
        // GET: /Booking/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BreedingHabitat BH = BHgateway.SelectById(id);
            if (BH == null)
            {
                return HttpNotFound();
            }
            return View(BH);
        }

        //
        // GET: /Booking/Create
        public ActionResult Create(int? id, string name)
        {

                return View();
            
            //BreedingHabitat booking = new BreedingHabitat();
            //booking.TourID = (int)id;
            //booking.TourName = name;
            //booking.DepartureDate = DateTime.Now;
            //return View(booking);
        }

        //
        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Reporter_Name,Contact_No,Email,Location,Longitude,Latitude,Details,Reported_Date,Status,Upload_Date")] BreedingHabitat BH)
        {
            if (ModelState.IsValid)
            {
                BHgateway.Insert(BH);
                return RedirectToAction("Index");
            }

            return View(BH);
        }

        //
        // GET: /Booking/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BreedingHabitat BH = BHgateway.SelectById(id);
            if (BH == null)
            {
                return HttpNotFound();
            }
            return View(BH);
        }

        //
        // POST: /Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reporter_Name,Contact_No,Email,Location,Longitude,Latitude,Details,Reported_Date,Status,Upload_Date")] BreedingHabitat BH)
        {
            if (ModelState.IsValid)
            {
                BHgateway.Update(BH);
                return RedirectToAction("Index");
            }
            return View(BH);
        }

        //
        // GET: /Booking/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BreedingHabitat BH = BHgateway.SelectById(id);
            if (BH == null)
            {
                return HttpNotFound();
            }
            return View(BH);
        }

        //
        // POST: /Booking/Delete/5
        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BHgateway.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
