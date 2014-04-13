using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UnifiedPostVoting.DAL
{
    /*
     * The Class is just for easing the connection creation. It also manages connection error.
     */
    public abstract class DataAccessLayer
    {
        protected SqlConnection GetConnection()
        {
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=UnifiedPostDB;Integrated Security=True");
                connection.Open();
            }
            catch(Exception)
            {
                MessageBox.Show("Error loading database...");
            }

            return connection;
        }
    }
}
