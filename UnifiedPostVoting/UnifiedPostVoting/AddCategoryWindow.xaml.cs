using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UnifiedPostVoting.DAL;
using UnifiedPostVoting.Models;

namespace UnifiedPostVoting
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private string name;
        private int pid;

        public AddCategoryWindow(int id)
        {
            InitializeComponent();

           this.pid = id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = TxtbxName.Text;

            ProductsDAL pdal = new ProductsDAL();

            //@name, @info, @votes, @parentid, 1, @cost
            pdal.AddCategory(new Product()
            {
                Cost=0,
                Info="",
                IsCategory=1,
                Name=name,
                ParentID=pid,
                VotesNumber=0,
                TemporalPoints=0,
            });

            this.Close();
        }
    }
}
