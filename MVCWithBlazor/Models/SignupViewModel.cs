using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Models
{
    public class SignupViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email adress is missing or invalid!")]
        public string EMail { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "The password is incorrect or missing!")]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Department { get; set; }
    }
}
