using System.ComponentModel.DataAnnotations;
using AwardSystemAPI.Domain.Enums;

namespace AwardSystemAPI.Application.DTOs;

public class NominationQuestionCreateDto: IValidatableObject
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(500)]
    public string QuestionText { get; set; } = string.Empty;
    
    [Required]
    public ResponseType ResponseType { get; set; }

    public List<string>? Options { get; set; }

    [Required]
    public int QuestionOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) 
    {
        if (ResponseType == Domain.Enums.ResponseType.MultipleChoice)
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
    public ResponseType ResponseType { get; set; }

    public List<string>? Options { get; set; }

    public int QuestionOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _) 
    {
        if (ResponseType == Domain.Enums.ResponseType.MultipleChoice)
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
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public int QuestionOrder { get; set; }
}