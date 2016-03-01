using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dengue.Models;
using System.Data.Entity;

namespace Dengue.DAL
{
    public class DengueClusterGateway : DataGateway<DengueCluster>
    {
        internal IEnumerable<DengueCluster> Order(string sortOrder, IEnumerable<DengueCluster> DengueCluster)
        {
            DengueCluster = from t in SelectAll()
                    select t;
            switch (sortOrder)
            {

                case "case_desc":

                    DengueCluster = DengueCluster.OrderByDescending(d => d.No_of_Cases);

                    break;
                case "default":
                    DengueCluster = DengueCluster.OrderBy(d => d.No_of_Cases);

                    break;
            }
            return DengueCluster.ToList();

        }
    }
}