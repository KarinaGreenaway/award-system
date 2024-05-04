using AwardSystemAPI.Application.DTOs;
using AwardSystemAPI.Common;
using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services
{
    public interface IAwardEventService
    {
        Task<ApiResponse<IEnumerable<AwardEventResponseDto>, string>> GetAllAwardEventsAsync();
        Task<ApiResponse<AwardEventResponseDto?, string>> GetAwardEventByIdAsync(int id);
        Task<ApiResponse<AwardEventResponseDto, string>> GetAwardEventByAwardProcessIdAsync(int id);
        Task<ApiResponse<AwardEventResponseDto, string>> CreateAwardEventAsync(AwardEventCreateDto dto);
        Task<ApiResponse<bool, string>> UpdateAwardEventAsync(int id, AwardEventUpdateDto dto);
        Task<ApiResponse<bool, string>> DeleteAwardEventAsync(int id);
    }
    
    public class AwardEventService : IAwardEventService
    {
        private readonly IAwardEventRepository _repository;
        private readonly ILogger<AwardEventService> _logger;
        private readonly IMapper _mapper;

        public AwardEventService(IAwardEventRepository repository, ILogger<AwardEventService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<AwardEventResponseDto>, string>> GetAllAwardEventsAsync()
        {
            var events = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<AwardEventResponseDto>>(events);
            return dtos.ToArray();
        }

        public async Task<ApiResponse<AwardEventResponseDto?, string>> GetAwardEventByIdAsync(int id)
        {
            var awardEvent = await _repository.GetByIdAsync(id);
            if (awardEvent == null)
            {
                _logger.LogWarning("AwardEvent with ID {Id} not found.", id);
                return $"AwardEvent with ID {id} not found.";
            }
            var responseDto = _mapper.Map<AwardEventResponseDto>(awardEvent);
            return responseDto;
        }
        
        public async Task<ApiResponse<AwardEventResponseDto, string>> GetAwardEventByAwardProcessIdAsync(int id)
        {
            var events = await _repository.GetByAwardProcessIdAsync(id);
            var dtos = _mapper.Map<AwardEventResponseDto>(events);
            return dtos;
        }

        public async Task<ApiResponse<AwardEventResponseDto, string>> CreateAwardEventAsync(AwardEventCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = _mapper.Map<AwardEvent>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(entity);
            _logger.LogInformation("Created AwardEvent with ID {Id}.", entity.Id);

            var responseDto = _mapper.Map<AwardEventResponseDto>(entity);
            return responseDto;
        }

        public async Task<ApiResponse<bool, string>> UpdateAwardEventAsync(int id, AwardEventUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("AwardEvent with ID {Id} not found for update.", id);
                return $"AwardEvent with ID {id} not found for update.";
            }
            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            
            entity.EventDateTime = DateTime.SpecifyKind(entity.EventDateTime, DateTimeKind.Utc);
            entity.CreatedAt = DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc);
            entity.UpdatedAt = DateTime.SpecifyKind(entity.UpdatedAt, DateTimeKind.Utc);
            
            await _repository.UpdateAsync(entity);
            _logger.LogInformation("Updated AwardEvent with ID {Id}.", id);
            return true;
        }

        public async Task<ApiResponse<bool, string>> DeleteAwardEventAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("AwardEvent with ID {Id} not found for deletion.", id);
                return $"AwardEvent with ID {id} not found for deletion.";
            }
            await _repository.DeleteAsync(entity);
            _logger.LogInformation("Deleted AwardEvent with ID {Id}.", id);
            return true;
        }
    }
}
