using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dengue.Models
{
    public class ResultModel
    {
        public bool selectedEvaluateArea { get; set; }
        public string street { get; set; }
        public string risklevel { get; set; }
        public double dengueClusterCases { get; set; }
        public double breedingCases { get; set; }
        public double breedingRegion { get; set; }
        public double dengueRegion { get; set; }
        public double dengueLocation { get; set; }
        public double breedingLocation { get; set; }
        public int noOfLocationInZone { get; set; }
        public int regionNumber { get; set; }
        public string region { get; set; }
        public string areaWeather { get; set; }

    }

}