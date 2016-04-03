using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dengue.Models
{
    public class StatisticsModel
    {
        public string topic { get; set; }
        public double total { get; set; }
        public double count { get; set; }
        public double mean { get; set; }
        public double variance { get; set; }
        public double sd { get; set; }

    }
}