using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    internal class UserModel
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
    }
}
