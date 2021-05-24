using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tagent.Domain.Entities
{
    [Table("Account")]
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(255)]
        public string Firstname { get; set; }

        [StringLength(255)]
        public string Lastname { get; set; }

        [StringLength(255)]
        public string Phone { get; set; }

        [StringLength(1050)]
        public string Image { get; set; }

        public Advisor Advisor { get; set; }
        public virtual ICollection<AccountRole> AccountRoles { get; set; }
    }
}