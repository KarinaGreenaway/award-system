namespace AwardSystemAPI.Application.DTOs.AwardProcessDtos;

public class AwardProcessResponseDto
{
    public int Id { get; set; }
    
    public string AwardsName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}