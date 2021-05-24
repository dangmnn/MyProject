using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tagent.Domain.Entities
{
    [Table("AccountRole")]
    public class AccountRole
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

    }
}
