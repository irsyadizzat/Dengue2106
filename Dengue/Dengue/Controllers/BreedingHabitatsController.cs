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
        private DengueContext db = new DengueContext();
        private WeatherGateway Weathergateway = new WeatherGateway();
        private DengueClusterGateway DengueClustergateway = new DengueClusterGateway();

        public ActionResult Index(string search, string sortCases)
        {
            ViewData["noDengueCase"] = DengueClustergateway.getNoCases();


            IEnumerable<BreedingHabitat> bH = BHgateway.SelectAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToUpper();
                bH = bH.Where(d => d.Location.ToUpper().Contains(search));
            }
            else
            {
                ViewBag.PriceSortParm = String.IsNullOrEmpty(sortCases) ? "case_desc" : "";
                switch (sortCases)
                {

                    case "case_desc":
                        bH = BHgateway.Order("case_desc", bH);
                        break;
                    default:
                        bH = BHgateway.Order("default", bH);

                        break;
                }
            }

            setWeatherDropdown();

            return View(bH);
        }
        public void setWeatherDropdown()
        {
            IEnumerable<Weather> weatherAll = Weathergateway.SelectAll();
            List<SelectListItem> weatherLocationList = new List<SelectListItem>();

            Boolean jurongAdded = false;

            foreach (Weather w in weatherAll)
            {
                if (w.Locations == "JURONG EAST/WEST" || w.Locations == "JURONG INDUSTRIAL ESTATE" || w.Locations == "JURONG ISLAND")
                {
                    if (jurongAdded == false)
                    {
                        weatherLocationList.Add(new SelectListItem() { Text = "JURONG", Value = "JURONG;" + w.Zone + ";" + w.Forecast });
                        jurongAdded = true;
                    }
                }
                else if (w.Locations == "HOLLAND VILLAGE")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "HOLLAND VILLAGE", Value = "HOLLAND;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations == "CITY")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "ORCHARD", Value = "ORCHARD;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations == "MACRITCHIE RESERVOIR")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = "MACRITCHIE", Value = "MACRITCHIE;" + w.Zone + ";" + w.Forecast });
                }
                else if (w.Locations != "PULAU TEKONG" && w.Locations != "PULAU UBIN" && w.Locations != "PEIRCE RESERVOIR" && w.Locations != "SOUTHERN ISLAND")
                {
                    weatherLocationList.Add(new SelectListItem() { Text = w.Locations, Value = w.Locations + ";" + w.Zone + ";" + w.Forecast });
                }
            }

            ViewBag.Weather = weatherLocationList;

            return;
        }
    }
}
