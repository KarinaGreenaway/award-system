using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IFeedbackFormQuestionRepository : IGenericRepository<FeedbackFormQuestion>
{
    Task<IEnumerable<FeedbackFormQuestion>> GetByAwardCategory(int awardEventId);
}

public class FeedbackFormQuestionRepository : GenericRepository<FeedbackFormQuestion>, IFeedbackFormQuestionRepository
{
    private readonly ILogger<GenericRepository<FeedbackFormQuestion>> _logger;

    public FeedbackFormQuestionRepository(AppDbContext context, ILogger<GenericRepository<FeedbackFormQuestion>> logger) : base(
        context, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<FeedbackFormQuestion>> GetByAwardCategory(int awardEventId)
    {
        try
        {
            return await Context.Set<FeedbackFormQuestion>()
                .Where(q => q.EventId == awardEventId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving FeedbackFormQuestions for AwardEvent ID {AwardEventId}.", awardEventId);
            throw new Exception($"An error occurred while retrieving FeedbackFormQuestions for AwardEvent ID {awardEventId}.", ex);
        }
    }
}