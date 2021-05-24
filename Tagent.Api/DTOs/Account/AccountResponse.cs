using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public IList<string> Role { get; set; }
        public string Status { get; set; }
        public AccountResponse()
        {
            Role = new List<string>();
        }
    }
    
}
