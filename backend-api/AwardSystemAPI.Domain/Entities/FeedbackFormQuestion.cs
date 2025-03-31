using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("feedbackFormQuestion")]
public class FeedbackFormQuestion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }  // References award_event table

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string ResponseType { get; set; } = null!;  // "text", "yes/no", "multiple choice"

    [MaxLength(500)]
    public string? Tooltip { get; set; }

    public int? QuestionOrder { get; set; }
}