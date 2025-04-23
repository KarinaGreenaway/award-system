using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface INomineeSummaryRepository : IGenericRepository<NomineeSummary>
{
    Task<NomineeSummary?> GetByNomineeIdAndCategoryIdAsync(int nomineeId, int categoryId);
    Task<NomineeSummary?> GetByTeamNominationIdAndCategoryIdAsync(int nomineeId, int categoryId);
    Task<IEnumerable<NomineeSummary?>> GetByCategoryIdAsync(int categoryId);
    Task<int> CountIndividualNominationsForNomineeAsync(int nomineeId, int categoryId);
}

public class NomineeSummaryRepository : GenericRepository<NomineeSummary>, INomineeSummaryRepository
{
    private readonly ILogger<GenericRepository<NomineeSummary>> _logger;
    
    public NomineeSummaryRepository(AppDbContext context, ILogger<GenericRepository<NomineeSummary>> logger) : base(
        context, logger)
    {
        _logger = logger;
    }
    public async Task<NomineeSummary?> GetByNomineeIdAndCategoryIdAsync(int nomineeId, int categoryId)
    {
        try
        {
            return await Context.Set<NomineeSummary>()
            .FirstOrDefaultAsync(ns => ns.NomineeId == nomineeId && ns.CategoryId == categoryId)
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving NomineeSummary for Nominee ID {NomineeId} and Category ID {CategoryId}.", nomineeId, categoryId);
            throw new Exception($"An error occurred while retrieving NomineeSummary for Nominee ID {nomineeId} and Category ID {categoryId}.", ex);
        }
    }
    public async Task<NomineeSummary?> GetByTeamNominationIdAndCategoryIdAsync(int teamNominationId, int categoryId)
    {
        try
        {
            return await Context.Set<NomineeSummary>()
            .FirstOrDefaultAsync(ns => ns.TeamNominationId == teamNominationId && ns.CategoryId == categoryId)
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving NomineeSummary for Team Nomination ID {teamNominationId} and Category ID {CategoryId}.", teamNominationId, categoryId);
            throw new Exception($"An error occurred while retrieving NomineeSummary for Team Nomination ID {teamNominationId} and Category ID {categoryId}.", ex);
        }
    }

    public async Task<IEnumerable<NomineeSummary?>> GetByCategoryIdAsync(int categoryId)
    {
        try
        {
            return await Context.Set<NomineeSummary>()
            .Where(ns => ns.CategoryId == categoryId)
            .ToListAsync()
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving NomineeSummaries for Category ID {CategoryId}.", categoryId);
            throw new Exception($"An error occurred while retrieving NomineeSummaries for Category ID {categoryId}.", ex);
        }
    }

    public async Task<int> CountIndividualNominationsForNomineeAsync(int nomineeId, int categoryId)
    {
        try
        {
            return await Context.Set<Nomination>()
            .CountAsync(n => n.NomineeId == nomineeId && n.CategoryId == categoryId)
            .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting nominations for Nominee ID {NomineeId} and Category ID {CategoryId}.", nomineeId, categoryId);
            throw new Exception($"An error occurred while counting nominations for Nominee ID {nomineeId} and Category ID {categoryId}.", ex);
        }
    }
}

