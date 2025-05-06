using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities
{
    [Table("awardProcess")]
    public class AwardProcess
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string AwardsName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}