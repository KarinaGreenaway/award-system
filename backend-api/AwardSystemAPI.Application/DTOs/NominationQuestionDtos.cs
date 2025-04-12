using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class NominationQuestionCreateDto: IValidatableObject
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression("text|yes_no|multiple_choice", ErrorMessage = "Invalid response type.")]
    public string ResponseType { get; set; } = "text";

    public List<string>? Options { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) 
    {
        if (ResponseType == "multiple_choice")
        {
            if (Options == null || Options.Count < 2)
                yield return new ValidationResult(
                    "At least two options are required for multiple choice.",
                    new[] { nameof(Options) });
            else if (Options.Any(string.IsNullOrWhiteSpace))
                yield return new ValidationResult(
                    "Multiple choice options cannot be empty.",
                    new[] { nameof(Options) });
        }
        else
        {
            if (Options != null && Options.Any())
                yield return new ValidationResult(
                    "Options may only be provided for multiple choice questions.",
                    new[] { nameof(Options) });
        }
    }
}

public class NominationQuestionUpdateDto: IValidatableObject
{
    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression("text|yes_no|multiple_choice", ErrorMessage = "Invalid response type.")]
    public string ResponseType { get; set; } = "text";

    public List<string>? Options { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) 
    {
        if (ResponseType == "multiple_choice")
        {
            if (Options == null || Options.Count < 2)
                yield return new ValidationResult(
                    "At least two options are required for multiple choice.",
                    new[] { nameof(Options) });
        }
        else if (Options != null && Options.Any())
        {
            yield return new ValidationResult(
                "Options may only be provided for multiple choice questions.",
                new[] { nameof(Options) });
        }
    }
}

public class NominationQuestionResponseDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string ResponseType { get; set; } = "text";
    public List<string>? Options { get; set; }
}