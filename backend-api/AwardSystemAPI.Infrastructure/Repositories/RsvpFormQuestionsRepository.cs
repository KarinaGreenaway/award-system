using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IRsvpFormQuestionsRepository: IGenericRepository<RsvpFormQuestion>
{ 
    Task<IEnumerable<RsvpFormQuestion>> GetByAwardCategory(int awardEventId);
}

public class RsvpFormQuestionsRepository: GenericRepository<RsvpFormQuestion>, IRsvpFormQuestionsRepository
{
    private readonly ILogger<GenericRepository<RsvpFormQuestion>> _logger;

    public RsvpFormQuestionsRepository(AppDbContext context, ILogger<GenericRepository<RsvpFormQuestion>> logger) : base(
        context, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<RsvpFormQuestion>> GetByAwardCategory(int awardEventId)
    {
        try
        {
            return await Context.Set<RsvpFormQuestion>()
                .Where(q => q.EventId == awardEventId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving RsvpFormQuestions for AwardEvent ID {AwardEventId}.", awardEventId);
            throw new Exception($"An error occurred while retrieving RsvpFormQuestions for AwardEvent ID {awardEventId}.", ex);
        }
    }
}