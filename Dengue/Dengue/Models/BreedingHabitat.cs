using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dengue.Models
{
    public class BreedingHabitat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BH_ID { get; set; }

        public string Reporter_Name { get; set; }
        public string Contact_No { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Details { get; set; }
        public int No_of_Cases { get; set; }
        public string Reported_Date { get; set; }
        public string zone { get; set; }
        //public bool Status { get; set; }

        public string Upload_Date { get; set; }
    }
}