using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedPostVoting.ModelView
{
    public class UserView : INotifyPropertyChanged
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Group { get; set; }
        public int Points { get; set; }
        public int Admin { get; set; }
        public int Pending { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
