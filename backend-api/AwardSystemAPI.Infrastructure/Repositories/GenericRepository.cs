using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    private readonly DbSet<T> _dbSet;
    private readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(AppDbContext context, ILogger<GenericRepository<T>> logger)
    {
        Context = context;
        _logger = logger;
        _dbSet = Context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all entities of type {EntityType}.", typeof(T).Name);
            throw new Exception("An error occurred while retrieving all entities.", ex);
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than zero.", nameof(id));

        try
        {
            return await _dbSet.FindAsync(id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the entity with ID {Id} of type {EntityType}.", id, typeof(T).Name);
            throw new Exception($"An error occurred while retrieving the entity with ID {id}.", ex);
        }
    }

    public async Task AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            await _dbSet.AddAsync(entity).ConfigureAwait(false);
            await SaveAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding an entity of type {EntityType}.", typeof(T).Name);
            throw new Exception("An error occurred while adding the entity.", ex);
        }
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            _dbSet.Update(entity);
            await SaveAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating an entity of type {EntityType}.", typeof(T).Name);
            throw new Exception("An error occurred while updating the entity.", ex);
        }
    }

    public async Task DeleteAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            _dbSet.Remove(entity);
            await SaveAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting an entity of type {EntityType}.", typeof(T).Name);
            throw new Exception("An error occurred while deleting the entity.", ex);
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await Context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes to the database for entity type {EntityType}.", typeof(T).Name);
            throw new Exception("An error occurred while saving changes to the database.", ex);
        }
    }
}