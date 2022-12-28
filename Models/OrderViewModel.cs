using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class OrderViewModel
    {
        [Key]
        public int Order_ID { get; set; }
        public int User_ID{ get; set; }  
        public int Hotel_ID { get; set; }
        [Required]
        public string Booking_date { get; set; }
        [Required]
        public int Number_of_beds { get; set; }
        [Required]
        public string Apartament_class { get; set; }
        [Required]
        public DateTime Arrival_time { get; set; }
        [Required]
        public int Days { get; set; }
        public string Status_ID { get; set; }

        public static string ImagePath { get; set; }
        public static int idHotel { get; set; }
    }
}
