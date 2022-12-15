using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomeCookieAuthentication.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [MaxLength(50), Required]
        public string UserName { get; set; }

        [MaxLength(200), Required]
        public string Password { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }

        public ICollection<Role> Roles { get; set; }

    }
}