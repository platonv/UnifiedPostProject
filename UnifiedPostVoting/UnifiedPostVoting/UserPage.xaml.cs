using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        ObservableCollection<Product> productsView = new ObservableCollection<Product>();

        private String UserName { get; set; }
        private int Points { get; set; }

        private int InitialPoints;
        private List<User> Users;
        private List<Product> Products;
        private List<Product>[] ActualProducts;
        private const int N=5;
        private const int MaxProds=100000;
        private int ant = 0;

        private ProductsDAL pdal = new ProductsDAL();

        public UserPage()
        {

            InitializeComponent();

            //Mock up
            UserName = CurentUser.FirstName + " " + CurentUser.LastName;
            InitialPoints = Points = CurentUser.Points;

            ReloadProducts();

            //Load the products
            LoadProducts(N);

            //Load information
            TextBlockUserName.Text = UserName;
            TextBlockUserPoints.Text = "" + Points;

            //Parcurs = new List<bool>();
            ActualProducts = new  List<Product>[MaxProds];

            CreateMenu();
        }

        private void CreateMenu()
        {
            foreach (Product p in Products) //almost the same thing as Constructmenu, only the fact that here the root-
            // menu item is the principal menu here
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
                //else
                //{
                //    ((MenuItem)GroupsMenu.Items[GroupsMenu.Items.Count - 1]).Tag = p.ID;
                //}
            }
            ReloadProducts();
        }

        private void LoadProducts(int N)
        {
            Products = pdal.GetAll();
        }

        private bool AllChildCategory(int root) //return true if all the children are leaves
        {
            bool ok = true;
            foreach (Product p in Products)
            {
                if (p.ParentID == root)
                    if (p.IsCategory == 1)
                        return false;
            }
            return ok;
        }

        //int pindex;
        void MI_Click(object sender, RoutedEventArgs e)
        {
            FrameViewWelcome.Visibility = Visibility.Hidden;
            MenuItem MIL=(MenuItem) sender;

            //productsView = ActualProducts[(int)MIL.Tag];

            productsView.Clear();
            try
            {
                foreach (Product p in ActualProducts[(int)MIL.Tag])
                {
                    if (p.IsCategory == 0)
                        productsView.Add(new Product()
                        {
                            ID = p.ID, //////aici
                            Info = p.Info,
                            IsCategory = p.IsCategory,
                            Name = p.Name,
                            ParentID = p.ParentID,
                            VotesNumber = p.VotesNumber,
                            Cost = p.Cost
                        });
                }
            }
            catch
            {
              
            }

            ProductsListBox.ItemsSource = productsView;
            
           /// pindex = Convert.ToInt32((int) MIL.Tag);
        }
        /*This procedure constructs the menu navigator 
         * based on the fact that the products are organized in a tree*/
        private void ConstructMenu(Product Prod, MenuItem menuitem)
        {
            List<Product> Local; 
            Local=new List<Product>();
            int root = Prod.ID;
            bool first = false;
            foreach (Product p in Products)
            {
                if ((p.ParentID == root) && (p.IsCategory==1))
                {
                    
                    MenuItem MI = new MenuItem();
                    MI.Header = p.Name; //Set the Header
                    MI.Tag = p.ID;
                    ConstructMenu(p, MI); //construct all the childs of the MenuItem
                    if (AllChildCategory(p.ID)) //if the childs aren`t leaves there has to be an event
                        MI.Click += MI_Click;
                    menuitem.Items.Add(MI); // add the menuitem
                    ((MenuItem)menuitem.Items[menuitem.Items.Count-1]).Tag = p.ID; // index the menuitem
                }
                else if (p.ParentID==root)
                {
                    first = true;
                    Local.Add(p);
                }
            }
            if (first)
            {
                ActualProducts[root] = Local;            
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }

        //Add Points Button
        private void BtnPlus_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel Stk = (StackPanel)btn.Parent;
            Product prod = Products.Find(p => p.ID == (int)Stk.Tag);
            if (Points - 1 >= 0)
            {
                Points --;
                prod.TemporalPoints++;
            }

            UpdatePointsView(Stk, prod);
        }
        //Remove Points Button
        private void BtnMinus_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel Stk = (StackPanel)btn.Parent;
            Product prod = Products.Find(p => p.ID == (int)Stk.Tag);

            if ((Points + 1 <= InitialPoints)&&(prod.TemporalPoints-1>=0))
            {
                prod.TemporalPoints--;
                Points ++;
            }
            UpdatePointsView(Stk, prod);
        }
        //Update the textbox and textblock
        private void UpdatePointsView(StackPanel Stk, Product prod)
        {
            TextBlockUserPoints.Text ="" + Points;
            foreach (var txtbx in Stk.Children)
            {
                if (txtbx is TextBlock)
                    ((TextBlock)txtbx).Text = "" + prod.TemporalPoints.ToString();
            }
        }

        private void ReloadProducts()
        {
            Products = pdal.GetAll();
            productsView.Clear();
            foreach (var p in Products)
            {
                if (p.IsCategory == 0)
                    productsView.Add(new Product()
                    {
                        ID=p.ID, ///////aici
                        Info = p.Info,
                        IsCategory = p.IsCategory,
                        Name = p.Name,
                        ParentID = p.ParentID,
                        VotesNumber = p.VotesNumber,
                        Cost = p.Cost
                    });
            }
        }

        private void SubmitPoints_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Product p in Products)
            {
                p.VotesNumber += p.TemporalPoints;

                pdal.SetVotes(p.ID, p.VotesNumber);

                p.TemporalPoints = 0;
            }

            UsersDAL udal = new UsersDAL();

            udal.SetPoints(CurentUser.ID, Points);

            productsView.Clear();
            ReloadProducts();
            GroupsMenu.Items.Clear();
            ActualProducts = new List<Product>[100];
            CreateMenu();
        }
    }
}
