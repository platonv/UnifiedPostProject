using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedPostVoting.ModelView
{
    class ProductView : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public int VotesNumber { get; set; }
        public int ParentID { get; set; }
        public int IsCategory { get; set; }
        public int Cost { get; set; }
        public int Money { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
