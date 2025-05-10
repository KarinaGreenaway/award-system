using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IRsvpRepository: IGenericRepository<Rsvp>
{
    Task<int> CountByAwardEventId(int awardEventId);
}

public class RsvpRepository: GenericRepository<Rsvp>, IRsvpRepository
{
    private readonly ILogger<GenericRepository<Rsvp>> _logger;

    public RsvpRepository(AppDbContext context, ILogger<GenericRepository<Rsvp>> logger) : base(context, logger)
    {
        _logger = logger;
    }
    
    public async Task<int> CountByAwardEventId(int awardEventId)
    {
        try
        {
            return await Context.Set<Rsvp>()
                .CountAsync(r => r.EventId == awardEventId)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting RSVPs for AwardEvent ID {AwardEventId}.", awardEventId);
            throw new Exception($"An error occurred while counting RSVPs for AwardEvent ID {awardEventId}.", ex);
        }
    }
}