using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dengue.Models
{
    public class DengueCluster
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DCluster_ID { get; set; }

        public string Description { get; set; }
        public int No_of_Cases { get; set; }
        public string Hyperlink { get; set; }
        public string Coordinates { get; set; }
        public string Alert_Level { get; set; }
        public string Upload_Date { get; set; }
    }
}