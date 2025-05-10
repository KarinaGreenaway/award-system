using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<Users, UserResponseDto>();
    }
}