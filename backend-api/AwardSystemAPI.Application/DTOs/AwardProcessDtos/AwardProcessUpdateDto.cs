using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs.AwardProcessDtos;

public class AwardProcessUpdateDto: IValidatableObject
{
    [Required]
    public string AwardsName { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult("EndDate must be after StartDate.", [nameof(EndDate)]);
        }
    }
}