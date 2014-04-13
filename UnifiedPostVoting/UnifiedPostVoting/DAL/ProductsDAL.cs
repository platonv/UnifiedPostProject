using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedPostVoting.Models;

namespace UnifiedPostVoting.DAL
{
    /*
     * Class for managing products DAL. It inherits from DataAccesLayer for the GetConnection function.
     */
    class ProductsDAL : DataAccessLayer
    {
        /*
         * This adds the product and it receives the id the database provides.. just in case...
         */
        public void Add(Product product)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO Products (Name, Info, VotesNumber, ParentID, IsCategory, Cost) VALUES (@name, @info, @votes, @parentid, 0, @cost)";

                cmd.Parameters.Add(new SqlParameter("@name", product.Name));
                cmd.Parameters.Add(new SqlParameter("@info", product.Info));
                cmd.Parameters.Add(new SqlParameter("@votes", product.VotesNumber));
                cmd.Parameters.Add(new SqlParameter("@parentid", product.ParentID));
                cmd.Parameters.Add(new SqlParameter("@cost", product.Cost));

                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT @@identity";

                product.ID = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        /*
         * This adds the product and it receives the id the database provides.. just in case...
         */
        public void AddCategory(Product product)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO Products (Name, Info, VotesNumber, ParentID, IsCategory, Cost) VALUES (@name, @info, @votes, @parentid, 1, @cost)";

                cmd.Parameters.Add(new SqlParameter("@name", product.Name));
                cmd.Parameters.Add(new SqlParameter("@info", product.Info));
                cmd.Parameters.Add(new SqlParameter("@votes", product.VotesNumber));
                cmd.Parameters.Add(new SqlParameter("@parentid", product.ParentID));
                cmd.Parameters.Add(new SqlParameter("@cost", product.Cost));

                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT @@identity";

                product.ID = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        /*
         * This updates a user's number of points
         */
        public void SetVotes(int id, int votes)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE Products SET VotesNumber=@votes WHERE ID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", id));
                cmd.Parameters.Add(new SqlParameter("@votes", votes));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * Standard..
         */
        public Product GetById(int id)
        {
            Product p = new Product();

            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Products WHERE ID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = Load(reader);
                    }
                }
            }
            return p;
        }

        /*
         * DUUH..
         */
        public List<Product> GetAll()
        {
            List<Product> allProducts = new List<Product>();


            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Products";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allProducts.Add(Load(reader));
                    }
                }
            }
            return allProducts;
        }

        /*
         * As name suggests
         */
        public List<Product> GetByParentId(int id)
        {
            List<Product> allProducts = new List<Product>();

            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Products WHERE ID = @id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allProducts.Add(Load(reader));
                    }
                }
            }
            return allProducts;
        }

        /*
         * This is used for easier Product instance creation
         */
        private Product Load(SqlDataReader reader)
        {
            int category;
            if (reader["IsCategory"] == null)
            {
                category = 0;
            }
            else
            {
                category = (int)reader["IsCategory"];
            }

            return new Product()
            {
                ID = (int)reader["ID"],
                ParentID = (int)reader["ParentID"],
                Name = (string)reader["Name"],
                Info = (string)reader["Info"],
                VotesNumber = (int)reader["VotesNumber"],
                Cost = (int)reader["Cost"],
                IsCategory = category
            };
        }
    }
}
