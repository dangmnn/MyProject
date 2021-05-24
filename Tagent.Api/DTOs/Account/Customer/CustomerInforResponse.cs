using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tagent.Api.DTOs.Account.Customer
{
    public class CustomerInforResponse
    {
        public string Email { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Phone { get; set; }

        public string Image { get; set; }

        public string CompanyName { get; set; }

        public string CompanyEmail { get; set; }

        public int Size { get; set; }

        public string Address { get; set; }

        public string CompanyPhone { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public string AreaOfExperties { get; set; }

        public string CoreTech { get; set; }

        public string CertificateOfIncoporation { get; set; }

        public string BusinessType { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ZipCode { get; set; }

        public string Logo { get; set; }

        public string WebSite { get; set; }
    }
}
