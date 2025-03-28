using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("judging_round")]
    public class JudgingRound : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AwardProcessId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string RoundName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public int CandidateCount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Deadline <= StartDate)
            {
                yield return new ValidationResult(
                    "Deadline must be later than the Start Date.",
                    new[] { nameof(Deadline), nameof(StartDate) });
            }
        }
    }
}