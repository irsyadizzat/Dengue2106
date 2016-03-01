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
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Details { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Reported_Date { get; set; }

        public bool Status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Upload_Date { get; set; }
    }
}