using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tagent.Api.DTOs.Account.Create;

namespace Tagent.Api.DTOs.Account.Advisor
{
    public class AdvisorAccountRequest : BaseAccountRequest
    {
        public int Age { get; set; }

        public string Gender { get; set; }
    }
}
