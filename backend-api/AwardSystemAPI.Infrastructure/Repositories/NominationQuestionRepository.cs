using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface INominationQuestionRepository : IGenericRepository<NominationQuestion>
{
    Task<IEnumerable<NominationQuestion>> GetByCategoryIdAsync(int categoryId);
}

public class NominationQuestionRepository : GenericRepository<NominationQuestion>, INominationQuestionRepository
{
    public NominationQuestionRepository(AppDbContext context, ILogger<GenericRepository<NominationQuestion>> logger)
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<NominationQuestion>> GetByCategoryIdAsync(int categoryId)
    {
        try
        {
            return await Context.Set<NominationQuestion>()
                .Where(q => q.CategoryId == categoryId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving nomination questions for category {categoryId}", ex);
        }
    }
}