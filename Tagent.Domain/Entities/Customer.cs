using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tagent.Domain.Entities
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
