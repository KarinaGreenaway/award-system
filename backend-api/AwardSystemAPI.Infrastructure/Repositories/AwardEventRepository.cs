using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IAwardEventRepository : IGenericRepository<AwardEvent>
{
    Task<AwardEvent?> GetByAwardProcessIdAsync(int id);
}
public class AwardEventRepository : GenericRepository<AwardEvent>, IAwardEventRepository
{
    private readonly ILogger<AwardEventRepository> _logger;

    public AwardEventRepository(AppDbContext context, ILogger<AwardEventRepository> logger)
        : base(context, logger)
    {
        _logger = logger;
    }

    public async Task<AwardEvent?> GetByAwardProcessIdAsync(int id)
    {
        try
        {
            return await Context.Set<AwardEvent>()
                .FirstOrDefaultAsync(n => n.AwardProcessId == id)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving AwardEvent records for AwardProcessId {AwardProcessId}.", id);
            throw new Exception("An error occurred while retrieving AwardEvent records by award process id.", ex);
        }
    }
}