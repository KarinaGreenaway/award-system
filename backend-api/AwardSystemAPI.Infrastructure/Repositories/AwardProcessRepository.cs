using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IAwardProcessRepository : IGenericRepository<AwardProcess>
{
    Task<AwardProcess?> GetActiveAsync();
}
public class AwardProcessRepository : GenericRepository<AwardProcess>, IAwardProcessRepository
{
    private readonly ILogger<AwardProcessRepository> _logger;

    public AwardProcessRepository(AppDbContext context, ILogger<AwardProcessRepository> logger)
        : base(context, logger)
    {
        _logger = logger;
    }

    public async Task<AwardProcess?> GetActiveAsync()
    {
        try
        {
            return await Context.Set<AwardProcess>()
                .FirstOrDefaultAsync(ac => ac.StartDate <= DateTime.UtcNow && ac.EndDate >= DateTime.UtcNow)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving active AwardProcess records.");
            throw new Exception("An error occurred while retrieving active AwardProcess records.", ex);
        }
    }
}