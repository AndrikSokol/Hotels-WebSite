using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
    public class HotelController : Controller
    {

        private readonly IConfiguration _configuration;
        public HotelController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // GET: Hotel
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("HotelViewAll",sqlConnection);
                sqlDa.SelectCommand.CommandType =CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
                sqlConnection.Close();
            }
            return View(dtbl);
        }



        //GET: Hotel/AddOrEdit/5
        public IActionResult AddOrEdit(int? id)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            HotelViewModel hotelViewModel = new HotelViewModel();
            if (id > 0)
                hotelViewModel = FetchHotelByID(id);
            return View(hotelViewModel);
        }

        // POST: Hotel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("Hotel_ID,Hotel_name,Adress,Phone,Image")] HotelViewModel hotelViewModel)
        {

            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    if (hotelViewModel.Image != null)
                    {
                        var currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\img\hotels");
                        var fileName = Guid.NewGuid().ToString() + ".jpg";
                        var destinationPath = Path.Combine(currentDirectory, fileName);
                        using (var stream = System.IO.File.Create(destinationPath))
                        {
                            hotelViewModel.Image.CopyTo(stream);
                        }

                        hotelViewModel.ImagePath = @"/img/hotels/" + fileName;


                    }
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("HotelAddOrEdit",sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("Hotel_ID", hotelViewModel.Hotel_ID);
                    sqlCmd.Parameters.AddWithValue("Hotel_name", hotelViewModel.Hotel_name);
                    sqlCmd.Parameters.AddWithValue("Adress", hotelViewModel.Adress);
                    sqlCmd.Parameters.AddWithValue("Phone", hotelViewModel.Phone);
                    sqlCmd.Parameters.AddWithValue("ImagePath", hotelViewModel.ImagePath != null ? hotelViewModel.ImagePath : hotelViewModel.ImagePath = " ");
                    sqlCmd.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotelViewModel);
        }

        // GET: Hotel/Delete/5
        public IActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("Role_Name") != "Admin")
                return RedirectToAction("Index", "Home");
            HotelViewModel hotelViewModel = FetchHotelByID(id);
            return View(hotelViewModel);
        }

        // POST: Hotel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("HotelDeleteByID", sqlConnection);
                
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Hotel_ID", id);
                sqlCmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public HotelViewModel FetchHotelByID(int? id)
        {
            HotelViewModel hotelViewModel = new HotelViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("HotelViewByID", sqlConnection);
                sqlDa.SelectCommand.Parameters.AddWithValue("Hotel_ID", id);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                
                sqlDa.Fill(dtbl);
                if(dtbl.Rows.Count ==1)
                {
                    hotelViewModel.Hotel_ID = Convert.ToInt32( dtbl.Rows[0]["Hotel_ID"].ToString());
                    hotelViewModel.Hotel_name = dtbl.Rows[0]["Hotel_name"].ToString();
                    hotelViewModel.Adress = dtbl.Rows[0]["Adress"].ToString();
                    hotelViewModel.Phone = dtbl.Rows[0]["Phone"].ToString();
                    hotelViewModel.ImagePath = dtbl.Rows[0]["ImagePath"].ToString();
                }
                sqlConnection.Close();
                
            }
            return hotelViewModel;
        }
    }
}
