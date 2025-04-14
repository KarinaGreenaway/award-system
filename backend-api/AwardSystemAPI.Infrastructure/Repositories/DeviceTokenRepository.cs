using AwardSystemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Infrastructure.Repositories;

public interface IDeviceTokenRepository: IGenericRepository<DeviceToken>
{
    Task<DeviceToken?> GetByUserIdAsync(int userId);
    Task AddOrUpdateTokenAsync(int userId, string token);
}

public class DeviceTokenRepository : GenericRepository<DeviceToken>, IDeviceTokenRepository
{
    private readonly ILogger<GenericRepository<DeviceToken>> _logger;

    public DeviceTokenRepository(AppDbContext context, ILogger<GenericRepository<DeviceToken>> logger) : base(context,
        logger)
    {
        _logger = logger;
    }

    public async Task<DeviceToken?> GetByUserIdAsync(int userId)
    {
        try
        {
            return await Context.Set<DeviceToken>()
                .FirstOrDefaultAsync(dt => dt.UserId == userId)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while retrieving device token for user ID {UserId}.", userId);
            throw new Exception($"An error occurred while retrieving device token for user with ID {userId}.", e);
        }
    }
    public async Task AddOrUpdateTokenAsync(int userId, string token)
    {
        try
        {
            var deviceToken = await GetByUserIdAsync(userId).ConfigureAwait(false);
            if (deviceToken == null)
            {
                deviceToken = new DeviceToken { UserId = userId, Token = token };
                await AddAsync(deviceToken).ConfigureAwait(false);
            }
            else
            {
                deviceToken.Token = token;
                await UpdateAsync(deviceToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while adding or updating device token for user ID {UserId}.", userId);
            throw new Exception($"An error occurred while adding or updating device token for user with ID {userId}.", e);
        }
    }
}
    