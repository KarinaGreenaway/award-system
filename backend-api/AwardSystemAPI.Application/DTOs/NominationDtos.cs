using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class NominationCreateDto : IValidatableObject
{
    [Required]
    public int CategoryId { get; set; }
    
    public int? NomineeId { get; set; }
    
    [MaxLength(255)]
    public string? TeamName { get; set; }
    
    public List<TeamMemberCreateDto>? TeamMembers { get; set; } = new();
    
    [Required]
    public List<NominationAnswerResponseDto> Answers { get; set; } = new();
    
    [MaxLength(50)]
    public string? Location { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NomineeId.HasValue)
        {
            if (!string.IsNullOrWhiteSpace(TeamName))
            {
                yield return new ValidationResult("Team name should not be provided for individual nominations.", new[] { nameof(TeamName) });
            }
            if (TeamMembers != null && TeamMembers.Any())
            {
                yield return new ValidationResult("Team member ids should not be provided for individual nominations.", new[] { nameof(TeamMembers) });
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(TeamName))
            {
                yield return new ValidationResult("Team name is required for team nominations.", new[] { nameof(TeamName) });
            }
            if (TeamMembers == null || !TeamMembers.Any())
            {
                yield return new ValidationResult("At least one team member id is required for team nominations.", new[] { nameof(TeamMembers) });
            }
        }
    }}

public class NominationAnswerCreateDto
{
    [Required]
    public string Question { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Answer { get; set; } = string.Empty;
}

public class NominationResponseDto
{
    public int Id { get; set; }
    public int CreatorId { get; set; }
    public int CategoryId { get; set; }
    public int? NomineeId { get; set; }
    public string? NomineeName { get; set; }
    public string? TeamName { get; set; }
    public string? AiSummary { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public List<NominationAnswerResponseDto> Answers { get; set; } = new();
    public List<TeamMemberResponseDto> TeamMembers { get; set; } = new();
    
}

public class NominationAnswerResponseDto
{
    public string Question { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class NominationUpdateDto : IValidatableObject
{
    // The type of nomination (team vs. individual) isn't changed via update.
    public string? TeamName { get; set; }
    public IEnumerable<TeamMemberUpdateDto>? TeamMembers { get; set; } = new List<TeamMemberUpdateDto>();
    public string? NomineeName { get; set; }
    [Required]
    public IEnumerable<NominationAnswerUpdateDto> Answers { get; set; } = new List<NominationAnswerUpdateDto>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Validate based on whether this nomination is team-based.
        if (!string.IsNullOrWhiteSpace(TeamName))
        {
            // Team nomination.
            if (TeamMembers == null || !TeamMembers.Any())
            {
                yield return new ValidationResult("At least one team member must be provided for team nominations.", new[] { nameof(TeamMembers) });
            };

            if (string.IsNullOrWhiteSpace(TeamName))
            {
                yield return new ValidationResult("Team name is required for team nominations.", new[] { nameof(TeamName) });
            }; 
        }
        else
        {
            // Individual nomination.
            if (string.IsNullOrWhiteSpace(NomineeName))
            {
                yield return new ValidationResult("Nominee name is required for individual nominations.", new[] { nameof(NomineeName) });
            }
        } 
    } 
} 
public class NominationAnswerUpdateDto
{
    [Required]
    public string Question { get; set; }

    [Required]
    public string Answer { get; set; } = string.Empty;
}
