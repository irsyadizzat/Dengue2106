using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using Dengue.Models;

namespace Dengue.DAL
{
    public class DengueContext : DbContext
    {
        public DengueContext() : base("DengueData")
        {

        }
        public DbSet<BreedingHabitat> BreedingHabitat { get; set; }
        public DbSet<DengueCaseHistory> DengueCaseHistory { get; set; }
        public DbSet<DengueCases> DengueCases { get; set; }
        public DbSet<DengueCluster> DengueCluster { get; set; }
        public DbSet<Weather> Weather { get; set; }

    }
}