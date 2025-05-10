using System.ComponentModel.DataAnnotations;

namespace AwardSystemAPI.Application.DTOs;

public class FeedbackCreateDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int EventId { get; set; }
    
    [Required]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    public List<FeedbackAnswerCreateDto>? Answers { get; set; }
    
}

public class FeedbackResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public List<FeedbackAnswerResponseDto>? Answers { get; set; }
}

public class FeedbackUpdateDto
{
    public int UserId { get; set; }
    public int EventId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public List<FeedbackAnswerUpdateDto>? Answers { get; set; }
}

public class FeedbackAnswerResponseDto
{
    public string Question { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class FeedbackAnswerUpdateDto
{
    [Required]
    public string Question { get; set; }
    
    [Required]
    public string Answer { get; set; } = string.Empty;
}

public class FeedbackAnswerCreateDto
{
    [Required]
    public string Question { get; set; }
    
    [Required]
    public string Answer { get; set; } = string.Empty;
}
