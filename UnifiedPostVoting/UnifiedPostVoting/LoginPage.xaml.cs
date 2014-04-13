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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnifiedPostVoting.DAL;
using UnifiedPostVoting.Models;

namespace UnifiedPostVoting
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    
    public partial class LoginPage : Page
    {
        UsersDAL uDAL = new UsersDAL();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void login_btn_click(object sender, RoutedEventArgs e)
        {
            string user = tb_usr.Text;
            if (uDAL.CheckUser(tb_usr.Text, pb_pwd.Password))
            {
                if (!uDAL.CheckIfAdmin(user))
                {
                    User usr = uDAL.GetByName(user);

                    CurentUser.FromUser(usr);

                    this.NavigationService.Navigate(new Uri("/UserPage.xaml", UriKind.Relative));
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/AdminPage.xaml", UriKind.Relative));
                }
            }
            else
            {
                incorect_lb.Visibility = Visibility.Visible;
            }
        }
    }
}
