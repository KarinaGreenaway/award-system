using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("team_member")]
public class TeamMember
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NominationId { get; set; }

    [Required]
    public int UserId { get; set; }
}