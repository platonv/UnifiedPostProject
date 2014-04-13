using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnifiedPostVoting.DAL;
using UnifiedPostVoting.Models;
using UnifiedPostVoting.ModelView;

namespace UnifiedPostVoting
{
    /*
     * Interaction logic for AdminPage.xaml
     */
    public partial class AdminPage : Page
    {
        //Model View Lists
        ObservableCollection<UserView> usersView = new ObservableCollection<UserView>();
        ObservableCollection<ProductView> productsView = new ObservableCollection<ProductView>();
        ObservableCollection<GroupView> groupsView = new ObservableCollection<GroupView>();

        //Model Lists
        List<User> users = new List<User>();
        List<Product> products = new List<Product>();
        List<Group> groups = new List<Group>();

        //DALs
        UsersDAL udal = new UsersDAL();
        ProductsDAL pdal = new ProductsDAL();
        GroupDAL gdal = new GroupDAL();

        //This is for stupid double call shit
        int prev = int.MaxValue;

        List<Product>[] ActualProducts;

        public AdminPage()
        {
            InitializeComponent();

            this.DataContext = this;

            ReloadProducts();
            ReloadGroups();
            ReloadUsers();

            ActualProducts = new List<Product>[100];

            users_lb.ItemsSource = usersView;
            rplb.ItemsSource = productsView;
            groups_lb.ItemsSource = groupsView;

            CreateMenu(); // this has to be runned everytime the list of prods and cats is modified
            AddButton.IsEnabled = false; //firstly disable the add button

            Frame tabFrame = new Frame();
           /* WelcomePage WP = new WelcomePage();
            tabFrame.Content = WP;
            WelcomePageView.Content = tabFrame;*/
        }

        /*
         * Create the Menu Navigator and Loads the ListBox with products
         */

        #region CreateMenu
        private void CreateMenu()
        {
            foreach (Product p in products) //almost the same thing as Constructmenu, only the fact that here the root-
            // menu item is the principal menu
            {
                if (p.ParentID == 0)
                {
                    MenuItem MI = new MenuItem();
                    MI.Header = p.Name;
                    if (AllChildCategory(p.ID))
                        MI.Click += MI_Click;

                    MI.Tag = p.ID;
                    ConstructMenu(p, MI);
                    GroupsMenu.Items.Add(MI);
                }

            }
            MenuItem AddMI = new MenuItem();
            AddMI.Header = "AddCategory"; //Set the Header
            AddMI.Tag = 0;
            AddMI.Click += AddMI_Click;
            GroupsMenu.Items.Add(AddMI);

        }

        private void ConstructMenu(Product Prod, MenuItem menuitem)
        {
            List<Product> Local;
            Local = new List<Product>();
            int root = Prod.ID;
            bool first = false;
            bool second = false;
            foreach (Product p in products)
            {
                if ((p.ParentID == root) && (p.IsCategory == 1))
                {
                    MenuItem MI = new MenuItem();
                    MI.Header = p.Name; //Set the Header
                    MI.Tag = p.ID;
                    ConstructMenu(p, MI); //construct all the childs of the MenuItem
                    if (AllChildCategory(p.ID)) //if the childs aren`t leaves there has to be an event
                        MI.Click += MI_Click;
                    menuitem.Items.Add(MI); // add the menuitem
                    second = true;
                    ((MenuItem)menuitem.Items[menuitem.Items.Count - 1]).Tag = p.ID; // index the menuitem
                }
                else if (p.ParentID == root)
                {
                    first = true;
                    Local.Add(p);
                }
            }
            if (first)
            {
                try { ActualProducts[root] = Local; }
                catch { MessageBox.Show("Something went wrong in ConstructMenu()"); }
            }
            else
            {
                MenuItem AddMI = new MenuItem();
                AddMI.Header = "Add Category"; //Set the Header
                AddMI.Tag = root;
                AddMI.Click += AddMI_Click;
                menuitem.Items.Add(AddMI);
            }
            if ((!first) && (!second))
            {
                AddButton.IsEnabled = true;
            }
        }

        void AddMI_Click(object sender, RoutedEventArgs e)
        {
            int cid = (int)((MenuItem)sender).Tag;

            AddCategoryWindow wnd = new AddCategoryWindow((int)((MenuItem)sender).Tag);

            wnd.Show();

            ReloadProducts();
            CreateMenu();
        }

        int pindex;
        int currentCatID = -1;
        void MI_Click(object sender, RoutedEventArgs e)
        {
            MenuItem MIL = (MenuItem)sender;
            AddButton.IsEnabled = true;// enable the addbutton with tha product list
            AddButton.Tag = (int)MIL.Tag;
            currentCatID = (int)MIL.Tag;
            ReloadProducts();
            ProductsListBox.ItemsSource = productsView; //Load the leaves
            pindex = (int)MIL.Tag;
        }


        private bool AllChildCategory(int root) //return true if all the children are leaves
        {
            bool ok = true;
            foreach (Product p in products)
            {
                if (p.ParentID == root)
                    if (p.IsCategory == 1)
                        return false;
            }
            return ok;
        }
        #endregion  

        /*
         * This manages the tab selection and loading of the elements
         */
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (prev != tc.SelectedIndex)
            {
                if (tc.SelectedIndex == 0)              //USERS
                {
                    ReloadUsers();
                }
                else if (tc.SelectedIndex == 1)         //GROUPS
                {
                    ReloadGroups();
                }
                else if (tc.SelectedIndex == 2)         //PRODUCTS
                {
                    ReloadProducts();
                }
            }
            prev = tc.SelectedIndex;

            DrawGrafic();
        }

        /*
          * *************************************************************************************************************************************
          *                                                      USERS PART
          * *************************************************************************************************************************************
          */

        #region users

        private void btn_addUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser addusrWnd = new AddUser();
            addusrWnd.Show();
            addusrWnd.Closed += addusrWnd_Closed;
        }

        void addusrWnd_Closed(object sender, EventArgs e)
        {
            ReloadUsers();
        }

        private void ReloadUsers()
        {
            users = udal.GetAllButAdmins().ToList();

            usersView.Clear();
            foreach (var v in users)
            {
                string gname = gdal.GetById(v.GroupID).Name;
                if (gname == null) gname = "None";
                if (v.Admin != 1)
                    usersView.Add(new UserView()
                    {
                        Admin = v.Admin,
                        FirstName = v.FirstName,
                        LastName = v.LastName,
                        Group = " " + gname,
                        Points = v.Points
                    });
            }
        }

        bool editing;
        private void btn_editUser_Click(object sender, RoutedEventArgs e)
        {
            if (editing)
            {
                btn_editUser.Content = "Edit";

                try
                {
                    udal.SetPoints(users[users_lb.SelectedIndex].ID, Convert.ToInt32(price_tb.Text));
                }
                catch { }

                ReloadUsers();

                price_tb.Visibility = System.Windows.Visibility.Hidden;

                editing = false;
            }
            else
            {
                btn_editUser.Content = "OK";

                price_tb.Visibility = System.Windows.Visibility.Visible;

                editing = true;
            }
        }

        private void Assign_btn(object sender, RoutedEventArgs e)
        {
            if (users_lb.SelectedIndex > -1)
            {
                int uid = users[users_lb.SelectedIndex].ID;

                GroupSelectionWindow gw = new GroupSelectionWindow(groups, uid);

                gw.Closed += gw_Closed;

                gw.Show();
            }
            else
            {
                MessageBox.Show("Please select an item in order to use it.");
            }

            ReloadGroups();
            ReloadUsers();
        }

        void gw_Closed(object sender, EventArgs e)
        {
            ReloadUsers();
            ReloadGroups();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (users_lb.SelectedIndex > -1)
            {
                try
                {
                    int uid = users[users_lb.SelectedIndex].ID;

                    udal.Remove(uid);
                }
                catch 
                {
                    MessageBox.Show("Error, please try again..", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                ReloadGroups();
                ReloadUsers();
            }
            else
            {
                MessageBox.Show("Please select an item in order to use it.");
            }
        }

        #endregion

        /*
         * *************************************************************************************************************************************
         *                                                      GROUPS PART
         * *************************************************************************************************************************************
         */

        #region groups

        private void ReloadGroups()
        {
            groups = gdal.GettAll().ToList();

            groupsView.Clear();
            foreach (var g in groups)
            {
                groupsView.Add(new GroupView()
                {
                    Name = g.Name
                });
            }
        }

        private void btn_createGroup_Click(object sender, RoutedEventArgs e)
        {
            string newGroupName = createGroup_tb.Text;

            gdal.Add(newGroupName);

            ReloadGroups();
        }

        private void add_user_to_group_btn_Click(object sender, RoutedEventArgs e)
        {
            if (groups_lb.SelectedIndex > -1)
            {
                int gid = groups[groups_lb.SelectedIndex].ID;

                UserSelectionWindow us = new UserSelectionWindow(users, gid);

                us.Show();
            }
            else
            {
                MessageBox.Show("Please select an item in order to use it.");
            }

            ReloadGroups();
            ReloadUsers();
        }

        private void deletebygroup_btn_Click(object sender, RoutedEventArgs e)
        {
            if (groups_lb.SelectedIndex > -1)
            {
                int gid = groups[groups_lb.SelectedIndex].ID;

                gdal.DeletebyGroupID(gid);
            }
            else
            {
                MessageBox.Show("Please select an item in order to use it.");
            }

            ReloadGroups();
            ReloadUsers();
        }

        private void setpts_btn_Click(object sender, RoutedEventArgs e)
        {
            if (groups_lb.SelectedIndex > -1)
            {
                int gid = groups[groups_lb.SelectedIndex].ID;
                int pts = 0;
                try
                {
                    pts = Convert.ToInt32(((Button)sender).Tag);
                }
                catch
                {
                    return;
                }

                udal.SetPointsToGroupID(gid, pts);
            }
            else
            {
                MessageBox.Show("Please select an item in order to use it.");
            }

            ReloadUsers();
            ReloadGroups();
        }

        #endregion

        /*
         * *************************************************************************************************************************************
         *                                                      PRODUCTS PART
         * *************************************************************************************************************************************
         */

        #region products

        private void ReloadProducts()
        {
            products = pdal.GetAll();

            productsView.Clear();
            foreach (var p in products)
            {
                if((p.IsCategory==0) && ((currentCatID == -1) || (currentCatID == p.ParentID)))
                productsView.Add(new ProductView()
                {
                    Info = p.Info,
                    IsCategory = p.IsCategory,
                    Name = p.Name,
                    ParentID = p.ParentID,
                    VotesNumber = p.VotesNumber,
                    Cost = p.Cost
                });
            }
        }

        #endregion

        /*
         * *************************************************************************************************************************************
         *                                                      GRAFIC PART
         * *************************************************************************************************************************************
         */

        #region graficPart

        public void DrawGrafic()
        {
            grafCanvas.Background = Brushes.White;
            grafCanvas.Children.Clear();

            double w = grafCanvas.Width;
            double h = grafCanvas.Height;

            Line ox = new Line();
            ox.Stroke = Brushes.Black;
            ox.X1 = 10;
            ox.X2 = grafCanvas.Width - 10;
            ox.Y1 = grafCanvas.Height - 100;
            ox.Y2 = grafCanvas.Height- 100;
            ox.StrokeThickness = 1;

            Line oy = new Line();
            oy.Stroke = Brushes.Black;
            oy.X1 = 20;
            oy.X2 = 20;
            oy.Y1 = 10;
            oy.Y2 = h-100;
            ox.StrokeThickness = 1;

            grafCanvas.Children.Add(oy);
            grafCanvas.Children.Add(ox);

            int prodNum = products.Count;

            int maxH = 0;
            double m = 0;

            foreach (Product p in products)
            {
                if (p.VotesNumber > maxH) maxH = p.VotesNumber;
                m += p.VotesNumber;
            }

            double yum = (h-110) / maxH;
            double size = (w-100)/products.Count/2;

            double x = size;

            DrawBar(grafCanvas, 21, w-30, m / products.Count * yum, Brushes.LightGray);

            foreach (Product p in products)
            {
                DrawBar(grafCanvas, x+1, size, yum*p.VotesNumber, Brushes.Red);

                Label lb = new Label();
                lb.Content = p.Name;
                Canvas.SetLeft(lb, x);
                Canvas.SetTop(lb, h - 90);

                grafCanvas.Children.Add(lb);

                Label lb2 = new Label();
                lb2.Content = p.VotesNumber.ToString();
                Canvas.SetLeft(lb2, x);
                Canvas.SetTop(lb2, grafCanvas.Height-100-yum * p.VotesNumber);

                grafCanvas.Children.Add(lb2);

                x += size * 2;
            }
        }

        private void DrawBar(Canvas c, double x, double width, double height, Brush b)
        {
            Rectangle rect = new Rectangle();
            
            rect.Width = width;
            rect.Height = height;
            rect.Fill = b;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, c.Height-100-height);

            c.Children.Add(rect);
        }

        #endregion

        /*
         * *************************************************************************************************************************************
         *                                                      ADD&UPDATES PART
         * *************************************************************************************************************************************
         */
        #region
        private void ProductImageBtn_Click_1(object sender, RoutedEventArgs e)
        {

            OpenFileDialog OFD = new OpenFileDialog();
            bool? click = OFD.ShowDialog();
            if (click == true)
            {
                MessageBox.Show("" + OFD.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//Add the product button event
        {

            Button btn = (Button)sender;
            // >> TextBoxNewPRName.text << the name of the newly added product
            // >> TextBoxNewPRinfo.text << the info of the newly added product
            // >> TextBoxNewPRCost.text  << the cost of the newly added product
            int ParentID = (int)((Canvas)((Grid)(btn.Parent)).Parent).Tag; //ParentID of the newly added product

            TextRange txtrngInfo = new TextRange(TextBoxNewPRinfo.Document.ContentStart, TextBoxNewPRinfo.Document.ContentEnd);
            TextRange txtrngName = new TextRange(TextBoxNewPRName.Document.ContentStart, TextBoxNewPRName.Document.ContentEnd);
            TextRange txtrngCost = new TextRange(TextBoxNewPRCost.Document.ContentStart, TextBoxNewPRCost.Document.ContentEnd);


            try
            {
                pdal.Add(new Product()
                {
                    Info = txtrngInfo.Text,
                    Name = txtrngName.Text,
                    ParentID = ParentID,
                    Cost = Convert.ToInt32(txtrngCost.Text),
                    IsCategory = 0,
                    VotesNumber = 0
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Please check data!");
            }

            ReloadProducts();
            GroupsMenu.Items.Clear();
            ActualProducts = new List<Product>[100];
            CreateMenu();
        }
        #endregion

        private void budget_btn_Click(object sender, RoutedEventArgs e)
        {
            int budget = 0;
            try
            {
                int s = 0;

                budget = Convert.ToInt32(budget_tb.Text);

                foreach (ProductView p in productsView)
                {
                    s += p.VotesNumber;
                }

                foreach (ProductView p in productsView)
                {
                    p.Money = budget * p.VotesNumber / s;
                }
            }
            catch
            {
                MessageBox.Show("Please insert numeric value");
            }
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("report.cvs"))
            {
                foreach (var p in productsView)
                {
                    writer.WriteLine(p.Name + "," + p.Money);
                }
            }
        }
    }
}
