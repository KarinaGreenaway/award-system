using AutoMapper;
using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Domain.Enums;
using AwardSystemAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services;

public interface IAnnouncementService
{
    Task<ApiResponse<AnnouncementResponseDto, string>> CreateAnnouncementAsync(AnnouncementCreateDto dto, int userId);
    Task<ApiResponse<bool, string>> UpdateAnnouncementAsync(int id, AnnouncementUpdateDto dto, int userId);
    Task<ApiResponse<bool, string>> DeleteAnnouncementAsync(int id, int userId);
    Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetMyCategoryAnnouncementsAsync(int sponsorId);
    Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetAllAnnouncementsAsync();
    Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetPublishedForAudienceAsync(TargetAudience audience);
    Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetAnnouncementsByCreatorIdAsync(int sponsorId);
    Task<ApiResponse<AnnouncementResponseDto?, string>> GetAnnouncementByIdAsync(int id);
}

public class AnnouncementService : IAnnouncementService
{
    private readonly IAnnouncementRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AnnouncementService> _logger;

    public AnnouncementService(
        IAnnouncementRepository repository,
        IMapper mapper,
        ILogger<AnnouncementService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<AnnouncementResponseDto, string>> CreateAnnouncementAsync(AnnouncementCreateDto dto, int userId)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var entity = _mapper.Map<Announcement>(dto);
        entity.CreatedBy = userId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.AddAsync(entity);
        _logger.LogInformation("Created Announcement with ID {Id}.", entity.Id);

        var responseDto = _mapper.Map<AnnouncementResponseDto>(entity);
        return responseDto;
    }

    public async Task<ApiResponse<bool, string>> UpdateAnnouncementAsync(int id, AnnouncementUpdateDto dto, int userId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Announcement with ID {Id} not found for update.", id);
            return $"Announcement with ID {id} not found.";
        }

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Updated Announcement with ID {Id}.", id);

        return true;
    }

    public async Task<ApiResponse<bool, string>> DeleteAnnouncementAsync(int id, int userId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Announcement with ID {Id} not found for deletion.", id);
            return $"Announcement with ID {id} not found.";
        }

        await _repository.DeleteAsync(entity);
        _logger.LogInformation("Deleted Announcement with ID {Id}.", id);

        return true;
    }

    public async Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetMyCategoryAnnouncementsAsync(int sponsorId)
    {
        var announcements = await _repository.GetByCreatedByAsync(sponsorId);
        var dtos = _mapper.Map<IEnumerable<AnnouncementResponseDto>>(announcements);
        return dtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetAllAnnouncementsAsync()
    {
        var announcements = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<AnnouncementResponseDto>>(announcements);
        return dtos.ToArray();
    }

    public async Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetPublishedForAudienceAsync(TargetAudience audience)
    {
        var announcements = await _repository.GetByAudienceAsync(audience, "published");
        var dtos = _mapper.Map<IEnumerable<AnnouncementResponseDto>>(announcements);
        return dtos.ToArray();
    }
        
    public async Task<ApiResponse<IEnumerable<AnnouncementResponseDto>, string>> GetAnnouncementsByCreatorIdAsync(int sponsorId)
    {
        var announcements = await _repository.GetByCreatedByAsync(sponsorId);
        var dtos = _mapper.Map<IEnumerable<AnnouncementResponseDto>>(announcements);
        return dtos.ToArray();
    }
    
    public async Task<ApiResponse<AnnouncementResponseDto?, string>> GetAnnouncementByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("Announcement with ID {Id} not found.", id);
            return $"Announcement with ID {id} not found.";
        }

        var dto = _mapper.Map<AnnouncementResponseDto>(entity);
        return dto;
    }
}