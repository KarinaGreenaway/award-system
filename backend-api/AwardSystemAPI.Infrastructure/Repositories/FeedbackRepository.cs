using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IFeedbackRepository : IGenericRepository<Feedback>
{
    Task<IEnumerable<Feedback>> GetByAwardEventId(int awardEventId);
    Task<int> CountByAwardEventId(int awardEventId);
}

public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    private readonly ILogger<GenericRepository<Feedback>> _logger;

    public FeedbackRepository(AppDbContext context, ILogger<GenericRepository<Feedback>> logger) : base(
        context, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Feedback>> GetByAwardEventId(int awardEventId)
    {
        try
        {
            return await Context.Set<Feedback>()
                .Include(f => f.Answers)
                .Where(f => f.EventId == awardEventId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving Feedbacks for AwardEvent ID {AwardEventId}.", awardEventId);
            throw new Exception($"An error occurred while retrieving Feedbacks for AwardEvent ID {awardEventId}.", ex);
        }
    }
    
    public async Task<int> CountByAwardEventId(int awardEventId)
    {
        try
        {
            return await Context.Set<Feedback>()
                .CountAsync(f => f.EventId == awardEventId)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting Feedbacks for AwardEvent ID {AwardEventId}.", awardEventId);
            throw new Exception($"An error occurred while counting Feedbacks for AwardEvent ID {awardEventId}.", ex);
        }
    }
}