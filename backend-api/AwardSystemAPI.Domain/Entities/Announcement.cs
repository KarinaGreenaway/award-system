using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("announcement")]
    public class Announcement : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = null!;

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        public bool IsPushNotification { get; set; }

        public DateTime? ScheduledTime { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; } // "draft" or "published"

        [Required]
        [MaxLength(50)]
        public required string Type { get; set; }    // "category", "sponsor"

        public int? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // If the announcement is published, ensure Title, Description, and Type are provided.
            if (Status.Equals("published", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(Title))
                {
                    yield return new ValidationResult(
                        "Title is required when the announcement is published.",
                        new[] { nameof(Title) });
                }
                if (string.IsNullOrWhiteSpace(Description))
                {
                    yield return new ValidationResult(
                        "Description is required when the announcement is published.",
                        new[] { nameof(Description) });
                }
                if (string.IsNullOrWhiteSpace(Type))
                {
                    yield return new ValidationResult(
                        "Type is required when the announcement is published.",
                        new[] { nameof(Type) });
                }
            }
        }
    }
}