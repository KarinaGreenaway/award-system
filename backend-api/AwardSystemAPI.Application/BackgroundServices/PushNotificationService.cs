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
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    public PushNotificationService(IDeviceTokenRepository deviceTokenRepository, IFirebaseNotificationService firebaseNotificationService)
    {
        _firebaseNotificationService = firebaseNotificationService;
    }

    public async Task SendPushAsync(string title, string message, TargetAudience audience)
    {
        await _firebaseNotificationService.SendPushAsync(title, message, audience);
    }
}