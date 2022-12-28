using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class CommonHelper
    {
        private IConfiguration _config;
        public CommonHelper(IConfiguration config)
        {
            _config = config;
        }

        public UserViewModel GetUserByUserName(string query)
        {
            UserViewModel user = new UserViewModel();
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        user.Id = Convert.ToInt32(dataReader["User_ID"]);
                        user.Role_ID = Convert.ToInt32(dataReader["Role_ID"]);
                        user.UserName = dataReader["UserName"].ToString();
                        user.Name = dataReader["Name"].ToString();
                        user.Email = dataReader["Email"].ToString();
                        user.Password = dataReader["PasswordHash"].ToString();
                    }
                }
                connection.Close();
            }
            return user;
        }


        public int DMLTransaction(string Query)
        {
            int Result;
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = Query;
                SqlCommand command = new SqlCommand(sql, connection);
                Result = command.ExecuteNonQuery();
                connection.Close();
            }
            return Result;
        }


        public bool UserAlreadyExists(string query)
        {
            bool flag = false;
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader rd = command.ExecuteReader();
                if (rd.HasRows)
                    flag = true;
                connection.Close();
            }
            return flag;
        }
        public string GetRole(string Email, string query)
        {

            string Role_Name = "";
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Role_Name = dataReader["Role_Name"].ToString();
                    }
                }
                connection.Close();
                return Role_Name;
            }
        }
        public int GetUser_ID(string Email)
        {
            string query = $"select User_ID from [user] where Email ='{Email}'";
            int User_ID =0;
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User_ID = Convert.ToInt32(dataReader["User_ID"]);
                    }
                }
                connection.Close();
                return User_ID;
            }
        }

        public List<string> GetImagePath(string query)
        {
            List<string> ImagePath = new List<string>();
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        ImagePath.Add(dataReader["ImagePath"].ToString());
                    }
                }
                connection.Close();
                return ImagePath;
            }

        }

        public int[]  GetDays(int Order_ID)
        {
            int days =0, user_id=0;
            int[] bill = { 0, 0 };
            string query = $"Select User_ID,Days from [Order] WHERE Order_ID = '{Order_ID}'";
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = query;
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        bill[0] = Convert.ToInt32(dataReader["User_ID"]);
                        bill[1] = Convert.ToInt32(dataReader["Days"]);
                    }
                }
                connection.Close();
                
                return bill;
            }

        }


    }
}
