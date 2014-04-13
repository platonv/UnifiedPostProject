using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedPostVoting.Models
{
    /*
     * Database equivalent of a product
     */
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int VotesNumber { get; set; }
        public int ParentID { get; set; }
        public int IsCategory { get; set; }
        public int Pending { get; set; }
        public int Cost { get; set; }

        public int TemporalPoints { get; set; }
    }
}
