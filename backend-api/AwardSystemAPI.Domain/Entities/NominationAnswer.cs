using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AwardSystemAPI.Domain.Entities;

[Table("nominationAnswer")]
[Index(nameof(NominationId), nameof(QuestionId), IsUnique = true)]
public class NominationAnswer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NominationId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [MaxLength(1000)]
    public string? Answer { get; set; } = string.Empty;
}