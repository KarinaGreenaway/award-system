using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("nomination")]
public class Nomination
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CreatorId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    // NomineeId is optional because for team nominations it's not be provided.
    public int? NomineeId { get; set; }

    [MaxLength(255)]
    public string? TeamName { get; set; }

    // AI summary can be lengthy, so no max length is specified.
    public string? AiSummary { get; set; }

    [MaxLength(50)]
    public string? Location { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<NominationAnswer> Answers { get; set; } = new List<NominationAnswer>();
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}