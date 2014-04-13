using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedPostVoting.Models;

namespace UnifiedPostVoting.DAL
{
    class GroupDAL : DataAccessLayer
    {
        /*
         * This adds a group
         */
        public void Add(string name)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO Groups (Name) VALUES (@name)";

                cmd.Parameters.Add(new SqlParameter("@name", name));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * DUUH..
         */
        public List<Group>GettAll()
        {
            List<Group> allGroups = new List<Group>();

            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Groups";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allGroups.Add(Load(reader));
                    }
                }
            }
            return allGroups;
        }

        /*
        * Deletes a group and makes all sets all it's users group id to 0.
        */
        public void DeletebyGroupID(int id)
        {
            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "UPDATE Users SET Points=0 WHERE GroupID = @id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();

                cmd.CommandText = "DELETE FROM Groups WHERE ID = @id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                cmd.ExecuteNonQuery();
            }
        }

        /*
         * Standard..
         */
        public Group GetById(int id)
        {
            Group g = new Group();

            using (var conn = GetConnection())
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Groups WHERE ID=@id";

                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        g = Load(reader);
                    }
                }
            }
            return g;
        }

        private Group Load(SqlDataReader reader)
        {
            return new Group()
            {
                ID = (int)reader["ID"],
                Name = (string)reader["Name"]
            };
        }
    }
}
