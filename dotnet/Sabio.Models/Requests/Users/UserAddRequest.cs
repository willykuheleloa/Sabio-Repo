using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Users
    {
    public class UserAddRequest
        {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$")]
        public string Email { get; set; }
        [Required]
        public string AvatarUrl { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required] 
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Password Confirm Must Match")]
        public string PasswordConfirm { get; set; }
        [Required]
        public string TenantId { get; set; }
        }
    }
