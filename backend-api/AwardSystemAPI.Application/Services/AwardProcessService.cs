using AwardSystemAPI.Domain.Entities;
using AwardSystemAPI.Infrastructure.Repositories;
using AwardSystemAPI.Common;
using AutoMapper;
using AwardSystemAPI.Application.DTOs.AwardProcessDtos;
using Microsoft.Extensions.Logging;

namespace AwardSystemAPI.Application.Services
{
    public interface IAwardProcessService
    {
        Task<ApiResponse<IEnumerable<AwardProcessResponseDto>, string>> GetAllAwardProcessesAsync();
        Task<ApiResponse<AwardProcessResponseDto?, string>> GetAwardProcessByIdAsync(int id);
        Task<ApiResponse<AwardProcessResponseDto, string>> CreateAwardProcessAsync(AwardProcessCreateDto dto);
        Task<ApiResponse<bool, string>> UpdateAwardProcessAsync(int id, AwardProcessUpdateDto dto);
        Task<ApiResponse<bool, string>> DeleteAwardProcessAsync(int id);
    }
    
    public class AwardProcessService : IAwardProcessService
    {
        private readonly IGenericRepository<AwardProcess> _repository;
        private readonly ILogger<AwardProcessService> _logger;
        private readonly IMapper _mapper;

        public AwardProcessService(
            IGenericRepository<AwardProcess> repository, 
            ILogger<AwardProcessService> logger,
            IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<AwardProcessResponseDto>, string>> GetAllAwardProcessesAsync()
        {
            var processes = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<AwardProcessResponseDto>>(processes);
            return result.ToArray();
        }

        public async Task<ApiResponse<AwardProcessResponseDto?, string>> GetAwardProcessByIdAsync(int id)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                return $"AwardProcess with ID {id} not found.";
            }
            
            var result = _mapper.Map<AwardProcessResponseDto>(process);
            return result;
        }

        public async Task<ApiResponse<AwardProcessResponseDto, string>> CreateAwardProcessAsync(AwardProcessCreateDto dto)
        {
            var process = _mapper.Map<AwardProcess>(dto);
            process.CreatedAt = DateTime.UtcNow;
            process.UpdatedAt = DateTime.UtcNow;
            
            await _repository.AddAsync(process);
            _logger.LogInformation("Created AwardProcess with ID {Id}.", process.Id);

            var responseDto = _mapper.Map<AwardProcessResponseDto>(process);
            return responseDto;
        }

        public async Task<ApiResponse<bool, string>> UpdateAwardProcessAsync(int id, AwardProcessUpdateDto dto)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                return $"AwardProcess with ID {id} not found for update.";
            }
            
            _mapper.Map(dto, process);
            
            process.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(process);
            _logger.LogInformation("Updated AwardProcess with ID {Id}.", id);
            return true;
        }

        public async Task<ApiResponse<bool, string>> DeleteAwardProcessAsync(int id)
        {
            var process = await _repository.GetByIdAsync(id);
            if (process == null)
            {
                return $"AwardProcess with ID {id} not found for deletion.";
            }
            
            await _repository.DeleteAsync(process);
            _logger.LogInformation("Deleted AwardProcess with ID {Id}.", id);
            return true;
        }
    }
}
