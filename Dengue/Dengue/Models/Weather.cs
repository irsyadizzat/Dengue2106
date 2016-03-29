using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dengue.Models
{
    public class Weather
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Weather_Id { get; set; }

        public String Issue_Date { get; set; }

        public string Forecast { get; set; }
        public string Zone { get; set; }
        public string Locations { get; set; }

    }
}