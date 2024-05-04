using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IRsvpService
{
    Task<ApiResponse<IEnumerable<RsvpFormQuestionResponseDto>, string>> GetRsvpFormQuestionsByAwardCategoryAsync(int awardEventId);
    Task<ApiResponse<RsvpFormQuestionResponseDto, string>> CreateRsvpFormQuestionAsync(RsvpFormQuestionCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateRsvpFormQuestionAsync(int questionId, RsvpFormQuestionUpdateDto dto);
}

public class RsvpService : IRsvpService
{
    private readonly IRsvpFormQuestionsRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<RsvpService> _logger;

    public RsvpService(
        IRsvpFormQuestionsRepository repository,
        IMapper mapper,
        ILogger<RsvpService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<RsvpFormQuestionResponseDto>, string>> GetRsvpFormQuestionsByAwardCategoryAsync(int awardEventId)
    {
        var questions = await _repository.GetByAwardCategory(awardEventId);
        var dtos = _mapper.Map<IEnumerable<RsvpFormQuestionResponseDto>>(questions);
        return dtos.ToArray();
    }
    
    public async Task<ApiResponse<RsvpFormQuestionResponseDto, string>> CreateRsvpFormQuestionAsync(RsvpFormQuestionCreateDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<RsvpFormQuestion>(dto);

        await _repository.AddAsync(entity);
        _logger.LogInformation("Created RsvpFormQuestion with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<RsvpFormQuestionResponseDto>(entity);
        return responseDto;
    }
    
    public async Task<ApiResponse<bool, string>> UpdateRsvpFormQuestionAsync(int questionId, RsvpFormQuestionUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(questionId);
        if (entity == null)
        {
            _logger.LogWarning("RsvpFormQuestion with ID {Id} not found.", questionId);
            return $"RsvpFormQuestion with ID {questionId} not found.";
        }

        _mapper.Map(dto, entity);

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Updated RsvpFormQuestion with ID {Id}.", entity.Id);

        return true;
    }
}