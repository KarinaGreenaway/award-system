using AwardSystemAPI.Application.DTOs.NotificationDtos;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AwardSystemAPI.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface INotificationService
{
    Task<ApiResponse<IEnumerable<NotificationResponseDto>, string>> GetNotificationsAsync(int userId);
    Task<ApiResponse<NotificationResponseDto?, string>> GetNotificationByIdAsync(int id);
    Task<ApiResponse<NotificationResponseDto, string>> CreateNotificationAsync(NotificationCreateDto dto);
    Task<ApiResponse<bool, string>> MarkAsReadAsync(int id);
}
    
public class NotificationService : INotificationService
{
    private readonly IGenericRepository<Notification> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IGenericRepository<Notification> repository,
        IMapper mapper,
        ILogger<NotificationService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<NotificationResponseDto>, string>> GetNotificationsAsync(int userId)
    {
        var notifications = await _repository.GetAllAsync();
        var filtered = notifications.Where(n => n.UserId == userId);
        var dtos = _mapper.Map<IEnumerable<NotificationResponseDto>>(filtered);
        return dtos.ToArray();
    }

    public async Task<ApiResponse<NotificationResponseDto?, string>> GetNotificationByIdAsync(int id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null)
        {
            return $"Notification with ID {id} not found.";
        }
        var dto = _mapper.Map<NotificationResponseDto>(notification);
        return dto;
    }

    public async Task<ApiResponse<NotificationResponseDto, string>> CreateNotificationAsync(NotificationCreateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var notification = _mapper.Map<Notification>(dto);
        notification.CreatedAt = DateTime.UtcNow;

        await _repository.AddAsync(notification);
        _logger.LogInformation("Created Notification with ID {Id}.", notification.Id);

        var responseDto = _mapper.Map<NotificationResponseDto>(notification);
        return responseDto;
    }

    public async Task<ApiResponse<bool, string>> MarkAsReadAsync(int id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null)
        {
            return $"Notification with ID {id} not found.";
        }

        if (!notification.Read)
        {
            notification.Read = true;
            await _repository.UpdateAsync(notification);
            _logger.LogInformation("Marked Notification with ID {Id} as read.", id);
        }
        return true;
    }
}