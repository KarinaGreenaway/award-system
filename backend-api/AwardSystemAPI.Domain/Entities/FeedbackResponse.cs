using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("feedbackResponse")]
    [Index(nameof(FeedbackId), nameof(QuestionId), IsUnique = true)]
    public class FeedbackResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FeedbackId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        public string? Answer { get; set; }
    }
}