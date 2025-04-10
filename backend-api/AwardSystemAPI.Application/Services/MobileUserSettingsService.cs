using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AwardSystemAPI.Application.DTOs.MobileUserSettingsDtos;

namespace AwardSystemAPI.Application.Services
{
    public interface IMobileUserSettingsService
    {
        Task<ApiResponse<MobileUserSettingsResponseDto, string>> GetSettingsAsync(int userId);
        Task<ApiResponse<MobileUserSettingsResponseDto, string>> UpdateSettingsAsync(int userId, MobileUserSettingsUpdateDto dto);
    }

    public class MobileUserSettingsService : IMobileUserSettingsService
    {
        private readonly IGenericRepository<MobileUserSettings> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<MobileUserSettingsService> _logger;

        public MobileUserSettingsService(IGenericRepository<MobileUserSettings> repository, IMapper mapper, ILogger<MobileUserSettingsService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<MobileUserSettingsResponseDto, string>> GetSettingsAsync(int userId)
        {
            var settings = (await _repository.GetAllAsync()).FirstOrDefault(s => s.UserId == userId);
            if (settings == null)
            {
                return $"Settings not found for user with ID {userId}.";
            }
            var dto = _mapper.Map<MobileUserSettingsResponseDto>(settings);
            return dto;
        }

        public async Task<ApiResponse<MobileUserSettingsResponseDto, string>> UpdateSettingsAsync(int userId, MobileUserSettingsUpdateDto dto)
        {
            var settings = (await _repository.GetAllAsync()).FirstOrDefault(s => s.UserId == userId);
            if (settings == null)
            {
                return $"Settings not found for user with ID {userId}.";
            }

            settings.PushNotifications = dto.PushNotifications;
            settings.AiFunctionality = dto.AiFunctionality;

            await _repository.UpdateAsync(settings);
            var responseDto = _mapper.Map<MobileUserSettingsResponseDto>(settings);
            return responseDto;
        }
    }
}
