using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tagent.Domain.Entities
{
    [Table("Agency")]
    public class Agency
    {
        [Key]
        public int Id { get; set; }

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
