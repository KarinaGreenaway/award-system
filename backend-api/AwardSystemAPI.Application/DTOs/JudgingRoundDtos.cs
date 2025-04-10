using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class JudgingRoundCreateDto: IValidatableObject
{
    [Required]
    public int AwardProcessId { get; set; }
        
    [Required]
    [MaxLength(255)]
    public string RoundName { get; set; } = string.Empty;
        
    [Required]
    public DateTime StartDate { get; set; }
        
    [Required]
    public DateTime Deadline { get; set; }
        
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Candidate count must be at least 1.")]
    public int CandidateCount { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate >= Deadline)
        {
            yield return new ValidationResult("Start date must be before the deadline.", [nameof(StartDate), nameof(Deadline)
            ]);
        }
    }
}

public class JudgingRoundUpdateDto: IValidatableObject
{
    [Required]
    [MaxLength(255)]
    public string RoundName { get; set; } = string.Empty;
        
    [Required]
    public DateTime StartDate { get; set; }
        
    [Required]
    public DateTime Deadline { get; set; }
        
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Candidate count must be at least 1.")]
    public int CandidateCount { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate >= Deadline)
        {
            yield return new ValidationResult("Start date must be before the deadline.", new[] { nameof(StartDate), nameof(Deadline) });
        }
    }
}

public class JudgingRoundResponseDto
{
    public int Id { get; set; }
    public int AwardProcessId { get; set; }
    public string RoundName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public int CandidateCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}