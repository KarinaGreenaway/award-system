using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("feedback")]
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<FeedbackResponse> Answers { get; set; } = new List<FeedbackResponse>();
    }
}