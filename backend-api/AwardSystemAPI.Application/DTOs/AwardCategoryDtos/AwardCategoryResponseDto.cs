namespace AwardSystemAPI.Application.DTOs.AwardCategoryDtos;

public class AwardCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "individual" or "team"
    public int SponsorId { get; set; }
    public string? IntroductionVideo { get; set; }
    public string? IntroductionParagraph { get; set; }
    public string ProfileStatus { get; set; } = string.Empty; // "draft" or "published"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}