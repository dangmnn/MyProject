using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account.Advisor
{
    public class AdvisorAccountResponse 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
