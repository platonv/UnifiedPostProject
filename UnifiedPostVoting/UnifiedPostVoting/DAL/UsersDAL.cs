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
     * Class for managing user DAL. It inherits from DataAccesLayer for the GetConnection function.
     */
    class UsersDAL : DataAccessLayer
    {
        /*
         * This adds the user and it receives the id the database provides.. just in case...
         */
        public void Add(User user)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO Users (Username, Password, FirstName, LastName, GroupID, Points, Admin) VALUES (@username, @password, @firstName, @lastName, @group, @points, @admin)";

                cmd.Parameters.Add(new SqlParameter("@username", user.UserName));
                cmd.Parameters.Add(new SqlParameter("@password", user.Password));
                cmd.Parameters.Add(new SqlParameter("@firstName", user.FirstName));
                cmd.Parameters.Add(new SqlParameter("@lastName", user.LastName));
                cmd.Parameters.Add(new SqlParameter("@group", user.GroupID));
                cmd.Parameters.Add(new SqlParameter("@points", user.Points));
                cmd.Parameters.Add(new SqlParameter("@admin", user.Admin));

                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT @@identity";

                user.ID = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        /*
         * This is called for the login
         */
        public bool CheckUser(string userName, string password)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Username = @username";

                cmd.Parameters.Add(new SqlParameter("@username", userName));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dbpwd = (string)reader["Password"];
                        dbpwd = dbpwd.Trim();
                        if (dbpwd == password)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*
         * This is called for the login
         */
        public User GetByName(string userName)
        {
            User user = new User();
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Username = @username";

                cmd.Parameters.Add(new SqlParameter("@username", userName));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = Load(reader);
                    }
                }
            }
            return user;
        }

        /*
         * This guess what it does!
         */
        public bool CheckIfAdmin(string username)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Username = @username";

                cmd.Parameters.Add(new SqlParameter("@username", username));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((int)reader["Admin"]==1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*
         * This updates a user's number of points
         */
        public void SetPoints(int id, int points)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE Users SET Points=@points WHERE ID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", id));
                cmd.Parameters.Add(new SqlParameter("@points", points));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * This updates the number of points of some users by a group id
         */
        public void SetPointsToGroupID(int id, int points)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE Users SET Points=@points WHERE GroupID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", id));
                cmd.Parameters.Add(new SqlParameter("@points", points));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * This removes a user
         */
        public void Remove(User user)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "DELETE FROM Users WHERE ID = @id";

                cmd.Parameters.Add(new SqlParameter("@id", user.ID));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * This gets all users
         */
        public IEnumerable<User> GetAll()
        {
            List<User> allUsers = new List<User>();


            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Users";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allUsers.Add(Load(reader));
                    }
                }
            }
            return allUsers;
        }

        /*
         * This gets all users but not the admins
         */
        public IEnumerable<User> GetAllButAdmins()
        {
            List<User> allUsers = new List<User>();


            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Users WHERE Admin!=1";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allUsers.Add(Load(reader));
                    }
                }
            }
            return allUsers;
        }

        private User Load(SqlDataReader reader)
        {
            return new User()
            {
                ID = (int)reader["ID"],
                FirstName = ((string)reader["FirstName"]).Trim(),
                UserName = ((string)reader["Username"]).Trim(),
                Password = ((string)reader["Password"]).Trim(),
                Points = (int)reader["Points"],
                Admin = (int)reader["Admin"],
                GroupID = (int)reader["GroupID"],
                LastName = ((string)reader["LastName"]).Trim()
            };
        }

        /*
         * This removes a user by id
         */
        public void Remove(int id)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "DELETE FROM Users WHERE ID = @id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * Assigns user to a new group by updating the user's group id property
         */
        public void AssignUserToGroup(int userid, int groupID)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE Users SET GroupID=@group WHERE ID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", userid));
                cmd.Parameters.Add(new SqlParameter("@group", groupID));

                cmd.ExecuteNonQuery();
            }
        }
    }
}
