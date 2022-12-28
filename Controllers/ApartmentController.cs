using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public ApartmentController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Apartament
        public IActionResult Index(int? id)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("RoomViewByHotel", sqlConnection);
                if (id != null)
                {
                    ApartmentViewModel.ChooseHotelID = (int)id;
                    sqlDa.SelectCommand.Parameters.AddWithValue("Hotel_ID", id);

                }
                else
                    sqlDa.SelectCommand.Parameters.AddWithValue("Hotel_ID", ApartmentViewModel.ChooseHotelID);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
                sqlConnection.Close();
            }
            return View(dtbl);
        }

        // GET: Apartament/AddOrEdit/
        public IActionResult AddOrEdit(int? idRoom, int? idHotel)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            ApartmentViewModel apartamentViewModel = new ApartmentViewModel();
            if (idRoom > 0)
                apartamentViewModel = FetchHotelByID(idRoom, idHotel);
            return View(apartamentViewModel);
        }

        // POST: Apartament/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int idHotel, [Bind("Room_ID,Hotel_ID,Room,Class,Amount_beds,Price")] ApartmentViewModel apartamentViewModel)
        {

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("RoomAddOrEdit", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Room_ID", apartamentViewModel.Room_ID);
                sqlCmd.Parameters.AddWithValue("Hotel_ID", ApartmentViewModel.ChooseHotelID);
                sqlCmd.Parameters.AddWithValue("Room", apartamentViewModel.Room);
                sqlCmd.Parameters.AddWithValue("Class", apartamentViewModel.Class);
                sqlCmd.Parameters.AddWithValue("Amount_beds", apartamentViewModel.Amount_beds);
                sqlCmd.Parameters.AddWithValue("Price", apartamentViewModel.Price);
                sqlCmd.ExecuteNonQuery();
                sqlConnection.Close();
                return RedirectToAction("Index", new { id = ApartmentViewModel.ChooseHotelID });
            }
            return View(apartamentViewModel);
        }

        // GET: Apartament/Delete/5
        public IActionResult Delete(int? idRoom, int? idHotel)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            ApartmentViewModel apartamentViewModel = FetchHotelByID(idRoom, idHotel);
            return View(apartamentViewModel);
        }

        // POST: Apartament/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int idRoom,int idHotel)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("RoomDeleteByID", sqlConnection);

                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Room_ID", idRoom);
/*                sqlCmd.Parameters.AddWithValue("Hotel_ID", idHotel);*/
                sqlCmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            return RedirectToAction("Index", new { id = ApartmentViewModel.ChooseHotelID});
        }

        [NonAction]
        public ApartmentViewModel FetchHotelByID(int? idRoom, int? idHotel)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("RoomViewByID", sqlConnection);
                sqlDa.SelectCommand.Parameters.AddWithValue("Room_ID", idRoom);
/*                sqlDa.SelectCommand.Parameters.AddWithValue("Hotel_ID", idHotel);*/
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;

                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    apartmentViewModel.Room_ID = Convert.ToInt32(dtbl.Rows[0]["Room_ID"].ToString());
                    apartmentViewModel.Hotel_ID = Convert.ToInt32(dtbl.Rows[0]["Hotel_ID"].ToString());
                    apartmentViewModel.Room = dtbl.Rows[0]["Room"].ToString();
                    apartmentViewModel.Class = dtbl.Rows[0]["Class"].ToString();
                    apartmentViewModel.Amount_beds = Convert.ToInt32(dtbl.Rows[0]["Amount_beds"].ToString());
                    apartmentViewModel.Price = Convert.ToInt32(dtbl.Rows[0]["Price"].ToString());
                }
                sqlConnection.Close();

            }
            return apartmentViewModel;
        }


        [HttpGet]
        public IActionResult searchApartments(int id)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("OrderViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dt);
                sqlConnection.Close();
            }
            int Order_ID = (int)dt.Rows[id]["Order_ID"];
            int Hotel_ID = (int)dt.Rows[id]["Hotel_ID"];
            int Number_of_beds = (int)dt.Rows[id]["Number_of_beds"];
            string Apartment_class = dt.Rows[id]["Apartament_class"].ToString();
            DataTable dtbl = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("searchApartments", sqlConnection);
                sqlDa.SelectCommand.Parameters.AddWithValue("Hotel_ID", Hotel_ID);
                sqlDa.SelectCommand.Parameters.AddWithValue("Number_of_beds", Number_of_beds);
                sqlDa.SelectCommand.Parameters.AddWithValue("Apartment_class", Apartment_class);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
                sqlConnection.Close();
                dtbl.Columns.Add("Order_ID", typeof(System.Int32));
                foreach(DataRow row in dtbl.Rows)
                {
                    row["Order_ID"] = Order_ID;
                }
                
            }
            if (dtbl.Rows.Count ==0)
            {
                return RedirectToAction("Orders", "Order", new { idOrder = Order_ID, idStatus = 0 } );
            }
            else  return View(dtbl);
        }

    }
}
