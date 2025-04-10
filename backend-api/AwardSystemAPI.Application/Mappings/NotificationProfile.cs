using AutoMapper;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Application.DTOs.NotificationDtos;

namespace AwardSystemAPI.Application.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationResponseDto>();
        CreateMap<NotificationCreateDto, Notification>();
    }
}