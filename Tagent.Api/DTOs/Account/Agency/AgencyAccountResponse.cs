using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account.Agency
{
    public class AgencyAccountResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
