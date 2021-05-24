using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tagent.Domain.Entities
{
    [Table("Verifier")]
    public  class Verifier
    {
        [Key]
        public int Id { get; set; }

        public int Age { get; set; }

        [StringLength(255)]
        public string Gender { get; set; }

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
    }
}
