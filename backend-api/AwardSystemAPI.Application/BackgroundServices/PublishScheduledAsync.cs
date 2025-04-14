using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AwardSystemAPI.Infrastructure.Repositories;

namespace AwardSystemAPI.Application.BackgroundServices
{
    public class AnnouncementPublisherService : BackgroundService
    {
        private readonly IAnnouncementRepository _announcementRepo;
        private readonly IPushNotificationService   _pushNotificationService;
        private readonly ILogger<AnnouncementPublisherService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AnnouncementPublisherService(
            IAnnouncementRepository announcementRepo,
            IPushNotificationService pushNotificationService,
            ILogger<AnnouncementPublisherService> logger)
        {
            _announcementRepo     = announcementRepo;
            _pushNotificationService  = pushNotificationService;
            _logger               = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AnnouncementPublisherService starting up.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    var due = await _announcementRepo.GetScheduledToPublishAsync(now);

                    foreach (var ann in due)
                    {
                        // Publish it
                        ann.Status    = "published";
                        ann.UpdatedAt = now;
                        await _announcementRepo.UpdateAsync(ann);
                        _logger.LogInformation("Published announcement {Id}.", ann.Id);

                        // If it's flagged for push, send it
                        if (ann.IsPushNotification)
                        {
                            await _pushNotificationService.SendPushAsync(
                                ann.Title,
                                ann.Description,
                                ann.Audience
                            );
                            _logger.LogInformation("Push sent for announcement {Id}.", ann.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while publishing scheduled announcements");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
            _logger.LogInformation("AnnouncementPublisherService stopping.");
        }
    }
}
