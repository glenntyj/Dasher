using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Dasher.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Username cannot be empty!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Empty password not allowed!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}