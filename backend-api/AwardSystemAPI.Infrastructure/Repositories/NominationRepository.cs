using AwardSystemAPI.Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface INominationRepository : IGenericRepository<Nomination>
{
    Task<IEnumerable<Nomination>> GetNominationsByCreatorIdAsync(int creatorId); 
    Task<IEnumerable<Nomination>> GetTeamNominationsForMemberAsync(int userId);
    Task<IEnumerable<Nomination>> GetNominationsForNomineeIdAsync(int nomineeId);
    Task<Nomination?> GetNominationByIdAsync(int id);
    Task<IEnumerable<Nomination>> GetNominationsByCategoryIdAsync(int categoryId);
}

public class NominationRepository : GenericRepository<Nomination>, INominationRepository
{
    private readonly ILogger<GenericRepository<Nomination>> _logger;
    
    public NominationRepository(AppDbContext context, ILogger<GenericRepository<Nomination>> logger) : base(context,
        logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Nomination>> GetNominationsByCreatorIdAsync(int creatorId)
    {
        try
        {
            return await Context.Set<Nomination>()
                .Include(n => n.TeamMembers)
                .Include(n => n.Answers)
                .Where(n => n.CreatorId == creatorId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving nominations for creator ID {CreatorId}.", creatorId);
            throw new Exception($"An error occurred while retrieving nominations for creator with ID {creatorId}.",
                ex);
        }
    }

    public async Task<IEnumerable<Nomination>> GetTeamNominationsForMemberAsync(int userId)
    {
        try
        {
            return await Context.Set<Nomination>()
                .Include(n => n.Answers)
                .Include(n => n.TeamMembers)
                .Where(n => n.TeamMembers.Any(tm => tm.UserId == userId))
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving team nominations for member ID {UserId}.", userId);
            throw new Exception($"An error occurred while retrieving team nominations for member with ID {userId}.",
                ex);
        }
    }

    public async Task<IEnumerable<Nomination>> GetNominationsForNomineeIdAsync(int nomineeId)
    {
        try
        {
            return await Context.Set<Nomination>()
                .Include(n => n.Answers)
                .Include(n => n.TeamMembers)
                .Where(n => n.NomineeId == nomineeId || n.TeamMembers.Any(tm => tm.Id == nomineeId))
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving nominations for nominee ID {NomineeId}.", nomineeId);
            throw new Exception($"An error occurred while retrieving nominations for nominee with ID {nomineeId}.",
                ex);
        }
    }

    public async Task<Nomination?> GetNominationByIdAsync(int id)
    {
        try
        {
            return await Context.Set<Nomination>()
                .Include(n => n.Answers)
                .Include(n => n.TeamMembers)
                .FirstOrDefaultAsync(n => n.Id == id)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving nomination with ID {Id}.", id);
            throw new Exception($"An error occurred while retrieving nomination with ID {id}.", ex);
        }
    }
    
    public async Task<IEnumerable<Nomination>> GetNominationsByCategoryIdAsync(int categoryId)
    {
        try
        {
            return await Context.Set<Nomination>()
                .Include(n => n.Answers)
                .Include(n => n.TeamMembers)
                .Where(n => n.CategoryId == categoryId)
                .ToListAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving nominations for category ID {CategoryId}.", categoryId);
            throw new Exception($"An error occurred while retrieving nominations for category with ID {categoryId}.",
                ex);
        }
    }
}