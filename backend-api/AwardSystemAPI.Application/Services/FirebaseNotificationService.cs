using AwardSystemAPI.Domain.Enums;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface  IFirebaseNotificationService
{
    Task SendPushAsync(string title, string body, TargetAudience audience);
    Task SubscribeToTopicAsync(string token, string topic);
    Task UnsubscribeFromTopicAsync(string token, string topic);
}
public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly ILogger<FirebaseNotificationService> _logger;
    private readonly FirebaseMessaging _messaging;

    public FirebaseNotificationService(
        IConfiguration config,
        ILogger<FirebaseNotificationService> logger)
    {
        _logger = logger;

        // Initialize the FirebaseApp once
        FirebaseApp app;
        if (FirebaseApp.DefaultInstance == null)
        {
            app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential
                    .FromFile(config["Firebase:ServiceAccountPath"]),
                ProjectId = config["Firebase:ProjectId"]
            });
        }
        else
        {
            app = FirebaseApp.DefaultInstance;
        }

        _messaging = FirebaseMessaging.GetMessaging(app);
    }

    public async Task SendPushAsync(string title, string body, 
        TargetAudience audience)
    {
        // Use FCM Topics: have each device subscribe to "mobileUsers" or "sponsors"
        //    then your message only needs to set Message.Topic = "mobileUsers".
        // TODO: Implement logic to subscribe devices to topics
        
        var topic = audience == TargetAudience.MobileUsers
            ? "mobileUsers"
            : "sponsors";

        var message = new Message
        {
            Topic = topic,
            Notification = new Notification
            {
                Title = title,
                Body  = body
            }
        };

        try
        {
            var response = await _messaging.SendAsync(message);
            _logger.LogInformation("FCM message sent: {Id}", response);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Error sending FCM notification");
            throw;
        }
    }
    
    public async Task SubscribeToTopicAsync(string token, string topic)
    {
        try
        {
            var response = await _messaging.SubscribeToTopicAsync(new[] { token }, topic);
            _logger.LogInformation("Subscribed to topic: {Response}", response);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Error subscribing to FCM topic");
            throw;
        }
    }
    
    public async Task UnsubscribeFromTopicAsync(string token, string topic)
    {
        try
        {
            var response = await _messaging.UnsubscribeFromTopicAsync(new[] { token }, topic);
            _logger.LogInformation("Unsubscribed from topic: {Response}", response);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Error unsubscribing from FCM topic");
            throw;
        }
    }
}