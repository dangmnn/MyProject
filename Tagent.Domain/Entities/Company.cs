using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tagent.Domain.Entities
{
    [Table("Company")]
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public int Size { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(12)]
        public string Phone { get; set; }

        public int Year { get; set; }

        [StringLength(1050)]
        public string Description { get; set; }

        [StringLength(255)]
        public string AreaOfExperties { get; set; }

        [StringLength(510)]
        public string CoreTech { get; set; }

        [StringLength(510)]
        public string CertificateOfIncoporation { get; set; }

        [StringLength(255)]
        public string BusinessType { get; set; }

        [StringLength(255)]
        public string Country { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        public int ZipCode { get; set; }

        [StringLength(510)]
        public string Logo { get; set; }

        [StringLength(255)]
        public string WebSite { get; set; }

        public Customer Customer { get; set; }

        public Agency Agency { get; set; }
    }
}
