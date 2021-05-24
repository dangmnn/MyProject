using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tagent.Domain.Entities
{
    [Table("Booking")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }


        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int AdvisorId { get; set; }
        [ForeignKey(nameof(AdvisorId))]
        public Advisor Advisor { get; set; }

        public DateTime Date { get; set; }

        public int WorkingScheduleId { get; set; }

    }
}
