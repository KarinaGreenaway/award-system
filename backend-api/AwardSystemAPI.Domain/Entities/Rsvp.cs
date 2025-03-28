using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("rsvp")]
    public class Rsvp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;  // 'attending', 'not attending'

        [Required]
        public DateTime RsvpDate { get; set; } = DateTime.UtcNow;
    }
}