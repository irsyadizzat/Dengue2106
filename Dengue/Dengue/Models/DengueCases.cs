using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dengue.Models
{
    public class DengueCases
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DC_ID { get; set; }

        public int No_of_Cases { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public string Zone { get; set; }
        public string Location { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Upload_Date { get; set; }
    }
}