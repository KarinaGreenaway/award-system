using AwardSystemAPI.Domain.Enums;

namespace AwardSystemAPI.Application.DTOs;

public class FeedbackFormQuestionResponseDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}

public class FeedbackFormQuestionCreateDto
{
    public int EventId { get; set; }
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}

public class FeedbackFormQuestionUpdateDto
{
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}