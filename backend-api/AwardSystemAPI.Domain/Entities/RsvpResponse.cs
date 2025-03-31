using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("rsvpResponse")]
public class RsvpResponse
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RsvpId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [MaxLength(1000)]
    public string? Answer { get; set; }
}