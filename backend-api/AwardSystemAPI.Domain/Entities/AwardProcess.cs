using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("awardProcess")]
    public class AwardProcess : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string AwardsName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }  // "active", "completed"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate.Value < StartDate)
            {
                yield return new ValidationResult(
                    "EndDate must be after StartDate.", [nameof(EndDate)]);
            }
        }
    }
}