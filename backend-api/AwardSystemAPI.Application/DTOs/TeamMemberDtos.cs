using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class TeamMemberCreateDto
{
    [Required]
    public int UserId { get; set; }
}

public class TeamMemberUpdateDto
{
    [Required]
    public int NominationId { get; set; }

    [Required]
    public int UserId { get; set; }
}

public class TeamMemberResponseDto
{
    public int Id { get; set; }
    public int NominationId { get; set; }
    public int UserId { get; set; }
    public string? TeamMemberName { get; set; }
}