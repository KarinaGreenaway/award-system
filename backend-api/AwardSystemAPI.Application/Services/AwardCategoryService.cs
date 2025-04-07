using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IAwardCategoryService
{
    Task<IEnumerable<AwardCategoryResponseDto>> GetAllAwardCategoriesAsync();
    Task<AwardCategoryResponseDto?> GetAwardCategoryByIdAsync(int id);
    Task<AwardCategoryResponseDto> CreateAwardCategoryAsync(AwardCategoryCreateDto dto);
    Task<bool> UpdateAwardCategoryAsync(int id, AwardCategoryUpdateDto dto);
    Task<bool> DeleteAwardCategoryAsync(int id);
    Task<IEnumerable<AwardCategoryResponseDto>> GetAwardCategoriesBySponsorIdAsync(int sponsorId);
}
public class AwardCategoryService : IAwardCategoryService
{
    private readonly IAwardCategoryRepository _repository;
    private readonly ILogger<AwardCategoryService> _logger;

    public AwardCategoryService(IAwardCategoryRepository repository, ILogger<AwardCategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<AwardCategoryResponseDto>> GetAllAwardCategoriesAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(category => new AwardCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            SponsorId = category.SponsorId,
            IntroductionVideo = category.IntroductionVideo,
            IntroductionParagraph = category.IntroductionParagraph,
            ProfileStatus = category.ProfileStatus,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        });
    }

    public async Task<AwardCategoryResponseDto?> GetAwardCategoryByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found.", id);
            return null;
        }

        return new AwardCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            SponsorId = category.SponsorId,
            IntroductionVideo = category.IntroductionVideo,
            IntroductionParagraph = category.IntroductionParagraph,
            ProfileStatus = category.ProfileStatus,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<AwardCategoryResponseDto> CreateAwardCategoryAsync(AwardCategoryCreateDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var category = new AwardCategory
        {
            Name = dto.Name,
            Type = dto.Type,
            SponsorId = dto.SponsorId,
            IntroductionVideo = dto.IntroductionVideo,
            IntroductionParagraph = dto.IntroductionParagraph,
            ProfileStatus = dto.ProfileStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category);
        _logger.LogInformation("Created AwardCategory with ID {Id}.", category.Id);

        return new AwardCategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            SponsorId = category.SponsorId,
            IntroductionVideo = category.IntroductionVideo,
            IntroductionParagraph = category.IntroductionParagraph,
            ProfileStatus = category.ProfileStatus,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<bool> UpdateAwardCategoryAsync(int id, AwardCategoryUpdateDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for update.", id);
            return false;
        }

        category.Name = dto.Name;
        category.Type = dto.Type;
        category.SponsorId = dto.SponsorId;
        category.IntroductionVideo = dto.IntroductionVideo;
        category.IntroductionParagraph = dto.IntroductionParagraph;
        category.ProfileStatus = dto.ProfileStatus;
        category.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(category);
        _logger.LogInformation("Updated AwardCategory with ID {Id}.", id);
        return true;
    }

    public async Task<bool> DeleteAwardCategoryAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("AwardCategory with ID {Id} not found for deletion.", id);
            return false;
        }

        await _repository.DeleteAsync(category);
        _logger.LogInformation("Deleted AwardCategory with ID {Id}.", id);
        return true;
    }

    public async Task<IEnumerable<AwardCategoryResponseDto>> GetAwardCategoriesBySponsorIdAsync(int sponsorId)
    {
        var categories = await _repository.GetBySponsorIdAsync(sponsorId);
        return categories.Select(c => new AwardCategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            SponsorId = c.SponsorId,
            IntroductionVideo = c.IntroductionVideo,
            IntroductionParagraph = c.IntroductionParagraph,
            ProfileStatus = c.ProfileStatus,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }
}