using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("feedbackResponse")]
    [Index(nameof(FeedbackId), nameof(Question), IsUnique = true)]
    public class FeedbackResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FeedbackId { get; set; }

        [Required]
        public string Question { get; set; }

        [MaxLength(1000)]
        public string? Answer { get; set; } = string.Empty;
    }
}