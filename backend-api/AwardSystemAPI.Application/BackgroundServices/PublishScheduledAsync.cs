using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AwardSystemAPI.Application.BackgroundServices
{
    public class AnnouncementPublisherService : BackgroundService
    {
        private readonly ILogger<AnnouncementPublisherService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly IServiceProvider _serviceProvider;

        public AnnouncementPublisherService(
            IServiceProvider serviceProvider,
            ILogger<AnnouncementPublisherService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AnnouncementPublisherService starting up.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var announcementRepo = scope.ServiceProvider
                        .GetRequiredService<IAnnouncementRepository>();
                    var pushNotificationService = scope.ServiceProvider
                        .GetRequiredService<IPushNotificationService>();
                    
                    var now = DateTime.UtcNow;
                    var due = await announcementRepo
                        .GetScheduledToPublishAsync(now);

                    foreach (var ann in due)
                    {
                        // Publish it
                        ann.Status    = "published";
                        ann.UpdatedAt = now;
                        await announcementRepo.UpdateAsync(ann);
                        _logger.LogInformation("Published announcement {Id}.", ann.Id);

                        // If it's flagged for push, send it
                        if (ann.IsPushNotification)
                        {
                            await pushNotificationService.SendPushAsync(
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
