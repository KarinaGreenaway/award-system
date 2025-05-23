using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("awardEvent")]
    public class AwardEvent
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AwardProcessId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Location { get; set; }

        [Required]
        public DateTime EventDateTime { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Directions { get; set; }
        
        public string? FeedbackSummary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}