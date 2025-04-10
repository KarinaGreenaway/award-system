using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AwardSystemAPI.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IAwardCategoryService
{
    Task<ApiResponse<IEnumerable<AwardCategoryResponseDto>, string>> GetAllAwardCategoriesAsync();
    Task<ApiResponse<AwardCategoryResponseDto?, string>> GetAwardCategoryByIdAsync(int id);
    Task<ApiResponse<AwardCategoryResponseDto, string>> CreateAwardCategoryAsync(AwardCategoryCreateDto dto);
    Task<ApiResponse<bool, string>> UpdateAwardCategoryAsync(int id, AwardCategoryUpdateDto dto);
    Task<ApiResponse<bool, string>> DeleteAwardCategoryAsync(int id);
    Task<ApiResponse<IEnumerable<AwardCategoryResponseDto>, string>> GetAwardCategoriesBySponsorIdAsync(int sponsorId);
}

public class AwardCategoryService : IAwardCategoryService
{
    private readonly IAwardCategoryRepository _repository;
    private readonly ILogger<AwardCategoryService> _logger;
    private readonly IMapper _mapper;

    public AwardCategoryService(IAwardCategoryRepository repository, ILogger<AwardCategoryService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<AwardCategoryResponseDto>, string>> GetAllAwardCategoriesAsync()
    {
        var categories = await _repository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<AwardCategoryResponseDto>>(categories);
        return result.ToArray();
    }

    public async Task<ApiResponse<AwardCategoryResponseDto?, string>> GetAwardCategoryByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found.", id);
            return $"AwardCategory with ID {id} not found.";
        }
        var result = _mapper.Map<AwardCategoryResponseDto>(category);
        return result;
    }

    public async Task<ApiResponse<AwardCategoryResponseDto, string>> CreateAwardCategoryAsync(AwardCategoryCreateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var category = _mapper.Map<AwardCategory>(dto);
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        await _repository.AddAsync(category);
        _logger.LogInformation("Created AwardCategory with ID {Id}.", category.Id);

        var responseDto = _mapper.Map<AwardCategoryResponseDto>(category);
        return responseDto;
    }

    public async Task<ApiResponse<bool, string>> UpdateAwardCategoryAsync(int id, AwardCategoryUpdateDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for update.", id);
            return $"AwardCategory with ID {id} not found for update.";
        }

        _mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(category);
        _logger.LogInformation("Updated AwardCategory with ID {Id}.", id);
        return true;
    }

    public async Task<ApiResponse<bool, string>> DeleteAwardCategoryAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for deletion.", id);
            return $"AwardCategory with ID {id} not found for deletion.";
        }

        await _repository.DeleteAsync(category);
        _logger.LogInformation("Deleted AwardCategory with ID {Id}.", id);
        return true;
    }

    public async Task<ApiResponse<IEnumerable<AwardCategoryResponseDto>, string>> GetAwardCategoriesBySponsorIdAsync(int sponsorId)
    {
        var categories = await _repository.GetBySponsorIdAsync(sponsorId);
        var result = _mapper.Map<IEnumerable<AwardCategoryResponseDto>>(categories);
        return result.ToArray();
    }
}