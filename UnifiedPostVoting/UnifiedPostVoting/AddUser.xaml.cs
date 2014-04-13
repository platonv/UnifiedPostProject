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
    /*
     * Interaction logic for AddUser.xaml
     */
    public partial class AddUser : Window
    {
        UsersDAL udal = new UsersDAL();

        public AddUser()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            User user = new User()
            {
                UserName = usr.Text,
                Password = pwd.Text,
                FirstName = fn.Text, 
                LastName = ln.Text,
                GroupID = Convert.ToInt32(gid.Text),
                Points = Convert.ToInt32(points.Text),
                Admin = ((bool)yesrb.IsChecked) ? 1 : 0
            };

            udal.Add(user);

            this.Close();
        }
    }
}
