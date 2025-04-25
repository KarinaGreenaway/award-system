using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AwardSystemAPI.Domain.Entities;

[Table("nomineeSummary")]
public class NomineeSummary
{
    [Key]
    public int Id { get; set; }

    public int? NomineeId { get; set; }
    
    [ForeignKey("NomineeId")]
    public Users? Nominee { get; set; }
    
    public int? TeamNominationId { get; set; }
    
    [ForeignKey("TeamNominationId")]
    public Nomination? TeamNomination { get; set; }

    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Location { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int? TotalNominations { get; set; }

    public bool IsPinned { get; set; } = false;

    public bool IsShortlisted { get; set; } = false;

    public bool IsWinner { get; set; } = false;
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}