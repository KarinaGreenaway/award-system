using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs
{
    public class AwardEventCreateDto : IValidatableObject
    {
        [Required]
        public int AwardProcessId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;
        
        [Required]
        public DateTime EventDateTime { get; set; }
        
        public string? Description { get; set; }
        
        public string? FeedbackSummary { get; set; }
        
        [MaxLength(500)]
        public string? Directions { get; set; } = string.Empty;
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventDateTime < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Event date and time must be in the future.",
                    new[] { nameof(EventDateTime) }
                );
            }
        }
    }

    public class AwardEventUpdateDto : IValidatableObject
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Location { get; set; } = string.Empty;
        
        [Required]
        public DateTime EventDateTime { get; set; }
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? Directions { get; set; } = string.Empty;
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventDateTime < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Event date and time must be in the future.",
                    new[] { nameof(EventDateTime) }
                );
            }
        }
    }

    public class AwardEventResponseDto
    {
        public int Id { get; set; }
        public int AwardProcessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }
        public string? Description { get; set; }
        public string? Directions { get; set; } = string.Empty;
        
        public string? FeedbackSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
