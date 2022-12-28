using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ApartmentViewModel
    {
        [Key]
        public int Room_ID { get; set; }
        [Required]
        public int Hotel_ID { get; set; }
        [Required]
        public string Room { get; set; }
        [Required]
        public string Class { get; set; }
        [Range(1,4,ErrorMessage = "Should be from 1 to 4")]
        public int Amount_beds { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Should be greated or equal to 1")]
        public int Price{ get; set; }

        public static int ChooseHotelID { get; set; }
    }
}
