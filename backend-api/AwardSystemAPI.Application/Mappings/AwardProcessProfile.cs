using AutoMapper;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Application.DTOs.AwardProcessDtos;

namespace AwardSystemAPI.Application.Mappings
{
    public class AwardProcessProfile : Profile
    {
        public AwardProcessProfile()
        {
            CreateMap<AwardProcess, AwardProcessResponseDto>();
            CreateMap<AwardProcessCreateDto, AwardProcess>();
            CreateMap<AwardProcessUpdateDto, AwardProcess>();
        }
    }
}