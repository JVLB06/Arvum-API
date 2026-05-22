using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ConnectionDTO
    {
        public int Id {  get; set; }
        public string Email { get; set; }
        public bool? Authenticated { get; set; }
    }
}
