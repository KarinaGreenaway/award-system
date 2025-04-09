using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class AwardProcessCreateDto: IValidatableObject
{
    [Required]
    public string AwardsName { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult("EndDate must be after StartDate.", [nameof(EndDate)]);
        }
    }
}