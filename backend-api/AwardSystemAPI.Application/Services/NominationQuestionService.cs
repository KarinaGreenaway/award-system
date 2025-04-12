using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface INominationQuestionService
{
    Task<ApiResponse<IEnumerable<NominationQuestionResponseDto>, string>> GetQuestionsByCategoryAsync(int categoryId);
    Task<ApiResponse<NominationQuestionResponseDto?, string>> GetQuestionByIdAsync(int id);
    Task<ApiResponse<NominationQuestionResponseDto, string>> CreateQuestionAsync(NominationQuestionCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateQuestionAsync(int id, NominationQuestionUpdateDto dto);
    Task<ApiResponse<bool, string>> DeleteQuestionAsync(int id);
}

public class NominationQuestionService : INominationQuestionService
{
    private readonly INominationQuestionRepository _repo;
    private readonly IAwardCategoryRepository _categoryRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<NominationQuestionService> _logger;

    public NominationQuestionService(
        INominationQuestionRepository repo,
        IAwardCategoryRepository categoryRepo,
        IMapper mapper,
        ILogger<NominationQuestionService> logger)
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<NominationQuestionResponseDto>, string>> GetQuestionsByCategoryAsync(int categoryId)
    {
        var questions = await _repo.GetByCategoryIdAsync(categoryId);
        var dtos = _mapper.Map<IEnumerable<NominationQuestionResponseDto>>(questions);
        return dtos.ToArray();
    }
        
    public async Task<ApiResponse<NominationQuestionResponseDto?, string>> GetQuestionByIdAsync(int id)
    {
        var questions = await _repo.GetByIdAsync(id);
        if (questions == null)
        {
            _logger.LogWarning("NominationQuestion {Id} not found.", id);
            return $"NominationQuestion with ID {id} not found.";
        }
        var responseDto = _mapper.Map<NominationQuestionResponseDto>(questions);
        return responseDto;
    }

    public async Task<ApiResponse<NominationQuestionResponseDto, string>> CreateQuestionAsync(NominationQuestionCreateDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
            
        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
        if (category == null) return $"Category ID {dto.CategoryId} not found.";

        var entity = _mapper.Map<NominationQuestion>(dto);
        await _repo.AddAsync(entity);
        _logger.LogInformation("Created NominationQuestion {Id}.", entity.Id);

        return _mapper.Map<NominationQuestionResponseDto>(entity);
    }

    public async Task<ApiResponse<bool, string>> UpdateQuestionAsync(int id, NominationQuestionUpdateDto dto)
    {
        var question = await _repo.GetByIdAsync(id);
        if (question == null) return $"NominationQuestion with ID {id} not found.";

        _mapper.Map(dto, question);
        await _repo.UpdateAsync(question);
        _logger.LogInformation("Updated NominationQuestion {Id}.", id);
        return true;
    }

    public async Task<ApiResponse<bool, string>> DeleteQuestionAsync(int id)
    {
        var question = await _repo.GetByIdAsync(id);
        if (question == null) return $"NominationQuestion with ID {id} not found.";

        await _repo.DeleteAsync(question);
        _logger.LogInformation("Deleted NominationQuestion {Id}.", id);
        return true;
    }
}