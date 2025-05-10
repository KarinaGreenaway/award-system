using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IJudgingRoundService
{
    Task<ApiResponse<JudgingRoundResponseDto, string>> CreateJudgingRoundAsync(JudgingRoundCreateDto dto);
    Task<ApiResponse<JudgingRoundResponseDto, string>> GetJudgingRoundByIdAsync(int id);
    Task<ApiResponse<bool, string>> UpdateJudgingRoundAsync(int id, JudgingRoundUpdateDto dto);
    Task<ApiResponse<bool, string>> DeleteJudgingRoundAsync(int id);
    Task<ApiResponse<IEnumerable<JudgingRoundResponseDto>, string>> GetJudgingRoundsByAwardProcessIdAsync(int awardProcessId);
}

public class JudgingRoundService : IJudgingRoundService
{
    private readonly IGenericRepository<JudgingRound> _repository;
    private readonly ILogger<JudgingRoundService> _logger;
    private readonly IMapper _mapper;

    public JudgingRoundService(IGenericRepository<JudgingRound> repository, ILogger<JudgingRoundService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponse<JudgingRoundResponseDto, string>> CreateJudgingRoundAsync(JudgingRoundCreateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<JudgingRound>(dto);
            
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        
        entity.CreatedAt = DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc);
        entity.UpdatedAt = DateTime.SpecifyKind(entity.UpdatedAt, DateTimeKind.Utc);
        entity.StartDate = DateTime.SpecifyKind(entity.StartDate, DateTimeKind.Utc);
        entity.Deadline = DateTime.SpecifyKind(entity.Deadline, DateTimeKind.Utc);

        await _repository.AddAsync(entity);
        _logger.LogInformation("Created JudgingRound with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<JudgingRoundResponseDto>(entity);
        return responseDto;
    }

    public async Task<ApiResponse<JudgingRoundResponseDto, string>> GetJudgingRoundByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("JudgingRound with ID {Id} not found.", id);
            return $"JudgingRound with ID {id} not found.";
        }
        var responseDto = _mapper.Map<JudgingRoundResponseDto>(entity);
        return responseDto;
    }

    public async Task<ApiResponse<bool, string>> UpdateJudgingRoundAsync(int id, JudgingRoundUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("JudgingRound with ID {Id} not found for update.", id);
            return $"JudgingRound with ID {id} not found for update.";
        }

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        
        entity.UpdatedAt = DateTime.SpecifyKind(entity.UpdatedAt, DateTimeKind.Utc);
        entity.StartDate = DateTime.SpecifyKind(entity.StartDate, DateTimeKind.Utc);
        entity.Deadline = DateTime.SpecifyKind(entity.Deadline, DateTimeKind.Utc);

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Updated JudgingRound with ID {Id}.", id);
        return true;
    }

    public async Task<ApiResponse<bool, string>> DeleteJudgingRoundAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("JudgingRound with ID {Id} not found for deletion.", id);
            return $"JudgingRound with ID {id} not found for deletion.";
        }

        await _repository.DeleteAsync(entity);
        _logger.LogInformation("Deleted JudgingRound with ID {Id}.", id);
        return true;
    }

    public async Task<ApiResponse<IEnumerable<JudgingRoundResponseDto>, string>> GetJudgingRoundsByAwardProcessIdAsync(int awardProcessId)
    {
        var rounds = (await _repository.GetAllAsync()).Where(r => r.AwardProcessId == awardProcessId);
        var responseDtos = _mapper.Map<IEnumerable<JudgingRoundResponseDto>>(rounds);
        return responseDtos.ToArray();
    }
}