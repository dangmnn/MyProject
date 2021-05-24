using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account.Create
{
    public class CreateAccountRequest : BaseAccountRequest
    {
        public string Image { get; set; }
    }
}
