using AwardSystemAPI.Application.Services;
using AwardSystemAPI.Domain.Enums;

namespace AwardSystemAPI.Application.BackgroundServices;

public interface IPushNotificationService
{ 
    Task SendPushAsync(string title, string message, TargetAudience audience);
}

public class PushNotificationService : IPushNotificationService
{
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    public PushNotificationService(IFirebaseNotificationService firebaseNotificationService)
    {
        _firebaseNotificationService = firebaseNotificationService;
    }

    public async Task SendPushAsync(string title, string message, TargetAudience audience)
    {
        await _firebaseNotificationService.SendPushAsync(title, message, audience);
    }
}