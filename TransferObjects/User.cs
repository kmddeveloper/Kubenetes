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
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
