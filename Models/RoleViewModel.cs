using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class RoleViewModel
    {
        [Key]
        public int Role_ID { get; set; }
        
        public string Role_name { get; set; }
    }
}
