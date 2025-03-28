using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("award_category")]
    public class AwardCategory : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Type { get; set; }  // "individual" or "team"

        [Required]
        public int SponsorId { get; set; }

        [MaxLength(255)]
        public string? IntroductionVideo { get; set; }

        public string? IntroductionParagraph { get; set; }

        [Required]
        [MaxLength(50)]
        public required string ProfileStatus { get; set; }  // "draft" or "published"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // If the category profile is published, ensure that IntroductionVideo, IntroductionParagraph, and Title are provided.
            if (!string.IsNullOrWhiteSpace(ProfileStatus) &&
                ProfileStatus.Equals("published", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(IntroductionVideo))
                {
                    yield return new ValidationResult(
                        "Introduction video is required when the category profile is published.",
                        new[] { nameof(IntroductionVideo) });
                }
                if (string.IsNullOrWhiteSpace(IntroductionParagraph))
                {
                    yield return new ValidationResult(
                        "Introduction paragraph is required when the category profile is published.",
                        new[] { nameof(IntroductionParagraph) });
                }
            }
        }
    }
}
