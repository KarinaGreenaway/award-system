using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class TeamMemberProfile: Profile
{
    public TeamMemberProfile()
    {
        CreateMap<TeamMemberCreateDto, TeamMember>().
            ForMember(dest => dest.NominationId, opt => opt.Ignore());
        CreateMap<TeamMemberUpdateDto, TeamMember>();
        CreateMap<TeamMember, TeamMemberResponseDto>().ReverseMap();
    }
}