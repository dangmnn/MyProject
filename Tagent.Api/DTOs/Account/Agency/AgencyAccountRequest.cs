using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tagent.Api.DTOs.Account.Create;

namespace Tagent.Api.DTOs.Account.Agency
{
    public class AgencyAccountRequest : BaseAccountRequest
    {
        public string CompanyName { get; set; }
    }
}
