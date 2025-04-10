using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs.AwardCategoryDtos;

public class AwardCategoryCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
        
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
        
    [Required]
    public int SponsorId { get; set; }
        
    [MaxLength(255)]
    public string? IntroductionVideo { get; set; }
        
    public string? IntroductionParagraph { get; set; }
        
    [Required]
    [MaxLength(50)]
    public string ProfileStatus { get; set; } = "draft";
}