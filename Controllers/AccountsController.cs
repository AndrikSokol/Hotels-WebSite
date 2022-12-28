using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Helpers;
using WebApplication1.Models;
using System.Web;

namespace WebApplication1.Controllers
{
    public class AccountsController : Controller
    {
        private IConfiguration _config;
        CommonHelper _helper;

        public AccountsController(IConfiguration config)
        {
            _config = config;
            _helper = new CommonHelper(_config);
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpGet]
        public IActionResult Account()
        {
            //            int User_ID = Convert.ToInt32(HttpContext.Session.GetString("User_ID"));
            //            string query = $"SELECT ImagePath from[Hotels],[Order]" +
            //                $"where User_ID = '{User_ID}' AND[Hotels].Hotel_ID = [Order].Hotel_ID";
            //            UserViewModel.ImagePath=_helper.GetImagePath(query);
            //            DataTable dtbl = new DataTable();
            //            using (SqlConnection sqlConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            //            {
            //                sqlConnection.Open();
            //                SqlDataAdapter sqlDa = new SqlDataAdapter("OrderViewByID", sqlConnection);
            //                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            //                sqlDa.SelectCommand.Parameters.AddWithValue("User_ID", User_ID);
            //                sqlDa.Fill(dtbl);
            //                sqlConnection.Close();
            //;
            //            }
            //            return View(dtbl);
            int User_ID = Convert.ToInt32(HttpContext.Session.GetString("User_ID"));
            string query = $"SELECT ImagePath from[Hotels],[Order]" +
                $"where User_ID = '{User_ID}' AND[Hotels].Hotel_ID = [Order].Hotel_ID";
            UserViewModel.ImagePath = _helper.GetImagePath(query);
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BillForUser", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("User_ID", User_ID);
                sqlDa.Fill(dtbl);
                sqlConnection.Close();
                ;
            }
            return View(dtbl);
        }

        [HttpGet]
        public IActionResult Logout ()
        {
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //Response.Cache.SetNoStore();

            //// Clears the session at the end of request
            //Session.Abandon();


            //HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            //HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            //HttpContext.Current.Response.AddHeader("Expires", "0");

            //HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            //HttpContext.Response.Headers.Add("Pragma", "no-cache");
            //HttpContext.Response.Headers.Add("Expires", "0");
            //HttpContext.Response.Headers.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel lvm)
        {
            if (string.IsNullOrEmpty(lvm.UserName) && string.IsNullOrEmpty(lvm.Password))
            {
                ViewBag.ErrorMsg = "UserName and Password Empty";
                return View();
            }
            else
            {
                bool Isfind = SignInMethod(lvm.UserName, lvm.Password);
                if (Isfind == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
        }


        private bool SignInMethod(string UserName, string Password)
        {
            bool flag = false;

            string query = $"select * from [user] where UserName ='{UserName}'";
            var userDetails = _helper.GetUserByUserName(query);

            if (userDetails.UserName != null && BCrypt.Net.BCrypt.Verify(Password,userDetails.Password))
            {
                flag = true;
                EntryIntoSession(userDetails.UserName,userDetails.Name,userDetails.Email,userDetails.Role_ID);
                //HttpContext.Session.Set<UserViewModel>("User", userDetails);
                //var user = HttpContext.Session.Get<UserViewModel>("User");
                //ViewBag.sessionUserName = user.UserName;
            }
            else
            {
                ViewBag.ErrorMsg = "UserName & Password wrong";
            }
            return flag;
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            vm.UserName = stripXSS(vm.UserName);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            Console.WriteLine(passwordHash.Length);
            string UserNameExitsQuery = $"select * from [User] where UserName='{vm.UserName}'";
            string EmailExitsQuery = $"select * from [User] where Email='{vm.Email}'";
            bool userNameExists = _helper.UserAlreadyExists(UserNameExitsQuery);
            bool EmailExists = _helper.UserAlreadyExists(EmailExitsQuery);
            if (userNameExists == true)
            {
                ViewBag.Error = "UserName aldready Exists";
                return View();
            }
            
            if (EmailExists == true)
            {
                ViewBag.Error = "Email aldready Exists";
                return View();
            }
            string Query = $"Insert into [User](Role_ID,UserName,Name,Email,PasswordHash)values('{1}','{vm.UserName}'," +
                $"'{vm.Name}','{vm.Email}','{passwordHash}')";

            int result = _helper.DMLTransaction(Query);
            if (result > 0)
            {
                EntryIntoSession(vm.UserName, vm.Name,vm.Email);
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        private string stripXSS(string value)
        {
            value = value.Replace("<", "");
            value = value.Replace(">", "");
            value = value.Replace("/", "");
            return value;
        }

        private void EntryIntoSession(string UserName,string Name,string Email,int Role_ID = 1)
        {
            
            HttpContext.Session.SetString("UserName", UserName);
            HttpContext.Session.SetString("Name", Name);
            HttpContext.Session.SetString("Email", Email);
            HttpContext.Session.SetString("Role_ID", Role_ID.ToString());
            string query = $"select Role_Name from Role,[user] where Role.Role_ID = [user].Role_ID And [user].Email = '{Email}'";
            HttpContext.Session.SetString("Role_Name", _helper.GetRole(Email,query));
            HttpContext.Session.SetString("User_ID" ,_helper.GetUser_ID(Email).ToString());

        }
    }
}
