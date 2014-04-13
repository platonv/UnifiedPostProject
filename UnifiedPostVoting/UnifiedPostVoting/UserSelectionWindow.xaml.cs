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
    /// Interaction logic for UserSelectionWindow.xaml
    /// </summary>
    public partial class UserSelectionWindow : Window
    {
        private List<User> users;
        private int groupID;
        private UsersDAL udal = new UsersDAL();

        public UserSelectionWindow(List<Models.User> users, int id)
        {
            InitializeComponent();

            this.users = users;
            this.groupID = id;
            
            users_lb.ItemsSource = users;
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            int userid = users[users_lb.SelectedIndex].ID;

            udal.AssignUserToGroup(userid, groupID);

            this.Close();
        }
    }
}
