using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Advisor
{
    public class AdvisorProfileView : BaseAccount
    {
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
    }
}
