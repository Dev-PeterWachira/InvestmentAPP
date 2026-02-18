using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Investment.API.Data;

namespace Investment.API.Models
{
    public class User
    {
        public int Id {get; set;}

        [Required]

        public string Email {get; set;} = string.Empty;
        [Required]
        public string FullName{get; set;} = string.Empty;
        [Required]
        public string phoneNumber{get;set;} = string.Empty;

        public string PasswordHash {get; set;} = string.Empty;

        public string Role {get; set;} = "Member";

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        public ICollection<Contribution> Contributions {get; set;} = new List<Contribution>();

    }
}