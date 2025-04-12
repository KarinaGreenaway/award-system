using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("nominationQuestion")]
public class NominationQuestion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; } = 0;

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string ResponseType { get; set; } = "text";  // "text", "yes_no", "multiple_choice"

    // JSON-serialized list of options; only used when ResponseType == "multiple_choice"
    public string? Options { get; set; }
}