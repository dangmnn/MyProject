using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account.Advisor
{
    public class AdvisorInforReponse
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
