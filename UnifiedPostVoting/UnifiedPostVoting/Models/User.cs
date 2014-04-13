using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedPostVoting.Models
{
    /*
    * Database equivalent of an user
    */
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GroupID { get; set; }
        public int Points { get; set; }
        public int Admin { get; set; }
        public int Pending { get; set; }
    }
}
