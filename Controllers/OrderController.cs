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

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration _config;
        CommonHelper _helper;

        public OrderController(IConfiguration config)
        {
            _config = config;
            _helper = new CommonHelper(_config);
        }

        [HttpGet]
        public IActionResult Order(int? idHotel,string? idImagePath)
        {
            if (HttpContext.Session.GetString("Role_Name") != "User")
                return RedirectToAction("Index", "Home");
            OrderViewModel.ImagePath = idImagePath;
            OrderViewModel.idHotel = (int)idHotel;
            return View();
        }

        [HttpPost]
        public IActionResult Order(OrderViewModel ovm)
        {
            string Email = HttpContext.Session.GetString("Email");
            int User_ID = _helper.GetUser_ID(Email);
            HttpContext.Session.SetString("User_ID", User_ID.ToString());
            string time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
           
            string Query = $"Insert into [Order](User_ID,Hotel_ID,Booking_date,Number_of_beds," +
                $"Apartament_class,Arrival_time,Days,Status_ID)values('{User_ID}','{OrderViewModel.idHotel}'," +
                $"'{time}','{ovm.Number_of_beds}','{ovm.Apartament_class}','{ovm.Arrival_time.ToString("MM/dd/yyyy HH:mm:ss")}'," +
                $"'{ovm.Days}','{2}')";
            int result = _helper.DMLTransaction(Query);
            if (result > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Orders(int? idOrder, int? idStatus, int? idHotel,int? idRoom,int? Price)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            if (idOrder != null && idStatus !=null)
            {
                string Query = $"UPDATE [Order] SET Status_ID = '{idStatus}' " +
                    $"WHERE Order_ID = '{idOrder}'";
                int result = _helper.DMLTransaction(Query);
                if(idStatus ==1)
                {
                    int[]bill = _helper.GetDays((int)idOrder);
                    string Query2 = $"Insert into [Bill](User_ID,Hotel_ID,Room_ID,Order_ID," +
                    $"Bill)values('{bill[0]}','{idHotel}','{idRoom}','{idOrder}','{Price*bill[1]}')";
                    int result1 = _helper.DMLTransaction(Query2);
                }
                

            }
            string query = $"SELECT ImagePath from[Hotels],[Order]" +
                $"where [Hotels].Hotel_ID = [Order].Hotel_ID";
            UserViewModel.ImagePath = _helper.GetImagePath(query);
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("OrderViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
                sqlConnection.Close();
            }
            return View(dtbl);
        }

        [HttpPost]
        public IActionResult Orders(int idRoom, int idStatus)
        {
           
            //DataTable dtbl = new DataTable();
            //using (SqlConnection sqlConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            //{
            //    sqlConnection.Open();
            //    SqlDataAdapter sqlDa = new SqlDataAdapter("OrderViewAll", sqlConnection);
            //    sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            //    sqlDa.Fill(dtbl);
            //    sqlConnection.Close();
            //}
            //return View(dtbl);
            return RedirectToAction("Orders");

        }

    }
}
