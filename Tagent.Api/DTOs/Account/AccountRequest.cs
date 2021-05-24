using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account
{
    public class AccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
