using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AwardSystemAPI.Domain.Enums;

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
    public ResponseType ResponseType { get; set; }
    
    public List<string>? Options { get; set; }

    [MaxLength(500)]
    public string? Tooltip { get; set; }

    public int? QuestionOrder { get; set; }
}