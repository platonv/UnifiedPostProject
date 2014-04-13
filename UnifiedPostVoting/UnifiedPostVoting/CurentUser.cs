using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedPostVoting.Models;

namespace UnifiedPostVoting
{
    public static class CurentUser
    {
        public static int ID { get; set; }
        public static string FirstName { get; set; }
        public static string  LastName { get; set; }
        public static int Points { get; set; }

        public static void FromUser(User usr)
        {
            ID = usr.ID;
            FirstName = usr.FirstName;
            LastName = usr.LastName;
            Points = usr.Points;
        }
    }
}
