using AwardSystemAPI.Domain.Enums;

namespace AwardSystemAPI.Application.DTOs;

public class RsvpFormQuestionResponseDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}

public class RsvpFormQuestionCreateDto
{
    public int EventId { get; set; }
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}

public class RsvpFormQuestionUpdateDto
{
    public string QuestionText { get; set; } = null!;
    public ResponseType ResponseType { get; set; }
    public List<string>? Options { get; set; }
    public string? Tooltip { get; set; }
    public int? QuestionOrder { get; set; }
}