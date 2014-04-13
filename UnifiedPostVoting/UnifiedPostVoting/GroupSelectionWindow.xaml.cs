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
    /// Interaction logic for GroupSelectionWindow.xaml
    /// </summary>
    public partial class GroupSelectionWindow : Window
    {
        private List<Group> groups;
        private UsersDAL gdal = new UsersDAL();
        private int uid;

        public GroupSelectionWindow(List<Group> g, int id)
        {
            InitializeComponent();

            this.groups = g;
            this.uid = id;
            
            groups_lb.ItemsSource = g;
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            int gid = groups[groups_lb.SelectedIndex].ID;

            gdal.AssignUserToGroup(uid, gid);

            this.Close();
        }
    }
}
