using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Enums;
using AwardSystemAPI.Infrastructure.Repositories;

namespace AwardSystemAPI.Application.BackgroundServices;

public interface IPushNotificationService
{ 
    Task SendPushAsync(string title, string message, TargetAudience audience);
}

public class PushNotificationService : IPushNotificationService
{
    private readonly IDeviceTokenRepository _deviceTokenRepository;
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    public PushNotificationService(IDeviceTokenRepository deviceTokenRepository, IFirebaseNotificationService firebaseNotificationService)
    {
        _deviceTokenRepository = deviceTokenRepository;
        _firebaseNotificationService = firebaseNotificationService;
    }

    public async Task SendPushAsync(string title, string message, TargetAudience audience)
    {
        var deviceTokens = await _deviceTokenRepository.GetAllAsync();
        var tokens = deviceTokens.Select(dt => dt.Token).ToList();

        if (tokens.Count > 0)
        {
            await _firebaseNotificationService.SendPushAsync(title, message, audience);
        }
    }
}