using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedPostVoting.ModelView
{
    /*
     * View equivalent of the group.. It also has a list with the users in the group.
     */
    public class GroupView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<UserView> Users { get; set; }
    }
}
