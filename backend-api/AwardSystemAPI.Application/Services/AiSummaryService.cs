using System.Text;
using AwardSystemAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AwardSystemAPI.Application.Services;

public interface IAiSummaryService
{
    Task<string> GenerateNominationSummaryAsync(string promptData);
    Task<string> GenerateAiFeedbackSummaryAsync(string feedbackJson);
}

public class AiSummaryService : IAiSummaryService
{
    private static readonly HttpClient client = new HttpClient();
    private readonly IVertexAiService _vertexAiService;
    private readonly ILogger<AiSummaryService> _logger;

    public AiSummaryService(ILogger<AiSummaryService> logger, IVertexAiService vertexAiService)
    {
        _logger = logger;
        _vertexAiService = vertexAiService;
    }

    public async Task<string> GenerateNominationSummaryAsync(string nominationJson)
    {
        return await _vertexAiService.GenerateNominationSummaryAsync(nominationJson);
    }
    
    public async Task<string> GenerateAiFeedbackSummaryAsync(string feedbackJson)
    {
        return await _vertexAiService.GenerateFeedbackSummaryAsync(feedbackJson);
    }
}