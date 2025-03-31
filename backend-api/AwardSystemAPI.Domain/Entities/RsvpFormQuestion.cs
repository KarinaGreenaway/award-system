using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("rsvpFormQuestion")]
public class RsvpFormQuestion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string QuestionText { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string ResponseType { get; set; } = null!;  // 'text', 'yes/no', 'multiple choice'

    [MaxLength(500)]
    public string? Tooltip { get; set; }

    public int? QuestionOrder { get; set; }
}