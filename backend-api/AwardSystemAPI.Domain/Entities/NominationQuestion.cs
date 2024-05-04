using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AwardSystemAPI.Domain.Enums;

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
    public ResponseType ResponseType { get; set; }

    public List<string>? Options { get; set; }
}