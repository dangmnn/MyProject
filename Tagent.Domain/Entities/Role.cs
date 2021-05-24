using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tagent.Domain.Entities
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<AccountRole> AccountRoles { get; set; }
    }
}
