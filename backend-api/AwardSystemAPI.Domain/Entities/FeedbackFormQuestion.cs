using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AwardSystemAPI.Domain.Enums;

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
    public ResponseType ResponseType { get; set; } // "text", "yes/no", "multiple choice"
    
    public List<string>? Options { get; set; }

    [MaxLength(500)]
    public string? Tooltip { get; set; }

    public int? QuestionOrder { get; set; }
}