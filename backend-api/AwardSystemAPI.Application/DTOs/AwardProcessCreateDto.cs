using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class AwardProcessCreateDto
{
    [Required]
    public string AwardsName { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}