using System.Text.Json;
using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;

namespace AwardSystemAPI.Application.Mappings;

public class NominationQuestionProfile : Profile
{
    public NominationQuestionProfile()
    {
        CreateMap<NominationQuestionCreateDto, NominationQuestion>()
            // JSON-serialize Options list into the entity's Options column
            .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => SerializeOptions(src.Options)));

        CreateMap<NominationQuestionUpdateDto, NominationQuestion>()
            .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => SerializeOptions(src.Options)));

        CreateMap<NominationQuestion, NominationQuestionResponseDto>()
            // JSON-deserialize Options column back into a List<string>
            .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => DeserializeOptions(src.Options)));
    }
    private static string? SerializeOptions(List<string>? options)
    {
        return options == null ? null : JsonSerializer.Serialize(options);
    }
    private static List<string>? DeserializeOptions(string? options)
    {
        return options == null ? null : JsonSerializer.Deserialize<List<string>>(options);
    }
}
