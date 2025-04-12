using System.Text;
using AwardSystemAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IAiSummaryService
{
    Task<string> GenerateNominationSummaryAsync(Nomination nomination, IEnumerable<NominationAnswer> answers);
}

public class AiSummaryService : IAiSummaryService
{
    private readonly ILogger<AiSummaryService> _logger;

    public AiSummaryService(ILogger<AiSummaryService> logger)
    {
        _logger = logger;
    }

    public Task<string> GenerateNominationSummaryAsync(Nomination nomination, IEnumerable<NominationAnswer> answers)
    {
        // TODO:Placeholder logic —  later call a real AI API here.

        var sb = new StringBuilder();
        sb.AppendLine("Summary of Nomination:");

        if (!string.IsNullOrWhiteSpace(nomination.TeamName))
            sb.AppendLine($"Team: {nomination.TeamName}");

        if (nomination.NomineeId != null)
            sb.AppendLine($"Nominee ID: {nomination.NomineeId}");

        sb.AppendLine("Key Points:");
        foreach (var answer in answers.Take(3)) // Take first 3 answers for brevity
        {
            sb.AppendLine($"- Q{answer.QuestionId}: {answer.Answer}");
        }

        return Task.FromResult(sb.ToString());
    }
}