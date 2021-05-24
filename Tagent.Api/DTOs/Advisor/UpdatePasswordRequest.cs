using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Advisor
{
    public class UpdatePasswordRequest
    {
        public int Id { get; set; }
        public string Password { get; set; }
    }
}
