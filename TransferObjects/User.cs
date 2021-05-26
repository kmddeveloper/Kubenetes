using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kubernetes.TransferObjects
{
    public class User
    {
        [Required(ErrorMessage = "User Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Id has to be greater than 0")]
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 2)]
        public string FirstName { get; set; }
        [StringLength(60, MinimumLength = 2)]
        public string LastName { get; set; }
    }

    public class Registration
    {
        [Required(ErrorMessage = "Email address is required")]
        [StringLength(60, MinimumLength = 2)]
        [EmailAddress(ErrorMessage="Invalid Email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(60, MinimumLength = 2)]
        public string Password { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(60, MinimumLength = 2)]
        public string FirstName { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required(ErrorMessage = "LastName is required")]      
        public string LastName { get; set; }
    }
}
