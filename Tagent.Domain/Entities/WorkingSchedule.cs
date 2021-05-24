using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tagent.Domain.Entities
{
    [Table("WorkingSchedule")]
    public class WorkingSchedule
    {
        [Key]
        public int Id { get; set; }

        public string Time { get; set; }


        public int AdvisorId { get; set; }
        [ForeignKey(nameof(AdvisorId))]
        public Advisor Advisor { get; set; }
    }
}
