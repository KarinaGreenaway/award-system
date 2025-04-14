using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IAnnouncementRepository : IGenericRepository<Announcement>
{
    Task<IEnumerable<Announcement>> GetByAudienceAsync(TargetAudience audience, string status);
    Task<IEnumerable<Announcement>> GetByCreatedByAsync(int userId);
    Task<IEnumerable<Announcement>> GetScheduledToPublishAsync(DateTime asOfUtc);
}
    
public class AnnouncementRepository 
    : GenericRepository<Announcement>, IAnnouncementRepository
{
    private readonly ILogger<GenericRepository<Announcement>> _logger;
    
    public AnnouncementRepository(AppDbContext context, ILogger<GenericRepository<Announcement>> logger)
        : base(context, logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Announcement>> GetByAudienceAsync(TargetAudience audience, string status)
    {
        try
        {
            return await Context.Set<Announcement>()
                .Where(a => a.Audience == audience && a.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcements for audience {Audience} with status {Status}.", audience, status);
            throw new Exception($"Error retrieving announcements for {audience} with status {status}.", ex);
        }
    }

    public async Task<IEnumerable<Announcement>> GetByCreatedByAsync(int userId)
    {
        try
        {
            return await Context.Set<Announcement>()
                .Where(a => a.CreatedBy == userId && a.Audience == TargetAudience.MobileUsers)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcements created by user {UserId}.", userId);
            throw new Exception($"Error retrieving announcements created by user {userId}.", ex);
        }
    }

    public async Task<IEnumerable<Announcement>> GetScheduledToPublishAsync(DateTime asOfUtc)
    {
        return await Context.Set<Announcement>()
            .Where(a => a.Status.Equals("draft", StringComparison.OrdinalIgnoreCase)
                        && a.ScheduledTime.HasValue
                        && a.ScheduledTime.Value <= asOfUtc)
            .ToListAsync()
            .ConfigureAwait(false);
    }
}