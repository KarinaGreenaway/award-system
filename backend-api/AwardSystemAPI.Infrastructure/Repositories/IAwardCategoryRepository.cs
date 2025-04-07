using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IAwardCategoryRepository : IGenericRepository<AwardCategory>
{
    Task<IEnumerable<AwardCategory>> GetBySponsorIdAsync(int sponsorId);
}