using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories
{
    public class AwardCategoryRepository : GenericRepository<AwardCategory>, IAwardCategoryRepository
    {
        private readonly ILogger<AwardCategoryRepository> _logger;

        public AwardCategoryRepository(AppDbContext context, ILogger<AwardCategoryRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<AwardCategory>> GetBySponsorIdAsync(int sponsorId)
        {
            try
            {
                return await Context.Set<AwardCategory>()
                    .Where(ac => ac.SponsorId == sponsorId)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving AwardCategory records for SponsorId {SponsorId}.", sponsorId);
                throw new Exception("An error occurred while retrieving AwardCategory records by sponsor id.", ex);
            }
        }
    }
}